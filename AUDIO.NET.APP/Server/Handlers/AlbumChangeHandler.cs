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
        private readonly ILogger<AlbumChangeHandler> _logger;
        public AlbumChangeHandler(ISmartLedManager ledManager, ILogger<AlbumChangeHandler> logger)
        {
            _ledManager = ledManager;
            _logger = logger;
        }

        public async Task Handle(AlbumChangeNotification notification, CancellationToken cancellationToken)
        {
            try
            {

                var color = notification.NewColor;
                if (string.IsNullOrEmpty(color))
                {
                   // await _ledManager.ResetAll();
                }
                else
                {
                    await _ledManager.SetColorToAll(color);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorMessages.ERROR_WHILE_CONNECTING_DEVICES, ex);
            }

        }
    }
}
