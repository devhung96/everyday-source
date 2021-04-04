using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class ChangePasswordRequest
    {
        public string PasswordOld { get; set; }
        public string PasswordNew { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
