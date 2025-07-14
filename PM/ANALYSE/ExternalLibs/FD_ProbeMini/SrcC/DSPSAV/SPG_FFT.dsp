# Microsoft Developer Studio Project File - Name="SPG_FFT" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Static Library" 0x0104

CFG=SPG_FFT - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "SPG_FFT.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "SPG_FFT.mak" CFG="SPG_FFT - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "SPG_FFT - Win32 Release" (based on "Win32 (x86) Static Library")
!MESSAGE "SPG_FFT - Win32 Debug" (based on "Win32 (x86) Static Library")
!MESSAGE "SPG_FFT - Win32 Debug the Release" (based on "Win32 (x86) Static Library")
!MESSAGE "SPG_FFT - Win32 Release_PII" (based on "Win32 (x86) Static Library")
!MESSAGE "SPG_FFT - Win32 Release_PIII" (based on "Win32 (x86) Static Library")
!MESSAGE "SPG_FFT - Win32 Release_PII_Fast" (based on "Win32 (x86) Static Library")
!MESSAGE "SPG_FFT - Win32 Release_PIII_Fast" (based on "Win32 (x86) Static Library")
!MESSAGE "SPG_FFT - Win32 Release_P4" (based on "Win32 (x86) Static Library")
!MESSAGE "SPG_FFT - Win32 Release_P4_Fast" (based on "Win32 (x86) Static Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=xicl6.exe
RSC=rc.exe

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SPG_FFT_Release"
# PROP Intermediate_Dir "SPG_FFT_Release"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /YX /FD /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"SPG_FFT.lib"

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "SPG_FFT___Win32_Debug"
# PROP BASE Intermediate_Dir "SPG_FFT___Win32_Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "SPG_FFT_Debug"
# PROP Intermediate_Dir "SPG_FFT_Debug"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /c
# ADD CPP /nologo /Zp16 /W4 /Gm /GX /ZI /Od /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /FD /GZ /c
# SUBTRACT CPP /YX
# ADD BASE RSC /l 0x40c /d "_DEBUG"
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"SPG_FFTd.lib"

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Debug the Release"
# PROP BASE Intermediate_Dir "Debug the Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SPG_FFT_DebugTheRelease"
# PROP Intermediate_Dir "SPG_FFT_DebugTheRelease"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /W4 /GX /Ox /Ot /Oa /Og /Oi /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD /c
# SUBTRACT BASE CPP /Os /YX
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Zi /Ox /Ot /Oa /Og /Oi /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SPG_FFT.lib"
# ADD LIB32 /nologo /out:"SPG_FFTdr.lib"

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SPG_FFT___Win32_Release_PII"
# PROP BASE Intermediate_Dir "SPG_FFT___Win32_Release_PII"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SPG_FFT_PII"
# PROP Intermediate_Dir "SPG_FFT_PII"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SPG_FFT.lib"
# ADD LIB32 /nologo /out:"SPG_FFT_PII.lib"

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SPG_FFT___Win32_Release_PIII"
# PROP BASE Intermediate_Dir "SPG_FFT___Win32_Release_PIII"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SPG_FFT_Release_PIII"
# PROP Intermediate_Dir "SPG_FFT_Release_PIII"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SPG_FFT.lib"
# ADD LIB32 /nologo /out:"SPG_FFT_PIII.lib"

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SPG_FFT___Win32_Release_PII_Fast"
# PROP BASE Intermediate_Dir "SPG_FFT___Win32_Release_PII_Fast"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SPG_FFT_Release_PII_Fast"
# PROP Intermediate_Dir "SPG_FFT_Release_PII_Fast"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D "IntelCompiler" /FAcs /FD -Qxi -QxM -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict -Qfp_port- /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SPG_FFT_PII.lib"
# ADD LIB32 /nologo /out:"SPG_FFT_PIIF.lib"

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SPG_FFT___Win32_Release_PIII_Fast"
# PROP BASE Intermediate_Dir "SPG_FFT___Win32_Release_PIII_Fast"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SPG_FFT_Release_PIII_Fast"
# PROP Intermediate_Dir "SPG_FFT_Release_PIII_Fast"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SPG_FFT_PIII.lib"
# ADD LIB32 /nologo /out:"SPG_FFT_PIII.lib"

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SPG_FFT___Win32_Release_P4"
# PROP BASE Intermediate_Dir "SPG_FFT___Win32_Release_P4"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SPG_FFT_Release_P4"
# PROP Intermediate_Dir "SPG_FFT_Release_P4"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -Qxi -QxM -QxK -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SPG_FFT_PIII.lib"
# ADD LIB32 /nologo /out:"SPG_FFT_P4.lib"

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "SPG_FFT___Win32_Release_P4_Fast"
# PROP BASE Intermediate_Dir "SPG_FFT___Win32_Release_P4_Fast"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "SPG_FFT_Release_P4_Fast"
# PROP Intermediate_Dir "SPG_FFT_Release_P4_Fast"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qip -Qipo -Qwp_ipo -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD CPP /nologo /G6 /Gr /Zp16 /W4 /GX /Ox /Ot /Oa /Og /Oi /Ob2 /Gy /I ".\SPGFFT" /I ".\fftw" /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /FD -G7 -Qxi -QxM -QxK -QxW -Qpc32 -Qrcd -Qvec_report3 -Qms -Qsox- -QIfist -Qsfalign16 -Qrestrict /c
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=xilink6.exe -lib
# ADD BASE LIB32 /nologo /out:"SPG_FFT_P4.lib"
# ADD LIB32 /nologo /out:"SPG_FFT_P4F.lib"

!ENDIF 

# Begin Target

# Name "SPG_FFT - Win32 Release"
# Name "SPG_FFT - Win32 Debug"
# Name "SPG_FFT - Win32 Debug the Release"
# Name "SPG_FFT - Win32 Release_PII"
# Name "SPG_FFT - Win32 Release_PIII"
# Name "SPG_FFT - Win32 Release_PII_Fast"
# Name "SPG_FFT - Win32 Release_PIII_Fast"
# Name "SPG_FFT - Win32 Release_P4"
# Name "SPG_FFT - Win32 Release_P4_Fast"
# Begin Group "Includes"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\ComplexType.h
# End Source File
# Begin Source File

SOURCE=.\FFT.h
# End Source File
# Begin Source File

SOURCE=.\Fwt\wavelet.h
# End Source File
# End Group
# Begin Group "Configuration"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\WhatFFT.h
# End Source File
# End Group
# Begin Group "SrcC_Dependencies"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\3DSelect.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\3DSelect.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\BRES.H

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CD_CHECK_KEY.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ColorGenerator.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ColorGenerator.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Colors256.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Colors256.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManager.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManager.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\CutManagerFileIO.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FIR_Design.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\FIR_Design.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Interpolateur.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Interpolateur.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PG_DiskBuffer.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\PG_DiskBuffer.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Profil3D.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Profil3D.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilConvert.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManager.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManager.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfileManagerFileIO.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\ProfilExtract.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SelectionProfile.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_bloc.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Sgraph_emc.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Sgraph_emc.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_geom.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Load3D.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_Normales.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_opts.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SGRAPH_tex.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Accumulateur.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_BmpIO.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Buttons.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Carac.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CaracFormatter.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_CD_Check.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Console.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Console.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Discretiseur.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Files.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Files.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_FloatIO.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_General.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Global_Network.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_Render.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Graphics_RenderPoly.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Histogram256.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Includes.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Initialise.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Initialise.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Inverseur.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_List.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_List.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Meca.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Mem.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Mem.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Mem_Type.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_MemLink.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_OPTS.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Network_Protocol.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ParaFit.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_ProgPrincipal.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_RingRec.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_SerialComm.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Son.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Son.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Timer.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_TxtIO.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_WavIO.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\UW_Utils.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_ConvertToGraph.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_General.agh

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\V_General.h

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "FWT"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Fwt\wavelet.cpp
# End Source File
# End Group
# Begin Group "SPGFFT"

# PROP Default_Filter ""
# Begin Group "Compile1st"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\FFT.cpp
# End Source File
# End Group
# Begin Group "Compile2nd"

# PROP Default_Filter ""
# Begin Group "FFT4ASM"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Spgfft\Fft4asm.cpp
# End Source File
# Begin Source File

SOURCE=.\Spgfft\Fft4asm.h
# End Source File
# End Group
# Begin Group "FFT4C"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Spgfft\Fft4c.cpp
# End Source File
# Begin Source File

SOURCE=.\Spgfft\Fft4c.h
# End Source File
# End Group
# Begin Group "fftw"

# PROP Default_Filter ""
# Begin Group "all"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\fftw\config.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\config.h
# End Source File
# Begin Source File

SOURCE=.\fftw\executor.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\f77_func.h
# End Source File
# Begin Source File

SOURCE=".\fftw\fftw-int.h"
# End Source File
# Begin Source File

SOURCE=.\fftw\fftw.h
# End Source File
# Begin Source File

SOURCE=.\fftw\fftwf77.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fftwnd.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_1.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_10.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_11.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_12.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_13.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_14.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_15.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_16.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_2.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_3.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_32.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_4.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_5.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_6.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_64.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_7.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_8.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fn_9.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_1.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_10.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_11.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_12.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_13.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_14.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_15.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_16.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_2.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_3.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_32.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_4.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_5.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_6.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_64.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_7.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_8.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\fni_9.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_10.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_16.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_2.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_3.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_32.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_4.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_5.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_6.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_64.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_7.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_8.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftw_9.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_10.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_16.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_2.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_3.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_32.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_4.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_5.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_6.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_64.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_7.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_8.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\ftwi_9.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\generic.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\malloc.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\planner.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\putils.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\rader.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\timer.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\twiddle.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\wisdom.cpp
# End Source File
# Begin Source File

SOURCE=.\fftw\wisdomio.cpp
# End Source File
# End Group
# Begin Source File

SOURCE=.\fftwcompat.h
# End Source File
# Begin Source File

SOURCE=.\SPG_Fftw.agh
# End Source File
# Begin Source File

SOURCE=.\SPG_Fftw.cpp
# End Source File
# End Group
# End Group
# Begin Group "NoCompile"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Spgfft\AsmFFT4G.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spgfft\AsmFFT4GO.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spgfft\AsmFFT4Z.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spgfft\CFFT4.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spgfft\CFFT4G.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Spgfft\KarnFFT4G.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Argum_Inline.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Argum_Inline_0_1.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\SPG_Argum_Inline_0_2pi.cpp

!IF  "$(CFG)" == "SPG_FFT - Win32 Release"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug"

# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Debug the Release"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_PIII_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ELSEIF  "$(CFG)" == "SPG_FFT - Win32 Release_P4_Fast"

# PROP BASE Exclude_From_Build 1
# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# End Group
# End Target
# End Project
