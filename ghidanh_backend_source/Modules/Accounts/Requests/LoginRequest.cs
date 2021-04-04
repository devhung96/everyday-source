using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Accounts.Entities.Account;

namespace Project.Modules.Accounts.Requests
{
    public class LoginRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public TYPE_ACCOUNT Type { get; set; }

    } 
    public class ForgotRequest
    {
        [Required]
        public string UserName { get; set; }
    }
}
