using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    public class SongResponse
    {
        [JsonPropertyName("results")]
        public List<Track> Songs { get; set; } = new List<Track>();
    }
}
