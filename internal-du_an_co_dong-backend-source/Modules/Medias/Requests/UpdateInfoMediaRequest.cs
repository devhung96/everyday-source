using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class UpdateInfoMediaRequest
    {
        public IFormFile MediaFile { get; set; }
        public string MediaTitle { get; set; }
        public string UserId { get; set; }
    }
}
