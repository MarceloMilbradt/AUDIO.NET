using AUDIO.NET.APP.Shared;
using SpotifyAPI.Web;

namespace AUDIO.NET.APP.Server.Services
{
    public interface ISpotifyAPI
    {
        Task Connect(string code);
        Task<TrackAudioAnalysis> GetAnalysis(FullTrack? track);
        Task<TrackAudioFeatures?> GetAudioFeatures();
        Task<FullTrack?> GetCurrentTrack();
        Task<PrivateUser?> GetCurrentUser();
        Task<TrackAudioFeatures> GetFeatures(FullTrack? track);
        Uri GetLoginUri();
        Task<FullTrack?> GetTrack();
        bool IsUserLoggedIn();
        Task<TrackDTO> GetFullInfo(FullTrack track);
        Task<CurrentlyPlaying?> GetCurrent();
    }
}