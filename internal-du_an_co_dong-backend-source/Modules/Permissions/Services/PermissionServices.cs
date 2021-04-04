using Project.App.Database;
using Project.Modules.Permissions.Entities;
using Project.Modules.Permissions.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Permissions.Services
{
    public interface IPermissionServices
    {
        Entities.PermissionOrganize Addpermission(AddPermission data);
        Entities.PermissionOrganize Deletepermission(string Code);
        List<Entities.PermissionOrganize> List();
    }
    public class PermissionServices:IPermissionServices
    {
        private readonly MariaDBContext mariaDBContext;
        public PermissionServices(MariaDBContext _mariaDBContext)
        {
            mariaDBContext = _mariaDBContext;
        }
        public Entities.PermissionOrganize Addpermission(AddPermission data)
        {
            Entities.PermissionOrganize permission = new Entities.PermissionOrganize() { PermissionCode= data.PermissionCode, PermissionName = data.PermissionName} ;
            mariaDBContext.Permissions.Add(permission);
            mariaDBContext.SaveChanges();
            return permission;
        }
        public Entities.PermissionOrganize Deletepermission(string Code)
        {
            Entities.PermissionOrganize permission = mariaDBContext.Permissions.Where(m => m.PermissionCode == Code).FirstOrDefault();
            if(permission is null)
            {
                return null;
            }    
            mariaDBContext.Remove(permission);
            return permission;
        }
        public List<Entities.PermissionOrganize> List()
        {
            List<Entities.PermissionOrganize> lst = new List<Entities.PermissionOrganize>();
            lst = mariaDBContext.Permissions.ToList();
            return lst;
        }
    }
}
