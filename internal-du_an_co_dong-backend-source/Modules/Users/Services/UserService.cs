using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Information;
using OfficeOpenXml.Table.PivotTable;
using Project.App.Database;
using Project.App.Helpers;
using Project.App.Providers;
using Project.App.Requests;
using Project.Modules.Authorities.Entities;
using Project.Modules.Events.Entities;
using Project.Modules.Organizes.Entities;
using Project.Modules.PermissionGroups.Entities;
using Project.Modules.PermissonUsers;
using Project.Modules.Users.Caches;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Project.Modules.Users.Entities.User;

namespace Project.Modules.Users.Services
{
    public interface IUserService
    {
        (EventUser user, string message) ChangePassword(ChangePasswordRequest request, string userId, string eventId);

        (object user, string message) ChangePasswordCMS(ChangePasswordRequest request, string userId);

        (object user, string message) EditUser(UpdateUserRequest request, string userId);

        (User user, string message) GetUser(string userId);

        ResponseTable GetUsers(RequestTable request, int userType);

        public Task<(object data, string message)> LoginBySuperAdminAsync(string userName, string pass);
        public Task<(object data, string message)> LoginAdminAsync(string userShareholderCode, string userStockCode, string pass);
        public (object data, string message, string code) LoginClient(string recapcha, string userShareholderCode, string pass, string eventId);
        Task<(object data, string message,string code)> AuthenticationAccountAsync(string userId, string eventId);
        public List<string> GetPermissionDefault(string userId);

    }
    public class UserService : IUserService
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly TokenHelper tokenHelper;
        private readonly IConfiguration _configuration;
        private readonly IManagementOTPService _managementOTPService;
        private readonly string _openViduUrl = "";


        public UserService(MariaDBContext mariaDBContext, IConfiguration configuration, IManagementOTPService managementOTPService)
        {
            this.mariaDBContext = mariaDBContext;
            this.tokenHelper = new TokenHelper(configuration);
            this._configuration = configuration;
            _managementOTPService = managementOTPService;
            _openViduUrl = configuration["OpenViduUrl"].toDefaultUrl("http://192.168.10.122:5000");
        }


        public (EventUser user, string message) ChangePassword(ChangePasswordRequest request, string userId, string eventId)
        {
            EventUser eventUser = mariaDBContext.EventUsers.FirstOrDefault(m => m.UserId.Equals(userId) && m.EventId.Equals(eventId));
            if (eventUser is null)
            {
                return (null, "TokenFaild");
            }

            if (eventUser.UserPassword.Equals((eventUser.UserSalt + request.PasswordOld).HashPassword()))
            {
                string saft = 5.RandomString();
                eventUser.UserPassword = (saft + request.PasswordNew).HashPassword();
                eventUser.UserSalt = saft;
                eventUser.PasswordSystem = null;
                mariaDBContext.EventUsers.Update(eventUser);
                mariaDBContext.SaveChanges();
                return (eventUser, "UpdatePasswordSuccess");
            }

            return (null, "PasswordIncorect");
        }


