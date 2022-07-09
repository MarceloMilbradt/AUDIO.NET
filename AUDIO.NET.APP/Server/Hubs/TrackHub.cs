using AUDIO.NET.APP.Server.Services;
using AUDIO.NET.APP.Shared;
using Microsoft.AspNetCore.SignalR;

namespace AUDIO.NET.APP.Server.Hubs
{
    public class TrackHub : Hub
    {
        ISpotify _spotify;

        public TrackHub(IServiceProvider serviceProvider)
        {
            _spotify = serviceProvider.GetRequiredService<ISpotify>();
        }

        public override async Task OnConnectedAsync()
        {
            await SendTrack(await _spotify.GetFullInfo());
            await base.OnConnectedAsync();
        }
        public static async Task SendTrackTo(IClientProxy clients, TrackDTO track)
        {
            await clients.SendAsync("TrackChange", track);
        }
        public async Task SendTrack(TrackDTO track)
        {
            await SendTrackTo(Clients.Caller, track);
        }
    }
}
