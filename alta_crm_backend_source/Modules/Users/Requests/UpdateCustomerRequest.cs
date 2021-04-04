using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class UpdateCustomerRequest
    {
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerAddress { get; set; }
        public int? CustomerGender { get; set; }
        public string ModeId { get; set; }
        public string CustomerModeKeyCode { get; set; }
        public List<IFormFile> CustomerImages { get; set; }
    }
}
