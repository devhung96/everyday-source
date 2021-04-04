using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Devices.Entities;
using Project.Modules.TicketDevices.Entities;
using Project.Modules.TicketDevices.Requests;
using Project.Modules.Tags.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project.Modules.Tickets.Entities;

namespace Project.Modules.TicketDevices.Validations
{
    public class AddTagDeviceValidator : AbstractValidator<AddTicketDerviceRequest>
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public AddTagDeviceValidator(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;
            RuleFor(x => x.TicketTypeId).NotNull().WithMessage("TicketTypeIdRequreid").Must(CheckTag).WithMessage("TicketTypeIdNotExist");
            RuleFor(x => x.DeviceIds).NotNull().WithMessage("DeviceIdRequreid").Must(CheckDevice).WithMessage("DeviceIdNotExist");
            RuleFor(x => x).Must(x => CheckExist(x.TicketTypeId, x.DeviceIds)).WithMessage("TagDeviceAlreadyExist");
          
        }
        public bool CheckTag(string tagId)
        {
            TicketType ticket = repository.TicketTypes.GetById(tagId);
            if (ticket is null)
            {
                return false;
            }
            return true;
        }
        public bool CheckDevice(List<string> deviceIds)
        {
            foreach(string deviceId in deviceIds)
            {
                Device tag = repository.Devices.GetById(deviceId);
                if (tag is null)
                {
                    return false;
                }
            }
            
            return true;
        }
        public bool CheckExist(string tagId, List<string> deviceIds)
        {
            List<TicketTypeDevice> tagDevice = repository.TicketTypeDevices.FindByCondition(x => deviceIds.Contains(x.DeviceId) && x.TicketTypeId.Equals(tagId)).ToList();
            if(tagDevice.Count ==0)
            {
                return true;
            }
            return false;
        }
    }
}
