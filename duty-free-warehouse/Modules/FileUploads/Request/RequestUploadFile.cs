using Microsoft.AspNetCore.Http;
using Project.Modules.FileUploads.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.FileUploads.Request
{
    public class RequestUploadFile
    {
        [Required]
        [EnumDataType(typeof(FileTye))]
        public FileTye fileType { get; set; }
        [Required]
        public IFormFile fileUpload { get; set; }
        public string fileName { get; set; }
    }
}
