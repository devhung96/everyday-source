using AutoMapper;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Project.App.Helpers;
using Project.Modules.Kafka.Producer;
using Project.Modules.Schedules.Requests;
using Project.Modules.Schedules.Services;
using Project.Modules.Tags.Enities;
using Project.Modules.Tags.Requests;
using Project.Modules.Users.Entities;
using Project.Modules.UsersModes.Entities;
using Project.Modules.UserTagModes.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Tags.Services
{

    public interface ITagService
    {
        public (Tag tag, string message) InsertTag(InsertTagRequest request);

        public (bool result, string message) DeleteTag(string tagId);

        public (Tag tag, string message) UpdateTag(string tagId, UpdateTagRequest request);

        public (Tag tag, string message) ShowById(string tagId);

        public object ShowAll(PaginationRequest request);

        public  (Tag tag, string message) UpdateReponsitory(string tagCode, string repositoryId);

    }
    public class TagService : ITagService
    {
        private readonly IRepositoryWrapperMariaDB _repositoryMariaWrapper;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        private readonly IMeetingScheduleService _meetingScheduleService;

        private readonly KafkaDependentProducer<string, string> _producer;

        public TagService(IRepositoryWrapperMariaDB repositoryMariaWrapper, IConfiguration configuration, IMapper mapper, KafkaDependentProducer<string, string> producer , IMeetingScheduleService meetingScheduleService)
        {
            _repositoryMariaWrapper = repositoryMariaWrapper;
            _configuration = configuration;
            _mapper = mapper;
            _producer = producer;

            _meetingScheduleService = meetingScheduleService;
        }



        public (Tag tag, string message) InsertTag(InsertTagRequest request)
        {
            try
            {
                TicketType ticketType = _repositoryMariaWrapper.Tickets.FindByCondition(x => x.TicketTypeId.Equals(request.TicketTypeId)).FirstOrDefault();
                if (ticketType is null) return (null, "TicketTypeNotFound");

                Tag checkCodeTag = _repositoryMariaWrapper.Tags.FindByCondition(x => x.TagCode.Equals(request.TagCode)).AsNoTracking().FirstOrDefault();
                if (checkCodeTag != null) return (null, "TagCodeExists");

                Tag newTag = _mapper.Map<InsertTagRequest, Tag>(request);


                newTag.TagRepeatString = request.ListRepeatValue is null ? null : JsonConvert.SerializeObject(request.ListRepeatValue);
                newTag.TagTimeStart = request.TagTimeStart is null ?  null  : TimeSpan.Parse(request.TagTimeStart);
                newTag.TagTimeEnd = request.TagTimeEnd is null ?  null : TimeSpan.Parse(request.TagTimeEnd);
                
                _repositoryMariaWrapper.Tags.Add(newTag);
                


                #region Send kafka
                _producer.Produce("ADD_TAG", new Message<string, string>()
                {
                    Key = DateTime.UtcNow.Ticks.ToString(),
                    Value = JsonConvert.SerializeObject(newTag)
                });
                #endregion  End send kafka
                _repositoryMariaWrapper.SaveChanges();

                return (newTag, "CreatedTagSuccess");
            }
            catch (Exception ex)
            {
                return (null, $"Error:Exception:{ex.Message}:{ex.InnerException}");
            }
        }


        public (bool result, string message) DeleteTag(string tagId)
        {
            try
            {
                Tag tag = _repositoryMariaWrapper.Tags.FindByCondition(x => x.TagId.Equals(tagId)).FirstOrDefault();
                if (tag is null) return (false, "TagNotFound");

                _repositoryMariaWrapper.Tags.Remove(tag);


                List<User> users = _repositoryMariaWrapper.Users.FindByCondition(x => x.UserTagIds.Contains(tag.TagId)).ToList();
                foreach(User user in users)
                {
                    user.UserTagIdsParse.Remove(tagId);
                    user.UserTagIds = JsonConvert.SerializeObject(user.UserTagIdsParse);
                    _repositoryMariaWrapper.Users.Update(user);
                    _repositoryMariaWrapper.SaveChanges();
                }
                #region Send kafka
                _producer.Produce("REMOVE_TAG", new Message<string, string>()
                {
                    Key = DateTime.UtcNow.Ticks.ToString(),
                    Value = JsonConvert.SerializeObject(tag)
                });
                #endregion End send kafka
                _repositoryMariaWrapper.SaveChanges();
                return (true, "DeleteTagSuccess");
            }
            catch (Exception ex)
            {
                return (false, $"Error:Exception:{ex.Message}:{ex.InnerException}");
            }

        }


        public (Tag tag, string message) UpdateReponsitory(string tagCode, string repositoryId)
        {
            try
            {
                Tag tag = _repositoryMariaWrapper.Tags.FindByCondition(x => x.TagCode.Equals(tagCode)).FirstOrDefault();
                if (tag is null) return (null, "TagCodeNotFound");

                tag.RepositoryId = repositoryId;
                _repositoryMariaWrapper.SaveChanges();
                return (tag, "UpdatedTagSuccess");
            }
            catch (Exception ex)
            {
                return (null, $"Error:Exception:{ex.Message}:{ex.InnerException}");
            }
        }



        public (Tag tag, string message) UpdateTag(string tagId, UpdateTagRequest request)
        {
            try
            {
                bool sendSchedule = false;

                Tag tag = _repositoryMariaWrapper.Tags.FindByCondition(x => x.TagId.Equals(tagId)).FirstOrDefault();
                if (tag is null) return (null, "TagNotFound");

                if(!tag.TagTimeEnd.HasValue || !tag.TagTimeStart.HasValue || !tag.TagDateEnd.HasValue || !tag.TagDateStart.HasValue)
                {
                    sendSchedule = true;
                }

                tag = _mapper.Map<UpdateTagRequest, Tag>(request, tag);
                tag.TagTimeStart = request.TagTimeStart is not null ? TimeSpan.Parse(request.TagTimeStart) : tag.TagTimeStart;
                tag.TagTimeEnd = request.TagTimeEnd is not null ? TimeSpan.Parse(request.TagTimeEnd) : tag.TagTimeEnd;
                tag.TagRepeatString = request.ListRepeatValue is null ? null : JsonConvert.SerializeObject(request.ListRepeatValue);
                tag.UpdatedAt = DateTime.UtcNow;

                _repositoryMariaWrapper.SaveChanges();


                if (sendSchedule)
                {
                    List<string> userIds = _repositoryMariaWrapper.UserTagModes.FindByCondition(x => x.TagId.Equals(tag.TagId)).Select(x => x.UserId).Distinct().ToList();
                    List<UserMode> userModes = _repositoryMariaWrapper.UserModes.FindByCondition(x => userIds.Contains(x.UserId)).ToList();

                    foreach (var userId in userIds)
                    {
                        List<string> modeIds = userModes.Where(x => x.UserId.Equals(userId)).Select(x => x.ModeId).Distinct().ToList();
                        (object registerMettingScheduleTmp, string messageMettingScheduleTmp) = _meetingScheduleService.RegisterMeetingScheduleByUserId(userId, modeIds);


                        List<UserTagMode> newUserTagModesTmp = _repositoryMariaWrapper.UserTagModes.FindByCondition(x => x.UserId.Equals(userId)).ToList();
                        (object registerScheduleTmp, string messageScheduleTmp) = _meetingScheduleService.RegisterMeetingWithTagMode(new RegisterMeetingWithTagMode
                        {
                            UserId = userId,
                            TagModes = _mapper.Map<List<TagMode>>(newUserTagModesTmp)
                        });
                    }
                  
                   
                }


                #region Send kafka
                _producer.Produce("UPDATE_TAG", new Message<string, string>()
                {
                    Key = DateTime.UtcNow.Ticks.ToString(),
                    Value = JsonConvert.SerializeObject(tag)
                });
                #endregion  End send kafka
               
                return (tag, "UpdatedTagSuccess");
            }
            catch (Exception ex)
            {
                return (null, $"Error:Exception:{ex.Message}:{ex.InnerException}");
            }
        }


        public (Tag tag, string message) ShowById(string tagId)
        {
            Tag tag = _repositoryMariaWrapper.Tags.FindByCondition(x => x.TagId.Equals(tagId)).Include(x=> x.TicketType).FirstOrDefault();
            if (tag is null) return (null, "TagNotFound");
            return (tag, "GetTagSuccess");
        }


        public object ShowAll(PaginationRequest request)
        {
            var queryTags = _repositoryMariaWrapper.Tags.FindByCondition(x =>
                                                                            String.IsNullOrEmpty(request.SearchContent) ||
                                                                            (!String.IsNullOrEmpty(x.TagDescription) && x.TagDescription.ToLower().Contains(request.SearchContent)) ||
                                                                            (!String.IsNullOrEmpty(x.TagName) && x.TagName.ToLower().Contains(request.SearchContent)) ||
                                                                            (!String.IsNullOrEmpty(x.TicketTypeId) && x.TicketTypeId.Contains(request.SearchContent))
                                                                        ).Include(x=> x.TicketType);

            var tags = SortHelper<Tag>.ApplySort(queryTags, request.OrderByQuery);
            var paginationTags = PaginationHelper<Tag>.ToPagedList(tags, request.PageNumber, request.PageSize);

            PaginationResponse<Tag> paginationResponse = new PaginationResponse<Tag>(paginationTags, paginationTags.PageInfo);
            return paginationResponse;
        }


    }
}
