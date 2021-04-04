using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualStudio.Services.Graph.Client;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.Events.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Groups.Requests;
using Project.Modules.Organizes.Entities;
using Project.Modules.PermissionGroups.Entities;
using Project.Modules.PermissionGroups.Requests;
using Project.Modules.PermissionGroups.Services;
using Project.Modules.Permissions.Entities;
using Project.Modules.UserPermissionEvents.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Project.Modules.Groups.Services
{
    public interface IGroupService
    {
        (Group group, string message) Store(StoreGroupRequest request, string userId);

        (Group group, string message) Delete(int groupId);

        (Group group, string message) Update(int groupId, UpdateGroupRequest request);

        (ResponseTable response, string message) ShowByEvent(string eventId, RequestTable request);
        (Group group, string message) Copy(int groupId);
        (List<Group> groups, string message) UpdateOrder(OrderRequest request);

    }
    public class GroupService : IGroupService
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly IPermissionGroupServices permissionGroupService;
        public GroupService(MariaDBContext mariaDBContext, IPermissionGroupServices permissionGroupService)
        {
            this.mariaDBContext = mariaDBContext;
            this.permissionGroupService = permissionGroupService;
        }

        public (Group group, string message) Copy(int groupId)
        {
            Group group = mariaDBContext.Groups.Find(groupId);

            #region kiểm tra group Id
            if (group is null)
            {
                return (null, "GroupNotExist");
            }
            #endregion

            #region Lấy thông tin Group tạo Group mới
            int order = mariaDBContext.Groups
                                      .Where(m => m.EventId.Equals(group.EventId))
                                      .Max(m => m.GroupOrder) + 1;
            Group groupNew = new Group
            {
                GroupName = group.GroupName,
                EventId = group.EventId,
                UserId = group.UserId,
                GroupOrder = order,
            };
            mariaDBContext.Groups.Add(groupNew);
            mariaDBContext.SaveChanges();
            #endregion

            #region Lấy tất cả quyền của Group trước Thêm vào Group mới
            List<int> permissionIds = mariaDBContext.PermissionGroups
                                                    .Where(m => m.GroupId == group.GroupID)
                                                    .Select(m => m.PermissionId)
                                                    .ToList();
            AddRequest data = new AddRequest
            {
                GroupId = groupNew.GroupID,
                PermissionIds = permissionIds,
            };
            var (permissionGroups, message) = permissionGroupService.Add(data);
            #endregion

            if (permissionGroups.Count == 0 && !message.Equals("success"))
            {
                return (null, message);
            }

            return (groupNew, "CopyGroupSuccess");
        }

        public (Group group, string message) Delete(int groupId)
        {
            Group group = mariaDBContext.Groups.Find(groupId);
            if (group is null)
                return (null, "GroupNotExist");
            //Xử lí khóa ngoại table user
            List<EventUser> eventUsers = mariaDBContext.EventUsers.Where(m => m.GroupId == groupId).ToList();
            if(eventUsers.Count()>0)
            {
                return (null, "GroupAreadlyUserNotDelete");
            }

            List<PermissionGroup> permissionGroups = mariaDBContext.PermissionGroups.Where(m => m.GroupId == groupId).ToList();
            mariaDBContext.RemoveRange(permissionGroups);
            mariaDBContext.Remove(group);
            mariaDBContext.SaveChanges();
            return (group, "DeleteSuccess");
        }

        public (ResponseTable response, string message) ShowByEvent(string eventId, RequestTable request)
        {

            #region Check EventId
            string eventIdDB = mariaDBContext.Events
                                             .Where(m => m.EventId.Equals(eventId) && m.EventStatus == EVENT_STATUS.ACTIVED)
                                             .Select(m => m.EventId)
                                             .FirstOrDefault();
            if (eventIdDB is null)
                return (null, "EventNotExist");
            #endregion

            List<Group> groups = mariaDBContext.Groups
                                               .Where(m => m.EventId.Equals(eventId) /*&&(!String.IsNullOrEmpty(request.Search)&& m.GroupName.Contains(request.Search))*/)
                                               .OrderByDescending(m => m.GroupCreatedAt)
                                               .ToList();

            #region Pagination
            ResponseTable response = new ResponseTable
            {
                DateResult = groups
                                .Skip((request.Page - 1) * request.Results)
                                .Take(request.Results)
                                .ToList(),
                Info = new Info
                {
                    Page = request.Page,
                    Results = groups
                                .Skip((request.Page - 1) * request.Results)
                                .Take(request.Results)
                                .Count(),
                    TotalRecord = groups.Count(),
                }
            };
            #endregion

            return (response, "ShowListGroupSuccess");
        }

        public (Group group, string message) Store(StoreGroupRequest request, string userId)
        {
            int order = 1;
            Group group = new Group();
            #region Lấy vị trí lớn nhất
            var groups = mariaDBContext.Groups
                                        .Where(m => m.EventId.Equals(request.EventId))
                                        .ToList();
            // int idGroup = mariaDBContext.Groups.Max(m => m.GroupID) + 1;
            if (groups.Count > 0)
            {
                order += groups.Max(m => m.GroupOrder);
            }
            #endregion

            using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
            {
                try
                {
                    #region Tạo Group
                    group = new Group
                    {
                        GroupName = request.GroupName,
                        UserId = userId,
                        EventId = request.EventId,
                        GroupOrder = order
                    };

                    mariaDBContext.Groups.Add(group);
                    mariaDBContext.SaveChanges();
                    #endregion

                    #region Lấy danh sách quyền thêm vào permissionGroup

                    List<int> permissionIds = new List<int>();

                    if (!(request.pemissionGroups is null))
                    {
                        foreach (var module in request.pemissionGroups)
                        {
                            foreach (var fn in module.Functions)
                            {
                                permissionIds.AddRange(fn.Permissions.Where(x => x.PermissionFlag == true).Select(m => m.PermissionId).ToList());
                            }
                        }

                        List<PermissionGroup> perGroups = new List<PermissionGroup>();
                        Permissions.Entities.PermissionOrganize permission;
                        PermissionGroup permissionGroup;
                        foreach (int pemissionId in permissionIds)
                        {
                            permission = mariaDBContext.Permissions
                                                           .Where(m => m.PermissionId == pemissionId)
                                                           .FirstOrDefault();
                            permissionGroup = new PermissionGroup()
                            {
                                GroupId = group.GroupID,
                                PermissionCode = permission.PermissionCode,
                                PermissionId = permission.PermissionId
                            };
                            perGroups.Add(permissionGroup);
                        }

                        mariaDBContext.PermissionGroups.AddRange(perGroups);
                        mariaDBContext.SaveChanges();
                    }


                    #endregion

                    transaction.Commit();
                    return (group, "AddGroupSuccess");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return (null, "AddGroupFaild");
                }


            }




        }

        public (Group group, string message) Update(int groupId, UpdateGroupRequest request)
        {
            using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
            {
                try
                {
                    Group group = mariaDBContext.Groups.Find(groupId);
                    if (group is null)
                        return (null, "GroupNotExist");



                    if (!(request.pemissionGroups is null))
                    {
                        List<int> pmInsert = new List<int>();
                        foreach (var module in request.pemissionGroups)
                        {
                            foreach (var fn in module.Functions)
                            {
                                pmInsert.AddRange(fn.Permissions.Where(x => x.PermissionFlag == true)
                                                                      .Select(m => m.PermissionId)
                                                                      .ToList());
                            }
                        }
                        List<int> pmRemove = mariaDBContext.PermissionGroups.Where(m => m.GroupId == groupId).Select(m=>m.PermissionId).ToList();
                        List<int> pmIntersect = pmInsert.Intersect(pmRemove).ToList();
                        pmInsert = pmInsert.Except(pmIntersect).ToList();
                        pmRemove = pmRemove.Except(pmIntersect).ToList();

                        #region xóa quyền trong Group trong PermissionGroup, UserPermissionEvent

                        List<PermissionGroup> permissionOld = mariaDBContext.PermissionGroups.Where(m => m.GroupId == groupId && pmRemove.Contains(m.PermissionId)).ToList();
                        mariaDBContext.PermissionGroups.RemoveRange(permissionOld);
                        Xoa_Tat_Ca_Quyen_User_trong_Group(groupId, pmRemove);

                        #endregion

                        #region Thêm mới quyền vào UserPermissionEvent sau khi xóa quyền của Group
                        
                        List<PermissionGroup> perGroups = new List<PermissionGroup>();
                        Permissions.Entities.PermissionOrganize permission;
                        PermissionGroup permissionGroup;
                        foreach (int pemissionId in pmInsert)
                        {
                            permission = mariaDBContext.Permissions
                                                           .Where(m => m.PermissionId == pemissionId)
                                                           .FirstOrDefault();
                            if (permission is null) continue;
                            permissionGroup = new PermissionGroup()
                            {
                                GroupId = group.GroupID,
                                PermissionCode = permission.PermissionCode,
                                PermissionId = permission.PermissionId
                            };
                            perGroups.Add(permissionGroup);
                        }
                        mariaDBContext.PermissionGroups.AddRange(perGroups);
                        Them_Quyen_trong_Group_vao_user(groupId, perGroups);
                        #endregion
                    }


                    _ = !(String.IsNullOrEmpty(request.GroupName)) ? group.GroupName = request.GroupName : group.GroupName;
                    mariaDBContext.Groups.Update(group);
                    mariaDBContext.SaveChanges();

                    transaction.Commit();
                    return (group, "UpdateGroupSuccess");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return (null, "UpdateGroupFaild");
                }
            }
        }

        public (List<Group> groups, string message) UpdateOrder(OrderRequest request)
        {
            List<Group> updateGroups = new List<Group>();
            Group group;
            foreach (Group item in request.Groups)
            {
                group = mariaDBContext.Groups.Find(item.GroupID);
                if (group is null)
                {
                    return (null, "GroupNotExist");
                }
                group.GroupOrder = item.GroupOrder;
                updateGroups.Add(group);
            }
            mariaDBContext.Groups.UpdateRange(updateGroups);
            mariaDBContext.SaveChanges();
            return (updateGroups, "UpdateGroupSuccess");
        }


        public void Xoa_Tat_Ca_Quyen_User_trong_Group(int groupId, List<int> permissionOlds)
        {
            List<string> userIds = mariaDBContext.EventUsers.Where(m => m.GroupId == groupId).Select(m => m.UserId).ToList();
            string eventId = mariaDBContext.EventUsers.Where(m => m.GroupId == groupId).Select(m => m.EventId).FirstOrDefault();
            List<UserPermissionEvent> permissionEvent = new List<UserPermissionEvent>();
            List<UserPermissionEvent> permissionUserEvent = mariaDBContext.UserPermissionEvents.Where(m=>m.EventId.Equals(eventId)).ToList();
            foreach (var userId in userIds)
            {
                    permissionEvent = permissionUserEvent.Where(m=> permissionOlds.Contains(m.PermissionId)).ToList();
                    mariaDBContext.UserPermissionEvents.RemoveRange(permissionEvent);
            }
        }

        public void Them_Quyen_trong_Group_vao_user(int groupId, List<PermissionGroup> permissionNews)
        {


            #region Lấy ra danh sách user trong user trong event theo group

            string eventId = mariaDBContext.EventUsers
                                                .Where(m => m.GroupId == groupId)
                                                .Select(m => m.EventId)
                                                .FirstOrDefault();

            List<string> userIds = mariaDBContext.EventUsers
                                                        .Where(m => m.GroupId == groupId && m.EventId.Equals(eventId))
                                                        .Select(m => m.UserId)
                                                        .ToList();


            #endregion

            #region Tạo mới List Quyền của User
            List<UserPermissionEvent> userPermissionEvents = new List<UserPermissionEvent>();
            UserPermissionEvent permissionEvent;
            foreach (var userId in userIds)
            {
                foreach (var permission in permissionNews)
                {
                    permissionEvent = new UserPermissionEvent
                    {
                        UserId = userId,
                        PermissionId = permission.PermissionId,
                        PermissionCode = permission.PermissionCode,
                        EventId = eventId,
                    };
                    userPermissionEvents.Add(permissionEvent);
                }
            }
            #endregion

            mariaDBContext.UserPermissionEvents.AddRange(userPermissionEvents);
        }
    }
}
