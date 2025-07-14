#pragma once
#include "H3IOHoloMAP.h"
//class CImageByte;

class CImageFlt32
{
public:
	CImageFlt32(unsigned long _ny=0L,	unsigned long _nx=0L,	float*  pf = NULL );
	virtual ~CImageFlt32(void);

	bool ReAlloc(unsigned long _ny=0L,	unsigned long _nx=0L,	float* pf = NULL );
	bool GetStat(float& min,float& max,float& mean, float& std, const CImageByte* const pMask)const;

	long nx;
	long ny;
	float * m_pData;
	bool m_bExternInitialisation;
	bool H3SaveData(LPCTSTR strFileName) const;
};

//class CImageByte
//{
//public:
//	CImageByte(unsigned long _ny=0L,	unsigned long _nx=0L, BYTE*  pf = NULL );
//	virtual ~CImageByte(void);
//
//	bool ReAlloc(unsigned long _ny=0L,	unsigned long _nx=0L, BYTE* pf = NULL );
//	void Fill(const BYTE b);
//
//	long nx;
//	long ny;
//	BYTE * m_pData;
//	bool m_bExternInitialisation;
//
//};
