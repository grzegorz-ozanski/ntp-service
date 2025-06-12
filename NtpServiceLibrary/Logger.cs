namespace NtpServiceLibrary
{
    /// <summary>
    /// Provides logging functionality for the service.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Initializes the logger.
        /// </summary>
        void Start();

        /// <summary>
        /// Writes a message to the log.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Write(string message);

        /// <summary>
        /// Writes a formatted message to the log.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="parameters">An array of objects to format.</param>
        void Write(string format, params object[] parameters);
    }
}
