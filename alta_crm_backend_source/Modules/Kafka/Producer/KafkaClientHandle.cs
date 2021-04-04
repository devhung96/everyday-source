using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Kafka.Producer
{
    public class KafkaClientHandle : IDisposable
    {
        IProducer<byte[], byte[]> kafkaProducer;
        public KafkaClientHandle(IConfiguration configuration)
        {
            var producerConfig = new ProducerConfig()
            {
                BootstrapServers = configuration["Kafka:ProducerSettings:BootstrapServers"]
            };


            kafkaProducer = new ProducerBuilder<byte[], byte[]>(producerConfig).Build();
        }

        public Handle Handle { get => this.kafkaProducer.Handle; }
        public void Dispose()
        {
            kafkaProducer.Flush(); //waiting when have errors or completed
            kafkaProducer.Dispose(); //vứt
        }
    }
}
