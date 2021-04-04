using FluentValidation;
using Microsoft.AspNetCore.Http;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Medias.Validations
{
    public class AddMediaValidator : AbstractValidator<AddMediaRequest>
    {
        private readonly IRepositoryWrapperMariaDB repository;
        private static readonly List<string> mediaType = new()
        {
            "video/mp4",
            "audio/mp3",
            "image/jpg",
            "image/jpeg",
        };
        public AddMediaValidator(IRepositoryWrapperMariaDB Repository)
        {
            repository = Repository;
            RuleFor(x => x.MediaUrl)
                .NotNull().WithMessage("MediaUrlNotNull")
                .NotEmpty().WithMessage("MediaUrlNotEmpty")
                .Must(CheckLenghtMediaUrl).WithMessage("MediaLenghtIsMaxed")
                .Must(CheckContentTypeMediaUrl).WithMessage("MediaContentTypeInvalid");

            RuleFor(x => x.MediaType)
                .Must(CheckValidateMediaType).WithMessage("MediaTypeInvalid");

            RuleFor(x => x.MediaName)
                .Must(CheckValidateMediaName).WithMessage("MediaNameIsExist")
                .NotNull().WithMessage("MediaNameNotNull")
                .NotEmpty().WithMessage("MediaNameNotEmpty");
        }

        private bool CheckLenghtMediaUrl(IFormFile formFile)
        {
            if (formFile != null && formFile.Length > 1 * 1024 * 1024 * 1024)
            {
                return false;
            }
            return true;
        }
        private bool CheckContentTypeMediaUrl(IFormFile formFile)
        {
            if (formFile != null && !formFile.ContentType.StartsWith("image") && !formFile.ContentType.StartsWith("video") && !formFile.ContentType.StartsWith("audio"))
            {
                return false;
            }
            return true;
        }


        private bool CheckValidateMediaType(string mediaTypeId)
        {
            if (string.IsNullOrEmpty(mediaTypeId)) return true;
            MediaType mediaType = repository.MediaTypes.FindByCondition(x => x.TypeId.Equals(mediaTypeId)).FirstOrDefault();
            return mediaType != null;
        }

        private bool CheckValidateMediaName(string mediaName)
        {
            Media media = repository.Medias.FindByCondition(x => x.MediaName.Equals(mediaName)).FirstOrDefault();
            return media is null;
        }
    }
}
