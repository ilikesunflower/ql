using CMS_App_Api.Helpers.AppSetting;
using CMS_EF.Models.Identity;
using CMS_Lib.DI;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMS_App_Api.Helpers
{
    public interface IJwtManager : IScoped
    {
        string GenerateJwtToken(ApplicationUser user);
        string GenerateJwtRefreshToken(ApplicationUser user);
    }

    public class JwtManager : IJwtManager
    {
        private readonly AppSettings _appSettings;

        public JwtManager(IOptions<AppSettings> appSettings)
        {
            this._appSettings = appSettings.Value;
        }

        public string GenerateJwtToken(ApplicationUser user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("token", "token")
                }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateJwtRefreshToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("token", "refreshToken")
                }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.RefreshTokenExpires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
