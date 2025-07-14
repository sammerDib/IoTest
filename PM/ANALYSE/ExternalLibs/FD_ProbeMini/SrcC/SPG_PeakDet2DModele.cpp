
#include "SPG_General.h"

#ifdef SPG_General_USEPEAKDET2D

#include "SPG_Includes.h"

//#include "..\SrcC\SPG_SysInc.h"

#include <string.h>

#include <float.h>

#include <stdlib.h>
#include <stdio.h>

#ifdef UseCFGFile
#define PARAMETERFILENAME "LaserParams.txt"
#endif

#define LDMODELEPARAMSDEF_YSearchWidthUp 50
#define LDMODELEPARAMSDEF_YSearchWidthDn 150
#define LDMODELEPARAMSDEF_radiusc		60
#define LDMODELEPARAMSDEF_camera_z		52
#define LDMODELEPARAMSDEF_dfocal		-200
#define LDMODELEPARAMSDEF_dccd			5
#define LDMODELEPARAMSDEF_CCDWidth		22.5
#define LDMODELEPARAMSDEF_CCDHeight		16.8677
#define LDMODELEPARAMSDEF_YReverse		1
float LDMODELEPARAMSDEF_LaserIncidence[LASER_ANGLE_COUNT][LASER_INCIDENCE_COUNT]={{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},{2.01100,1.94600,1.88900},};
float LDMODELEPARAMSDEF_LaserOuverture[LASER_INCIDENCE_COUNT]={1.12900,1.25600,1.13500};
float LDMODELEPARAMSDEF_LaserLensloc[LASER_INCIDENCE_COUNT]={82.4700,65.8200,52.1900};
#define LDMODELEPARAMSDEF_LaserRefAngle		0
#define LDMODELEPARAMSDEF_LaserNumberingDirection		1
#define LDMODELEPARAMSDEF_LaserTilt(i)		0
#define LDMODELEPARAMSDEF_LaserAxisOffset	0
#define LDMODELEPARAMSDEF_LaserAxisGroundDirectionRad	0