        public (object user, string message) EditUser(UpdateUserRequest request, string userId)
        {
            UserSuper user = mariaDBContext.UserSupers.Where(m=>m.UserSuperId.Equals(userId)).FirstOrDefault();
            if (user is null)
            {
                User userAdmin =mariaDBContext.Users
                                                .Where(m => m.UserId.Equals(userId))
                                                .FirstOrDefault();
                if(userAdmin is null)
                {
                    return (null, "UserNotExist");
                }


                User checkShareholder = mariaDBContext.Users
                                                   .Where(m => m.ShareholderCode.Equals(request.ShareholderCode)
                                                          && (m.OrganizeId.Equals(userAdmin.OrganizeId)
                                                          && (!m.UserId.Equals(userAdmin.UserId))))
                                                   .FirstOrDefault();
                if ((checkShareholder is null))
                {
                    return (null, "ShareholderCodeNotExist");
                }
                #region MergeData
                _ = !String.IsNullOrEmpty(request.UserEmail) ? userAdmin.UserEmail = request.UserEmail : userAdmin.UserEmail;
                _ = !String.IsNullOrEmpty(request.FullName) ? userAdmin.FullName = request.FullName : userAdmin.FullName;
                _ = !String.IsNullOrEmpty(request.ShareholderCode) ? userAdmin.ShareholderCode = request.ShareholderCode : userAdmin.ShareholderCode;
                _ = !String.IsNullOrEmpty(request.IdentityCard) ? userAdmin.IdentityCard = request.IdentityCard : userAdmin.IdentityCard;
                _ = !String.IsNullOrEmpty(request.PlaceOfIssue) ? userAdmin.PlaceOfIssue = request.PlaceOfIssue : userAdmin.PlaceOfIssue;
                _ = !String.IsNullOrEmpty(request.PhoneNumber) ? userAdmin.PhoneNumber = request.PhoneNumber : userAdmin.PhoneNumber;
                _ = !String.IsNullOrEmpty(request.IssueDate.ToString()) ? userAdmin.IssueDate = request.IssueDate.Value : userAdmin.IssueDate;
                userAdmin.UserImage = request.UserImage;
                #endregion
                mariaDBContext.Users.Update(userAdmin);
                mariaDBContext.SaveChanges();
                return (userAdmin, "UpdateUserSuccess");

            }



            #region MergeData
            _ = !String.IsNullOrEmpty(request.UserEmail) ? user.Email = request.UserEmail : user.Email;
            _ = !String.IsNullOrEmpty(request.UserImage) ? user.Image = request.UserImage : user.Image;
            _ = !String.IsNullOrEmpty(request.FullName) ? user.FullName = request.FullName : user.FullName;
            #endregion

            mariaDBContext.UserSupers.Update(user);
            mariaDBContext.SaveChanges();
            return (user, "UpdateUserSuccess");
        }

        public (User user, string message) GetUser(string userId)
        {
            User user = mariaDBContext.Users.Where( m => m.UserId.Equals(userId))
                                            .FirstOrDefault();

            if (user is null)
            {
                return (null, "UserNotExist");
            }
            return (user, "Success");
        }

        public ResponseTable GetUsers(RequestTable request, int groupId)
        {
            ResponseTable table;

            List<User> users;
            if (request.Page == 0)
            {
                #region ShowAll
                users = mariaDBContext.Users.Where(
                    m => m.UserEmail.Contains(request.Search) || m.UserEmail.Contains(request.Search))
                    .ToList();
                table = new ResponseTable
                {
                    DateResult = users,
                    Total = users.Count
                };
                #endregion
            }
            else
            {
                #region Pagination
                users = mariaDBContext.Users.Where(
                    m => (m.UserEmail.Contains(request.Search)))
                    .ToList();
                List<User> result = users.Skip((request.Page - 1) * (request.Results)).Take(request.Results).ToList();
                table = new ResponseTable
                {
                    DateResult = users,
                    Info = new Info
                    {
                        Page = request.Page,
                        Results = request.Results,
                        TotalRecord = result.Count(),
                    },
                    Total = users.Count()
                };
                #endregion
            }
            return table;
        }

        public object Login(LoginRequest login)
        {
        
            User user = mariaDBContext.Users.Where(m => m.IdentityCard.Equals(login.UserName) || m.PhoneNumber.Equals(login.UserName))
                             .FirstOrDefault();
            string token = tokenHelper.BuildToken(user);
            object result = new
            {
                userName = user.FullName,
                tokenAccess = token,
            };
            return result;
        }

        public (User user, string message) StoreUser(AddUserRequest request)
        {
            string salt = 5.RandomString();
            User user = mariaDBContext.Users.Where(
                                                m => m.PhoneNumber.Equals(request.PhoneNumber)
                                                || (m.IdentityCard.Equals(request.IdentityCard))
                                               )
                                            .FirstOrDefault();
            if (!(user is null))
            {
                return (null, "UserAreadlyExist");
            }

            #region new User
            user = new User
            {
                UserId = Guid.NewGuid().ToString(),
                UserEmail = request.UserEmail,
                UserImage = request.UserImage,
                UserSalt = salt,
                UserPassword = (salt + request.UserPassword).HashPassword(),
                //UserStocks = request.UserStock,
                UserCreatedAt = DateTime.UtcNow.AddHours(7),
                FullName = request.FullName,
                IdentityCard = request.IdentityCard,
                IssueDate = request.IssueDate,
                PlaceOfIssue = request.PlaceOfIssue,
                StockCode = request.StockCode,
                PhoneNumber = request.PhoneNumber,
            };
            #endregion

            mariaDBContext.Users.Add(user);
            mariaDBContext.SaveChanges();

            string userType = null;
            switch (request.UserType)
            {
                case 0:
                    userType = "CLIENT";
                    break;
                case USER_TYPE.ADMIN:
                    userType = "ADMIN";
                    break;

            }
            var permissionUser = new PermissionUser
            {
                UserId = user.UserId,
                PermissionCode = userType
            };
            mariaDBContext.PermissionUsers.Add(permissionUser);
            mariaDBContext.SaveChanges();


            return (user, "AddUserSuccess");



        }


