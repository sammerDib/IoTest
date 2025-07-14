#include "GenericInstaller.iss"

[Types]
; No specific type for ANALYSE

[Components]
Name: Analyse; Description: "Analyse"; Types: full; Flags: DisableNoUninstallWarning
Name: Analyse\Client; Description: "Analyse Client"; Types: full; Flags: DisableNoUninstallWarning
Name: Analyse\Server; Description: "Analyse Server"; Types: full; Flags: DisableNoUninstallWarning
Name: AppLauncher; Description: "AppLauncher Software"; Types: full; Flags: DisableNoUninstallWarning
Name: DataAccess; Description: "DataAccess Software"; Types: full; Flags: DisableNoUninstallWarning
Name: DataFlow; Description: "DataFlow Software"; Types: full; Flags: DisableNoUninstallWarning
Name: ResultViewer; Description: "ResultViewer Software"; Types: full; Flags: DisableNoUninstallWarning
Name: UTO; Description: "UTO Software"; Types: full; Flags: DisableNoUninstallWarning


[Files]
Source: "{#Source}\Bin\Analyse\Client\*"; DestDir: "{app}\Bin\Analyse\Client"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: Analyse\Client
Source: "{#Source}\Bin\Analyse\Server\*"; DestDir: "{app}\Bin\Analyse\Server"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: Analyse\Server
Source: "{#Source}\Bin\AppLauncher\*"; DestDir: "{app}\Bin\AppLauncher"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: AppLauncher
Source: "{#Source}\Bin\DataAccess\*"; DestDir: "{app}\Bin\DataAccess"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: DataAccess
Source: "{#Source}\Bin\Dataflow\*"; DestDir: "{app}\Bin\Dataflow"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: DataFlow
Source: "{#Source}\Bin\ResultViewer\*"; DestDir: "{app}\Bin\ResultViewer"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: ResultViewer
Source: "{#Source}\Bin\UTO\*"; DestDir: "{app}\Bin\UTO"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: UTO