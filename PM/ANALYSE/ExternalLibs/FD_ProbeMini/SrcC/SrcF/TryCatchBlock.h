/*
 * $Id: TryCatchBlock.h 5752 2007-10-04 09:36:25Z n-combe $
 */

//
// Facilite l'utilisation de blocs de gestion des exceptions __try __except
//
// Pour ne pas utiliser ces fonctionnalites, ne definissez pas la macro
// DoTryCatch
//

//fdef DoTryCatch

/*
//ragma SPGMSG(__FILE__,__LINE__,"Using __try __except blocks")

static void GenericSnapShotFct(LPEXCEPTION_POINTERS e, char* FctName)
{
	char Msg[512];
	sprintf(Msg,"Exception %.04X in %s",e->ExceptionRecord->ExceptionCode,FctName);
	SPG_List(Msg);
	return;
}

static int ExceptionFilter(LPEXCEPTION_POINTERS e, void snapshotfct(LPEXCEPTION_POINTERS,char*) , char* fctname)
{
	LTG_Enter(Global,LT_OSAbnormalTermination,e->ExceptionRecord->ExceptionCode);

	char Msg[512];
	sprintf(Msg,"%sLogTimeExcpt_%0.4X_%s.txt",Global.ProgDir,e->ExceptionRecord->ExceptionCode,fctname);
	
	SPG_LogTimeSaveToFile_OneRow(Global.LT,Msg);

	if(snapshotfct!=0) snapshotfct(e,fctname);
	LTG_Exit(Global,LT_OSAbnormalTermination,e->ExceptionRecord->ExceptionCode);

	if(e->ExceptionRecord->ExceptionFlags==EXCEPTION_NONCONTINUABLE)
	{
		return EXCEPTION_CONTINUE_SEARCH;
	}
	else if(e->ExceptionRecord->ExceptionCode==EXCEPTION_INT_DIVIDE_BY_ZERO)
	{
		return EXCEPTION_EXECUTE_HANDLER;
		return EXCEPTION_CONTINUE_EXECUTION;
	}
	else
	{
		return EXCEPTION_CONTINUE_SEARCH;
		//return EXCEPTION_CONTINUE_EXECUTION;
	}
	//continuable : EXCEPTION_CONTINUE_EXECUTION
	//EXCEPTION_CONTINUE_EXECUTION
}

//efine BEGIN_TRYCATCH_BLOCK			__try {

//efine END_TRYCATCH_BLOCK(snapshotfct,fctname) } __except ( ExceptionFilter(GetExceptionInformation(),snapshotfct,fctname) ) {	; }	

//lse // if ndef (DoTryCatch)

//efine BEGIN_TRYCATCH_BLOCK(name)
//efine END_TRYCATCH_BLOCK(name, msg, instr)



//ndif // ndef (DoTryCatch)

*/

