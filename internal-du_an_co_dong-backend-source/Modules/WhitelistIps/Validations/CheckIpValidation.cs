using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.WhitelistIps.Validations
{
    public class CheckIpValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string ipAddress = value as string;
            if (!ipAddress.ValidateIPv4())
            {
                return new ValidationResult("IPv4 Is Invalid");
            }
            return ValidationResult.Success;
        }
    }
}
