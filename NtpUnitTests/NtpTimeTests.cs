using NtpServiceLibrary;
using System.Net;
using System.Net.Sockets;

namespace NtpUnitTests
{
    public class NtpTimeTests
    {
        private class MockUdpClient : IUdpClient
        {
            public int ReceiveTimeout { get; set; }
            public void Connect(string hostname, int port) { }
            public int Send(byte[] dgram, int bytes) => bytes;
            public byte[] Receive(ref IPEndPoint remoteEP)
            {
                byte[] response = new byte[48];
                DateTime expected = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                TimeSpan since1900 = expected - new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                uint intPart = (uint)(since1900.TotalSeconds);
                uint fractPart = (uint)((since1900.TotalSeconds - intPart) * (1L << 32));

                // Big endian
                byte[] intBytes = BitConverter.GetBytes(intPart);
                byte[] fractBytes = BitConverter.GetBytes(fractPart);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(intBytes);
                    Array.Reverse(fractBytes);
                }

                Array.Copy(intBytes, 0, response, 40, 4);
                Array.Copy(fractBytes, 0, response, 44, 4);
                return response;
            }
            public void Dispose() { }
        }
        private class FailingUdpClient : IUdpClient
        {
            public int ReceiveTimeout { get; set; }
            public void Connect(string hostname, int port) { }
            public int Send(byte[] dgram, int bytes) => throw new SocketException();
            public byte[] Receive(ref IPEndPoint remoteEP) => throw new NotImplementedException();
            public void Dispose() { }
        }

        [Fact]
        public void RetrieveNTPTime_ParsesCorrectResponse()
        {
            var mockClient = new MockUdpClient();
            DateTime expected = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var result = NtpTime.RetrieveNTPTime("dummy", 123, mockClient);
            Assert.Equal(expected, result);
        }
        [Fact]
        public void RetrieveNTPTime_BadServerName()
        {
            var mockClient = new MockUdpClient();
            Assert.Throws<ArgumentNullException>(() => NtpTime.RetrieveNTPTime("", 123, mockClient));
        }
        [Fact]
        public void NTPTime_TestNTPPacket()
        {
            var packet = new NtpPacket(NtpPacket.Version.Version3, NtpPacket.Mode.Client);
            Assert.Throws<NotImplementedException>(() => new NtpPacket(NtpPacket.Version.Version3, NtpPacket.Mode.Server));
            Assert.Throws<NotImplementedException>(() => new NtpPacket(NtpPacket.Version.Version2, NtpPacket.Mode.Client));
            Assert.Throws<NotImplementedException>(() => new NtpPacket(NtpPacket.Version.Version2, NtpPacket.Mode.Server));
        }
        [Fact]
        public void RetrieveNTPTime_ThrowsInvalidOperationException_OnSocketError()
        {
            var client = new FailingUdpClient();
            var ex = Assert.Throws<InvalidOperationException>(() =>
                NtpTime.RetrieveNTPTime("dummy", 123, client)
            );
            Assert.IsType<SocketException>(ex.InnerException);
        }
        [Fact]
        public void SwapEndianness_ReturnsCorrectValue()
        {
            uint original = 0x12345678;
            uint expected = 0x78563412;
            uint actual = NtpPacket.SwapEndianness(original);
            Assert.Equal(expected, actual);
        }
        [Theory]
        [InlineData(null)]
        [InlineData(10)]
        public void Parse_ThrowsArgumentException_OnInvalidData(int? length)
        {
            byte[] data = length == null ? null! : new byte[length.Value];
            Assert.Throws<ArgumentException>(() => NtpPacket.Parse(data));
        }

        [Fact]
        public void NtpPacket_Size_IsCorrect()
        {
            var packet = new NtpPacket(NtpPacket.Version.Version3, NtpPacket.Mode.Client);
            Assert.Equal(48, packet.Size);
        }
    }
}
