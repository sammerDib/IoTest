

#include "SPG_General.h"

#ifdef SPG_General_USEPEAKDET2D

#include "SPG_Includes.h"

#include <string.h>
#ifdef DebugPeakDet2D
#include <stdio.h>
#endif
#ifdef DebugFloat
#include <float.h>
#endif

#ifndef SPG_General_USEPARAFIT
#error Missing  SPG_General_USEPARAFIT
#endif

#define PEAKDET2DPARAMSDEF_SizeX				256 //orientation de l'image
#define PEAKDET2DPARAMSDEF_SizeY				256 //orientation de l'image
#define PEAKDET2DPARAMSDEF_Rotate				0 //orientation de l'image
#define PEAKDET2DPARAMSDEF_SpatialFilterKernelSize 15 //taille du filtre spatial 2D
#define PEAKDET2DPARAMSDEF_LowPassTimeCste		0.3 //moyennage temporel entre images 0=pas de moyennage
#define PEAKDET2DPARAMSDEF_PowLow				0.2 //filtrage pour récuperer le fond continu
#define PEAKDET2DPARAMSDEF_PowHigh				5 //filtrage pour recuperer le niveau max du signal
#define PEAKDET2DPARAMSDEF_MinSignalLevel		6 //niveau mini de SignalHigh-NoiseLow à considérer (compense une régularisation insuffisante)
#define PEAKDET2DPARAMSDEF_RegulariseKernelSize 127 //regularisation niveaux high/low
#define PEAKDET2DPARAMSDEF_WeightFilterMode		2	 //active la convolution directionnelle
#define PEAKDET2DPARAMSDEF_WeightFilterKernelSize	25 //regularisation ligne
#define PEAKDET2DPARAMSDEF_WeightFilterAngle	0.5 //rad	orientation ligne
#define PEAKDET2DPARAMSDEF_MaxResultPerRow		8 //longueur de la liste de pixels candidats à considérer
#define PEAKDET2DPARAMSDEF_RegularisedRefine	0 //1 ou 0
#define PEAKDET2DPARAMSDEF_Selectivity			100 //parametre de la loi de calcul de distance (PeakDet2D_GetDistance)
#define PEAKDET2DPARAMSDEF_Threshold			0.3 //seuil pour ignorer les points de faible intensité

//#define PEAKDET_SIGNAL(D2D) D2D.NoiseHigh+(D2D.NoiseHigh-D2D.NoiseLow)/D2D.PeakTimeCste
//#define PEAKDET_ISSIGNAL(D2D,V) (V>D2D.SignalLow)
//#define PEAKDET_WEIGHT(D2D,V) (V-D2D.SignalLow)/(D2D.SignalHigh-D2D.SignalLow)

//#define CD(P) P_Data(P)[i]


//fdef DebugPeakDet2D
//efine CPY(m) sprintf(Msg,#m " = %1.5f",(float)m);Console_Add(D2D.C,Msg);D2D.m=m
//lse
//efine CPY(m) D2D.m=m
//ndif

void SPG_CONV D2DLoadFromCFG(PEAKDET2DPARAMS& D2DParams, SPG_CONFIGFILE& CFG)
{
	memset(&D2DParams,0,sizeof(PEAKDET2DPARAMS));
	D2DParams.Version=PEAKDET2DPARAMS_VERSION;
	CFG_GetIntDC(CFG,	D2DParams.SizeX					,	768	, "Image size X");
	CFG_GetIntDC(CFG,	D2DParams.SizeY					,	576	, "Image size Y");
	CFG_GetIntDC(CFG,	D2DParams.Rotate				,	0	, "Image rotate 90° (0 or 1)");
	CFG_GetIntDC(CFG,	D2DParams.SpatialFilterKernelSize,	15	, "Image prefiltering");
	CFG_GetFloatDC(CFG,	D2DParams.LowPassTimeCste		,	0	, "Image sequence temporal lowpass");
	CFG_GetFloatDC(CFG,	D2DParams.PowLow				,	0.2	, "Exponent for background light intensity evaluation");
	CFG_GetFloatDC(CFG,	D2DParams.PowHigh				,	5	, "Exponent for laser light intensity evaluation");
	CFG_GetFloatDC(CFG,	D2DParams.MinSignalLevel		,	6	, "Min difference between background and laser light (regularisation)");
	CFG_GetIntDC(CFG,	D2DParams.RegulariseKernelSize	,	79	, "Filter size for light intensity evaluation");
	CFG_GetIntDC(CFG,	D2DParams.WeightFilterMode		,	2	, "0=No filter 1=Bow Tie 2=Bow Tie+Horizontal");
	CFG_GetIntDC(CFG,	D2DParams.WeightFilterKernelSize,	25	, "Filter for horizontal line enhancement");
	CFG_GetFloatDC(CFG,	D2DParams.WeightFilterAngle		,	0.5	, "Line tilt tolerance for filtering");
	CFG_GetIntDC(CFG,	D2DParams.MaxResultPerRow		,	8	, "Number of most-intense pixel to consider for processing");
	CFG_GetIntDC(CFG,	D2DParams.RegularisedRefine		,	0	, "Perform line position measurement on normalized prefiltered image (0) or regularized normalized prefiltered image (1)");
	CFG_GetFloatDC(CFG,	D2DParams.Selectivity			,	100	, "Law parameter for result sorting - distance vs intensity criteria");
	CFG_GetFloatDC(CFG,	D2DParams.Threshold				,	0.3	, "Minimum normalized line intensity");
	return;
}

/* fonctions de l'interface */

