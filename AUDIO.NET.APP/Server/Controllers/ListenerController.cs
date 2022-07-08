using AUDIO.NET.APP.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AUDIO.NET.APP.Server.Controllers
{
    [Route("Listen")]
    [ApiController]
    public class ListenerController : ControllerBase
    {
        private readonly ILogger<ListenerController> _logger;
        private readonly ISpotify _spotifyListener;

        public ListenerController(ILogger<ListenerController> logger, ISpotify listener)
        {
            _logger = logger;
            _spotifyListener = listener;
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
                   _spotifyListener.Start();
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
