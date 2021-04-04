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
    public class ValidateDateAdded : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            StoreDetail request = (StoreDetail)validationContext.ObjectInstance;
            DateTime dateTime;
            var _maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            DeClaration deClaration = _maria.Declarations.FirstOrDefault(x => x.DeClaNumber.Equals(request.DeClaNumber));
            if (deClaration is null)
                return new ValidationResult("Số tờ khai nhập không tồn tại.");
            if (!String.IsNullOrEmpty(request.DateAdded))
            {
                dateTime = DateTime.ParseExact(request.DateAdded, "dd/MM/yyyy", null);
                if (dateTime < deClaration.DeClaDateReData)
                {
                    return new ValidationResult("Ngày nhập kho không hợp lệ.");
                }
            }
            return ValidationResult.Success;
        }
    }
}