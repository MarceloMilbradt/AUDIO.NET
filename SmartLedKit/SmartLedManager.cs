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
        public SmartLedManager(string accessId, string apiSecret, string anyDeviceId) : base(5000, accessId, apiSecret, anyDeviceId)
        {
            try
            {
                var cts = new CancellationTokenSource();
                StartSearchingForDevices(25, cts.Token);
            }
            catch (Exception)
            {

            }
        }

        public async Task SetColorToAll(string color)
        {
            await SetDpsForAll(new Dictionary<DataPoint, object> { { DataPoint.COLOR, color }, { DataPoint.MODE, "colour" } });
        }
    }
}
