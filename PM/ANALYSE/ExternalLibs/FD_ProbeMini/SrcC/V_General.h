
typedef long LONG;
typedef unsigned long DWORD;
typedef short SHORT;
typedef unsigned short WORD;
typedef unsigned char BYTE;

#ifndef SPG_General_PGLib
typedef struct
{
	float x;
	float y;
	float z;
} V_VECT;
#else
typedef PGLVector V_VECT;
#endif

typedef struct
{
	V_VECT pos;
	V_VECT axex;
	V_VECT axey;
	V_VECT axez;
} V_REPERE;

#include <math.h>

#ifdef SPG_General_FastMath

#define ceil(f) ceilf(f)
#define fabs(f) fabsf(f)
#ifdef IntelCompilerFPU
//pb avec floor compilateur Intel
#else
#define floor(f) floorf(f)
#endif

/* Power functions */
#define pow(a,e) powf(a,e)
#define powfInt(a,e) powf((float)(a),(float)(e))
#define powInt(a,e) (int)(powf((float)(a),(float)(e)))
#define sqrt(f) sqrtf(f)
#define sqrtInt(f) (int)(sqrtf((float)(f)))

/* Exponential and logarithmic functions */
#define exp(f) expf(f)
#define log(f) logf(f)
#define log10(f) log10f(f)

/* Trigonometric functions */

#define cos(f) cosf(f)
#define sin(f) sinf(f)
#define tan(f) tanf(f)

#define acos(f) acosf(f)
#define asin(f) asinf(f)
#define atan(f) atanf(f)

#else

#define powfInt(a,e) pow((float)(a),(float)(e))
#define powInt(a,e) (int)(pow((float)(a),(float)(e)))
#define sqrtInt(f) (int)(sqrt((float)(f)))

#endif

#define V_4PI 12.566370614359172953850573533118
#define V_QPI V_4PI
#define V_2PI 6.28318530717958647692528676655901
#define V_DPI V_2PI
#define V_PI 3.1415926535897932384626433832795
#define V_HPI 1.5707963267948966192313216916398
#define V_E 2.7182818284590452353602874713527

#define V_DEGtoRAD(x) (V_PI*(x)/180)

#define V_TST(x,m) (((x)&(m))==(m))
#define V_NOTST(x,m) (((x)&(m))!=(m))

#define V_Max(A,B) ((A>B)?A:B)
#define V_Min(A,B) ((A<B)?A:B)
#define V_IsBound(X,Min,Max) ((X>=Min)&&(X<Max))
#define V_InclusiveBound(X,Min,Max) ((X>=Min)&&(X<=Max))
#define V_Sature(X,Min,Max) (((X>=Min)&&(X<=Max))?X:((X>=Max)?Max:Min))
#define V_SatureUP(X,Max) ((X<=Max)?X:Max)
#define V_Signe(S) ((S>0)?1:((S<0)?-1:0))
#define V_Swap(Type,A,B) {Type SwapTemp=A;A=B;B=SwapTemp;}
#define V_SWAP(Type,A,B) {Type SwapTemp=A;A=B;B=SwapTemp;}
#define V_SwapMin(Type,A,B) {Type SwapTemp=A;A=B;if(SwapTemp<B) B=SwapTemp;}
#ifdef IntelCompilerFPU
#define V_Round(x) ((int)(x))
#else
#define V_Round(x) ((int)floor(x+0.5f))
#endif
#define V_Floor(x) ((int)floor(x))
#define V_Ceil(x) ((int)ceil(x))
//conversion rapide non specifiee
#define V_IntToFloat(x) ((float)(x))
#define V_FloatToInt(x) ((int)(x))
#define V_FloatToLong(x) ((LONG)(x))
#define V_FloatToShort(x) ((SHORT)(x))
#define V_FloatToByte(x) ((BYTE)(x))

#define V_IntToDouble(x) ((double)(x))
#define V_DoubleToInt(x) ((int)(x))

#define V_DoubleToFloat(x) ((float)(x))
#define V_FloatToDouble(x) ((double)(x))

#define V_IsPowerOfTwo(N) (N&(N-1))

