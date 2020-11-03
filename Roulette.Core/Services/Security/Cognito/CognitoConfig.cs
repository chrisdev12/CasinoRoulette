using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Roulette.Core.Services.Security.System;
using System;
using System.Net;

namespace Roulette.Core.Services.Security
{
    public class CognitoConfig : EnviromentVariables
    {
        public JwtBearerOptions SetBearerJwt(JwtBearerOptions options)
        {

            options.Audience = cognitoAppClientId;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (s, securityToken, identifier, parameters) =>
                {
                    var json = new WebClient().DownloadString(parameters.ValidIssuer + "/.well-known/jwks.json");
                    return JsonConvert.DeserializeObject<JsonWebKeySet>(json).Keys;
                },
                ValidateIssuer = true,
                ValidIssuer = $"https://cognito-idp.{awsRegion}.amazonaws.com/{cognitoPoolId}",
                ValidateLifetime = true,
                LifetimeValidator = (before, expires, token, param) => expires > DateTime.UtcNow,
                ValidateAudience = true
            };
            options.IncludeErrorDetails = true;
            options.SaveToken = true;
            options.Authority = $"https://cognito-idp.{awsRegion}.amazonaws.com/{cognitoPoolId}";
            options.RequireHttpsMetadata = true;

            return options;
        }
    }
}
