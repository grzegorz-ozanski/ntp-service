using Microsoft.Win32;

namespace NtpServiceLibrary
{
    /// <summary>
    /// Defines a contract for accessing value keys.
    /// </summary>
    public interface IKey
    {
        /// <summary>
        /// Gets the value associated with the specified name.
        /// </summary>
        /// <param name="name">key name</param>
        /// <returns>value stored under the name</returns>
        object GetValue(string name);
        /// <summary>
        /// Gets the kind of value associated with the specified name.
        /// </summary>
        /// <param name="name">key name</param>
        /// <returns>Win32 registry value kind</returns>
        RegistryValueKind GetValueKind(string name);
        /// <summary>
        /// Opens a subkey with the specified name.
        /// </summary>
        /// <param name="name">key name</param>
        /// <returns>Subkey or null if not present</returns>
        IKey OpenSubKey(string name);
        /// <summary>
        /// Retrieves the names of all values in the key.
        /// </summary>
        /// returns>A list of value names.</returns>
        string[] GetValueNames();
    }


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
