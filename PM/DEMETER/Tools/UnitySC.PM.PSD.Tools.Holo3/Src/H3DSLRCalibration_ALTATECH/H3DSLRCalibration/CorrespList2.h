// CorrespList2.h: interface for the CCorrespList2 class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_CORRESPLIST2_H__A8E7FE80_832A_11D8_BF05_00095B087A04__INCLUDED_)
#define AFX_CORRESPLIST2_H__A8E7FE80_832A_11D8_BF05_00095B087A04__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "Rotation.h"

#define CL_TYPE double
#define CL_ARRAY   CH3Array< CL_TYPE >
#define CL_ARRAY2D CH3Array2D< CL_TYPE >
#define CL_MATRIX  CH3Matrix< CL_TYPE >

class CCorrespList2  
{
public:
	CCorrespList2();
	CCorrespList2(const CL_ARRAY2D& Pix, const CL_ARRAY2D& Metric);
	virtual ~CCorrespList2();

	bool Load(const CString FileName);
	bool CheckPlanarity();
	bool SetPix(const H3_ARRAY2D_FLT64& Pix);
	bool SetMetric(const H3_ARRAY2D_FLT64& Metric);
	void operator=(const CCorrespList2& CL);

public:
	bool m_bInitialised;
	bool m_bHinitialised;
	size_t m_nTarget;
	CL_ARRAY2D Pixel;
	CL_ARRAY2D MetricData;

	//fonction du constructeur
	CString ImageName;

	//elts valides (finis)
	CL_ARRAY2D validPixel;
	CL_ARRAY2D validMetricData;

	//initialisés par CheckPlanarity
	bool m_3D;//1 si mire 3D
	CL_MATRIX m_V;//un elt calculé en meme temps que la planeité et utile par ailleurs
	CL_ARRAY2D  m_MetricMean; //point moyen

	//initialisé lors du calcul des parametres extrinseque
	//fonction de la liste et d'une camera
	CL_ARRAY2D Err;	//erreur//initialisé quand les param extrinseques sont calculés	
	CRotation H;
};

#endif // !defined(AFX_CORRESPLIST2_H__A8E7FE80_832A_11D8_BF05_00095B087A04__INCLUDED_)
