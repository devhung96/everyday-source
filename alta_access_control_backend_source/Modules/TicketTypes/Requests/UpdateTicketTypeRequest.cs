using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TicketTypes.Requests
{
    public class UpdateTicketTypeRequest
    {
        public string TicketTypeId { get; set; } = null;
        public string TicketTypeName { get; set; }
        public string TicketTypeDescription { get; set; }
        public List<string> DeviceIds { get; set; }
    }
}
