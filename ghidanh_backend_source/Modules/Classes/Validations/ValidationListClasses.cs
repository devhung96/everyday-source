using Project.Modules.Classes.Requests;
using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Classes.Validations
{
    public class ValidationListClasses : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ListClassesRequest request = (ListClassesRequest)value;
            if (request.Type == 1 && (String.IsNullOrEmpty(request.SortOrder) || String.IsNullOrEmpty(request.SortField)))
            {
                return new ValidationResult("RequestTableIsRequired");
            }
            return ValidationResult.Success;
        }
    }
}
