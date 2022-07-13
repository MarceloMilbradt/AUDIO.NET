using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AUDIO.NET.APP.Server.Services;
using AUDIO.NET.APP.Server.Utils;

namespace AUDIO.NET.APP.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ISpotifyAPI _spotify;

        public AuthController(ILogger<AuthController> logger, ISpotifyAPI spotify)
        {
            _logger = logger;
            _spotify = spotify;
        }

        [HttpGet("token")]
        public async Task<IActionResult> Token(string code)
        {
            using (_logger.BeginScope(ErrorMessages.CONNECTING_TO_SPOTIFY))
            {
                try
                {
                    await _spotify.Connect(code);
                    _logger.LogInformation(ErrorMessages.CONNECTED_TO_SPOTIFY);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ErrorMessages.ERROR_WHILE_CONNECTING_SPOTIFY, ex);
                }
                return new RedirectResult("/");
            }
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            return new RedirectResult(_spotify.GetLoginUri().ToString(), true);
        }
    }
}
