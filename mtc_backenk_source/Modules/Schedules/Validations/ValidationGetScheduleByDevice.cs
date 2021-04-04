using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Validations;
using Project.Modules.Schedules.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Validations
{
    public class ValidationGetScheduleByDevice : AbstractValidator<GetScheduleByDevice>
    {
        private IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        public ValidationGetScheduleByDevice(IRepositoryWrapperMariaDB repositoryWrapperMariaDB)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;

            RuleFor(x => x.DeviceId)
               .NotNull().WithMessage("{PropertyName}IsRequired")
               .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
               .Must(IsExistDevice)
               .WithMessage("DeviceNotFound")
               .WithName("DeviceId");

            RuleFor(x => x.ScheduleDateBegin)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .CheckDataTime(format: "yyyy-MM-dd")
                .WithName("ScheduleDateBegin");

            RuleFor(x => x.ScheduleDateEnd)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .CheckDataTime(format: "yyyy-MM-dd")
                .WithName("ScheduleDateEnd");
        }
        private bool IsExistDevice(string deviceId)
        {
            return RepositoryWrapperMariaDB.Devices.FirstOrDefault(x => x.DeviceId.Equals(deviceId)) != null;
        }

    }
}
