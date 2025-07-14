
#include "SPG_General.h"

#ifdef SPG_General_USEProfil

//#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <float.h>

#include "SPG_Includes.h"
#include "SPG_SysInc.h"


void SPG_CONV P_Extract(Profil& PDest, Profil& PSrc, int X0, int Y0, int SX, int SY)
{
	CHECK(P_Etat(PSrc)==0,"P_Extract: Profil source nul",return);
	CHECK(X0<0,"P_Extract",return);	CHECK(Y0<0,"P_Extract",return);
	CHECK(X0+SX>P_SizeX(PSrc),"P_Extract",return);
	CHECK(Y0+SY>P_SizeY(PSrc),"P_Extract",return);
	
	CHECK(P_Create(PDest,SX,SY,PSrc.H.XScale,PSrc.H.YScale,PSrc.H.UnitX,PSrc.H.UnitY,PSrc.H.UnitZ,P_Msk(PSrc)!=0),"P_Extract",return);

	for(int y=0;y<SY;y++)
	{
		for(int x=0;x<SX;x++)
		{
			if(P_Data(PSrc)&&(P_Data(PDest)) )
			{
				P_Element(PDest,x,y)=P_Element(PSrc,(x+X0),(y+Y0));
			}
			if(P_Msk(PSrc)&&(P_Msk(PDest)))
			{
				P_ElementMsk(PDest,x,y)=P_ElementMsk(PSrc,(x+X0),(y+Y0));
			}
		}
	}
	return;
}

void SPG_CONV P8_GradientMoyen(Profil8& P, int XPos,int YPos, int SizeX, int SizeY, float& G)
{
	G=0;
	CHECK(P8_Etat(P)==0,"P8_GradientMoyen: Profil nul",return);
	CHECK(P8_Data(P)==0,"P8_GradientMoyen: Profil vide",return);

	for(int y=YPos;y<YPos+SizeY-1;y++)
	{
		float GLine=0;
		BYTE* B=P8_Data(P)+y*P8_SizeX(P)+XPos;
		BYTE* BNextX=B+1;
		BYTE* BNextY=B+P8_SizeX(P);
		for(int x=0;x<SizeX-1;x++)
		{
			GLine+=abs((int)(*B)-(int)(*BNextX));
			GLine+=abs((int)(*B)-(int)(*BNextY));
			B++;
			BNextX++;
			BNextY++;
		}
		G+=GLine;
	}
	return;
}

void SPG_CONV P8_HiFreqMoyen(Profil8& P, int XPos,int YPos, int SizeX, int SizeY, float& G)
{
	G=0;
	CHECK(P8_Etat(P)==0,"P8_HiFreqMoyen: Profil nul",return);
	CHECK(P8_Data(P)==0,"P8_HiFreqMoyen: Profil vide",return);

	for(int y=YPos+2;y<YPos+SizeY-2;y++)
	{
		float GLine=0;
		BYTE* B=P8_Data(P)+y*P8_SizeX(P)+XPos;
		for(int x=2;x<SizeX-2;x++)
		{
			GLine+=abs(B[-2]-2*B[-1]+2*B[0]-2*B[1]+B[2]);
			GLine+=abs(B[-2*P8_SizeX(P)]-2*B[-1*P8_SizeX(P)]+2*B[0]-2*B[1*P8_SizeX(P)]+B[2*P8_SizeX(P)]);
			B++;
		}
		G+=GLine;
	}
	return;
}
/*
void SPG_CONV P8_GetHistogram(Profil8& P, HIST_TYPE& H)
{
	CHECK(H.NumCater!=256,"P8_GetHistogram: L'histogramme doit exister avec 256 categories", return);
	HIST_Clear(HIST);
	CHECK(P8_Etat(P)==0,"P8_GetHistogram: Profil nul",return);
	CHECK(P8_Data(P)==0,"P8_GetHistogram: Profil vide",return);
	for(int i=0;i<P8_SizeX(P)*P8_SizeY(P);i++)
	{
		HIST_Add(H,P8_Data(P)[i]);
	}
	return;
}
*/

void SPG_CONV P_FindMinMax(Profil& P, float &fMin, float &fMax)
{
	CHECK(P_Etat(P)==0,"P_FindMinMax: Profil nul",fMin=fMax=0;return);
	CHECK(P_Data(P)==0,"P_FindMinMax: Profil vide",fMin=fMax=0;return);
	int i;
	if(P_Msk(P)==0)
	{
		float lMin,lMax;
		lMin=lMax=*P_Data(P);
		float * RAWDATA=P_Data(P);
		for(i=0;i<P_SizeX(P)*P_SizeY(P)-1;i+=2)
		{
			if(*RAWDATA>*(RAWDATA+1))
			{
				if(*RAWDATA>lMax) lMax=*RAWDATA;
				if(*(RAWDATA+1)<lMin) lMin=*(RAWDATA+1);
			}
			else
			{
				if(*RAWDATA<lMin) lMin=*RAWDATA;
				if(*(RAWDATA+1)>lMax) lMax=*(RAWDATA+1);
			}
			RAWDATA+=2;
		}
		fMin=lMin;
		fMax=lMax;
	}
	else
	{
	for(i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		if(P_Msk(P)[i]==0)
		{
		fMin=fMax=P_Data(P)[i];
		break;
		}
	}
	if (i==P_SizeX(P)*P_SizeY(P))
	{
		fMin=fMax=0;
		return;
	}
	float* FD=P_Data(P)+i;
	for(;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		if(P_Msk(P)[i]==0)
		{
			if(*FD<fMin) 
				fMin=*FD;
			else if (*FD>fMax) 
				fMax=*FD;
		}
		*FD++;
	}
	}
	CD_G_CHECK_EXIT(21,13);
	CHECKFLOAT(fMin,"P_FindMinMax");
	CHECKFLOAT(fMax,"P_FindMinMax");
	return;
}

int SPG_CONV P_FindMax(Profil& P, int& PosX, int& PosY)
{
	CHECK(P_Etat(P)==0,"P_FindMax: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_FindMax: Profil nul",return 0);
	float Max=0;
	int y;
	for(y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if((P_Msk(P)==0)||(P_ElementMsk(P,x,y)==0))
			{
				Max=P_Element(P,x,y);
				PosX=x;
				PosY=y;
				break;
			}
			if(x<P_SizeX(P)) break;
		}
		if(y<P_SizeY(P)) break;
	}

	if(y==P_SizeY(P)) return 0;
	for(;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(((P_Msk(P)==0)||(P_ElementMsk(P,x,y)==0))&&(P_Element(P,x,y)>Max))
			{
				Max=P_Element(P,x,y);
				PosX=x;
				PosY=y;
			}
		}
	}
	CHECKFLOAT(Max,"P_FindMax");
	return -1;
}

int SPG_CONV P_FindMin(Profil& P, int& PosX, int& PosY)
{
	CHECK(P_Etat(P)==0,"P_FindMin: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_FindMin: Profil nul",return 0);
	float Min=0;
	int y;
	for(y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if((P_Msk(P)==0)||(P_ElementMsk(P,x,y)==0))
			{
				Min=P_Element(P,x,y);
				PosX=x;
				PosY=y;
				break;
			}
			if(x<P_SizeX(P)) break;
		}
		if(y<P_SizeY(P)) break;
	}

	if(y==P_SizeY(P)) return 0;
	for(;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(((P_Msk(P)==0)||(P_ElementMsk(P,x,y)==0))&&(P_Element(P,x,y)<Min))
			{
				Min=P_Element(P,x,y);
				PosX=x;
				PosY=y;
			}
		}
	}
	CHECKFLOAT(Min,"P_FindMin");
	return -1;
}

