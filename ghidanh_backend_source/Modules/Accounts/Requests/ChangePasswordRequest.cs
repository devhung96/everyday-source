using Project.Modules.Accounts.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Accounts.Requests
{
    public class ChangePasswordRequest
    {
        [Required]
        public string PasswordOld { get; set; }
        [Required]
        public string PasswordNew { get; set; }
        [Required]
        public string Confirm { get; set; }
    }
    public class ResetPasswordRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public Account.TYPE_ACCOUNT Type { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
