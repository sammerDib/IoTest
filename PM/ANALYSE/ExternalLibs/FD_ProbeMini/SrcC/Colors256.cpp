
#include "SPG_General.h"

#ifdef SPG_General_USEColors256

#include "SPG_Includes.h"
#include "SPG_SysInc.h"

#include <stdio.h>
#ifdef DebugFloat
#include <float.h>
#endif
#include <string.h>

#ifdef SPG_General_USESGRAPH
#define C256_NORMAL_ALPHA SG_NORMAL_ALPHA
#else
#define C256_NORMAL_ALPHA 0
#endif

#define LoadFilterTXT "Texte\0*.txt\0"
#define SaveFilterTXT LoadFilterTXT
#define C256_TXT SPG_TXT
#define C256_TXTlfIndex 1
#define C256_TXTsfIndex 1

#define LoadFilterPAL "JASC Palette\0*.pal\0"
#define SaveFilterPAL LoadFilterPAL
#define C256_PAL 2
#define C256_PALlfIndex 2
#define C256_PALsfIndex 2

//definit les types de fichiers possibles
int SPG_CONV C256_GetExtens(char *FullName)
{
	int l;
	l=strlen(FullName);
	if (l==0) return 0;

	int i;
	for(i=l;i>0;i--)
	{
		if(FullName[i]=='.') break;
	}
	if ((i==0)||(i==l)||(i==(l-1)))
	{
		return 0;
	}
	if (_stricmp(FullName+i,".txt")==0) return C256_TXT;
	if (_stricmp(FullName+i,".pal")==0) return C256_PAL;
	return 0;
}


void SPG_CONV CN_Init(PixCoul* BC, int N)
{
	for(int i=0;i<N;i++)
	{
		float T=(float)(3.0*i/(float)(N-1));
		if (T<0)
		{
BC[i].B = 64;
BC[i].V = 64;
BC[i].R = 64;
BC[i].A = C256_NORMAL_ALPHA;
		}
		else if (T<1)
		{
BC[i].B = V_FloatToByte((1 - T) * 64);
BC[i].V = V_FloatToByte(64 + (T * 128));
BC[i].R = BC[i].B;
BC[i].A = C256_NORMAL_ALPHA;
		}
		else if (T<2)
		{
BC[i].B = 0;
BC[i].V = V_FloatToByte(192 + 63 * (T - 1));
BC[i].R = V_FloatToByte((T - 1) * 255);
BC[i].A = C256_NORMAL_ALPHA;
		}
		else if (T<3)
		{
BC[i].B = 0;
BC[i].V = V_FloatToByte((3 - T) * 255);
BC[i].R = 255;
BC[i].A = C256_NORMAL_ALPHA;
		}
		else
		{
BC[i].B = 0;
BC[i].V = 0;
BC[i].R = 255;
BC[i].A = C256_NORMAL_ALPHA;
		}
	}
	return;
}

int SPG_CONV CNE_Init(PixCoul* BC, int N)
{					//00RRGGBB
	DWORD DWC[]={	0x00404040,//gris
					0x00404080,//gris bleu
					0x00004080,//bleu sombre
					0x00008080,//cyan
					0x0000C040,//vert
					0x0000FF00,//vert pur
					0x0080FF00,//vert jaune
					0x00C0FF00,//
					0x00FFFF00,//jaune
					0x00FFC000,//
					0x00FF8000,//
					0x00FF4000,//
					0x00FF0000,//rouge
					0x00FF0040,//
					0x00C00080,//rouge violet
					0x008000C0,//bleu violet
					0x004040FF,//bleu
					0x008080FF,//bleu clair
					0x00C0C0FF,
					0x00FFFFFF};//blanc

	PixCoul* PCsrc=(PixCoul*)DWC;
	int Nsrc=sizeof(DWC)/sizeof(DWORD);
	int K=(N-1)/(Nsrc-1); //N-element terminal sur nbre d'intervalles

	int idest=0;
	int isrc=0;
	{for(;isrc<Nsrc-1;isrc++)
	{
		{for(int it=0;it<K;it++)
		{
			BC[idest].R=PCsrc[isrc].R+(((PCsrc[isrc+1].R-PCsrc[isrc].R)*it)/K);
			BC[idest].V=PCsrc[isrc].V+(((PCsrc[isrc+1].V-PCsrc[isrc].V)*it)/K);
			BC[idest].B=PCsrc[isrc].B+(((PCsrc[isrc+1].B-PCsrc[isrc].B)*it)/K);
			idest++;
		}}
	}}

	BC[idest].R=PCsrc[isrc].R;//element terminal
	BC[idest].V=PCsrc[isrc].V;
	BC[idest].B=PCsrc[isrc].B;
	idest++;

	return idest;
}

