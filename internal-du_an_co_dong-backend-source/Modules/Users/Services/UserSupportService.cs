using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML.Trainers.FastTree;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.App.Database;
using Project.Modules.Events.Services;
using Project.Modules.Organizes.Entities;
using Project.Modules.PermissionOrganizes.Entities;
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

namespace Project.Modules.Users.Services
{
    public interface IUserSupportService
    {
        (object data, string message) LoginCMS(string recaptcha, string userName, string passWord, string userStockCode = null);
        (object data, string message, string code) CheckRoleEvent(string userID, string eventID, string permissionsEvent, bool FlagCMS, string permissionDefault);
        (object data, string message, string code) RemoveRoomVidu(string userID, string eventID, string permissionsEvent, bool FlagCMS, string permissionDefault, string Token);
        (object data, string message) ChangeStatusVidu(string userID, string eventID, string token);
        public (object data, string message) LoginSuperAdmin(string recaptcha, string userName, string passWord);
        public (string token, string message) RefreshToken(string userId, string permissionDefault);

        public (object data, string message, string code) GetTokenViduLandingPage(string userID, string eventID, string permissionsEvent, string type = "PUBLISHER");
        (object data, string message) InviteSpeak(string eventId, string token, InviteSpeakRequest request);
        (object data, string message) ConfirmInviteSpeak(string eventId, string token, ConfirmInviteSpeakRequest request);
    }

    public class UserSupportService : IUserSupportService
    {
        private readonly MariaDBContext mariaDBContext;
        private readonly IConfiguration configuration;
        private readonly IUserService userService;
        private readonly string ViduURL = "";
        private readonly ISoketIO soketIO;
        public UserSupportService(MariaDBContext mariaDBContext, IConfiguration configuration, IUserService userService, ISoketIO soketIO)
        {
            this.mariaDBContext = mariaDBContext;
            this.configuration = configuration;
            this.userService = userService;
            ViduURL = configuration["OpenViduUrl"].toDefaultUrl();
            this.soketIO = soketIO;
        }



        public (object data, string message) LoginCMS(string recaptcha, string userName, string passWord, string userStockCode = null)
        {
            #region Recaptcha
            MultipartFormDataContent reCaptchaContent = new MultipartFormDataContent();
            reCaptchaContent.Add(new StringContent(configuration["GoogleReCaptcha:SecretKey"]), "secret");
            reCaptchaContent.Add(new StringContent(recaptcha), "response");
            var resultReCaptcha = HttpMethod.Post.SendRequestWithFormDataContent(configuration["GoogleReCaptcha:URL"] + "/siteverify", reCaptchaContent);
            JObject reCaptchaData = JObject.Parse(resultReCaptcha.responseData);
            if (resultReCaptcha.responseStatusCode != GeneralHelper.OK)
                return (null, resultReCaptcha.responseData);
            if (!(bool)reCaptchaData["success"])
                return (null, reCaptchaData["error-codes"][0].ToString());
            #endregion

            bool isEmail = userName.CheckEmail();
            if (isEmail)
            {
                var userSuperAll = mariaDBContext.UserSupers.Where(x => x.Level == UserSuper.USER_TYPE.ADMIN).ToList();
                UserSuper userSuper = userSuperAll.FirstOrDefault(x => x.Email == userName && x.Level == UserSuper.USER_TYPE.ADMIN);
                if (userSuper is null) return (null, "LoginInformationIncorrect");

                if (userSuper.Level == UserSuper.USER_TYPE.ADMIN && userSuper.OrganizeId is null) return (null, "UserNotExistOrgainze");
                var passEncryption = (userSuper.Salt + passWord).HashPassword();

                if (passEncryption != userSuper.Password) return (null, "LoginInformationIncorrect");
                string token = BuildTokenForUserSuper(userSuper.UserSuperId);

                return (new { token = token }, "LoginSuccess");
            }
            else
            {
                var organizes = mariaDBContext.Organizes.ToList();
                var userAdminAll = mariaDBContext.Users.ToList();

                var organize = organizes.FirstOrDefault(x => x.OrganizeCodeCk == userStockCode);
                if (organize is null) return (null, "LoginInformationIncorrect");

                var userAdmin = userAdminAll.FirstOrDefault(x => x.ShareholderCode == userName && x.OrganizeId == organize.OrganizeId);
                if (userAdmin is null)
                    return (null, "LoginInformationIncorrect");
                var passEncryption = (userAdmin.UserSalt + passWord).HashPassword();
                if (userAdmin.UserPassword != passEncryption)
                    return (null, "LoginInformationIncorrect");


                var permissionEvent = this.GetPermissionEvent(userAdmin.UserId);
                if (permissionEvent.Count == 0) return (null, "AccountHasNotPermission");

                //var token = BuildingTokenForUserAdmin(userAdmin.UserId);
                var token = BuildingTokenForUserAdminNew(userAdmin.UserId);
                return (new { token = token }, "LoginSuccess");
            }
        }


