using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Documents.Entities
{
    [Table("shareholder_documents")]
    public class DocumentFile
    {
        [Key]
        [Column("document_id")]
        public string DocumentID { get; set; } = Guid.NewGuid().ToString();
        [Column("document_name")]
        public string DocumentName { get; set; }
        [Column("document_link")]
        public string DocumentLink { get; set; }
        [Column("document_type")]
        public string DocumentType { get; set; }
        [Column("document_description")]
        public string DocumentDescription { get; set; }
        [Column("event_id")]
        public string EventId { get; set; }
        [Column("document_created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Column("user_id")]
        public string UserId { set; get; }
        [NotMapped]
        public string UserName { set; get; }
    }
}
