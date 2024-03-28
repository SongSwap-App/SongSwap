using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.FileProviders;

namespace SongSwap_React_app.Models.Services
{
    public class AuthorizationService
    {
        private readonly IConfiguration _configuration;
        private readonly SecretClient? _secretClient;
        private string? clientId = null;
        private string? clientSecret = null;
        private string? jwtSecret = null;

        public AuthorizationService(IConfiguration configuration)
        {
            _configuration = configuration;
            try
            {
                _secretClient = new SecretClient(vaultUri: new Uri(configuration.GetValue<string>("KeyVault")), credential: new DefaultAzureCredential());
            }
            catch
            {
                _secretClient = null;
            }

        }

        public string GetBasic64Authentication()
        {
            if (string.IsNullOrEmpty(clientId))
            {
                clientId = GetSecretOrEnvVar("MusicApiClientId");
            }
            if (string.IsNullOrEmpty(clientSecret))
            {
                clientSecret = GetSecretOrEnvVar("MusicApiClientSecret");
            }

            var plaintAuthorizationText = System.Text.Encoding.UTF8.GetBytes(clientId + ":" + clientSecret);
            var base64Text = Convert.ToBase64String(plaintAuthorizationText);
            return base64Text;
        }

        public string GetJwtSecret()
        {
            if (string.IsNullOrEmpty(jwtSecret))
            {
                jwtSecret = GetSecretOrEnvVar("JWTSecret");
            }

            return jwtSecret;
        }

        private string GetSecretOrEnvVar(string key)
        {
            const string DOCKER_SECRET_PATH = "/run/secrets/";
            if (Directory.Exists(DOCKER_SECRET_PATH))
            {
                IFileProvider provider = new PhysicalFileProvider(DOCKER_SECRET_PATH);
                IFileInfo fileInfo = provider.GetFileInfo(key);
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

            if (_secretClient != null)
            {
                KeyVaultSecret secret = _secretClient.GetSecret(key).Value;
                string value = secret.Value;

                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }

            return _configuration.GetValue<string>(key);
        }
    }
}