        public (object data, string message) LoginSuperAdmin(string recaptcha, string userName, string passWord)
        {
            #region Recaptcha
            MultipartFormDataContent reCaptchaContent = new MultipartFormDataContent();
            reCaptchaContent.Add(new StringContent(configuration["GoogleReCaptcha:SecretKey"]), "secret");
            reCaptchaContent.Add(new StringContent(recaptcha), "response");
            var resultReCaptcha = HttpMethod.Post.SendRequestWithFormDataContent(configuration["GoogleReCaptcha:URL"] + "/siteverify", reCaptchaContent);
            JObject reCaptchaData = JObject.Parse(resultReCaptcha.responseData);
            if (resultReCaptcha.responseStatusCode != GeneralHelper.OK)
                return (null, resultReCaptcha.responseData);
            if (!(bool)reCaptchaData["success"])
                return (null, reCaptchaData["error-codes"][0].ToString());
            #endregion
            var userSuperAll = mariaDBContext.UserSupers.Where(x => x.Level == UserSuper.USER_TYPE.SUPER).ToList();
            UserSuper userSuper = userSuperAll.FirstOrDefault(x => x.Email == userName && x.Level == UserSuper.USER_TYPE.SUPER);
            if (userSuper is null) return (null, "Thông tin đăng nhập không chính xác.");

            var passEncryption = (userSuper.Salt + passWord).HashPassword();
            if (passEncryption != userSuper.Password)
                return (null, "Thông tin đăng nhập không chính xác.");
            string token = BuildTokenForUserSuper(userSuper.UserSuperId);
            return (new { token = token }, "Đăng nhập thành công!");

        }


        public (string token, string message) RefreshToken(string userId, string permissionDefault)
        {

            switch (permissionDefault)
            {
                case "ADMIN":
                    {
                        UserSuper userSuper = mariaDBContext.UserSupers.FirstOrDefault(x => x.UserSuperId == userId && x.Level == UserSuper.USER_TYPE.ADMIN);
                        if (userSuper is null) return (null, "Tài khoản không đúng ");
                        string token = BuildTokenForUserSuper(userSuper.UserSuperId);
                        return (token, "Cập nhật token thành công!");
                    }

                case "CLIENT":
                    {
                        var userAdmin = mariaDBContext.Users.FirstOrDefault(x => x.UserId == userId);
                        if (userAdmin is null) return (null, "Tài khoản không đúng ");
                        var token = BuildingTokenForUserAdminNew(userAdmin.UserId);
                        return (token, "Cập nhật token thành công!");
                    }

                default:
                    {
                        return ("", "Permission default error");
                    }

            }
        }


        public string BuildTokenForUserSuper(string userId)
        {
            var user = mariaDBContext.UserSupers.FirstOrDefault(x => x.UserSuperId.Equals(userId));
            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("OrganizeId", user.OrganizeId == null ? "" : user.OrganizeId)
            };

            var permissonDefault = userService.GetPermissionDefault(userId);

