

#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include "SCM_ExtensOverwrite.h"

//extension sciOVERWRITE

SCX_EXTOVERWRITE(scxOverwrite)
{
	scxCHECKG(C, "scxOverwrite");
	//CHECKTWO((C->CI->Type&sciOVERWRITE)==0,"scxOverwrite not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_OVERWRITE]==0,"scxOverwrite non implémenté",C->CI->Name,return 0);
	return (SCX_OVERWRITE(C->UserFctPtr[sci_EXT_OVERWRITE]))(Data, DataLen, Offset, C->UserFctData[sci_EXT_OVERWRITE]?C->UserFctData[sci_EXT_OVERWRITE]:C);
}

SCX_EXTGETTOTALSIZE(scxGetTotalSize)
{
	Size=0;
	scxCHECKG(C, "scxGetTotalSize");
	//CHECKTWO((C->CI->Type&sciOVERWRITE)==0,"scxGetTotalSize not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_GETTOTALSIZE]==0,"scxGetTotalSize non implémenté",C->CI->Name,return 0);
	return (SCX_GETTOTALSIZE(C->UserFctPtr[sci_EXT_GETTOTALSIZE]))(Size, C->UserFctData[sci_EXT_GETTOTALSIZE]?C->UserFctData[sci_EXT_GETTOTALSIZE]:C);
}
#endif
