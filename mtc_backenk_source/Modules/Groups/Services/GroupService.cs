using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Groups.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Sevices
{
    public interface IGroupService
    {
        List<Group> ShowAll();
        PaginationResponse<Group> ShowTable(PaginationRequest request, string groupId, bool supperAdmin = false);
        PaginationResponse<User> ShowUserInGroup(PaginationRequest request, string grouoId);
        (Group group, string message) Detail(string groupId);
        (object data, string message) Update(UpdateGroupRequest request, string groupId);
        (object data, string message) UpdateStatus(string groupId);
        (object data, string message) ExtendTime(ExtendGroupRequest request, string groupId, string userId);
        (object data, string message) Delete(string groupId);
        (object data, string message) DeleteRange(ListGroupRequest request);
        (object data, string message) Create(StoreGroupRequest request);
        (object data, string message) UpdateStatus(List<string> groupIds, Group.STATUS status);

    }
    public class GroupService : IGroupService
    {
        private readonly IRepositoryWrapperMariaDB _repositoryMaria;
        private readonly IRepositoryWrapperMongoDB _repositoryMongo;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;
        public GroupService(IRepositoryWrapperMariaDB repositoryMaria, IMapper mapper, IRepositoryWrapperMongoDB repositoryMongo, IConfiguration configuration)
        {
            this._repositoryMaria = repositoryMaria;
            this._repositoryMongo = repositoryMongo;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        public (object data, string message) Create(StoreGroupRequest request)
        {
            if(string.IsNullOrEmpty(request.GroupCode))
            {
                return (null, "GroupCodeRequired");
            }

            bool isCode = _repositoryMaria.Groups.FindByCondition(x => x.GroupCode.Equals(request.GroupCode)).Any();
            if (isCode)
            {
                return (null, "GroupCodeAlreadyExist");
            }
            Group group = mapper.Map<Group>(request);
            if (request.ImageGroup is not null)
            {
                string path = request.ImageGroup.UploadFile("groups").Result;
                group.GroupImage = configuration["Backend-Url"] + "/groups/" + path;
            }
            _repositoryMaria.Groups.Add(group);
            _repositoryMaria.SaveChanges();
            Role role = new Role()
            {
                GroupId = group.GroupId,
                RoleCode = $"ADMIN_{group.GroupCode}",
                RoleName = "Admin",
                RoleLevel = UserLevelEnum.USER
            };

            _repositoryMaria.Roles.Add(role);
            _repositoryMaria.SaveChanges();

            List<RolePermission> rolePermissions = new List<RolePermission>();
            IQueryable<string> permissionCodes = _repositoryMaria.Permissions.FindByCondition(x => x.Level == UserLevelEnum.USER).Select(x => x.PermissionCode);
            foreach (string permissionCode in permissionCodes)
            {
                rolePermissions.Add(new RolePermission { RoleId = role.RoleId, PermissionCode = permissionCode });
            }
            _repositoryMaria.RolePermissions.AddRange(rolePermissions);
            _repositoryMaria.SaveChanges();
            return (group, "AddSuccess");
        }

        public (object data, string message) Delete(string groupId)
        {
            Group group = _repositoryMaria.Groups.GetById(groupId);
            if (group is null)
            {
                return (null, "GroupNotExist");
            }
            if (!string.IsNullOrEmpty(group.GroupImage))
            {
                string folder = configuration["Backend-Url"];
                string fileOld = group.GroupImage.Replace(folder+"/", "");
                GeneralHelper.DeleteFile(fileOld);
            }
            _repositoryMaria.Groups.Remove(group);
            _repositoryMaria.SaveChanges();
            return (group, "Success");
        }

        public (object data, string message) DeleteRange(ListGroupRequest request)
        {
            request.GroupIds = request.GroupIds.Distinct().ToList();
            List<Group> groups = _repositoryMaria.Groups.FindByCondition(x => request.GroupIds.Contains(x.GroupId)).ToList();
            if (groups.Count == request.GroupIds.Count)
            {
                _repositoryMaria.Groups.RemoveRange(groups);
                _repositoryMaria.SaveChanges();
                return ("DeleteListGroupSuccess", "Success");
            }
            return (null, "ListGroupIdInValid");
        }

        public (Group group, string message) Detail(string groupId)
        {
            Group group = _repositoryMaria.Groups.GetById(groupId);
            if (group is null)
            {
                return (null, "GroupNotExist");
            }
            return (group, "Success");
        }

        public (object data, string message) ExtendTime(ExtendGroupRequest request, string groupId, string userId)
        {

            Group group = _repositoryMaria.Groups.GetById(groupId);
            if (group is null)
            {
                return (null, "GroupNotExist");
            }
            if (request.Expired <= DateTime.Now)
            {
                return (null, "ExpiredAtInValid");
            }

            User user = _repositoryMaria.Users.GetById(userId);
            if (user is null)
            {
                return (null, "UserNotExist");
            }
            GroupHistory groupHistory = new GroupHistory()
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName,
                UserId = user.UserId,
                UserName = user.UserName,
                ExpiredNew = request.Expired,
                ExpiredOld = group.Expired,
                ChangedAt = DateTime.Now
            };
            group.Expired = request.Expired;
            _repositoryMaria.Groups.Update(group);
            _repositoryMaria.SaveChanges();
            _repositoryMongo.GroupHistories.Add(groupHistory);
            return (group, "UpdateSuccess");
        }

        public List<Group> ShowAll()
        {
            List<User> users = _repositoryMaria.Users.FindByCondition(x => !string.IsNullOrEmpty(x.GroupId) && x.UserLevel == UserLevelEnum.USER).ToList();
            List<Group> groups = _repositoryMaria.Groups.FindAll().ToList();
            return groups;
        }

        public PaginationResponse<Group> ShowTable(PaginationRequest request, string groupId, bool supperAdmin = false)
        {
            List<User> users = _repositoryMaria.Users.FindByCondition(x => !string.IsNullOrEmpty(x.GroupId)).ToList();
            List<Device> devices = _repositoryMaria.Devices.FindByCondition(x => !string.IsNullOrEmpty(x.UserId)).ToList();
            IQueryable<Group> groups = _repositoryMaria.Groups.FindByCondition(x => (string.IsNullOrEmpty(groupId) && supperAdmin) || x.GroupId.Equals(groupId));

            groups = groups.Where(x => string.IsNullOrEmpty(request.SearchContent) ||
                                      (!string.IsNullOrEmpty(x.GroupCode) && x.GroupCode.ToLower().Contains(request.SearchContent.ToLower())) ||
                                      (!string.IsNullOrEmpty(x.GroupName) && x.GroupName.ToLower().Contains(request.SearchContent.ToLower())));

            foreach (Group group in groups)
            {
                List<string> userIds = users.Where(x => x.GroupId.Equals(group.GroupId)).Select(x => x.UserId).ToList();
                group.TotalDevice = devices.Count(x => userIds.Contains(x.UserId));
                group.TotalUser = userIds.Count;
                if (group.Expired.HasValue && group.Expired <= DateTime.Now)
                {
                    group.GroupStatus = Group.STATUS.Expired;
                }
            }
            SortHelper<Group>.ApplySort(groups, request.OrderByQuery);
            PaginationHelper<Group> result = PaginationHelper<Group>.ToPagedList(groups, request.PageNumber, request.PageSize);
            PaginationResponse<Group> paginationResponse = new PaginationResponse<Group>(result, result.PageInfo);
            return (paginationResponse);
        }

        public PaginationResponse<User> ShowUserInGroup(PaginationRequest request, string groupId)
        {
            (Group group, string message) = Detail(groupId);
            if (group is null)
            {
                return (null);
            }
            IQueryable<User> users = _repositoryMaria.Users.FindByCondition(x => x.GroupId != null && x.GroupId.Equals(groupId)).Include(x => x.Role);
            users = users.Where(x => string.IsNullOrEmpty(request.SearchContent) || (
                                    x.UserEmail.ToLower().Contains(request.SearchContent.ToLower())
                                    || (!string.IsNullOrEmpty(x.UserName) && x.UserName.ToLower().Contains(request.SearchContent.ToLower()))
                                    ));
            foreach (User user in users.Where(x => x.ExpiredAt.HasValue && x.ExpiredAt <= DateTime.Now))
            {
                user.UserStatus = UserStatusEnum.EXPIRED;
            }
            SortHelper<User>.ApplySort(users, request.OrderByQuery);
            PaginationHelper<User> result = PaginationHelper<User>.ToPagedList(users, request.PageNumber, request.PageSize);
            PaginationResponse<User> paginationResponse = new PaginationResponse<User>(result, result.PageInfo);
            return (paginationResponse);
        }

        public (object data, string message) Update(UpdateGroupRequest request, string groupId)
        {
            Group group = _repositoryMaria.Groups.GetById(groupId);
            if (group is null)
            {
                return (group, "GroupNotExist");
            }

            bool isCode = _repositoryMaria.Groups.FindByCondition(x => x.GroupCode.Equals(request.GroupCode) && !x.GroupId.Equals(groupId)).Any();
            if (isCode)
            {
                return (null, "GroupCodeAlreadyExist");
            }


            group = mapper.Map(request, group);
            if (!string.IsNullOrEmpty(group.GroupImage) && request.ImageGroup is not null)
            {
                string folder = configuration["Backend-Url"];
                string fileOld = group.GroupImage.Replace(folder + "/", "");
                GeneralHelper.DeleteFile(fileOld);
                string path = request.ImageGroup.UploadFile("groups").Result;
                group.GroupImage = configuration["Backend-Url"] + "/groups/" + path;
            }
            _repositoryMaria.Groups.Update(group);
            _repositoryMaria.SaveChanges();
            return (group, "UpdateSuccess");

        }

        public (object data, string message) UpdateStatus(string groupId)
        {

            Group group = _repositoryMaria.Groups.GetById(groupId);
            if (group is null)
            {
                return (group, "GroupNotExist");
            }
            group.GroupStatus = group.GroupStatus == Group.STATUS.Active ? Group.STATUS.UnActive : Group.STATUS.Active;
            _repositoryMaria.Groups.Update(group);
            _repositoryMaria.SaveChanges();
            return (group, "Success");
        }

        public (object data, string message) UpdateStatus(List<string> groupIds, Group.STATUS status)
        {
            groupIds = groupIds.Distinct().ToList();
            List<Group> groups = _repositoryMaria.Groups.FindByCondition(x => groupIds.Contains(x.GroupId)).ToList();
            if (groupIds.Count != groups.Count && groups.Count > 0)
            {
                return (null, "ListGroupInvalid");
            }
            foreach (Group group in groups)
            {
                group.GroupStatus = status;
            }
            _repositoryMaria.Groups.UpdateRange(groups);
            _repositoryMaria.SaveChanges();
            return ("Success", "UpdateSuccess");
        }
    }
}
