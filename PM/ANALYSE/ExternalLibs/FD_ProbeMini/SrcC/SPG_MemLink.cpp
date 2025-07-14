
#include "SPG_General.h"

#ifdef SPG_General_USEMELINK

#include "SPG_Includes.h"

#include <string.h>
#include <stdlib.h>

//codage différentiel ou remplacement
#define MELINK_MODE_MSK 		0x80
#define MELINK_MODE_SQUARE_REPLACE  0x00
#define MELINK_MODE_SQUARE_DIFF	0x80

//packing mode
#define MELINK_PACKING_MSK		0x7C
#define MELINK_FULL					0x00
#define MELINK_RGB_PACKED_4			0x04
#define MELINK_RGB_PACKED_5_S0		0x08
#define MELINK_RGB_PACKED_5_S1		0x0C
#define MELINK_RGB_PACKED_5_S2		0x10
#define MELINK_RGB_PACKED_5_S3		0x14
#define MELINK_RGB_PACKED_5_S4		0x18
#define MELINK_RGB_PACKED_5_S5		0x1C
#define MELINK_RGB_PACKED_5_S6		0x20

//Taille des blocs
#define MELINK_SQUARE_MSK		0x03
#define MELINK_SQUARE_INC		0x01
#define MELINK_SQUARE_MAX		0x03
#define MELINK_SQUARE_8				0x00
#define MELINK_SQUARE_16			0x01
#define MELINK_SQUARE_32			0x02
#define MELINK_SQUARE_64			0x03


#define MELINK_SizeFromMode(FlagSize) (8<<(FlagSize&MELINK_SQUARE_MSK))

#define RGB_PACK_4(B,P) P=((B[0]&0xC0)>>6)|((B[1]&0xC0)>>4)|((B[2]&0xC0)>>2)
#define RGB_UNPACK_4(B,P) {B[0]=((P<<6)&0xC0);B[1]=((P<<4)&0xC0);B[2]=((P<<2)&0xC0);B[0]|=(B[0]&0x40)>>1;B[1]|=(B[1]&0x40)>>1;B[2]|=(B[2]&0x40)>>1;}
//#define RGB_UNPACK_4(B,P) {RGB_TRAILING(B[0],P&3);RGB_TRAILING(B[1],(P>>2)&3);RGB_TRAILING(B[2],(P>>4)&3)}

//#define RGB_TRAILING(B,P) {BYTE RGB_TRAILING_LOCAL_MACRO=P;RGB_TRAILING_LOCAL_MACRO|=(RGB_TRAILING_LOCAL_MACRO<<2);B=(RGB_TRAILING_LOCAL_MACRO|(RGB_TRAILING_LOCAL_MACRO<<4));}


//#define RGB_PACK_5_S0(B0,B1,P) P=V_Sature((2+(int)B0[0]-B1[0]),0,4)+5*V_Sature((2+(int)B0[1]-B1[1]),0,4)+25*V_Sature((2+(int)B0[2]-B1[2]),0,4)
//#define RGB_PACK_5_SN(B0,B1,N,P) P=V_Sature((2+(int)(B0[0]>>N)-(B1[0]>>N)),0,4)+5*V_Sature((2+(int)(B0[1]>>N)-(B1[1]>>N)),0,4)+25*V_Sature((2+(int)(B0[2]>>N)-(B1[2]>>N)),0,4)
#define RGB_PACK_5_SN(B0,B1,N,P) {int Bias=2;if(N>0){Bias=(5<<(N-1));};int LOCAL_R1=(Bias+(int)(B0[0])-(int)(B1[0]))>>N;int LOCAL_V1=(Bias+(int)(B0[1])-(int)(B1[1]))>>N;int LOCAL_B1=(Bias+(int)(B0[2])-(int)(B1[2]))>>N;P=V_Sature(LOCAL_R1,0,4)+5*V_Sature(LOCAL_V1,0,4)+25*V_Sature(LOCAL_B1,0,4);}

//#define RGB_UNPACK_5_S0(B,P) B[0]+=(P%5)-2;B[1]+=((P/5)%5)-2;B[2]+=((P/25)%5)-2;
#define RGB_UNPACK_5_SN(B,N,P) {int LOCAL_R1=(int)B[0]+(((P%5)-2)<<N);int LOCAL_V1=(int)B[1]+((((P/5)%5)-2)<<N);int LOCAL_B1=(int)B[2]+((((P/25)%5)-2)<<N);B[0]=V_Sature(LOCAL_R1,0,255);B[1]=V_Sature(LOCAL_V1,0,255);B[2]=V_Sature(LOCAL_B1,0,255);}


