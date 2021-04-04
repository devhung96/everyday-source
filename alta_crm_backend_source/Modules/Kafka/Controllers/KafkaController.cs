using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.App.Controllers;
using Project.Modules.Kafka.Producer;
using Project.Modules.Kafka.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Kafka.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KafkaController : BaseController
    {
        private readonly IConfiguration Configuration;
        private string topic;
        private readonly KafkaDependentProducer<string, string> Producer;
        public KafkaController(IConfiguration configuration, KafkaDependentProducer<string, string> producer)
        {
            Configuration = configuration;
            this.topic = configuration["Kafka:FrivolousTopic"];
            Producer = producer;
        }

        [HttpGet]
        public async Task<IActionResult> KafkaTest()
        {
            Message<string, string> message = new Message<string, string> { Key = "Test", Value = "cannot be tracked because another instance with the same key value for is already being tracked" };

            //for (int i = 0; i < 2; i++)
            //{
                this.Producer.Produce("UPDATE_TAG_RESPONSE", message, deliveryReportHandleString);
            //}

            Console.WriteLine(topic);
            return Ok(1);
        }

        [HttpPost]
        public async Task<IActionResult> SendKafka([FromBody] ProducerRequest producerRequest)
        {
            Message<string, string> message = new Message<string, string> { Key = null, Value = JsonConvert.SerializeObject(producerRequest.Request) };

            Producer.Produce(producerRequest.Producer, message, deliveryReportHandleString);
            return Ok(1);
        }
        private void deliveryReportHandleString(DeliveryReport<string, string> deliveryReport)
        {
            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                Console.WriteLine($"Message delivery failed: {deliveryReport.Message.Value}");
            }
        }
        private void deliveryReportHandleNull(DeliveryReport<Null, string> deliveryReport)
        {
            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                Console.WriteLine($"Message delivery failed: {deliveryReport.Message.Value}");
            }
        }
    }
}
