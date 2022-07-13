using SmartLedKit.Core;
using SmartLedKit.Core.Devices;

namespace SmartLedKit
{
    public interface IDeviceManager
    {
        IDevice GetDevice(string id);
        Task FindDevices();
        Task ResetAll();
        Task ResetDevice(string id);
        Task SetDpsForAll(IDictionary<DataPoint, object> dps);
        Task SetDpsForDevice(string id, IDictionary<DataPoint, object> dps);
        Task TurnOffAll();
        Task TurnOffDevice(string id);
        Task TurnOnAll();
        Task TurnOnDevice(string id);
    }
}