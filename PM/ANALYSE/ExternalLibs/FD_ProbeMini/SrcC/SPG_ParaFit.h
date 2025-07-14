
#ifdef SPG_General_USEPARAFIT
/*
void SPG_CONV SPG_ParaFitL(float* D, float& a, float& b, float& c, int FitLen);
void SPG_FASTCONV SPG_ParaFit3(float D[3], float& a, float& b, float& c);
void SPG_FASTCONV SPG_ParaFit5(float D[5], float& a, float& b, float& c);
void SPG_FASTCONV SPG_ParaFit7(float D[7], float& a, float& b, float& c);
void SPG_FASTCONV SPG_ParaFit9(float D[9], float& a, float& b, float& c);
void SPG_FASTCONV SPG_ParaFit11(float D[11], float& a, float& b, float& c);
void SPG_FASTCONV SPG_ParaFit13(float D[13], float& a, float& b, float& c);
*/
//ax2+bx+c
//sommet x=-b/2a
//y=a*b2/4a2 -b*b/2a +c = b2/4a - b2/2a +c = c-b2/4a 
#define SPG_ParaFit3( am1,a0 ,ap1,                                        a,b,c) {float Sum1=am1+ap1;float Diff1=ap1-am1;a=0.5f*Sum1-a0;b=0.5f*Diff1;c=a0;}
#define SPG_ParaFit5( am2,am1,a0 ,ap1,ap2,                                a,b,c) {float Sum1=am1+ap1;float Diff1=ap1-am1;float Sum2=am2+ap2;float Diff2=ap2-am2;a=(2*Sum2-Sum1-2*a0)/14.0f;b=(2*Diff2+Diff1)/10.0f;c=(-3*Sum2+12*Sum1+17*a0)/35.0f;}
#define SPG_ParaFit7( am3,am2,am1,a0 ,ap1,ap2,ap3,                        a,b,c) {float Sum1=am1+ap1;float Diff1=ap1-am1;float Sum2=am2+ap2;float Diff2=ap2-am2;float Sum3=am3+ap3;float Diff3=ap3-am3;a=(5*Sum3-3*Sum1-4*a0)/84.0f;b=(3*Diff3+2*Diff2+Diff1)/28.0f;c=(-2*Sum3+3*Sum2+6*Sum1+7*a0)/21.0f;}
#define SPG_ParaFit9( am4,am3,am2,am1,a0 ,ap1,ap2,ap3,ap4,                a,b,c) {float Sum1=am1+ap1;float Diff1=ap1-am1;float Sum2=am2+ap2;float Diff2=ap2-am2;float Sum3=am3+ap3;float Diff3=ap3-am3;float Sum4=am4+ap4;float Diff4=ap4-am4;a=(28*Sum4+7*Sum3-8*Sum2-17*Sum1-20*a0)/924.0f;b=(4*Diff4+3*Diff3+2*Diff2+Diff1)/60.0f;c=(-21*Sum4+14*Sum3+39*Sum2+54*Sum1+59*a0)/231.0f;}
#define SPG_ParaFit11(am5,am4,am3,am2,am1,a0 ,ap1,ap2,ap3,ap4,ap5,        a,b,c) {float Sum1=am1+ap1;float Diff1=ap1-am1;float Sum2=am2+ap2;float Diff2=ap2-am2;float Sum3=am3+ap3;float Diff3=ap3-am3;float Sum4=am4+ap4;float Diff4=ap4-am4;float Sum5=am5+ap5;float Diff5=ap5-am5;a=(15*Sum5+6*Sum4-Sum3-6*Sum2-9*Sum1-10*a0)/858.0f;b=(5*Diff5+4*Diff4+3*Diff3+2*Diff2+Diff1)/110.0f;c=(-36*Sum5+9*Sum4+44*Sum3+69*Sum2+84*Sum1+89*a0)/429.0f;}
#define SPG_ParaFit13(am6,am5,am4,am3,am2,am1,a0 ,ap1,ap2,ap3,ap4,ap5,ap6,a,b,c) {float Sum1=am1+ap1;float Diff1=ap1-am1;float Sum2=am2+ap2;float Diff2=ap2-am2;float Sum3=am3+ap3;float Diff3=ap3-am3;float Sum4=am4+ap4;float Diff4=ap4-am4;float Sum5=am5+ap5;float Diff5=ap5-am5;float Sum6=am6+ap6;float Diff6=ap6-am6;a=(22*Sum6+11*Sum5+2*Sum4-5*Sum3-10*Sum2-13*Sum1-14*a0)/2002.0f;b=(6*Diff6+5*Diff5+4*Diff4+3*Diff3+2*Diff2+Diff1)/182.0f;c=(-11*Sum6+9*Sum4+16*Sum3+21*Sum2+24*Sum1+25*a0)/143.0f;}

