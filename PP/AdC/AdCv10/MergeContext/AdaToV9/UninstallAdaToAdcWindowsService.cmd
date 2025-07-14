@setlocal enableextensions
@cd /d "%~dp0"
NET STOP AdaToAdc
C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe /u AdaToAdc.exe
sc delete AdaToAdc
PAUSE
