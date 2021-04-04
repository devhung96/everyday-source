using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.DependencyInjection;
using Project.Modules.Ratings.Enities;
using Project.Modules.Ratings.Requests;
using Project.Modules.Users.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Ratings.FluentValidation
{
    public class InsertRatingFluent : AbstractValidator<InsertRatingRequest>
    {
        public InsertRatingFluent()
        {

            RuleFor(x => x.UserId)
                .SetValidator(new UserIdExistsValidator());
                   //.Cascade(CascadeMode.Stop)
                   //.NotNull()
                   //.WithMessage("{PropertyName}IsNotNull")
                   //.WithName("UserId");

            RuleFor(x => x.RobotId).SetValidator(new RobotExistsValidator());
            RuleFor(x => x.Star).Must(x => x <= 5 && x >= 0)
                .WithMessage("StarInvalid")
                //.Must(x => x.Length > 10 && x.Length < 15)
                //    .WithMessage("Name should be between 10 and 15 chars");
                //.NotEmpty().WithMessage("TagNameIsRequired")
                ;
        }
    }
    public class UserIdExistsValidator : PropertyValidator
    {
        public UserIdExistsValidator()
        {

        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyName is null || context.PropertyValue is null)
            {
                return true;
            }

            IRepositoryWrapperMariaDB MariaDb = context.GetServiceProvider().GetRequiredService<IRepositoryWrapperMariaDB>();
            User user = MariaDb.Users.FindByCondition(x => x.UserId == context.PropertyValue.ToString()).FirstOrDefault();
            if (user is null)
            {
                context.Rule.MessageBuilder = c => "UserIdNotExists";
                return false;
            }
            return true;
        }
    }
    public class RobotExistsValidator : PropertyValidator
    {
        public RobotExistsValidator()
        {

        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyName is null || context.PropertyValue is null)
            {
                return true;
            }

            IRepositoryWrapperMariaDB MariaDb = context.GetServiceProvider().GetRequiredService<IRepositoryWrapperMariaDB>();
            Robot robot = MariaDb.Robots.FindByCondition(x => x.RobotId == context.PropertyValue.ToString()).FirstOrDefault();
            if (robot is null)
            {
                context.Rule.MessageBuilder = c => "RobotNotExists";
                return false;
            }
            return true;
        }
    }
}
