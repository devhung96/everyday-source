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
    public interface IRoleService
    {
        (object data, string message) Create(CreateRoleRequest roleRequest, string groupId, int level);
        (object data, string message) Update(UpdateRoleRequest request, string Id);
        (object data, string message) Delete(string Id);
        (object data, string message) Detail(string Id);
        (object data, string message) ShowAllRole(UserLevelEnum level,string groupId = null);
        (object data, string message) ShowAllByLevel(UserLevelEnum level);

    }
    public class RoleService : IRoleService
    {
        private readonly IRepositoryWrapperMariaDB _repository;
        public RoleService(IRepositoryWrapperMariaDB repository)
        {
            _repository = repository;
        }
        public (object data, string message) Create(CreateRoleRequest roleRequest, string groupId, int level)
        {
            Role role = _repository.Roles.FindByCondition(x => x.RoleCode == roleRequest.RoleCode)
                                               .FirstOrDefault();
            if (role != null)
            {
                return (null, $"RoleCodeAlreadyExist");
            }
            Dictionary<string, string> permissionInLevels = _repository.Permissions.FindByCondition(x => x.Level == (UserLevelEnum)level)
                                                                                  .ToDictionary(x => x.PermissionCode, x => x.PermissionCode);

            foreach (var permission in roleRequest.PermissionIds)
            {

                if (!permissionInLevels.ContainsKey(permission))
                {
                    return (null, "PermissionNotExistLevel");
                }
            }
            role = new Role
            {
                RoleCode = roleRequest.RoleCode,
                RoleLevel =(UserLevelEnum)level,
                RoleName = roleRequest.RoleName,
                GroupId = string.IsNullOrEmpty(groupId)? null: groupId
            };
            _repository.Roles.Add(role);
            _repository.SaveChanges();

            List<RolePermission> rolePermissions = new List<RolePermission>();
            foreach (string idPermission in roleRequest.PermissionIds)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = role.RoleId,
                    PermissionCode = idPermission
                });
            }

            _repository.RolePermissions.AddRange(rolePermissions);
            _repository.SaveChanges();
            return (role, "CreateSuccess");
        }

        public (object data, string message) Delete(string Id)
        {
            Role role = _repository.Roles.GetById(Id);
            if (role is null)
            {
                return (null, $"RoleNotExist");
            }
            _repository.Roles.Remove(role);
            _repository.SaveChanges();
            return (role, "DeleteSuccess");
        }

        public (object data, string message) Detail(string Id)
        {
            Role role = _repository.Roles.GetById(Id);
            if (role is null)
            {
                return (null, $"RoleNotExist");
            }
            List<string> permissionIds = _repository.RolePermissions.FindByCondition(x => x.RoleId.Equals(Id))
                                                                 .Select(x => x.PermissionCode)
                                                                 .ToList();
            role.Permissions = _repository.Permissions.FindByCondition(x => permissionIds.Contains(x.PermissionCode)).ToList();
            return (role, "Success");
        }

        // Lấy danh sách tất cả module và quyền 
        public (object data, string message) ShowAllRole(UserLevelEnum level,string groupId)
        {
            List<Role> roles = _repository.Roles.FindByCondition(x =>(UserLevelEnum) x.RoleLevel == level).Include(x=>x.RolePermissions).ToList();
            roles = roles.Where(x => string.IsNullOrEmpty(groupId) || (!string.IsNullOrEmpty(x.GroupId) && x.GroupId.Equals(groupId))).ToList();
            List<string> permissionIds = _repository.RolePermissions.FindByCondition(x => (roles.Select(y => y.RoleId).ToList()).Contains(x.RoleId))
                                                                  .Select(x => x.PermissionCode)
                                                                  .ToList();
            foreach (Role role in roles)
            {
                role.Permissions = _repository.Permissions.FindByCondition(x => role.RolePermissions.Select(x=>x.PermissionCode).Contains(x.PermissionCode)).ToList();
            }
            return (roles, "Success");
        }
        public (object data, string message) ShowAllByLevel(UserLevelEnum level)
        {
            List<Role> roles = _repository.Roles.FindByCondition(x =>(UserLevelEnum) x.RoleLevel == level).ToList();
            List<string> permissionIds = _repository.RolePermissions.FindByCondition(x => (roles.Select(y => y.RoleId).ToList()).Contains(x.RoleId))
                                                                  .Select(x => x.PermissionCode)
                                                                  .ToList();
            foreach (Role role in roles)
            {
                role.Permissions = _repository.Permissions.FindByCondition(x => permissionIds.Contains(x.PermissionCode)).ToList();
            }
            return (roles, "Success");
        }

        public (object data, string message) Update(UpdateRoleRequest request, string Id)
        {
            Role role = _repository.Roles.GetById(Id);
            if (role is null)
            {
                return (null, $"RoleNotExist");
            }
            Dictionary<string, string> permissionInLevels = _repository.Permissions.FindByCondition(x => x.Level == role.RoleLevel)
                                                                                  .ToDictionary(x => x.PermissionCode, x => x.PermissionCode);

            foreach (var permission in request.PermissionIds)
            {

                if (!permissionInLevels.ContainsKey(permission))
                {
                    return (null, "PermissionNotExistLevel");
                }
            }
            List<RolePermission> rolePermissionsDB = _repository.RolePermissions.FindByCondition(x => x.RoleId.Equals(Id)).ToList();
            _repository.RolePermissions.RemoveRange(rolePermissionsDB);
            _repository.SaveChanges();

            List<RolePermission> rolePermissions = new List<RolePermission>();
            foreach (string idPermission in request.PermissionIds)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = Id,
                    PermissionCode = idPermission,
                });
            }
            _repository.RolePermissions.AddRange(rolePermissions);
            _repository.SaveChanges();

            return (role, "UpdateSuccess");
        }
    }
}
