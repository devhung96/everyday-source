using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Project.Modules.Accounts.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Project.App.Helpers
{
    public class TokenHelper
    {
        private readonly IConfiguration config;
        public TokenHelper(IConfiguration config)
        {
            this.config = config;
        }

        public string GenerateToken(Account account, List<string> permissionUsers = null)
        {

            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("AccountId", account.AccountId));
            if (account.AccountType == Account.TYPE_ACCOUNT.CMS)
                claims.Add(new Claim("Type", "ADMIN"));
            if (account.AccountType == Account.TYPE_ACCOUNT.LECTURER)
                claims.Add(new Claim("Type", "LECTURER"));
            if (account.AccountType == Account.TYPE_ACCOUNT.STUDENT)
                claims.Add(new Claim("Type", "STUDENT"));
            claims.Add(new Claim("Permission", Newtonsoft.Json.JsonConvert.SerializeObject(permissionUsers)));
            foreach (string item in permissionUsers)
            {
                claims.Add(new Claim(ClaimTypes.Role, item.Trim()));
            }
            var exp = DateTime.UtcNow.AddDays(int.Parse(config["TokenSettings:ExpireToken"]));
            if (account.AccountType == Account.TYPE_ACCOUNT.STUDENT)
            {
                exp = DateTime.UtcNow.AddDays(int.Parse(config["TokenSettings:ExpireTokenLanding"]));
            }
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenSettings:Key"].ToString()));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(config["TokenSettings:Issuer"].ToString(), config["TokenSettings:Issuer"].ToString(), claims, expires: exp, signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