void SPG_CONV P_Update(Profil& PN, Profil& PF, Profil& PMin, Profil& PMax, float dt, float dtzero)
{
	for(int i=0;i<P_SizeX(PN)*P_SizeY(PN);i++)
	{
		if(PF.D[i]<PMin.D[i]) PMin.D[i]=PF.D[i]; else PMin.D[i]+=dtzero*(PF.D[i]-PMin.D[i]);
		if(PF.D[i]>PMax.D[i]) PMax.D[i]=PF.D[i]; else PMax.D[i]+=dtzero*(PF.D[i]-PMax.D[i]);
		float& X=PF.D[i];
		float& MAX=PMax.D[i];
		float& MIN=PMin.D[i];
		if(MAX>MIN)
		{
			//float R= (1/X - 1/MAX) * (MIN*MAX)/(MAX-MIN) ;
			float R=(MAX-X)/(MAX-MIN);

			if(R<0) R=0;
			if(R>1) R=1;
			PN.D[i]+=dt*(R-PN.D[i]);
		}
		else
		{
			PN.D[i]=0;
		}
	}
	return;
}

void SPG_CONV P_InterleavedFindMax(Profil& P, int Start, int Stop, int Step, int& PosOfMax)
{
	float ValMax=P_Data(P)[Start];
	for(int i=Start+Step;i<Stop;i+=Step)
	{
		if(P_Data(P)[i]>ValMax)
		{
			ValMax=P_Data(P)[i];
			PosOfMax=i;
		}
	}
	return;
}

void SPG_CONV P_FullFindMaxFast(Profil& P, int Start, int Stop, int BiggestStep, int& PosOfMax)
{
	P_InterleavedFindMax(P,Start,Stop,BiggestStep,PosOfMax);
	while(BiggestStep>1)
	{
	BiggestStep>>=2;
	if(BiggestStep<=0) BiggestStep=1;
	if(PosOfMax-BiggestStep>Start) Start=PosOfMax-BiggestStep;
	if(PosOfMax+BiggestStep<Stop) Stop=PosOfMax+BiggestStep;
	P_InterleavedFindMax(P,Start,Stop,BiggestStep,PosOfMax);
	}
	return;
}

int SPG_CONV P_Add(Profil& PRes, Profil& PAdd)
{
	CHECK(P_Etat(PRes)==0,"P_Substract: Profil destination nul",return 0);
	CHECK(P_Etat(PAdd)==0,"P_Substract: Profil operande nul",return 0);
	CHECK((P_SizeX(PRes)!=P_SizeX(PAdd))&&(P_SizeY(PRes)!=P_SizeY(PAdd)),"P_Substract: Tailles inegales",return 0)
	for(int i=0;i<P_SizeX(PRes)*P_SizeY(PRes);i++)
	{
		PRes.D[i]+=PAdd.D[i];
		if(P_Msk(PRes)&&P_Msk(PAdd))
			PRes.Msk[i]|=PAdd.Msk[i];
	}
	return -1;
}

int SPG_CONV P_Substract(Profil& PRes, Profil& PSub)
{
	CHECK(P_Etat(PRes)==0,"P_Substract: Profil destination nul",return 0);
	CHECK(P_Etat(PSub)==0,"P_Substract: Profil operande nul",return 0);
	CHECK((P_SizeX(PRes)!=P_SizeX(PSub))&&(P_SizeY(PRes)!=P_SizeY(PSub)),"P_Substract: Tailles inegales",return 0)
	for(int i=0;i<P_SizeX(PRes)*P_SizeY(PRes);i++)
	{
		PRes.D[i]-=PSub.D[i];
		if(P_Msk(PRes)&&P_Msk(PSub))
			PRes.Msk[i]|=PSub.Msk[i];
	}
	return -1;
}

int SPG_CONV P_RevSubstract(Profil& PRes, Profil& PSub)
{
	CHECK(P_Etat(PRes)==0,"P_RevSubstract: Profil destination nul",return 0);
	CHECK(P_Etat(PSub)==0,"P_RevSubstract: Profil operande nul",return 0);
	CHECK((P_SizeX(PRes)!=P_SizeX(PSub))&&(P_SizeY(PRes)!=P_SizeY(PSub)),"P_Substract: Tailles inegales",return 0)
	for(int i=0;i<P_SizeX(PRes)*P_SizeY(PRes);i++)
	{
		PRes.D[i]=PSub.D[i]-PRes.D[i];
		if(P_Msk(PRes)&&P_Msk(PSub))
			PRes.Msk[i]|=PSub.Msk[i];
	}
	return -1;
}

int SPG_CONV P_Divise(Profil& PRes, Profil& PDiviseur)
{
	CHECK(P_Etat(PRes)==0,"P_Divise: Profil destination nul",return 0);
	CHECK(P_Etat(PDiviseur)==0,"P_Divise: Profil operande nul",return 0);
	CHECK((P_SizeX(PRes)!=P_SizeX(PDiviseur))&&(P_SizeY(PRes)!=P_SizeY(PDiviseur)),"P_Divise: Tailles inegales",return 0)
	for(int i=0;i<P_SizeX(PRes)*P_SizeY(PRes);i++)
	{
		if(
			((P_Msk(PRes)==0)||(PRes.Msk[i]==0))&&
			((P_Msk(PDiviseur)==0)||(PDiviseur.Msk[i]==0))&&
			(PDiviseur.D[i]!=0))
		{
			PRes.D[i]/=PDiviseur.D[i];
		}
		else
		{
			PRes.D[i]=0;
			if (P_Msk(PRes)) PRes.Msk[i]=1;
		}
	}
	return -1;
}

void SPG_CONV P_Normalize(Profil& PRes, Profil& PDiviseur)
{
	CHECK((P_SizeX(PRes)!=P_SizeX(PDiviseur))&&(P_SizeY(PRes)!=P_SizeY(PDiviseur)),"P_Divise: Tailles inegales",return)
	for(int i=0;i<P_SizeX(PRes)*P_SizeY(PRes);i++)
	{
		if(
			((P_Msk(PRes)==0)||(PRes.Msk[i]==0))&&
			((P_Msk(PDiviseur)==0)||(PDiviseur.Msk[i]==0))&&
			(PDiviseur.D[i]!=0))
		{
			PRes.D[i]/=PDiviseur.D[i];
			if(PRes.D[i]>1) PRes.D[i]=1;
			if(PRes.D[i]<0) PRes.D[i]=0;
		}
		else
		{
			PRes.D[i]=0;
			if (P_Msk(PRes)) PRes.Msk[i]=1;
		}
	}
	return;
}

