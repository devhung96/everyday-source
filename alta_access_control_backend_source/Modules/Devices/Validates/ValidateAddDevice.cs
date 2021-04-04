using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Devices.Requests;
using Project.Modules.DeviceTypes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Validates
{
    public class AddDeviceValidator : AbstractValidator<AddDeviceRequest>
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public AddDeviceValidator(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;
            RuleFor(x => x.DeviceName).NotNull().WithMessage("DeviceNameRequreid");
            RuleFor(x => x.DevicePassword).NotNull().WithMessage("DevicePasswordRequreid");
            RuleFor(x => x.DeviceTypeId).NotNull().WithMessage("DeviceTypeIdRequreid").Must(CheckTypeDevice).WithMessage("DeviceTypeIdNotExist");
        }
        public bool CheckTypeDevice(string id)
        {
            DeviceType deviceType = repository.DeviceTypes.FindByCondition(x=>x.DeviceTypeId.Equals(id)).FirstOrDefault();
            if(deviceType is null)
            {
                return false;
            }
            return true;
        }
    }
}
