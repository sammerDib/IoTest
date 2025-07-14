
#ifndef LISE_PROCESS_H
#define LISE_PROCESS_H

int ProcessPic(LISE& Lise,PICRESULT* BufferResultat,RING_BUFFER_POS& WriteResult,int Voie);
int PicMoyenne(LISE& Lise,PICRESULT* BufferResultat,RING_BUFFER_POS& WriteResult,int Voie);
int FindPicSature(LISE& Lise,PICRESULT* BufferResultat,RING_BUFFER_POS &WriteResultChannelProcess);
int WriteThickness(LISE& Lise);
int WritePeaksPeriod(LISE& Lise,PERIOD_RESULT PeriodResult,FILE* FileSavePeak);
int WritePeaks(LISE& Lise,int NombreDePics,int Voie,FILE* FileSavePeak);
int PicDetection(LISE& Lise,PICRESULT* BufferResultat,double* Buffer,RING_BUFFER_POS &WriteResultChannelProcess, int Voie, bool bChan1BeforeChan2);
int FindThickness(LISE& Lise, float fPositionRefOpt, float fToleranceRefOpt);
int MemorisationResultatsPeriode(LISE& Lise);
int FindNBestPeak(LISE& Lise,RING_BUFFER_POS& WriteResult, bool bRefOptTheorique,float fPositionRefOpt,float fToleranceRefOpt);
int BufferMoyenne(LISE& Lise,double* BufferTemp,RING_BUFFER_POS& WriteResultChannelProcess);

#endif