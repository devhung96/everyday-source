using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Project.App.Database;
using Project.App.Requests;
using Project.Modules.PermissonUsers;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Services
{
    public interface IUseSuperService
    {
        ResponseTable  ListSuperAdmin(RequestTable request);
        (UserSuper superAdmin, string message) AddUserSuper(AddUserSuperRequest request);
        (UserSuper superAdmin, string message) GetUserSuper(string userSuperId);
        (UserSuper superAdmin, string message) DeleteUserSuper(string userSuperId);

        (UserSuper superAdmin, string message) EditUserSuper(string userSuperId, UpdateUserSuperRequest request);
    }
    public class UserSuperService : IUseSuperService
    {
        private readonly MariaDBContext mariaDBContext;
        public UserSuperService(MariaDBContext mariaDBContext)
        {
            this.mariaDBContext = mariaDBContext;
        }
        public (UserSuper superAdmin, string message) AddUserSuper(AddUserSuperRequest request)
        {
            string Email = mariaDBContext.UserSupers
                                         .Where(m => m.Email.Equals(request.Email))
                                         .Select(m => m.Email)
                                         .FirstOrDefault();

            if (!(String.IsNullOrEmpty(Email)))
            {
                return (null, "EmailAreadlyExist");
            }
            string salt = 5.RandomString();
            UserSuper userSuper = new UserSuper
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = (salt + request.Password).HashPassword(),
                Image = request.Image,
                Salt = salt,
                Level = UserSuper.USER_TYPE.SUPER
            };
            mariaDBContext.UserSupers.Add(userSuper);
            mariaDBContext.SaveChanges();


            PermissionUser permissionUser = new PermissionUser { PermissionCode = "SUPER", UserId = userSuper.UserSuperId };
            mariaDBContext.PermissionUsers.Add(permissionUser);
            mariaDBContext.SaveChanges();
            return (userSuper, "AddUserSupperSuccess");
        }

        public (UserSuper superAdmin, string message) DeleteUserSuper(string userSuperId)
        {
            (UserSuper superAdmin, string message) = GetUserSuper(userSuperId);
            if (superAdmin is null)
            {
                return (null, message);
            }
            superAdmin.DeleteStatus = User.DELETE_STATUS.DELETED;
            mariaDBContext.UserSupers.Update(superAdmin);
            PermissionUser permissionUser = mariaDBContext.PermissionUsers
                                                          .Where(m => m.UserId.Equals(superAdmin.UserSuperId))
                                                          .FirstOrDefault();

            mariaDBContext.PermissionUsers.Remove(permissionUser);
            mariaDBContext.SaveChanges();
            return (superAdmin, "Success");
        }

        public (UserSuper superAdmin, string message) EditUserSuper(string userSuperId, UpdateUserSuperRequest request)
        {
            (UserSuper superAdmin, string message) = GetUserSuper(userSuperId);
            if (superAdmin is null)
            {
                return (null, message);
            }

            _ = String.IsNullOrEmpty(request.FullName) ? superAdmin.FullName : superAdmin.FullName = request.FullName;
            _ = String.IsNullOrEmpty(request.Email) ? superAdmin.Email : superAdmin.Email = request.Email;
            _ = String.IsNullOrEmpty(request.Image) ? superAdmin.Image : superAdmin.Image = request.Image;
            mariaDBContext.UserSupers.Update(superAdmin);
            mariaDBContext.SaveChanges();
            return (superAdmin, "Success");
        }

        public (UserSuper superAdmin, string message) GetUserSuper(string userSuperId)
        {
            UserSuper userSuper = mariaDBContext.UserSupers.Where( m=>
                                                                       m.UserSuperId.Equals(userSuperId)&&
                                                                       m.DeleteStatus == User.DELETE_STATUS.EXIST
                                                                       ).FirstOrDefault();
            if (userSuper is null)
            {
                return (null, "UserSupperNotExist");
            }
            return (userSuper, "Success");
        }

        public ResponseTable ListSuperAdmin(RequestTable request)
        {
            List<string> superAdminIds = mariaDBContext.PermissionUsers.Where(m => m.PermissionCode.Equals("SUPER"))
                                                                      .Select(m => m.UserId)
                                                                      .ToList();

            List<UserSuper> userSupers = mariaDBContext.UserSupers.Where(m => superAdminIds.Contains(m.UserSuperId)).ToList();

            userSupers = userSupers.Where(m => m.Email.Contains(request.Search) || m.FullName.Contains(request.Search)).ToList();

            ResponseTable response = new ResponseTable
            {
                DateResult = userSupers.Skip((request.Page - 1) * request.Results)
                                        .Take(request.Results)
                                        .ToList(),
                Info = new Info
                {
                    Page = request.Page,
                    Results = request.Results,
                    TotalRecord = userSupers.Count()
                },
                
            };
            return response;

        }
    }
}
