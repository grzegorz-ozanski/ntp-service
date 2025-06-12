using System;

namespace NtpServiceLibrary
{
    /// <summary>
    /// Provides extension methods for the <see cref="TimeSpan"/> structure.
    /// </summary>
    public static class TimeSpanExtension
    {
        /// <summary>
        /// Formats the <see cref="TimeSpan"/> into a human-readable string
        /// with correct pluralization for hours, minutes, and seconds.
        /// </summary>
        /// <param name="timespan">The <see cref="TimeSpan"/> to format.</param>
        /// <returns>
        /// A string in the format "X hour(s) Y minute(s) Z second(s)".
        /// </returns>
        public static string Format(this TimeSpan timespan)
        {
            return string.Format("{0} hour{1} {2} minute{3} {4} second{5}",
                timespan.Hours, Suffix(timespan.Hours),
                timespan.Minutes, Suffix(timespan.Minutes),
                timespan.Seconds, Suffix(timespan.Seconds)
                );
        }

        /// <summary>
        /// Returns the plural suffix "s" if the value is not 1.
        /// </summary>
        /// <param name="value">The numeric value to check.</param>
        /// <returns>"s" if value is not 1; otherwise, an empty string.</returns>
        private static string Suffix(int value)
        {
            return value != 1 ? "s" : "";
        }
    }
}
