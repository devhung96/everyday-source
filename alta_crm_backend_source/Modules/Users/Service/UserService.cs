using AutoMapper;
using Confluent.Kafka;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Project.App.Helpers;
using Project.Modules.Faces.FaceServices;
using Project.Modules.Groups.Enities;
using Project.Modules.Kafka.Producer;
using Project.Modules.Medias.Services;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using Project.Modules.Users.UserKafka;
using Project.Modules.UserTagModes.Entities;
using Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Project.Modules.Tags.Enities;
using System.Diagnostics;
using Project.Modules.UsersModes.Entities;
using Project.Modules.UserCodes.Enities;

namespace Project.Modules.Users.Service
{
    public interface IUserService
    {

        public JToken getKeyWelcome(string lon = null, string lat = null);

        #region Show all
        (PaginationResponse<User> data, string message) ShowAllPagination(PaginationRequest paginationRequest);
        (PaginationResponse<User> data, string message) ShowAllCustomerPagination(PaginationRequest paginationRequest);
        (PaginationResponse<User> data, string message) ShowAllEmployeePagination(PaginationRequest paginationRequest);
        (PaginationResponse<User> data, string message) ShowAllUserByTag(UserByTagPagination paginationRequest);
        User ShowDetail(string userId);
        #endregion

        Task<(User data, string message)> StoredAsync(UserStoredRequest valueInput);


        #region Api app
        (CustomerWelCome data, string message) getProfile(string userId, JwtSecurityToken tokenInfor = null);
        (User data, string messgae) CheckUserExists(string email);
        (CustomerWelCome data, string message) LoginTestV2(string name, string key);
        (CustomerWelCome data, string message) VerificationCode(string userCode);
        (CustomerWelCome data, string message) DetectFace(LoginFaceRequest registerFaceRequest);
        #endregion

        (PaginationResponse<User> paginationResponse, string message) GetUsersByGroup(PaginationRequest pagination, string groupId);
        IQueryable<User> GetUserCondition(Expression<Func<User, bool>> expression = null);

    }
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;
        private readonly IRepositoryWrapperMariaDB _repositoryWrapperMariaDB;
        private readonly IMediaService _mediaService;
        private readonly IConfiguration _configuration;
        public const string User = "USER";
        private readonly IFaceService _faceService;
        private readonly HandleTask<RegisterFace> _handleTaskRegister;
        private readonly ISupportUserService _supportUserService;
        private readonly IFaceGrpcService _faceGrpcService;


        public UserService(IFaceGrpcService faceGrpcService, IMapper mapper, IRepositoryWrapperMariaDB repositoryWrapperMariaDB, IMediaService mediaService, IConfiguration configuration, HandleTask<RegisterFace> _HandleTaskRegister, IFaceService faceService, ISupportUserService supportUserService)
        {
            _mediaService = mediaService;
            _repositoryWrapperMariaDB = repositoryWrapperMariaDB;
            _mapper = mapper;
            _configuration = configuration;
            _handleTaskRegister = _HandleTaskRegister;
            _faceService = faceService;
            _supportUserService = supportUserService;
            _faceGrpcService = faceGrpcService;
        }
        public (PaginationResponse<User> data, string message) ShowAllPagination(PaginationRequest paginationRequest)
        {
            IQueryable<User> users = _repositoryWrapperMariaDB.Users.FindAll().Include(x => x.Group);
            users = users.ApplySort<User>(paginationRequest.OrderByQuery);
            users = users.Where(x =>
                   string.IsNullOrEmpty(paginationRequest.SearchContent)
                   || (!string.IsNullOrEmpty(x.UserLastName) && !string.IsNullOrEmpty(x.UserFirstName) && (x.UserLastName + " " + x.UserFirstName).ToLower().Contains(paginationRequest.SearchContent.ToLower()))
                   || (!string.IsNullOrEmpty(x.UserCode) && x.UserCode.ToLower().Contains(paginationRequest.SearchContent.ToLower()))
                   || (!string.IsNullOrEmpty(x.UserEmail) && x.UserEmail.ToLower().Contains(paginationRequest.SearchContent.ToLower()))
                   || (!string.IsNullOrEmpty(x.UserPhone) && x.UserPhone.ToLower().Contains(paginationRequest.SearchContent.ToLower()))
                   || (!string.IsNullOrEmpty(x.UserAddress) && x.UserAddress.ToLower().Contains(paginationRequest.SearchContent.ToLower()))
                   || (!string.IsNullOrEmpty(x.Group.GroupName) && x.Group.GroupName.ToLower().Contains(paginationRequest.SearchContent.ToLower()))
                );
            PaginationHelper<User> paginationUsers = PaginationHelper<User>.ToPagedList(users, paginationRequest.PageNumber, paginationRequest.PageSize);
            PaginationResponse<User> paginationResponse = new PaginationResponse<User>(paginationUsers, paginationUsers.PageInfo);

            return (paginationResponse, "ShowAllPaginationSuccess");
        }

