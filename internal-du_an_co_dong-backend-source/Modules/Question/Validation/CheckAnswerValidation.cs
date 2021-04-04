using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Question.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Validation
{
    public class CheckAnswerValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
            try
            {
                //JArray jArray = JArray.Parse(value.ToString());
                var input = JsonConvert.SerializeObject(value);
                var check = JsonConvert.DeserializeObject<List<Selection>>(input);
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult("Answers không đúng định dạng mảng.");
            }
        }
    }
}
