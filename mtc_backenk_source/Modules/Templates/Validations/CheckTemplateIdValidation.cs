using Project.App.Databases;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Templates.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Validations
{
    public class CheckTemplateIdValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IRepositoryWrapperMariaDB _mariaDBContext = (IRepositoryWrapperMariaDB)validationContext.GetService(typeof(IRepositoryWrapperMariaDB));
            string templateId = value as string;
            if(string.IsNullOrEmpty(templateId))
            {
                return ValidationResult.Success;
            }
            if (value != null)
            {
                Template template = _mariaDBContext.Templates.FindByCondition(x => x.TemplateId.Equals(templateId)).FirstOrDefault();

                if (template == null)
                {
                    return new ValidationResult("TemplateIdDoNotExists");
                }
            }
            return ValidationResult.Success;
        }
    }
}
