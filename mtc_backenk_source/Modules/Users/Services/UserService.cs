using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project.App.DesignPatterns.ObserverPatterns;
using Project.App.DesignPatterns.Reponsitories;
using Project.App.Helpers;
using Project.App.Requests;
using Project.Modules.Groups.Entities;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Requests;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.Modules.Users.Services
{
    public interface IUserService
    {
        (User user, string message) Store(StoreUserRequest storeUser);
        (User user, string message) ChangePassword(ResetPasswordRequest request, string userId);
        (User user, string message) Edit(UpdateUserRequest request, string Id);
        (User user, string message) EditProfile(UpdateProfileRequest request, string Id);
        (User user, string message) EditStatus(string Id);
        (User user, string message) EditExpired(ExtendRequest request, string Id);
        (string token, string message) Login(LoginRequest loginRequest);
        (string token, string message) Logout(string accessToken);
        (List<User> data, string message) ShowAll(string GroupId = null, UserLevelEnum userLevel = UserLevelEnum.SUPERADMIN);
        (bool result, string message) DeleteUser(string userId);  
        (bool result, string message) DeleteRangeUser(DeleteUserRequest request, UserLevelEnum userLevelEnum, string groupId);
        (User user, string message) ShowDetail(string userId);
        (User user, string message) SendMailForgotPassword(string email);
        (User user, string message) ResetPassword(string CodeOTP, ResetPasswordRequest request);
        PaginationResponse<User> ShowTable(PaginationRequest request, string groupId = null, UserLevelEnum userLevel = UserLevelEnum.SUPERADMIN);
        (object data, string message) UpdateStatus(List<string> userIds, UserStatusEnum status);
    }

    public class UserService : IUserService
    {
        private readonly IRepositoryWrapperMariaDB repository;
        private readonly IDistributedCache cache;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        public UserService(IRepositoryWrapperMariaDB repository, IMapper mapper, IConfiguration configuration, IDistributedCache distribute)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.cache = distribute;
            this.configuration = configuration;
        }

        public (bool result, string message) DeleteUser(string userId)
        {
            (User user, string message) = ShowDetail(userId);
            if (user is null)
            {
                return (false, message);
            }
            if (!string.IsNullOrEmpty(user.UserImage))
            {
                string fileOld = user.UserImage.Replace(configuration["Backend - Url"] + "/", "");
                GeneralHelper.DeleteFile(fileOld);
            }
            repository.Users.Remove(user);
            repository.SaveChanges();
            return (true, "DeleteSuccess");
        }

        /// <summary>
        /// Kiem tra Id user
        /// Kiem ra email 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        public (User user, string message) Edit(UpdateUserRequest request, string Id)
        {
            #region Handle User
            User user = repository.Users.GetById(Id);
            if (user is null)
            {
                return (null, "UserNotExist");
            }
         
            // Status = Het han van duoc cap nhat thong tin

            Role role = new Role();
            if (!string.IsNullOrEmpty(request.RoleId) && user.RoleId != request.RoleId)
            {
                role = repository.Roles.FindByCondition(x=>x.RoleId.Equals(request.RoleId)).Include(x=>x.RolePermissions).FirstOrDefault();
                if (role is null)
                {
                    return (null, "RoleNotExist");
                }
            }
            string roleOld = user.RoleId;
            user = mapper.Map(request, user);
            user.UpdatedAt = DateTime.UtcNow;
            repository.Users.Update(user);
            repository.SaveChanges();
        
            #endregion End Handle

            #region Handle Permission update Role
            if (! string.IsNullOrEmpty(request.RoleId)&&! roleOld.Equals(request.RoleId))
            {
                List<string> pemissionDeletes = repository.RolePermissions.FindByCondition(x => x.RoleId.Equals(roleOld)).Select(x => x.PermissionCode).ToList();
                List<UserPermission> rmPermission = repository.UserPermissions.FindByCondition(x => pemissionDeletes.Contains(x.PermissionCode) && x.UserId.Equals(user.UserId)).ToList();
                repository.UserPermissions.RemoveRange(rmPermission);
                repository.SaveChanges();
            }

            if (!string.IsNullOrEmpty(request.RoleId) && !roleOld.Equals(request.RoleId))
            {
                List<UserPermission> userPermissions = new List<UserPermission>();
                foreach (RolePermission rolePermission in role.RolePermissions)
                {
                    userPermissions.Add(new UserPermission()
                    {
                        PermissionCode = rolePermission.PermissionCode,
                        UserId = user.UserId,
                    });
                }
                repository.UserPermissions.AddRange(userPermissions);
            }
            #endregion End Permission 

            repository.SaveChanges();
            if (user.ExpiredAt <= DateTime.Now)
            {
                user.UserStatus = UserStatusEnum.EXPIRED;
            }
            return (user, "UpdateUserSuccess");
        }
        public (User user, string message) EditProfile(UpdateProfileRequest request, string Id)
        {
            #region Handle User
            User user = repository.Users.GetById(Id);
            if (user is null)
            {
                return (null, "UserNotExist");
            }
            if (user.ExpiredAt <= DateTime.UtcNow)
            {
                user.UserStatus = UserStatusEnum.EXPIRED;
                return (null, "UserExpired");
            }
            #endregion 
            if(! (request.Image is null))
            {
                if(!string.IsNullOrEmpty(user.UserImage))
                {
                    string fileOld = user.UserImage.Replace(configuration["Backend - Url"] + "/", "");
                    GeneralHelper.DeleteFile(fileOld);
                }
                string path = request.Image.UploadFile("users").Result;
                user.UserImage = configuration["Backend-Url"] + "/users/" + path; 
            }
            user.UserName = string.IsNullOrEmpty(request.UserName) ? user.UserName : request.UserName;
            user.UpdatedAt = DateTime.UtcNow;
            repository.Users.Update(user);
            repository.SaveChanges();
            return (user, "Success");
        }
        public (User user, string message) EditStatus(string Id)
        {
            (User user, string message) = ShowDetail(Id);
            if (user is null)
            {
                return (user, message);
            }
            if (user.ExpiredAt <= DateTime.Now)
            {
                return (null, "UserExpired");
            }
            user.UserStatus = user.UserStatus == UserStatusEnum.ACTIVE ? UserStatusEnum.DEACTIVE : UserStatusEnum.ACTIVE;
            user.UpdatedAt = DateTime.Now;
            repository.Users.Update(user);
            repository.SaveChanges();
            return (user, "UpdateUserSuccess");
        }
        public (User user, string message) EditExpired(ExtendRequest request, string Id)
        {
            (User user, string message) = ShowDetail(Id);
            if (user is null)
            {
                return (user, message);
            }
            if (request.ExpiredAt <= DateTime.Now)
            {
                return (null, "ExpiredAtInvalid");
            }
            user.ExpiredAt = request.ExpiredAt;
            user.UpdatedAt = DateTime.Now;
            repository.Users.Update(user);
            repository.SaveChanges();
            return (user, "UpdateUserSuccess");
        }

        public (string token, string message) Logout(string accessToken)
        {
            BlacklistTokens blacklistTokens = repository.BlacklistTokens
                                                        .FindByCondition(x => x.BlackListToken.Equals(accessToken))
                                                        .FirstOrDefault();
            if (blacklistTokens is null)
            {
                return (null, "TokenFail");
            }
            else
            {
                repository.BlacklistTokens.Remove(blacklistTokens);
                repository.SaveChanges();
                return (accessToken, "LogoutSuccess");
            }
        }

        public (User user, string message) ShowDetail(string userId)
        {
            User user = repository.Users.FindByCondition(x => x.UserId.Equals(userId))
                                        .Include(x => x.UserPermissions)
                                        .FirstOrDefault();
            if (user is null)
            {
                return (null, "UserNotExist");
            }
            if (user.ExpiredAt >= DateTime.Now)
            {
                user.UserStatus = UserStatusEnum.EXPIRED;
            }
            return (user, "GetSuccess");
        }

        public PaginationResponse<User> ShowTable(PaginationRequest request, string groupId = null, UserLevelEnum userLevel = UserLevelEnum.SUPERADMIN)
        {
            IQueryable<User> users = ShowAll(groupId,userLevel).data.AsQueryable();
            users = users.Where(x => string.IsNullOrEmpty(request.SearchContent) || (
                               x.UserEmail.Contains(request.SearchContent)
                               || (!string.IsNullOrEmpty(x.UserName) && x.UserName.Contains(request.SearchContent))
                               ));
            users = SortHelper<User>.ApplySort(users, request.OrderByQuery);
            PaginationHelper<User> result = new PaginationHelper<User>(users.ToList(), users.Count(), request.PageNumber, request.PageSize);
            PaginationResponse<User> paginationResponse = new PaginationResponse<User>(users.AsEnumerable(), result.PageInfo);

            foreach(User user in paginationResponse.PagedData.ToList())
            {
                user.Permissions = repository.Permissions.FindByCondition(x => user.UserPermissions.Select(x => x.PermissionCode).Contains(x.PermissionCode)).ToList();
            }    
            return (paginationResponse);
        }

        /// <summary>
        /// Kiem tra Email da ton tai chua
        /// </summary>
        /// <param name="storeUser"></param>
        /// <returns></returns>
        public (User user, string message) Store(StoreUserRequest storeUser)
        {

            #region Validate User
            if (!storeUser.UserPass.Equals(storeUser.UserPassConfirm))
            {
                return (null, "PasswordNotMatch");
            }
            bool isEmail = repository.Users.FindByCondition(x => x.UserEmail.Equals(storeUser.UserEmail)).Any();
            if (isEmail)
            {
                return (null, "EmailAreadlyExist");
            }
            Group group = repository.Groups.GetById(storeUser.GroupId);
            if (!string.IsNullOrEmpty(storeUser.GroupId) && group is null)
            {
                return (null, "GroupNotExist");
            }
            // truyen groupID mac dinh la user
            storeUser.UserLevel = !string.IsNullOrEmpty(storeUser.GroupId) ? UserLevelEnum.USER : storeUser.UserLevel;
            Role role = new Role();
            if (!string.IsNullOrEmpty(storeUser.RoleId))
            {
                role = repository.Roles.FindByCondition(x => x.RoleId.Equals(storeUser.RoleId))?
                                       .FirstOrDefault();
                if (role is null)
                {
                    return (null, "RoleNotExist");
                }
                if (role.RoleLevel != storeUser.UserLevel)
                {
                    return (null, "RoleInValid");
                }
            }
            else
            {
                storeUser.RoleId = null;
            }
            #endregion

            #region Handle Insert Db User
            User user = mapper.Map<User>(storeUser);
            user.GroupId = string.IsNullOrEmpty(storeUser.GroupId) ? null : storeUser.GroupId;
     
            string saft = 6.RandomString();
            user.UserPass = (storeUser.UserPass + saft).HashPassword();
            user.UserSaft = saft;
            repository.Users.Add(user);
            repository.SaveChanges();

            #endregion

            #region Add Permision By Role
            if (!string.IsNullOrEmpty(storeUser.RoleId))
            {
                List<string> idPermissions = repository.RolePermissions.FindByCondition(x => x.RoleId.Equals(storeUser.RoleId)).Select(x => x.PermissionCode).ToList();
                role.Permissions = repository.Permissions.FindByCondition(x => idPermissions.Contains(x.PermissionCode)).ToList();
                List<UserPermission> userPermissions = new List<UserPermission>();
                foreach (Permission permission in role.Permissions)
                {
                    userPermissions.Add(new UserPermission()
                    {
                        PermissionCode = permission.PermissionCode,
                        UserId = user.UserId,
                    });
                }
                repository.UserPermissions.AddRange(userPermissions);
                repository.SaveChanges();
            }
            #endregion

            return (user, "AddUserSuccess");
        }


        private string BuildTokenForUser(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim("user_email",user.UserEmail),
                new Claim("user_id", user.UserId),
                new Claim("is_level", ((int)user.UserLevel).ToString()),
                new Claim("group_id", (string.IsNullOrEmpty(user.GroupId)) ? "": user.GroupId)
            };
            List<string> userPermissionCode;
            userPermissionCode = repository.UserPermissions
                                                 .FindByCondition(x => x.UserId.Equals(user.UserId)).Select(x => x.PermissionCode)
                                                 .ToList();
            foreach (var item in userPermissionCode)
            {
                claims.Add(new Claim(ClaimTypes.Role, item));
                claims.Add(new Claim("Permissions", item));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var _expires = DateTime.Now.AddHours(int.Parse(configuration["Jwt:Expires"].ToString()));
            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Issuer"],
                claims,
                expires: _expires,
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string token, string message) Login(LoginRequest loginRequest)
        {
            User user = repository.Users.FindByCondition(x => x.UserEmail.Equals(loginRequest.UserEmail)).Include(x=>x.Group)
                                        .FirstOrDefault();
            if (user is null)
            {
                return (null, "EmailNotExistSystem");
            }

            var str = (loginRequest.Password + user.UserSaft).HashPassword();
            if (str != user.UserPass)
            {
                return (null, "PasswordIncorrect");
            }
            if(user.UserStatus == UserStatusEnum.DEACTIVE)
            {
                return (null, "UserDeactive");
            }    
            if(user.ExpiredAt.HasValue && user.ExpiredAt<=DateTime.UtcNow || (user.Group is not null &&  user.Group.Expired.HasValue && user.Group.Expired <= DateTime.UtcNow))
            {
                return (null, "UserExpired");
            }    
            string token = BuildTokenForUser(user);
            BlacklistTokens blacklistTokens = new BlacklistTokens
            {
                BlackListToken = token
            };
            repository.BlacklistTokens.Add(blacklistTokens);
            repository.SaveChanges();
            return (token, "LoginSuccess");
        }

        public (List<User> data, string message) ShowAll(string GroupId = null, UserLevelEnum userLevel = UserLevelEnum.SUPERADMIN)
        {
            List<User> users = (string.IsNullOrEmpty(GroupId)) ? repository.Users.FindByCondition(x => x.UserLevel == userLevel)
                                                                                 .Include(x => x.Group)
                                                                                 .Include(x => x.Role)
                                                                                 .Include(x=>x.UserPermissions)
                                                                                 .ToList()
                                                               : repository.Users.FindByCondition(x => x.UserLevel == userLevel
                                                                                                    && x.GroupId != null
                                                                                                    && x.GroupId.Equals(GroupId)
                                                                                                  )
                                                                                .Include(x => x.Group)
                                                                                .Include(x => x.Role)
                                                                                .Include(x=>x.UserPermissions)
                                                                                .ToList();
            foreach (User user in users.Where(x => x.ExpiredAt <= DateTime.Now))
            {
                user.UserStatus = UserStatusEnum.EXPIRED;
            }
            return (users, "ShowAllSuccess");
        }

        public (User user, string message) SendMailForgotPassword(string email)
        {
            User user = repository.Users.FindByCondition(x => x.UserEmail.Equals(email)).FirstOrDefault();
            if (user is null)
            {
                return (null, "UserNotExist");
            }
            string CodeOTP = 6.RandomString();
            while (cache.Get(CodeOTP) is not null)
            {
                CodeOTP = 6.RandomString();
            }
            string link = configuration["ForgotPassword:Url"] + CodeOTP;
            SendMail(user, link);
            cache.SetString(CodeOTP, user.UserEmail, new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(5)));
            return (user, "SendMailSuccess");
        }
        private void SendMail(User user, string link)
        {
            SendMailRequest data = new SendMailRequest()
            {
                MessageSubject = "NOTICE ON MTC SYSTEM PASSWORD RESET",
                MessageContent = $"<b>Dear {user.UserName} </b>. <br/> This is link reset your password: { link } . " +
                $"<br/>Please visit the link to reset your password, the link will expire in 5 minutes."
                + "<br/>If you did not ask for a password reset, you can ignore this email."
                + "<br/>Best regards !",

                Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = user.UserEmail}
                }

            };
            ObserverPattern.Instance.Emit("SendMail", data);
        }

        public (User user, string message) ResetPassword(string CodeOTP, ResetPasswordRequest request)
        {
            string email = cache.GetString(CodeOTP);
            if (string.IsNullOrEmpty(email))
            {
                return (null, "CodeOtpExpired");
            }
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                return (null, "PasswordAndConfirmNotMatch");
            }
            User user = repository.Users.FindByCondition(x => x.UserEmail.Equals(email)).FirstOrDefault();
            user.UserPass = (request.Password + user.UserSaft).HashPassword();
            repository.Users.Update(user);
            repository.SaveChanges();
            return (user, "UpdatePasswordSuccess");
        }

        public (bool result, string message) DeleteRangeUser(DeleteUserRequest request, UserLevelEnum userLevelEnum, string groupId)
        {
            request.UserIds = request.UserIds.Distinct().ToList();
            if(userLevelEnum == UserLevelEnum.USER)
            {
                List<User> users = repository.Users.FindByCondition(x => x.UserLevel == userLevelEnum && x.GroupId.Equals(groupId) && request.UserIds.Contains(x.UserId)).ToList();
                if(users.Count == request.UserIds.Count)
                {

                    foreach (User user in users)
                    {
                        if (!string.IsNullOrEmpty(user.UserImage))
                        {
                            string fileOld = user.UserImage.Replace(configuration["Backend - Url"] + "/", "");
                            GeneralHelper.DeleteFile(fileOld);
                        }
                    }
                    repository.Users.RemoveRange(users);
                    repository.SaveChanges();
                    return (true, "Success");
                }
                return (false, "ListUserInValid");
               
            }
            else
            {
                List<User> users = repository.Users.FindByCondition(x => request.UserIds.Contains(x.UserId)).ToList();
                foreach (User user in users)
                {
                    if (!string.IsNullOrEmpty(user.UserImage))
                    {
                        string fileOld = user.UserImage.Replace(configuration["Backend - Url"] + "/", "");
                        GeneralHelper.DeleteFile(fileOld);
                    }
                }
                if (users.Count == request.UserIds.Count)
                {
                    repository.Users.RemoveRange(users);
                    repository.SaveChanges();
                    return (true, "Success");
                }
                return (false, "ListUserInValid");
            }
        }

        public (User user, string message) ChangePassword(ResetPasswordRequest request, string userId)
        {
            if(!request.Password.Equals(request.ConfirmPassword))
            {
                return (null, "PasswordNotMatch");
            }
            (User user, string message)  = ShowDetail(userId);
            if(user is null)
            {
                return (null, message);
            }
            user.UserSaft = 6.RandomString();
            user.UserPass = (request.Password + user.UserSaft).HashPassword();
            repository.Users.Update(user);
            repository.SaveChanges();
            return (user, "ChangePasswordSuccess");
        }

        public (object data, string message) UpdateStatus(List<string> userIds, UserStatusEnum status)
        {
            if(userIds.Count > 0)
            {
                userIds = userIds.Distinct().ToList();
            }
            List<User> users = repository.Users.FindByCondition(x => userIds.Contains(x.UserId)).ToList();
            if (userIds.Count != users.Count && users.Count > 0)
            {
                return (null, "ListUserInvalid");
            }
            foreach (User user in users.Where(x=>x.ExpiredAt>DateTime.Now))
            {
              user.UserStatus = status;
            }
            repository.Users.UpdateRange(users);
            repository.SaveChanges();
            return ("Success", "UpdateSuccess");
        }

     
    }
}