//int SPG_CONV PeakDet2D_Init(SPG_PEAKDET2D& D2D, int SizeX, int SizeY, int Rotate, int SpatialFilterKernelSize, float LowPassTimeCste, float PowLow, float PowHigh, float MinSignalLevel, int RegulariseKernelSize, int WeightFilterKernelSize, float WeightFilterAngle, int MaxResultPerRow, int RegularisedRefine, float Selectivity, float Threshold)
int SPG_CONV PeakDet2D_Init(SPG_PEAKDET2D& D2D, PEAKDET2DPARAMS& Params, int sizeofParams)
{
	memset(&D2D,0,sizeof(SPG_PEAKDET2D));
#ifdef DebugPeakDet2D

	SPG_Config_Save("PeakDet2D.txt");

	char Msg[256];
	FileConsole_Create(D2D.C,0,0,0,0,"PeakDet2D.txt",0,CONSOLE_WITHDATE);
	Console_Add(D2D.C,"PeakDet2D_Init ENTER");
#endif

	CHECK(!SPG_STVIsValidVersion(Params,PEAKDET2DPARAMS),"PeakDet2D_Init",return 0);
	DbgCHECK(sizeofParams!=sizeof(PEAKDET2DPARAMS),"PeakDet2D_Init");

	SPG_STVGetParam(D2D.,	SizeX,						PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	SizeY,						PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	Rotate,						PEAKDET2DPARAMS,Params,sizeofParams);

	if(D2D.Rotate)
	{
		V_SWAP(int,D2D.SizeX,D2D.SizeY);
	}

	D2D.SizeT=D2D.SizeX*D2D.SizeY;

	SPG_STVGetParam(D2D.,	SpatialFilterKernelSize,	PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	LowPassTimeCste,			PEAKDET2DPARAMS,Params,sizeofParams); 	
	SPG_STVGetParam(D2D.,	PowLow,						PEAKDET2DPARAMS,Params,sizeofParams);	
	SPG_STVGetParam(D2D.,	PowHigh,					PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	MinSignalLevel,				PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	RegulariseKernelSize,		PEAKDET2DPARAMS,Params,sizeofParams);
	
	SPG_STVGetParam(D2D.,	WeightFilterMode,		PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	WeightFilterKernelSize,		PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	WeightFilterAngle,			PEAKDET2DPARAMS,Params,sizeofParams);

	SPG_STVGetParam(D2D.,	MaxResultPerRow,			PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	RegularisedRefine,			PEAKDET2DPARAMS,Params,sizeofParams);

	SPG_STVGetParam(D2D.,	Selectivity,				PEAKDET2DPARAMS,Params,sizeofParams);
	SPG_STVGetParam(D2D.,	Threshold,					PEAKDET2DPARAMS,Params,sizeofParams);

	//toutes les fonctions supposent que tous les profils internes
	//sont de dimensions D2D.SizeX x D2D.SizeY
	P_Create(D2D.Signal,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	P_Create(D2D.Average,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);

	P_Create(D2D.ReferencePL,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	P_Create(D2D.NoiseLowPL,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	P_Create(D2D.NoiseLow,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	//P_Create(D2D.NoiseHigh,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);

	//P_Create(D2D.SignalLow,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	P_Create(D2D.ReferencePH,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	P_Create(D2D.SignalHighPH,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	P_Create(D2D.SignalHigh,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);

	P_Create(D2D.Weight,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	P_Create(D2D.WeightApriori,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);
	P_Create(D2D.WeightAprioriErase,D2D.SizeX,D2D.SizeY,1,1,0,0,0,0);


	D2D.Result=SPG_TypeAlloc(D2D.MaxResultPerRow*D2D.SizeX,SPG_PEAKPOSITION,"D2D Result");

	D2D.Reset=1;

	if(D2D.WeightFilterMode) PeakDet2D_InitKernelFilterWeight(D2D.Kernel,D2D.WeightFilterKernelSize,D2D.WeightFilterAngle);


#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_InitTimer(D2DT_Total				  ,"Total");
	S_InitTimer(D2DT_Update				  ,"Update");
	S_InitTimer(D2DT_UpdateRGB			  ,"UpdateRGB");
	S_InitTimer(D2DT_GetLine			  ,"GetLine");
	S_InitTimer(D2DT_ComputeSource		  ,"ComputeSource");
	S_InitTimer(D2DT_ComputeReference	  ,"ComputeReference");
	S_InitTimer(D2DT_ComputeLevels		  ,"ComputeLevels");
	S_InitTimer(D2DT_Regularise			  ,"Regularise");
	S_InitTimer(D2DT_ComputeWeight		  ,"ComputeWeight");
	S_InitTimer(D2DT_FilterWeight		  ,"FilterWeight");
	S_InitTimer(D2DT_FindYMaxAndErase	  ,"FindYMaxAndErase");
	S_InitTimer(D2DT_GetYList			  ,"GetYList");
	S_InitTimer(D2DT_RefineY			  ,"RefineY");
	S_InitTimer(D2DT_DrawResultRGB		  ,"DrawResultRGB");
	S_InitTimer(D2DT_RGB24toFloat		  ,"RGB24toFloat");
	S_InitTimer(D2DT_RGB24toFloatAndRotate,"RGB24toFloatAndRotate");

	S_StartTimer(D2DT_Total);
#endif
#endif
#ifdef DebugPeakDet2D
	Console_Close(D2D.C);
	FileConsole_Create(D2D.C,0,0,0,0,"PeakDet2D.txt",0,CONSOLE_WITHDATE);
	Console_Add(D2D.C,"PeakDet2D_Init RETURN");
#endif

	return D2D.Etat=PEAKDET_OK;
}

//#undef CPY

void SPG_CONV PeakDet2D_Close(SPG_PEAKDET2D& D2D)
{
#ifdef DebugTimer
#ifdef SPG_General_USETimer

	S_StopTimer(D2DT_Total);

	S_PrintRatio(D2D.T,D2DT_MAX);

#endif
#endif

#ifdef DebugMem
#ifdef DebugPeakDet2D
	Console_Add(D2D.C,"PeakDet2D_Close ENTER");
#endif
#endif

	P_Close(D2D.Signal);
	P_Close(D2D.Average);

	P_Close(D2D.ReferencePL);
	P_Close(D2D.NoiseLowPL);
	P_Close(D2D.NoiseLow);
	//P_Close(D2D.NoiseHigh);

	//P_Close(D2D.SignalLow);
	P_Close(D2D.ReferencePH);
	P_Close(D2D.SignalHighPH);
	P_Close(D2D.SignalHigh);

	P_Close(D2D.Kernel);

	P_Close(D2D.Weight);
	P_Close(D2D.WeightApriori);
	P_Close(D2D.WeightAprioriErase);

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	for(int i=0; i<D2DT_MAX; i++)
	{
		S_CloseTimer(D2D.T[i]);
	}
#endif
#endif

	SPG_MemFree(D2D.Result);
#ifdef DebugPeakDet2D
#ifdef DebugMem
	Console_Add(D2D.C,"PeakDet2D_Close RETURN");
	Console_Close(D2D.C);
#endif
#endif

	memset(&D2D,0,sizeof(SPG_PEAKDET2D));
	return;
}

//si Reset=0 dans ce cas on a les modes suivant
//Fast=0: mode normal (mise à jour des niveaux, filtrage spatial)
//Fast=1: mode 'preview' (pas de mise à jour des niveaux, normalistation et filtrage spatial)
//Fast=2: mode 'raw' (pas de mise à jour des niveaux, pas de normalisation, pas de filtrage spatial)
//si Reset=1: 
//recalcule de zero les niveaux de signal (lent mais nécessaire à la première image d'une séquence)
//Fast est ignoré
int SPG_CONV PeakDet2D_Update(SPG_PEAKDET2D& D2D, Profil& Signal, int Reset, int Fast)
{
	CHECK(D2D.Etat==0,"PeakDet2D_Update",return 0);
	CHECK(P_Etat(Signal)==0,"PeakDet2D_Update",return 0);

	//debogage
	D2D.Etape=0;
	D2D.SideEffect=0;

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_Update);
#endif
#endif

	//si on passe de fast à non fast un reset est imposé
	//if((D2D.Fast>0)&&(Fast==0)) D2D.Reset=1;

	D2D.Fast=Fast;

	//si le reset a déjà été mis par une autre fonction 
	//on le garde
	D2D.Reset|=Reset;
	//reset sera effacé avant le return de la fonction update

	//en mode reset on efface le flag Fast
	if(D2D.Reset) D2D.Fast=0;

	PeakDet2D_ComputeSource(D2D,Signal);
	//le signal à traité est désormais *(D2D.Source)

	if(D2D.Fast==0)
	{//si mode normal (ou reset)
		PeakDet2D_ComputeReference(D2D);
		if(D2D.Reset)
		{	//initialisation des seuils
			P_Copy(D2D.NoiseLowPL,D2D.ReferencePL);
			P_Copy(D2D.SignalHighPH,D2D.ReferencePH);
		}
		//calcule les niveaux haut et bas du signal
		if(D2D.Reset==0)
		{
			PeakDet2D_Regularise(D2D,D2D.RegulariseKernelSize,1);
		}
		else
		{
			int EtapeSav=D2D.Etape;
			PeakDet2D_Regularise(D2D,D2D.RegulariseKernelSize*3,3);
			D2D.Etape=EtapeSav;
			PeakDet2D_Regularise(D2D,D2D.RegulariseKernelSize,2);
		}
		PeakDet2D_ComputeLevels(D2D);

	}
	else
	{//en mode fast>0 on a sauté ces étapes
		D2D.Etape=4;
	}

	if(D2D.Fast<=1)
	{	//normalise
		PeakDet2D_ComputeWeight(D2D);//calcule la probabilité normalisée d'appartenance à la ligne laser

		//introduit la notion d'appartenance à priori d'un point 
		//à une ligne en fonction des points alentours. On implémente
		//une convolution avec un noyeau en forme de noeud papillon
		//angle donne la plage d'orientation possible 'a priori' de la ligne
		//de part et d'autre de l'horizontale et en radians
		//PeakDet2D_FilterWeight(D2D, D2D.WeightFilterKernelSize, D2D.WeightFilterAngle);//applique une sorte de régularisation
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_FilterWeight);
#endif
#endif
		//REGULARISATION A PRIORI DE WEIGHT
		if(D2D.WeightFilterMode)
		{
			memset(P_Data(D2D.WeightApriori),0,P_SizeX(D2D.WeightApriori)*P_SizeY(D2D.WeightApriori)*sizeof(float));
			//P_Convolve(D2D.WeightApriori,D2D.Weight,D2D.Kernel);
			P_ConvolveFast(D2D.WeightApriori,D2D.Weight,D2D.Kernel);
			//D2D.SideEffect+=(P_SizeX(D2D.Kernel)+1)>>1;
			if(D2D.WeightFilterMode==2)
			{
				P_FastConvLowPassH(D2D.WeightApriori,D2D.WeightFilterKernelSize);
				//P_FastConvLowPassH(D2D.WeightApriori,D2D.WeightFilterKernelSize/3);
				//P_FastConvLowPassH(D2D.WeightApriori,D2D.WeightFilterKernelSize/4);
			}
		}
		else
		{
			P_Copy(D2D.WeightApriori,D2D.Weight);
		}
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_FilterWeight);
#endif
#endif


		P_Copy(D2D.WeightAprioriErase,D2D.WeightApriori);

		P_SetBorderVal(D2D.WeightAprioriErase,0,D2D.SideEffect);

		for(int x=0;x<D2D.SizeX;x++)
		{
			int Count=PeakDet2D_GetYList(D2D,D2D.WeightAprioriErase,x,D2D.Result+x*D2D.MaxResultPerRow);//remplis la liste de résultats pour la colonne x

			for(int i=0;i<Count;i++)//Count <= D2D.MaxResultPerRow
			{
				PeakDet2D_RefineY(D2D,D2D.RegularisedRefine?D2D.WeightApriori:D2D.Weight,x,D2D.Result[x*D2D.MaxResultPerRow+i]);//remplis la liste de résultats pour la colonne x
				//PeakDet2D_RefineY(D2D,D2D.WeightApriori,x,D2D.Result[x*D2D.MaxResultPerRow+i]);//remplis la liste de résultats pour la colonne x
			}
		}
	}
	else
	{//pas de normalisation, pas de filtrage
		P_Copy(D2D.WeightAprioriErase,*(D2D.Source));

		for(int x=0;x<D2D.SizeX;x++)
		{
			int Count=PeakDet2D_GetYList(D2D,D2D.WeightAprioriErase,x,D2D.Result+x*D2D.MaxResultPerRow);//remplis la liste de résultats pour la colonne x

			for(int i=0;i<Count;i++)//Count <= D2D.MaxResultPerRow
			{
				PeakDet2D_RefineY(D2D,(*D2D.Source),x,D2D.Result[x*D2D.MaxResultPerRow+i]);//remplis la liste de résultats pour la colonne x
			}
		}
	}

	D2D.Reset=0;

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_Update);
#endif
#endif

	return PEAKDET_OK;
}

int SPG_CONV PeakDet2D_UpdateRGB(SPG_PEAKDET2D& D2D, BYTE* srcRGB24, int Pitch, int POCT, int Reset, int Fast)
{
	CHECK(D2D.Etat==0,"PeakDet2D_UpdateRGB",return 0);
	CHECK(srcRGB24==0,"PeakDet2D_UpdateRGB",return 0);
	CHECK(Pitch<POCT*(D2D.Rotate?D2D.SizeY:D2D.SizeX),"PeakDet2D_UpdateRGB",return 0);

	D2D.SideEffect=0;

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_UpdateRGB);
#endif
#endif

	if(!D2D.Rotate)
	{
		RGB24toFloat(D2D, D2D.Signal, srcRGB24, D2D.SizeX, D2D.SizeY, Pitch, POCT);
	}
	else
	{//en mode tourné à 90° la taille initiale de l'image est D2D.SizeY x D2D.SizeX
		RGB24toFloatAndRotate(D2D, P_Data(D2D.Signal), srcRGB24, D2D.SizeY, D2D.SizeX, Pitch, POCT);
	}

	int SideEffectSav=D2D.SideEffect;
	//PeakDet2D_Update remet à zero le compteur d'effet de bords
	//donc on lui réincrémentera son compteur avec les prétraitements 
	//qu'on vient de faire

	PeakDet2D_Update(D2D,D2D.Signal,Reset,Fast);

	D2D.SideEffect+=SideEffectSav;

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_UpdateRGB);
#endif
#endif
	return PEAKDET_OK;
}

