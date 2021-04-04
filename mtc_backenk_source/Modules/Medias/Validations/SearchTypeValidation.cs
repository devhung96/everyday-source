using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Validations
{
    public class SearchTypeValidation : ValidationAttribute
    {
        private readonly string[] searchTypeAccepted = new[] { "Default", "KeyWord", "ByTime" };
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string searchType = value as string;
            if (!searchTypeAccepted.Any(x => x.Equals(value)))
            {
                return new ValidationResult("Search type is not supported.");
            }
            return ValidationResult.Success;
        }
    }
}
