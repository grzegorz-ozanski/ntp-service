using System.Runtime.InteropServices;

namespace NtpServiceLibrary
{
    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStateInfo
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint;
    };

    public class ServiceStatus
    {
        private static ServiceStatus _instance = null;
        private ServiceStateInfo _serviceStateInfo;
        private ServiceStatus()
        {
            _serviceStateInfo = new ServiceStateInfo
            {
                dwWaitHint = 100000                
            };
        }

        private static ServiceStatus GetInstance()
        {
            if (_instance == null)
                _instance = new ServiceStatus();
            return _instance;
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStateInfo serviceStatus);

        public static bool Set(System.IntPtr handle, ServiceState state)
        {
            var instance = GetInstance();
            instance._serviceStateInfo.dwCurrentState = state;
            return SetServiceStatus(handle, ref instance._serviceStateInfo);
        }
    }
}
