using Project.App.Database;
using Project.Modules.DeClarations.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Validatations
{
    public class ValidateDeclaNumberExportAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string DeclareNumberExport = value as string;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            DeClaration deClaration = _maria.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(DeclareNumberExport));
            if (deClaration is null)
                return new ValidationResult("Số tờ khai xuất không tồn tại.");
            else
            {
                if (deClaration.DeClaStatus == DeClaStatus.confirm)
                    return new ValidationResult("Tờ khai xuất đã xác nhận.");
            }
            return ValidationResult.Success;
        }
    }
}
