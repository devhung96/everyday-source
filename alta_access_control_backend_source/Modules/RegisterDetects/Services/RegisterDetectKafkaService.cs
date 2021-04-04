using AutoMapper;
using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Kafka;
using Project.Modules.RegisterDetects.Requests;
using Project.Modules.Tags.Entities;
using Project.Modules.Tags.Requests;
using Project.Modules.Tags.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Services
{
    public interface IRegisterDetectKafkaService
    {
        public Task RegisterDetectUserHandelListen(string strData, string keyKafka);
        public Task RegisterDetectUserMutiHandelListen(string strData, string keyKafkaRequest);

        public Task UnRegisterDetectUserHandelListen(string strData, string keyKafka);
        public Task UnRegisterDetectUserMutiHandelListen(string strData, string keyKafkaRequest);
        public Task UnRegisterDetectUserWithUserIdHandelListen(string strData, string keyKafkaRequest);
    }

    public class RegisterDetectKafkaService : IRegisterDetectKafkaService
    {
        private readonly KafkaProducer<string, string> _producer;
        private readonly IRegisterDetectService _registerDetectService;
        public RegisterDetectKafkaService(KafkaProducer<string, string> producer, IRegisterDetectService registerDetectService)
        {
            _producer = producer;
            _registerDetectService = registerDetectService;
        }

      

        public async Task RegisterDetectUserHandelListen(string strData , string keyKafkaRequest)
        {
            try
            {
                RegisterUserDetectRequest prepareData = JsonConvert.DeserializeObject<RegisterUserDetectRequest>(strData);
                (object result , string message) =  _registerDetectService.RegisterUserDetect(prepareData);

                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    IsSuccess = false,
                    Message = message
                };

                if (result != null)
                {
                    kafkaRequest.DataResult = JsonConvert.SerializeObject(result);
                    kafkaRequest.IsSuccess = true;
                }
                var resKafka =  await _producer.ProduceAsync(TopicDefine.REGISTER_USER_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString()});
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"RegisterDetectUserHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
            catch (Exception ex)
            {
                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    IsSuccess = false,
                    Message = $"Error:Exception:{ex.Message}"
                };
                var resKafka = await _producer.ProduceAsync(TopicDefine.REGISTER_USER_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString() });
                if(resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"RegisterDetectUserHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
        }

        public async Task RegisterDetectUserMutiHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                RegisterUserDetectMutilRequest prepareData = JsonConvert.DeserializeObject<RegisterUserDetectMutilRequest>(strData);
                (object result, string message) = _registerDetectService.RegisterUserDetectMutil(prepareData);

                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    IsSuccess = false,
                    Message = message
                };

                if (result != null)
                {
                    kafkaRequest.DataResult = JsonConvert.SerializeObject(result);
                    kafkaRequest.IsSuccess = true;
                }
                var resKafka = await _producer.ProduceAsync(TopicDefine.REGISTER_USER_MUTIL_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString() });
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"RegisterDetectUserMutiHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
            catch (Exception ex)
            {
                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    IsSuccess = false,
                    Message = $"Error:Exception:{ex.Message}"
                };
                var resKafka = await _producer.ProduceAsync(TopicDefine.REGISTER_USER_MUTIL_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString() });
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"RegisterDetectUserMutiHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
        }


        public async Task UnRegisterDetectUserHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                UnRegisterUserDetectRequest prepareData = JsonConvert.DeserializeObject<UnRegisterUserDetectRequest>(strData);
                (bool result, string message) = _registerDetectService.UnRegisterUserDetect(prepareData);

                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    IsSuccess = false,
                    Message = message
                };

                if (result)
                {
                    kafkaRequest.DataResult = "";
                    kafkaRequest.IsSuccess = true;
                }

                var resKafka =  await _producer.ProduceAsync(TopicDefine.UN_REGISTER_USER_RESPONSE, new Message<string, string> { Key= keyKafkaRequest,  Value = JObject.FromObject(kafkaRequest).ToString() });
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"Error:Produce:Kafka");
                }
            }
            catch (Exception ex)
            {
                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    IsSuccess = false,
                    Message = $"Error:Exception:{ex.Message}"
                };
                var resKafka =  await _producer.ProduceAsync(TopicDefine.UN_REGISTER_USER_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString() });
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"UnRegisterDetectUserHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
        }


        public async Task UnRegisterDetectUserMutiHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                UnRegisterUserDetectMutilRequest prepareData = JsonConvert.DeserializeObject<UnRegisterUserDetectMutilRequest>(strData);
                (bool result, string message) = _registerDetectService.UnRegisterUserMutil(prepareData);

                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    DataResult = "",
                    IsSuccess = false,
                    Message = message
                };

                if (result)
                {
                    kafkaRequest.DataResult = "";
                    kafkaRequest.IsSuccess = true;
                }

                var resKafka = await _producer.ProduceAsync(TopicDefine.UN_REGISTER_USER_MUTIL_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString() });
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"UnRegisterDetectUserMutiHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
            catch (Exception ex)
            {
                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    DataResult = "",
                    IsSuccess = false,
                    Message = $"Error:Exception:{ex.Message}"
                };
                var resKafka = await _producer.ProduceAsync(TopicDefine.UN_REGISTER_USER_MUTIL_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString() });
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"UnRegisterDetectUserMutiHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
        }
    
        public  async Task UnRegisterDetectUserWithUserIdHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                (bool result, string message) = _registerDetectService.UnRegisterUserWithUserId(strData);

                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    DataResult = "",
                    IsSuccess = false,
                    Message = message
                };

                if (result)
                {
                    kafkaRequest.IsSuccess = true;
                }

                var resKafka = await _producer.ProduceAsync(TopicDefine.UN_REGISTER_USER_WITH_USERID_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString() });
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"UnRegisterDetectUserWithUserIdHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
            catch (Exception ex)
            {
                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    DataResult = "",
                    IsSuccess = false,
                    Message = $"Error:Exception:{ex.Message}"
                };
                var resKafka =  await _producer.ProduceAsync(TopicDefine.UN_REGISTER_USER_WITH_USERID_RESPONSE, new Message<string, string> { Key = keyKafkaRequest, Value = JObject.FromObject(kafkaRequest).ToString() });
                if (resKafka.Status == PersistenceStatus.NotPersisted)
                {
                    Console.WriteLine($"UnRegisterDetectUserWithUserIdHandelListen:Error:Produce:Kafka:Key:{keyKafkaRequest}");
                }
            }
        }


    }


}
