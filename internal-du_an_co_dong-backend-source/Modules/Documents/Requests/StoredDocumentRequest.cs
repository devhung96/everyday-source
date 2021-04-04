using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Documents.Requests
{
    public class StoredDocumentRequest
    {
        [Required(ErrorMessage = " Là trường bắt buộc."), CheckUrlDocumentRequest]
        public string DocumentLink { get; set; }
        [Required(ErrorMessage = " Là trường bắt buộc.")]
        public string EventId { get; set; }
        [Required(ErrorMessage = " Là trường bắt buộc.")]
        public string DocumentType { get; set; }
        [Required(ErrorMessage = " Là trường bắt buộc.")]
        public string DocumentName { get; set; }
    }
}