        /// <summary>
        /// + Description: Login cho quản lý tối cao.
        /// + User : SUPERADMIN  
        /// + Login info  : Email and Password
        /// + Quy trình : Vào bảng user_super lấy thông tin . Qua user_permisson lấy thông tin mặc định.
        /// </summary>
        /// <returns></returns>
        public async Task<(object data, string message)> LoginBySuperAdminAsync(string userName, string pass)
        {
            //    var passEncryption = pass.HashPassword();
            //    UserSuper userSuper = mariaDBContext.UserSupers.FirstOrDefault(x => x.Email == userName && x.Password == passEncryption);
            //    if (userSuper is null) return (null, "Incorrect account information");

            //    string token = this.BuildingTokenForUser(userSuper.UserSuperId);
            return (new { token = "" }, "Login success!");
        }


        /// <summary>
        /// + Description: Login trang quản lý tổ chức đó.
        /// + User : ADMIN, Client có quyền vào.
        /// + Admin login info : Email and Password
        /// + Client login info = : CMND and Password
        /// + Quy trình : Vào bảng user lấy thông tin . Qua user_permisson lấy thông tin mặc định. Và permisson group
        /// + id:(1) hung.tran@gmail.com   245   123123     1
        /// + id:(2) hung.tran@gmail.com   245   123123     1
        /// </summary>
        /// <returns></returns>
        public async Task<(object data, string message)> LoginAdminAsync(string userShareholderCode, string userStockCode, string pass)
        {
            //bool isEmail = userShareholderCode.CheckEmail();

            //var token = "";
            //User user;
            //if (isEmail)
            //{
            //    // Admin login
            //    user = mariaDBContext.Users.FirstOrDefault(x => x.UserEmail == userShareholderCode && x.StockCode == userStockCode );
            //}
            //else
            //{
            //    // Client login
            //    user = mariaDBContext.Users.FirstOrDefault(x => x.ShareholderCode == userShareholderCode && x.StockCode == userStockCode );
            //}


            //if (user is null) return (null, "Incorrect account information");
            //var passEncryption = (user.UserSalt +pass).HashPassword();
            //if(user.UserPassword != passEncryption) return (null, "Incorrect account information");

            //token = BuildingTokenForUser(user.UserId);
            return (new { token = "" }, "Login success!");
        }

        /// <summary>
        /// Description: Login vào trang lading page . 
        /// + User :  Client.
        /// + Client login info : CMND and Password Result : Send mail OTP
        /// </summary>
        /// <returns></returns>
        public (object data, string message, string code) LoginClient(string recapcha, string userShareholderCode, string pass, string eventId)
        {
            #region Recaptcha
            MultipartFormDataContent reCaptchaContent = new MultipartFormDataContent();
            reCaptchaContent.Add(new StringContent(_configuration["GoogleReCaptcha:SecretKey"]), "secret");
            reCaptchaContent.Add(new StringContent(recapcha), "response");
            var resultReCaptcha = HttpMethod.Post.SendRequestWithFormDataContent(_configuration["GoogleReCaptcha:URL"] + "/siteverify", reCaptchaContent);
            JObject reCaptchaData = JObject.Parse(resultReCaptcha.responseData);
            if (resultReCaptcha.responseStatusCode != GeneralHelper.OK)
                return (null, resultReCaptcha.responseData, "ErorrCaptcha");
            if (!(bool)reCaptchaData["success"])
                return (null, reCaptchaData["error-codes"][0].ToString(), "ErorrCaptcha");
            #endregion

            Event _eventFlag = mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_eventFlag is null) return (null, "EventNotFound", "EventNotFound");

