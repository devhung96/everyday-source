using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Medias.Entities;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Project.Modules.Medias.Validations
{
    public class CheckMediaIDValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IRepositoryWrapperMariaDB _mariaDBContext = (IRepositoryWrapperMariaDB)validationContext.GetService(typeof(IRepositoryWrapperMariaDB));
            int mediaID = (int)value;
            if (value != null)
            {
                Media playList = _mariaDBContext.Medias.FindByCondition(x => x.MediaId.Equals(mediaID)).FirstOrDefault();
                if (playList is null)
                {
                    return new ValidationResult("Media id do not exists");
                }
            }

            return ValidationResult.Success;
        }
    }
}