            foreach (var item in permissonDefault)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
                claims.Add(new Claim("PermissionDefault", item));
            }
            //Dictionary<string, int> types = new Dictionary<string, int>()
            //{
            //            {"CM-MIC", 31},
            //            {"TW-ICE", 15}
            //};
            ////List<int> listRoles = new List<int>();
            //int totalModules = 14;
            //for (int i = 0; i < totalModules; i++)
            //{
            //    //Xét có quyền nào thuộc modules thứ i thì bỏ zô
            //    //listRoles.Add(15);
            //    claims.Add(new Claim(ClaimTypes.Role, "15"));
            //}

            //string valuePermission = Newtonsoft.Json.JsonConvert.SerializeObject(types);
            //claims.Add(new Claim("permission", valuePermission));
            // get permisson event 
            List<object> result = new List<object>();

            var events = this.GetCodeEventWithUser(user.OrganizeId);
            foreach (var item in events)
            {
                result.Add(new
                {
                    EventID = item,
                    PermissionCode = new List<string>()
                });
            }

            claims.Add(new Claim("PermissionEvents", JsonConvert.SerializeObject(result)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var _expires = DateTime.Now.AddHours(configuration["Jwt:Expires"].toInt());

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Issuer"],
                claims,
                expires: _expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public string BuildingTokenForUserAdmin(string userId)
        {
            var user = mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(userId));

            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("OrganizeId", user  == null ? "" : user.OrganizeId)
            };

            ;


            var permissonDefault = userService.GetPermissionDefault(userId);

            foreach (var item in permissonDefault)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
                claims.Add(new Claim("PermissionDefault", item));
            }
            var hung = this.GetPermissionEvent(userId);
            claims.Add(new Claim("PermissionEvents", JsonConvert.SerializeObject(hung)));


            //foreach (var item in permissonGroup)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, item));
            //    claims.Add(new Claim("Permissions", item));
            //}

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var _expires = DateTime.Now.AddHours(configuration["Jwt:Expires"].toInt());

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Issuer"],
                claims,
                expires: _expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        public string BuildingTokenForUserAdminNew(string userId)
        {
            var user = mariaDBContext.Users.FirstOrDefault(x => x.UserId.Equals(userId));

            List<Claim> claims = new List<Claim>
            {
                new Claim("UserId", userId),
                new Claim("OrganizeId", user  == null ? "" : user.OrganizeId)
            };

            ;


            var permissonDefault = userService.GetPermissionDefault(userId);

            foreach (var item in permissonDefault)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
                claims.Add(new Claim("PermissionDefault", item));
            }
            var hung = this.GetPermissionEventNew(userId);
            claims.Add(new Claim("PermissionEvents", JsonConvert.SerializeObject(hung)));


            //foreach (var item in permissonGroup)
            //{
            //    claims.Add(new Claim(ClaimTypes.Role, item));
            //    claims.Add(new Claim("Permissions", item));
            //}

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var _expires = DateTime.Now.AddHours(configuration["Jwt:Expires"].toInt());

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Issuer"],
                claims,
                expires: _expires,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
        public List<object> GetPermissionEvent(string userId)
        {
            var permissionEvent = mariaDBContext.UserPermissionEvents.Where(x => x.UserId.Equals(userId)).ToList();
            var listGroupbyEvent = permissionEvent.GroupBy(x => x.EventId).ToList();
            List<object> result = new List<object>();
            foreach (var item in listGroupbyEvent)
            {
                var temp = new
                {
                    EventID = item.Key,
                    PermissionCode = item.Select(x => x.PermissionCode).ToList()
                };
                result.Add(temp);
            }
            return result;
        }
        public List<object> GetPermissionEventNew(string userId)
        {
            var permissionEvent = mariaDBContext.UserPermissionEvents.Where(x => x.UserId.Equals(userId)).ToList();
            var listGroupbyEvent = permissionEvent.GroupBy(x => x.EventId).ToList();
            List<object> result = new List<object>();
            foreach (var item in listGroupbyEvent)
            {
                var temp = new
                {
                    EventID = item.Key,
                    PermissionCode = GetPermission(item.Select(x => x.PermissionId).ToList())
                };
                result.Add(temp);
            }
            return result;
        }
        //Get Permission
        public List<int> GetPermission(List<int> permissionIDs)
        {
            #region way first
            //List<int> result = new List<int>();
            //List<JObject> pms = new List<JObject>();
            //foreach (int item in permissionIDs) // Get all permision
            //{
            //    var permission = mariaDBContext.Permissions.Find(item);
            //    if (permission != null)
            //    {
            //        var xx = new { index = permission.FunctionId, code = permission.PermissionCode };
            //        pms.Add(JObject.FromObject(xx));
            //    }
            //}
            //for (int i = 0; i < 15; i++)
            //{
            //    result.Add(pms.Where(x => int.Parse(x["index"].ToString()) == i).Sum(x => int.Parse(x["code"].ToString())));
            //}
            //return result;
            #endregion
            var permissionOrganizes = mariaDBContext.Permissions
                .Where(x => permissionIDs.Contains(x.PermissionId)).ToList()
                .GroupBy(x => x.FunctionId)
                .OrderBy(x => x.Key).ToList();
            int maxFunction = mariaDBContext.Functions.Max(x => x.FunctionId); // số lớn nhất của function (tổng function)
            int[] result = new int[maxFunction + 1];
            foreach (var item in permissionOrganizes)
            {
                result[item.Key] = item.Sum(x => int.Parse(x.PermissionCode));
            }
            return result.ToList();
        }

