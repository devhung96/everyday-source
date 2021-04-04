using Microsoft.Extensions.DependencyInjection;
using Project.Modules.Groups.Enities;
using Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Validation
{
    public class GroupExistsValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }
            var MariaDb = validationContext.GetRequiredService<IRepositoryWrapperMariaDB>();
            Group group = MariaDb.Groups.FindByCondition(x => x.GroupId == value.ToString()).FirstOrDefault();
            if (group is null)
            {
                return new ValidationResult("GroupNotExists");
            }
            return ValidationResult.Success;            
        }
    }
}
