using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Project.Modules.Medias.Requests
{
    public class UpdateMediaTypeRequest
    {
        public string TypeName { get; set; }

        [DataType(DataType.Upload)]
        //[MaxFileSize(5 * 1024 * 1024)]
        //[AllowedExtensions(new string[] { ".jpg", ".png" })]
        public IFormFile TypeIcon { get; set; }
        public string TypeCommnet { get; set; }
    }
}
