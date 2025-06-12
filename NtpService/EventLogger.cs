using System;
using System.Diagnostics;
using NtpServiceLibrary;

namespace NtpService
{
    /// <summary>
    /// Provides an implementation of <see cref="ILogger"/> that writes log messages to the Windows Event Log.
    /// </summary>
    internal class EventLogger : ILogger
    {
        private EventLog _logger;
        private readonly string _source;
        private readonly string _logName;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventLogger"/> class.
        /// </summary>
        /// <param name="logName">The name of the event log.</param>
        /// <param name="sourceName">The source name for the event log entries.</param>
        public EventLogger(string logName, string sourceName)
        {
            _logName = logName;
            _source = sourceName;
            _logger = null;
        }

        /// <summary>
        /// Initializes the event logger and creates the event source if it does not exist.
        /// </summary>
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

        /// <summary>
        /// Writes a message to the Windows Event Log as an information entry.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Write(string message)
        {
            _logger.WriteEntry(message, EventLogEntryType.Information);
        }

        /// <summary>
        /// Writes a formatted message to the Windows Event Log as an information entry.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An array of objects to format.</param>
        public void Write(string format, params object[] parameters)
        {
            _logger.WriteEntry(string.Format(format, parameters), EventLogEntryType.Information);
        }
    }
}
