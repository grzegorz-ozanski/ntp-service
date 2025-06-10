param (
    [string]$ServiceName = "MyService",
    [string]$ServiceExe = "C:\Program Files\MyService\MyService.exe",
    [string]$SourcePath = "$PSScriptRoot\MyService",
    [switch]$ForceReinstall
)

$InstallPath = [System.IO.Path]::GetDirectoryName($ServiceExe)

# Ensure script runs with admin privileges
Function Test-Admin {
    $wid = [System.Security.Principal.WindowsIdentity]::GetCurrent()
    $prp = New-Object System.Security.Principal.WindowsPrincipal($wid)
    $adminRole = [System.Security.Principal.WindowsBuiltInRole]::Administrator
    return $prp.IsInRole($adminRole)
}

if (-not (Test-Admin)) {
    Write-Host "Restarting as Administrator..."
    Start-Process powershell -ArgumentList "-ExecutionPolicy Bypass -File `"$PSCommandPath`" -ServiceName `"$ServiceName`" -ServiceExe `"$ServiceExe`" -SourcePath `"$SourcePath`" $ForceReinstall" -Verb RunAs
    exit
}

# Stop and remove existing service if it exists or if forced reinstall is enabled
if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) {
    Write-Host "Stopping existing service..."
    Stop-Service -Name $ServiceName -Force
    sc.exe delete $ServiceName
    Start-Sleep -Seconds 3
}

# Remove old files if forced reinstall
if ($ForceReinstall -and (Test-Path $InstallPath)) {
    Write-Host "Removing old service files..."
    Remove-Item -Path $InstallPath -Recurse -Force
}

# Copy new files
Write-Host "Copying new binaries from $SourcePath to $InstallPath..."
New-Item -Path $InstallPath -ItemType Directory -Force
Copy-Item -Path "$SourcePath\*" -Destination $InstallPath -Recurse -Force

# Install new service
Write-Host "Installing service..."
sc.exe create $ServiceName binPath= "$ServiceExe" start= auto
sc.exe failure $ServiceName reset=86400 actions=restart/5000/restart/5000/restart/5000

# Start service
Start-Service -Name $ServiceName

Write-Host "Service '$ServiceName' installed and started successfully!"
