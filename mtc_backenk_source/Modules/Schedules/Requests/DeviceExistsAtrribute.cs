//using Project.App.Databases;
//using Project.Modules.Devices.Entities;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Project.Modules.Schedules.Requests
//{
//    public class DeviceExistsAtrribute : ValidationAttribute
//    {
//        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        {
//            MariaDBContext _context = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
//            var idDevice = (int)value;
//            Device device = _context.Devices.Find(idDevice);
//            if (device == null) return new ValidationResult(GetErrorMessage(idDevice));
//            return ValidationResult.Success;
//        }

//        public string GetErrorMessage(int idDevice)
//        {
//            return $"Device not exists";
//        }
//    }
//}
