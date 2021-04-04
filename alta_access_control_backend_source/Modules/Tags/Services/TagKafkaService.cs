using AutoMapper;
using Confluent.Kafka;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Kafka;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.Tags.Entities;
using Project.Modules.Tags.Requests;
using Project.Modules.Tags.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Services
{
    public interface ITagKafkaService
    {
        public void AddTag(string strData, string key);

        public void UpdateTag(string strData, string key);

        public void RemoveTag(string strData, string key);
    }
    public class TagKafkaService : ITagKafkaService
    {
        private readonly KafkaProducer<string, string> producer;
        private readonly ITagService tagService;
        private readonly IMapper mapper;

        private readonly IRepositoryWrapperMariaDB repository;


        public TagKafkaService(KafkaProducer<string, string> producer, ITagService tagService, IMapper mapper, IRepositoryWrapperMariaDB repository)
        {
            this.producer = producer;
            this.tagService = tagService;
            this.mapper = mapper;

            this.repository = repository;
        }

        public  void  AddTag(string strData, string key)
        {
            try
            {
                AddTagRequest addTagRequest = JsonConvert.DeserializeObject<AddTagRequest>(strData);
                (Tag tag, string message) = tagService.Store(addTagRequest,true);

                KafkaRequest kafkaRequest = new KafkaRequest()
                {
                    IsSuccess = false,
                    Message = message
                };
                if (tag != null)
                {
                    kafkaRequest.IsSuccess = true;
                    kafkaRequest.DataResult = JObject.FromObject(new { RepositoryId = tag.RepositoryId , TagCode = tag.TagCode }).ToString();
                }
                 producer.Produce(TopicDefine.ADD_TAG_RESPONSE, new Message<string, string> { Key = key, Value = JObject.FromObject(kafkaRequest).ToString() });
            }
            catch (Exception ex)
            {
                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    DataResult = "",
                    IsSuccess = false,
                    Message = $"Erro:Exception:{ex.Message}"
                };
                 producer.Produce(TopicDefine.ADD_TAG_RESPONSE, new Message<string, string> { Key = key, Value = JObject.FromObject(kafkaRequest).ToString() });
            }
        }
        public void UpdateTag(string strData, string key)
        {
            try
            {
                UpdateTagRequest updateTagRequest = JsonConvert.DeserializeObject<UpdateTagRequest>(strData);
                (object tag, string message) = tagService.UpdateTagByCode(updateTagRequest);


                KafkaRequest kafkaRequest = new KafkaRequest()
                {
                    IsSuccess = false,
                    Message = message
                };

                if(tag != null)
                {
                    kafkaRequest.IsSuccess = true;
                    kafkaRequest.DataResult = JObject.FromObject(tag).ToString();
                }
                producer.Produce(TopicDefine.UPDATE_TAG_RESPONSE, new Message<string, string> { Key = key, Value = JObject.FromObject(kafkaRequest).ToString()});
            }
            catch (Exception ex)
            {
                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    DataResult = "",
                    DataRequest = strData,
                    IsSuccess = false,
                    Message = $"Erro:Exception:{ex.Message}"

                };
                producer.Produce(TopicDefine.UPDATE_TAG_RESPONSE, new Message<string, string> { Key = key, Value = JObject.FromObject(kafkaRequest).ToString() });
            }
        }
        public void RemoveTag(string strData, string key)
        {
            try
            {
                UpdateTagRequest tagRequest = JsonConvert.DeserializeObject<UpdateTagRequest>(strData);

                (Tag tag, string message) = tagService.DeleteByCode(tagRequest.TagCode);
                KafkaRequest kafkaRequest = new KafkaRequest() 
                { 
                    IsSuccess = false,
                    Message = message
                };
                if (tag != null)
                {
                    kafkaRequest.IsSuccess = true;
                    kafkaRequest.Message = message;
                }
                producer.Produce(TopicDefine.REMOVE_TAG_RESPONSE, new Message<string, string> { Key = key, Value = JObject.FromObject(kafkaRequest).ToString() });


            }
            catch (Exception ex)
            {
                KafkaRequest kafkaRequest = new KafkaRequest
                {
                    DataRequest = strData,
                    IsSuccess = false,
                    Message = $"Erro:Exception:{ex.Message}",
                };
                producer.Produce(TopicDefine.REMOVE_TAG_RESPONSE, new Message<string, string> { Key = key, Value = JObject.FromObject(kafkaRequest).ToString() });
            }
        }

    }
}
