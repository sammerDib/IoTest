
#include "SPG_General.h"

#ifdef SPG_General_USECaracF

#include "SPG_MinIncludes.h"

#include <stdio.h>
#include <string.h>
#define _CRTDBG_MAP_ALLOC  
#include <stdlib.h>  
#include <crtdbg.h>  
#ifdef SPG_General_PGLib
#include "PGLib\PGlib.h"
#pragma SPGMSG(__FILE__,__LINE__,"CaracFormatter using PGLib")
#else
//#pragma message("CaracFormatter using SGRAPH")
#endif

bool SPG_CONV CF_IsNumeric(char *s)
{
	CHECK(s==0,"CF_IsNumeric",return 0);

	if(*s==0) return false;
	if((*s=='+')||(*s=='-')) s++;

	int e=0; int p=0;
	for(;*s!=0;s++)
	{
		if(*s=='.') {p++; if(p>1) return false;}
		else if((*s=='e')||(*s=='E')) {e++; if(e>1) return false;}
		else if(!V_InclusiveBound(*s,'0','9')) return false;
	}
	return true;
}

//concatene a R le nombre N
void SPG_CONV CF_GetString(char * R, cfdbl N,int ChSignif)
{

	ChSignif=V_Sature(ChSignif,1,24);

	R+=strlen(R);
	if (N==0) 
	{
		strcpy(R,"0");
		return;
	}
	if (N<0) 
	{
		N=-N;
		(*R)='-';
		R++;
	}
	int NS=V_Floor(log((cfdbl)N)/log(10.0));
	cfdbl NN=(cfdbl)(N*pow(10.0,-NS-1)+0.5*pow(0.1,ChSignif));
	
	if (NN>=1) 
	{
		NN=0.1;
		NS++;
	}
	
	/*
	*R='0';
	R++;
	*R='.';
	R++;
	*/
	int PointPos=1;
	if ((NS<ChSignif)&&(NS>=-1))
	{
		PointPos=1+NS;
		NS=0;
	}

	{
	int NSN=0;
	if(NS<0) NSN++;
	if(abs(NS)>9) NSN++;
	ChSignif=V_Max(1,ChSignif-NSN);
	}

	int i;
	for(i=0;i<ChSignif;i++)
	{
		if(i==PointPos) 
		{
			if(i==0)
			{
			*R='0';
			R++;
			}
			*R='.';
			R++;
		}
		*R=CF_ChXtract(&NN);
		R++;
	}
	if (NS) 
		sprintf(R,"E%d",NS); 
	else 
		*R=0;

	return;
}

void SPG_CONV CF_GetStringZ(char * R, cfdbl N,int ChSignif)
{
	R[0]=0;
	CF_GetString(R,N,ChSignif);
}


char SPG_FASTCONV CF_ChXtract(cfdbl * Num)
{
	*Num*=10;
	char N=(char)V_Sature(V_Floor(*Num),0,9);
	*Num-=N;
	return N+'0';
}

//recupere un nombre en virgule fixe
//juste compile, jamais relu, jamais essaye
//"ca ne marchera jamais"
void SPG_CONV CF_GetVFixString(char * R, cfdbl N, int ChAv, int ChAp)
{
	if (N<0)
	{
		N=-N;
		*R='-';
		R++;
	}

	cfdbl ChMax=(cfdbl)powfInt(10,ChAv-1);

	int WriteZeros=0;
	int i;
	for (i=ChAv-1;i>-ChAp;i--)
	{
		if (i==-1)
		{
			*R='.';
			R++;
		}

		char P=V_Floor(N/ChMax);
		N-=P*ChMax;
		if (P) WriteZeros=1;
		if (WriteZeros)
		{
			*R='0'+P;
			R++;
		}
		ChMax/=10;
	}
	*R=0;
	return;
}

char* SPG_CONV CF_StrFind(char * C, char * SearchFor, int MaxLen)
{
	intptr_t l=strlen(SearchFor);
	for(char*i=C;i<C+MaxLen-l;i++)
	{
		if(*i==*SearchFor)
		{
			if(strncmp(i,SearchFor,l)==0) return i;
		}
	}
	return 0;
}

int SPG_CONV CF_StrLineLen(char * C, int MaxLen)
{
	for(char*i=C;i<C+MaxLen;i++)
	{
		if((*i=='\r')||(*i=='\n')) return i-C;
	}//||(*i=='\n')
	return 0;
}

