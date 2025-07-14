// Dib.cpp: implementation of the CDib class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "H3DisplayTools.h"
#include "Dib.h"
#include <math.h>

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

/////////////////////////////////////////////////////////////////////////////
// Copyright (C) 1998 by Jorge Lodos
// All rights reserved
//
// Distribute and use freely, except:
// 1. Don't alter or remove this notice.
// 2. Mark the changes you made
//
// Send bug reports, bug fixes, enhancements, requests, etc. to:
//    lodos@cigb.edu.cu
/////////////////////////////////////////////////////////////////////////////

//  dib.cpp
//

#include <windowsx.h>
#include <afxadv.h>
#include <io.h>
#include <errno.h>

/////////////////////////////////////////////////////////////////////////////
// CDib

static CString strModule("CDib");

IMPLEMENT_DYNAMIC(CDib, CObject)

CDib::CDib()
{
	m_pBMI = nullptr;
	m_pBits = nullptr;
	m_pPalette = nullptr;
}

CDib::~CDib()
{
	Free();
}

void CDib::Free()
{
	// Make sure all member data that might have been allocated is freed.
	if (m_pBits) { GlobalFreePtr(m_pBits);m_pBits = nullptr;}
	if (m_pBMI) {GlobalFreePtr(m_pBMI);m_pBMI = nullptr;}
	if (m_pPalette) {m_pPalette->DeleteObject();delete m_pPalette;m_pPalette = nullptr;}
}

BOOL CDib::Paint(HDC hDC, LPRECT lpDCRect,bool btrue,CRect rcRect) const
{
	if (!m_pBMI)
		return FALSE;

	CRect rcDib(
		CPoint(0,0),
		CPoint(Width()-1,Height()-1));
	if(rcRect.Height() == 0,rcRect.Width() == 0)
	{
		rcRect=rcDib;
		btrue=false;
	}
//	else
//		rcDib=rcRect;

	return Paint(hDC,lpDCRect,rcDib,btrue,rcRect);
}

/*************************************************************************
 *
 * Paint()
 *
 * Parameters:
 *
 * HDC hDC          - DC to do output to
 *
 * LPRECT lpDCRect  - rectangle on DC to do output to
 *
 * LPRECT lpDIBRect - rectangle of DIB to output into lpDCRect
 *
 * CPalette* pPal   - pointer to CPalette containing DIB's palette
 *
 * Return Value:
 *
 * BOOL             - TRUE if DIB was drawn, FALSE otherwise
 *
 * Description:
 *   Painting routine for a DIB.  Calls StretchDIBits() or
 *   SetDIBitsToDevice() to paint the DIB.  The DIB is
 *   output to the specified DC, at the coordinates given
 *   in lpDCRect.  The area of the DIB to be output is
 *   given by lpDIBRect.
 *
 ************************************************************************/
