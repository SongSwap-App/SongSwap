namespace SongSwap_React_app.Models.Services
{
    public class AuthenticationService
    {
        private readonly IConfiguration _configuration;

        public AuthenticationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetBasic64Authentication()
        {
            var plaintAuthorizationText = System.Text.Encoding.UTF8.GetBytes(_configuration["MusicApi:ClientId"] + ":" + _configuration["MusicApi:ClientSecret"]);
            var base64Text = System.Convert.ToBase64String(plaintAuthorizationText);
            return base64Text;
        }
    }
}
