using Project.App.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Question.Validation
{
    public class CheckSessionID : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return new ValidationResult("không được bỏ trống.");
            }
            var _dbContext = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));
            string sessionID = value.ToString();
            var session = _dbContext.Sessions.FirstOrDefault(x => x.SessionId == sessionID);
            if (session is null)
            {
                return new ValidationResult(" không tồn tại.");
            }
            return ValidationResult.Success;
        }
    }
}
