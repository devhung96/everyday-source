using Project.Modules.Events.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Validations
{
    public class AllowedExtensionsUrl : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsUrl(string[] Extensions)
        {
            _extensions = Extensions;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = value as string;
            if (String.IsNullOrEmpty(file))
            {
                return ValidationResult.Success;
            }

            var pathLocal = file.GetLocalPathUrl();
            string extension = pathLocal.GetExtensionFile();

            if (!_extensions.Contains(extension.ToLower()))
            {
                return new ValidationResult(GetErrorMessage());
            }
            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"TheFileIsNotInTheCorrectFormat";
        }

    }
}
