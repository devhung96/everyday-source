using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Organizes.Validations
{
    public class CheckUrlAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
           
            if (value is null)
            {
                return ValidationResult.Success;
            }

            var url = value as string;
            Regex urlRx = new Regex(@"^(http|ftp|https|www)://([\w+?\.\w+])+([a-zA-Z0-9\~\!\@\#\$\%\^\&\*\(\)_\-\=\+\\\/\?\.\:\;\'\,]*)?$", RegexOptions.IgnoreCase);
            Match match = urlRx.Match(url);
            if (!match.Success)
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"ThePathIsNotCorrect";
        }
    }
}
