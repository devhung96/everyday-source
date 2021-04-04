using Newtonsoft.Json;
using Project.App.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Enities
{
    [Table("hd_bank_ticket")]
    public class TicketType
    {
        [Key, Column("ticket_type_id")]
        public string TicketTypeId { get; set; }

        [Column("ticket_type_name")]
        public string TicketTypeName { get; set; }

        [Column("ticket_type_description")]
        public string TicketTypeDescription { get; set; }

        [Column("devices")]
        public string Devices { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = GeneralHelper.DateTimeDatabase;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public List<string> ListDevices
        {
            get
            {
                if (Devices is null)
                {
                    return new List<string>();
                }
                return JsonConvert.DeserializeObject<List<string>>(Devices);
            }
            set
            {
                ListDevices = value;
            }
        }

        [NotMapped]
        public List<Tag> Tags { get; set; }
    }
}
