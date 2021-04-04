using System;
using System.ComponentModel.DataAnnotations;

namespace Project.App.Validations
{
    public class AgeNotUnderValidation : ValidationAttribute
    {
        private readonly int Age;
        public AgeNotUnderValidation(int Age)
        {
            this.Age = Age;
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
