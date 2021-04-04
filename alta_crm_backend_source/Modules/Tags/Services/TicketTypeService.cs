using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.App.Helpers;
using Project.Modules.Tags.Enities;
using Project.Modules.Tags.Requests;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Services
{

    public interface ITicketTypeService
    {
        public (object data, string message) InsertTicket(AddTicketRequest request);
        public (object data, object errros, string message) InsertManyTicket(AddMutiTicketTypeRequest request);


        public (object data, string message) UpdateTicketType(UpdateTicketRequet requet);

        public (bool result, string message) DeleteTicketType(string ticketTypeId);
        public (bool result, string message) DeleteManyTicketType(List<string> ticketTypeIds);

        public (TicketType data, string message) GetTicketById(string ticketId);
        public object ShowAll(PaginationRequest request);
    }
    public class TicketTypeService : ITicketTypeService
    {
        private readonly IRepositoryWrapperMariaDB _repositoryMariaWrapper;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public TicketTypeService(IRepositoryWrapperMariaDB repositoryMariaWrapper, IConfiguration configuration, IMapper mapper)
        {
            _repositoryMariaWrapper = repositoryMariaWrapper;
            _configuration = configuration;
            _mapper = mapper;
        }


        #region Insert
        public (object data, string message) InsertTicket(AddTicketRequest request)
        {
            TicketType newTicketType = _mapper.Map<AddTicketRequest, TicketType>(request);
            newTicketType.Devices = JsonConvert.SerializeObject(request.DeviceIds);

            var ticketType = _repositoryMariaWrapper.Tickets.FindByCondition(x => x.TicketTypeId == newTicketType.TicketTypeId)/*.Include(x => x.Tags)*/.FirstOrDefault();
            if (ticketType != null) return (null, "TicketTypeAlreadyExists");

            _repositoryMariaWrapper.Tickets.Add(newTicketType);
            _repositoryMariaWrapper.SaveChanges();
            return (newTicketType, "CreateTicketTypeSuccess");
        }


        public (object data, object errros, string message) InsertManyTicket(AddMutiTicketTypeRequest request)
        {
            List<TicketType> oldTicketTypes = _repositoryMariaWrapper.Tickets.FindAll().ToList();

            List<TicketType> newTicketTypes = new List<TicketType>();
            foreach (var item in request.TicketTypes)
            {
                TicketType newTicketType = _mapper.Map<BaseTicketTypeRequest, TicketType>(item);
                TicketType isIdTicketType = oldTicketTypes.FirstOrDefault(x => x.TicketTypeId.Equals(item.TicketTypeId));
                if (isIdTicketType != null)
                {

                    item.MessageErrors.Add("TicketTypeIdExists");
                    continue;
                }
                newTicketType.Devices = JsonConvert.SerializeObject(item.DeviceIds);
                newTicketTypes.Add(newTicketType);
            }
            if (request.TicketTypes.Any(x => x.MessageErrors.Count > 0)) return (null, request, "ErrorRequestInvalid");

            _repositoryMariaWrapper.Tickets.AddRange(newTicketTypes);
            _repositoryMariaWrapper.SaveChanges();
            return (newTicketTypes, null, "CreateTicketTypesSuccess");
        }

        #endregion End insert


        public (object data, string message) UpdateTicketType(UpdateTicketRequet requet)
        {
            var ticketType = _repositoryMariaWrapper.Tickets.FindByCondition(x => x.TicketTypeId == requet.TicketTypeId).FirstOrDefault();
            if (ticketType is null) return (null, "TicketTypeNotFound");


            ticketType = _mapper.Map<UpdateTicketRequet,TicketType>(requet, ticketType);
            _repositoryMariaWrapper.SaveChanges();

            return (ticketType, "UpdateSuccess");
        }


        #region Delete ticketType 
        public (bool result, string message) DeleteTicketType(string ticketTypeId)
        {
            var ticketType = _repositoryMariaWrapper.Tickets.FindByCondition(x => x.TicketTypeId == ticketTypeId).FirstOrDefault();
            if (ticketType is null) return (false, "TicketTypeNotFound");

            _repositoryMariaWrapper.Tickets.Remove(ticketType);
            _repositoryMariaWrapper.SaveChanges();
            return (true, "DeleteTicketTypeSuccess");
        }

        public (bool result, string message) DeleteManyTicketType(List<string> ticketTypeIds)
        {
            List<TicketType> ticketTypes = _repositoryMariaWrapper.Tickets.FindByCondition(x => ticketTypeIds.Contains(x.TicketTypeId)).ToList();
            _repositoryMariaWrapper.Tickets.RemoveRange(ticketTypes);
            _repositoryMariaWrapper.SaveChanges();

            return (true, "DeleteTicketTypeSuccess");
        }
        #endregion



        public (TicketType data, string message) GetTicketById(string ticketId)
        {
            var ticketType = _repositoryMariaWrapper.Tickets.FindByCondition(x => x.TicketTypeId == ticketId).FirstOrDefault();
            if (ticketType is null) return (null, "TicketTypeNotFound");

            ticketType.Tags = _repositoryMariaWrapper.Tags.FindByCondition(y => y.TicketTypeId.Equals(ticketType.TicketTypeId)).ToList();
            return (ticketType, "GetTicketTypeSuccess");
        }

        public object ShowAll(PaginationRequest request)
        {
            var queryTicketTypes = _repositoryMariaWrapper.Tickets.FindByCondition(x =>
                                                                                        String.IsNullOrEmpty(request.SearchContent) ||
                                                                                        (!String.IsNullOrEmpty(x.TicketTypeDescription) && x.TicketTypeDescription.ToLower().Contains(request.SearchContent)) ||
                                                                                        (!String.IsNullOrEmpty(x.TicketTypeName) && x.TicketTypeName.ToLower().Contains(request.SearchContent)) ||
                                                                                        (!String.IsNullOrEmpty(x.Devices) && x.Devices.Contains(request.SearchContent))
                                                                                   );

            List<Tag> tags = _repositoryMariaWrapper.Tags.FindByCondition(y => queryTicketTypes.Select(x => x.TicketTypeId).Contains(y.TicketTypeId)).ToList();

            foreach (var item in queryTicketTypes)
            {
                item.Tags = tags.Where(x => x.TicketTypeId.Equals(item.TicketTypeId)).ToList();
            }

            var ticketTypes = SortHelper<TicketType>.ApplySort(queryTicketTypes, request.OrderByQuery);
            var paginationTicketTypes = PaginationHelper<TicketType>.ToPagedList(ticketTypes, request.PageNumber, request.PageSize);

            PaginationResponse<TicketType> paginationResponse = new PaginationResponse<TicketType>(paginationTicketTypes, paginationTicketTypes.PageInfo);
            return paginationResponse;
        }





    }
}
