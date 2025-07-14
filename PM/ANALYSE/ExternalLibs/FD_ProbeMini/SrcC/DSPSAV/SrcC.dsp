# Microsoft Developer Studio Project File - Name="SrcC" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Static Library" 0x0104

CFG=SrcC - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "SrcC.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "SrcC.mak" CFG="SrcC - Win32 Debug"
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FAcs /YX"SPG_Includes.h" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
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
# ADD CPP /nologo /Zp16 /W4 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX"SPG_Includes.h" /FD /c
# SUBTRACT CPP /Fr
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Zi /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FAcs /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FAcs /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FAcs /FD /P -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FAcs /Fa"SrcC_Release_PIII_Fast/" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
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
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FAcs /FD /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SrcC.lib"
# ADD LIB32 /nologo /out:"SrcC.lib"

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
# Begin Group "SrcC"

# PROP Default_Filter ""
# Begin Group "SourceFiles"

# PROP Default_Filter ""
# Begin Group "3D"

# PROP Default_Filter ""
# Begin Group "3DCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\3DSelect.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\Profil3D.cpp
# End Source File
# Begin Source File

SOURCE=.\SGRAPH.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_bloc.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Load3D.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.cpp
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_pgl.cpp
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_tex.cpp
# ADD CPP /Ze
# End Source File
# End Group
# Begin Group "3DH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\3DSelect.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\3DSelect.h
# End Source File
# Begin Source File

SOURCE=.\Profil3D.agh
# End Source File
# Begin Source File

SOURCE=.\Profil3D.h
# End Source File
# Begin Source File

SOURCE=.\SGRAPH.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH.h
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_bloc.agh
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.agh
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.h
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Load3D.agh
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Normales.agh
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.agh
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.h
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_pgl.agh
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_tex.agh
# End Source File
# End Group
# End Group
# Begin Group "IO"

# PROP Default_Filter ""
# Begin Group "IOCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\CutManagerFileIO.cpp
# End Source File
# Begin Source File

SOURCE=.\HDF_Loader.cpp
# End Source File
# Begin Source File

SOURCE=.\PRO_Loader.cpp
# End Source File
# Begin Source File

SOURCE=.\ProfileManagerFileIO.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_FileList.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_FileListSpec.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Files.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.cpp
# End Source File
# Begin Source File

SOURCE=.\SUR_Loader.cpp
# End Source File
# End Group
# Begin Group "IOH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\CutManagerFileIO.agh
# End Source File
# Begin Source File

SOURCE=.\DigitalSurf.h
# End Source File
# Begin Source File

SOURCE=.\HDF_Loader.agh
# End Source File
# Begin Source File

SOURCE=.\HDF_Loader.h
# End Source File
# Begin Source File

SOURCE=.\PRO_Loader.agh
# End Source File
# Begin Source File

SOURCE=.\PRO_Loader.h
# End Source File
# Begin Source File

SOURCE=.\ProfileManagerFileIO.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.h
# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.h
# End Source File
# Begin Source File

SOURCE=.\SPG_FileList.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_FileList.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Files.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Files.h
# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.h
# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.h
# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.h
# End Source File
# Begin Source File

SOURCE=.\SUR_Loader.agh
# End Source File
# Begin Source File

SOURCE=.\SUR_Loader.h
# End Source File
# End Group
# End Group
# Begin Group "Math"

# PROP Default_Filter ""
# Begin Group "MathCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\FIR_Design.cpp
# End Source File
# Begin Source File

SOURCE=.\FIR_Invert.cpp
# End Source File
# Begin Source File

SOURCE=.\HYP_Walker.cpp
# End Source File
# Begin Source File

SOURCE=.\Interpolateur.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Interact2D.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Inverseur.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_ParaFit.cpp
# End Source File
# Begin Source File

SOURCE=.\V_General.cpp
# ADD CPP /Ze
# End Source File
# End Group
# Begin Group "MathH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\BRES.H
# End Source File
# Begin Source File

SOURCE=.\FIR_Design.agh
# End Source File
# Begin Source File

SOURCE=.\FIR_Design.h
# End Source File
# Begin Source File

SOURCE=.\FIR_Invert.h
# End Source File
# Begin Source File

SOURCE=.\HYP_Walker.agh
# End Source File
# Begin Source File

SOURCE=.\HYP_Walker.h
# End Source File
# Begin Source File

SOURCE=.\Interpolateur.agh
# End Source File
# Begin Source File

SOURCE=.\Interpolateur.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Interact2D.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Inverseur.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.h
# End Source File
# Begin Source File

SOURCE=.\SPG_ParaFit.h
# End Source File
# Begin Source File

SOURCE=.\V_General.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_General.h
# End Source File
# End Group
# End Group
# Begin Group "2D"

# PROP Default_Filter ""
# Begin Group "2DCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\ColorGenerator.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\Colors256.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\CutManager.cpp
# End Source File
# Begin Source File

SOURCE=.\CutManagerMath.cpp
# End Source File
# Begin Source File

