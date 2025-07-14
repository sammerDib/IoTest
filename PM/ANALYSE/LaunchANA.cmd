start /d ".\Service\UnitySC.PM.ANA.Service.Host\bin\Debug\" .\Service\UnitySC.PM.ANA.Service.Host\bin\Debug\UnitySC.PM.ANA.Service.Host.exe -c 4MET2223 -sh -sf -rf 
TIMEOUT 4
start /d ".\Client\UnitySC.PM.ANA.Client\bin\Debug\" .\Client\UnitySC.PM.ANA.Client\bin\Debug\UnitySC.PM.ANA.Client.exe -c 4MET2223

pause
