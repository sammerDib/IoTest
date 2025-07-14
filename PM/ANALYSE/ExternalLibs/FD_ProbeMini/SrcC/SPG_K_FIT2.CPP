
#include "SPG_General.h"

#ifdef SPG_General_USEPARAFIT

#include "SPG_Includes.h"

#include <memory.h>

//ce fichier a des options de compilation non optimisées pour ne pas etre trop lent à compiler

#define K_FIT2_LEN 128

void SPG_CONV K_FIT2_Init(K_FIT2& KF2)
{
	KF2.Len=K_FIT2_LEN; //int NFit=2; 
	KF2.e=SPG_TypeAlloc( (1+KF2.Len) , K_FIT2_ELT* ,"K_FIT2_3_128_Init" );
	int S=0;//nombre total d'element
	int L;
	for(L=0;L<=KF2.Len;L++) { S+=L; }//nombre total d'element
	KF2.e[0]=SPG_TypeAlloc(S,K_FIT2_ELT,"K_FIT2_3_128_Init");
	S=0;
	for(L=0;L<=KF2.Len;L++)//pour chaque longueur de fit possible
	{
		KF2.e[L]=KF2.e[0]+S;//ajoute l'adresse de base du tableau à l'offset
		S+=L;
	}
//contient une directive de compilation conditionnelle qui désactive les longueurs supérieures à 64
#include "K_FIT2_3_128.cpp"
	return;
}

void SPG_CONV K_FIT2_Close(K_FIT2& KF)
{
	CHECK(KF.e==0,"K_FIT2_Close",return);
	SPG_MemFree(KF.e[0]);
	SPG_MemFree(KF.e);
	SPG_ZeroStruct(KF);
	return;
}

//les fonctions de calcul sont dans SPG_K_FIT_RT (Branche RealTimeSourceFiles)

#endif


/*
//import de fichiers de coefficients mathematica

#include "..\SrcC\SPG.h"
#include "..\SrcC\SPG_SysInc.h"

#include <stdio.h>


int SPG_CONV CallBack(DWORD hwnd, DWORD uMsg, DWORD wParam, DWORD lParam)
{
	return 0;
}

#define SKIP(p) {while((*p==' ')||(*p=='\r')||(*p=='\n')) p++;}
#define PASS(p,c,M) {SKIP(p);DbgCHECK(*p!=c,M);p++;SKIP(p);}

int WINAPI WinMain( HINSTANCE hInstance, 
				   // handle to current instance 
				   
				   HINSTANCE hPrevInstance, 
				   // handle to previous instance 
				   
				   LPSTR lpCmdLine, 
				   // pointer to command line 
				   
				   int nCmdShow 
				   // show state of window 
				   
				   )
{
	SPG_WinMainStart((int)hInstance,0,0,0,SPG_SM_UserFriendly,G_ECRAN_DIBSECT,"ProgPrincipal","SrcC",CallBack);

	int NFit=2;
	int Len=128;

#define NAMELEN 1024

	char Filename[NAMELEN];
	sprintf(Filename,"C:\\Sylvain\\Developpement C - Docs and ZIP\\Documentation\\K_FIT%i_%i_%i.txt",NFit,NFit+1,Len);

	if(SPG_GetLoadName(SPG_TXT,Filename,NAMELEN))
	{
		int TxtLen=0;
		BYTE* TxtMem=SPG_LoadFile(Filename,TxtLen,1);
		BYTE* TxtCur=TxtMem;
		sprintf(Filename,"C:\\Sylvain\\Developpement C\\SrcC\\K_FIT%i_%i_%i.cpp",NFit,NFit+1,Len);
		FILE* F=fopen(Filename,"wb+");
		PASS(TxtCur,'{',"Accolade ouvrante recherchée en fichier");
		for(int L=NFit+1;L<=Len;L++)//pour chaque longueur de fit possible
		{
			fprintf(F,"\r\n\r\n\r\n// ##### FIT DEGRE %i LONGUEUR %i #####\r\n",NFit,L);
			PASS(TxtCur,'{',"Accolade ouvrante recherchée coefficients de polynome");
			for(int n=0;n<=NFit;n++)//pour chaque coefficient (a,b,c,...) du polynome
			{
				PASS(TxtCur,'{',"Accolade ouvrante recherchée en liste poids echantillons");

				fprintf(F,"\r\n// ----- Calcul du coefficient %c . x^%i  - somme de %i termes -----\r\n",'a'+NFit-n,n,L);

				for(int i=0;i<L;i++)//pour chaque echantillon du fit de longueur L
				{
					for(int T=0;T<40;T++)
					{
						if((TxtCur[T]=='}')||(TxtCur[T]==','))
						{
							DbgCHECK(T==0,"Nombre attendu");
							if(T)
							{
								BYTE endOfNumber=0;
								V_SWAP(BYTE,endOfNumber,TxtCur[T]);
								char ProgLine[512];
								sprintf(ProgLine,"KF%i[%i].e[%i].a[%i]=%s;\r\n",NFit,L,  i,  n,TxtCur);
								//SPG_List(ProgLine);
								fwrite(ProgLine,strlen(ProgLine),1,F);
								V_SWAP(BYTE,endOfNumber,TxtCur[T]);
								TxtCur+=T;
								break;
							}
						}
					}

					if(i!=L-1)
					{
						PASS(TxtCur,',',"Séparateur recherchée en liste poids echantillons");
					}
				}
				PASS(TxtCur,'}',"Terminateur recherchée en liste poids echantillons");
				if(n!=NFit)
				{
					PASS(TxtCur,',',"Séparateur recherchée coefficients de polynome");
				}
			}
			PASS(TxtCur,'}',"Terminateur recherchée coefficients de polynome");
			if(L!=Len)
			{
				PASS(TxtCur,',',"Séparateur recherchée en fichier");
			}
		}
		PASS(TxtCur,'}',"Terminateur recherchée en fichier");
		SPG_MemFree(TxtMem);
		fclose(F);
	}
	SPG_WinMainClose();
	return 0;
}

//script mathematica
Clear[x, X, XZ, Y, FP, f, MXP, NFIT, NP, i, TC]
TC[NFIT_, NP_] := Module[{XZ, X, MXP, i},
    XZ = Table[1, {i, NP}];
    X = Table[(2*i - 1 - NP)/2, {i, NP}];
    MXP = Table[If[i > 1, X^(i - 1), XZ], {i, NFIT + 1}];
    Inverse[MXP.Transpose[MXP]].MXP]

Table[N[TC[4, i]], {i, 5, 128}] >> "C:\Sylvain\Mathematica\K_FIT4.txt"
(* avant utilisation faire 'remplacer tout' *^ par e *)

*/
