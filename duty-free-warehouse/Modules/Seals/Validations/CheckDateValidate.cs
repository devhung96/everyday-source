using System;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Seals.Validations
{
    public class CheckDateValidateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                if (value != null && value.ToString() != "" && !DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime check))
                    return new ValidationResult("Nhập sai định dạng ngày tháng, định dạng đúng là: 'dd/MM/yyyy'");

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult("Parse error: " + ex.Message);
            }
        }

    }
}
