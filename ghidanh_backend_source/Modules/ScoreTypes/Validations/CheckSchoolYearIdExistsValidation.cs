using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Validations
{
    public class CheckSchoolYearIdExistsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool SchoolYearId = db.SchoolYears.FindByCondition(x => x.SchoolYearId.Equals(id)).Any();
            if (SchoolYearId == false)
            {
                return new ValidationResult("SchoolYearIdDoNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