//fait un degrade a partir de 3 int
//calcule des effets de lumiere sur une
//texture 2D: axe X -> degrade en Z
//texture 2D: axe Y -> lumiere
//BC=pointeur de table de couleurs preexistant, 0 sinon
PixCoul* SPG_CONV C256_Init(PixCoul* BC, int Gradient,int MA, int MB)
{
	int Rouge,Vert,Bleu,IncrR,IncrV,IncrB,MFact;
	int i;
	
	if (BC==0) BC=SPG_TypeAlloc(256,PixCoul,"Table de couleurs");
	
	CHECK(BC==0,"C256_Init: Allocation echouee",return 0);
	
	if (Gradient==0)
	{//my standard
		CN_Init(BC,256);
	}
	else
	{
		
		Rouge=0;
		Vert=0;
		Bleu=0;
		MFact=V_Max(Gradient,1);
		
		IncrR=0;
		IncrV=0;
		IncrB=0;
		
		while (((IncrR|IncrV|IncrB)==0)&&(MA!=0))
		{
			IncrR=MFact*(MA&3);
			MA>>=2;
			IncrV=MFact*(MA&3);
			MA>>=2;
			IncrB=MFact*(MA&3);
			MA>>=2;
		}
		
		if (MA&1) {Rouge=0xff;IncrR=-IncrR;}
		MA>>=1;
		if (MA&1) {Vert=0xff; IncrV=-IncrV;}
		MA>>=1;
		if (MA&1) {Bleu=0xff; IncrB=-IncrB;}
		MA>>=1;
		
		
		for (i=0;i<256;i++)
		{
			if (Rouge>255) 
			{
				IncrR=-MFact*(MB&3);
				MB>>=2;
				Rouge=255;
			}
			if (Rouge<0) 
			{
				IncrR=MFact*(MB&3);
				MB>>=2;
				Rouge=0;
			}
			if (Vert>255) 
			{
				IncrV=-MFact*(MB&3);
				MB>>=2;
				Vert=255;
			}
			if (Vert<0) 
			{
				IncrV=MFact*(MB&3);
				MB>>=2;
				Vert=0;
			}
			if (Bleu>255) 
			{
				IncrB=-MFact*(MB&3);
				MB>>=2;
				Bleu=255;
			}
			if (Bleu<0) 
			{
				IncrB=MFact*(MB&3);
				MB>>=2;
				Bleu=0;
			}
			while (((IncrR|IncrV|IncrB)==0)&&(MA!=0))
			{
				IncrR=MFact*(MA&3);
				MA>>=2;
				IncrV=MFact*(MA&3);
				MA>>=2;
				IncrB=MFact*(MA&3);
				MA>>=2;
			}
			
			BC[i].B=(BYTE)Bleu;
			BC[i].V=(BYTE)Vert;
			BC[i].R=(BYTE)Rouge;
			BC[i].A=C256_NORMAL_ALPHA;
			Rouge+=IncrR;
			Vert+=IncrV;
			Bleu+=IncrB;
		}
	}
	return BC;
}

void SPG_CONV C256_InterpoleT(PixCoul* T, int Istart, int Istop, int Rstart, int Rstop, int Vstart, int Vstop, int Bstart, int Bstop)
{
	CHECK(T==0,"C256_InterpoleT: Tableau nul",return);
	int Delta=1+Istop-Istart;
	for(int i=V_Max(Istart,0);i<=V_Min(Istop,255);i++)
	{
		T[i].R=(BYTE)(Rstart+((Rstop-Rstart)*(i-Istart))/Delta);
		T[i].V=(BYTE)(Vstart+((Vstop-Vstart)*(i-Istart))/Delta);
		T[i].B=(BYTE)(Bstart+((Bstop-Bstart)*(i-Istart))/Delta);
		T[i].A=C256_NORMAL_ALPHA;
	}
	return;
}

