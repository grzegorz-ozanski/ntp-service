sc queryex NtpService | grep PID | cut -d: -f 2 | xargs taskkill "//f" "//pid"
