using FluentValidation;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Detections.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Detections.Validations
{
    public class DetectValidate : AbstractValidator<DetectTagDeviceRequest>
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        public DetectValidate(IRepositoryWrapperMariaDB repositoryWrapperMariaDB)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;

            RuleFor(x => x.DeviceId)
                .NotNull()
                .WithMessage("DeviceIdIsNotNull")
                .NotEmpty()
                .WithMessage("DeviceIdIsNotEmpty");

            RuleFor(x => x.ModeId)
                .NotNull()
                .WithMessage("ModeIdIsNotNull")
                .NotEmpty()
                .WithMessage("ModeIdIsNotEmpty")
                .Must(CheckMode)
                .WithMessage("ModeIdIsNotExist");

            RuleFor(x => x.Image)
                .NotNull()
                .When(x => x.ModeId == "Face_ID")
                .WithMessage("ImageIsNotNull")
                .NotEmpty()
                .When(x => x.ModeId == "Face_ID")
                .WithMessage("ImageIsNotEmpty");

            RuleFor(x => x.KeyCode)
                .NotNull()
                .When(x => x.ModeId != "Face_ID")
                .WithMessage("KeyCodeIsNotNull")
                .NotEmpty()
                .When(x => x.ModeId != "Face_ID")
                .WithMessage("KeyCodeIsNotEmpty");


        }
        public bool CheckMode(string modeId)
        {
            return RepositoryWrapperMariaDB.Modes.FindByCondition(x => x.ModeId.Equals(modeId)).Any();
        }
    }
}
