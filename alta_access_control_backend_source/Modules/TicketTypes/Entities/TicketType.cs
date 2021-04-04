using Project.Modules.Devices.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using static Project.Modules.DeviceTypes.Entities.DeviceType;

namespace Project.Modules.Tickets.Entities
{
    [Table("hdbank_ac_ticket_types")]
    public class TicketType
    {
        [Key]
        [Column("ticket_type_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TicketTypeId { get; set; }
        [Column("ticket_type_name")]
        public string TicketTypeName { get; set; }
        [Column("ticket_type_description")]
        public string TicketTypeDescription { get; set; }
        [Column("ticket_type_status")]
        public STATUS TicketTypeStatus { get; set; } = STATUS.Active;
        [Column("ticket_type_created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        [NotMapped] 
        public List<Device> Devices { get; set; }
        [NotMapped]
        public List<string> DeviceIds { get; set; }
        
    }
}
