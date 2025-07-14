
#ifndef _DBL_ED_LOG_H_
#define _DBL_ED_LOG_H_

typedef enum
{
	PRIO_WARNING,
	PRIO_ERROR,
	PRIO_INFO,
	PRIO_DEBUG,

} LOG_PRIO;


// function to log information in probe double Lise Ed
int DblEdDisplayDAQmxError(LISE_ED& LiseEd,int32 error,char* FileError);
void __cdecl LogDblED(DBL_LISE_ED& DblLiseEd, LOG_PRIO Prio,char* format,...);

#endif // _DBL_ED_LOG_H