int SPG_CONV MELINK_InitSend(MELINK_SEND& MSND, int Type, void* Mem, int Pitch, int PitchE, int SizeX, int SizeY)
{
	memset(&MSND,0,sizeof(MELINK_SEND));
	CHECK((MSND.Local.M=(BYTE*)Mem)==0,"MELINK_InitSend: Donnees nulles",return 0);
	MSND.Local.PitchE=PitchE;//distance entre deux elements consecutifs (octets)
	MSND.Local.Pitch=Pitch;//distance entre deux lignes (octets)
	CHECK(MELINK_InitFormat(MSND.MF,Type,SizeX,SizeY)==0,"MELINK_InitSend: MELINK_InitFormat echoue",return 0);
	//pour l'alias memoire on choisit le format le plus compact est
	/*
	MSND.Remote.PitchE=MSND.MF.SizeE;//distance entre deux elements consecutifs (octets)
	MSND.Remote.Pitch=MSND.Remote.PitchE*MSND.MF.SizeX;//distance entre deux lignes (octets)
	*/
	//mais ce mode n'est pas supporte par les fonctions de distance abs(Local-Remote)
	//donc on garde le meme format
	MSND.Remote.PitchE=MSND.Local.PitchE;//distance entre deux elements consecutifs (octets)
	MSND.Remote.Pitch=MSND.Local.Pitch;//distance entre deux lignes (octets)
	MSND.RemoteBackup.PitchE=MSND.Local.PitchE;//distance entre deux elements consecutifs (octets)
	MSND.RemoteBackup.Pitch=MSND.Local.Pitch;//distance entre deux lignes (octets)

	CHECK((MSND.Remote.M=SPG_TypeAlloc(MSND.Remote.Pitch*MSND.MF.SizeY,BYTE,"Remote memstate simule"))==0,"MELINK_InitSend: Allocation echouee",return 0);
	CHECK((MSND.RemoteBackup.M=SPG_TypeAlloc(MSND.Remote.Pitch*MSND.MF.SizeY,BYTE,"Remote memstate backup"))==0,"MELINK_InitSend: Allocation echouee",return 0);

	MSND.FlagSize=MELINK_SQUARE_MAX;//on demarre en taille maxi
	MSND.Downsampling=V_Min(8,MELINK_SizeFromMode(0));
	MSND.YNumber=MSND.XNumber=0;
	CHECK(MELINK_InitFormat(MSND.DWNS_MF,Type,SizeX/MSND.Downsampling,SizeY/MSND.Downsampling)==0,"MELINK_InitSend: MELINK_InitFormat echoue",return 0);

#ifdef DebugMelinkTimer
	S_InitTimer(MSND.Total,"Total");
	S_InitTimer(MSND.FindMaxDiffSquare,"FindMaxDiffSquare");
	S_InitTimer(MSND.ChooseEncodeMode,"ChooseEncodeMode");
	S_InitTimer(MSND.EncodeMsg,"EncodeMsg");
	S_InitTimer(MSND.SetRcvMsgInternal,"SetRcvMsgInternal");
	S_InitTimer(MSND.StartTime,"StartTime");
	S_StartTimer(MSND.StartTime);
#endif

	return MSND.Etat=MELINK_OK;
}

int SPG_CONV MELINK_InitFormat(MELINK_FORMAT& MF, int Type, int SizeX, int SizeY)
{
	CHECK(!V_InclusiveBound(Type,1,4),"MELINK_InitFormat: Type de donnees invalide",return 0);
	CHECK(SizeX<=0,"MELINK_InitFormat: Tailel invalide",return 0);
	CHECK(SizeY<=0,"MELINK_InitFormat: Tailel invalide",return 0);
	MF.Type=Type;
	MF.SizeX=SizeX;
	MF.SizeY=SizeY;
	switch(Type)
	{
	case MELINK_TYPE_RGB:
		MF.SizeE=3;
		break;
	case MELINK_TYPE_BYTE:
		MF.SizeE=1;
		break;
	case MELINK_TYPE_FLOAT:
		MF.SizeE=4;
		break;
	}
	return -1;
}

