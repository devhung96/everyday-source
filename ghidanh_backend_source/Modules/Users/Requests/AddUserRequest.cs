using Microsoft.AspNetCore.Http;
using Project.App.Validations;
using Project.Modules.Users.Valdations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class AddUserRequest
    {
        public string UserName { get; set; }
        [Required,EmailValidation]
        public string UserEmail { get; set; }
        public IFormFile Image { get; set; }
        public string Password { get; set; }
        [GroupValidation]
        public string GroupId { get; set; }
    }
    public class UpdateUserRequest
    {
        public string UserName { get; set; }
        public IFormFile Image { get; set; }
        [GroupValidation]
        public string GroupId { get; set; }
    }
}
