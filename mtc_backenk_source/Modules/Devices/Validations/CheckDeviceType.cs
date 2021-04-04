using Project.App.DesignPatterns.Reponsitories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Validations
{
    public class CheckDeviceType : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string DeviceTypeID = (string)value;
            if(DeviceTypeID is null)
            {
                return ValidationResult.Success;
            }
            var _mariaDBContext = (IRepositoryWrapperMariaDB)validationContext.GetService(typeof(IRepositoryWrapperMariaDB));
            var deviceType = _mariaDBContext.DeviceTypes.FindByCondition(x => x.DeviceTypeId == DeviceTypeID).FirstOrDefault();
            if(deviceType is null)
            {
                return new ValidationResult("DeviceTypeID not exists.");
            }
            return ValidationResult.Success;
        }
    }
}
