using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SongSwap_React_app.Infrastructure;
using SongSwap_React_app.Models;
using SongSwap_React_app.Models.Services;
using System.Buffers.Text;
using System.Net;
using System.Numerics;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace SongSwap_React_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    [Authorize]
    public class PlaylistController : ControllerBase
    {
        private readonly AuthorizationService _authorizationService;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string musicapi_url = "https://api.musicapi.com/api";
        private readonly IHubContext<ProgressHub> _hub;

        public PlaylistController(AuthorizationService authorizationService, IHttpClientFactory httpClientFactory, IHubContext<ProgressHub> hub)
        {
            _authorizationService = authorizationService;
            _httpClientFactory = httpClientFactory;
            _hub = hub;
        }


        [HttpGet()]
        public async Task<IActionResult> GetPlaylists()
        {
            string? userId = User.FindFirstValue("SourceIntegrationId");
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("Couldn`t get value from token");
                return BadRequest("Couldn`t get value from token");
            }
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{musicapi_url}/{userId}/playlists");
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
            string? userId = User.FindFirstValue("SourceIntegrationId");
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + _authorizationService.GetBasic64Authentication());
            var request = new HttpRequestMessage(HttpMethod.Get, $"{musicapi_url}/{userId}/playlists/{playlistId}/items");
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonSerializer.Deserialize<PlaylistItemsResponse>(content);
                return Ok(items!.Items);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("import/{playlistId}")]
        public async Task<IActionResult> ImportPlaylist(string playlistId, CancellationToken cancellationToken)
        {
            await _hub.Clients.All.SendAsync("ReceiveMessage", playlistId, new ProgressDto { Status = "started", Now = 0 }, cancellationToken: cancellationToken);

            string? sourceId = User.FindFirstValue("SourceIntegrationId");
            string? destinationId = User.FindFirstValue("DestIntegrationId");

            if (string.IsNullOrEmpty(sourceId) || string.IsNullOrEmpty(destinationId))
            {
                return Unauthorized();
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("Authorization", "Basic " + _authorizationService.GetBasic64Authentication());

            var sourceRequest = new HttpRequestMessage(HttpMethod.Get, $"{musicapi_url}/{sourceId}/playlists/{playlistId}/items");
            var sourceResponce = await client.SendAsync(sourceRequest, cancellationToken);
            var sourceContent = await sourceResponce.Content.ReadAsStringAsync(cancellationToken);
            var source = JsonSerializer.Deserialize<PlaylistItemsResponse>(sourceContent);

            if (source == null)
            {
                return NotFound();
            }

            while (source.Items.Length != source.TotalItems)
            {
                var expandSourceRequest = new HttpRequestMessage(HttpMethod.Get, $"{musicapi_url}/{sourceId}/playlists/{playlistId}/items?nextParam={source.NextParam}");
                var expandResponse = await client.SendAsync(expandSourceRequest, cancellationToken);
                var expandContent = await expandResponse.Content.ReadAsStringAsync(cancellationToken);
                var expand = JsonSerializer.Deserialize<PlaylistItemsResponse>(expandContent);

                if (expand == null)
                {
                    return NotFound();
                }

                source.Items = source.Items.Concat(expand.Items).ToArray();
                source.NextParam = expand.NextParam;
            }


            var nameRequest = new HttpRequestMessage(HttpMethod.Get, $"{musicapi_url}/{sourceId}/playlists/{playlistId}");
            var nameResponce = await client.SendAsync(nameRequest, cancellationToken);
            var name = JsonNode.Parse(await nameResponce.Content.ReadAsStringAsync(cancellationToken))!["name"]!.ToString();

            List<string> itemIds = new();
            int count = 0;

            foreach (var item in source.Items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var artist = item.Artists?.FirstOrDefault()?.Name;
                var searchRequest = new HttpRequestMessage(HttpMethod.Post, $"{musicapi_url}/{destinationId}/search");
                searchRequest.Options.Set(new HttpRequestOptionsKey<int>("limitParam"), 1);
                var searchRequestBody = new List<KeyValuePair<string, string>>()
                {
                    new("type", "track"),
                    new("track", item.Name),
                    new("artist", artist ?? string.Empty),
                    new("album", string.Empty),
                    new("isrc",  string.Empty)
                };
                searchRequest.Content = new FormUrlEncodedContent(searchRequestBody);

                var searchResponce = await client.SendAsync(searchRequest, cancellationToken);

                var content = await searchResponce.Content.ReadAsStringAsync(cancellationToken);
                var searchResult = JsonSerializer.Deserialize<PlaylistItemsResponse>(content);

                if (searchResult == null || searchResult.TotalItems == 0)
                {
                    continue;
                }

                count++;
                itemIds.Add(searchResult.Items[0].Id);
                await _hub.Clients.All.SendAsync("ReceiveMessage", playlistId,
                    new ProgressDto { Status = "search", Now = (int)Math.Round((double)(100 * count) / source.Items.Length) },
                    cancellationToken: cancellationToken);
            }

            await _hub.Clients.All.SendAsync("ReceiveMessage", playlistId, new ProgressDto { Status = "importing", Now = 100 }, cancellationToken: cancellationToken);
            var createRequest = new HttpRequestMessage(HttpMethod.Post, $"{musicapi_url}/{destinationId}/playlists");
            var createRequestBody = new List<KeyValuePair<string, string>>
            {
                new("name", name)
            };
            createRequest.Content = new FormUrlEncodedContent(createRequestBody);
            var createResponse = await client.SendAsync(createRequest, cancellationToken);
            createResponse.EnsureSuccessStatusCode();
            var createResponseBody = await createResponse.Content.ReadAsStringAsync(cancellationToken);
            var newPlaylist = JsonSerializer.Deserialize<Playlist>(createResponseBody);

            var populateRequest = new HttpRequestMessage(HttpMethod.Post, $"{musicapi_url}/{destinationId}/playlists/{newPlaylist!.Id}/items");
            var populateRequestBody = new List<KeyValuePair<string, string>>();

            foreach (var itemId in itemIds)
            {
                populateRequestBody.Add(new KeyValuePair<string, string>("itemIds[]", itemId));
            }
            populateRequest.Content = new FormUrlEncodedContent(populateRequestBody);
            var populateResponse = await client.SendAsync(populateRequest, cancellationToken);
            populateResponse.EnsureSuccessStatusCode();

            await _hub.Clients.All.SendAsync("ReceiveMessage", playlistId, new ProgressDto { Status = "done", Now = 100 }, cancellationToken);
            return Ok();
        }
    }
}
