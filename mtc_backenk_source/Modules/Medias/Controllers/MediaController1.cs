//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Project.App.Controllers;
//using Project.Modules.Medias.Entities;
//using Project.Modules.Medias.Services;
//using Project.Modules.MediaTypes.Services;
//using System.Collections.Generic;

//namespace Project.Modules.Medias.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [Authorize]
//    //[MiddlewareFilter(typeof(CheckUserExistsMiddlewareAttribute))]
//    public class MediaController1 : BaseController
//    {
//        private readonly IMediaService _mediaService;
//        private readonly IMediaTypeService _mediaTypeService;
//        public MediaController1(IMediaService mediaService, IMediaTypeService mediaTypeService)
//        {
//            _mediaService = mediaService;
//            _mediaTypeService = mediaTypeService;
//        }

//        //[HttpGet()]
//        //public IActionResult ShowAll([FromQuery] RequestTable requestTable)
//        //{
//        //    int userId = User.FindFirst("user_id").Value.toInt();
//        //    string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//        //    (List<Media> medias, string message) = _mediaService.ShowAll(urlRequest, userId);
//        //    #region Paganation
//        //    medias = medias.Where(x =>
//        //        string.IsNullOrEmpty(requestTable.search) ||
//        //        (!string.IsNullOrEmpty(requestTable.search) &&
//        //            (
//        //                x.MediaName.ToLower().Contains(requestTable.search.ToLower()) ||
//        //                x.MediaUrl.ToLower().Contains(requestTable.search.ToLower()) ||
//        //                x.MediaComment.ToLower().Contains(requestTable.search.ToLower())
//        //            )
//        //        )).ToList();

//        //    ResponseTable responseTable = new ResponseTable()
//        //    {
//        //        results = medias.Skip((requestTable.page - 1) * requestTable.results).Take(requestTable.results).ToList(),
//        //        info = new Info()
//        //        {
//        //            page = requestTable.page,
//        //            totalRecord = medias.Count,
//        //            results = requestTable.results
//        //        }
//        //    };
//        //    #endregion
//        //    return Ok(responseTable);
//        //}

//        [HttpGet("no-datatable")]
//        public IActionResult ShowAllNoDataTable()
//        {
//            string userId = User.FindFirst("user_id").Value;
//            string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//            (List<Media> medias, string message) = _mediaService.ShowAll(urlRequest, userId);
//            return ResponseOk(medias);
//        }

//        [HttpGet("{idMedia}")]
//        public IActionResult Show(string idMedia)
//        {
//            string userId = User.FindFirst("user_id").Value;
//            string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//            (Media data, string message) = _mediaService.Show(idMedia, urlRequest, userId);
//            if (data == null) return ResponseBadRequest(message);
//            return ResponseOk(data, message);
//        }

//        //[HttpGet("activated")]
//        //public IActionResult ShowMediaActivated([FromQuery] RequestTable requestTable)
//        //{
//        //    int userId = User.FindFirst("user_id").Value.toInt();
//        //    string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//        //    (List<Media> medias, string message) = _mediaService.ShowMediaActivated(urlRequest, userId);
//        //    #region Paganation
//        //    medias = medias.Where(x =>
//        //        string.IsNullOrEmpty(requestTable.search) ||
//        //        (!string.IsNullOrEmpty(requestTable.search) &&
//        //            (
//        //                x.MediaName.ToLower().Contains(requestTable.search.ToLower()) ||
//        //                x.MediaUrl.ToLower().Contains(requestTable.search.ToLower()) ||
//        //                x.MediaComment.ToLower().Contains(requestTable.search.ToLower())
//        //            )
//        //        )).ToList();

//        //    ResponseTable responseTable = new ResponseTable()
//        //    {
//        //        results = medias.Skip((requestTable.page - 1) * requestTable.results).Take(requestTable.results).ToList(),
//        //        info = new Info()
//        //        {
//        //            page = requestTable.page,
//        //            totalRecord = medias.Count,
//        //            results = requestTable.results
//        //        }
//        //    };
//        //    #endregion
//        //    return Ok(responseTable);
//        //}

//        //[HttpGet("not-activated")]
//        //public IActionResult ShowMediaNotActivated([FromQuery] RequestTable requestTable)
//        //{
//        //    int userId = User.FindFirst("user_id").Value.toInt();
//        //    string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//        //    (List<Media> medias, string message) = _mediaService.ShowMediaNotActivated(urlRequest, userId);
//        //    #region Paganation
//        //    medias = medias.Where(x =>
//        //        string.IsNullOrEmpty(requestTable.search) ||
//        //        (!string.IsNullOrEmpty(requestTable.search) &&
//        //            (
//        //                x.MediaName.ToLower().Contains(requestTable.search.ToLower()) ||
//        //                x.MediaUrl.ToLower().Contains(requestTable.search.ToLower()) ||
//        //                x.MediaComment.ToLower().Contains(requestTable.search.ToLower())
//        //            )
//        //        )).ToList();

