
#ifndef _DBL_LISE_ED_DLL_SETTINGS_H_
#define _DBL_LISE_ED_DLL_SETTINGS_H_

int DBL_CalibrateDblLise(void* s,double* CalibrationArray);
int SetSystemMode(void* s,MODE ModeProbeBeforeCalib,int ProbeNum ,ACQUISITIONMODE ModeDblEdBeforeCalib);
int DBL_SetVisibleProbe(void* s,int Channel);
int DBL_SetCurrentProbe(void* s,int Channel, bool bUpdateProbeVisible);
int DBL_SetMasterDevice(DBL_LISE_ED& DblLiseEd);
void CreateCalibrationFile(DBL_LISE_ED& DblLiseEd);
void LogCalibrationFile(DBL_LISE_ED& DblLiseEd,double* CalibArray,double* ThicknessMeasured,double* QualityArray,double CalculateTh,int error,int _iIndexLowerAG);
void CloseCalibrationFile(DBL_LISE_ED& DblLiseEd);
void ClearPicMoyenne(void* s);

#endif