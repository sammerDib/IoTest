
#include "SPG_General.h"

#ifdef SPG_General_USEULIPS

#include "SPG_Includes.h"
#include <memory.h>
#include <string.h>
#include <stdio.h>

void SPG_CONV ULIPS_Combine(ULIPS_ListElement& L, ULIPS_ETAT Etat)
{
	if(L.Etat==ULIPS_ETAT_Normal)
	{
		L.Etat=Etat;
		L.PosInList=0;
	}
	else if(L.Etat==ULIPS_ETAT_QueryOne)
	{
		if(Etat==ULIPS_ETAT_StartQueryList)
		{
			L.Etat=ULIPS_ETAT_StartQueryList;
			L.PosInList=0;
		}
	}
	/*
	else if((L.Etat==ULIPS_ETAT_StartQueryList)||(L.Etat==ULIPS_ETAT_ContinueQueryList))
	{
	}
	*/
	return;
}

int SPG_CONV ULIPS_AddOrUpdate(ULIPS_List& L, ULIPS_Element& E, SPG_NET_ADDR& NetAddr, ULIPS_ETAT Etat)
{
	if(SPG_IsEqualNetAddr(ULocal(L).NetAddr,NetAddr)) return 0;
	if(SPG_IsLocalHost(ULocal(L).NetAddr,NetAddr)) return 0;
	if(E.LifeTime==1)
	{
		E.NetAddr=NetAddr;
	}
	for(int i=1;i<L.NumElement;i++)
	{
		//if(memcmp(&L.List[i].E,&E,sizeof(ULIPS_Element))==0)
		if(SPG_IsEqualNetAddr(L.List[i].E.NetAddr,E.NetAddr))
		{
			ULIPS_Combine(L.List[i],Etat);
			if(L.List[i].E.LifeTime>=E.LifeTime)
			{
				L.List[i].E=E;
				if(L.List[i].E.LifeTime==1) 
					L.List[i].LastReceive=L.CurTime;
				//else
				//	L.List[i].LastReceive=L.StartTime;
			}
			return i;
		}
	}
	if(i>=L.MaxElement) return -1;
	L.List[i].E=E;
	L.List[i].Etat=ULIPS_ETAT_Normal;
	ULIPS_Combine(L.List[i],Etat);
	L.List[i].LastReceive=L.CurTime;//evite qu'il soit directement trop vieux
	L.List[i].LastSend=L.StartTime;
	L.List[i].LastSendQueryList=L.StartTime;
	L.NumElement++;
	//ULIPS_Send(L,0,ULIPS_COMMANDE_QUERYLIST,L.List[i].E.NetAddr);
	return L.NumElement;
}

int SPG_CONV ULIPS_Del(ULIPS_List& L, SPG_NET_ADDR& NetAddr)
{
	for(int i=1;i<L.NumElement;i++)
	{
		//if(memcmp(&L.List[i].E,&E,sizeof(ULIPS_Element))==0)
		if(SPG_IsEqualNetAddr(L.List[i].E.NetAddr,NetAddr))
		{
			if(i<L.NumElement-1) memmove(&L.List[i],&L.List[i+1],(L.NumElement-i-1)*sizeof(ULIPS_ListElement));
			return --L.NumElement;
		}
	}
	return -1;
}

int SPG_CONV ULIPS_Del(ULIPS_List& L, ULIPS_Element& E)
{
	for(int i=1;i<L.NumElement;i++)
	{
		//if(memcmp(&L.List[i].E,&E,sizeof(ULIPS_Element))==0)
		if(SPG_IsEqualNetAddr(L.List[i].E.NetAddr,E.NetAddr))
		{
			if(i<L.NumElement-1) memmove(&L.List[i],&L.List[i+1],(L.NumElement-i-1)*sizeof(ULIPS_ListElement));
			return --L.NumElement;
		}
	}
	return -1;
}

int SPG_CONV ULIPS_Del(ULIPS_List& L, ULIPS_Element& E, SPG_NET_ADDR& NetAddr)
{
	if(E.LifeTime==1)
	{
		E.NetAddr=NetAddr;
	}
	for(int i=1;i<L.NumElement;i++)
	{
		//if(memcmp(&L.List[i].E,&E,sizeof(ULIPS_Element))==0)
		if(SPG_IsEqualNetAddr(L.List[i].E.NetAddr,E.NetAddr))
		{
			if(i<L.NumElement-1) memmove(&L.List[i],&L.List[i+1],(L.NumElement-i-1)*sizeof(ULIPS_ListElement));
			return --L.NumElement;
		}
	}
	return -1;
}

