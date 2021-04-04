using Project.App.DesignPatterns.Repositories;
using Project.Modules.Classes.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Classes.Validations
{
    public class ValidateStore : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            StoreClassRequest request = (StoreClassRequest)value;
            Regex regex = new Regex("^[a-zA-Z0-9 ]*$");
            if (!(regex.IsMatch(request.ClassCode)))
                return new ValidationResult("ClassCodeInValid");
            IRepositoryMariaWrapper reponsitoryWrapper = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            if (reponsitoryWrapper.Classes.FirstOrDefault(x => x.ClassCode.Equals(request.ClassCode)) != null)
            {
                return new ValidationResult("ClassAlreadyExist");
            }
            if (request.ClassQuantityStudent.Value < 1)
            {
                return new ValidationResult("QuantityIsValid");
            }
            if (reponsitoryWrapper.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(request.SchoolYearID)) is null)
            {
                return new ValidationResult("SchoolYearIDNotFound");
            }
            if (reponsitoryWrapper.Courses.FirstOrDefault(x => x.CourseId.Equals(request.CourseID)) is null)
            {
                return new ValidationResult("CourseIDNotFound");
            }
            if(request.ClassAmount.Value  < 0 || request.ClassAmount.Value > double.MaxValue)
            {
                return new ValidationResult("ClassAmountInvalid");
            }
            if (request.SurchargeData != null && request.SurchargeData.Count > 0 && !request.SurchargeData.Any(x => x.CheckAmount))
            {
                return new ValidationResult("SurchargeAmountInvalid");
            }
            return ValidationResult.Success;
        }
    }
}
