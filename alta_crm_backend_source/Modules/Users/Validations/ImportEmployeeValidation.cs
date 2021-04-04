using FluentValidation;
using Project.App.Vatidations;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Validations
{
    public class ImportEmployeeValidation : AbstractValidator<ImportEmployeeRequest>
    {
        public ImportEmployeeValidation()
        {
            RuleFor(x => x.FileZip)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("{PropertyName}IsNotNull")
                .NotEmpty().WithMessage("{PropertyName}IsNotEmpty")
                .CheckExtensionsFile(_extensions: new[] { ".zip" })
                .CheckMaxFileSizeFile(50*1024*1024)
                .WithName("FileZip");

        }
    }
}
