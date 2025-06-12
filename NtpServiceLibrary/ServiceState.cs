using System.Runtime.InteropServices;

namespace NtpServiceLibrary
{
    /// <summary>
    /// Represents possible states of a Windows service.
    /// </summary>
    public enum ServiceState
    {
        /// <summary>Service is stopped.</summary>
        SERVICE_STOPPED = 0x00000001,
        /// <summary>Service is starting.</summary>
        SERVICE_START_PENDING = 0x00000002,
        /// <summary>Service is stopping.</summary>
        SERVICE_STOP_PENDING = 0x00000003,
        /// <summary>Service is running.</summary>
        SERVICE_RUNNING = 0x00000004,
        /// <summary>Service is resuming from a paused state.</summary>
        SERVICE_CONTINUE_PENDING = 0x00000005,
        /// <summary>Service is pausing.</summary>
        SERVICE_PAUSE_PENDING = 0x00000006,
        /// <summary>Service is paused.</summary>
        SERVICE_PAUSED = 0x00000007,
    }

    /// <summary>
    /// Contains information about the current status of a Windows service.
    /// Used for interop with the Windows Service Control Manager.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStateInfo
    {
        /// <summary>Type of the service.</summary>
        public int dwServiceType;
        /// <summary>Current state of the service.</summary>
        public ServiceState dwCurrentState;
        /// <summary>Controls accepted by the service.</summary>
        public int dwControlsAccepted;
        /// <summary>Win32 exit code.</summary>
        public int dwWin32ExitCode;
        /// <summary>Service-specific exit code.</summary>
        public int dwServiceSpecificExitCode;
        /// <summary>Checkpoint value for lengthy operations.</summary>
        public int dwCheckPoint;
        /// <summary>Estimated time required for a pending operation, in milliseconds.</summary>
        public int dwWaitHint;
    };

    /// <summary>
    /// Provides methods for updating the service status with the Windows Service Control Manager.
    /// </summary>
    public class ServiceStatus
    {
        private static ServiceStatus _instance = null;
        private ServiceStateInfo _serviceStateInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStatus"/> class.
        /// </summary>
        private ServiceStatus()
        {
            _serviceStateInfo = new ServiceStateInfo
            {
                dwWaitHint = 100000                
            };
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="ServiceStatus"/>.
        /// </summary>
        /// <returns>The singleton instance.</returns>
        private static ServiceStatus GetInstance()
        {
            if (_instance == null)
                _instance = new ServiceStatus();
            return _instance;
        }

        /// <summary>
        /// Sets the current service status using the Windows API.
        /// </summary>
        /// <param name="handle">A handle to the service status.</param>
        /// <param name="state">The new state to set.</param>
        /// <returns>True if the status was set successfully; otherwise, false.</returns>
        public static bool Set(System.IntPtr handle, ServiceState state)
        {
            var instance = GetInstance();
            instance._serviceStateInfo.dwCurrentState = state;
            return SetServiceStatus(handle, ref instance._serviceStateInfo);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStateInfo serviceStatus);
    }
}
