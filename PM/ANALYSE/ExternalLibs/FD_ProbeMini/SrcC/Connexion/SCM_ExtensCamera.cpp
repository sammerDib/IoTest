

#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include "SCM_ExtensCamera.h"

SCX_EXTCAMGETSIZE(scxCamGetSize)
{
	scxCHECKG(C, "scxCamGetSize");
	//CHECKTWO((C->CI->Type&sciCAMERA)==0,"scxCamGetSize not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_CAMGETSIZE]==0,"scxCamGetSize non implémenté",C->CI->Name,return 0);
	return (SCX_CAMGETSIZE(C->UserFctPtr[sci_EXT_CAMGETSIZE]))(SizeX, SizeY, SizePix, PixelSizeX, PixelSizeY, C->UserFctData[sci_EXT_CAMGETSIZE]?C->UserFctData[sci_EXT_CAMGETSIZE]:C);
}

SCX_EXTCAMGETSTATUS(scxCamGetStatus)
{
	scxCHECKG(C, "scxCamGetStatus");
	//CHECKTWO((C->CI->Type&sciCAMERA)==0,"scxCamGetStatus not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_CAMGETSTATUS]==0,"scxCamGetStatus non implémenté",C->CI->Name,return 0);
	return (SCX_CAMGETSTATUS(C->UserFctPtr[sci_EXT_CAMGETSTATUS]))(Flag, C->UserFctData[sci_EXT_CAMGETSTATUS]?C->UserFctData[sci_EXT_CAMGETSTATUS]:C);
}

#endif