void SPG_CONV MELINK_CloseSend(MELINK_SEND& MSND)
{
#ifdef DebugMelinkTimer
	if(S_IsOK(MSND.Total))
	{
		S_PrintRatio(&MSND.Total,5);
		S_CloseTimer(MSND.Total);
		S_CloseTimer(MSND.FindMaxDiffSquare);
		S_CloseTimer(MSND.ChooseEncodeMode);
		S_CloseTimer(MSND.EncodeMsg);
		S_CloseTimer(MSND.SetRcvMsgInternal);
	S_StopTimer(MSND.StartTime);
	S_CloseTimer(MSND.StartTime);
	}
#endif
	if(MSND.Remote.M) SPG_MemFree(MSND.Remote.M);
	if(MSND.RemoteBackup.M) SPG_MemFree(MSND.RemoteBackup.M);
	memset(&MSND,0,sizeof(MELINK_SEND));
	return;
}

int SPG_CONV MELINK_InitRcv(MELINK_RCV& MRCV, int Type, void* Mem, int Pitch, int PitchE, int SizeX, int SizeY)
{
	memset(&MRCV,0,sizeof(MELINK_RCV));
	MRCV.Local.Pitch=Pitch;//distance entre deux lignes (octets)
	MRCV.Local.PitchE=PitchE;//distance entre deux elements consecutifs (octets)
	CHECK((MRCV.Local.M=(BYTE*)Mem)==0,"MELINK_InitRcv: Donnees nulles",return 0);
	CHECK(MELINK_InitFormat(MRCV.MF,Type,SizeX,SizeY)==0,"MELINK_InitRcv: MELINK_InitFormat echoue",return 0);
	BYTE* PLine=MRCV.Local.M;
	for(int y=0;y<MRCV.MF.SizeY;y++)
	{
		BYTE* PCol=PLine;
		for(int x=0;x<MRCV.MF.SizeX;x++)
		{
			memset(PCol,0,MRCV.MF.SizeE);
			PCol+=MRCV.Local.PitchE;
		}
		PLine+=MRCV.Local.Pitch;
	}

	return MRCV.Etat=MELINK_OK;
}

void SPG_CONV MELINK_CloseRcv(MELINK_RCV& MRCV)
{
	memset(&MRCV,0,sizeof(MELINK_RCV));
	return;
}

int SPG_CONV MELINK_FullFillSendMsg(MELINK_SEND& MSND,BYTE* Msg, int MaxLen)
{
	CHECK(MSND.Etat==0,"MELINK_FullFillSendMsg: MELINK_SEND non initialise",return 0);

	SPG_MemFastCheck();

	int Len=0;
	int OneLen;
	bool AlwaysInf=(MSND.FlagSize<MELINK_SQUARE_MAX);
	bool AllowDecLen=true;
	while(OneLen=MELINK_GetSendMsg(MSND,(MELINK_MSG*)(Msg+Len),MaxLen-Len,AllowDecLen))
	{
		SPG_MemFastCheck();

		AllowDecLen=false;
		if((4*OneLen)>MaxLen) AlwaysInf=false;
		Len+=OneLen;
	}
	//CHECK(AlwaysInf,"MELINK_FullFillSendMsg: Incrementation de la taille",MSND.FlagSize+=MELINK_SQUARE_INC);
	if(AlwaysInf&&(Len>0)) MSND.FlagSize+=MELINK_SQUARE_INC;
#ifdef DebugMelinkTimer
	float Ecoule;
	S_GetTimerRunningTime(MSND.StartTime,Ecoule);
	MSND.TotalSent+=Len;
	MSND.DebitMoyen=0.9f*MSND.DebitMoyen+0.1f*(float)MSND.TotalSent/Ecoule;
	if(Ecoule>2)
	{
		S_StopTimer(MSND.StartTime);
		S_StartTimer(MSND.StartTime);
		MSND.TotalSent=0;
	}
#endif
	return Len;
}

