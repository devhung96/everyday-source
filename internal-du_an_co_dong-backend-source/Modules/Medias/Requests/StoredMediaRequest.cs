using Microsoft.AspNetCore.Http;
using Project.Modules.Medias.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class StoredMediaRequest
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        public List<IFormFile> MediaFiles { get; set; }
        [Required(ErrorMessage = "FieldIsRequired")]
        public string OrganizeId { get; set; }
        [CheckParentIdValidation]
        public string ParentId { get; set; }
    }
}
