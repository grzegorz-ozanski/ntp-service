using System;
using System.Diagnostics;
using NtpServiceLibrary;

namespace NtpService
{
    internal class EventLogger : ILogger
    {
        private EventLog _logger;
        private readonly string _source;
        private readonly string _logName;
        public EventLogger(string logName, string sourceName)
        {
            _logName = logName;
            _source = sourceName;
            _logger = null;
        }

        public void Start()
        {
            if (!EventLog.SourceExists(_source))
            {
                EventLog.CreateEventSource(_source, _logName);
            }

            _logger = new EventLog()
            {
                Log = _logName,
                Source = _source
            };
        }

        public void Write(string message)
        {
            _logger.WriteEntry(message, EventLogEntryType.Information);
        }

        public void Write(string format, params object[] parameters)
        {
            _logger.WriteEntry(string.Format(format, parameters), EventLogEntryType.Information);
        }
    }
}
