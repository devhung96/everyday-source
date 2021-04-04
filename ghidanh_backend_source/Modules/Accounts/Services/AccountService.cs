using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Project.App.DesignPatterns.ObserverPatterns;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Accounts.Entities;
using Project.Modules.Accounts.Requests;
using Project.Modules.Groups.Entities;
using Project.Modules.Lecturers.Entities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Accounts.Services
{
    public interface IAccountService
    {
        (object data, string message) Login(LoginRequest request);
        (Personal data, string message) Profile(string accountId);
        (object data, string message) Fogot(ForgotRequest request);
        (object data, string message) ResetPassword(ResetPasswordRequest request);
        (object data, string message) ChangePassword(ChangePasswordRequest request, string AccountId);
        (object user, string message) FogotUpdatePassword(ForgotPasswordRequest request, string key);
    }
    public class AccountService : IAccountService
    {
        private readonly IRepositoryMariaWrapper repository;
        private readonly IConfiguration configuration;
        private readonly IDistributedCache cacheDBContext;
        private readonly TokenHelper tokenHelper;

        public AccountService(IRepositoryMariaWrapper repositoryWrapper, IConfiguration _configuration, IDistributedCache _cacheDBContext)
        {
            repository = repositoryWrapper;
            cacheDBContext = _cacheDBContext;
            configuration = _configuration;
            this.tokenHelper = new TokenHelper(_configuration);
        }

        public (object data, string message) ChangePassword(ChangePasswordRequest request, string accountId)
        {
            Account account = repository.Accounts.GetById(accountId);
            if(account is null)
            {
                return (null, "TokenFaild");
            }    
            if(! request.Confirm.Equals(request.PasswordNew))
            {
                return (null, "ConfirmNotMatchPasswordNew");
            }    
            if(! (request.PasswordOld + account.Saft).HashPassword().Equals(account.Password)  )
            {
                return (null, "IncorrectPassword");
            }
            account.Password = (request.PasswordNew + account.Saft).HashPassword();
            account.AccountUpdate = DateTime.Now;
            account.Token = null;
            repository.Accounts.UpdateMaria(account);
            repository.SaveChanges();
            return (new AccountResponse(account), "ChangePasswordSuccess");
        }

        public (object data, string message) Fogot(ForgotRequest request)
        {

            User isUser = repository.Users.FirstOrDefault(m => m.UserEmail.Equals(request.UserName));
            string accountId, email;
            if (isUser is null)
            {
                Lecturer isLecturer = repository.Lecturers.FirstOrDefault(m => m.LecturerCode.Equals(request.UserName));
                if(isLecturer is null)
                { return (null, "UserNameNotExist"); }
                accountId = isLecturer.AccountId;
                email = isLecturer.LecturerEmail;
              
             }
            else
            {
                accountId = isUser.AccountId;
                email = isUser.UserEmail;
            }

            string key = 8.RandomString();
            var option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(180));
            cacheDBContext.SetString(key, accountId, option);
            string link = configuration["ForgotPassword:Url"] + key;
            SendEmail(email, link);
            return (email, "SendMailSuccess");
        }
        public void SendEmail(string email, string link)
        {
            ObserverPattern.Instance.Emit("SendEmail", new SendMailRequest
            {

                MessageSubject = "Forgot Ghi Danh system account password",
                MessageContent = "Here is the link to reset your login password: " + link
                                            + "<br/> Please visit the link to create a new password."
                                              + "<br/> Your link will expire in 3 minutes."
                                             + "<br/> Sincerely!",
                Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = email}
                }
            });
        }


        public (string email, string message) CheckKey(string key)
        {
            string accountId = cacheDBContext.GetString(key);
            if (accountId is null)
            {
                return (null, "KeyDoesNotExist");
            }
            return (accountId, "Success");
        }


        public (object user, string message) FogotUpdatePassword(ForgotPasswordRequest request, string key)
        {
            var (accountId, message) = CheckKey(key);

            if (String.IsNullOrEmpty(accountId))
                return (null, message);
            cacheDBContext.Remove(key);

            Account account = repository.Accounts.GetById(accountId);

            string saft = 5.RandomString();
            account.Saft = saft;
            account.Password = (request.PasswordNew + saft).HashPassword();
            repository.Accounts.UpdateMaria(account);
            repository.SaveChanges();
            return (new AccountResponse(account), "Success");
        }


        public (object data, string message) Login(LoginRequest request)
        {
            if (!repository.Accounts.Any(a => a.AccountCode.Equals(request.UserName)))
            {
                return (null, "AccountCodeIsNotExist");
            }
        
            Account account= repository.Accounts.FirstOrDefault(a => a.AccountCode.Equals(request.UserName)&& a.AccountType == request.Type);
            if(account is null)
            {
                return(null,"UserNameNotExist");
            }    
            

            if (! account.Password.Equals((request.Password+account.Saft).HashPassword()))
            {
                return (null, "PasswordIsNotCorrect");
            }
                      
            List<string> permissionCodes = repository.AccountPermissions.FindByCondition(p => p.AccountId.Equals(account.AccountId)).Select(p => p.PermissionCode).ToList();

            if(account.AccountType ==Account.TYPE_ACCOUNT.CMS)
            {
                List<Permission> permissionCms = repository.Permissions.FindByCondition(m => m.PermissionType == Permission.Page.CMS).ToList();
                permissionCodes = permissionCms.Where(m => permissionCodes.Contains(m.PermissionCode)).Select(m => m.PermissionCode).ToList();
            }    

           
            account.Token = tokenHelper.GenerateToken(account,permissionCodes);
            account.LoginAt = DateTime.Now;
            repository.Accounts.UpdateMaria(account);
            repository.SaveChanges();
            return ( new AccountResponse(account), "LoginSuccess");
        }

        public (object data, string message) ResetPassword(ResetPasswordRequest request)
        {
            Account account = repository.Accounts.FirstOrDefault(m => m.AccountCode.Equals(request.UserName) && m.AccountType == request.Type);
            if(account is null)
            { return (account, "UserNameNotExist"); }
            account.Password = (request.Password + account.Saft).HashPassword();
            repository.Accounts.UpdateMaria(account);
            repository.SaveChanges();
            return (new AccountResponse(account), "ResetUserSuccess");
        }

        public (Personal data, string message) Profile(string accountId)
        {
            Account account = repository.Accounts.GetById(accountId);
            if (account is null)
                return (null, "TokenFaild");
            Personal profile = new Personal() ;
            switch (account.AccountType)
            {
                case Account.TYPE_ACCOUNT.CMS:
                    profile = repository.Users.FindByCondition(m => m.UserEmail.Equals(account.AccountCode))!.Select(m=>new Personal(m)).FirstOrDefault();
                    break;
                case Account.TYPE_ACCOUNT.STUDENT:
                    profile = repository.Students.FindByCondition(m => m.StudentCode.Equals(account.AccountCode))!.Select(m => new Personal(m)).FirstOrDefault();
                    break;  
                case Account.TYPE_ACCOUNT.LECTURER:
                    profile = repository.Lecturers.FindByCondition(m => m.LecturerCode.Equals(account.AccountCode))!.Select(m => new Personal(m)).FirstOrDefault();
                    break;
            }
            if (profile == null)
                return (null, "TokenFaild");
            return (profile, "Success");
        }
    }
}
