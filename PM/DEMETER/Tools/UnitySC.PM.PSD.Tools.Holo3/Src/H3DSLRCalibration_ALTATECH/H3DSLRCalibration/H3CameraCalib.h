// H3CameraCalib.h: interface for the CH3CameraCalib class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_H3CAMERACALIB_H__B2D429FE_A0E9_41E1_A3F7_4F32792E6910__INCLUDED_)
#define AFX_H3CAMERACALIB_H__B2D429FE_A0E9_41E1_A3F7_4F32792E6910__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include <vector>
#include "H3Target.h"

#if XML_FILE
#include "H3XMLFile.h"
#endif

#ifdef _DLL
#  ifdef __H3_DSLR_CALIBRATION__
#    define H3CAMERACALIB_EXPORT_DECL __declspec(dllexport)
#  else
#    define H3CAMERACALIB_EXPORT_DECL __declspec(dllimport)
#  endif
#else
#  define H3CAMERACALIB_EXPORT_DECL
#endif

#include "Extrinsic_param.h"
#include "H3Mire.h"
#include "H3Camera.h"

//Cette classe derive de la classe CH3Camera qui ne gere que des position pixels mais pas d'image.
//Elle fait l'interface pour le calibrage et d'autres operation.
//le 'ST' dans son nom est trompeur (pas de stereo la dedans).
class H3CAMERACALIB_EXPORT_DECL CH3CameraCalib  : public CH3Camera
{
public:
	CH3CameraCalib(const CString& calibFolder);

    CString m_CalibFolder;

	virtual ~CH3CameraCalib();
	CH3CameraCalib& operator=(const CH3CameraCalib& STCC);

	bool Calib();	//calibrage à partir des images entrées via AddImage

	bool AddImage(const H3_ARRAY2D_PT2DFLT32 &CornersPos, const size_t ImagesLi, const size_t ImagesCo);
	bool AddImage(const CCorrespList2& CL);
	bool AddImage(const H3_ARRAY2D_FLT32 &Ima, H3_ARRAY2D_PT2DFLT32 *pPt=nullptr);//permet d'ajouter des images dans la liste qui va servir à calibrer
	bool AddImage(const H3_ARRAY2D_UINT8 &Ima, H3_ARRAY2D_PT2DFLT32 *pPt=nullptr);//permet d'ajouter des images dans la liste qui va servir à calibrer
	void RemoveLastImage();//permet d'enlever la derniere image entrée dans la liste
	vector< strPos > m_Pos;//position des intersection pour les differentes images
	H3_ARRAY_PT2DFLT32 m_Px;//position pixel des intersections de la mire dans la dernière image
	H3_ARRAY_PT2DFLT32 GetLastIntersect() { return m_Px; };

	bool GetExtrinsic(CExtrinsic_param& outEP, const H3_ARRAY2D_PT2DFLT32 &CornersPos)const;//permet une fois la camera calibrée, d'obtenir les parametre ext. de la mire à partir d'une image
	bool GetExtrinsic(CExtrinsic_param& outEP, const H3_ARRAY2D_UINT8 &Ima, bool OriginOnMire=true)const;//cv2mp_200912//permet une fois la camera calibrée, d'obtenir les parametre ext. de la mire à partir d'une image

	void SetParamIntrin(const stParamIntrin& newParam);
	void SetParamIntrinErr(const stParamIntrinErr& newParam);

	CString m_ID;//permet de distinguer une camera d'une autre?
#if XML_FILE
	bool SaveSettings(H3XMLFile* file,CString strSection=CString(""));
	bool LoadSettings(H3XMLFile* file,CString strSection=CString(""));
#else
	bool SaveSettings(const CString& strFileName);
	bool LoadSettings(const CString& strFileName);
#endif
	
	CH3Mire GetMire()const{return m_Mire;};
	unsigned int GetMireLi()const{return m_Mire.GetLi();};
	unsigned int GetMireCo()const{return m_Mire.GetCo();};
	H3_ARRAY2D_FLT64 GetMireMetric()const{return m_Mire.GetMetric();};
	void MireInit(const CString& strFile){m_Mire.LoadMire(strFile);};
	CString GetMireFileName() {return m_Mire.GetFileMire();};

	CH3Target m_Target;	//La cible qui permet le calibrage
	float m_fReprojectionErrorMaxi;

	void Calibrated(bool bCalibrated) {m_bIsCalibrated=bCalibrated;};
	bool IsCalibrated() {return m_bIsCalibrated;};
private:
	bool m_bIsCalibrated;
	long m_nbImages;
	void initIntrin();
	void initIntrinErr();
	
	CH3Mire m_Mire;//description de la mire de calibrage//public jusqu'au 140910
#if XML_FILE
	H3XMLFile* file;
#endif
};

#endif // !defined(AFX_H3CAMERACALIB_H__B2D429FE_A0E9_41E1_A3F7_4F32792E6910__INCLUDED_)
