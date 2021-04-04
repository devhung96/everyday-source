using FluentValidation;
using FluentValidation.Validators;
using Project.Modules.Groups.Requests;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Groups.FluentValidation
{
    public class UpdateGroupFluent : AbstractValidator<UpdateGroupRequest>
    {
        public UpdateGroupFluent()
        {
            //RuleFor(x => x.GroupName)();
            //RuleFor(x => x.GroupDetailsRequests).Empty();
        }
    }

}
