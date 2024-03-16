using Microsoft.AspNetCore.SignalR;
using SongSwap_React_app.Models;

namespace SongSwap_React_app.Infrastructure
{
    public class ProgressHub : Hub
    {
        public async Task SendMessage(string playlistId, ProgressDto progress)
        {
            await Clients.All.SendAsync("ReceiveMessage", playlistId, progress);
        }
    }
}
