﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Request
{
    public class AddModuleRequest
    {
        [Required]
        public string name { get; set; }
    }
}
