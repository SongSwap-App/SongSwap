using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SongSwap_React_app.Startup
{
    public static class JwtConfiguration
    {
        public static IServiceCollection AddConfiguredJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var secretClient = new SecretClient(vaultUri: new Uri(configuration.GetValue<string>("KeyVault")), credential: new DefaultAzureCredential());

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetSecretKey(secretClient)))
                };
            });

            return services;
        }

        private static string GetSecretKey(SecretClient? secretClient = null)
        {
            const string DOCKER_SECRET_PATH = "/run/secrets/";
            if (Directory.Exists(DOCKER_SECRET_PATH))
            {
                IFileProvider provider = new PhysicalFileProvider(DOCKER_SECRET_PATH);
                IFileInfo fileInfo = provider.GetFileInfo("JwtSecretKey");
                if (fileInfo.Exists)
                {
                    using (var stream = fileInfo.CreateReadStream())
                    using (var streamReader = new StreamReader(stream))
                    {
                        var content = streamReader.ReadToEnd();
                        return content;
                    }
                }
            }

            if (secretClient != null)
            {
                KeyVaultSecret secret = secretClient.GetSecret("JWTSecret").Value;
                return secret.Value;
            }

            return "superSecretKey@345ssdssssdsdssdsdsds";
        }
    }
}
