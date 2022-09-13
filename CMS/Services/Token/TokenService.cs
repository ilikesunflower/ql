using CMS_Lib.DI;
using JWT;
using JWT.Algorithms;
using JWT.Builder;
using JWT.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using CMS_Lib.Util;

namespace CMS.Services.Token
{
    public interface ITokenService : IScoped
    {
        string GenerateJwtToken(Dictionary<string, object> payload);

        IDictionary<string, object> DecodeJwtToken(string token);
    }

    public class TokenService : ITokenService
    {
        private readonly IConfigurationSection _appSettings;
        private readonly ILogger<TokenService> _iLogger;

        public TokenService(IConfiguration iConfiguration, ILogger<TokenService> iLogger)
        {
            this._appSettings = iConfiguration.GetSection(CmsConsts.AppSetting);
            this._iLogger = iLogger;
        }

        [Obsolete]
        public string GenerateJwtToken(Dictionary<string, object> payload)
        {
            var secret = this._appSettings.GetValue<string>("TokenSecret");
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm(); // symmetric
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            var token = encoder.Encode(payload, secret);
            return token;
        }

        [Obsolete]
        public IDictionary<string, object> DecodeJwtToken(string token)
        {
            try
            {
                var secret = this._appSettings.GetValue<string>("TokenSecret");
                IDictionary<string, object> payload = JwtBuilder.Create()
                    .WithAlgorithm(new HMACSHA256Algorithm()) // symmetric
                    .WithSecret(secret)
                    .MustVerifySignature()
                    .Decode<IDictionary<string, object>>(token);
                return payload;
            }
            catch (Exception ex)
            {
                this._iLogger.LogError("DecodeJwtToken fail", ex);
                return null;
            }
        }
    }
}
