using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Validations
{
    public class ValidatePhoneNumber: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string phoneNumber = value as string;
            if(String.IsNullOrEmpty(phoneNumber))
            {
                return ValidationResult.Success;
            }
            Regex phone = new Regex(@"(0)+([0-9]{9})");
            if(!( phone.IsMatch(phoneNumber)))
            {
                return new ValidationResult("InValid");
            }
            return ValidationResult.Success;
        }
    }
}
