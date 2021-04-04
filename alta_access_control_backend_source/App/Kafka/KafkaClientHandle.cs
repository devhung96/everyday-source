using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;

namespace Project.App.Kafka
{
    public class KafkaClientHandle : IDisposable
    {
        private readonly IProducer<byte[], byte[]> _kafkaProducer;
        private readonly IConfiguration _configuration;
        public KafkaClientHandle(IConfiguration configuration)
        {
            _configuration = configuration;

            var conf = new ProducerConfig();
            configuration.GetSection("OutsideSystems:Kafka:ProducerSettings").Bind(conf);
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
