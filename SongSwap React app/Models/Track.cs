using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    public class Track
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("isrc")]
        public string? Isrc { get; set; } = string.Empty;

        [JsonPropertyName("artists")]
        public Artist[] Artists { get; set; } = Array.Empty<Artist>();
    }
}