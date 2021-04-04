using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.App.Database;

namespace Project.Modules.Logs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : BaseController
    {
        private readonly MongoDBContext mongoDBContext;
        public LogController(MongoDBContext MongoDBContext)
        {
            mongoDBContext = MongoDBContext;
        }

        public IActionResult GetLog()
        {
            if (!int.TryParse(Request.Query["limit"], out int limit))
            {
                limit = 100;
            }

            return ResponseOk(mongoDBContext.LogAccesses.Find(_ => true).ToList()
                .OrderByDescending(x => x.CreatedAt)
                .Take(limit)
                .Select(x => new {
                    x.IpAddress,
                    Header = string.IsNullOrEmpty(x.Header as string) ? null : JsonConvert.DeserializeObject(x.Header as string),
                    x.Query,
                    Body = string.IsNullOrEmpty(x.Body as string) ? null : JsonConvert.DeserializeObject(x.Body as string),
                    Response = string.IsNullOrEmpty(x.Response as string) ? null : JsonConvert.DeserializeObject(x.Response as string),
                    Time = x.CreatedAt
                }));
        }
    }
}