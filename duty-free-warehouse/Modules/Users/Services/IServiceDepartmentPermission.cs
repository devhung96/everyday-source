using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Services
{

    public interface IServiceDepartmentPermission
    {
        (object data, string message) CreatePermissionAndDepartment(AddDepartmentPermissionRequest addGroupModule);
        (object data, string message) GetPermissionWithDepartment();
    }

    public class ServiceDepartmentPermission : IServiceDepartmentPermission
    {
        public readonly IConfiguration config;
        public readonly MariaDBContext dBContext;
        public ServiceDepartmentPermission(IConfiguration _config, MariaDBContext _dBContext)
        {
            config = _config;
            dBContext = _dBContext;
        }


        public (object data, string message) CreatePermissionAndDepartment(AddDepartmentPermissionRequest addGroupModule)
        {

            List<DepartmentPermissions> permissionold = dBContext.DepartmentPermissions.Where(x => !addGroupModule.permissioncode.Contains(x.PermissionCode) && x.DepartmentCode == addGroupModule.departmentcode).ToList();
            for (int i = 0; i < addGroupModule.permissioncode.Count; i++)
            {
                DepartmentPermissions groupModule = dBContext.DepartmentPermissions.FirstOrDefault(x => x.PermissionCode == addGroupModule.permissioncode[i] && x.DepartmentCode == addGroupModule.departmentcode);
                if (groupModule != null)
                    continue;
                dBContext.DepartmentPermissions.Add(new DepartmentPermissions()
                {
                    PermissionCode = addGroupModule.permissioncode[i],
                    PermissionID = addGroupModule.permissionid[i],
                    DepartmentCode = addGroupModule.departmentcode,
                    DepartmentID = addGroupModule.departmentid
                });
            }
            dBContext.DepartmentPermissions.RemoveRange(permissionold);
            dBContext.SaveChanges();
            return ("Thành công!!!", "Cập nhật quyền cho bộ phận thành công.!!!");
        }

        public (object data, string message) GetPermissionWithDepartment()
        {
            List<Department> departments = dBContext.Departments.Include(x => x.DepartmentPermissions).ThenInclude<Department, DepartmentPermissions, Permission>(x => x.Permission).ThenInclude(x => x.Module).OrderByDescending(x => x.CreatedAt).ToList();

            var result = departments.Select(x => new
            {
                departmentID = x.ID,
                departmentcode = x.Code,
                departmentname = x.Name,
                permissions = x.DepartmentPermissions.Select(y => new
                {
                    permissionid = y.PermissionID,
                    permissioncode = y.PermissionCode,
                    permissonname = y.Permission.Name,
                    module = new
                    {
                        moduleid = y.Permission.Module.ID,
                        modulecode = y.Permission.Module.Code,
                        modulename = y.Permission.Module.Name
                    }
                }).ToList()
            });

            return (result, "Get permission with department");
        }
    }
}
