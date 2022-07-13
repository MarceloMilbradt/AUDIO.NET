using AUDIO.NET.APP.Server.Services;
using AUDIO.NET.APP.Server.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace AUDIO.NET.APP.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> _logger;
        private readonly ISpotifyAPI _spotify;

        public PlayerController(ILogger<PlayerController> logger, ISpotifyAPI spotify)
        {
            _logger = logger;
            _spotify = spotify;
        }
        [HttpGet("Features")]
        public async Task<TrackAudioFeatures?> GetFeatures()
        {
            try
            {
                var features = await _spotify.GetAudioFeatures();
                return features;
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorMessages.ERROR_WHILE_GETTING_VALUE, ex);
                return null;
            }
        }
        [HttpGet("Track")]
        public async Task<FullTrack?> GetTrack()
        {
            try
            {
                var track = await _spotify.GetTrack();
                return track;
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorMessages.ERROR_WHILE_GETTING_VALUE, ex);
                return null;
            }
        }
    }
}
