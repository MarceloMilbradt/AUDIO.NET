
namespace SmartLedKit
{
    public interface ISmartLedManager : IDeviceManager
    {
        Task SetColorToAll(string color);
    }
}