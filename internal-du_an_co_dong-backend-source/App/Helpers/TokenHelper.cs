using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project.App.Database;
using Project.Modules.Permissions.Entities;
using Project.Modules.PermissonUsers;
using Project.Modules.Users.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Project.App.Helpers
{
    public class TokenHelper
    {
        private readonly IConfiguration config;
        public TokenHelper(IConfiguration config)
        {
            this.config = config;
        }
        public string BuildToken(User user)
        {
            List<Claim> claims = new List<Claim>() { new Claim("UserID", user.UserId.ToString()), new Claim("IdentityCard", user.IdentityCard), new Claim("UserEmail", user.UserEmail)};// CMS Type=0 : client =1
            var optionsBuilder = new DbContextOptionsBuilder<MariaDBContext>();
            optionsBuilder.UseMySql(config["ConnectionSetting:MariaDBSettings:ConnectionStrings"]);
            using (var mariaDBContext = new MariaDBContext(optionsBuilder.Options))
            {
                List<string> permissionUsers = mariaDBContext.PermissionUsers.Where(m => m.UserId == user.UserId).Select(m => m.PermissionCode).ToList();
                foreach(var item in permissionUsers)
                {
                    claims.Add(new Claim(ClaimTypes.Role,item.Trim()));
                }
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"].ToString()));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(config["Jwt:Issuer"].ToString(), config["Jwt:Issuer"].ToString(), claims, expires: DateTime.UtcNow.AddDays(int.Parse(config["Jwt:dayExpired"])), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