BOOL CDib::Paint(HDC hDC, LPRECT lpDCRect, LPRECT lpDIBRect,bool btrue,CRect rcRect) const
{


	if (!m_pBMI)
		return FALSE;

	HPALETTE hPal = NULL;           // Our DIB's palette
	HPALETTE hOldPal = NULL;        // Previous palette

	// Get the DIB's palette, then select it into DC
	if (m_pPalette != nullptr)
	{
		hPal = (HPALETTE) m_pPalette->m_hObject;

		// Select as background since we have
		// already realized in forground if needed
		hOldPal = ::SelectPalette(hDC, hPal, TRUE);
	}

	// Make sure to use the stretching mode best for color pictures 
	::SetStretchBltMode(hDC, COLORONCOLOR);

	// Determine whether to call StretchDIBits() or SetDIBitsToDevice() 
	BOOL bSuccess;

	if ((RECTWIDTH(lpDCRect)  == RECTWIDTH(lpDIBRect)) &&
	   (RECTHEIGHT(lpDCRect) == RECTHEIGHT(lpDIBRect)))
	{

		bSuccess = ::SetDIBitsToDevice(
			hDC,        // hDC
			lpDCRect->left,					// DestX
			lpDCRect->top,					// DestY
			RECTWIDTH(lpDCRect),			// nDestWidth
			RECTHEIGHT(lpDCRect),			// nDestHeight
			lpDIBRect->left,				// SrcX
			(int)Height() -
			lpDIBRect->top -
			RECTHEIGHT(lpDIBRect),			// SrcY
			0,								// nStartScan
			(WORD)Height(),					// nNumScans
			m_pBits,						// lpBits
			m_pBMI,							// lpBitsInfo
			DIB_RGB_COLORS);				// wUsage


	}
	else
	{
		if (btrue==false)
		{
			bSuccess = ::StretchDIBits(
				hDC,							// hDC
				lpDCRect->left,					// DestX
				lpDCRect->top,					// DestY
				RECTWIDTH(lpDCRect),			// nDestWidth
				RECTHEIGHT(lpDCRect),			// nDestHeight
				lpDIBRect->left,				// SrcX
				lpDIBRect->top,					// SrcY
				RECTWIDTH(lpDIBRect)+1,			// wSrcWidth
				RECTHEIGHT(lpDIBRect)+1,		// wSrcHeight
				m_pBits,						// lpBits
				m_pBMI,							// lpBitsInfo
				DIB_RGB_COLORS,					// wUsage
				SRCCOPY);						// dwROP
		}
		else
		{
			bSuccess = ::StretchDIBits(
				hDC,							// hDC
				lpDCRect->left,					// DestX
				lpDCRect->top,					// DestY
				RECTWIDTH(lpDCRect),			// nDestWidth
				RECTHEIGHT(lpDCRect),			// nDestHeight
				rcRect.left,
				rcRect.top,
				rcRect.Width(),
				rcRect.Height(),
				m_pBits,						// lpBits
				m_pBMI,							// lpBitsInfo
				DIB_RGB_COLORS,					// wUsage
				SRCCOPY);		
			// dwROP
/*
			bSuccess = ::StretchDIBits(
				hDC,									// hDC
				lpDCRect->left+RECTWIDTH(lpDCRect)/4,	// DestX
				lpDCRect->top,						// DestY
				RECTWIDTH(lpDCRect)*3/4,				// nDestWidth
				RECTHEIGHT(lpDCRect)*1/4,				// nDestHeight
				rcRect.left+rcRect.Width()/4, 
				rcRect.top+rcRect.Height()*3/4,
				rcRect.Width()*3/4,
				rcRect.Height()*1/4,
				m_pBits,						// lpBits
				m_pBMI,							// lpBitsInfo
				DIB_RGB_COLORS,					// wUsage
				SRCCOPY);		
			// dwROP
			bSuccess = ::StretchDIBits(
				hDC,									// hDC
				lpDCRect->left,							// DestX
				lpDCRect->top+RECTHEIGHT(lpDCRect)/4,	// DestY
				RECTWIDTH(lpDCRect),					// nDestWidth
				RECTHEIGHT(lpDCRect)*3/4,					// nDestHeight
				rcRect.left,
				rcRect.top,
				rcRect.Width(),
				rcRect.Height()*3/4,
				m_pBits,						// lpBits
				m_pBMI,							// lpBitsInfo
				DIB_RGB_COLORS,					// wUsage
				SRCCOPY);		
			// dwROP
*/
			bSuccess = ::StretchDIBits(
				hDC,							// hDC
				lpDCRect->left,					// DestX
				lpDCRect->top,					// DestY
				RECTWIDTH(lpDCRect)/4,			// nDestWidth
				RECTHEIGHT(lpDCRect)/4,			// nDestHeight
				lpDIBRect->left,				// SrcX
				lpDIBRect->top,					// SrcY
				RECTWIDTH(lpDIBRect)+1,			// wSrcWidth
				RECTHEIGHT(lpDIBRect)+1,		// wSrcHeight
				m_pBits,						// lpBits
				m_pBMI,							// lpBitsInfo
				DIB_RGB_COLORS,					// wUsage
				SRCCOPY);						// dwROP


			CDC *pDC =CDC::FromHandle (hDC);

			CPen myPen;
			CPen* pOldPen;
			LOGPEN penparms;

			penparms.lopnStyle = BS_SOLID;
			penparms.lopnColor = RGB(255,255,0);
			penparms.lopnWidth.x = 1;
			penparms.lopnWidth.y = 1;
			myPen.CreatePenIndirect(&penparms);

			CBrush myBrush;
			CBrush* pOldBrush;
			LOGBRUSH brushparms;

			brushparms.lbStyle = BS_HOLLOW;
			brushparms.lbColor = RGB(255,255,0);
			myBrush.CreateBrushIndirect(&brushparms);

			pOldPen = pDC->SelectObject(&myPen);
			pOldBrush = pDC->SelectObject(&myBrush);

			//Draw the Object

			pDC->Rectangle(lpDCRect->left,lpDCRect->top,lpDCRect->left+(RECTWIDTH(lpDCRect)+1)/4,lpDCRect->top+(RECTHEIGHT(lpDCRect)+1)/4);

			pDC->Rectangle(
				lpDCRect->left+((rcRect.left*(RECTWIDTH(lpDCRect))/RECTWIDTH(lpDIBRect)))/4,
				lpDCRect->top+((RECTHEIGHT(lpDCRect))-(rcRect.top*(RECTHEIGHT(lpDCRect))/RECTHEIGHT(lpDIBRect)))/4,
				lpDCRect->left+((rcRect.right*(RECTWIDTH(lpDCRect))/RECTWIDTH(lpDIBRect)))/4,
				lpDCRect->top+((RECTHEIGHT(lpDCRect))-(rcRect.bottom*(RECTHEIGHT(lpDCRect))/RECTHEIGHT(lpDIBRect)))/4);


			pDC->SelectObject(pOldPen); // Deselect Pen
			pDC->SelectObject(pOldBrush); // Deselect brush

			myPen.DeleteObject();
			pOldPen->DeleteObject();
			myBrush.DeleteObject();
			pOldBrush->DeleteObject();

		}
	}

	// Reselect old palette 
	if (hOldPal != NULL)
	{
		::SelectPalette(hDC, hOldPal, TRUE);
	}

   return bSuccess;
}