#ifdef SPG_General_USEFiles
#ifdef SPG_General_USETXT
//BC=pointeur de table de couleurs preexistant, 0 sinon
PixCoul* SPG_CONV C256_Load(PixCoul* BC,char* SuggestedName)
{
	char ColorFile[MaxProgDir];
	strcpy(ColorFile,SuggestedName);
	if(SPG_GetLoadName(SPG_TXT,ColorFile,MaxProgDir))
	{
		return C256_LoadFromFile(BC,ColorFile);
	}
	else
	{
		return BC;
	}
}
#endif
#endif

PixCoul* SPG_CONV C256_LoadFromFile(PixCoul* BC, char* ColorFile)
{
	bool PreviouslyFilled=true;
	if (BC==0) 
	{
		PreviouslyFilled=false;
		BC=SPG_TypeAlloc(256,PixCoul,"Table de couleurs");
	}
		
	CHECK(BC==0,"C256_Load: Allocation echouee",return BC);

	int SizeX,SizeY;
	float* Table=Text_Read(ColorFile,SizeX,SizeY,0);
	CHECK(Table==0,"C256_Load: Text_Read: Chargement echoue",SPG_MemFree(Table);if (!PreviouslyFilled) SPG_MemFree(BC);return BC);
	if((SizeX==3)&&(SizeY==256))
	{//RGB en colonnes
		for(int i=0;i<256;i++)
		{
			BC[i].B = V_FloatToByte(Table[3*i+2]);
			BC[i].V = V_FloatToByte(Table[3*i+1]);
			BC[i].R = V_FloatToByte(Table[3*i]);
			BC[i].A = C256_NORMAL_ALPHA;
		}
	}
	else if ((SizeX==256)&&(SizeY==3))
	{//RGB en lignes
		for(int i=0;i<256;i++)
		{
			BC[i].B = (BYTE)V_Sature(V_FloatToInt(Table[i]),0,255);
			BC[i].V = (BYTE)V_Sature(V_FloatToInt(Table[i+256]),0,255);
			BC[i].R = (BYTE)V_Sature(V_FloatToInt(Table[i+512]),0,255);
			BC[i].A = C256_NORMAL_ALPHA;
		}
	}
	else if (SizeX==4)
	{//IRBG 0<i<255 en colonnes
		int LastIndex=0;
		int LastR=0;
		int LastV=0;
		int LastB=0;
		for(int i=0;i<SizeY;i++)
		{
			int Index=V_Sature(V_FloatToInt(Table[4*i]),0,255);
			int R=V_Sature(V_FloatToInt(Table[4*i+1]),0,255);
			int V=V_Sature(V_FloatToInt(Table[4*i+2]),0,255);
			int B=V_Sature(V_FloatToInt(Table[4*i+3]),0,255);
			C256_InterpoleT(BC,LastIndex,Index,LastR,R,LastV,V,LastB,B);
			LastR=R;
			LastV=V;
			LastB=B;
			LastIndex=Index;
		}
	}
	else if (SizeY==4)
	{//IRBG 0<i<255 en lignes
		int LastIndex=0;
		int LastR=0;
		int LastV=0;
		int LastB=0;
		for(int i=0;i<SizeX;i++)
		{
			int Index=V_Sature(V_FloatToInt(Table[i]),0,255);
			int R=V_Sature(V_FloatToInt(Table[i+SizeX]),0,255);
			int V=V_Sature(V_FloatToInt(Table[i+2*SizeX]),0,255);
			int B=V_Sature(V_FloatToInt(Table[i+3*SizeX]),0,255);
			C256_InterpoleT(BC,LastIndex,Index,LastR,R,LastV,V,LastB,B);
			LastR=R;
			LastV=V;
			LastB=B;
			LastIndex=Index;
		}
	}
	else
	{
#ifdef DebugList
		SPG_List("C256_Load: Formats :\nR V B\nR V B\n...\n\nR R R ...\nV V V ...\nB B B ...\n\nI R V B\nI R V B\n...\n\nI I I ...\nR R R ...\nV V V ...\nB B B ...\n\n");
#endif
		if (!PreviouslyFilled)
		{
			SPG_MemFree(BC);
		}
	}
	SPG_MemFree(Table);
	return BC;
}

