using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Medias.Entities;
using Project.Modules.Medias.Requests;
using Project.Modules.Medias.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Medias.Controllers
{
    [Route("api/mediaType")]
    [ApiController]
    [Authorize]
    //[MiddlewareFilter(typeof(CheckUserExistsMiddlewareAttribute))]
    public class MediaTypeController : BaseController
    {
        private readonly IMediaTypeService MediaTypeService;
        private readonly IMapper Mapper;
        public MediaTypeController(IMediaTypeService mediaTypeService, IMapper mapper)
        {
            MediaTypeService = mediaTypeService;
            Mapper = mapper;
        }
        [HttpGet]
        public IActionResult ShowAll([FromQuery] PaginationRequest request)
        {
            string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
            IQueryable<MediaType> mediaTypes = MediaTypeService.ShowAll();
            PaginationHelper<MediaType>  pagination = PaginationHelper<MediaType>.ToPagedList(mediaTypes, request.PageNumber, request.PageSize);
            PaginationResponse<MediaTypeResponse> response = new PaginationResponse<MediaTypeResponse>
                (pagination.Select(x => new MediaTypeResponse(x, urlRequest)), pagination.PageInfo);

            return ResponseOk(response);
        }

        [HttpPost()]
        public async Task<IActionResult> StoreAsync([FromForm] AddMediaTypeRequest input)
        {
            string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
            MediaType newMediaType = new MediaType
            {
                TypeName = input.TypeName,
                TypeComment = input.TypeCommnet,
            };

            if (input.TypeIcon != null)
            {
                (string iconPath, _) = await GeneralHelper.UploadFileOld(input.TypeIcon, "uploads/icons");
                newMediaType.TypeIcon = GeneralHelper.UrlCombine("uploads/icons", iconPath);
            }
            (MediaType data, string message) = MediaTypeService.Store(newMediaType, urlRequest);

            if (data == null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }

        [HttpPut("{mediaTypeId}")]
        public async Task<IActionResult> UpdateAsync([FromForm] UpdateMediaTypeRequest request, string mediaTypeId)
        {
            MediaType mediaType = MediaTypeService.Show(mediaTypeId);
            if (mediaType is null)
            {
                return ResponseBadRequest("MediaTypeNotFound");
            }

            Mapper.Map(request, mediaType);

            if (request.TypeIcon != null)
            {
                (string iconPath, _) = await GeneralHelper.UploadFileOld(request.TypeIcon, "uploads/icons");
                mediaType.TypeIcon = GeneralHelper.UrlCombine("uploads/icons", iconPath);
            }
            MediaType data = MediaTypeService.Update(mediaType);
            return ResponseOk(data, "UpdateMediaTypeSuccess");
        }

        [HttpDelete("{mediaTypeId}")]
        public IActionResult Delete(string mediaTypeId)
        {
            (bool result, string message) = MediaTypeService.Destroy(mediaTypeId);
            if (!result) return ResponseBadRequest(message);
            return ResponseOk(null, message);
        }

        [HttpGet("{mediaTypeId}")]
        public IActionResult DetailMediaType(string mediaTypeId)
        {
            string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
            MediaType mediaType = MediaTypeService.Show(mediaTypeId);
            if (mediaType is null)
            {
                return ResponseBadRequest("MediaTypeNotFound");
            }

            mediaType.TypeIcon = GeneralHelper.UrlCombine(urlRequest, mediaType.TypeIcon);
            return ResponseOk(mediaType, "ShowDetailMedaiTypeSuccess");
        }
    }
}