        public async Task<(User data, string message)> StoredAsync(UserStoredRequest valueInput)
        {
            IDbContextTransaction transaction = _repositoryWrapperMariaDB.BeginTransaction();
            string MediaFullPath = "";

            List<string> codeExists = _repositoryWrapperMariaDB.Users.FindAll().Select(x => x.UserCode).ToList();
            string UserCode =  GeneralHelper.RandomCode(6,codeExists);
            User user = _mapper.Map<UserStoredRequest, User>(valueInput);
            user.UserImage = MediaFullPath;
            user.UserCode = UserCode;
            _repositoryWrapperMariaDB.Users.Add(user);
            try
            {
                _repositoryWrapperMariaDB.SaveChanges();
                transaction.Commit();
                return (user, "RegisterUserSuccess");
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return (null, e.Message);
            }
        }




        public User ShowDetail(string userId)
        {
            User user = _repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).Include(x => x.Group).FirstOrDefault();
            if (user != null)
            {
                user.UserTags = _repositoryWrapperMariaDB.Tags.FindByCondition(x => user.UserTagIdsParse.Contains(x.TagId)).ToList();
                foreach (var item in _repositoryWrapperMariaDB.UserModes.FindByCondition(x => user.UserId.Contains(x.UserId) && x.ModeId.Equals("Face_ID")).ToList())
                {
                    item.UserModeImage = _mediaService.GetUrl(_configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE_REGISTER"], item.UserModeImage);
                    user.UserImages.Add(item.UserModeImage);
                    user.UserObjectImages.Add(item);
                }
                user.UserModes = _repositoryWrapperMariaDB.UserModes.FindByCondition(x => x.UserId.Equals(userId) && !x.ModeId.Equals("Face_ID")).ToList();



            }
            return user;
        }


        public string getPageName(Group group)
        {
            if (group is null)
                return null;
            var key = $"OutsideSystems:UserType:{group.GroupCode}";
            string valueInput = _configuration.GetSection(key).Value;
            return valueInput;
        }
        public JToken getKeyWelcome(string lon = null, string lat = null)
        {
            var timer = new Stopwatch();
            timer.Start();

            string urlApi = $"{_configuration["OutsideSystems:HDBankBooking"]}/api/Cms/Schedules?Lat={lat}&Lon={lon}";
            (string ResponseData, int? ResponseStatusCode) = HttpMethod.Get.SendRequestWithStringContent(urlApi);

            timer.Stop();
            Console.WriteLine("*********** Child getKeyWelcome" + timer.Elapsed.ToString(@"m\:ss\.fff"));


            if (ResponseStatusCode != 200)
                return default(JToken);
            JToken result = JToken.Parse(ResponseData);
            return result["data"];
        }


