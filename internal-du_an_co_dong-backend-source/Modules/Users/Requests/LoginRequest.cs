using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    [ValidationLogin]
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
