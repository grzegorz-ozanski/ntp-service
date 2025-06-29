using System;

namespace NtpServiceLibrary
{
    internal class NtpPacket
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
            if (version != Version.Version3 || mode != Mode.Client)
                throw new NotImplementedException();

            Buffer = new byte[BufferSize];
            Buffer[0] = 0x1B;
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

        internal static uint SwapEndianness(uint x)
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
}
