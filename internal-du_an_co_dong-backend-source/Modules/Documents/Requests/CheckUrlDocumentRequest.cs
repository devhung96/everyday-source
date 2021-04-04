using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Documents.Requests
{
    public class CheckUrlDocumentRequest : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string url = value as string;
            Uri uriResult;
            Uri.TryCreate(url, UriKind.Absolute, out uriResult);
            if (uriResult != null && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Đây không phải là đường dẫn");
        }
    }
}
