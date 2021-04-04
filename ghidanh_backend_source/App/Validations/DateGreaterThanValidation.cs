using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations 
{
    public class DateGreaterThanValidation : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanValidation(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DateTime timeEnd = DateTime.Parse(value.ToString());
            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            DateTime timeStart = (DateTime) property.GetValue(validationContext.ObjectInstance);
            if (timeEnd <= timeStart)
            {
                return new ValidationResult("Time end must greater than time start");
            }
            return ValidationResult.Success;

        }
    }
}