/*
BOOL CDib::Paint(HDC hDC, LPRECT lpDCRect, LPRECT lpDIBRect) const
{
	if (!m_pBMI)
		return FALSE;

	HPALETTE hPal = NULL;           // Our DIB's palette
	HPALETTE hOldPal = NULL;        // Previous palette

	// Get the DIB's palette, then select it into DC
	if (m_pPalette != NULL)
	{
		hPal = (HPALETTE) m_pPalette->m_hObject;

		// Select as background since we have
		// already realized in forground if needed
		hOldPal = ::SelectPalette(hDC, hPal, TRUE);
	}

	// Make sure to use the stretching mode best for color pictures 
	::SetStretchBltMode(hDC, COLORONCOLOR);

	// Determine whether to call StretchDIBits() or SetDIBitsToDevice() 
	BOOL bSuccess;

	if ((RECTWIDTH(lpDCRect)  == RECTWIDTH(lpDIBRect)) &&
	   (RECTHEIGHT(lpDCRect) == RECTHEIGHT(lpDIBRect)))
		bSuccess = ::SetDIBitsToDevice(hDC,        // hDC
								   lpDCRect->left,             // DestX
								   lpDCRect->top,              // DestY
								   RECTWIDTH(lpDCRect),        // nDestWidth
								   RECTHEIGHT(lpDCRect),       // nDestHeight
								   lpDIBRect->left,            // SrcX
								   (int)Height() -
									  lpDIBRect->top -
									  RECTHEIGHT(lpDIBRect),     // SrcY
								   0,                          // nStartScan
								   (WORD)Height(),             // nNumScans
								   m_pBits,                    // lpBits
								   m_pBMI,                     // lpBitsInfo
								   DIB_RGB_COLORS);            // wUsage
   else
	  bSuccess = ::StretchDIBits(hDC,            // hDC
							   lpDCRect->left,               // DestX
							   lpDCRect->top,                // DestY
							   RECTWIDTH(lpDCRect),          // nDestWidth
							   RECTHEIGHT(lpDCRect),         // nDestHeight
							   lpDIBRect->left,              // SrcX
							   lpDIBRect->top,               // SrcY
							   RECTWIDTH(lpDIBRect),         // wSrcWidth
							   RECTHEIGHT(lpDIBRect),        // wSrcHeight
							   m_pBits,                      // lpBits
							   m_pBMI,                       // lpBitsInfo
							   DIB_RGB_COLORS,               // wUsage
							   SRCCOPY);                     // dwROP

	// Reselect old palette 
	if (hOldPal != NULL)
	{
		::SelectPalette(hDC, hOldPal, TRUE);
	}

   return bSuccess;
}
*/
/*************************************************************************
 *
 * CreatePalette()
 *
 * Return Value:
 *
 * TRUE if succesfull, FALSE otherwise
 *
 * Description:
 *
 * This function creates a palette from a DIB by allocating memory for the
 * logical palette, reading and storing the colors from the DIB's color table
 * into the logical palette, creating a palette from this logical palette,
 * and then returning the palette's handle. This allows the DIB to be
 * displayed using the best possible colors (important for DIBs with 256 or
 * more colors).
 *
 ************************************************************************/
bool CDib::CreatePalette()
{
	if (!m_pBMI)
		return false;

   //get the number of colors in the DIB
   WORD wNumColors = NumColors();

   if (wNumColors != 0)
   {
		// allocate memory block for logical palette
		HANDLE hLogPal = ::GlobalAlloc(GHND, sizeof(LOGPALETTE) + sizeof(PALETTEENTRY)*wNumColors);

		// if not enough memory, clean up and return NULL
		if (hLogPal == 0)
			return false;

		LPLOGPALETTE lpPal = (LPLOGPALETTE)::GlobalLock((HGLOBAL)hLogPal);

		// set version and number of palette entries
		lpPal->palVersion = PALVERSION;
		lpPal->palNumEntries = (WORD)wNumColors;

		for (int i = 0; i < (int)wNumColors; i++)
		{
			lpPal->palPalEntry[i].peRed   = m_pBMI->bmiColors[i].rgbRed;
			lpPal->palPalEntry[i].peGreen = m_pBMI->bmiColors[i].rgbGreen;
			lpPal->palPalEntry[i].peBlue  = m_pBMI->bmiColors[i].rgbBlue;
			lpPal->palPalEntry[i].peFlags = 0;
		}

		/* create the palette and get handle to it */
		if (m_pPalette)
		{
			m_pPalette->DeleteObject();
			delete m_pPalette;
		}
		m_pPalette = new CPalette;
		bool bResult=false;
		if (m_pPalette->CreatePalette(lpPal))
			bResult=true;

		::GlobalUnlock((HGLOBAL) hLogPal);
		::GlobalFree((HGLOBAL) hLogPal);
		return bResult;
	}

	return true;
}

/*************************************************************************
 *
 * Width()
 *
 * Return Value:
 *
 * DWORD            - width of the DIB
 *
 * Description:
 *
 * This function gets the width of the DIB from the BITMAPINFOHEADER
 * width field 
 *
 ************************************************************************/

DWORD CDib::Width() const
{
	if (!m_pBMI)
		return 0;

	/* return the DIB width */
	return m_pBMI->bmiHeader.biWidth;
}


/*************************************************************************
 *
 * Height()
 *
 * Return Value:
 *
 * DWORD            - height of the DIB
 *
 * Description:
 *
 * This function gets the height of the DIB from the BITMAPINFOHEADER
 * height field 
 *
 ************************************************************************/

DWORD CDib::Height() const
{
	if (!m_pBMI)
		return 0;
	
	/* return the DIB height */
	return m_pBMI->bmiHeader.biHeight;
}


/*************************************************************************
 *
 * PaletteSize()
 *
 * Return Value:
 *
 * WORD             - size of the color palette of the DIB
 *
 * Description:
 *
 * This function gets the size required to store the DIB's palette by
 * multiplying the number of colors by the size of an RGBQUAD 
 *
 ************************************************************************/
WORD CDib::PaletteSize() const
{
	if (!m_pBMI)
		return 0;

	return NumColors() * sizeof(RGBQUAD);
}

/*************************************************************************
 *
 * NumColors()
 *
 * Return Value:
 *
 * WORD             - number of colors in the color table
 *
 * Description:
 *
 * This function calculates the number of colors in the DIB's color table
 * by finding the bits per pixel for the DIB (whether Win3.0 or other-style
 * DIB). If bits per pixel is 1: colors=2, if 4: colors=16, if 8: colors=256,
 * if 24, no colors in color table.
 *
 ************************************************************************/
WORD CDib::NumColors() const
{
	if (!m_pBMI)
		return 0;

	WORD wBitCount;  // DIB bit count

	/*  The number of colors in the color table can be less than 
	 *  the number of bits per pixel allows for (i.e. lpbi->biClrUsed
	 *  can be set to some value).
	 *  If this is the case, return the appropriate value.
	 */

	DWORD dwClrUsed;

	dwClrUsed = m_pBMI->bmiHeader.biClrUsed;
	if (dwClrUsed != 0)
		return (WORD)dwClrUsed;

	/*  Calculate the number of colors in the color table based on
	 *  the number of bits per pixel for the DIB.
	 */
	wBitCount = m_pBMI->bmiHeader.biBitCount;

	/* return number of colors based on bits per pixel */
	switch (wBitCount)
	{
		case 1:
			return 2;

		case 4:
			return 16;

		case 8:
			return 256;

		default:
			return 0;
	}
}

