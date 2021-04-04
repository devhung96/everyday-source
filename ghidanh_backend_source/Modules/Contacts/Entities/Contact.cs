using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Contacts.Entities
{
    [Table("rc_contacts")]
    public class Contact
    {
        [Key]
        [Column("contact_id")]
        public string ContactId { get; set; } = Guid.NewGuid().ToString();
        [Column("contact_name")]
        public string ContactName { get; set; }
        [Column("contact_phone")]
        public string ContactPhone { get; set; }
        [Column("contact_email")]
        public string ContactEmail { get; set; }
        [Column("contact_content")]
        public string ContactContent { get; set; }
        [Column("contact_created_at")]
        public DateTime ContactCreatedAt { get; set; } = DateTime.Now;


    }
}
