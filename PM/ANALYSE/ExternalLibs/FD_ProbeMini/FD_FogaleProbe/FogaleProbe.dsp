# Microsoft Developer Studio Project File - Name="FogaleProbe" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=FogaleProbe - Win32 DebugUSBOnly
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "FogaleProbe.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "FogaleProbe.mak" CFG="FogaleProbe - Win32 DebugUSBOnly"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "FogaleProbe - Win32 Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "FogaleProbe - Win32 Debug" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "FogaleProbe - Win32 DebugNoHardware" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "FogaleProbe - Win32 ReleaseNoHardware" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "FogaleProbe - Win32 ReleaseUSBOnly" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "FogaleProbe - Win32 DebugUSBOnly" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

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
# ADD BASE CPP /nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /YX /FD /c
# ADD CPP /nologo /G6 /Gr /Zp16 /MT /W3 /Ox /Ot /Oa /Og /Oi /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /FD /c
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
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386 /out:"FogaleProbe.dll"

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

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
# ADD BASE CPP /nologo /MTd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /YX /FD /GZ /c
# ADD CPP /nologo /Zp16 /MTd /W3 /Gm /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /Fr /FD /GZ /c
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
# ADD LINK32 kernel32.lib user32.lib gdi32.lib comdlg32.lib /nologo /dll /debug /machine:I386 /out:"FogaleProbe.dll" /pdbtype:sept
# SUBTRACT LINK32 /pdb:none

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "FogaleProbe___Win32_DebugNoHardware"
# PROP BASE Intermediate_Dir "FogaleProbe___Win32_DebugNoHardware"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "DebugNoHardware"
# PROP Intermediate_Dir "DebugNoHardware"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /Zp16 /MTd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /FR /FD /GZ /c
# SUBTRACT BASE CPP /YX
# ADD CPP /nologo /Zp16 /MTd /W4 /Gm /Zi /Od /Gy /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /D "NOHARDWARE" /FD /GZ /c
# SUBTRACT CPP /Fr /YX
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "_DEBUG"
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /debug /machine:I386 /out:"FogaleProbe.dll" /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib gdi32.lib comdlg32.lib /nologo /dll /pdb:none /debug /machine:I386 /out:"FogaleProbe.dll"

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "FogaleProbe___Win32_ReleaseNoHardware"
# PROP BASE Intermediate_Dir "FogaleProbe___Win32_ReleaseNoHardware"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "ReleaseNoHardware"
# PROP Intermediate_Dir "ReleaseNoHardware"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /FD /c
# SUBTRACT BASE CPP /YX
# ADD CPP /nologo /G6 /Gr /Zp16 /MT /W3 /Ox /Ot /Oa /Og /Oi /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /D "NOHARDWARE" /FD /c
# SUBTRACT CPP /YX
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386 /out:"FogaleProbe.dll"
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386 /out:"FogaleProbe.dll"

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "FogaleProbe___Win32_ReleaseUSBOnly"
# PROP BASE Intermediate_Dir "FogaleProbe___Win32_ReleaseUSBOnly"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "FogaleProbe_ReleaseUSBOnly"
# PROP Intermediate_Dir "FogaleProbe_ReleaseUSBOnly"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /MT /W3 /Ox /Ot /Oa /Og /Oi /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /D "NOHARDWARE" /FD /c
# SUBTRACT BASE CPP /YX
# ADD CPP /nologo /G6 /Gr /Zp16 /MT /W3 /Ox /Ot /Oa /Og /Oi /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /D "NOHARDWARE" /D "SPG_General_USECyUSB" /FD /c
# SUBTRACT CPP /YX
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386 /out:"FogaleProbe.dll"
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /machine:I386 /out:"FogaleProbe.dll"

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "FogaleProbe___Win32_DebugUSBOnly"
# PROP BASE Intermediate_Dir "FogaleProbe___Win32_DebugUSBOnly"
# PROP BASE Ignore_Export_Lib 0
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "FogaleProbe_DebugUSBOnly"
# PROP Intermediate_Dir "FogaleProbe_DebugUSBOnly"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /Zp16 /MTd /W4 /Gm /Zi /Od /Gy /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /D "NOHARDWARE" /FD /GZ /c
# SUBTRACT BASE CPP /Fr /YX
# ADD CPP /nologo /Zp16 /MTd /W4 /Gm /Zi /Od /Gy /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "FOGALEPROBE_EXPORTS" /D "NOHARDWARE" /D "SPG_General_USECyUSB" /FD /GZ /c
# SUBTRACT CPP /Fr /YX
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "_DEBUG"
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib comdlg32.lib /nologo /dll /pdb:none /debug /machine:I386 /out:"FogaleProbe.dll"
# ADD LINK32 kernel32.lib user32.lib gdi32.lib comdlg32.lib /nologo /dll /pdb:none /debug /machine:I386 /out:"FogaleProbe.dll"