SOURCE=.\CutXManager.cpp
# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.cpp
# End Source File
# Begin Source File

SOURCE=.\ProfileManager.cpp
# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.cpp
# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.cpp
# End Source File
# End Group
# Begin Group "2DH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\ColorGenerator.agh
# End Source File
# Begin Source File

SOURCE=.\ColorGenerator.h
# End Source File
# Begin Source File

SOURCE=.\Colors256.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Colors256.h
# End Source File
# Begin Source File

SOURCE=.\CutManager.agh
# End Source File
# Begin Source File

SOURCE=.\CutManager.h
# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.agh
# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.h
# End Source File
# Begin Source File

SOURCE=.\ProfileManager.agh
# End Source File
# Begin Source File

SOURCE=.\ProfileManager.h
# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.agh
# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.h
# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.agh
# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.h
# End Source File
# End Group
# End Group
# Begin Group "Memory"

# PROP Default_Filter ""
# Begin Group "MemoryCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\PG_DiskBuffer.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Mem.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.cpp
# End Source File
# End Group
# Begin Group "MemoryH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\PG_DiskBuffer.agh
# End Source File
# Begin Source File

SOURCE=.\PG_DiskBuffer.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Mem.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Mem.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Mem_Type.h
# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.h
# End Source File
# End Group
# End Group
# Begin Group "System"

# PROP Default_Filter ""
# Begin Group "SystemCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_CD_Check.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Console.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SPG_Global.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SPG_Global_Network.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Initialise.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_List.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SPG_ProgPrincipal.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Son.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.cpp
# ADD CPP /Ze
# End Source File
# End Group
# Begin Group "SystemH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_CD_Check.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Console.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Console.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Global.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Global.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Global_Network.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Initialise.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Initialise.h
# End Source File
# Begin Source File

SOURCE=.\SPG_List.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_List.h
# End Source File
# Begin Source File

SOURCE=.\SPG_ProgPrincipal.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ProgPrincipal.h
# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Son.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Son.h
# End Source File
# Begin Source File

SOURCE=.\SPG_SpecMacro.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.h
# End Source File
# End Group
# End Group
# Begin Group "RealTimeSourceFiles"

# PROP Default_Filter ""
# Begin Group "RealTimeCPP"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Sgraph_emc.cpp
# ADD CPP /Ze
# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Normales.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Render.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_RenderPoly.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.cpp
# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.cpp
# ADD CPP /Ze
# End Source File
# End Group
# Begin Group "RealTimeH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Sgraph_emc.agh
# End Source File
# Begin Source File

SOURCE=.\Sgraph_emc.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Render.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_RenderPoly.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.h
# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.agh

!IF  "$(CFG)" == "SrcC - Win32 Release"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.h
# End Source File
# End Group
# End Group
# Begin Group "BreakHookAndDummyCompile"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\BreakHook.cpp
# End Source File
# Begin Source File

SOURCE=.\BreakHook.h
# End Source File
# End Group
# Begin Group "NoCompile"

# PROP Default_Filter ""
# Begin Group "NCMath"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_Argum_Inline.cpp

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

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

SOURCE=.\SPG_Network.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.cpp
# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.cpp
# End Source File
# End Group
# Begin Group "NetworkH"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_Network.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Network.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.h
# End Source File
# End Group
# End Group
# End Group
# End Group
# Begin Group "Lib"

# PROP Default_Filter ""
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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "PGlib"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\PGLib\pglib.h
# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_3d.h
# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_display.h
# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_input.h
# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_math.h
# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_sound.h
# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_std.h
# End Source File
# Begin Source File

SOURCE=.\PGLib\pglib_wm.h
# End Source File
# Begin Source File

SOURCE=.\PGLib\pgltypes.h
# End Source File
# End Group
# Begin Group "Documentation"

# PROP Default_Filter "*.txt"
# Begin Group "Preprocessor"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\3DSelect.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\BreakHook.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ColorGenerator.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Colors256.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManager.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManagerFileIO.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FIR_Design.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\HDF_Loader.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Interpolateur.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PG_DiskBuffer.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PRO_Loader.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Profil3D.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManager.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManagerFileIO.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_bloc.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Sgraph_emc.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Load3D.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Normales.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_pgl.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_tex.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CD_Check.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Console.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FileList.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Files.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global_Network.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Render.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_RenderPoly.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Initialise.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Inverseur.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_List.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Mem.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ParaFit.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ProgPrincipal.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Son.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SUR_Loader.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_General.i

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

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Source File

SOURCE=.\Afaire.txt

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\AGH_Helper.txt

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "Configuration"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG_General.h
# End Source File
# End Group
# Begin Group "Includes"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\SPG.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Includes.h
# End Source File
# Begin Source File

SOURCE=.\SPG_MinIncludes.h
# End Source File
# End Group
# Begin Group "SPG_FFT_Dependencies"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\ComplexType.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FFT.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\WhatFFT.h

!IF  "$(CFG)" == "SrcC - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Listing"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SrcC - Win32 Release_Safe"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# End Target
# End Project
