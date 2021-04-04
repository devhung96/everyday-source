using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Settings.Entitites
{
    [Table("mtc_setting_tbl")]
    public class Setting
    {
        
        [Key]
        [Column("setting_id")]
        public int SettingId { get; set; }

        [Column("setting_key")]
        public string SettingKey { get; set; }

        [Column("setting_value")]
        public string SettingValue { get; set; }

        [Column("setting_type")]
        public int SettingType { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
    public class SettingReponse
    {
        public int SettingId { get; set; }

        public string SettingKey { get; set; }

        public object SettingValue { get; set; }

        public int SettingType { get; set; }

        public DateTime CreatedAt { get; set; } 

        public DateTime? UpdatedAt { get; set; }

        public SettingReponse(Setting setting)
        {
            SettingId = setting.SettingId;
            SettingKey = setting.SettingKey;
            SettingType = setting.SettingType;
            if(setting.SettingType == (int)EnumSettingType.String)
            {
                SettingValue = setting.SettingValue;
            }else
            {
                SettingValue = JsonConvert.DeserializeObject(setting.SettingValue);
            }
            CreatedAt = setting.CreatedAt;
            UpdatedAt = setting.UpdatedAt;
        }
    }
    public enum EnumSettingType
    {
        String = 1,
        Object = 2,
        Array = 3
    }
}
