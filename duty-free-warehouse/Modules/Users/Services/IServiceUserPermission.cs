using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Request;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Users.Services
{
    public interface IServiceUserPermission
    {
        (object data, string message) UpdateUserPermission(UpdatePermissionUser permissionUser);
        (object data, string message) GetPermissionUser(int userid);
    }

    public class ServiceUserPermission : IServiceUserPermission
    {
        public readonly IConfiguration config;
        public readonly MariaDBContext dBContext;
        public ServiceUserPermission(IConfiguration _config, MariaDBContext _dBContext)
        {
            config = _config;
            dBContext = _dBContext;
        }

        public (object data, string message) GetPermissionUser(int userid)
        {
            User user = dBContext.Users.Include(x=>x.Department).Include(x => x.UserPermissions).ThenInclude<User, UserPermission, Permission>(x => x.Permission).ThenInclude(x => x.Module).FirstOrDefault(x => x.UserId == userid);
            if (user == null)
                return (null, "User does not exist.");
            return (new {
                UserID = user.UserId,
                UserName = user.UserName,
                FullName = user.FullName,
                CreatedAt = user.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"),
                Department = new { deparmentName = user.Department.Name, departmentCode = user.Department.Code },
                permissions = user.UserPermissions.Select(x => new
                {
                    permissionid = x.PermissionID,
                    permissioncode = x.PermissionCode,
                    permissionname = x.Permission.Name,
                    module = new
                    {
                        moduleid = x.Permission.ModuleID,
                        modulename = x.Permission.Module?.Name,
                        modulecode = x.Permission.Module?.Code
                    }
                }).ToList()
            }, "Success!!!");
        }

        public (object data, string message) UpdateUserPermission(UpdatePermissionUser permissionUser)
        {
            List<UserPermission> permissionold = dBContext.UserPermissions.Where(x => !permissionUser.PermissionCode.Contains(x.PermissionCode) && x.UserID == permissionUser.UserID).ToList();
            for (int i = 0; i < permissionUser.PermissionCode.Count; i++)
            {
                UserPermission groupModule = dBContext.UserPermissions.FirstOrDefault(x => x.PermissionCode == permissionUser.PermissionCode[i] && x.UserID == permissionUser.UserID);
                if (groupModule != null)
                    continue;
                dBContext.UserPermissions.Add(new UserPermission()
                {
                    PermissionCode = permissionUser.PermissionCode[i],
                    PermissionID = permissionUser.PermissionID[i],
                    UserID = permissionUser.UserID.Value
                });
            }
            dBContext.UserPermissions.RemoveRange(permissionold);
            dBContext.SaveChanges();
            return ("Thành công!!!", "Cập nhật quyền của người dùng thành công!!!");
        }

    }
}
