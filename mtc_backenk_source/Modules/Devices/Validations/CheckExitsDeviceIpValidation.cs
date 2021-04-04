using Project.App.DesignPatterns.Reponsitories;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Project.Modules.Devices.Validations
{
    public class CheckExitsDeviceIpValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _mariaDBContext = (IRepositoryWrapperMariaDB)validationContext.GetService(typeof(IRepositoryWrapperMariaDB));

            if (value is null)
            {
                return new ValidationResult("DeviceIpIsRequired");
            }
            string ip = value.ToString();
            if (string.IsNullOrEmpty(ip))
            {
                return new ValidationResult("LogInNameIsNull");
            }
            var checkIP = _mariaDBContext.Devices.FindByCondition(x => x.LoginName == ip).FirstOrDefault();
            if (checkIP == null)
            {
                return ValidationResult.Success;

            }
            return new ValidationResult("TheDeviceLogInNameAlreadyExists");

        }
    }
}
