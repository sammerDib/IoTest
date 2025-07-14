#include "GenericInstaller.iss"

[Types]

[Components]
Name: Demeter; Description: "Demeter"; Types: full; Flags: DisableNoUninstallWarning
Name: Demeter\Client; Description: "Demeter Client"; Types: full; Flags: DisableNoUninstallWarning
Name: Demeter\Server; Description: "Demeter Server"; Types: full; Flags: DisableNoUninstallWarning
Name: AppLauncher; Description: "AppLauncher Software"; Types: full; Flags: DisableNoUninstallWarning
Name: DataAccess; Description: "DataAccess Software"; Types: full; Flags: DisableNoUninstallWarning
Name: DataFlow; Description: "DataFlow Software"; Types: full; Flags: DisableNoUninstallWarning
Name: AdC; Description: "AdC"; Types: full; Flags: DisableNoUninstallWarning
Name: ResultViewer; Description: "ResultViewer Software"; Types: full; Flags: DisableNoUninstallWarning
Name: UTO; Description: "UTO Software"; Types: full; Flags: DisableNoUninstallWarning


[Files]
Source: "{#Source}\Bin\Demeter\Client\*"; DestDir: "{app}\Bin\Demeter\Client"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: Demeter\Client
Source: "{#Source}\Bin\Demeter\Server\*"; DestDir: "{app}\Bin\Demeter\Server"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: Demeter\Server
Source: "{#Source}\Bin\AppLauncher\*"; DestDir: "{app}\Bin\AppLauncher"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: AppLauncher
Source: "{#Source}\Bin\DataAccess\*"; DestDir: "{app}\Bin\DataAccess"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: DataAccess
Source: "{#Source}\Bin\Dataflow\*"; DestDir: "{app}\Bin\Dataflow"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: DataFlow
Source: "{#Source}\Bin\AdC\*"; DestDir: "{app}\Bin\AdC"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: AdC
Source: "{#Source}\Bin\ResultViewer\*"; DestDir: "{app}\Bin\ResultViewer"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: ResultViewer
Source: "{#Source}\Bin\UTO\*"; DestDir: "{app}\Bin\UTO"; Flags: promptifolder recursesubdirs createallsubdirs; Check: CheckReleasePresence; Components: UTO