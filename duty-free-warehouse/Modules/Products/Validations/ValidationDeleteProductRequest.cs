using Project.App.Database;
using Project.Modules.Products.Entities;
using Project.Modules.Products.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Validations
{
    public class ValidationDeleteProductRequestAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DeleteProductRequest request = value as DeleteProductRequest;
            if (request is null)
                return new ValidationResult("Mã sản phẩm không thể rỗng");
            return ValidationResult.Success;
        }
    }
}
