# UTO Controller

Source code of UTO ECs

## Deployment

To deploy this project run

```powershell
.\build-dist.cmd
```

-or-

```powershell
.\build\_build.exe --profile dist
```

-or full command-

```powershell
.\build\_build.exe Clean DistApps --configuration Release --solution UnitySC.UTO.Controller.sln
```

For more commands you can run

```pwpowershellsh
.\build\_build.exe --help
```

# Update Agileo packages
nuget push *.nupkg -src https://unitysc.pkgs.visualstudio.com/UnityControl/_packaging/USP_ExternalNugetPackage/nuget/v3/index.json -ApiKey az
