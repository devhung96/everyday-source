using Project.App.Databases;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class ExistListClassIdsValidation : ValidationAttribute
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            HashSet<string> classIds = (HashSet<string>)value;
            MariaDBContext dBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            foreach(string classId in classIds)
            {
                if (!dBContext.Classes.Any(c => c.ClassId.Equals(classId)))
                {
                    return new ValidationResult("Contain Class is not exit");
                }
            }
            return ValidationResult.Success;
        }
    }
}