void SPG_CONV C256_Close(PixCoul*BC)
{
	DbgCHECK(BC==0,"Close256Color: Table de couleurs nulle");
	if (BC)
		SPG_MemFree(BC);
	BC=0;
	return;
}

#ifdef SPG_General_USEFilesWindows
int SPG_CONV C256_Save(PixCoul* BC, char* SuggestedName)
{
	SPG_StackAllocZ(OPENFILENAME,OFN);

	char ResultFile[MaxProgDir];
	strcpy(ResultFile,SuggestedName);

	OFN.lStructSize=sizeof(OFN);
#ifdef SPG_General_USEGlobal
	OFN.hwndOwner=(HWND)Global.hWndWin;
	OFN.hInstance=(HINSTANCE)Global.hInstance;
#endif
	OFN.lpstrFilter=SaveFilterTXT SaveFilterPAL"\0";
	OFN.lpstrFile=ResultFile;
	OFN.nMaxFile=MaxProgDir;
	OFN.lpstrFileTitle=0;
	OFN.nMaxFileTitle=0;
#ifdef SPG_General_USEGlobal
	OFN.lpstrInitialDir=Global.CurDir;
#endif
	OFN.lpstrTitle= SPG_COMPANYNAME" - Enregistrer une table de couleurs";
	OFN.Flags=OFN_OVERWRITEPROMPT|OFN_FILEMUSTEXIST|OFN_HIDEREADONLY|OFN_PATHMUSTEXIST;
	OFN.nFileOffset=0;//retourne la position du nom hors chemin
	OFN.nFileExtension=0;//idem pour l'extension
	OFN.lpstrDefExt=0;
	OFN.lCustData=0;
	OFN.lpfnHook=0;
	OFN.lpTemplateName=0;

	DoEvents(SPG_DOEV_MIN);
	if (GetSaveFileName(&OFN))
	{
		SPG_SetCurDirFrom(ResultFile);
		SPG_WaitMouseRelease();
		int E=C256_GetExtens(ResultFile);
		if ((E!=C256_TXT)
			&&(E!=C256_PAL)
			)
		{
			//attention ce code doit etre en toute rigeur modifie
			//lorsqu'on change les combinaisons de types de fichiers
			if (OFN.nFilterIndex==C256_TXTsfIndex) SPG_SetExtens(ResultFile,".txt");
			if (OFN.nFilterIndex==C256_PALsfIndex) SPG_SetExtens(ResultFile,".pal");
		}
		C256_SaveToFile(BC,ResultFile);
		SPG_StackCheck(OFN);
		return -1;
	}
	else
	{
		SPG_StackCheck(OFN);
		SPG_WaitMouseRelease();
		return 0;
	}
}
#endif

void SPG_CONV C256_SaveToFile(PixCoul* BC, char* PName)
{
	if (C256_GetExtens(PName)==C256_TXT)
	{
		C256_WriteTXT(BC,PName);
	}
	else if (C256_GetExtens(PName)==C256_PAL)
	{
		C256_WritePAL(BC,PName);
	}
	else
	{
		char Msg[1024];
		sprintf(Msg,"Le fichier %s\nn'a pas une extension connue.\nSauvegardez a nouveau.",PName);
#ifdef SPG_General_USEGlobal
		MessageBox((HWND)Global.hWndWin,Msg,"Enregistrement d'une table de couleurs",0);
#else
		MessageBox(0,Msg,"Enregistrement d'une table de couleurs",0);
#endif
	}
	return;
}

void SPG_CONV C256_WritePAL(PixCoul* BC, char* PName)
{
	CHECK(BC==0,"C256_WritePAL: Tableau vide",return);
	//SPG_SetExtens(PName,".pal");
	FILE*F=fopen(PName,"wb");
	CHECKTWO(F==0,"Impossible d'ouvrir le fichier",PName,return);
	char Head[]="JASC-PAL\r\n0100\r\n256\r\n";
	fwrite(Head,sizeof(Head)-1,1,F);
	for(int i=0;i<256;i++)
	{
		char Cmp[64];
		sprintf(Cmp,"%d %d %d\r\n",(int)BC[i].R,(int)BC[i].V,(int)BC[i].B);
		fwrite(Cmp,strlen(Cmp),1,F);
	}
	fclose(F);
	return;
}

