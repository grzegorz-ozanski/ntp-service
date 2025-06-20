using Microsoft.Win32;

namespace NtpServiceLibrary
{
    /// <summary>
    /// Provides a wrapper around the Windows Registry key to implement the IKey interface.
    /// </summary>
    internal class RegistryKeyWrapper : IKey
    {
        private readonly RegistryKey _registryKey;
        public RegistryKeyWrapper(RegistryKey registryKey)
        {
            _registryKey = registryKey;
        }
        public object GetValue(string name) => _registryKey.GetValue(name);
        public RegistryValueKind GetValueKind(string name) => _registryKey.GetValueKind(name);
        public IKey OpenSubKey(string name)
        {
            var subkey = _registryKey.OpenSubKey(name);
            return subkey != null ? new RegistryKeyWrapper(subkey) : null;
        }
        public string[] GetValueNames() => _registryKey.GetValueNames();
    }

    /// <summary>
    /// Provides a settings provider that reads NTP service configuration from the Windows Registry.
    /// </summary>
    public class RegistrySettingsProvider : ISettingsProvider
    {
        private readonly ILogger _logger;
        private readonly string[] _subKeys;
        private readonly Settings _settings;
        private readonly IKey _registryKey;


        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrySettingsProvider"/> class.
        /// </summary>
        /// <param name="serviceName">The name of the service whose settings are to be read.</param>
        /// <param name="logger">Logger instance for logging events and errors.</param>
        /// <param name="registryKey">Root node of registry do use, or null if the Windows default (HKLM) is to be used.</param>
        public RegistrySettingsProvider(string serviceName, ILogger logger, IKey registryKey = null)
        {
            _subKeys = new string[] { "SYSTEM", "CurrentControlSet", "Services", serviceName, "Parameters" };
            _logger = logger;
            _settings = new Settings();
            _registryKey = registryKey ?? new RegistryKeyWrapper(Registry.LocalMachine);
        }

        /// <summary>
        /// Converts a <see cref="RegistryValueKind"/> to its string representation.
        /// </summary>
        /// <param name="valueKind">The registry value kind.</param>
        /// <returns>String representation of the registry value kind.</returns>
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

        /// <summary>
        /// Reads a value from the registry and assigns it to the specified <see cref="SettingsValue{T}"/>.
        /// Logs a message if the value type does not match the expected type.
        /// </summary>
        /// <typeparam name="T">The type of the setting value.</typeparam>
        /// <param name="settingsValue">Reference to the settings value to assign.</param>
        /// <param name="registryKey">The registry key containing the value.</param>
        /// <param name="registryValue">The name of the registry value.</param>
        /// <param name="expectedType">The expected registry value type.</param>
        /// <param name="message">Additional message for logging.</param>
        /// <returns>Updated log message.</returns>
        private string ReadValue<T>(ref SettingsValue<T> settingsValue, IKey registryKey, string registryValue, 
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

        /// <summary>
        /// Reads the NTP service settings from the Windows Registry.
        /// If the registry key or values are missing, default values are used.
        /// </summary>
        /// <returns>A <see cref="Settings"/> object containing the loaded or default settings.</returns>
        public Settings Read()
        {
            IKey registryKey = _registryKey;
            foreach (var key in _subKeys)
            {
                registryKey = registryKey.OpenSubKey(key);
                if (registryKey == null)
                {
                    _logger.Write("No settings found in registry, using default values:\n{0}: {1}\n{2}: {3}\n{4}: {5}",
                        "Server", (string)_settings.NTPServer,
                        "Port", (int)_settings.NTPPort,
                        "PollIntervalHours", (int)_settings.NTPPollIntervalHours
                        );
                    return _settings;
                }
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
