using AUDIO.NET.APP.Server.Utils;
using AUDIO.NET.APP.Shared;
using Serilog;
using SpotifyAPI.Web;
using System.Drawing;
using ILogger = Serilog.ILogger;

namespace AUDIO.NET.APP.Server.Services.Implementation
{
    public class SpotifyListener : SpotifyBase, ISpotify
    {
        string _currentUlr = string.Empty;
        bool _isListening;
        int _refreshRate = 500;
        readonly ILogger logger;
        Color _currentColor;
        FullTrack _currentTrack;
        public bool IsListening() => _isListening;
        public SpotifyListener(string url, string clientId, string clientSecret) : base(url, clientId, clientSecret)
        {
            logger = Log.Logger.ForContext<SpotifyListener>();
        }
        public async Task Start()
        {
            await Start(_ => { });
        }
        public async Task Start(Action<Task<TrackDTO>> onTrackChange)
        {

            logger.Information("Now Listening");
            if (_isListening)
                return;
            _isListening = true;
            while (true)
            {
                var playing = await Listen(onTrackChange);
                ChangeRefreshRate(playing);
                Thread.Sleep(_refreshRate);
            }
        }

        private void ChangeRefreshRate(bool playing)
        {
            _refreshRate = playing ? 500 : 2000;
        }
        private async Task<bool> Listen(Action<Task<TrackDTO>> onTrackChange)
        {
            try
            {
                if (_spotify == null) return false;

                var currentlyPlaying = await GetCurrentTrack();

                if (currentlyPlaying == null)
                {
                    logger.Information("Reseting Colors");
                    _currentUlr = string.Empty;
                    DeviceConnector.ResetColor();
                    return false;
                }

                var url = currentlyPlaying.Album.ExternalUrls["spotify"];

                if (_currentTrack?.Id == currentlyPlaying.Id) return true;
                _currentTrack = currentlyPlaying;
                logger.Information("Now Playing {track}", currentlyPlaying.Name);
                if (url != _currentUlr)
                {
                    _currentColor = ColorScraper.ScrapeColorForAlbum(url);
                    var hsvColor = ColorScraper.ConvertToHSV(_currentColor);
                    DeviceConnector.ChangeColor(hsvColor);
                    logger.Information("New Color {color}", hsvColor);
                }
                onTrackChange(GetFullInfo());
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Error while listening, {ex}", ex);
                return false;
            }
        }

        public async Task<TrackDTO> GetFullInfo()
        {
            try
            {
                var feturesTask =  GetFeatures(_currentTrack);
                var analysisTask =  GetAnalysis(_currentTrack);
                await Task.WhenAll(feturesTask,analysisTask);
                return new TrackDTO(_currentTrack, feturesTask.Result, _currentColor, analysisTask.Result);
            }
            catch (Exception ex)
            {
                logger.Error("Error while getting infos, {ex}", ex);
                return new TrackDTO(null, null, Color.Transparent, null);
            }
        }


    }
}