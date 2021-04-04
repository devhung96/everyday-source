using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Project.App.Databases;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.Modules.Medias.Entities;
using Project.Modules.TemplateDetails.Entities;
using Project.Modules.TemplateDetails.Requests;
using Project.Modules.Templates.Entities;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.TemplateDetails.Services
{
    public interface ITemplateDetailServices
    {
        (TemplateDetail templateDetail, string message) Store(StoreTemplateDetail storeTemplateDetail);
        (TemplateDetail templateDetail, string message) Update(UpdateTemplatedetail updateTemplatedetail, string templateDetailId);
        (bool check, string message) Delete(List<string> templateDetailIds);
        (PaginationResponse<TemplateDetail> templateDetails, string message) ShowByTemplateId(PaginationRequest request, string url);
        (TemplateDetail templateDetail, string message) Show(string templateDetailId, string url);
        (List<TemplateDetail> templateDetail, string message) UpdateRange(UpdateTemplateDetailVer2 updateTemplatedetail, string templateId);
        (object result, string message) StoreTemplateAndTemplateDetail(StoreTemplate storeTemplate, string userId);
    }
    public class TemplateDetailServices : ITemplateDetailServices
    {
        private readonly IRepositoryWrapperMariaDB RepositoryWrapperMariaDB;
        private readonly IMapper Mapper;
        public TemplateDetailServices(IRepositoryWrapperMariaDB repositoryWrapperMariaDB, IMapper mapper)
        {
            RepositoryWrapperMariaDB = repositoryWrapperMariaDB;
            Mapper = mapper;
        }
        public (TemplateDetail templateDetail, string message) Store(StoreTemplateDetail storeTemplateDetail)
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
            RepositoryWrapperMariaDB.TemplateDetails.Add(templateDetail);
            RepositoryWrapperMariaDB.SaveChanges();
            return (templateDetail, "CreateTemplateDetailSuccess");
        }
        public (TemplateDetail templateDetail, string message) Update(UpdateTemplatedetail updateTemplatedetail, string templateDetailId)
        {
            TemplateDetail templateDetail = RepositoryWrapperMariaDB.TemplateDetails.FirstOrDefault(x => x.TemplateDetailId == templateDetailId);

            if (templateDetail is null)
            {
                return (null, "TemplateDetailNotFound");
            }

            try
            {
                Mapper.Map(updateTemplatedetail, templateDetail);
                RepositoryWrapperMariaDB.SaveChanges();
            }
            catch (Exception ex)
            {
                return (null, ex.Message);
            }

            return (templateDetail, "UpdateTemplateDetailSuccess");
        }

        public (List<TemplateDetail> templateDetail, string message) UpdateRange(UpdateTemplateDetailVer2 updateTemplatedetail, string templateId)
        {
            Template template = RepositoryWrapperMariaDB.Templates.FirstOrDefault(x => x.TemplateId.Equals(templateId));

            if (template is null)
            {
                return (null, "TemplateNotFound");
            }

            template.TemplateName = updateTemplatedetail.TemplateName;
            template.TemplateRatioX = updateTemplatedetail.TemplateRatioX;
            template.TemplateRatioY = updateTemplatedetail.TemplateRatioY;
            template.TemplateRotate = updateTemplatedetail.TemplateRotate;

            var listError = "";
            List<TemplateDetail> listResult = new List<TemplateDetail>();
            List<TemplateDetail> templateDetails = RepositoryWrapperMariaDB.TemplateDetails.FindByCondition(x => x.TemplateId == templateId).ToList();
            RepositoryWrapperMariaDB.TemplateDetails.RemoveRange(templateDetails);

            foreach (var item in updateTemplatedetail.ListTemplateDetails)
            {
                TemplateDetail tmpTemplateDetail = new TemplateDetail
                {
                    TemplateId = templateId,
                    TempPointHeight = item.TempPointHeight,
                    TempPointWidth = item.TempPointWidth,
                    TempRatioX = item.TempRatioX,
                    TempRatioY = item.TempRatioY,
                    Zindex = item.Zindex,
                    MediaId = item.MediaId
                };
                RepositoryWrapperMariaDB.TemplateDetails.Add(tmpTemplateDetail);
                listResult.Add(tmpTemplateDetail);
            }
            RepositoryWrapperMariaDB.SaveChanges();
            return (listResult, listError);
        }

        public (bool check, string message) Delete(List<string> templateDetailIds)
        {
            List<TemplateDetail> templateDetails = RepositoryWrapperMariaDB.TemplateDetails.FindByCondition(x => templateDetailIds.Contains(x.TemplateDetailId)).ToList();
            RepositoryWrapperMariaDB.TemplateDetails.RemoveRange(templateDetails);
            RepositoryWrapperMariaDB.SaveChanges();
            return (true, "DeleteTemplateDetailSuccess");
        }

        public (PaginationResponse<TemplateDetail> templateDetails, string message) ShowByTemplateId(PaginationRequest request, string url)
        {
            var listTemplateDetails = RepositoryWrapperMariaDB.TemplateDetails
                .FindAll()
                .Include(x => x.Template)
                .Include(x => x.Media)
                .AsQueryable();

            foreach (var listTemplateDetail in listTemplateDetails)
            {
                listTemplateDetail.Media.CombineMediaUrl(url);
            }

            listTemplateDetails = SortHelper<TemplateDetail>.ApplySort(listTemplateDetails, request.OrderByQuery);
            PaginationHelper<TemplateDetail> result = PaginationHelper<TemplateDetail>.ToPagedList(listTemplateDetails, request.PageNumber, request.PageSize);
            PaginationResponse<TemplateDetail> response = new PaginationResponse<TemplateDetail>(result, result.PageInfo);

            return (response, "Success");
        }
        public (TemplateDetail templateDetail, string message) Show(string templateDetailId, string url)
        {
            TemplateDetail templateDetail = RepositoryWrapperMariaDB.TemplateDetails.FindAll()
                .Include(x => x.Template)
                .Include(x => x.Media)
                .FirstOrDefault(x => x.TemplateDetailId == templateDetailId);
            if (templateDetail == null) return (null, "TemplateDetailNotFound");
            templateDetail.Media.CombineMediaUrl(url);
            return (templateDetail, "Success");
        }

        public (object result, string message) StoreTemplateAndTemplateDetail(StoreTemplate storeTemplate, string userId)
        {
            List<TemplateDetail> result = new List<TemplateDetail>();

            Template template = new Template()
            {
                TemplateName = storeTemplate.TemplateName,
                TemplateRatioX = storeTemplate.templateRatioX,
                TemplateRatioY = storeTemplate.templateRatioY,
                TemplateRotate = storeTemplate.templateRotate,
                UserId = userId
            };
            RepositoryWrapperMariaDB.Templates.Add(template);
            RepositoryWrapperMariaDB.SaveChanges();

            var TemplateDetails = storeTemplate.templateDetails;
            foreach (var item in TemplateDetails)
            {
                TemplateDetail templateDetail = new TemplateDetail()
                {
                    Zindex = item.Zindex,
                    MediaId = item.MediaId,
                    TemplateId = template.TemplateId,
                    TempRatioX = item.TempRatioX,
                    TempRatioY = item.TempRatioY,
                    TempPointWidth = item.TempPointWidth,
                    TempPointHeight = item.TempPointHeight
                };
                RepositoryWrapperMariaDB.TemplateDetails.Add(templateDetail);
                RepositoryWrapperMariaDB.SaveChanges();
                result.Add(templateDetail);
            }

            return (result, "Success");
        }
    }
}
