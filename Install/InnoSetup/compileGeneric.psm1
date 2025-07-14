$PathToCompiler = "C:\Program Files (x86)\Inno Setup 6\ISCC.exe"

Function LaunchCompilation($PathToISS, $ScriptDir){


    # Find the first (and only I hope) .zip file in the current directory
    $zipFile = Get-ChildItem -Path $ScriptDir -Filter "*.zip" | Select-Object -First 1

    # Check if the .zip file was found
    if ($null -eq $zipFile) {
        Write-Host "No .zip file found in the current directory."
        Exit -1
    }

    $executableName = $zipFile.BaseName + "-Installer"

    # Define the destination folder using the full absolute path
    $destinationFolder = Join-Path -Path $ScriptDir -ChildPath "Release"

    # Extract the .zip file to the destination folder
    Expand-Archive -Path $zipFile.FullName -DestinationPath $destinationFolder -Force

    # Write the full name of the destination folder
    Write-Host "Full path of the destination folder: $destinationFolder"

    $versionFile = Get-ChildItem -Path $destinationFolder -Filter "Version_*.xml" | Select-Object -First 1

    if ($null -eq $versionFile) {
        Write-Host "No version file found in the directory."
        Exit -1
    }
    $versionFilePath = $versionFile.Name
    $xmlDoc = [xml](Get-Content $versionFile.FullName)
    $version = $xmlDoc.USPVersion.Version.Trim('v')
    if ($null -eq $version){
        Write-Host "Impossible to read version number."
        Exit -1
    }
    Write-Host ""
    Write-Host ""
    Write-Host "Path to Compiler : $PathToCompiler"
    Write-Host "Path to ISS : $PathToISS"
    Write-Host "Version File : $versionFilePath"
    Write-Host "Version Number : $version"
    Write-Host "Source Path : $destinationFolder"
    Write-Host "Executable Name : $executableName"
    Write-Host ""
    Write-Host "$PathToCompiler $PathToISS /DMyAppVersion=$version /DSource=$destinationFolder /DVersionFile=$versionFilePath /DExecutableName=$executableName"
    Write-Host ""
    Start-Sleep -Seconds 5
    & $PathToCompiler $PathToISS /DMyAppVersion=$version /DSource=$destinationFolder /DVersionFile=$versionFilePath /DExecutableName=$executableName
    Write-Host ""
}

Export-ModuleMember -Function LaunchCompilation