int SPG_CONV MELINK_GetSendMsg(MELINK_SEND& MSND,MELINK_MSG* Msg, int MaxLen, bool AllowDecLen)
{
	CHECK(MSND.Etat==0,"MELINK_FullFillSendMsg: MELINK_SEND non initialise",return 0);
	int Len=0;
	CHECK(MSND.Etat==0,"MELINK_GetSendMsg: MELINK_SND non initialise",return 0);
	CHECK(Msg==0,"MELINK_GetSendMsg: Message nul",return 0);

#ifdef DebugMelinkTimer
	S_StartTimer(MSND.Total);
#endif
	/*
	for(int FlagSize=MELINK_SQUARE_MAX;FlagSize>=0;FlagSize-=MELINK_SQUARE_INC)
	{
		int Size=MELINK_SizeFromMode(FlagSize);
		if(((int)sizeof(MELINK_MSG)+Size*Size*MSND.MF.SizeE)<MaxLen) break;
	}
	//CHECK(FlagSize<=0,"MELINK_GetSendMsg: Taille de buffer trop faible",return 0);
	if(FlagSize<0) return 0;
	*/
	SPG_MemFastCheck();

	//position d'origine de la recherche downsamplee
	if((++MSND.XNumber)==MSND.Downsampling)
	{
		MSND.XNumber=0;
		if((++MSND.YNumber)==MSND.Downsampling)
		{
			MSND.YNumber=0;
		}
	}

	MELINK_DATA MD0;
	MD0.M=MELINK_ElementPtr(MSND.Local,MSND.XNumber,MSND.YNumber);
	MD0.Pitch=MSND.Local.Pitch*MSND.Downsampling;
	MD0.PitchE=MSND.Local.PitchE*MSND.Downsampling;
	MELINK_DATA MD1;
	MD1.M=MELINK_ElementPtr(MSND.Remote,MSND.XNumber,MSND.YNumber);
	MD1.Pitch=MSND.Remote.Pitch*MSND.Downsampling;
	MD1.PitchE=MSND.Remote.PitchE*MSND.Downsampling;

	int x,y;
#ifdef DebugMelinkTimer
	S_StartTimer(MSND.FindMaxDiffSquare);
#endif
	if(MELINK_FindMaxDiffSquare(MSND.DWNS_MF,MD0,MD1,MELINK_SizeFromMode(MSND.FlagSize)/MSND.Downsampling,x,y))
	{
#ifdef DebugMelinkTimer
		S_StopTimer(MSND.FindMaxDiffSquare);
#endif
#ifdef DebugMelinkTimer
		S_StartTimer(MSND.ChooseEncodeMode);
#endif
		//version OK
		//int EncodeMode=MELINK_ChooseEncodeMode(MSND,MSND.Local,MSND.Remote,x*MSND.Downsampling,y*MSND.Downsampling,MSND.FlagSize);
		//version acceleree au detriment de la qualite
		int EncodeMode=MELINK_ChooseEncodeMode(MSND.MF.Type,MD0,MD1,x,y,MELINK_SizeFromMode(MSND.FlagSize)/MSND.Downsampling);
#ifdef DebugMelinkTimer
		S_StopTimer(MSND.ChooseEncodeMode);
#endif
		SPG_MemFastCheck();
#ifdef DebugMelinkTimer
		S_StartTimer(MSND.EncodeMsg);
#endif
		Len=MELINK_EncodeMsg(MSND,MSND.Local,MSND.Remote,Msg,x*MSND.Downsampling,y*MSND.Downsampling,EncodeMode|MSND.FlagSize,MaxLen-MELINK_HEADER_SIZE,AllowDecLen);
#ifdef DebugMelinkTimer
		S_StopTimer(MSND.EncodeMsg);
#endif
		SPG_MemFastCheck();
		if(Len==0) 
		{
#ifdef DebugMelinkTimer
			S_StopTimer(MSND.Total)
#endif
			return 0;
		}
	}
	else
	{
#ifdef DebugMelinkTimer
		S_StopTimer(MSND.FindMaxDiffSquare);
#endif
#ifdef DebugMelinkTimer
		S_StopTimer(MSND.Total)
#endif
		return 0;
	}

#ifdef DebugMelinkTimer
	S_StartTimer(MSND.SetRcvMsgInternal);
#endif
	MELINK_SetRcvMsgInternal(MSND.MF,MSND.Remote,Msg,Len);
#ifdef DebugMelinkTimer
	S_StopTimer(MSND.SetRcvMsgInternal);
#endif
#ifdef DebugMelinkTimer
	S_StopTimer(MSND.Total)
#endif
	return Len;
}

int SPG_CONV MELINK_FullSetRcvMsg(MELINK_RCV& MRCV,BYTE* Msg, int Len)
{
	CHECK(MRCV.Etat==0,"MELINK_FullSetRcvMsg: MELINK_RCV non initialise",return 0);
	int ReadLen=0;
	int OneLen;
	while((Len>0)&&(OneLen=MELINK_SetRcvMsg(MRCV,(MELINK_MSG*)(Msg+ReadLen),Len)))
	{
		ReadLen+=OneLen;
		Len-=OneLen;
	}
	return ReadLen;
}