float SPG_CONV PeakDet2D_GetDistance(float Consigne, float Y1, float I1, float Selectivity2)
{
	if(I1<=0) return 0;//la distance croit comme le carré de la distance à la consigne divisé par l'intensité du pic
	return (Selectivity2+(Y1-Consigne)*(Y1-Consigne))/(I1);
}

/***************************************************************************

  PEAKDET2D_GETLINE

***************************************************************************/

int SPG_CONV PeakDet2D_GetLine(SPG_PEAKDET2D& D2D, float* Y, BYTE* Yvalid, float* YConsigne, float* YMin, float* YMax, BYTE* YConsigneValid, SPG_PEAKPOSITION* YParameters)
{
	CHECK(D2D.Etat==0,"PeakDet2D_GetLine",return 0);
	CHECK(Y==0,"PeakDet2D_GetLine",return 0);
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_GetLine);
#endif
#endif

	float Selectivity2=D2D.Selectivity*D2D.Selectivity;

	if(YConsigne)
	{//une courbe est définie comme référence, la fonction retourne les points
	//les plus proches de cette référence (pour être sélectif s'il y a plusieurs lignes
	//dans l'image
		for(int x=0;x<D2D.SizeX;x++)
		{
			int iFound=-1; 
			Y[x]=0;
			if(Yvalid) Yvalid[x]=0;//marque le point comme invalide
			if(YParameters) YParameters[x].Etat=0;
			if((YConsigneValid==0)||(YConsigneValid[x]))
			{//ne recherche pas la ligne le long des bords de l'image (effets de bord des traitements)
				SPG_PEAKPOSITION* ResultRow=D2D.Result+x*D2D.MaxResultPerRow;
				for(int i=0;i<D2D.MaxResultPerRow;i++)
				{
					if(ResultRow[i].Etat==SPG_PEAKPOSITIONFLOAT)
					{//si c'est un résultat valide //DbgCHECK(ResultRow[i].DbgAbscisse!=x,"PeakDet2D_GetLine");
						if(YMin)//si on a passé en argument des bornes de recherche
						{//ne considere que les resultats entre les bornes
							if(YMin[x]>ResultRow[i].Position) continue;
						}
						if(YMax)//si on a passé en argument des bornes de recherche, 
						{//ne considere que les resultats entre les bornes
							if(YMax[x]<ResultRow[i].Position) continue;
						}
				//if(x==754)
				//{
				//	int a=0;
				//}
						if(ResultRow[i].Intensity<D2D.Threshold) continue;


						if(	(iFound==-1)|| //au premier tour de boucle on a iFound=-1, et on prend le premier point de la liste sans condition
							(PeakDet2D_GetDistance(YConsigne[x],ResultRow[iFound].Position,ResultRow[iFound].IntensityInt,Selectivity2)>
								PeakDet2D_GetDistance(YConsigne[x],ResultRow[i].Position,ResultRow[i].IntensityInt,Selectivity2))
						  )
						{ 
							iFound=i;	
							Y[x]=ResultRow[i].Position;
							if(Yvalid) Yvalid[x]=1;
							if(YParameters) YParameters[x]=ResultRow[i];
/*fdef DebugList
							if(!((YMin[x]<ResultRow[i].Position)&&(YMax[x]>ResultRow[i].Position))) 
							{
								DbgCHECK(1,"PeakDet2D_GetLine out of min/max");
							}
#end*/
						}
					}
				}
			}
		}
	}
	else
	{//pas de consigne, ne sortir que le point le plus intense
		for(int x=0;x<D2D.SizeX;x++)
		{
			Y[x]=0;
			if(Yvalid) Yvalid[x]=0;//marque le point comme invalide
			if((x>=D2D.SideEffect)&&(x<D2D.SizeX-D2D.SideEffect))
			{//ne recherche pas la ligne le long des bords de l'image (effets de bord des traitements)
				SPG_PEAKPOSITION* ResultRow=D2D.Result+x*D2D.MaxResultPerRow;
				//for(int i=D2D.MaxResultPerRow-1;i>=0;i--)
				for(int i=0;i<D2D.MaxResultPerRow;i++)
				{
					if(ResultRow[i].Etat==SPG_PEAKPOSITIONFLOAT)
					{//si c'est un résultat valide
						Y[x]=ResultRow[i].Position;
						if(Yvalid) Yvalid[x]=1;
						if(YParameters) YParameters[x]=ResultRow[i];
						break;
					}
				}
			}
		}
	}

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_GetLine);
#endif
#endif
	return PEAKDET_OK;
}

