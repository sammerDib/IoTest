

#include "SPG_General.h"

#ifdef SPG_General_USESUR
#ifdef SPG_General_USEProfil

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <stdio.h>
#include <string.h>

#include "DigitalSurf.h"


int SPG_CONV P_ReadSUR(Profil& P, char* FName)
{
	memset(&P,0,sizeof(Profil));

	FILE*F=fopen(FName,"rb");
	CHECKTWO(F==0,"P_ReadSUR: Ne peut ouvrir le fichier",FName,return 0);

	SURF Surf;
	fread(&Surf,sizeof(Surf),1,F);

	P_Create(P,Surf.nb_pt_ligne,Surf.nb_ligne,
		Surf.pas_x,Surf.pas_y,
		Surf.nom_axe_x,Surf.nom_axe_y,Surf.nom_axe_z,0);

	//float*Result=SPG_TypeAlloc(SizeX*SizeY,float,"HDF_Read data");

	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			SHORT Temp16;
			fread(&Temp16,2,1,F);
			P_Data(P)[x+P_SizeX(P)*y]=Surf.pas_z*(float)Temp16;
		}
	}

	fclose(F);
	return -1;
}

int SPG_CONV P_WriteSUR(Profil& P, char* FName)
{
	FILE*F=fopen(FName,"wb");
	CHECKTWO(F==0,"P_WriteSUR: Ne peut ouvrir le fichier",FName,return 0);
	CHECK(P_Etat(P)==0,"SUR_Write: Profil nul",return 0);
	CHECK(P_Data(P)==0,"SUR_Write: Profil vide",return 0);

	float fMin,fMax;
	P_FindMinMax(P,fMin,fMax);

	CHECK(fMin==fMax,"SUR_Write: Profil plat non exporte",return 0);

	SPG_StackAllocZ(SURF,Surf);

	strcpy(Surf.code,"DIGITAL SURF");		// 1
	Surf.format = 0;		// 2
	Surf.nb_obj = 1;		// 3
	Surf.nb_ver = 1;		//4
	Surf.type_etud = 2;	//5 
	Surf.nb_bit_pt = 16;	//12
	Surf.pt_min= -32768;		//13
	Surf.pt_max = +32767;		//14
	Surf.nb_pt_ligne = P_SizeX(P) ;//15
	Surf.nb_ligne = P_SizeY(P);	//16
	Surf.nb_total_pt = P_SizeX(P)*P_SizeY(P);	//17

	Surf.pas_x = P_XScale(P);
	Surf.pas_y = P_YScale(P);
	Surf.pas_z = (fMax-fMin)/65536.0f;

	strcpy(Surf.nom_axe_x,P_UnitX(P));	//21
	strcpy(Surf.nom_axe_y,P_UnitY(P));	//21
	strcpy(Surf.nom_axe_z,P_UnitZ(P));	//21
	strcpy(Surf.unite_lg_axe_x,P_UnitX(P));	//21
	strcpy(Surf.unite_lg_axe_y,P_UnitY(P));	//21
	strcpy(Surf.unite_lg_axe_z,P_UnitZ(P));	//21
	strcpy(Surf.unite_pas_x,P_UnitX(P));//27
	strcpy(Surf.unite_pas_y,P_UnitY(P));//27
	strcpy(Surf.unite_pas_z,P_UnitZ(P));//27

	Surf.rap_unite_x=1;		//30
	Surf.rap_unite_y=1;		//31
	Surf.rap_unite_z=1;		//32

	SPG_StackCheck(Surf);

	fwrite(&Surf, sizeof(SURF), 1, F);

	SPG_PtrAlloc(ShortTemp,P_SizeX(P),SHORT,"Tableau temporaire SUR_Write");

	float fMid=0.5f*(fMax+fMin);
	for(int y=0;y<P_SizeY(P);y++)
	{
		SHORT*SHT=ShortTemp;
		for(int x=0;x<P_SizeX(P);x++)
		{
			if((P_Msk(P)==0)||(P_ElementMsk(P,x,y)==0))
				*SHT=(SHORT)V_Round(((P_Element(P,x,y)-fMid)*65535/(fMax-fMin)));
			else
				*SHT=-32768;
			SHT++;
		}
		fwrite(ShortTemp,P_SizeX(P)*sizeof(SHORT),1,F);
	}

	SPG_MemFree(ShortTemp);

	fclose(F);
	return -1;
}

void SPG_CONV SUR_BatchConvertToRF(char * WorkDir)
{
	Profil P;

	char FName[1024];
	SPG_ConcatPath(FName,WorkDir,"*.sur");

	
	WIN32_FIND_DATA FFD;

	HANDLE H=FindFirstFile(FName,&FFD);
	if(H==INVALID_HANDLE_VALUE) return;

	do
	{
		char CWName[1024];
		SPG_ConcatPath(CWName,WorkDir,FFD.cFileName);
		if (P_ReadSUR(P,CWName)) 
		{
			SPG_SetExtens(CWName,".rf");
			//Cut_Write(C,CWName,S_RetourChariot,0);
			P_WriteRF(P,CWName,1);
			P_Close(P);
		}
	} while(FindNextFile(H,&FFD));

	FindClose(H);

	return;
}

#endif
#endif

