

#include "SPG_General.h"

#ifdef SPG_General_USECarac

#include "SPG_Includes.h"

#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <ctype.h>

#define MaxPrintLines 512

int SPG_CONV C_LoadCaracLib(C_Lib& LB, G_Ecran& E, const char * WorkDir,const char * S)
{
	memset(&LB,0,sizeof(C_Lib));
	DbgCHECK(!SPG_GLOBAL_ETAT(OK),"C_LoadCaracLib: Call SPG_Initialise first");

	CHECK(E.Etat==0,"C_LoadCaracLib: Ecran invalide",return 0);
//, BYTE CharMin, BYTE CharMax)
	char FullName[MaxProgDir];
	if(WorkDir) 
		SPG_ConcatPath(FullName,Global.ProgDir,WorkDir);
	else
		strcpy(FullName,Global.ProgDir);
	SPG_ConcatPath(FullName,FullName,S);
	//LB.EIMG=(G_Ecran*)SPG_MemAlloc(sizeof(G_Ecran),"G_Ecran Caracteres");
	SPG_SetExtens(FullName,".bmp");
	IF_CD_G_CHECK(5,return 0);
	CHECKTWO(G_InitEcranFromFile(LB.EIMG,E.POCT,0,FullName)==0,"C_LoadCaracLib: Chargement echoue sur le fichier",FullName,C_CloseCaracLib(LB);return 0);
	SPG_SetMemName(LB.EIMG.MECR,"CaracLib");
	
//charmin et charmax devraient etre lus depuis un fichier
	/*
	LB.CharMin=CharMin;//' ';
	LB.CharMax=CharMax;//'_';

	LB.SizeX=LB.EIMG.SizeX/(1+LB.CharMax-LB.CharMin);
	LB.SizeY=LB.EIMG.SizeY;
	LB.SpaceY=LB.EIMG.SizeY+1;
	*/
	SPG_SetExtens(FullName,".txt");
	int CSizeX,CSizeY;
	float* CVal = Text_Read(FullName,CSizeX,CSizeY,0);
	CHECKTWO((CVal==0)||(CSizeX*CSizeY!=3),"C_LoadCaracLib: Fichier texte de config invalide",FullName,SPG_MemFree(CVal);C_CloseCaracLib(LB);return 0);

	LB.CharMin=(BYTE)V_Round(CVal[0]);
	LB.CharMax=(BYTE)V_Round(CVal[1]);
	LB.SizeX=LB.EIMG.SizeX/(1+LB.CharMax-LB.CharMin);
	LB.SizeY=LB.EIMG.SizeY;
	LB.SpaceY=LB.EIMG.SizeY+V_Round(CVal[2]);
	SPG_MemFree(CVal);

	G_Make24(LB.EIMG,(*LB.EIMG.MECR),LB.BackGroundColor.Coul); LB.BackGroundColor.A=G_NORMAL_ALPHA;

	LB.Etat=1;
	return -1;
}

void SPG_CONV C_CloseCaracLib(C_Lib& LB)
{
	if (LB.Etat) G_CloseEcran(LB.EIMG);
	LB.Etat=0;
	return;
}

void SPG_CONV C_Print(G_Ecran& E, int PosX,int PosY, char* Ligne, C_Lib& LB,int Alignement)
{
	if (LB.Etat==0) return;
	CHECK(G_Etat(E)==0,"C_Print",return);
	CHECK(E.MECR==0,"C_PrintUni: Ecran inaccessible",return);
	int MaxLen[MaxPrintLines];
	//memset(MaxLen,0,MaxPrintLines*sizeof(int));
	int CurLen=0;
	int LineCount=0;
	int l=strlen(Ligne);
	MaxLen[0]=0;
	int i;
	for(i=0;i<l;i++)
	{
		if(Ligne[i]=='\n')
		{
			LineCount++;
			if(LineCount>MaxPrintLines-2) 
			{
				LineCount--;
				l=i;
				i--;
			}
			CurLen=0;
			MaxLen[LineCount]=0;
		}
		else
		{
			CurLen++;
			MaxLen[LineCount]=V_Max(CurLen,MaxLen[LineCount]);
		}
	}
	if (CurLen) LineCount++;
	if (LineCount==0) return;
	
	int wPosX=PosX;
	int wPosY=PosY;
	
	if (Alignement&YDN)
	{
		wPosY-=LineCount*LB.SpaceY;
	}
	else if (Alignement&YCENTER)
	{
		wPosY-=LineCount*LB.SpaceY/2;
	}
	
	CurLen=0;
	LineCount=0;
	for(i=0;i<l;i++)
	{
		if(Ligne[i]=='\n')
		{
			LineCount++;
			CurLen=0;
		}
		else
		{
			int cPosY=wPosY+LineCount*LB.SpaceY;
			int cPosX=wPosX+CurLen*LB.SizeX;
			if (Alignement&XRIGHT)
			{
				cPosX-=MaxLen[LineCount]*LB.SizeX;
			}
			else if  (Alignement&XCENTER)
			{
				cPosX-=MaxLen[LineCount]*LB.SizeX/2;
			}
			
			//int G_BlitFromMem(G_Ecran& E,int X,int Y, BYTE*Button, int Pitch, int SizeX,int SizeY)
			BYTE LG=Ligne[i];
			if (V_InclusiveBound((BYTE)LG,LB.CharMin,LB.CharMax)==0) 
			{
				LG=(BYTE)toupper(LG);
			}
			if (V_InclusiveBound((BYTE)LG,LB.CharMin,LB.CharMax))
			{
				if(
					V_IsBound(cPosX,0,E.SizeX)&&
					V_IsBound(cPosY,0,E.SizeY)&&
					V_InclusiveBound(cPosX+LB.SizeX,0,E.SizeX)&&
					V_InclusiveBound(cPosY+LB.SizeY,0,E.SizeY)
					)
				{
					if (Alignement&FONT_TRANSP)
						G_BlitNEFromMem(E,cPosX,cPosY,PixEcrPTR(LB.EIMG,LB.SizeX*(LG-LB.CharMin),0),LB.EIMG.Pitch,LB.SizeX,LB.SizeY,LB.BackGroundColor);
					else
						G_BlitFromMem(E,cPosX,cPosY,PixEcrPTR(LB.EIMG,LB.SizeX*(LG-LB.CharMin),0),LB.EIMG.Pitch,LB.SizeX,LB.SizeY);
				}
			}
			CurLen++;
		}
		
	}
	
	return;
}


