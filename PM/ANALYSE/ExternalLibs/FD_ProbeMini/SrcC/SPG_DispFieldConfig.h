
//#define DF_CheckFloatNZ(x,Msg) CHECK((_finite(x)==0)||(fabsf(x)>1.0e6f)||(fabsf(x)<1.0e-6f),Msg,;)
//#define DF_CheckFloatS(x,Msg) CHECK((_finite(x)==0)||(fabsf(x)>1.0e15f),Msg,;)
//#define DF_CheckFloat(x,Msg) CHECK((_finite(x)==0)||(fabsf(x)>1.0e10f),Msg,;)
#define DF_CheckFloatNZ(x,Msg)
#define DF_CheckFloatS(x,Msg)
#define DF_CheckFloat(x,Msg)

//#define DF_PREFILTER
//#define DF_FASTEST
//typedef camTypPixel SPG_CAMPIXEL;

#if(DF_VERSION>=4)
//#define DF_NOCALIBR
//#define DF_SAVEGRADINFO
//#define DF_CALIBRATIONSCALE 1
#define DF_CALIBRATIONSCALE ((DF.PX->Oversampling+1)/2)
//#define DF_CALIBRATIONSCALE (DF.PX->Oversampling-1)
#else
#define DF_CALIBRATIONSCALE 1
#endif

//#define DISPSATURATION 2.5
#undef DISPSATURATION

//flags de debogage
//#define DF_SAVECALIBR
//#define DF_SAVEKERNEL


#if(DF_VERSION==3)
#define DF_GRADSAMPLING PIXINT_RELAX_OVERSAMPLING
#endif

//change le sens des vecteurs vitesse en permuttant les define
#define SPG_DF_SENSNEG(v) (-(v))
#define SPG_DF_SENSPOS(v) (v)
