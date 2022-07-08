using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Shared
{
    public static class ColorScraper
    {
        public static Color ScrapeColorForAlbum(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
           
            var mainDiv = doc.GetElementbyId("main");
            var target = mainDiv
                .FirstChild
                .FirstChild
                .FirstChild
                .FirstChild
                .FirstChild
                .FirstChild;
            return GetColorForElement(target);
        }

        private static Color GetColorForElement(HtmlNode node)
        {
            var attributes = node.GetAttributes();
            var style = attributes.First(a => a.Name == "style").Value;

            var rgx = Regex.Match(style, "#\\w+");
            var color = rgx.Groups.Values.First().Value;
            return ColorTranslator.FromHtml(color);
        }
        public static HSV ConvertToHSV(Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            return new HSV
            {
                h = (int)Math.Round(color.GetHue()),
                s = (int)Math.Round(((max == 0) ? 0 : 1d - (1d * min / max)) * 1000),
                v = (int)Math.Round(max / 255d * 1000),
            };
        }

    }

    public struct HSV
    {
        public int h;
        public int s;
        public int v;
        public override string ToString()
        {
            return h.ToString("X4").ToLower() + s.ToString("X4").ToLower() + v.ToString("X4").ToLower();
        }
    }
}
