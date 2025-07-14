# USP Installer
This folder contains :
- PowerShell Script Module (.psm1) : Generic module that will be referenced by individual scripts.
- PowerShell Scripts (.ps1) : One per product (ANA, DMT, ...) that references the common module.
- Generic InnoSetup script (.iss) : Generic behaviour shared by all components
- Specific InnoSetup script (.iss) : Specific script that references the common script.

All those files are combined to create an installer for any USP product.

**_Currently, only ANALYSE is supported_**

## How to use it

Here is the expected file hierarchy to use the script :
```
.
├ compile<Product>.ps1
├ compileGeneric.psm1
├ ReleaseFile.zip (name template : *.zip)
│    ├ Bin\
│    │  └ %Usual Bin Content%
│    ├ UNLogoBigTrans.ico
│    └ Version_X.X.X.xml (name template : Version_*.xml)
├ <Product>Installer.iss
└ GenericInstaller.iss
```
To compile project, simply run the .ps1 script associated to your product. Here is the new hierarchy :
```
.
├ compile<Product>.ps1
├ compileGeneric.psm1
├ ReleaseFile.zip (name template : *.zip)
│    ├ Bin\
│    ├ UNLogoBigTrans.ico
│    └ Version_X.X.X.xml (name template : Version_*.xml)
├ Release\ 
│    └ %Content of the .zip archive%
├ Output\ 
│    └ USPInstaller.exe 
├ <Product>Installer.iss
└ GenericInstaller.iss
```
You can now deploy and use the USPInstaller.exe to install the new version of USP