        public (object data, string message, string code) CheckRoleEvent(string userID, string eventID, string permissionsEvent, bool FlagCMS, string permissionDefault)
        {
            if (String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(eventID))
                return (null, "Người dùng hoặc sự kiện không đúng", "UserOrEventIsIncorrect");
            string url = $"{ViduURL}/api/get-token";
            if (!FlagCMS)
            {
                JArray listEvent = JArray.Parse(permissionsEvent);
                if (!(listEvent.Any(x => x.SelectToken("EventID").ToString().Equals(eventID))))
                {
                    return (null, "Người dùng không có quyền truy cập sự kiện này.", "UserDoesNotHaveAccessToThisEvent");
                }
                else
                {
                    //JObject dataEvent = JObject.Parse(listEvent.FirstOrDefault(x => x.SelectToken("EventID").ToString().Equals(eventID)).ToString());
                    //var checkPermisson = false;
                    //if (dataEvent["PermissionCode"] != null && !String.IsNullOrEmpty(dataEvent["PermissionCode"].ToString()))
                    //{
                    //    JArray permissions = JArray.Parse(dataEvent["PermissionCode"].ToString());
                    //    checkPermisson = permissions.Any(x => x.Equals("SHOW_ALL_OPERATE_MANAGER"));
                    //}
                    object content = new
                    {
                        // Update 17-03-2021
                        //role = /*checkPermisson == true ? "PUBLISHER" :*/ "SUBSCRIBER",
                        role = "PUBLISHER",
                        user = userID,
                        room = eventID
                    };
                    var tokenAPI = HttpMethod.Post.SendRequestAsync2(url, content);
                    if (tokenAPI.Result.responseStatusCode != GeneralHelper.OK)
                        return (null, tokenAPI.Result.responseData, "Error");
                    return (JObject.Parse(tokenAPI.Result.responseData), "Thành công", "Success");
                }
            }
            else
            {
                int checkPermisson = -1;
                JArray listEvent = JArray.Parse(permissionsEvent);
                if (!(listEvent.Any(x => x.SelectToken("EventID").ToString().Equals(eventID))))
                {
                    return (null, "Người dùng không có quyền truy cập sự kiện này.", "UserDoesNotHaveAccessToThisEvent");
                }
                else
                {
                    if (permissionDefault.Equals("CLIENT"))
                    {
                        JObject dataEvent = JObject.Parse(listEvent.FirstOrDefault(x => x.SelectToken("EventID").ToString().Equals(eventID)).ToString());
                        if (dataEvent["PermissionCode"] != null && !String.IsNullOrEmpty(dataEvent["PermissionCode"].ToString()))
                        {
                            JArray permissions = JArray.Parse(dataEvent["PermissionCode"].ToString());
                            checkPermisson = int.Parse(permissions[1].ToString());
                        }
                    }
                    else if (permissionDefault.Equals("ADMIN"))
                    {
                        checkPermisson = 2;
                    }
                }
                object content = new
                {
                    role = checkPermisson == 2 ? "PUBLISHER" : "SUBSCRIBER",
                    user = userID,
                    room = eventID
                };
                var tokenAPI = HttpMethod.Post.SendRequestAsync2(url, content);
                if (tokenAPI.Result.responseStatusCode != GeneralHelper.OK)
                    return (null, tokenAPI.Result.responseData, "Error");
                return (JObject.Parse(tokenAPI.Result.responseData), "Thành công", "Success");
            }
        }


