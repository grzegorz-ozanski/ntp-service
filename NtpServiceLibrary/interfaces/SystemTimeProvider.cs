namespace NtpServiceLibrary
{
    /// <summary>
    /// Provides an interface for interacting with system time.
    /// </summary>
    public interface ISystemTimeProvider
    {
        /// <summary>
        /// Sets the system time to the specified value.
        /// </summary>
        /// <param name="st">A reference to a <see cref="Win32SystemTime.SystemTime"/> structure containing the new system time.</param>
        /// <returns>True if the system time was successfully set; otherwise, false.</returns>
        bool SetSystemTime(ref Win32SystemTime.SystemTime st);

        /// <summary>
        /// Retrieves the current system time in Coordinated Universal Time (UTC).
        /// </summary>
        /// <param name="st">A reference to a <see cref="Win32SystemTime.SystemTime"/> structure to receive the current system time.</param>
        /// <returns>True if the system time was successfully retrieved; otherwise, false.</returns>
        bool GetSystemTime(ref Win32SystemTime.SystemTime st);

        /// <summary>
        /// Retrieves the current local time.
        /// </summary>
        /// <param name="st">A reference to a <see cref="Win32SystemTime.SystemTime"/> structure to receive the current local time.</param>
        /// <returns>True if the local time was successfully retrieved; otherwise, false.</returns>
        bool GetLocalTime(ref Win32SystemTime.SystemTime st);
    }

}
