using AUDIO.NET.APP.Server.Hubs;
using AUDIO.NET.APP.Server.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using SmartLedKit;

namespace AUDIO.NET.APP.Server.Handlers
{
    public class TrackChangeHandler : INotificationHandler<TrackChangeNotification>
    {
        private readonly IHubContext<TrackHub> _trackHub;
        public TrackChangeHandler(IHubContext<TrackHub> trackHub)
        {
            _trackHub = trackHub;
        }

        public async Task Handle(TrackChangeNotification notification, CancellationToken cancellationToken)
        {
           await TrackHub.SendTrackTo(_trackHub.Clients.All, notification.TrackInfo);
        }
    }
}
