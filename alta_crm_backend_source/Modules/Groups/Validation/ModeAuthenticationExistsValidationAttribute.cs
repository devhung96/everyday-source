using Microsoft.Extensions.DependencyInjection;
using Project.Modules.Groups.Enities;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.Validation
{
    public class ModeAuthenticationExistsValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("ModeAuthenticationIdRequired");
            }

            var MariaDb = validationContext.GetRequiredService<IRepositoryWrapperMariaDB>();
            ModeAuthentication modeAuthentication = MariaDb.ModeAuthentications.FindByCondition(x => x.ModeAuthenticationId == value.ToString()).FirstOrDefault();
            if (modeAuthentication is null)
            {
                return new ValidationResult("ModeAuthenticationNotExists");
            }
            return ValidationResult.Success;
        }
    }
}
