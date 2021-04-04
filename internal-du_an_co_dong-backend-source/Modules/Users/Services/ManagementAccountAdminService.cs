using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using Project.App.Database;
using Project.App.Providers;
using Project.Modules.Organizes.Entities;
using Project.Modules.PermissonUsers;
using Project.Modules.Users.Caches;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Users.Services
{

    public interface IManagementAccountAdminService
    {
        public (List<UserSuper> users, string message) ShowAllUserAdmin();
        public (UserSuper user, string message) CreateUserAdmin(UserSuper user);
        public (UserSuper user, string message) EditAdmin(UpdateUserAdminRequest request, string userId);
        public (UserSuper user, string message) DeleteAdmin(string userId);
        public (UserSuper user, string message) ResetAdmin(string userId);
        // public (object user, string message) ForgotPassword(RequestForgotAdmin request);
    }
    public class ManagementAccountAdminService : IManagementAccountAdminService
    {
        private readonly MariaDBContext _mariaDBContext;
        public ManagementAccountAdminService(MariaDBContext mariaDBContext)
        {
            _mariaDBContext = mariaDBContext;

        }

        public (UserSuper user, string message) CreateUserAdmin(UserSuper user)
        {

            UserSuper checkUser = _mariaDBContext.UserSupers.Where(m =>
                                                                        m.Email.Equals(user.Email) && 
                                                                        m.DeleteStatus == User.DELETE_STATUS.EXIST
                                                                    ).FirstOrDefault();
            if (!(checkUser is null))
            {
                return (null, "EmailAreadlyExist");
            }
            string salt = 5.RandomString();
            user.Password = (salt + user.Password).HashPassword();
            user.Salt = salt;
            if (!String.IsNullOrEmpty(user.OrganizeId))
            {
                user.Level = UserSuper.USER_TYPE.ADMIN;
            }
            _mariaDBContext.UserSupers.Add(user);
            _mariaDBContext.SaveChanges();

            PermissionUser permissionUser = new PermissionUser()
            {
                UserId = user.UserSuperId,
                PermissionCode = PermissionUser.PERMISSION_DEFAULT.ADMIN.ToString(),
            };
            _mariaDBContext.PermissionUsers.Add(permissionUser);
            _mariaDBContext.SaveChanges();

            return (user, "AddAdminOrgainzeSuccess");

        }

        public (UserSuper user, string message) DeleteAdmin(string userId)
        {
            UserSuper user = _mariaDBContext.UserSupers.Where(m =>
                                                                    m.UserSuperId.Equals(userId)
                                                                    && m.DeleteStatus == (int)User.DELETE_STATUS.EXIST
                                                              )
                                                              .FirstOrDefault();
            
            if (user is null)
            {
                return (null, "");
            }
            Organize organize = _mariaDBContext.Organizes.Where(m => !(String.IsNullOrEmpty(user.OrganizeId)) && m.OrganizeId.Equals(user.OrganizeId)).FirstOrDefault();
            if (!(organize is null))
            {
                return (null, "UserManageOrganizationNotDelete");
            }
            _mariaDBContext.UserSupers.Remove(user);
            _mariaDBContext.SaveChanges();
            return (user, "DeleteAdminSuccess");
        }

        public (UserSuper user, string message) EditAdmin(UpdateUserAdminRequest request, string userId)
        {
            UserSuper user = _mariaDBContext.UserSupers.Where(m =>
                                                                m.UserSuperId.Equals(userId) && 
                                                                m.DeleteStatus == (int)User.DELETE_STATUS.EXIST
                                                              ).FirstOrDefault();
            if (user is null)
            {
                return (null, "UserIdNotExist");
            }

            #region MergeData
            _ = !String.IsNullOrEmpty(request.UserImage) ? user.Image = request.UserImage : user.Image;
            _ = !String.IsNullOrEmpty(request.FullName) ? user.FullName = request.FullName : user.FullName;
            #endregion

            _mariaDBContext.UserSupers.Update(user);
            _mariaDBContext.SaveChanges();
            return (user, "UpdateUserAdminSuccess");
        }


        public (UserSuper user, string message) ResetAdmin(string userId)
        {

            UserSuper userAdmin = _mariaDBContext.UserSupers
                                                         .Where(x =>
                                                                     x.UserSuperId.Equals(userId)
                                                                     && x.Level == (int)UserSuper.USER_TYPE.ADMIN
                                                                     && x.DeleteStatus == (int)User.DELETE_STATUS.EXIST
                                                               )
                                                         .FirstOrDefault();
            if (userAdmin is null)
            {
                return (null, "UserNotExist");
            }
            string passwordNew = 6.RandomString();
            userAdmin.Password = (userAdmin.Salt + passwordNew).HashPassword();
            _mariaDBContext.UserSupers.Update(userAdmin);
            _mariaDBContext.SaveChanges();
            TransportPatternProvider.Instance.Emit("SendEmail", new SendMailRequest
            {
                MessageSubject = "Thông báo làm mới mật khẩu trang đăng nhập trang quản trị Đại hội cổ đông ",
                MessageContent = "Mật khẩu mới của bạn là : <b>" + passwordNew 
                        + "</b> . <br/> Để đảm bảo vấn đề bảo mật tài khoản, vui lòng đăng nhập và đổi mật khẩu."
                        + "<br/> Trân trọng!",
                Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = userAdmin.Email}
                }
            });
            return (userAdmin, "SendEmailSuccess");
        }

        public (List<UserSuper> users, string message) ShowAllUserAdmin()
        {

            List<string> idAdmins = _mariaDBContext.PermissionUsers
                                                   .Where(x => x.PermissionCode == "ADMIN")
                                                   .Select(x => x.UserId)
                                                   .ToList();

            List<UserSuper> users = _mariaDBContext.UserSupers
                                                        .Where(x =>
                                                                    idAdmins.Contains(x.UserSuperId)
                                                                    && x.Level == (int)UserSuper.USER_TYPE.ADMIN
                                                                    && x.DeleteStatus == (int)User.DELETE_STATUS.EXIST
                                                              )
                                                        .Include(m => m.Organize)
                                                        .ToList();
            return (users, "ShowListUserSuccess");
        }





    }
}
