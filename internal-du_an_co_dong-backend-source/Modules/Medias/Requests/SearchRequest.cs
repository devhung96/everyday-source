using Microsoft.AspNetCore.Mvc;
using Project.Modules.Medias.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class SearchRequest
    {
        [Required]
        [SearchTypeValidation]
        public string SearchType { get; set; }
        public string SearchContent { get; set; }
        [Required(ErrorMessage = "FieldIsRequired")]
        public string OrganizeId { get; set; }
        public int limit { get; set; }
        public int index { get; set; }
    }
}
