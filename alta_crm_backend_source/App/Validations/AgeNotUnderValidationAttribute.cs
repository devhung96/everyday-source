using System;
using System.ComponentModel.DataAnnotations;

namespace Project.App.Validations
{
    public class AgeNotUnderValidationAttribute : ValidationAttribute
    {
        private readonly int Age;
        public AgeNotUnderValidationAttribute(int age)
        {
            Age = age;
        }
        public override bool IsValid(object value)
        {
            if(value != null)
            {
                DateTime birthday = (DateTime)value;
                if ((DateTime.UtcNow.Year - birthday.Year) < Age)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
