using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using Project.App.Database;

using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Services
{
    public interface IMediaService
    {
        Media Store(IFormFile MediaFile, string userId, string mediaType, StoredMediaRequest data);
        Media FindID(string mediaID);
        Media Update(UpdateInfoMediaRequest data, Media media);
        Media Delete(Media media); 
        List<Media> Search(SearchRequest data);
    }
    public class MediaService : IMediaService
    {
        private IConfiguration _config;
        private readonly MariaDBContext _mariaDBContext;
        private Random random = new Random();
        private StringBuilder build = new StringBuilder();
        public MediaService(MariaDBContext mariaDBContext, IConfiguration config)
        {
            _mariaDBContext = mariaDBContext;
            _config = config;
        }
        public Media Store(IFormFile MediaFile, string userId, string mediaType, StoredMediaRequest data)
        {

            string mediaTitle = CheckNameFile(MediaFile.FileName, data.OrganizeId, data.ParentId);
            string folderPath = ConvertPath(data.ParentId);
            folderPath = folderPath.Trim().Replace(" ", "_");
            string apiUploadFile = $"{_config["MediaService:MediaIp"]}/api/media/upload";
            MultipartFormDataContent multiContent = new MultipartFormDataContent();
            var fileStreamContent = new StreamContent(MediaFile.OpenReadStream());
            multiContent.Add(fileStreamContent, "mediaFiles", MediaFile.FileName);
            multiContent.Add(new StringContent(folderPath), "folderSave");
            (string ResponseData, int? ResponseStatusCode) ResponseGet = HttpMethod.Post.SendRequestWithFormDataContent(apiUploadFile, multiContent);
            JObject datas = JObject.Parse(ResponseGet.ResponseData);
            string fileName = (string)datas["data"][0]["mediaName"];
            Media media = new Media(
                    fileName,
                    folderPath,
                    userId,
                    mediaTitle,
                    mediaType,
                    data.OrganizeId,
                    data.ParentId
                );

            _mariaDBContext.Medias.Add(media);
            _mariaDBContext.SaveChanges();
            User user = _mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            media.UserName = user?.FullName;
            return media;
        }

        private string RandomString(int length, string chars)
        {
            build.Clear();
            int i = 0;
            while (i < length)
            {
                build.Append(chars[random.Next(0, chars.Length)]);
                i++;
            }
            return build.ToString();
        }
        public Media FindID(string mediaID)
        {
            Media media = _mariaDBContext.Medias.Where(x => x.MediaID.Equals(mediaID)).FirstOrDefault();
            if (media is null)
                return null;
            return media;
        }
        public Media Update(UpdateInfoMediaRequest data, Media media)
        {
            _mariaDBContext.Entry(media).CurrentValues.SetValues(data);
            _mariaDBContext.SaveChanges();
            return media;
        }
        public Media Delete(Media media)
        {
            string apiMediaService = $"{_config["MediaService:MediaIp"]}/api/media";
            string jsonString = JsonConvert.SerializeObject(new { mediaUrl = media.MediaPath + media.MediaName });
            Task<(string ResponseData, int? ResponseStatusCode)> ResponseGet = HttpMethod.Delete.SendRequestWithStringContentAsync(apiMediaService, jsonString);
            _mariaDBContext.Medias.Remove(media);
            _mariaDBContext.SaveChanges();
            return media;
        }
        public List<Media> Search(SearchRequest data)
        {
            List<Media> medias = _mariaDBContext.Medias
                .Where(x => x.OrganizeId.Equals(data.OrganizeId))
                .ToList();
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
            return medias;
        }
        public string ConvertPath(string ParentId, string path = "/")
        {
            Media mediaFolder = _mariaDBContext.Medias.FirstOrDefault(x => x.MediaID.Equals(ParentId));
            if (mediaFolder == null)
            {
                return path;
            }
            path = "/" + mediaFolder.MediaName + path;
            path = ConvertPath(mediaFolder.ParentId, path);
            return path;
        }
        public string CheckNameFile(string nameFile,string OrganizeId,string ParentId)
        {
            Media media = _mariaDBContext.Medias.FirstOrDefault(
                x => 
                    x.MediaTitle.Equals(nameFile) 
                    && x.OrganizeId.Equals(OrganizeId) 
                    && x.ParentId.Equals(ParentId)
                );
            if (media == null)
            {
                return nameFile;
            }
            nameFile = $"{RandomString(int.Parse(_config["Random:Length"]), $"{_config["Random:Char"]}{_config["Random:Char"].ToUpper()}{_config["Random:Number"]}")}_{nameFile}";
            nameFile = CheckNameFile(nameFile, media.OrganizeId, media.ParentId);
            return nameFile;
        }
    }
}
