using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SongSwap_React_app.Models.Services;
using System.Text;

namespace SongSwap_React_app.Startup
{
    public static class JwtConfiguration
    {
        public static IServiceCollection AddConfiguredJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var _authService = services.BuildServiceProvider().GetService<AuthorizationService>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration.GetValue<string>("ApplicationURL"),
                    ValidAudience = configuration.GetValue<string>("ClientURL"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authService.GetJwtSecret()))
                };
            });

            return services;
        }
    }
}
