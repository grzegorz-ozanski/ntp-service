using NtpServiceLibrary;

namespace NtpUnitTests
{
    public class SettingsValueTests
    {
        [Fact]
        public void DefaultValue_IsSetCorrectly()
        {
            var value = new SettingsValue<int>(42);
            Assert.Equal(42, value.Get());
        }

        [Fact]
        public void Set_ChangesValueAndSource()
        {
            var value = new SettingsValue<string>("default");
            value.Set("new", "test");
            Assert.Equal("new", value.Get());
            Assert.Contains("test", value.ToString());
            Assert.Equal("new", (string)value);
        }
    }
}