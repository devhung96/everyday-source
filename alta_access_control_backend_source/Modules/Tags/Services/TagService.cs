using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.RegisterDetects.Entities;
using Project.Modules.Tags.Entities;
using Project.Modules.Tags.Requests;
using Project.Modules.Tickets.Entities;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using Repository = SurveillanceManagement.Repository;
using SurveillanceManagementServiceClient = SurveillanceManagement.SurveillanceManagement.SurveillanceManagementClient;
using System.Threading.Tasks;
using RepositoriesManagement;
using SurveillanceManagement;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Project.Modules.Tags.Services
{
    public interface ITagService
    {
        (Tag tag, string message) Store(AddTagRequest request, bool IsCrm = false);
        (object data, string message) Update(UpdateTagRequest request, string tagId);

        (Tag tag, string message) DeleteById(string tagId);
        (Tag tag, string message) DeleteByCode(string tagCode);


        (Tag tag, string message) Detail(string TagId);
        (Tag tag, string message) DetailCode(string TagCode);


        PaginationResponse<Tag> ShowTable(PaginationRequest request);
        PaginationResponse<RegisterDetectDetail> UserTicketInTagCode(UserTicketRequest request);

        (object data, string message) UpdateTagByCode(UpdateTagRequest request);

    }
    public class TagService : ITagService
    {
        private readonly IRepositoryWrapperMariaDB repository;
        private readonly IMapper mapper;
        private readonly IConfiguration configuaration;
        private readonly IServiceScopeFactory serviceScopeFactory;

        private readonly RepositoryManagement.RepositoryManagementClient repositoryManagement;
        private readonly SurveillanceManagementServiceClient surcveillanceServiceClient;


        public TagService(IRepositoryWrapperMariaDB reposirory, IMapper mapper, IServiceScopeFactory serviceScopeFactory, RepositoryManagement.RepositoryManagementClient repositoryManagement, SurveillanceManagementServiceClient surcveillanceServiceClient, IConfiguration configuaration)
        {
            this.repository = reposirory;
            this.configuaration = configuaration;
            this.mapper = mapper;
            this.surcveillanceServiceClient = surcveillanceServiceClient;
            this.repositoryManagement = repositoryManagement;
            this.serviceScopeFactory = serviceScopeFactory;
        }


        #region Tag delete
        public (Tag tag, string message) DeleteById(string tagId)
        {
            using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            IRepositoryWrapperMariaDB repositoryNew = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

            Tag tag = repositoryNew.Tags.GetById(tagId);
            if (tag is null) return (null, "TagNotExist");

            repositoryNew.Tags.Remove(tag);
            repositoryNew.SaveChanges();

            return (tag, "DeleteSuccess");
        }

        public (Tag tag, string message) DeleteByCode(string tagCode)
        {
            using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            IRepositoryWrapperMariaDB repositoryNew = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

            Tag tag = repositoryNew.Tags.FindByCondition(x => x.TagCode.Equals(tagCode)).FirstOrDefault();
            if (tag is null) return (null, "TagNotExist");

            List<RegisterDetect> registerDetects = repository.RegisterDetects
                                                      .FindByCondition(x => x.TagCode.Equals(tag.TagCode) && x.TicketTypeId.Equals(tag.TicketTypeId))
                                                      .ToList();
            if (registerDetects.Count() > 0)
            {
                repository.RegisterDetects.RemoveRange(registerDetects);
                repository.SaveChanges();
            }


            repositoryNew.Tags.Remove(tag);
            repositoryNew.SaveChanges();

            return (tag, "DeleteSuccess");
        }

        #endregion End DeleteTag



        #region Create - Update


        public (Tag tag, string message) Store(AddTagRequest request, bool IsCrm = false)
        {
            using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            IRepositoryWrapperMariaDB repositoryNew = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

            Tag tag = repositoryNew.Tags.FindByCondition(x => x.TagCode.Equals(request.TagCode)).FirstOrDefault();
            if (tag != null) return (null, "TagAlreadyExist");


            if (!string.IsNullOrEmpty(request.TicketTypeId))
            {
                TicketType ticketType = repositoryNew.TicketTypes.FindByCondition(x => x.TicketTypeId.Equals(request.TicketTypeId)).FirstOrDefault();
                if (ticketType is null) return (null, "TicketTypeIdNotExist");
            }


            Tag newTag = mapper.Map<AddTagRequest, Tag>(request);
            if (!string.IsNullOrEmpty(request.TagTimeStart) && !string.IsNullOrEmpty(request.TagTimeEnd))
            {
                try
                {
                    TimeSpan timeStart = TimeSpan.Parse(request.TagTimeStart);
                    TimeSpan timeStop = TimeSpan.Parse(request.TagTimeEnd);
                    newTag.TagTimeStart = timeStart;
                    newTag.TagTimeEnd = timeStop;
                }
                catch
                {
                    return (null, "TimeInValid");
                }

            }
            newTag.TagType = IsCrm ? Tag.TAG_TYPE.CRM : Tag.TAG_TYPE.AccessControll;
            newTag.TagRepeatValue = newTag.ListRepeatValue is null ? null : JsonConvert.SerializeObject(newTag.ListRepeatValue);
            try
            {
                #region GppcRepository
                var repositoryRequest = new RepositoryCreateRequest
                {
                    Name = newTag.TagCode,
                    Type = "VIP"
                };
                RepositoriesManagement.Repository repositoryResponse = repositoryManagement.create(repositoryRequest);
                newTag.RepositoryId = repositoryResponse.Id;
                #endregion

                #region GrpcSurveillance

                Surveillance surveillance = surcveillanceServiceClient.get(new Common.String { Value = configuaration["OutsideSystems:FaceSettings:SurveillanceId"] });
                surveillance.Repositories.Add(new Repository
                {
                    Id = repositoryResponse.Id
                });

                try
                {
                    surveillance = surcveillanceServiceClient.update(surveillance);
                }
                catch (Exception exSurveillance)
                {
                    repositoryManagement.delete(repositoryResponse);
                    return (null, exSurveillance.Message.ToString());
                }
                #endregion
            }
            catch (Exception exRepository)
            {
                return (null, exRepository.Message.ToString());
            }

            repositoryNew.Tags.Add(newTag);
            repositoryNew.SaveChanges();
            return (newTag, "AddSuccess");
        }
        public (object data, string message) Update(UpdateTagRequest request, string tagId)
        {
            using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            IRepositoryWrapperMariaDB repositoryNew = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

            Tag tag = repositoryNew.Tags.GetById(tagId);
            if (tag is null) return (null, "TagNotFound");

            request.TagStatus = request.TagStatus.HasValue ? request.TagStatus : tag.TagStatus;
            tag = mapper.Map<UpdateTagRequest, Tag>(request, tag);

            repositoryNew.SaveChanges();


            return (tag, "UpdatedTagSuccess");
        }

        public (object data, string message) UpdateTagByCode(UpdateTagRequest request)
        {
            using IServiceScope serviceScope = serviceScopeFactory.CreateScope();
            IRepositoryWrapperMariaDB repositoryNew = serviceScope.ServiceProvider.GetRequiredService<IRepositoryWrapperMariaDB>();

            Tag tag = repositoryNew.Tags.FindByCondition(x => x.TagCode.Equals(request.TagCode)).FirstOrDefault();
            if (tag is null) return (null, "TagNotFound");

            request.TagStatus = request.TagStatus.HasValue ? request.TagStatus : tag.TagStatus;

            List<RegisterDetect> registerDetects = repository.RegisterDetects
                                                           .FindByCondition(x => x.TagCode.Equals(request.TagCode) && x.TicketTypeId.Equals(request.TicketTypeId))
                                                           .ToList();

            if (!string.IsNullOrEmpty(request.TicketTypeId) && !request.TicketTypeId.Equals(tag.TicketTypeId))
            {

                foreach (RegisterDetect detect in registerDetects)
                {
                    detect.TicketTypeId = request.TicketTypeId;
                }
                repository.RegisterDetects.UpdateRange(registerDetects);
            }
            if (!string.IsNullOrEmpty(request.TagTimeStart) && !string.IsNullOrEmpty(request.TagTimeEnd)
                && request.TagDateEnd.HasValue && request.TagDateStart.HasValue)
            {
                List<string> idDetects = registerDetects.Select(x => x.RegisterDetectId).ToList();
                List<RegisterDetectDetail> detectDetails = repository.RegisterDetectDetails.FindByCondition(x => idDetects.Contains(x.RegisterDetectId)).ToList();

                foreach (RegisterDetectDetail detail in detectDetails)
                {
                    detail.RgDectectDetailDateBegin = request.TagDateStart.Value;
                    detail.RgDectectDetailDateEnd = request.TagDateEnd.Value;
                    detail.RgDectectDetailRepeat = request.TagRepeat.Value;
                    try
                    {
                        detail.RgDectectDetailTimeBegin = TimeSpan.Parse(request.TagTimeStart);
                        detail.RgDectectDetailTimeEnd = TimeSpan.Parse(request.TagTimeEnd);
                    }
                    catch
                    {
                        continue;
                    };
                    detail.RgDectectDetailRepeatValue = request.TagRepeatValue;
                }
                repository.RegisterDetectDetails.UpdateRange(detectDetails);
                repository.SaveChanges();

            }
            else
            {
                // Trường hợp cập nhật giá trị thời của tag về null đi xóa data registerDetect
                if (registerDetects.Count > 0)
                {
                    repository.RegisterDetects.AddRange(registerDetects);
                    repository.SaveChanges();
                }

            }
            tag = mapper.Map<UpdateTagRequest, Tag>(request, tag);
            tag.TagRepeatValue = request.ListRepeatValue is null ? "" : JsonConvert.SerializeObject(request.ListRepeatValue);

            repositoryNew.SaveChanges();
            return (tag, "UpdatedTagSuccess");
        }

        #endregion


        #region show
        public (Tag tag, string message) Detail(string TagId)
        {
            Tag tag = repository.Tags.GetById(TagId);

            if (tag is null)
            {
                return (null, "TagNotExist");
            }
            return (tag, "Success");
        }

        public (Tag tag, string message) DetailCode(string TagCode)
        {
            Tag tag = repository.Tags.FindByCondition(x => x.TagCode.Equals(TagCode)).FirstOrDefault();

            if (tag is null)
            {
                return (null, "TagNotExist");
            }
            return (tag, "Success");
        }

        public PaginationResponse<Tag> ShowTable(PaginationRequest request)
        {
            var data = repository.Tags.FindByCondition(x => string.IsNullOrEmpty(request.SearchContent)
                                                        || x.TagName.ToLower().Contains(request.SearchContent.ToLower())
                                                        || x.TagDescription.ToLower().Contains(request.SearchContent.ToLower())
                );
            //.Select(x => new Tag(x));

            var result = PaginationHelper<Tag>.ToPagedList(data, request.PageNumber, request.PageSize);
            data = SortHelper<Tag>.ApplySort(data, request.OrderByQuery);
            PaginationResponse<Tag> response = new PaginationResponse<Tag>(data, result.PageInfo);
            return response;
        }

        public PaginationResponse<RegisterDetectDetail> UserTicketInTagCode(UserTicketRequest request)
        {
            Tag tag = repository.Tags.FindByCondition(x => x.TagCode.Equals(request.TagCode)).FirstOrDefault();
            if (tag is null)
            {
                return null;
            }
            List<string> registerDetectIds = repository.RegisterDetects.FindByCondition(x => x.TagCode.Equals(request.TagCode)).Select(x => x.RegisterDetectId).ToList();

            IQueryable<RegisterDetectDetail> registerDetectDetails = repository.RegisterDetectDetails.FindByCondition(x => registerDetectIds.Contains(x.RegisterDetectId)).Include(x => x.RegisterDetect);
            registerDetectDetails = registerDetectDetails.Where(x => string.IsNullOrEmpty(request.SearchContent) || (x.RegisterDetect.UserName != null && x.RegisterDetect.UserName.Contains(request.SearchContent)));
            var result = PaginationHelper<RegisterDetectDetail>.ToPagedList(registerDetectDetails, request.PageNumber, request.PageSize);
            registerDetectDetails = SortHelper<RegisterDetectDetail>.ApplySort(registerDetectDetails, request.OrderByQuery);
            PaginationResponse<RegisterDetectDetail> response = new PaginationResponse<RegisterDetectDetail>(registerDetectDetails, result.PageInfo);
            return response;
        }
        #endregion

    }
}
