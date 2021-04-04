using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;

namespace Project.App.Kafka
{
    public class KafkaClientHandle : IDisposable
    {
        private readonly IProducer<byte[], byte[]> _kafkaProducer;
        private readonly IConfiguration _configuration;
        public KafkaClientHandle(IConfiguration configuration)
        {
            _configuration = configuration;

            var conf = new ProducerConfig()
            {
                ClientId = Dns.GetHostName(),
                BootstrapServers = configuration["OutsideSystems:Kafka:ProducerSettings:BootstrapServers"],
            };
                
            _kafkaProducer = new ProducerBuilder<byte[], byte[]>(conf).Build();
        }

        public Handle Handle { get => _kafkaProducer.Handle; }

        public void Dispose()
        {
            _kafkaProducer.Flush();
            _kafkaProducer.Dispose();
        }

    }
}
