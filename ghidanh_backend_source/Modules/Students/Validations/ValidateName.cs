using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Students.Validations
{
    public class ValidateNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string name = value as string;
            if(string.IsNullOrEmpty(name))
            {
                return ValidationResult.Success;
            }    
            string characterSpecial = "0123456789.+/*-~!@#$%^&*(){}|:;,?><?";
            for (int i= 0 ; i < name.Length;i++)
            {
                if (characterSpecial.Contains(name[i].ToString()))
                {
                    return new ValidationResult("NameInvalid");
                }
            }
            return ValidationResult.Success;
        }
    }
    public class ValidateLastNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string name = value as string;
            if (string.IsNullOrEmpty(name))
            {
                return ValidationResult.Success;
            }
            string characterSpecial = "0123456789.+/*-~!@#$%^&*(){}|:;,?><?";
            for (int i = 0; i < name.Length; i++)
            {
                if (characterSpecial.Contains(name[i].ToString()))
                {
                    return new ValidationResult("LastNameInvalid");
                }
            }
            return ValidationResult.Success;
        }
    }
        public class ValidateFistNameAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string name = value as string;
            if(string.IsNullOrEmpty(name))
            {
                return ValidationResult.Success;
            }    
            string characterSpecial = "0123456789.+/*-~!@#$%^&*(){}|:;,?><?";
            for (int i= 0 ; i < name.Length;i++)
            {
                if (characterSpecial.Contains(name[i].ToString()))
                {
                    return new ValidationResult("FistNameInvalid");
                }
            }
            return ValidationResult.Success;
        }
    }
}
