using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Middlewares
{
    public class ValidateDeviceAtribute : Attribute
    {
        public bool IsCheckDeviceExpired { get;set; }
        public bool IsCheckStatus { get; set; }
    }
}
