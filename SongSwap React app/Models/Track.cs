﻿using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    public class Track
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}