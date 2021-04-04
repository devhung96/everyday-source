using Confluent.Kafka;
using Newtonsoft.Json;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Kafka;
using Project.Modules.Groups.Entities;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Services
{
    public interface IMediaSupport
    {
        public List<Media> GetMediaByListIds(GetMediaByIdsRequest request);
        public List<MediaIsGroup> CheckMediaInGroup(CheckMediaInGroupRequest request);
        public (object data, string message) ShareMediaIntoGroup(ShareIntoGroupRequest request);
        public (object data, string message) ApproveMedias(ShareMediasAllGroupRequest request);
        public (object data, string message) ShareListMediasAllGroup(ShareMediasAllGroupRequest request);

        public (object data, string message) ShareMultipleIntoGroup(ShareMultipleIntoGroupRequest request);
    }
    public class MediaSupport : IMediaSupport
    {
        private readonly IRepositoryWrapperMariaDB _repositoryWrapperMariaDB;
        private readonly KafkaProducer<string,string> _kafkaProducer;
        public MediaSupport(IRepositoryWrapperMariaDB repositoryWrapperMariaDB, KafkaProducer<string, string> kafkaProducer)
        {
            _repositoryWrapperMariaDB = repositoryWrapperMariaDB;
            _kafkaProducer = kafkaProducer;
        }

        public List<Media> GetMediaByListIds(GetMediaByIdsRequest request)
        {
            return _repositoryWrapperMariaDB.Medias.FindByCondition(x => request.MediaIds.Contains(x.MediaId)).ToList();
        }


        public List<MediaIsGroup> CheckMediaInGroup(CheckMediaInGroupRequest request)
        {

            List<MediaGroup> mediasInGroup = _repositoryWrapperMariaDB.MediaGroups.FindByCondition(x => x.GroupId.Equals(request.GroupId)).ToList();

            List<MediaIsGroup> result = new List<MediaIsGroup>();

            foreach (var item in mediasInGroup)
            {

                result.Add(new MediaIsGroup
                {
                    IsGroup = mediasInGroup.Any(x => x.MediaId.Equals(item.MediaId)),
                    MediaId = item.MediaId
                });
            }
            return result;
        }


        public (object data, string message) ShareMediaIntoGroup(ShareIntoGroupRequest request)
        {
            List<Media> medias = _repositoryWrapperMariaDB.Medias.FindByCondition(x => request.MediaIds.Contains(x.MediaId)).ToList();
            if (medias.Count != request.MediaIds.Count) return (null, "MediaNotFound");

            Group group = _repositoryWrapperMariaDB.Groups.FindByCondition(x => x.GroupId.Equals(request.GroupId)).FirstOrDefault();
            if (group is null) return (null, "GroupNotFound");

            List<MediaGroup> mediasInGroup = _repositoryWrapperMariaDB.MediaGroups.FindByCondition(x => x.GroupId.Equals(request.GroupId)).ToList();

            bool isShareMediaGroup = medias.Any(x => mediasInGroup.Select(x => x.MediaId).Contains(x.MediaId));
            if (isShareMediaGroup) return (null, "MediaAlreadyShared");

            List<MediaGroup> newMediaGroups = new List<MediaGroup>();
            foreach (var media in medias)
            {
                newMediaGroups.Add(new MediaGroup
                {
                    GroupId = group.GroupId,
                    MediaId = media.MediaId,
                });
            }
            _repositoryWrapperMariaDB.MediaGroups.AddRange(newMediaGroups);
            _repositoryWrapperMariaDB.SaveChanges();
            return (newMediaGroups, "Success");
        }

        public (object data, string message) ShareMultipleIntoGroup(ShareMultipleIntoGroupRequest request)
        {
            Media media = _repositoryWrapperMariaDB.Medias.FindByCondition(x => x.MediaId.Equals(request.MediaId)).FirstOrDefault();
            if (media is null) return (null, "MediaNotFound");

            List<Group> groups = _repositoryWrapperMariaDB.Groups.FindByCondition(x => request.GroupIds.Contains(x.GroupId)).ToList();
            if (groups.Count != request.GroupIds.Count) return (null, "GroupNotFound");

            List<MediaGroup> mediasInGroup = _repositoryWrapperMariaDB.MediaGroups.FindByCondition(x => x.MediaId.Equals(request.MediaId)).ToList();

            bool isShareMediaGroup = groups.Any(x => mediasInGroup.Select(x => x.GroupId).Contains(x.GroupId));
            if (isShareMediaGroup) return (null, "MediaAlreadyShared");

            List<MediaGroup> newMediaGroups = new List<MediaGroup>();
            foreach (var groupId in request.GroupIds.Where(x => !mediasInGroup.Select(x => x.GroupId).Contains(x)).ToList())
            {
                newMediaGroups.Add(new MediaGroup
                {
                    GroupId = groupId,
                    MediaId = media.MediaId,
                });
            }
            _repositoryWrapperMariaDB.MediaGroups.AddRange(newMediaGroups);
            _repositoryWrapperMariaDB.SaveChanges();
            return (newMediaGroups, "Success");
        }

        public (object data, string message) ShareListMediasAllGroup(ShareMediasAllGroupRequest request)
        {
            List<Media> medias = _repositoryWrapperMariaDB.Medias.FindByCondition(x => request.MediaIds.Contains(x.MediaId)).ToList();
            if (medias.Count != request.MediaIds.Count) return (null, "MediaNotFound");
            List<string> groupIds = _repositoryWrapperMariaDB.Groups.FindByCondition(x => x.Expired >= DateTime.UtcNow).Select(x => x.GroupId).ToList();

            List<MediaGroup> mediasInGroup = _repositoryWrapperMariaDB.MediaGroups.FindByCondition(x => request.MediaIds.Contains(x.MediaId)).ToList();

            List<MediaGroup> newMediaGroups = new List<MediaGroup>();
            foreach (Media media in medias)
            {
                foreach (var groupId in groupIds)
                {
                    if (!mediasInGroup.Any(x => x.MediaId.Equals(media.MediaId) && x.GroupId.Equals(groupId)))
                    {
                        newMediaGroups.Add(new MediaGroup
                        {
                            GroupId = groupId,
                            MediaId = media.MediaId,
                        });
                    }
                }
            }

            _repositoryWrapperMariaDB.MediaGroups.AddRange(newMediaGroups);
            _repositoryWrapperMariaDB.SaveChanges();
            return (newMediaGroups, "Success");
        }
        public (object data, string message) ApproveMedias(ShareMediasAllGroupRequest request)
        {
            List<Media> medias = _repositoryWrapperMariaDB.Medias.FindByCondition(x => request.MediaIds.Contains(x.MediaId)).ToList();
            if (medias.Count != request.MediaIds.Count) return (null, "MediaNotFound");
            foreach (Media media in medias)
            {
                media.MediaStatus = MediaStatusEnum.Confirm;
            }
            _repositoryWrapperMariaDB.Medias.UpdateRange(medias);
            _repositoryWrapperMariaDB.SaveChanges();
            _kafkaProducer.Produce(TopicDefine.SHARE_MEDIA_ALL_GROUP, new Message<string, string>() { Key = DateTime.Now.Ticks.ToString(),Value = JsonConvert.SerializeObject(request) });
            Console.WriteLine("--Send------SHARE_MEDIA_ALL_GROUP--------");
            return (medias, "Success");
        }
    }
}
