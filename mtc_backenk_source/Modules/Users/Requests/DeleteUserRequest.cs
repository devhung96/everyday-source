﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class DeleteUserRequest
    {
        public List<string> UserIds { get; set; }
    }
}