/* fonctions internes */

//calcule le signal source (avec ou sans passe bas temporel)
int SPG_CONV PeakDet2D_ComputeSource(SPG_PEAKDET2D& D2D, Profil& Current)
{
	CHECK(D2D.Etat==0,"PeakDet2D_ComputeAverage",return 0);
	CHECK(P_Etat(Current)==0,"PeakDet2D_ComputeAverage",return 0);

	//debogage
	CHECK(D2D.Etape!=0,"PeakDet2D_ComputeAverage",return 0);
	D2D.Etape=1;
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_ComputeSource);
#endif
#endif

	if(D2D.LowPassTimeCste<=0)
	{//pas de moyennage temporel
		D2D.Source=&Current;
	}
	else
	{
		D2D.Source=&D2D.Average;
		if(D2D.Reset)
		{//moyennage temporel, etape 0
			P_Copy(D2D.Average,Current);//a la premiere image on copie directemnt
		}
		else
		{//moyennage temporel, etapes >0 //LowPassTimeCste=0.1 -> 10 images, LowPassTimeCste=0.5 -> 2 images, LowPassTimeCste=0.05 -> 20 images
			for(int i=0;i<D2D.SizeT;i++)
			{
				P_Data(D2D.Average)[i]=
					P_Data(D2D.Average)[i]*(1-D2D.LowPassTimeCste)+
					P_Data(Current)[i]*D2D.LowPassTimeCste;
			}
		}
	}
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_ComputeSource);
#endif
#endif
	return PEAKDET_OK;
}

