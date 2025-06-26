namespace NtpServiceLibrary
{
    /// <summary>
    /// Provides methods for updating the service status with the Windows Service Control Manager.
    /// </summary>
    public interface IServiceStatusProvider
    {
        /// <summary>
        /// Updates the status of the service with the Windows Service Control Manager.
        /// </summary>
        /// <param name="handle">A handle to the service status structure.</param>
        /// <param name="serviceStatus">A reference to a <see cref="ServiceStateInfo"/> structure that contains the updated status information.</param>
        /// <returns>
        /// <c>true</c> if the service status was successfully updated; otherwise, <c>false</c>.
        /// </returns>
        bool SetServiceStatus(System.IntPtr handle, ref ServiceStateInfo serviceStatus);
    }

}
