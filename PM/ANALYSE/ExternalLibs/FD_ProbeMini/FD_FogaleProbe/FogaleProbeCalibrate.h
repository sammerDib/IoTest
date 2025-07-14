
int FPAGetIndex(int ProbeID, char* MaterialName, double* Index);
int FPADefineSample(int ProbeID, char* Name, char* SampleNumber, double* Thickness, double* Tolerance, char** MatName, double* Type, int NbThickness, double Gain, double QualityThreshold);
int FPADefineZ20Sample(int ProbeID, double Gain);
int FPAGainLoopZ20(int ProbeID);
