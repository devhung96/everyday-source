using Project.App.Database;
using Project.Modules.Declarations.Requests;
using Project.Modules.DeClarations.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Declarations.Validatations
{
    public class ValidateDateExport  : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            StoreExportDetail request = (StoreExportDetail)validationContext.ObjectInstance;
            DateTime dateTime;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            DeClaration deClaration = _maria.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(request.DeClaNumberExport));
            if (!String.IsNullOrEmpty(request.DateExported))
            {
                dateTime = DateTime.ParseExact(request.DateExported, "dd/MM/yyyy", null);
                if (dateTime < deClaration.DeClaDateReData)
                {
                    return new ValidationResult("Ngày xuất kho không hợp lệ.");
                }
            }
            return ValidationResult.Success;
        }
    }
}