        public string BuidTokenBase(User user, string namePage = null, string keyWelcome = null, string CustomerName = null)
        {
            int expriedAt = int.Parse(_configuration["TokenSettings:ExpireToken"]);
            var access_claim = new[] {
                new Claim("UserId", user?.UserId ?? "default"),
                new Claim("UserEmail", user?.UserEmail ?? "default"),
                new Claim("UserCode", user?.UserCode ?? "default"),
                new Claim("NamePage", namePage ?? "default"),
                new Claim("KeyWelcome", keyWelcome ?? "default"),
                new Claim("CustomerName", CustomerName ?? "default"),
                new Claim(JwtRegisteredClaimNames.Iat, System.Convert.ToBase64String(Encoding.UTF8.GetBytes(user?.UserId ?? "default"))),
                new Claim(JwtRegisteredClaimNames.Exp, DateTime.Now.AddHours(expriedAt).ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["TokenSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var access_token = new JwtSecurityToken(_configuration["TokenSettings:Issuer"],
              _configuration["Jwt:Issuer"],
              access_claim,
              notBefore: DateTime.Now,
              expires: DateTime.Now.AddHours(expriedAt),
              signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(access_token);
        }
        public string BuildToken(User user, string namePage = null, string keyWelcome = null, string CustomerName = null)
        {
            return BuidTokenBase(user, namePage, keyWelcome, CustomerName);
        }
        public (CustomerWelCome data, string message) getProfile(string userId, JwtSecurityToken tokenInfor = null)
        {
            var keyWellcome = getKeyWelcome();
            if (string.IsNullOrEmpty(userId))
            {
                CustomerWelCome customerWelComeNoToken = new CustomerWelCome()
                {
                    NamePage = "Home",
                    KeyWelcome = keyWellcome?.SelectToken("key")?.ToString(),
                    KeyCodeValue = keyWellcome?.SelectToken("keyCodeValue")?.ToArray()
                };
                return (customerWelComeNoToken, "Success");
            }
            if (userId == "default" && tokenInfor != null)
            {
                CustomerWelCome customerWelComeDefault = new CustomerWelCome()
                {
                    NamePage = tokenInfor.Claims.FirstOrDefault(claim => claim.Type == "NamePage")?.Value,
                    KeyWelcome = tokenInfor.Claims.FirstOrDefault(claim => claim.Type == "KeyWelcome")?.Value,
                    CustomerName = tokenInfor.Claims.FirstOrDefault(claim => claim.Type == "CustomerName")?.Value
                };
                return (customerWelComeDefault, "Success");
            }
            User user = _repositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userId));
            if (user is null)
            {
                return (null, "UserDoNotExists");
            }
            Group group = _repositoryWrapperMariaDB.Groups.FirstOrDefault(x => user != null && x.GroupId.Equals(user.GroupId));
            string PageName = getPageName(group);

            CustomerWelCome customerWelCome = new CustomerWelCome(user)
            {
                KeyWelcome = keyWellcome?.SelectToken("key")?.ToString(),
                NamePage = PageName,
                KeyCodeValue = keyWellcome?.SelectToken("keyCodeValue")?.ToArray(),
                CustomerImage = _mediaService.GetUrl(_configuration["OutsideSystems:AWS_S3:S3_BUCKET_FACE_REGISTER"], user.UserImage)
            };
            return (customerWelCome, "getProfileSuccess");
        }

        public (User data, string messgae) CheckUserExists(string email)
        {
            User user = _repositoryWrapperMariaDB.Users
                .FindByCondition(x => x.UserEmail.Equals(email))
                .Include(x => x.Group)
                .FirstOrDefault();
            if (user is null)
            {
                return (null, "UserDoNotExistst");
            }
            return (user, "UserDoNotExistst");
        }

        public (CustomerWelCome data, string message) LoginTestV2(string name, string pageName)
        {
            pageName ??= "default";
            User user = new User
            {
                UserLastName = name,
                UserGender = 1
            };

            JToken keyWellcome = getKeyWelcome();

            CustomerWelCome customerWelCome = new CustomerWelCome(user)
            {
                KeyWelcome = keyWellcome?.SelectToken("key")?.ToString(),
                NamePage = pageName,
                Token = BuildTokenV2(user.UserLastName, keyWellcome?.SelectToken("key")?.ToString(), pageName)
            };

            return (customerWelCome, "loginTestV2");
        }
        public string BuildTokenV2(string name, string keyWelcome, string namePage)
        {
            return BuidTokenBase(null, namePage, keyWelcome, name);
        }


        public (CustomerWelCome data, string message) DetectFace(LoginFaceRequest registerFaceRequest)
        {
            var timer = new Stopwatch();
            timer.Start();
            //(DetectFace data, string messageFaceDetect) = _faceService.DetectFace(registerFaceRequest.Image);  use kafka

            string userId = "";

            if (registerFaceRequest.ModeId.Equals("Face_ID"))
            {
                (DetectFace data, string messageFaceDetect) = _faceGrpcService.DetectFace(registerFaceRequest.Image);
                if (data is null) return (null, messageFaceDetect);
                timer.Stop();
                Console.WriteLine("*********** Total: DetectFace" + timer.Elapsed.ToString(@"m\:ss\.fff"));
                userId = data.ExtenId;
            }

            else if (registerFaceRequest.ModeId.Equals("Card_ID"))
            {
                UserMode userMode = _repositoryWrapperMariaDB.UserModes.FirstOrDefault(x => x.ModeId.Equals("Card_ID") && x.UserModeKeyCode.Equals(registerFaceRequest.CardId));

                if (userMode is null)
                {
                    return (null, "CardIdNotFound");
                }

                userId = userMode.UserId;
            }

            User user = _repositoryWrapperMariaDB.Users.FindByCondition(x => x.UserId.Equals(userId)).Include(x => x.Group).FirstOrDefault();
            if (user is null) return (null, "UserDoNotExists");

            if (user.Group is null) return (null, "GroupNotFound");


            var timer1 = new Stopwatch();
            timer1.Start();
            string PageName = getPageName(user.Group);
            timer1.Stop();
            Console.WriteLine("*********** Total: getPageName: " + timer1.Elapsed.ToString(@"m\:ss\.fff"));

            var timer2 = new Stopwatch();
            timer2.Start();
            //var keyWellcome = getKeyWelcome();
            timer2.Stop();
            Console.WriteLine("*********** Total: getKeyWelcome:: " + timer2.Elapsed.ToString(@"m\:ss\.fff"));


            CustomerWelCome customerWelCome = new CustomerWelCome(user);
            //{
            //    NamePage = PageName,
            //    KeyWelcome = keyWellcome?.SelectToken("key")?.ToString(),
            //    KeyCodeValue = keyWellcome?.SelectToken("keyCodeValue")?.ToArray()
            //};


            customerWelCome.Token = BuildToken(user, customerWelCome.NamePage, customerWelCome.KeyWelcome, user.UserFirstName + user.UserLastName);

            return (customerWelCome, "loginSuccess");

        }

        public (CustomerWelCome data, string message) VerificationCode(string userCodeActive)
        {
            UserCode userCode = _repositoryWrapperMariaDB.UserCodeds.FindByCondition(x => x.UserCodeActive.Equals(userCodeActive)).FirstOrDefault();
            if(userCode is null) return (null, "UserCodeActiveNotFound");

            User user = _repositoryWrapperMariaDB.Users.FirstOrDefault(x => x.UserId.Equals(userCode.UserId));
            if (user is null) return (null, "UserDoNotExists");

            Group group = _repositoryWrapperMariaDB.Groups.FirstOrDefault(x => x.GroupId.Equals(user.GroupId));
            //string pageName = getPageName(group);
            //var keyWellcome = getKeyWelcome();

            if (user is null)
            {
                return (null, "UserDoNotExists");
            }
            CustomerWelCome customerWelCome = new CustomerWelCome(user);
            //{
            //    NamePage = pageName,
            //    KeyWelcome = keyWellcome?.SelectToken("key")?.ToString(),
            //    KeyCodeValue = keyWellcome?.SelectToken("keyCodeValue")?.ToArray()
            //};
            customerWelCome.Token = BuildToken(user, customerWelCome.NamePage, customerWelCome.KeyWelcome, user.UserFirstName + user.UserLastName);
            return (customerWelCome, "VerificationCodeSuccess");
        }

        public (PaginationResponse<User> paginationResponse, string message) GetUsersByGroup(PaginationRequest pagination, string groupId)
        {
            IQueryable<User> users = _repositoryWrapperMariaDB.Users.FindByCondition(x => x.GroupId.Equals(groupId));
            users = users.ApplySort<User>(pagination.OrderByQuery);
            users = users.Where(x =>
                   string.IsNullOrEmpty(pagination.SearchContent)
                   || (!string.IsNullOrEmpty(x.UserLastName) && !string.IsNullOrEmpty(x.UserFirstName) && (x.UserLastName + " " + x.UserFirstName).Contains(pagination.SearchContent))
                   || (!string.IsNullOrEmpty(x.UserCode) && x.UserCode.Contains(pagination.SearchContent))
                   || (!string.IsNullOrEmpty(x.UserEmail) && x.UserEmail.Contains(pagination.SearchContent))
                   || (!string.IsNullOrEmpty(x.UserPhone) && x.UserPhone.Contains(pagination.SearchContent))
                   || (!string.IsNullOrEmpty(x.UserAddress) && x.UserAddress.Contains(pagination.SearchContent))
                );
            PaginationHelper<User> dataUser = PaginationHelper<User>.ToPagedList(users, pagination.PageNumber, pagination.PageSize);
            PaginationResponse<User> paginationResponse = new PaginationResponse<User>(dataUser, dataUser.PageInfo);
            return (paginationResponse, "GetUsersByGroupSuccess");
        }

        public IQueryable<User> GetUserCondition(Expression<Func<User, bool>> expression = null)
        {
            return _repositoryWrapperMariaDB.Users.FindByCondition(expression);
        }



        public (PaginationResponse<User> data, string message) ShowAllCustomerPagination(PaginationRequest paginationRequest)
        {

            IQueryable<User> users = _repositoryWrapperMariaDB.Users.FindByCondition(x => x.GroupId.Equals("customer")).Include(x => x.Group);
            PaginationHelper<User> dataUser = PaginationHelper<User>.ToPagedList(users, paginationRequest.PageNumber, paginationRequest.PageSize);

            PaginationResponse<User> paginationResponse = new PaginationResponse<User>(dataUser, dataUser.PageInfo);
            return (paginationResponse, "ShowAllPaginationSuccess");
        }

        public (PaginationResponse<User> data, string message) ShowAllEmployeePagination(PaginationRequest paginationRequest)
        {
            IQueryable<User> users = _repositoryWrapperMariaDB.Users.FindByCondition(x => !x.GroupId.Equals("customer")).Include(x => x.Group);
            PaginationHelper<User> dataUser = PaginationHelper<User>.ToPagedList(users, paginationRequest.PageNumber, paginationRequest.PageSize);
            PaginationResponse<User> paginationResponse = new PaginationResponse<User>(dataUser, dataUser.PageInfo);
            return (paginationResponse, "ShowAllPaginationSuccess");
        }

        public (PaginationResponse<User> data, string message) ShowAllUserByTag(UserByTagPagination request)
        {
            Tag tag = _repositoryWrapperMariaDB.Tags.FirstOrDefault(x => x.TagId.Equals(request.TagId));
            if (tag is null)
            {
                return (null, "TagNotExist");
            }
            List<string> userIds = _repositoryWrapperMariaDB.UserTagModes.FindByCondition(x => x.TagId.Equals(request.TagId)).Select(x => x.UserId).ToList();
            var users = _repositoryWrapperMariaDB.Users.FindByCondition(x => userIds.Contains(x.UserId)).Include(x => x.Group);
            PaginationHelper<User> dataUser = PaginationHelper<User>.ToPagedList(users, request.PageNumber, request.PageSize);

            PaginationResponse<User> paginationResponse = new PaginationResponse<User>(dataUser, dataUser.PageInfo);
            return (paginationResponse, "ShowAllUserByTagSuccess");
        }

    }
}