/*
 * $Id: TryCatchBlock.h 5752 2007-10-04 09:36:25Z n-combe $
 */

//
// Facilite l'utilisation de blocs de gestion des exceptions __try __except
//
// Pour ne pas utiliser ces fonctionnalites, ne definissez pas la macro
// DoTryCatch
//

#ifdef DoTryCatch

#pragma SPGMSG(__FILE__,__LINE__,"Using __try __except blocks")

/**
 * This macro starts a new try-catch block. Provide a name for the block.
 */
#define BEGIN_TRYCATCH_BLOCK(name) \
	EXCEPTION_POINTERS *name; \
	__try {


/**
 * This macro ends a try-catch block. Provide the name of the block matching the
 * BEGIN_TRYCATCH_BLOCK() macro.
 * The string "msg" is displayed only in case of an exception.
 * The instruction "instr" is executed only in case of an exception.
 */
#define END_TRYCATCH_BLOCK(name, msg, instr) } \
	__except (name = (EXCEPTION_POINTERS*) _exception_info(), EXCEPTION_EXECUTE_HANDLER) \
	{ \
		unsigned int TRYCATCH_BLOCK_code = _exception_code(); \
		printExceptionReport(TRYCATCH_BLOCK_code, name); \
		memUsedMemory(); \
		if (msg && strlen(msg)) \
		{ \
			logMessage(LOG_GENERAL, LOG_ERROR, LOG_MSGBOX, msg); \
		} \
		instr; \
	}

#else // if ndef (DoTryCatch)

#define BEGIN_TRYCATCH_BLOCK(name)
#define END_TRYCATCH_BLOCK(name, msg, instr)

#endif // ndef (DoTryCatch)