int SPG_CONV CF_LineCount(char* Chaine)
{
	intptr_t lmax=strlen(Chaine);
	int LCount=1;
	for(int i=0;i<lmax-1;i++)
	{
		if (Chaine[i]=='\n') LCount++;
	}
	return LCount;
}

int SPG_CONV SPG_ReadInt(char* &c)
{
	int Somme=0;
	bool NegSigne=0;
	while(*c==' ') c++; 
	if(*c=='-') {c++;NegSigne=true;}
	else if(*c=='+') {c++;}

	for(;;c++)
	{
		if(V_InclusiveBound(*c,'0','9')) Somme=Somme*10+(*c-'0');
		else break;
	}
	return NegSigne?-Somme:Somme;
}

float SPG_CONV SPG_fPowInt(float f, int N)
{
	if(N==0) return 1;
	else if(N<0) return 1/SPG_fPowInt(f,-N);
	else
	{
		float P=1;
		do { if(N&1) P*=f; f*=f; } while(N>>=1);
		return P;
	}
}

float SPG_CONV SPG_ReadFloat(char* &c)
{
	int Somme=0; //float ~7 décimales, int Somme accepte jusqu'à 9, résultat invalide au delà
	int E=0;
	int E0=-1;
	int Signe=1;
	while(*c==' ') c++; 
	if(*c=='-') {c++;Signe=-1;}
	else if(*c=='+') {c++;}

	for(;;c++)
	{
		if(V_InclusiveBound(*c,'0','9')) { if(E<8) { Somme=Somme*10+(*c-'0'); E++; } }
		else if(*c=='.') { E0=E; }
		else if((*c=='e')||(*c=='E')) { if(E0==-1) E0=E; c++; E-=SPG_ReadInt(c); break; }
		else break;
	}
	if(E0==-1) E0=E;
	return Signe*Somme*SPG_fPowInt(10,E0-E);
}

double SPG_CONV SPG_dPowInt(double f, int N)
{
	if(N==0) return 1;
	else if(N<0) return 1/SPG_dPowInt(f,-N);
	else
	{
		double P=1;
		do { if(N&1) P*=f; f*=f; } while(N>>=1);
		return P;
	}
}

double SPG_CONV SPG_ReadDouble(char* &c)
{
	__int64 Somme=0; //double ~16 décimales, __int64 Somme accepte jusqu'à 18, résultat invalide au delà
	int E=0;
	int E0=-1;
	int Signe=1;
	while(*c==' ') c++; 
	if(*c=='-') {c++;Signe=-1;}
	else if(*c=='+') {c++;}

	for(;;c++)
	{
		if(V_InclusiveBound(*c,'0','9')) { if(E<17) { Somme=Somme*10+(*c-'0'); E++; } }
		else if(*c=='.') { E0=E; }
		else if((*c=='e')||(*c=='E')) { if(E0==-1) E0=E; c++; E-=SPG_ReadInt(c); break; }
		else break;
	}
	if(E0==-1) E0=E;
	return Signe*Somme*SPG_dPowInt(10,E0-E);
}

//obsolete
int SPG_CONV CF_ReadInt(char* Chaine, int StartPos, int StopPos)
{
	CHECK(Chaine==0,"CF_ReadInt",return 0);
	CHECK(StopPos<StartPos,"CF_ReadInt",return 0);
	char s=Chaine[StopPos+1];
	Chaine[StopPos+1]=0;
	int V=atoi(Chaine+StartPos);
	Chaine[StopPos+1]=s;
	return V;
}

float SPG_CONV CF_ReadFloat(char* Chaine, int StartPos, int StopPos)
{
	CHECK(Chaine==0,"CF_ReadFloat",return 0);
	CHECK(StopPos<StartPos,"CF_ReadFloat",return 0);
	char s=Chaine[StopPos+1];
	Chaine[StopPos+1]=0;
	float V=atof(Chaine+StartPos);
	Chaine[StopPos+1]=s;
	return V;
}

double SPG_CONV CF_ReadDouble(char* Chaine, int StartPos, int StopPos)
{
	CHECK(Chaine==0,"CF_ReadDouble",return 0);
	CHECK(StopPos<StartPos,"CF_ReadDouble",return 0);
	char s=Chaine[StopPos+1];
	Chaine[StopPos+1]=0;
	double V=atof(Chaine+StartPos);
	Chaine[StopPos+1]=s;
	return V;
}


#endif