/*************************************************************************
 *
 * Save()
 *
 * Saves the specified DIB into the specified CFile.  The CFile
 * is opened and closed by the caller.
 *
 * Parameters:
 *
 * CFile& file - open CFile used to save DIB
 *
 * Return value: Number of saved bytes or CFileException
 *
 *************************************************************************/
DWORD CDib::Save(CFile& file) const
{
	BITMAPFILEHEADER bmfHdr; // Header for Bitmap file
	DWORD dwDIBSize;

	if (m_pBMI == nullptr)
		return 0;

	// Fill in the fields of the file header

	// Fill in file type (first 2 bytes must be "BM" for a bitmap)
	bmfHdr.bfType = DIB_HEADER_MARKER;  // "BM"

	// Calculating the size of the DIB is a bit tricky (if we want to
	// do it right).  The easiest way to do this is to call GlobalSize()
	// on our global handle, but since the size of our global memory may have
	// been padded a few bytes, we may end up writing out a few too
	// many bytes to the file (which may cause problems with some apps).
	//
	// So, instead let's calculate the size manually (if we can)
	//
	// First, find size of header plus size of color table.  Since the
	// first DWORD in both BITMAPINFOHEADER and BITMAPCOREHEADER conains
	// the size of the structure, let's use this.
	dwDIBSize = *(LPDWORD)&m_pBMI->bmiHeader + PaletteSize();  // Partial Calculation

	// Now calculate the size of the image
	if ((m_pBMI->bmiHeader.biCompression == BI_RLE8) || (m_pBMI->bmiHeader.biCompression == BI_RLE4))
	{
		// It's an RLE bitmap, we can't calculate size, so trust the
		// biSizeImage field
		dwDIBSize += m_pBMI->bmiHeader.biSizeImage;
	}
	else
	{
		DWORD dwBmBitsSize;  // Size of Bitmap Bits only

		// It's not RLE, so size is Width (DWORD aligned) * Height
		dwBmBitsSize = WIDTHBYTES((m_pBMI->bmiHeader.biWidth)*((DWORD)m_pBMI->bmiHeader.biBitCount)) * m_pBMI->bmiHeader.biHeight;
		dwDIBSize += dwBmBitsSize;

		// Now, since we have calculated the correct size, why don't we
		// fill in the biSizeImage field (this will fix any .BMP files which
		// have this field incorrect).
		m_pBMI->bmiHeader.biSizeImage = dwBmBitsSize;
	}

	// Calculate the file size by adding the DIB size to sizeof(BITMAPFILEHEADER)
	bmfHdr.bfSize = dwDIBSize + sizeof(BITMAPFILEHEADER);
	bmfHdr.bfReserved1 = 0;
	bmfHdr.bfReserved2 = 0;

	// * Now, calculate the offset the actual bitmap bits will be in
	// * the file -- It's the Bitmap file header plus the DIB header,
	// * plus the size of the color table.
	 
	bmfHdr.bfOffBits = (DWORD)sizeof(BITMAPFILEHEADER) + m_pBMI->bmiHeader.biSize + PaletteSize();

	// Write the file header
	file.Write((LPSTR)&bmfHdr, sizeof(BITMAPFILEHEADER));
	DWORD dwBytesSaved = sizeof(BITMAPFILEHEADER); 

	// Write the DIB header
	UINT nCount = sizeof(BITMAPINFO) + (NumColors()-1)*sizeof(RGBQUAD);
	dwBytesSaved += nCount; 
	file.Write(m_pBMI, nCount);
	
	// Write the DIB bits
	DWORD dwBytes = m_pBMI->bmiHeader.biBitCount * Width();
  // Calculate the number of bytes per line
	if (dwBytes%32 == 0)
		dwBytes /= 8;
	else
		dwBytes = dwBytes/8 + (32-dwBytes%32)/8 + (((32-dwBytes%32)%8 > 0) ? 1 : 0); 
	nCount = dwBytes * Height();
	dwBytesSaved += nCount; 
	//file.WriteHuge(m_pBits, nCount);
	file.Write(m_pBits, nCount);

	return dwBytesSaved;
}

/*************************************************************************

  Function:  Read (CFile&)

   Purpose:  Reads in the specified DIB file into a global chunk of
			 memory.

   Returns:  Number of read bytes.

*************************************************************************/
DWORD CDib::Read(CFile& file)
{
	// Ensures no memory leaks will occur
	Free();
	
	BITMAPFILEHEADER bmfHeader;

	// Go read the DIB file header and check if it's valid.
	if (file.Read((LPSTR)&bmfHeader, sizeof(bmfHeader)) != sizeof(bmfHeader))
		return 0;
	if (bmfHeader.bfType != DIB_HEADER_MARKER)
		return 0;
	DWORD dwReadBytes = sizeof(bmfHeader);

	// Allocate memory for DIB
	m_pBMI = (LPBITMAPINFO)GlobalAllocPtr(GHND, bmfHeader.bfOffBits-sizeof(BITMAPFILEHEADER) + 256*sizeof(RGBQUAD));
	if (m_pBMI == 0)
		return 0;

	// Read header.
	if (file.Read(m_pBMI, bmfHeader.bfOffBits-sizeof(BITMAPFILEHEADER)) != (UINT)(bmfHeader.bfOffBits-sizeof(BITMAPFILEHEADER)))
	{
		GlobalFreePtr(m_pBMI);
		m_pBMI = nullptr;
		return 0;
	}
	dwReadBytes += bmfHeader.bfOffBits-sizeof(BITMAPFILEHEADER);

	DWORD dwLength = (DWORD)file.GetLength();
	// Go read the bits.
	m_pBits = (LPBYTE)GlobalAllocPtr(GHND, dwLength - bmfHeader.bfOffBits);
	if (m_pBits == 0)
	{
		GlobalFreePtr(m_pBMI);
		m_pBMI = nullptr;
		return 0;
	}
	
//	if (file.ReadHuge(m_pBits, dwLength-bmfHeader.bfOffBits) != (dwLength - bmfHeader.bfOffBits))
	if (file.Read(m_pBits, dwLength-bmfHeader.bfOffBits) != (dwLength - bmfHeader.bfOffBits))
	{
		GlobalFreePtr(m_pBMI);
		m_pBMI = nullptr;
		GlobalFreePtr(m_pBits);
		m_pBits = nullptr;
		return 0;
	}
	dwReadBytes += dwLength - bmfHeader.bfOffBits;

	CreatePalette();
	return dwReadBytes;
}

