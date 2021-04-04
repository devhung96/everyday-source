using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Validations
{
    public class ValidationDateTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string datetime = value as string;

            if(!DateTime.TryParseExact(datetime, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                return new ValidationResult(" truyền vào không hợp lệ");
            }
           return ValidationResult.Success;

        }
    }
}