void SPG_CONV C256_WriteTXT(PixCoul* BC, char* PName)
{
	CHECK(BC==0,"C256_WriteTXT: Tableau vide",return);
	FILE*F=fopen(PName,"wb");
	CHECKTWO(F==0,"Impossible d'ouvrir le fichier",PName,return);
	for(int i=0;i<256;i++)
	{
		char Cmp[64];
		sprintf(Cmp,"%d %d %d\r\n",BC[i].R,BC[i].V,BC[i].B);
		fwrite(Cmp,strlen(Cmp),1,F);
	}
	fclose(F);
	return;
}

#ifdef SPG_General_USESGRAPH

#ifndef SPG_General_PGLib

int SPG_CONV C256_FillTexture(SG_FullTexture& T, PixCoul*BC)
{
	CHECK(T.T.MemTex==0,"C256_FillTexture: Texture nulle",return 0);
	CHECK(BC==0,"C256_FillTexture: Table de couleurs nulle",return 0);

	int y;
	for(y=0;y<T.SizeY;y++)
	{
		if(y<=(T.SizeY/2))
		{
			float K=y/(float)(T.SizeY/2);//sqrt(0.25+0.75*(2*y)/(float)(T.SizeY-1));
			int x;
			for(x=0;x<T.SizeX;x++)
			{
				Texel(T.T,x,y).B=V_FloatToByte(K*BC[(x*255)/(T.SizeX-1)].B);
				Texel(T.T,x,y).V=V_FloatToByte(K*BC[(x*255)/(T.SizeX-1)].V);
				Texel(T.T,x,y).R=V_FloatToByte(K*BC[(x*255)/(T.SizeX-1)].R);
			}
		}
		else
		{
			float K=(T.SizeY-1-y)/(float)(T.SizeY/2);//sqrt((2*T.SizeY-2*y)/(float)(T.SizeY-1));
			int x;
			for(x=0;x<T.SizeX;x++)
			{
				Texel(T.T,x,y).B=(BYTE)(255-V_FloatToInt(K*(255-BC[(x*255)/(T.SizeX-1)].B)));
				Texel(T.T,x,y).V=(BYTE)(255-V_FloatToInt(K*(255-BC[(x*255)/(T.SizeX-1)].V)));
				Texel(T.T,x,y).R=(BYTE)(255-V_FloatToInt(K*(255-BC[(x*255)/(T.SizeX-1)].R)));
			}
		}

	}
	//SG_SaveTexture("C256_FillTexture.bmp",T);
	//C256_Save(BC,"C256_FillTexture");
	return -1;
}
#endif

void SPG_CONV C256_MakeFaceColor(SG_Bloc& B, int SizeX, int SizeY, SG_FullTexture& T, V_VECT& Light, float Diffusivity)
{
	int y;
	for (y=0;y<SizeY;y++)
	{
		int x;
		for(x=0;x<SizeX;x++)
		{
			SG_FACE* CurFace=B.MemFaces+x+SizeX*y;
#ifndef SGE_EMC
			V_VECT DiffX;
			V_VECT DiffY;
			V_Operate3(DiffX,=CurFace->NumP2->P,-CurFace->NumP1->P);
			V_Operate3(DiffY,=CurFace->NumP4->P,-CurFace->NumP1->P);
			V_VECT FNormal;
			V_VectVect(DiffX,DiffY,FNormal);

			float LightToNormal;
			V_ScalVect(FNormal,Light,LightToNormal);
			float NrmNormal;
			V_ModVect_Unsafe(FNormal,NrmNormal);
			//float SL=0.5*(1+(LightToNormal/NrmNormal));
			float SL=LightToNormal/NrmNormal;
#else
			float SL;
			V_ScalVect(CurFace->Normale,Light,SL);
#endif

			float L = pow(V_Sature(SL,0,1),Diffusivity);
			CurFace->YT1=V_FloatToShort(L*(SG_TexSizeY(T)-1));

			if(x>0)
			{
				CurFace[-1].YT2=CurFace->YT1;

			if(y>0)
			{
				CurFace[-1-SizeX].YT4=CurFace[-1].YT1;
				CurFace[-1-SizeX].YT3=CurFace[-1].YT2;
			}
			}
		}
		B.MemFaces[SizeX-1+SizeX*y].YT2=B.MemFaces[SizeX-1+SizeX*y].YT1;
		if (y>0) 
		{
			SG_FACE* CurFace=B.MemFaces+SizeX-1+SizeX*(y-1);
			CurFace->YT4=CurFace[SizeX].YT1;
			CurFace->YT3=CurFace[SizeX].YT2;
		}
	}
	{
		int x;
		for(x=0;x<SizeX;x++)
		{
			SG_FACE* CurFace=B.MemFaces+x+SizeX*(SizeY-1);
			CurFace->YT4=CurFace->YT1;
			CurFace->YT3=CurFace->YT2;
		}
	}
	return;
}