!ENDIF 

# Begin Target

# Name "FogaleProbe - Win32 Release"
# Name "FogaleProbe - Win32 Debug"
# Name "FogaleProbe - Win32 DebugNoHardware"
# Name "FogaleProbe - Win32 ReleaseNoHardware"
# Name "FogaleProbe - Win32 ReleaseUSBOnly"
# Name "FogaleProbe - Win32 DebugUSBOnly"
# Begin Group "Source Files"

# PROP Default_Filter ""
# Begin Group "FogaleProbeInternal"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\FogaleProbe.cpp
# End Source File
# Begin Source File

SOURCE=.\FogaleProbeCommonInterface.h
# End Source File
# Begin Source File

SOURCE=.\FogaleProbeDouble.cpp
# End Source File
# Begin Source File

SOURCE=.\FogaleProbeHelper.cpp
# End Source File
# Begin Source File

SOURCE=.\FogaleProbeInternal.h
# End Source File
# Begin Source File

SOURCE=.\NIDAQmxConfig.h
# End Source File
# End Group
# Begin Group "FP_LISE_General"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\FP_LISE_General\LISE_Process.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_General\LISE_Process.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_General\LISE_Struct.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_General\LISE_Struct_Process.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_General\PeakMatch.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_General\PeakMatch.h
# End Source File
# End Group
# Begin Group "FP_LISELS"

# PROP Default_Filter ""
# Begin Group "FP_LISELS_ExcludeFromBuild"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\FP_LISELS\Acquisition.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Environnement.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\General.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\InitialisationGlobale.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_DLL.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Main.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\SourcesOptique.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\SwitchOptique.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Source File

SOURCE=..\FP_LISELS\Acquisition.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Analog.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Analog.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Buffer.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Buffer.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Chariot.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Chariot.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Config.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Config.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\ConfigMessage.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\ConfigMessage.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Environnement.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Erreur.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Erreur.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\General.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\InitialisationGlobale.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LS_DLL.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LS_DLL.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_DLL.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_DLL_Internal.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_DLL_Internal.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_DLL_Process.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_DLL_Process.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_DLL_UI.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_DLL_UI.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\LISE_LSLI_ReturnValue.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Log.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Log.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Moteur.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Moteur.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Process.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Process.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Save.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Save.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\SourcesOptique.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Switch.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Switch.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\SwitchOptique.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\VariableGlobale.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Zoom.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISELS\Zoom.h
# End Source File
# End Group
# Begin Group "FP_LISEED"

# PROP Default_Filter ""
# Begin Group "FP_LiseED_ExcludeFromBuild"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_External.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\Main.cpp

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Acquisition.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Acquisition.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Config.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Config.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Create.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Create.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_External.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_General.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Internal.CPP
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Internal.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Log.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Log.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Process.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Process.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Reglages.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_Reglages.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_UI.CPP
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_UI_Fct.H
# End Source File
# Begin Source File

SOURCE=..\FP_LISEED\LISE_ED_DLL_UI_Struct.H
# End Source File
# End Group
# Begin Group "FP_CHROM"

# PROP Default_Filter ""
# Begin Group "FP_CHROM_ExcludeFromBuild"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\FP_CHROM\CHR_DLL_Real.CPP

!IF  "$(CFG)" == "FogaleProbe - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseNoHardware"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "FogaleProbe - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Source File

