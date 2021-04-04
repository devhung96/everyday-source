using Project.App.Database;
using Project.Modules.PermissionGroups.Requests;
using Project.Modules.PermissionGroups.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Project.Modules.Permissions.Entities;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Project.Modules.PermissionGroups.Services
{
    public interface IPermissionGroupServices
    {
        (List<PermissionGroup> permissionGroups, string message) Add(AddRequest data);
        PermissionGroup Delete(int PermissionId);
        List<PermissionGroup> GetGroups(int groupId);
    }
    public class PermissionGroupServices: IPermissionGroupServices
    {
        private readonly MariaDBContext mariaContext;
        public PermissionGroupServices ( MariaDBContext _mariaDBContext)
        {
            mariaContext = _mariaDBContext;
        }
        public (List<PermissionGroup> permissionGroups, string message) Add(AddRequest data)
        {
            List<PermissionGroup> permissionGroups= new List<PermissionGroup>();
            Permissions.Entities.PermissionOrganize permission;
            PermissionGroup permissionGroup;
            foreach (int pemissionId in data.PermissionIds)
            {
                 permission = mariaContext.Permissions
                                                .Where(m => m.PermissionId == pemissionId)
                                                .FirstOrDefault();
                if (permission is null)
                    return (null, "PermissionNotExist");
                 permissionGroup = new PermissionGroup()
                                    {
                                        GroupId = data.GroupId,
                                        PermissionCode = permission.PermissionCode,
                                        PermissionId = permission.PermissionId
                                    };
                permissionGroups.Add(permissionGroup);
            }    
            
            mariaContext.PermissionGroups.AddRange(permissionGroups);
            mariaContext.SaveChanges();
            return (permissionGroups, "success");
        }
        public PermissionGroup Delete(int PermissionGroupId)
        {
            PermissionGroup permissionGroup = new PermissionGroup();
            permissionGroup = mariaContext.PermissionGroups.Where(m=>m.PermissionGroupId==PermissionGroupId).FirstOrDefault();
            if(permissionGroup is null)
            {
                return permissionGroup;
            }
            mariaContext.PermissionGroups.Remove(permissionGroup);
            mariaContext.SaveChanges();
            return permissionGroup;
        }
        public List<PermissionGroup> GetGroups(int groupId)
        {
            List<PermissionGroup> lst = new List<PermissionGroup>();
            lst = mariaContext.PermissionGroups.Where(m => m.GroupId == groupId).ToList();
            return lst;
        }
     
    }
}
