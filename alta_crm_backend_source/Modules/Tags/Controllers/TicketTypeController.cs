using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Tags.Enities;
using Project.Modules.Tags.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketTypeController : BaseController
    {
        private readonly ITicketTypeService _ticketTypeService;
        public TicketTypeController(ITicketTypeService ticketTypeService)
        {
            _ticketTypeService = ticketTypeService;
        }

        /// <summary>
        /// Hiển thị tất cả group (Có pagination) (Huỳnh Anh)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public  IActionResult GetTickets([FromQuery] PaginationRequest request)
        {
            var result = _ticketTypeService.ShowAll(request);
            return ResponseOk(result); 
        }
    }
}
