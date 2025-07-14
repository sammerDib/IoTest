// stdafx.cpp : fichier source incluant simplement les fichiers Include standard
// TreatDegauchy0.pch représente l'en-tête précompilé
// stdafx.obj contient les informations de type précompilées

#include "stdafx.h"

void LogThis( int nSrc, int nTypeEvt,const CString& csMsg )
{
	//LogThis(nSrc,nTypeEvt,csMsg.GetBuffer()); TODO sde log dans Demeter
	//csMsg.ReleaseBuffer();
}

#define SIZE_SZBUFF 512
CString Fmt(LPCTSTR pFmt, ...  )
{
	CString ;
	char szBuffer[SIZE_SZBUFF];
	va_list args;
	va_start(args, pFmt);
	//_vsntprintf( szBuffer, SIZE_SZBUFF, pFmt, args ); // function deprecated  !!!
	int nSize = vsnprintf_s( szBuffer, sizeof(szBuffer), _TRUNCATE, pFmt, args);
	va_end(args);
	return (CString(szBuffer));
}

/// performance timer
LARGE_INTEGER	g_llPerfTimeCurTicks;
double			g_dPerfTimeTicksPerSec = 0.0;
double			GetPerfTime()
{
	if(g_dPerfTimeTicksPerSec == 0.0)
	{
		// Performance counter, Use QueryPerformanceFrequency() to get frequency of timer.
		LARGE_INTEGER qwTicksPerSec;
		if( QueryPerformanceFrequency( &qwTicksPerSec ) )
		{
			g_dPerfTimeTicksPerSec = (double) qwTicksPerSec.QuadPart;
		}
	}
	QueryPerformanceCounter( &g_llPerfTimeCurTicks );
	return ((double) ( g_llPerfTimeCurTicks.QuadPart) / g_dPerfTimeTicksPerSec)*1000.0;
}

void CreateDir(CString&  p_csPath)
{
	char DirName[256];;
	char* p = p_csPath.GetBuffer();
	if (PathFileExists(p))
	{
		p_csPath.ReleaseBuffer();
		return;
	}

	char* q = DirName; 
	while(*p)
	{
		if (('\\' == *p) || ('/' == *p))
		{
			if (':' != *(p-1))
			{
				CreateDirectory(DirName, NULL);
			}
		}
		*q++ = *p++;
		*q = '\0';
	}
	CreateDirectory(DirName, NULL);
	p_csPath.ReleaseBuffer();
}