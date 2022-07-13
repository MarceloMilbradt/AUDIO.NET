using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedKit.Core.Network.Responses
{
    public class DeviceDataPoints
    {
        public DeviceDataPoints()
        {
            Dps = new Dictionary<int, object>();
        }
        public Dictionary<int, object> Dps { get; set; }
        public IDictionary<DataPoint, object> ToKnownDataPoints()
        {
            var dataPoints = new Dictionary<DataPoint, object>();
            foreach (var dataPoint in Dps)
            {
                if (Enum.IsDefined(typeof(DataPoint), dataPoint.Key))
                    dataPoints.Add((DataPoint)dataPoint.Key, dataPoint.Value);
            }
            return dataPoints;
        }
    }
}
