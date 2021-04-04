using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class UpdateUserAdminRequest
    {
        public string UserImage { get; set; }
        public string FullName { get; set; }
    }
}
