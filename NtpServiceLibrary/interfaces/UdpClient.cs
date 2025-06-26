using System;
using System.Net;

namespace NtpServiceLibrary
{
    /// <summary>
    /// Represents a UDP client interface for sending and receiving data over a network.
    /// </summary>
    public interface IUdpClient : IDisposable
    {
        /// <summary>
        /// Connects the UDP client to a specified host and port.
        /// </summary>
        /// <param name="hostname">The name of the host to connect to.</param>
        /// <param name="port">The port number to connect to.</param>
        void Connect(string hostname, int port);

        /// <summary>
        /// Sends a datagram to the connected host.
        /// </summary>
        /// <param name="dgram">The data to send.</param>
        /// <param name="bytes">The number of bytes to send from the datagram.</param>
        /// <returns>The number of bytes sent.</returns>
        int Send(byte[] dgram, int bytes);

        /// <summary>
        /// Receives a datagram from a remote host.
        /// </summary>
        /// <param name="remoteEP">The endpoint of the remote host from which the data is received.</param>
        /// <returns>The received data as a byte array.</returns>
        byte[] Receive(ref IPEndPoint remoteEP);

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, the client will wait for a receive operation to complete.
        /// </summary>
        int ReceiveTimeout { get; set; }
    }
}
