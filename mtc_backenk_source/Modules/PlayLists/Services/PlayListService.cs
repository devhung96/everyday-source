using Project.Modules.PlayLists.Requests;
using Project.Modules.PlayLists.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Modules.Users.Entities;
using Project.App.DesignPatterns.Reponsitories;
using AutoMapper;
using Project.App.Helpers;
using Microsoft.EntityFrameworkCore.Storage;
using Project.Modules.Templates.Services;
using Project.Modules.Templates.Entities;
using Project.Modules.Templates.Requests;
using Project.Modules.TemplateDetails.Entities;
using Project.Modules.TemplateDetails.Requests;

namespace Project.Modules.PlayLists.Services
{
    public interface IPlayListService
    {
        (PaginationResponse<PlayList> data, string message) ShowAll(PaginationRequest requestTable, string userId,string url);
        (PaginationResponse<PlayList> data, string message) ShowAllV2(PaginationRequest requestTable, string userId, string url, string groupId);
        (PlayList data, string message) FindID(string playListID, string url = null);
        (PlayList data, string message) Store(StorePlayListRequest value, string userID);
        (PlayList data, string message) Delete(string playListID);
        (PlayList data, string message) AssignUser(string playListID, string userId);
        (PlayList data, string message) Edit(PlayList playList, UpdatePlayListRequest valueUpdate);
        (object data, string message) StoreMultiple(StoreMultiPlayListDetailRequest value, string userID);
        (object data, string message) DeletePlayListDetail(string playlistId);
        (bool data, string message) CopyPlayListDetail(CopyPlaylistDetailRequest valueInput, string userId);
        (object data, string message) StoreMultipleV2(StoreMultiPlayListDetailRequest value, string userID);

        (PlayList data, string message) EditV2(UpdatePlayListRequest valueUpdate, string playListId);

    }
    public class PlayListService : IPlayListService
    {
        private readonly IRepositoryWrapperMariaDB repositoryWrapperMariaDB;
        private readonly IMapper Mapper;
        public PlayListService(IRepositoryWrapperMariaDB _repositoryWrapperMariaDB, IMapper _Mapper)
        {
            repositoryWrapperMariaDB = _repositoryWrapperMariaDB;
            Mapper = _Mapper;
        }

        public (PlayList data, string message) AssignUser(string playListID, string userId)
        {
            (PlayList playList, string message) = FindID(playListID);
            if (playList == null)
                return (null, message);
            if(playList.PlayListAssignUserId != userId)
            {
                DeletePlayListDetail(playList.PlayListId);
            }
            playList.PlayListAssignUserId = userId;
            repositoryWrapperMariaDB.PlayLists.Update(playList);
            repositoryWrapperMariaDB.SaveChanges();
            return (playList, "AssignUserSuccess");
        }

        public (PlayList data, string message) Delete(string playListId)
        {
            PlayList playList = repositoryWrapperMariaDB.PlayLists.FindByCondition(x=>x.PlayListId.Equals(playListId)).FirstOrDefault();
            if (playList == null)
                return (null, "playListIdNotFound");
            //PlayListDetail playListDetail = _context.PlayListDetails.Where(x => x.playListId == playList.value.playListId).FirstOrDefault();
            //if (playListDetail != null)
            //    return (null, "Delete Unsuccessfull. PlayListDetail exist PlayList !!!");
            repositoryWrapperMariaDB.PlayLists.Remove(playList);
            repositoryWrapperMariaDB.SaveChanges();
            return (playList, "DeletePlaylistSuccess");
        }
         
        public (PlayList data, string message) Edit(PlayList playList, UpdatePlayListRequest valueUpdate)
        {
            if(!string.IsNullOrEmpty(valueUpdate.PlayListAssignUserId) && playList.PlayListAssignUserId != valueUpdate.PlayListAssignUserId)
            {
                DeletePlayListDetail(playList.PlayListId);
            }
            playList = Mapper.Map(valueUpdate, playList);
            repositoryWrapperMariaDB.PlayLists.Update(playList);
            repositoryWrapperMariaDB.SaveChanges();
            return (playList, "UpdatePlaylistSuccess");
        }


        public (PlayList data, string message) EditV2(UpdatePlayListRequest valueUpdate , string playListId)
        {
            PlayList playList =  repositoryWrapperMariaDB.PlayLists.FindByCondition(x => x.PlayListId.Equals(playListId)).FirstOrDefault();
            if (playList == null)
                return (null, "playListIdNotFound");

            if (!string.IsNullOrEmpty(valueUpdate.PlayListAssignUserId) && playList.PlayListAssignUserId != valueUpdate.PlayListAssignUserId)
            {
                DeletePlayListDetail(playList.PlayListId);
            }
            playList = Mapper.Map(valueUpdate, playList);
            repositoryWrapperMariaDB.PlayLists.Update(playList);
            repositoryWrapperMariaDB.SaveChanges();
            return (playList, "UpdatePlaylistSuccess");
        }

