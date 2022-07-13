using Newtonsoft.Json;

namespace AUDIO.NET.APP.Server.Utils
{
    public class Configuration
    {
        public SpotifyConfiguration spotify;
        public TuyaConfiguration tuya;

    }
    public struct SpotifyConfiguration
    {
        public string redirectUrl;
        public string clientId;
        public string clientSecret;
    }
    public struct TuyaConfiguration
    {
        public string accessId;
        public string apiSecret;
        public string anyDeviceId;
    }
}