#ifdef _DEBUG
void CDib::Dump(CDumpContext& dc) const
{
	CObject::Dump(dc);
}
#endif

//////////////////////////////////////////////////////////////////////////
//// Clipboard support

//---------------------------------------------------------------------
//
// Function:   CopyToHandle
//
// Purpose:    Makes a copy of the DIB to a global memory block.  Returns
//             a handle to the new memory block (NULL on error).
//
// Returns:    Handle to new global memory block.
//
//---------------------------------------------------------------------

HGLOBAL CDib::CopyToHandle() const
{
	CSharedFile file;
	try
	{
		if (Save(file)==0)
			return 0;
	}
	catch (CFileException* e)
	{
		e->Delete();
		return 0;
	}
		
	return file.Detach();
}

//---------------------------------------------------------------------
//
// Function:   ReadFromHandle
//
// Purpose:    Initializes from the given global memory block.  
//
// Returns:    Number of read bytes.
//
//---------------------------------------------------------------------

DWORD CDib::ReadFromHandle(HGLOBAL hGlobal)
{
	CSharedFile file;
	file.SetHandle(hGlobal, FALSE);
	DWORD dwResult = Read(file);
	file.Detach();
	return dwResult;
}

//////////////////////////////////////////////////////////////////////////
//// Serialization support

void CDib::Serialize(CArchive& ar) 
{
	CFile* pFile = ar.GetFile();
	ASSERT(pFile != nullptr);
	if (ar.IsStoring())
	{	// storing code
		Save(*pFile);
	}
	else
	{	// loading code
		Read(*pFile);
	}
}

typedef struct 
{
	BYTE b;
	BYTE g;
	BYTE r;
}BGR24;

bool CDib::Set(const H3_ARRAY2D_RGB24 &SrcBuf)
{
	size_t nSizeY=SrcBuf.GetLi();	
	size_t nSizeX=SrcBuf.GetCo();

	if (!ReAlloc(nSizeX,nSizeY,24)) return false;

	long DIBSizeX=m_pBMI->bmiHeader.biWidth;
	long DIBSizeY=m_pBMI->bmiHeader.biHeight;
	long DIBSizeBit=m_pBMI->bmiHeader.biBitCount;
	long DIBSizeByte=DIBSizeBit/8;

	H3_RGB24 * pS=(H3_RGB24 *)SrcBuf.GetData();
	for (size_t y=0;y<nSizeY;y++)
	{
		long Off=(DIBSizeY-1-y)*DIBSizeX*DIBSizeByte;
		BGR24 *pD=(BGR24*)(m_pBits+Off);
		for (size_t x=0;x<nSizeX;x++)
		{
			pD->r=pS->r;
			pD->g=pS->g;
			pD->b=pS->b;
			pD++;pS++;
		}
	}

/*
	if (!(m_pPalette))
	{
		CreatePalette();
	}
*/
	return true;
}

bool CDib::Set(H3_RGB24 *pData,long nSizeX,long nSizeY,long nPitch)
{
	if (!ReAlloc(nSizeX,nSizeY,24))
	{
		return false;
	}

	long DIBSizeX=m_pBMI->bmiHeader.biWidth;
	long DIBSizeY=m_pBMI->bmiHeader.biHeight;
	long DIBSizeBit=m_pBMI->bmiHeader.biBitCount;
	long DIBSizeByte=DIBSizeBit/8;


//  	float d=(float)fabs(High-Low);
//	float Scale=d/(255.0F-8);
//	float Offset=(float)Low;

//	if (Scale!=0)
	{
		H3_RGB24 * pS=(H3_RGB24 *)pData;
		for (long y=0;y<nSizeY;y++)
		{
			H3_RGB24 *pS1=pS;
			long Off=(DIBSizeY-1-y)*DIBSizeX*DIBSizeByte;
			BGR24 *pD=(BGR24*)(m_pBits+Off);
			for (long x=0;x<nSizeX;x++)
			{
				pD->r=pS1->r;
				pD->g=pS1->g;
				pD->b=pS1->b;
				pD++;pS1++;
			}
			pS+=nPitch;
		}
	}
/*
	if (!(m_pPalette))
	{
		CreatePalette();
	}
*/
	return true;
}