void SPG_CONV ULIPS_Send(ULIPS_List& L, int ListEntry, ULIPS_COMMANDE Commande, SPG_NET_ADDR& IP)
{
	ULIPS_Request R;
	R.Commande=Commande;
	R.E=L.List[ListEntry].E;
	R.E.LifeTime=V_Min((int)R.E.LifeTime+1,255);
	L.List[ListEntry].LastSend=L.CurTime;
	SPG_SendUDP(L.NET,IP,&R,sizeof(ULIPS_Request));
	return;
}

int SPG_CONV ULIPS_Read(ULIPS_List& L, int& Commande, ULIPS_Element& E, SPG_NET_ADDR& IP)
{
	ULIPS_Request R;
	if(SPG_ReadUDP(L.NET,IP,&R,sizeof(ULIPS_Request))==0) return 0;
	Commande=R.Commande;
	E=R.E;
	return -1;
}

void SPG_CONV ULIPS_Save(ULIPS_List& L, char* FileName)
{
	CHECK(L.Etat==0,"ULIPS_Save",return);
	char Msg[128];
	FILE*F=fopen(FileName,"wb+");
	if(F)
	{
		for(int i=1;i<L.NumElement;i++)
		{
			if((L.List[i].E.NetAddr.IP0!=127)||(L.List[i].E.NetAddr.IP1!=0)||(L.List[i].E.NetAddr.IP0!=0)||(L.List[i].E.NetAddr.IP0!=1))
			{
				sprintf(Msg,"%i.%i.%i.%i:%i\r\n",
					L.List[i].E.NetAddr.IP0,
					L.List[i].E.NetAddr.IP1,
					L.List[i].E.NetAddr.IP2,
					L.List[i].E.NetAddr.IP3,
					L.List[i].E.NetAddr.Port);
				fwrite(Msg,strlen(Msg),1,F);
			}
		}
		fclose(F);
	}
	return;
}

void SPG_CONV ULIPS_Load(ULIPS_List& L, char* FileName)
{
	CHECK(L.Etat==0,"ULIPS_Load",return);
	FILE*F=fopen(FileName,"rb");
	CHECKTWO(F==0,"ULIPS_Load: File not found",FileName,return);
	if(F)
	{
		char* Msg;
		char* MsgOrigin;
		fseek(F,0,SEEK_END);
		int FileLen=ftell(F);
		fseek(F,0,SEEK_SET);
		if(FileLen>1)
		{
			MsgOrigin=Msg=SPG_TypeAlloc(FileLen,char,"ULIPS_Load");
			fread(Msg,FileLen,1,F);
			char* i=0;
			while(i=SPG_StrFind(Msg,"\r\n"))
			{
				if(i!=Msg)
				{
				*i=0;
				SPG_NET_ADDR SNA;
				if(SPG_Resolve(SNA,Msg))
				{
					if((SNA.IP0!=127)||(SNA.IP1!=0)||(SNA.IP0!=0)||(SNA.IP0!=1))
					{
						ULIPS_Send(L,0,ULIPS_COMMANDE_QUERYLIST,SNA);
					}
				}
				Msg=i+2;
				}
				else
					break;
			}
			while(i=SPG_StrFind(Msg,"\n"))
			{
				if(i!=Msg)
				{
				*i=0;
				SPG_NET_ADDR SNA;
				if(SPG_Resolve(SNA,Msg))
				{
					if((SNA.IP0!=127)||(SNA.IP1!=0)||(SNA.IP0!=0)||(SNA.IP0!=1))
					{
						ULIPS_Send(L,0,ULIPS_COMMANDE_QUERYLIST,SNA);
					}
				}
				Msg=i+1;
				}
				else
					break;
			}
			SPG_MemFree(MsgOrigin);
		}
		fclose(F);
	}
	return;
}

