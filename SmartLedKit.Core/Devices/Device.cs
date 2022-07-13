using SmartLedKit.Core.Network;
using SmartLedKit.Core.Network.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartLedKit.Core.Devices
{
    public class Device : NetworkDevice, IDevice
    {
        IDictionary<DataPoint, object>? inititalState;
        public Device(string ip, string localKey, string deviceId) : base(ip, localKey, deviceId)
        {
        }

        public async Task<string?> GetJson()
        {
            byte[] request = EncodeRequest(Command.DP_QUERY, CreatePayload());
            byte[] encryptedResponse = await SendAsync(request);
            LocalResponse response = DecodeResponse(encryptedResponse);
            return response.JSON;
        }
        public async Task<DeviceDataPoints?> Get()
        {
            var json = await GetJson();
            return JsonConvert.DeserializeObject<DeviceDataPoints>(json);
        }

        public async Task Set(IDictionary<DataPoint, object> dps)
        {
            var json = CreatePayload(dps);
            byte[] request = EncodeRequest(Command.CONTROL, json);
            await SendAsync(request);
        }

        public async Task TurnOff()
        {
            var command = new Dictionary<DataPoint, object>() { { DataPoint.POWER, false } };
            var json = CreatePayload(command);
            byte[] request = EncodeRequest(Command.CONTROL, json);
            await SendAsync(request);
        }

        public async Task TurnOn()
        {
            var command = new Dictionary<DataPoint, object>() { { DataPoint.POWER, true }  };
            var json = CreatePayload(command);
            byte[] request = EncodeRequest(Command.CONTROL, json);
            await SendAsync(request);
        }

        public async Task<bool> IsOn()
        {
            var response =  await Get();
            if(response == null)
                return false;
            bool hasValue = response.Dps.TryGetValue((int)DataPoint.POWER, out object? value);
            if (!hasValue)
                return false;
            return Convert.ToBoolean(value);
        }

        public async Task Reset()
        {
            if (inititalState == null)
                return;
            await Set(inititalState);
        }

        public async Task CreateDefault()
        {
            if (inititalState == null)
            {
                var dps = await Get();
                if (dps == null)
                    return;
                inititalState = dps.ToKnownDataPoints();
            }
        }

        public string GetId()
        {
            return DeviceId;
        }

        public void SetIp(string ip)
        {
           IP = ip;
        }
    }
}
