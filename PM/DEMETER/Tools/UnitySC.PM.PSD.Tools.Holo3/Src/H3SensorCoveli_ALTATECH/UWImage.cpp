// UWImage.cpp: implementation of the UWImage class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "coveli.h"
#include "H3AppToolsDecl.h"
#include "UWImage.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

extern CCoveliApp theApp;

static CString strModule("UWImage");

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
/*! 
* 	\fn      UWImage::UWImage()
* 	\brief   
* 	\return  
* 	\remarks 
*/ 
UWImage::UWImage()
{
	m_bIsInitialised=false;
}

/*! 
* 	\fn      :void UWImage::UWImage(const UWImage & W)
* 	\brief   : constructeur de copy
* 	\param   : const UWImage & UW 
* 	\return  : 
* 	\remarks : 
*/
UWImage::UWImage(const UWImage & UW)
{
	if (this!=&UW)
	{
		this->Copy(UW);
	}
}

/*! 
* 	\fn      :void UWImage::UWImage(const H3_ARRAY2D_FLT32& PhaseX,const H3_ARRAY2D_FLT32& PhaseY)
* 	\brief   : constructeur de copy
* 	\param   : const H3_ARRAY2D_FLT32 & PhaseX et PhaseY 
* 	\return  : 
* 	\remarks :
*/
UWImage::UWImage(const H3_ARRAY2D_FLT32& PhaseX,const H3_ARRAY2D_FLT32& PhaseY):m_aX(PhaseX),m_aY(PhaseY)
{
	const long nLi=m_aX.GetLi(),nCo=m_aX.GetCo(),nSz=nLi*nCo;
	//verif
	if(nLi!=m_aY.GetLi() || nCo!=m_aY.GetCo() )
	{
		m_aX.ReAlloc(0L,0L);
		m_aY.ReAlloc(0L,0L);
		m_aMask.ReAlloc(0L,0L);

		m_bIsInitialised=false;
		return;
	}
	else
	{
		m_aMask.ReAlloc(nLi,nCo);
	}

	//initialisation des masques
	long i;
	for(i=0;i<nSz;i++)
		m_aMask[i]=(!_isnan(m_aX[i]+m_aY[i]));

	m_bIsInitialised=true;
}


/*! 
* 	\fn      :void UWImage::Copy(const UWImage & UW)
* 	\brief   : constructeur de copy
* 	\param   : const UWImage & UW 
* 	\return  : 
* 	\remarks : 
*/
void UWImage::Copy(const UWImage & UW)
{
	if (this != &UW)
	{
		m_aX=UW.m_aX;
		m_aY=UW.m_aY;
		m_aMask=UW.m_aMask;
	}
}

/*! 
* 	\fn      :void UWImage::operator=(const UWImage & W)
* 	\brief   : 
* 	\param   : const UWImage & UW 
* 	\return  : 
* 	\remarks : 
*/
UWImage & UWImage::operator=(const UWImage & W)
{
	if (this==&W) return *this;

	Copy(W);

	return *this;
}

/*! 
* 	\fn      :void UWImage::~UWImage()
* 	\brief   : 
* 	\param   :
* 	\return  : 
* 	\remarks : 
*/
UWImage::~UWImage()
{

}

/*! 
* 	\fn      :void UWImage::GetX()const
* 	\brief   : 
* 	\param   :
* 	\return  : 
* 	\remarks : 
*/
H3_ARRAY2D_FLT32 UWImage::GetX()const
{
	return m_aX;
}

/*! 
* 	\fn      :void UWImage::GetY()const
* 	\brief   : 
* 	\param   :
* 	\return  : 
* 	\remarks : 
*/
H3_ARRAY2D_FLT32 UWImage::GetY()const
{
	return m_aY;
}

/*! 
* 	\fn      :void UWImage::GetMask()const
* 	\brief   : 
* 	\param   :
* 	\return  : 
* 	\remarks : 
*/
H3_ARRAY2D_UINT8 UWImage::GetMask()const
{
	return m_aMask;
}

/*! 
* 	\fn      :void UWImage::GetLi()const 
* 	\brief   : 
* 	\param   :
* 	\return  : 
* 	\remarks : 
*/
unsigned long UWImage::GetLi()const
{
	return m_aMask.GetLi();
}

/*! 
* 	\fn      :void UWImage::GetCo()const 
* 	\brief   : 
* 	\param   :
* 	\return  : 
* 	\remarks : 
*/
unsigned long UWImage::GetCo()const
{
	return m_aMask.GetCo();
}

/******************************************/
