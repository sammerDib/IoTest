# Microsoft Developer Studio Project File - Name="Unwrapping" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Static Library" 0x0104

CFG=Unwrapping - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "Unwrapping.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "Unwrapping.mak" CFG="Unwrapping - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "Unwrapping - Win32 Release" (based on "Win32 (x86) Static Library")
!MESSAGE "Unwrapping - Win32 Debug" (based on "Win32 (x86) Static Library")
!MESSAGE "Unwrapping - Win32 Debug the Release" (based on "Win32 (x86) Static Library")
!MESSAGE "Unwrapping - Win32 Release_PII" (based on "Win32 (x86) Static Library")
!MESSAGE "Unwrapping - Win32 Release_PIII" (based on "Win32 (x86) Static Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=xicl6.exe
RSC=rc.exe

!IF  "$(CFG)" == "Unwrapping - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Unwrapping_Release"
# PROP Intermediate_Dir "Unwrapping_Release"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD BASE RSC /l 0x40c
# ADD RSC /l 0x40c
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"Unwrapping.lib"

!ELSEIF  "$(CFG)" == "Unwrapping - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Unwrapping_Debug"
# PROP Intermediate_Dir "Unwrapping_Debug"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /Z7 /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /Zp16 /W4 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /FD /c
# SUBTRACT CPP /YX
# ADD BASE RSC /l 0x40c
# ADD RSC /l 0x40c
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"Unwrappingd.lib"

!ELSEIF  "$(CFG)" == "Unwrapping - Win32 Debug the Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Debug the Release"
# PROP BASE Intermediate_Dir "Debug the Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "DebugTheRelease"
# PROP Intermediate_Dir "DebugTheRelease"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Zi /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms /c
# ADD BASE RSC /l 0x40c
# ADD RSC /l 0x40c
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"Unwrappingdr.lib"

!ELSEIF  "$(CFG)" == "Unwrapping - Win32 Release_PII"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Unwrapping___Win32_Release_PII"
# PROP BASE Intermediate_Dir "Unwrapping___Win32_Release_PII"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Unwrapping_Release_PII"
# PROP Intermediate_Dir "Unwrapping_Release_PII"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -Qpc32 -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD BASE RSC /l 0x40c
# ADD RSC /l 0x40c
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"Unwrapping.lib"
# ADD LIB32 /nologo /out:"Unwrapping_PII.lib"

!ELSEIF  "$(CFG)" == "Unwrapping - Win32 Release_PIII"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Unwrapping___Win32_Release_PIII"
# PROP BASE Intermediate_Dir "Unwrapping___Win32_Release_PIII"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Unwrapping_Release_PIII"
# PROP Intermediate_Dir "Unwrapping_Release_PIII"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD BASE RSC /l 0x40c
# ADD RSC /l 0x40c
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"Unwrapping.lib"
# ADD LIB32 /nologo /out:"Unwrapping_PIII.lib"

!ENDIF 

# Begin Target

# Name "Unwrapping - Win32 Release"
# Name "Unwrapping - Win32 Debug"
# Name "Unwrapping - Win32 Debug the Release"
# Name "Unwrapping - Win32 Release_PII"
# Name "Unwrapping - Win32 Release_PIII"
# Begin Group "Unwrapping"

# PROP Default_Filter ""
# Begin Group "Algo"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Algo\brcut.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\brcut.h
# End Source File
# Begin Source File

SOURCE=.\Algo\dipole.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\dipole.h
# End Source File
# Begin Source File

SOURCE=.\Algo\dxdygrad.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\dxdygrad.h
# End Source File
# Begin Source File

SOURCE=.\Algo\file.h
# End Source File
# Begin Source File

SOURCE=.\Algo\getqual.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\getqual.h
# End Source File
# Begin Source File

SOURCE=.\Algo\gold.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\gold.h
# End Source File
# Begin Source File

SOURCE=.\Algo\GoldsteinUnwrap.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\GoldsteinUnwrap.h
# End Source File
# Begin Source File

SOURCE=.\Algo\grad.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\grad.h
# End Source File
# Begin Source File

SOURCE=.\Algo\list.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\list.h
# End Source File
# Begin Source File

SOURCE=.\Algo\maskfat.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\maskfat.h
# End Source File
# Begin Source File

SOURCE=.\Algo\path.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\path.h
# End Source File
# Begin Source File

SOURCE=.\Algo\pi.h
# End Source File
# Begin Source File

SOURCE=.\Algo\qualgrad.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\qualgrad.h
# End Source File
# Begin Source File

SOURCE=.\Algo\QualGuidedUnwrap.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\QualGuidedUnwrap.h
# End Source File
# Begin Source File

SOURCE=.\Algo\quality.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\quality.h
# End Source File
# Begin Source File

SOURCE=.\Algo\qualpseu.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\qualpseu.h
# End Source File
# Begin Source File

SOURCE=.\Algo\qualvar.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\qualvar.h
# End Source File
# Begin Source File

SOURCE=.\Algo\residues.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\residues.h
# End Source File
# Begin Source File

SOURCE=.\Algo\util.cpp
# End Source File
# Begin Source File

SOURCE=.\Algo\util.h
# End Source File
# End Group
# Begin Source File

SOURCE=.\UW_Utils.cpp
# End Source File
# Begin Source File

SOURCE=.\UW_Utils.h
# End Source File
# End Group
# End Target
# End Project
