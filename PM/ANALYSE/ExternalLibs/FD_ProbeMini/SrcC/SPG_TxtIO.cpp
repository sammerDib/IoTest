
#include "SPG_General.h"

#ifdef SPG_General_USETXT

#define SPG_TxtIO_RET "\r\n"

#include "SPG_Includes.h"

#include <stdio.h>
#include <string.h>
#include <ctype.h>


double* SPG_CONV Text_ReadDouble(char* FName, int& SizeX, int& SizeY, int OnlyGetSize)
{//SizeX=nb colonnes, SizeY=nb lignes
	SizeX=SizeY=0;
	double* Df=0; int FileLen=0;
	char* D=(char*)SPG_LoadFileAlloc(FName,FileLen,0);
	if(D==0) return 0;
	{
		int CurSizeX=0; int WaitForFirstNumber=1;//1:separateurs avant le premier nombre de la ligne 2:separateurs de ligne
		for(int i=0;i<FileLen;i++)
		{
			if(((D[i]=='\t')||(D[i]==',')||(D[i]==';')||(D[i]==' '))&&(WaitForFirstNumber==0))
			{//un separateur (qui n'est pas avant le premier nombre)
				CurSizeX++;
				SizeX=V_Max(SizeX,CurSizeX);
				WaitForFirstNumber=1;
			}
			else if(((D[i]=='\n')||(D[i]=='\r'))&&(WaitForFirstNumber!=2))
			{//un saut de ligne (qui n'est pas avant le premier nombre ni apres un caractere qui est deja un saut de ligne ex LF apres CR)
				SizeY++; 
				if (WaitForFirstNumber!=1) CurSizeX++;//n'incremente pas sizex si la ligne commence par un separateur de ligne
				SizeX=V_Max(SizeX,CurSizeX);
				CurSizeX=0;
				WaitForFirstNumber=2;//attente de la fin des caractères de saut de ligne ex LF apres CR
			}
			else if(isdigit(D[i])||(D[i]=='.')||(D[i]=='e')||(D[i]=='E')||(D[i]=='+')||(D[i]=='-'))
			{
				WaitForFirstNumber=0;
			}
		}
		if (CurSizeX!=0) SizeY++;
	}
	if((OnlyGetSize)||(SizeX==0)||(SizeY==0)) { SPG_MemFree(D); return 0; }
	{
		SPG_MemFastCheck();
		Df=SPG_TypeAlloc(SizeX*SizeY,double,"TxtFile Data"); if(Df==0) return 0;
		int CurSizeX=0; int CurSizeY=0; int WaitForFirstNumber=1;
		for(int i=0;i<FileLen;i++)
		{
			if(((D[i]=='\t')||(D[i]==',')||(D[i]==';')||(D[i]==' '))&&(WaitForFirstNumber==0))
			{//un separateur
				CurSizeX++;
				WaitForFirstNumber=1;
			}
			else if(((D[i]=='\n')||(D[i]=='\r'))&&(WaitForFirstNumber!=2))
			{
				CurSizeY++;
				CurSizeX=0;
				WaitForFirstNumber=2;
			}
			else if(isdigit(D[i])||(D[i]=='.')||(D[i]=='e')||(D[i]=='E')||(D[i]=='+')||(D[i]=='-'))
			{
				/*
				int istart=i; while ((i<FileLen)&& (isdigit(D[i])||(D[i]=='.')||(D[i]=='e')||(D[i]=='E')||(D[i]=='+')||(D[i]=='-')) ) {i++;}
				BYTE SavedEnd=D[i];
				D[i]=0;//pour ne pas scanner sur des km
				sscanf( D+istart, "%le", Df+CurSizeX+SizeX*CurSizeY );
				D[i]=SavedEnd;
				i--;
				*/
				char* Di=D+i;
				Df[CurSizeX+SizeX*CurSizeY]=SPG_ReadDouble(Di);
				i=Di-D-1;
				WaitForFirstNumber=0;
			}
		}
	}
	SPG_MemFree(D);
	return Df;
}

