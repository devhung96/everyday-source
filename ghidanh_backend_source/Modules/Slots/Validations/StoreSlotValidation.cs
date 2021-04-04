using Project.Modules.Slots.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Slots.Validations
{
    public class StoreSlotValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            StoreSlotRequest request = (StoreSlotRequest) value;
            try
            {
                TimeSpan timeStart = TimeSpan.Parse(request.SlotStartAt);
                TimeSpan timeEnd = TimeSpan.Parse(request.SlotEndAt);
                if (timeEnd <= timeStart)
                {
                    return new ValidationResult("TimeEndMustGreaterThanTimeStart");
                }
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult("TimeIsNotCorrectFormat");
            }
        }
    }
}
