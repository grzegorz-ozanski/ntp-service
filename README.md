# ⏰ NtpService

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
- .NET Framework 4.7.2

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

## 🧪 Testing / Debug Mode

To run the service logic in console mode (for testing):

```bash
NtpServiceConsole.exe
```

You can mock the logger or override settings provider via DI in code.

## 🔍 Project Structure

```
NtpService/
├── NtpService/           # Windows Service project
│   ├── NtpService.cs     # Main service class
│   ├── ProjectInstaller* # Service installer definition
│   └── EventLogger.cs    # Event log logger
├── NtpServiceConsole/    # Console runner for debugging
├── NtpServiceLibrary/    # Shared logic: NTP, Win32 interop, settings, logging
│   ├── NtpTime.cs
│   ├── WindowsTime.cs
│   ├── Settings.cs
│   └── Logger.cs
└── README.md             # This file
```

## 🚧 Planned Improvements
* Add some debugging logs, toggled by registry settings
* Add unit tests
* Add installer


## 🙋 Author

Created by **Grzegorz Ożański**  
with a bit of ChatGPT support for formatting and polish ;)

## 📄 License

MIT License