float* SPG_CONV Text_Read(const char* FName, int& SizeX, int& SizeY, int OnlyGetSize)
{//SizeX=nb colonnes, SizeY=nb lignes
	SizeX=SizeY=0;
	float* Df=0; int FileLen=0;
	char* D=(char*)SPG_LoadFileAlloc(FName,FileLen,0);
	if(D==0) return 0;
	{
		int CurSizeX=0; int WaitForFirstNumber=1;//1:separateurs avant le premier nombre de la ligne 2:separateurs de ligne
		for(int i=0;i<FileLen;i++)
		{
			if(((D[i]=='\t')||(D[i]==',')||(D[i]==';')||(D[i]==' '))&&(WaitForFirstNumber==0))
			{//un separateur (qui n'est pas avant le premier nombre)
				CurSizeX++;
				SizeX=V_Max(SizeX,CurSizeX);
				WaitForFirstNumber=1;
			}
			else if(((D[i]=='\n')||(D[i]=='\r'))&&(WaitForFirstNumber!=2))
			{//un saut de ligne (qui n'est pas avant le premier nombre ni apres un caractere qui est deja un saut de ligne ex LF apres CR)
				SizeY++; 
				if (WaitForFirstNumber!=1) CurSizeX++;//n'incremente pas sizex si la ligne commence par un separateur de ligne
				SizeX=V_Max(SizeX,CurSizeX);
				CurSizeX=0;
				WaitForFirstNumber=2;//attente de la fin des caractères de saut de ligne ex LF apres CR
			}
			else if(isdigit(D[i])||(D[i]=='.')||(D[i]=='e')||(D[i]=='E')||(D[i]=='+')||(D[i]=='-'))
			{
				WaitForFirstNumber=0;
			}
		}
		if (CurSizeX!=0) SizeY++;
	}
	if((OnlyGetSize)||(SizeX==0)||(SizeY==0)) { SPG_MemFree(D); return 0; }
	{
		SPG_MemFastCheck();
		Df=SPG_TypeAlloc(SizeX*SizeY,float,"TxtFile Data"); if(Df==0) return 0;
		int CurSizeX=0; int CurSizeY=0; int WaitForFirstNumber=1;
		for(int i=0;i<FileLen;i++)
		{
			if(((D[i]=='\t')||(D[i]==',')||(D[i]==';')||(D[i]==' '))&&(WaitForFirstNumber==0))
			{//un separateur
				CurSizeX++;
				WaitForFirstNumber=1;
			}
			else if(((D[i]=='\n')||(D[i]=='\r'))&&(WaitForFirstNumber!=2))
			{
				CurSizeY++;
				CurSizeX=0;
				WaitForFirstNumber=2;
			}
			else if(isdigit(D[i])||(D[i]=='.')||(D[i]=='e')||(D[i]=='E')||(D[i]=='+')||(D[i]=='-'))
			{
				/*
				int istart=i; while ((i<FileLen)&& (isdigit(D[i])||(D[i]=='.')||(D[i]=='e')||(D[i]=='E')||(D[i]=='+')||(D[i]=='-')) ) {i++;}
				BYTE SavedEnd=D[i];
				D[i]=0;//pour ne pas scanner sur des km
				sscanf( D+istart, "%f", Df+CurSizeX+SizeX*CurSizeY );
				D[i]=SavedEnd;
				i--;
				*/
				char* Di=D+i;
				Df[CurSizeX+SizeX*CurSizeY]=SPG_ReadFloat(Di);
				i=Di-D-1;// -1 à cause du i++ de la boucle for
				WaitForFirstNumber=0;
			}
		}
	}
	SPG_MemFree(D);
	return Df;
}

