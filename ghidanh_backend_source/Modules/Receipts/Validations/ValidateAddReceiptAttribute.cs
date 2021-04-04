using Project.App.Databases;
using Project.Modules.Receipts.Requests;
using Project.Modules.Students.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.Modules.Receipts
{
    public class ValidateAddReceiptAttribute: ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            ReceiptRequest request = value as ReceiptRequest;
            MariaDBContext mariaDB = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            RegistrationStudy registration = mariaDB.RegistrationStudies.FirstOrDefault(m => m.ClassId.Equals(request.ClassId) && m.StudentId.Equals(request.StudentId));
            return registration != null ? ValidationResult.Success : new ValidationResult("StudentNotExistInClass");
        }
    }
}
