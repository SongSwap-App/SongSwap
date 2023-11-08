using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SongSwap_React_app.Models;
using SongSwap_React_app.Models.Services;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SongSwap_React_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class PlaylistController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly ILogger<PlaylistController> _logger;

        public PlaylistController(ILogger<PlaylistController> logger, AuthenticationService authenticationService)
        {
            _logger = logger;
            _authenticationService = authenticationService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetPlaylistsByUserUUID(string userId) 
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{userId}/playlists/");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Basic " + _authenticationService.GetBasic64Authentication());
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode) 
            {
                var content = await response.Content.ReadAsStringAsync();
                var playlists = JsonSerializer.Deserialize<PlaylistsResponse>(content);
                return Ok(playlists);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized("Reathorization required");
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
