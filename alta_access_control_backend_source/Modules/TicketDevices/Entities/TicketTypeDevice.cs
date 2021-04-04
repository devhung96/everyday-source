using Project.Modules.Devices.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TicketDevices.Entities
{
    [Table("hdbank_ac_ticket_type_devices")]
    public class TicketTypeDevice
    {
        [Key]
        [Column("ticket_type_device_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TicketTypeDeviceId { get; set; }
        [Column("ticket_type_id")]
        public string TicketTypeId { get; set; }
        [Column("device_id")]
        public string DeviceId { get; set; }
        [Column("ticket_type_device_created_at")]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public Device Device { get; set; }
    }
}