int SPG_CONV P_GetUndersW(Profil& Pdest, Profil& P, int xMin, int xMax, int yMin, int yMax, int Unders)
{
	memset(&Pdest,0,sizeof(Profil));
	CHECK(P_Etat(P)==0,"P_GetUndersW: Profil source nul",return 0);
	CHECK(V_InclusiveBound(xMin,0,P_SizeX(P))==0,"P_GetUndersW: xMin est hors du profil source",return 0);
	CHECK(V_InclusiveBound(xMax,0,P_SizeX(P))==0,"P_GetUndersW: xMax est hors du profil source",return 0);
	CHECK(V_InclusiveBound(yMin,0,P_SizeY(P))==0,"P_GetUndersW: yMin est hors du profil source",return 0);
	CHECK(V_InclusiveBound(yMax,0,P_SizeY(P))==0,"P_GetUndersW: yMax est hors du profil source",return 0);
	CHECK(P_Create(Pdest,(xMax-xMin)/Unders,(yMax-yMin)/Unders,P_XScale(P)*Unders,P_YScale(P)*Unders,P_UnitX(P),P_UnitY(P),P_UnitZ(P),(int)P_Msk(P))==0,"P_GetUndersW",return 0);
	int y;
	for(y=0;y<P_SizeY(Pdest);y++)
	{
		int ymDest=y*P_SizeX(Pdest);
		int ymSrc=(y*Unders+yMin)*P_SizeX(P)+xMin;
		int x;
		if(P_Msk(Pdest))
		{
		for(x=0;x<P_SizeX(Pdest);x++)
		{
			//Pdest.D[ymDest+x]=0;
			int ymSrcx=ymSrc+Unders*x;
			int ymDestx=ymDest+x;
			Pdest.D[ymDestx]=0;
			int Count=0;
			for(int ys=0;ys<Unders;ys++)
			{
			int ymSrcy=ymSrcx+ys*P_SizeX(P);
			for(int xs=0;xs<Unders;xs++)
			{
				if(P_Msk(P)[ymSrcy+xs]==0)
				{
			  Pdest.D[ymDestx]+=P_Data(P)[ymSrcy+xs];
			  Count++;
				}
			}
			}
			if(Count)
			{
				Pdest.D[ymDestx]/=Count;
				Pdest.Msk[ymDestx]=0;
			}
			else
			{
				Pdest.Msk[ymDestx]=1;
			}
		}
		}
		else
		{
		for(x=0;x<P_SizeX(Pdest);x++)
		{
			Pdest.D[ymDest+x]=P_Data(P)[ymSrc+Unders*x];
		}
		}
	}
	return P_Etat(Pdest);
}

int SPG_CONV P_GetUnders(Profil& Pdest, Profil& P, int Unders)
{//pas de gestion du masque
	CHECK(P_Etat(P)==0,"P_GetUndersW: Profil source nul",return 0);

	const int YM=V_Min(P_SizeY(Pdest),P_SizeY(P)/Unders);
	const int XM=V_Min(P_SizeX(Pdest),P_SizeX(P)/Unders);
	const float Nrm=1.0f/(Unders*Unders);
	int y;
	for(y=0;y<YM;y++)
	{
		int ymDest=y*P_SizeX(Pdest);
		int ymSrc=(y*Unders)*P_SizeX(P);
		int x;
	for(x=0;x<XM;x++)
	{
		//Pdest.D[ymDest+x]=0;
		const int ymSrcx=ymSrc+Unders*x;
		const int ymDestx=ymDest+x;
		Pdest.D[ymDestx]=0;
		for(int ys=0;ys<Unders;ys++)
		{
			const int ymSrcy=ymSrcx+ys*P_SizeX(P);
			for(int xs=0;xs<Unders;xs++)
			{
			  Pdest.D[ymDestx]+=P_Data(P)[ymSrcy+xs];
			}
		}
		Pdest.D[ymDestx]*=Nrm;
	}
	}
	return -1;
}

int SPG_CONV P_HalfsampleMin(Profil& Pdest, Profil& P)
{
	CHECK(P_Etat(P)==0,"P_HalfsampleMin",return 0);
	P_Create(Pdest,P_SizeX(P)/2,P_SizeY(P)/2);
	for(int y=0;y<P_SizeY(Pdest);y++)
	{
		for(int x=0;x<P_SizeX(Pdest);x++)
		{
			float M=P_Element(P,2*x,2*y);
			M=V_Min(M,P_Element(P,(2*x+1),2*y));
			M=V_Min(M,P_Element(P,2*x,(2*y+1)));
			M=V_Min(M,P_Element(P,(2*x+1),(2*y+1)));
			P_Element(Pdest,x,y)=M;
		}
	}
	return P_Etat(Pdest);
}

int SPG_CONV P_DblSample(Profil& Pdest, Profil& P)
{
	CHECK(P_Etat(P)==0,"P_HalfsampleMin",return 0);
	P_Create(Pdest,P_SizeX(P)*2,P_SizeY(P)*2);
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			float M=P_Element(P,x,y);
			P_Element(Pdest,2*x,2*y)=M;
			P_Element(Pdest,(2*x+1),2*y)=M;
			P_Element(Pdest,2*x,(2*y+1))=M;
			P_Element(Pdest,(2*x+1),(2*y+1))=M;
		}
	}
	P_FastConvLowPass(Pdest,3);
	P_FastConvLowPass(Pdest,3);
	return P_Etat(Pdest);
}

int SPG_CONV P_DblSampleEx(Profil& Pdest, Profil& P)
{
	CHECK(P_Etat(P)==0,"P_HalfsampleMin",return 0);
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			float M=P_Element(P,x,y);
			P_Element(Pdest,2*x,2*y)=M;
			P_Element(Pdest,(2*x+1),2*y)=M;
			P_Element(Pdest,2*x,(2*y+1))=M;
			P_Element(Pdest,(2*x+1),(2*y+1))=M;
		}
	}
	P_FastConvLowPass(Pdest,3);
	return P_Etat(Pdest);
}

void SPG_CONV P_SetBorderMsk(Profil& P, int BorderWidth)
{
	CHECK(P_Etat(P)==0,"P_SetBorderMsk: Profil nul",return);
	CHECK(P_Msk(P)==0,"P_SetBorderMsk: Profil sans masque",return);
	CHECK(!V_IsBound((2*BorderWidth),0,(V_Min(P_SizeX(P),P_SizeY(P)))),"P_SetBorderMsk: Taille de bord masque incorrecte",return);
	{
	BYTE*LeftCol=P_Msk(P);
	BYTE*RightCol=P_Msk(P)+P_SizeX(P)-BorderWidth;
	for(int y=0;y<P_SizeY(P);y++)
	{
		memset(LeftCol,1,BorderWidth);
		memset(RightCol,1,BorderWidth);
		LeftCol+=P_SizeX(P);
		RightCol+=P_SizeX(P);
	}
	}
	{
	BYTE*UpLine=P_Msk(P);
	BYTE*BotLine=P_Msk(P)+P_SizeX(P)*(P_SizeY(P)-BorderWidth);
	for(int y=0;y<BorderWidth;y++)
	{
		memset(UpLine,1,P_SizeX(P));
		memset(BotLine,1,P_SizeX(P));
		UpLine+=P_SizeX(P);
		BotLine+=P_SizeX(P);
	}
	}
	return;
}

void SPG_CONV P_SetBorderVal(Profil& P, float Value, int BorderWidth)
{
	CHECK(P_Etat(P)==0,"P_SetBorderVal: Profil nul",return);
	CHECK(!V_IsBound((2*BorderWidth),0,(V_Min(P_SizeX(P),P_SizeY(P)))),"P_SetBorderVal: Taille de bord masque incorrecte",return);
	{
	float*LeftCol=P_Data(P);
	float*RightCol=P_Data(P)+P_SizeX(P)-BorderWidth;
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<BorderWidth;x++)
		{
			LeftCol[x]=Value;
			RightCol[x]=Value;
		}
		LeftCol+=P_SizeX(P);
		RightCol+=P_SizeX(P);
	}
	}
	{
	float*UpLine=P_Data(P);
	float*BotLine=P_Data(P)+P_SizeX(P)*(P_SizeY(P)-BorderWidth);
	for(int y=0;y<BorderWidth;y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			UpLine[x]=Value;
			BotLine[x]=Value;
		}
		UpLine+=P_SizeX(P);
		BotLine+=P_SizeX(P);
	}
	}
	return;
}

