using AUDIO.NET.APP.Server.Services;
using AUDIO.NET.APP.Server.Utils;
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
        private readonly ISpotifyAPI _spotify;

        public UserController(ILogger<UserController> logger, ISpotifyAPI spotify)
        {
            _logger = logger;
            _spotify = spotify;
        }
        [HttpGet]
        public async Task<PrivateUser?> GetFeatures()
        {
            try
            {
                var user = await _spotify.GetCurrentUser();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorMessages.ERROR_WHILE_GETTING_VALUE, ex);
                return null;
            }
        }
    }
}
