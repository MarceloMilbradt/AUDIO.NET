using Newtonsoft.Json;

namespace SmartLedKit.Core.Devices
{
    public struct DeviceStatus
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("value")]
        public object Value { get; set; }
    }
}
