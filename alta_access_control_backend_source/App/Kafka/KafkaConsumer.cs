using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Detections.Models;
using Project.Modules.Detections.Services;
using Project.Modules.RegisterDetects.Services;
using Project.Modules.Tags.Services;
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
        private readonly IRegisterDetectKafkaService _registerDetectKafkaService;
        private readonly ITagKafkaService tagKafkaService;
        private readonly HandleTask<DetectFace> HandleTaskDetect;

        public KafkaConsumer(IConfiguration configuration, IRegisterDetectKafkaService registerDetectKafkaService, HandleTask<DetectFace> handleTaskDetect, ITagKafkaService tagKafkaService)
        {
            var consumerConfig = new ConsumerConfig();
            configuration.GetSection("OutsideSystems:Kafka:ConsumerSettings").Bind(consumerConfig);
            _kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            this._topics = configuration.GetSection("OutsideSystems:Kafka:ConsumerSettings:Topics").Get<IEnumerable<string>>();
            _registerDetectKafkaService = registerDetectKafkaService;
            this.tagKafkaService = tagKafkaService;
            HandleTaskDetect = handleTaskDetect;
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

                    #region Handle response kafka
                    switch (cr.Topic)
                    {
                        #region Register user detect
                        case TopicDefine.REGISTER_USER_REQUEST:
                            {
                                Console.WriteLine($"Kafka listen ---> REGISTER_USER_REQUEST:{cr.Message.Key}");
                                _registerDetectKafkaService.RegisterDetectUserHandelListen(cr.Message.Value, cr.Message.Key);
                                break;
                            }

                        case TopicDefine.REGISTER_USER_MUTIL_REQUEST:
                            {
                                Console.WriteLine($"Kafka listen ---> REGISTER_USER_MUTIL_REQUEST:{cr.Message.Key}");
                                _registerDetectKafkaService.RegisterDetectUserMutiHandelListen(cr.Message.Value, cr.Message.Key);
                                break;
                            }

                        case TopicDefine.UN_REGISTER_USER_REQUEST:
                            {
                                Console.WriteLine($"Kafka listen ---> UN_REGISTER_USER_REQUEST:{cr.Message.Key}");
                                _registerDetectKafkaService.UnRegisterDetectUserHandelListen(cr.Message.Value, cr.Message.Key);
                                break;
                            }

                        case TopicDefine.UN_REGISTER_USER_MUTIL_REQUEST:
                            {
                                Console.WriteLine($"Kafka listen ---> UN_REGISTER_USER_MUTIL_REQUEST:{cr.Message.Key}");
                                _registerDetectKafkaService.UnRegisterDetectUserMutiHandelListen(cr.Message.Value, cr.Message.Key);
                                break;
                            }

                        case TopicDefine.UN_REGISTER_USER_WITH_USERID_REQUEST:
                            {
                                Console.WriteLine($"Kafka listen ---> UN_REGISTER_USER_WITH_USERID_REQUEST:{cr.Message.Key}");
                                _registerDetectKafkaService.UnRegisterDetectUserWithUserIdHandelListen(cr.Message.Value, cr.Message.Key);
                                break;
                            }



                        #endregion End register user detect

                        #region Tag
                        case TopicDefine.ADD_TAG:
                            {
                                Console.WriteLine($"Kafka listen ---> ADD_TAG---: {cr.Message.Key}");
                                tagKafkaService.AddTag(cr.Message.Value, cr.Message.Key);
                                break;
                            }
                        case TopicDefine.UPDATE_TAG:
                            {
                                Console.WriteLine($"Kafka listen ---> UPDATE_TAG---:{cr.Message.Key} ");
                                tagKafkaService.UpdateTag(cr.Message.Value, cr.Message.Key);
                                break;
                            }
                        case TopicDefine.REMOVE_TAG:
                            {
                                Console.WriteLine($"Kafka listen ---> REMOVE_TAG---:{cr.Message.Key} ");
                                tagKafkaService.RemoveTag(cr.Message.Value, cr.Message.Key);
                                break;
                            }
                      
                        case TopicDefine.DETECT_FACE_RESPONSE:
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("DETECT_FACE_RESPONSE --- " + cr.Message.Key);
                                Console.ForegroundColor = ConsoleColor.White;
                                DetectFace detectFace = new DetectFace();
                                detectFace = JsonConvert.DeserializeObject<DetectFace>(cr.Message.Key);
                                HandleTaskDetect.Action(detectFace.Record, detectFace);
                                break;
                            }
                        #endregion

                        #region Default Log
                        default:
                            {
                                Console.WriteLine($"**OTHER**:{cr.Message.Key}");
                                break;
                            }
                            #endregion Default Log
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
