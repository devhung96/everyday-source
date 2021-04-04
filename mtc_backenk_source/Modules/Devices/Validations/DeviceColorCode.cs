using Project.App.DesignPatterns.Reponsitories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Validations
{
    public class DeviceColorCode : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _mariaDBContext = (IRepositoryWrapperMariaDB)validationContext.GetService(typeof(IRepositoryWrapperMariaDB));
            string colorCode = value.ToString();
            if(colorCode is null)
            {
                return new ValidationResult("Color Code is required !!");
            }
            
            var checkColorCode = _mariaDBContext.Devices.FindByCondition(x => x.ColorCode == colorCode).FirstOrDefault();
            if(checkColorCode != null)
            {
                return new ValidationResult("Color Code is exists !!");
            }
            return ValidationResult.Success;
        }
    }
}
