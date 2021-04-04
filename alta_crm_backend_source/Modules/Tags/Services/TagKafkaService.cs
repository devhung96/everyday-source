using Newtonsoft.Json;
using Project.Modules.Kafka.Producer;
using Project.Modules.Tags.Requests;
using Project.Modules.Tags.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Services
{

    public interface ITagKafkaService
    {
        public void CreateTagHandelListen(string strData, string keyKafkaRequest);
        public void UpdateTagHandelListen(string strData, string keyKafkaRequest);
        public void RemoveTagHandelListen(string strData, string keyKafkaRequest);
    }
    public class TagKafkaService: ITagKafkaService
    {
        private readonly KafkaDependentProducer<string, string> _producer;


        private readonly ITagService _tagService;

        public TagKafkaService(KafkaDependentProducer<string, string> producer, ITagService tagService)
        {
            _producer = producer;
            _tagService = tagService;
        }

        public void CreateTagHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                BaseTagKafkaResponse baseTbaseResponseKafkaagKafka = JsonConvert.DeserializeObject<BaseTagKafkaResponse>(strData);
               
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateTicketTypeHandelListen:Error:{ex.Message}{ex.InnerException}");
            }

        }

        public void UpdateTagHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                BaseTagKafkaResponse baseResponseKafka = JsonConvert.DeserializeObject<BaseTagKafkaResponse>(strData);
                if (!String.IsNullOrEmpty(baseResponseKafka.DataResult))
                {
                    InsertTagKafkaResponse updateTagKafkaResponse = JsonConvert.DeserializeObject<InsertTagKafkaResponse>(baseResponseKafka.DataResult);
                    _tagService.UpdateReponsitory(tagCode: updateTagKafkaResponse.TagCode,repositoryId: updateTagKafkaResponse.RepositoryId);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTagHandelListen:Error:{ex.Message}{ex.InnerException}");
            }

        }

        public void RemoveTagHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                BaseTagKafkaResponse baseResponseKafka = JsonConvert.DeserializeObject<BaseTagKafkaResponse>(strData);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"RemoveTagHandelListen:Error:{ex.Message}{ex.InnerException}");
            }

        }


    }
}
