using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.DeviceTypes.Entities
{
    [Table("hdbank_ac_device_types")]
    public class DeviceType
    {
        [Key]
        [Column("device_type_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DeviceTypeId { get; set; }
        [Column("device_type_name")]
        public string DeviceTypeName { get; set; }
        [Column("device_type_status")]
        public STATUS DeviceTypeStatus { get; set; } = STATUS.Active;
        [Column("device_type_created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public enum STATUS
        {
            Active =1,
            UnActive =0
        }
    }
}
