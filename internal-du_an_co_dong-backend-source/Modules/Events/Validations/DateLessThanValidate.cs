using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Events.Validations
{
    public class DateLessThanValidate : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateLessThanValidate(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("AttributeDoesNotExist");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            if (currentValue > comparisonValue)
                return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return "TheStartTimeMustBeGreaterThanTheEndTime";
        }
    }
}
