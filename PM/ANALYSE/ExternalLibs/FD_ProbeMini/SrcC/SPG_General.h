
#include "Config\SPG_Warning.h"

#define TOSTRING(v) #v
#define SPGMSG(f,l,s) message(__FILE__"("TOSTRING(l)"): " s)

//Flags de débogages, conventions d'appel
#ifdef _DEBUG
//DEBUG
//ragma SPGMSG(__FILE__,__LINE__,"DEBUG")
#include "Config\SPG_DebugConfig.h"
#else
//RELEASE
//ragma SPGMSG(__FILE__,__LINE__,"RELEASE")
#include "Config\SPG_ReleaseConfig.h"
#endif

//Compilateur INTEL
//efine IntelCompiler

//include un seul fichier de définition des modules à utiliser

#ifdef SRCCCONFIGMINI

#include "Config\SrcC_Mini.h"

#elif defined(SRCC_CAPABLADE)

#include "Config\CapabladeV3Config.h"
#error "Not allowed in capablade V3"
#else
//rror "Not allowed in capablade V3"
//#include "Config\LenscanSignalConfig.h"
#include "Config\FogaleProbeConfig.h"
//#include "Config\JeuxAubesConfig.h"
//#include "Config\JeuxAubesECLConfig.h"
//#include "Config\WatchLogTimeConfig.h"
//#include "Config\CutViewDemoConfig.h"
//#include "Config\JeuxAubesNoWConfig.h"
//#include "Config\DIOSynthesisConfig.h"
//#include "Config\BinarizeConfig.h"
//#include "Config\ConnexionConfig.h"
//#include "Config\AllConfig.h"
//#include "Config\SphereConfig.h"	//sans messages de preconditions
//#include "Config\CameraConfig.h"		//2DConvert,Ondes,BeamTracking,Sphere
//#include "Config\AllNoFFTConfig.h"		//2DConvert,Ondes,BeamTracking,Sphere
//#include "Config\CameraConfig.h"
//#include "Config\DblBuffConfig.h"			//DblBuffer
//#include "Config\CalPadConfig.h"
//#include "Config\JeuxAubesSNECMAConfig.h" //LSFIT
//#include "Config\Decorrelle2DConfig.h"
//#include "Config\WatchLogTimeConfig.h"
//#include "Config\LISE_EDI_DLLConfigTest.h" //LISE ED sur clock interferometrique
//#include "Config\LISE_EDI_DLLConfig.h" //LISE ED sur clock interferometrique
//#include "Config\LISE_ED_DLLConfigTest.h" //LISE ED
//#include "Config\LISE_ED_DLLConfig.h" //LISE ED
//#include "Config\APE_DLLConfig.h"		//APE
//#include "Config\ChromConfig.h"	
//#include "Config\SCX_DLLConfig.h"	
//#include "Config\MMF_DLLConfig.h"	
//#include "Config\SPG_CommConfig.h"
//#include "Config\LoopThreadConfig.h"
//#include "Config\TomoConfig.h" //SPG_Tomographie
//#include "Config\RaccordConfig.h" //StepHeight
//#include "Config\RGRtoAVIConfig.h"
//#include "Config\TempMonitorConfig.h"
//#include "Config\AubesConfig.h" //ne plus utiliser
//#include "Config\CamConfig.h"		//CamGrab,CamPan
//#include "Config\VideoExtractConfig.h" //ViedoExtract,VideoRecord
//#include "Config\AllNoFFTNetwork.h"
//#include "Config\UlipsConfig.h"
//#include "Config\UlipsServerConfig.h"
//#include "Config\VoitureConfig.h"
//#include "Config\SmallConfig.h"			//Demo3D,Bulle
//#include "Config\TMT_ReadConfig.h"			//Demo3D,Bulle
//#include "Config\WinampPlugInConfig.h"
//#include "Config\TestPortSerieConfig.h"
//#include "Config\BatchConvertConfig.h"
//#include "Config\LineDetectConfig.h"
#endif

//Configuration de la mémoire
#ifdef SPG_General_USEWindows
#define SPG_UseGlobalAlloc
//efine SPG_UseMalloc
//efine SPG_UseNew //modif pour voir le detail dans memory validator // attention avec newTZ la dimension allouee poour les SCI_PARAMETERS est incorrecte (pb de remplacement avec plusieurs structures definies localements et de tailles differentes - _CrtMemCheck renvoie une erreur juste apres sciParameters=0 dans PACKETCreateInterface)
#else
#define SPG_UseMalloc
#endif

//Configuration du timer
//mesure la frequence CPU dans une boucle plutot que dans un sleep
#define IntelSpeedStepFix
//utilise RDTSC plutot que QueryPerformanceFrequency dans SPG_Timer.h
#define TimerPentium
//Compilateur INTEL
#ifdef IntelCompiler
//Compilateur INTEL: influe sur la definition de V_Round dans V_General.h
#define IntelCompilerFPU
#pragma SPGMSG(__FILE__,__LINE__,"Assume Intel Compiler FPU")
#else
//Compilateur microsoft: restrict n'est pas défini
#define restrict
//ragma SPGMSG(__FILE__,__LINE__,"Assume Microsoft")
#endif

#ifndef SPG_COMPANYNAME
#define SPG_COMPANYNAME "Sylvain PETITGRAND"
#endif

#ifdef IntelCompiler
#pragma SPGMSG(__FILE__,__LINE__,"IntelCompiler")
#endif

#ifdef _WIN64
//FDE #error _WIN64
#endif


#include "SPG_Win98\SPG_WindowsConfig.h"

#define SPG_General_Included










