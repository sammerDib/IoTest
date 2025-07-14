/*
 * $Id: LISE_ED_DLL_General.h 6636 2008-01-07 14:31:37Z m-abet $
 */

#ifndef LISE_ED_DLL_GENERAL_H
#define LISE_ED_DLL_GENERAL_H

#include "LISE_ED_DLL_Internal.h"

typedef struct
{
	int DLL_State;		// Initialisée ou non

	LISE_ED LISE;

// Fichier de log
//	FILE* FileLog;

} DLL_STATIC_STATE;

#endif