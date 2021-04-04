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
    public class ValidAddPermissionAndDepartmentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MariaDBContext _mariaDBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            AddDepartmentPermissionRequest change = (AddDepartmentPermissionRequest)value;


            if (change.departmentcode != null && change.permissioncode != null)
            {
                Department userType = _mariaDBContext.Departments.FirstOrDefault(x => x.Code == change.departmentcode);
                if (userType == null)
                    return new ValidationResult($"Bộ phận với mã {change.departmentcode} không tồn tại!!!");
                foreach (var item in change.permissioncode)
                {
                    Permission permission = _mariaDBContext.Permissions.FirstOrDefault(x => x.Code == item);
                    if (permission == null)
                        return new ValidationResult($"Quyền với mã {item} không tồn tại!!!");
                    change.permissionid.Add(permission.ID);
                }


                change.departmentid = userType.ID;
            }
            return ValidationResult.Success;

        }
    }
}