int SPG_CONV MELINK_SetRcvMsg(MELINK_RCV& MRCV,MELINK_MSG* Msg,int Len)
{
	CHECK(MRCV.Etat==0,"MELINK_SetRcvMsg: MELINK_RCV non initialise",return 0);
	CHECK(Msg==0,"MELINK_SetRcvMsg: Message nul",return 0);
	CHECK(Len<sizeof(MELINK_MSG),"MELINK_SetRcvMsg: Message nul",return 0);
	return MELINK_SetRcvMsgInternal(MRCV.MF,MRCV.Local,Msg,Len-sizeof(MELINK_MSG));
}

#define GET_LINE_POINTER(MD,PLine,x0,y0) int PLine=MD.PitchE*x0+MD.Pitch*y0;
#define FOR_ALL_ELT_IN_SQUARE_Y(Size,yc,PLine,PCol) for(int yc=0;yc<Size;yc++){int PCol=PLine;
#define NEXT_ELT_IN_SQUARE_Y(MD,PLine) PLine+=MD.Pitch;}
#define FOR_ALL_ELT_IN_SQUARE_X(Size,xc) for(int xc=0;xc<Size;xc++){
#define NEXT_ELT_IN_SQUARE_X(MD,PCol) PCol+=MD.PitchE;}

#define FOR_ALL_ELT_IN_SQUARE(MD,Size,x0,y0,xc,yc,PLine,PCol) GET_LINE_POINTER(MD,PLine,x0,y0) FOR_ALL_ELT_IN_SQUARE_Y(Size,yc,PLine,PCol) FOR_ALL_ELT_IN_SQUARE_X(Size,xc)
#define NEXT_ELT_IN_SQUARE(MD,Size,PLine,PCol) NEXT_ELT_IN_SQUARE_X(MD,PCol) NEXT_ELT_IN_SQUARE_Y(MD,PLine)
#define ELT_PTR(MD,PCol) (MD.M+PCol)

#define GET_LINE_POINTER_FAST(MD,PLine,x0,y0) BYTE* PLine=MD.M+MD.PitchE*x0+MD.Pitch*y0;
#define FOR_ALL_ELT_IN_SQUARE_Y_FAST(Size,yc,PLine,PCol) for(int yc=0;yc<Size;yc++){BYTE* PCol=PLine;
#define NEXT_ELT_IN_SQUARE_Y_FAST(MD,PLine) PLine+=MD.Pitch;}
#define FOR_ALL_ELT_IN_SQUARE_X_FAST(Size,xc) for(int xc=0;xc<Size;xc++){
#define NEXT_ELT_IN_SQUARE_X_FAST(MD,PCol) PCol+=MD.PitchE;}

#define FOR_ALL_ELT_IN_SQUARE_FAST(MD,Size,x0,y0,xc,yc,PLine,PCol) GET_LINE_POINTER_FAST(MD,PLine,x0,y0) FOR_ALL_ELT_IN_SQUARE_Y_FAST(Size,yc,PLine,PCol) FOR_ALL_ELT_IN_SQUARE_X_FAST(Size,xc)
#define NEXT_ELT_IN_SQUARE_FAST(MD,Size,PLine,PCol) NEXT_ELT_IN_SQUARE_X_FAST(MD,PCol) NEXT_ELT_IN_SQUARE_Y_FAST(MD,PLine)
#define ELT_PTR_FAST(MD,PCol) (PCol)

#define FOR_ALL_SQUARE(MF,Size,x0,y0) for(int y0=0;(y0+Size)<=MF.SizeY;y0+=Size){for(int x0=0;(x0+Size)<=MF.SizeX;x0+=Size){
#define NEXT_SQUARE() } }

void SPG_CONV MELINK_GetStatsRGB(MELINK_DATA& MD0, MELINK_DATA& MD1, int x, int y, int Size, MELINK_STATS_RGB& MSTATS)
{
	MSTATS.DifferentialError.Max=0;
	FOR_ALL_ELT_IN_SQUARE(MD0,Size,x,y,xc,yc,PLine,PCol)
	int D=abs((int)ELT_PTR(MD0,PCol)[0]-ELT_PTR(MD1,PCol)[0]);
	if(D>MSTATS.DifferentialError.Max) MSTATS.DifferentialError.Max=D;
	D=abs((int)ELT_PTR(MD0,PCol)[1]-ELT_PTR(MD1,PCol)[1]);
	if(D>MSTATS.DifferentialError.Max) MSTATS.DifferentialError.Max=D;
	D=abs((int)ELT_PTR(MD0,PCol)[2]-ELT_PTR(MD1,PCol)[2]);
	if(D>MSTATS.DifferentialError.Max) MSTATS.DifferentialError.Max=D;
	NEXT_ELT_IN_SQUARE(MD0,Size,PLine,PCol)
	return;
}

