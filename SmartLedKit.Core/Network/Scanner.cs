using SmartLedKit.Core.Network.Responses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace SmartLedKit.Core.Network
{
    public class Scanner
    {
        private const ushort UDP_PORT31 = 6666;      // Tuya 3.1 UDP Port
        private const ushort UDP_PORTS33 = 6667;     // Tuya 3.3 encrypted UDP Port
        private const string UDP_KEY = "yGAdlopoPVldABfn";


        private ProtocolVersion _versionsToFind;
        private bool running = false;
        private UdpClient udpServer31 = null;
        private UdpClient udpServer33 = null;
        private Thread udpListener31 = null;
        private Thread udpListener33 = null;
        private List<DeviceScanInfo> devices = new List<DeviceScanInfo>();

        public event EventHandler<DeviceScanInfo> OnDeviceInfoReceived;
        public event EventHandler<DeviceScanInfo> OnNewDeviceInfoReceived;
        public Scanner(ProtocolVersion versions = ProtocolVersion.V31 | ProtocolVersion.V33)
        {
            _versionsToFind = versions;
        }

        public void Start(CancellationToken cancellationToken)
        {
            Stop();
            running = true;
            devices.Clear();

            if (_versionsToFind.HasFlag(ProtocolVersion.V31))
            {
                udpServer31 = new UdpClient(UDP_PORT31);
                udpListener31 = new Thread(()=> UdpListenerThread(ProtocolVersion.V31,udpServer31, cancellationToken));
                udpListener31.Start();
            }
            if (_versionsToFind.HasFlag(ProtocolVersion.V33)){
                udpServer33 = new UdpClient(UDP_PORTS33);
                udpListener33 = new Thread(() => UdpListenerThread(ProtocolVersion.V33, udpServer33, cancellationToken));
                udpListener33.Start();
            }
        }

        public void Stop()
        {
            running = false;
            if (udpServer31 != null)
            {
                udpServer31.Dispose();
                udpServer31 = null;
            }
            if (udpServer33 != null)
            {
                udpServer33.Dispose();
                udpServer33 = null;
            }
            udpListener31 = null;
            udpListener33 = null;
        }

        private void UdpListenerThread(ProtocolVersion version,UdpClient udpServer, CancellationToken cancellationToken)
        {
            byte[] udp_key;
            using (var md5 = MD5.Create())
            {
                udp_key = md5.ComputeHash(Encoding.ASCII.GetBytes(UDP_KEY));
            }

            while (running)
            {
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        Stop();
                        return;
                    }
                    IPEndPoint ep = null;
                    var data = udpServer.Receive(ref ep);
                    var response = Parser.DecodeResponse(data, udp_key, version);
                    Parse(response.JSON);
                }
                catch
                {
                    if (!running) return;
                    throw;
                }
            }
        }


        private void Parse(string json)
        {
            var deviceInfo = JsonConvert.DeserializeObject<DeviceScanInfo>(json);
            OnDeviceInfoReceived?.Invoke(this, deviceInfo);
            if ((OnNewDeviceInfoReceived) != null && !devices.Contains(deviceInfo))
            {
                devices.Add(deviceInfo);
                OnNewDeviceInfoReceived?.Invoke(this, deviceInfo);
            }
        }
    }
}
