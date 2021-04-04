using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Medias.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Project.Modules.Medias.Services
{
    public interface IMediaTypeService
    {
        (MediaType data, string message) Store(MediaType media, string url);
        IQueryable<MediaType> ShowAll();
        MediaType Show(string idMediaType);
        MediaType Update(MediaType mediaType);
        (bool reulst, string message) Destroy(string idMediaType);

    }

    public class MediaTypeService : IMediaTypeService
    {
        private readonly IRepositoryWrapperMariaDB Repository;
        private readonly IMapper Mapper;
        public MediaTypeService(IRepositoryWrapperMariaDB repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }
        public (bool reulst, string message) Destroy(string idMediaType)
        {
            MediaType mediaType = Repository.MediaTypes.FindByCondition(x => x.TypeId == idMediaType).FirstOrDefault();
            if (mediaType == null)
            {
                return (false, $"Media type with id : {idMediaType} not found");
            }

            mediaType.TypeStatus = MediaTypeStatusEnum.DELETED;
            Repository.SaveChanges();
            GeneralHelper.DeleteFile(mediaType.TypeIcon);
            return (true, "Deleted successfully");
        }

        public MediaType Show(string idMediaType)
        {
            return Repository.MediaTypes.FindByCondition(x => x.TypeId == idMediaType).FirstOrDefault();
        }

        public IQueryable<MediaType> ShowAll()
        {
            return Repository.MediaTypes
                .FindByCondition(x => x.TypeStatus == MediaTypeStatusEnum.USERD);
        }

        public (MediaType data, string message) Store(MediaType mediaType, string url)
        {
            //mediaType.TypeCode = GeneralHelper.RandomCode(5);
            Repository.MediaTypes.Add(mediaType);
            Repository.SaveChanges();
            if (!string.IsNullOrEmpty(mediaType.TypeIcon))
            {
                mediaType.TypeIcon = GeneralHelper.UrlCombine(url, mediaType.TypeIcon);
            }
            return (mediaType, "Created media type success");
        }

        public MediaType Update(MediaType mediaType)
        {
            if (mediaType.TypeIcon != null)
            {
                GeneralHelper.DeleteFile(mediaType.TypeIcon);
            }

            Repository.MediaTypes.Update(mediaType);
            Repository.SaveChanges();
            return mediaType;
        }
    }
}
