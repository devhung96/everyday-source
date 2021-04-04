using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Project.Modules.Tags.Response;
using Project.Modules.Schedules.Models;
using Project.Modules.Schedules.Services;
using Project.Modules.Tags.Services;
using Project.Modules.Users.Requests;
using Project.Modules.Users.UserKafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Project.Modules.Tags.Requests;

namespace Project.Modules.Kafka.Consumer
{
    public class RequestTimeConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> KafkaConsumerChannels;
        private readonly IEnumerable<string> Channels;
        private readonly HandleTask<RegisterFace> HandleTaskRegister;
        private readonly HandleTask<DetectFace> HandleTaskDetectFace;
        private readonly HandleTask<DeleteFace> HandleTaskDeleteFace;
        public IServiceProvider Services { get; }
        private readonly HandleTaskSchedule<RegisterUserResponse> RegisterUserResponses;


        private readonly ITicketTypeKafkaService _ticketTypeKafkaService;
        private readonly ITagKafkaService _tagKafkaService;


        public RequestTimeConsumer(
            HandleTask<RegisterFace> _HandleTaskRegister,
            IConfiguration configuration,
            IServiceProvider services,
            HandleTaskSchedule<RegisterUserResponse> registerUserResponse,
            HandleTask<DetectFace> handleTaskFace,
            HandleTask<DeleteFace> _HandleTaskDeleteFace,
            ITicketTypeKafkaService ticketTypeKafkaService,
            ITagKafkaService tagKafkaService
            )
        {
            Services = services;
            var consumerConfig = new ConsumerConfig()
            {
                BootstrapServers = configuration["Kafka:ConsumerSettings:BootstrapServers"],
                GroupId = configuration["Kafka:ConsumerSettings:GroupId"],
                AllowAutoCreateTopics = true,//channels
            };
            this.Channels = configuration.GetSection("Kafka:AllowedSucribes").Get<IEnumerable<string>>();
            this.KafkaConsumerChannels = new ConsumerBuilder<string, string>(consumerConfig).Build();

            RegisterUserResponses = registerUserResponse;
            HandleTaskRegister = _HandleTaskRegister;
            HandleTaskDetectFace = handleTaskFace;
            HandleTaskDeleteFace = _HandleTaskDeleteFace;


            _ticketTypeKafkaService = ticketTypeKafkaService;
            _tagKafkaService = tagKafkaService;

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();
            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            KafkaConsumerChannels.Subscribe(Channels);
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var crChannels = this.KafkaConsumerChannels.Consume(cancellationToken);
                    switch (crChannels.Topic)
                    {
                        #region Handel face kafka
                        case "FACE_REMOVE_RESPONSE":
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("FACE_REMOVE_RESPONSE --- " + crChannels.Message.Value);
                                Console.ForegroundColor = ConsoleColor.White;
                                DeleteFace deleteFace = JsonConvert.DeserializeObject<DeleteFace>(crChannels.Message.Value);
                                HandleTaskDeleteFace.Action(deleteFace.Record, deleteFace);
                                break;
                            }

                        case "REGISTER_FACE_RESPONSE":
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("REGISTER_FACE_RESPONSE --- " + crChannels.Message.Value);
                                Console.ForegroundColor = ConsoleColor.White;
                                RegisterFace registerFace = JsonConvert.DeserializeObject<RegisterFace>(crChannels.Message.Value);
                                HandleTaskRegister.Action(registerFace.Record, registerFace);
                                break;
                            }

                        case "DETECT_FACE_RESPONSE":
                            {

                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("DETECT_FACE_RESPONSE --- " + crChannels.Message.Value);
                                Console.ForegroundColor = ConsoleColor.White;
                                DetectFace detectFace = JsonConvert.DeserializeObject<DetectFace>(crChannels.Message.Value);
                                HandleTaskDetectFace.Action(detectFace.Record, detectFace);
                                break;
                            }
                        #endregion End  handel face kafka

                        #region Handel tag
                        case "ADD_TAG_RESPONSE":
                            {
                                
                                Console.WriteLine("ADD_TAG_RESPONSE: " + crChannels.Message.Key);
                                _tagKafkaService.UpdateTagHandelListen(crChannels.Message.Value, crChannels.Message.Key);

                                break;
                            }
                        case "UPDATE_TAG_RESPONSE":
                            {
                                Console.WriteLine("UPDATE_TAG_RESPONSE: " + crChannels.Message.Key);
                                break;
                            }

