using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Products.Requests
{
    public class ImportProductRequest
    {
        [Required]
        public IFormFile fileUpload { get; set; }
        public string fileName { get; set; }
    }
}
