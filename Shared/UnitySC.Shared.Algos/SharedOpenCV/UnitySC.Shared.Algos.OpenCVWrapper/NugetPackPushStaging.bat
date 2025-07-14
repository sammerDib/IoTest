@echo on
nuget pack UnitySC.Shared.Algos.OpenCVWrapper.nuspec -Verbosity detailed -build -properties Configuration=Release;Plateforme=x64

##ajout personnal nugetconfig
#nuget sources update #ou add
#                     -Name HeLios_staging -src https://unitysc.pkgs.visualstudio.com/UnityControl/_packaging/HeLios_staging/nuget/v3/index.json
#                     -username rtinchi@unityscoutlook.onmicrosoft.com 
#                     -passwword uepsq4vcvh2kvdicyvflayya2xzsnch5jzkpme37vjbgdal54ulq        #pat token 
##creation du code api key
#nuget setapikey alta -source HeLios_staging 

nuget push UnitySC.Shared.Algos.OpenCVWrapper.*.nupkg -Verbosity detailed -SkipDuplicate -Source HeLios_staging -apikey alta
pause