void SPG_CONV P_ExtendBorderVal(Profil& P, int BorderWidth)
{
	CHECK(P_Etat(P)==0,"P_SetBorderVal: Profil nul",return);
	CHECK(!V_IsBound((2*BorderWidth+2),0,(V_Min(P_SizeX(P),P_SizeY(P)))),"P_SetBorderVal: Taille de bord masque incorrecte",return);
	{
	float*LeftCol=P_Data(P);
	float*RightCol=P_Data(P)+P_SizeX(P)-BorderWidth;
	for(int y=0;y<P_SizeY(P);y++)
	{
		float ValueL=LeftCol[BorderWidth];
		float ValueR=RightCol[-1];
		for(int x=0;x<BorderWidth;x++)
		{
			LeftCol[x]=ValueL;
			RightCol[x]=ValueR;
		}
		LeftCol+=P_SizeX(P);
		RightCol+=P_SizeX(P);
	}
	}
	{
	float*UpLine=P_Data(P);
	float*UpLineRef=P_Data(P)+BorderWidth*P_SizeX(P);
	float*BotLine=P_Data(P)+P_SizeX(P)*(P_SizeY(P)-BorderWidth);
	float*BotLineRef=P_Data(P)+P_SizeX(P)*(P_SizeY(P)-BorderWidth-1);
	for(int y=0;y<BorderWidth;y++)
	{
		memcpy(UpLine,UpLineRef,P_SizeX(P)*sizeof(float));
		memcpy(BotLine,BotLineRef,P_SizeX(P)*sizeof(float));
		/*
		for(int x=0;x<P_SizeX(P);x++)
		{
			UpLine[x]=Value;
			BotLine[x]=Value;
		}
		*/
		UpLine+=P_SizeX(P);
		BotLine+=P_SizeX(P);
	}
	}
	return;
}

void SPG_CONV P_RemoveIsolated(Profil& P, int Radius)
{
	CHECK(P_Etat(P)==0,"P_RemoveIsolated: Profil nul",return);
	CHECK(P_Data(P)==0,"P_RemoveIsolated: Profil vide",return);
	CHECK(P_Msk(P)==0,"P_RemoveIsolated: Profil sans masque",return);
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(P_ElementMsk(P,x,y)==0)
			{
				for(int ys=V_Max(y-Radius,0);ys<=V_Min(y+Radius,P_SizeY(P)-1);ys++)
				{
					for(int xs=V_Max(x-Radius,0);xs<=V_Min(x+Radius,P_SizeX(P)-1);xs++)
					{
						if(P_ElementMsk(P,xs,ys)==0) 
						{
							if((xs!=x)&&(ys!=y)) goto found_one;
						}
					}
				}
				P_ElementMsk(P,x,y)=1;
				//P_Element(P,x,y)=0;
				//SPG_List("Pixel masque");
			}
found_one:;
		}
	}
	return;
}

void SPG_CONV P_RemoveBorder(Profil& P, int Radius)
{
	CHECK(P_Etat(P)==0,"P_RemoveBorder: Profil nul",return);
	CHECK(P_Data(P)==0,"P_RemoveBorder: Profil vide",return);
	CHECK(P_Msk(P)==0,"P_RemoveBorder: Profil sans masque",return);
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(P_ElementMsk(P,x,y)==0)
			{
				int UnmaskedElement=0;
				for(int ys=V_Max(y-Radius,0);ys<=V_Min(y+Radius,P_SizeY(P)-1);ys++)
				{
					for(int xs=V_Max(x-Radius,0);xs<=V_Min(x+Radius,P_SizeX(P)-1);xs++)
					{
						if((P_ElementMsk(P,xs,ys)&1)==0) 
						{
							UnmaskedElement++;
						}
					}
				}
				if(UnmaskedElement<=(2*Radius+1)*(Radius+1))
				{
				//P_Element(P,x,y)=0;
					P_ElementMsk(P,x,y)|=2;
				}
				//SPG_List("Pixel masque");
			}
			else
			{
				P_ElementMsk(P,x,y)|=2;
			}
		}
	}
	BYTE* PMsk=P_Msk(P);
	for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		*PMsk>>=1;
		PMsk++;
	}
	return;
}

int SPG_CONV P_FillInTheBlanks(Profil& P, int MskSafe, int SearchMax, int MskValue, int MinFound)
{
	CHECK(P_Etat(P)==0,"P_FillInTheBlanks: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_FillInTheBlanks: Profil vide",return 0);
	CHECK(P_Msk(P)==0,"P_FillInTheBlanks: Profil sans masque",return 0);
	CHECK(SearchMax==0,"P_FillInTheBlanks: La taille maximale a remplir doit etre specifiee",return 0);
	//SPG_MemFastCheck();
	int Filled=0;
	BYTE MskMskVal=MskValue|2;
	//BYTE MskValVal=MskValue&~2;
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(P_ElementMsk(P,x,y)&MskMskVal)// ==MskValVal)
			{//elements invalides si le masque contient des bits de MskValue à 1
				float Total=0;
				int Found=0;
				for(int Search=1;Search<=SearchMax;Search++)
				{
					//int xsearch,ysearch;
					
					//bord superieur/inferieur
					{
					int ysearchup=V_Max(y-Search,0);
					int ysearchdn=V_Min(y+Search,P_SizeY(P)-1);

					for(int xsearch=V_Max(x-Search,0);
					xsearch<=V_Min(x+Search,P_SizeX(P)-1);
					xsearch++)
					{
if(P_ElementMsk(P,xsearch,ysearchup)==0) 
{
	Total+=P_Element(P,xsearch,ysearchup);
	Found++;
}
if(P_ElementMsk(P,xsearch,ysearchdn)==0) 
{
	Total+=P_Element(P,xsearch,ysearchdn);
	Found++;
}
					}
					}

					//bord gauche/droit
					{
					int xsearchlf=V_Max(x-Search,0);
					int xsearchrt=V_Min(x+Search,P_SizeX(P)-1);

					for(int ysearch=V_Max(y-Search+1,0);
					ysearch<=V_Min(y+Search-1,P_SizeY(P)-1);
					ysearch++)
					{
if(P_ElementMsk(P,xsearchlf,ysearch)==0) 
{
	Total+=P_Element(P,xsearchlf,ysearch);
	Found++;
}
if(P_ElementMsk(P,xsearchrt,ysearch)==0) 
{
	Total+=P_Element(P,xsearchrt,ysearch);
	Found++;
}
					}
					}

				//longueur du perimetre de recherche: 8*Search
				//aire de la recherche (2*Search+1)*(2*Search+1)
				if(((MinFound==0)&&(Found>0))||(Found>=(2*Search+1)*Search))
				{
					P_Element(P,x,y)=Total/Found;
					Filled++;
					//interpole et place le bit 2
					if (MskSafe)
						P_ElementMsk(P,x,y)|=2;//
						else
						P_ElementMsk(P,x,y)=2;
					break;
				}
				}
			}
		}
	}

	BYTE* PMsk=P_Msk(P);
	for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		*PMsk&=~2;//enleve le bit 2
		PMsk++;
	}
	//SPG_MemFastCheck();
	return Filled;
}

void SPG_CONV P_Separate(Profil& PLow, Profil& PSrc, int Iter)
{
	P_Copy(PLow,PSrc);
	{for(int i=0;i<Iter;i++)
	{
		P_MaskConv3x3(PLow);
	}}
	{for(int i=0;i<P_SizeX(PLow)*P_SizeY(PLow);i++)
	{
		P_Data(PSrc)[i]-=P_Data(PLow)[i];
	}}
	return;
}

