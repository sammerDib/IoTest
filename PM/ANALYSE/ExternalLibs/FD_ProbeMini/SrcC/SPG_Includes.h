#include <windows.h>

#ifdef SPG_General_PGLib
#include "PGLib\PGlib.h"
#pragma message(__FILE__"(4): Using PGLib")
#endif

#include "SPG_MinIncludes.h"

#ifdef SPG_General_USESTEREOVIS
#include "SPG_StereoVision.h"
#endif

#ifdef SPG_General_USEHYP
#include "HYP_Walker.h"
#endif

#ifdef SPG_General_USETXT
#include "SPG_TxtIO.h"
#endif
#ifdef SPG_General_USEFLOATIO
#include "SPG_FloatIO.h"
#endif
#ifdef SPG_General_USEBMPIO
#include "SPG_BmpIO.h"
#endif
#ifdef SPG_General_USEWAVIO
#include "SPG_WavIO.h"
#endif

#ifdef SPG_General_USEButtons
#include "SPG_Buttons.h"
#endif

#ifdef SPG_General_USEAVI
#include "SPG_AVI.h"//vfw32.lib
#endif

#ifdef SPG_General_USEMELINK
#include "SPG_MemLink.h"
#endif

//fdef SPG_General_USENetwork
//nclude "SPG_Network.h"//ws2_32.lib
//ndif

//fdef SPG_General_USENetworkEmule
//nclude "SPG_NetworkEmule.h"
//ndif

//fdef SPG_General_USENetwork_Protocol
//nclude "SPG_Network_Protocol.h"
//ndif

//fdef SPG_General_USENetwork_OPTS
//nclude "SPG_Network_OPTS.h"
//ndif

//fdef SPG_General_USEULIPS
//nclude "ULIPS.h"
//ndif

//fdef SPG_General_USEULIPSINTERFACE
//nclude "ULIPS_Interface.h"
//ndif

#ifdef SPG_General_USECDCHECK
#include "SPG_CD_Check.h"
#endif

#ifdef SPG_General_USELICENSECHECK
#include "SPG_License_Check.h"
#endif

#ifdef SPG_General_USEGlobal
#include "SPG_GlobalConst.h"
#endif

//fdef SPG_General_USEGLOBALLOGTIME //macros de remplacement definies dans SPG_Global_LogTime
#include "SPG_Global_LogTime.h"
//ndif

//fdef SPG_General_USEWLTG
#include "LogTime.h"
//ndif

#ifdef SPG_General_USEProgPrincipal
#include "SPG_ProgPrincipal.h"
#endif

#ifdef SPG_General_USECONFIGFILE
#include "SPG_ConfigFile.h"
#endif

#ifdef SPG_General_USECONFIGFILEDLG
#include "Connexion\SPG_ConfigFileDlg.h"
//nclude "Connexion\flipDialogBox.h"
#endif

#ifdef SPG_General_USEGraphics
#ifdef SPG_General_USESGRAPH
//#ifdef SGE_EMC
#ifdef SPG_General_USECarac
#ifdef SPG_General_USEButtons
#include "SGRAPH_opts.h"
#endif
#endif
//#endif
#endif
#endif

#ifdef SPG_General_USESON
#include "SPG_Son.h"
#endif

#ifdef SPG_General_USEInterpolateur
#include "Interpolateur.h"
#endif

#ifdef SPG_General_USEColorGenerator
#include "ColorGenerator.h"
#endif

#ifdef SPG_General_USEColors256
#include "Colors256.h"
#endif

#ifdef SPG_General_USE3DSelect
#include "3DSelect.h"
#endif

#ifdef SPG_General_USEFiles
#include "SPG_Files.h"
#include "Logfile.h"
#endif

#ifdef SPG_General_USEFileList
#include "SPG_FileList.h"
#endif

#ifdef SPG_General_USEFFT
#include "FFT.h"
#endif

#ifdef SPG_General_USEFWT
#include "FWT\wavelet.h"
#endif

#ifdef SPG_General_USEDiskBuffer
#include "PG_DiskBuffer.h"
#endif

#ifdef SPG_General_USERingBuffer
#include "PG_RingBuffer.h"

#endif

#ifdef SPG_General_USESnake
#include "SPG_Snake.h"
#endif

#ifdef SPG_General_USECut
#include "CutManager.h"
#include "SPG_MultiCutView.h"
#endif

#ifdef SPG_General_USECutView
#include "CutView.h"
#endif

#ifdef SPG_General_USEGraphicHistoryCurves
#include "GraphicHistoryCurves.h"
#endif

#ifdef SPG_General_USEProfil
#include "ProfileManager.h"
#include "P_IList.h"
#endif

#ifdef SPG_General_USEGEFFECT
#include "SPG_AAGraphics.h"
#endif

//fdef SPG_General_USEWindow
#include "SPG_Window.h"
//ndif

#ifdef SPG_General_USEProfil3D
#include "Profil3D.h"
#endif

#ifdef SPG_General_USECut
#ifdef SPG_General_USEProfil
#ifdef SPG_General_USEGraphics
#include "SelectionProfile.h"
#endif
#endif
#endif

