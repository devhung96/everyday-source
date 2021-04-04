using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.TicketDevices.Entities;
using Project.Modules.TicketDevices.Requests;
using Project.Modules.TicketDevices.Services;
using Project.Modules.Tags.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Modules.Tickets.Entities;

namespace Project.Modules.TicketDevices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketTypeDevicesController : BaseController
    {
        private readonly ITicketDeviceService ticketDeviceService;
        public TicketTypeDevicesController(ITicketDeviceService ticketDeviceService)
        {
            this.ticketDeviceService = ticketDeviceService;
        }
        [HttpPost]
        public IActionResult AddTagDevice([FromBody] AddTicketDerviceRequest request)
        {
            (List<TicketTypeDevice> ticketDevices, string message) = ticketDeviceService.AddTicketDevice(request);
            if (ticketDevices is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(ticketDevices, message);
        }
        [HttpGet("ShowTicketTypeByDevice/{deviceId}")]
        public IActionResult ShowTagByDevice([FromQuery] PaginationRequest request, string deviceId)
        {
            PaginationResponse<TicketType> data = ticketDeviceService.ShowTicketByDevice(request, deviceId);

            return ResponseOk(data, "Success");
        } 
        [HttpGet("ShowDeviceByTicket/{ticketTypeId}")]
        public IActionResult ShowDeviceByTag([FromQuery] PaginationRequest request, string ticketTypeId)
        {
            PaginationResponse<Device> data = ticketDeviceService.ShowDeviceByTicket(request, ticketTypeId);

            return ResponseOk(data, "Success");
        } 
        [HttpGet("ShowDeviceNotInByTicket/{ticketTypeId}")]
        public IActionResult ShowDeviceNotInByTicket( string ticketTypeId)
        {
            List<Device> data = ticketDeviceService.ShowDeviceNotInTicket(ticketTypeId);

            return ResponseOk(data, "Success");
        }
        [HttpDelete]
        public IActionResult Remove([FromBody] DeleteTicketDerviceRequest request)
        {
            (TicketTypeDevice ticketTypeDevice, string message)  = ticketDeviceService.RemoveTicketDevice(request);
            if(ticketTypeDevice is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk("Success", message);
        }
    }
}
