using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Project.App.Database;
using Project.Modules.GroupOrganizes.Entities;
using Project.Modules.GroupOrganizes.Requests;
using Project.Modules.Organizes.Entities;
using Project.Modules.PermissionOrganizes.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.GroupOrganizes.Services
{
    public interface IGroupOrganizeService
    {
        public object ShowAllPermissonByGroupOrganize(int idGroup);
        public (GroupOrganize groupOrganize, string message) AddGroupOrganize(AddGroupOrganizeRequest request, string userId);
        public (GroupOrganize groupOrganize, string message) UpdateGroupOrganize(int groupOrgarizeId, UpdateGroupOrganizeRequest request);
        public (GroupOrganize groupOrganize, string message) DeleteGroupOrganize(int goupOrgainzeId);
    }
    public class GroupOrganizeService : IGroupOrganizeService
    {
        private MariaDBContext mariaDBContext;
        public GroupOrganizeService(MariaDBContext mariaDBContext)
        {
            this.mariaDBContext = mariaDBContext;
        }

        public (GroupOrganize groupOrganize, string message) AddGroupOrganize(AddGroupOrganizeRequest request, string userId)
        {
            int order = 1;
            GroupOrganize groupOrganize = new GroupOrganize();
            Organize organize = mariaDBContext.Organizes.Find(request.OrganizeId);
            if (organize is null)
            {
                return (null, "OrganizeNotExist");
            }
            #region Lấy vị trí lớn nhất
            var groupOrganizes = mariaDBContext.GroupOrganizes
                                        .Where(m => m.OrganizeId.Equals(request.OrganizeId))
                                        .ToList();
            // int idGroup = mariaDBContext.GroupOrganizes.Max(m => m.GroupID) + 1;
            if (groupOrganizes.Count > 0)
            {
                order += groupOrganizes.Max(m => m.GroupOrganizeOrder);
            }
            #endregion

            using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
            {
                try
                {
                    #region Tạo GroupOrganize
                    groupOrganize = new GroupOrganize
                    {
                        GroupOrganizeName = request.GroupOrganizeName,
                        UserId = userId,
                        OrganizeId = request.OrganizeId,
                        GroupOrganizeOrder = order
                    };

                    mariaDBContext.GroupOrganizes.Add(groupOrganize);
                    mariaDBContext.SaveChanges();
                    #endregion

                    #region Lấy danh sách quyền thêm vào permissionGroupOrganize

                    List<int> permissionIds = new List<int>();

                    if (!(request.PemissionGroupOrganizes is null))
                    {
                        foreach (var module in request.PemissionGroupOrganizes)
                        {
                            foreach (var fn in module.FunctionOrganizes)
                            {
                                permissionIds.AddRange(fn.PermissionOrganizes.Where(x => x.PermissionFlag == true).Select(m => m.PermissionOrganizeId).ToList());
                            }
                        }

                        List<PermissionGroupOrganize> perGroups = new List<PermissionGroupOrganize>();
                        PermissionOrganize permissionOrganize;
                        PermissionGroupOrganize permissionGroup;
                        foreach (int pemissionId in permissionIds)
                        {
                            permissionOrganize = mariaDBContext.PermissionOrganizes
                                                           .Where(m => m.PermissionOrganizeId == pemissionId)
                                                           .FirstOrDefault();
                            permissionGroup = new PermissionGroupOrganize()
                            {
                                GroupOrganizeId = groupOrganize.GroupOrganizeId,
                                PermissionOrganizeCode = permissionOrganize.PermissionOrganizeCode,
                                PermissionOrganizeId = permissionOrganize.PermissionOrganizeId
                            };
                            perGroups.Add(permissionGroup);
                        }

                        mariaDBContext.PermissionGroupOrganizes.AddRange(perGroups);
                        mariaDBContext.SaveChanges();
                    }


                    #endregion

                    transaction.Commit();
                    return (groupOrganize, "AddGroupOrganizeSuccess");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return (null, "AddFaild");

                }
            }
        }

        public (GroupOrganize groupOrganize, string message) UpdateGroupOrganize (int groupOrgarizeId, UpdateGroupOrganizeRequest request)
        {
            using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
            {
                try
                {
                    GroupOrganize groupOrganize = mariaDBContext.GroupOrganizes.Find(groupOrgarizeId);
                    if (groupOrganize is null)
                        return (null, "GroupOrganizeNotExist");


                    if (!(request.PemissionGroupOrganizes is null))
                    {
                        #region Xóa Quyền cũ trong permissionGroup

                        List<PermissionGroupOrganize> permissionOld = mariaDBContext.PermissionGroupOrganizes.Where(m => m.GroupOrganizeId == groupOrgarizeId).ToList();
                        mariaDBContext.PermissionGroupOrganizes.RemoveRange(permissionOld);
                        mariaDBContext.SaveChanges();

                        #region xóa quyền trong GroupOrganize trong UserPermissionEvent

                        List<int> permissionOldIds = permissionOld.Select(m => m.PermissionOrganizeId).ToList();
                        Xoa_Tat_Ca_Quyen_User_trong_Group_To_chuc(groupOrgarizeId, permissionOldIds);
                        #endregion

                        #endregion

                        #region Lấy danh sách quyền thêm vào permissionGroup

                        if (!(request.PemissionGroupOrganizes is null))
                        {
                            List<int> permissionIds = new List<int>();

                            if (!(request.PemissionGroupOrganizes is null))
                            {
                                foreach (var module in request.PemissionGroupOrganizes)
                                {
                                    foreach (var fn in module.FunctionOrganizes)
                                    {
                                        permissionIds.AddRange(fn.PermissionOrganizes.Where(x => x.PermissionFlag == true)
                                                                              .Select(m => m.PermissionOrganizeId)
                                                                              .ToList());
                                    }
                                }

                                List<PermissionGroupOrganize> perGroupOrganizes = new List<PermissionGroupOrganize>();
                                PermissionOrganize permission;
                                PermissionGroupOrganize permissionGroup;
                                foreach (int pemissionId in permissionIds)
                                {
                                    permission = mariaDBContext.PermissionOrganizes
                                                                   .Where(m => m.PermissionOrganizeId == pemissionId)
                                                                   .FirstOrDefault();
                                    if (permission is null) continue;
                                    permissionGroup = new PermissionGroupOrganize()
                                    {
                                        GroupOrganizeId = groupOrganize.GroupOrganizeId,
                                        PermissionOrganizeCode = permission.PermissionOrganizeCode,
                                        PermissionOrganizeId = permission.PermissionOrganizeId
                                    };
                                    perGroupOrganizes.Add(permissionGroup);
                                }
                                mariaDBContext.PermissionGroupOrganizes.AddRange(perGroupOrganizes);
                                mariaDBContext.SaveChanges();

                                #region Thêm mới quyền vào UserPermissionEvent sau khi xóa quyền của GroupOrganize


                                Them_Quyen_trong_Group_To_chuc_vao_user(groupOrgarizeId, perGroupOrganizes);
                                #endregion
                            }
                        }

                        #endregion

                    }



                    _ = !(String.IsNullOrEmpty(request.GroupOrganizeName)) ? groupOrganize.GroupOrganizeName = request.GroupOrganizeName : groupOrganize.GroupOrganizeName;
                    mariaDBContext.GroupOrganizes.Update(groupOrganize);
                    mariaDBContext.SaveChanges();

                    transaction.Commit();
                    return (groupOrganize, "UpdateGroupOrganizeSuccess");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return (null, "UpdateFaild");
                }
            }
        }

        public (GroupOrganize groupOrganize, string message) DeleteGroupOrganize(int groupOrgainzeId)
        {
            GroupOrganize group = mariaDBContext.GroupOrganizes.Find(groupOrgainzeId);
            if(group is null)
            {
                return (null, "GroupOrganizeNotExist");
            }
            User user = mariaDBContext.Users.Where(m => m.GroupOrganizeId == group.GroupOrganizeId && m.OrganizeId == group.OrganizeId).FirstOrDefault();
            if(!(user is null))
            {
                return (null, "GroupOrganizeAreadlyNotDelete");
            }
            List<PermissionGroupOrganize> permissionGroupOrganizes = mariaDBContext.PermissionGroupOrganizes.Where(m => m.GroupOrganizeId == group.GroupOrganizeId).ToList();
            mariaDBContext.PermissionGroupOrganizes.RemoveRange(permissionGroupOrganizes);
            mariaDBContext.GroupOrganizes.Remove(group);
            mariaDBContext.SaveChanges();
            return (group, "DeleteSuccess");
        }

        public object ShowAllPermissonByGroupOrganize(int groupOrganizeId)
        {
            GroupOrganize groupOrganize = mariaDBContext.GroupOrganizes.Find(groupOrganizeId);
            if (groupOrganize is null)
                return null;
            List<int> permissionOrganizeIds = mariaDBContext.PermissionGroupOrganizes
                                                                        .Where(m => m.GroupOrganizeId == groupOrganizeId)
                                                                        .Select(m => m.PermissionOrganizeId)
                                                                        .ToList();

            List<ModuleOrganize> moduleOrganizes = mariaDBContext.ModuleOrganizes
                                                                            .Include(m => m.FunctionOrganizes)
                                                                            .ThenInclude(m => m.PermissionOrganizes)
                                                                            .ToList();

            foreach (var module in moduleOrganizes)
            {
                foreach (var fno in module.FunctionOrganizes)
                {
                    var permissionOrganize = fno.PermissionOrganizes
                                                        .Where(m => permissionOrganizeIds.Contains(m.PermissionOrganizeId))
                                                        .ToList();
                    permissionOrganize.ForEach(m => m.PermissionFlag = true);
                }
            }

            var permissionTypes = mariaDBContext.PermissionTypes.ToList();
            return new { groupOrganize = groupOrganize, headers = permissionTypes, pemissionGroupOrganizes = moduleOrganizes };

        }

        public void Xoa_Tat_Ca_Quyen_User_trong_Group_To_chuc(int groupOrganizeId, List<int> permissionOrgarizeOlds)
        {
            List<string> userIds = mariaDBContext.Users.Where(m => m.GroupOrganizeId == groupOrganizeId).Select(m => m.UserId).ToList();
            string organizeId = mariaDBContext.Users.Where(m => m.GroupOrganizeId == groupOrganizeId).Select(m => m.OrganizeId).FirstOrDefault();
            List<UserPermissionOrganize> byEvent = mariaDBContext.UserPermissionOrganizes.Where(m => m.OrganizeId.Equals(organizeId)).ToList();

            List<UserPermissionOrganize> userPermissionOrganizes = new List<UserPermissionOrganize>();
            UserPermissionOrganize userPermissionOrganize;
            foreach (var userId in userIds)
            {
                foreach (var permissionOrganizeId in permissionOrgarizeOlds)
                {
                    userPermissionOrganize = mariaDBContext.UserPermissionOrganizes.Where(m => m.UserId.Equals(userId) && m.PermissionOrganizeId == permissionOrganizeId).FirstOrDefault();
                    userPermissionOrganizes.Add(userPermissionOrganize);
                }
            }
            mariaDBContext.UserPermissionOrganizes.RemoveRange(userPermissionOrganizes);
            mariaDBContext.SaveChanges();
        }

        public void Them_Quyen_trong_Group_To_chuc_vao_user(int groupOrganizeId, List<PermissionGroupOrganize> permissionOrganizeNews)
        {


            #region Lấy ra danh sách user trong user trong event theo group

            string organizeId = mariaDBContext.Users
                                                .Where(m => m.GroupOrganizeId == groupOrganizeId)
                                                .Select(m => m.OrganizeId)
                                                .FirstOrDefault();

            List<string> userIds = mariaDBContext.Users
                                                        .Where(m => m.GroupOrganizeId == groupOrganizeId && m.OrganizeId.Equals(organizeId))
                                                        .Select(m => m.UserId)
                                                        .ToList();


            #endregion

            #region Tạo mới List Quyền của User
            List<UserPermissionOrganize> userPermissionOrganizes = new List<UserPermissionOrganize>();
            UserPermissionOrganize userPermissionOrganize;
            foreach (var userId in userIds)
            {
                foreach (var permissionOrganize in permissionOrganizeNews)
                {
                    userPermissionOrganize = new UserPermissionOrganize
                    {
                        UserId = userId,
                        PermissionOrganizeId = permissionOrganize.PermissionOrganizeId,
                        PermissionOrganizeCode = permissionOrganize.PermissionOrganizeCode,
                        OrganizeId = organizeId,
                    };
                    userPermissionOrganizes.Add(userPermissionOrganize);
                }
            }
            #endregion

            mariaDBContext.UserPermissionOrganizes.AddRange(userPermissionOrganizes);
            mariaDBContext.SaveChanges();
        }
    }
}
