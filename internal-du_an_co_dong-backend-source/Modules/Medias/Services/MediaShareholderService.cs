
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Project.App.Database;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Services
{
    public interface IMediaShareholderService
    {
        Media Store(IFormFile MediaFile, string userId, string mediaType, StoredMediaShareholderRequest data);
        Media FindID(string mediaID);
        Media Update(UploadMediaOrganizeRequest data, Media media);
        Media Delete(Media media);
        List<Media> Search(ShowAllMediaOrganizeRequest data, string host = null);
    }
    public class MediaShareholderService : IMediaShareholderService
    {
        private IConfiguration _config;
        private readonly MariaDBContext _mariaDBContext;
        public MediaShareholderService(MariaDBContext mariaDBContext, IConfiguration config)
        {
            _mariaDBContext = mariaDBContext;
            _config = config;
        }
        public Media Store(IFormFile MediaFile, string userId, string mediaType, StoredMediaShareholderRequest data)
        {
            string folderPath = "";
            folderPath = ConvertPath(data.ParentId, folderPath);
            string fileName = MediaFile.UploadFile(folderPath).Result;
            Media media = new Media(
                    fileName,
                    folderPath,
                    userId,
                    data.MediaTitle,
                    mediaType,
                    data.OrganizeId,
                    data.ParentId
                );
            _mariaDBContext.Medias.Add(media);
            _mariaDBContext.SaveChanges();
            return media;
        }
        public Media FindID(string mediaID)
        {
            Media media = _mariaDBContext.Medias.Where(x => x.MediaID.Equals(mediaID)).FirstOrDefault();
            if (media is null)
                return null;
            media.MediaURL = _config["UrlImage"] + media.MediaURL;
            return media;
        }
        public Media Update(UploadMediaOrganizeRequest data, Media media)
        {
            _mariaDBContext.Entry(media).CurrentValues.SetValues(data);
            _mariaDBContext.SaveChanges();
            return media;
        }

        public Media Delete(Media media)
        {
            _mariaDBContext.Medias.Remove(media);
            _mariaDBContext.SaveChanges();
            return media;
        }

        public List<Media> Search(ShowAllMediaOrganizeRequest data, string host = null)
        {
            List<Media> medias = _mariaDBContext.Medias
                .Where(x =>
                x.OrganizeId.Equals(data.OrganizeId)
                ).ToList(); ;
            switch (data.SearchType)
            {
                case "KeyWord":
                    medias = medias.Where(x => x.MediaTitle.Contains(data.SearchContent)).ToList();
                    break;
                case "ByTime":
                    medias = medias.Where(x => x.CreatedAt.Month == DateTime.Parse(data.SearchContent).Month && x.CreatedAt.Year == DateTime.Parse(data.SearchContent).Year).ToList();
                    break;
                default:
                    medias = medias.OrderByDescending(x => x.CreatedAt).ToList();
                    break;
            }
            foreach (var item in medias)
            {
                item.MediaURL = $"{host}{item.MediaURL}";

            }
            return medias;
        }
        public string ConvertPath(string ParentId, string path)
        {
            Media mediaFolder = _mariaDBContext.Medias.FirstOrDefault(x => x.MediaID.Equals(ParentId));
            if (mediaFolder == null)
                return path;
            path = "/" + mediaFolder.MediaName + path;
            path = ConvertPath(mediaFolder.ParentId, path);
            return path;
        }

    }
}
