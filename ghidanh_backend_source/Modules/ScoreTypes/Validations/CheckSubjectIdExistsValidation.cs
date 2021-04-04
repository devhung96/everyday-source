using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Validations
{
    public class CheckSubjectIdExistsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool SubjectId = db.Subjects.FindByCondition(x => x.SubjectId.Equals(id)).Any();
            if (SubjectId == false)
            {
                return new ValidationResult("SubjectIdDoNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
