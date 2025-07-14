# ANALYSE
ANALYSE need backend server, frontend client and a connexion to DataAccess (application hosted in Unity server) 
To start client and server with simulated hardware and 4MET2223 configuration use LaunchANA.cmd
default user/pass : unity/unity

## The backend server
The server is `UnitySC.PM.ANA.Service.Host.exe`
Use command line arguments to define configuration and simulation.
Options:
  -c, --config <config>    [Optional] Use a specific configuration. If option is not defined the computer name is used
  -sh, --simulateHardware  [Optional] Simulate all hardwares. If option is not defined IsSimulated defined in
                           HardwareConfiguration are used
  -sf, --simulateFlow      [Optional] Simulate all flows. If option is not defined the flows are not simulate
  -rf, --reportAllFlow     [Optional] Enable all flows report. If option is not defined WriteReportIsEnabled defined in FlowsConfiguration are used
  --version                Show version information
  -?, -h, --help           Show help and usage information

Ex: -c 4MET2223 -sh -sf -rf


## The frontend client
The client is `ANAClient\Application\UnitySC.PM.ANA.Client.exe`
Use command line arguments to define configuration and simulation.
Options:
  -c, --config <config>  [Optional] Use a specific configuration. If option is not defined the computer name is used
  --version              Show version information
  -?, -h, --help         Show help and usage information

Ex: -c 4MET2223