//calcule les niveaux de signal à la puissance nécessaire pour utiliser la régularisation
int SPG_CONV PeakDet2D_ComputeReference(SPG_PEAKDET2D& D2D)
{
	CHECK(D2D.Etat==0,"PeakDet2D_ComputeReference",return 0);
	CHECK(D2D.Source==0,"PeakDet2D_ComputeReference",return 0);

	//debogage
	CHECK(D2D.Etape!=1,"PeakDet2D_ComputeReference",return 0);
	D2D.Etape=2;
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_ComputeReference);
#endif
#endif

	float* DSource=P_Data((*D2D.Source));
	for(int i=0;i<D2D.SizeT;i++)
	{
		CHECK(DSource[i]<0,"PeakDet2D_ComputeReference",DSource[i]=0);
		P_Data(D2D.ReferencePH)[i]=pow(DSource[i],D2D.PowHigh);
		CHECKFLOAT(P_Data(D2D.ReferencePH)[i],"PeakDet2D_ComputeReference");
		P_Data(D2D.ReferencePL)[i]=pow(DSource[i],D2D.PowLow);
		CHECKFLOAT(P_Data(D2D.ReferencePL)[i],"PeakDet2D_ComputeReference");
	}
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_ComputeReference);
#endif
#endif
	return PEAKDET_OK;
}

//a l'inverse calcule les niveaux de signal pour les utiliser pour calculer Weight
int SPG_CONV PeakDet2D_ComputeLevels(SPG_PEAKDET2D& D2D)
{
	CHECK(D2D.Etat==0,"PeakDet2D_ComputeLevels",return 0);

	//debogage
	CHECK(D2D.Etape!=3,"PeakDet2D_ComputeLevels",return 0);
	D2D.Etape=4;
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_ComputeLevels);
#endif
#endif

	CHECK(D2D.Source==0,"PeakDet2D_ComputeLevels",return 0);

	float invPowHigh=1/D2D.PowHigh;
	float invPowLow=1/D2D.PowLow;
	for(int i=0;i<D2D.SizeT;i++)
	{
		
		//CHECK(P_Data(D2D.SignalHighPH)[i]<0,"PeakDet2D_ComputeLevels",P_Data(D2D.SignalHighPH)[i]=0);
		if(P_Data(D2D.SignalHighPH)[i]<0)
		{
			P_Data(D2D.SignalHigh)[i]=0;
		}
		else
		{
			P_Data(D2D.SignalHigh)[i]=pow(P_Data(D2D.SignalHighPH)[i],invPowHigh);
		}
		CHECKFLOAT(P_Data(D2D.SignalHigh)[i],"PeakDet2D_ComputeLevels");

		//CHECK(P_Data(D2D.NoiseLowPL)[i]<0,"PeakDet2D_ComputeLevels",P_Data(D2D.NoiseLowPL)[i]=0);
		if(P_Data(D2D.NoiseLowPL)[i]<0)
		{
			P_Data(D2D.NoiseLow)[i]=0;
		}
		else
		{
			P_Data(D2D.NoiseLow)[i]=pow(P_Data(D2D.NoiseLowPL)[i],invPowLow);
		}
		CHECKFLOAT(P_Data(D2D.NoiseLow)[i],"PeakDet2D_ComputeLevels");
	}
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_ComputeLevels);
#endif
#endif
	return PEAKDET_OK;
}

//filtre les niveaux de signal haut et bas en fonction des références calculées
int SPG_CONV PeakDet2D_Regularise(SPG_PEAKDET2D& D2D, int RegulariseKernelSize, int Iter)
{
	CHECK(D2D.Etat==0,"PeakDet2D_Regularise",return 0);

	//debogage
	CHECK(D2D.Etape!=2,"PeakDet2D_Regularise",return 0);
	D2D.Etape=3;
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_Regularise);
#endif
#endif

	//CHECK(D2D.LowPassTimeCste<=0,"PeakDet2D_Regularise",return 0);

	//travaille sur le carré du signal pour avoir
	//un effet d'extension spatiale non linéaire favorisant
	//l'extension du signal utile
	//reciproquement travaille sur la racine carrée du bruit
	P_LowPassMax(D2D.SignalHighPH,D2D.ReferencePH,RegulariseKernelSize,Iter);
	//lisse le signal de bruit avec un filtre 'min'
	P_LowPassMin(D2D.NoiseLowPL,D2D.ReferencePL,RegulariseKernelSize,Iter);
	//posttraitement:
	//lisse le signal pour que l'estimateur de l'enveloppe du signal
	//soit à sommet plat pour ne pas modifier la forme locale de la 
	//distribution d'intensité
	P_FastConvLowPass(D2D.SignalHighPH,RegulariseKernelSize);
	//P_FastConvLowPass(D2D.SignalHighPH,RegulariseKernelSize);
	P_FastConvLowPass(D2D.NoiseLowPL,RegulariseKernelSize);
	//P_FastConvLowPass(D2D.NoiseLowPL,RegulariseKernelSize);
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_Regularise);
#endif
#endif
	return PEAKDET_OK;
}

int SPG_CONV PeakDet2D_ComputeWeight(SPG_PEAKDET2D& D2D)
{
	CHECK(D2D.Etat==0,"PeakDet2D_ComputeResult",return 0);

	//debogage
	CHECK(D2D.Etape!=4,"PeakDet2D_ComputeLevels",return 0);
	D2D.Etape=5;
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_ComputeWeight);
#endif
#endif

	{//calcule le signal normalisé (signal-minorant du bruit) / (majorant du signal - minorant du bruit)
		float const* const DSource=P_Data((*D2D.Source));
		float const* const DSHigh=P_Data(D2D.SignalHigh);
		float const* const DSLow=P_Data(D2D.NoiseLow);
		float	   * const DWeight=P_Data(D2D.Weight);
		for(int i=0;i<D2D.SizeT;i++)
		{
			float W;
			float D=DSHigh[i]-DSLow[i];
			if(D>D2D.MinSignalLevel)//en unité intensité 0-255
				{W=(DSource[i]-DSLow[i])/D;}
			else 
				{W=0;}
			DWeight[i]=W;
		}
	}
	//P_SetBorderVal(D2D.Weight,0,D2D.RegulariseKernelSize);//met à zero le bord pour les effets de bord

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_ComputeWeight);
#endif
#endif
	return PEAKDET_OK;
}

