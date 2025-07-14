# Microsoft Developer Studio Project File - Name="SrcC" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Static Library" 0x0104

CFG=SrcC - Win32 DebugUSBOnly
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "SrcC.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "SrcC.mak" CFG="SrcC - Win32 DebugUSBOnly"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "SrcC - Win32 Release" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Debug" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Debug the Release" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_PII" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_PIII" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_PII_Fast" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_PIII_Fast" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_P4" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_Listing" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_P4_Fast" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_Safe" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_Win98Header" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 DebugBrowse" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 Release_DebugInfo" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 DebugNoHardware" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 ReleaseNoHardware" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 ReleaseUSBOnly" (based on "Win32 (x86) Static Library")
!MESSAGE "SrcC - Win32 DebugUSBOnly" (based on "Win32 (x86) Static Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=xicl6.exe
RSC=rc.exe

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release"
# PROP Intermediate_Dir "SrcC_Release"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /G6 /Gr /Zp16 /MT /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# SUBTRACT CPP /YX
# ADD BASE RSC /l 0x40c
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"SrcC.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "SrcC_Debug"
# PROP Intermediate_Dir "SrcC_Debug"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /Z7 /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /Zp16 /MTd /W4 /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /FD /c
# SUBTRACT CPP /Fr /YX
# ADD BASE RSC /l 0x40c
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"SrcCd.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "DebugTheRelease"
# PROP BASE Intermediate_Dir "DebugTheRelease"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_DebugTheRelease"
# PROP Intermediate_Dir "SrcC_DebugTheRelease"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Zi /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD BASE RSC /l 0x40c
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"SrcCdr.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_PII"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_PII"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release_PII"
# PROP Intermediate_Dir "SrcC_Release_PII"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /MT /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC.lib"
# ADD LIB32 /nologo /out:"SrcC_PII.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_PIII"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_PIII"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release_PIII"
# PROP Intermediate_Dir "SrcC_Release_PIII"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC.lib"
# ADD LIB32 /nologo /out:"SrcC_PIII.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_PII_Fast"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_PII_Fast"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release_PII_Fast"
# PROP Intermediate_Dir "SrcC_Release_PII_Fast"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC_PII.lib"
# ADD LIB32 /nologo /out:"SrcC_PIIF.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_PIII_Fast"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_PIII_Fast"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release_PIII_Fast"
# PROP Intermediate_Dir "SrcC_Release_PIII_Fast"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC_PIIF.lib"
# ADD LIB32 /nologo /out:"SrcC_PIIIF.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_P4"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_P4"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release_P4"
# PROP Intermediate_Dir "SrcC_Release_P4"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC_PIII.lib"
# ADD LIB32 /nologo /out:"SrcC_P4.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_Listing"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_Listing"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release_Listing"
# PROP Intermediate_Dir "SrcC_Release_Listing"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FAcs /FD /P -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC_P4.lib"
# ADD LIB32 /nologo /out:"SrcC_P4r.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_P4_Fast"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_P4_Fast"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release_P4_Fast"
# PROP Intermediate_Dir "SrcC_Release_P4_Fast"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC_P4.lib"
# ADD LIB32 /nologo /out:"SrcC_P4F.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_Safe"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_Safe"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_Release_Safe"
# PROP Intermediate_Dir "SrcC_Release_Safe"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FAcs /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qrestrict /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FAcs /FD /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC.lib"
# ADD LIB32 /nologo /out:"SrcC.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_Win98Header"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_Win98Header"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC___Win32_Release_Win98Header"
# PROP Intermediate_Dir "SrcC___Win32_Release_Win98Header"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FAcs /FD /P -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FAcs /FD /P -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC_P4r.lib"
# ADD LIB32 /nologo /out:"SrcC_P4r.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "SrcC___Win32_DebugBrowse"
# PROP BASE Intermediate_Dir "SrcC___Win32_DebugBrowse"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "SrcC_DebugBrowse"
# PROP Intermediate_Dir "SrcC_DebugBrowse"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /Zp16 /W4 /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX"SPG_Includes.h" /FD /c
# SUBTRACT BASE CPP /Fr
# ADD CPP /nologo /Zp16 /W4 /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /Fr /FD /c
# SUBTRACT CPP /YX
# ADD BASE RSC /l 0x40c /d "_DEBUG"
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcCd.lib"
# ADD LIB32 /nologo /out:"SrcCd.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_Release_DebugInfo"
# PROP BASE Intermediate_Dir "SrcC___Win32_Release_DebugInfo"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC___Win32_Release_DebugInfo"
# PROP Intermediate_Dir "SrcC___Win32_Release_DebugInfo"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /YX /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# SUBTRACT CPP /YX
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC.lib"
# ADD LIB32 /nologo /out:"SrcC.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "SrcC___Win32_DebugNoHardware"
# PROP BASE Intermediate_Dir "SrcC___Win32_DebugNoHardware"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "SrcC_DebugNoHardware"
# PROP Intermediate_Dir "SrcC_DebugNoHardware"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /Zp16 /W4 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /FD /c
# SUBTRACT BASE CPP /Fr /YX
# ADD CPP /nologo /Zp16 /MTd /W4 /Gm /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /D "NOHARDWARE" /FD /c
# SUBTRACT CPP /Fr /YX
# ADD BASE RSC /l 0x40c /d "_DEBUG"
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcCd.lib"
# ADD LIB32 /nologo /out:"SrcCd.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_ReleaseNoHardware"
# PROP BASE Intermediate_Dir "SrcC___Win32_ReleaseNoHardware"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_ReleaseNoHardware"
# PROP Intermediate_Dir "SrcC_ReleaseNoHardware"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /MT /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# SUBTRACT BASE CPP /YX
# ADD CPP /nologo /G6 /Gr /Zp16 /MT /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "NOHARDWARE" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# SUBTRACT CPP /YX
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC.lib"
# ADD LIB32 /nologo /out:"SrcC.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SrcC___Win32_ReleaseUSBOnly"
# PROP BASE Intermediate_Dir "SrcC___Win32_ReleaseUSBOnly"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SrcC_ReleaseUSBOnly"
# PROP Intermediate_Dir "SrcC_ReleaseUSBOnly"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /MT /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "NOHARDWARE" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# SUBTRACT BASE CPP /YX
# ADD CPP /nologo /G6 /Gr /Zp16 /MT /W4 /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "NOHARDWARE" /D "SPG_General_USECyUSB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# SUBTRACT CPP /YX
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC.lib"
# ADD LIB32 /nologo /out:"SrcC.lib"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "SrcC___Win32_DebugUSBOnly"
# PROP BASE Intermediate_Dir "SrcC___Win32_DebugUSBOnly"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "SrcC_DebugUSBOnly"
# PROP Intermediate_Dir "SrcC_DebugUSBOnly"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /Zp16 /MTd /W4 /Gm /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /D "NOHARDWARE" /FD /c
# SUBTRACT BASE CPP /Fr /YX
# ADD CPP /nologo /Zp16 /MTd /W4 /Gm /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /D "NOHARDWARE" /D "SPG_General_USECyUSB" /FD /c
# SUBTRACT CPP /Fr /YX
# ADD BASE RSC /l 0x40c /d "_DEBUG"
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcCd.lib"
# ADD LIB32 /nologo /out:"SrcCd.lib"

!ENDIF 

# Begin Target

# Name "SrcC - Win32 Release"
# Name "SrcC - Win32 Debug"
# Name "SrcC - Win32 Debug the Release"
# Name "SrcC - Win32 Release_PII"
# Name "SrcC - Win32 Release_PIII"
# Name "SrcC - Win32 Release_PII_Fast"
# Name "SrcC - Win32 Release_PIII_Fast"
# Name "SrcC - Win32 Release_P4"
# Name "SrcC - Win32 Release_Listing"
# Name "SrcC - Win32 Release_P4_Fast"
# Name "SrcC - Win32 Release_Safe"
# Name "SrcC - Win32 Release_Win98Header"
# Name "SrcC - Win32 DebugBrowse"
# Name "SrcC - Win32 Release_DebugInfo"
# Name "SrcC - Win32 DebugNoHardware"
# Name "SrcC - Win32 ReleaseNoHardware"
# Name "SrcC - Win32 ReleaseUSBOnly"
# Name "SrcC - Win32 DebugUSBOnly"
# Begin Group "SrcC"

# PROP Default_Filter ""
# Begin Group "3D"

# PROP Default_Filter ""
# Begin Group "3DCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\3DSelect.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Profil3D.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_bloc.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Load3D.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_pgl.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_tex.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_StereoVision.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "3DH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\3DSelect.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\3DSelect.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Profil3D.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Profil3D.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_bloc.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Load3D.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Normales.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_pgl.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_tex.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_StereoVision.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "IO"

# PROP Default_Filter ""
# Begin Group "IOCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\CutManagerFileIO.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\HDF_Loader.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Logfile.cpp
# End Source File
# Begin Source File

SOURCE=.\PRO_Loader.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManagerFileIO.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spg_avi.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FileList.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FileListSpec.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Files.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_RingRecFileIO.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_VidCap.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_VidCapEmule.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SUR_Loader.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "IOH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\CutManagerFileIO.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\DigitalSurf.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\HDF_Loader.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\HDF_Loader.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Logfile.h
# End Source File
# Begin Source File

SOURCE=.\PRO_Loader.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PRO_Loader.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManagerFileIO.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spg_avi.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FileList.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FileList.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Files.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Files.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_RingRecFileIO.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_VidCap.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SUR_Loader.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SUR_Loader.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "Math"

# PROP Default_Filter ""
# Begin Group "MathCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\FIR_Design.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FIR_Invert.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\HYP_Walker.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Interpolateur.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Align.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_BinCorr.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_DecalageDePhase.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Demodule.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_DispField.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_DispFieldByte.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_DispFieldFloat.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Interact2D.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Interact3D.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Inverseur.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_K_FIT2.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# ADD CPP /Oy
# SUBTRACT CPP /Ox /Ot /Og /Oi

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# ADD BASE CPP /Oy /Ob1
# SUBTRACT BASE CPP /Ox /Ot /Og /Oi /Os
# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# ADD BASE CPP /Oy /Ob1
# SUBTRACT BASE CPP /Ox /Ot /Og /Oi /Os
# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_K_FIT3.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# ADD CPP /Oy
# SUBTRACT CPP /Ox /Ot /Og /Oi

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# ADD BASE CPP /Oy /Ob1
# SUBTRACT BASE CPP /Ox /Ot /Og /Oi /Os
# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# ADD BASE CPP /Oy /Ob1
# SUBTRACT BASE CPP /Ox /Ot /Og /Oi /Os
# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_K_FIT4.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# ADD CPP /Oy
# SUBTRACT CPP /Ox /Ot /Og /Oi

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# ADD BASE CPP /Oy /Ob1
# SUBTRACT BASE CPP /Ox /Ot /Og /Oi /Os
# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# ADD BASE CPP /Oy /Ob1
# SUBTRACT BASE CPP /Ox /Ot /Og /Oi /Os
# ADD CPP /Oy /Ob1
# SUBTRACT CPP /Ox /Ot /Og /Oi /Os

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_K_FITW.CPP
# End Source File
# Begin Source File

SOURCE=.\SPG_LearningInvert.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_LearningSet.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Linearize.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ParaFit.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PixelInterpole.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Scale.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_WeightBuffer.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_General.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "MathH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\BRES.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FIR_Design.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FIR_Design.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FIR_Invert.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\HYP_Walker.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\HYP_Walker.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Interpolateur.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Interpolateur.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Align.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_BinCorr.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_DecalageDePhase.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Demodule.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_DispField.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_DispFieldConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Interact2D.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Interact3D.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Inverseur.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_K_FIT.H
# End Source File
# Begin Source File

SOURCE=.\SPG_LearningSet.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Linearize.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ParaFit.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PixelInterpole.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Scale.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_WeightBuffer.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_General.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_General.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "2D"

# PROP Default_Filter ""
# Begin Group "2DCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\ColorGenerator.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Colors256.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManager.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManagerMath.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutView.cpp
# End Source File
# Begin Source File

SOURCE=.\CutXManager.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\P_IList.cpp
# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManager.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManagerFilters.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManagerMath.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_AAGraphics.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CameraHighPass.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Effect.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_MultiCutView.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Paint.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "2DH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\ColorGenerator.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ColorGenerator.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Colors256.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Colors256.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManager.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManager.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutView.h
# End Source File
# Begin Source File

SOURCE=.\P_IList.h
# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManager.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManager.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_AAGraphics.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CameraHighPass.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Effect.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_MultiCutView.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Paint.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "Memory"

# PROP Default_Filter ""
# Begin Group "MemoryCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\PG_DiskBuffer.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PG_RingBuffer.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Mem.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Snake.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_String.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_StringIO.cpp
# End Source File
# End Group
# Begin Group "MemoryH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\PG_DiskBuffer.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PG_DiskBuffer.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PG_RingBuffer.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Mem.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Mem_Type.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Snake.h
# End Source File
# Begin Source File

SOURCE=.\SPG_String.h
# End Source File
# Begin Source File

SOURCE=.\SPG_StringIO.h
# End Source File
# End Group
# End Group
# Begin Group "System"

# PROP Default_Filter ""
# Begin Group "SystemCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_CD_Check.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ConfigFile.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Console.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\SPG_DefineList.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global_LogTime.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global_MotionBlur.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global_Network.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_List.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_LogTime.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_LoopThread.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_ProgPrincipal.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# SUBTRACT CPP /YX

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Son.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TreeView.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Window.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_wLogTime.cpp
# End Source File
# Begin Source File

SOURCE=.\SpinLock.cpp
# End Source File
# End Group
# Begin Group "SystemH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_CD_Check.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ConfigFile.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Console.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\SPG_DefineList.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global_LogTime.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global_MotionBlur.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global_Network.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_GlobalConst.h
# End Source File
# Begin Source File

SOURCE=.\SPG_List.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_List.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_LogTime.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_LoopThread.h
# End Source File
# Begin Source File

SOURCE=.\SPG_ProgPrincipal.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Son.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Son.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SpecMacro.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TreeView.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Window.h
# End Source File
# Begin Source File

SOURCE=.\SPG_wLogTime.h
# End Source File
# Begin Source File

SOURCE=.\SpinLock.h
# End Source File
# End Group
# Begin Group "ConfigFileDlg"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Connexion\flipDialogBox.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\flipDialogBox.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SPG_ConfigFileDlg.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SPG_ConfigFileDlg.h
# End Source File
# End Group
# End Group
# Begin Group "RealTimeSourceFiles"

# PROP Default_Filter ""
# Begin Group "RealTimeCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Sgraph_emc.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Normales.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Render.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_RenderPoly.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_K_FIT_RT.CPP
# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "RealTimeH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Sgraph_emc.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Sgraph_emc.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Render.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_RenderPoly.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "BreakHookAndDummyCompile"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\BreakHook.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\BreakHook.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "NoCompile"

# PROP Default_Filter ""
# Begin Group "NCMath"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\K_FIT2_3_128.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\K_FIT3_4_128.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\K_FIT4_5_128.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Argum_Inline.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Argum_Inline_0_1.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Argum_Inline_0_2pi.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "NC2D"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\GraphicsInlineX.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\GraphicsInlineY.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\InlineBuildCoulC.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\InlineCombineColor.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\InlinePrepareSegment.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\InlineRenderSegment.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\InlineRenderSegmentX.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Render_NoCompile.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "NCSystem"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\CD_CHECK_KEY.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SpecMacroEnder.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SpecMacroHeader.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "NCMemory"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SpecList.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SpecList.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SpecStack.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SpecStack.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Inline_RGBP5SN.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Inline_RGBUP5SN.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "Network"

# PROP Default_Filter ""
# Begin Group "NetworkCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Spg_ftp.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_NetworkEmule.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spg_smtp.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ULIPS.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ULIPS_Interface.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "NetworkH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Spg_ftp.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_NetworkEmule.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spg_smtp.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ULIPS.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ULIPS_Interface.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "MathLineDetect"

# PROP Default_Filter ""
# Begin Group "Modele"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_PeakDet2DModele.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet2DModele.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet2DModeleMatlab.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet2DModeleMatlab.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet2DModeleParams.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "PeakDet"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_PeakDet2D.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet2D.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet2DParams.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Source File

SOURCE=.\SPG_PeakDet2DGetLaserLine.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_PeakDet2DGetLaserLine.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_StructVersionCheck.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "Connexion"

# PROP Default_Filter ""
# Begin Group "Hard"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CameraEmulee.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CameraEmulee.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CameraPad.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CameraPad.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CamMulti.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CamMulti.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CyUSB.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CyUSB.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_CyUSB_Internal.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_File.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_File.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_File_Internal.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_FTD2XX.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_FTD2XX.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_NIDAQmx.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_NIDAQmx.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_NIDAQmx_Internal.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_RS232.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_RS232.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_UDP.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_UDP.h
# End Source File
# End Group
# Begin Group "Soft"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_ASCIIHEX.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_ASCIIHEX.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_BandPassRS.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_BandPassRS.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Buffer.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Buffer.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_EmuleLISEED.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_EmuleLISEED.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_EmuleLISEEDI.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_EmuleLISEEDI.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Merge.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Merge.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Packet.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Packet.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Packet_Internal.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_SCHK.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_SCHK.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Void.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Void.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SPG_PacketBuffer.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SPG_PacketBuffer.h
# End Source File
# End Group
# Begin Group "ToDo"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Avir.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Aviw.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Chain.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Chain.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Double.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Double.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Filer.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Filew.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_PacketRIA.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_PacketRIA.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Transcode.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_Transcode.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "scxConnexionDbg"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Connexion\SCM_ConnexionDbg.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ConnexionDbg.h
# End Source File
# End Group
# Begin Group "Interfaces"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Connexion\SCM_Interface_Packet.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Interface_Packet.h
# End Source File
# End Group
# Begin Group "Extensions"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensCamera.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensCamera.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensContinuous.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensContinuous.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensOverwrite.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensOverwrite.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensProtocol.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensWriteThrough.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_ExtensWriteThrough.h
# End Source File
# End Group
# Begin Group "Tools"

# PROP Default_Filter ""
# Begin Source File

SOURCE="..\NIDAQ_SDK\DAQmx ANSI C Dev\include\DummyNIDAQmx.h"
# End Source File
# Begin Source File

SOURCE=.\SCM_NIDAQmxEnum.cpp
# End Source File
# Begin Source File

SOURCE=.\SCM_NIDAQmxEnum.h
# End Source File
# Begin Source File

SOURCE=..\NIDAQ_SDK\SPG_NIDAQmxConfig.h
# End Source File
# End Group
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Connexion_UID.h
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Interface.cpp
# End Source File
# Begin Source File

SOURCE=.\Connexion\SCM_Interface.h
# End Source File
# End Group
# End Group
# Begin Group "Configuration"

# PROP Default_Filter ""
# Begin Group "Config"

# PROP Default_Filter ""
# Begin Group "Projects"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Config\AllConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\AllNoFFTConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\AllNoFFTNetwork.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\APE_DLLConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\AubesConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\BatchConvertConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\CamConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\ChromConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\Decorrelle2DConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\FogaleProbeConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\JeuxAubesConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\JeuxAubesECLConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\JeuxAubesSNECMAConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\LenscanMotorAxisConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\LineDetectConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\LISE_ED_DLLConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\LISE_ED_DLLConfigTest.h
# End Source File
# Begin Source File

SOURCE=.\Config\LISE_EDI_DLLConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\LoopThreadConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\MMF_DLLConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\RaccordConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\RGRtoAVIConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\SCX_DLLConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\SmallConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\SPG_CommConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\SphereConfig.h
# End Source File
# Begin Source File

SOURCE=.\Config\TempMonitorConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\TestPortSerieConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\TomoConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\UlipsConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\UlipsServerConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\VideoExtractConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\VoitureConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\WinampPlugInConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Source File

SOURCE=.\Config\SPG_DebugConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\SPG_ReleaseConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Config\SPG_Warning.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Source File

SOURCE=.\SPG_General.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "Includes"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Includes.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_InhibitedFunctions.h
# End Source File
# Begin Source File

SOURCE=.\SPG_MinIncludes.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SysInc.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Win98\SPG_Win98Define.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Win98\SPG_Win98Full.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Win98\SPG_Win98Hack.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Win98\SPG_WindowsConfig.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "Lib"

# PROP Default_Filter ""
# Begin Group "PGlib"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\PGLib\pglib.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_3d.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_display.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_input.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_math.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_sound.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_std.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_wm.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\pgltypes.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PGLib\PGLib.lib

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "Sgraph"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Sgraph.lib

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "NewMat10"

# PROP Default_Filter ""
# Begin Group "Source"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\NewMat10\Source\BANDMAT.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\CHOLESKY.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\EVALUE.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\FFT.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\HHOLDER.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\JACOBI.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\MYEXCEPT.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWFFT.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT1.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT2.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT3.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT4.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT5.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT6.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT7.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT8.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT9.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMATEX.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMATNL.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMATRM.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\SOLUTION.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\SORT.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\SUBMAT.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\SVD.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "Headers"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\NewMat10\Source\BOOLEAN.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\CONTROLW.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\INCLUDE.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\MYEXCEPT.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMAT.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMATAP.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMATIO.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMATNL.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMATRC.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NEWMATRM.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\PRECISIO.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\SOLUTION.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT.H

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "LibHeader"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\NewMat10\NewMat10LibHeader.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "Example"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\NewMat10\Source\EXAMPLE.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\GARCH.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\NL_EX.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\SL_EX.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TEST_EXC.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT1.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT2.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT3.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT4.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT5.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT6.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT7.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT8.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMT9.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTA.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTB.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTC.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTD.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTE.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTF.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTG.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTH.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTI.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTJ.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTK.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTL.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\NewMat10\Source\TMTM.CPP

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "Documentation"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\NewMat10\Doc\NM10.HTM

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "SPG_FFT"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\ComplexType.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FFT.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\WhatFFT.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# Begin Group "PhaseUnwrapping"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\UW_Utils.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

!ENDIF 

# End Source File
# End Group
# End Group
# Begin Group "WinPrep"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_Win98\SPG_Win98.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Win98\SPG_Win98Full.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Win98Normal.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Win98SPG.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Win98Header"

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugBrowse"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_DebugInfo"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseNoHardware"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 ReleaseUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 DebugUSBOnly"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# End Target
# End Project
