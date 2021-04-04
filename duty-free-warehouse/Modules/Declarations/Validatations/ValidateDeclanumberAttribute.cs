using Project.App.Database;
using Project.Modules.DeClarations.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Validatations
{
    public class ValidateDeclanumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string key = value as string;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            DeClaration deClaration = _maria.Declarations.FirstOrDefault(x=>x.DeClaNumber.Equals(key));
            if (deClaration != null)
                return new ValidationResult("Số tờ khai đã tồn tại.");
            return ValidationResult.Success;
        }
    }
}
