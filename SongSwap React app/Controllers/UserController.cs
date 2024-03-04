using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SongSwap_React_app.Infrastructure;
using SongSwap_React_app.Models;
using SongSwap_React_app.Models.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SongSwap_React_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class UserController : ControllerBase
    {
        private const string homepage = "http://localhost:3000/";
        private readonly AuthorizationService _authorizationService;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(AuthorizationService authorizationService, IHttpClientFactory httpClientFactory)
        {
            _authorizationService = authorizationService;
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> GetUserData()
        {
            string? integrationId = User.FindFirstValue("SourceIntegrationId");
            string? sourcePlatform = User.FindFirstValue("SourcePlatform");
            string? destPlatform = User.FindFirstValue("DestinationPlatform");

            if (string.IsNullOrEmpty(integrationId) || string.IsNullOrEmpty(sourcePlatform) || string.IsNullOrEmpty(destPlatform))
            {
                Console.WriteLine("Couldn`t get token values");
                return Unauthorized("IntegrationId is not found");
            }
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{integrationId}/users/profile");
            request.Headers.Add("Authorization", "Basic " + _authorizationService.GetBasic64Authentication());
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                User user = JsonSerializer.Deserialize<User>(data)!;
                user.Id = integrationId;
                user.Source = sourcePlatform;
                user.Destination = destPlatform;

                return Ok(user);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized("Reauthorization required");
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult LogOut()
        {
            var ck = Request.Cookies.Keys;
            foreach (string cookie in ck)
            {
                Response.Cookies.Delete(cookie);
            }
            Console.WriteLine("Logged out");
            return Ok();
        }

        [HttpGet("callback/source")]
        public IActionResult Callback(string data64, string dest)
        {
            if (string.IsNullOrEmpty(data64))
            {
                return BadRequest();
            }
            byte[] data = Convert.FromBase64String(data64);
            string decodedString = System.Text.Encoding.UTF8.GetString(data);
            var node = JsonNode.Parse(decodedString)!;

            Response.Cookies.Append("SourceIntegrationId", node["integrationUserUUID"]!.ToString());
            Response.Cookies.Append("SourcePlatform", node["integration"]!["type"]!.ToString());
            Response.Cookies.Append("DestinationPlatform", dest);


            return Redirect($"https://app.musicapi.com/songswap/{dest}/auth?returnUrl=https://localhost:5000/api/user/callback/destination");
        }

        [HttpGet("callback/destination")]
        public IActionResult CallbackDestination(string data64)
        {
            if (string.IsNullOrEmpty(data64))
            {
                return BadRequest();
            }
            byte[] data = Convert.FromBase64String(data64);
            string decodedString = Encoding.UTF8.GetString(data);
            var node = JsonNode.Parse(decodedString)!;

            Response.Cookies.Append("DestIntegrationId", node["integrationUserUUID"]!.ToString());
            Request.Cookies.TryGetValue("SourceIntegrationId", out string? integrationId);
            Request.Cookies.TryGetValue("SourcePlatform", out string? sourcePlatform);
            Request.Cookies.TryGetValue("DestinationPlatform", out string? destPlatform);

            if (string.IsNullOrEmpty(integrationId) || string.IsNullOrEmpty(sourcePlatform) || string.IsNullOrEmpty(destPlatform))
            {
                return Unauthorized("IntegrationId is not found");
            }

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("superSecretKey@345ssdssssdsdssdsdsds"));
            var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var tokeOptions = new JwtSecurityToken(
                issuer: "https://localhost:5000",
                audience: "http://localhost:3000",
                claims: new List<Claim>()
                {
                    new("SourceIntegrationId", integrationId),
                    new("SourcePlatform", sourcePlatform),
                    new("DestinationPlatform", destPlatform),
                    new("DestIntegrationId", node["integrationUserUUID"]!.ToString())
                },
                signingCredentials: signinCredentials,
                expires: DateTime.Now.AddHours(2)
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);

            return Redirect(homepage + "callback?token=" + tokenString);
        }
    }
}
