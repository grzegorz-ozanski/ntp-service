using NtpServiceLibrary;

namespace NtpUnitTests
{
    public class SettingsTests
    {
        [Fact]
        public void DefaultValue_IsSetCorrectly()
        {
            Settings settings = new();
            Assert.NotNull(settings);
            Assert.Equal(Settings.DefaultNTPServer, settings.NTPServer.Get());
            Assert.Equal(Settings.DefaultNTPPollIntervalHours, settings.NTPPollIntervalHours.Get());
            Assert.Equal(Settings.DefaultNTPPort, settings.NTPPort.Get());
        }

        [Fact]
        public void DefaultValue_ToString()
        {
            Settings settings = new();
            string value = settings.ToString();
            foreach (string s in new string[] 
            { Settings.DefaultNTPServer, Settings.DefaultNTPPollIntervalHours.ToString(), Settings.DefaultNTPPort.ToString(),
            "Server: ", "Port: ", "PollIntervalHours: "}
            )
            {
                Assert.Contains(s, value);
            }
        }
    }
}
