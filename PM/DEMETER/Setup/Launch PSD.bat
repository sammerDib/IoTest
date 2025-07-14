@echo off
tasklist /FI "IMAGENAME eq UnitySC.DataAccess.Service.Host.exe" | find /i "UnitySC.DataAccess.Servic" 

IF ERRORLEVEL 1 GOTO LAUNCHDATAACCESS
IF ERRORLEVEL 0 GOTO NEXTPROGRAMS

:LAUNCHDATAACCESS
@echo Start DataAccess
start /d ".\DataAccess" UnitySC.DataAccess.Service.Host.exe

goto NEXTPROGRAMS

:NEXTPROGRAMS
taskkill /im UnitySC.PM.PSD.Service.Host.exe /F 2> nul

start /d ".\Server" UnitySC.PM.PSD.Service.Host.exe
start /d ".\Client" UnitySC.PM.PSD.Client.exe

