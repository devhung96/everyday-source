using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.PlayLists.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.PlayLists.Validations
{
    public class CheckPlayListIDValidation : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _mariaDBContext = (IRepositoryWrapperMariaDB)validationContext.GetService(typeof(IRepositoryWrapperMariaDB));
            string playListID = value.ToString();
            if (value != null)
            {
                PlayList playList = _mariaDBContext.PlayLists.FindByCondition(x => x.PlayListId.Equals(playListID)).FirstOrDefault();
                if (playList == null)
                    return new ValidationResult("playListID do not exists!!!");
            }
            return ValidationResult.Success;
        }
    }
}