void SPG_CONV C256_MakeFaceBlend(SG_FullBloc& B, SG_FullBloc& BRef, V_VECT Light, float Diffusivity)
{
	CHECK(B.Etat==0,"C256_MakeFaceBlend: bloc invalide",return);
	CHECK(B.DB.MemFaces==0,"C256_MakeFaceBlend: bloc invalide",return);
	CHECK(BRef.Etat==0,"C256_MakeFaceBlend: bloc invalide",return);
	CHECK(BRef.DB.MemFaces==0,"C256_MakeFaceBlend: bloc invalide",return);
	for(int NumF=0;NumF<B.DB.NombreF;NumF++)
	{
		SG_FACE* CurFace=B.DB.MemFaces+NumF;
#ifndef SGE_EMC
		V_VECT DiffX;
		V_VECT DiffY;
		V_Operate3(DiffX,=CurFace->NumP2->P,-CurFace->NumP1->P);
		V_Operate3(DiffY,=CurFace->NumP4->P,-CurFace->NumP1->P);
		V_VECT FNormal;
		V_VectVect(DiffX,DiffY,FNormal);

		float LightToNormal;
		V_ScalVect(FNormal,Light,LightToNormal);
		float NrmNormal;
		V_ModVect_Unsafe(FNormal,NrmNormal);
		//float SL=0.5*(1+(LightToNormal/NrmNormal));
		float SL=LightToNormal/NrmNormal;
#else
		float SL;
		V_ScalVect(CurFace->Normale,Light,SL);
#endif
#ifndef SGE_EMC
#ifndef SPG_General_PGLib
		float L = pow(V_Sature(SL,0,1),Diffusivity);
		CurFace->Couleur.A=SG_MAX_ALPHA*L;
#else
//#define G_CombineFaceAndLight(CDest,CFace,CLight) CDest.B=(BYTE)(((CFace.B<<8)+CLight.A*(CLight.B-CFace.B))>>8);CDest.V=(BYTE)(((CFace.V<<8)+CLight.A*(CLight.V-CFace.V))>>8);CDest.R=(BYTE)(((CFace.R<<8)+CLight.A*(CLight.R-CFace.R))>>8)
		float L = pow(V_Sature(SL,0,1),Diffusivity);
		if(L>0.5)
		{
		BYTE ALPHA=511.9*(L-0.5);
		G_BlendColor(CurFace->Couleur,BRef.DB.MemFaces[NumF].Couleur,ALPHA);
		}
		else
		{
		WORD ALPHA=511.9*L;
		CurFace->Couleur.B=(BRef.DB.MemFaces[NumF].Couleur.B*ALPHA)>>8;
		CurFace->Couleur.V=(BRef.DB.MemFaces[NumF].Couleur.V*ALPHA)>>8;
		CurFace->Couleur.R=(BRef.DB.MemFaces[NumF].Couleur.R*ALPHA)>>8;
		}
#endif
#else
		float L = pow(V_Sature(SL,0,1),Diffusivity);
		if(L>0.5)
		{
		BYTE ALPHA=(BYTE)V_Floor((float)511.99f*(L-0.5f));
		G_BlendColor(CurFace->Couleur,BRef.DB.MemFaces[NumF].Couleur,ALPHA);
		}
		else
		{
		WORD ALPHA=(WORD)V_Floor(511.99f*L);
		CurFace->Couleur.B=(BYTE)((BRef.DB.MemFaces[NumF].Couleur.B*ALPHA)>>8);
		CurFace->Couleur.V=(BYTE)((BRef.DB.MemFaces[NumF].Couleur.V*ALPHA)>>8);
		CurFace->Couleur.R=(BYTE)((BRef.DB.MemFaces[NumF].Couleur.R*ALPHA)>>8);
		}
#endif
	}
	return;
} 
#endif

#endif