#define V_Wrap1(N,W) if (N>=W) N-=W
#define V_WrapUp(N,W) (N>=W)?N-W:N
#define V_WrapDn(N,W) (N<0)?N+W:N
#define V_FullWrapUp(N,W) while(N>=W) N-=W
#define V_FullWrapDn(N,W) while(N<0) N+=W
#define V_Wrap(N,W) if(N>=W) V_FullWrapUp(N,W); else V_FullWrapDn(N,W);
#define V_WrapPi(N) if(N>0) while(N>=V_PI) N-=V_2PI; else while(N<-V_PI) N+=V_2PI;
#define V_Wrap_0_2Pi(N) if(N>0) while(N>=V_DPI) N-=V_2PI; else while(N<0) N+=V_2PI;

#define V_MakeDigit(X) ('0'+(X))
#define V_MakeHexaDigit(X) (((X)>9)?('A'-10+(X)):('0'+(X)))

#define V_DisabledString(s) ((s==0)||(s[0]==0)||(stricmp(s,"empty")==0)||(stricmp(s,"void")==0)||(stricmp(s,"no")==0)||(stricmp(s,"none")==0)||(stricmp(s,"nul")==0)||(stricmp(s,"null")==0))


#define V_Operate1(V1,O) {V1.x O;V1.y O;V1.z O;}
#define V_Operate2(V1,V2) {V1.x V2.x;V1.y V2.y;V1.z V2.z;}
#define V_Operate3(V1,V2,V3) {V1.x V2.x V3.x;V1.y V2.y V3.y;V1.z V2.z V3.z;}
#define V_Operate4(V1,V2,V3,V4) {V1.x V2.x V3.x V4.x;V1.y V2.y V3.y V4.y;V1.z V2.z V3.z V4.z;}
#define V_Operate5(V1,V2,V3,V4,V5) {V1.x V2.x V3.x V4.x V5.x;V1.y V2.y V3.y V4.y V5.y;V1.z V2.z V3.z V4.z V5.z;}

#define V_Operate1V(V1,O) {V1.x O,V1.y O,V1.z O}
#define V_Operate2V(V1,V2) {V1.x V2.x,V1.y V2.y,V1.z V2.z}
#define V_Operate3V(V1,V2,V3) {V1.x V2.x V3.x,V1.y V2.y V3.y,V1.z V2.z V3.z}
#define V_Operate4V(V1,V2,V3,V4) {V1.x V2.x V3.x V4.x,V1.y V2.y V3.y V4.y,V1.z V2.z V3.z V4.z}
#define V_Operate5V(V1,V2,V3,V4,V5) {V1.x V2.x V3.x V4.x V5.x,V1.y V2.y V3.y V4.y V5.y,V1.z V2.z V3.z V4.z V5.z}

#define V_SetXYZ(V,cx,cy,cz) {V.x=cx;V.y=cy;V.z=cz;}
#define V_StackAllocXYZ(V,cx,cy,cz) V_VECT V={cx,cy,cz}

/*
inline double MTRand::randNorm( const double mean, const double stddev )
{
	// Return a real number from a normal (Gaussian) distribution with given
	// mean and standard deviation by polar form of Box-Muller transformation
	double x, y, r;
	do
	{
		x = 2.0 * rand() - 1.0;
		y = 2.0 * rand() - 1.0;
		r = x * x + y * y;
	}
	while ( r >= 1.0 || r == 0.0 );
	double s = sqrt( -2.0 * log(r) / r );
	return mean + x * s * stddev;
}
*/

#ifdef DebugFloat
#define V_CheckSup(V) DbgCHECK((_finite(V.x)==0)||(fabs(V.x)>1.0e9),"Valeur impossible");DbgCHECK((_finite(V.y)==0)||(fabs(V.y)>1.0e9),"Valeur impossible");DbgCHECK((_finite(V.z)==0)||(fabs(V.z)>1.0e9),"Valeur impossible")
#define V_CheckFloat(FloatRes,Msg) CHECKFLOAT(FloatRes,Msg)
#else
#define V_CheckSup(V)
#define V_CheckFloat(FloatRes,Msg)
#endif

