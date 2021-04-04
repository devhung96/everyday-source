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
    public class ValidationAddMenuAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            AddMenuRequest data = value as AddMenuRequest;

            var maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var menu = maria.Menus.Where(m => m.MenuID == data.MenuId).FirstOrDefault();
            if (menu is null)
            {
                return new ValidationResult("Mã menu không tồn tại.");
            }

            var product = maria.Products.Where(m => m.ProductCode.Equals(data.ProductCode) && m.ProductStatus == 1).FirstOrDefault();
            if (product is null)
            {
                return new ValidationResult("Sản phẩm không tồn tại.");
            }

            if (!(maria.MenuDetails.Where(m => m.ProductCode.Equals(data.ProductCode) && m.MenuId == data.MenuId).FirstOrDefault() is null))
            {
                return new ValidationResult("Sản phẩm đã tồn tại trong Menu.");
            }

            return ValidationResult.Success;
        }
    }
}