#ifdef SPG_General_USEPrXt
#include "ProfilExtract.h"
#endif

#ifdef SPG_General_USERINGREC
#include "SPG_RingRec.h"
#include "SPG_RingRecFileIO.h"
#endif

#ifdef SPG_General_USEHIST
#include "SPG_Histogram256.h"
#endif

#ifdef SPG_General_USEACC
#include "SPG_Accumulateur.h"
#endif

#ifdef SPG_General_USEINVJ0
#include "SPG_Inverseur.h"
#endif

#ifdef SPG_General_USEPrCV
#include "ProfilConvert.h"
#endif

#ifdef SPG_General_USEDCRZ
#include "SPG_Discretiseur.h"
#endif

#ifdef SPG_General_USEINTERACT3D
#include "SPG_Interact3D.h"
#endif

#ifdef SPG_General_USEINTERACT2D
#include "SPG_Interact2D.h"
#endif

#ifdef SPG_General_USEMECA
#include "SPG_Meca.h"
#endif

#ifdef SPG_General_USESerialComm
#include "SPG_SerialComm.h"
#endif

#if defined(SPG_General_USEPARAFIT)||defined(SPG_General_USEWEIGHTEDPARAFIT)
#include "SPG_ParaFit.h"
#include "SPG_K_FIT.h"
#endif

#ifdef SPG_General_USEDECPHASE
#include "SPG_DecalageDePhase.h"
#endif

#ifdef SPG_General_USEFirDesign
#include "FIR_Design.h"
#endif

#ifdef SPG_General_USEFirInvert
#include "FIR_Invert.h"
#endif

#ifdef SPG_General_USEUnwrap
#include "UW_Utils.h"
#endif

#ifdef SPG_General_USEFTP
#include "SPG_FTP.h"
#endif

#ifdef SPG_General_USESMTPPOP3
#include "SPG_SMTP.h"
#endif

#ifdef SPG_General_USETREEVIEW
#include "SPG_TreeView.h"
#endif

#ifdef SPG_General_USEVidCap
#include "SPG_VidCap.h"
#endif

#ifdef SPG_General_USEPIXINT
#include "SPG_PixelInterpole.h"
#endif

#ifdef SPG_General_USEWEIGHTBUFFER
#include "SPG_WeightBuffer.h"
#endif
 
#ifdef SPG_General_USEDISPFIELD
#include "SPG_DispField.h"
#endif

#ifdef SPG_General_USEPaint
#include "SPG_Paint.h"
#endif

#ifdef SPG_General_USEAlign
#include "SPG_Align.h"
#endif

#ifdef SPG_General_USEPEAKDET
#include "SPG_PeakDet.h"
#endif

#ifdef SPG_General_USENEWMATLIB
//#error "hah"
#include "NewMat10\NewMat10LibHeader.h"
#endif

#ifdef SPG_General_USEPEAKDET2D
#include "SPG_PeakDet2DParams.h"
#include "SPG_PeakDet2D.h"
#include "SPG_PeakDet2DModeleParams.h"
#include "SPG_PeakDet2DModeleMatlab.h"
#include "SPG_PeakDet2DModele.h"
#include "SPG_PeakDet2DGetLaserLine.h"
#endif

#ifdef SPG_General_USECONNEXION
#include "Connexion\SCM_Connexion_UID.h"
#include "Connexion\SCM_Connexion.h"
#include "Connexion\SCM_Interface.h"
#include "Connexion\SCM_Interface_Packet.h"
#ifdef SPG_General_USESCXDISPATCH
#include "Connexion\SCM_Interface_Dispatch.h"
#endif
#endif

#ifdef SPG_General_USEBackupMe
#include "BackupMe.h"
#include "SCM_ConnexionBackupMe.h"
#endif

#include "SCM_NIDAQmxEnum.h"

#include "SPG_StringIO.h"

#include "SPG_Linearize.h"

//fdef SPG_USEGLOBALTHREADNAME
#include "SPG_GlobalThreadName.h"
//ndif

#ifdef SPG_General_USEGlobal
#include "SPG_Global.h"
#include "SPG_Mem.h"
#endif

#ifdef SPG_General_USEGENERICEXCEPTIONHANDLER
#include "SPG_SysInc.h"
#endif
#include "SPG_ExceptionGenericHandler.h"

#ifdef SPG_General_USELOOPTHREAD
#include "SPG_LoopThread.h"
#endif

#ifdef SPG_General_USEDEMODULE
#include "SPG_Demodule.h"
#endif

//fdef SPG_General_USEWLTG
#include "LogTimeWatch.h"
//ndif

#ifdef SPG_General_USECameraHighPass
#include "SPG_CameraHighPass.h"
#endif

#include "SPG_FastConvolve.h"

#ifndef DebugMem

#ifdef SPG_UseMalloc
#include <malloc.h>
#endif

#ifdef SPG_UseGlobalAlloc//include pour la declaration de globalalloc
#include "SPG_SysInc.h"
#endif

#endif

