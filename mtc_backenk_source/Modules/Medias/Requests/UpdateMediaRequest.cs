using Microsoft.AspNetCore.Http;

namespace Project.Modules.Medias.Requests
{
    public class UpdateMediaRequest
    {
        public string MediaName { get; set; }
        public string MediaComment { get; set; }

        public string MediaExtend { get; set; }
        public IFormFile MediaUrlOption { get; set; }
    }
}