void SPG_CONV P_MskThresholdAbs(Profil& PDest, Profil& PSrc, float Threshold, BYTE MskOr, BYTE MskAnd)
{
	CHECK(P_Etat(PDest)==0,"P_MskThreshold",return);
	CHECK(P_Etat(PSrc)==0,"P_MskThreshold",return);
	CHECK((P_Msk(PSrc)==0)||(P_Msk(PDest)==0),"P_MskThreshold",return);
	CHECK(P_SizeX(PDest)!=P_SizeX(PSrc),"P_MskThreshold",return);
	CHECK(P_SizeY(PDest)!=P_SizeY(PSrc),"P_MskThreshold",return);
	for(int i=0;i<P_SizeX(PSrc)*P_SizeY(PSrc);i++)
	{
		if(fabs(P_Data(PSrc)[i])>Threshold)
		{
			P_Msk(PDest)[i]&=MskAnd;
			P_Msk(PDest)[i]|=(MskOr|P_Msk(PSrc)[i]);
		}
	}
	return;
}

void SPG_CONV P_MskThresholdMinMax(Profil& PDest, Profil& PSrc, float fMin, float fMax, BYTE MskOr, BYTE MskAnd)
{
	CHECK(P_Etat(PDest)==0,"P_MskThreshold",return);
	CHECK(P_Etat(PSrc)==0,"P_MskThreshold",return);
	CHECK((P_Msk(PSrc)==0)||(P_Msk(PDest)==0),"P_MskThreshold",return);
	CHECK(P_SizeX(PDest)!=P_SizeX(PSrc),"P_MskThreshold",return);
	CHECK(P_SizeY(PDest)!=P_SizeY(PSrc),"P_MskThreshold",return);
	for(int i=0;i<P_SizeX(PSrc)*P_SizeY(PSrc);i++)
	{
		if(!V_IsBound(P_Data(PSrc)[i],fMin,fMax))
		{
			P_Msk(PDest)[i]&=MskAnd;
			P_Msk(PDest)[i]|=(MskOr|P_Msk(PSrc)[i]);
		}
	}
	return;
}

void SPG_CONV P_NonLinearFilter(Profil& P, float Threshold)
{
	CHECK(P_Etat(P)==0,"P_NonLinearFilter",return);
	CHECK(P_Msk(P)==0,"P_NonLinearFilter",return);
	Profil PHigh; 	P_DupliquateWithMsk(PHigh,P);

	P_MaskConv3x3(PHigh);P_MaskConv3x3(PHigh);P_MaskConv3x3(PHigh);P_MaskConv3x3(PHigh);

	int Count=0;float Sum=0;
	for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		P_Data(PHigh)[i]-=P_Data(P)[i];
		if((P_Msk(P)==0)||(P_Msk(P)[i]==0))
		{ Count++; Sum+=P_Data(PHigh)[i]*P_Data(PHigh)[i]; }
	}
	//PDest contient les données brutes, PHigh contient le critère de sélection pic/point valide
	float Seuil=Threshold*sqrt(Sum/(1.0f+Count)); P_MskThresholdAbs(P,PHigh,Seuil,4,0xff);
	int Iter=0; while((P_FillInTheBlanks(P,0,5,4)!=0)&&((Iter++)<5));
	P_Close(PHigh);
	return;
}

/*
void SPG_CONV P_NonLinearFilter(Profil& PDest, Profil& PSrc, float Threshold)
{
	CHECK(P_Etat(PSrc)==0,"P_NonLinearFilter",return);
	P_DupliquateWithMsk(PDest,PSrc);
	Profil PHighPass;
	P_Dupliquate(PHighPass,PSrc);
	P_MaskConv3x3(PDest);
	P_MaskConv3x3(PDest);
	P_MaskConv3x3(PDest);
	P_MaskConv3x3(PDest);
	//P_MaskConv3x3(PDest);
	//P_MaskConv3x3(PIntermediate);
	int Count=0;
	float Sum=0;
	for(int i=0;i<P_SizeX(PDest)*P_SizeY(PDest);i++)
	{
		P_Data(PHighPass)[i]=P_Data(PSrc)[i]-P_Data(PDest)[i];
		if((P_Msk(PDest)==0)||(P_Msk(PDest)[i]==0))
		{
			Count++;
			Sum+=P_Data(PHighPass)[i]*P_Data(PHighPass)[i];
		}
	}
	float Seuil=Threshold*sqrt(Sum/(1.0f+Count));
	for(i=0;i<P_SizeX(PDest)*P_SizeY(PDest);i++)
	{
		if(fabs(P_Data(PHighPass)[i])>Seuil)
		{
			P_Msk(PDest)[i]|=4;
		}
		else
		{
			P_Data(PDest)[i]=P_Data(PSrc)[i];
			//P_Data(PDest)[i]+=P_Data(PHighPass)[i];
		}
	}
	P_Close(PHighPass);
	int Iter=0;
	while((P_FillInTheBlanks(PDest,0,5,4)!=0)&&((Iter++)<5));


	//P_ForAll(PDest,i,if(P_Msk(PDest)[i]&4) P_Msk(PDest)[i]=0;);
	return;
}
*/

void SPG_CONV P_YReverse(Profil& P)
{
	CHECK(P_Etat(P)==0,"P_YReverse: Profil nul",return);
	CHECK(P_Data(P)==0,"P_YReverse: Profil vide",return);
	
	{for(int y=0;y<(P_SizeY(P)>>1);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			V_Swap(float,P_Element(P,x,y),P_Element(P,x,P_SizeY(P)-1-y));
		}
	}}

	if(P_Msk(P))
	{
		for(int y=0;y<(P_SizeY(P)>>1);y++)
		{
			for(int x=0;x<P_SizeX(P);x++)
			{
				V_Swap(BYTE,P_ElementMsk(P,x,y),P_ElementMsk(P,x,P_SizeY(P)-1-y));
			}
		}
	}

	return;
}

void SPG_CONV P_Unwrap_0_1(Profil& P)
{
	CHECK(P_Etat(P)==0,"P_Unwrap_0_1: Profil nul",return);
	CHECK(P_Data(P)==0,"P_Unwrap_0_1: Profil vide",return);
	int xcentre=P_SizeX(P)>>1;
	int ycentre=P_SizeY(P)>>1;

	//deplie colonne centrale, centre vers bord inferieur
//	int StepCount=0;
	float LastZ=P_Element(P,xcentre,ycentre);
	int StepCount=0;
	int y;
	for(y=ycentre;y<P_SizeY(P);y++)
	{
		P_UnwrapGeneric(P_Element(P,xcentre,y),StepCount,LastZ);
		//deplie ligne, centre vers bord droit
		{
			float LastZL=LastZ;
			int StepCountL=StepCount;
			for(int xl=xcentre+1;xl<P_SizeX(P);xl++)
			{
				P_UnwrapGeneric(P_Element(P,xl,y),StepCountL,LastZL);
			}
		}
		//deplie ligne, centre vers bord gauche
		{
			float LastZL=LastZ;
			int StepCountL=StepCount;
			for(int xl=xcentre-1;xl>=0;xl--)
			{
				P_UnwrapGeneric(P_Element(P,xl,y),StepCountL,LastZL);
			}
		}
	}
	//deplie colonne centrale, centre vers bord superieur
	//int StepCount=0;
	LastZ=P_Element(P,xcentre,ycentre);
	StepCount=0;
	for(y=ycentre-1;y>=0;y--)
	{
		P_UnwrapGeneric(P_Element(P,xcentre,y),StepCount,LastZ);

		//deplie ligne, centre vers bord droit
		{
//			int StepCountL=0;
			float LastZL=LastZ;
			int StepCountL=StepCount;
			for(int xl=xcentre+1;xl<P_SizeX(P);xl++)
			{
				P_UnwrapGeneric(P_Element(P,xl,y),StepCountL,LastZL);
			}
		}
		//deplie ligne, centre vers bord gauche
		{
//			int StepCountL=0;
			float LastZL=LastZ;
			int StepCountL=StepCount;
			for(int xl=xcentre-1;xl>=0;xl--)
			{
				P_UnwrapGeneric(P_Element(P,xl,y),StepCountL,LastZL);
			}
		}
	}

	return;
}


