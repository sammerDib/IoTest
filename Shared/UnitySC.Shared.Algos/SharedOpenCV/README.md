# Algos.SharedOpenCV

The SharedOpenCV solution is an hybrid project which contain C++ and CSharp code.
The C++ code will use third party libraries, like OpenCV for image processing.
The dependency management on these libraries is left to a tool named [VCPKG](https://vcpkg.io/en/index.html).
VCPKG is able to download binaries of third parties, as well as download and compile source code if no binary is available for specific operating system or compiler.

Azur Devops CI will correctly extract and use specific version of vcpkg


### Windows
if no vcpkg directory is installed :
- create directory such as c:\Dev\ (We recommend 'C:\src' or 'C:\Dev' paths, otherwise you may have path problems for some builds.)
- open elevated admin right cmd line at this location
- run : git clone https://github.com/Microsoft/vcpkg.git
- run : cd c:\Dev\vcpkg
- run :  git checkout 2022.04.12
	or ( git checkout e809a42f87565e803b2178a0c11263f462d1800a )
- run : bootstrap-vcpkg.bat
- run : vcpkg install opencv4:x64-windows
	// or any package to install
- run : vcpkg integrate install


#### Notes about OpenCV

version of OpenCV is automatically provided.
If you need a custom version of OpenCV, you will have to provide via vcpkg opencv submod.


## Build project

open SharedOpenCV.sln


#### Release nuget

change version file in UnitySC.Shared.Algos.OpenCVWrapper
change version in UnitySC.Shared.Algos.OpenCVWrapper.nuspec
* WARNING: if you added a new DLL dependency, make sure to also add the DLL to the files list of the NUSPEC file. See FileNotFoundException section below.
git  commit-push your modifications
launch USP_Manual_PushSharedAlgosOpenCV.yml pipeline

#### Local tests

If you want to locally test the nuget package, follow these steps:
- If you don't already have a nuget.exe file, do the following:
	- Download a nuget.exe from https://www.nuget.org/downloads (currently we use the version 6.0.0.280)
	- Put it in the C# wrapper folder alongside the file NugetPackPushLocal.bat
- Modify the .nuspec and the .rc file to set up a new version (any new unique version number will do)
- Start NugetPackPushLocal.bat: it will create the folder C:\_LocalNuGetPack if it does not exist, and compile/publish the nuget package with the new version into it
- If you don't already have added a local nuget package source to you Visual Studio:
	- Go to Tools/Options/Nuget Package Manager/Package Sources
	- Click on the "+" button to add a new source
	- Set its name to "Local packages" and its source to "C:\_LocalNuGetPack"
	- Set it up to the top of the list with the upward arrow, so that it gets picked before any other source
- Update the version of the package in your solution:
	- Right click on the solution
	- Go to Manage Nuget Packages for Solution
	- Find you package (it should be under "Updates" as you've just published a new version)
	- Update to your new version
- Now your should be able to locally use the new package.
- Don't forget to revert the version changes before commiting if you've used temporary version numbers!

#### FileNotFoundException: Could not load file or assembly 'MyDll.dll' or one of its dependencies

This runtime error will be thrown at runtime in a program that uses your nuget package.
It happens if you've added a new dependency to the Nuget package, but you forgot to add the dll to the files list of the nuspec file.
Be careful that you also have to manually add all the dependencies of your new dll!

If you want to know the list of dependencies of a dll, you can start a Visual Studio command prompt, and run the following command:
`dumpbin /dependents your_dll_file.dll`

Example:
I've added a dependency to A.dll, which itself depends on B.dll. Then, I'll have to add the 2 lines below to the nuspec file:
`<file src="..\x64\$configuration$\A.dll" target="lib\net48" />`
`<file src="..\x64\$configuration$\B.dll" target="lib\net48" />`
If I run `dumpbin /dependents A.dll`, it will return the following:
```
File Type: DLL

  Image has the following dependencies:

    B.dll
```