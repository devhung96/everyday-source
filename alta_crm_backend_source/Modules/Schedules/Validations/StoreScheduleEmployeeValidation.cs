using FluentValidation;
using Project.App.Vatidations;
using Project.Modules.Schedules.Entities;
using Project.Modules.Schedules.Requests;
using Project.Modules.Users.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Schedules.Validations
{
    public class StoreScheduleEmployeeValidation : AbstractValidator<StoreScheduleEmployeeRequest>
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        public StoreScheduleEmployeeValidation(IRepositoryWrapperMariaDB repositoryWrapperMariaDB)
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


            RuleFor(x => x.ModeId)
                .NotNull().WithMessage("ModeIdIsNotNull")
                .Must(CheckMode).WithMessage("ModeInvalid");

            RuleFor(x => x.UserId)
                .NotNull().WithMessage("UserIdIsNotNull")
                .Must(CheckUser).WithMessage("UserInvalid");

            RuleFor(x => x)
                .Must(x => CheckTag(x.TagId, x.UserId, x.ModeId))
                .WithMessage("TagDoesNotExist");

        }

        private bool CheckMode(string modeId)
        {
            return RepositoryWrapperMariaDB.ModeAuthentications.FindByCondition(x => x.ModeAuthenticationId.Equals(modeId)).Any();
        }

        private bool CheckTag(string tagId, string userId, string modeId)
        {
            return RepositoryWrapperMariaDB.UserTagModes.FindByCondition(x => x.TagId.Equals(tagId) && x.UserId.Equals(userId) && x.ModeId.Equals(modeId)).Any();
        }

        private bool CheckUser(string userId)
        {
            User user = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId));

            if (user is null)
            {
                return false;
            }

            if (user.GroupId.Equals("customer")) // check user là employee
            {
                return false;
            }

            return true;
        }

    }
}
