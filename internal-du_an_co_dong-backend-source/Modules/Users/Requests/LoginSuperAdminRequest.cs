﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class LoginSuperAdminRequest
    {
        [Required]
        public string UserEmail { get; set; }

        [Required]
        public string UserPass { get; set; }
    }
}
