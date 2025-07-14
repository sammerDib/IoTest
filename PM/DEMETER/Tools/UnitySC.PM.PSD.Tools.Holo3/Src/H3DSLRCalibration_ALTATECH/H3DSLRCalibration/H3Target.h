// Target.h: interface for the CTarget class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_TARGET_H__5F69F3E4_8255_11D8_BF05_00095B087A04__INCLUDED_)
#define AFX_TARGET_H__5F69F3E4_8255_11D8_BF05_00095B087A04__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "H3ARRAY2D.h"

#define XML_FILE 0

#if XML_FILE
	#include "H3XMLFile.h"
#endif

#ifdef _DLL
#  ifdef __H3_DSLR_CALIBRATION__
#    define H3TARGET_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3TARGET_EXPORT_DECL __declspec(dllimport)
#  endif
#else
#  define H3TARGET_EXPORT_DECL
#endif

#define TARGET_CIRCLE 0
#define TARGET_SQUARE 1
#define TARGET_LINES 2

struct PARAM_TARGET_STRUCT
{

	long nWintX;
	long nWintY;

	float fdefaultCoeffFilter;

	float fDefaultSeuil2;
	float fDefaultSeuil3;
	float fDefaultN;

	long  nCornerFinderFirstGuessStepSE;
	long  nCornerFinderFirstGuessN;
	float fCornerFinderResolution;
	long  nCornerFinderMaxIter;
	float fCornerFinderSeuil;

	long nba;

};

#include "SortFct.h"

class H3TARGET_EXPORT_DECL CH3Target  
{
public:
	bool m_2D;
	bool CheckPlanarity(H3_ARRAY2D_FLT32& X);

#if XML_FILE
	BOOL LoadCalib(H3XMLFile* file,int Indice);
	BOOL SaveCalib(H3XMLFile* file,int Indice);
#else
	BOOL LoadCalib(CString strFileName,int Indice);
	BOOL SaveCalib(CString strFileName,int Indice);
#endif

	H3_ARRAY2D_PT2DFLT32 Find(const H3_ARRAY2D_UINT8& Image, short TargetType, bool OriginOnTarget=true);
	H3_ARRAY2D_PT2DFLT32 Find(const H3_ARRAY2D_FLT32& Image, short TargetType);
	CH3Target(const CString& FileName,int CamNum=0);
	virtual ~CH3Target();
	PARAM_TARGET_STRUCT m_param;
#if XML_FILE
	H3XMLFile* file;
#endif
};

#endif // !defined(AFX_TARGET_H__5F69F3E4_8255_11D8_BF05_00095B087A04__INCLUDED_)
