# Microsoft Developer Studio Project File - Name="NEWMAT10" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Application" 0x0101

CFG=NEWMAT10 - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "NEWMAT10.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "NEWMAT10.mak" CFG="NEWMAT10 - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "NEWMAT10 - Win32 Release" (based on "Win32 (x86) Application")
!MESSAGE "NEWMAT10 - Win32 Debug" (based on "Win32 (x86) Application")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Release"
# PROP Intermediate_Dir "Release"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /YX /FD /c
# ADD CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /YX /FD /c
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "NDEBUG"
# ADD RSC /l 0x40c /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /machine:I386

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Debug"
# PROP Intermediate_Dir "Debug"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /YX /FD /GZ /c
# ADD CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /YX /FD /GZ /c
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /win32
# ADD BASE RSC /l 0x40c /d "_DEBUG"
# ADD RSC /l 0x40c /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /debug /machine:I386 /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /debug /machine:I386 /pdbtype:sept

!ENDIF 

# Begin Target

# Name "NEWMAT10 - Win32 Release"
# Name "NEWMAT10 - Win32 Debug"
# Begin Group "Source Files"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Source\BANDMAT.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\CHOLESKY.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\EVALUE.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\FFT.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\HHOLDER.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\JACOBI.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\MYEXCEPT.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWFFT.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT1.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT2.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT3.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT4.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT5.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT6.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT7.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT8.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT9.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMATEX.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMATNL.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMATRM.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\SOLUTION.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\SORT.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\SUBMAT.CPP
# End Source File
# Begin Source File

SOURCE=.\Source\SVD.CPP
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Source\BOOLEAN.H
# End Source File
# Begin Source File

SOURCE=.\Source\CONTROLW.H
# End Source File
# Begin Source File

SOURCE=.\Source\INCLUDE.H
# End Source File
# Begin Source File

SOURCE=.\Source\MYEXCEPT.H
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMAT.H
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMATAP.H
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMATIO.H
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMATNL.H
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMATRC.H
# End Source File
# Begin Source File

SOURCE=.\Source\NEWMATRM.H
# End Source File
# Begin Source File

SOURCE=.\Source\PRECISIO.H
# End Source File
# Begin Source File

SOURCE=.\Source\SOLUTION.H
# End Source File
# Begin Source File

SOURCE=.\Source\TMT.H
# End Source File
# End Group
# Begin Group "Examples"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\Source\EXAMPLE.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\GARCH.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\NL_EX.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\SL_EX.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TEST_EXC.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT1.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT2.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT3.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT4.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT5.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT6.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT7.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT8.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMT9.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTA.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTB.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTC.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTD.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTE.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTF.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTG.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTH.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTI.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTJ.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTK.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTL.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# Begin Source File

SOURCE=.\Source\TMTM.CPP

!IF  "$(CFG)" == "NEWMAT10 - Win32 Release"

!ELSEIF  "$(CFG)" == "NEWMAT10 - Win32 Debug"

# PROP Exclude_From_Build 1

!ENDIF 

# End Source File
# End Group
# Begin Group "LibHeader"

# PROP Default_Filter ""
# Begin Source File

SOURCE=.\NewMat10LibHeader.h
# End Source File
# End Group
# End Target
# End Project