                        case "REMOVE_TAG_RESPONSE":
                            {
                                Console.WriteLine("REMOVE_TAG_RESPONSE: " + crChannels.Message.Key);
                                break;
                            }
                        #endregion End handel tag

                        #region Kafka To Access Control
                        case "REGISTER_USER_RESPONSE":
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("--- REGISTER_USER_RESPONSE --- " + crChannels.Message.Value);
                                Console.ForegroundColor = ConsoleColor.White;
                                RegisterUserResponse registerUserResponse = JsonConvert.DeserializeObject<RegisterUserResponse>(crChannels.Message.Value);
                                RegisterUserResponses.Action(crChannels.Message.Key, registerUserResponse);
                                break;
                            }

                        case "UN_REGISTER_USER_RESPONSE":
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("--- UN_REGISTER_USER_RESPONSE --- " + crChannels.Message.Value);
                                Console.ForegroundColor = ConsoleColor.White;
                                RegisterUserResponse registerUserResponse = JsonConvert.DeserializeObject<RegisterUserResponse>(crChannels.Message.Value);
                                RegisterUserResponses.Action(crChannels.Message.Key, registerUserResponse);
                                break;
                            }
                        case "UN_REGISTER_USER_MUTIL_RESPONSE":
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("--- UN_REGISTER_USER_MUTIL_RESPONSE --- " + crChannels.Message.Value);
                                Console.ForegroundColor = ConsoleColor.White;
                                RegisterUserResponse registerUserResponse = JsonConvert.DeserializeObject<RegisterUserResponse>(crChannels.Message.Value);
                                RegisterUserResponses.Action(crChannels.Message.Key, registerUserResponse);
                                break;
                            }
                        case "REGISTER_USER_MUTIL_RESPONSE":
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("--- REGISTER_USER_MUTIL_RESPONSE --- " + crChannels.Message.Value);
                                Console.ForegroundColor = ConsoleColor.White;
                                RegisterUserResponse registerUserResponse = JsonConvert.DeserializeObject<RegisterUserResponse>(crChannels.Message.Value);
                                RegisterUserResponses.Action(crChannels.Message.Key, registerUserResponse);
                                break;
                            }
                        case "UN_REGISTER_USER_WITH_USERID_RESPONSE":
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("--- UN_REGISTER_USER_WITH_USERID_RESPONSE --- " + crChannels.Message.Value);
                                Console.ForegroundColor = ConsoleColor.White;
                                RegisterUserResponse registerUserResponse = JsonConvert.DeserializeObject<RegisterUserResponse>(crChannels.Message.Value);
                                RegisterUserResponses.Action(crChannels.Message.Key, registerUserResponse);
                                break;
                            }
                        #endregion  End  Kafka To Access Control

                        #region Ticket Type 
                        case "ADD_TICKET_TYPE":
                            {
                                Console.WriteLine("ADD_TICKET_TYPE: " + crChannels.Message.Key);
                                _ticketTypeKafkaService.CreateTicketTypeHandelListen(crChannels.Message.Value, crChannels.Message.Key);
                                break;
                            }
                            
                        case "UPDATE_TICKET_TYPE":
                            {
                                Console.WriteLine("UPDATE_TICKET_TYPE: " + crChannels.Message.Key);
                                _ticketTypeKafkaService.UpdateTicketTypeHandelListen(crChannels.Message.Value, crChannels.Message.Key);
                                break;
                            }
                            
                        case "REMOVE_TICKET_TYPE":
                            {
                                Console.WriteLine("REMOVE_TICKET_TYPE: " + crChannels.Message.Key);
                                _ticketTypeKafkaService.DeleteTicketTypeHandelListen(crChannels.Message.Value, crChannels.Message.Key);
                                break;
                            }
                        #endregion End TicketType

                        #region Default Log
                        default:
                            {
                                Console.WriteLine($"**OTHER**:{crChannels.Message.Key}");
                                break;
                            }
                       #endregion Default Log

                    }
                   
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
                    Console.WriteLine("Consume Error TryCatch");
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }
    }
}