//-1=invalide sinon renvoie y
int SPG_CONV P_FindYMaxAndErase(Profil& PW, int x, int& yFound, float& yNorm, int& yWidth)
{
	CHECK(P_Etat(PW)==0,"P_FindYMaxAndErase",return -1);
	CHECK(!V_IsBound(x,0,P_SizeX(PW)),"P_FindYMaxAndErase",return -1);

	float* W=P_Data(PW)+x;

	yFound=-1;
	yWidth=0;
	yNorm=0;
	//trouve le maximum absolu
	for(int y=0;y<P_SizeY(PW);y++)
	{
		if(W[y*P_SizeX(PW)]>yNorm)
		{
			yNorm=W[y*P_SizeX(PW)];
			yFound=y;
		}
	}

	if(yFound<0) 
	{
		return -1;
	}

	yWidth=1;

	//supprime l'intégralité de l'épaisseur de la ligne  (jusqu'au prochain minimum local)
	int yErase;
	for(yErase=yFound+1;yErase<P_SizeY(PW)-1;yErase++)
	{
		if(
			W[(yErase+1)*P_SizeX(PW)]<W[(yErase)*P_SizeX(PW)]) 
		{
			W[(yErase)*P_SizeX(PW)]=0;
			yWidth++;
		}
		else
			break;
	}
	for(yErase=yFound-1;yErase>0;yErase--)
	{
		if(
			W[(yErase-1)*P_SizeX(PW)]<W[(yErase)*P_SizeX(PW)]) 
		{
			W[(yErase)*P_SizeX(PW)]=0;
			yWidth++;
		}
		else
			break;
	}
	W[yFound*P_SizeX(PW)]=0;

	return yFound;
}

float SPG_CONV P_GetOffset(Profil& P)
{
	CHECK(P_Etat(P)==0,"P_GetOffset: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_GetOffset: Profil vide",return 0);
	float Sum=0;
	if(P_Msk(P)==0)
	{
		for(int i=0;i<P_SizeY(P);i++)
		{//précision numérique
			float* Src=P_Data(P)+i*P_SizeX(P);
			float xSum=0;
			for(int j=0;j<P_SizeX(P);j++)
			{
				xSum+=Src[j];
			}
			Sum+=xSum;
		}
		Sum/=P_SizeX(P)*P_SizeY(P);
	}
	else
	{
		int Count=0;
		for(int i=0;i<P_SizeY(P);i++)
		{//précision numérique
			float* Src=P_Data(P)+i*P_SizeX(P);
			BYTE* SrcM=P_Msk(P)+i*P_SizeX(P);
			float xSum=0;
			for(int j=0;j<P_SizeX(P);j++)
			{
				if(SrcM[j]==0)
				{
					xSum+=Src[j];
					Count++;
				}
			}
			Sum+=xSum;
		}
		Sum/=V_Max(Count,1);
	}
	return Sum;
}

float SPG_CONV P_RemoveOffset(Profil& P)
{
	CHECK(P_Etat(P)==0,"P_RemoveOffset: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_RemoveOffset: Profil vide",return 0);
	float Sum=P_GetOffset(P);
	if(P_Msk(P)==0)
	{
		for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
		{
			P_Data(P)[i]-=Sum;
		}
	}
	else
	{
		for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
		{
			P_Data(P)[i]-=Sum;
			if(P_Msk(P)[i]) P_Data(P)[i]=0;
		}
	}
	return Sum;
}


int SPG_CONV P_Apodise(Profil& P, int BorderSize, int Iter)
{
	CHECK(P_Etat(P)==0,"P_Apodise",return 0);

	for(int i=0;i<Iter;i++)
	{
		P_RemoveOffset(P);
		for(int y=0;y<P_SizeY(P);y++)
		{
			for(int x=0;x<P_SizeX(P);x++)
			{
				int Dx=V_Min(x,P_SizeX(P)-1-x);
				int Dy=V_Min(y,P_SizeY(P)-1-y);
				int D=V_Min(Dx,Dy);	if(D>BorderSize) D=BorderSize;
				P_Data(P)[x+y*P_SizeX(P)]*=pow((float)D/(float)BorderSize,1.0f/Iter);
			}
		}
	}
	return -1;
}


void SPG_CONV P_GetTilt(Profil& P, float& TiltX, float& TiltY)
{//attention ne retourne que la moitié du tilt (convention de calcul du moment D=2*x-xmoy)
	TiltX=0;
	TiltY=0;
	CHECK(P_Etat(P)==0,"P_GetTilt: Profil nul",return);
	CHECK(P_Data(P)==0,"P_GetTilt: Profil vide",return);
	float* D=P_Data(P);
	for(int y=0;y<P_SizeY(P);y++)
	{
		float TiltX_XSum=0;
		float TiltY_XSum=0;
		int YMoment=(2*y-P_SizeY(P));
		for(int x=0;x<P_SizeX(P);x++)
		{
			TiltX_XSum+=D[x]*(2*x-P_SizeX(P));
			TiltY_XSum+=D[x]*YMoment;
		}
		TiltX+=TiltX_XSum;
		TiltY+=TiltY_XSum;
		D+=P_SizeX(P);
	}
	TiltX=3*TiltX/(P_SizeX(P)*P_SizeX(P)*P_SizeX(P)*P_SizeY(P));
	TiltY=3*TiltY/(P_SizeX(P)*P_SizeY(P)*P_SizeY(P)*P_SizeY(P));
	return;
}

void SPG_CONV P_RemoveTilt(Profil& P)
{
	CHECK(P_Etat(P)==0,"P_GetTilt: Profil nul",return);
	CHECK(P_Data(P)==0,"P_GetTilt: Profil vide",return);
	float TiltX,TiltY;
	P_GetTilt(P,TiltX,TiltY);
	float* D=P_Data(P);
	for(int y=0;y<P_SizeY(P);y++)
	{
		float YMoment=TiltY*(2*y-P_SizeY(P));
		for(int x=0;x<P_SizeX(P);x++)
		{
			D[x]-=(2*x-P_SizeX(P))*TiltX-YMoment;
		}
		D+=P_SizeX(P);
	}
	return;
}

#ifdef SPG_General_USEHIST

