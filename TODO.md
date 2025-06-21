# ✅ TODO before Publishing `NtpService`

This checklist covers all recommended steps before making the repository public as part of your portfolio.

---

## ✅ Core Functionality

- [x] Modular project structure: `NtpService`, `Console`, `Library`
- [x] Raw NTP implementation over UDP with proper endian handling
- [x] System time set using Win32 `SetSystemTime`
- [x] Logging via `ILogger` with default `EventLogger` to Windows Event Log
- [x] Exception handling and error logging for WinAPI calls
- [x] Console fallback mode (`NtpServiceConsole.exe`)
- [x] Configuration via Windows Registry (`RegistrySettingsProvider`)

---

## 📁 Project Structure

- [x] Solution split into 3 projects: `Service`, `Console`, `Library`
- [x] `.gitignore` configured, no `bin/`, `obj/`, temp files committed
- [x] Consistent naming for files, classes, and namespaces
- [x] Removed temporary files (`*.InstallLog`, `.user`, test leftovers)

---

## 📝 Documentation

- [x] Professional `README.md` with build and usage instructions
- [x] Add XML documentation (`/// <summary>`) to all public APIs
- [x] Add a "Planned improvements" or Roadmap section to README
- [x] Note in README that `InstallUtil` is a temporary install method

---

## 🧪 Testing & Debugging

- [x] Console runner allows standalone testing
- [x] At least 1 unit test (e.g. `RetrieveAndSetTime` with `MockLogger`)
- [x] Ability to inject mocks for `ILogger` and `ISettingsProvider`

---

## 📸 Optional Enhancements

- [ ] Screenshot from Event Viewer showing log entries
- [ ] Example `.reg` file with default registry configuration
- [ ] Architecture diagram (e.g., UDP → NTP → Win32 API → Event Log)
- [x] Add GitHub Actions or build status badge

---

## 🌐 Repository Readiness

- [x] License file (MIT)
- [x] Author attribution in README
- [ ] Repo description and tags on GitHub
- [x] No test credentials, secrets or sensitive data
- [x] Clean build with no warnings

---

You can check off items as you go. Once complete, this project is ready to show publicly as an example of your engineering work.