int SPG_CONV MELINK_ChooseEncodeMode(int Type, MELINK_DATA& MD0, MELINK_DATA& MD1, int x, int y, int Size)
{
	//int Size=MELINK_SizeFromMode(FlagSize);

	//return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S4|FlagSize;

	switch(Type)
	{
	case MELINK_TYPE_RGB:
		{
		MELINK_STATS_RGB MSTATS;
		MELINK_GetStatsRGB(MD0,MD1,x,y,Size,MSTATS);
			//return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S5;//5;
		if(MSTATS.DifferentialError.Max<3)
		{
			return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S0;
		}
		else if(MSTATS.DifferentialError.Max<6)
		{
			return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S1;
		}
		else if(MSTATS.DifferentialError.Max<12)
		{
			return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S2;
		}
		else if(MSTATS.DifferentialError.Max<24)
		{
			return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S3;
		}
		else if(MSTATS.DifferentialError.Max<48)
		{
			return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S4;
		}
		else if(MSTATS.DifferentialError.Max<96)
		{
			return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S5;
		}
		/*
		else if(MSTATS.DifferentialError.Max<=192)
		{
			return MELINK_MODE_SQUARE_DIFF|MELINK_RGB_PACKED_5_S6;
		}
		*/
		else
		{
			return MELINK_MODE_SQUARE_REPLACE|MELINK_RGB_PACKED_4;
		}
		}
	default:
		return MELINK_MODE_SQUARE_REPLACE;
	}
}

//#define MELINK_CheckForMsgLen(Len,Max,AllowDecLen) CHECK(Len>Max,"MELINK_EncodeMsg: Taille max atteinte", {{if((MSND.FlagSize>0)&&(AllowDecLen)) MSND.FlagSize-=MELINK_SQUARE_INC;} return 0);}
#define MELINK_CheckForMsgLen(Len,Max,AllowDecLen) 	SPG_MemFastCheck();if(Len>Max) {{if((MSND.FlagSize>0)&&(AllowDecLen)) MSND.FlagSize-=MELINK_SQUARE_INC;} return 0;}

int SPG_CONV MELINK_EncodeMsg(MELINK_SEND& MSND, MELINK_DATA& MD0, MELINK_DATA& MD1, MELINK_MSG* Msg, int x, int y, int EncodeMode, int MaxMsgLen, bool AllowDecLen)
{
	if(MaxMsgLen<=0) return 0;
	int Size=MELINK_SizeFromMode(EncodeMode);
	Msg->Mode=EncodeMode;
	Msg->PosX=x/Size;
	Msg->PosY=y/Size;
	SPG_MemFastCheck();

	switch((Msg->Mode)&MELINK_MODE_MSK)
	{
	case MELINK_MODE_SQUARE_REPLACE:
		{
			switch(Msg->Mode&MELINK_PACKING_MSK)
			{
			case MELINK_FULL://marche dans tous les MELINK_TYPE
				{
					int Len=0;
					FOR_ALL_ELT_IN_SQUARE_FAST(MD0,Size,x,y,xc,yc,PLine,PCol)
					MELINK_CheckForMsgLen(Len+MSND.MF.SizeE,MaxMsgLen,AllowDecLen)
					memcpy(Msg->M+Len,ELT_PTR_FAST(MD0,PCol),MSND.MF.SizeE);
					Len+=MSND.MF.SizeE;
					NEXT_ELT_IN_SQUARE_FAST(MD0,Size,PLine,PCol)
					SPG_MemFastCheck();
					return Len+sizeof(MELINK_MSG);
				}
			case MELINK_RGB_PACKED_4://ne marche qu'en RGB, sinon unsuported
				{
					int Len=0;
					int Count=0;
					BYTE CodingRP4;
					FOR_ALL_ELT_IN_SQUARE_FAST(MD0,Size,x,y,xc,yc,PLine,PCol)
					BYTE RP4;
					RGB_PACK_4(ELT_PTR_FAST(MD0,PCol),RP4);
					if(Count==0)
					{
						Count++;
						CodingRP4=RP4;
					}
					else if(RP4==CodingRP4)
					{
						if((++Count)==4)
						{
							CodingRP4|=((Count-1)<<6);
					MELINK_CheckForMsgLen(Len+1,MaxMsgLen,AllowDecLen)
							Msg->M[Len++]=CodingRP4;
							Count=0;
						}
					}
					else
					{
						CodingRP4|=((Count-1)<<6);
					MELINK_CheckForMsgLen(Len+1,MaxMsgLen,AllowDecLen)
						Msg->M[Len++]=CodingRP4;
						Count=1;
						CodingRP4=RP4;
					}
					NEXT_ELT_IN_SQUARE_FAST(MD0,Size,PLine,PCol)
					if(Count)
					{
						CodingRP4|=((Count-1)<<6);
					MELINK_CheckForMsgLen(Len+1,MaxMsgLen,AllowDecLen)
						Msg->M[Len++]=CodingRP4;
						Count=0;
					}
					SPG_MemFastCheck();
					return Len+sizeof(MELINK_MSG);
				}
			}
		}
	case MELINK_MODE_SQUARE_DIFF:
		{
			switch(Msg->Mode&MELINK_PACKING_MSK)
			{
#define NPACK 0
case MELINK_RGB_PACKED_5_S0://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBP5SN.h"
#define NPACK 1
case MELINK_RGB_PACKED_5_S1://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBP5SN.h"
#define NPACK 2
case MELINK_RGB_PACKED_5_S2://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBP5SN.h"
#define NPACK 3
case MELINK_RGB_PACKED_5_S3://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBP5SN.h"
#define NPACK 4
case MELINK_RGB_PACKED_5_S4://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBP5SN.h"
#define NPACK 5
case MELINK_RGB_PACKED_5_S5://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBP5SN.h"
#define NPACK 6
case MELINK_RGB_PACKED_5_S6://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBP5SN.h"
			}
		}
		//unsoported
	//case MELINK_MODE_SQUARE_UNIFORM:
		//unsuported
	}
	SPG_MemFastCheck();
	return 0;
}

