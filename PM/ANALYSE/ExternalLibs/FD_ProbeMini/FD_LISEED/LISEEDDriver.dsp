# Microsoft Developer Studio Project File - Name="LISEEDDriver" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=LISEEDDriver - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "LISEEDDriver.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "LISEEDDriver.mak" CFG="LISEEDDriver - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "LISEEDDriver - Win32 Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "LISEEDDriver - Win32 Debug" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "LISEEDDriver - Win32 ReleaseD" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "LISEEDDriver - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Release"
# PROP Intermediate_Dir "Release"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "LISEEDDRIVER_EXPORTS" /YX /FD /c
# ADD CPP /nologo /Zp16 /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "LISEEDDRIVER_EXPORTS" /FD /c
# SUBTRACT CPP /YX
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386
# SUBTRACT LINK32 /debug

!ELSEIF  "$(CFG)" == "LISEEDDriver - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Debug"
# PROP Intermediate_Dir "Debug"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MTd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "LISEEDDRIVER_EXPORTS" /YX /FD /GZ /c
# ADD CPP /nologo /MTd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "LISEEDDRIVER_EXPORTS" /FR /FD /GZ /c
# SUBTRACT CPP /YX
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "_DEBUG"
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /debug /machine:I386 /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /debug /machine:I386 /pdbtype:sept

!ELSEIF  "$(CFG)" == "LISEEDDriver - Win32 ReleaseD"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "LISEEDDriver___Win32_ReleaseD"
# PROP BASE Intermediate_Dir "LISEEDDriver___Win32_ReleaseD"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "LISEEDDriver___Win32_ReleaseD"
# PROP Intermediate_Dir "LISEEDDriver___Win32_ReleaseD"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /Zp16 /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "LISEEDDRIVER_EXPORTS" /FD /c
# SUBTRACT BASE CPP /YX
# ADD CPP /nologo /Zp16 /MT /W3 /GX /Zi /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "LISEEDDRIVER_EXPORTS" /FD /c
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /debug /machine:I386 /out:"ReleaseD/LISEEDDriver.dll"

!ENDIF 

# Begin Target

# Name "LISEEDDriver - Win32 Release"
# Name "LISEEDDriver - Win32 Debug"
# Name "LISEEDDriver - Win32 ReleaseD"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Group "DLL Interface"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\LISE_ED_DLL_External.cpp
# End Source File
# End Group
# Begin Group "Internal"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\LISE_ED_DLL_Acquisition.cpp
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Config.cpp
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Create.cpp
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Internal.CPP
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Log.cpp
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Process.cpp
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Reglages.cpp
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_UI.CPP
# End Source File
# Begin Source File

SOURCE=.\PeakMatch.cpp
# End Source File
# End Group
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Group "DLL Interface Headers"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\LISE_ED_DLL_External.h
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_General.h
# End Source File
# End Group
# Begin Group "Internal Headers"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\LISE_ED_DLL_Acquisition.h
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Config.h
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Create.h
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Internal.h
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Log.h
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Process.h
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_Reglages.h
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_UI_Fct.H
# End Source File
# Begin Source File

SOURCE=.\LISE_ED_DLL_UI_Struct.H
# End Source File
# Begin Source File

SOURCE=.\NIDAQmx.h
# End Source File
# Begin Source File

SOURCE=.\PeakMatch.h
# End Source File
# End Group
# End Group
# Begin Group "Resource Files"

# PROP Default_Filter "ico;cur;bmp;dlg;rc2;rct;bin;rgs;gif;jpg;jpeg;jpe"
# End Group
# End Target
# End Project
