using Project.App.DesignPatterns.Repositories;
using Project.Modules.ScoreTypes.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.ScoreTypes.Validations
{
    public class CheckCreateSocreTypeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CreateScoreTypeRequest request = value as CreateScoreTypeRequest;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            if (db.ScoreTypes.Any(x=>x.ScoreTypeName.Equals(request.ScoreTypeName)))
            {
                return new ValidationResult("ScoreTypeNameDuplicate");
            }
            return ValidationResult.Success;
        }
    }
}
