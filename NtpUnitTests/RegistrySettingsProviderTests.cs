using Microsoft.Win32;
using Moq;
using NtpServiceLibrary;
using System.Runtime.Versioning;

namespace NtpUnitTests
{
    class MockValue
    {
        private readonly object _value;
        private readonly RegistryValueKind _kind;
        public MockValue(object value)
        {
            _value = value;
            _kind = value switch
            {
                int => RegistryValueKind.DWord,
                string => RegistryValueKind.String,
                _ => RegistryValueKind.Unknown
            };
        }
        public object Get() => _value;
        public RegistryValueKind GetKind() => _kind;
    }
    class MockKey : IKey
    {
        private readonly Dictionary<string, MockValue> _values = new();
        private readonly Dictionary<string, MockKey> _subKeys = new();
        public MockKey()
        {
        }

        public MockKey(string name, MockKey child)
        {
            _subKeys[name] = child;
        }

        public MockKey(Dictionary<string, MockValue> values)
        {
            _values = values;
        }
        public MockKey(Dictionary<string, object> values)
        {
            foreach (var item in values)
            {
                _values[item.Key] = new MockValue(item.Value);
            }
        }
        public void SetValue(string name, MockValue value)
        {
            _values[name] = value;
        }
        public object GetValue(string name) => _values.TryGetValue(name, out var value) ? value.Get() : null!;
        public RegistryValueKind GetValueKind(string name) => _values.TryGetValue(name, out var value) ? value.GetKind() : RegistryValueKind.Unknown;
        public IKey OpenSubKey(string name) => _subKeys.TryGetValue(name, out var subKey) ? subKey : null!;
        public string[] GetValueNames() => [.. _values.Keys];
    }
    public class RegistrySettingsProviderTests
    {
        private static IKey BuildKeyHierarchy(Dictionary<string, IKey> structure)
        {
            var mock = new Mock<IKey>();
            foreach (var kvp in structure)
            {
                mock.Setup(m => m.OpenSubKey(kvp.Key)).Returns(kvp.Value);
            }
            return mock.Object;
        }
        private static MockKey BuildMockHierarchy(Dictionary<string, object> values)
        {
            var leafKey = new MockKey(values);

            foreach (var name in new[] { "Parameters", "MyService", "Services", "CurrentControlSet", "SYSTEM" })
                leafKey = new MockKey(name, leafKey);
            return leafKey;
        }
        private static bool ArgsContain(object[] args, string expectedMessage)
        {
            return args.OfType<string>().Any(s => s.Contains(expectedMessage));
        }
        private static void VerifyLoggerCall(Mock<ILogger> logger, string expectedMessage)
        {
            logger.Verify(l => l.Write(It.IsAny<string>(), It.IsAny<object[]>()), Times.Once);
            Assert.True(logger.Invocations.Any(invocation =>
                invocation.Arguments.Count > 1 &&
                invocation.Arguments[0] is string format &&
                invocation.Arguments[1] is object[] args &&
                (ArgsContain([format], expectedMessage) || ArgsContain(args, expectedMessage))
                ),
                $"Expected log containing '{expectedMessage}'");
        }

        [Fact]
        public void Read_ValidValues_ReturnsExpectedSettings()
        {
            // Arrange
            var root = BuildMockHierarchy(new Dictionary<string, object>
            {
                { "Server", "ool.ntp.org" },
                { "Port", 23 },
                { "PollIntervalHours", 1 }
            });
            var logger = new Mock<ILogger>();
            var provider = new RegistrySettingsProvider("MyService", logger.Object, root);

            // Act
            var settings = provider.Read();

            // Assert
            Assert.Equal("ool.ntp.org", settings.NTPServer.Get());
            Assert.Equal(23, settings.NTPPort.Get());
            Assert.Equal(1, settings.NTPPollIntervalHours.Get());
        }

        [Fact]
        public void Read_MissingRegistryKey_ReturnsDefaultSettings()
        {
            var root = new Mock<IKey>();
            root.Setup(k => k.OpenSubKey("SYSTEM")).Returns((IKey)null!); // Missing right away

            var logger = new Mock<ILogger>();
            var provider = new RegistrySettingsProvider("Missing", logger.Object, root.Object);

            var settings = provider.Read();

            Assert.Equal(Settings.DefaultNTPServer, settings.NTPServer.Get());
            Assert.Equal(Settings.DefaultNTPPort, settings.NTPPort.Get());
            Assert.Equal(Settings.DefaultNTPPollIntervalHours, settings.NTPPollIntervalHours.Get());

            VerifyLoggerCall(logger, "No settings found in registry");
        }

        [Fact]
        public void Read_InvalidValueKind_UsesDefaultAndLogs()
        {
            // Arrange
            var root = BuildMockHierarchy(new Dictionary<string, object> { { "Port", "bad" } });

            var logger = new Mock<ILogger>();
            var provider = new RegistrySettingsProvider("MyService", logger.Object, root);

            var settings = provider.Read();

            VerifyLoggerCall(logger, "Invalid data type");
        }

        [Fact]
        public void Read_UnexpectedRegistryValue_LogsAndIgnores()
        {
            var root = BuildMockHierarchy(new Dictionary<string, object>
            {
                { "Server", "ntp" },
                { "UnknownSetting", "uknown" }
            });

            var logger = new Mock<ILogger>();
            var provider = new RegistrySettingsProvider("MyService", logger.Object, root);

            var settings = provider.Read();

            Assert.Equal("ntp", settings.NTPServer.Get());

            VerifyLoggerCall(logger, "Unexpected parameter");
        }
        [Fact]
        public void Read_UnsupportedValueKind_LogsAndFallsBackToDefault()
        {
            var root = BuildMockHierarchy(new Dictionary<string, object>
            {
                { "Server", DateTime.Now } // unsupported type
            });

            var logger = new Mock<ILogger>();
            var provider = new RegistrySettingsProvider("MyService", logger.Object, root);

            var settings = provider.Read();

            Assert.Equal(Settings.DefaultNTPServer, settings.NTPServer.Get());
            VerifyLoggerCall(logger, "Invalid data type for key 'Server'");
            VerifyLoggerCall(logger, "<unsupported>");
        }

        [SupportedOSPlatform("windows")]
        [Fact]
        public void RegistryKeyWrapper_DelegatesCorrectly()
        {
            var baseKey = Registry.LocalMachine;
            var wrapper = new RegistryKeyWrapper(baseKey);

            var subkey = wrapper.OpenSubKey("SOFTWARE");
            Assert.NotNull(subkey);

            var names = wrapper.GetValueNames();
            Assert.NotNull(names);
        }

    }
}
