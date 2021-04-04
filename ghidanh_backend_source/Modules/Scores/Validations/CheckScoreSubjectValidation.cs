using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Scores.Validations
{
    public class CheckScoreSubjectValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<double?> scoreSubject = value as List<double?>;
            bool scoreSubjectCheck = scoreSubject.Any(x => x < 0 || x > 10);
            if (scoreSubjectCheck == true)
            {
                return new ValidationResult("scoreSubject0between10");
            }
            return ValidationResult.Success;
        }
    }
}
