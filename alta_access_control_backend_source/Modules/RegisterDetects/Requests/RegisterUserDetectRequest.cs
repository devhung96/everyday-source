using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Requests
{
    public class RegisterUserDetectRequest
    {
        public string RgDectectUserId { get; set; }
        public string RgDectectKey { get; set; }

        public string ModeId { get; set; }
        public string TagCode { get; set; }
        public string TicketTypeId { get; set; }

        public List<RegisterDettectDetailRequest> RegisterDettectDetailRequests { get; set; }

        public string RgDectectExtension { get; set; }

    }
}