//calcule WeightApriori à partir de Weight et d'une hypothèse d'orientation des lignes et d'alignement des points
int SPG_CONV PeakDet2D_InitKernelFilterWeight(Profil& Kernel, int KernelSize, float Angle)
{
	KernelSize|=1;//kernel est de taille impaire pour ne pas avoir d'effet de translation
	//lors de la convolution
	//exemple KernelSize = 11

	int KernelSizeX=(KernelSize|=1);
	int KernelSizeY=V_Ceil(KernelSizeX*sin(Angle))|1;

	P_Create(Kernel,KernelSizeX,KernelSizeY);
	int KcX=KernelSizeX/2;//division entière avec troncature pour trouver le centre
	int KcY=KernelSizeY/2;//division entière avec troncature pour trouver le centre
	//exemple Kc = 11/2 = 5
	for(int y=0;y<=KcY;y++)
	{
		for(int x=0;x<=KcX;x++)
		{
			float p=KcX/(KcX+5.0f*x*x/KcX);
			//float p=3.0f/(3.0f+x);
			if((x!=0)||(y!=0))
			{
				/*
				//implementation 1
				float Amin=atan2(V_Max((y-1),0),x);
				float Amax=atan2(y,x);
				if(Amin>Angle) 
				{
					p=0;//on est hors de la plage angulaire possible pour la ligne
				}
				else if((Amax>Angle) && (Amax>Amin))//on est sur le bord: transition douce
				{
					p*=(Angle-Amin)/(Amax-Amin);
				}
				//else = if(Amax<Angle) = ne pas changer p//on est dans la plage
				*/
				//implementation 2
				//float A=atan2(V_Max((y-1),0),x);
				float A=atan2(V_Max((y-0.5f),0),x+0.5f);
				if(A<Angle)
				{
					p*=cos(V_PI*A/(2*Angle));
				}
				else
				{
					p=0;
				}

			}
			P_Element(Kernel,KcX+x,KcY+y)=p;
			P_Element(Kernel,KcX-x,KcY+y)=p;
			P_Element(Kernel,KcX+x,KcY-y)=p;
			P_Element(Kernel,KcX-x,KcY-y)=p;
		}
	}

	P_Apodise(Kernel,3,2);
	return PEAKDET_OK;
}

int SPG_CONV PeakDet2D_GetYList(SPG_PEAKDET2D& D2D,Profil& WErase,int x,SPG_PEAKPOSITION* Result)
{//remplis MaxResultPerRow éléments du tableau Result avec les positions y trouvées par
//l'estimateur WeightApriori
	CHECK(D2D.Etat==0,"PeakDet2D_GetYList",return 0);
	CHECK(Result==0,"PeakDet2D_GetYList",return 0);
	CHECK(P_Etat(WErase)==0,"PeakDet2D_GetYList",return 0);
	CHECK(!V_IsBound(x,0,D2D.SizeX),"PeakDet2D_GetYList",return 0);

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_GetYList);
#endif
#endif

	int Count=0;
	while(Count<D2D.MaxResultPerRow)
	{//la détection des lignes se fait avec 'weight a priori' pour plus de détectivité
		if(P_FindYMaxAndErase(WErase,x, Result[Count].PosInt, Result[Count].IntensityInt, Result[Count].WidthInt)<0) break;
		//Result[Count].DbgAbscisse=x;
		Result[Count].Etat=SPG_PEAKPOSITIONINT;
		Count++;
	}
	for(int i=Count;i<D2D.MaxResultPerRow;i++)
	{//efface la fin de la liste de résultat si il y a moins de MaxResultPerRow résultats
		Result[i].Etat=SPG_PEAKPOSITIONINVALID;
	}

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_GetYList);
#endif
#endif
	
	return Count;
}

int SPG_CONV PeakDet2D_RefineY(SPG_PEAKDET2D& D2D, Profil& W, int x, SPG_PEAKPOSITION& Result)
{//raffine un résultat entier
//la mesure de position se fait avec 'weight' pour plus de sensibilité
	CHECK(D2D.Etat==0,"PeakDet2D_RefineY",return 0);
	CHECK(P_Etat(W)==0,"PeakDet2D_RefineY",return 0);
	CHECK(!V_IsBound(x,0,D2D.SizeX),"PeakDet2D_RefineY",return 0);
	if(Result.Etat!=SPG_PEAKPOSITIONINT) return 0;

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_RefineY);
#endif
#endif

	//DbgCHECK(Result.fit_x!=x,"PeakDet2D_RefineY");

	float* F=P_Data(W)+x;//la mesure de position se fait avec 'weight' pour plus de sensibilité
	//float* F=P_Data(D2D.WeightAprioriErase)+x;//debogage
	int FPitch=D2D.SizeX;

//on fitte avec des paraboles de 3 à 13 pixels 
//centrées entre y-2 et y+2 par rapport à la valeur
//retournée par GetYList
#define ScanRange 1

	//macro définie seulement en debug qui alloue un tableau
	//de n+2 éléments (un élément de plus au début et a la fin 
	//pour vérifier les écritures hors du tableau
	SPG_ArrayStackAlloc(int,fit_y,SPG_ParaFitArraySize*(2*ScanRange+1));
	SPG_ArrayStackAlloc(float,fit_a,SPG_ParaFitArraySize*(2*ScanRange+1));
	SPG_ArrayStackAlloc(float,fit_b,SPG_ParaFitArraySize*(2*ScanRange+1));
	SPG_ArrayStackAlloc(float,fit_c,SPG_ParaFitArraySize*(2*ScanRange+1));

	//scanne les positions possibles du laser autour de l'estimation int
	int t=SPG_ParaFit3to13Scan(F,FPitch,0,D2D.SizeY,Result.PosInt,ScanRange,fit_y,fit_a,fit_b,fit_c);

	//vérification de taille des tableaux
	DbgCHECK(t!=(SPG_ParaFitArraySize*(2*ScanRange+1)),"PeakDet2D_GetYList: SPG_ParaFit3to13Scan");

	//recherche le meilleur fit selon un critère de dimensions de fit et de largeur de ligne
	float ConfidenceMax;//valeur du maximum
	int iFound=-1;//position du maximum, -1=invalide
	int fFound=-1;
	//float SumPosition=0;
	//float SumWeight=0;

	//on ne part pas de zero pour exclure le fit de dimension 3
	for(int f=0;f<SPG_ParaFitArraySize;f++)
	{
		for(int s=0;s<2*ScanRange+1;s++)
		{
			int i=f+s*SPG_ParaFitArraySize;
			if(fit_a[i]>=0) continue;//parabole inversée: inexploitable
			float ySommet=-fit_b[i]/(2*fit_a[i]);//position du sommet de la parabole
			float DeltaMax=0.7f;//+0.5f*f;
			if((ySommet<-DeltaMax)||(ySommet>DeltaMax)) continue;//la parabole n'est pas centrée sur les données

//#define WidthBestRatio 1.5
			//float Confidence=fabs(Width/(SPG_ParaFitMin+2*f)-WidthBestRatio);

			float Discriminant=fit_b[i]*fit_b[i]-4*fit_a[i]*fit_c[i];
			//float Confidence=-fit_a[i];
			float Width=-sqrt(Discriminant)/fit_a[i];//ecart entre les zéros de la parabole, a<0;
			//if(Width>(1+1.0f*(SPG_ParaFitMin+2*f))) continue;
			float Position=fit_y[i]+ySommet;
			if(!V_IsBound(Position,0,D2D.SizeY)) continue; //si la ligne est hors de l'ecran on ignore le resultat
			float Intensity=fit_c[i]-fit_b[i]*fit_b[i]/(4*fit_a[i]);
			if(Intensity<=0) continue;

			//l'estimateur de la qualité du fit repose sur le fait que la largeur 
			//entre les deux zéro est du même ordre de grandeur que la longueur du fit
			float Confidence=fabs(
				Width-1.5f*(SPG_ParaFitMin+2*f)	//1.5
				);///(SPG_ParaFitMin+2*f);
			
			if((iFound==-1)||(Confidence<ConfidenceMax))
			{
				Result.Width=-sqrt(Discriminant)/fit_a[i];//ecart entre les zéros de la parabole, a<0;
				Result.Position=Position;
				Result.Intensity=Intensity; //c-b²/4a
				Result.FitSize=SPG_ParaFitMin+2*f;
				Result.fit_y=fit_y[i];
				Result.fit_a=fit_a[i];
				Result.fit_b=fit_b[i];
				Result.fit_c=fit_c[i];
				Result.Etat=SPG_PEAKPOSITIONFLOAT;
				ConfidenceMax=Confidence;
				iFound=i;
				fFound=f;
			}
		}
	}
	//les tableaux statiques c'est dangereux
	SPG_ArrayStackCheck(fit_y);	SPG_ArrayStackCheck(fit_a);	SPG_ArrayStackCheck(fit_b);	SPG_ArrayStackCheck(fit_c);

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_RefineY);
#endif
#endif

	if(fFound==-1) 
		return 0;
	else
		return PEAKDET_OK;
}

