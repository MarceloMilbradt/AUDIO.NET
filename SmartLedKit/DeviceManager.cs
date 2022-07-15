using SmartLedKit.Core;
using SmartLedKit.Core.Devices;
using SmartLedKit.Core.Network;
using SmartLedKit.Core.Network.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedKit
{
    public class DeviceManager : IDeviceManager
    {
        private Dictionary<string, IDevice> _devices = new Dictionary<string, IDevice>();
        TuyaApi.Region _region = TuyaApi.Region.WesternAmerica;
        string _accessId = string.Empty;
        string _apiSecret = string.Empty;
        string _anyDeviceId = string.Empty;
        int _timeout = 20_000;
        public DeviceManager(int timeout, string accessId, string apiSecret, string anyDeviceId)
        {
            this._timeout = timeout;
            this._accessId = accessId;
            this._apiSecret = apiSecret;
            this._anyDeviceId = anyDeviceId;
        }
        public void StartSearchingForDevices(int minutes, CancellationToken token)
        {
            Task.Run(async () => {
                while (!token.IsCancellationRequested)
                {
                    await FindDevices();
                    await Task.Delay(TimeSpan.FromMinutes(minutes), token);
                }
            }, token);
        }

        public async Task FindDevices()
        {
            var api = new TuyaApi(region: _region, accessId: _accessId, apiSecret: _apiSecret);
            var devicesInApi = await api.GetAllDevicesInfoAsync(anyDeviceId: _anyDeviceId);
            foreach (var device in devicesInApi)
            {
                var dev = new Device(device.Ip, device.LocalKey, device.Id);
                if (!HasDevice(device.Id))
                    _devices.Add(dev.DeviceId, dev);
            }

            var scanner = new Scanner(ProtocolVersion.V33);
            scanner.OnNewDeviceInfoReceived += async (object? sender, DeviceScanInfo e) =>
            {
                var hasDevice = TryGetDevice(e.GwId!, out var device);
                if (!hasDevice) return;

                device.SetIp(e.IP!);
                device.CreateDefault();
                var isOn = await device.IsOn();
                if (!isOn) device.TurnOn();
            };
            var token = new CancellationTokenSource();
            scanner.Start(token.Token);
            await Task.Delay(_timeout);
            token.Cancel();
        }
        public async Task SetDpsForAll(IDictionary<DataPoint, object> dps)
        {
            var devices = GetAllDevices();
            var tasks = devices.Select(device => device.Set(dps));
            await Task.WhenAll(tasks);
        }
        public async Task TurnOnAll()
        {
            var devices = GetAllDevices();
            var tasks = devices.Select(device => device.TurnOn());
            await Task.WhenAll(tasks);
        }
        public async Task TurnOffAll()
        {
            var devices = GetAllDevices();
            var tasks = devices.Select(device => device.TurnOff());
            await Task.WhenAll(tasks);
        }

        private Dictionary<string, IDevice>.ValueCollection GetAllDevices()
        {
            return _devices.Values;
        }

        public async Task ResetAll()
        {
            var devices = GetAllDevices();
            var tasks = devices.Select(device => device.Reset());
            await Task.WhenAll(tasks);
        }
        public async Task SetDpsForDevice(string id, IDictionary<DataPoint, object> dps)
        {
            var device = GetDevice(id);
            await device.Set(dps);
        }
        public async Task TurnOnDevice(string id)
        {
            var device = GetDevice(id);
            await device.TurnOn();
        }
        public async Task TurnOffDevice(string id)
        {
            var device = GetDevice(id);
            await device.TurnOff();
        }
        public async Task ResetDevice(string id)
        {
            var device = GetDevice(id);
            await device.Reset();
        }
        public IDevice GetDevice(string id)
        {
            _devices.TryGetValue(id, out var deviceFound);
            return deviceFound!;
        }
        public bool HasDevice(string id)
        {
            return _devices.ContainsKey(id);
        }
        public bool TryGetDevice(string id, out IDevice deviceFound)
        {
            return _devices.TryGetValue(id, out deviceFound);
        }
    }
}
