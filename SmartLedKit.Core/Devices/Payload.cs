namespace SmartLedKit.Core.Devices
{
    public struct Payload
    {
        public IDictionary<int, object> dps;
        public string t;
        public string uid;
        public string gwId;
        public string devId;

        public Payload(IDictionary<int, object> dps, string uid, string gwId)
        {
            this.dps = dps;
            this.t = (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds.ToString("0");
            this.uid = gwId;
            this.devId = gwId;
            this.gwId = gwId;
        }
    }
}