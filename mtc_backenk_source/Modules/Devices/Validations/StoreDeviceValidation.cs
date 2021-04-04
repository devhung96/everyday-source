using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Devices.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Project.Modules.Devices.Validations
{
    public class StoreDeviceValidation : AbstractValidator<StoreDevice>
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public StoreDeviceValidation(IRepositoryWrapperMariaDB repository)
        {
            Regex regEx = new Regex("^[a-zA-Z0-9.]*$");
            this.repository = repository;
            RuleFor(x => x.DeviceName)
                .NotNull()
                .WithMessage("{PropertyName}NotNull")
                .NotEmpty()
                .WithMessage("{PropertyName}NotEmpty");
            RuleFor(x => x.Image)
                .NotNull()
                .WithMessage("{PropertyName}NotNull")
                .NotEmpty()
                .WithMessage("{PropertyName}NotEmpty");
            RuleFor(x => x.LoginName)
               .NotNull()
               .WithMessage("LoginNameNotNull")
               .NotEmpty()
               .WithMessage("LoginNameNotEmpty")
               .Matches(regEx);
            RuleFor(x => x.DevicePass)
               .NotNull()
               .WithMessage("DevicePassNotNull")
               .NotEmpty()
               .WithMessage("DevicePassNotEmpty")
               .MaximumLength(40)
               .WithMessage("PasswordCannotBeLongerThan40Characters")
               .MinimumLength(8)
               .WithMessage("PasswordCannotBeLessThan8Characters");
            RuleFor(x => x.DeviceTypeId)
               .NotNull()
               .WithMessage("DeviceTypeIdNotNull")
               .NotEmpty()
               .WithMessage("DeviceTypeIdNotEmpty")
               .Must(CheckDeviceType)
               .WithMessage("DeviceTypeIdNotFound");
            RuleFor(x => x.DeviceExpired)
               .NotNull()
               .WithMessage("DeviceExpiredNotNull")
               .NotEmpty()
               .WithMessage("DeviceExpiredNotEmpty");
            RuleFor(x => x.DeviceWarrantyExpiresDate)
               .NotNull()
               .WithMessage("DeviceWarrantyExpiresDateNotNull")
               .NotEmpty()
               .WithMessage("DeviceWarrantyExpiresDateNotEmpty");
            RuleFor(x => x.DeviceSku)
               .NotNull()
               .WithMessage("DeviceSkuNotNull")
               .NotEmpty()
               .WithMessage("DeviceSkuNotEmpty");
            RuleFor(x => x.GroupId)
              .Must(CheckGroup)
              .WithMessage("GroupIdDoNotExists");

        }
        public bool CheckDeviceType(string deviceTypeId)
        {
            return repository.DeviceTypes.FindByCondition(x => x.DeviceTypeId.Equals(deviceTypeId)).Any();
        }
        public bool CheckGroup(string groupId)
        {
            if(string.IsNullOrEmpty(groupId))
            {
                return true;
            }
            return repository.Groups.FindByCondition(x => x.GroupId.Equals(groupId)).Any();
        }
    }
}
