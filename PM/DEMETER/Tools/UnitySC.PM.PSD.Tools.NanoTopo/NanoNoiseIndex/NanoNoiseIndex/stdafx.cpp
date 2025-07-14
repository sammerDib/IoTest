
// stdafx.cpp : fichier source incluant simplement les fichiers Include standard
// NanoNoiseIndex.pch représente l'en-tête précompilé
// stdafx.obj contient les informations de type précompilées

#include "stdafx.h"

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
