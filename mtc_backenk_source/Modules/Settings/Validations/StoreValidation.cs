using Project.Modules.Settings.Entitites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Settings.Validations
{
    public class StoreValidation
    {
        [Required]
        public string SettingKey { get; set; }
        [Required]
        public object SettingValue { get; set; }
        [Required]
        public EnumSettingType SettingType { get; set; }
    }
}
