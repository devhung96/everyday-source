using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Operations;
using Project.App.Controllers;
using Project.Modules.Exchangerates.Entities;
using Project.Modules.Exchangerates.Requests;
using Project.Modules.Exchangerates.Services;

namespace Project.Modules.Exchangerates.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExchangerateController : BaseController
    {
        public readonly IExchangerateService exchangerateService;
        public ExchangerateController(IExchangerateService _exchangerateService)
        {
            exchangerateService = _exchangerateService;
        }
        
        [HttpPost("createExchangerate")]
        public IActionResult CreateExchangerate([FromBody] ExchangerateCreateRequest valueInput)
        {
            (Exchangerate data, string message) = exchangerateService.CreateExchangerate(valueInput);
            return ResponseOk(data, message);
        }
        [HttpGet]
        public IActionResult ShowAll()
        {
            List<Exchangerate> exchangerates = exchangerateService.GetAll();
            return ResponseOk(exchangerates, "ShowAllSuccess");
        }
    }
}