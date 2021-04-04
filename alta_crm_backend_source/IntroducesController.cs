using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Project
{
    [Route("")]
    [ApiController]
    public class IntroducesController : ControllerBase
    {
        [HttpGet]
        public ContentResult Introduce()
        {
            string tenplate = "<!doctype html> <html lang='en'> <head> <style> .version { animation: color-change 1s infinite; font-weight: bold; font-size: 20px} @keyframes color-change { 0% { color: red; } 50% { color: blue; } 100% { color: red; } } </style> </head> <body> <div class='version'>Version: 0.0.0.1</div> </body> </html>";
            return base.Content(tenplate, "text/html");
        }
    }
}
