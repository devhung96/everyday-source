using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Controllers;
using Project.App.Kafka;
using Project.Modules.RegisterDetects.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestKafkaDetectController : BaseController
    {
        private readonly KafkaProducer<string, string> _producer;

        public TestKafkaDetectController(KafkaProducer<string, string> producer)
        {
            _producer = producer;
        }

        [HttpGet]
        public async Task<IActionResult> TestAsync()
        {
            RegisterUserDetectRequest registerUserDetectRequest = new RegisterUserDetectRequest
            {
                ModeId = "2",
                RgDectectKey = "2",
                RgDectectExtension = "",
                RgDectectUserId = "2",
                TagCode = "2",
                TicketTypeId = "",
                RegisterDettectDetailRequests = new List<RegisterDettectDetailRequest>
                {
                    new RegisterDettectDetailRequest
                    {
                        RgDectectDetailTimeBegin = "string",
                        RgDectectDetailTimeEnd = "string"
                    }
                }
            };

            string strDataRequest = JsonConvert.SerializeObject(registerUserDetectRequest);
            await _producer.ProduceAsync(TopicDefine.REGISTER_USER_REQUEST, new Message<string, string> { Value = strDataRequest });
            return ResponseOk();
        }


        [HttpGet("test")]
        public async Task<IActionResult> Test1Async()
        {
            RegisterUserDetectRequest registerUserDetectRequest = new RegisterUserDetectRequest
            {
                ModeId = "2",
                RgDectectKey = "2",
                RgDectectExtension = "",
                RgDectectUserId = "2",
                TagCode = "2",
                TicketTypeId = "",
                RegisterDettectDetailRequests = new List<RegisterDettectDetailRequest>
                {
                    new RegisterDettectDetailRequest
                    {
                        RgDectectDetailTimeBegin = "string",
                        RgDectectDetailTimeEnd = "string"
                    }
                }
            };

            string strDataRequest = JsonConvert.SerializeObject(registerUserDetectRequest);
            await _producer.ProduceAsync(TopicDefine.REGISTER_USER_RESPONSE, new Message<string, string> { Value = strDataRequest });
            return ResponseOk();
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDetectRequest request)
        {
            string strDataRequest = JsonConvert.SerializeObject(request);
            await _producer.ProduceAsync(TopicDefine.REGISTER_USER_REQUEST, new Message<string, string> { Value = strDataRequest });
            return ResponseOk();
        }

        [HttpPost("unregister")]
        public async Task<IActionResult> UnRegister([FromBody] UnRegisterUserDetectRequest request)
        {
            string strDataRequest = JsonConvert.SerializeObject(request);
            await _producer.ProduceAsync(TopicDefine.UN_REGISTER_USER_REQUEST, new Message<string, string> { Value = strDataRequest });
            return ResponseOk();
        }


        [HttpPost("unregister-mutil")]
        public async Task<IActionResult> UnRegisterMutil([FromBody] UnRegisterUserDetectMutilRequest request)
        {
            string strDataRequest = JsonConvert.SerializeObject(request);
            await _producer.ProduceAsync(TopicDefine.UN_REGISTER_USER_MUTIL_REQUEST, new Message<string, string> { Value = strDataRequest });
            return ResponseOk();
        }
      
    }
}
