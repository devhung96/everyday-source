using Newtonsoft.Json;
using Project.Modules.Question.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Validation
{
    public class CheckListMedia : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }
            try
            {
                //var listMedia = JsonConvert.DeserializeObject<List<MediaCustome>>(value.ToString());
                var listMedia2 = value as List<MediaCustome>;
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(" không đúng định dạng mảng.");
            }
        }
    }
}
