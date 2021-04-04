using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class CheckSemestersIdExistsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool semestersId = db.Semesters.FindByCondition(x => x.SemesterId.Equals(id)).Any();
            if (semestersId == false)
            {
                return new ValidationResult("SemestersIdDoNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
