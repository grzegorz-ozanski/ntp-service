namespace NtpServiceLibrary
{
    /// <summary>
    /// Defines a contract for reading NTP service settings from a configuration source.
    /// </summary>
    public interface ISettingsProvider
    {
        /// <summary>
        /// Reads and returns the NTP service settings.
        /// </summary>
        /// <returns>
        /// A <see cref="Settings"/> object containing the loaded configuration values.
        /// </returns>
        Settings Read();
    }
}
