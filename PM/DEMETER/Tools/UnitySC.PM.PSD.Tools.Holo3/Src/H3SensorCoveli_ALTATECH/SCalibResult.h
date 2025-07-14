#if !defined(CALIB_RESUL__INCLUDED_)
#define CALIB_RESUL__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "h3AppToolsDecl.h"
#include "h3array2d.h"
#include "Extrinsic_param.h"

struct SCalibResults
{
	CExtrinsic_param ep_MireHMAP;// screen dans repere Camera
	//CExtrinsic_param ep_ObjRef;//wafer ref dans repere Camera

	float f_MireHMAP_convert_phiX2mm;//coefficient sur l'ecran (mireHMAP) de conversion phase vers mm
	float f_MireHMAP_convert_phiY2mm;

	float f_phi_on_pix_Xref;		//phase sur le point ref (marque affichée sur le moniteur)
	float f_phi_on_pix_Yref;

	H3_ARRAY2D_FLT64 a2d_fit_coef_UWX,a2d_fit_coef_UWY;//fit des cartes de phases demodulée sur l'objet ref

	unsigned long Pref_X, Pref_Y;	//position du point ref sur l'objet ref

	int Save(CString FileName,CString FileX,CString FileY){
		bool b=true;
		b&= (ep_MireHMAP.Save(FileName,_T("EP_MIRE")) == FALSE) ? false : true;
		//b&= (ep_ObjRef.Save(FileName,_T("EP_REF")) == FALSE) ? false : true;

		b&=H3WritePrivProfile(_T("GeneralFact"),_T("PhiX2mm"),f_MireHMAP_convert_phiX2mm,FileName);
		b&=H3WritePrivProfile(_T("GeneralFact"),_T("PhiY2mm"),f_MireHMAP_convert_phiY2mm,FileName);

		b&=H3WritePrivProfile(_T("GeneralFact"),_T("phi_on_pix_Xref"),f_phi_on_pix_Xref,FileName);
		b&=H3WritePrivProfile(_T("GeneralFact"),_T("phi_on_pix_Yref"),f_phi_on_pix_Yref,FileName);

		b&=H3WritePrivProfileInt(_T("GeneralFact"),_T("PixRefX"),(int)Pref_X,FileName);
		b&=H3WritePrivProfileInt(_T("GeneralFact"),_T("PixRefY"),(int)Pref_Y,FileName);

		b&=a2d_fit_coef_UWX.Save(FileX);
		b&=a2d_fit_coef_UWY.Save(FileY);
			
		if(b)
			return 0;
		else
			return 1;
	};

	int Load(CString FileName,CString FileX,CString FileY){
		bool b=true;
		b&=(ep_MireHMAP.Load(FileName,_T("EP_MIRE")) == FALSE) ? false : true;
		//b&=(ep_ObjRef.Load(FileName,_T("EP_REF")) == FALSE) ? false : true;

		f_MireHMAP_convert_phiX2mm=H3GetPrivProfileFloat(_T("GeneralFact"),_T("PhiX2mm"),FileName);
		f_MireHMAP_convert_phiY2mm=H3GetPrivProfileFloat(_T("GeneralFact"),_T("PhiY2mm"),FileName);
		b&= !(fabs(f_MireHMAP_convert_phiX2mm)<FLT_EPSILON || fabs(f_MireHMAP_convert_phiY2mm)<FLT_EPSILON);

		f_phi_on_pix_Xref=H3GetPrivProfileFloat(_T("GeneralFact"),_T("phi_on_pix_Xref"),FileName);
		f_phi_on_pix_Yref=H3GetPrivProfileFloat(_T("GeneralFact"),_T("phi_on_pix_Yref"),FileName);
		b&= !(fabs(f_phi_on_pix_Xref)>1e20 || fabs(f_phi_on_pix_Yref)>1e20 );//max possible = 2pi*pixels_moniteur(X ou y)/pas_reseau(X ou Y)

		Pref_X=H3GetPrivProfileLong(_T("GeneralFact"),_T("PixRefX"),FileName);
		Pref_Y=H3GetPrivProfileLong(_T("GeneralFact"),_T("PixRefY"),FileName);
		b&= !(Pref_X>1e20 || Pref_Y>1e20);

		b&=a2d_fit_coef_UWX.Load(FileX);
		b&=a2d_fit_coef_UWY.Load(FileY);
			
		if(b)
			return 0;
		else
			return 1;
	};
};

#endif