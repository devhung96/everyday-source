﻿using Project.Modules.Settings.Entitites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Settings.Requests
{
    public class UpdateSettingRequest
    {
        public string SettingKey { get; set; }
        public object SettingValue { get; set; }
        public EnumSettingType SettingType { get; set; }
    }
}