            var userAll = mariaDBContext.Users.Where(x => x.OrganizeId.Equals(_eventFlag.OrganizeId)).ToList();
            User user = userAll.FirstOrDefault(x => x.ShareholderCode == userShareholderCode&& x.OrganizeId.Equals(_eventFlag.OrganizeId));           
            if (user is null) return (null, "IncorrectAccountInformation", "IncorrectAccountInformation");

            //if (_eventFlag.EventFlag == EVENT_FLAG.END) return (null, "Sự kiện đã kết thúc", "TheEventHasEnded");

            EventUser eventUser = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId == eventId && x.UserId == user.UserId);
            if (eventUser is null) return (null, "TheAccountDoesNotHaveAccessToThisEvent", "TheAccountDoesNotHaveAccessToThisEvent");

            if(_eventFlag.EventFlag == EVENT_FLAG.BEGIN && eventUser.UserLatch == USER_LATCH.OFF)
            {
                return (null, "UnreachableBecauseTheEventHasStarted", "UnreachableBecauseTheEventHasStarted");
            }

            Authority authority = mariaDBContext.Authorities.FirstOrDefault(x => x.AuthorityUserID == user.UserId && x.AuthorityType == AuthorityType.EVENT && x.EventID.Equals(eventId));
            if(authority != null)
            {
                return (null, "TheAccountDoesNotHaveAccessToThisEvent:Uyquen", "TheAccountDoesNotHaveAccessToThisEvent:Uyquen");
            }
            //if (_eventFlag.EventFlag == EVENT_FLAG.BEGIN && eventUser.UserLoginStatus == USER_LOGIN_STATUS.OFF)
            //{
            //    return (null, "Không thể truy cập do sự kiện đã bắt đầu", "UnreachableBecauseTheEventHasStarted");
            //}    

            var passEncryption = (eventUser.UserSalt + pass).HashPassword();
            if (eventUser.UserPassword != passEncryption) return (null, "IncorrectAccountInformation", "IncorrectAccountInformation");

            if(eventUser.UserStock <= 0 ) return (null, "TheAccountDoesNotHaveAccessToThisEvent", "TheAccountDoesNotHaveAccessToThisEvent");

            var token = BuildingTokenClientStep1(user.UserId, eventId);
            eventUser.LoginAt = DateTime.Now;
            mariaDBContext.EventUsers.Update(eventUser);
            mariaDBContext.SaveChanges();
            return (new { token }, "LoginSuccess", "LoginSuccess");
        }


        /// <summary>
        /// Description: Xac thuc OTP . Tạo token
        /// </summary>
        /// <returns></returns>
        public async Task<(object data, string message, string code)> AuthenticationAccountAsync(string userId, string eventId)
        {
            Event _eventFlag = mariaDBContext.Events.FirstOrDefault(x => x.EventId == eventId);
            if (_eventFlag is null) return (null, "EventNotFound", "EventNotFound");

            var token = BuildingTokenClientStep2(userId, eventId);
            EventUser eventUser = mariaDBContext.EventUsers.FirstOrDefault(x => x.EventId == eventId && x.UserId == userId);
            if (eventUser is null) return (null, "AccountDoesNotHaveAccessToTheEvent", "AccountDoesNotHaveAccessToTheEvent");

            if (_eventFlag.EventFlag == EVENT_FLAG.BEGIN && eventUser.UserLatch == USER_LATCH.OFF)
            {
                return (null, "UnreachableBecauseTheEventHasStarted", "UnreachableBecauseTheEventHasStarted");
            }

            eventUser.UserLoginStatus = USER_LOGIN_STATUS.ON;
            eventUser.UserToken = token;
            mariaDBContext.SaveChanges();
            await RegisterUserToOpenViduAsync(userId);
            return (new { token }, "LoginSuccess", "LoginSuccess");
        }

        public string BuildingTokenClientStep1(string userId, string eventId)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("EventId", eventId)
            };


            claims.Add(new Claim(ClaimTypes.Role, "AUTHENCATION-CLIENT-STEP1"));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var _expires = DateTime.UtcNow.AddHours(7).AddSeconds(_configuration["TimeExpiredOTP"].TimeExpiredOTP());

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: _expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }


        public string BuildingTokenClientStep2(string userId, string eventId = "")
        {
            var user = this.GetUserInfo(userId);

            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("EventId", eventId),
                new Claim("OrganizeId", user  == null ? "" : user.OrganizeId)
            };

            List<object> result = new List<object>();
            result.Add(new
            {
                EventID = eventId,
                PermissionCode = "[]"
            });

            claims.Add(new Claim("PermissionEvents", JsonConvert.SerializeObject(result)));
            

            var permissonDefault = this.GetPermissionDefault(userId);

            foreach (var item in permissonDefault)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
                claims.Add(new Claim("PermissionDefault", item));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var _expires = DateTime.Now.AddHours(_configuration["Jwt:Expires"].toInt());

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: _expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }


        /// <summary>
        /// Get permission default for user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetPermissionDefault(string userId)
        {
            var userPermission = mariaDBContext.PermissionUsers
                                                    .Where(x => x.UserId == userId)
                                                    .Select(x => x.PermissionCode)
                                                    .ToList();
            return userPermission;
        }


        public User GetUserInfo(string userId)
        {
            var users = mariaDBContext.Users
                                                   .FirstOrDefault(x => x.UserId == userId);

            return users;
        }

        /// <summary>
        /// Get the user permission in group.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetPermissionGroup(string userId)
        {
            var groupUser = mariaDBContext.EventUsers.Where(x => x.UserId == userId).ToList();
            List<string> permissonGroup = new List<string>();
            foreach (var item in groupUser)
            {
                var permissionGroup = mariaDBContext.PermissionGroups
                                                            .Where(x => x.GroupId == item.GroupId)
                                                            .Select(x => x.PermissionCode)
                                                            .ToList();
                if (permissionGroup is null) continue;
                permissonGroup.AddRange(permissionGroup);
            }
            return permissonGroup;
        }


        public async Task RegisterUserToOpenViduAsync(string userId, string eventId = null, List<string> permissonGroup = null)
        {
            if (String.IsNullOrEmpty(eventId) || permissonGroup is null) return;
            string url = _openViduUrl + "/api/get-token";

            var checkPermisson = permissonGroup.Any(x => x.Equals("SHOW_ALL_OPERATE_MANAGER"));
            object body = new
            {
                role = checkPermisson == true ? "SUBSCRIBE" : "PUBLISHER",
                user = userId,
                room = eventId
            };

            (string responseData, int? responseStatusCode) = await HttpMethod.Post.SendRequestAsync2(url, body);
            if (responseStatusCode == 200)
            {
                var data = responseData;
            }
            return;


        }

        public (object user, string message) ChangePasswordCMS(ChangePasswordRequest request, string userId)
        {
            User user = mariaDBContext.Users.Where(m => m.UserId.Equals(userId)).FirstOrDefault();
            if (user is null)
            {
                UserSuper userAdmin = mariaDBContext.UserSupers
                                                        .Where(m =>
                                                                    m.UserSuperId.Equals(userId) &&
                                                                    m.Level == UserSuper.USER_TYPE.ADMIN &&
                                                                    m.DeleteStatus == User.DELETE_STATUS.EXIST
                                                                    ).FirstOrDefault();
                if (userAdmin is null)
                {
                    return (null, "PasswordIncorect");
                }
                else
                {

                    if (userAdmin.Password.Equals((userAdmin.Salt + request.PasswordOld).HashPassword()))
                    {
                        string salt = 5.RandomString();
                        userAdmin.Salt = salt;
                        userAdmin.Password = (salt + request.PasswordNew).HashPassword();
                        mariaDBContext.UserSupers.Update(userAdmin);
                        mariaDBContext.SaveChanges();
                        return (userAdmin, "ChangePasswordSuccess");
                    }
                }

            }
            else
            {
                if (user.UserPassword.Equals((user.UserSalt + request.PasswordOld).HashPassword()))
                {
                    string salt = 5.RandomString();
                    user.UserSalt = salt;
                    user.UserPassword = (salt + request.PasswordNew).HashPassword();
                    mariaDBContext.Users.Update(user);
                    mariaDBContext.SaveChanges();
                    return (user, "ChangePasswordSuccess");
                }
            }
            return (null, "PasswordIncorect");

        }
    }
}
