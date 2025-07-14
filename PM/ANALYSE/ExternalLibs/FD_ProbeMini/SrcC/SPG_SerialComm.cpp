

#include "SPG_General.h"

#ifdef SPG_General_USESerialComm

#include "SPG_Includes.h"
#include "SPG_SysInc.h"
#include <memory.h>
#include <stdio.h>

#ifdef DebugSerialComm
static void SPG_CONV SPG_LogCOM(SPG_SERIALCOMM& SSC, char Direction, BYTE* Buffer, int Len)
{
	char M[MaxProgDir];
	sprintf(M,"%c %i\t",Direction,Len);
	int L=strlen(M);
	for(int i=0;i<Len;i++)
	{
		if(V_InclusiveBound(Buffer[i],32,91)||V_InclusiveBound(Buffer[i],93,126))
		{
			M[L]=Buffer[i];
			M[L+1]=0;
			L++;
		}
		else
		{
			L+=sprintf(M+L,"[%i]",(int)Buffer[i]);
		}
	}
	strcpy(M+L,"\r\n");L+=2;
	fwrite(M,1,L,(FILE*)SSC.F);
	return;
}
#endif

void SPG_CONV CloseComm(SPG_SERIALCOMM& SSC)
{
	CHECK(SSC.Etat==0,"CloseComm",return);
	CloseHandle(SSC.hCom);
#ifdef DebugSerialComm
	fclose((FILE*)SSC.F);
#endif
	SPG_ZeroStruct(SSC);
	return;
}

/*
//
// DTR Control Flow Values.
//
#define DTR_CONTROL_DISABLE    0x00
#define DTR_CONTROL_ENABLE     0x01
#define DTR_CONTROL_HANDSHAKE  0x02

//
// RTS Control Flow Values
//
#define RTS_CONTROL_DISABLE    0x00
#define RTS_CONTROL_ENABLE     0x01
#define RTS_CONTROL_HANDSHAKE  0x02
#define RTS_CONTROL_TOGGLE     0x03

#define NOPARITY            0
#define ODDPARITY           1
#define EVENPARITY          2
#define MARKPARITY          3
#define SPACEPARITY         4

#define ONESTOPBIT          0
#define ONE5STOPBITS        1
#define TWOSTOPBITS         2
*/