void SPG_CONV P_GetSurfaceTiltEstimate(Profil& P, float& TiltX, float& TiltY, int NumFilt)
{//calcule le tilt en X et Y
	CHECK(P_Etat(P)==0,"P_GetSurfaceTiltEstimate: Profil nul",return);
	CHECK(P_Data(P)==0,"P_GetSurfaceTiltEstimate: Profil vide",return);
	
	Profil PFilt,PdX,PdY;

//duplique le profil P
	P_Dupliquate(PFilt,P);
	P_Create(PdX,P_SizeX(P),P_SizeY(P));//tableaux des pentes X et Y
	P_Create(PdY,P_SizeX(P),P_SizeY(P));//la derniere ligne/colonne ne sera jamais utilisée

//PFilt est la version deux fois filtrée (moyenne 3x3) de P
	for(int i=0;i<NumFilt;i++)
	{
		P_MaskConv3x3(PFilt);
	}

//histogramme 2D (tableau de DWORD ou de int, théoriquement il faut éviter les float)
	HIST2D_TYPE H;
	int HistSize=128;//taille X et Y de l'histogramme
	int Seuil=(P_SizeX(PFilt)-1)*(P_SizeY(PFilt)-1)*0.5;//seuil
	HIST2D_Init(H,HistSize,HistSize);//alloue

	float dMax=0;//valeur max de la pente
	int pLine=0;
	{for(int y=0;y<P_SizeY(PFilt)-1;y++)
	{//parcourt le profil PFilt et trouve la pente max (en valeur absolue)
		int Elt=pLine;
		for(int x=0;x<P_SizeX(PFilt)-1;x++)
		{//la derniere ligne/colonne n'est pas remplie par le calcul de pente
			float dx=P_Data(PFilt)[Elt+1]-P_Data(PFilt)[Elt];//pente x = p(x+1,y)-p(x,y)
			float dy=P_Data(PFilt)[Elt+P_SizeX(PFilt)]-P_Data(PFilt)[Elt];//pente y = p(x,y+1)-p(x,y)
			P_Data(PdX)[Elt]=dx;//ecrit la pente x et y dans les tableaux PdX et PdY (P_Data(P)=P.D)
			P_Data(PdY)[Elt]=dy;
			if(fabs(dx)>dMax) dMax=fabs(dx);//trouve le max en valeur absolue
			if(fabs(dy)>dMax) dMax=fabs(dy);
			Elt++;
		}
		pLine+=P_SizeX(PFilt);
	}}

	//baisse le seuil de pente dMax par pas de x0.5 jusqu'a ce que le nombre de points 
	//de pente inférieure à dMax devienne inférieur à Seuil
	float ZoomStep=2;
	int Count=0;
	do
	{
		Count=0;
		dMax/=ZoomStep;
		pLine=0;
		{for(int y=0;y<P_SizeY(PFilt)-1;y++)
		{//la dernière ligne/colonne n'est pas remplie par le calcul de pente
			int Elt=pLine;
			for(int x=0;x<P_SizeX(PFilt)-1;x++)
			{
				if((fabs(P_Data(PdX)[Elt])<dMax)&&(fabs(P_Data(PdY)[Elt])<dMax))
				{
					if((P_Data(PdX)[Elt]!=0)&&(P_Data(PdY)[Elt]!=0))
					{
						Count++;
					}
				}
				Elt++;
			}
			pLine+=P_SizeX(PFilt);
		}}
	} while(Count>Seuil);
	//remet le meme seuil qu'a l'itération antérieure
	dMax*=ZoomStep;

	pLine=0;
	{for(int y=0;y<P_SizeY(PFilt)-1;y++)
	{//la dernière ligne/colonne n'est pas utilisée
		int Elt=pLine;
		for(int x=0;x<P_SizeX(PFilt)-1;x++)
		{//ajoute tous les points dans l'histogramme 2D en prenant dMax comme échelle max en x et y
			int nx=HistSize*(0.5f*P_Data(PdX)[Elt]/dMax+0.5f);
			int ny=HistSize*(0.5f*P_Data(PdY)[Elt]/dMax+0.5f);
			if(
				V_IsBound(nx,0,(HistSize-1))&&
				V_IsBound(ny,0,(HistSize-1))&&
				((P_Msk(P)==0)||(P_Msk(P)[Elt]==0))
				)
			{//ajoute un dans la case nx,ny correspondant à la pente locale
				HIST2D_Add(H,nx,ny);
			}
			Elt++;
		}
		pLine+=P_SizeX(PFilt);
	}}

	//P_SaveToFile(PdX,"GradX.bmp");
	//P_SaveToFile(PdY,"GradY.bmp");
	float MeanX,MeanY;
	//calcule la valeur moyenne de l'histogramme
	HIST2D_Stats(H,MeanX,MeanY);
	{for(int i=0;i<2;i++)
	{//centre la valeur moyenne bien en face du sommet de l'histogramme
		HIST2D_UpdateMean(H,HistSize/6,MeanX,MeanY);
	}}

	P_Close(PFilt);
	P_Close(PdX);
	P_Close(PdY);
	HIST2D_Close(H);

	/*
	//affichage de l'histogramme pour débogage
	G_Ecran E;
	G_InitMemoryEcran(E,3,HistSize,HistSize);
	HIST2D_Draw(H,E);
	G_DrawPixel(E,MeanX,HistSize-1-MeanY,0x00ff00);
	G_SaveEcran(E,"P_RemoveTiltHist2D.bmp");
	G_CloseEcran(E);
	*/
	
	//calcule finalement la valeur de la pente 'moyenne'
	TiltX=0.5f*(2*MeanX-HistSize)*dMax/HistSize;
	TiltY=0.5f*(2*MeanY-HistSize)*dMax/HistSize;
	return;
}

void SPG_CONV P_RemoveSurfaceTilt(Profil& P, int NumFilt)
{
	CHECK(P_Etat(P)==0,"P_GetTilt: Profil nul",return);
	CHECK(P_Data(P)==0,"P_GetTilt: Profil vide",return);
	float TiltX,TiltY;
	for(int i=0;i<3;i++)
	{//itère 6 fois car ce n'est par parfait du premier coup quand le tilt initial est tres grand
		P_GetSurfaceTiltEstimate(P,TiltX,TiltY, NumFilt);//calcule le tilt
		float* D=P_Data(P);
		float XCenter=P_SizeX(P)*TiltX;
		float DTiltX=2*TiltX;
		for(int y=0;y<P_SizeY(P);y++)
		{//soustrait la pente au profil
			float YMoment=TiltY*(2*y-P_SizeY(P));
			float XYSum=-XCenter+YMoment;
			for(int x=0;x<P_SizeX(P);x++)
			{
				//D[x]-=(2*x-P_SizeX(P))*TiltX+YMoment;
				D[x]-=x*DTiltX+XYSum;
			}
			D+=P_SizeX(P);
		}
	}
	return;
}

int SPG_CONV P_GetHist(DWORD* HistData, int HistSize, Profil& P, float& fMin, float& fStep)
{//histogramme des hauteurs normal
	fMin=0;
	fStep=1;
	CHECK(P_Etat(P)==0,"P_GetHist: Profil nul",return 0);
	CHECKPOINTER_L(HistData,HistSize*sizeof(DWORD),"P_GetHist",return 0);
	SPG_Memset(HistData,0,HistSize*sizeof(DWORD));
	float fMax;
	P_FindMinMax(P,fMin,fMax);
	fStep=(fMax-fMin)/HistSize;
	float idf=1/fStep;
	for(int i=0;i<P_SizeX(P)*P_SizeY(P);i++)
	{
		if((P_Msk(P)==0)||(P_Msk(P)[i]==0))
		{
			int n=(P_Data(P)[i]-fMin)*idf;
			n=V_Sature(n,0,HistSize-1);
			HistData[n]++;
		}
	}
	return -1;
}

int SPG_CONV P_GetWHist(DWORD* HistData, int HistSize, Profil& P, float& fMin, float& fStep)
{//histogramme des hauteurs pondéré
	fMin=0;
	fStep=1;
	CHECK(P_Etat(P)==0,"P_GetHist: Profil nul",return 0);
	CHECKPOINTER_L(HistData,HistSize*sizeof(DWORD),"P_GetHist",return 0);
	SPG_Memset(HistData,0,HistSize*sizeof(DWORD));
	float fMax;
	P_FindMinMax(P,fMin,fMax);
	fStep=(fMax-fMin)/HistSize;
	float idf=1/fStep;
	int R=V_Min(P_SizeX(P),P_SizeY(P))/4;
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(P_Msk(P)[x+y*P_SizeX(P)]==0)
			{
				int n=(P_Data(P)[x+y*P_SizeX(P)]-fMin)*idf;
				n=V_Sature(n,0,HistSize-1);
				int W=x;
				W=V_Min(W,(P_SizeX(P)-x));//poids=min(x,sx-x,y,sy-y)-(sx+sy)/6 (>0)
				W=V_Min(W,y);
				W=V_Min(W,(P_SizeY(P)-y));
				W-=R;
				W=V_Max(W,0);
				HistData[n]+=W;
			}
		}
	}
	return -1;
}