int SPG_CONV MELINK_SetRcvMsgInternal(MELINK_FORMAT& MF, MELINK_DATA& MD0, MELINK_MSG* Msg, int Len)
{
	CHECK(Msg==0,"MELINK_SetRcvMsgInternal: Message nul",return 0);
	CHECK(Len==0,"MELINK_SetRcvMsgInternal: Message nul",return 0);

	int Size=MELINK_SizeFromMode(Msg->Mode);
	switch(Msg->Mode&MELINK_MODE_MSK)
	{
	case MELINK_MODE_SQUARE_REPLACE:
		{
			switch(Msg->Mode&MELINK_PACKING_MSK)
			{
			case MELINK_FULL://marche dans tous les MELINK_TYPE
				{
					BYTE* MsgM=Msg->M;
					FOR_ALL_ELT_IN_SQUARE_FAST(MD0,Size,Msg->PosX*Size,Msg->PosY*Size,xc,yc,PLine,PCol)
					memcpy(ELT_PTR_FAST(MD0,PCol),MsgM,MF.SizeE);
					MsgM+=MF.SizeE;
					NEXT_ELT_IN_SQUARE_FAST(MD0,Size,PLine,PCol)
					return MsgM-Msg->M+sizeof(MELINK_MSG);
				}
			case MELINK_RGB_PACKED_4://ne marche qu'en RGB, sinon unsuported
				{
					int Count=0;
					BYTE* MsgM=Msg->M;
					BYTE CodingRP4;
					FOR_ALL_ELT_IN_SQUARE_FAST(MD0,Size,Msg->PosX*Size,Msg->PosY*Size,xc,yc,PLine,PCol)
					if(Count==0)
					{
						CodingRP4=*MsgM++;
						Count=(CodingRP4>>6)+1;
					}
					RGB_UNPACK_4(ELT_PTR_FAST(MD0,PCol),CodingRP4);
					Count--;
					NEXT_ELT_IN_SQUARE_FAST(MD0,Size,PLine,PCol)
					return MsgM-Msg->M+sizeof(MELINK_MSG);
				}
			}
		}
	case MELINK_MODE_SQUARE_DIFF:
		{
			switch(Msg->Mode&MELINK_PACKING_MSK)
			{
#define NPACK 0
			case MELINK_RGB_PACKED_5_S0://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBUP5SN.h"
#define NPACK 1
			case MELINK_RGB_PACKED_5_S1://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBUP5SN.h"
#define NPACK 2
			case MELINK_RGB_PACKED_5_S2://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBUP5SN.h"
#define NPACK 3
			case MELINK_RGB_PACKED_5_S3://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBUP5SN.h"
#define NPACK 4
			case MELINK_RGB_PACKED_5_S4://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBUP5SN.h"
#define NPACK 5
			case MELINK_RGB_PACKED_5_S5://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBUP5SN.h"
#define NPACK 6
			case MELINK_RGB_PACKED_5_S6://ne marche qu'en RGB, sinon unsuported
#include "SPG_Inline_RGBUP5SN.h"
			/*
			case MELINK_RGB_PACKED_5_S4://ne marche qu'en RGB, sinon unsuported
				{
					int Len=0;
					int Count=0;
					BYTE CodingRP5;
					FOR_ALL_ELT_IN_SQUARE(MD0,Size,Msg->PosX*Size,Msg->PosY*Size,xc,yc,PLine,PCol)
					if(Count==0)
					{
						CodingRP5=Msg->M[Len++];
						Count=(CodingRP5>>7)+1;
						CodingRP5&=0x7F;
					}
					RGB_UNPACK_5_SN(ELT_PTR(MD0,PCol),4,CodingRP5);
					Count--;
					NEXT_ELT_IN_SQUARE(MD0,Size,PLine,PCol)
					return Len+sizeof(MELINK_MSG);
				}
				*/
			}
		}
	}
	return 0;
}

