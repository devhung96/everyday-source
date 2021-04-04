﻿using FluentValidation;
using Project.Modules.Users.Requests;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class UserStoredValidation : AbstractValidator<UserStoredRequest>
    {
        private readonly IRepositoryWrapperMariaDB repositoryWrapperMariaDB;
        public UserStoredValidation(IRepositoryWrapperMariaDB _repositoryWrapperMariaDB)
        {
            repositoryWrapperMariaDB = _repositoryWrapperMariaDB;
            RuleFor(x => x.UserFirstName)
                .NotEmpty()
                .WithMessage("UserFirstNameNotEmpty");
            RuleFor(x => x.UserLastName)
                .NotEmpty()
                .WithMessage("UserLastNameNotEmpty");
            RuleFor(x => x.UserEmail)
                .NotEmpty()
                .WithMessage("UserEmailNotEmpty")
                .EmailAddress()
                .WithMessage("UserEmailIsNotEmail")
                .Must(CheckEmailDuplicate)
                .WithMessage("UserEmailIsDuplicate");
            RuleFor(x => x.GroupId)
                 .NotEmpty()
                 .WithMessage("GroupIdNotEmpty")
                 .Must(CheckGroupId)
                 .WithMessage("GroupIdDoNotExists");
            //RuleFor(x => x.TagId)
            //    .Must(CheckTagId)
            //    .WithMessage("TagIdDoNotExists");

        }

        public bool CheckGroupId(string groupId)
        {
            if(string.IsNullOrEmpty(groupId))
            {
                return true;
            }
            bool result = repositoryWrapperMariaDB.Groups.FindByCondition(x => x.GroupId.Equals(groupId)).Any();
            if(result)
            {
                return true;
            }
            return false;
        }
        public bool CheckTagId(string tagId)
        {
            if (string.IsNullOrEmpty(tagId))
            {
                return true;
            }
            bool result = repositoryWrapperMariaDB.Tags.FindByCondition(x => x.TagId.Equals(tagId)).Any();
            if (result)
            {
                return true;
            }
            return false;
        }
        public bool CheckEmailDuplicate(string userEmail)
        {
            bool result = repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserEmail.Equals(userEmail)).Any();
            if(result)
            {
                return false;
            }
            return true;
        }

    }
}
