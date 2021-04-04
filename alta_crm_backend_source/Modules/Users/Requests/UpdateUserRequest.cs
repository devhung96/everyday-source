using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class UpdateUserRequest
    {
        public string GroupId { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string UserPhone { get; set; }
        public string UserAddress { get; set; }
        public int? UserGender { get; set; } 

        public string ModeId { get; set; }

        public string UserModeKeyCode { get; set; }

        public List<IFormFile> UserImages { get; set; }
        public List<string> TagIdsParse { get; set; }
    }
}
