using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    public class CreateResponse
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("id")]
        public string PlaylistId { get; set; } = string.Empty;
    }
}
