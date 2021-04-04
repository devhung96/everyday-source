using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Validations
{
    public class CheckListIdsValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            List<string> mediaIs = value as List<string>;
            if (mediaIs.Count == 0)
                return new ValidationResult("Trường id là bắt buộc.");
            return ValidationResult.Success;
        }
    }
}
