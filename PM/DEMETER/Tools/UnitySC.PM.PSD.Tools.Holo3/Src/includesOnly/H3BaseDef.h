/* Modif
ajout le 02/12/10 de: 	H3Replace(str,string("int>"),string("__int32>"));



*/
#ifndef H3BASEDEF__INCLUDED_
#define H3BASEDEF__INCLUDED_

#include "H3Complex.h"
#include <string>

using namespace std;

#define H3_INT8				__int8
#define H3_INT16			__int16
#define H3_INT32			__int32
#define H3_INT64			__int64
#define H3_UINT8			unsigned __int8
#define H3_UINT16			unsigned __int16
#define H3_UINT32			unsigned __int32
#define H3_UINT64			unsigned __int64
#define H3_FLT32			float
#define H3_FLT64			double
#define H3_FLT80			long double
#define H3_CPXFLT32			CH3Complex< H3_FLT32 >
#define H3_CPXFLT64			CH3Complex< H3_FLT64 >

#define H3_ZEROS			0
#define H3_ONES				1

#define H3_TYPE_UNSIGNED	0x0000
#define H3_TYPE_SIGNED		0x0100
#define H3_TYPE_FLOAT		0x0200
#define H3_TYPE_COMPLEX		0x0400

#define H3_TYPE_UINT8		0x0008 + H3_TYPE_UNSIGNED
#define H3_TYPE_UINT16		0x0010 + H3_TYPE_UNSIGNED
#define H3_TYPE_UINT32		0x0020 + H3_TYPE_UNSIGNED

#define H3_TYPE_INT8		0x0008 + H3_TYPE_SIGNED
#define H3_TYPE_INT16		0x0010 + H3_TYPE_SIGNED
#define H3_TYPE_INT32		0x0020 + H3_TYPE_SIGNED

#define H3_TYPE_FLT32		0x0020 + H3_TYPE_FLOAT
#define H3_TYPE_FLT64		0x0040 + H3_TYPE_FLOAT

#define H3_TYPE_CPXFLT32	0x0020 + H3_FLOAT + H3_TYPE_COMPLEX
#define H3_TYPE_CPXFLT64	0x0040 + H3_FLOAT + H3_TYPE_COMPLEX

#define H3_TYPE_RGB24		0x1000 + H3_TYPE_UINT8
//#define H3_TYPE_PT2DFLT32	0x000 + H3_TYPE_FLT32    //0x0120 + 0x0200							 
//#define H3_TYPE_PT3DFLT32	0x0200 + H3_TYPE_FLT32    //0x0220 + 0x0200
//NaN
#include <limits>
static float H3GetFPNaN()
{
	//1 bit de signe à 0 ou 1,
	//8 bits d'exposant à 1,
	//23 bits de mantisse, mantisse non nulle(mantisse nulle pour infini)
	//si la mantisse commence par 1: Quiet NaN
	//si la mantisse commence par 0: Signal NaN (erreur)
	float Value;
	long *pf=(long *)&Value;
	*pf=0x7FC00000;

	return Value;
}

static double H3GetFPdNaN()
{
	//1 bit de signe à 0 ou 1,
	//11 bits d'exposant à 1,(7FF)
	//52 bits de mantisse, mantisse non nulle(mantisse nulle pour infini)
	//si la mantisse commence par 1: Quiet NaN
	//si la mantisse commence par 0: Signal NaN (erreur)
	//ATTENTION: NaN peut etre > ou < à un double avec Visual C++ au 19/04/07
	double Value;
	//solution 1
	//hyper *pf=(hyper*)&Value;
	//*pf=0x7FFFFFFFFFFFFFFF;
	//*pf=0x7FF8000000000000;
	//*pf=0xffffffff7fffffff;

	//solution 2
	//Value=numeric_limits<double>::quiet_NaN();//(Value>PI) est true

	//solution 3
	unsigned long nan[2]={0xffffffff, 0x7fffffff};
	Value = *( double* )nan;
	return Value;
}

const double H3dNaN=H3GetFPdNaN();
const float  H3fNaN=H3GetFPNaN();
const float  H3NaN=H3GetFPNaN();

//pour compatibilités prgm CV
const double dNaN=H3GetFPdNaN();
const float  NaN=H3GetFPNaN();

const double PI=3.14159265358979;
const double TWO_PI=6.28318530717959;
const float fPI=(3.1415927f);
const float fTWO_PI=(6.2831853f);

// Remplace dans la chaine str1 toutes les sous chaines str2 par str3
// Auteur : EC
// Date :03/08/07
static void H3Replace(string &str1, const string &str2, const string &str3)
{
	size_t n;//long n=-1;
	while ((n=str1.find(str2))!=string::npos)//>0)
	{
		str1.replace(n,str2.length(),str3);
	}
}

// Remplace dans la chaine de caractere passee en parametre :
//	"char" par "__int8"
//	"short" par "__int16"
// Auteur : EC
// Date : 03/08/07
 static void H3ChangeTypeIdName(string &str)
{
	H3Replace(str,string("char"),string("__int8"));
	H3Replace(str,string("short"),string("__int16"));
	H3Replace(str,string("int>"),string("__int32>"));

	//eliminer ' const ' si cette chaine apparait dans le nom
	//par ex:  'class CH3Array2D<class CH3Point3D<float> > const *'
	//sera remplacé par 'class CH3Array2D<class CH3Point3D<float> > *'
	H3Replace(str,string(" const "),string(" "));//cv 200308
}

#endif
