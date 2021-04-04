using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class StoreMediaRequest
    {
        public List<IFormFile> MediaFiles { get; set; }  // danh sách file
    }
   
}
