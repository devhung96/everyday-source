using Project.App.Database;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validation
{
    public class ValidUpdatePermissionUserAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MariaDBContext _mariaDBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            UpdatePermissionUser change = (UpdatePermissionUser)value;


            if (change.UserID != null && change.PermissionCode != null)
            {
                User userType = _mariaDBContext.Users.FirstOrDefault(x => x.UserId == change.UserID);
                if (userType == null)
                    return new ValidationResult($"Người dùng không tồn tại!!!");
                foreach (var item in change.PermissionCode)
                {
                    Permission permission = _mariaDBContext.Permissions.FirstOrDefault(x => x.Code == item);
                    if (permission == null)
                        return new ValidationResult($"Quyền với mã code {item} không tồn tại!!!");
                    change.PermissionID.Add(permission.ID);
                }
            }
            return ValidationResult.Success;
        }
    }
}
