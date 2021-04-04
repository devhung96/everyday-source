using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Requests
{
    public class ValidateRequestTable : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            RequestTable change = (RequestTable)value;
            if (string.IsNullOrEmpty(change.SortField) && string.IsNullOrEmpty(change.SortOrder))
                return ValidationResult.Success;
            else if ((!string.IsNullOrEmpty(change.SortField) && string.IsNullOrEmpty(change.SortOrder)) ||
                (string.IsNullOrEmpty(change.SortField) && !string.IsNullOrEmpty(change.SortOrder)))
                return new ValidationResult("NullSortFiledOrSortOrder");
            if (!change.SortOrder.ToLower().Equals("descend") && !change.SortOrder.ToLower().Equals("ascend"))
                return new ValidationResult("SortOrderIsIncorrect(descend,ascend)");
            change.SortOrder = change.SortOrder.Replace("end", "");
            return ValidationResult.Success;
        }

    }
}