#ifdef DebugFloatHard
#define V_HardCheckSup(V) V_CheckSup(V)
#define V_HardCheckFloat(FloatRes,Msg) V_CheckFloat(FloatRes,Msg)
#define V_HardCheckNorm(FloatRes) DbgCHECK(!V_IsBound(FloatRes,1.0e-6,1.0e6),"Norme hors norme")
#define V_HardCheckScal(FloatRes) DbgCHECK(fabs(FloatRes)>1.0e9,"Produt scalaire impossible")
#else
#define V_HardCheckSup(V)
#define V_HardCheckFloat(FloatRes,Msg)
#define V_HardCheckNorm(FloatRes)
#define V_HardCheckScal(FloatRes)
#endif

//void ZeroV(V_VECT& V);
//void V_AddVect(V_VECT& V1, V_VECT& V2, V_VECT& V_VRES);
//#define V_AddVect(V1,V2,V3) V_Operate3(V3,V1,+V2)
//void V_SubVect(V_VECT& V1, V_VECT& V2, V_VECT& V_VRES);
//#define V_SubVect(V1,V2,V3) V_Operate3(V3,V1,-V2)
//void V_AddCVect(float C1, V_VECT& V1,float C2, V_VECT& V2, V_VECT& V_VRES);
//#define  V_AddCVect(float C1, V_VECT& V1,float C2, V_VECT& V2, V_VECT& V_VRES);
//void V_MulVect(float C, V_VECT& VRES);
//float V_ScalVect(V_VECT& V1, V_VECT& V2);
#define V_ScalVect(V1,V2,RES) {V_HardCheckSup(V1);V_HardCheckSup(V2);RES=V1.x*V2.x;float VSV_TMP1=V1.y*V2.y;float VSV_TMP2=V1.z*V2.z;RES+=VSV_TMP1;RES+=VSV_TMP2;V_HardCheckScal(RES);}
#define V_ScalDiffVect(V1,V2P,V2M,RES) {RES=V1.x*(V2P.x-V2M.x);float SDV_TMP1=V1.y*(V2P.y-V2M.y);float SDV_TMP2=V1.z*(V2P.z-V2M.z);RES+=SDV_TMP1;RES+=SDV_TMP2;}
//float V_Mod2Vect(V_VECT& V1);
#define V_Mod2Vect(V1,RES) V_ScalVect(V1,V1,RES)
//float V_ModVect(V_VECT& V1);
#define V_ModVect(V1,RES) {float MDV_TMP;V_ScalVect(V1,V1,MDV_TMP);V_HardCheckNorm(MDV_TMP);RES=sqrt(MDV_TMP);}
#define V_ModVect_Unsafe(V1,RES) {float MDV_TMP;V_ScalVect(V1,V1,MDV_TMP);RES=sqrt(MDV_TMP);}
#define V_InvModVect(V1,RES) {float MDV_TMP;V_ScalVect(V1,V1,MDV_TMP);V_HardCheckNorm(MDV_TMP);RES=1.0f/sqrt(MDV_TMP);}

#define V_Mod2DiffVect(V1,V2,RES) {RES=(V2.x-V1.x)*(V2.x-V1.x);float M2FV_TMP1=(V2.y-V1.y)*(V2.y-V1.y);float M2FV_TMP2=(V2.z-V1.z)*(V2.z-V1.z);RES+=M2FV_TMP1;RES+=M2FV_TMP2;V_HardCheckNorm(RES);}
#define V_Mod2DiffVect_Unsafe(V1,V2,RES) {RES=(V2.x-V1.x)*(V2.x-V1.x);float M2FV_TMP1=(V2.y-V1.y)*(V2.y-V1.y);float M2FV_TMP2=(V2.z-V1.z)*(V2.z-V1.z);RES+=M2FV_TMP1;RES+=M2FV_TMP2;}
#define V_ModDiffVect(V1,V2,RES) {float MFV_TMP;V_Mod2DiffVect(V1,V2,MFV_TMP);RES=sqrt(MFV_TMP);}
#define V_ModDiffVect_Unsafe(V1,V2,RES) {float MFV_TMP;V_Mod2DiffVect_Unsafe(V1,V2,MFV_TMP);RES=sqrt(MFV_TMP);}
//sqrt((V2.x-V1.x)*(V2.x-V1.x)+(V2.y-V1.y)*(V2.y-V1.y)+(V2.z-V1.z)*(V2.z-V1.z))

