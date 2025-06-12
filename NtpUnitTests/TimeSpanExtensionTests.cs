using NtpServiceLibrary;

namespace NtpUnitTests
{
    public class TimeSpanExtensionTests
    {
        [Fact]
        public void Format_ReturnsCorrectSingular()
        {
            var ts = new TimeSpan(1, 1, 1);
            Assert.Equal("1 hour 1 minute 1 second", ts.Format());
        }

        [Fact]
        public void Format_ReturnsCorrectPlural()
        {
            var ts = new TimeSpan(2, 3, 4);
            Assert.Equal("2 hours 3 minutes 4 seconds", ts.Format());
        }
    }
}