/*
bool CDib::Set(H3_ARRAY2D_FLT32 &SrcBuf,H3_FLT32 Low,H3_FLT32 High)
{
	long nSizeY=SrcBuf.GetLi();	
	long nSizeX=SrcBuf.GetCo();

	if (!ReAlloc(nSizeX,nSizeY))
	{
		return false;
	}

	long DIBSizeX=m_pBMI->bmiHeader.biWidth;
	long DIBSizeY=m_pBMI->bmiHeader.biHeight;
  	float d=(float)fabs(High-Low);
	float Scale=d/(255.0F-8);
	float Offset=(float)Low;

	H3_FLT32 * pS=(H3_FLT32 *)SrcBuf.GetData();

	if (Scale!=0)
	{
		for (long y=0;y<nSizeY;y++)
		{
			long Off=(DIBSizeY-1-y)*DIBSizeX;
			LPBYTE pDest=m_pBits+Off;
			for (long x=0;x<nSizeX;x++)
			{
				if (!_isnan(*pS))
				{
					if (*pS>=Low)
					{
						if (*pS<=High)
						{
							float val = (*pS - Offset)/Scale+8;
							*pDest=(BYTE)(val);
						}
						else
						{
							*pDest=2;
						}
					}
					else
					{
						*pDest=1;
					}
				}
				else
				{
					*pDest=0;
				}
				pDest++;pS++;
			}
		}
	}

	if (!(m_pPalette))
	{
		CreatePalette();
	}

	return true;
}
*/

bool CDib::Set(const H3_ARRAY2D_FLT32 &SrcBuf,const H3_ARRAY_FLT32 &Range)
{
	size_t nSizeY=SrcBuf.GetLi();	
	size_t nSizeX=SrcBuf.GetCo();

	float Low=Range[0];
	float High=Range[1];

	float Low1=Range[2];
	float High1=Range[3];


	if (!ReAlloc(nSizeX,nSizeY,8))
	{
		return false;
	}

	long DIBSizeX=m_pBMI->bmiHeader.biWidth;
	long DIBSizeY=m_pBMI->bmiHeader.biHeight;
  	float d=(float)fabs(High-Low);
	float Scale=d/(255.0F-8);
	float Offset=(float)Low;

	H3_FLT32 * pS=(H3_FLT32 *)SrcBuf.GetData();

	if (Scale!=0)
	{
		for (long y=0;y<nSizeY;y++)
		{
			long Off=(DIBSizeY-1-y)*DIBSizeX;
			LPBYTE pDest=m_pBits+Off;
			for (long x=0;x<nSizeX;x++)
			{
				if (!_isnan(*pS))
				{
					if (*pS>=Low1)
					{
						if (*pS<=High1)
						{
							if (*pS>=Low)
							{
								if (*pS<=High)
								{
									float val = (*pS - Offset)/Scale + 8;
									*pDest=(BYTE)(val);
								}
								else
								{
									*pDest=255;
								}
							}
							else
							{
								*pDest=8;
							}
						}
						else
						{
							*pDest=2;
						}
					}
					else
					{
						*pDest=1;
					}
				}
				else
				{
					*pDest=0;
				}
				pDest++;pS++;
			}
		}
	}

	if (!(m_pPalette))
	{
		CreatePalette();
	}

	return true;
}

bool CDib::Set(unsigned char *pData,long nSizeX,long nSizeY,long nPitch, H3_ARRAY_FLT32 &Range)
{
	if (!ReAlloc(nSizeX,nSizeY,8))
	{
		return false;
	}

	float Low=Range[0];
	float High=Range[1];

	float Low1=Range[2];
	float High1=Range[3];

	long DIBSizeX=m_pBMI->bmiHeader.biWidth;
	long DIBSizeY=m_pBMI->bmiHeader.biHeight;

  	float d=(float)fabs(High-Low);
	float Scale=d/(255.0F-8);
	float Offset=(float)Low;


	if (Scale!=0)
	{
		H3_UINT8 * pS=(H3_UINT8 *)pData;
		for (long y=0;y<nSizeY;y++)
		{
			H3_UINT8 *pS1=pS;
			long Off=(DIBSizeY-1-y)*DIBSizeX;
			LPBYTE pDest=m_pBits+Off;
			for (long x=0;x<nSizeX;x++)
			{
				if (*pS1>=Low1)
				{
					if (*pS1<=High1)
					{
						if (*pS1>=Low)
						{
							if (*pS1<=High)
							{
								float val = (*pS1 - Offset)/Scale + 8;
								*pDest=(BYTE)(val);
							}
							else
							{
								*pDest=255;
							}
						}
						else
						{
							*pDest=8;
						}
					}
					else
					{
						*pDest=2;
					}
				}
				else
				{
					*pDest=1;
				}
				pDest++;pS1++;
			}
			pS+=nPitch;
		}
	}

	if (!(m_pPalette))
	{
		CreatePalette();
	}

	return true;
}

bool CDib::Set(unsigned char *pData,long nSizeX,long nSizeY,long nPitch,H3_UINT8 Low,H3_UINT8 High)
{
	H3_ARRAY_FLT32 Range(4);
	Range[0]=Low;
	Range[1]=High;
	Range[2]=Low;
	Range[3]=High;

	return Set(pData,nSizeX,nSizeY,nPitch,Range);
}

bool CDib::Set(unsigned short *pData,long nSizeX,long nSizeY,long nPitch, long nLow, long nHigh)
{
	if (!ReAlloc(nSizeX,nSizeY,8)) return false;

	long DIBSizeX=m_pBMI->bmiHeader.biWidth;
	long DIBSizeY=m_pBMI->bmiHeader.biHeight;
  	float d=(float)fabs((float)(nHigh-nLow));
	float Scale=d/(255.0F-8);
	float Offset=(float)nLow;

	if (Scale!=0)
	{
		H3_UINT16 * pS=(H3_UINT16 *)pData;
		for (long y=0;y<nSizeY;y++)
		{
			H3_UINT16 *pS1=pS;
			long Off=(DIBSizeY-1-y)*DIBSizeX;
			LPBYTE pDest=m_pBits+Off;
			for (long x=0;x<nSizeX;x++)
			{
				if (*pS1>=nLow)
				{
					if (*pS1<=nHigh)
					{
						float val = (*pS1 - Offset)/Scale+8;
						*pDest=(BYTE)(val);
					}
					else
					{
						*pDest=2;
					}
				}
				else
				{
					*pDest=1;
				}
				pDest++;pS1++;
			}
			pS+=nPitch;
		}
	}

	if (!(m_pPalette))
	{
		CreatePalette();
	}

	return true;
}