//calcule le projete absolu d'un vecteur (FORCE) (ne tient pas compte de la position du repere)
#define V_ProjAbs(Rep,V,Absolu) V_Operate4(Absolu,=V.x*Rep.axex,+V.y*Rep.axey,+V.z*Rep.axez);
//calcule la position absolue d'un point a partir de ses coordonnees relatives
#define V_CalcAbs(Rep,V,Absolu) V_Operate5(Absolu,=Rep.pos,+V.x*Rep.axex,+V.y*Rep.axey,+V.z*Rep.axez);

#define V_NormaliseSafe(V) {float V_N;V_Mod2Vect(V,V_N); if(V_N>0) {float V_U=1.0f/sqrt(V_N); V_Operate1(V,*=V_U);} else V_SetXYZ(V,0,0,0);}
#define V_NormalizeSafe(V) V_NormaliseSafe(V)
#define V_Normalise(V) {float V_N;V_InvModVect(V,V_N);V_Operate1(V,*=V_N);}
#define V_Normalize(V) V_Normalise(V)

//buggee V_Orthonorme(Rep) {V_Normalise(Rep.axex);{float s1=V_ScalVect(Rep.axex,Rep.axey);V_Operate2(Rep.axey,-=s1*Rep.axex);}V_Normalise(Rep.axey);{float s1=V_ScalVect(Rep.axex,Rep.axez);V_Operate2(Rep.axez,-=s1*Rep.axex);}{float s1=V_ScalVect(Rep.axey,Rep.axez);V_Operate2(Rep.axez,-=s1*Rep.axey);}V_Normalise(Rep.axez);}
#define V_Orthonorme(Rep) {V_Normalise(Rep.axex);{float s1=Rep.axex.x*Rep.axey.x;float s2=Rep.axex.x*Rep.axez.x;s1+=Rep.axex.y*Rep.axey.y;s2+=Rep.axex.y*Rep.axez.y;s1+=Rep.axex.z*Rep.axey.z;s2+=Rep.axex.z*Rep.axez.z;Rep.axey.x-=s1*Rep.axex.x;Rep.axez.x-=s2*Rep.axex.x;Rep.axey.y-=s1*Rep.axex.y;Rep.axez.y-=s2*Rep.axex.y;Rep.axey.z-=s1*Rep.axex.z;Rep.axez.z-=s2*Rep.axex.z;};V_Normalise(Rep.axey);{float s1;V_ScalVect(Rep.axey,Rep.axez,s1);V_Operate2(Rep.axez,-=s1*Rep.axey);}V_Normalise(Rep.axez);}
#define V_OrthonormeSafe(Rep) {V_NormaliseSafe(Rep.axex);{float s1=Rep.axex.x*Rep.axey.x;float s2=Rep.axex.x*Rep.axez.x;s1+=Rep.axex.y*Rep.axey.y;s2+=Rep.axex.y*Rep.axez.y;s1+=Rep.axex.z*Rep.axey.z;s2+=Rep.axex.z*Rep.axez.z;Rep.axey.x-=s1*Rep.axex.x;Rep.axez.x-=s2*Rep.axex.x;Rep.axey.y-=s1*Rep.axex.y;Rep.axez.y-=s2*Rep.axex.y;Rep.axey.z-=s1*Rep.axex.z;Rep.axez.z-=s2*Rep.axex.z;};V_NormaliseSafe(Rep.axey);{float s1;V_ScalVect(Rep.axey,Rep.axez,s1);V_Operate2(Rep.axez,-=s1*Rep.axey);}V_NormaliseSafe(Rep.axez);}

