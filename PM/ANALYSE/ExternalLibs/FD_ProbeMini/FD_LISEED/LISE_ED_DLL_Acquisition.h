/*
 * $Id: LISE_ED_DLL_Acquisition.h 8257 2009-02-16 17:52:50Z S-PETITGRAND $
 */

#ifndef LISE_ED_DLL_ACQUISITION_H
#define LISE_ED_DLL_ACQUISITION_H

int StartAcquisition(LISE_ED& LiseEd);
int FillSignalEmule(LISE_ED& LiseEd);
int ReadSignalEmule(LISE_ED& LiseEd,char* FileName);
int AcqRead(LISE_ED& LiseEd,int32& NumSample,double* BufferTemp,int BufferSize);
int AcqReadEmule(LISE_ED& LiseEd,int32& NumSample,double* BufferTemp,int BufferSize);
int AcqReaddeviceConnected(LISE_ED& LiseEd,int32& NumSample,double* BufferTemp,int BufferSize);
int ReadPuissTemperature(LISE_ED& LiseEd,double* Valeur);
int ReadPuissAveraged(LISE_ED& LiseEd,double* Valeur);
int AcqReadContinu(LISE_ED& LiseEd,int& NumSample, int* BufferTemp, int BufferSize);
int CalculeLongueurBuffer(LISE& Lise,int32& DataLen0, int32& Datalen1);
//int32 CVICALLBACK EveryNSamplesCallback(TaskHandle taskHandle, int32 everyNsamplesEventType, uInt32 nSamples, void *callbackData);
int AcqAndProcess(LISE_ED& LiseEd);
DWORD WINAPI AcquisitionEtProcess( LPVOID lpParam );

#endif