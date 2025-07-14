// UWImage.h: interface for the UWImage class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_UWIMAGE_H__65E0B466_8301_4B14_9A46_800FF5F88D00__INCLUDED_)
#define AFX_UWIMAGE_H__65E0B466_8301_4B14_9A46_800FF5F88D00__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "H3Point2D.h"

class UWImage  
{
public:
	UWImage();
	UWImage(const H3_ARRAY2D_FLT32& PhaseX,const H3_ARRAY2D_FLT32& PhaseY);
	UWImage & operator=(const UWImage& UW);
	void Copy(const UWImage & UW);	
	UWImage(const UWImage & UW);

	virtual ~UWImage();

	H3_ARRAY2D_FLT32 GetX()const;
	H3_ARRAY2D_FLT32 GetY()const;
	H3_ARRAY2D_UINT8 GetMask()const;

	unsigned long GetLi()const;
	unsigned long GetCo()const; 

public:
	H3_ARRAY2D_FLT32 m_aX;
	H3_ARRAY2D_FLT32 m_aY;
	H3_ARRAY2D_UINT8 m_aMask;
	bool m_bIsInitialised;
};

#endif // !defined(AFX_UWIMAGE_H__65E0B466_8301_4B14_9A46_800FF5F88D00__INCLUDED_)
