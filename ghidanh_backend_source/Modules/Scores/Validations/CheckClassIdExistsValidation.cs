using Project.App.DesignPatterns.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class CheckClassIdExistsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string id = value as string;
            var db = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            bool ClassId = db.Classes.FindByCondition(x => x.ClassId.Equals(id)).Any();
            if (ClassId == false)
            {
                return new ValidationResult("ClassIdDoNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
