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

    public interface IServiceStatusProvider
    {
        bool SetServiceStatus(System.IntPtr handle, ref ServiceStateInfo serviceStatus);
    }

    public class SystemServiceStatusProvider : IServiceStatusProvider
    {
        [DllImport("advapi32.dll", SetLastError = true, EntryPoint = "SetServiceStatus")]
        public static extern bool _SetServiceStatus(System.IntPtr handle, ref ServiceStateInfo serviceStatus);
        public bool SetServiceStatus(System.IntPtr handle, ref ServiceStateInfo serviceStatus) => _SetServiceStatus(handle, ref serviceStatus);
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
        private IServiceStatusProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceStatus"/> class.
        /// </summary>
        private ServiceStatus(IServiceStatusProvider provider)
        {
            _provider = provider ?? new SystemServiceStatusProvider();
            _serviceStateInfo = new ServiceStateInfo
            {
                dwWaitHint = 100000                
            };
        }

        /// <summary>
        /// Gets the singleton instance of <see cref="ServiceStatus"/>.
        /// </summary>
        /// <returns>The singleton instance.</returns>
        private static ServiceStatus GetInstance(IServiceStatusProvider provider)
        {
            if (_instance == null)
                _instance = new ServiceStatus(provider);
            return _instance;
        }

        /// <summary>
        /// Sets the provider.
        /// </summary>
        /// <param name="provider">Service status manipulation object.</param>
        public static void SetProvider(IServiceStatusProvider provider)
        {
            _instance._provider = provider ?? new SystemServiceStatusProvider();
        }

        /// <summary>
        /// Deletes the singleton instance (e.g. for use in testing).
        /// </summary>
        internal static void ClearInstance()
        {
            _instance = null;
        }

        /// <summary>
        /// Sets the current service status using the Windows API.
        /// </summary>
        /// <param name="handle">A handle to the service status.</param>
        /// <param name="state">The new state to set.</param>
        /// <param name="provider">Service status manipulation object.</param>
        /// <returns>True if the status was set successfully; otherwise, false.</returns>
        public static bool Set(System.IntPtr handle, ServiceState state, IServiceStatusProvider provider=null)
        {
            var instance = GetInstance(provider);
            instance._serviceStateInfo.dwCurrentState = state;
            return instance._provider.SetServiceStatus(handle, ref instance._serviceStateInfo);
        }
    }
}
