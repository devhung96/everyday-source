﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class UploadMediaOrganizeRequest
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        public IFormFile MediaFile { get; set; }
        [Required(ErrorMessage = "FieldIsRequired")]
        public string OriganizeId { get; set; }
        public string MediaType { get; set; }
        public string MediaTitle { get; set; }
        public string ParentId { get; set; }
    }
}
