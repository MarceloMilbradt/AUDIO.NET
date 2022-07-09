using AUDIO.NET.APP.Server.Services;
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
        private readonly ISpotify _spotifyListener;

        public PlayerController(ILogger<PlayerController> logger, ISpotify listener)
        {
            _logger = logger;
            _spotifyListener = listener;
        }
        [HttpGet("Features")]
        public async Task<TrackAudioFeatures?> GetFeatures()
        {
            try
            {
                var features = await _spotifyListener.GetAudioFeatures();
                return features;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting features, {ex}", ex);
                return null;
            }
        }
        [HttpGet("Track")]
        public async Task<FullTrack?> GetTrack()
        {
            try
            {
                var track = await _spotifyListener.GetTrack();
                return track;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting Track, {ex}", ex);
                return null;
            }
        }
    }
}
