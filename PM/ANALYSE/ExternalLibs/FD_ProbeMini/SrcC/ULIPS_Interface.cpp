
#include "SPG_General.h"

#ifdef SPG_General_USEULIPSINTERFACE

#include "SPG_Includes.h"

#include <memory.h>
#include <string.h>
#include <stdio.h>

int SPG_CONV ULIPS_InitInterface(ULIPS_Interface& UI, ULIPS_List& L, G_Ecran& E)
{
	memset(&UI,0,sizeof(ULIPS_Interface));
	CHECK(L.Etat==0,"ULIPS_InitInterface:ULIPS_List non initialisé",return 0);
	UI.L=&L;
	UI.E=E;
	C_LoadCaracLib(UI.CL,E);
	B_LoadButtonsLib(UI.BL,E,1);
	int CurrentX=2;
	int CurrentY=UI.CL.SpaceY;
	int BSpaceX=UI.BL.SizeX[ClickSprite]+UI.CL.SizeX;
	int BSpaceY=UI.BL.SizeY[ClickSprite]+UI.CL.SpaceY+2;

	UI.IP0=L.List[0].E.NetAddr.IP0;
	UI.IP1=L.List[0].E.NetAddr.IP1;
	UI.IP2=L.List[0].E.NetAddr.IP2;
	UI.IP3=L.List[0].E.NetAddr.IP3;
	UI.Port=L.List[0].E.NetAddr.Port;

	UI.bIP0=B_CreateCliquableNumericIntButton(UI.BL,E,UI.CL,CurrentX,CurrentY,3,&UI.IP0);
	CurrentX+=5*UI.CL.SizeX;
	UI.bIP1=B_CreateCliquableNumericIntButton(UI.BL,E,UI.CL,CurrentX,CurrentY,3,&UI.IP1);
	CurrentX+=5*UI.CL.SizeX;
	UI.bIP2=B_CreateCliquableNumericIntButton(UI.BL,E,UI.CL,CurrentX,CurrentY,3,&UI.IP2);
	CurrentX+=5*UI.CL.SizeX;
	UI.bIP3=B_CreateCliquableNumericIntButton(UI.BL,E,UI.CL,CurrentX,CurrentY,3,&UI.IP3);
	CurrentX+=5*UI.CL.SizeX;
	UI.bPort=B_CreateCliquableNumericIntButton(UI.BL,E,UI.CL,CurrentX,CurrentY,5,&UI.Port);
	CurrentX+=7*UI.CL.SizeX;
	UI.bQuery=B_CreateClickButton(UI.BL,E,CurrentX,CurrentY);
	B_PrintLabel(UI.BL,UI.bQuery,UI.CL,"Query");
	CurrentX+=2*BSpaceX;
	UI.bQueryList=B_CreateClickButton(UI.BL,E,CurrentX,CurrentY);
	B_PrintLabel(UI.BL,UI.bQueryList,UI.CL,"QueryList");
	CurrentX+=2*BSpaceX;
	UI.bDeleteMe=B_CreateClickButton(UI.BL,E,CurrentX,CurrentY);
	B_PrintLabel(UI.BL,UI.bDeleteMe,UI.CL,"DeleteMe");
	CurrentX+=2*BSpaceX;
	UI.bBusy=B_CreateCheckButton(UI.BL,E,CurrentX,CurrentY);
	B_PrintLabel(UI.BL,UI.bBusy,UI.CL,"Busy");
	CurrentX+=BSpaceX;
	/*
	UI.bQuit=B_CreateClickButton(UI.BL,E,CurrentX,CurrentY);
	B_PrintLabel(UI.BL,UI.bQuit,UI.CL,"Quit");
	CurrentX+=BSpaceX;
	*/
	B_RedrawButtonsLib(UI.BL);
	return UI.Etat=-1;
}

