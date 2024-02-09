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
            var plaintAuthorizationText = System.Text.Encoding.UTF8.GetBytes(_configuration["MusicApi_ClientId"] + ":" + _configuration["MusicApi_ClientSecret"]);
            var base64Text = System.Convert.ToBase64String(plaintAuthorizationText);
            return base64Text;
        }
    }
}
