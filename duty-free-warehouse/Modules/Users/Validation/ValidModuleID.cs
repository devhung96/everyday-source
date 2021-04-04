using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validation
{
    public class ValidModuleIDAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult("Trường mã module không được rỗng!!!");
            MariaDBContext _mariaDBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            if (!_mariaDBContext.Modules.Any(x => x.Code == value.ToString()))
                return new ValidationResult($"Mã module {value.ToString()} không tồn tại!!!");
            return ValidationResult.Success;
        }
    }
}