int SPG_CONV OpenComm(SPG_SERIALCOMM& SSC,int Port,int Vitesse, int DTRControl, int RTSControl, int Parity, int StopBit)
{
	SPG_ZeroStruct(SSC);

	//char NomComm[5];
	//strcpy(NomComm,"COM1");
	//*((BYTE*)(NomComm)+3)=(BYTE)('1'+Port);

//	http://msdn.microsoft.com/library/default.asp?url=/library/en-us/fileio/fs/createfile.asp
//	To specify a COM port number greater than 9, use the following syntax: "\\\\.\\COM10". This syntax works for all port numbers and hardware that allows COM port numbers to be specified.
	char NomComm[32];
	sprintf(NomComm,"\\\\.\\COM%i",Port);

	SSC.hCom=CreateFile(NomComm,GENERIC_READ|GENERIC_WRITE,0,0,OPEN_EXISTING,0,NULL);

	if( ((int)SSC.hCom==-1)||((int)SSC.hCom==0) )
	{
#ifdef DebugList
		char Msg[256]; SPG_GetLastWinError(Msg,256); strcat(Msg,"\r\n"); strcat(Msg,NomComm); DbgCHECKTWO(1,"OpenComm:CreateFile failed",Msg);
#endif
		SPG_ZeroStruct(SSC);
		return 0;
	}

#ifdef DebugSerialComm
	char File[MaxProgDir];
	char Name[MaxProgDir];
	sprintf(Name,"Log_COM%i.txt",Port);
	SPG_ConcatPath(File,Global.LogDir,Name);
	SSC.F=fopen(File,"wb+");
#endif

	CHECK(SetupComm(SSC.hCom,2048,2048)==0,"OpenComm:SetupComm failed",CloseHandle(SSC.hCom);memset(&SSC,0,sizeof(SPG_SERIALCOMM));return 0;);	
	{
		DCB dcb;
		GetCommState(SSC.hCom,&dcb);
		dcb.BaudRate=Vitesse;
		dcb.fBinary=1;
		dcb.fParity=(Parity==NOPARITY?0:1);
		dcb.Parity=Parity;
		dcb.fOutxCtsFlow=0;
		dcb.fOutxDsrFlow=0;
		dcb.fDtrControl=DTRControl; //DTR_CONTROL_ENABLE;
		dcb.fDsrSensitivity=0;
		dcb.fTXContinueOnXoff=0;
		dcb.fOutX=0;
		dcb.fInX=0;
		dcb.fRtsControl=RTSControl; //RTS_CONTROL_HANDSHAKE précédemment utilisé; DISABLE nécessaire pour le LAAS;
		dcb.fAbortOnError=0;
		dcb.ByteSize=8;
		dcb.Parity=Parity;
		dcb.StopBits=StopBit;
		if(SetCommState(SSC.hCom,&dcb)==0)
		{
#ifdef DebugList
			char Msg[256]; SPG_GetLastWinError(Msg,256); DbgCHECKTWO(1,"OpenComm:SetCommState failed",Msg);
#endif
		}
	}
	{
		COMMTIMEOUTS ct;
		ct.ReadIntervalTimeout=MAXDWORD;
		ct.ReadTotalTimeoutMultiplier=0;
		ct.ReadTotalTimeoutConstant=0;
		ct.WriteTotalTimeoutMultiplier=5;
		ct.WriteTotalTimeoutConstant=5;
		SetCommTimeouts(SSC.hCom,&ct);
	}
	PurgeComm(SSC.hCom,PURGE_TXABORT|PURGE_RXABORT|PURGE_TXCLEAR|PURGE_RXCLEAR);
	return SSC.Etat=-1;
}


void SPG_CONV GetBuffState(SPG_SERIALCOMM& SSC, int& OctsIn, int& OctsOut)
{
	OctsOut=OctsIn=0;
	if(SSC.Etat==0) return;
	COMSTAT cst;
	DWORD CodeErreur;
	ClearCommError(SSC.hCom,&CodeErreur,&cst);
	OctsIn=cst.cbInQue;
	OctsOut=cst.cbOutQue;
	return;
}

//static int ReadCommDebug=0; //pour nourrir le programme en débogage

DWORD SPG_CONV ReadComm(SPG_SERIALCOMM& SSC, void* Buffer, int OctetsALire)
{
	if(SSC.Etat==0) return 0;
/*
#define CE_RXOVER           0x0001  // Receive Queue overflow
#define CE_OVERRUN          0x0002  // Receive Overrun Error
#define CE_RXPARITY         0x0004  // Receive Parity Error
#define CE_FRAME            0x0008  // Receive Framing error
#define CE_BREAK            0x0010  // Break Detected
#define CE_TXFULL           0x0100  // TX Queue is full
#define CE_PTO              0x0200  // LPTx Timeout
#define CE_IOE              0x0400  // LPTx I/O Error
#define CE_DNS              0x0800  // LPTx Device not selected
#define CE_OOP              0x1000  // LPTx Out-Of-Paper
#define CE_MODE             0x8000  // Requested mode unsupported
*/
	COMSTAT cst;
	DWORD CodeErreur;
	CHECK(ClearCommError(SSC.hCom,&CodeErreur,&cst)==0,"ReadComm",return 0);//ClearCommError failed
	DbgCHECK(CodeErreur,"ReadComm");
	DbgCHECK(CodeErreur&CE_RXOVER,"ReadComm: Receive Queue overflow");
	DbgCHECK(CodeErreur&CE_OVERRUN,"ReadComm: Receive Overrun Error");
	DbgCHECK(CodeErreur&CE_RXPARITY,"ReadComm: Receive Parity Error");
	DbgCHECK(CodeErreur&CE_FRAME,"ReadComm: Receive Framing error");
	DbgCHECK(CodeErreur&CE_BREAK,"ReadComm: Break Detected");
	DbgCHECK(CodeErreur&CE_TXFULL,"ReadComm: TX Queue is full");

	DWORD octetslus;
	ReadFile(SSC.hCom,Buffer,OctetsALire,&octetslus,NULL);

#ifdef DebugSerialComm
	if(octetslus) SPG_LogCOM(SSC, 'R', (BYTE*)Buffer, octetslus);
#endif

/*
	{
		switch(ReadCommDebug)
		{//RIA MMC reponse à une requete
		case 0:
			//Sleep(2000);
			break;
		case 10:
			memcpy(Buffer,"\x02" "103C620000E5000142020C8",octetslus=24);
			break;
		case 12:
			memcpy(Buffer,"0203F00A34380210",octetslus=16);
			break;
		case 14:
			memcpy(Buffer,"0000000B6" "\x03",octetslus=10);
			break;
		case 16:
			//memcpy(Buffer,"0000000B6" "\x03",octetslus=10);
			break;
		};
		Sleep(5);
		ReadCommDebug++;
	}
*/

	return octetslus;
}

