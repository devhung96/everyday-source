using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Models
{
    public class RegisterUserResponse
    {
        public string DataResult { get; set; }
        public string DataRequest { get; set; }
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
    }
}
