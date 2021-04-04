using FluentValidation;
using Project.App.Vatidations;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Requests;
using Repository;
using System;

namespace Project.Modules.Schedules.Validations
{
    public class ImportScheduleCustomerValidation : AbstractValidator<ImportScheduleCustomerRequest>
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;

        public ImportScheduleCustomerValidation(IRepositoryWrapperMariaDB repositoryWrapperMariaDB)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;

            RuleFor(x => x.ScheduleName)
            .NotNull().WithMessage("{PropertyName}IsNotNull")
            .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
            .WithName("ScheduleName");

            RuleFor(x => x.ScheduleDateStart)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName}IsNotNull")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
                .Must((item, scheduleDateStart) => FluentValidationExtensions.GreateThanStrDateTime(scheduleDateStart, item.ScheduleDateEnd, format: "yyyy-MM-dd HH:mm:ss", repeatType: item.ScheduleRepeatType)).WithMessage("ScheduleDateEndGreateThanScheduleDateStart")
                .WithName("ScheduleDateStart");


            RuleFor(x => x.ScheduleDateEnd)
                .Cascade(CascadeMode.Stop)
                .CheckDataTime(format: "yyyy-MM-dd HH:mm:ss")
                .WithName("ScheduleDateEnd");

            RuleFor(x => x.ScheduleValues)
              .Cascade(CascadeMode.Stop)
              .NotNull().WithMessage("{PropertyName}IsNotNull")
              .Must((item, scheduleValues) => FluentValidationExtensions.CheckScheduleByScheduleType(scheduleValues, item.ScheduleRepeatType)).WithMessage("{PropertyName}Invalid")
              .When(x => x.ScheduleRepeatType != ScheduleRepeatType.NoRepeat && x.ScheduleRepeatType != ScheduleRepeatType.Daily)
              .WithName("ScheduleValues");

        }
        
    }
}
