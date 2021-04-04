using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Project.App.Databases;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Medias.Entities;
using Project.Modules.TemplateDetails.Entities;
using Project.Modules.Templates.Entities;
using Project.Modules.Templates.Requests;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Services
{
    public interface ITemplateServices
    {
        (Template template, string message) Store(StoreTemplateRequest request, string UserID);
        (Template template, string message) FindID(string templateId, string userId, string url);
        (PaginationResponse<Template> listTemplate, string message) Show(PaginationRequest request, string userId, string url);
        (bool result, string message) Delete(List<string> templateIds);
        (Template template, string message) Update(UpdateTemplate updateTemplate, string TemplateID);
        (Template template, string message) Update(Template newTemplate, string templateId);
        (object data, string message) StoreTemplateDefault(StoreTemplateDefaultRequest request);
    }
    public class TemplateServices : ITemplateServices
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        private readonly IMapper Mapper;
        public TemplateServices(IRepositoryWrapperMariaDB repositoryWrapperMaria, IMapper mapper)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMaria;
            Mapper = mapper;
        }
        public (Template template, string message) Store(StoreTemplateRequest request, string userId)
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
            RepositoryWrapperMariaDB.Templates.Add(template);
            RepositoryWrapperMariaDB.SaveChanges();
            return (template, "CreateTemplateSuccess");
        }
        public (Template template, string message) FindID(string templateId, string userId, string url)
        {
            string GroupID = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId))?.GroupId;

            List<string> templateIds = RepositoryWrapperMariaDB.TemplateShares
                .FindByCondition(x => x.GroupId.Equals(GroupID) || x.UserCreateTemplate.Equals(userId))
                .Select(x => x.TemplateId)
                .ToList();

            //var _template = RepositoryWrapperMariaDB.Templates
            //    .FindByCondition(x => x.TemplateId == templateId && templateIds.Contains(x.TemplateId))
            //    .Include(x => x.User).Include(x => x.TemplateDetails)
            //    .ThenInclude(x => x.Media)
            //    .FirstOrDefault();

            var _template = RepositoryWrapperMariaDB.Templates
                .FindByCondition(x => x.TemplateId == templateId && (templateIds.Contains(x.TemplateId) || x.UserId.Equals(userId)))
                .Include(x => x.User).Include(x => x.TemplateDetails)
                .ThenInclude(x => x.Media)
                .FirstOrDefault();


            if (_template is null)
            {
                return (null, "TemplateNotFound");
            }

            foreach (var templateDetail in _template.TemplateDetails)
            {
                templateDetail.Media.CombineMediaUrl(url);
            }

            return (_template, "Success");
        }
        public (PaginationResponse<Template> listTemplate, string message) Show(PaginationRequest request, string userId, string url)
        {
            string groupId = RepositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId))?.GroupId;

            List<string> templateIds = RepositoryWrapperMariaDB.TemplateShares
                .FindByCondition(x => x.GroupId.Equals(groupId) || x.UserCreateTemplate.Equals(userId))
                .Select(x => x.TemplateId)
                .ToList();

            //var templates = RepositoryWrapperMariaDB.Templates
            //    .FindByCondition(x => templateIds.Contains(x.TemplateId))
            //    .Include(x => x.User)
            //    .Include(x => x.TemplateDetails)
            //    .ThenInclude(x => x.Media)
            //    .OrderByDescending(x => x.TemplateCreatedAt)
            //    .ToList();


            var templates = RepositoryWrapperMariaDB.Templates
                .FindByCondition(x => (templateIds.Contains(x.TemplateId) || x.UserId.Equals(userId))
                 &&
                (string.IsNullOrEmpty(request.SearchContent) || x.TemplateName.ToLower().Contains(request.SearchContent.ToLower())))
                .Include(x => x.User)
                .Include(x => x.TemplateDetails)
                .ThenInclude(x => x.Media)
                .OrderByDescending(x => x.TemplateCreatedAt)
                .ToList();

            foreach (var template in templates)
            {
                foreach (var templateDetail in template.TemplateDetails)
                {
                    templateDetail.Media.CombineMediaUrl(url);
                }
            }

            var temp = templates.AsQueryable();
            temp = SortHelper<Template>.ApplySort(temp, request.OrderByQuery);
            PaginationHelper<Template> result = PaginationHelper<Template>.ToPagedList(temp, request.PageNumber, request.PageSize);
            PaginationResponse<Template> response = new PaginationResponse<Template>(result, result.PageInfo);
            return (response, "Success");
        }
        public (bool result, string message) Delete(List<string> templateIds)
        {
            List<Template> templates = RepositoryWrapperMariaDB.Templates.FindByCondition(x => templateIds.Contains(x.TemplateId)).ToList();
            RepositoryWrapperMariaDB.Templates.RemoveRange(templates);
            RepositoryWrapperMariaDB.SaveChanges();
            return (true, "DeleteTemplateSuccess");
        }
        public (Template template, string message) Update(UpdateTemplate request, string templateId)
        {
            Template template = RepositoryWrapperMariaDB.Templates.FindByCondition(x => x.TemplateId == templateId).FirstOrDefault();

            if (template is null)
            {
                return (null, "TemplateNotFound");
            }

            try
            {
                Mapper.Map(request, template);
                template.TemplateDuration = request.TemplateDuration;
                RepositoryWrapperMariaDB.SaveChanges();
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }

            return (template, "UpdateTemplateSuccess");
        }

        public (Template template, string message) Update(Template newTemplate, string templateId)
        {
            Template template = RepositoryWrapperMariaDB.Templates.FirstOrDefault(x => x.TemplateId == templateId);
            if (template is null)
            {
                return (null, "TemplateNotFound");
            }

            try
            {
                Mapper.Map(newTemplate, template);
                RepositoryWrapperMariaDB.SaveChanges();
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }
            return (template, "UpdateTemplateSuccess");
        }

        public (object data, string message) StoreTemplateDefault(StoreTemplateDefaultRequest request)
        {
            List<Template> templateDefaults = RepositoryWrapperMariaDB.Templates.FindByCondition(x => x.TemplateDefault == TemplateDefault.Default).ToList();

            if (templateDefaults.Count > 0)
            {
                List<string> templateIds = templateDefaults.Select(x => x.TemplateId).ToList();

                TemplateDetail checkTemplateDefault = RepositoryWrapperMariaDB.TemplateDetails.FirstOrDefault(x => x.MediaId.Equals(request.MediaId) && templateIds.Contains(x.TemplateId));
                if(checkTemplateDefault != null)
                {
                    return ("Success", "CreateTemplateDefaultSuccess");
                }
            }

            Template template = new Template();
            RepositoryWrapperMariaDB.Templates.Add(template);
            template.TemplateName = $"Default_{template.TemplateId}";
            template.TemplateRatioX = 0;
            template.TemplateRatioY = 0;
            template.TemplateDefault = TemplateDefault.Default;
            template.UserId = request.UserId;

            TemplateDetail templateDetail = new TemplateDetail
            {
                MediaId = request.MediaId,
                TemplateId = template.TemplateId,
                TempPointHeight = 0,
                TempPointWidth = 0,
                TempRatioX = 0,
                TempRatioY = 0,
                Zindex = 0
            };
            RepositoryWrapperMariaDB.TemplateDetails.Add(templateDetail);

            RepositoryWrapperMariaDB.SaveChanges();

            return ("Success", "CreateTemplateDefaultSuccess");
        }
    }
}
