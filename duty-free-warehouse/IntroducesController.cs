using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Project
{
    [Route("")]
    [ApiController]
    public class IntroducesController : ControllerBase
    {
        [HttpGet]
        public IActionResult Introduce()
        {
            return Ok("Version: 0.1");
        }
    }
}
