namespace SmartLedKit
{
    public abstract class ManagerBuilderBase<T>:
        IBuildStage<T>,
        IDeviceIdStage<T>,
        IApiSecretSelectionStage<T>
        where T : class
    {
        protected string _accessId = String.Empty;
        protected string _apiSecret = String.Empty;
        protected string _anyDeviceId = String.Empty;
        protected int _timeout = 20_000;


        public IDeviceIdStage<T> AndApiSecret(string apiSecret)
        {
            _apiSecret = apiSecret;
            return this;
        }

        public abstract T Build();

        public IBuildStage<T> ConnectionTimeoutOf(int timeout)
        {
            _timeout = timeout;
            return this;
        }
        public IBuildStage<T> UsingDeviceId(string anyDeviceId)
        {
            _anyDeviceId = anyDeviceId;
            return this;
        }

        public IApiSecretSelectionStage<T> WithAccessId(string accessId)
        {
            _accessId = accessId;
            return this;
        }
    }
}