bool CDib::Alloc(long nSizeX, long nSizeY, long nSizeBit)
{	
	CString strFunction("Alloc()");

	m_nUsedSizeX=nSizeX;
	m_nUsedSizeY=nSizeY;

	//////////////////////////////////////////////////////////////////////////
	// EC le 08/04/2002 pour que StretchDIBits fonctionne il faut un multiple
	// de 4 pour SizeX
	long nBufSizeX=nSizeX;
	if (nSizeX%4) nBufSizeX=(((nSizeX-1)/4)+1)*4;
	long nBufSizeY=nSizeY;
	if (nSizeY%4) nBufSizeY=(((nSizeY-1)/4)+1)*4;
	//////////////////////////////////////////////////////////////////////////

	if (!m_pBMI)
	{
		switch(nSizeBit)
		{
		case 8:
			{
				m_pBMI = (LPBITMAPINFO)GlobalAllocPtr(GHND,																
						sizeof(BITMAPINFOHEADER) + 256*sizeof(RGBQUAD));
				if (m_pBMI == 0) return false;

				m_pBMI->bmiHeader.biSize=sizeof(BITMAPINFOHEADER);							
				m_pBMI->bmiHeader.biWidth=nBufSizeX;									
				m_pBMI->bmiHeader.biHeight=nBufSizeY;									
				m_pBMI->bmiHeader.biPlanes=1;												
				m_pBMI->bmiHeader.biBitCount=(WORD)nSizeBit;												
				m_pBMI->bmiHeader.biCompression=BI_RGB;										
				m_pBMI->bmiHeader.biSizeImage=0;											
				m_pBMI->bmiHeader.biXPelsPerMeter=1;										
				m_pBMI->bmiHeader.biYPelsPerMeter=1;										
				m_pBMI->bmiHeader.biClrUsed=0;												
				m_pBMI->bmiHeader.biClrImportant=0;

				for (long k=0;k<256;k++)													
				{														
					m_pBMI->bmiColors[k].rgbRed=0;											
					m_pBMI->bmiColors[k].rgbGreen=0;										
					m_pBMI->bmiColors[k].rgbBlue=0;											
					m_pBMI->bmiColors[k].rgbReserved=0;										
				}
			}
			break;

		case 24:
			{
				m_pBMI = (LPBITMAPINFO)GlobalAllocPtr(GHND,																
						sizeof(BITMAPINFOHEADER) + 256*sizeof(RGBQUAD));
				if (m_pBMI == 0) return false;

				m_pBMI->bmiHeader.biSize=sizeof(BITMAPINFOHEADER);							
				m_pBMI->bmiHeader.biWidth=nBufSizeX;									
				m_pBMI->bmiHeader.biHeight=nBufSizeY;									
				m_pBMI->bmiHeader.biPlanes=1;												
				m_pBMI->bmiHeader.biBitCount=(WORD)nSizeBit;												
				m_pBMI->bmiHeader.biCompression=BI_RGB;										
				m_pBMI->bmiHeader.biSizeImage=0;											
				m_pBMI->bmiHeader.biXPelsPerMeter=1;										
				m_pBMI->bmiHeader.biYPelsPerMeter=1;										
				m_pBMI->bmiHeader.biClrUsed=0;												
				m_pBMI->bmiHeader.biClrImportant=0;

				// theoriquement pas nécessaire ici car on est en RGB24
				for (long k=0;k<256;k++)													
				{														
					m_pBMI->bmiColors[k].rgbRed=0;											
					m_pBMI->bmiColors[k].rgbGreen=0;										
					m_pBMI->bmiColors[k].rgbBlue=0;											
					m_pBMI->bmiColors[k].rgbReserved=0;										
				}
			}
			break;

			
/*
// EC 18/08/05 voir remarque dans SetColormaps()
		case 24:
			m_pBMI = (LPBITMAPINFO)GlobalAllocPtr(GHND,
				sizeof(BITMAPINFOHEADER));
			if (m_pBMI == 0) return false;

			m_pBMI->bmiHeader.biSize=sizeof(BITMAPINFOHEADER);							
			m_pBMI->bmiHeader.biWidth=nBufSizeX;									
			m_pBMI->bmiHeader.biHeight=nBufSizeY;									
			m_pBMI->bmiHeader.biPlanes=1;												
			m_pBMI->bmiHeader.biBitCount=nSizeBit;												
			m_pBMI->bmiHeader.biCompression=BI_RGB;										
			m_pBMI->bmiHeader.biSizeImage=0;											
			m_pBMI->bmiHeader.biXPelsPerMeter=1;										
			m_pBMI->bmiHeader.biYPelsPerMeter=1;										
			m_pBMI->bmiHeader.biClrUsed=0;												
			m_pBMI->bmiHeader.biClrImportant=0;

			break;
*/

		default:
			H3DisplayError(strModule,strFunction,"Paramètre 'nSizeBit' invalide");
			return false;
			break;
		}
																																								
		m_pBits = (LPBYTE)GlobalAllocPtr(
			GHND,
			m_pBMI->bmiHeader.biWidth*m_pBMI->bmiHeader.biHeight*m_pBMI->bmiHeader.biBitCount/8);						
		if (m_pBits == 0)															
		{																			
			GlobalFreePtr(m_pBMI);													
			m_pBMI = nullptr;															
			return false;															
		}	

		return true;
	}

	return false;
}

