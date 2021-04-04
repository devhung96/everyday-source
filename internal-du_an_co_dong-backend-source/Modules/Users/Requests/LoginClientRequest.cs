using Microsoft.AspNetCore.Http;
using Project.Modules.Sessions.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class LoginClientRequest
    {
        [Required]
        public string ShareholderCode { get; set; }
    
        [Required]
        public string UserPass { get; set; }

        [Required]
        [CheckEventExistsAttribute]
        public string EventId { get; set; }
        [Required]
        public string Recaptcha { get; set; }
        
    }
}
