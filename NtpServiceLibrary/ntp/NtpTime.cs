using System.Net.Sockets;
using System.Net;
using System;

namespace NtpServiceLibrary
{
    public class NtpTime
    {
        public const int ServerTimeout = 5000;
        /// <summary>
        /// Retrieves the current time from an NTP server over UDP.
        /// </summary>
        /// <param name="ntpServer">Hostname or IP address of the NTP server.</param>
        /// <param name="ntpPort">UDP port, typically 123.</param>
        /// <returns>UTC DateTime.</returns>
        public static DateTime RetrieveNTPTime(string ntpServer, int ntpPort, IUdpClient udpClient = null)
        {
            if (string.IsNullOrWhiteSpace(ntpServer))
                throw new ArgumentNullException(nameof(ntpServer));

            IUdpClient client = udpClient ?? new UdpClientAdapter();
            try
            {
                using (client)
                {
                    client.ReceiveTimeout = ServerTimeout;

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
