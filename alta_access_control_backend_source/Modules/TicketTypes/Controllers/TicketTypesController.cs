using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Tags.Entities;
using Project.Modules.Tickets.Entities;
using Project.Modules.Tickets.Services;
using Project.Modules.TicketTypes.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TicketTypes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketTypesController : BaseController
    {
        private readonly ITicketTypeService typeService;
        public TicketTypesController(ITicketTypeService typeService)
        {
            this.typeService = typeService;
        }
        [HttpGet("{ticketTypeId}")]
        public IActionResult Detail(string ticketTypeId)
        {
            (TicketType ticketType, string message) = typeService.Detail(ticketTypeId);
            if (ticketType is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(ticketType, message);
        }
        [HttpDelete("{ticketTypeId}")]
        public async Task<IActionResult> RemoveAsync(string ticketTypeId)
        {
            (TicketType ticketType, string message) = await typeService.DeleteTicketAsync(ticketTypeId);
            if (ticketType is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(ticketType, message);
        }
        [HttpGet]
        public IActionResult ShowTable([FromQuery] PaginationRequest request)
        {
            PaginationResponse<TicketType> response = typeService.ShowTable(request);
            return ResponseOk(response);
        }
        [HttpGet("TagByTicket/{ticketTypeId}")]
        public IActionResult TagByTicket([FromQuery] PaginationRequest request, string ticketTypeId)
        {
            PaginationResponse<Tag> response = typeService.ShowTagInTicketType(request, ticketTypeId);
            return ResponseOk(response);
        }
        [HttpPost]
        public async Task<IActionResult> AddNewAsync([FromBody] AddTicketTypeRequest request)
        {
            (TicketType ticket, string messge) = await typeService.AddTicketAsync(request);
            if (ticket is null)
            {
                return ResponseBadRequest(messge);
            }
            return ResponseOk(ticket);
        }
        [HttpPut("{ticketTypeId}")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateTicketTypeRequest request, string ticketTypeId)
        {
            (TicketType ticket, string messge) = await typeService.UpdateTicketAsync(request, ticketTypeId);
            if (ticket is null)
            {
                return ResponseBadRequest(messge);
            }
            return ResponseOk(ticket);
        }

        #region Function test
        [HttpPost("TestAddMutiple")]
        public IActionResult TestAddMutiple()
        {
            typeService.TestAddMutiTicketTypesAsync();
            return ResponseOk("TestAddMutiTicketTypesAsync");
        }

        [HttpDelete("TestDeleteMutiple")]
        public IActionResult TestDeleteMutiple()
        {
            typeService.TestDeleteMutiTicketTypesAsync();
            return ResponseOk("TestDeleteMutiTicketTypesAsync");
        }
        #endregion


    }
}
