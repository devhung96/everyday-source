using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Requests
{
    public class StoredFolderEventRequest
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        public string EventId { get; set; }
        [Required(ErrorMessage = "FieldIsRequired")]
        public string MediaName { get; set; }
        [Required(ErrorMessage = "FieldIsRequired")]
        public string MediaDocumentType { get; set; }
        public string ParentId { get; set; }
        public string MediaPath { get; set; }
    }
}
