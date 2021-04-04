using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class TokenVidu
    {
        [Required]
        public string Token  { get; set; }
    }
}
