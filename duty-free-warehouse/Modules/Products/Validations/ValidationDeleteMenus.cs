
using Project.App.Database;
using Project.Modules.Products.Entities;
using Project.Modules.Products.Requests;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Products.Validations
{
    public class ValidationDeleteMenusAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DeleteMenusRequest request = value as DeleteMenusRequest;
            if (request is null)
            {
                return new ValidationResult("Menu không thể rỗng");
            }

            var Maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            foreach(int item in request.MenuIds)
            {
                MenuDetail menu = Maria.MenuDetails.Find(item);
                if (menu is null)
                {
                    return new ValidationResult("MenuId không tồn tại.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
