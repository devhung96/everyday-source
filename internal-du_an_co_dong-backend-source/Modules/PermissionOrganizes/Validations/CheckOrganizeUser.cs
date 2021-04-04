
using Project.App.Database;
using Project.Modules.Organizes.Entities;
using Project.Modules.PermissionOrganizes.Requests;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PermissionOrganizes.Validations
{
    public class CheckOrganizeUser :ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var request = value as PermissionOrganizeByUserRequest;
            MariaDBContext mariaDBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            User user = mariaDBContext.Users.Where(m=>m.UserId.Equals(request.UserId)).FirstOrDefault() ;
            if (user is null)
            {
                return new ValidationResult("UserNotExist");
            }
            Organize organize = mariaDBContext.Organizes.Where(m => m.OrganizeId.Equals(request.OrganizeId) && m.OrganizeStatus == ORGANIZE_STATUS.ACTIVED).FirstOrDefault();
            if(organize is null )
            {
                return new ValidationResult("OrganizeNotExist");

            }
            return ValidationResult.Success;
        }
    }
}
