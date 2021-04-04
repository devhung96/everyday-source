using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class ValidationEmail: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value as string;
            if (String.IsNullOrEmpty(email))
                return ValidationResult.Success;
            Regex mail = new Regex(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            if (!(mail.IsMatch(email)))
                return new ValidationResult("InValid");
            return ValidationResult.Success;
        }
    }
}