void SPG_CONV ULIPS_UpdateInterface(ULIPS_Interface& UI)
{
	B_UpdateButtonsLib(UI.BL,SPG_Global_MouseX,SPG_Global_MouseY,SPG_Global_MouseLeft);
	UI.EditNetAddr.IP0=UI.IP0;
	UI.EditNetAddr.IP1=UI.IP1;
	UI.EditNetAddr.IP2=UI.IP2;
	UI.EditNetAddr.IP3=UI.IP3;
	UI.EditNetAddr.Port=UI.Port;
	if(B_IsChangedToClick(UI.BL,UI.bQuery))
	{
		ULIPS_Send(*(UI.L),0,ULIPS_COMMANDE_QUERYONE,UI.EditNetAddr);
	}
	if(B_IsChangedToClick(UI.BL,UI.bQueryList))
	{
		ULIPS_Send(*(UI.L),0,ULIPS_COMMANDE_QUERYLIST,UI.EditNetAddr);
	}
	if(B_IsChangedToClick(UI.BL,UI.bDeleteMe))
	{
		ULIPS_Send(*(UI.L),0,ULIPS_COMMANDE_DEL,UI.EditNetAddr);
		ULIPS_Del(*(UI.L),UI.EditNetAddr);
	}
	if(B_IsChangedToClick(UI.BL,UI.bBusy))
	{
		ULIPS_ChangeState(*(UI.L),ULIPS_STATE_BUSY);
	}
	if(B_IsChangedToNotClick(UI.BL,UI.bBusy))
	{
		ULIPS_ChangeState(*(UI.L),ULIPS_STATE_NORMAL);
	}
	char Element[512];
	G_DrawRect(UI.E,0,UI.BL.SizeY[ClickSprite]+UI.CL.SpaceY+2,G_SizeX(UI.E),G_SizeY(UI.E),0xFFFFFF);
	int i=0;
	int y=UI.BL.SizeY[ClickSprite]+UI.CL.SpaceY+2;
	while(y<UI.E.SizeY)
	{
		if(ULIPS_PrintElement(*(UI.L),i,Element))
		{
			if(UI.L->List[i].E.LifeTime<=1)
			{
				if(UI.L->List[i].E.State&ULIPS_STATE_BUSY)
				{
					C_PrintUni(UI.E,0,y,Element,UI.CL,0,0x80808080);
				}
				else
				{
					C_Print(UI.E,0,y,Element,UI.CL);
				}
				y+=UI.CL.SpaceY;
			}
			i++;
		}
		else break;
	}
	return;
}

void SPG_CONV ULIPS_CloseInterface(ULIPS_Interface& UI)
{
	C_CloseCaracLib(UI.CL);
	B_CloseButtonsLib(UI.BL);
	memset(&UI,0,sizeof(ULIPS_Interface));
	return;
}

int SPG_CONV ULIPS_PrintElement(ULIPS_List& L, int ListEntry, char* Element)
{
	Element[0]=0;
	if(ListEntry==0)
	{
		sprintf(Element,"%i:%s %i.%i.%i.%i:%i App:%s",// Etat:%i",
			L.List[ListEntry].E.APP.ApplicationPort,
			L.List[ListEntry].E.ID.Name,
			L.List[ListEntry].E.NetAddr.IP0,
			L.List[ListEntry].E.NetAddr.IP1,
			L.List[ListEntry].E.NetAddr.IP2,
			L.List[ListEntry].E.NetAddr.IP3,
			L.List[ListEntry].E.NetAddr.Port,
			L.List[ListEntry].E.APP.AppName);
			//(int)L.List[ListEntry].E.State);
		return -1;
	}
	else if(V_IsBound(ListEntry,0,L.NumElement))
	{
		sprintf(Element,"%i:%s %i.%i.%i.%i:%i App:%s",// Etat:%i P%ims",
			L.List[ListEntry].E.APP.ApplicationPort,
			L.List[ListEntry].E.ID.Name,
			L.List[ListEntry].E.NetAddr.IP0,
			L.List[ListEntry].E.NetAddr.IP1,
			L.List[ListEntry].E.NetAddr.IP2,
			L.List[ListEntry].E.NetAddr.IP3,
			L.List[ListEntry].E.NetAddr.Port,
			L.List[ListEntry].E.APP.AppName);
			//(int)L.List[ListEntry].E.State,
			//abs((int)(1000*(L.List[ListEntry].LastSend-L.List[ListEntry].LastReceive)/Global.CPUClock)));

		return -1;
	}
	return 0;
}

void SPG_CONV ULIPS_PrintList(ULIPS_Interface& UI, char* List)
{
	List[0]=0;
	char Element[4*ULIPS_NAME_SIZE];
	for(int i=0;i<UI.L->NumElement;i++)
	{
		ULIPS_PrintElement(*(UI.L),i,Element);
		strcat(Element,"\n");
		strcat(List,Element);
	}
	return;
}

#endif

