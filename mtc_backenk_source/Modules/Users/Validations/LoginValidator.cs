using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        private readonly IRepositoryWrapperMariaDB repository;
        public LoginValidator(IRepositoryWrapperMariaDB repository)
        {
            this.repository = repository;
            RuleFor(x => x.UserEmail).NotNull().WithMessage("{PropertyName}NotNull")
                                    .NotEmpty().WithMessage("{PropertyName}NotEmpty")
                                    .EmailAddress().WithMessage("EmailInvalid");
           
        }
  
      
    }
}
