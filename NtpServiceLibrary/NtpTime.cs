using System.Net.Sockets;
using System.Net;
using System;

namespace NtpServiceLibrary
{
    /// <summary>
    /// Provides methods for retrieving time from an NTP server.
    /// </summary>
    public class NtpTime
    {
        public const int ServerTimeout = 5000;

        private class NtpPacket
        {
            public enum Version
            {
                Version2,
                Version3

            }
            public enum Mode
            {
                Client,
                Server

            }
            public const uint BufferSize = 48;
            public const int IntPartOffset = 40;
            public const int FractPartOffset = 44;
            public byte[] Buffer { get; }

            public NtpPacket(Version version, Mode mode)
            {
                Buffer = new byte[BufferSize];
                switch (mode)
                {
                    case Mode.Client:
                        if (version == Version.Version3)
                        {
                            Buffer[0] = 0x1B;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                        break;
                    case Mode.Server:
                        throw new NotImplementedException();
                }
            }

            public static DateTime Parse(byte[] data)
            {
                if (data == null || data.Length < BufferSize)
                    throw new ArgumentException("Invalid NTP response");

                uint intPart = SwapEndianness(BitConverter.ToUInt32(data, IntPartOffset));
                uint fractPart = SwapEndianness(BitConverter.ToUInt32(data, FractPartOffset));

                double milliseconds = intPart * 1000.0 + (fractPart * 1000.0 / (1UL << 32));
                return new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);
            }

            private static uint SwapEndianness(uint x)
            {
                return (x >> 24) |
                       ((x & 0x00FF0000) >> 8) |
                       ((x & 0x0000FF00) << 8) |
                       (x << 24);
            }

            public int Size
            {
                get { return Buffer.Length; }
            }
        }

        /// <summary>
        /// Retrieves the current time from an NTP server over UDP.
        /// </summary>
        /// <param name="ntpServer">Hostname or IP address of the NTP server.</param>
        /// <param name="ntpPort">UDP port, typically 123.</param>
        /// <returns>UTC DateTime.</returns>
        public static DateTime RetrieveNTPTime(string ntpServer, int ntpPort)
        {
            if (string.IsNullOrWhiteSpace(ntpServer))
                throw new ArgumentNullException(nameof(ntpServer));

            try
            {
                using (UdpClient client = new UdpClient())
                {
                    client.Client.ReceiveTimeout = ServerTimeout;

                    client.Connect(ntpServer, ntpPort);

                    NtpPacket packet = new NtpPacket(NtpPacket.Version.Version3, NtpPacket.Mode.Client);

                    client.Send(packet.Buffer, packet.Size);

                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    return NtpPacket.Parse(client.Receive(ref remoteEndPoint));

                }
            }
            catch (SocketException ex)
            {
                throw new InvalidOperationException($"Failed to contact NTP server {ntpServer}:{ntpPort}", ex);
            }
        }
    }
}
