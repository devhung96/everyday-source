using Newtonsoft.Json;
using Project.Modules.DeviceTypes.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Entities
{
    [Table("hdbank_ac_devices")]
    public class Device
    {
        [Key]
        [Column("device_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string DeviceId { get; set; }
        [Column("device_name")]
        public string DeviceName { get; set; }
        [Column("device_code")]
        public string DeviceCode { get; set; } 
        [Column("device_password")]
        public string DevicePassword { get; set; }
        [Column("device_mac")]
        public string DeviceMac { get; set; }
        [Column("device_type_id")]
        public string DeviceTypeId { get; set; }
        [Column("device_created")]
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DeviceType DeviceType { get; set; }


        [Column("device_settings")]
        public string DeviceSettings { get; set; }

        [NotMapped]
        public object DeviceSettingData
        {
            get
            {
                if (String.IsNullOrEmpty(DeviceSettings)) return null;
                return JsonConvert.DeserializeObject<object>(DeviceSettings);
            }
            
        }

    }
}
