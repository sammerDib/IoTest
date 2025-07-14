// Dib.h: interface for the CDib class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_DIB_H__AE3C409B_670A_4AA5_948C_5DC8AB69BF20__INCLUDED_)
#define AFX_DIB_H__AE3C409B_670A_4AA5_948C_5DC8AB69BF20__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "H3Array2D.h"

#ifndef _INC_DIB
#define _INC_DIB

/* DIB constants */
#define PALVERSION   0x300

/* Dib Header Marker - used in writing DIBs to files */
#define DIB_HEADER_MARKER   ((WORD) ('M' << 8) | 'B')

/* DIB Macros*/
#define RECTWIDTH(lpRect)     ((lpRect)->right - (lpRect)->left)
#define RECTHEIGHT(lpRect)    ((lpRect)->bottom - (lpRect)->top)

// WIDTHBYTES performs DWORD-aligning of DIB scanlines.  The "bits"
// parameter is the bit count for the scanline (biWidth * biBitCount),
// and this macro returns the number of DWORD-aligned bytes needed
// to hold those bits.

#define WIDTHBYTES(bits)    (((bits) + 31) / 32 * 4)

class CDib : public CObject
{
	DECLARE_DYNAMIC(CDib)

// Constructors
public:
	CDib();

// Attributes
protected:
	LPBYTE m_pBits;
	LPBITMAPINFO m_pBMI;
	CPalette* m_pPalette;

public:
	DWORD Width()     const;
	DWORD Height()    const;
	WORD  NumColors() const;
	BOOL  IsValid()   const { return (m_pBMI != nullptr); }

// Operations
public:

	bool Set(unsigned short *pData,long nSizeX,long nSizeY,long nPitch, long nLow, long nHigh);
	bool Set(const H3_ARRAY2D_RGB24 &SrcBuf);
	bool Set(const H3_ARRAY2D_FLT32 &SrcBuf,const H3_ARRAY_FLT32 &Range);

	bool Set(unsigned char *pData,long nSizeX,long nSizeY,long nPitch,H3_UINT8 Low=0,H3_UINT8 High=UCHAR_MAX);
	bool Set(unsigned char *pData,long nSizeX,long nSizeY,long nPitch,H3_ARRAY_FLT32 &Range);
	bool Set(H3_RGB24 *pData,long nSizeX,long nSizeY,long nPitch);


	bool SetColorMaps(H3_ARRAY2D_UINT8 &ColorMap1,H3_ARRAY2D_UINT8 &ColorMap2);
	BOOL Paint(HDC hDC, LPRECT,bool btrue=false,CRect rcRect=CRect(0,0,0,0)) const;
	BOOL  Paint(HDC, LPRECT, LPRECT,bool btrue=false,CRect rcRect=CRect(0,0,0,0)) const;
	HGLOBAL CopyToHandle()           const;
	DWORD Save(CFile& file)          const;
	DWORD Read(CFile& file);
	DWORD ReadFromHandle(HGLOBAL hGlobal);
	void Invalidate() { Free(); }

	virtual void Serialize(CArchive& ar);

// Implementation
public:
	virtual ~CDib();

protected:
	bool  CreatePalette();
	WORD  PaletteSize() const;
	void Free();

#ifdef _DEBUG
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:
	long m_nUsedSizeX;
	long m_nUsedSizeY;
	bool ReAlloc(long nSizeX, long nSizeY,long nSizeBit);
	bool Alloc(long nSizeX, long nSizeY, long nSizeBit);
};

#endif //!_INC_DIB
#endif // !defined(AFX_DIB_H__AE3C409B_670A_4AA5_948C_5DC8AB69BF20__INCLUDED_)
