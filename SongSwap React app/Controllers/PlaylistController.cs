using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SongSwap_React_app.Infrastructure;
using SongSwap_React_app.Models;
using SongSwap_React_app.Models.Services;
using System.Buffers.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SongSwap_React_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class PlaylistController : ControllerBase
    {
        private readonly AuthorizationService _authorizationService;
        private readonly IHttpClientFactory _httpClientFactory;

        public PlaylistController(AuthorizationService authorizationService, IHttpClientFactory httpClientFactory)
        {
            _authorizationService = authorizationService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet()]
        public async Task<IActionResult> GetPlaylistsByUserUUID()
        {
            Request.Cookies.TryGetValue("SourceIntegrationId", out string? userId);
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("Couldn`t get value from cookies");
                return BadRequest("Couldn`t get value from cookies");
            }
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{userId}/playlists");
            request.Headers.Add("Authorization", "Basic " + _authorizationService.GetBasic64Authentication());
            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return Unauthorized("Reathorization required");
            }
            else if (!response.IsSuccessStatusCode)
            {
                return BadRequest();
            }

            var content = await response.Content.ReadAsStringAsync();
            var playlists = JsonSerializer.Deserialize<PlaylistsResponse>(content);
            
            if (playlists == null)
            {
                return NotFound();
            }

            return Ok(playlists.Playlists);
        }

        [HttpGet("{playlistId}")]
        public async Task<IActionResult> GetPlaylistItems(string playlistId)
        {
            Request.Cookies.TryGetValue("SourceIntegrationId", out string? userId);
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + _authorizationService.GetBasic64Authentication());
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{userId}/playlists/{playlistId}/items");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<SearchResponse>(content);
                return Ok(items!.Items);
            }
            else 
            {
                return BadRequest(); 
            }
        }

        [HttpPost("import/{playlistId}")]
        public async Task<IActionResult> CreatePlaylist(string playlistId)
        {
            Request.Cookies.TryGetValue("SourceIntegrationId", out string? sourceId);
            Request.Cookies.TryGetValue("DestIntegrationId", out string? destinationId);

            if (string.IsNullOrEmpty(sourceId) || string.IsNullOrEmpty(destinationId))
            {
                return Unauthorized();
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + _authorizationService.GetBasic64Authentication());

            var sourceRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{sourceId}/playlists/{playlistId}/items");
            var sourceResponce = await client.SendAsync(sourceRequest);
            var sourceContent = await sourceResponce.Content.ReadAsStringAsync();
            var source = JsonSerializer.Deserialize<SearchResponse>(sourceContent);

            if (source == null)
            {
                return NotFound();
            }


            var nameRequest = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{sourceId}/playlists/{playlistId}");
            var nameResponce = await client.SendAsync(nameRequest);
            var name = JsonNode.Parse(await nameResponce.Content.ReadAsStringAsync())!["name"]!.ToString();

            List<string> itemIds = new();

            foreach (var item in source.Items)
            {
                var searchRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.musicapi.com/api/{destinationId}/search");
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

                if (searchResult == null)
                {
                    continue;
                }

                itemIds.Add(searchResult.Items[0].Id);
            }
            Console.WriteLine(itemIds.Count);

            var createRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.musicapi.com/api/{destinationId}/playlists");
            var createRequestBody = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name", name)
            };
            createRequest.Content = new FormUrlEncodedContent(createRequestBody);
            var createResponse = await client.SendAsync(createRequest);
            createResponse.EnsureSuccessStatusCode();
            var createResponseBody = await createResponse.Content.ReadAsStringAsync();
            var newPlaylist = JsonSerializer.Deserialize<Playlist>(createResponseBody);

            var populateRequest = new HttpRequestMessage(HttpMethod.Post, $"https://api.musicapi.com/api/{destinationId}/playlists/{newPlaylist!.Id}/items");
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
