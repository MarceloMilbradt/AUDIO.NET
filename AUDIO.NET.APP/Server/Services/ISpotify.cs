using AUDIO.NET.APP.Shared;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Server.Services
{
    public interface ISpotify
    {
        Task Connect(string code);
        Task Start();
        Task Start(Action<Task<TrackDTO>> onTrackChange);
        Uri GetLoginUri();
        bool IsListening();
        Task<TrackAudioFeatures?> GetAudioFeatures();
        Task<PrivateUser?> GetCurrentUser();
        Task<FullTrack?> GetTrack();
        Task<TrackDTO> GetFullInfo();
    }
}
