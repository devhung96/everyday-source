using Project.App.Databases;
using Project.Modules.SchoolYears.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Semesters.Validation
{
    public class SchoolYearExistValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("SchoolYearIdRequired");
            }
            string SchoolYearId = value.ToString();
            MariaDBContext mariaDBContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            SchoolYear schoolYears = mariaDBContext.SchoolYears.FirstOrDefault(x => x.SchoolYearId.Equals(SchoolYearId));
            if (schoolYears is null)
            {
                return new ValidationResult("SchoolYearNotFound");
            }
            return ValidationResult.Success;
        }
    }
}
