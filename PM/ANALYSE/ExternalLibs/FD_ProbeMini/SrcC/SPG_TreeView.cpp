
#include "..\SrcC\SPG.h"

#ifdef SPG_General_USETREEVIEW
//#include "SPG_TreeView.h"
#include <string.h>
#include <stdio.h>

int SPG_CONV SPG_TreeInit(SPG_Tree& T, int MaxElements, int UserSize)
{
	memset(&T,0,sizeof(SPG_Tree));
	T.MaxElements=MaxElements;
	T.UserSize=UserSize;
	T.E=SPG_MemAlloc(T.MaxElements*(sizeof(SPG_TreeElement)+UserSize),"SPG_TreeInit");
	return -1;
}

void SPG_CONV SPG_TreeClose(SPG_Tree& T)
{
	SPG_MemFree(T.E);
	memset(&T,0,sizeof(SPG_Tree));
	return;
}

int SPG_CONV SPG_TreeAdd(SPG_Tree& T, void* Element, void* User)
{
	CHECK(T.NumElements>=T.MaxElements,"SPG_TreeAddRel",return 0);
	SPG_TreeElement(T,T.NumElements)=Element;
	memcpy(SPG_TreeUser(T,T.NumElements),User,T.UserSize);
	T.NumElements++;
	return -1;
}

#endif