void SPG_CONV MDLoadFromCFG(LDMODELEPARAMS& MDParams, SPG_CONFIGFILE& CFG)
{
	memset(&MDParams,0,sizeof(LDMODELEPARAMS));
	MDParams.Version=LDMODELEPARAMS_VERSION;
	CFG_GetIntDC(CFG,	MDParams.SizeX	,	768	, "Modele image size X");
	CFG_GetIntDC(CFG,	MDParams.SizeY	,	576	, "Modele image size X");
	CFG_GetFloatDC(CFG,	MDParams.YSearchWidthUp,	80	, "Search aera around laser modele - Up direction (pixels)");
	CFG_GetFloatDC(CFG,	MDParams.YSearchWidthDn,	120	, "Search aera around laser modele - Down direction (pixels)");
	CFG_GetFloatDC(CFG,	MDParams.radiusc,	60		, "Canon radius (mm)");
	CFG_GetFloatDC(CFG,	MDParams.camera_z,	52		, "On axis offset of the camera position (mm)");
	CFG_GetFloatDC(CFG,	MDParams.dfocal	,	-200	, "distance of the focal plane to the main axis: positve value -> above (mm)");
	CFG_GetFloatDC(CFG,	MDParams.dccd	,	5		, "Distance of the CCD virtual plane to the main axis: positive value -> above (mm)");
	CFG_GetFloatDC(CFG,	MDParams.CCDWidth,	22.5000	, "Virtual CCD Width (mm)");
	CFG_GetFloatDC(CFG,	MDParams.CCDHeight,	16.8677	, "Virtual CCD Height (mm)");
	CFG_GetIntDC(CFG,	MDParams.YReverse,	0		, "Modele Y Reverse: 0 or 1 - not coded");
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[0][0],	2.01100	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[0][1],	1.94600	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[0][2],	1.88900	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[1][0],	2.01100	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[1][1],	1.94600	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[1][2],	1.88900	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[2][0],	2.01100	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[2][1],	1.94600	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[2][2],	1.88900	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[3][0],	2.01100	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[3][1],	1.94600	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[3][2],	1.88900	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[4][0],	2.01100	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[4][1],	1.94600	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[4][2],	1.88900	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[5][0],	2.01100	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[5][1],	1.94600	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserIncidence[5][2],	1.88900	, "Laser incidence");//angle par rapport à l'axe du tube
	CFG_GetFloatDC(CFG,	MDParams.LaserOuverture[0],	1.0		, "Laser line length");//largeur de chacun des trois faisceaux
	CFG_GetFloatDC(CFG,	MDParams.LaserOuverture[1],	1.4		, "Laser line length");//largeur de chacun des trois faisceaux
	CFG_GetFloatDC(CFG,	MDParams.LaserOuverture[2],	1.3		, "Laser line length");//largeur de chacun des trois faisceaux
	CFG_GetFloatDC(CFG,	MDParams.LaserLensloc[0],	82.4700	, "Virtual laser origin");//calculé par le programme de Bruno
	CFG_GetFloatDC(CFG,	MDParams.LaserLensloc[1],	65.8200	, "Virtual laser origin");//calculé par le programme de Bruno
	CFG_GetFloatDC(CFG,	MDParams.LaserLensloc[2],	52.1900	, "Virtual laser origin");//calculé par le programme de Bruno
	CFG_GetFloatDC(CFG,	MDParams.LaserRefAngle	,	0		, "Laser #0 position relative to camera origin");//position des lasers par rapport au laser #0
	CFG_GetIntDC(CFG,	MDParams.LaserNumberingDirection,1	, "Laser numbering relative to camera turn angle direction (convention, +1 or -1)");//position des lasers par rapport au laser #0
	CFG_GetFloatDC(CFG,	MDParams.LaserTilt[0]	,	0	, "Laser fixturing rotation");//les lasers sont cylindriques donc les lignes peuvent ne pas être parfaitement horizontales
	CFG_GetFloatDC(CFG,	MDParams.LaserTilt[1]	,	0	, "Laser fixturing rotation");//les lasers sont cylindriques donc les lignes peuvent ne pas être parfaitement horizontales
	CFG_GetFloatDC(CFG,	MDParams.LaserTilt[2]	,	0	, "Laser fixturing rotation");//les lasers sont cylindriques donc les lignes peuvent ne pas être parfaitement horizontales
	CFG_GetFloatDC(CFG,	MDParams.LaserTilt[3]	,	0	, "Laser fixturing rotation");//les lasers sont cylindriques donc les lignes peuvent ne pas être parfaitement horizontales
	CFG_GetFloatDC(CFG,	MDParams.LaserTilt[4]	,	0	, "Laser fixturing rotation");//les lasers sont cylindriques donc les lignes peuvent ne pas être parfaitement horizontales
	CFG_GetFloatDC(CFG,	MDParams.LaserTilt[5]	,	0	, "Laser fixturing rotation");//les lasers sont cylindriques donc les lignes peuvent ne pas être parfaitement horizontales
	CFG_GetFloatDC(CFG,	MDParams.LaserAxisOffset	,	0	, "Weight effect: laser height sinusoidal correction");//la souris 'pointe' vers le haut ou le bas à cause du poids, du jeu mécanique dans le tube, et du jeu mécanique de la caméra
	CFG_GetFloatDC(CFG,	MDParams.LaserAxisGroundDirectionRad	,	0	, "Weight effect: ground direction relative to laser 0");//le 'bas' est une direction par rapport au laser 0
	return;
}

