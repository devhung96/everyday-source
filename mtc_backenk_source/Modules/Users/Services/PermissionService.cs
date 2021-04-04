using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Users.Services
{
    public interface IPermissionService
    {
        (object data, string message) CreatePermission(CreatePermissionRequest newPermission);

        (object data, string message) ShowAllPermission(UserLevelEnum level);
    }
    public class PermissionService: IPermissionService
    {
        private readonly IRepositoryWrapperMariaDB  repository;
        private readonly IMapper mapper;
        public PermissionService(IRepositoryWrapperMariaDB mariaDBContext, IMapper mapper)
        {
            repository = mariaDBContext;
            this.mapper = mapper;
        }

        public (object data, string message) CreatePermission(CreatePermissionRequest newPermission)
        {
            Permission permission = repository.Permissions.FindByCondition(x => x.PermissionCode == newPermission.PermissionCode)
                                                          .FirstOrDefault();

            if (permission != null)
            {
                return (null, "PermissionAlreadyExists");
            }

            Permission permissionNew = mapper.Map<Permission>(newPermission); 
            repository.Permissions.Add(permissionNew);
            repository.SaveChanges();
            return (newPermission, "Success");
        }

        public (object data, string message) ShowAllPermission(UserLevelEnum level)
        {
            List<Permission> Permissions = repository.Permissions.FindByCondition(x => (UserLevelEnum)x.Level == level)
                                                                 .ToList(); 
            return (Permissions, "Success");
        }
    }
}
