using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class LoginCMSRequest
    {
        [Required]
        public string UserName { get; set; }
        public string StockCode { get; set; }
        [Required]
        public string UserPass { get; set; }
        [Required]
        public string Recaptcha  { get; set; }
    }
}
