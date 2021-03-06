using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Shared
{
    public class TrackDTO
    {
        public FullTrack? Track { get; }
        public TrackAudioFeatures? Features { get; }
        public List<double> Levels { get; }
        public string RGBColor { get; }
        public Color Color { get; }
        public TrackDTO(FullTrack? track, TrackAudioFeatures? features, Color color, TrackAudioAnalysis analysis)
        {
            Track = track;
            Features = features;
            RGBColor = $"{color.R},{color.G},{color.B}";
            Color = color;
            if (analysis != null)
            {
                Levels = new WaveForm().FromTrackAnalysis(analysis);
            }
            else
            {
                Levels = new List<double>();
            }
        }
    }
}
