using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    [Serializable]
    public class Playlist
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName ("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("totalItems")]
        public int TotalItems { get; set; }

        public List<Track> Items { get; set; } = new List<Track>();
    }
}
