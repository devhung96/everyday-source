using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Modules.App.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.App.Mqtt
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestMqttController : ControllerBase
    {
        private readonly ISendScheduleService _sendScheduleService;
        public TestMqttController(IMqttSingletonService mqttSingletonService, ISendScheduleService sendScheduleService)
        {
            _sendScheduleService = sendScheduleService;
        }

        [HttpGet]
        public IActionResult InitMqtt()
        {
            _sendScheduleService.SenMqttSchedule();
            return Ok();
        }
    }
}
