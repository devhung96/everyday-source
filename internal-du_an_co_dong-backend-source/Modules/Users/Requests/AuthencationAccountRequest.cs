using Microsoft.AspNetCore.Http;
using Project.Modules.Organizes.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Requests
{
    public class AuthencationAccountRequest
    {

        [Required]
        [DataType(DataType.Upload)]
        [MaxFileSizeAttribute(200)]
        [AllowedExtensions(new string[] { ".png", ".sgv", ".jpg", ".jpeg" })]
        public List<IFormFile> UserPhotoUrls { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        [MaxFileSizeAttribute(200)]
        //[AllowedExtensions(new string[] { ".png", ".sgv", ".jpg", ".jpeg" })]
        public IFormFile UserIdentityCardUrl { get; set; }
    }
}
