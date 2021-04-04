using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.GroupOrganizes.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.PermissionOrganizes.Entities;
using Project.Modules.PermissionOrganizes.Requests;
using Project.Modules.Permissions.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Services
{
    public interface IPermissionOrganizeService
    {
        public object ShowAllPermissonOrganize();
        (ResponseTable response, string message) ShowAllUserPermissionOrganize(RequestTable request, string organizeId);
        object ShowPermissionOrganizeOfUser(PermissionOrganizeByUserRequest request);
        (User user, string message) UpdatePermissionOrganizeUser(PermissionOrganizeByUserRequest request);
        (User user, string message)AddPermissionOrganizeUser(PermissionOrganizeByUserRequest request);
        (List<User> users, string message) ShowUserOrganize(string organizeId);
    }
    public class PermissionOrganizeService : IPermissionOrganizeService
    {
        private readonly MariaDBContext mariaDBContext;
        public PermissionOrganizeService(MariaDBContext mariaDBContext)
        {

            this.mariaDBContext = mariaDBContext;
        }

        public (User user, string message) AddPermissionOrganizeUser(PermissionOrganizeByUserRequest request)
        {
            User userOragnize = mariaDBContext.Users.Where(m => 
                                                                m.OrganizeId.Equals(request.OrganizeId) && 
                                                                m.UserId.Equals(request.UserId)
                                                           ).FirstOrDefault();
            if( userOragnize is null)
            {
                return (null, "UserNotExistInOrganize");
            }    

            List<int> permissionIds = new List<int>();
            foreach (var module in request.pemissionOrganizeUsers)
            {
                foreach (var fn in module.FunctionOrganizes)
                {
                    permissionIds.AddRange(fn.PermissionOrganizes.Where(x => x.PermissionFlag == true)
                                                                 .Select(m => m.PermissionOrganizeId)
                                                                 .ToList());
                }
            }
            List<Entities.PermissionOrganize> permissionNews = mariaDBContext.PermissionOrganizes
                                                                                    .Where(m => permissionIds.Contains(m.PermissionOrganizeId))
                                                                                    .ToList();
            #region Thêm mới quyền
            List<UserPermissionOrganize> userPermissionNews = new List<UserPermissionOrganize>();
            UserPermissionOrganize userPermission;
            foreach (var permission in permissionNews)
            {
                userPermissionNews.Add(new UserPermissionOrganize
                {
                    UserId = request.UserId,
                    OrganizeId = request.OrganizeId,
                    PermissionOrganizeId = permission.PermissionOrganizeId,
                    PermissionOrganizeCode = permission.PermissionOrganizeCode,
                });
            }

            mariaDBContext.UserPermissionOrganizes.AddRange(userPermissionNews);
            mariaDBContext.SaveChanges();
            #endregion


            return (userOragnize, "AddPermissionUserSuccess");
        }

        public object ShowAllPermissonOrganize()
        {
            List<PermissionType> permissionTypes = mariaDBContext.PermissionTypes.ToList();
            List<ModuleOrganize> moduleOrganizes = mariaDBContext.ModuleOrganizes
                                                                        .Include(m =>m.FunctionOrganizes)
                                                                            .ThenInclude(m=>m.PermissionOrganizes)
                                                                            .ToList();
            var permissionOrganize = mariaDBContext.PermissionOrganizes.ToList();
            return new { headers = permissionTypes, pemissionGroupOrganizes = moduleOrganizes };
        }

        public (ResponseTable response, string message) ShowAllUserPermissionOrganize(RequestTable request, string organizeId)
        {
            Organize checkOrganize = mariaDBContext.Organizes.Find(organizeId);
            if(checkOrganize is null)
            {
                return (null, "OrganizeNotExist");
            }

            List<string> userIds = mariaDBContext.UserPermissionOrganizes
                                                                         .Where(m => m.OrganizeId.Equals(organizeId))
                                                                         .GroupBy(m => m.UserId)
                                                                         .Select(m => m.Key)
                                                                         .ToList();
            List<User> users = mariaDBContext.Users
                                             .Where(m => userIds.Contains(m.UserId))
                                             .ToList();

            users = users.Where(m => String.IsNullOrEmpty(request.Search) ||
                                    (!String.IsNullOrEmpty(request.Search) && 
                                    ((m.FullName.Contains(request.Search))||
                                    (m.UserEmail.Contains(request.Search))||
                                    m.ShareholderCode.Contains(request.Search)))
                                    ).ToList();
            ResponseTable responseTable = new ResponseTable
            {
                DateResult = users.Skip((request.Page - 1) * request.Results)
                                 .Take(request.Results)
                                 .ToList(),
                Info = new Info
                {
                    Page = request.Page,
                    Results = request.Results,
                    TotalRecord = users.Count(),
                }
            };
            return (responseTable,null);
        }
        public object ShowPermissionOrganizeOfUser(PermissionOrganizeByUserRequest request)
        {
            User user = mariaDBContext.Users.Find(request.UserId);
            List<int> permissionOrganizeIds = mariaDBContext.UserPermissionOrganizes
                                                                       .Where(m => m.UserId == request.UserId&&m.OrganizeId.Equals(request.OrganizeId))
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
            return new { user = user, headers = permissionTypes, pemissionOrganizeUsers = moduleOrganizes };
        }

        public (List<User> users, string message) ShowUserOrganize(string organizeId)
        {
            string organize = mariaDBContext.Organizes
                                            .Where(m => m.OrganizeId.Equals(organizeId) && m.OrganizeStatus == ORGANIZE_STATUS.ACTIVED)
                                            .Select(m=>m.OrganizeId)
                                            .FirstOrDefault();
            if(String.IsNullOrEmpty(organize))
            {
                return (null, "OrganizeNotExist");
            }
            List<string> userIds = mariaDBContext.UserPermissionOrganizes
                                                            .Where(m => m.OrganizeId.Equals(organizeId))
                                                            .Select(m=>m.UserId)
                                                            .ToList();

            List<User> users = mariaDBContext.Users.Where(
                                                        m => m.OrganizeId.Equals(organizeId) && 
                                                        (! userIds.Contains(m.UserId))
                                                    ).ToList();
            return (users, "ShowListSuccess");    
        }

        public (User user, string message) UpdatePermissionOrganizeUser(PermissionOrganizeByUserRequest request)
        {
            User user = mariaDBContext.Users.Where(m => m.UserId.Equals(request.UserId)).FirstOrDefault();
            if (!(request.pemissionOrganizeUsers is null))
            {
                using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
                {
                    try
                    {
                        #region Xóa tất cả quyền cũ
                        List<UserPermissionOrganize> permissionOlds = mariaDBContext.UserPermissionOrganizes
                                                                            .Where(
                                                                                    m => m.UserId.Equals(request.UserId) &&
                                                                                    m.OrganizeId.Equals(request.OrganizeId)
                                                                                ).ToList();
                        mariaDBContext.UserPermissionOrganizes.RemoveRange(permissionOlds);
                        mariaDBContext.SaveChanges();
                        #endregion

                        #region Lấy danh sách quyền mới 
                        List<int> permissionIds = new List<int>();
                        foreach (var module in request.pemissionOrganizeUsers)
                        {
                            foreach (var fn in module.FunctionOrganizes)
                            {
                                permissionIds.AddRange(fn.PermissionOrganizes.Where(x => x.PermissionFlag == true)
                                                                             .Select(m => m.PermissionOrganizeId)
                                                                             .ToList());
                            }
                        }

                        List<Entities.PermissionOrganize> permissionNews = mariaDBContext.PermissionOrganizes
                                                                                            .Where(m => permissionIds.Contains(m.PermissionOrganizeId))
                                                                                            .ToList();
                        #endregion

                        #region Thêm mới quyền
                        List<UserPermissionOrganize> userPermissionNews = new List<UserPermissionOrganize>();
                        foreach (var permission in permissionNews)
                        {
                            userPermissionNews.Add(new UserPermissionOrganize
                            {
                                UserId = request.UserId,
                                OrganizeId = request.OrganizeId,
                                PermissionOrganizeId = permission.PermissionOrganizeId,
                                PermissionOrganizeCode = permission.PermissionOrganizeCode,
                            });
                        }

                        mariaDBContext.UserPermissionOrganizes.AddRange(userPermissionNews);
                        mariaDBContext.SaveChanges();
                        #endregion

                        transaction.Commit();
                        return (user, "UpdateSuccess");
                    }
                    catch
                    {
                        transaction.Rollback();
                        return (null, "UpdateFaild");
                    }

                }
            }
            return (user, "UpdateSuccess");
        }
    }
}
