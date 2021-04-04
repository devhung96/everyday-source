using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace Project.Modules.Medias.Requests
{
    public class AddMediaRequest
    {
        public string MediaName { get; set; }
        public IFormFile MediaUrl { get; set; }
        public IFormFile MediaUrlOptional { get; set; }
        public string MediaType { get; set; } = "Default";
        public string MediaComment { get; set; }
        public string GroupIds { get; set; }
        public string MediaExtend { get; set; } 
    }

}
