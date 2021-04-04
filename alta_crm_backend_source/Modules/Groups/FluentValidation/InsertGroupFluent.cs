using FluentValidation;
using FluentValidation.Validators;
using Microsoft.Extensions.DependencyInjection;
using Project.Modules.Groups.Enities;
using Project.Modules.Groups.Requests;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.FluentValidation
{
    public class InsertGroupFluent : AbstractValidator<InsertGroupRequest>
    {
        public InsertGroupFluent()
        {
            RuleFor(x => x.GroupName).NotNull().WithMessage("GroupNameRequired");
            //RuleFor(x => x.GroupDetailsRequests).Empty();
            RuleFor(x => x.GroupCode).NotNull().WithMessage("GroupCodeRequired").SetValidator(new InsertGroupCodeValidator());
        }
    }

    public class InsertGroupDetailFluent : AbstractValidator<InsertGroupDetailsRequest>
    {
        public InsertGroupDetailFluent()
        {
            RuleFor(x => x.Time).Empty();
            //RuleFor(x => x.GroupId).SetValidator(new GroupExistsValidator());
            RuleFor(x => x.ModeAuthenticationId).Empty();
        }
    }


    public class InsertGroupCodeValidator : PropertyValidator
    {
        public InsertGroupCodeValidator()
        {

        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyName is null || context.PropertyValue is null)
            {
                context.Rule.MessageBuilder = c => "GroupCodeRequired";
                return false;
            }

            IRepositoryWrapperMariaDB MariaDb = context.GetServiceProvider().GetRequiredService<IRepositoryWrapperMariaDB>();
            Group group = MariaDb.Groups.FindByCondition(x => x.GroupCode == context.PropertyValue.ToString()).FirstOrDefault();
            if (group is not null)
            {
                context.Rule.MessageBuilder = c => "GroupCodeIsExists";
                return false;
            }
            return true;
        }

    }

    public class ModeAuthenticationExistsValidator : PropertyValidator
    {
        public ModeAuthenticationExistsValidator()
        {

        }
        protected override bool IsValid(PropertyValidatorContext context)
        {
            if (context.PropertyName is null || context.PropertyValue is null)
            {
                context.Rule.MessageBuilder = c => "ModeAuthenticationIdRequired";
                return false;
            }

            IRepositoryWrapperMariaDB MariaDb = context.GetServiceProvider().GetRequiredService<IRepositoryWrapperMariaDB>();
            ModeAuthentication modeAuthentication = MariaDb.ModeAuthentications.FindByCondition(x => x.ModeAuthenticationId == context.PropertyValue.ToString()).FirstOrDefault();
            if (modeAuthentication is null)
            {
                context.Rule.MessageBuilder = c => "ModeAuthenticationNotExists";
                return false;
            }
            return true;
        }

    }
}
