namespace NtpServiceLibrary
{
    public class SettingsValue<T>
    {
        private T _value;
        private string _source = "";
        private bool _changed = false;
        public SettingsValue(T defaultValue) {
            _value = defaultValue;
        }

        public T Get()
        {
            return _value;
        }

        public static explicit operator T(SettingsValue<T> value)
        {
            return value.Get();
        }

        public void Set(T value, string source="")
        {
            _value = value;
            _source = source;
            _changed = true;
        }
        public override string ToString()
        {
            return string.Format("{0}{1}{2}",
                _value.ToString(),
                _changed ? "" : " (default)",
                _source != "" ? string.Format(" <{0}>", _source) : ""
            );
        }

    }
    public class Settings
    {
        private const string DefaultNTPServer = "pool.ntp.org";
        private const int DefaultNTPPort = 123;
        private const int DefaultNTPPollIntervalHours = 6;

        public SettingsValue<string> NTPServer = new SettingsValue<string>(DefaultNTPServer);
        public SettingsValue<int> NTPPort = new SettingsValue<int>(DefaultNTPPort);
        public SettingsValue<int> NTPPollIntervalHours = new SettingsValue<int>(DefaultNTPPollIntervalHours);

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
