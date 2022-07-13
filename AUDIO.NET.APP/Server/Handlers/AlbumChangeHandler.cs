using AUDIO.NET.APP.Server.Hubs;
using AUDIO.NET.APP.Server.Notifications;
using AUDIO.NET.APP.Server.Utils;
using MediatR;
using SmartLedKit;

namespace AUDIO.NET.APP.Server.Handlers
{
    public class AlbumChangeHandler : INotificationHandler<AlbumChangeNotification>
    {
        private readonly ISmartLedManager _ledManager;
        public AlbumChangeHandler(ISmartLedManager ledManager)
        {
            _ledManager = ledManager;
        }

        public async Task Handle(AlbumChangeNotification notification, CancellationToken cancellationToken)
        {
            var color = notification.NewColor;
            if (string.IsNullOrEmpty(color))
            {
               await _ledManager.ResetAll();
            }
            else
            {
                _ledManager.SetColorToAll(color);
            }

        }
    }
}
