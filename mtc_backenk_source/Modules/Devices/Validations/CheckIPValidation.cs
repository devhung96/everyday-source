using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Validations
{
    public class CheckIPValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string ip = value.ToString();
            if (ip is null)
            {
                return new ValidationResult("Device Ip is required !!");
            }
            const string regexPattern = @"^([\d]{1,3}\.){3}[\d]{1,3}$";
            var regex = new Regex(regexPattern);
            if (string.IsNullOrEmpty(ip))
            {
                return new ValidationResult("IP address  is null");
            }
            if (!regex.IsMatch(ip) || ip.Split('.').SingleOrDefault(s => int.Parse(s) > 255) != null)
            {
                return new ValidationResult("Invalid IP Address");
            }
            return ValidationResult.Success;
        }
    }
}
