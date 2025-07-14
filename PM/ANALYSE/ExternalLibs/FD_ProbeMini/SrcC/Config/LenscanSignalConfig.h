
#define LenscanSignalConfig_h

//3DSelect.h
//#define SPG_General_USE3DSelect
//Bres.h
#define SPG_General_USEBRES
//ColorGenerator.h
#define SPG_General_USEColorGenerator
//Colors256.h
#define SPG_General_USEColors256
//SPG_Connexion.h
#define SPG_General_USECONNEXION
//SPG_ConfigFile.h
#define SPG_General_USECONFIGFILE
//SPG_ConfigFileDlg.h
#define SPG_General_USECONFIGFILEDLG
//BackupMe.h
#define SPG_General_USEBackupMe
//CutManager.h
#define SPG_General_USECut
//GraphicHistoryCurves.h
#define SPG_General_USEGraphicHistoryCurves
//CutView.h
#define SPG_General_USECutView
//SPG_Scale.h
#define SPG_General_USEScale
//HDF_Loader.h
//#define SPG_General_USEHDF
//Interpolateur.h
//#define SPG_General_USEInterpolateur
//PG_DiskBuffer.h
//#define SPG_General_USEDiskBuffer
//PRO_Loader.h
//#define SPG_General_USEPRO
//Profil3D.h
//#define SPG_General_USEProfil3D
//ProfilConvert.h
//#define SPG_General_USEPrCV
//ProfilManager.h
#define SPG_General_USEProfil
//ProfilExtract
//#define SPG_General_USEPrXt
//SGRAPH.h
//#define SPG_General_USESGRAPH
//SGRAPH_OPTS.h
//#define SPG_General_USESGRAPH_OPTS
//SPG_Accumulateur.h
//#define SPG_General_USEACC
//SPG_Window.h
#define SPG_General_USEWindow
//SPG_Buttons.h
#define SPG_General_USEButtons
//SPG_Carac.h
#define SPG_General_USECarac
//SPG_CaracFormatter.h
#define SPG_General_USECaracF
//SPG_Console.h
#define SPG_General_USEConsole
//SPG_Discretiseur.h
//#define SPG_General_USEDCRZ
//SPG_Interact2D.h
//#define SPG_General_USEINTERACT2D
//SPG_Files.h
#define SPG_General_USEFiles
//SPG_FileList.h
#define SPG_General_USEFileList
//SPG_Global.h
#define SPG_General_USEGlobal
//SPG_Graphics.h
#define SPG_General_USEGraphics
//SPG_GraphicsRenderPoly.h
//#define SPG_General_USEGraphicsRenderPoly
//SPG_Graphics_Effects.h
#define SPG_General_USEGEFFECT
//SPG_Histogram256.h
//#define SPG_General_USEHIST
//SPG_Inverseur.h
//#define SPG_General_USEINVJ0
//SPG_MECA.h
//#define SPG_General_USEMECA
//SPG_MemLink.h
//#define SPG_General_USEMELINK
//SPG_Network.h
//#define SPG_General_USENetwork
//SPG_NetworkEmule.h
//#define SPG_General_USENetworkEmule
//SPG_Network_OPTS.h
//#define SPG_General_USENetwork_OPTS
//SPG_NetworkProtocol.h
//#define SPG_General_USENetwork_Protocol
//SPG_ParaFit.h
#define SPG_General_USEPARAFIT
//SPG_K_FITW.cpp
#define SPG_General_USEWEIGHTEDPARAFIT
//SPG_ProgPrincipal.h
#define SPG_General_USEProgPrincipal
//PG_RingBuffer.h
#define SPG_General_USERingBuffer
//SPG_RingRec.h
//#define SPG_General_USERINGREC
//SPG_SerialComm.h
//#define SPG_General_USESerialComm
//SPG_Son.h
//#define SPG_General_USESON
//SPG_Timer.h
#define SPG_General_USETimer
//SPG_TxtIO.h
#define SPG_General_USETXT
//SPG_BmpIO.h
#define SPG_General_USEBMPIO
//SPG_FloatIO.h
#define SPG_General_USEFLOATIO
//SPG_WavIO.h
//#define SPG_General_USEWAVIO
//SUR_Loader.h
//#define SPG_General_USESUR
//SPG_DecalageDePhase.h
//#define SPG_General_USEDECPHASE
//FFT.h
//#define SPG_General_USEFFT
//FirDesign.h
//#define SPG_General_USEFirDesign
//UW_Utils.h
//#define SPG_General_USEUnwrap
//SPG_CD_Check.h
//#define SPG_General_USECDCHECK
//SPG_License_Check.h
//#define SPG_General_USELICENSECHECK
//SPG_SMTP.h
//#define SPG_General_USESMTPPOP3
//SPG_FTP.h
//#define SPG_General_USEFTP
//SPG_TreeView.h
//#define SPG_General_USETREEVIEW
//"SPG_LoopThread.h"
#define SPG_General_USELOOPTHREAD
//"SPG_ExceptionGenericHandler.h"
#define SPG_General_USEGENERICEXCEPTIONHANDLER
//SPG_Demodule.h
//#define SPG_General_USEDEMODULE
//SPG_GlobalThreadName.h
//efine SPG_USEGLOBALTHREADNAME
//changer aussi les sources de newmatlib (decocher exclude from build)
#define SPG_General_USENEWMATLIB
//SPG_SpinLock.h
#define SPG_General_USESpinLock
//WLTG.h
#define SPG_General_USEWLTG
#define SPG_General_USEWLTG_GLOBAL

#define SPG_General_USECONFIGFILE
#define SPG_General_USECONFIGFILEDLG

//divers
#define SPG_General_USEWindows
#define SPG_General_USEFilesWindows
//c'est l'un
//#define SPG_General_PGLib
//ou l'autre
//#define SGE_EMC
#define SGE_DiscardFace

#define SPG_General_USENIDAQmxEnum //même en mode émulé
#ifndef NOHARDWARE
//#define SPG_General_USECyUSB
#define SPG_General_USENIDAQmx				//Active acquisition NIDAQ Lenscan/Capablade (necessite NIDAQmx installé sur la machine)
//efine SPG_General_USESTIL					//Pour activer la sonde STIL (necessite la DLL STIL)
#define SPG_General_USECollimator
#define SPG_General_USEBench
//efine NoELISED
#endif

//fdef SPG_DEBUGCONFIG
//efine DebugSerialComm
//ndif

//Config pour les programmes en release mais pour lesquels on veut voir les messages d'erreur
#define DebugList
//#define DebugTimer

//Pour les cas particuliers de debogage en mode release
//#define DebugFloat
//#define DebugMem
//#define DebugNetwork

#define SPG_COMPANYNAME "FogaleProbe"

