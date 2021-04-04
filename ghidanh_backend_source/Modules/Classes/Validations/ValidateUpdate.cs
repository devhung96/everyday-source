using Project.App.DesignPatterns.Repositories;
using Project.Modules.Classes.Entities;
using Project.Modules.Classes.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Classes.Validations
{
    public class ValidateUpdate : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            UpdateClassRequest request = (UpdateClassRequest)value;
            Regex regex = new Regex("^[a-zA-Z0-9 ]*$");
            if (!(regex.IsMatch(request.ClassCode)))
                return new ValidationResult("ClassCodeInValid");
            IRepositoryMariaWrapper repositoryWrapper = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            if (repositoryWrapper.Classes.FirstOrDefault(x => x.ClassId.Equals(request.ClassID)) is null)
            {
                return new ValidationResult("ClassNotFound");
            }
            if (repositoryWrapper.Classes.FirstOrDefault(x => x.ClassCode.Equals(request.ClassCode) && !x.ClassId.Equals(request.ClassID)) != null)
            {
                return new ValidationResult("ClassCodeAlreadyExist");
            }
            if (repositoryWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(request.SchoolYearID)) is null)
            {
                return new ValidationResult("SchoolYearIDNotFound");
            }
            if (repositoryWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(request.CourseID)) is null)
            {
                return new ValidationResult("CourseIDNotFound");
            }
            if (request.ClassQuantityStudent.HasValue)
            {
                int currentQuantity = repositoryWrapper.RegistrationStudies.FindByCondition(x => x.ClassId.Equals(request.ClassID)).Count();
                if(request.ClassQuantityStudent.Value < 0 || request.ClassQuantityStudent.Value > int.MaxValue || request.ClassQuantityStudent.Value < currentQuantity)
                {
                    return new ValidationResult("ClassQuantityStudentInvalid");
                }
            }
            if(request.ClassAmount.HasValue)
            {
                if (request.ClassAmount.Value < 0 || request.ClassAmount.Value > double.MaxValue)
                {
                    return new ValidationResult("ClassAmountInvalid");
                }
            }
            if (request.SurchargeData != null && request.SurchargeData.Count > 0 && !request.SurchargeData.Any(x => x.CheckAmount))
            {
                return new ValidationResult("SurchargeAmountInvalid");
            }
            return ValidationResult.Success;
        }
    }
}
