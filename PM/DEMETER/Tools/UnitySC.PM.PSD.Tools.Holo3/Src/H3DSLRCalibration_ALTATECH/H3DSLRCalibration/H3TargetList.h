// H3TargetList.h: interface for the CH3TargetList class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_H3TARGETLIST_H__9CF75771_048C_4D35_8600_AFDE62E1BD81__INCLUDED_)
#define AFX_H3TARGETLIST_H__9CF75771_048C_4D35_8600_AFDE62E1BD81__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "VImage.h"

#define H_CENTER (0)
#define H_NBTARGET (1)

class AFX_EXT_CLASS CH3TargetList  
{
public:
	CH3TargetList();
	virtual ~CH3TargetList();

	virtual long Find(const VImage &Image)=0;
	virtual bool InitControlParam(long ParamName ,bool ParamValue)=0;
	virtual bool InitSearchParam(long ParamName ,long ParamValue)=0;	
	virtual bool InitSearchParam(long ParamName ,float ParamValue)=0;

	bool GetCenters(H3_ARRAY_PT2DFLT32* pPx);
	virtual bool GetControlParam(long ParamName)const=0 ;
	virtual float GetSearchParam(long ParamName)const=0 ;
	bool GetControlValue(long ParamName , H3_ARRAY_PT2DFLT32 *pParamValue)const;
	bool GetControlValue(long ParamName, unsigned long *p_nbTarget)const;
	
	void Draw(const CString& Title=_T("Ok to continue"));
protected:
	H3_ARRAY_PT2DFLT32	m_Px;//coordonnées pixels des centres cible trouvés
	H3_ARRAY2D_UINT8	m_Image;//image dans laquel les centre cibles sont cherchés

//	ControlParam ...
//	SearchParam ...
};

#endif // !defined(AFX_H3TARGETLIST_H__9CF75771_048C_4D35_8600_AFDE62E1BD81__INCLUDED_)