//        //    ResponseTable responseTable = new ResponseTable()
//        //    {
//        //        results = medias.Skip((requestTable.page - 1) * requestTable.results).Take(requestTable.results).ToList(),
//        //        info = new Info()
//        //        {
//        //            page = requestTable.page,
//        //            totalRecord = medias.Count,
//        //            results = requestTable.results
//        //        }
//        //    };
//        //    #endregion
//        //    return Ok(responseTable);
//        //}

//        //[HttpPost]
//        //[RequestSizeLimit(long.MaxValue)]
//        //[RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
//        //public async Task<IActionResult> StoreAsync([FromForm] StoreValidation input)
//        //{
//        //    string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//        //    string userId = User.FindFirst("user_id").Value;

//        //    Media newMedia = new Media()
//        //    {
//        //        MediaName = input.mediaName,
//        //        TypeId = input.mediaType,
//        //        MediaComment = input.mediaComment,
//        //        UserId = userId
//        //    };
//        //    if (input.mediaUrl != null)
//        //    {
//        //        (string videoPath, string typeFile) = await GeneralHelper.UploadFile(input.mediaUrl, "uploads/medias");
//        //        newMedia.MediaUrl = GeneralHelper.UrlCombine("uploads/medias", videoPath);
//        //        newMedia.MediaTypeCode = typeFile;

//        //    }
//        //    (Media media, string message) = _mediaService.Store(newMedia, urlRequest, userId);
//        //    media.MediaUrl = null;
//        //    return ResponseOk(media, message);
//        //}


//        //[HttpPut("{idMedia}")]
//        //[RequestSizeLimit(long.MaxValue)]
//        //[RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
//        //public async Task<IActionResult> UpdateAsync([FromForm] UpdateValidation input, string idMedia)
//        //{
//        //    string userId = User.FindFirst("user_id").Value;
//        //    string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//        //    Media newMedia = new Media()
//        //    {
//        //        MediaName = input.mediaName,
//        //        MediaComment = input.mediaComment,
//        //        MediaStatus = input.mediaStatus,
//        //        UpdatedAt = DateTime.Now
//        //    };
//        //    if (input.mediaUrl != null)
//        //    {
//        //        (string videoPath, string typeFile) = await GeneralHelper.UploadFile(input.mediaUrl, "uploads/medias");
//        //        newMedia.MediaUrl = GeneralHelper.UrlCombine("uploads/medias", videoPath);
//        //        newMedia.MediaTypeCode = typeFile;
//        //    }
//        //    (Media data, string message) = _mediaService.Update(idMedia, newMedia, urlRequest, userId);
//        //    if (data == null) return ResponseBadRequest(message);
//        //    return ResponseOk(data, message);
//        //}


//        [HttpDelete("{idMedia}")]
//        public IActionResult DeleteAsync(string idMedia)
//        {
//            string userId = User.FindFirst("user_id").Value;
//            string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//            (bool result, string message) = _mediaService.Destroy(idMedia, urlRequest, userId);
//            if (!result) return ResponseBadRequest(message);
//            return ResponseOk(result, message);
//        }



//        //[HttpGet("by-type/{idMediaType}")]
//        //public IActionResult ShowMediaByType([FromQuery] RequestTable requestTable, int idMediaType)
//        //{
//        //    int userId = User.FindFirst("user_id").Value.toInt();
//        //    string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//        //    (MediaType mediaType, string messageMediaType) = _mediaTypeService.Show(idMediaType, urlRequest);
//        //    if (mediaType == null) return ResponseBadRequest(messageMediaType);

//        //    (List<Media> medias, string message) = _mediaService.ShowMediaByTypeMedia(idMediaType, urlRequest, userId);
//        //    #region Paganation
//        //    medias = medias.Where(x =>
//        //        string.IsNullOrEmpty(requestTable.search) ||
//        //        (!string.IsNullOrEmpty(requestTable.search) &&
//        //            (
//        //                x.MediaName.ToLower().Contains(requestTable.search.ToLower()) ||
//        //                x.MediaUrl.ToLower().Contains(requestTable.search.ToLower()) ||
//        //                x.MediaComment.ToLower().Contains(requestTable.search.ToLower())
//        //            )
//        //        )).ToList();

//        //    ResponseTable responseTable = new ResponseTable()
//        //    {
//        //        results = medias.Skip((requestTable.page - 1) * requestTable.results).Take(requestTable.results).ToList(),
//        //        info = new Info()
//        //        {
//        //            page = requestTable.page,
//        //            totalRecord = medias.Count,
//        //            results = requestTable.results
//        //        }
//        //    };
//        //    #endregion
//        //    return Ok(responseTable);
//        //}


//        //[HttpPost("search")]
//        //public IActionResult Search([FromBody] SearchRequest data)
//        //{
//        //    int userId = User.FindFirst("user_id").Value.toInt();
//        //    string urlRequest = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value;
//        //    (List<Media> medias, string message) = _mediaService.Search(data, urlRequest, userId);

//        //    PaginationResponse pagination = new PaginationResponse();
//        //    pagination.perPage = data.limit;
//        //    pagination.page = data.index;
//        //    pagination.total = medias.Count;
//        //    pagination.data = medias.Skip((int)pagination.page).Take((int)pagination.perPage).ToArray();
//        //    return Ok(pagination);
//        //}
//    }
//}