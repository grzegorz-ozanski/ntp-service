using System.Net;
using System.Net.Sockets;

namespace NtpServiceLibrary
{
    /// <summary>
    /// Provides methods for retrieving time from an NTP server.
    /// </summary>
    internal class UdpClientAdapter : IUdpClient
    {
        private readonly UdpClient _client;
        public UdpClientAdapter()
        {
            _client = new UdpClient();
        }

        public int ReceiveTimeout
        {
            get => _client.Client.ReceiveTimeout;
            set => _client.Client.ReceiveTimeout = value;
        }

        public void Connect(string hostname, int port) => _client.Connect(hostname, port);

        public int Send(byte[] dgram, int bytes) => _client.Send(dgram, bytes);

        public byte[] Receive(ref IPEndPoint remoteEP) => _client.Receive(ref remoteEP);

        public void Dispose() => _client.Dispose();
    }

}
