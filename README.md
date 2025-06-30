# ⏰ NtpService

![CI](https://github.com/grzegorz-ozanski/ntp-service/actions/workflows/ntp-service.yml/badge.svg)
[![Unit tests](https://img.shields.io/badge/Unit%20tests-Passed%3A37%20Failed%3A0-brightgreen)](https://github.com/grzegorz-ozanski/ntp-service/actions/runs/15966725289)
[![Coverage](https://grzegorz-ozanski.github.io/ntp-service/coverage/badge_linecoverage.svg)](https://grzegorz-ozanski.github.io/ntp-service/coverage/)

Windows Service for periodically synchronizing the system time with an NTP server.
Designed for reliable, lightweight operation on Windows machines without requiring external time sync software.

I decided to create this project because, for some reason, on my Windows machine automatic synchronization with NTP server cannot be configured.

## ✨ Features

- ✅ Windows Service implementation using `ServiceBase`
- ✅ NTP time retrieval via low-level UDP client
- ✅ Native system time setting through Win32 API
- ✅ EventLog-based logging (or pluggable logger)
- ✅ Registry-based or injectable configuration
- ✅ Optional console runner for testing/debugging

## 🖥️ Requirements

- Windows 10 or newer (admin privileges required)
- .NET 8.0 (for console runner)

## ⚙️ Configuration

The service reads its settings from the Windows Registry by default, including:

- NTP server (e.g. `pool.ntp.org`)
- Port (default: `123`)
- Sync interval in hours

You can modify this logic by implementing your own `ISettingsProvider`.

## 📦 Build

You can build the solution in Visual Studio 2019+ or with `msbuild`:

```powershell
msbuild NtpService.sln /p:Configuration=Release
```

## 🚀 Installation (temporary method)

Until a proper installer is provided, you can register the service using `InstallUtil`.For example, 
assuming that `InstallUtil` is installed in `C:\Windows\Microsoft.NET\Framework\v4.0.30319`, the command 
to install the `Release` service build would look like this:

```powershell
powershell -Command "Start-Process cmd -ArgumentList ('/c', 
  'C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe /u C:\Progra~1\NtpService\NtpService.exe ^
  & rmdir /s /q C:\Progra~1\NtpService ^
  & mkdir C:\Progra~1\NtpService ^
  & copy NtpService\bin\Release\NtpService.exe  C:\Progra~1\NtpService ^
  & copy NtpServiceLibrary\bin\Release\NtpServiceLibrary.dll C:\Progra~1\NtpService ^
  & C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe C:\Progra~1\NtpService\NtpService.exe ^
  & net start NtpService')" -Verb RunAs
```

> ℹ️ This approach uses hardcoded paths and requires administrative privileges. Use with caution.

### 📝 Event Log Integration

This service writes runtime logs to the Windows Event Log using a custom event log named **`"Ntp Service Log"`** and source **`"Ntp Service"`**.

#### ⚙️ Automatic Registration

The service attempts to automatically register the event log and source during startup if they do not exist:

```csharp
if (!EventLog.SourceExists(_source))
{
    EventLog.CreateEventSource(_source, _logName);
}
```
where `_source` is `"Ntp Service"` and `_logName` is `"Ntp Service Log"`.

> 🛑 **Administrator rights are required** to register a new event log source. This should happen only once, e.g. during installation or first run.

#### 🔍 Viewing Logs

Once registered and an event is logged, you can view entries under:

```
Event Viewer → Applications and Services Logs → Ntp Service Log
```

The logs may not appear initially due to Event Viewer caching issues. In such a case:

1. Close and reopen Event Viewer.
2. Run the following command to ensure the log is enabled:
   ```powershell
   wevtutil sl "Ntp Service Log" /e:true
   ```

Alternatively, verify that logs are being written using:

```powershell
wevtutil qe "Ntp Service Log" /f:text /c:5
```

## 🧪 Testing / Debug Mode

To run the service logic in console mode (for testing):

```bash
NtpServiceConsole.exe
```

You can mock the logger or override settings provider via DI in code.

### Coverage Tests:
First, install the `ReportGenerator` tool globally if you haven't done so:
```bash
dotnet add package ReportGenerator --version 5.4.8
```
Then, run the tests with coverage collection enabled. The coverage results will be generated in the `NtpUnitTests/TestResults` directory.
```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:NtpUnitTests/TestResults/**/coverage.cobertura.xml -targetdir:coveragereport
```
or
```bash
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:NtpUnitTests/TestResults/**/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html,TextSummary
```
to create text coverage report as well.

## 🔍 Project Structure

```
NtpService/
├── NtpService/           # Windows Service project
│   ├── NtpService.cs     # Main service class
│   ├── ProjectInstaller* # Service installer definition placeholder
│   └── EventLogger.cs    # Event log logger
├── NtpServiceConsole/    # Console runner for debugging
├── NtpServiceConsole8/   # Console runner for debugging, .NET 8 version
├── NtpServiceLibrary/    # Shared logic: NTP, Win32 interop, settings, logging
│   ├── NtpTime.cs
│   ├── WindowsTime.cs
│   ├── Settings.cs
│   └── Logger.cs
├── NtpUnitTests/         # Basic unit tests
└── README.md             # This file
```

## 🚧 Planned Improvements
* Add some debugging logs, toggled by registry settings
* Add more unit tests
* Add installer


## 🙋 Author

Created by **Grzegorz Ożański**  
with a bit of ChatGPT support for formatting and polish ;)

## 📄 License

MIT License