int SPG_CONV MELINK_FindMaxDiffSquare(MELINK_FORMAT& MF, MELINK_DATA& MD0, MELINK_DATA& MD1, int Size, int& x, int& y)
{
	switch(MF.Type)
	{
	case MELINK_TYPE_RGB:
		{
	int MaxDist=0;
	FOR_ALL_SQUARE(MF,Size,x0,y0)
	int Dist=0;
	int PosDiff=MD1.M-MD0.M;
	FOR_ALL_ELT_IN_SQUARE_FAST(MD0,Size,x0,y0,xc,yc,PLine,PCol)
	Dist+=MELINK_DIST_RGB(ELT_PTR_FAST(MD0,PCol),ELT_PTR_FAST(MD1,PCol+PosDiff));
	NEXT_ELT_IN_SQUARE_FAST(MD0,Size,PLine,PCol)
	if(Dist>MaxDist) 
			{x=x0;y=y0;MaxDist=Dist;}
	else if(Dist==MaxDist)
	{
		if((x^y)&1)
			{x=x0;y=y0;MaxDist=Dist;}
	}
	NEXT_SQUARE()
	if(MaxDist==0) return 0;
		}
		break;
	case MELINK_TYPE_BYTE:
		{
	int MaxDist=0;
	FOR_ALL_SQUARE(MF,Size,x0,y0)
	int Dist=0;
	FOR_ALL_ELT_IN_SQUARE(MD0,Size,x0,y0,xc,yc,PLine,PCol)
	Dist+=MELINK_DIST_BYTE(ELT_PTR(MD0,PCol),ELT_PTR(MD1,PCol));
	NEXT_ELT_IN_SQUARE(MD0,Size,PLine,PCol)
	if(Dist>MaxDist) {x=x0;y=y0;MaxDist=Dist;};
	NEXT_SQUARE()
	if(MaxDist==0) return 0;
		}
		break;
	case MELINK_TYPE_FLOAT:
		{
	float MaxDist=0;
	FOR_ALL_SQUARE(MF,Size,x0,y0)
	float Dist=0;
	FOR_ALL_ELT_IN_SQUARE(MD0,Size,x0,y0,xc,yc,PLine,PCol)
	Dist+=MELINK_DIST_FLOAT(ELT_PTR(MD0,PCol),ELT_PTR(MD1,PCol));
	NEXT_ELT_IN_SQUARE(MD0,Size,PLine,PCol)
	if(Dist>MaxDist) {x=x0;y=y0;MaxDist=Dist;};
	NEXT_SQUARE()
	if(MaxDist==0) return 0;
		}
		break;
	}
	return -1;
}

#undef GET_LINE_POINTER
#undef FOR_ALL_ELT_IN_SQUARE_Y
#undef NEXT_ELT_IN_SQUARE_Y
#undef FOR_ALL_ELT_IN_SQUARE_X
#undef NEXT_ELT_IN_SQUARE_X

#undef FOR_ALL_ELT_IN_SQUARE
#undef NEXT_ELT_IN_SQUARE
#undef ELT_PTR

#undef FOR_ALL_SQUARE
#undef NEXT_SQUARE

#endif



