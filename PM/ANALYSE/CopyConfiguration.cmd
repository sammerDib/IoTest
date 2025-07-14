echo "--------------------------------------"  
echo "Copy configuration and calibration files"
echo "--------------------------------------"  
xcopy  %~dp0_ConfigurationAndCalibration %~dp0Service\UnitySC.PM.ANA.Service.Host\bin /Y /E
pause