using Microsoft.EntityFrameworkCore;
using Project.App.Database;
using Project.Modules.Events.Entities;
using Project.Modules.Groups.Entities;
using Project.Modules.Organizes.Requests;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Validations
{
    public class ValidateAddUserEventRequest :ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var data = value as AddUserToEventRequest;
            MariaDBContext mariaDBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Group group = mariaDBContext.Groups.Where(m => m.GroupID == data.GroupId).FirstOrDefault();
            if (group is null)
            {
                return new ValidationResult("Vai trò không tồn tại.");
            }
            Event _event = mariaDBContext.Events.Where(m => m.EventId.Equals(data.EventId)).Include(m=>m.Organize).FirstOrDefault();
            if (_event is null)
            {
                return new ValidationResult("Sự kiện không tồn tại.");
            }
            if(_event.Organize is null)
            {

                return new ValidationResult("Tổ chức không tồn tại.");
            }
            if(data.UserStock > int.MaxValue )
            {
                return new ValidationResult("Số cổ phần là số nguyên kiểu Int32.");
            }    
            return ValidationResult.Success;
        }
    }
}