int SPG_CONV ULIPS_StartNetUpdate(ULIPS_List& L, int ApplicationPort, int ULIPS_Port, int MaxElement)
{
	memset(&L,0,sizeof(ULIPS_List));
	if(SPG_InitUDP(L.NET,ULIPS_Port)==0)
	{
		CHECK(SPG_InitUDP(L.NET)==0,"ULIPS_StartNetUpdate",return 0);
	}
	L.MaxElement=MaxElement;
	S_InitTimer(L.T,"ULIPS_StartNetUpdate");
	L.List=SPG_TypeAlloc(L.MaxElement,ULIPS_ListElement,"ULIPS_StartNetUpdate");
	ULocal(L).State=ULIPS_STATE_NORMAL;
	ULocal(L).APP.ApplicationPort=ApplicationPort;
	strncpy(ULocal(L).ID.Name,L.NET.ServerName,ULIPS_NAME_SIZE);
	strncpy(ULocal(L).APP.AppName,Global.WClassName,ULIPS_NAME_SIZE);
	ULocal(L).NetAddr=L.NET.LocalNetAddr;
	L.List[0].Etat=ULIPS_ETAT_Normal;
	L.NumElement=1;
	float RFact=((float)((L.NET.LocalNetAddr.IP0+L.NET.LocalNetAddr.IP1+L.NET.LocalNetAddr.IP2+L.NET.LocalNetAddr.IP3)&255))/255.0f;
#ifdef DebugNetwork
	L.QueryListOldestPeriod=Global.CPUClock*20*(1+RFact);
	L.QueryOldestPeriod=Global.CPUClock*5*(1+RFact);
	L.DeleteOldestPeriod=Global.CPUClock*30*(1+RFact);
	L.SendListElementPeriod=(Global.CPUClock/4)*(1+RFact);
#else
	L.QueryListOldestPeriod=Global.CPUClock*20*(1+RFact);
	L.QueryOldestPeriod=Global.CPUClock*5*(1+RFact);
	L.DeleteOldestPeriod=Global.CPUClock*120*(1+RFact);
	L.SendListElementPeriod=(Global.CPUClock/4)*(1+RFact);
#endif
	S_StartTimer(L.T);
	S_GetTimerRunningCount(L.T,L.CurTime);
	L.StartTime=L.CurTime;
	return (L.Etat=-1);
}

int SPG_CONV ULIPS_UpdateQueryOldest(ULIPS_List& L)
{
	S_TimerCountType TLast=L.CurTime;
	int ilast=0;
	for(int i=1;i<L.NumElement;i++)
	{
		if(L.List[i].LastSend<TLast)
		{
			TLast=L.List[i].LastSend;
			ilast=i;
		}
	}
	if((ilast)&&(TLast<L.CurTime-L.QueryOldestPeriod))
	{
		ULIPS_Send(L,0,L.List[ilast].E.LifeTime>1?ULIPS_COMMANDE_QUERYLIST:ULIPS_COMMANDE_QUERYONE,L.List[ilast].E.NetAddr);
		if(L.List[ilast].E.LifeTime>1) L.List[ilast].LastSendQueryList=L.CurTime;
		L.List[ilast].LastSend=L.CurTime;
		return ilast;
	}
	return 0;
}

int SPG_CONV ULIPS_UpdateQueryListOldest(ULIPS_List& L)
{
	S_TimerCountType TLast=L.CurTime;
	int ilast=0;
	for(int i=1;i<L.NumElement;i++)
	{
		if(L.List[i].LastSendQueryList<TLast)
		{
			TLast=L.List[i].LastSendQueryList;
			ilast=i;
		}
	}
	if((ilast)&&(TLast<L.CurTime-L.QueryListOldestPeriod))
	{
		ULIPS_Send(L,0,ULIPS_COMMANDE_QUERYLIST,L.List[ilast].E.NetAddr);
		L.List[ilast].LastSend=L.List[ilast].LastSendQueryList=L.CurTime;
		return ilast;
	}
	return 0;
}

int SPG_CONV ULIPS_UpdateDeleteOldest(ULIPS_List& L)
{
	S_TimerCountType TLast=L.CurTime;
	int ilast=0;
	for(int i=1;i<L.NumElement;i++)
	{
		if(L.List[i].LastReceive<TLast)
		{
			TLast=L.List[i].LastReceive;
			ilast=i;
		}
	}
	if((ilast)&&(TLast<(L.CurTime-L.DeleteOldestPeriod)))
	{
		if(L.List[ilast].E.State&ULIPS_STATE_BUSY) 
		{
			L.List[i].LastReceive=L.CurTime;
		}
		else
		{
			ULIPS_Del(L,L.List[ilast].E);
			return -1;
		}
		//ULIPS_Send(L,0,ULIPS_COMMANDE_QUERYLIST,L.List[ilast].E.NetAddr);
		//return ilast;
	}
	return 0;
}