        public (PlayList data, string message) FindID(string playListId, string url = null)
        {
            PlayList playList = repositoryWrapperMariaDB.PlayLists.FindByCondition(x => x.PlayListId.Equals(playListId))
                .Include(x=>x.PlayListDetail)
                .ThenInclude(x=>x.Template)
                .ThenInclude(x=>x.TemplateDetails)
                .ThenInclude(x => x.Media)
                .Include(x=>x.User)
                .FirstOrDefault();
            if (playList == null)
                return (null, "playListIdNotFound");
            foreach (var playListDetail in playList.PlayListDetail)
            {
                foreach (var templateDetail in playListDetail.Template.TemplateDetails)
                {
                    templateDetail.Media.CombineMediaUrl(url);
                }
            }
            return (playList, "ShowDetailPlaylitsSuccess");
        }

        public (PaginationResponse<PlayList> data, string message) ShowAll(PaginationRequest requestTable, string userId, string url)
        {
            IQueryable<PlayList> playLists = repositoryWrapperMariaDB.PlayLists
                .FindAll()
                .Include(x => x.User)
                .Include(x=>x.PlayListDetail)
                .ThenInclude(x=>x.Template)
                .ThenInclude(x=>x.TemplateDetails)
                .ThenInclude(x=>x.Media)
                .OrderByDescending(x => x.CreatedAt);
            playLists = playLists.Where(x => x.UserId == userId);
            List<PlayList> data = playLists.Where(x =>
                string.IsNullOrEmpty(requestTable.SearchContent) ||
                (!string.IsNullOrEmpty(requestTable.SearchContent) &&
                    (
                        x.PlayListName.ToLower().Contains(requestTable.SearchContent.ToLower()) ||
                        x.CreatedAt.ToString().ToLower().Contains(requestTable.SearchContent.ToLower()) ||
                        x.PlayListComment.ToLower().Contains(requestTable.SearchContent.ToLower())
                    )
                )).ToList();
            foreach (PlayList playList in data)
            {
                foreach (var playListDetail in playList.PlayListDetail)
                {
                    foreach (var TemplateDetail in playListDetail.Template.TemplateDetails)
                    {
                        TemplateDetail.Media.CombineMediaUrl(url);
                    }
                }
            }
            var result = data.AsQueryable();
            PaginationHelper<PlayList> playListInfo = PaginationHelper<PlayList>.ToPagedList(result, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<PlayList> paginationResponse = new PaginationResponse<PlayList>(playListInfo, playListInfo.PageInfo);
            return (paginationResponse, "ShowAllSuccess");
        }

        public (PaginationResponse<PlayList> data, string message) ShowAllV2(PaginationRequest requestTable, string userId, string url, string groupId)
        {
            IQueryable<PlayList> playLists = repositoryWrapperMariaDB.PlayLists
                .FindAll()
                .Include(x => x.User)
                .Where(x=>x.User.GroupId == groupId)
                .Include(x => x.PlayListDetail)
                .ThenInclude(x => x.Template)
                .ThenInclude(x => x.TemplateDetails)
                .ThenInclude(x => x.Media)
                .OrderByDescending(x => x.CreatedAt);
            //playLists = playLists.Where(x => x.UserId == userId);
            List<PlayList> data = playLists.Where(x =>
                string.IsNullOrEmpty(requestTable.SearchContent) ||
                (!string.IsNullOrEmpty(requestTable.SearchContent) &&
                    (
                        x.PlayListName.ToLower().Contains(requestTable.SearchContent.ToLower()) ||
                        x.CreatedAt.ToString().ToLower().Contains(requestTable.SearchContent.ToLower()) ||
                        x.PlayListComment.ToLower().Contains(requestTable.SearchContent.ToLower())
                    )
                )).ToList();
            foreach (PlayList playList in data)
            {
                foreach (var playListDetail in playList.PlayListDetail)
                {
                    foreach (var TemplateDetail in playListDetail.Template.TemplateDetails)
                    {
                        TemplateDetail.Media.CombineMediaUrl(url);
                    }
                }
            }
            var result = data.AsQueryable();
            PaginationHelper<PlayList> playListInfo = PaginationHelper<PlayList>.ToPagedList(result, requestTable.PageNumber, requestTable.PageSize);
            PaginationResponse<PlayList> paginationResponse = new PaginationResponse<PlayList>(playListInfo, playListInfo.PageInfo);
            return (paginationResponse, "ShowAllSuccess");
        }

        public (PlayList data, string message) Store(StorePlayListRequest value, string userId)
        {
            PlayList data = Mapper.Map<PlayList>(value);
            data.UserId = userId;
            repositoryWrapperMariaDB.PlayLists.Add(data);
            repositoryWrapperMariaDB.SaveChanges();
            return (data, "CreatePlayListSuccess");

        }
        public (object data, string message) StoreMultiple(StoreMultiPlayListDetailRequest value, string userID)
        {

            IDbContextTransaction dbContextTransaction = repositoryWrapperMariaDB.BeginTransaction();
            PlayList data = new PlayList()
            {
                PlayListName = value.PlayListName,
                PlayListComment = value.PlayListComment,
                PlayListLoop = value.PlayListLoop,
                PlayListAssignUserId = value.PlayListAssignUserId
            };
            data.UserId = userID;
            repositoryWrapperMariaDB.PlayLists.Add(data);
            repositoryWrapperMariaDB.SaveChanges();
            try
            {
                List<PlayListDetail> playListDetails = new List<PlayListDetail>();
                foreach (StorePlayListDetailMultipleRequest item in value.storePlayListDetails)
                {
                    PlayListDetail orderNewPlaylistDetail = repositoryWrapperMariaDB.PlayListDetails.FindByCondition(x => x.PlayListId.Equals(data.PlayListId)).ToList().LastOrDefault();
                    item.OrderMedia = 0;
                    if (orderNewPlaylistDetail != null)
                    {
                        item.OrderMedia = orderNewPlaylistDetail.OrderMedia + 1;
                    }
                    else
                    {
                        item.OrderMedia = 1;
                    };
                    PlayListDetail playListDetail = Mapper.Map<PlayListDetail>(item);
                    playListDetail.PlayListId = data.PlayListId;
                    repositoryWrapperMariaDB.PlayListDetails.Add(playListDetail);
                    repositoryWrapperMariaDB.SaveChanges();
                    playListDetails.Add(playListDetail);
                }
                dbContextTransaction.Commit();
                return (new { data, playListDetails }, "CreatePlayListSuccess");
            }
            catch (Exception e)
            {
                dbContextTransaction.Rollback();
                return (null, e.InnerException.Message);
            }

        }
        public (object data, string message) StoreMultipleV2(StoreMultiPlayListDetailRequest value, string userID)
        {
            IDbContextTransaction dbContextTransaction = repositoryWrapperMariaDB.BeginTransaction();
            PlayList data = new PlayList()
            {
                PlayListName = value.PlayListName,
                PlayListComment = value.PlayListComment,
                PlayListLoop = value.PlayListLoop,
                PlayListAssignUserId = value.PlayListAssignUserId
            };
            data.UserId = userID;
            repositoryWrapperMariaDB.PlayLists.Add(data);
            repositoryWrapperMariaDB.SaveChanges();
            try
            {
                List<PlayListDetail> playListDetails = new List<PlayListDetail>();
                foreach (StorePlayListDetailMultipleRequest item in value.storePlayListDetails)
                {
                    if(string.IsNullOrEmpty(item.TemplateId) && !string.IsNullOrEmpty(item.MediaId))
                    {
                        StoreTemplateRequest storeTemplateRequest = new StoreTemplateRequest()
                        {
                            TemplateName = "Template Default"+ 6.RandomString(),
                            TemplateRatioX = 0,
                            TemplateRatioY = 0,
                            TemplateRotate = 0,
                            TemplateDuration = item.TemplateDuration,
                        };
                        (Template template, string message) = Store(storeTemplateRequest, userID);
                        if(template is null)
                        {
                            dbContextTransaction.Rollback();
                            return (null, message);
                        }
                        StoreTemplateDetail storeTemplateDetail = new StoreTemplateDetail()
                        {
                            Zindex = 0,
                            MediaId = item.MediaId,
                            TemplateId = template.TemplateId,
                            TempRatioX = 0,
                            TempRatioY =0,
                            TempPointWidth = 0,
                            TempPointHeight = 0
                        };
                        (TemplateDetail templateDetail, string messageTemplateDetail) = StoreTemplateDetail(storeTemplateDetail);
                        if (templateDetail is null)
                        {
                            dbContextTransaction.Rollback();
                            return (null, message);
                        }
                        item.TemplateId = template.TemplateId;
                    }
                    PlayListDetail orderNewPlaylistDetail = repositoryWrapperMariaDB.PlayListDetails.FindByCondition(x => x.PlayListId.Equals(data.PlayListId)).ToList().LastOrDefault();
                    item.OrderMedia = 0;
                    if (orderNewPlaylistDetail != null)
                    {
                        item.OrderMedia = orderNewPlaylistDetail.OrderMedia + 1;
                    }
                    else
                    {
                        item.OrderMedia = 1;
                    };
                    PlayListDetail playListDetail = Mapper.Map<PlayListDetail>(item);
                    playListDetail.PlayListId = data.PlayListId;
                    repositoryWrapperMariaDB.PlayListDetails.Add(playListDetail);
                    repositoryWrapperMariaDB.SaveChanges();
                    playListDetails.Add(playListDetail);
                }
                dbContextTransaction.Commit();
                return (new { data, playListDetails }, "CreatePlayListSuccess");
            }
            catch (Exception e)
            {
                dbContextTransaction.Rollback();
                return (null, e.InnerException.Message);
            }
        }
        public (Template template, string message) Store(StoreTemplateRequest request, string userId)
        {
            try
            {
                Template template = new Template()
                {
                    TemplateName = request.TemplateName,
                    TemplateRatioX = request.TemplateRatioX,
                    TemplateRatioY = request.TemplateRatioY,
                    TemplateRotate = request.TemplateRotate,
                    TemplateDuration = request.TemplateDuration,
                    UserId = userId
                };
                repositoryWrapperMariaDB.Templates.Add(template);
                repositoryWrapperMariaDB.SaveChanges();
                return (template, "CreateTemplateSuccess");

            }
            catch (Exception e)
            {

                return (null, $"{e.StackTrace}-----{e.Message}");
            }
            
        }
        public (TemplateDetail templateDetail, string message) StoreTemplateDetail(StoreTemplateDetail storeTemplateDetail)
        {
            try
            {
                TemplateDetail templateDetail = new TemplateDetail()
                {
                    Zindex = storeTemplateDetail.Zindex,
                    MediaId = storeTemplateDetail.MediaId,
                    TemplateId = storeTemplateDetail.TemplateId,
                    TempRatioX = storeTemplateDetail.TempRatioX,
                    TempRatioY = storeTemplateDetail.TempRatioY,
                    TempPointWidth = storeTemplateDetail.TempPointWidth,
                    TempPointHeight = storeTemplateDetail.TempPointHeight
                };
                repositoryWrapperMariaDB.TemplateDetails.Add(templateDetail);
                repositoryWrapperMariaDB.SaveChanges();
                return (templateDetail, "CreateTemplateDetailSuccess");
            }
            catch (Exception e)
            {
                return (null, $"{e.StackTrace}-----{e.Message}");
            }
        }
        public (object data, string message) DeletePlayListDetail(string playlistId)
        {
            List<PlayListDetail> playListDetails = repositoryWrapperMariaDB.PlayListDetails.FindByCondition(x => x.PlayListId.Equals(playlistId)).ToList();
            repositoryWrapperMariaDB.PlayListDetails.RemoveRange(playListDetails);
            repositoryWrapperMariaDB.SaveChanges();
            return (playListDetails, "DeletePlayListDetailSuccess");
        }

        public (bool data, string message) CopyPlayListDetail(CopyPlaylistDetailRequest valueInput, string userId)
        {
            PlayList playListCopy = repositoryWrapperMariaDB.PlayLists.FindByCondition(x => x.PlayListId.Equals(valueInput.PlayListIdCopy)).FirstOrDefault();
            if (playListCopy is null)
            {
                return (false, "playListIdNotFound");
            }
            if (playListCopy.UserId != userId && playListCopy.PlayListAssignUserId != userId)
            {
                return (false, "copyFail");
            }
            List<PlayListDetail> playListDetailCopy = repositoryWrapperMariaDB.PlayListDetails.FindByCondition(x => x.PlayListId.Equals(playListCopy.PlayListId)).ToList();
            PlayList playListPaste = repositoryWrapperMariaDB.PlayLists.FindByCondition(x => x.PlayListId.Equals(valueInput.PlayListIdPaste)).FirstOrDefault();
            if (playListPaste is null)
            {
                return (false, "playListIdNotFound");
            }
            List<PlayListDetail> playListDetailOlds = repositoryWrapperMariaDB.PlayListDetails.FindByCondition(x => x.PlayListId.Equals(playListPaste.PlayListId)).ToList();
            if(playListDetailOlds.Count > 0)
            {
                repositoryWrapperMariaDB.PlayListDetails.RemoveRange(playListDetailOlds);
            }
            foreach (var item in playListDetailCopy)
            {
                PlayListDetail playListDetailPaste = new PlayListDetail()
                {
                    TemplateId = item.TemplateId,
                    TimeBegin = item.TimeBegin,
                    TimeEnd = item.TimeEnd,
                    OrderMedia = item.OrderMedia,
                    PlayListId = playListPaste.PlayListId,
                    CreatedAt = item.CreatedAt,
                    UpdatedAt = item.UpdatedAt,
                    TemplateDuration = item.TemplateDuration
                };
                repositoryWrapperMariaDB.PlayListDetails.Add(playListDetailPaste);
            }
            repositoryWrapperMariaDB.SaveChanges();
            return (true, "CopyPlayListDetailSuccess");
        }
    }
}
