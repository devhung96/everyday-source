using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Devices.Entities;
using Project.Modules.TicketDevices.Entities;
using Project.Modules.Tags.Entities;
using Project.Modules.Tickets.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Modules.TicketDevices.Requests;
using Microsoft.EntityFrameworkCore;

namespace Project.Modules.TicketDevices.Services
{
    public interface ITicketDeviceService
    {
        (List<TicketTypeDevice> tagDevice, string message) AddTicketDevice(AddTicketDerviceRequest request);
        (TicketTypeDevice tagDevice, string message) RemoveTicketDevice(DeleteTicketDerviceRequest request);
        PaginationResponse<Device> ShowDeviceByTicket(PaginationRequest request, string ticketId);
        List<Device> ShowDeviceNotInTicket(string ticketId);
        PaginationResponse<TicketType> ShowTicketByDevice(PaginationRequest request, string deviceId);
    }
    public class TicketDeviceService : ITicketDeviceService
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public TicketDeviceService(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;
        }

        public (List<TicketTypeDevice> tagDevice, string message) AddTicketDevice(AddTicketDerviceRequest request)
        {
            List<TicketTypeDevice> ticketTypeDevices = new List<TicketTypeDevice>();
            foreach (string deviceId in request.DeviceIds)
            {
                ticketTypeDevices.Add(new TicketTypeDevice
                {
                    TicketTypeId = request.TicketTypeId,
                    DeviceId = deviceId
                });
            }

            repository.TicketTypeDevices.AddRange(ticketTypeDevices);
            repository.SaveChanges();
            return (ticketTypeDevices, "AddService");
        }

        public (TicketTypeDevice tagDevice, string message) RemoveTicketDevice(DeleteTicketDerviceRequest request)
        {
            TicketTypeDevice ticketDevice = repository.TicketTypeDevices
                                                      .FindByCondition(x => x.DeviceId.Equals(request.DeviceId) &&
                                                                               x.TicketTypeId.Equals(request.TicketTypeId))
                                                      .FirstOrDefault();
            if (ticketDevice is null)
            {
                return (null, "TicketType-DeviceNotExist");
            }
            repository.TicketTypeDevices.Remove(ticketDevice);
            repository.SaveChanges();
            return (ticketDevice, "DeleteSuccess");
        }

        public PaginationResponse<Device> ShowDeviceByTicket(PaginationRequest request, string ticketId)
        {
            List<string> deviceIds = repository.TicketTypeDevices
                                               .FindByCondition(x => x.TicketTypeId.Equals(ticketId))
                                               .Select(x => x.DeviceId)
                                               .ToList();

            var data = repository.Devices.FindByCondition(x => deviceIds.Contains(x.DeviceId)
                                                            && (string.IsNullOrEmpty(request.SearchContent)||
                                                                x.DeviceName.ToLower().Contains(request.SearchContent.ToLower()))
                                                           ).Include(x => x.DeviceType);
            var result = PaginationHelper<Device>.ToPagedList(data, request.PageNumber, request.PageSize);
            data = SortHelper<Device>.ApplySort(data, request.OrderByQuery).Include(x => x.DeviceType);
            PaginationResponse<Device> response = new PaginationResponse<Device>(data, result.PageInfo);
            return response;
        }

        public List<Device> ShowDeviceNotInTicket(string ticketId)
        {
            List<string> deviceIds = repository.TicketTypeDevices
                                               .FindByCondition(x => x.TicketTypeId.Equals(ticketId))
                                               .Select(x => x.DeviceId)
                                               .ToList();
            List<Device> data = repository.Devices.FindByCondition(x => !deviceIds.Contains(x.DeviceId)).ToList();

            return data;
        }

        public PaginationResponse<TicketType> ShowTicketByDevice(PaginationRequest request, string deviceId)
        {
            List<string> ticketIds = repository.TicketTypeDevices
                                               .FindByCondition(x => x.DeviceId.Equals(deviceId))
                                               .Select(x => x.TicketTypeId)
                                               .ToList();

            var data = repository.TicketTypes.FindByCondition(x => ticketIds.Equals(x.TicketTypeId)
                                                            && (string.IsNullOrEmpty(request.SearchContent)
                                                                || x.TicketTypeName.ToLower().Contains(request.SearchContent.ToLower()))
                                                           );
            var result = PaginationHelper<TicketType>.ToPagedList(data, request.PageNumber, request.PageSize);
            data = SortHelper<TicketType>.ApplySort(data, request.OrderByQuery);
            PaginationResponse<TicketType> response = new PaginationResponse<TicketType>(data, result.PageInfo);
            return response;
        }
    }
}
