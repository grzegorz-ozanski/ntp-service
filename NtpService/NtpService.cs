using System;
using System.ServiceProcess;
using System.Timers;
using NtpServiceLibrary;

namespace NtpService
{
    /// <summary>
    /// Windows Service for synchronizing system time with an NTP server.
    /// </summary>
    public partial class NtpService : ServiceBase
    {
        private const string LogName = "Ntp Service Log";
        private const string SourceName = "Ntp Service";

        private Settings _settings;
        private Timer _timer;

        private readonly ISettingsProvider _settingsProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NtpService"/> class.
        /// </summary>
        /// <param name="settingsProvider">Optional settings provider.</param>
        /// <param name="logger">Optional logger.</param>
        public NtpService(ISettingsProvider settingsProvider = null, ILogger logger = null)
        {
            ServiceName = "NtpService";
            AutoLog = true;
            CanHandleSessionChangeEvent = true;
            CanStop = true;
            CanPauseAndContinue = false;
            InitializeComponent();
            
            _logger = logger ?? new EventLogger(LogName, SourceName);
            _settingsProvider = settingsProvider ?? new RegistrySettingsProvider(ServiceName, _logger);
        }

        /// <summary>
        /// Called when the service starts.
        /// </summary>
        /// <param name="args">Startup arguments.</param>
        protected override void OnStart(string[] args)
        {
            ServiceStatus.Set(ServiceHandle, ServiceState.SERVICE_START_PENDING);
            base.OnStart(args);

            try
            {
                _logger.Start();
                _logger.Write("Service is starting...");

                _settings = _settingsProvider.Read();

                if (_settings == null)
                {
                    throw new InvalidOperationException("Settings could not be loaded.");
                }

                _timer = InitTimer((int)_settings.NTPPollIntervalHours);
                RetrieveAndSetTime();

                ServiceStatus.Set(ServiceHandle, ServiceState.SERVICE_RUNNING);
                _logger.Write("Service started successfully.");
            }
            catch (Exception ex)
            {
                _logger.Write("Fatal error in OnStart: {0}", ex);
                // Stop service if startup could not be completed
                Stop();
            }
        }

        /// <summary>
        /// Called when a session change event occurs.
        /// </summary>
        /// <param name="changeDescription">Session change details.</param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            base.OnSessionChange(changeDescription);

            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionUnlock:
                    _logger.Write("Setting NTP time on SessionUnlock event");
                    RetrieveAndSetTime();
                    break;
            }
        }

        /// <summary>
        /// Called when the service stops.
        /// </summary>
        protected override void OnStop()
        {
            ServiceStatus.Set(ServiceHandle, ServiceState.SERVICE_STOP_PENDING);
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            base.OnStop();
            ServiceStatus.Set(ServiceHandle, ServiceState.SERVICE_STOPPED);
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            RetrieveAndSetTime();
        }

        private void RetrieveAndSetTime()
        {
            try
            {
                string ntpServer = (string)_settings?.NTPServer;
                int ntpPort = (int)_settings?.NTPPort;

                if (string.IsNullOrWhiteSpace(ntpServer))
                {
                    _logger.Write("NTP server not configured.");
                    return;
                }

                DateTime ntpTime = NtpTime.RetrieveNTPTime(ntpServer, ntpPort);
                _logger.Write("Received NTP time from {0}: {1}", ntpServer, ntpTime);

                try
                {
                    Win32SystemTime systemTime = new Win32SystemTime(new SystemTimeProvider());
                    systemTime.Set(ntpTime);
                    DateTime localTime = systemTime.GetLocal();
                    _logger.Write("System time successfully set to: {0}", localTime);
                }
                catch (Exception ex)
                {
                    _logger.Write("Failed to set system time: {0}", ex.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Write("Failed to retrieve time from NTP server: {0}", ex.Message);
            }
        }
        private Timer InitTimer(int hours)
        {
            TimeSpan timespan = new TimeSpan(hours, 0, 0);
            Timer timer = new Timer(timespan.TotalMilliseconds);
            timer.Elapsed += OnTimerElapsed;
            timer.Start();
            _logger.Write("Timer set to {0}", timespan.Format());
            return timer;
        }
    }
}
