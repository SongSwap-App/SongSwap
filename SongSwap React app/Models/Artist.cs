using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    [Serializable]
    public class Artist
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
    }
}
