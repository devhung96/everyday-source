using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Services
{
    public interface IUserPermissionService
    {
        (object data, string message) GetPermissionUser(string userId);
        (object data, string message) UpdatePermissionUser(string userId, UpdatePermissionUserRequest request);
    }
    public class UserPermissionService: IUserPermissionService
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public UserPermissionService(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;
        }
        public (object data, string message) GetPermissionUser(string userId)
        {
            User user = repository.Users.FindByCondition(x => x.UserId == userId).FirstOrDefault();
            if (user is null) return (null, "UserNotExist");
            List<string> permissionCodes = repository.UserPermissions.FindByCondition(x => x.UserId.Equals(userId)).Select(x => x.PermissionCode).ToList();
            user.Permissions = repository.Permissions.FindByCondition(x => permissionCodes.Contains(x.PermissionCode)).ToList();
            return (user.Permissions, "Success");
        }
        public (object data , string message) UpdatePermissionUser(string userId, UpdatePermissionUserRequest request)
        {
            User user = repository.Users.GetById(userId);
            if (user is null) return (null, "UserNotExist");

            List<UserPermission> prepareRemoveAllPermissionUser = repository.UserPermissions
                                                                            .FindByCondition(x => x.UserId == userId)
                                                                            .ToList();
            repository.UserPermissions.RemoveRange(prepareRemoveAllPermissionUser);
            List<UserPermission> prepareInsertPermissionUser = new List<UserPermission>();
            foreach (var item in request.PermissionCodes)
            {
                UserPermission tmpUserPermission = new UserPermission
                {
                    PermissionCode = item,
                    UserId = userId
                };
                repository.UserPermissions.Add(tmpUserPermission);
                prepareInsertPermissionUser.Add(tmpUserPermission);
            }
            repository.SaveChanges();
            return (prepareInsertPermissionUser, "UpdateSuccess");
        }
    }
}
