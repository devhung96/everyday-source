using Project.App.Database;
using Project.Modules.DeClarations.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Validatations
{
    public class ValidateParentDeClarationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string parentNumber = value as string;
            if (String.IsNullOrEmpty(parentNumber))
                return ValidationResult.Success;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            DeClaration deClaration = _maria.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(parentNumber));
            if (deClaration is null)
                return new ValidationResult("Số tờ khai nhập không tồn tại.");
            return ValidationResult.Success;
        }
    }
}
