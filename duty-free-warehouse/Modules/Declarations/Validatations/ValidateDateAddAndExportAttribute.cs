using Project.Modules.Declarations.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Validatations
{
    public class ValidateDateAddAndExportAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DeclarationStore request = (DeclarationStore)validationContext.ObjectInstance;
            DateTime dateTime;
            if (request.Type == 1)
            {
                if (!String.IsNullOrEmpty(request.Content["dateadded"]?.ToString()))
                {
                    DateTime.TryParse(request.Content["dateadded"].ToString(), out dateTime);
                    if (dateTime < request.DeClaDateReData)
                    {
                        return new ValidationResult("Ngày nhập kho không hợp lệ.");
                    }
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(request.Content["dateexported"]?.ToString()))
                {
                    DateTime.TryParse(request.Content["dateexported"].ToString(), out dateTime);
                    if (dateTime < request.DeClaDateReData)
                    {
                        return new ValidationResult("Ngày xuất kho không hợp lệ.");
                    }
                }
            }
            return ValidationResult.Success;
        }
    }
}