SOURCE=..\FP_CHROM\cDark.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\cDark.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\CHR_DLL.H
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\CHR_DLL_Emul.CPP
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\CHR_DLL_Internal.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\CHR_Double_Module.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\CHR_Double_Module.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\CHR_Module.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\CHR_Module.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\Chrom.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\Chrom.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\ChromConfig.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\ChromConfiguration.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\ChromConfiguration.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\ChromDoubleConfiguration.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\ChromDoubleConfiguration.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\ChromReturnValues.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\Comm.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\Comm.h
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\cWhiteRef.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_CHROM\cWhiteRef.h
# End Source File
# End Group
# Begin Group "FP_LenScan"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\FP_LenScan\GBPMConstants.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\GBPMKernelMapping.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanDemodulation.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanDemodulation.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanHardware.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanHardware.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanMeas.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanMeas.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanNoHardware.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanProbe.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanProbe.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanSignal.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanSignal.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanStruct.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanUI.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LenScanUI.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LMatch.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LMatch.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\LMatchEx.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\MaterialDatabase.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\MaterialDatabase.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScan\MaterialDatabaseFile.cpp
# End Source File
# End Group
# Begin Group "FP_LenScanMotorAxis"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxis.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxis.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisC844.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisC844.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisTHK.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisTHK.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisVenus1.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisVenus1.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisWindow.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisWindow.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisWindowXY.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\MotorAxisWindowXY.h
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\SPG_HippieGraphics.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LenScanMotorAxis\SPG_HippieGraphics.h
# End Source File
# End Group
# Begin Group "NIDAQmx_SDK"

# PROP Default_Filter ""
# Begin Source File

SOURCE="..\NIDAQ_SDK\DAQmx ANSI C Dev\include\NIDAQmx.h"
# End Source File
# Begin Source File

SOURCE=..\NIDAQ_SDK\SPG_NIDAQmxConfig.h
# End Source File
# End Group
# Begin Group "CypressUSB_SDK"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\CypressUSB_SDK\CyAPI.h
# End Source File
# End Group
# Begin Group "HASP_SDK"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\HASP_SDK\HASP.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\HASP.h
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\hasp_hl.h
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\hasp_hl_cpp.h
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\hasp_hl_cpp_.h
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\haspbase.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\haspfeature.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\haspfile.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\hasphandle.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\hasphasp.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\haspimpl.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\haspinfo.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\hasplegacy.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\hasplock.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\haspmain.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\haspmap.cpp
# End Source File
# Begin Source File

SOURCE=..\HASP_SDK\hasptime.cpp
# End Source File
# End Group
# Begin Group "FP_DBLLISEED"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\FP_DBLLISEED\DBL_LISE_ED_DLL_Config.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_DBLLISEED\DBL_LISE_ED_DLL_Config.h
# End Source File
# Begin Source File

SOURCE=..\FP_DBLLISEED\DBL_LISE_ED_DLL_Internal.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_DBLLISEED\DBL_LISE_ED_DLL_Internal.h
# End Source File
# Begin Source File

SOURCE=..\FP_DBLLISEED\DBL_LISE_ED_DLL_Log.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_DBLLISEED\DBL_LISE_ED_DLL_Log.h
# End Source File
# Begin Source File

SOURCE=..\FP_DBLLISEED\DBL_LISE_ED_DLL_Settings.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_DBLLISEED\DBL_LISE_ED_DLL_Settings.h
# End Source File
# End Group
# Begin Group "FP_LISEED_EXTENDED"

# PROP Default_Filter ""
# Begin Source File

SOURCE=..\FP_LISE_ED_EXTENDED\LISE_ED_EXT_Acquisition.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_ED_EXTENDED\LISE_ED_EXT_Acquisition.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_ED_EXTENDED\LISE_ED_EXT_Internal.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_ED_EXTENDED\LISE_ED_EXT_Internal.h
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_ED_EXTENDED\LISE_ED_EXT_Process.cpp
# End Source File
# Begin Source File

SOURCE=..\FP_LISE_ED_EXTENDED\LISE_ED_EXT_Process.h
# End Source File
# End Group
# End Group
# Begin Group "Header Files"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\FogaleProbe.def
# End Source File
# Begin Source File

SOURCE=.\FogaleProbe.h
# End Source File
# Begin Source File

SOURCE=.\FogaleProbeParamID.h
# End Source File
# Begin Source File

SOURCE=.\FogaleProbeReturnValues.h
# End Source File
# End Group
# End Target
# End Project
