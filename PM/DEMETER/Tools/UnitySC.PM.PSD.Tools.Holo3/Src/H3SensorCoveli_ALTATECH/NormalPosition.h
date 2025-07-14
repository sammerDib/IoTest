// NormalPosition.h: interface for the NormalPositionclass.

//Version qui fonctionne avec des camera dont on connait les parametres intinseques!!!
//un pixel de la camera C voit un point P de la mireHMap par reflexion sur un point M de l'objet à mesurer
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_NORMALPOSITION_H__0E75C40C_D6F8_11D7_BEA6_00095B087A04__INCLUDED_)
#define AFX_NORMALPOSITION_H__0E75C40C_D6F8_11D7_BEA6_00095B087A04__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

#include "h3matrix.h"
#include "H3CameraCalib.h"
#include "CoVeLi_Struct.h"

enum TRAITEMENT { POINT_TRAITE_SS_SUCCES =(-9L), POINT_TRAITE =(3L), POINT_A_TRAITER =(2L), POINT_A_NE_PAS_TRAITER =(0L), HORS_MATRICE =(-10L)};

#define H3_ARRAY2D_PT2DUINT32 CH3Array2D< H3_POINT2D_UINT32 > 
struct DataIn{
	H3_ARRAY2D_FLT32	*paMesX,*paMesY,*paMesZ;//coord 3D des points de mireHMap  vus par les pixels de la camera decrits par paPix _ dans rep objet (cf m_ep_MireHMAP pour passer du rep mire au rep objet)
	H3_ARRAY2D_UINT8	*paMask;
	H3_ARRAY2D_PT2DUINT32*paPix;//nullptr si on utilise toute l'image. si on utilise un echantillonage, il faut se souvenir que l'information de contiguité des pixels sera utilisé
	unsigned long nx,ny;//nb pixels cam
};

struct DataOut_2D{
	H3_ARRAY2D_PT3DFLT32*paPos;
	H3_ARRAY2D_V3DFLT32 *paN;
	H3_ARRAY2D_INT8		*paMask;
};

struct DataOut_1D{
	H3_ARRAY_PT3DFLT32	*paPos;
	H3_ARRAY_V3DFLT32	*paN;
	H3_ARRAY_INT8		*paMask;

	DataOut_1D(const size_t n){paPos->Alloc(n);paN->Alloc(n);paMask->Alloc(n);};
};

struct DataOut_0D{
	H3_POINT3D_FLT32	Pos;
	H3_VECTOR3D_FLT32	N;
	__int8				Mask;
};

class NormalPosition 
{
public:
    bool SetParams(const CH3Camera& CC,
						  const CExtrinsic_param& Screen,
						  const CExtrinsic_param& Obj);

	explicit NormalPosition();

	long ComputeMaps_Z0(const H3_ARRAY2D_FLT32& MesX,
						const H3_ARRAY2D_FLT32& MesY,
						const H3_ARRAY2D_FLT32& MesZ,
						const H3_ARRAY2D_UINT8& Mask,
						H3_ARRAY2D_PT3DFLT32 & Position,
						H3_ARRAY2D_V3DFLT32 & Vecteurs,
						H3_ARRAY2D_INT8 & aMask)const;

	long ComputeMaps_Z (const H3_ARRAY2D_FLT32& MesX,
						const H3_ARRAY2D_FLT32& MesY,
						const H3_ARRAY2D_FLT32& MesZ,
						const H3_ARRAY2D_UINT8& Mask,
						const size_t pixRefX, const size_t pixRefY,
						const float refZ,
						H3_ARRAY2D_PT3DFLT32 & Position,
						H3_ARRAY2D_V3DFLT32 & Vecteurs,
						H3_ARRAY2D_INT8 & aMask)const;
	
	~NormalPosition();
protected:
/////////////////////////////////Cas3D

    bool Set_is3D(bool);

	size_t ComputeMaps(	const H3_ARRAY2D_FLT32& MesX,
						const H3_ARRAY2D_FLT32& MesY,
						const H3_ARRAY2D_FLT32& MesZ,
						const H3_ARRAY2D_UINT8& Mask,
						H3_ARRAY2D_PT3DFLT32  & Position,
						H3_ARRAY2D_V3DFLT32   & Vecteurs,
						H3_ARRAY2D_INT8       & aMask,
						const H3_POINT2D_UINT32& PosPixelPointDeDepart,
						const float AltitudePointDeDepart)const;

	 void CalculPointRef(size_t index0,const float M_Altitude,const DataIn& DI, DataOut_0D& DO)const;

	 H3_VECTOR3D_FLT64 getVectObs (const size_t li, const size_t co) const {return m_PreComputed_NormalizedCM(li,co);};
	 H3_POINT3D_FLT64  getPointObs(const size_t li, const size_t co) const {return m_PreComputed_M(li,co);};

	 size_t Integr_( H3_POINT2D_UINT32& Depart_pix,const DataIn& DI, DataOut_2D& DO)const;//integre sur une image
	 
	 __int8 CalculPointVoisin(const size_t validIndex0,const size_t newIndex0, const DataIn& DI, DataOut_2D& DO)const;
	 __int8 CalculPointVoisin(const size_t newIndex0, const DataIn& DI,const size_t indexOut,const size_t newIndexOut, DataOut_1D& DO)const;

	 size_t CalculNext(const H3_POINT2D_UINT32& pxDepart,const DataIn & DI,DataOut_2D& DO, const H3_POINT2D_FLT32& fpx_Delta)const;
	 size_t CalculNext(const H3_POINT2D_UINT32& pxDepart,const DataIn & DI,DataOut_1D& DO, const H3_POINT2D_FLT32& fpx_Delta)const;
	 
	 H3_POINT3D_FLT64 pospix2mm(const double z,const unsigned long index)const;
	 H3_POINT2D_INT32 NouvelleRef(const H3_ARRAY2D_INT8& aMask,long &DimZoneRechercheRef)const;
	 H3_POINT2D_INT32 NouvelleRef_fct(const H3_ARRAY2D_INT8& aMask,const H3_ARRAY_UINT32& Points_a_traiter,long dist)const;
// fin cas 3D
private:

	 H3_ARRAY2D_V3DFLT64 m_PreComputed_NormalizedCM;
	 H3_ARRAY2D_PT3DFLT32 m_PreComputed_M;
	 CExtrinsic_param m_ep_MireHMAP;
	 CExtrinsic_param m_ep_Cam;

	 bool m_isInitialised;//initialisation des variables statics ok
};

#endif // !defined(AFX_NORMALPOSITION_H__0E75C40C_D6F8_11D7_BEA6_00095B087A04__INCLUDED_)
