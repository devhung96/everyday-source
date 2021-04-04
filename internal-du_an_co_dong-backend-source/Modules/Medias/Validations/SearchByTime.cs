using Project.Modules.Medias.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Validations
{
    public class SearchByTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string searchContent = value as string;
            SearchRequest searchRequest = (SearchRequest)validationContext.ObjectInstance;
            if (searchRequest.SearchType.Equals("ByTime") && !DateTime.TryParse(searchContent, out _))
                return new ValidationResult("TimeFormatIsInvalid");
            return ValidationResult.Success;
        }
    }
}