void SPG_CONV C_PrintUni(G_Ecran& E, int PosX,int PosY, char* Ligne, C_Lib& LB,int Alignement, DWORD Couleur)
{
	if (LB.Etat==0) return;
	CHECK(G_Etat(E)==0,"C_PrintUni",return);
	CHECK(E.MECR==0,"C_PrintUni: Ecran inaccessible",return);
	int MaxLen[MaxPrintLines];
	//memset(MaxLen,0,MaxPrintLines*sizeof(int));
	int CurLen=0;
	int LineCount=0;
	int l=strlen(Ligne);
	MaxLen[0]=0;
	int i;
	for(i=0;i<l;i++)
	{
		if(Ligne[i]=='\n')
		{
			LineCount++;
			CurLen=0;
			MaxLen[LineCount]=0;
		}
		else
		{
			CurLen++;
			MaxLen[LineCount]=V_Max(CurLen,MaxLen[LineCount]);
		}
	}
	if (CurLen) LineCount++;
	if (LineCount==0) return;
	
	int wPosX=PosX;
	int wPosY=PosY;
	
	if (Alignement&YDN)
	{
		wPosY-=LineCount*LB.SpaceY;
	}
	else if (Alignement&YCENTER)
	{
		wPosY-=LineCount*LB.SpaceY/2;
	}
	
	CurLen=0;
	LineCount=0;
	for(i=0;i<l;i++)
	{//for i
		if(Ligne[i]=='\n')
		{
			LineCount++;
			CurLen=0;
		}
		else
		{//for i if(Ligne)
			int cPosY=wPosY+LineCount*LB.SpaceY;
			int cPosX=wPosX+CurLen*LB.SizeX;
			if (Alignement&XRIGHT)
			{
				cPosX-=MaxLen[LineCount]*LB.SizeX;
			}
			else if  (Alignement&XCENTER)
			{
				cPosX-=MaxLen[LineCount]*LB.SizeX/2;
			}
			
			//int G_BlitFromMem(G_Ecran& E,int X,int Y, BYTE*Button, int Pitch, int SizeX,int SizeY)
			BYTE LG=Ligne[i];
			if (V_InclusiveBound((BYTE)LG,LB.CharMin,LB.CharMax)==0) 
			{
				LG=(BYTE)toupper(LG);
			}
			if (V_InclusiveBound((BYTE)LG,LB.CharMin,LB.CharMax))
			{//for i if(Ligne) if(IBound)
				if(
					V_IsBound(cPosX,0,E.SizeX)&&
					V_IsBound(cPosY,0,E.SizeY)&&
					V_InclusiveBound(cPosX+LB.SizeX,0,E.SizeX)&&
					V_InclusiveBound(cPosY+LB.SizeY,0,E.SizeY)
					)
				{//for i if(Ligne) if(IBound) if(Bound)
					//if (Alignement&FONT_TRANSP)
{//for i if(Ligne) if(IBound) if(Bound) if(Align)
	int X=cPosX;
	int Y=cPosY;
	BYTE*Button=PixEcrPTR(LB.EIMG,LB.SizeX*(LG-LB.CharMin),0);
	int Pitch=LB.EIMG.Pitch;
	int SizeX=LB.SizeX;
	int SizeY=LB.SizeY;
	
	

	
	int LLine=SizeX*E.POCT;
	if (E.POCT==4)
	{
		DWORD* BLine=(DWORD*)Button;
		DWORD* ELine=(DWORD*)PixEcrPTR(E,X,Y);
		int yt;
		for(yt=0;yt<SizeY;yt++)
		{
			int xt;
			for(xt=0;xt<SizeX;xt++)
			{
				if (BLine[xt]!=LB.BackGroundColor.Coul) ELine[xt]=Couleur;//BLine[xt];
			}
			(*(int*)&BLine)+=Pitch;
			(*(int*)&ELine)+=E.Pitch;
		}
	}
	else if (E.POCT==3)
	{
		DWORD CoulTransp=LB.BackGroundColor.Coul;
		BYTE* BLine=Button;
		BYTE* ELine=PixEcrPTR(E,X,Y);
		int yt;
		for(yt=0;yt<SizeY;yt++)
		{
			int xt;
			for(xt=0;xt<LLine;xt+=3)
			{
				if(memcmp(BLine+xt,&CoulTransp,3))
					*(PixCoul24*)(ELine+xt)=*(PixCoul24*)&Couleur;
			}
			(*(int*)&BLine)+=Pitch;
			(*(int*)&ELine)+=E.Pitch;
		}
	}
	else if (E.POCT==2)
	{
		WORD CoulTransp=G_Make16From24(LB.BackGroundColor);
		WORD CoulWrite=G_Make16From24(Couleur);
		WORD* BLine=(WORD*)Button;
		WORD* ELine=(WORD*)PixEcrPTR(E,X,Y);
		int yt;
		for(yt=0;yt<SizeY;yt++)
		{
			int xt;
			for(xt=0;xt<SizeX;xt++)
			{
				if (BLine[xt]!=CoulTransp) ELine[xt]=CoulWrite;
			}
			(*(int*)&BLine)+=Pitch;
			(*(int*)&ELine)+=E.Pitch;
		}
	}
	else if (E.POCT==1)
	{
		BYTE CoulTransp=G_Make8From24(LB.BackGroundColor);
		BYTE CoulWrite=G_Make8From24(Couleur);
		BYTE* BLine=Button;
		BYTE* ELine=PixEcrPTR(E,X,Y);
		int yt;
		for(yt=0;yt<SizeY;yt++)
		{
			int xt;
			for(xt=0;xt<SizeX;xt++)
			{
				if (BLine[xt]!=CoulTransp) ELine[xt]=CoulWrite;
			}
			(*(int*)&BLine)+=Pitch;
			(*(int*)&ELine)+=E.Pitch;
		}
	}
}//for i if(Ligne) if(IBound) if(Bound) if(Align)
				}//for i if(Ligne) if(IBound) if(Bound)
			}//for i if(Ligne) if(IBound)
			CurLen++;
		}//for i if(Ligne)
	}//for i
	return;
}

