using Microsoft.EntityFrameworkCore;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Groups.Entities;
using Project.Modules.Templates.Entities;
using Project.Modules.Templates.Requests.TemplateShares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Templates.Services
{
    public interface ITemplateShareService
    {
        (List<TemplateShare> data, string message) Store(StoreTemplateShareRequest request);
        (object data, string message) Delete(DeleteTemplateShareRequest request, string templateId);
        (PaginationResponse<Group> data, string message) GetGroupByTemplate(PaginationRequest request, string templateId);
    }

    public class TemplateShareService : ITemplateShareService
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;

        public TemplateShareService(IRepositoryWrapperMariaDB repositoryWrapperMariaDB)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;
        }

        public (List<TemplateShare> data, string message) Store(StoreTemplateShareRequest request)
        {
            Template template = RepositoryWrapperMariaDB.Templates.FirstOrDefault(x => x.TemplateId.Equals(request.TemplateId));
            if (template is null)
            {
                return (null, "TemplateNotFound");
            }

            Group group = RepositoryWrapperMariaDB.Groups.FirstOrDefault(x => request.GroupIds.Contains(x.GroupId));
            if (group is null)
            {
                return (null, "GroupUserNotFound");
            }

            TemplateShare templateShareCheck = RepositoryWrapperMariaDB.TemplateShares.FirstOrDefault(x => x.TemplateId.Equals(request.TemplateId) && request.GroupIds.Contains(x.GroupId));
            if (templateShareCheck != null)
            {
                return (null, "TheTemplateHasAlreadyBennSharedWithThisGroup");
            }

            List<TemplateShare> templateShares = new List<TemplateShare>();

            foreach (var GroupId in request.GroupIds)
            {
                TemplateShare templateShare = new TemplateShare
                {
                    GroupId = GroupId,
                    TemplateId = request.TemplateId,
                    UserCreateTemplate = template.UserId
                };
                RepositoryWrapperMariaDB.TemplateShares.Add(templateShare);
                templateShares.Add(templateShare);
            }

            RepositoryWrapperMariaDB.SaveChanges();
            return (templateShares, "CreateTemplateShareSuccess");
        }

        public (object data, string message) Delete(DeleteTemplateShareRequest request, string templateId)
        {
            List<TemplateShare> templateShares = RepositoryWrapperMariaDB.TemplateShares.FindByCondition(x => x.TemplateId.Equals(templateId) && request.GroupIds.Contains(x.GroupId)).ToList();
            RepositoryWrapperMariaDB.TemplateShares.RemoveRange(templateShares);
            RepositoryWrapperMariaDB.SaveChanges();
            return ("Success", "DeleteTemplateShareSuccess");
        }

        public (PaginationResponse<Group> data, string message) GetGroupByTemplate(PaginationRequest request, string templateId)
        {
            Template template = RepositoryWrapperMariaDB.Templates
                .FindByCondition(x => x.TemplateId.Equals(templateId))
                .Include(x => x.User)
                .FirstOrDefault();

            if (template is null)
            {
                return (null, "TemplateNotFound");
            }

            List<Group> groups = new List<Group>();

            if (template.User != null)
            {
                Group group = RepositoryWrapperMariaDB.Groups.FirstOrDefault(x => x.GroupId.Equals(template.User.GroupId));
                if (group != null)
                {
                    groups.Add(group);
                }
            }

            List<TemplateShare> templateShares = RepositoryWrapperMariaDB.TemplateShares
                .FindByCondition(x => x.TemplateId.Equals(templateId))
                .ToList();

            foreach (TemplateShare templateShare in templateShares)
            {
                Group group = RepositoryWrapperMariaDB.Groups.FirstOrDefault(x => x.GroupId.Equals(templateShare.GroupId));
                if (group != null)
                {
                    if (!groups.Any(x => x.GroupId.Equals(group.GroupId)))
                    {
                        groups.Add(group);
                    }
                }
            }

            //if (groups.Any(x => x is null))
            //{
            //    return (null, "GroupUserNotFound");
            //}

            PaginationHelper<Group> result = PaginationHelper<Group>.ToPagedList(groups.AsQueryable(), request.PageNumber, request.PageSize);
            PaginationResponse<Group> response = new PaginationResponse<Group>(result, result.PageInfo);

            return (response, "Success");
        }
    }
}
