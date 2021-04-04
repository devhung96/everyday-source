using Newtonsoft.Json;
using Project.Modules.Semesters.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Holidays.Validation
{
    public class CheckTimeValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("RequestFail");
            }
            BaseParamsSemester baseParams = JsonConvert.DeserializeObject<BaseParamsSemester>(JsonConvert.SerializeObject(value));
            //DateTime
            DateTime TimeStart;
            DateTime TimeEnd;
            if (!DateTime.TryParseExact(baseParams.TimeStart, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out TimeStart))
            {
                return new ValidationResult("TimeStartNotValid");
            }
            if (!DateTime.TryParseExact(baseParams.TimeEnd, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out TimeEnd))
            {
                return new ValidationResult("TimeStartNotValid");
            }
            if (TimeEnd < TimeStart)
            {
                return new ValidationResult("TimeEndNotValid");
            }
            return ValidationResult.Success;
        }
    }
}
