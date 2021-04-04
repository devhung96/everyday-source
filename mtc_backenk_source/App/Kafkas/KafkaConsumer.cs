using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Medias.Requests;
using Project.Modules.Medias.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project.App.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> _kafkaConsumer;
        private readonly IEnumerable<string> _topics;
        private readonly IMediaSupport mediaSupport;
        public KafkaConsumer(IConfiguration configuration, IMediaSupport mediaSupport)
        {
            var consumerConfig = new ConsumerConfig() { 
            GroupId =configuration["OutsideSystems:Kafka:ConsumerSettings:GroupId"],
            BootstrapServers = configuration["OutsideSystems:Kafka:ConsumerSettings:BootstrapServers"]
            };
            this.mediaSupport = mediaSupport;
            _kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            this._topics = configuration.GetSection("OutsideSystems:Kafka:AllowedSucribes").Get<IEnumerable<string>>();

        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            _kafkaConsumer.Subscribe(_topics);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = _kafkaConsumer.Consume(cancellationToken);
                    string message = cr.Message.Value;
                    #region Handle response kafka
                    switch (cr.Topic)
                    {
                        case TopicDefine.APPROVE_MEDIAS:
                            {
                                Console.WriteLine("--Listen-------APPROVE_MEDIAS---------");

                                ShareMediasAllGroupRequest shareMedias = JsonConvert.DeserializeObject<ShareMediasAllGroupRequest>(message);
                                (object data,  _) =  mediaSupport.ApproveMedias(shareMedias);
                              
                                Console.WriteLine("--Handle----APPROVE_MEDIAS------" + data.ToString());
                            }
                            break;
                        case TopicDefine.SHARE_MEDIA_ALL_GROUP:
                            {
                                Console.WriteLine("--Listen-------SHARE_MEDIA_ALL_GROUP---------");
                                ShareMediasAllGroupRequest shareMedias = JsonConvert.DeserializeObject<ShareMediasAllGroupRequest>(message);
                                (object data, _) =mediaSupport.ShareListMediasAllGroup(shareMedias);
                                Console.WriteLine("--Handle-------SHARE_MEDIA_ALL_GROUP---------: " + data.ToString());
                            }
                            break;

                    }
                    #endregion End:  Handle response kafka

                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }

        public override void Dispose()
        {
            _kafkaConsumer.Close();
            _kafkaConsumer.Dispose();

            base.Dispose();
        }

    }
}
