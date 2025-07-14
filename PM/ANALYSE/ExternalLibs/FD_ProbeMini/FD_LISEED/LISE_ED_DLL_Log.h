/*
 * $Id: LISE_ED_DLL_Log.h 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#ifndef LISE_ED_DLL_LOG_H
#define LISE_ED_DLL_LOG_H

// ## general functio of log

// pour logguer des fichiers de résultats
void LogPeakUsed(LISE& Lise,PERIOD_RESULT* LastNMeasure,int Index,int Mode=0);
void LogPeakRBIInfo(LISE& Lise,int Index,double ThicknessValue,int ModeMatching = 0);
void LogPeakRBIInfoResult(LISE& Lise,int Index,int NumBestPeak, int NumMatchingSuccess, int IndexStart, int IndexStop);
void LogPeakComment(LISE& Lise,char* Comment);
void LogPeakResult(LISE& Lise,double* Thickness, double* Quality);
void LogPeakCommentResult(LISE& Lise,char* Comment);
void LogPeakRefine(LISE& Lise,PERIOD_RESULT* LastNMeasure,double* refineValues,bool bMoins);
void RestartPicMoyenne(LISE& Lise);
void ManagePicMoyenne(LISE& Lise);

// fonction pour faire un log du signal brut après acquisition
void LogSignalBrut(LISE_ED& LiseEd,double* BufferTemp,int NumSample);
int LogPeriodSignal(LISE_ED& LiseEd);
void LogFilePeriodNSignal(LISE_ED& LiseEd);

void LogLEDInfo(LOGFILE& log,char* Message,bool bDisplayInLogFile);
void __cdecl logF(LISE_ED* LiseEd, char* format, ...);
int SaveLastWaveformError(LISE_ED& LiseEd,bool bFunctionInThread);
#ifdef NOHARDWARE
#define DisplayDAQmxError(LiseEd,error,FileError)
#else
void DisplayDAQmxError(LISE_ED& LiseEd,int32 error,char* FileError);
#endif
//void LogSetOutput(LISE& Lise,int Output);
/*void LogMsg(LISE& Lise,char* M);
void LogMsgV(LISE& Lise,char* M, double v);
void LogMsgS(LISE& Lise,char* M, char* S);
void LogVal(LISE& Lise,double v);
void LogTime(LISE& Lise);*/

#ifdef _WATCHTIME_

void WatchTimeInit(WATCH_TIME_STRUCT& Watcher,char* Name);
void WatchTimeRestart(WATCH_TIME_STRUCT& Watcher);
void WatchTimeWatch(WATCH_TIME_STRUCT& Watcher, char* Name);
void WatchTimeStop(WATCH_TIME_STRUCT& Watcher);
void WatchTimeClose(WATCH_TIME_STRUCT& Watcher);

#endif

#endif