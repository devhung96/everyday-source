using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; }
        public string RoleId { get; set; }
    }
    public class UpdateProfileRequest
    {
        public string UserName { get; set; }
        public IFormFile Image { get; set; }
    }
    public class ExtendRequest
    {
        public DateTime ExpiredAt { get; set; }
    } 
    public class ExtendGroupRequest
    {
        public DateTime Expired { get; set; }
    }
}