        public (object data, string message, string code) GetTokenViduLandingPage(string userID, string eventID, string permissionsEvent, string type = "SUBSCRIBER")
        {
            if (String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(eventID))
                return (null, "Người dùng hoặc sự kiện không đúng", "UserOrEventIsIncorrect");
            string url = $"{ViduURL}/api/get-token";

            JArray listEvent = JArray.Parse(permissionsEvent);
            if (!(listEvent.Any(x => x.SelectToken("EventID").ToString().Equals(eventID))))
            {
                return (null, "Người dùng không có quyền truy cập sự kiện này.", "UserDoesNotHaveAccessToThisEvent");
            }
            else
            {

                object content = new
                {
                    role = type,
                    user = userID,
                    room = eventID
                };
                var tokenAPI = HttpMethod.Post.SendRequestAsync2(url, content);
                if (tokenAPI.Result.responseStatusCode != GeneralHelper.OK)
                    return (null, tokenAPI.Result.responseData, "Error");
                return (JObject.Parse(tokenAPI.Result.responseData), "Thành công", "Success");
            }

        }





        public (object data, string message, string code) RemoveRoomVidu(string userID, string eventID, string permissionsEvent, bool FlagCMS, string permissionDefault, string Token)
        {
            if (String.IsNullOrEmpty(userID) || String.IsNullOrEmpty(eventID))
                return (null, "Người dùng hoặc sự kiện không đúng", "UserOrEventIsIncorrect");
            string url = $"{ViduURL}/api/remove-user";
            if (!FlagCMS)
            {
                JArray listEvent = JArray.Parse(permissionsEvent);
                if (!(listEvent.Any(x => x.SelectToken("EventID").ToString().Equals(eventID))))
                {
                    return (null, "Người dùng không có quyền truy cập sự kiện này.", "UserDoesNotHaveAccessToThisEvent");
                }
                else
                {
                    //JObject dataEvent = JObject.Parse(listEvent.FirstOrDefault(x => x.SelectToken("EventID").ToString().Equals(eventID)).ToString());
                    //var checkPermisson = false;
                    //if (dataEvent["PermissionCode"] != null && !String.IsNullOrEmpty(dataEvent["PermissionCode"].ToString()))
                    //{
                    //    JArray permissions = JArray.Parse(dataEvent["PermissionCode"].ToString());
                    //    checkPermisson = permissions.Any(x => x.Equals("SHOW_ALL_OPERATE_MANAGER"));
                    //}
                    object content = new
                    {
                        role = /*checkPermisson == true ? "PUBLISHER" :*/ "SUBSCRIBER",
                        user = userID,
                        room = eventID,
                        token = Token
                    };
                    var tokenAPI = HttpMethod.Post.SendRequestAsync2(url, content);
                    if (tokenAPI.Result.responseStatusCode != GeneralHelper.OK)
                        return (null, tokenAPI.Result.responseData, "Error");
                    return (JObject.Parse(tokenAPI.Result.responseData), "Thành công", "Success");
                }
            }
            else
            {
                int checkPermisson = -1;
                JArray listEvent = JArray.Parse(permissionsEvent);
                if (!(listEvent.Any(x => x.SelectToken("EventID").ToString().Equals(eventID))))
                {
                    return (null, "Người dùng không có quyền truy cập sự kiện này.", "UserDoesNotHaveAccessToThisEvent");
                }
                else
                {
                    if (permissionDefault.Equals("CLIENT"))
                    {
                        JObject dataEvent = JObject.Parse(listEvent.FirstOrDefault(x => x.SelectToken("EventID").ToString().Equals(eventID)).ToString());
                        if (dataEvent["PermissionCode"] != null && !String.IsNullOrEmpty(dataEvent["PermissionCode"].ToString()))
                        {
                            JArray permissions = JArray.Parse(dataEvent["PermissionCode"].ToString());
                            checkPermisson = int.Parse(permissions[1].ToString());
                        }
                    }
                    else if (permissionDefault.Equals("ADMIN"))
                    {
                        checkPermisson = 2;
                    }
                }
                object content = new
                {
                    role = checkPermisson == 2 ? "PUBLISHER" : "SUBSCRIBER",
                    user = userID,
                    room = eventID,
                    token = Token
                };
                var tokenAPI = HttpMethod.Post.SendRequestAsync2(url, content);
                if (tokenAPI.Result.responseStatusCode != GeneralHelper.OK)
                    return (null, tokenAPI.Result.responseData, "Error");
                return (JObject.Parse(tokenAPI.Result.responseData), "Thành công", "Success");
            }
        }

