using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Validations
{
    public class CheckParentIdValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string parentId = value as string;
            var _mariaDb = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            var data = _mariaDb.Medias.FirstOrDefault(x => x.MediaID.Equals(parentId));
            if (data != null && !data.MediaType.Equals("folder"))
                return new ValidationResult("parentId không phải là thư mực");
            return ValidationResult.Success;
        }
    }
}
