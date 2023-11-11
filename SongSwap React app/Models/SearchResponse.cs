using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    [Serializable]
    public class SearchResponse
    {
        [JsonPropertyName("results")]
        public SearchItem[] Items { get; set; } = Array.Empty<SearchItem>();
    }

    [Serializable]
    public class SearchItem
    {
        [JsonPropertyName ("id")]
        public string Id { get; set; } = string.Empty;
    }
}
