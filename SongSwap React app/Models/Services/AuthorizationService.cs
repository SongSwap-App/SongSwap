using Microsoft.Extensions.FileProviders;

namespace SongSwap_React_app.Models.Services
{
    public class AuthorizationService
    {
        private readonly IConfiguration _configuration;

        public AuthorizationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetBasic64Authentication()
        {
            var plaintAuthorizationText = System.Text.Encoding.UTF8.GetBytes(GetSecretOrEnvVar("MusicApi_ClientId") + ":" + GetSecretOrEnvVar("MusicApi_ClientSecret"));
            var base64Text = System.Convert.ToBase64String(plaintAuthorizationText);
            return base64Text;
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

            return _configuration.GetValue<string>(key);
        }
    }
}
