using Project.Modules.Sessions.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Sessions.Requests
{
    public class UpdateSessionRequest
    {
        public string SessionName { get; set; }
        public string SessionTitle { get; set; }

        public int SessionSort { get; set; }


        public string SessionDescription { get; set; }
        [CheckEventExistsAttribute]
        public string EventId { get; set; }
    }
}
