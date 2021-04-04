using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Project.App.Controllers;
using Project.Modules.Seals.Entity;
using Project.Modules.Seals.Request;
using Project.Modules.Seals.Response;
using Project.Modules.Seals.Services;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityPairController : BaseController
    {
        private readonly ICityPairService cityPairService;
        public CityPairController(ICityPairService CityPairService)
        {
            cityPairService = CityPairService;
        }
        [HttpPost]
        public IActionResult AddCityPair([FromBody]AddCityPairRequest request)
        {
            (CityPairResponse citypair, string message) = cityPairService.AddCityPair(request);
            if(citypair is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(citypair, message);
        }

        [HttpGet]
        public IActionResult ListCityPair()
        {
            int.TryParse(Request.Query["limit"], out int limit);
            int.TryParse(Request.Query["page"], out int page);
            string search = Request.Query["search"].ToString();
            TableResponse citypairs = cityPairService.ListCityPair(limit, page, search);
            return ResponseOk(citypairs);
        }
        [HttpGet("importData")]
        public IActionResult ImportData()
        {
            cityPairService.GenerateCityPair();
            return ResponseOk();
        }
        [HttpGet("updateTypeSchedule")]
        public IActionResult UpdateTypeSchedule()
        {
            cityPairService.UpdateTypeSchedule();
            return ResponseOk();
        }
        [HttpGet("route")]
        public IActionResult ListDeparture(string departure)
        {
            List<string> arrivals = cityPairService.ListDeparture(departure);
            return ResponseOk(arrivals);
        }
        [HttpGet("{cityPairId}")]
        public IActionResult ListCityPair(int cityPairId)
        {
            Citypair citypair = cityPairService.DetailCityPair(cityPairId);
            if(citypair is null)
            {
                return ResponseBadRequest("Không tìm thấy cityPair");
            }
            return ResponseOk(new CityPairResponse(citypair));
        }
        [HttpPut("{cityPairId}")]
        public IActionResult UpdateCityPair([FromBody]AddCityPairRequest request, int cityPairId)
        {
            Citypair citypair = cityPairService.DetailCityPair(cityPairId);
            if (citypair is null)
            {
                return ResponseBadRequest("Không tìm thấy cityPair");
            }
            CityPairResponse cityPairResponse = cityPairService.UpdateCityPair(request, citypair);
            return ResponseOk(cityPairResponse, "Cập nhật thành công");
        }
        [HttpGet("listSealOnDay")]
        public IActionResult ListSealOnDay()
        {
            object data = cityPairService.ListSealOnDay(DateTime.Now);
            return ResponseOk(data);
        }

        [HttpPut("sealOnDay/{citypairId}")]
        public IActionResult UpdateSealOnDay([FromBody] List<SealOnDayRequest> requests, int copy, int citypairId)
        {
            (object data, string message) = cityPairService.UpdateSealNumber(citypairId, requests, copy);
            if(data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
    }
}