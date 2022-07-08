using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AUDIO.NET.APP.Shared.Interfaces;

namespace AUDIO.NET.APP.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly ISpotify _spotifyListener;

        public AuthController(ILogger<AuthController> logger, ISpotify listener)
        {
            _logger = logger;
            _spotifyListener = listener;
        }

        [HttpGet("token")]
        public async Task<IActionResult> Token(string code)
        {
            using (_logger.BeginScope("Connecting to Spotify"))
            {
                try
                {
                    await _spotifyListener.Connect(code);
                    _logger.LogInformation("Connected to Spotify");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unnable to Connecto to Spotify, {ex}", ex);
                }
                return new RedirectResult("/");
            }
        }


        [HttpGet("login")]
        public IActionResult Login()
        {
            return new RedirectResult(_spotifyListener.GetLoginUri().ToString(), true);
        }
    }
}
