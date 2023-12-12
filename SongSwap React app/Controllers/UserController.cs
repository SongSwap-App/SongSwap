﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SongSwap_React_app.Models;
using SongSwap_React_app.Models.Services;
using System.Text.Json;

namespace SongSwap_React_app.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigin")]
    public class UserController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        private readonly IHttpClientFactory _httpClientFactory;

        public UserController(AuthenticationService authenticationService, IHttpClientFactory httpClientFactory)
        {
            _authenticationService = authenticationService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserData(string id)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.musicapi.com/api/{id}/users/profile");
            request.Headers.Add("Authorization", "Basic " + _authenticationService.GetBasic64Authentication());
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode) 
            {
                var data = await response.Content.ReadAsStringAsync();
                User user = JsonSerializer.Deserialize<User>(data)!;
                user.Id = id;
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
    }
}
