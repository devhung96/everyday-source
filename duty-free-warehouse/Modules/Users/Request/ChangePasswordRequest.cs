using Project.Modules.Users.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Request
{
    [ValidChangePassword]
    public class ChangePasswordRequest
    {
        [Required]
        public string oldPassword { get; set; }

        [Required]

        public string newPassword { get; set; }
        [Required]
        public string confirmPassword { get; set; }
    }
}
