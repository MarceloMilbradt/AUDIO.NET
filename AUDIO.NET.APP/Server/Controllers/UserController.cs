using AUDIO.NET.APP.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace AUDIO.NET.APP.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ISpotify _spotifyListener;

        public UserController(ILogger<UserController> logger, ISpotify listener)
        {
            _logger = logger;
            _spotifyListener = listener;
        }
        [HttpGet]
        public async Task<PrivateUser?> GetFeatures()
        {
            try
            {
                var user = await _spotifyListener.GetCurrentUser();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(@"Error getting user, {ex}", ex);
                return null;
            }
        }
    }
}
