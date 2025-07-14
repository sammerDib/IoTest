# ANALYSE client application

The client application needs a data storage and a backend server to run.

## The DataStorage
The data storage service is implemented in the `DataAccess` solution, which is outside `ANALYSE` solution.
Note that this service will connect a concrete database server at Unity office, so that being at the office
or using the VPN is mandatory.

## The backend server
The server is implemented in the `Service/UnitySC.PM.ANA.Host` project of `ANALYSE` solution.
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
The client is implemented in the `Client/UnitySC.PM.ANA.Client` project of `ANALYSE` solution.
Use command line arguments to define configuration and simulation.
Options:
  -c, --config <config>  [Optional] Use a specific configuration. If option is not defined the computer name is used
  --version              Show version information
  -?, -h, --help         Show help and usage information

Ex: -c 4MET2223

## All together

The data strorage has its own life. The preffered way is to compile the executable and keep it somewhere. 
It can then be started when needed.
The server and the client can be launched together from Visual Studio to allow debugging of each side, using simultaneous starting projects.
See [here](https://docs.microsoft.com/en-us/visualstudio/ide/how-to-set-multiple-startup-projects?view=vs-2022) for details on how to launch multiple projects simultaneously.
