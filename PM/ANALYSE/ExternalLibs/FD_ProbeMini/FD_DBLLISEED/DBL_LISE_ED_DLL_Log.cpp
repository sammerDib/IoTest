
#include <windows.h>
#include <stdio.h>
#include <string.h>

// ## probe-common headers ##
#include "..\SrcC\SPG.h"
#include "../FD_FogaleProbe/NIDAQmxConfig.h"
#include "../FD_FogaleProbe/FogaleProbeReturnValues.h"
// ## probe-common headers ##

// ## probe-specific headers ##
#include "../FD_LISE_General/LISE_Consts.h"
#include "../FD_LISE_General/PeakMatch.h"
#include "../FD_LISE_General/LISE_Struct_Process.h"
#include "../FD_LISE_General/LISE_Struct.h"

#include "../FD_LISEED/LISE_ED_DLL_UI_Struct.h"
#include "../FD_LISEED/LISE_ED_DLL_Internal.h"

// ## probe-specific headers ##
#include "DBL_LISE_ED_DLL_Internal.h"
#include "DBL_LISE_ED_DLL_Log.h"


#ifndef NOHARDWARE
int DblEdDisplayDAQmxError(LISE_ED& LiseEd,int32 error,char* FileError)
{
#ifdef DEVICECONNECTED
	if(LiseEd.bLiseEDConnected == true)
	{
		if( DAQmxFailed(error) )
		{
			char errBuff[2048];
			DAQmxGetExtendedErrorInfo(errBuff,2048);
			if(LiseEd.Lise.bDebug == true)
			{
				LogfileF(*LiseEd.Lise.Log,errBuff);
			}
			if(LiseEd.Lise.DisplayNIError == 0)
			{
				if(Global.EnableList>=1)
				{
					MessageBox(0,fdwstring(errBuff),fdwstring(FileError),0);
				}
			}

			// retourne une erreur pour le cas d'un fail
			return FP_FAIL;
		}
	}
#endif //DEVICECONNECTED

	return FP_OK;
}
#endif

void __cdecl LogDblED(DBL_LISE_ED& DblLiseEd, LOG_PRIO Prio,char* format,...)
{
	char strMessage[MAXSTR];
	strcpy(strMessage,"[DBL_LISE_ED]\t");

	bool bDontLogInstructionCauseDebug = false;

	switch(Prio)
	{
	case PRIO_ERROR:
		strcat(strMessage,"[ERROR]\t");
		break;

	case PRIO_WARNING:
		strcat(strMessage,"[WARNING]\t");
		break;

	case PRIO_INFO:
		strcat(strMessage,"[INFO]\t");
		break;

	case PRIO_DEBUG:
		bDontLogInstructionCauseDebug = true;
#ifdef _DEBUG
		strcat(strMessage,"[DEBUG]\t");
#endif
		break;

	default:
		strcat(strMessage,"[INFO]\t");
		break;
	}

	if(!bDontLogInstructionCauseDebug && DblLiseEd.LiseEd[0].Lise.bDebug)
	{
		SPG_ArrayStackAlloc(char,message,MAXSTR);

		va_list args;
		va_start(args, format);
		_vsnprintf(message, MAXLOGMSG, format, args);

		SPG_ArrayStackCheck(message);
		va_end(args);
		
		message[MAXLOGMSG - 1] = '\0';

		strcat(strMessage,message);
		Logfile(*DblLiseEd.Log, strMessage);
	}
}