void SPG_CONV C_PrintWithBorder(G_Ecran& E, int PosX,int PosY, char* Ligne, C_Lib& CL,int Alignement, DWORD Couleur)
{
	C_PrintUni(E,
		PosX-1,
		PosY-1,
		Ligne,CL,
		Alignement,Couleur);
	C_PrintUni(E,
		PosX,
		PosY-1,
		Ligne,CL,
		Alignement,Couleur);
	C_PrintUni(E,
		PosX+1,
		PosY-1,
		Ligne,CL,
		Alignement,Couleur);
	C_PrintUni(E,
		PosX+1,
		PosY,
		Ligne,CL,
		Alignement,Couleur);
	C_PrintUni(E,
		PosX+1,
		PosY+1,
		Ligne,CL,
		Alignement,Couleur);
	C_PrintUni(E,
		PosX,
		PosY+1,
		Ligne,CL,
		Alignement,Couleur);
	C_PrintUni(E,
		PosX-1,
		PosY+1,
		Ligne,CL,
		Alignement,Couleur);
	C_PrintUni(E,
		PosX-1,
		PosY,
		Ligne,CL,
		Alignement,Couleur);
	C_Print(E,
		PosX,
		PosY,
		Ligne,CL,
		Alignement|FONT_TRANSP);
		
	/*
	C_PrintUni(SEGraph,
		B.DB.MemPoints[xsel+SizeX*ysel].XECR-3,
		B.DB.MemPoints[xsel+SizeX*ysel].YECR-4,
		Msg,CL,
		FONT_TRANSP|XRIGHT|YUP,0xffffffff);
	C_PrintUni(SEGraph,
		B.DB.MemPoints[xsel+SizeX*ysel].XECR-4,
		B.DB.MemPoints[xsel+SizeX*ysel].YECR-5,
		Msg,CL,
		FONT_TRANSP|XRIGHT|YUP,0xffffffff);
	C_PrintUni(SEGraph,
		B.DB.MemPoints[xsel+SizeX*ysel].XECR-4,
		B.DB.MemPoints[xsel+SizeX*ysel].YECR-3,
		Msg,CL,
		FONT_TRANSP|XRIGHT|YUP,0xffffffff);
	C_Print(SEGraph,
		B.DB.MemPoints[xsel+SizeX*ysel].XECR-4,
		B.DB.MemPoints[xsel+SizeX*ysel].YECR-4,
		Msg,CL,
		FONT_TRANSP|XRIGHT|YUP);
		*/
	return;
}

#endif
