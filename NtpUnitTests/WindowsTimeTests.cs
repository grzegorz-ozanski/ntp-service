using NtpServiceLibrary;
using Xunit.Abstractions;

namespace NtpUnitTests
{
    public class Win32SystemTimeTests
    {
        private class MockTimeProvider : ISystemTimeProvider
        {
            private readonly bool _setSystemTimeResult;
            public MockTimeProvider(bool setSystemTimeResult = false)
            {
                _setSystemTimeResult = setSystemTimeResult;
            }
            public bool SetSystemTime(ref Win32SystemTime.SystemTime st) => _setSystemTimeResult;
            public bool GetSystemTime(ref Win32SystemTime.SystemTime st) => false;
            public bool GetLocalTime(ref Win32SystemTime.SystemTime st) => false;
        }
        private readonly ITestOutputHelper _output;
        public Win32SystemTimeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Set_Mock()
        {
            var systemTime = new Win32SystemTime(new MockTimeProvider(setSystemTimeResult: true));

            systemTime.Set(DateTime.UtcNow);
        }

        [Fact]
        public void Set_ThrowsArgumentException_WhenDateTimeIsNotUtc()
        {
            var local = DateTime.Now;
            var unspecified = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            Win32SystemTime systemTime = new Win32SystemTime(new SystemTimeProvider());

            Assert.Throws<ArgumentException>(() => systemTime.Set(local));
            Assert.Throws<ArgumentException>(() => systemTime.Set(unspecified));
        }

        [Fact]
        public void Set_ThrowsInvalidOperationException_WhenSetSystemTimeFails()
        {
            var systemTime = new Win32SystemTime(new MockTimeProvider());

            Assert.Throws<InvalidOperationException>(() => systemTime.Set(DateTime.UtcNow));
        }

        [Fact]
        public void Get_ThrowsInvalidOperationException_WhenSetSystemTimeFails()
        {
            var systemTime = new Win32SystemTime(new MockTimeProvider());

            Assert.Throws<InvalidOperationException>(() => systemTime.Get());
        }

        [Fact]
        public void GetLocal_ThrowsInvalidOperationException_WhenSetSystemTimeFails()
        {
            var systemTime = new Win32SystemTime(new MockTimeProvider());

            Assert.Throws<InvalidOperationException>(() => systemTime.GetLocal());
        }
        [Fact]
        public void GetLocal_ReturnsCurrentLocalTime()
        {
            Win32SystemTime systemTime = new Win32SystemTime(new SystemTimeProvider());
            DateTime now = DateTime.Now;

            var value = systemTime.GetLocal();
            Assert.InRange((value - now).TotalMilliseconds, -100, 100);
        }

        [Fact]
        public void Get_ReturnsCurrentUTCTime()
        {
            Win32SystemTime systemTime = new Win32SystemTime(new SystemTimeProvider());
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);

            DateTime utcNow = DateTime.UtcNow;
            DateTime now = DateTime.Now;
            var value = systemTime.Get();

            _output.WriteLine($"UTC Now: {utcNow}, Local Now: {now}, Offset: {offset}, Value: {value}");
            Assert.InRange((value - utcNow).TotalMilliseconds, -100, 100);
            Assert.InRange((value + offset - now).TotalMilliseconds, -100, 100);
        }
    }
}