DWORD SPG_CONV WriteComm(SPG_SERIALCOMM& SSC, void* Buffer, int OctetsAEcrire)
{
	if(SSC.Etat==0) return 0;
	CHECK(OctetsAEcrire==0,"WriteComm",return 0);
	COMSTAT cst;
	DWORD CodeErreur;
	CHECK(ClearCommError(SSC.hCom,&CodeErreur,&cst)==0,"WriteComm",return 0);//ClearCommError failed
	DbgCHECK(CodeErreur,"WriteComm");
	DbgCHECK(CodeErreur&CE_RXOVER,"WriteComm: Receive Queue overflow");
	DbgCHECK(CodeErreur&CE_OVERRUN,"WriteComm: Receive Overrun Error");
	DbgCHECK(CodeErreur&CE_RXPARITY,"WriteComm: Receive Parity Error");
	DbgCHECK(CodeErreur&CE_FRAME,"WriteComm: Receive Framing error");
	DbgCHECK(CodeErreur&CE_BREAK,"WriteComm: Break Detected");
	DbgCHECK(CodeErreur&CE_TXFULL,"WriteComm: TX Queue is full");

	DWORD octetsecrits=0;
	WriteFile(SSC.hCom,Buffer,OctetsAEcrire,&octetsecrits,0);
	
	if(octetsecrits==0)
	{
#ifdef DebugList
		char Msg[256]; SPG_GetLastWinError(Msg,256); DbgCHECKTWO(1,"WriteComm failed",Msg);
#endif
	}
#ifdef DebugSerialComm
	SPG_LogCOM(SSC, 'W', (BYTE*)Buffer, octetsecrits);
#endif
	return octetsecrits;
}

void SPG_CONV CommDTR(SPG_SERIALCOMM& SSC, int DTR_Level)
{
	if(SSC.Etat==0) return;
	DCB dcb;
	GetCommState(SSC.hCom,&dcb);
	dcb.fDtrControl=DTR_Level?DTR_CONTROL_ENABLE:DTR_CONTROL_DISABLE;
	if(SetCommState(SSC.hCom,&dcb)==0)
	{
#ifdef DebugList
		char Msg[256]; SPG_GetLastWinError(Msg,256); DbgCHECKTWO(1,"OpenComm:SetCommState failed",Msg);
#endif
	}
	return;
}

/**
 * Vide le buffer de reception du port serie.
 */
int SPG_CONV CommFlushRxBuffer(SPG_SERIALCOMM& SSC)
{
	PurgeComm(SSC.hCom,PURGE_TXABORT|PURGE_RXABORT|PURGE_TXCLEAR|PURGE_RXCLEAR);
	return 0;
}

#endif

