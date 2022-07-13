using SmartLedKit.Core.Network.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedKit.Core.Devices
{
    public interface IDevice
    {
        public string GetId();
        public void SetIp(string ip);
        public Task<bool> IsOn();
        public Task TurnOn();
        public Task TurnOff();
        public Task CreateDefault();
        public Task Reset();
        public Task Set(IDictionary<DataPoint, object> dps);
        public Task<string?> GetJson();
        public Task<DeviceDataPoints?> Get();
    }
}
