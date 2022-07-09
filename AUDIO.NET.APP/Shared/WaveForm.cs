using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Shared
{
    public class WaveForm
    {
        float trackDuration;
        List<double> levels;
        float min;
        float max;
        public List<double> FromTrackAnalysis(TrackAudioAnalysis analysis)
        {
            levels = new List<double>(1000);
            trackDuration = analysis.Track.Duration;
            IEnumerable<Segment> segments = GetSegments(analysis);
            min = segments.Min(l => l.loudness);
            max = segments.Max(l => l.loudness);

            MeasureLoudness(segments);
            return levels;
        }

        private void MeasureLoudness(IEnumerable<Segment> segments)
        {
            for (var i = 0d; i < 1; i += 0.001)
            {
                var segment = segments.First(s => i <= s.start + s.duration);
                var loudness = Math.Round(segment.loudness / max * 100) / 100;
                levels.Add(loudness);
            }
        }

        private IEnumerable<Segment> GetSegments(TrackAudioAnalysis analysis)
        {
            return analysis.Segments.Select(segment =>
            {
                var loudness = segment.LoudnessMax;
                var start = segment.Start / trackDuration;
                var duration = segment.Duration / trackDuration;
                return new Segment
                {
                    start = start,
                    duration = duration,
                    loudness = 1 - (Math.Min(Math.Max(loudness, -35), 0) / -35),
                };
            });
        }
    }
}
