using Microsoft.Win32;
namespace NtpServiceLibrary
{
    public class RegistrySettingsProvider : ISettingsProvider
    {
        private readonly ILogger _logger;
        private readonly string _serviceName;
        private readonly Settings _settings;

        public RegistrySettingsProvider(string serviceName, ILogger logger)
        {
            _serviceName = serviceName;
            _logger = logger;
            _settings = new Settings();
        }

        private string RegistryValueKindToString(RegistryValueKind valueKind)
        {
            switch (valueKind)
            {
                case RegistryValueKind.String:
                    return "string";
                case RegistryValueKind.DWord:
                    return "dword";
                default:
                    return "<unsupported>";
            }
        }

        private string ReadValue<T>(ref SettingsValue<T> settingsValue, RegistryKey registryKey, string registryValue, 
            RegistryValueKind expectedType, string message)
        {
            var value = registryKey.GetValue(registryValue);
            var actualType = registryKey.GetValueKind(registryValue);
            if (actualType != expectedType)
            {
                return string.Format("{0}Invalid data type for key '{1}'. Expected: '{2}', actual: '{3}'. Falling back to default value {4}\n",
                    message, registryValue, RegistryValueKindToString(expectedType), RegistryValueKindToString(actualType), (T)settingsValue);
            }
            settingsValue.Set((T)value, "registry");
            return message;
        }
        public Settings Read()
        {
            RegistryKey registryKey = Registry.LocalMachine;
            string[] subkeys = { "SYSTEM", "CurrentControlSet", "Services", _serviceName, "Parameters" };
            foreach (var key in subkeys)
            {
                registryKey = registryKey.OpenSubKey(key);
            }
            if (registryKey == null)
            {
                _logger.Write("No settings found in registry, using default values:\n{0}: {1}\n{2}: {3}\n{4}: {5}",
                    "Server", (string)_settings.NTPServer,
                    "Port", (int)_settings.NTPPort,
                    "PollIntervalHours", (int)_settings.NTPPollIntervalHours
                    );
                return _settings;
            }
            string additionalMessage = "";

            foreach (var registryValue in registryKey.GetValueNames())
            {
                switch (registryValue)
                {
                    case "Server":
                        additionalMessage = ReadValue(ref _settings.NTPServer, registryKey, registryValue, 
                            RegistryValueKind.String, additionalMessage);
                        break;
                    case "Port":
                        additionalMessage = ReadValue(ref _settings.NTPPort, registryKey, registryValue, 
                            RegistryValueKind.DWord, additionalMessage);
                        break;
                    case "PollIntervalHours":
                        additionalMessage = ReadValue(ref _settings.NTPPollIntervalHours, registryKey, registryValue, 
                            RegistryValueKind.DWord, additionalMessage);
                        break;
                    default:
                        additionalMessage += string.Format("Unexpected parameter '{0}', ignoring\n", registryValue);
                        break;
                }
            }
            _logger.Write("Service settings:\n{0}{1}", _settings.ToString(), additionalMessage);
            return _settings;
        }
    }
}
