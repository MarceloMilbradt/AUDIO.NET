using AUDIO.NET.APP.Shared;
using MediatR;
using SpotifyAPI.Web;

namespace AUDIO.NET.APP.Server.Notifications
{
    public class TrackChangeNotification : INotification
    {
        public TrackDTO TrackInfo { get; }

        public TrackChangeNotification( TrackDTO track)
        {
            TrackInfo = track;
        }
    }
}
