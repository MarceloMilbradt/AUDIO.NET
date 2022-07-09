using SpotifyAPI.Web;

namespace AUDIO.NET.APP.Server.Services.Implementation
{
    public class SpotifyBase
    {
        protected readonly string _url;
        protected readonly string _clientId;
        protected readonly string _clientSecret;
        protected private SpotifyClient? _spotify;

        public SpotifyBase(string url, string clientId, string clientSecret)
        {
            _url = url;
            _clientId = clientId;
            _clientSecret = clientSecret;
        }

        private static PlayerCurrentlyPlayingRequest CreateRequest()
        {
            return new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track);
        }
        public async Task Connect(string code)
        {
            if (_spotify != null)
                return;

            Uri uri = new(_url);
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(_clientId, _clientSecret, code, uri)
                );
            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new AuthorizationCodeAuthenticator(_clientId, _clientSecret, response));
            _spotify = new SpotifyClient(config);

        }

        public async Task<TrackAudioFeatures?> GetAudioFeatures()
        {
            if (_spotify is null) return null;
            var track = await GetCurrentTrack();
            if (track is null) return null;
            return await GetFeatures(track);
        }

        public async Task<TrackAudioFeatures> GetFeatures(FullTrack? track)
        {
            return await _spotify.Tracks.GetAudioFeatures(track.Id);
        }
        public async Task<TrackAudioAnalysis> GetAnalysis(FullTrack? track)
        {
            return await _spotify.Tracks.GetAudioAnalysis(track.Id);
        }
        public async Task<PrivateUser?> GetCurrentUser()
        {
            if (_spotify is null) return null;
            var user = await _spotify.UserProfile.Current();
            return user;
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
        public async Task<FullTrack?> GetTrack()
        {
            return await GetCurrentTrack();
        }

        protected async Task<FullTrack?> GetCurrentTrack()
        {
            if (_spotify is null) return null;

            var currentlyPlaying = await _spotify.Player.GetCurrentlyPlaying(CreateRequest());
            if (currentlyPlaying == null)
            {
                return null;
            }
            var track = (FullTrack)currentlyPlaying.Item;
            return track;
        }
    }
}