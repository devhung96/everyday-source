using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Validations
{
    public class ValidateRequestTable : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RequestTable request = (RequestTable)value;
            if (string.IsNullOrEmpty(request.SortField) && string.IsNullOrEmpty(request.SortOrder))
            {
                return ValidationResult.Success;
            }               
            else if ((!string.IsNullOrEmpty(request.SortField) && string.IsNullOrEmpty(request.SortOrder)) ||
                (string.IsNullOrEmpty(request.SortField) && !string.IsNullOrEmpty(request.SortOrder)))
            {
                return new ValidationResult("NullSortFiledOrSortOrder");
            }
            if (!request.SortOrder.ToLower().Equals("descend") && !request.SortOrder.ToLower().Equals("ascend") && !request.SortOrder.ToLower().Equals("desc") && !request.SortOrder.ToLower().Equals("asc"))
            {
                return new ValidationResult("SortOrderIsIncorrect(descend,ascend)");
            }
            if (request.Limit <= 0)
            {
                return new ValidationResult("Limit is must a positive number");
            }
            if (request.Page < 0)
            {
                return new ValidationResult("Page is must a positive number");
            }
            request.SortOrder = request.SortOrder.Replace("end", "");
            return ValidationResult.Success;
        }
    }
}
