using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class ImportEmployeeRequest
    {
        [Required]
        public IFormFile FileZip { get; set; }
    }
}
