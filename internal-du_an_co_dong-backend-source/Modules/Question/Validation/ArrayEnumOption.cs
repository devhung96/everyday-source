using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Validation
{
    public class ArrayEnumOption : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
            try
            {
                JArray jArray = JArray.Parse(value.ToString());

                var a = jArray.ToObject<List<int>>();
                //var checkInList = a.Any(x => ArrayEnumOption.IsDefined(x))

                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(" sai định dạng array.");
            }
        }
    }
}
