namespace NtpServiceLibrary
{
    /// <summary>
    /// Represents a strongly-typed setting value with change tracking and source information.
    /// </summary>
    /// <typeparam name="T">Type of the setting value.</typeparam>
    public class SettingsValue<T>
    {
        private T _value;
        private string _source = "";
        private bool _changed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsValue{T}"/> class with a default value.
        /// </summary>
        /// <param name="defaultValue">The default value.</param>
        public SettingsValue(T defaultValue)
        {
            _value = defaultValue;
        }

        /// <summary>
        /// Gets the current value.
        /// </summary>
        public T Get()
        {
            return _value;
        }

        public static explicit operator T(SettingsValue<T> value)
        {
            return value.Get();
        }

        /// <summary>
        /// Sets the value and marks it as changed.
        /// </summary>
        /// <param name="value">The new value.</param>
        /// <param name="source">Optional source description.</param>
        public void Set(T value, string source = "")
        {
            _value = value;
            _source = source;
            _changed = true;
        }

        /// <summary>
        /// Returns a string representation of the value, including change and source info.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                _value.ToString(),
                _changed ? "" : " (default)",
                _source != "" ? string.Format(" <{0}>", _source) : ""
            );
        }
    }

    /// <summary>
    /// Holds all configurable NTP service settings.
    /// </summary>
    public class Settings
    {
        private const string DefaultNTPServer = "pool.ntp.org";
        private const int DefaultNTPPort = 123;
        private const int DefaultNTPPollIntervalHours = 6;

        /// <summary>
        /// NTP server address.
        /// </summary>
        public SettingsValue<string> NTPServer = new SettingsValue<string>(DefaultNTPServer);

        /// <summary>
        /// NTP server port.
        /// </summary>
        public SettingsValue<int> NTPPort = new SettingsValue<int>(DefaultNTPPort);

        /// <summary>
        /// Poll interval in hours.
        /// </summary>
        public SettingsValue<int> NTPPollIntervalHours = new SettingsValue<int>(DefaultNTPPollIntervalHours);

        /// <summary>
        /// Returns a string representation of all settings.
        /// </summary>
        public override string ToString()
        {
            return string.Format("Server: {0}\nPort: {1}\nPollIntervalHours: {2}\n",
                NTPServer.ToString(),
                NTPPort.ToString(),
                NTPPollIntervalHours.ToString()
            );
        }
    }
}
