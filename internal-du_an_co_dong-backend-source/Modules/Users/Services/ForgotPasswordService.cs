using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Project.App.Database;
using Project.App.Providers;
using Project.Modules.Events.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.Users.Caches;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project.Modules.Users.Services
{
    public interface IForgotPasswordService
    {
        (User user, string message) CheckEmail(RequestForgot request);

        public (string value, string message) CheckKey(string key);

        public (EventUser user, string message) UpdatePassword(ForgotPasswordRequest request, string key);

        (Object user, string message) CheckEmailAdmin(RequestForgotAdmin request);
        (object value, string message) CheckKeyAdmin(string key);
        (object user, string message) UpdatePasswordAdmin(ForgotPasswordRequest request, string key);
    }
    public class ForgotPasswordService : IForgotPasswordService
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly IConfiguration config;
        private readonly IDistributedCache cacheDBContext;
        private readonly IManagementOTPService cacheService;

        public ForgotPasswordService(MariaDBContext mariaDB, IConfiguration configuration, IDistributedCache cache, IManagementOTPService managementOTPService)
        {
            this.mariaDBContext = mariaDB;
            this.cacheDBContext = cache;
            this.config = configuration;
            this.cacheService = managementOTPService;
        }

        public (User user, string message) CheckEmail(RequestForgot request)
        {
            Event @event = mariaDBContext.Events.Find(request.EventId);
            if (@event is null)
            {
                return (null, "EventNotExist");
            }
            //Organize organize = mariaDB.Organizes.Where(m => m.OrganizeCodeCk.Equals(request.StockCode)).FirstOrDefault();
            //if(organize is null)
            //{
            //    return (null, "OrganizeCode is not exist");
            //}
            User user = mariaDBContext.Users.Where(m => m.ShareholderCode.Equals(request.ShareholderCode) && (m.OrganizeId.Equals(@event.OrganizeId)))
                                     .FirstOrDefault();
            if (user is null)
                return (null, "ShareholderCodeNotExist");

            int idEventUser = mariaDBContext.EventUsers.Where(m => m.EventId.Equals(@event.EventId) && m.UserId.Equals(user.UserId)).Select(m => m.EventUserId).FirstOrDefault();

            string key = 8.RandomString();
            var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(300));
            cacheDBContext.SetString(key, idEventUser.ToString(), option);

            string link = config["ForgotPassword:Url"] + @event.EventId + "/" + key;

            TransportPatternProvider.Instance.Emit("SendEmail", new App.Providers.SendMailRequest
            {
                MessageSubject = "Thông báo quên mật khẩu trang Đại hội cổ đông",
                MessageContent = "Đây là liên kết đặt lại mật khẩu của bạn: " + link + ". Vui lòng truy cập liên kết để đặt lại mật khẩu của bạn, liên kết sẽ hết hạn sau 5 phút."
                + "<br/> Nếu bạn không yêu cầu đặt lại mật khẩu, bạn có thể bỏ qua email này."
                + "<br/>Trân trọng !",
                Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = user.UserEmail}
                }
            });

            return (user, "SendEmailSuccess");
        }

        public (string value, string message) CheckKey(string key)
        {
            var eventUserId = cacheDBContext.GetString(key);
            if (eventUserId is null)
            {
                return (null, "KeyNotExist");
            }
            return (eventUserId, "Success");
        }

        public (EventUser user, string message) UpdatePassword(ForgotPasswordRequest request, string key)
        {
            var (eventUserId, message) = CheckKey(key);

            if (String.IsNullOrEmpty(eventUserId))
                return (null, "KeyNotExist");
            cacheDBContext.Remove(key);

            EventUser user = mariaDBContext.EventUsers
                    .FirstOrDefault(m => m.EventUserId == int.Parse(eventUserId));

            string salt = 5.RandomString();
            user.UserSalt = salt;
            user.UserPassword = (salt + request.PasswordNew).HashPassword();
            user.PasswordSystem = null;
            mariaDBContext.EventUsers.Update(user);
            mariaDBContext.SaveChanges();

            return (user, "UpdatePasswordSuccess");
        }

        #region Quen mat khau ADMIN

        public (object user, string message) CheckEmailAdmin(RequestForgotAdmin request)
        {
            string Key = 10.RandomString();

            bool isEmail = request.UserEmail.CheckEmail();
            if (isEmail == false)
            {
                var organizes = mariaDBContext.Organizes.ToList();
                var organize = organizes.FirstOrDefault(x => x.OrganizeCodeCk.Equals(request.StockCode));
                if (organize is null) return (null, "UserNotExist");
                var users = mariaDBContext.Users.Where(x => x.OrganizeId.Equals(organize.OrganizeId)).ToList();
                User user = users.FirstOrDefault(m => m.ShareholderCode.Equals(request.UserEmail));
                string link = config["ForgotPassword:UrlAdmin"] + Key;

                TransportPatternProvider.Instance.Emit("SendEmail", new App.Providers.SendMailRequest
                {
                    MessageSubject = "Thông báo quên mật khẩu trang Đại hội cổ đông",
                    MessageContent = "Đây là liên kết đặt lại mật khẩu của bạn: " + link + ". Vui lòng truy cập liên kết để đặt lại mật khẩu của bạn, liên kết sẽ hết hạn sau 5 phút."
                                 + "<br/> Nếu bạn không yêu cầu đặt lại mật khẩu, bạn có thể bỏ qua email này."
                                 + "<br/>Trân trọng !",
                    Contacts = new List<SendMailContact>
                    {
                        new SendMailContact{ContactEmail = user.UserEmail}
                    }
                });

                cacheService.SetOTP(Key, user.UserId);
                return (user, "SendEmailSuccess");

               
            }
            else
            {
               
                UserSuper userSuper = mariaDBContext.UserSupers.Where(m => m.Email.Equals(request.UserEmail)).FirstOrDefault();

                // Xu li torng bang userSuper
                if (userSuper is null)
                {
                    return (null, "UserNotExist");
                }
                else
                {
                    //string title = $"{config["EmailSettings:Subject"]}";
                    //string mail = userSuper.Email;
                    string link = config["ForgotPassword:UrlAdmin"] + Key;

                    TransportPatternProvider.Instance.Emit("SendEmail", new App.Providers.SendMailRequest
                    {
                        MessageSubject = "Thông báo quên mật khẩu trang Đại hội cổ đông",
                        MessageContent = "Đây là liên kết đặt lại mật khẩu của bạn: " + link + ". Vui lòng truy cập liên kết để đặt lại mật khẩu của bạn, liên kết sẽ hết hạn sau 5 phút."
                                 + "<br/> Nếu bạn không yêu cầu đặt lại mật khẩu, bạn có thể bỏ qua email này."
                                 + "<br/> Trân trọng !",
                        Contacts = new List<SendMailContact>
                        {
                            new SendMailContact{ContactEmail = userSuper.Email}
                        }
                    });

                    cacheService.SetOTP(Key, userSuper.UserSuperId);
                    return (userSuper, "SendEmailSuccess");
                }

            }
        }

        public (object value, string message) CheckKeyAdmin(string key)
        {
            var userId = cacheService.GetOTP(key);
            if (userId is null)
            {
                return (null, "KeyNotExist");
            }
            return (userId, "Success");
        }

        public (object user, string message) UpdatePasswordAdmin(ForgotPasswordRequest request, string key)
        {
            (object value, string message) = CheckKeyAdmin(key);
            if (value is null)
            {
                return (null, message);
            }
            string salt = 5.RandomString();
            cacheService.RemoveOTP(key);

            User user = mariaDBContext.Users.Find(value.ToString());
            if (user is null)
            {
                UserSuper userSuper = mariaDBContext.UserSupers.Find(value.ToString());
                if (userSuper is null)
                {
                    return (null, "UpdateFaild");
                }

                userSuper.Password = (salt + request.PasswordNew).HashPassword();
                userSuper.Salt = salt;
                mariaDBContext.UserSupers.Update(userSuper);
                mariaDBContext.SaveChanges();

                return (userSuper, "UpdatePasswordSuccess");
            }
            else
            {
                user.UserPassword = (salt + request.PasswordNew).HashPassword();
                user.UserSalt = salt;
                user.PasswordSystem = null;
                mariaDBContext.Users.Update(user);
                mariaDBContext.SaveChanges();
                return (user, "UpdatePasswordSuccess");
            }
        }


        #endregion
    }
}
