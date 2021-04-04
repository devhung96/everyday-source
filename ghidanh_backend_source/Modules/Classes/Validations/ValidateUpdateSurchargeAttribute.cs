using Project.App.DesignPatterns.Repositories;
using Project.Modules.Classes.Entities;
using Project.Modules.Classes.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Classes.Validations
{
    public class ValidateUpdateSurchargeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            UpdateSurchargeRequest request = value as UpdateSurchargeRequest;
            IRepositoryMariaWrapper repositoryWrapper = (IRepositoryMariaWrapper)validationContext.GetService(typeof(IRepositoryMariaWrapper));
            Surcharge surcharge = repositoryWrapper.Surcharges.FirstOrDefault(x => x.SurchargeId.Equals(request.SurchargeId));
            if (surcharge is null)
            {
                return new ValidationResult("SurchargeNotFound");
            }
            Class @class = repositoryWrapper.Classes.FirstOrDefault(x => x.ClassId.Equals(surcharge.ClassId));
            if (@class is null || @class.Admission == Class.STATUS_OPEN.CLOSE)
            {
                return new ValidationResult("ClassNotFound");
            }
            Class newClass = repositoryWrapper.Classes.FirstOrDefault(x => x.ClassId.Equals(request.ClassId));
            if (newClass is null || newClass.Admission == Class.STATUS_OPEN.CLOSE)
            {
                return new ValidationResult("NewClassNotFound");
            }
            if(request.SurchargeAmount.Value < 0 || request.SurchargeAmount.Value > double.MaxValue)
            {
                return new ValidationResult("SurchargeAmountInvalid");
            }
            return ValidationResult.Success;
        }
    }
}
