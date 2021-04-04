using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Mqtt
{
    public class MqttRequest
    {
        public string Key { get; set; }
        public object Data { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
