using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class InviteSpeakRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int Status { get; set; }
    }
}