int SPG_CONV P_GetW2Hist(DWORD* HistData, int HistSize, Profil& P, float& fMin, float& fStep)
{//histogramme des hauteurs pondéré
	fMin=0;
	fStep=1;
	CHECK(P_Etat(P)==0,"P_GetHist: Profil nul",return 0);
	CHECKPOINTER_L(HistData,HistSize*sizeof(DWORD),"P_GetHist",return 0);
	SPG_Memset(HistData,0,HistSize*sizeof(DWORD));
	float fMax;
	P_FindMinMax(P,fMin,fMax);
	fStep=(fMax-fMin)/HistSize;
	float idf=1/fStep;
	int R=V_Min(P_SizeX(P),P_SizeY(P))/4;
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(P_Msk(P)[x+y*P_SizeX(P)]==0)
			{
				int n=(P_Data(P)[x+y*P_SizeX(P)]-fMin)*idf;
				n=V_Sature(n,0,HistSize-1);
				int W=x;
				W=V_Min(W,(P_SizeX(P)-x));//poids=min(x,sx-x,y,sy-y)-(sx+sy)/6 (>0)
				W=V_Min(W,y);
				W=V_Min(W,(P_SizeY(P)-y));
				W-=R;
				W*=V_Max(W,0);
				HistData[n]+=W;
			}
		}
	}
	return -1;
}

int SPG_CONV P_GetW3Hist(DWORD* HistData, int HistSize, Profil& P, float& fMin, float& fStep)
{//histogramme des hauteurs pondéré
	fMin=0;
	fStep=1;
	CHECK(P_Etat(P)==0,"P_GetHist: Profil nul",return 0);
	CHECKPOINTER_L(HistData,HistSize*sizeof(DWORD),"P_GetHist",return 0);
	SPG_Memset(HistData,0,HistSize*sizeof(DWORD));
	float fMax;
	P_FindMinMax(P,fMin,fMax);
	fStep=(fMax-fMin)/HistSize;
	float idf=1/fStep;
	int R=V_Min(P_SizeX(P),P_SizeY(P))/4;
	for(int y=0;y<P_SizeY(P);y++)
	{
		for(int x=0;x<P_SizeX(P);x++)
		{
			if(P_Msk(P)[x+y*P_SizeX(P)]==0)
			{
				int n=(P_Data(P)[x+y*P_SizeX(P)]-fMin)*idf;
				n=V_Sature(n,0,HistSize-1);
				int W=x;
				W=V_Min(W,(P_SizeX(P)-x));//poids=min(x,sx-x,y,sy-y)-(sx+sy)/6 (>0)
				W=V_Min(W,y);
				W=V_Min(W,(P_SizeY(P)-y));
				W-=R;
				W*=V_Max(W,0);
				W*=V_Max(W,0);
				HistData[n]+=W;
			}
		}
	}
	return -1;
}

int SPG_CONV P_GetStepHeight(Profil& P, float& S0, float& S1, int NumFilt, int TakeAll)
{
	CHECK(P_Etat(P)==0,"P_GetTilt: Profil nul",return 0);
	CHECK(P_Data(P)==0,"P_GetTilt: Profil vide",return 0);

	S1=S0=0;

	P_RemoveSurfaceTilt(P, NumFilt);

	int HistSize=2*(P_SizeX(P)+P_SizeY(P));
	HIST_TYPE H;
	HIST_Init(H,HistSize);
	float fMin,fStep;
	P_GetHist(H.Cumul,HistSize,P,fMin,fStep);
/*
	G_Ecran E;
	G_InitMemoryEcran(E,3,HistSize,HistSize);
	HIST_Draw(H,E);
	G_SaveEcran(E,"P_GetHist.bmp");
	G_CloseEcran(E);
*/

	int R=0;

	int Moyenne=0;
	int WSum=0;
	{for(int i=0;i<H.NumCater;i++)
	{
		Moyenne+=H.Cumul[i]*i;
		WSum+=H.Cumul[i];
	}}
	Moyenne/=WSum;
	int Mediane=(Moyenne+H.NumCater/2)/2;

	if(TakeAll)
	{//moyenne
		int WSum0=0;
		S0=0;
		{for(int i=0;i<Mediane;i++)
		{
			S0+=i*H.Cumul[i];
			WSum0+=H.Cumul[i];
		}}
		S0=(S0/WSum0)*fStep+fMin;
		
		int WSum1=0;
		S1=0;
		{for(int i=Mediane;i<HistSize;i++)
		{
			S1+=i*H.Cumul[i];
			WSum1+=H.Cumul[i];
		}}
		S1=(S1/WSum1)*fStep+fMin;
	}
	else
	{//peak + parafit
		int P0=HIST_LocalSearch(H,0,Mediane);
		int P1=HIST_LocalSearch(H,Mediane,HistSize-1);

		float a0=-1;	float a1=-1;
		float b0=0;		float b1=0;
		float c0=0;		float c1=0;
		//float D0=0;		float D1=0;

		if((P0>0)&&(P0<HistSize-1))
		{
			float zm=H.Cumul[P0-1];
			float zc=H.Cumul[P0];
			float zp=H.Cumul[P0+1];
			SPG_ParaFit3(zm,zc,zp,a0,b0,c0);
			if((a0<0)&&(c0>0)) 
			{
				S0=(P0-0.5f*b0/a0)*fStep+fMin;
				R=-1;
			}
		}
		else 
		{
				S0=P0*fStep+fMin;
				R=-1;
		}
		if((P1>0)&&(P1<HistSize-1))
		{
			float zm=H.Cumul[P1-1];
			float zc=H.Cumul[P1];
			float zp=H.Cumul[P1+1];
			SPG_ParaFit3(zm,zc,zp,a1,b1,c1);
			if((a1<0)&&(c1>0))
			{ 
				S1=(P1-0.5f*b1/a1)*fStep+fMin;
				R=-1;
			}
		}
		else
		{
				S1=P1*fStep+fMin;
				R=-1;
		}

	}
	
	HIST_Close(H);

/*
	Profil Bas;
	P_Dupliquate(Bas,P);
	P_MskThresholdMinMax(Bas,P,S0-D0,S0+D0,1,0xff);
	P_SaveToFile(P,"P_GetStepHeightBas.bmp");
	P_Close(Bas);

	Profil Haut;
	P_Dupliquate(Haut,P);
	P_MskThresholdMinMax(Haut,P,S1-D1,S0+D1,1,0xff);
	P_SaveToFile(P,"P_GetStepHeightHaut.bmp");
	P_Close(Haut);
*/
/*
	char Msg[512];
	sprintf(Msg,"Bin Count:%i P0:%i P1:%i Bin Size:%.4f S1:%.4f S2%.4f S2-S1:%.4f",HistSize,P0,P1,(fMax-fMin)/HistSize,S0,S1,(S1-S0));
	//SPG_List(Msg);
	MessageBox((HWND)Global.hWndWin,Msg,"Step Height",MB_OK);
*/
	return R;
}

#endif


void SPG_CONV P_ScrollLeft(Profil& P)
{
	CHECK(P_Etat(P)==0,"P_ScrollLeft: Profil nul",return);
	CHECK(P_Data(P)==0,"P_ScrollLeft: Profil vide",return);
	for(int y=0;y<P_SizeY(P);y++)
	{
		memmove(P_Pointeur(P,0,y),P_Pointeur(P,1,y),(P_SizeX(P)-1)*sizeof(float));
	}
}


#endif

