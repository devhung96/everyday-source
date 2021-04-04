using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Project.App.Controllers;
using Project.App.Requests;
using Project.Modules.Organizes.Entities;
using Project.Modules.Organizes.Requests;
using Project.Modules.Organizes.Services;
using System.Linq.Dynamic.Core;
namespace Project.Modules.organizes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizeController : BaseController
    {
        private readonly IOrganizeService _Organizeservice;

        private readonly IConfiguration _configuration;
        private readonly int isHttps = 0;
        private readonly IMapper _mapper;
        private readonly string _urlMedia = "";


        public OrganizeController(IOrganizeService Organizeservice, IConfiguration configuration, IMapper mapper)
        {
            _Organizeservice = Organizeservice;
            _configuration = configuration;
            isHttps = _configuration["IsHttps"].toHttps();
            _mapper = mapper;
            _urlMedia = _configuration["MediaService:MediaUrl"];
        }

        [Authorize(Roles = "SUPER")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateOrganizeRequest request)
        {
            string userId = User.FindFirst("UserId").Value.ToString();

            request.OrganizeLogoUrl = request.OrganizeLogoUrl.GetLocalPathUrl();
            Organize newOrganize = _mapper.Map<Organize>(request);
            newOrganize.UserId = userId;
            (Organize data, string message, string code) = _Organizeservice.Create(newOrganize);
            if (data is null) return ResponseBadRequest(message, code);
            return ResponseOk(data, message, code);
        }

        [Authorize(Roles = "SUPER,ADMIN,CLIENT")]
        [HttpPut("{idOrganize}")]
        public IActionResult Update(string idOrganize, [FromBody] UpdateOrganizeRequest request)
        {
            request.OrganizeLogoUrl = request.OrganizeLogoUrl.GetLocalPathUrl();
            Organize newOrganize = _mapper.Map<Organize>(request);
            (Organize data, string message, string code) = _Organizeservice.Update(idOrganize, newOrganize);
            if (data is null) return ResponseBadRequest(message, code);
            return ResponseOk(data, message, code);
        }

        [Authorize(Roles = "SUPER")]
        [HttpDelete("{idOrganize}")]
        public IActionResult DeleteAsync(string idOrganize)
        {
            (bool result, string message, string code) = _Organizeservice.Delete(idOrganize);
            if (!result) return ResponseBadRequest(message, code);
            return ResponseOk(result, message, code); 
        }

        [Authorize(Roles = "SUPER,CLIENT,ADMIN")]
        [HttpGet("{idOrganize}")]
        public IActionResult Show(string idOrganize)
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (Organize data, string message,string code) = _Organizeservice.Show(idOrganize, urlRequest);
            if (data is null) return ResponseBadRequest(message, code);
            return ResponseOk(data, message, code);
        }

        [Authorize(Roles = "SUPER")]
        [HttpPost("show-all")]
        public IActionResult ShowAll([FromBody] RequestTable requestTable) 
        {
            string urlRequest = HttpContext.GetBaseURL(isHttps);
            (List<Organize> data, string message, string code) = _Organizeservice.ShowAll(urlRequest);

            #region Paganation
            data = data.Where(x =>
                string.IsNullOrEmpty(requestTable.Search) ||
                (!string.IsNullOrEmpty(requestTable.Search) &&
                    (
                        x.OrganizeName.ToLower().Contains(requestTable.Search.ToLower()) ||
                        x.OrganizeIntroduce.ToLower().Contains(requestTable.Search.ToLower()) ||
                        x.UserAdmin.ToLower().Contains(requestTable.Search.ToLower()) ||
                        x.UserId.ToLower().Contains(requestTable.Search.ToLower())
                    )
                )).AsQueryable().OrderBy(OrderValue(requestTable.SortField, requestTable.SortOrder)).ToList();

            ResponseTable responseTable = new ResponseTable()
            {
                DateResult = data.Skip((requestTable.Page - 1) * requestTable.Results).Take(requestTable.Results).ToList(),
                Info = new Info()
                {
                    Page = requestTable.Page,
                    TotalRecord = data.Count,
                    Results = requestTable.Results
                }
            };
            #endregion
            return Ok(responseTable);

        }





        public string OrderValue(string sortColumn, string SortDir)
        {
            return sortColumn + " " + SortDir;
        }





    }
}