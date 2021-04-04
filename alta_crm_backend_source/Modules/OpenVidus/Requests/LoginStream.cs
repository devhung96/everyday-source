using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.OpenVidus.Requests
{
    public class LoginStream
    {
        //[Required]
        public string StreamId { get; set; }

        //[Required]
        public string Password { get; set; }
    }
}