int SPG_CONV PeakDet2D_DrawResultRGB(SPG_PEAKDET2D& D2D, BYTE* dstRGB24, int Pitch, int POCT, int DisplayType)
{
	CHECK(D2D.Etat==0,"PeakDet2D_DrawResultRGB",return 0);
	CHECK(dstRGB24==0,"PeakDet2D_DrawResultRGB",return 0);
	CHECK((POCT!=3)&&(POCT!=4),"PeakDet2D_DrawResultRGB",return 0);
	CHECK(Pitch<POCT*(D2D.Rotate?D2D.SizeY:D2D.SizeX),"PeakDet2D_DrawResultRGB",return 0);

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_DrawResultRGB);
#endif
#endif

	for(int x=0;x<D2D.SizeX;x++)
	{
		SPG_PEAKPOSITION* RowResult=D2D.Result+x*D2D.MaxResultPerRow;
		for(int i=0;i<D2D.MaxResultPerRow;i++)
		{
			SPG_PEAKPOSITION& Result=RowResult[i];

			if(Result.Etat==SPG_PEAKPOSITIONFLOAT)
			{

				if(DisplayType==0)
				{
					int ys=V_Round(Result.Position);
					if((ys>=0)&&(ys<D2D.SizeY))
					{
						int ipix; if(!D2D.Rotate) {ipix=POCT*x+ys*Pitch;} else {ipix=POCT*ys+x*Pitch;}
						dstRGB24[ipix]=0;
						dstRGB24[ipix+1]=255;
						dstRGB24[ipix+2]=0;
					}
				}
				else if (DisplayType==1)
				{
#define DRAW_OFFSET 20
					int y=V_Round(Result.Position)+DRAW_OFFSET;
					int T=V_Round(0.5+Result.Width/1.8);
					for(int ys=V_Max(y-T,0);ys<V_Min(y+T+1,D2D.SizeY);ys++)
					{
						int ipix; if(!D2D.Rotate) {ipix=POCT*x+ys*Pitch;} else {ipix=POCT*ys+x*Pitch;}

						int xi=(ys-Result.fit_y-DRAW_OFFSET);

						int R=256*(xi*xi*Result.fit_a+xi*Result.fit_b+Result.fit_c);
						int G=0;
						int B=0;
						
						if(xi==0)
						{
							B=128;
							G=128;
						}
						if(abs(xi)==((Result.FitSize-1)/2))
						{
							G=64;
						}
						dstRGB24[ipix]=B;
						dstRGB24[ipix+1]=G;
						dstRGB24[ipix+2]=V_Sature(R,0,255);
					}
				}
			}
		}
	}
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_DrawResultRGB);
#endif
#endif
	return PEAKDET_OK;
}

void SPG_CONV PeakDet2D_DrawLaserParameters(G_Ecran& E,PixCoul& Color,SPG_PEAKPOSITION* YParameters,int NumY)
{
	CHECK(E.Etat==0,"PeakDet2D_DrawLaserParameters",return);
	CHECK(YParameters==0,"PeakDet2D_DrawLaserParameters",return);
	for(int x=0;x<NumY;x++)
	{
		if(YParameters[x].Etat)
		{
			//G_DrawPixel(E,x,YParameters[x].Position,Color.Coul);
			int iyc=V_Round(YParameters[x].Position);
			int iw=V_Round(0.5+YParameters[x].Width/1.8);
			for(int y=iyc-iw;y<=iyc+iw;y++)
			{
				int xi=y-iyc;
				float fc=(xi*xi*YParameters[x].fit_a+xi*YParameters[x].fit_b+YParameters[x].fit_c);
				fc=V_Sature(fc,0,1);
				PixCoul CM;
				CM.B=Color.B*fc;
				CM.V=Color.V*fc;
				CM.R=Color.R*fc;
				G_DrawPixel(E,x,y,CM.Coul);
			}
			{
				PixCoul CM=Color;
				CM.B=0x80;
				G_DrawPixel(E,x,iyc-(YParameters[x].FitSize-1)/2,CM.Coul);
				G_DrawPixel(E,x,iyc+(YParameters[x].FitSize-1)/2,CM.Coul);
			}
		}
	}
	return;
}

/*

FONCTIONS SPECIFIQUES DE CALCUL DE L'INTENSITE D'UN LASER ROUGE A PARTIR D'UNE IMAGE RGB

*/

//ces constantes sont à sortir en paramètre,
//les valeurs optimum sont à déterminer en fonction
//de la selectivité en longueur d'onde des composantes RVB, 
//et de la longueur d'onde du laser

//detection de l'intensité du laser
//soustraction du fond continu
/*
//cas avec suppression du fond continu
const float wR=1.3f;
const float wG=-0.2f;
const float wB=-1.1f;
*/
//detection de l'intensité du laser
//quand la composante rouge est saturée
/*
//cas avec suppression du fond continu
const float wGs=3.5f;
const float wBs=-3.5f;
*/

//cas sans suppression du fond continu
#define wR 0.6f
#define wG 0.35f
#define wB 0.35f

//cas sans suppression du fond continu
#define wGs 1.6f//2.5f
#define wBs 0.7f//-0.5f

