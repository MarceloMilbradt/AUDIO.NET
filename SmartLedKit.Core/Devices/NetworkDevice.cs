using SmartLedKit.Core.Network;
using SmartLedKit.Core.Network.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;

namespace SmartLedKit.Core.Devices
{
    public class NetworkDevice : IDisposable
    {

        private TcpClient client = null;
        private SemaphoreSlim sem = new SemaphoreSlim(1);

        public string IP { get; set; }
        public string LocalKey { get; set; }
        public string DeviceId { get; private set; }
        public int Port { get; private set; } = 6668;
        public ProtocolVersion ProtocolVersion { get; set; }
        public int ConnectionTimeout { get; set; } = 5000;

        public int ReceiveTimeout { get; set; }
        public int NetworkErrorRetriesInterval { get; set; } = 100;
        public int NullRetriesInterval { get; set; } = 0;

        public bool PermanentConnection { get; set; } = false;
        public NetworkDevice(string ip, string localKey, string deviceId)
        {
            IP = ip;
            LocalKey = localKey;
            DeviceId = deviceId;
            ProtocolVersion = ProtocolVersion.V33;
            Port = 6668;
            ReceiveTimeout = 2000;
        }


        public NetworkDevice(string ip, string deviceId) : this(ip, null, deviceId)
        {
        }
        public LocalResponse DecodeResponse(byte[] data)
        {
            if (string.IsNullOrEmpty(LocalKey)) throw new ArgumentException("LocalKey is not specified", "LocalKey");
            return Parser.DecodeResponse(data, Encoding.UTF8.GetBytes(LocalKey), ProtocolVersion);
        }

        public void Dispose()
        {
            client?.Close();
            client?.Dispose();
            client = null;
        }

        public byte[] EncodeRequest(Command command, string json)
        {
            if (string.IsNullOrEmpty(LocalKey)) throw new ArgumentException("LocalKey is not specified", "LocalKey");
            return Parser.EncodeRequest(command, json, Encoding.UTF8.GetBytes(LocalKey), ProtocolVersion);
        }
      
        public string CreatePayload(IDictionary<int,object> dps)
        {
            var t = (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds.ToString("0");
            return JsonConvert.SerializeObject(new Payload(dps, DeviceId, DeviceId));
        }
        public string CreatePayload(IDictionary<DataPoint,object> dps)
        {
            var _dps = new Dictionary<int, object>();
            foreach (var d in dps) _dps.Add((int)d.Key, d.Value);
            return JsonConvert.SerializeObject(new Payload(_dps, DeviceId, DeviceId));
        }
        public string CreatePayload()
        {
            var t = (DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds.ToString("0");
            return JsonConvert.SerializeObject(new Payload(null, DeviceId, DeviceId));
        }
       
        public async Task<Dictionary<int, object>> GetDpsAsync(int retries = 5, int nullRetries = 1, int? overrideRecvTimeout = null, CancellationToken cancellationToken = default)
        {
            var requestJson = CreatePayload();
            var response = await SendAsync(Command.DP_QUERY, requestJson, retries, nullRetries, overrideRecvTimeout, cancellationToken);
            if (string.IsNullOrEmpty(response.JSON))
                throw new InvalidDataException("Response is empty");
            var root = JObject.Parse(response.JSON);
            var dps = JsonConvert.DeserializeObject<Dictionary<string, object>>(root.GetValue("dps").ToString());
            return dps.ToDictionary(kv => int.Parse(kv.Key), kv => kv.Value);
        }

        public async Task<LocalResponse> SendAsync(Command command, string json, int retries = 2, int nullRetries = 1, int? overrideRecvTimeout = null, CancellationToken cancellationToken = default)
            => DecodeResponse(await SendAsync(EncodeRequest(command, json), retries, nullRetries, overrideRecvTimeout, cancellationToken));

        public async Task<byte[]> SendAsync(byte[] data, int retries = 2, int nullRetries = 1, int? overrideRecvTimeout = null, CancellationToken cancellationToken = default)
        {
            Exception lastException = null;
            while (retries-- > 0)
            {
                if (!PermanentConnection || client?.Connected == false)
                {
                    client?.Close();
                    client?.Dispose();
                    client = null;
                }
                try
                {
                    using (await sem.WaitDisposableAsync(cancellationToken))
                    {
                        if (client == null)
                            client = new TcpClient();
                        if (!client.ConnectAsync(IP, Port).Wait(ConnectionTimeout))
                            throw new IOException("Connection timeout");
                        var stream = client.GetStream();
                        await stream.WriteAsync(data, 0, data.Length, cancellationToken).ConfigureAwait(false);
                        return await ReceiveAsync(stream, nullRetries, overrideRecvTimeout, cancellationToken);
                    }
                }
                catch (Exception ex) when (ex is IOException or TimeoutException or SocketException)
                {
                    lastException = ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (!PermanentConnection || client?.Connected == false || lastException != null)
                    {
                        client?.Close();
                        client?.Dispose();
                        client = null;
                    }
                }
                await Task.Delay(NetworkErrorRetriesInterval, cancellationToken);
            }
            throw lastException;
        }

        private async Task<byte[]> ReceiveAsync(NetworkStream stream, int nullRetries = 1, int? overrideRecvTimeout = null, CancellationToken cancellationToken = default)
        {
            byte[] result;
            byte[] buffer = new byte[1024];
            using (var ms = new MemoryStream())
            {
                int length = buffer.Length;
                while (ms.Length < 16 || (length = BitConverter.ToInt32(Parser.BigEndian(ms.ToArray().Skip(12).Take(4)).ToArray(), 0) + 16) < ms.Length)
                {
                    var timeoutCancellationTokenSource = new CancellationTokenSource();
                    var readTask = stream.ReadAsync(buffer, 0, length, cancellationToken: cancellationToken);
                    var timeoutTask = Task.Delay(overrideRecvTimeout ?? ReceiveTimeout, cancellationToken: timeoutCancellationTokenSource.Token);
                    var t = await Task.WhenAny(readTask, timeoutTask).ConfigureAwait(false);
                    timeoutCancellationTokenSource.Cancel();
                    int bytes = 0;
                    if (t == timeoutTask)
                    {
                        if (stream.DataAvailable)
                            bytes = await stream.ReadAsync(buffer, 0, length, cancellationToken);
                        else
                            throw new TimeoutException();
                    }
                    else if (t == readTask)
                    {
                        bytes = await readTask;
                    }
                    ms.Write(buffer, 0, bytes);
                }
                result = ms.ToArray();
            }
            if (result.Length <= 28 && nullRetries > 0) // empty response
            {
                await Task.Delay(NullRetriesInterval, cancellationToken);
                result = await ReceiveAsync(stream, nullRetries - 1, overrideRecvTimeout: overrideRecvTimeout, cancellationToken);
            }
            return result;
        }
    }
}