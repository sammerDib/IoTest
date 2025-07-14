@echo off
if not exist "C:\_LocalNuGetPack" echo "Create Local Directory => C:\_LocalNuGetPack
if not exist "C:\_LocalNuGetPack" mkdir C:\_LocalNuGetPack
@echo on
nuget pack -Verbosity detailed -OutputDirectory C:\_LocalNuGetPack -properties Configuration=Release;Plateforme=x64 
pause
