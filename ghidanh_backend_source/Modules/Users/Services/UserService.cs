using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Project.App.Databases;
using Project.App.DesignPatterns.Repositories;
using Project.App.Helpers;
using Project.Modules.Accounts.Entities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Services
{
    public interface IUserService
    {
        Task<(User user, string message)> Store(AddUserRequest request);
        Task<(User user, string message)> Update(UpdateUserRequest request,string userId);
        (User user, string message) Detail(string userId);
        (User user, string message) Delete(string userId);
        (ResponseTable data, string message) ShowAll(RequestTable requestTable);
        (User user, string message) UpdatePermission(UpdatePermissionRequest request);
    }
    public class UserService : IUserService
    {
        private readonly IRepositoryMariaWrapper repository;
        private readonly MariaDBContext mariaDBContext;
        private readonly IMapper mapper;
        private readonly string pathForder = "Admin/" + DateTime.Now.ToString("yyyyMMdd");
        public UserService(IRepositoryMariaWrapper _repository, IMapper _mapper,MariaDBContext mariaDBContext)
        {
            this.repository = _repository;
            this.mapper = _mapper;
            this.mariaDBContext = mariaDBContext;
        }

        public (User user, string message) Delete(string userId)
        {
            (User user, string message) = Detail(userId);
            if(user is null)
            {
                return (user, message);
            }
            if (!(user.UserImage is null))
            {
                _ = GeneralHelper.DeleteFile(user.UserImage);
            }
                repository.Users.RemoveMaria(user);
            repository.SaveChanges();

            List<AccountPermission> accountPermissions = repository.AccountPermissions
                                                                   .FindByCondition(m => m.AccountId.Equals(user.AccountId))
                                                                   .ToList();

            repository.AccountPermissions.RemoveRangeMaria(accountPermissions);
            repository.SaveChanges();

            repository.Accounts.RemoveMaria(repository.Accounts.GetById(user.AccountId));
            repository.SaveChanges();
            return (user, "DeleteSuccess");
            
        }

        public (User user, string message) Detail(string userId)
        {
            User user = repository.Users.GetById(userId);
            if (user is null)
            {
                return (null, "UserNotExist");
            }
            return (user, "Sussess");
        }

        public (ResponseTable data, string message ) ShowAll(RequestTable requestTable)
        {
            List<User> users = repository.Users.FindAll().ToList();
            users = users.Where(m => String.IsNullOrEmpty(requestTable.Search) ||
                                  (m.UserEmail.Contains(requestTable.Search) || m.UserName.Contains(requestTable.Search))).ToList();

            users = requestTable.Page == 0 ? users : users.Skip((requestTable.Page - 1) * requestTable.Limit).Take(requestTable.Limit).ToList();
            foreach (User user in users)
            {
                user.Group = repository.Groups.GetById(user.GroupId);
            }    
            foreach(User user in users)
            {
                user.PermissionCodes = repository.AccountPermissions.FindByCondition(m => m.AccountId.Equals(user.AccountId)).Select(m => m.PermissionCode).ToList();
            }
            ResponseTable response = new ResponseTable
            {
                Data = users,
                Info = new Info
                {
                    Page = requestTable.Page,
                    Limit = users.Count,
                    TotalRecord = users.Count,
                }
            };
            return (response, "ShowSuccess");
        }

        public async Task<(User user, string message)> Store(AddUserRequest request)
        {
            using (IDbContextTransaction transaction = mariaDBContext.Database.BeginTransaction())
            {
                try
                {

                    Account account = repository.Accounts.FirstOrDefault(m => m.AccountCode.Equals(request.UserEmail));
                    if (!(account is null))
                    {
                        return (null, "EmailAreadyExist");
                    }
                    User user = mapper.Map<User>(request);
                    string saft = 5.RandomString();

                    #region Thêm quyền, thêm tài khoản login
                    account = new Account() { AccountCode = request.UserEmail, Password = (request.Password + saft).HashPassword(), Saft = saft ,AccountType = Account.TYPE_ACCOUNT.CMS, GroupCode = request.GroupId };
                    repository.Accounts.Add(account);
                    repository.SaveChanges();
                    List<AccountPermission> accountPermissions = new List<AccountPermission>();
                    List<string> permissionIds = repository.GroupPermissions.FindByCondition(m => m.GroupId.Equals(request.GroupId)).Select(m => m.PermissionCode).ToList();
                    foreach(string permissionId in permissionIds)
                    {
                        accountPermissions.Add(new AccountPermission(account.AccountId, permissionId));
                    }
                  
                    repository.AccountPermissions.AddRange(accountPermissions);
                    repository.SaveChanges();
                    #endregion

                    if (!(request.Image is null))
                    {
                        (string path, string fullName) = await GeneralHelper.UploadFileProAsync(request.Image, pathForder);
                        user.UserImage = path;
                    }
                    user.AccountId = account.AccountId;
                    repository.Users.Add(user);
                    repository.SaveChanges();
                    transaction.Commit();
                    return (user,"StoreUserSuccess");
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    return (null, "StoreStudentFaild");
                }

            }
        }

        public async Task<(User user, string message)> Update(UpdateUserRequest request, string userId)
        {
            User user = repository.Users.GetById(userId);
            if(user is null)
            {
                return (null, "UserNotExit");
            }
            if( !(request.Image is null))
            {
                _ = GeneralHelper.DeleteFile(user.UserImage);
                (string path, _)  = await GeneralHelper.UploadFileProAsync(request.Image, pathForder);
                user.UserImage = path;
            }
            user.UserName = string.IsNullOrEmpty(request.UserName) ? user.UserName : request.UserName; 

            if(string.IsNullOrEmpty(user.GroupId)|| ! user.GroupId.Equals(request.GroupId))
            {
                List<AccountPermission> deletePers = repository.AccountPermissions.FindByCondition(m => m.AccountId.Equals(user.AccountId)).ToList();
                repository.AccountPermissions.RemoveRangeMaria(deletePers);

                List<AccountPermission> perAdds = new List<AccountPermission>();
                List<string> perCodes = repository.GroupPermissions.FindByCondition(m => m.GroupId.Equals(request.GroupId)).Select(m => m.PermissionCode).ToList();
                foreach (string perCode in perCodes)
                {
                    perAdds.Add(new AccountPermission(user.AccountId, perCode));
                }

                repository.AccountPermissions.AddRange(perAdds);
                user.GroupId = request.GroupId;
            }    
            repository.Users.UpdateMaria(user);
            repository.SaveChanges();
            return (user, "UpdateUserSuccess");
        }

        public (User user, string message) UpdatePermission(UpdatePermissionRequest request)
        {
            User user = repository.Users.FirstOrDefault(m => m.UserId.Equals(request.UserId));
            if(user is null)
            {
                return (null, "UserNotExist");
            }
            List<AccountPermission> PermissionDB = repository.AccountPermissions.FindByCondition(m => m.AccountId.Equals(user.AccountId)).ToList();
            repository.AccountPermissions.RemoveRangeMaria(PermissionDB);
            repository.SaveChanges();
            List<AccountPermission> accountPermissions = new List<AccountPermission>();
            foreach( string permisionCode in request.PermissionCodes)
            {
                accountPermissions.Add(new AccountPermission() { AccountId = user.AccountId, PermissionCode = permisionCode });
            }
            repository.AccountPermissions.AddRange(accountPermissions);
            repository.SaveChanges();
            return (user, "Success");
        }
    }
}