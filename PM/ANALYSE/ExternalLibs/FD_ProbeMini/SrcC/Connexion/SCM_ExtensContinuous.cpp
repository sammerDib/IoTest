

#include "..\SPG_General.h"

#ifdef SPG_General_USECONNEXION

#include "..\SPG_Includes.h"

#include "SCM_ExtensContinuous.h"

SCX_EXTSTARTCONTINUOUSREAD(scxStartContinuousRead)
{
	scxCHECKG(C, "scxStartContinuousRead");
	//CHECKTWO((C->CI->Type&sciCONTINUOUS)==0,"scxStartContinuousRead not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_STARTCONTINUOUSREAD]==0,"scxStartContinuousRead non implémenté",C->CI->Name,return 0);
	return (SCX_STARTCONTINUOUSREAD(C->UserFctPtr[sci_EXT_STARTCONTINUOUSREAD]))(C->UserFctData[sci_EXT_STARTCONTINUOUSREAD]?C->UserFctData[sci_EXT_STARTCONTINUOUSREAD]:C);
}

SCX_EXTSTOPCONTINUOUSREAD(scxStopContinuousRead)
{
	scxCHECKG(C, "scxStopContinuousRead");
	//CHECKTWO((C->CI->Type&sciCONTINUOUS)==0,"scxStopContinuousRead not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_STOPCONTINUOUSREAD]==0,"scxStopContinuousRead non implémenté",C->CI->Name,return 0);
	return (SCX_STOPCONTINUOUSREAD(C->UserFctPtr[sci_EXT_STOPCONTINUOUSREAD]))(C->UserFctData[sci_EXT_STOPCONTINUOUSREAD]?C->UserFctData[sci_EXT_STOPCONTINUOUSREAD]:C);
}

SCX_EXTSTARTCONTINUOUSWRITE(scxStartContinuousWrite)
{
	scxCHECKG(C, "scxStartContinuousWrite");
	//CHECKTWO((C->CI->Type&sciCONTINUOUS)==0,"scxStartContinuousWrite not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_STARTCONTINUOUSWRITE]==0,"scxStartContinuousWrite non implémenté",C->CI->Name,return 0);
	return (SCX_STARTCONTINUOUSWRITE(C->UserFctPtr[sci_EXT_STARTCONTINUOUSWRITE]))(C->UserFctData[sci_EXT_STARTCONTINUOUSWRITE]?C->UserFctData[sci_EXT_STARTCONTINUOUSWRITE]:C);
}

SCX_EXTSTOPCONTINUOUSWRITE(scxStopContinuousWrite)
{
	scxCHECKG(C, "scxStopContinuousWrite");
	//CHECKTWO((C->CI->Type&sciCONTINUOUS)==0,"scxStopContinuousWrite not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_STOPCONTINUOUSWRITE]==0,"scxStopContinuousWrite non implémenté",C->CI->Name,return 0);
	return (SCX_STOPCONTINUOUSWRITE(C->UserFctPtr[sci_EXT_STOPCONTINUOUSWRITE]))(C->UserFctData[sci_EXT_STOPCONTINUOUSWRITE]?C->UserFctData[sci_EXT_STOPCONTINUOUSWRITE]:C);
}

SCX_EXTGETCONTINUOUSFREQUENCY(scxGetContinuousFrequency)
{
	scxCHECKG(C, "scxGetContinuousFrequency");
	//CHECKTWO((C->CI->Type&sciCONTINUOUS)==0,"scxStopContinuousWrite not supported",C->CI->Name,return 0);
	CHECKTWO(C->UserFctPtr[sci_EXT_GETCONTINUOUSFREQUENCY]==0,"scxGetContinuousFrequency non implémenté",C->CI->Name,return 0);
	return (SCX_GETCONTINUOUSFREQUENCY(C->UserFctPtr[sci_EXT_GETCONTINUOUSFREQUENCY]))(Frequency,C->UserFctData[sci_EXT_GETCONTINUOUSFREQUENCY]?C->UserFctData[sci_EXT_GETCONTINUOUSFREQUENCY]:C);
}



#endif
