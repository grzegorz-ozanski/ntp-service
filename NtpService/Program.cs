using System.ServiceProcess;

namespace NtpService
{
    /// <summary>
    /// Contains the main entry point for the NtpService Windows Service application.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Starts the NtpService as a Windows Service.
        /// </summary>
        static void Main()
        {
            ServiceBase.Run(new NtpService());
        }
    }
}
