using Microsoft.EntityFrameworkCore;
using Project.App.DesignPatterns.Reponsitories;
using Project.Modules.Groups.Entities;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Response;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Project.Modules.Medias.Services
{

    public interface IMediaService
    {
        User DetailUser(string userId);
        Media DetailMedia(string mediaId);
        Media Store(Media media, string url);
        // api active deactive dung api update luon.
        (Media data, string message) Update(Media media);
        void Destroy(Media media, string url);
        void RenderVideoffmpeg(string strUrl);


        (List<Media> data, string message) ShowAll(string url, string userId);
        (Media data, string message) GetMediaForDevice(string idMedia, string url);
        //(List<Media> data, string message) ShowMediaNotActivated(string url, string userId);
        //(List<Media> data, string message) ShowMediaActivated(string url, string userId);
        //(List<Media> data, string message) ShowMediaByTypeMedia(string idMediaType, string url, string userId);

        IQueryable<Media> ListMedia();
        object ListMediaOnYear();
        IEnumerable<MediaGroupResponse> GetListMediaGroup(Media media);
        Group DetailGroupUser(string groupId);
        void DeleteGroupMediaOld(Media media);
        void UpdateListMediaGroup(List<MediaGroup> mediaGroups);
    }
    public class MediaService : IMediaService
    {
        private readonly IRepositoryWrapperMariaDB Repository;
        public MediaService(IRepositoryWrapperMariaDB repository)
        {
            Repository = repository;
        }

        public object ListMediaOnYear()
        {
            var media = ListMedia().OrderByDescending(x => x.CreateAt).ToList().GroupBy(x => x.CreateAt.Year);
            return media.Select(x => new ListMediaOnYear(x.Key, x)).ToList();
        }

        public User DetailUser(string userId)
        {
            return Repository.Users.FindByCondition(x => x.UserId.Equals(userId)).FirstOrDefault();
        }

        public IQueryable<Media> ListMedia()
        {
            return Repository.Medias.FindAll().Include(x => x.User).Include(x => x.MediaGroups);
        }

        public Media DetailMedia(string mediaId)
        {
            return Repository.Medias
                .FindByCondition(x => x.MediaId.Equals(mediaId))
                .Include(x => x.User)
                .Include(x => x.MediaGroups)
                .FirstOrDefault();
        }

        public void Destroy(Media media, string url)
        {
            if (media.MediaUrl != null && media.TypeId != "Link")
            {
                string urlVideo = GeneralHelper.UrlCombine(url, media.MediaUrl);
                string forderRender = GeneralHelper.GetFileNameWithoutExtension(urlVideo);
                var didretory = GeneralHelper.GetDirectoryFromFile(urlVideo);
                string yourDirectory = GeneralHelper.UrlCombine(didretory, forderRender);
                try
                {
                    GeneralHelper.DeleteFile(media.MediaUrl);
                    GeneralHelper.ClearFolder(yourDirectory);
                }
                catch (Exception)
                {
                    Console.WriteLine("File Not Found");
                }
            }

            if (media.MediaUrlOptional != null)
            {
                try
                {
                    GeneralHelper.DeleteFile(media.MediaUrlOptional);
                }
                catch (Exception)
                {
                    Console.WriteLine("File Not Found");
                }
            }
            //media.MediaStatus = MediaStatusEnum.DELETED;
            //Repository.Medias.Update(media);
            Repository.Medias.Remove(media);
            Repository.SaveChanges();
        }

        public (List<Media> data, string message) ShowAll(string url, string userId)
        {
            User user = DetailUser(userId);
            List<Media> medias = Repository.Medias
                .FindAll()
                .Include(x => x.User)
                .Where(x => user.UserLevel == UserLevelEnum.SUPERADMIN || x.UserId == userId)
                .Select(x => new Media
                {
                    MediaId = x.MediaId,
                    MediaName = x.MediaName,
                    MediaUrl = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(x.MediaUrl)),
                    MediaStatus = x.MediaStatus,
                    TypeId = x.TypeId,
                    MediaComment = x.MediaComment,
                    MediaSize = x.MediaSize,
                    MediaDuration = x.MediaDuration,
                    UserId = x.UserId,
                    MediaMd5 = x.MediaMd5,
                    CreateAt = x.CreateAt,
                    UpdatedAt = x.UpdatedAt,
                    User = x.User,
                    MediaTypeCode = x.MediaTypeCode
                })
                .OrderByDescending(x => x.CreateAt)
                .ToList();

            return (medias, "ShowAllSuccess");
        }

        //public (List<Media> data, string message) ShowMediaActivated(string url, string userId)
        //{
        //    User user = DetailUser(userId);
        //    List<Media> medias = Repository.Medias
        //        .FindAll()
        //        .Include(x => x.User)
        //        .Where(x => (user.UserLevel == UserLevelEnum.SUPERADMIN || x.UserId == userId) && x.MediaStatus == MediaStatusEnum.ACTIVE)
        //        .Select(x => new Media
        //        {
        //            MediaId = x.MediaId,
        //            MediaName = x.MediaName,
        //            MediaUrl = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(x.MediaUrl)),
        //            MediaStatus = x.MediaStatus,
        //            TypeId = x.TypeId,
        //            MediaComment = x.MediaComment,
        //            MediaSize = x.MediaSize,
        //            MediaDuration = x.MediaDuration,
        //            UserId = x.UserId,
        //            MediaMd5 = x.MediaMd5,
        //            CreateAt = x.CreateAt,
        //            User = x.User,
        //            UpdatedAt = x.UpdatedAt,
        //            MediaTypeCode = x.MediaTypeCode
        //        })
        //         .OrderByDescending(x => x.CreateAt)
        //        .ToList();
        //    return (medias, "Show all success");
        //}

        //public (List<Media> data, string message) ShowMediaByTypeMedia(string idMediaType, string url, string userId)
        //{
        //    User user = DetailUser(userId);
        //    List<Media> medias = Repository.Medias
        //        .FindAll()
        //        .Include(x => x.User)
        //        .Where(x => (user.UserLevel == UserLevelEnum.SUPERADMIN || x.UserId == userId))
        //        .Select(x => new Media
        //        {
        //            MediaId = x.MediaId,
        //            MediaName = x.MediaName,
        //            MediaUrl = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(x.MediaUrl)),
        //            MediaStatus = x.MediaStatus,
        //            TypeId = x.TypeId,
        //            MediaComment = x.MediaComment,
        //            MediaSize = x.MediaSize,
        //            MediaDuration = x.MediaDuration,
        //            UserId = x.UserId,
        //            MediaMd5 = x.MediaMd5,
        //            CreateAt = x.CreateAt,
        //            UpdatedAt = x.UpdatedAt,
        //            User = x.User,
        //            MediaTypeCode = x.MediaTypeCode
        //        })
        //        .OrderByDescending(x => x.CreateAt)
        //        .ToList();
        //    return (medias, "Show all success");
        //}

        //public (List<Media> data, string message) ShowMediaNotActivated(string url, string userId)
        //{
        //    User user = DetailUser(userId);
        //    List<Media> medias = Repository.Medias
        //        .FindAll()
        //        .Include(x => x.User)
        //        .Where(x => (user.UserLevel == UserLevelEnum.SUPERADMIN || x.UserId == userId) && x.MediaStatus == MediaStatusEnum.DEACTIVATED)
        //        .Select(x => new Media
        //        {
        //            MediaId = x.MediaId,
        //            MediaName = x.MediaName,
        //            MediaUrl = GeneralHelper.UrlCombine(url, GeneralHelper.GetURLVideo(x.MediaUrl)),
        //            MediaStatus = x.MediaStatus,
        //            TypeId = x.TypeId,
        //            MediaComment = x.MediaComment,
        //            MediaSize = x.MediaSize,
        //            MediaDuration = x.MediaDuration,
        //            UserId = x.UserId,
        //            MediaMd5 = x.MediaMd5,
        //            CreateAt = x.CreateAt,
        //            UpdatedAt = x.UpdatedAt,
        //            User = x.User,
        //            MediaTypeCode = x.MediaTypeCode
        //        })
        //        .OrderByDescending(x => x.CreateAt)
        //        .ToList();
        //    return (medias, "Show all success");
        //}

        public Media Store(Media media, string url)
        {
            if(media.TypeId != "Link")
            {
                string urlVideo = GeneralHelper.UrlCombine(url, media.MediaUrl);
                media.MediaMd5 = GeneralHelper.GetMD5File(urlVideo); // set md5 file video
                RenderVideoffmpeg(urlVideo);
                if (string.IsNullOrEmpty(media.MediaName))
                {
                    media.MediaName = GeneralHelper.GetFileNameWithoutExtension(urlVideo);
                }
            }
            
            Repository.Medias.Add(media);
            Repository.SaveChanges();
            return media;
        }

        public void RenderVideoffmpeg(string strUrl)
        {
            string forderRender = GeneralHelper.GetFileNameWithoutExtension(strUrl);
            string nameFile = GeneralHelper.GetFileName(strUrl);
            var didretory = GeneralHelper.GetDirectoryFromFile(strUrl);
            string pathVideo = GeneralHelper.UrlCombine(didretory, nameFile);
            string pathM3u8 = GeneralHelper.UrlCombine(didretory, forderRender, forderRender + ".m3u8");
            string yourDirectory = GeneralHelper.UrlCombine(didretory, forderRender);

            if (!Directory.Exists(yourDirectory))
            {
                Directory.CreateDirectory(yourDirectory);
            }

            //string commnadRun = ($" -i {pathVideo} -profile:v baseline -level 3.0  -start_number 0 -map 0 -hls_list_size 0  -f hls {pathM3u8} 2>/dev/null >/dev/null &");
            string commnadRun = ($"-i {pathVideo} -codec: copy -bsf:v h264_mp4toannexb -start_number 0 -hls_time 10 -hls_list_size 0 -f hls {pathM3u8} ");
            Console.WriteLine(commnadRun);
            GeneralHelper.ffmpegRun(commnadRun);
        }

        public (Media data, string message) Update(Media media)
        {
            Repository.Medias.Update(media);
            Repository.SaveChanges();
            return (media, "UpdatedSuccess");

        }

        public (Media data, string message) GetMediaForDevice(string idMedia, string url)
        {
            Media media = Repository.Medias
                .FindAll()
                .Include(x => x.User)
                .FirstOrDefault(x => x.MediaId == idMedia);
            if (media == null)
            {
                return (null, $"Media with id: {idMedia} not found");
            }

            media.MediaUrl = GeneralHelper.UrlCombine(url, media.MediaUrl);
            return (media, "ShowSuccess");
        }

        public IEnumerable<MediaGroupResponse> GetListMediaGroup(Media media)
        {
            List<Group> groups = Repository.Groups.FindAll().ToList();
            List<string> mediaGroups = 
                Repository.MediaGroups.FindByCondition(x => x.MediaId.Equals(media.MediaId)).Select(x => x.GroupId).ToList();

            IEnumerable<MediaGroupResponse> response =
                groups.Select(x => new MediaGroupResponse(x, mediaGroups.Contains(x.GroupId) ? true : false));
            
            return response;
        }

        public Group DetailGroupUser(string groupId)
        {
            return Repository.Groups.FindByCondition(x => x.GroupId.Equals(groupId)).FirstOrDefault();
        }

        public void DeleteGroupMediaOld(Media media)
        {
            var mediaGroups = Repository.MediaGroups.FindByCondition(x => x.MediaId.Equals(media.MediaId));
            Repository.MediaGroups.RemoveRange(mediaGroups);
            Repository.SaveChanges();
        }

        public void UpdateListMediaGroup(List<MediaGroup> mediaGroups)
        {
            Repository.MediaGroups.AddRange(mediaGroups);
            Repository.SaveChanges();
        }
    }

}