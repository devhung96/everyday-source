using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TicketDevices.Requests
{
    public class AddTicketDerviceRequest
    {
        [Required]
        public string TicketTypeId { get; set; }
        [Required]
        public List<string> DeviceIds { get; set; }
    }
    public class DeleteTicketDerviceRequest
    {
        [Required]
        public string TicketTypeId { get; set; }
        [Required]
        public string DeviceId { get; set; }
    }
}
