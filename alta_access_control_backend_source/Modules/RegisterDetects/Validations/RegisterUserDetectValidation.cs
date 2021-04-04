using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Validations;
using Project.Modules.RegisterDetects.Requests;
using Project.Modules.Tags.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.RegisterDetects.Validations
{
    public class RegisterUserDetectValidation : AbstractValidator<RegisterUserDetectRequest>
    {
        private readonly IRepositoryWrapperMariaDB _repositoryMariaWrapper;
        public RegisterUserDetectValidation(IRepositoryWrapperMariaDB repositoryMariaWrapper)
        {
            _repositoryMariaWrapper = repositoryMariaWrapper;

            RuleFor(x => x.RgDectectUserId)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("{PropertyName}IsNotNull")
                   .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                   .WithName("RgDectectUserId");

            RuleFor(x => x.RgDectectKey)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("{PropertyName}IsNotNull")
                   .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                   .WithName("RgDectectKey");

            RuleFor(x => x.ModeId)
                   .Cascade(CascadeMode.Stop)
                   .NotNull().WithMessage("{PropertyName}IsNotNull")
                   .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                   .WithName("ModeId");


            //RuleFor(x => x.TagId)
            //      .Cascade(CascadeMode.Stop)
            //      .NotNull().WithMessage("{PropertyName}IsNotNull")
            //      .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
            //      .WithName("TagId");

            RuleFor(x => x.TagCode)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("{PropertyName}IsNotNull")
                 .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                 .WithName("TagCode");

            RuleFor(x => x.TicketTypeId)
                 .Cascade(CascadeMode.Stop)
                 .NotNull().WithMessage("{PropertyName}IsNotNull")
                 .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                 .Must((item, ticketTypeId) => IsTickedTypeExistsInTag(ticketTypeId, item.TagCode)).WithMessage("{PropertyName}Invalid")
                 .WithName("TicketTypeId");


            RuleFor(x => x.RegisterDettectDetailRequests)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("{PropertyName}IsNotNull")
            .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
            .When(x => x.RegisterDettectDetailRequests.Count <= 0).WithMessage("{PropertyName}TotalItemGreaterThan0")
            .WithName("RegisterDettectDetailRequests");


            RuleForEach(x => x.RegisterDettectDetailRequests)
            .SetValidator(new RegisterDettectDetailValidation())
            .WithName("RegisterDettectDetailRequests");



        }
        public bool IsTickedTypeExistsInTag(string tickedTypeId, string tagCode)
        {
            Tag tag = _repositoryMariaWrapper.Tags.FindByCondition(x => x.TagCode.Equals(tagCode)).FirstOrDefault();
            if (tag is null) return false;
            if (tagCode.Equals("customer"))
            {
                return  _repositoryMariaWrapper.TicketTypes.FindByCondition(x => x.TicketTypeId.Equals(tickedTypeId)).Any();

            }

            if (tag.TicketTypeId != tickedTypeId) return false;
            return true;
        }
    }


}
