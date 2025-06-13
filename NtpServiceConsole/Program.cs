///<summary>
///.NET 8.0 version of console application for NTP Service
using System;
using NtpServiceLibrary;
using NtpServiceConsole;

/// <summary>
/// Entry point for the NtpServiceConsole application.
/// This console application retrieves the current time from an NTP server and displays it.
/// It attempts to read configuration from the Windows Registry, falling back to default settings if necessary.
/// Works as a test client for the NTP service library.
/// </summary>
class Program
{
    /// <summary>
    /// Main method for the console application.
    /// Initializes logging, loads settings, retrieves NTP time, and displays results.
    /// Handles and reports errors gracefully.
    /// </summary>
    static void Main()
    {
        try
        {
            var serviceName = "NtpService";
            var logger = new ConsoleLogger(serviceName);
            Settings settings = new Settings();
            try
            {
                var settingsProvider = new RegistrySettingsProvider(serviceName, logger);
                settings = settingsProvider.Read();
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("WARNING: Registry settings for {0} does not exist, falling back to default ones:\n{1}", serviceName, settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine("WARNING: Unknown exception occured while attempting to read {0} settings, falling back to default ones.\n{1}\n" +
                    "Error details:\n{2}", serviceName, settings, ex);
            }
            if (settings != null)
            {
                var currentTime = NtpTime.RetrieveNTPTime((string)settings.NTPServer, (int)settings.NTPPort);
                Console.WriteLine("Current date and time from NTP server: " + currentTime.ToLocalTime());
            }
            else
            {
                Console.WriteLine("ERROR: Cannot get neither default nor registry settings for {0}", serviceName);
            }
            TimeSpan timeSpan = new TimeSpan(6, 0, 0);
            Console.WriteLine("{0:%h} hours {0:%m} minutes {0:%m} seconds", timeSpan);

        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }
}