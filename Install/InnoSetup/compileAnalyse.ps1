
$ScriptDir = Split-Path -parent $MyInvocation.MyCommand.Path
$PathToISS = "$ScriptDir\AnalyseInstaller.iss"
Import-Module $ScriptDir\compileGeneric.psm1 -Force

LaunchCompilation -PathToISS $PathToISS -ScriptDir $ScriptDir