//convertit l'image RGB et image d'intensité laser
void RGB24toFloat(SPG_PEAKDET2D& D2D, Profil& Pdst, BYTE* srcRGB24, int SizeX, int SizeY, int PitchSrc, int POCT, bool FilterInPlace)
{
//SizeX x SizeY sont les dimensions de l'image originale RGB
//le tableau destination *dst doit avoir les dimensions SizeX x SizeY
	CHECK(P_Etat(Pdst)==0,"RGB24toFloat",return);
	CHECK(srcRGB24==0,"RGB24toFloat",return);
	CHECK((POCT!=3)&&(POCT!=4),"RGB24toFloat",return);
	CHECK(PitchSrc<POCT*SizeX,"RGB24toFloat",return);

	G_Ecran E;

	if(FilterInPlace)
	{
		G_InitAliasMemEcran(E,srcRGB24,PitchSrc,POCT,SizeX,SizeY);
	}
	else
	{
		G_InitMemoryEcran(E,POCT,SizeX,SizeY);

		G_Ecran ETmp;
		G_InitAliasMemEcran(ETmp,srcRGB24,PitchSrc,POCT,SizeX,SizeY);
		G_Copy(E,ETmp);
		G_CloseEcran(ETmp);
	}


	if(D2D.SpatialFilterKernelSize==2)
	{
		G_Soften2x2(E);
	}
	else if(D2D.SpatialFilterKernelSize>=3)
	{
		for(int i=2;i<D2D.SpatialFilterKernelSize;i++)
		{
			G_Soften3x3rz(E);
		}
	}

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_RGB24toFloat);
#endif
#endif

#ifdef DebugRGBCorrel
	FILE* F;
	F=fopen("LineDetect.txt","wb");
	if(F)
	{ //ecris la ligne d'entete
		char sRVB[256]; sprintf(sRVB,"R\tV\tB\tI\r\n"); fwrite(sRVB,strlen(sRVB),1,F);
	}
#endif

	CHECK((G_SizeX(E)>P_SizeX(Pdst)),"RGB24toFloat",return);
	CHECK((G_SizeY(E)>P_SizeY(Pdst)),"RGB24toFloat",return);

	float* dstW=P_Data(Pdst)
		+((P_SizeX(Pdst)-G_SizeX(E))>>1)
		+((P_SizeY(Pdst)-G_SizeY(E))>>1)*P_SizeX(Pdst);
	BYTE* srcR=G_MECR(E);

	for(int y=0;y<G_SizeY(E);y++)
	{
		for(int x=0;x<G_SizeX(E);x++)
		{

			float I1=
				wB*(srcR[POCT*x])+
				wG*(srcR[POCT*x+1])+
				wR*(srcR[POCT*x+2]);//cas rouge non saturé

			/*
			float I2=
				wGs*(srcR[POCT*x+1])+
				wR*(srcR[POCT*x+2]);//cas rouge saturé

			dstW[x]=V_Max(I1,I2);
			*/
			dstW[x]=I1;
#ifdef DebugRGBCorrel
			if(F)
			{
				char sRVB[256];
				int R=srcR[POCT*x+2]; int V=srcR[POCT*x+1]; int B=srcR[POCT*x];
				if((R>70)||(V>70)||(B>70))
				{
					sprintf(sRVB,"%i\t%i\t%i\t%.1f\r\n",R,V,B,dst[x]);
					fwrite(sRVB,strlen(sRVB),1,F);
				}
			}
#endif
		}
		dstW+=P_SizeX(Pdst);
		srcR+=G_Pitch(E);
	}

	int B;
	D2D.SideEffect+=(B=((1+P_SizeX(Pdst)-G_SizeX(E))>>1));
	P_ExtendBorderVal(Pdst,B);

	G_CloseEcran(E);

#ifdef DebugRGBCorrel
	if(F) {fclose(F);F=0;}
#endif
#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_RGB24toFloat);
#endif
#endif
	return;
}

//convertit l'image RGB et image d'intensité laser, avec rotation à 90° de l'image
void RGB24toFloatAndRotate(SPG_PEAKDET2D& D2D, float* dst, BYTE* srcRGB24, int SizeX, int SizeY, int PitchSrc, int POCT)
{
//SizeX x SizeY sont les dimensions de l'image originale RGB
//le tableau destination *dst doit avoir les dimensions SizeY x SizeX
	CHECK(dst==0,"RGB24toFloatAndRotate",return);
	CHECK(srcRGB24==0,"RGB24toFloatAndRotate",return);
	CHECK((POCT!=3)&&(POCT!=4),"RGB24toFloatAndRotate",return);
	CHECK(PitchSrc<POCT*SizeX,"RGB24toFloatAndRotate",return);

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StartTimer(D2DT_RGB24toFloatAndRotate);
#endif
#endif

#ifdef DebugRGBCorrel
	FILE* F;
	F=fopen("LineDetect.txt","wb");
	if(F)
	{ //ecris la ligne d'entete
		char sRVB[256]; sprintf(sRVB,"R\tV\tB\tI\r\n"); fwrite(sRVB,strlen(sRVB),1,F);
	}
#endif

	if(PitchSrc==0) PitchSrc=POCT*SizeX;

	for(int y=0;y<SizeY;y++)
	{
		for(int x=0;x<SizeX;x++)
		{
			float I1=
				wB*srcRGB24[POCT*x]+
				wG*srcRGB24[POCT*x+1]+
				wR*srcRGB24[POCT*x+2];//cas rouge non saturé

			float I2=
				wGs*srcRGB24[POCT*x+1]+
				wR*srcRGB24[POCT*x+2];//cas rouge saturé

			dst[x*SizeY]=V_Max(I1,I2);//x,y dand l'image RGB donne y,x dans le profil résultat (rotation)
			//y,x signifie que x c'est le numero de la ligne et y le numero de la colonne
			//pour cela on utilise dst[x*SizeY] car une ligne de dst est de longueur SizeY
			//(car le tableau destination *dst doit avoir les dimensions SizeY x SizeX)
#ifdef DebugRGBCorrel
			if(F)
			{
				char sRVB[256];
				int R=srcRGB24[POCT*x+2]; int V=srcRGB24[POCT*x+1]; int B=srcRGB24[POCT*x];
				if((R>70)||(V>70)||(B>70))
				{
					sprintf(sRVB,"%i\t%i\t%i\t%.1f\r\n",R,V,B,dst[x]);
					fwrite(sRVB,strlen(sRVB),1,F);
				}
			}
#endif
		}
		dst++;//colonne suivante
		srcRGB24+=PitchSrc;
	}

#ifdef DebugRGBCorrel
	if(F) {fclose(F);F=0;}
#endif

#ifdef DebugTimer
#ifdef SPG_General_USETimer
	S_StopTimer(D2DT_RGB24toFloatAndRotate);
#endif
#endif

	return;
}

#undef wR
#undef wG
#undef wB
#undef wGs
#undef wBs

#endif
