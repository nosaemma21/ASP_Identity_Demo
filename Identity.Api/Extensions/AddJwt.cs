using System.Text;
using Identity.Api.CustomErrors;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Identity.Api.Extensions;

public static class AddJwt
{
    public static IServiceCollection AddJwtConfig(
        this IServiceCollection service,
        [FromServices] IConfiguration configuration
    )
    {
        service
            .AddAuthentication(o =>
            {
                o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                var Issuer = configuration["Jwt:Issuer"];
                var Audience = configuration["Jwt:Audience"];
                var Secret = configuration["Jwt:Secret"];

                if (Issuer is null || Audience is null || Secret is null)
                {
                    throw new JwtException("Something is wrong in the JWT config");
                }

                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = Issuer,
                    ValidateIssuer = true,
                    ValidAudience = Audience,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret)),
                };
            });

        return service;
    }
}
