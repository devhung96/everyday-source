//using Project.App.Database;
//using Project.Modules.Users.Requests;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Project.Modules.Users.Validations
//{
//    public class ValidateLogin:ValidationAttribute
//    {
//        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
//        {
//            var data = value as LoginRequest;
//            var maria = (MariaDBContext)validationContext.GetService(typeof(MariaDBContext));

//            var user = maria.Users
//                .FirstOrDefault(x => x.UserEmail.Equals(data.UserName));
          
//            if (user is null || (!(user.UserPassword.Equals((user.UseSaft + data.Password).HashPassword()))))
//                return new ValidationResult("UserNameOrPasswordIsIncorrect");

//            return ValidationResult.Success;
//        }
//    }
//}
