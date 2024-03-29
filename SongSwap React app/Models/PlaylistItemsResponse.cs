using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    [Serializable]
    public class PlaylistItemsResponse
    {
        [JsonPropertyName("results")]
        public Track[] Items { get; set; } = Array.Empty<Track>();

        [JsonPropertyName("nextParam")]
        public string? NextParam { get; set; }

        [JsonPropertyName("totalItems")]
        public int? TotalItems { get; set; }
    }
}