        public List<string> GetCodeEventWithUser(string organizeId)
        {
            var resutl = mariaDBContext.Events.Where(x => x.OrganizeId == organizeId).Select(x => x.EventId).ToList();
            return resutl;
        }

        public (object data, string message) ChangeStatusVidu(string userID, string eventID, string token)
        {
            string urlRM = $"{ViduURL}/api/remove-user";
            object contentRM = new
            {
                role = "SUBSCRIBER",
                user = userID,
                room = eventID
            };
            var tokenAPI_RM = HttpMethod.Post.SendRequestAsync2(urlRM, contentRM);
            if (tokenAPI_RM.Result.responseStatusCode != GeneralHelper.OK)
                return (null, tokenAPI_RM.Result.responseData);
            string urlCR = $"{ViduURL}/api/get-token";
            object contentCR = new
            {
                role = "PUBLISHER",
                user = userID,
                room = eventID
            };
            var tokenAPI_CR = HttpMethod.Post.SendRequestAsync2(urlCR, contentCR);
            if (tokenAPI_CR.Result.responseStatusCode != GeneralHelper.OK)
                return (null, tokenAPI_CR.Result.responseData);
            #region
            List<string> userIDs = new List<string>();
            userIDs.Add(userID);
            soketIO.ForwardAsync(eventID, token, token, "speak-invite", userIDs, "0");
            #endregion
            return (JObject.Parse(tokenAPI_CR.Result.responseData), "Thành công");
        }

        public (object data, string message) InviteSpeak(string eventId, string token, InviteSpeakRequest request)
        {
            EventUser eventUser = mariaDBContext.EventUsers.FirstOrDefault(x => x.UserId.Equals(request.UserId) && x.EventId.Equals(eventId));

            if (eventUser is null)
            {
                return (null, "EventUserNotFound");
            }

            eventUser.UserInviteStatus = request.Status;
            mariaDBContext.SaveChanges();

            List<string> userIDs = new List<string>();
            userIDs.Add(request.UserId);
            object dataSocket = new
            {
                userId = request.UserId,
                statusSpeaking = request.Status
            };
            soketIO.ForwardAsync(eventId, dataSocket, token, "invite_speak", userIDs, "0");
            return ("Success", "Thành công");
        }

        public (object data, string message) ConfirmInviteSpeak(string eventId, string token, ConfirmInviteSpeakRequest request)
        {
            EventUser eventUser = mariaDBContext.EventUsers.FirstOrDefault(x => x.UserId.Equals(request.UserId) && x.EventId.Equals(eventId));

            if (eventUser is null)
            {
                return (null, "EventUserNotFound");
            }

            eventUser.UserInviteStatus = request.Status;
            mariaDBContext.SaveChanges();

            object dataSocket = new
            {
                userId = request.UserId,
                statusSpeaking = request.Status
            };

            soketIO.ForwardAsync(eventId, dataSocket, token, "confirm_invite_speak", null, "1");
            return ("Success", "Thành công");
        }
    }
}
