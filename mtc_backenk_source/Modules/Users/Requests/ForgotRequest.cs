using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class ForgotRequest
    {
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
    public class ResetPasswordRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
