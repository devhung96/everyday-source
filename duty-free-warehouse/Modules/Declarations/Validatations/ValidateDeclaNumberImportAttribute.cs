using Project.App.Database;
using Project.Modules.DeClarations.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Validatations
{
    public class ValidateDeclaNumberImportAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string DeclareNumberImport = value as string;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            DeClaration deClaration = _maria.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(DeclareNumberImport));
            if (deClaration is null)
                return new ValidationResult("Số tờ khai nhập không tồn tại.");
            else
            {
                if(deClaration.DeClaStatus == DeClaStatus.unconfimred)
                    return new ValidationResult("Tờ khai nhập đã xác nhận.");
            }
            return ValidationResult.Success;
        }
    }
}
