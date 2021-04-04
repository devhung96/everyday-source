using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Sessions.Validations
{
    public class CheckEventExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string eventId = (string)value;

            if (String.IsNullOrEmpty(eventId))
            {
                return ValidationResult.Success;
            }

            var _mariaDb = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var Event = _mariaDb.Events.FirstOrDefault(x => x.EventId == eventId);
            if (Event is null)
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"EventNotFound";
        }
    }
}
