using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Project.App.Database;
using Project.App.Helpers;
using Project.App.Providers;
using Project.Modules.Users.Entities;
using Project.Modules.Users.Request;
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
    public interface IServiceUser
    {
        string BuildToken(User user);
        bool CheckPass(string input, User consider);
        string CreateHash(string input);
        JwtSecurityToken DecodeToken(string token);
        User DecodeTokenGetIdUser(string token);
        (User user, string message) CheckEmail(string email);
        (User user, string message) UpdatePassword(ForgotPasswordRequest request, string key);
        (object data, string message) CreateNewUser(RequestCreateNewUser createNewUser);

        (object data, string message) Login(RequestLogin request);

        (object data, string message) GetAllUser();

        (object data, string message) UpdateUser(int userid, UpdateUserRequest request);
        (object data, string message) DeleteUser(int userid);

        (object data, string message) ChangePassword(string token, ChangePasswordRequest changepass);
    }

    public class ServiceUser : IServiceUser
    {
        public readonly IConfiguration config;
        public readonly MariaDBContext dBContext;
        private readonly IDistributedCache cacheDBContext;
        public ServiceUser(IConfiguration _config, MariaDBContext _dBContext, IDistributedCache cache)
        {
            config = _config;
            dBContext = _dBContext;
            cacheDBContext = cache;
        }

        public JwtSecurityToken DecodeToken(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }

        public string BuildToken(User user)
        {
            List<Claim> claims = new List<Claim> { new Claim("UserID", user.UserId.ToString()), new Claim("UserName", user.UserName) };
            List<string> permissions = new List<string>();
            foreach (var item in user.UserPermissions)
            {
                permissions.Add(item.PermissionCode);
                claims.Add(new Claim(ClaimTypes.Role, item.PermissionCode));
            }
            claims.Add(new Claim("Permissions", JsonConvert.SerializeObject(permissions)));
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"].ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(config["Jwt:Issuer"].ToString(), config["Jwt:Issuer"].ToString(), claims, expires: DateTime.UtcNow.AddHours(7).AddDays(30), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string CreateHash(string input)
        {
            using (System.Security.Cryptography.SHA512 sha = System.Security.Cryptography.SHA512.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public bool CheckPass(string input, User consider)
        {
            string hash = CreateHash(input + consider.Salt);
            if (hash == consider.Password)
                return true;
            return false;
        }

        public (object data, string message) CreateNewUser(RequestCreateNewUser createNewUser)
        {
            User u = dBContext.Users.FirstOrDefault(x => x.UserName.Equals(createNewUser.UserName));
            if (u != null)
                return (null, "Username đã tồn tại.");

            Department department = dBContext.Departments.Include(x => x.DepartmentPermissions).FirstOrDefault(x => x.Code == createNewUser.DepartmentCode);
            if (department == null)
                return (null, "Mã bộ phận không tồn tại.");
            string salt = 8.RandomString();
            u = new User() { UserName = createNewUser.UserName, Salt = salt, Password = CreateHash(createNewUser.Password + salt), FullName = createNewUser.FullName };
            foreach (var item in department.DepartmentPermissions)
            {
                dBContext.UserPermissions.Add(new UserPermission
                {
                    PermissionCode = item.PermissionCode,
                    UserID = u.UserId,
                    PermissionID = item.PermissionID
                });
            }
            u.DepartmentID = department.ID;
            dBContext.Users.Add(u);
            dBContext.SaveChanges();
            return (new { UserID = u.UserId, UserName = u.UserName, FullName = u.FullName, CreatedAt = u.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"), Department = new { deparmentName = department.Name, departmentCode = department.Code } }, "Tạo tài khoản thành không!!!");
        }

        public User DecodeTokenGetIdUser(string token)
        {
            string userid = this.DecodeToken(token).Claims.First(x => x.Type.Equals("UserID")).Value;
            return dBContext.Users.Where(x => x.UserId == int.Parse(userid)).FirstOrDefault();
        }

        public (object data, string message) Login(RequestLogin request)
        {
            User u = dBContext.Users.Include(x => x.UserPermissions).FirstOrDefault(x => x.UserName.Equals(request.username));
            if (u == null)
                return (null, "Username không tồn tại.");
            if (!CheckPass(request.password, u))
                return (null, "Password không đúng.");
            string token = BuildToken(u);
            return (new { token = token, username = u.UserName, fullname = u.FullName }, "Đăng nhập thành công!!!");
        }

        public (object data, string message) GetAllUser()
        {
            return (dBContext.Users.Include(x => x.Department).Select(x => new { UserID = x.UserId, UserName = x.UserName, FullName = x.FullName, CreatedAt = x.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss"), Department = new { deparmentName = x.Department.Name, departmentCode = x.Department.Code } }).ToList(), "Get all user");
        }

        public (object data, string message) UpdateUser(int userid, UpdateUserRequest request)
        {
            User u = dBContext.Users.Include(x => x.Department).Include(x => x.UserPermissions).FirstOrDefault(x => x.UserId == userid);
            if (u == null)
                return (null, "Người dùng không tồn tại");
            if (!string.IsNullOrEmpty(request.userName))
            {
                User u2 = dBContext.Users.FirstOrDefault(x => x.UserName.Equals(request.userName) && x.UserId != u.UserId);
                if (u2 != null)
                    return (null, "Tên đăng nhập đã tồn tại");
                u.UserName = request.userName;
            }
            if (!string.IsNullOrEmpty(request.fullName))
                u.FullName = request.fullName;

            if (!string.IsNullOrEmpty(request.departmentcode))
            {
                Department dp = dBContext.Departments.Include(x => x.DepartmentPermissions).FirstOrDefault(x => x.Code.Equals(request.departmentcode));
                if (dp == null)
                    return (null, "Không tìm thấy mã phòng ban.");
                if (dp.Code != u.Department.Code)
                {
                    u.DepartmentID = dp.ID;
                    dBContext.UserPermissions.RemoveRange(u.UserPermissions);
                    foreach (var item in dp.DepartmentPermissions)
                    {
                        dBContext.UserPermissions.Add(new UserPermission
                        {
                            UserID = u.UserId,
                            PermissionID = item.PermissionID,
                            PermissionCode = item.PermissionCode
                        });
                    }
                }
            }
            dBContext.SaveChanges();
            return ("Thành công!!!", "Cập nhật người dùng thành công");

        }


        public (object data, string message) ChangePassword(string token, ChangePasswordRequest changepass)
        {
            User user = DecodeTokenGetIdUser(token);
            if (!CheckPass(changepass.oldPassword, user))
                return (null, "Mật khẩu cũ không đúng.");
            user.Password = CreateHash(changepass.newPassword + user.Salt);
            dBContext.SaveChanges();
            return ("Thành công!!!", "Đổi mật khẩu thành công.");
        }

        public (object data, string message) DeleteUser(int userid)
        {
            User u = dBContext.Users.FirstOrDefault(x => x.UserId == userid);
            if (u == null)
                return (null, "Nguời dùng không tồn tại.");
            u.Enable = false;
            dBContext.SaveChanges();
            return ("Thành công!!!", "Xóa người dùng thành công.");
        }

        public (User user, string message) CheckEmail(string email)
        {
            User user = dBContext.Users.FirstOrDefault(m => m.UserName.Equals(email));
            if (user is null)
            {
                return (null, "Tài khoản không tồn tại.");
            }

            string key = 6.RandomString();
            DistributedCacheEntryOptions option = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(300));
            cacheDBContext.SetString(key, email, option);

            string link = config["ForgotPassword:Url"] + key;
            SendEmail(user.UserName, link);

            return (user, "Gửi email thành công.");

        }
        public void SendEmail(string email, string link)
        {
            TransportPatternProvider.Instance.Emit("SendEmail", new SendMailRequest
            {

                MessageSubject = "Quên mật khẩu đăng nhập hệ thống DUTY FREE WAREHOUSE",
                MessageContent = "Đây là liên kết đổi mật khẩu đăng nhập của bạn: " + link
                                            + "<br/> Vui lòng truy cập liên kết và tạo mật khẩu mới."
                                              + "<br/> Liên kết của bạn sẽ hết hạn sau 5 phút."
                                             + "<br/> Trân trọng!",
                Contacts = new List<SendMailContact>
                {
                    new SendMailContact{ContactEmail = email}
                }
            });
        }
        public (string email, string message) CheckKey(string key)
        {
            string email = cacheDBContext.GetString(key);
            return string.IsNullOrEmpty(email) ? (null, "Mã xác nhận không tồn tại. ") : (email, "Thành công.");
        }
        public (User user, string message) UpdatePassword(ForgotPasswordRequest request, string key)
        {
            (string email, string message) = CheckKey(key);

            if (string.IsNullOrEmpty(email))
            {
                return (null,message);
            }

            cacheDBContext.Remove(key);

            User user = dBContext.Users.FirstOrDefault(m => m.UserName.Equals(email));

            string saft = 8.RandomString();
            user.Salt = saft;
            user.Password = CreateHash(request.PasswordNew + saft);
            dBContext.Users.Update(user);
            dBContext.SaveChanges();

            return (user, "Cập nhật mật khẩu thành công.");
        }
    }
}
