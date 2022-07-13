using AUDIO.NET.APP.Server.Utils;
using AUDIO.NET.APP.Shared;
using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using SpotifyAPI.Web;
using System.Drawing;

namespace AUDIO.NET.APP.Server.Services.Implementation
{
    public class SpotifyApi : ISpotifyAPI
    {
        protected readonly string _url;
        protected readonly string _clientId;
        protected readonly string _clientSecret;
        protected private SpotifyClient? _spotify;
        private bool _isLoggedIn;
        IAppCache cache = new CachingService();
        private AuthorizationCodeTokenResponse _authorizationCodeTokenResponse;
        public SpotifyApi(Configuration configuration)
        {
            _url = configuration.spotify.redirectUrl;
            _clientId = configuration.spotify.clientId;
            _clientSecret = configuration.spotify.clientSecret;
            _authorizationCodeTokenResponse = FileManager.ReadFromJsonFile<AuthorizationCodeTokenResponse>("credentials.json");
            if (_authorizationCodeTokenResponse != null && !_authorizationCodeTokenResponse.IsExpired)
            {
                LogIn(_authorizationCodeTokenResponse);
            }
        }

        private static PlayerCurrentlyPlayingRequest CreateRequest()
        {
            return new PlayerCurrentlyPlayingRequest(PlayerCurrentlyPlayingRequest.AdditionalTypes.Track);
        }
        public bool IsUserLoggedIn()
        {
            return _isLoggedIn;
        }
        public async Task Connect(string code)
        {
            if (_spotify != null)
                return;

            Uri uri = new(_url);
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(_clientId, _clientSecret, code, uri)
                );

            LogIn(response);
        }

        private void LogIn(AuthorizationCodeTokenResponse response)
        {
            FileManager.WriteToJsonFile("credentials.json", response);
            var config = SpotifyClientConfig.CreateDefault()
                .WithAuthenticator(new AuthorizationCodeAuthenticator(_clientId, _clientSecret, response));

            _spotify = new SpotifyClient(config);
            _isLoggedIn = true;
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
            if (track is null) return null;
            return await _spotify.Tracks.GetAudioFeatures(track.Id);
        }
        public async Task<TrackAudioAnalysis> GetAnalysis(FullTrack? track)
        {
            if (track is null) return null;
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

        public async Task<FullTrack?> GetCurrentTrack()
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
        public async Task<CurrentlyPlaying?> GetCurrent()
        {
            if (_spotify is null) return null;

            var currentlyPlaying = await _spotify.Player.GetCurrentlyPlaying(CreateRequest());
            if (currentlyPlaying == null)
            {
                return null;
            }
            return currentlyPlaying;
        }
        public async Task<TrackDTO> GetFullInfo(FullTrack track)
        {
            try
            {
                return await cache.GetOrAddAsync(track.Id, async entry =>
                {
                    var feturesTask = GetFeatures(track);
                    var analysisTask = GetAnalysis(track);
                    var color = ColorScraper.ScrapeColorForAlbum(track.Album.ExternalUrls["spotify"]);
                    await Task.WhenAll(feturesTask, analysisTask);
                    var info = new TrackDTO(track, feturesTask.Result, color, analysisTask.Result);
                    return info;
                }, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(10) });
            }
            catch (Exception)
            {
                return new TrackDTO(null, null, Color.Transparent, null);
            }
        }
    }
}