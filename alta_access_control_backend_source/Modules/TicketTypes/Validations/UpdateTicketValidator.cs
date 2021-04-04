using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Devices.Entities;
using Project.Modules.Tickets.Entities;
using Project.Modules.TicketTypes.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TicketTypes.Validations
{
    public class UpdateTicketValidator : AbstractValidator<UpdateTicketTypeRequest>
    {
        private readonly IRepositoryWrapperMariaDB repositoryWrapper;
        public UpdateTicketValidator(IRepositoryWrapperMariaDB repositoryWrapper)
        {
            this.repositoryWrapper = repositoryWrapper;
            RuleFor(x => x.DeviceIds).Must(IsDevice).When(x => x.DeviceIds.Count > 0).WithMessage("DeviceIdsInvalid");
        }
        private bool IsDevice(List<string> deviceIds)
        {
            deviceIds = deviceIds.Distinct().ToList();
            List<Device> devices = repositoryWrapper.Devices.FindByCondition(x => deviceIds.Contains(x.DeviceId)).ToList();
            if (devices.Count < deviceIds.Count)
            {
                return false;
            }
            return true;
        } 
        private bool IsTicketType(string TicketTypeID)
        {
           TicketType ticketType = repositoryWrapper.TicketTypes.GetById(TicketTypeID);
            if (ticketType is null)
            {
                return false;
            }
            return true;
        }
    }
}