#define SPG_ParaFitL(D, a, b, c, FitLen) switch(FitLen){case 3:SPG_ParaFit3((D)[0],(D)[1],(D)[2],a,b,c);break;case 5:SPG_ParaFit5((D)[0],(D)[1],(D)[2],(D)[3],(D)[4],a,b,c);break;case 7:SPG_ParaFit7((D)[0],(D)[1],(D)[2],(D)[3],(D)[4],(D)[5],(D)[6],a,b,c);break;case 9:SPG_ParaFit9((D)[0],(D)[1],(D)[2],(D)[3],(D)[4],(D)[5],(D)[6],(D)[7],(D)[8],a,b,c);break;case 11:SPG_ParaFit11((D)[0],(D)[1],(D)[2],(D)[3],(D)[4],(D)[5],(D)[6],(D)[7],(D)[8],(D)[9],(D)[10],a,b,c);break;default:SPG_ParaFit13((D)[0],(D)[1],(D)[2],(D)[3],(D)[4],(D)[5],(D)[6],(D)[7],(D)[8],(D)[9],(D)[10],(D)[11],(D)[12],a,b,c);}

#define SPG_ParaFitMin 3
#define SPG_ParaFitMax 13

#define SPG_ParaFitArraySize (1+(SPG_ParaFitMax-SPG_ParaFitMin)/2)

int SPG_CONV SPG_ParaFit3to13Safe(float* D, int Pitch, int PosMin, int PosMax, int Pos, float* a, float* b, float* c);
int SPG_CONV SPG_ParaFit3to13(float* D, int Pitch, int Pos, float* a, float* b, float* c);
int SPG_CONV SPG_ParaFit3to13Scan(float* D, int Pitch, int PosMin, int PosMax, int Pos0, int ScanRange, int* x, float* a, float* b, float* c);


typedef struct
{
	double c0;
	double cx;
	double cx2;
} FIT_COEFF;

typedef struct
{
	double c0;
	double cx;
	double cy;
	double cx2;
	double cy2;
} FIT_COEFF_2D;

// 1D
FIT_COEFF lsCalculate(float* D,int len);
FIT_COEFF lsCalculateM(float* D, BYTE *M, int len);
FIT_COEFF lsCalculate2float(float* D,int len);
FIT_COEFF lsCalculate2float(float* D1, int N1, float* D2, int N2, int Step);
FIT_COEFF lsCalculate2double(double* D,int len);
FIT_COEFF lsCalculate2short(short* D, int InterleaveStep, int sx);
FIT_COEFF lsCalculate2short(short* D1, int N1, short* D2, int N2, int Step);
FIT_COEFF lsCalculate2M(float* D, BYTE *M, int len);

// 2D
FIT_COEFF_2D lsCalculate2d(float* D,int sx,int sy);
FIT_COEFF_2D lsCalculate2dM(float* D,BYTE* M,int sx,int sy);
FIT_COEFF_2D lsCalculate2d2(float* D,int sx,int sy);
FIT_COEFF_2D lsCalculate2d2M(float* D,BYTE* M,int sx,int sy);


#endif

