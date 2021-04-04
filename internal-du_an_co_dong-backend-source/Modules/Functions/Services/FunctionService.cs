using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Services.Common;
using Project.App.Database;
using Project.Modules.Functions.Entities;
using Project.Modules.Functions.Requests;
using Project.Modules.Groups.Entities;
using Project.Modules.Permissions.Entities;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Functions.Services
{
    public interface IFunctionService
    {
        List<Module> ShowAllFunction();
        public object ShowAllPermisson();

        public object ShowAllPermissonByGroup(int idGroup);
    }
    public class FunctionService : IFunctionService
    {
        private readonly MariaDBContext mariaDBContext;
        public FunctionService(MariaDBContext mariaDBContext)
        {
            this.mariaDBContext = mariaDBContext;
        }
        public List<Module> ShowAllFunction()
        {
            List<Module> modules = mariaDBContext.Modules.Include(m=>m.Functions).ThenInclude(m=>m.Permissions).ToList();
           
             return modules;
        }
        public object ShowAllPermisson()
        {
            List<PermissionType> permissionTypes = mariaDBContext.PermissionTypes.ToList();
            List<Module> modules = mariaDBContext.Modules.Include(m => m.Functions).ThenInclude(m => m.Permissions).ToList();
            return new { headers = permissionTypes, pemissionGroups = modules } ;
        }

        public object ShowAllPermissonByGroup(int idGroup)
        {
            List<PermissionType> permissionTypes = mariaDBContext.PermissionTypes.ToList();
            Group group = mariaDBContext.Groups.FirstOrDefault(m => m.GroupID.Equals(idGroup));
            if(group is null)
            {
                return null;
            }    
            List<int> idPermissons = mariaDBContext.PermissionGroups.Where(x => x.GroupId == idGroup).Select(x => x.PermissionId).ToList();
            List<Module> modules = mariaDBContext.Modules.Include(m => m.Functions).ThenInclude(m => m.Permissions).ToList();
            foreach (var module in modules)
            {
                foreach (var fn in module.Functions)
                {
                    var tmp = fn.Permissions.Where(x => idPermissons.Contains(x.PermissionId)).ToList();
                    tmp.ForEach(x=> x.PermissionFlag = true);
                }
            }
            return new {group, headers = permissionTypes, pemissionGroups = modules };
        }

    }
}
