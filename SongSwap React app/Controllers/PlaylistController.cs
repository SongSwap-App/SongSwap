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

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            string playlistId = "PLTOafDscBbuxUBpBAow7O9CMYVknfhVP4";
            string userId = "c854382e-1952-4371-8812-7085144334cc";
            
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{userId}/playlists/" + playlistId);
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Basic " + _authenticationService.GetBasic64Authentication());
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsStringAsync();
            Playlist playlist = JsonSerializer.Deserialize<Playlist>(data)!;
            var itemsRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{userId}/playlists/" + playlistId + "/items");
            itemsRequest.Headers.Add("Accept", "application/json");
            itemsRequest.Headers.Add("Authorization", "Basic " + _authenticationService.GetBasic64Authentication());
            var itemsResponse = await client.SendAsync(itemsRequest);
            itemsResponse.EnsureSuccessStatusCode();
            var itemsData = await itemsResponse.Content.ReadAsStringAsync();
            playlist.Items = JsonSerializer.Deserialize<SongResponse>(itemsData)!.Songs;
            return Ok(playlist);
        }

        [HttpGet("2")]
        public async Task<IActionResult> TestMusicAPI()
        {
            var clientId = "c854382e-1952-4371-8812-7085144334cc";
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.musicapi.com/api/" + clientId + "/playlists");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("Authorization", "Basic " + _authenticationService.GetBasic64Authentication());
            var responce = await client.SendAsync(request);
            responce.EnsureSuccessStatusCode();
            return Ok(responce.Content.ReadAsStringAsync());
        }

    }
}
