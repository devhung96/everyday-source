using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Requests
{
    public class UnRegisterUserDetectMutilRequest
    {
        public List<UnRegisterUserDetectRequest> UnRegisterUserDetects { get; set; }
    }
}