#define V_VectVect(V1, V2, V_VRES) {V_VRES.x=V1.y*V2.z-V1.z*V2.y;V_VRES.y=V1.z*V2.x-V1.x*V2.z;V_VRES.z=V1.x*V2.y-V1.y*V2.x;}
#define V_TranslateRepS(Rep, Transl) V_Operate4(Rep.pos,+= Transl.x*Rep.axex,+ Transl.y*Rep.axey,+ Transl.z*Rep.axez);
#define V_RotateRepS(Rep, Rotat) {V_REPERE NewRep=Rep;V_Operate3(NewRep.axex,+= Rotat.z*Rep.axey,- Rotat.y*Rep.axez);V_Operate3(NewRep.axey,+= Rotat.x*Rep.axez,- Rotat.z*Rep.axex);V_Operate3(NewRep.axez,+= Rotat.y*Rep.axex,- Rotat.x*Rep.axey);V_Orthonorme(NewRep);Rep=NewRep;}
#define V_TranslateRep(Rep, O, Transl) V_Operate4(Rep.pos,+= O Transl.x*Rep.axex,+ O Transl.y*Rep.axey,+ O Transl.z*Rep.axez);
#define V_RotateRep(Rep, O, Rotat) {V_REPERE NewRep=Rep;V_Operate3(NewRep.axex,+= O Rotat.z*Rep.axey,- O Rotat.y*Rep.axez);V_Operate3(NewRep.axey,+= O Rotat.x*Rep.axez,- O Rotat.z*Rep.axex);V_Operate3(NewRep.axez,+= O Rotat.y*Rep.axex,- O Rotat.x*Rep.axey);V_Orthonorme(NewRep);Rep=NewRep;}
#define V_ProjRep(Rep,V,V_Res) {V_ScalVect(V,Rep.axex,V_Res.x);V_ScalVect(V,Rep.axey,V_Res.y);V_ScalVect(V,Rep.axez,V_Res.z);}
#define V_CalcRep(Rep,V,V_Res) {V_VECT Absolu;V_Operate3(Absolu,=V,-Rep.pos);V_ScalVect(Absolu,Rep.axex,V_Res.x);V_ScalVect(Absolu,Rep.axey,V_Res.y);V_ScalVect(Absolu,Rep.axez,V_Res.z);}
#define V_CalcRepRep(Rep,V,NewRep,NewV) {V_VECT Absolu;V_CalcAbs(Rep, V, Absolu);V_VECT T;V_Operate3(T,=Absolu,-NewRep.pos);V_ScalVect(T,NewRep.axex,NewV.x);V_ScalVect(T,NewRep.axey,NewV.y);V_ScalVect(T,NewRep.axez,NewV.z);}

//Macros declarant des operateurs assouplis pour les struct(enum)
#define UOP_ENUM_C1(T) T() { return; }
#define UOP_ENUM_C2(T) T(const ENUM e) { this->e=e; return; }
#define UOP_ENUM_OR(T) __inline T& operator|=(const ENUM& right)	{	(*(int*)this) |= (*(int*)&right);	return *this;	}
#define UOP_ENUM_AND(T) __inline T& operator&=(const ENUM& right)	{	(*(int*)this) &= (*(int*)&right);	return *this;	}
#define UOP_ENUM_EQU(T) __inline T& operator=(const ENUM& right)	{	(*(int*)this) = (*(int*)&right);	return *this;	}
#define UOP_ENUM_ISEQU(T) __inline int operator==(const ENUM& right)	{	return ((*(int*)this) == (*(int*)&right));	}
#define UOP_ENUM_ISNEQU(T) __inline int operator!=(const ENUM& right)	{	return ((*(int*)this) != (*(int*)&right));	}
#define OP_ENUM_OR(T) __inline T operator|(const ENUM& right)	{	T result; (*(int*)&result) = (*(int*)this)|(*(int*)&right); return result;	}
#define OP_ENUM_AND(T) __inline T operator&(const ENUM& right)	{	T result; (*(int*)&result) = (*(int*)this)&(*(int*)&right); return result;	}

#define SOP_ENUM_OR(T) __inline T::ENUM operator|(const T::ENUM& left,const T::ENUM& right) { T::ENUM result; (*(int*)&result) = (*(int*)&left)|(*(int*)&right); return result; }
#define SOP_ENUM_NOT(T) __inline T::ENUM operator~(const T::ENUM& that) { T::ENUM result; (*(int*)&result) = ~(*(int*)&that); return result; }

#define OP_ENUM_DECL(T) UOP_ENUM_C1(T); UOP_ENUM_C2(T); UOP_ENUM_OR(T); UOP_ENUM_AND(T); UOP_ENUM_EQU(T); UOP_ENUM_ISEQU(T); UOP_ENUM_ISNEQU(T); OP_ENUM_OR(T); OP_ENUM_AND(T);
#define OP_ENUM_STATICDECL(T) SOP_ENUM_OR(T); SOP_ENUM_NOT(T);
#define enum_test(x,n) ((*(int*)&x)&((int)x.n))

int SPG_CONV V_BitReverse(int x, int nb);



