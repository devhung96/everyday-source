using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Controllers;
using Project.App.Helpers;
using Project.Modules.Faces.FaceServices;
using Project.Modules.Kafka.Producer;
using Project.Modules.Medias.Services;
using Project.Modules.Schedules.Requests;
using Project.Modules.Schedules.Services;
using Project.Modules.Tags.Enities;
using Project.Modules.Tags.Services;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.Service;
using Project.Modules.Users.UserKafka;
using Project.Modules.UserTagModes.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMediaService _mediaService;
        private readonly IEmployeeService _employeeService;
        private readonly string BucketFaceRegister;
        private readonly string BucketFaceDetect;
        private readonly IConfiguration Configuration;


        public UsersController(IUserService userService, IMediaService mediaService, IEmployeeService employeeService, IConfiguration configuration)
        {
            Configuration = configuration;
            BucketFaceRegister = Configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE_REGISTER"];
            BucketFaceDetect = Configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE-DETECT"];
            _userService = userService;
            _mediaService = mediaService;
            _employeeService = employeeService;
        }


        #region Show all
        [HttpGet]
        public async Task<IActionResult> ShowAll([FromQuery] PaginationRequest paginationRequest)
        {
            (PaginationResponse<User> data, string message) = _userService.ShowAllPagination(paginationRequest);
            return ResponseOk(data, message);
        }

        [HttpGet("Customers")]
        public async Task<IActionResult> ShowAllCustomerPagination([FromQuery] PaginationRequest paginationRequest)
        {
            (PaginationResponse<User> data, string message) = _userService.ShowAllCustomerPagination(paginationRequest);
            return ResponseOk(data, message);
        }
        [HttpGet("Employees")]
        public async Task<IActionResult> ShowAllEmployeesPagination([FromQuery] PaginationRequest paginationRequest)
        {
            (PaginationResponse<User> data, string message) = _userService.ShowAllEmployeePagination(paginationRequest);
            return ResponseOk(data, message);
        }

        [HttpGet("ByGroup/{groupId}")]
        public IActionResult GetUsersByGroup([FromQuery] PaginationRequest paginationRequest, string groupId)
        {
            (PaginationResponse<User> paginationResponse, string message) = _userService.GetUsersByGroup(paginationRequest, groupId);
            return ResponseOk(paginationResponse, message);
        }
        #endregion

        #region Detail user
        [HttpGet("{userId}")]
        public async Task<IActionResult> ShowDetail(string userId)
        {
            User user = _userService.ShowDetail(userId);
            if (user is null)
            {
                return ResponseBadRequest("UserIdDoNotExists");
            }
            return ResponseOk(user, "ShowDetailSuccess");
        }

        #endregion

        #region Them xoa sua user
        [HttpPost("v2")]
        public IActionResult CreateUserV2([FromForm] CreateUserRequest request)
        {
            (object data, string message) = _employeeService.CreateUser(request);

            if (data is null) return ResponseBadRequest(message);

            return ResponseOk(data, message);
        }

        [HttpPut("v2/{userId}")]
        public IActionResult UpdateUserV2([FromForm] UpdateUserRequest request, string userId)
        {
            (object data, string message) = _employeeService.UpdateUser(request, userId);

            if (data is null) return ResponseBadRequest(message);

            return ResponseOk(data, message);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            (bool result, string message) = _employeeService.DeleteUser(userId);
            if (!result) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }

        [HttpDelete("FaceImage/{userId}/{userModeKeyCode}")]
        public async Task<IActionResult> DeleteUser(string userId, string userModeKeyCode)
        {
            (bool result, string message) = _employeeService.DeleteOneFaceImage(userId, userModeKeyCode);
            if (!result) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }

        #endregion

        #region Get file  cms and web

        [Authorize]
        [HttpGet("File")]
        public async Task<IActionResult> GetFile()
        {
            string userId = User.FindFirst("UserId")?.Value ?? "";
            User user = _userService.GetUserCondition(x => x.UserId == userId).FirstOrDefault();
            if (user is null)
            {
                return ResponseBadRequest("TokenInvalid");
            }
            if (user.UserImage is null)
            {
                return ResponseNotFound();
            }
            (byte[] byteData, string mes, object sss) = _mediaService.GetFile(BucketFaceRegister, user.UserImage).Result;
            if (byteData is null)
            {
                return ResponseNotFound();
            }
            new FileExtensionContentTypeProvider().TryGetContentType(user.UserImage.Split("/").LastOrDefault(), out string contentType);
            return File((byte[])byteData, contentType ?? "application/octet-stream");
        }


        [HttpGet("GetFileId/{userId}")]
        public async Task<IActionResult> GetFileUser(string userId)
        {
            User user = _userService.GetUserCondition(x => x.UserId == userId).FirstOrDefault();
            if (user is null)
            {
                return ResponseBadRequest("TokenInvalid");
            }
            if (user.UserImage is null)
            {
                return ResponseNotFound();
            }
            (byte[] byteData, string mes, object sss) = _mediaService.GetFile(BucketFaceRegister, user.UserImage).Result;
            if (byteData is null)
            {
                return ResponseNotFound();
            }
            new FileExtensionContentTypeProvider().TryGetContentType(user.UserImage.Split("/").LastOrDefault(), out string contentType);
            return File((byte[])byteData, contentType ?? "application/octet-stream");
        }
        #endregion


        #region Api app
        [HttpPost("login")]
        public IActionResult login([FromForm] LoginFaceRequest loginFaceRequest)
        {
            (object data, string message) = _userService.DetectFace(loginFaceRequest);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }

        [HttpGet("profile")]
        public IActionResult Profile()
        {
            string authorize = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorize))
            {
                (CustomerWelCome dataNoToken, string messageNoToken) = _userService.getProfile(null);
                return ResponseOk(dataNoToken, messageNoToken);
            }
            var handler = new JwtSecurityTokenHandler();
            string token = authorize.Replace("Bearer ", "");
            var tokenS = handler.ReadToken(token) as JwtSecurityToken;
            var userId = tokenS.Claims.FirstOrDefault(claim => claim.Type == "UserId")?.Value;
            (CustomerWelCome data, string message) = _userService.getProfile(userId, tokenS);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }

        [HttpGet("checkUserExists/{email}")]
        public IActionResult checkUserExists(string email)
        {
            (User data, string messgae) = _userService.CheckUserExists(email);
            if (data is null)
            {
                return ResponseBadRequest(messgae);
            }
            return ResponseOk(data, messgae);
        }

        [Authorize]
        [HttpGet("profileV2")]
        public IActionResult profileV2()
        {
            string namePage = User.FindFirst("NamePage").Value;
            string keyWelcome = User.FindFirst("KeyWelcome").Value;
            string customerName = User.FindFirst("CustomerName").Value;
            CustomerWelCome customerWelCome = new CustomerWelCome()
            {
                NamePage = namePage,
                KeyWelcome = keyWelcome,
                CustomerName = customerName
            };
            return ResponseOk(customerWelCome, "profileSuccess");
        }

        [HttpGet("loginTestV2/{name}/{key}")]
        public IActionResult loginTestV2(string name, string key)
        {
            if (String.IsNullOrEmpty(name) || String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(key) || String.IsNullOrEmpty(key))
            {
                return ResponseBadRequest("NameAndKeyInvalid");
            }
            (CustomerWelCome data, string message) = _userService.LoginTestV2(name, key);
            if (data.Token is null)
            {
                return ResponseBadRequest("TryAgain");
            }
            return ResponseOk(data, message);
        }


        /// <summary>
        /// Đăng nhập user bằng code.
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [HttpGet("verificationCode/{userCode}")]
        public IActionResult VerificationCode(string userCode)
        {
            (CustomerWelCome data, string message) = _userService.VerificationCode(userCode);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }


        /// <summary>
        /// Cập nhật thông tin khách hàng sau khi verifi code. Có cả đăng ký face nếu truyền hình lên.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPut("App/UpdateInfo")]
        public IActionResult UpdateInfo([FromForm] UpdateCustomerRequest request)
        {
            var test = JsonConvert.SerializeObject(request);
            Console.WriteLine(test);
            string userId = User.FindFirst("UserId")?.Value;
            (object result, string message) = _employeeService.UpdateCustomer(request, userId);
            if (result is null) return ResponseBadRequest(message);
            return ResponseOk(result, message);
        }

        #endregion


        #region Tagmode

        //[HttpGet("GetUserTagMode")]
        //public async Task<IActionResult> GetUserTagMode([FromQuery] GetUserTagModeRequest request)
        //{
        //    List<UserTagMode> userTagModes = _userService.GetUserTagMode(x =>
        //                            x.ModeId == request.ModeId
        //                            && x.UserId == request.UserId
        //                        );
        //    IQueryable<Tag> tags = _tagService.GetAll(x => userTagModes.Select(x => x.TagId).ToList().Any(z => z == x.TagId));

        //    #region RequestTable
        //    //Sort
        //    tags = tags.ApplySort<Tag>(request.OrderByQuery);
        //    //Search
        //    tags = tags.Where(m => String.IsNullOrEmpty(request.SearchContent) ||
        //                               (!String.IsNullOrEmpty(m.TagComment) && m.TagComment.ToLower().Contains(request.SearchContent)) ||
        //                               (!String.IsNullOrEmpty(m.TagName) && m.TagName.ToLower().Contains(request.SearchContent)) ||
        //                               (!String.IsNullOrEmpty(m.TicketTypeId) && m.TicketTypeId.ToLower().Contains(request.SearchContent))
        //                               )
        //                        ;
        //    var Pagination = PaginationHelper<Tag>.ToPagedList(tags.AsEnumerable(), request.PageNumber, request.PageSize);
        //    #endregion
        //    return ResponseOk(PaginationResponse<Tag>.PaginationInfo(Pagination.ToList(), Pagination.PageInfo), "GetTagsSuccess");
        //}

        [HttpGet("ShowByTag")]
        public IActionResult ShowUserByTag([FromQuery] UserByTagPagination request)
        {
            (PaginationResponse<User> paginationResponse, string message) = _userService.ShowAllUserByTag(request);
            if (paginationResponse is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(paginationResponse, message);

        }
        #endregion


        #region Import
        [HttpPost("ImportEmployeesSync")]
        public IActionResult ImportEmployeesSync([FromForm] ImportEmployeeRequest request)
        {
            (object data, string message) = _employeeService.ImportEmployeesSync(request);
            if (data is null)
            {
                return ResponseBadRequest(message);
            }
            return ResponseOk(data, message);
        }
        #endregion

        [HttpGet("LoadImageByPath")]
        public IActionResult LoadImage([FromQuery] LoadFile loadFile)
        {
            (byte[] byteData, string mes, object sss) = _mediaService.GetFile(BucketFaceRegister, loadFile.Path).Result;
            if (byteData is null)
            {
                return ResponseNotFound();
            }
            new FileExtensionContentTypeProvider().TryGetContentType(loadFile.Path.Split("/").LastOrDefault(), out string contentType);
            return File(byteData, contentType ?? "application/octet-stream");

        }


        #region Setup Init project
        [HttpGet("SyncAllUser")]
        public IActionResult SyncAllUser()
        {
            var result = _employeeService.SyncAllUser();
            return ResponseOk(result);
        }

        [HttpDelete("DeleteAll")]
        public IActionResult DeleteAll()
        {
            var result = _employeeService.DeleteUserAll();
            return ResponseOk(result);
        }

        #endregion



    }
}
