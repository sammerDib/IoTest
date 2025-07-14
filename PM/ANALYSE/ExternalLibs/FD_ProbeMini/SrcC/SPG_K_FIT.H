
#if defined(SPG_General_USEPARAFIT)||defined(SPG_General_USEWEIGHTEDPARAFIT)

typedef float kfloat;

typedef struct
{
	union
	{
		kfloat a[3]; //c + bx +axx
		struct
		{
			kfloat _c;
			kfloat _b;
			kfloat _a;
		};
	};
} K_FIT2_ELT;

typedef struct
{
	union
	{
		kfloat a[4]; //d + cx + bxx +axxx
		struct
		{
			kfloat _d;
			kfloat _c;
			kfloat _b;
			kfloat _a;
		};
	};
} K_FIT3_ELT;

typedef struct
{
	union
	{
		kfloat a[5]; //e + dx + cxx + bxx +axxxx
		struct
		{
			kfloat _e;
			kfloat _d;
			kfloat _c;
			kfloat _b;
			kfloat _a;
		};
	};
} K_FIT4_ELT;

typedef struct
{
	K_FIT2_ELT** e;//de zero à FIT_LEN INCLUS
	int Len;//max inclus
} K_FIT2;

typedef struct
{
	K_FIT3_ELT** e;//de zero à FIT_LEN INCLUS
	int Len;//max inclus
} K_FIT3;


typedef struct
{
	K_FIT4_ELT** e;//de zero à FIT_LEN INCLUS
	int Len;//max inclus
} K_FIT4;

#endif

#ifdef SPG_General_USEPARAFIT
void SPG_CONV K_FIT2_Init(K_FIT2& KF2);
void SPG_CONV K_FIT3_Init(K_FIT3& KF3);
void SPG_CONV K_FIT4_Init(K_FIT4& KF4);

void SPG_CONV K_FIT2_Close(K_FIT2& KF);
void SPG_CONV K_FIT3_Close(K_FIT3& KF);
void SPG_CONV K_FIT4_Close(K_FIT4& KF);

#endif

#ifdef SPG_General_USEWEIGHTEDPARAFIT

typedef kfloat(SPG_CONV * SPG_FITWEIGHT)(void* User, kfloat x); //x=(2*n+1-N)/(kfloat)N [-1+1/N ; 1-1/N]

void SPG_CONV K_FIT2W_Init(K_FIT2& KF2, int Len, SPG_FITWEIGHT w, void* User);
void SPG_CONV K_FIT3W_Init(K_FIT3& KF3, int Len, SPG_FITWEIGHT w, void* User);
void SPG_CONV K_FIT4W_Init(K_FIT4& KF4, int Len, SPG_FITWEIGHT w, void* User);

void SPG_CONV K_FIT2W_Close(K_FIT2& KF);
void SPG_CONV K_FIT3W_Close(K_FIT3& KF);
void SPG_CONV K_FIT4W_Close(K_FIT4& KF);


template <typename K_FIT> void SPG_CONV K_FITW_OutputElt(K_FIT& KF, int FITORDER, char* FileName)
{
	FILE* F=fopen(FileName,"wb+");	
	CHECKTWO(F==0,"K_FITW_OutputElt : fopen failed",FileName,return);
	for(int L=1;L<=KF.Len;L++)//pour chaque longueur de fit possible
	{
		fprintf(F,"\r\n\r\n\r\n// ##### FIT DEGRE %i LONGUEUR %i #####\r\n",FITORDER,L);
		for(int n=0;n<=FITORDER;n++)//pour chaque coefficient (a,b,c,...) du polynome
		{
			fprintf(F,"\r\n// ----- Calcul du coefficient %c . x^%i  - somme de %i termes -----\r\n",'a'+FITORDER-n,n,L);
			for(int i=0;i<L;i++)//pour chaque echantillon du fit de longueur L
			{
				char TxtCur[64]; CF_GetStringZ(TxtCur,KF.e[L][i].a[n],CF_DIGITFLOAT+2);
				fprintf(F,"KF%i[%i].e[%i].a[%i]=%s;\r\n",FITORDER,L,  i,  n,TxtCur);
			}
		}
	}
	fclose(F);
	return;
}

#endif

#if defined(SPG_General_USEPARAFIT)||defined(SPG_General_USEWEIGHTEDPARAFIT)

//Fonctions implémentées dans SPG_K_FIT_RT.cpp (RealTimeSourceFiles)

void SPG_CONV K_FIT2_Short(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K);
void SPG_CONV K_FIT2_Short_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K);
void SPG_CONV K_FIT2_Float(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K);
void SPG_CONV K_FIT2_Float_Unlimited(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K);
void SPG_CONV K_FIT2_FloatMonoA(float* D, int Len, K_FIT2_ELT& R, K_FIT2& K);
void SPG_CONV K_FIT2_FloatMono(float* D, int Len, K_FIT2_ELT& R, K_FIT2& K);

void SPG_CONV K_FIT2_INVShort(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf);
void SPG_CONV K_FIT2_INVShort_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf);
void SPG_CONV K_FIT2_SQRShort(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf);
void SPG_CONV K_FIT2_SQRShort_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, kfloat Vinf);
void SPG_CONV K_FIT2_INVFloat(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, float Vinf);
void SPG_CONV K_FIT2_INVFloat_Unlimited(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K, float Vinf);

void SPG_CONV K_FIT2_Double(double* D1, int Len1, double* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K);
//id SPG_CONV K_FIT2C_Short(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K);
//id SPG_CONV K_FIT2C_Float(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT2_ELT& R, K_FIT2& K);
void SPG_CONV K_FIT3_Short(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT3_ELT& R, K_FIT3& K);
void SPG_CONV K_FIT3_Float(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT3_ELT& R, K_FIT3& K);

void SPG_CONV K_FIT4_Short(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K);
void SPG_CONV K_FIT4_Short_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K);
void SPG_CONV K_FIT4_Float(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K);
void SPG_CONV K_FIT4_Float_Unlimited(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K);

void SPG_CONV K_FIT4_INVShort(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf);
void SPG_CONV K_FIT4_INVShort_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf);
void SPG_CONV K_FIT4_SQRShort(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf);
void SPG_CONV K_FIT4_SQRShort_Unlimited(short int* D1, int Len1, short int* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, kfloat Vinf);
void SPG_CONV K_FIT4_INVFloat(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, float Vinf);
void SPG_CONV K_FIT4_INVFloat_Unlimited(float* D1, int Len1, float* D2, int Len2, int Step, K_FIT4_ELT& R, K_FIT4& K, float Vinf);

#endif