int SPG_CONV ULIPS_UpdateSendListElement(ULIPS_List& L)
{
	int Sent=0;
	S_TimerCountType TLast=L.CurTime;
	for(int i=1;i<L.NumElement;i++)
	{
		if(L.List[i].Etat==ULIPS_ETAT_StartQueryList)
		{
			L.List[i].Etat=ULIPS_ETAT_ContinueQueryList;
			L.List[i].PosInList=0;
		}
		if(L.List[i].Etat==ULIPS_ETAT_QueryOne) L.List[i].PosInList=0;
		if((L.List[i].Etat==ULIPS_ETAT_ContinueQueryList)||(L.List[i].Etat==ULIPS_ETAT_QueryOne))
		{
			if(L.List[i].PosInList==i) L.List[i].PosInList++;
			if(L.List[i].LastSend<L.CurTime-L.SendListElementPeriod)
			{
				if(L.List[i].PosInList<L.NumElement)
				{
					ULIPS_Send(L,L.List[i].PosInList,ULIPS_COMMANDE_ADD,L.List[i].E.NetAddr);
					Sent=-1;
					L.List[i].PosInList++;
					L.List[i].LastSend=L.CurTime;
				}
			}
		}
		if((L.List[i].PosInList>=L.NumElement)||(L.List[i].Etat==ULIPS_ETAT_QueryOne))
		{
			L.List[i].Etat=ULIPS_ETAT_Normal;
			L.List[i].PosInList=0;
		}
	}
	return Sent;
}

int SPG_CONV ULIPS_UpdateReadMsg(ULIPS_List& L)
{
	int Commande;
	ULIPS_Element E;
	SPG_NET_ADDR NetAddr;
	int MsgCount=0;
	while(ULIPS_Read(L,Commande,E,NetAddr))
	{
		MsgCount++;
		switch(Commande)
		{
			case ULIPS_COMMANDE_ADD:
				ULIPS_AddOrUpdate(L,E,NetAddr,ULIPS_ETAT_Normal);
				break;
			case ULIPS_COMMANDE_DEL:
				ULIPS_Del(L,E,NetAddr);
				break;
			case ULIPS_COMMANDE_QUERYLIST:
				if(ULocal(L).State&ULIPS_STATE_BUSY) break;
				ULIPS_AddOrUpdate(L,E,NetAddr,ULIPS_ETAT_StartQueryList);
				break;
			case ULIPS_COMMANDE_QUERYONE:
				ULIPS_AddOrUpdate(L,E,NetAddr,ULIPS_ETAT_QueryOne);
				break;
		}
	}
	return MsgCount;
}

int SPG_CONV ULIPS_UpdateNetUpdate(ULIPS_List& L)
{
	CHECK(L.Etat==0,"ULIPS_UpdateNetUpdate:ULIPS_List non initialisé",return 0);

	S_GetTimerRunningCount(L.T,L.CurTime);

	if(ULIPS_UpdateReadMsg(L)) return -1;

	if(ULocal(L).State&ULIPS_STATE_BUSY) return 0;

	if(ULIPS_UpdateQueryOldest(L)) return -1;//rafraichit les plus vieux

	if(ULIPS_UpdateQueryListOldest(L)) return -1;//rafraichit la liste

	if(ULIPS_UpdateSendListElement(L)) return -1;//envoie la liste

	if(ULIPS_UpdateDeleteOldest(L)) return -1;//enleve le plus vieux

	return 0;
}

void SPG_CONV ULIPS_StopNetUpdate(ULIPS_List& L)
{
	CHECK(L.Etat==0,"ULIPS_StopNetUpdate:ULIPS_List non initialisé",return);
	for(int i=1;i<L.NumElement;i++)
	{
		ULIPS_Send(L,0,ULIPS_COMMANDE_DEL,L.List[i].E.NetAddr);
	}
	SPG_MemFree(L.List);
	S_StopTimer(L.T);
	S_CloseTimer(L.T);
	SPG_CloseUDP(L.NET);
	return;
}

void SPG_CONV ULIPS_ChangeState(ULIPS_List& L, int State)
{
	ULocal(L).State=State;
	for(int i=1;i<L.NumElement;i++)
	{
		ULIPS_Send(L,0,ULIPS_COMMANDE_ADD,L.List[i].E.NetAddr);
		L.List[i].LastReceive=L.List[i].LastSend=L.CurTime;
	}
	return;
}

#endif

