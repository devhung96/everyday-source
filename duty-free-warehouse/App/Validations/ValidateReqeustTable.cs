using Project.App.Requests;
using System.ComponentModel.DataAnnotations;

namespace Project.App.Validations
{
    public class ValidateReqeustTableAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RequestTable change = (RequestTable)value;
            if (string.IsNullOrEmpty(change.sortField) && string.IsNullOrEmpty(change.sortOrder))
                return ValidationResult.Success;
            else if ((!string.IsNullOrEmpty(change.sortField) && string.IsNullOrEmpty(change.sortOrder)) ||
                (string.IsNullOrEmpty(change.sortField) && !string.IsNullOrEmpty(change.sortOrder)))
                return new ValidationResult("Null sortFiled or sortOrder!!!");
            if (!change.sortOrder.ToLower().Equals("descend") && !change.sortOrder.ToLower().Equals("ascend"))
                return new ValidationResult("Sort order is incorrect.(descend, ascend) !!!");
            change.sortOrder = change.sortOrder.Replace("end", "");
            return ValidationResult.Success;
        }

    }
}
