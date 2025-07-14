// stdafx.cpp : fichier source incluant simplement les fichiers Include standard
// NanoCore.pch représente l'en-tête précompilé
// stdafx.obj contient les informations de type précompilées

#include "stdafx.h"

#include <fstream>
#include <chrono>
#include <ctime>

void LogThis( int nSrc, int nTypeEvt, const CString& csMsg )
{
    // TODO sde reactivate logs, but this time in Demeter.
 	//LogThis(nSrc,nTypeEvt,csMsg.GetBuffer());

    // Temp!!! IBE for temporary logs
    // Time
    auto When = std::chrono::system_clock::now();
    std::time_t end_time = std::chrono::system_clock::to_time_t(When);

    // Write info
    std::ofstream MyStream;
    MyStream.open("C:\\Temp\\NanoTopoTempLog_NanoCore.txt", std::ofstream::out | std::ofstream::app);
    MyStream << std::ctime(&end_time) << csMsg << std::endl;
    MyStream.close();
}

#define SIZE_SZBUFF 512
CString Fmt(LPCTSTR pFmt, ...  )
{
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