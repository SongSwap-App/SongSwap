using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    public class SongResponse
    {
        [JsonPropertyName("results")]
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}
