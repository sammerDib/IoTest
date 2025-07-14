@echo off
if not exist "C:\_LocalNuGetPack" echo "Create Local Directory => C:\_LocalNuGetPack
if not exist "C:\_LocalNuGetPack" mkdir C:\_LocalNuGetPack
@echo on
nuget pack UnitySC.Shared.Algos.CppWrapper.nuspec -Verbosity detailed -OutputDirectory C:\_LocalNuGetPack -build -properties Configuration=Release;Plateforme=x64
pause
