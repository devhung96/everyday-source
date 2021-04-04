using Project.Modules.Devices.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Entities
{
    [Table("mtc_schedule_device")]
    public class ScheduleDevice
    {
        [Key]
        [Column("schedule_device_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string ScheduleDeviceId { get; set; }
        [Column("device_id")]
        public string DeviceId { get; set; }
        [Column("schedule_id")]
        public string ScheduleId { get; set; }
        public Device Device { get; set; }
        public Schedule Schedule { get; set; }
        [Column("schedule_device_created_at")]
        public DateTime ScheduleDeviceCreatedAt { get; set; } = DateTime.UtcNow;
    }
}
