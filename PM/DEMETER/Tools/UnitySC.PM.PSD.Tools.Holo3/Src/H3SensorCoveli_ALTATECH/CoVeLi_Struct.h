

#if !defined(PT_COMPLET__INCLUDED_)
#define PT_COMPLET__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "h3array2d.h"
//#include "UWImage.h"

struct SInfo
{
	CString strOperator;
	CString strDate;
	CString strComments;
};

struct SMesure{
	H3_ARRAY2D_PT3DFLT32 aPts;
	H3_ARRAY2D_V3DFLT32  aNs;
	H3_ARRAY2D_INT8  aMask;
};

struct S_3DCoord_onHMapMire
{
	H3_ARRAY2D_FLT32 a2dCoordX,a2dCoordY,a2dCoordZ;
	H3_ARRAY2D_UINT8 a2dMask;

	S_3DCoord_onHMapMire()
	{
	}
	bool Init(const H3_ARRAY2D_FLT32& X,const H3_ARRAY2D_FLT32& Y,const H3_ARRAY2D_UINT8& M)
	{
		//check
		size_t nLi=M.GetLi(),nCo=M.GetCo();
		if(nLi!=X.GetLi() || nCo!=X.GetCo() || nLi!=Y.GetLi() || nCo!=Y.GetCo())
			return false;

		a2dCoordX=X;
		a2dCoordY=Y;
		a2dCoordZ.ReAlloc(nLi,nCo);
		a2dCoordZ.Fill(0);
		a2dMask=M;
	}
	~S_3DCoord_onHMapMire()
	{
		a2dCoordX.Free();
		a2dCoordY.Free();
		a2dCoordZ.Free();
		a2dMask.Free();
	}
};


#endif