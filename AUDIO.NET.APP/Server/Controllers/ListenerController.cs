using AUDIO.NET.APP.Server.Hubs;
using AUDIO.NET.APP.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace AUDIO.NET.APP.Server.Controllers
{
    [Route("Listen")]
    [ApiController]
    public class ListenerController : ControllerBase
    {
        private readonly ILogger<ListenerController> _logger;
        private readonly ISpotify _spotifyListener;
        private readonly IHubContext<TrackHub> _trackHub;
        public ListenerController(ILogger<ListenerController> logger, ISpotify listener, IHubContext<TrackHub> trackHub)
        {
            _logger = logger;
            _spotifyListener = listener;
            _trackHub = trackHub;
        }

        [HttpGet]
        public IActionResult Listen()
        {
            using (_logger.BeginScope("Listening to Spotify"))
            {
                try
                {
                    if(_spotifyListener.IsListening())
                        return Ok();
                   _spotifyListener.Start(newTrack => TrackHub.SendTrackTo(_trackHub.Clients.All, newTrack.Result));
                }
                catch (Exception ex)
                {
                    _logger.LogError("Eror whule listening to Spotify, {ex}", ex);
                }
                return Ok();
            }
        }
    }
}
