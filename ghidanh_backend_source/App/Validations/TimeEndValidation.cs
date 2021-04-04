using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class TimeEndValidation : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public TimeEndValidation(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            TimeSpan timeEnd = TimeSpan.Parse(value.ToString());
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            TimeSpan timeStart = TimeSpan.Parse(property.GetValue(validationContext.ObjectInstance).ToString());
            if (timeEnd <= timeStart)
            {
                return new ValidationResult("TimeEndMustGreaterThanTimeStart");
            }
            return ValidationResult.Success;

        }
    }
}