#ifdef UseCFGFile
int SPG_CONV SPG_PeakModeleInit(SPG_PEAKMODELE& PM, int CCDSizeX, int CCDSizeY, char* ParameterFileName)
#else
int SPG_CONV SPG_PeakModeleInit(SPG_PEAKMODELE& PM, int CCDSizeX, int CCDSizeY, LDMODELEPARAMS& Params, int sizeofParams)
#endif
{
	memset(&PM,0,sizeof(SPG_PEAKMODELE));//interdit si la structure contient des objets ayant des constructeurs implicites qui ont déjà été appelés
	PM.CCDSizeX=CCDSizeX;
	PM.CCDSizeY=CCDSizeY;

#ifdef UseCFGFile
	if(ParameterFileName==0)
	{
		CFG_Init(PM.CFG,Global.ProgDir,PARAMETERFILENAME,32);
	}
	else
	{
		CFG_Init(PM.CFG,0,ParameterFileName,32);
	}

	CFG_GetFloatDC(PM.CFG,PM.YSearchWidthUp,	30,	
	"Search width around laser theoretical line, Up direction");
	CFG_GetFloatDC(PM.CFG,PM.YSearchWidthDn,	220,	
	"Search width around laser theoretical line, Dn direction");

	//charge les paramètres géométriques du programme matlab
	SPG_PeakModeleLoadMatlabParams(PM.CFG,PM.p);

	CFG_Save(PM.CFG);//reécrit les valeurs par défaut si elles n'existaient pas
#else

	CHECK(!SPG_STVIsValidVersion(Params,LDMODELEPARAMS),"SPG_PeakModeleInit",return 0);
	DbgCHECK(sizeofParams!=sizeof(LDMODELEPARAMS),"SPG_PeakModeleInit");

	SPG_STVGetParam(PM.,	YSearchWidthUp						,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.,	YSearchWidthDn						,	LDMODELEPARAMS,Params,sizeofParams);

	PM.p.CCDSizeX=PM.CCDSizeX;
	PM.p.CCDSizeY=PM.CCDSizeY;

	SPG_STVGetParam(PM.p.,	radiusc								,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.p.,	camera_z							,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.p.,	dfocal								,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.p.,	dccd								,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.p.,	CCDWidth							,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.p.,	CCDHeight							,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.p.,	YReverse							,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGet2DArray_static(PM.p.,	LaserIncidence,LASER_ANGLE_COUNT,LASER_INCIDENCE_COUNT	,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetArray_static(PM.p.,	LaserOuverture,LASER_INCIDENCE_COUNT,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetArray_static(PM.p.,	LaserLensloc,LASER_INCIDENCE_COUNT	,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.p.,	LaserRefAngle						,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.p.,	LaserNumberingDirection				,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetArray_inline(PM.p.,	LaserTilt,LASER_ANGLE_COUNT	,	LDMODELEPARAMS,Params,sizeofParams);

	SPG_STVGetParam(PM.,	LaserAxisOffset						,	LDMODELEPARAMS,Params,sizeofParams);
	SPG_STVGetParam(PM.,	LaserAxisGroundDirectionRad			,	LDMODELEPARAMS,Params,sizeofParams);


#endif
	//Allocation de l'espace de travail pour combiner les lignes
	//buffer de longueur SizeX * (LASER_COUNT+LASER_INCIDENCE_COUNT+4)
	//regroupe 25 allocations en une

#define FORALLLASER(instruction) 	{for(int LaserID=0;LaserID<LASER_COUNT;LaserID++){instruction}}
#define FORALLINCIDENCE(instruction) 	{for(int Incidence=0;Incidence<LASER_INCIDENCE_COUNT;Incidence++){instruction}}
	{
		int YSize=CCDSizeX * (LASER_COUNT*5+LASER_INCIDENCE_COUNT+4);
		PM.YALLOC=SPG_TypeAlloc(YSize,float,"SPG_PeakModeleInit:YALLOC");
		float* YOFFSET=PM.YALLOC;
		FORALLLASER(PM.YModele[LaserID]=YOFFSET; YOFFSET+=PM.CCDSizeX;);
		FORALLLASER(PM.YModeleMin[LaserID]=YOFFSET; YOFFSET+=PM.CCDSizeX;);
		FORALLLASER(PM.YModeleMax[LaserID]=YOFFSET; YOFFSET+=PM.CCDSizeX;);
		FORALLLASER(PM.YMesure[LaserID]=YOFFSET; YOFFSET+=PM.CCDSizeX;);
		FORALLLASER(PM.YMesureDelta[LaserID]=YOFFSET; YOFFSET+=PM.CCDSizeX;);
		FORALLINCIDENCE(PM.YCombine[Incidence]=YOFFSET; YOFFSET+=PM.CCDSizeX;);
		PM.YResultat=YOFFSET; YOFFSET+=PM.CCDSizeX;
		PM.YFiltSrc=YOFFSET; YOFFSET+=PM.CCDSizeX;
		PM.YFiltTmp=YOFFSET; YOFFSET+=PM.CCDSizeX;
		PM.YFiltDst=YOFFSET; YOFFSET+=PM.CCDSizeX;
		CHECK(YOFFSET!=(PM.YALLOC+YSize),"SPG_PeakModeleInit",return 0);
	}
	{
		int YVSize=CCDSizeX * (LASER_COUNT*3+LASER_INCIDENCE_COUNT+1);
		PM.YVALLOC=SPG_TypeAlloc(YVSize,BYTE,"SPG_PeakModeleInit:YALLOC");
		BYTE* YVOFFSET=PM.YVALLOC;
		FORALLLASER(PM.YModeleValid[LaserID]=YVOFFSET; YVOFFSET+=PM.CCDSizeX;);
		FORALLLASER(PM.YModeleMinMaxValid[LaserID]=YVOFFSET; YVOFFSET+=PM.CCDSizeX;);
		FORALLLASER(PM.YMesureValid[LaserID]=YVOFFSET; YVOFFSET+=PM.CCDSizeX;);
		FORALLINCIDENCE(PM.YCombineValid[Incidence]=YVOFFSET; YVOFFSET+=PM.CCDSizeX;);
		PM.YResultatValid=YVOFFSET; YVOFFSET+=PM.CCDSizeX;
		CHECK(YVOFFSET!=(PM.YVALLOC+YVSize),"SPG_PeakModeleInit",return 0);
	}


	return PM.Etat=PEAKDET_OK;
}

int SPG_CONV SPG_PeakModeleGetY(SPG_PEAKMODELE& PM, float CameraAngleRotationRad, float CameraAngleTiltRad, int LaserID, float* Y, BYTE* Yvalid, int NumY)//Y est de longueur CCDSizeX NumY est la longueur de Y pour vérification de la correspondance
{//cette fonction reprend le calcul de Y(X) en fonction des parametres precalculés
	CHECK(PM.Etat==0,"SPG_PeakModeleGetY",return 0);
	CHECK(Y==0,"SPG_PeakModeleGetY",return 0);
	CHECK(!V_IsBound(LaserID,0,LASER_COUNT),"SPG_PeakModeleGetY",return 0);
	CHECK(NumY!=PM.CCDSizeX,"SPG_PeakModeleGetY",return 0);
	DbgCHECK(CameraAngleTiltRad>(1+FLT_EPSILON)*V_PI/2,"SPG_PeakModeleGetY: Camera tilt>90° unsupported");
	if(CameraAngleTiltRad>V_PI/2) CameraAngleTiltRad=V_PI/2;

	//entrées
	MATLAB_PARAM& p=PM.p;
	p.CameraAngleRotationRad=CameraAngleRotationRad;
	p.CameraAngleTiltRad=CameraAngleTiltRad;
	p.LaserID=LaserID;

	//sorties
	MATLAB_VAR m;//cette structure pourrait contenir des objets (Matrices), ne pas mettre
	//dans une structure c 'normale' sinon les memset(0) qui initialisent les structures font planter
	m.Y=Y;
	m.Yvalid=Yvalid;
	m.NumY=NumY;

	//calcule les parametres de projection (BB,nnorm,Pcam,NCONSTRAINT,psconstraint)
	simurays(m,p);

	return PEAKDET_OK;
}

void SPG_CONV SPG_PeakModeleClose(SPG_PEAKMODELE& PM)
{
	CHECK(PM.Etat==0,"SPG_PeakModeleClose",return);
	SPG_MemFree(PM.YALLOC);PM.YALLOC=0;
	SPG_MemFree(PM.YVALLOC);PM.YVALLOC=0;
#ifdef UseCFGFile
	CFG_Close(PM.CFG);
#endif
	PM.Etat=0;
	//memset(&PM,0,sizeof(SPG_PEAKMODELE));//interdit si la structure contient des objets ayant des destructeurs implicites appelés plus tard
	return;
}

#endif


