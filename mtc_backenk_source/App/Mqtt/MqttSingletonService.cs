using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Project.App.Mqtt
{
    public interface IMqttSingletonService
    {
        Task PingMessage(string topic, string payload, bool retain = true);
        Task SubscribeNewTopicAsync(string chanel);
    }
    public class MqttSingletonService : IMqttSingletonService
    {

        private readonly IConfiguration _configuration;
        private readonly int _portSettting;
        private readonly string _urlSetting;
        private IServiceScopeFactory serviceScopeFactory;

        private readonly IMqttClient _mqttClient;
        private readonly List<string> topics = new List<string>();

        public MqttSingletonService(IConfiguration configuration, IServiceScopeFactory _serviceScopeFactory)
        {
            _configuration = configuration;
            serviceScopeFactory = _serviceScopeFactory;
            _urlSetting = _configuration["Mqtt:Server"];
            _portSettting = int.Parse(_configuration["Mqtt:Port"]);
            topics = _configuration.GetSection("Mqtt:Topic").Get<List<string>>();

            _mqttClient = new MqttFactory().CreateMqttClient();
            var options = new MqttClientOptionsBuilder().WithTcpServer(_urlSetting, _portSettting).Build();


            _mqttClient.UseDisconnectedHandler(async e =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
                await Task.Delay(TimeSpan.FromSeconds(5));

                try
                {
                    await _mqttClient.ConnectAsync(options, CancellationToken.None);
                }
                catch
                {
                    Console.WriteLine("### RECONNECTING FAILED ###");
                }
            });

            _mqttClient.UseApplicationMessageReceivedHandler(e =>
            {
               
                #region Handel topic name
                
                string topicName = e.ApplicationMessage.Topic;
                List<string> arrListStr = new List<string>();
                if (topicName.Contains("/"))
                {
                    arrListStr = topicName.Split('/').ToList();
                    topicName = $"{arrListStr[0]}/{arrListStr[1]}";
                }

                #endregion


                #region Handle response 
                switch (topicName)
                {

                    case TopicDefine.ONLINE_DEVICE:
                        {
                            try
                            {
                                if (e.ApplicationMessage.Payload != null)
                                {
                                    string dataPrepare = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                                    Console.WriteLine($"Mqtt listen ---> ONLINE_DEVICE:{dataPrepare}");
                                    MqttOnlineDevideResponse dataInput = JsonConvert.DeserializeObject<MqttOnlineDevideResponse>(dataPrepare);
                                    if (dataInput.status is null) break;
                                    using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
                                    var deviceService = serviceScope.ServiceProvider.GetRequiredService<IDeviceServices>();
                                    (DeviceResponse data, string message) = deviceService.ChangePowerDevice(arrListStr[2], dataInput.status.Value);
                                }
                            }
                            catch (Exception ex)
                            {

                                PingMessage("Error:ONLINE_DEVICE", ex.Message);
                            }
                           
                               

                            break;
                        }
                    case TopicDefine.STATUS_DEVICE:
                        {
                            Console.WriteLine($"Mqtt listen ---> STATUS_DEVICE:{e.ApplicationMessage.Payload}");
                            break;
                        }
                    case TopicDefine.RESTART_DEVICE:
                        {
                            Console.WriteLine($"Mqtt listen ---> RESTART_DEVICE:{e.ApplicationMessage.Payload}");
                            break;
                        }
                    case TopicDefine.DELETE_MEMORY_DEVICE:
                        {
                            Console.WriteLine($"Mqtt listen ---> DELETE_MEMORY_DEVICE:{e.ApplicationMessage.Payload}");
                            break;
                        }
                    case TopicDefine.LOCK_DEVICE:
                        {
                            Console.WriteLine($"Mqtt listen ---> LOCK_DEVICE:{e.ApplicationMessage.Payload}");
                            break;
                        }




                    #region Default Log
                    default:
                    {
                        Console.WriteLine($"**OTHER**:{e.ApplicationMessage.Topic}");
                        break;
                    }
                    #endregion Default Log
                }
                #endregion End:  Handle response 
            });

            _mqttClient.UseConnectedHandler(async e =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");

                // Subscribe to a topic
                foreach (var item in topics)
                {
                    await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(item).Build());
                }

                Console.WriteLine("### SUBSCRIBED ###");
            });

            _ = _mqttClient.ConnectAsync(options, CancellationToken.None).Result;
        }
        public async Task SubscribeNewTopicAsync(string topic)
        {
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            _ = PingMessage(topic, "Ping new topic");
        }

        public async Task PingMessage(string topic, string payload, bool retain = true)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(payload)
                .WithExactlyOnceQoS()
                .WithRetainFlag(retain)
                .Build();
            await _mqttClient.PublishAsync(message, CancellationToken.None);
        }

    }
}