int SPG_CONV Text_Write(float * D, int SizeX,int SizeY, char*FName, int ChiffSignif, int Separateur)
{//SizeX=nb colonnes, SizeY=nb lignes
	//if(V_GetExtens(FName)==V_TXT)

		CHECK((D==0)||(SizeX<=0)||(SizeY<=0),"TxtWrite: Profil nul",return 0);
		//CHECK(P.D==0,"TxtWrite: Donnees nulles",return 0);

		FILE*F=fopen(FName,"wb");
		CHECKTWO(F==0,"Impossible d'ouvrir le fichier",FName,return 0);
/*
	if((WithHeader)&&(P.UnitX[0]!=0)&&(P.UnitY[0]!=0)&&(P.UnitZ[0]!=0)&&(P.XScale!=0)&&(P.YScale!=0))
	{
		char Msg[256];
		Msg[0]=0;
		strcat(Msg,"UniteX:");
		CF_GetString(Msg,P.XScale,6);
		strcat(Msg,P.UnitX);

		strcat(Msg,"UniteY:");
		CF_GetString(Msg,P.YScale,6);
		strcat(Msg,P.UnitY);

		strcat(Msg,"AxeZ:");
		strcat(Msg,P.UnitZ);

		strcat(Msg,"\n");
		fwrite(Msg,strlen(Msg),1,F);
	}
*/
		char FSep[2];
		FSep[1]=0;

		if  (Separateur==S_Virgule)
		FSep[0]=',';
		else if  (Separateur==S_PointVirgule)
		FSep[0]=';';
		else if  (Separateur==S_Espace)
		FSep[0]=' ';
		else //if (Separateur==S_Tabulation)
		FSep[0]='\t';

		int x,y;
		for(y=0;y<SizeY;y++)
		{
			for(x=0;x<SizeX-1;x++)
			{
				char Val[64];
				//sprintf(Val,FSep,Df[x+SizeX*y]);
				Val[0]=0;
				CF_GetString(Val,D[x+SizeX*y],ChiffSignif);
				strcat(Val,FSep);
				fwrite(Val,strlen(Val),1,F);
			}
				char Val[64];
				Val[0]=0;
				CF_GetString(Val,D[x+SizeX*y],ChiffSignif);
				strcat(Val,SPG_TxtIO_RET);
				fwrite(Val,strlen(Val),1,F);
		}

		fclose(F);
		return -1;
}

int SPG_CONV Text_WriteDouble(double * D, int SizeX,int SizeY, char*FName, int ChiffSignif, int Separateur)
{//SizeX=nb colonnes, SizeY=nb lignes
	//if(V_GetExtens(FName)==V_TXT)

		CHECK((D==0)||(SizeX<=0)||(SizeY<=0),"TxtWrite: Profil nul",return 0);
		//CHECK(P.D==0,"TxtWrite: Donnees nulles",return 0);

		FILE*F=fopen(FName,"wb");
		CHECKTWO(F==0,"Impossible d'ouvrir le fichier",FName,return 0);

		char FSep[2];
		FSep[1]=0;

		if  (Separateur==S_Virgule)
		FSep[0]=',';
		else if  (Separateur==S_PointVirgule)
		FSep[0]=';';
		else if  (Separateur==S_Espace)
		FSep[0]=' ';
		else //if (Separateur==S_Tabulation)
		FSep[0]='\t';

		int x,y;
		for(y=0;y<SizeY;y++)
		{
			for(x=0;x<SizeX-1;x++)
			{
				char Val[64];
				//sprintf(Val,FSep,Df[x+SizeX*y]);
				Val[0]=0;
				CF_GetString(Val,D[x+SizeX*y],ChiffSignif);
				strcat(Val,FSep);
				fwrite(Val,strlen(Val),1,F);
			}
				char Val[64];
				Val[0]=0;
				CF_GetString(Val,D[x+SizeX*y],ChiffSignif);
				strcat(Val,SPG_TxtIO_RET);
				fwrite(Val,strlen(Val),1,F);
		}

		fclose(F);
		return -1;
}

#endif


