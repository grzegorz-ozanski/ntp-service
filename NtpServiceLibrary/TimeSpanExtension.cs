using System;

namespace NtpServiceLibrary
{
    public static class TimeSpanExtension
    {
        public static string Format(this TimeSpan timespan)
        {
            return string.Format("{0} hour{1} {2} minute{3} {4} second{5}",
                timespan.Hours, Suffix(timespan.Hours),
                timespan.Minutes, Suffix(timespan.Minutes),
                timespan.Seconds, Suffix(timespan.Seconds)
                );
        }
        private static string Suffix(int value)
        {
            return value != 1 ? "s" : "";
        }
    }
}
