using AutoMapper;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.App.Kafka;
using Project.Modules.Devices.Entities;
using Project.Modules.Tags.Entities;
using Project.Modules.TicketDevices.Entities;
using Project.Modules.Tickets.Entities;
using Project.Modules.TicketTypes.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tickets.Services
{
    public interface ITicketTypeService
    {
        Task<(TicketType ticket, string messge)> AddTicketAsync(AddTicketTypeRequest request);
        Task<(TicketType ticket, string messge)> UpdateTicketAsync(UpdateTicketTypeRequest request, string tickeTypeId);
        Task<(TicketType ticket, string messge)> DeleteTicketAsync(string Id);
        (TicketType ticketType, string message) Detail(string Id);
        PaginationResponse<TicketType> ShowTable(PaginationRequest request);
        PaginationResponse<Tag> ShowTagInTicketType(PaginationRequest request, string tickKetID);

        #region Function test
        Task<object> TestAddMutiTicketTypesAsync();
        Task<object> TestDeleteMutiTicketTypesAsync();
        #endregion
    }
    public class TicketTypeService : ITicketTypeService
    {
        private readonly IRepositoryWrapperMariaDB _repositoryWrapperMariaDB;
        private readonly IMapper mapper;
        private readonly KafkaProducer<string, string> producer;
        public TicketTypeService(IRepositoryWrapperMariaDB reponsitory, KafkaProducer<string, string> producer, IMapper mapper)
        {
            this.producer = producer;
            this._repositoryWrapperMariaDB = reponsitory;
            this.mapper = mapper;
        }

        public async Task<(TicketType ticket, string messge)> AddTicketAsync(AddTicketTypeRequest request)
        {
            TicketType ticketType = new TicketType()
            {
                TicketTypeName = request.TicketTypeName,
                TicketTypeDescription = request.TicketTypeDescription
            };
            _repositoryWrapperMariaDB.TicketTypes.Add(ticketType);
            _repositoryWrapperMariaDB.SaveChanges();

            List<TicketTypeDevice> ticketDevices = new List<TicketTypeDevice>();
            foreach (string deviceId in request.DeviceIds)
            {
                ticketDevices.Add(new TicketTypeDevice()
                {
                    TicketTypeId = ticketType.TicketTypeId,
                    DeviceId = deviceId
                });
            }
            if (ticketDevices.Count > 0)
            {
                _repositoryWrapperMariaDB.TicketTypeDevices.AddRange(ticketDevices);
                _repositoryWrapperMariaDB.SaveChanges();
            }

            PublishTicketTypeRequest publishTicketType = mapper.Map<PublishTicketTypeRequest>(request);
            publishTicketType.TicketTypeId = ticketType.TicketTypeId;

            producer.Produce(TopicDefine.ADD_TICKET_TYPE, new Message<string, string>()
                                                                                    {
                                                                                        Key = DateTime.UtcNow.Ticks.ToString(),
                                                                                        Value = JObject.FromObject(publishTicketType).ToString()
                                                                                    });
           
            return (ticketType, "AddSuccess");
        }

        public async Task<(TicketType ticket, string messge)> DeleteTicketAsync(string Id)
        {
            TicketType ticketType = _repositoryWrapperMariaDB.TicketTypes.GetById(Id);
            if (ticketType is null)
            {
                return (null, "TicketTypeNotExist");
            }
            _repositoryWrapperMariaDB.TicketTypes.Remove(ticketType);
            _repositoryWrapperMariaDB.SaveChanges();

            producer.Produce(TopicDefine.REMOVE_TICKET_TYPE,
                                                             new Message<string, string>()
                                                             {
                                                                 Key = DateTime.UtcNow.Ticks.ToString(),
                                                                 Value = JObject.FromObject(new DeleteTicketTypeRequest()
                                                                 {
                                                                     TicketTypeId = ticketType.TicketTypeId
                                                                 }).ToString()
                                                             });
          
            return (ticketType, "RemoveSuccess");
        }

        public (TicketType ticketType, string message) Detail(string Id)
        {
            TicketType ticketType = _repositoryWrapperMariaDB.TicketTypes.GetById(Id);

            if (ticketType is null)
            {
                return (null, "TicketTypeNotExist");
            }
            ticketType.Devices = _repositoryWrapperMariaDB.TicketTypeDevices
                                            .FindByCondition(x => x.TicketTypeId.Equals(Id))
                                            .Include(x => x.Device)
                                            .Select(x => x.Device)
                                            .ToList();
            return (ticketType, "Success");
        }

        public PaginationResponse<TicketType> ShowTable(PaginationRequest request)
        {
            IQueryable<TicketType> data = _repositoryWrapperMariaDB.TicketTypes.FindByCondition(x => string.IsNullOrEmpty(request.SearchContent)
                                                               || (x.TicketTypeName.ToLower().Contains(request.SearchContent.ToLower()))
                                                              );
            data = SortHelper<TicketType>.ApplySort(data, request.OrderByQuery);
            var result = PaginationHelper<TicketType>.ToPagedList(data, request.PageNumber, request.PageSize);
            PaginationResponse<TicketType> response = new PaginationResponse<TicketType>(data, result.PageInfo);

            foreach (TicketType ticketType in response.PagedData.ToList())
            {
                List<TicketTypeDevice> devices = _repositoryWrapperMariaDB.TicketTypeDevices
                                                            .FindByCondition(x => x.TicketTypeId.Equals(ticketType.TicketTypeId))
                                                            .ToList();
                ticketType.DeviceIds = devices.Count > 0 ? devices.Select(x => x.DeviceId).ToList() : null;
            }
            return response;
        }

        public PaginationResponse<Tag> ShowTagInTicketType(PaginationRequest request, string tickKetID)
        {
            IQueryable<Tag> data = _repositoryWrapperMariaDB.Tags.FindByCondition(x => x.TicketTypeId.Equals(tickKetID));
            data = SortHelper<Tag>.ApplySort(data, request.OrderByQuery);
            var result = PaginationHelper<Tag>.ToPagedList(data, request.PageNumber, request.PageSize);
            PaginationResponse<Tag> response = new PaginationResponse<Tag>(data, result.PageInfo);
            return response;
        }

        public async Task<(TicketType ticket, string messge)> UpdateTicketAsync(UpdateTicketTypeRequest request, string ticketTypeId)
        {

            TicketType ticketType = _repositoryWrapperMariaDB.TicketTypes.GetById(ticketTypeId);
            if (ticketType is null)
            {
                return (null, "TicketTypeNotExist");
            }
            mapper.Map(request, ticketType);
            List<TicketTypeDevice> deviceDb = _repositoryWrapperMariaDB.TicketTypeDevices
                                                         .FindByCondition(x => x.TicketTypeId.Equals(ticketType.TicketTypeId))
                                                         .ToList();

            _repositoryWrapperMariaDB.TicketTypeDevices.RemoveRange(deviceDb);
            _repositoryWrapperMariaDB.SaveChanges();

            List<TicketTypeDevice> devicesNew = new List<TicketTypeDevice>();
            foreach (string deviceId in request.DeviceIds)
            {
                devicesNew.Add(new TicketTypeDevice() { TicketTypeId = ticketTypeId, DeviceId = deviceId });
            }

            _repositoryWrapperMariaDB.TicketTypeDevices.AddRange(devicesNew);
            //_repositoryWrapperMariaDB.TicketTypes.Update(ticketType);
            _repositoryWrapperMariaDB.SaveChanges();

            request.TicketTypeId = ticketTypeId;

            producer.Produce(TopicDefine.UPDATE_TICKET_TYPE,
                                                                new Message<string, string>()
                                                                {
                                                                    Key = DateTime.UtcNow.Ticks.ToString(),
                                                                    Value = JObject.FromObject(request).ToString()
                                                                });
          

            return (ticketType, "UpdateSuccess");
        }


        #region Function test
        public async Task<object> TestAddMutiTicketTypesAsync()
        {
            for (int i = 0; i < 10; i++)
            {
                await AddTicketAsync(new AddTicketTypeRequest
                {
                    DeviceIds = new List<string>(),
                    TicketTypeDescription = $"[Alta-test] Mutiple:{i}",
                    TicketTypeName = $"[Alta-test] Mutiple:{i}"
                });
            }
            return null;
        }


        public async Task<object> TestDeleteMutiTicketTypesAsync()
        {
            List<TicketType> ticketTypes = _repositoryWrapperMariaDB.TicketTypes.FindByCondition(x => x.TicketTypeName.StartsWith("[Alta-test]")).ToList();
            foreach (var item in ticketTypes)
            {
                await DeleteTicketAsync(item.TicketTypeId);
            }
            return new { 
                    count = ticketTypes.Count
            };
        }

        #endregion

    }
}
