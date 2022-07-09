using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AUDIO.NET.APP.Server.Utils
{ 
    public static class DeviceConnector
    {
        public static void ChangeColor(HSV color)
        {
            Get(color.ToString());
        }
        public static void ResetColor()
        {
            Get("default");
        }
        private static void Get(string color)
        {
            using (var client = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate }))
            {
                client.BaseAddress = new Uri("http://localhost:3000");
                HttpResponseMessage response = client.GetAsync("/"+ color).Result;
                response.EnsureSuccessStatusCode();
            }
        }
    }
}
