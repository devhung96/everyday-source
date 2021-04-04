using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Modules.Reports.Requests;
using Project.Modules.Reports.Services;

namespace Project.Modules.Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {

        private readonly IReportService _reportService;
        
        public TestController(IReportService reportService)
        {
            _reportService = reportService;

        }

        //[HttpGet("test/{id}")]
        //public IActionResult Test(string id)
        //{

        //    _soketIO.ForwardAsync("Y25PUZ", null, "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJVc2VySWQiOiJiYTg1MzRlOC01MTdmLTRjZmEtYjZmZC1mOWYzNGRlYjc2NDAiLCJPcmdhbml6ZUlkIjoiMzg4YTQzMWItNTc0Zi00MWZkLWFjNzQtOGMwNjc1OTg0OTg4IiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQURNSU4iLCJQZXJtaXNzaW9uRGVmYXVsdCI6IkFETUlOIiwicGVybWlzc2lvbiI6IntcIkNNLU1JQ1wiOjMxLFwiVFctSUNFXCI6MTV9IiwiUGVybWlzc2lvbkV2ZW50cyI6Ilt7XCJFdmVudElEXCI6XCI4UUlTQzJcIixcIlBlcm1pc3Npb25Db2RlXCI6XCJbXVwifSx7XCJFdmVudElEXCI6XCJITlBCVjdcIixcIlBlcm1pc3Npb25Db2RlXCI6XCJbXVwifSx7XCJFdmVudElEXCI6XCJZMjVQVVpcIixcIlBlcm1pc3Npb25Db2RlXCI6XCJbXVwifV0iLCJleHAiOjE1OTE5Mzc2NTksImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NDQzNTEiLCJhdWQiOiJodHRwOi8vbG9jYWxob3N0OjQ0MzUxIn0.a4zvMuCOClT6CEvk5yd3WWZir9o7x5A4QqMA9x3gyZk", "star_event", null, "1", "1");
        //    object result = _reportService.SupportGetInfo(id);
        //    return Ok(result);
        //}



        [HttpPost("test-submit")]
        public IActionResult TestSubmit(TestSubmitRequest request)
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            _reportService.SubmitSocketChartAsync(request.eventId, request.questionId, token, request.optionQuestion);
            return Ok();
        }


        //[HttpPost("test-socket")]
        //public IActionResult TestSocket(TestSocketRequest request)
        //{
        //    var token = HttpContext.Request.Headers["Authorization"].ToString();
        //    _reportService.SubmitSocketChartAsync(request.eventId, request.questionId, token, request.optionQuestion);
        //    return Ok();
        //}



        [HttpPost("test-submit")]
        public IActionResult TestSubmit()
        {
            var hung = _reportService.SupportGetChartWithSession("82f86914-82d5-464f-9e88-003d4d67715b");
            return Ok(hung);
        }

    }
}