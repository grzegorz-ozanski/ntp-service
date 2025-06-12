using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NtpServiceLibrary
{
    /// <summary>
    /// Provides static methods for interacting with the Windows system time.
    /// </summary>
    public static class Win32SystemTime
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetSystemTime(ref SystemTime st);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemTime(ref SystemTime st);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetLocalTime(ref SystemTime st);

        [StructLayout(LayoutKind.Sequential)]
        public struct SystemTime
        {
            public ushort Year;
            public ushort Month;
            public ushort DayOfWeek;
            public ushort Day;
            public ushort Hour;
            public ushort Minute;
            public ushort Second;
            public ushort Millisecond;
        }

        /// <summary>
        /// Sets the system time (UTC).
        /// </summary>
        /// <param name="dt">The UTC time to set.</param>
        public static void Set(DateTime dt)
        {
            if (dt.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("SetSystemTime requires DateTimeKind.Utc", nameof(dt));
            }

            SystemTime systemTime = new SystemTime
            {
                Year = (ushort)dt.Year,
                Month = (ushort)dt.Month,
                Day = (ushort)dt.Day,
                DayOfWeek = (ushort)dt.DayOfWeek,
                Hour = (ushort)dt.Hour,
                Minute = (ushort)dt.Minute,
                Second = (ushort)dt.Second,
                Millisecond = (ushort)dt.Millisecond
            };

            if (!SetSystemTime(ref systemTime))
            {
                int err = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"SetSystemTime failed: {new Win32Exception(err).Message}", new Win32Exception(err));
            }
        }

        /// <summary>
        /// Gets the current system time (UTC).
        /// </summary>
        /// <returns>The current UTC time.</returns>
        public static DateTime Get()
        {
            SystemTime systemTime = new SystemTime();
            if (!GetSystemTime(ref systemTime))
            {
                int err = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"GetSystemTime failed: {new Win32Exception(err).Message}", new Win32Exception(err));
            }
            return new DateTime(systemTime.Year, systemTime.Month, systemTime.Day, systemTime.Hour, systemTime.Minute, systemTime.Second, systemTime.Millisecond);
        }

        /// <summary>
        /// Gets the current local system time.
        /// </summary>
        /// <returns>The current local time.</returns>
        public static DateTime GetLocal()
        {
            SystemTime systemTime = new SystemTime();
            if (!GetLocalTime(ref systemTime))
            {
                int err = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"GetLocalTime failed: {new Win32Exception(err).Message}", new Win32Exception(err));
            }
            return new DateTime(systemTime.Year, systemTime.Month, systemTime.Day, systemTime.Hour, systemTime.Minute, systemTime.Second, systemTime.Millisecond);
        }
    }
}
