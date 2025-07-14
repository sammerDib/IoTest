
#include "SPG_General.h"

#ifdef SPG_General_USESGRAPH

#ifdef SPG_General_PGLib
#include "PGLib\PGlib.h"
#pragma SPGMSG(__FILE__,__LINE__,"V_ConvertToGraph using PGLib")
#endif

#include "SPG_InhibitedFunctions.h"

#include "V_General.h"
#include "SPG_Graphics.h"
#include "SGRAPH.h"
/*
void V_MakeSG_Mobil(SG_Mobil& SG_Rep,V_REPERE& V_Rep)
{
	if (sizeof(V_REPERE)==sizeof(SG_Mobil))
		//SG_Rep=*((SG_Mobil*)&V_Rep);
	{
		SG_Rep.X=V_Rep.pos.x;
		SG_Rep.Y=V_Rep.pos.y;
		SG_Rep.Z=V_Rep.pos.z;
//l'axe x est l'axe longitudinal de la voiture
		SG_Rep.XDIR=V_Rep.axex.x;
		SG_Rep.YDIR=V_Rep.axex.y;
		SG_Rep.ZDIR=V_Rep.axex.z;
//l'axe y est l'axe lateral de la voiture
		SG_Rep.XDPX=-V_Rep.axey.x;
		SG_Rep.YDPX=-V_Rep.axey.y;
		SG_Rep.ZDPX=-V_Rep.axey.z;
//l'axe z est l'axe vertical de la voiture
		SG_Rep.XDPY=V_Rep.axez.x;
		SG_Rep.YDPY=V_Rep.axez.y;
		SG_Rep.ZDPY=V_Rep.axez.z;
	}
	else
		SPG_List("Erreur de types - Recompilation OBLIGATOIRE");
	return;
}
*/
/*
void SPG_CONV V_GenereBloc(SG_Bloc &BlocRef, SG_Bloc &BlocMobil, V_REPERE& V_Rep)
{
	int i;
	for(i=0;i<BlocRef.NombreP;i++)
	{
		V_Operate5(BlocMobil.MemPoints[i].P,
			=V_Rep.pos,
			+BlocRef.MemPoints[i].P.x*V_Rep.axex,
			+BlocRef.MemPoints[i].P.y*V_Rep.axey,
			+BlocRef.MemPoints[i].P.z*V_Rep.axez);

	}
	return;
}
*/
void SPG_CONV V_GenereBloc(const SG_FullBloc &BlocRef, SG_FullBloc &BlocMobil, const V_REPERE& V_Rep)
{
	{
	int NP=BlocRef.DB.NombreP;
	SG_PNT3D* MP=BlocMobil.DB.MemPoints;
	SG_PNT3D* RP=BlocRef.DB.MemPoints;
	for(int i=0;i<NP;i++)
	{
		V_Operate5(MP[i].P,
			=V_Rep.pos,
			+RP[i].P.x*V_Rep.axex,
			+RP[i].P.y*V_Rep.axey,
			+RP[i].P.z*V_Rep.axez);
	}
	}
#ifdef SGE_EMC
	{
	int NF=BlocRef.DB.NombreF;
	SG_FACE* MF=BlocMobil.DB.MemFaces;
	SG_FACE* RF=BlocRef.DB.MemFaces;
	for(int i=0;i<NF;i++)
	{
		V_Operate4(MF[i].Normale,
			=RF[i].Normale.x*V_Rep.axex,
			+RF[i].Normale.y*V_Rep.axey,
			+RF[i].Normale.z*V_Rep.axez);
	}
	}
#endif

	V_Operate3(BlocMobil.BRef,=BlocRef.BRef,+V_Rep.pos);

	return;
}

#endif



