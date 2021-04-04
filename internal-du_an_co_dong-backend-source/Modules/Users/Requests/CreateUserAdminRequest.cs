using Project.Modules.Users.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Project.Modules.Users.Entities.User;

namespace Project.Modules.Users.Requests
{
    public class CreateUserAdminRequest
    {
        public string Password { get; set; }
        public string Image { get; set; }
        [Required]
        [ValidateEmailSystem]
        public string Email { get; set; }
        public string FullName { get; set; }
        public string OrganizeId { get; set; }
    }
}
