using AUDIO.NET.APP.Shared.Interfaces;
using SpotifyAPI.Web;

namespace AUDIO.NET.APP.Shared
{
    public class SpotifyListener : ISpotify
    {
        private SpotifyClient? _spotify;
        string _url;
        string _clientId;
        string _clientSecret;
        string _currentUlr = string.Empty;
        bool isListening;
        public bool IsListening() => isListening;
        public SpotifyListener(string url, string clientId, string clientSecret)
        {
            _url = url;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }
        public Uri GetLoginUri()
        {
            var loginRequest = new LoginRequest(new Uri(_url), _clientId, LoginRequest.ResponseType.Code)
            {
                Scope = new[] { Scopes.UserReadCurrentlyPlaying, Scopes.UserReadPrivate }
            };
            var uri = loginRequest.ToUri();
            return uri;
        }
        public async Task Connect(string code)
        {
            if (isListening)
                return;

            Uri uri = new Uri(_url);
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(_clientId, _clientSecret, code, uri)
                );
            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new AuthorizationCodeAuthenticator(_clientId, _clientSecret, response));
            _spotify = new SpotifyClient(config);
        }
        public async Task Start()
        {
            if (isListening)
                return;
            isListening = true;
            while (true)
            {
                await Listen();
                Thread.Sleep(500);
            }
        }
        private async Task<FullTrack> GetCurrentTrack()
        {
            var currentlyPlaying = await _spotify.Player.GetCurrentlyPlaying(CreateRequest());
            if (currentlyPlaying == null)
            {
                return null;
            }
            var track = (FullTrack)currentlyPlaying.Item;
            return track;
        }

        public async Task<TrackAudioFeatures> GetAudioFeatures()
        {
            var track = await GetCurrentTrack();
            var features = await _spotify.Tracks.GetAudioFeatures(track.Id);
            return features;
        }
        public async Task<PrivateUser> GetCurrentUser()
        {
            var user = await _spotify.UserProfile.Current();
            return user;
        }
        public async Task<FullTrack> GetTrack()
        {
            return await GetCurrentTrack();
        }
        public async Task Listen()
        {
            try
            {
                if (_spotify == null) return;

                var currentlyPlaying = await GetCurrentTrack();
                if (currentlyPlaying == null)
                {
                    DeviceConnector.ResetColor();
                    return;
                }
                var url = currentlyPlaying.Album.ExternalUrls["spotify"];
                if (_currentUlr == url) return;
                _currentUlr = url;
                var color = ColorScraper.ScrapeColorForAlbum(url);
                var hsvColor = ColorScraper.ConvertToHSV(color);

                DeviceConnector.ChangeColor(hsvColor);
            }
            catch (Exception ex)
            {
                return;
            }
        }
        private static PlayerCurrentlyPlayingRequest CreateRequest()
        {
            return new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track);
        }
    }
}