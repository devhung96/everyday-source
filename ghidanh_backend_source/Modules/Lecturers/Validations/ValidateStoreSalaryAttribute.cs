using Project.App.DesignPatterns.Repositories;
using Project.Modules.Lecturers.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Lecturers.Validations
{
    public class ValidateStoreSalaryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            StoreSalary request = value as StoreSalary;
            IRepositoryMariaWrapper repositoryWrapper = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            List<string> listClassId = repositoryWrapper.Classes.FindByCondition(x => x.CourseID.Equals(request.CourseId)).Select(x => x.ClassId).ToList();
            if (listClassId.Count < 1)
            {
                return new ValidationResult("NoClassesExistInThisCourse");
            }
            List<string> classIds = repositoryWrapper.ClassSchedules.FindByCondition(x => listClassId.Contains(x.ClassId) && x.LecturerId.Equals(request.LectureId)).Select(x => x.ClassId).Distinct().ToList();
            if (classIds.Count < 1)
            {
                return new ValidationResult("ThisTeacherIsNotInChargeOfAnyClass");
            }
            var aa = repositoryWrapper.Receipts.FindByCondition(x => classIds.Contains(x.ClassId)).Sum(x => x.TypeAmount);
            if (request.BaseSalary != repositoryWrapper.Receipts.FindByCondition(x => classIds.Contains(x.ClassId)).Sum(x => x.TypeAmount))
            {
                return new ValidationResult("BaseSalaryInvalid");
            }
            if(request.Percent.Value < 0 || request.Percent.Value > 100)
            {
                return new ValidationResult("PercentInvalid");
            }
            if(request.TotalSalary.Value > request.BaseSalary.Value)
            {
                return new ValidationResult("TotalSalaryInvalid");
            }
            return ValidationResult.Success;
        }
    }
}
