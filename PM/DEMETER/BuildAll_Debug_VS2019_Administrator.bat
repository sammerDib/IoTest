
@echo off
pushd "%~dp0"

@echo *******************************************************************************
@echo FOCUS QUALITY COMPILATION
@echo *******************************************************************************
ren FocusQuality\FocusQuality.sln_ FocusQuality.sln
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" FocusQuality\FocusQuality.sln /build Debug
if errorlevel 1 goto :error_FocusQuality
ren FocusQuality\FocusQuality.sln FocusQuality.sln_

@echo.
@echo *******************************************************************************
@echo HOLO3-GLOBALTOPO COMPILATION
@echo *******************************************************************************
ren Tools\UnitySC.PM.PSD.Tools.Holo3\UnitySC.PM.PSD.Tools.Holo3.sln_ UnitySC.PM.PSD.Tools.Holo3.sln
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" Tools\UnitySC.PM.PSD.Tools.Holo3\UnitySC.PM.PSD.Tools.Holo3.sln /rebuild Debug
if errorlevel 1 goto :error_Holo3-GlobalTopo
ren Tools\UnitySC.PM.PSD.Tools.Holo3\UnitySC.PM.PSD.Tools.Holo3.sln UnitySC.PM.PSD.Tools.Holo3.sln_

@echo.
@echo *******************************************************************************
@echo NANO CALIB COMPILATION
@echo *******************************************************************************
ren Tools\UnitySC.PM.PSD.Tools.TopoCalib\UnitySC.PM.PSD.Tools.TopoCalib.sln_ UnitySC.PM.PSD.Tools.TopoCalib.sln
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" Tools\UnitySC.PM.PSD.Tools.TopoCalib\UnitySC.PM.PSD.Tools.TopoCalib.sln /rebuild Debug
if errorlevel 1 goto :error_NanoCalib
ren Tools\UnitySC.PM.PSD.Tools.TopoCalib\UnitySC.PM.PSD.Tools.TopoCalib.sln UnitySC.PM.PSD.Tools.TopoCalib.sln_

@echo.
@echo *******************************************************************************
@echo PSD COMPILATION
@echo *******************************************************************************
"C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\Common7\IDE\devenv.com" PSD.sln /rebuild Debug
if errorlevel 1 goto :error_PSD

@echo.
@echo *******************************************************************************
@echo COMPILATION TERMINATED WITH SUCCESS
@echo *******************************************************************************
pause
goto :end

:error_FocusQuality
@echo.
@echo *******************************************************************************
@echo ERROR IN FOCUS_QUALITY
@echo *******************************************************************************
pause
goto :end

:error_PSD
@echo.
@echo *******************************************************************************
@echo ERROR IN PSD
@echo *******************************************************************************
pause
goto :end

:error_Holo3-GlobalTopo
@echo.
@echo *******************************************************************************
@echo ERROR IN HOLO3-GLOBALTOPO
@echo *******************************************************************************
pause
goto :end

:error_NanoCalib
@echo.
@echo *******************************************************************************
@echo ERROR IN NANOCALIB
@echo *******************************************************************************
pause
goto :end


:end
