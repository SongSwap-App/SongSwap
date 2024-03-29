using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    [Serializable]
    public class SearchResponse
    {
        [JsonPropertyName("results")]
        public SearchItem[] Items { get; set; } = Array.Empty<SearchItem>();

        [JsonPropertyName("nextParam")]
        public string? NextParam { get; set; }

        [JsonPropertyName("totalItems")]
        public int? TotalItems { get; set; }
    }

    [Serializable]
    public class SearchItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
}
