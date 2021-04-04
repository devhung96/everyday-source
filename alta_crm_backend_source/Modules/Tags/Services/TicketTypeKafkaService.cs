using Newtonsoft.Json;
using Project.Modules.Kafka.Producer;
using Project.Modules.Tags.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Services
{
    public interface ITicketTypeKafkaService
    {
        public void CreateTicketTypeHandelListen(string strData, string keyKafkaRequest);
        public void UpdateTicketTypeHandelListen(string strData, string keyKafkaRequest);
        public void DeleteTicketTypeHandelListen(string strData, string keyKafkaRequest);
    }
    public class TicketTypeKafkaService : ITicketTypeKafkaService
    {

        private readonly KafkaDependentProducer<string, string> _producer;


        private readonly ITicketTypeService _ticketTypeService;


        public TicketTypeKafkaService(KafkaDependentProducer<string, string> producer, ITicketTypeService ticketTypeService)
        {
            _producer = producer;
            _ticketTypeService = ticketTypeService;
        }


        public void CreateTicketTypeHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                AddTicketRequest newTicketRequest = JsonConvert.DeserializeObject<AddTicketRequest>(strData);
               (object data, string message) = _ticketTypeService.InsertTicket(newTicketRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CreateTicketTypeHandelListen:Error:{ex.Message}{ex.InnerException}");
            }
            
        }

        public void UpdateTicketTypeHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                UpdateTicketRequet updateTicketRequest = JsonConvert.DeserializeObject<UpdateTicketRequet>(strData);
                (object data, string message) = _ticketTypeService.UpdateTicketType(updateTicketRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateTicketTypeHandelListen:Error:{ex.Message}{ex.InnerException}");
            }
        }

        public void DeleteTicketTypeHandelListen(string strData, string keyKafkaRequest)
        {
            try
            {
                DeleteTicketRequest deleteTicketRequest = JsonConvert.DeserializeObject<DeleteTicketRequest>(strData);
                (object data, string message) = _ticketTypeService.DeleteTicketType(deleteTicketRequest.TicketTypeId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteTicketTypeHandelListen:Error:{ex.Message}{ex.InnerException}");
            }
        }

    }
}
