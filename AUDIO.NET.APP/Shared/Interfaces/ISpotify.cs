using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Shared.Interfaces
{
    public interface ISpotify
    {
        Task Connect(string code);
        Task Start();
        Uri GetLoginUri();
        bool IsListening();
        Task<TrackAudioFeatures> GetAudioFeatures();
        Task<PrivateUser> GetCurrentUser();
        Task<FullTrack> GetTrack();
    }
}
