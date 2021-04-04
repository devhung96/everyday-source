using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Kafka.Producer
{
    /// <summary>
    /// Sẵn có KafkaClientHandle. tạo Kafka.Message{K,V} của producer cho kafka
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class KafkaDependentProducer<K, V>
    {
        IProducer<K, V> kafkaHandle;
        public KafkaDependentProducer(KafkaClientHandle handle)
        {
            kafkaHandle = new DependentProducerBuilder<K, V>(handle.Handle).Build();
        }

        //Asynchronously send a single message to a Kafka topic. The partition the message is sent to is determined by the partitioner defined using the 'partitioner' configuration  property.
        //gửi tin nhắn bất đồng bộ cho kênh subcribe
        public Task ProduceAsync(string topic, Message<K, V> message) => this.kafkaHandle.ProduceAsync(topic, message);



        public void Produce(string topic, Message<K, V> message, Action<DeliveryReport<K, V>> deliveryHanle = null)
            => this.kafkaHandle.Produce(topic,message, deliveryHanle);



        public void Flush(TimeSpan timeout)
            => this.kafkaHandle.Flush(timeout);
    }
}
