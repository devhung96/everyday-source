using Project.App.Databases;
using Project.Modules.Accounts.Entities;
using Project.Modules.Lecturers.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Validations
{
    public class ValidateLecturerCodeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string Code = value as string;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Account account = mariaDB.Accounts.FirstOrDefault(m => m.AccountCode.Equals(Code));
            if (account is null)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("LecturerCodeAreadlyExist");
        }
    }
    public class ValidateLecturerEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string Email = value as string;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Lecturer lecturer = mariaDB.Lecturers.FirstOrDefault(m => m.LecturerEmail.Equals(Email));
            if (lecturer is null)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("LecturerEmailAreadlyExist");
        }
    }
    public class ValidateLecturerPhoneAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string Phone = value as string;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            Lecturer lecturer;
            if (Phone.Length==12)
            {
                lecturer= mariaDB.Lecturers.FirstOrDefault(m => m.LecturerPhone.Equals(Phone));
                if(lecturer is null)
                {
                    Phone = Phone.Remove(0, 3);
                    Phone = Phone.Insert(0, "0");
                }
                else
                {
                    return new ValidationResult("LecturerPhoneAreadlyExist");
                }
            }
            lecturer = mariaDB.Lecturers.FirstOrDefault(m => m.LecturerPhone.Equals(Phone));
            if (lecturer is null)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("LecturerPhoneAreadlyExist");
        }
    }
}
