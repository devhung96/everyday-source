using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Request
{
    public class UpdateUserRequest
    {
        [ValidationEmail]
        public string userName { get; set; }
        public string fullName { get; set; }
        public string departmentcode { get; set; }
    }
}
