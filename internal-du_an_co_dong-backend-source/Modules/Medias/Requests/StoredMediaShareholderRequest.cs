using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class StoredMediaShareholderRequest
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        public string OrganizeId { get; set; }
        [Required(ErrorMessage = "FieldIsRequired")]
        public string MediaTitle { get; set; }
        [Required(ErrorMessage = "FieldIsRequired")]
        public string MediaDocumentType { get; set; }
        public int? MediaFileType { get; set; }
        public string ParentId { get; set; }
    }
}
