using NtpServiceLibrary;
using System.Net;

namespace NtpUnitTests
{
    public class UdpClientAdapterTests
    {
        [Fact]
        public void UdpClientAdapter_CanSendAndReceive()
        {
            const int packetSize = 48; // NTP packet size
            const int receiveTimeout = 5000; // 5 seconds
            using var client = new UdpClientAdapter();
            client.ReceiveTimeout = receiveTimeout; // Set a timeout for receiving data
            client.Connect("pool.ntp.org", 123);

            var request = new byte[packetSize];
            request[0] = 0x1B;

            var bytesSent = client.Send(request, request.Length);

            var remoteEP = new IPEndPoint(IPAddress.Any, 0);
            var response = client.Receive(ref remoteEP);

            Assert.Equal(receiveTimeout, client.ReceiveTimeout);
            Assert.Equal(packetSize, bytesSent);
            Assert.True(response.Length > 0);
        }
    }
}
