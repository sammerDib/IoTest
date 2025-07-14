

#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include "SCM_ExtensWriteThrough.h"

// extension sciWRITETHROUGH

SCX_EXTWRITETHROUGH(scxWriteThrough)
{
	scxCHECKG(C, "scxWriteThrough");
	//CHECKTWO((C->CI->Type&sciWRITETHROUGH)==0,"scxWriteThrough not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_WRITETHROUGH]==0,"scxWriteThrough non implémenté",C->CI->Name,return 0);
	return (SCX_WRITETHROUGH(C->UserFctPtr[sci_EXT_WRITETHROUGH]))(Data, DataLen, C->UserFctData[sci_EXT_WRITETHROUGH]?C->UserFctData[sci_EXT_WRITETHROUGH]:C);
}

SCX_EXTREADTHROUGH(scxReadThrough)
{
	scxCHECKR(C, "scxReadThrough");
	//CHECKTWO((C->CI->Type&sciWRITETHROUGH)==0,"scxReadThrough not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_READTHROUGH]==0,"scxReadThrough non implémenté",C->CI->Name,DataLen=0;return 0);
	return (SCX_READTHROUGH(C->UserFctPtr[sci_EXT_READTHROUGH]))(Data, DataLen, C->UserFctData[sci_EXT_READTHROUGH]?C->UserFctData[sci_EXT_READTHROUGH]:C);
}

#endif
