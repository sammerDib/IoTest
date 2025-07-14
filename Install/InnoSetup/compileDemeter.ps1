
$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
$PathToISS = "$ScriptDir\DemeterInstaller.iss"
Import-Module $ScriptDir\compileGeneric.psm1 -Force

LaunchCompilation -PathToISS $PathToISS -ScriptDir $ScriptDir