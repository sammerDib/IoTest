#ifdef INC_SPG_PeakDet2DGetLaserLine_INC
#error SPG_PeakDet2DGetLaserLine Included twice
#endif
#define	INC_SPG_PeakDet2DGetLaserLine_INC

#define LASERMODE_ALL 0
#define LASERMODE_PAIR 1
#define LASERMODE_IMPAIR 2

#define PEAKDET2D_INITIALRESULT 1
#define PEAKDET2D_COMBINERESULT 2
#define PEAKDET2D_NOFILTER		4

typedef struct
{
	int NumY;
	float* Y;
	BYTE* Yvalid;
} SCANDATA;

typedef struct
{
	float xMin;
	float xMax;
	float yMin;
	float yMax;
	float xStep;
	float yStep;
} SCANAERA;

float SPG_CONV ScanGetCorrection(SCANDATA& SD, int x, float a, float b);
float SPG_CONV ScanGetDistance(SCANDATA& SD, float a, float b);
float SPG_CONV ScanOptimize(SCANDATA& SD, SCANAERA& SA, float& Min_x, float& Min_y);
void SPG_CONV ScanDefineScanAera(SCANAERA& SA, float xMin, float xMax, float yMin, float yMax, int NX, int NY);
void SPG_CONV ScanRefineScanAera(SCANAERA& SA, float Min_x, float Min_y, int NX, int NY);
void SPG_CONV PeakDet2D_LowPassFilter(float* Ysrc, float* Ydst, BYTE* Yvalid, int NumY);
int SPG_CONV PeakDet2D_MinTotalVar(SCANDATA& SD, SCANAERA& SA, int NX, int NY, float& Min_x, float& Min_y);
float SPG_CONV PeakDet2D_GetMeanVal(float* Y, BYTE* Yvalid, int NumY);
void SPG_CONV PeakDet2D_Filter(float* Y, BYTE* Yvalid, int NumY, float* Ytmp);

void SPG_CONV PeakDet2D_GetLaserModele(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, float CameraAngleRotationRad, float CameraAngleTiltRad, int LaserMode=LASERMODE_ALL);
//int SPG_CONV PeakDet2D_GetLaserLine(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, float* Y, BYTE* Yvalid, int NumY, float CameraAngleRotationRad, float CameraAngleTiltRad, int ProcessingMode=PEAKDET2DINITIAL_RESULT, int LaserMode=LASERMODE_ALL, SPG_PEAKPOSITION** YParameters_LASER_COUNT=0);
int SPG_CONV PeakDet2D_GetLaserLine(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, float* Y, BYTE* Yvalid, int NumY, float CameraAngleRotationRad, float CameraAngleTiltRad, int ProcessingMode=PEAKDET2D_INITIALRESULT, int LaserMode=LASERMODE_ALL, SPG_PEAKPOSITION** YParameters_LASER_COUNT=0);
void SPG_CONV PeakDet2D_DrawLaserModele(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, G_Ecran& E, float CameraAngleRotationRad, float CameraAngleTiltRad, int LaserMode=LASERMODE_ALL);
void SPG_CONV PeakDet2D_DrawLaserResult(SPG_PEAKDET2D& D2D, SPG_PEAKMODELE& PM, G_Ecran& E);
