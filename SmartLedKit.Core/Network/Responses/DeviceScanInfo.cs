using Newtonsoft.Json;
using System;

namespace SmartLedKit.Core.Network.Responses
{
    public struct DeviceScanInfo : IEquatable<DeviceScanInfo>
    {
        [JsonProperty("ip")]
        public string? IP { get; set; } = null;

        [JsonProperty("gwId")]
        public string? GwId { get; set; } = null;

        [JsonProperty("active")]
        public int Active { get; set; } = 0;

        [JsonProperty("ability")]
        public int Ability { get; set; } = 0;

        [JsonProperty("mode")]
        public int Mode { get; set; } = 0;

        [JsonProperty("encrypt")]
        public bool Encryption { get; set; } = false;

        [JsonProperty("productKey")]
        public string? ProductKey { get; set; } = null;

        [JsonProperty("version")]
        public string? Version { get; set; } = null;

        public bool Equals(DeviceScanInfo other)
            => IP == other.IP && GwId == other.GwId;

        public override string ToString()
            => $"IP: {IP}, gwId: {GwId}, product key: {ProductKey}, encryption: {Encryption}, version: {Version}";
    }

}
