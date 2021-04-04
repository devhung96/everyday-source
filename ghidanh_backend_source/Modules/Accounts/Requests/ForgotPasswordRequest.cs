using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    [ValidateForgotPasswordRequest]
    public class ForgotPasswordRequest
    {
        public string PasswordNew { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
