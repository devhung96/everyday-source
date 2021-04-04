using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Documents.Requests
{
    public class UpdateDocumentRequest
    {
        [Required(ErrorMessage = "FieldIsRequired")]
        public string DocumentLink { get; set; }
        [Required(ErrorMessage = "FieldIsRequired")]
        public string EventId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentType { get; set; }
    }
}