/*
bool CDib::Alloc(long nSizeX, long nSizeY)
{			
	m_nUsedSizeX=nSizeX;
	m_nUsedSizeY=nSizeY;

	//////////////////////////////////////////////////////////////////////////
	// EC le 08/04/2002 pour que StretchDIBits fonctionne il faut un multiple
	// de 4 pour SizeX
//	long nBufSizeX=nSizeX;
//	if (nSizeX%4)
//		nBufSizeX=((nSizeX/4)+1)*4;
//	long nBufSizeY=nSizeY;

	long nBufSizeX=nSizeX;
	if (nSizeX%4)
		nBufSizeX=(((nSizeX-1)/4)+1)*4;
	long nBufSizeY=nSizeY;
	if (nSizeY%4)
		nBufSizeY=(((nSizeY-1)/4)+1)*4;
	//////////////////////////////////////////////////////////////////////////

	if (!m_pBMI)
	{
		m_pBMI = (LPBITMAPINFO)GlobalAllocPtr(									
					GHND,																
					sizeof(BITMAPINFOHEADER) + 256*sizeof(RGBQUAD));					
		if (m_pBMI == 0)
		{
			return false;	
		}
																					
		m_pBMI->bmiHeader.biSize=sizeof(BITMAPINFOHEADER);							
		m_pBMI->bmiHeader.biWidth=nBufSizeX;									
		m_pBMI->bmiHeader.biHeight=nBufSizeY;									
		m_pBMI->bmiHeader.biPlanes=1;												
		m_pBMI->bmiHeader.biBitCount=8;												
		m_pBMI->bmiHeader.biCompression=BI_RGB;										
		m_pBMI->bmiHeader.biSizeImage=0;											
		m_pBMI->bmiHeader.biXPelsPerMeter=1;										
		m_pBMI->bmiHeader.biYPelsPerMeter=1;										
		m_pBMI->bmiHeader.biClrUsed=0;												
		m_pBMI->bmiHeader.biClrImportant=0;		
		for (long k=0;k<256;k++)													
		{														
			m_pBMI->bmiColors[k].rgbRed=0;											
			m_pBMI->bmiColors[k].rgbGreen=0;										
			m_pBMI->bmiColors[k].rgbBlue=0;											
			m_pBMI->bmiColors[k].rgbReserved=0;										
		}	
																						
		m_pBits = (LPBYTE)GlobalAllocPtr(GHND,m_pBMI->bmiHeader.biWidth*m_pBMI->bmiHeader.biHeight);						
		if (m_pBits == 0)															
		{																			
			GlobalFreePtr(m_pBMI);													
			m_pBMI = NULL;															
			return false;															
		}	

		return true;
	}

	return false;
}
*/  

bool CDib::ReAlloc(long nSizeX, long nSizeY, long nSizeBit)
{
	if (m_pBMI)
	{
		if (nSizeX!=m_nUsedSizeX ||
			nSizeY!=m_nUsedSizeY ||
			m_pBMI->bmiHeader.biBitCount!=nSizeBit)
		{
			Free();
			return Alloc(nSizeX,nSizeY,nSizeBit);
		}
		return true;
	}

	return Alloc(nSizeX,nSizeY,nSizeBit);
}

bool CDib::SetColorMaps(H3_ARRAY2D_UINT8 &ColorMap1,H3_ARRAY2D_UINT8 &ColorMap2)
{
	if (!m_pBMI)
	{
		return false;
	}

	// Remarque EC 18/08/05, il ne faudrait pas faire cela si le tableau
	// contenant la palette de couleur bmiColors n'existe pas ou n'a pas
	// les bonnes dimensions. (pbme si images RGB 24bits pour lesquels
	// bmiColors n'est theoriquement pas necessaire. Mais pour eviter
	// les plantage un tableau de 256 elements bmiColors est tout de meme
	// alloue lors de l'allocation d'une image RGB24


	if ((ColorMap2.GetCo()!=3) ||
		(ColorMap2.GetLi()!=8))
	{
		for (long k=0;k<8;k++)
		{
			m_pBMI->bmiColors[k].rgbRed=0;											
			m_pBMI->bmiColors[k].rgbGreen=0;										
			m_pBMI->bmiColors[k].rgbBlue=0;											
			m_pBMI->bmiColors[k].rgbReserved=0;										
		}
	}
	else
	{
		for (long k=0;k<8;k++)
		{
			m_pBMI->bmiColors[k].rgbRed=ColorMap2(k,0);											
			m_pBMI->bmiColors[k].rgbGreen=ColorMap2(k,1);										
			m_pBMI->bmiColors[k].rgbBlue=ColorMap2(k,2);											
			m_pBMI->bmiColors[k].rgbReserved=0;										
		}
	}

	float s=256.0F/(256.0F-8.0F);
	if ((ColorMap1.GetCo()!=3) ||
		(ColorMap1.GetLi()!=256))
	{
		for (long k=0;k<256-8;k++)													
		{			
			m_pBMI->bmiColors[k+8].rgbRed=(BYTE)(k*s);
			m_pBMI->bmiColors[k+8].rgbGreen=(BYTE)(k*s);	
			m_pBMI->bmiColors[k+8].rgbBlue=(BYTE)(k*s);											
			m_pBMI->bmiColors[k+8].rgbReserved=0;										
		}	
	}
	else
	{
		for (long k=0;k<256-8;k++)													
		{														
			m_pBMI->bmiColors[k+8].rgbRed=ColorMap1((long)(k*s),0);											
			m_pBMI->bmiColors[k+8].rgbGreen=ColorMap1((long)(k*s),1);										
			m_pBMI->bmiColors[k+8].rgbBlue=ColorMap1((long)(k*s),2);											
			m_pBMI->bmiColors[k+8].rgbReserved=0;										
		}	
	}
	CreatePalette();
	return true;
}

