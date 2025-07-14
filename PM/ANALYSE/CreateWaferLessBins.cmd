echo "--------------------------------------"  
echo "Create Wafer Less Binary files        "
echo "--------------------------------------"  
copy  /Y .\Client\UnitySC.PM.ANA.Client\bin\Debug\UnitySC.PM.ANA.Client.exe .\Client\UnitySC.PM.ANA.Client\bin\Debug\UnitySC.PM.ANA.Client.Wl.exe 
copy  /Y .\Client\UnitySC.PM.ANA.Client\bin\Debug\UnitySC.PM.ANA.Client.exe.config .\Client\UnitySC.PM.ANA.Client\bin\Debug\UnitySC.PM.ANA.Client.Wl.exe.config

copy  /Y .\Service\UnitySC.PM.ANA.Service.Host\bin\Debug\UnitySC.PM.ANA.Service.Host.exe .\Service\UnitySC.PM.ANA.Service.Host\bin\Debug\UnitySC.PM.ANA.Service.Host.Wl.exe
copy  /Y .\Service\UnitySC.PM.ANA.Service.Host\bin\Debug\UnitySC.PM.ANA.Service.Host.exe.config .\Service\UnitySC.PM.ANA.Service.Host\bin\Debug\UnitySC.PM.ANA.Service.Host.Wl.exe.config
pause