using SmartLedKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedKit
{
    public class SmartLedManager : DeviceManager, ISmartLedManager
    {
        public SmartLedManager(int timeout, string accessId, string apiSecret, string anyDeviceId) : base(timeout, accessId, apiSecret, anyDeviceId)
        {
        }

        public async Task SetColorToAll(string color)
        {
            await SetDpsForAll(new Dictionary<DataPoint, object> { { DataPoint.COLOR, color } });
        }
    }
}
