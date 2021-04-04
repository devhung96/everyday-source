using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Validations
{
    public class ValidateEvent : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            string eventId = value as string;
            MariaDBContext maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var _event = maria.Events.Find(eventId);
            if (_event is null)
            {
                return new ValidationResult("NotExist");
            }
            return ValidationResult.Success;
        }
    }
}