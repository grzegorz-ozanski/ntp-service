using System;
using NtpServiceLibrary;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("NtpUnitTests")]

namespace NtpServiceConsole8
{
    /// <summary>
    /// Provides a simple logger implementation that writes log messages to the console.
    /// </summary>
    internal class ConsoleLogger : ILogger
    {
        /// <summary>
        /// The name of the service to include in log messages.
        /// </summary>
        readonly string _serviceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLogger"/> class.
        /// </summary>
        /// <param name="serviceName">The name of the service to prefix log messages with.</param>
        public ConsoleLogger(string serviceName)
        {
            _serviceName = serviceName;
        }

        /// <summary>
        /// Initializes the logger. No action is required for the console logger.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Writes a message to the console log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Write(string message)
        {
            Console.WriteLine(string.Format("{0}: {1}", _serviceName, message));
        }

        /// <summary>
        /// Writes a formatted message to the console log.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An array of objects to format.</param>
        public void Write(string format, params object[] parameters)
        {
            Console.WriteLine(string.Format("{0}: {1}", _serviceName, string.Format(format, parameters)));
        }
    }
}
