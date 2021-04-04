using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TicketTypes.Requests
{
    public class AddTicketTypeRequest
    {
        [Required]
        public string TicketTypeName { get; set; }
        public string TicketTypeDescription { get; set; }
        public List<string> DeviceIds { get; set; } = new List<string>();
    } 
    public class PublishTicketTypeRequest
    {
        public string TicketTypeId { get; set; }
        public string TicketTypeName { get; set; }
        public string TicketTypeDescription { get; set; }
        public List<string> DeviceIds { get; set; } = new List<string>();
    }
    public class DeleteTicketTypeRequest
    {

        public string TicketTypeId { get; set; }
        public List<string> DeviceIds { get; set; }
    }
}
