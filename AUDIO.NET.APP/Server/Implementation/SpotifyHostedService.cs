using AUDIO.NET.APP.Server.Notifications;
using AUDIO.NET.APP.Server.Services;
using AUDIO.NET.APP.Server.Services.Implementation;
using AUDIO.NET.APP.Server.Utils;
using AUDIO.NET.APP.Shared;
using MediatR;
using SpotifyAPI.Web;
using System.Drawing;

namespace AUDIO.NET.APP.Server.Implementation
{
    public class SpotifyHostedService : BackgroundService
    {
        private readonly ILogger<SpotifyHostedService> _logger;
        private readonly ISpotifyAPI _spotifyApi;
        private readonly IMediator _mediator;

        private string _currentAlbumUrl = string.Empty;
        private FullTrack _currentTrack;

        public SpotifyHostedService(ILogger<SpotifyHostedService> logger, IMediator mediator, ISpotifyAPI spotifyApi)
        {
            _logger = logger;
            _mediator = mediator;
            _spotifyApi = spotifyApi;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            _logger.LogInformation(ErrorMessages.NOW_LISTENING);
            int nextIterationIn = 1000;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_spotifyApi.IsUserLoggedIn())
                    {
                        var currentlyPlaying = await _spotifyApi.GetCurrent();

                        if (currentlyPlaying == null)
                        {
                            _logger.LogInformation(ErrorMessages.NOW_PLAYING, "Nothing");
                            nextIterationIn = 10000;
                            await HandleNoTrackIsPlaying(stoppingToken);
                        }
                        else
                        {
                            var track = (FullTrack)currentlyPlaying.Item;
                            nextIterationIn = GetNextIterationTimeout(currentlyPlaying, track);
                            await HandleWhenTrackIsPlaying(track, stoppingToken);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ErrorMessages.ERROR_WHILE_LISTENING, ex);
                }
                finally
                {
                    await Task.Delay(nextIterationIn, stoppingToken);
                }
            }
        }

        private static int GetNextIterationTimeout(CurrentlyPlaying? currentlyPlaying, FullTrack track)
        {
            int nextIterationIn;
            if (currentlyPlaying.IsPlaying)
            {
                var progress = Convert.ToInt32(currentlyPlaying.ProgressMs);
                var progressPercentage = progress * 100f / track.DurationMs;
                if (progressPercentage > 20 && progressPercentage < 80)
                {
                    if (track.DurationMs / 10 > 30_000)
                    {
                        nextIterationIn = track.DurationMs / 100;
                    }
                    else
                    {
                        nextIterationIn = track.DurationMs / 10;

                    }

                }
                else
                {
                    nextIterationIn = 1000;
                }
            }
            else
            {
                nextIterationIn = 1000;
            }

            return nextIterationIn;
        }

        private async Task HandleNoTrackIsPlaying(CancellationToken stoppingToken)
        {
            if (_currentAlbumUrl != string.Empty)
            {
                _currentAlbumUrl = string.Empty;
                await _mediator.Publish(new AlbumChangeNotification(_currentAlbumUrl), stoppingToken);
            }
        }

        private async Task HandleWhenTrackIsPlaying(FullTrack currentlyPlaying, CancellationToken stoppingToken)
        {
            var url = currentlyPlaying.Album.ExternalUrls["spotify"];
            if (_currentTrack?.Id != currentlyPlaying.Id)
            {
                _logger.LogInformation(ErrorMessages.NOW_PLAYING, currentlyPlaying.Name);
                _currentTrack = currentlyPlaying;
                var trackInfo = await _spotifyApi.GetFullInfo(_currentTrack);
                var hsvColor = ColorScraper.ConvertToHSV(trackInfo.Color);
                await _mediator.Publish(new TrackChangeNotification(trackInfo), stoppingToken);
                if (url != _currentAlbumUrl)
                {
                    _currentAlbumUrl = url;
                    await _mediator.Publish(new AlbumChangeNotification(hsvColor.ToString()), stoppingToken);
                }
            }
        }
    }
}
