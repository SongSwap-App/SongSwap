using System.Text.Json.Serialization;

namespace SongSwap_React_app.Models
{
    [Serializable]
    public class PlaylistsResponse
    {
        [JsonPropertyName("results")]
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();
    }
}
