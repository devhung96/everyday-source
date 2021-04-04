using Project.App.Database;
using Project.Modules.Products.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Validations
{
    public class ValidateProductCodeAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string Code = value as string;
            var Maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Product product = Maria.Products.Find(Code);
            if (!(product is null))
                return new ValidationResult("Mã sản phẩm đã tồn tại.");
            return ValidationResult.Success;
        }
    }
}
