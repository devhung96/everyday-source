using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Validations
{
    public class CheckScoreTypeIdExistsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool scoreTypeId = db.ScoreTypes.FindByCondition(x => x.ScoreTypeId.Equals(id)).Any();
            if (scoreTypeId == false)
            {
                return new ValidationResult("ScoreTypeIdDoNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
