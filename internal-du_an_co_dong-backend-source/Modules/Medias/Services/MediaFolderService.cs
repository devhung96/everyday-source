using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using Project.App.Database;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Services
{
    public interface IMediaFolderService
    {
        Media StoredFolderOrganize(string userId, StoredFolderOrganizeRequest stored);
        Media Delete(string folderId, string host);
        Media FindId(string folderId, string host);
        Media Update(string folderId, string host, UpdateFileRequest updateFileRequest);
        List<Media> ShowAll();
        object ShowFolderOrganizeAll(string OrganizeId, string host);
        List<string> GetPathParent(string folderId, List<string> data);
        List<Media> GetPathParent2(string folderId, List<Media> data);
    }
    public class MediaFolderService : IMediaFolderService
    {
        private readonly MariaDBContext _mariaDBContext;
        private readonly IConfiguration _configuration;
        private Random random = new Random();
        private StringBuilder build = new StringBuilder();
        public MediaFolderService(MariaDBContext mariaDBContext, IConfiguration configuration)
        {
            _mariaDBContext = mariaDBContext;
            _configuration = configuration;
        }
        public Media Delete(string folderId, string host)
        {
            Media mediaFolder = FindId(folderId, host);
            if (mediaFolder == null)
            {
                return null;
            }
            List<Media> folders = new List<Media>();
            folders = GetPathParent2(mediaFolder.MediaID, folders);
            folders.Reverse();
            folders.Add(mediaFolder);
            foreach (var folder in folders)
            {
                string path = "";
                if (!folder.MediaType.Equals("folder"))
                { 
                    path = $"{folder.MediaPath}";
                    
                    string apiMediaService = $"{_configuration["MediaService:MediaIp"]}/api/media";
                    string jsonString = JsonConvert.SerializeObject(new { mediaUrl = path });
                    Task<(string ResponseData, int? ResponseStatusCode)> ResponseGet = HttpMethod.Delete.SendRequestWithStringContentAsync(apiMediaService, jsonString);
                    JObject datas = JObject.Parse(ResponseGet.Result.ResponseData);

                }
                else
                {
                    path = $"/{folder.MediaPath}";
                    string jsonInString = JsonConvert.SerializeObject(new { mediaFolderPath = path });
                    string apiMediaFolder = $"{_configuration["MediaService:MediaIp"]}/api/mediafolder/delete-path";
                    Task<(string ResponseData, int? ResponseStatusCode)> ResponseGet = HttpMethod.Post.SendRequestWithStringContentAsync(apiMediaFolder, jsonInString);
                    JObject datas = JObject.Parse(ResponseGet.Result.ResponseData);

                }

            }
            List<string> ids = new List<string>();
            ids = CheckParentIdDelete(mediaFolder.MediaID, ids);
            ids.Add(folderId);
            foreach (var item in ids)
            {
                Media id = FindId(item, host);
                if(id == null)
                {
                    continue;
                }
                _mariaDBContext.Medias.Remove(id);
                _mariaDBContext.SaveChanges();
            }
            return mediaFolder;
        }

        public List<string> GetPathParent(string folderId,List<string> data)
        {
            Media media = _mariaDBContext.Medias.FirstOrDefault(x => x.ParentId.Equals(folderId));
            if(media == null)
            {
                return data;
            }
            data.Add(media.MediaPath);
            return data;
        }
       
        public List<Media> GetPathParent2(string folderId, List<Media> data)
        {
            List<Media> medias = _mariaDBContext.Medias.Where(x => x.ParentId.Equals(folderId)).ToList();
            if (medias.Count == 0)
            {
                return data;
            }
            foreach (var media in medias)
            {
                if (!media.MediaType.Equals("folder"))
                {
                    media.MediaPath = media.MediaPath + media.MediaName;
                    data.Add(media);
                }
                else
                {
                    data.Add(media);
                }
                GetPathParent2(media.MediaID, data);
            }
           
            return data;
        }

        public List<string> CheckParentIdDelete(string folderId, List<string> id)
        {
            List<Media> mediaParents = _mariaDBContext.Medias.Where(x => x.ParentId.Equals(folderId)).ToList();
            if (mediaParents.Count == 0)
            {
                return id;
            }
            foreach (var mediaParent in mediaParents)
            {
                id.Add(mediaParent.MediaID);
                id = CheckParentIdDelete(mediaParent.MediaID, id);
            }
            return id;
        }

        public Media FindId(string folderId, string host)
        {
            Media mediaFolder = _mariaDBContext.Medias.FirstOrDefault(x => x.MediaID.Equals(folderId));
            if (mediaFolder == null)
            {
                return null;
            }
            if (String.IsNullOrEmpty(mediaFolder.MediaPath))
            {
                mediaFolder.MediaURL = $"{host}/{mediaFolder.MediaName}";
            }
            else
            {
                mediaFolder.MediaURL = $"{host}{mediaFolder.MediaPath}{mediaFolder.MediaName}";
            }
            return mediaFolder;
        }

        public Media StoredFolderOrganize(string userId, StoredFolderOrganizeRequest stored)
        {
            string path = ConvertPath(stored.ParentId);
            string FolderTitle = CheckNameFile(stored.MediaName, stored.OrganizeId,stored.ParentId);
            Media mediaFolder = new Media();
            mediaFolder.OrganizeId = stored.OrganizeId;
            mediaFolder.MediaName = FolderTitle;
            mediaFolder.MediaType = "folder";
            mediaFolder.ParentId = stored.ParentId;
            mediaFolder.MediaTitle = FolderTitle;
            mediaFolder.UserId = userId;
            User user = _mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            mediaFolder.UserName = user == null ? null : user.FullName;
            string dataInput = $"{path}{FolderTitle}".Substring(1);
            string jsonInString = JsonConvert.SerializeObject(new { mediaFolderPath = dataInput.Trim().Replace(" ", "_") });
            string apiMediaFolder = $"{_configuration["MediaService:MediaIp"]}/api/mediafolder";
            Task<(string ResponseData, int? ResponseStatusCode)> ResponseGet = HttpMethod.Post.SendRequestWithStringContentAsync(apiMediaFolder, jsonInString);
            if(ResponseGet.Result.ResponseStatusCode == 400)
            {
                return null;
            }    
            JObject data = JObject.Parse(ResponseGet.Result.ResponseData);
            mediaFolder.MediaPath = data["data"]["mediaFolderPath"].ToString();
            _mariaDBContext.Medias.Add(mediaFolder);
            _mariaDBContext.SaveChanges();
            return mediaFolder;
        }

        public string ConvertPath(string ParentId, string path = "/")
        {
            Media mediaFolder = _mariaDBContext.Medias.FirstOrDefault(x => x.MediaID.Equals(ParentId));
            if (mediaFolder == null)
                return path;
            path = "/" + mediaFolder.MediaName + path;
            path = ConvertPath(mediaFolder.ParentId, path);
            return path;
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
        public string CheckNameFile(string nameFile, string OrganizeId, string ParentId)
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
            nameFile = $"{RandomString(int.Parse(_configuration["Random:Length"]), $"{_configuration["Random:Char"]}{_configuration["Random:Char"].ToUpper()}{_configuration["Random:Number"]}")}_{nameFile}";
            nameFile = CheckNameFile(nameFile, media.OrganizeId, media.ParentId);
            return nameFile;
        }

        public Media Update(string folderId, string host, UpdateFileRequest updateFileRequest)
        {
            Media mediaFolder = FindId(folderId, host);
            if (mediaFolder == null)
                return null;
            mediaFolder.MediaTitle = updateFileRequest.MediaTitle;
            _mariaDBContext.SaveChanges();
            return mediaFolder;
        }

        public List<Media> ShowAll()
        {
            List<Media> mediaFolder = _mariaDBContext.Medias.ToList();
            return mediaFolder;
        }

        public object ShowFolderOrganizeAll(string OrganizeId, string host)
        {
            List<Media> mediaFolders = ShowAll().Where(x => x.OrganizeId == OrganizeId).ToList();
            foreach (var mediaFolder in mediaFolders)
            {
                if (!mediaFolder.MediaType.Equals("folder"))
                { 
                    mediaFolder.MediaURL = $"{host}{mediaFolder.MediaPath}{mediaFolder.MediaName}";
                }
                else
                {
                    mediaFolder.MediaURL = $"{host}/{mediaFolder.MediaPath}";
                }
            }
            return mediaFolders;
        }

        
    }
}
