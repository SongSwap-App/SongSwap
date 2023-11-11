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
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{userId}/playlists");
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

        [HttpPost("{userId}")]
        public async Task<IActionResult> CreatePlaylist(string userId, [FromBody] Playlist source)
        {
            if (userId == null || source == null)
            {
                return BadRequest("Invalid user ID");
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + _authenticationService.GetBasic64Authentication());

            List<string> itemIds = new();

            foreach (var item in source.Items)
            {
                var searchRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.musicapi.com/api/{userId}/search");
                searchRequest.Options.Set(new HttpRequestOptionsKey<int>("limitParam"), 1);
                var searchRequestBody = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("type", "track"),
                    new KeyValuePair<string, string>("track", item.Name),
                    new KeyValuePair<string, string>("artist", string.Empty),
                    new KeyValuePair<string, string>("album", string.Empty),
                    new KeyValuePair<string, string>("isrc", string.Empty)
                };
                searchRequest.Content = new FormUrlEncodedContent(searchRequestBody);

                var searchResponce = await client.SendAsync(searchRequest);

                var content = await searchResponce.Content.ReadAsStringAsync();
                var searchResult = JsonSerializer.Deserialize<SearchResponse>(content);

                if (searchResult != null)
                {
                    itemIds.Add(searchResult.Items[0].Id);
                }
            }

            var createRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.musicapi.com/api/{userId}/playlists");
            var createRequestBody = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name", source.Name)
            };
            createRequest.Content = new FormUrlEncodedContent(createRequestBody);
            var createResponse = await client.SendAsync(createRequest);
            createResponse.EnsureSuccessStatusCode();
            var createResponseBody = await createResponse.Content.ReadAsStringAsync();
            var newPlaylist = JsonSerializer.Deserialize<Playlist>(createResponseBody);
        

            var populateRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.musicapi.com/api/{userId}/playlists/{newPlaylist!.Id}/items");
            var populateRequestBody = new List<KeyValuePair<string, string>>();

            foreach (var itemId in itemIds)
            {
                populateRequestBody.Add(new KeyValuePair<string, string>("itemIds[]", itemId));
            }
            populateRequest.Content = new FormUrlEncodedContent(populateRequestBody);   
            var populateResponse = await client.SendAsync(populateRequest);
            populateResponse.EnsureSuccessStatusCode();
            

            return Ok();
        }
    }
}
