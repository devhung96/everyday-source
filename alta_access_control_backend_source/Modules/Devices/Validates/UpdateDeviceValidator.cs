using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Devices.Entities;
using Project.Modules.Devices.Requests;
using Project.Modules.DeviceTypes.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Validates
{
    public class UpdateDeviceValidator: AbstractValidator<UpdateDeviceRequest>
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public UpdateDeviceValidator(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;
            RuleFor(x => x.DeviceTypeId).Must(CheckTypeDevice).WithMessage("DeviceTypeIdNotExist");
        }
        public bool CheckTypeDevice(string id)
        {
            if (string.IsNullOrEmpty(id))
                return true;
            DeviceType deviceType = repository.DeviceTypes.GetById(id);
            if (deviceType is null)
            {
                return false;
            }
            return true;
        }
    }
}
