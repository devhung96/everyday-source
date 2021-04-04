using Project.App.Database;
using Project.Modules.PermissonUsers.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project.Modules.PermissonUsers.Services
{
    public interface IPerUserServices
    {
        bool TextPermission(AddPerUserRequest data);
        PermissionUser Save(AddPerUserRequest data);
        PermissionUser Delete(int ID);
        List<PermissionUser> List(int UserID);
        List<PermissionUser> ListData();
    }
    public class PerUserServices:IPerUserServices
    {
        private readonly MariaDBContext mariaDBContext;
        public PerUserServices( MariaDBContext _mariaDBContext)
        {
            mariaDBContext = _mariaDBContext;
        }
        public bool TextPermission(AddPerUserRequest data)
        {
            bool  text;// text = true => đã tồn tại
            PermissionUser permissionUser = mariaDBContext.PermissionUsers.Where(m => m.UserId.Equals( data.UserID) && m.PermissionCode == data.PermissionCode).FirstOrDefault();
            _ = permissionUser is null ? text = false: text=true
                ;
            return text;
        }
        public PermissionUser Save(AddPerUserRequest data) {

            PermissionUser permissionUser = new PermissionUser() {UserId = data.UserID, PermissionCode = data.PermissionCode };
            mariaDBContext.PermissionUsers.Add(permissionUser);
            mariaDBContext.SaveChanges();
            return permissionUser;
        }
        public PermissionUser Delete(int ID)
        {
            PermissionUser permissionUser = mariaDBContext.PermissionUsers.Find(ID);
            mariaDBContext.PermissionUsers.Remove(permissionUser);
            mariaDBContext.SaveChanges();
            return permissionUser;
        }
        public List<PermissionUser> List(int UserID)
        {
            List < PermissionUser > lst = mariaDBContext.PermissionUsers.Where(m => m.UserId.Equals( UserID)).ToList();
            return lst;
        }
        public List<PermissionUser> ListData()
        {
            List<PermissionUser> lst = mariaDBContext.PermissionUsers.ToList();
            return lst;
        }
    }
}
