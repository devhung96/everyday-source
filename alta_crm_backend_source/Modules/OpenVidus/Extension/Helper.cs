using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project.Modules.OpenVidus.Extension
{
    public class Helper
    {
        private readonly IConfiguration Configuration;
        public Helper(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string GenerateToken(List<string> permissionUsers = null, bool stranger = false, string roomId = "")
        {
            List<Claim> claims = new List<Claim>();
            if (stranger)
            {
                claims.Add(new Claim("AccountId", Guid.NewGuid().ToString()));
                claims.Add(new Claim("Type", "DEFAULT"));
                claims.Add(new Claim("RoomId", roomId));
            }
            else
            {
                claims.Add(new Claim("Type", "ADMIN"));
                claims.Add(new Claim("RoomId", roomId));
            }
            claims.Add(new Claim("Permission", Newtonsoft.Json.JsonConvert.SerializeObject(permissionUsers)));
            if (permissionUsers != null)
            {
                foreach (string item in permissionUsers)
                {
                    claims.Add(new Claim(ClaimTypes.Role, item.Trim()));
                }
            }
            var exp = DateTime.UtcNow.AddDays(int.Parse(Configuration["TokenSettings:ExpireToken"]));
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["TokenSettings:Key"].ToString()));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(Configuration["TokenSettings:Issuer"].ToString(), Configuration["TokenSettings:Issuer"].ToString(), claims, expires: exp, signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
