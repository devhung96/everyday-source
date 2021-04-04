using Project.Modules.Question.Request;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Validation
{
    public class CheckFeedbackValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }
            Feedback feedback = (Feedback)value;
            if (feedback.Content is null || feedback.Title is null)
            {
                return new ValidationResult("Nội dung và tiêu đề không được bỏ trống.");
            }
            return ValidationResult.Success;
        }
    }
}
