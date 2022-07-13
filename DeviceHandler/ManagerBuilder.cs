using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedKit
{
    public class SmartLedManagerBuilder : ManagerBuilderBase<ISmartLedManager>
    {
        public override ISmartLedManager Build()
        {
            return new SmartLedManager(_timeout, _accessId, _apiSecret, _anyDeviceId);
        }
    }
    public class ManagerBuilder : ManagerBuilderBase<IDeviceManager>
    {
        public override IDeviceManager Build()
        {
            return new DeviceManager(_timeout, _accessId, _apiSecret, _anyDeviceId);
        }
    }

    public interface IBuilder<T>
    {
        IApiSecretSelectionStage<T> WithAccessId(string accessId);
    }

    public interface IApiSecretSelectionStage<T>
    {
        IDeviceIdStage<T> AndApiSecret(string apiSecret);
    }

    public interface IDeviceIdStage<T>
    {
        IBuildStage<T> UsingDeviceId(string anyDeviceId);
    }

    public interface IBuildStage<T>
    {
        IBuildStage<T> ConnectionTimeoutOf(int timeout);
        T Build();
    }
}
