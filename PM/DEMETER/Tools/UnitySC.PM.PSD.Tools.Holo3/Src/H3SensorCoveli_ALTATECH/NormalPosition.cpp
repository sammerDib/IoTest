// NormalPosition.cpp: implementation of the NormalPosition class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "coveli.h"
#include "NormalPosition.h"
#include <queue>

#include "SCalibResult.h"

static CString strModule("NormalPosition");

#define TEST 0 //

static void SaveArrayVector3D(CString Rep,CString FileName,H3_ARRAY2D_V3DFLT32& V)
{
	const unsigned long nLi=V.GetLi(),nCo=V.GetCo();
	H3_ARRAY2D_FLT32 X(nLi,nCo),Y(nLi,nCo),Z(nLi,nCo);
	float *px=X.GetData(),*py=Y.GetData(),*pz=Z.GetData();
	H3_VECTOR3D_FLT32 *pV=V.GetData();

	long i=nLi*nCo;
	while(i--){
		(*(px++))=pV->x;
		(*(py++))=pV->y;
		(*(pz++))=pV->z;
		(pV++);
	}
	X.Save(Rep+FileName+_T("_x.hbf"));
	Y.Save(Rep+FileName+_T("_y.hbf"));
	Z.Save(Rep+FileName+_T("_z.hbf"));
}

static void SaveArrayVector3D(CString Rep,CString FileName,H3_ARRAY_V3DFLT32& V)
{
	const unsigned long nLi=V.GetSize(),nCo=3;
	H3_ARRAY2D_FLT32 Vec(nLi,nCo);
	float *pv=Vec.GetData();
	H3_VECTOR3D_FLT32 *pV=V.GetData();

	long i=nLi;
	while(i--){
		(*(pv++))=pV->x;
		(*(pv++))=pV->y;
		(*(pv++))=pV->z;
		(pV++);
	}
	Vec.Save(Rep+FileName+_T(".hbf"));
}

static void SaveArrayPoint3D(CString Rep,CString FileName,H3_ARRAY2D_PT3DFLT32& P)
{
	const unsigned long nLi=P.GetLi(),nCo=P.GetCo();
	H3_ARRAY2D_FLT32 X(nLi,nCo),Y(nLi,nCo),Z(nLi,nCo);
	float *px=X.GetData(),*py=Y.GetData(),*pz=Z.GetData();
	H3_POINT3D_FLT32 *pP=P.GetData();

	long i=nLi*nCo;
	while(i--){
		(*(px++))=pP->x;
		(*(py++))=pP->y;
		(*(pz++))=pP->z;
		(pP++);
	}
	X.Save(Rep+FileName+_T("_x.hbf"));
	Y.Save(Rep+FileName+_T("_y.hbf"));
	Z.Save(Rep+FileName+_T("_z.hbf"));
}

static void SaveArrayPoint3D(CString Rep,CString FileName,H3_ARRAY_PT3DFLT32& P)
{
	const unsigned long nLi=P.GetSize(),nCo=3;
	H3_ARRAY2D_FLT32 Pt(nLi,nCo);
	float *p=Pt.GetData();
	H3_POINT3D_FLT32 *pP=P.GetData();

	long i=nLi;
	while(i--){
		(*(p++))=pP->x;
		(*(p++))=pP->y;
		(*(p++))=pP->z;
		(pP++);
	}
	Pt.Save(Rep+FileName+_T(".hbf"));
}

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

/*! 
* 	\fn      NormalPosition::NormalPosition()
* 	\brief   ne fait presque rien
* 	\return  
* 	\remarks
*/ 

NormalPosition::NormalPosition()
{
	
}

/*! 
* 	\fn      : NormalPosition::~NormalPosition
* 	\brief   : 
* 	\return  : 
* 	\remarks : ne fait rien
*/ 
NormalPosition::~NormalPosition()
{

}

/*! 
* 	\fn      bool NormalPosition::SetParams(const CH3Camera& CC, const CExtrinsic_param& ep_Screen,const CExtrinsic_param& ep_Cam)
* 	\brief   initialise les donnees membres static
* 	\m_ep_MireHMAP;
* 	\m_ep_Cam;
* 	\m_PreComputed_NormalizedCM;
* 	\m_PreComputed_M;
* 	\m_isInitialised=false;
* 	\return  bool
* 	\remarks à faire une fois par campagne
*/
bool NormalPosition::SetParams(const CH3Camera& CC, const CExtrinsic_param& ep_Screen,const CExtrinsic_param& ep_Cam)
{
	CString strFunction("SetParams");
	H3DebugInfo(strModule,strFunction,"");

	m_ep_MireHMAP=ep_Screen;
	m_ep_Cam=ep_Cam;

	const unsigned long Cam_szX=CC.nx,Cam_szY=CC.ny;
	unsigned long i,j,k;

	H3_ARRAY2D_FLT64 Pixel(2,Cam_szX*Cam_szY),PixelNormalised(2,Cam_szX*Cam_szY);
	double *pPixelX=Pixel.GetData(),*pPixelY=pPixelX+Cam_szX*Cam_szY,I;

	for(i=0L,k=0L;i<Cam_szY;i++){
		I=double(i);
		for(j=0L;j<Cam_szX;j++,k++){
			(*(pPixelX++))=(double)j;
			(*(pPixelY++))=I;
		}
	}
	bool b1= CC.normalise(Pixel,PixelNormalised);//pixels normalisés dans rep cam

	Pixel.~CH3Array2D();

	m_PreComputed_NormalizedCM.ReAlloc(Cam_szY,Cam_szX);
	m_PreComputed_M.ReAlloc(Cam_szY,Cam_szX);

	const double	ccx=m_ep_Cam.m_Tc[0],
					ccy=m_ep_Cam.m_Tc[1],
					ccz=m_ep_Cam.m_Tc[2];
	const double	R00=m_ep_Cam.m_Rc[0],	R01=m_ep_Cam.m_Rc[1],	R02=m_ep_Cam.m_Rc[2],
					R10=m_ep_Cam.m_Rc[3],	R11=m_ep_Cam.m_Rc[4],	R12=m_ep_Cam.m_Rc[5],
					R20=m_ep_Cam.m_Rc[6],	R21=m_ep_Cam.m_Rc[7],	R22=m_ep_Cam.m_Rc[8];
	double vx,vy,vz,norm,factor;

	for(i=0L,k=0L;i<Cam_szY;i++)
		for(j=0L;j<Cam_szX;j++,k++){
			vx=PixelNormalised(0,k);
			vy=PixelNormalised(1,k);
			vz=1.0;
			norm=::sqrt(vx*vx+vy*vy+1.0);
			vx /= norm;
			vy /= norm;
			vz /= norm;

			m_PreComputed_NormalizedCM[k]=H3_VECTOR3D_FLT64(R00*vx+R01*vy+R02*vz,
															R10*vx+R11*vy+R12*vz,
															R20*vx+R21*vy+R22*vz);

			//dans le rep obj, M (ref) se trouve dans le plan z=0 >> C(refObj)+ CM_norm * (zC) donne M
			factor=-ccz/m_PreComputed_NormalizedCM[k].z;
			m_PreComputed_M[k]=H3_POINT3D_FLT64(ccx+m_PreComputed_NormalizedCM[k].x*factor,ccy+m_PreComputed_NormalizedCM[k].y*factor,0.0);
		}
		m_isInitialised=true;

		return true;
}

/*! 
* 	\fn      NormalPosition::ComputeMaps(	const H3_ARRAY2D_FLT32& MesX,
											const H3_ARRAY2D_FLT32& MesY,
											const H3_ARRAY2D_FLT32& MesZ,
											const H3_ARRAY2D_UINT8& Mask,
											H3_ARRAY2D_PT3DFLT32 & Position,
											H3_ARRAY2D_V3DFLT32 & Vecteurs,
											H3_ARRAY2D_INT8 & aMask)const
* 	\brief   Calcul des cartos de formes et vecteurs normaux
* 	\param   H3_ARRAY2D_PT3DFLT32 & Position :tableau ds lequel seront enregistrées les positions calculées
* 	\param   H3_ARRAY2D_V3DFLT32 & Vecteurs: tableau ds lequel seront enregistrées les vecteurs calculées
* 	\param   H3_ARRAY2D_INT8 & aMask: masque de travail (ne sont traités que les POINT_A_TRAITER qui sont transformés en POINT_TRAITE)
* 	\return  long: nombre de points traités
* 	\remarks ici, les points sont tous quasi dans le plan Z=0 et donc le calcul de leur position est immédiat
*/
long NormalPosition::ComputeMaps_Z0(const H3_ARRAY2D_FLT32& MesX,
									const H3_ARRAY2D_FLT32& MesY,
									const H3_ARRAY2D_FLT32& MesZ,
									const H3_ARRAY2D_UINT8& Mask,
									H3_ARRAY2D_PT3DFLT32 & Position,
									H3_ARRAY2D_V3DFLT32 & Vecteurs,
									H3_ARRAY2D_INT8 & aMask)const
{
	CString strFunction("ComputeMaps");
	H3DebugInfo(strModule,strFunction,"");

	if(!m_isInitialised)
	{
		H3DisplayError(strModule,strFunction,"parametres statics non initialisés. calcul impossible.");
		return 0L;
	}

	size_t nLi=Mask.GetLi();
	size_t nCo=Mask.GetCo();
	size_t nSz=nLi*nCo;

	size_t  i=0;

	if(!(nLi==Position.GetLi() && nCo==Position.GetCo()))
		Position.ReAlloc(nLi,nCo);

	Position.Fill(H3_POINT3D_FLT32(NaN,NaN,NaN));

	if(!(nLi==Vecteurs.GetLi() && nCo==Vecteurs.GetCo()))
		Vecteurs.ReAlloc(nLi,nCo);
	Vecteurs.Fill(H3_VECTOR3D_FLT32(NaN,NaN,NaN));
	//initialisation
	long nbPointsTraites=0;

	//Calcul Principal
	H3_VECTOR3D_FLT64 V_MP;
	H3_VECTOR3D_FLT64 V_Normal;

	for(i=0;i<nSz;i++){
		if(Mask[i]){
			V_MP=H3_VECTOR3D_FLT64(m_PreComputed_M[i],H3_POINT3D_FLT64(MesX[i],MesY[i],MesZ[i]));
			V_MP.Normalize();
			V_Normal=V_MP-m_PreComputed_NormalizedCM[i];
			V_Normal.Normalize();

			Vecteurs[i]=H3_VECTOR3D_FLT32(V_Normal.x,V_Normal.y,V_Normal.z);
			Position[i]=m_PreComputed_M[i];

			nbPointsTraites++;
		}
	}
	aMask=Mask;

	return nbPointsTraites;
}
/*
* 	\remarks ici, les points sont sur une surface continue et continuement dérivable qui passe par un point vu sur le pixel (refX, refY) d'altitude refZ dans le repere de calibrage.
*/
long NormalPosition::ComputeMaps_Z (const H3_ARRAY2D_FLT32& MesX,
									const H3_ARRAY2D_FLT32& MesY,
									const H3_ARRAY2D_FLT32& MesZ,
									const H3_ARRAY2D_UINT8& Mask,
									const size_t pixRefX, const size_t pixRefY,
									const float refZ,
									H3_ARRAY2D_PT3DFLT32 & Position,
									H3_ARRAY2D_V3DFLT32 & Vecteurs,
									H3_ARRAY2D_INT8 & aMask)const
{
	CString strFunction("ComputeMaps");
	H3DebugInfo(strModule,strFunction,"");

	if(!m_isInitialised)
	{
		H3DisplayError(strModule,strFunction,"parametres statics non initialisés. calcul impossible.");
		return 0L;
	}

	size_t nLi=Mask.GetLi();
	size_t nCo=Mask.GetCo();
	size_t nSz=nLi*nCo;

	size_t  i=0;

	if(!(nLi==Position.GetLi() && nCo==Position.GetCo()))
		Position.ReAlloc(nLi,nCo);

	Position.Fill(H3_POINT3D_FLT32(NaN,NaN,NaN));

	if(!(nLi==Vecteurs.GetLi() && nCo==Vecteurs.GetCo()))
		Vecteurs.ReAlloc(nLi,nCo);
	Vecteurs.Fill(H3_VECTOR3D_FLT32(NaN,NaN,NaN));
	//initialisation
	long nbPointsTraites= ComputeMaps(MesX, MesY,MesZ, Mask,Position,Vecteurs,aMask,H3_POINT2D_UINT32(pixRefX, pixRefY), refZ);

	return nbPointsTraites;
}


///////////////////////////////////////////////////////////cas 3D //////////////////////////////

/*! 
* 	\fn      NormalPosition::ComputeMaps(	const H3_ARRAY2D_FLT32& MesX,MesY,MesZ,
											H3_ARRAY2D_PT3DFLT32 & Position,
											H3_ARRAY2D_V3DFLT32 & Vecteurs,
											H3_ARRAY2D_INT32 & aMask,
											const H3_POINT2D_INT32& PosPixelPointDeDepart,
											const float AltitudePointDeDepart)				const
* 	\brief   Calcul des cartos de formes et vecteurs normaux
* 	\param   MesX,MesY,MesZ, cartos des coords des points vus sur la mire HMAP dans ref objet
* 	\param   H3_ARRAY2D_PT3DFLT32 & Position :tableau ds lequel seront enregistrées les positions calculées
* 	\param   H3_ARRAY2D_V3DFLT32 & Vecteurs: tableau ds lequel seront enregistrées les vecteurs calculées
* 	\param   H3_ARRAY2D_INT32 & aMask: masque de travail (ne sont traités que les POINT_A_TRAITER qui sont transformés en POINT_TRAITE)
* 	\param   const H3_POINT2D_INT32& PosPixelPointDeDepart : pixel à partir duquel l'integration peut commencer 
* 	\param   const float AltitudePointDeDepart: altitude du point imagé sur le pixel ci dessus
* 	\return  long: nombre de points traités
* 	\remarks 
*/
size_t NormalPosition::ComputeMaps(	const H3_ARRAY2D_FLT32& MesX,
									const H3_ARRAY2D_FLT32& MesY,
									const H3_ARRAY2D_FLT32& MesZ,
									const H3_ARRAY2D_UINT8& Mask,
									H3_ARRAY2D_PT3DFLT32  & Position,
									H3_ARRAY2D_V3DFLT32	  & Vecteurs,
									H3_ARRAY2D_INT8       & aMask,
									const H3_POINT2D_UINT32& PosPixelPointDeDepart,
									const float AltitudePointDeDepart)const
{
	CString strFunction("ComputeMaps 3D");
	//H3DebugInfo(strModule,strFunction,"");//appel frequent

#if TEST
gInfo.ReAlloc(aMask.GetLi(),aMask.GetCo());
gInfo.Fill(0.0f);
#endif
	if(!m_isInitialised)
	{
		H3DisplayError(strModule,strFunction,"parametres statics non initialisés. calcul impossible.");
		return 0L;
	}

	const size_t nLi= Mask.GetLi(), nCo= Mask.GetCo();

	DataIn DI;
	DI.paMask= (H3_ARRAY2D_UINT8*)&Mask;
	DI.paMesX= (H3_ARRAY2D_FLT32*)&MesX;
	DI.paMesY= (H3_ARRAY2D_FLT32*)&MesY;
	DI.paMesZ= (H3_ARRAY2D_FLT32*)&MesZ;
	DI.paPix = nullptr;//à modifier si les tableaux passés en argument n'ont pas autant de cases que la camera a de pixels
	DI.nx    = nCo;
	DI.ny	 = nLi;

	if(Position.GetLi() != nLi || Position.GetCo() != nCo){
		Position.ReAlloc(nLi,nCo);
	}
	if(Vecteurs.GetLi() != nLi || Vecteurs.GetCo() != nCo){
		Vecteurs.ReAlloc(nLi,nCo);
	}
	if(aMask.GetLi() != nLi || aMask.GetCo() != nCo){
		aMask.ReAlloc(nLi,nCo);
	}

	DataOut_2D DO;
	DO.paPos=  &Position;
	DO.paN=    &Vecteurs;
	DO.paMask= &aMask;

	DataOut_0D DO_0D;

	//initialisation
	H3_POINT2D_INT32 loc_PosPixelPointDeDepart = (H3_POINT2D_INT32)PosPixelPointDeDepart;
	size_t index,index0= PosPixelPointDeDepart.x + PosPixelPointDeDepart.y * nCo;
	if(nullptr == DI.paPix){
		index= index0;
	}
	else{
		H3_POINT2D_UINT32 PosPixelPointDeDepart0= (*(DI.paPix))[index0];
		unsigned long nCo0= NormalPosition::m_PreComputed_NormalizedCM.GetCo();
		index= PosPixelPointDeDepart0.x + PosPixelPointDeDepart0.y * nCo0;
	}
	size_t nbPointsTraites=0;
	Position.Fill(NaN);
	Vecteurs.Fill(NaN);
	aMask= Mask;

	//Calcul Principal
	long px_xD= loc_PosPixelPointDeDepart.x;
	long px_yD= loc_PosPixelPointDeDepart.y;

	CalculPointRef(index0, AltitudePointDeDepart, DI, DO_0D);
	(*(DO.paMask))[index0]= DO_0D.Mask;
	(*(DO.paN))   [index0]= DO_0D.N;
	(*(DO.paPos)) [index0]= DO_0D.Pos;

	nbPointsTraites= 1;

////////////
	long ind=0;
	long DimZoneRechercheRef= 10;

	while((loc_PosPixelPointDeDepart.x >= 0) && (loc_PosPixelPointDeDepart.y >= 0)){

		nbPointsTraites += Integr_(H3_POINT2D_UINT32(loc_PosPixelPointDeDepart.x,loc_PosPixelPointDeDepart.y), DI, DO);

		loc_PosPixelPointDeDepart = NouvelleRef(aMask, DimZoneRechercheRef);
//H3DebugInfo(strModule, strFunction, "end0");
	}

#if TEST
gInfo.Save("c:\\temp\\gInfo.hbf");
#endif

//H3DebugInfo(strModule,strFunction,"end");
	return nbPointsTraites;
}

/*! 
* 	\fn      void NormalPosition::pospix2mm(const double z,const unsigned long index)const
* 	\brief   calcul de la position 3D d'un point à partir de sa position pixel et de son altitude
* 	\param   const unsigned long index: index du pixel sur la camera 
* 	\param   const double M_Altitude : exprimé dans le ref objet
* 	\return  H3_POINT3D_FLT64
* 	\remarks 
*/
H3_POINT3D_FLT64 NormalPosition::pospix2mm(const double z, const unsigned long index)const
{
	H3_POINT3D_FLT64  P = m_PreComputed_M[index];				//P.z = 0
	H3_VECTOR3D_FLT64 V = m_PreComputed_NormalizedCM[index] ;
	V *= ( z / V.z);
	P.x += V.x;
	P.y += V.y;
	P.z += V.z;													//P.z = z
	return P;
}

/*! 
* 	\fn      void NormalPosition::CalculPointRef(unsigned long index0,const float M_Altitude,const DataIn& DI, DataOut_0D& DO)
* 	\brief   calcul de la position 3D et du vecteur normal en un point à partir de sa position pixel et de son altitude
* 	\param   index0: index du pix dans DI 
* 	\param   const float M_Altitude : altitude du point
* 	\param   DI: dataIn (l'ensemble des points 3D vus par la camera sur la mire dans rep objet) 
* 	\param   DO: dataOut (le point 3D sur l'objet) 
* 	\return  void
* 	\remarks 
*/ 

void NormalPosition::CalculPointRef(size_t index0,const float M_Altitude,const DataIn& DI, DataOut_0D& DO)const
{
	unsigned long index;
	H3_POINT3D_FLT64 M;

	if(nullptr==DI.paPix){
		index=index0;
	}
	else{
		unsigned long x = (*(DI.paPix))[index0].x , y = (*(DI.paPix))[index0].y;
		index = x + y * DI.nx;
	}

	M = (DO.Pos) = pospix2mm( M_Altitude , index ) ;

	H3_VECTOR3D_FLT64 M_PointMireImageSurM( (*(DI.paMesX))[index] -M.x , (*(DI.paMesY))[index] -M.y ,(*(DI.paMesZ))[index] - M.z );

	M_PointMireImageSurM.Normalize();

	H3_VECTOR3D_FLT64 Normal = M_PointMireImageSurM - m_PreComputed_NormalizedCM[index];

	Normal.Normalize();

	(DO.N)   = H3_VECTOR3D_FLT32(Normal.x, Normal.y, Normal.z);
	(DO.Mask)= POINT_TRAITE;
}

/*! 
* 	\fn      : NormalPosition::CalculPointVoisin
* 	\brief   : Calcul de la position d'un point à partir de celle d'un autre point
*				Le calcul est valide si les points sont proches (altitudes et normales quasi identiques)
* 	\param   : const unsigned long validIndex0 : index d'un point completement initialisé 
* 	\param   : const H3_POINT2D_FLT32& px_dM : increment pixel 
* 	\param   : SPtComplet& newM : point dont l'image se forme sur le pixel (M.px+px_dM) et dont on veut connaitre la position 3D 
* 	\return  : long (point traité/non traité)
* 	\remarks : const unsigned long validIndex0,const unsigned long newIndex0, const DataIn& DI, DataOut_2D& DO
*/ 
__int8 NormalPosition::CalculPointVoisin(const size_t validIndex0, const size_t newIndex0, const DataIn& DI, DataOut_2D& DO)const
{
	size_t validIndex,newIndex;

	if(nullptr == DI.paPix){
		validIndex = validIndex0;
		newIndex   = newIndex0;
	}
	else{
		unsigned long x = (*(DI.paPix))[validIndex0].x , y = (*(DI.paPix))[validIndex0].y;
		validIndex = x + y * DI.nx;

		x= (*(DI.paPix))[newIndex0].x;   y = (*(DI.paPix))[newIndex0].y;
		newIndex = x + y * DI.nx;
	}


	long Condition;

	H3_POINT3D_FLT64  M = (*(DO.paPos))[validIndex0];
	H3_VECTOR3D_FLT32 V = (*(DO.paN))[validIndex0];
	H3_VECTOR3D_FLT64 dV(V.x,V.y,V.z);

	H3_POINT3D_FLT64  newM;// = (*(DO.paPos))[newIndex0];
	H3_VECTOR3D_FLT32 newV;// = (*(DO.paN))[newIndex0];
	H3_VECTOR3D_FLT64 dnewV;

	//determination des coordonnees mm du point a traiter en supposant son altitude=OldM_mm.z
	newM= pospix2mm(M.z , newIndex);

//CString msg; msg.Format("%d %f %f %f ; %d %f %f %f",validIndex,M.x,M.y,M.z,newIndex,newM.x,newM.y,newM.z);AfxMessageBox(msg);

	//on dit que le vecteur qui va de A à B est perpendiculaire au vecteur normal à la surface en A
	H3_VECTOR3D_FLT64 AB(M , newM);
	newM.z = M.z - (AB.Prod_Scal(dV) /dV.z );
	
	//vecteur CM CP MP
	H3_VECTOR3D_FLT64 newM_PointMireImageSur_newM( (*(DI.paMesX))[newIndex] - newM.x , (*(DI.paMesY))[newIndex] - newM.y , (*(DI.paMesZ))[newIndex] - newM.z );
	//normal
	newM_PointMireImageSur_newM.Normalize();

	dnewV = newM_PointMireImageSur_newM - m_PreComputed_NormalizedCM[newIndex];
	dnewV.Normalize();
	
	//on dit que le vecteur qui va de M à OldM est perpendiculaire au vecteur normal à la surface au milieu de AB
	for(long i=1;i<5;i++){//5: parce que ca marche
		//vecteur normal au milieu du segment M OldM
		H3_VECTOR3D_FLT64 normalMilieu(dnewV + dV);
		normalMilieu.Normalize();
		newM = pospix2mm( newM.z , newIndex );
		//PointM permettant que la perpendicularité soit respectée
		H3_VECTOR3D_FLT64 AB( M , newM );
		//z=z0-(nx.(x-x0)+ny.(y-y0))/nz //on ecrit en fait:z=z0-(nx.(x-x0)+ny.(y-y0)+nz.0)/nz
		newM.z = newM.z - ( AB.Prod_Scal(normalMilieu) / normalMilieu.z );
		//vecteur CM CP MP
	
		//CAMERA_PointMireImageSurM=VECTEUR(MesX(newM_pix),MesY(newM_pix),0);
		newM_PointMireImageSur_newM = H3_VECTOR3D_FLT64((*(DI.paMesX))[newIndex] - newM.x , (*(DI.paMesY))[newIndex] - newM.y , (*(DI.paMesZ))[newIndex] - newM.z );

		newM_PointMireImageSur_newM.Normalize();
	
		dnewV = newM_PointMireImageSur_newM - m_PreComputed_NormalizedCM[newIndex];
		dnewV.Normalize();
	}

	if ( _isnan(newM.Sum()) || _isnan(dnewV.Sum()) ){
		Condition= long(POINT_TRAITE_SS_SUCCES);
		(*(DO.paMask))[newIndex0] = POINT_TRAITE_SS_SUCCES;
	}
	else{
		Condition= long(POINT_TRAITE);
		newV= H3_VECTOR3D_FLT32(dnewV.x, dnewV.y, dnewV.z);

		(*(DO.paPos)) [newIndex0] = newM;
		(*(DO.paN))   [newIndex0] = newV;
		(*(DO.paMask))[newIndex0] = POINT_TRAITE;
	}

	return Condition;
}

__int8 NormalPosition::CalculPointVoisin(const size_t newIndex0, const DataIn& DI,const size_t indexOut,const size_t newIndexOut, DataOut_1D& DO)const
{
	unsigned long newIndex;

	if(nullptr == DI.paPix){
		newIndex=newIndex0;
	}
	else{
		unsigned long x,y;
		x = (*(DI.paPix))[newIndex0].x;  y = (*(DI.paPix))[newIndex0].y;
		newIndex = x + y * DI.nx;
	}

	long Condition;

	H3_POINT3D_FLT64  M = (*(DO.paPos))[indexOut];
	H3_VECTOR3D_FLT32 V = (*(DO.paN))[indexOut];
	H3_VECTOR3D_FLT64 dV(V.x,V.y,V.z);

	H3_POINT3D_FLT64  newM = (*(DO.paPos))[newIndexOut];
	H3_VECTOR3D_FLT32 newV = (*(DO.paN))[newIndexOut];
	H3_VECTOR3D_FLT64 dnewV;

	//determination des coordonnees mm du point a traiter en supposant son altitude=OldM_mm.z
	newM=pospix2mm( M.z , newIndex );

	//on dit que le vecteur qui va de A à B est perpendiculaire au vecteur normal à la surface en A
	H3_VECTOR3D_FLT64 AB = H3_VECTOR3D_FLT64(M , newM);
	newM.z = M.z - (AB.Prod_Scal(dV) / dV.z);
	
	//vecteur CM CP MP
	H3_VECTOR3D_FLT64 newM_PointMireImageSur_newM((*(DI.paMesX))[newIndex] - newM.x , (*(DI.paMesY))[newIndex] - newM.y , (*(DI.paMesZ))[newIndex] - newM.z);
	//normal
	newM_PointMireImageSur_newM.Normalize();

	dnewV = newM_PointMireImageSur_newM - m_PreComputed_NormalizedCM[newIndex];
	dnewV.Normalize();
	
	//on dit que le vecteur qui va de M à OldM est perpendiculaire au vecteur normal à la surface au milieu de AB
	for(long i=1;i<5;i++){
		//vecteur normal au milieu du segment M OldM
		H3_VECTOR3D_FLT64 normalMilieu = dnewV + dV;
		normalMilieu.Normalize();
		newM = pospix2mm( newM.z , newIndex);
		//PointM permettant que la perpendicularité soit respectée
		H3_VECTOR3D_FLT64 AB(M,newM);
		//z=z0-(nx.(x-x0)+ny.(y-y0))/nz //on ecrit en fait:z=z0-(nx.(x-x0)+ny.(y-y0)+nz.0)/nz
		newM.z= newM.z - (AB.Prod_Scal( normalMilieu ) / normalMilieu.z);
		//vecteur CM CP MP
	
		//CAMERA_PointMireImageSurM=VECTEUR(MesX(newM_pix),MesY(newM_pix),0);
		newM_PointMireImageSur_newM= H3_VECTOR3D_FLT64( (*(DI.paMesX))[newIndex]-newM.x, (*(DI.paMesY))[newIndex]-newM.y, (*(DI.paMesZ))[newIndex]-newM.z );

		newM_PointMireImageSur_newM.Normalize();
	
		dnewV= newM_PointMireImageSur_newM - m_PreComputed_NormalizedCM[newIndex];
		dnewV.Normalize();
	}

	if ( _isnan(newM.Sum()) || _isnan(dnewV.Sum()) ){
		Condition=long(POINT_TRAITE_SS_SUCCES);
	}
	else{
		Condition=long(POINT_TRAITE);
		newV=H3_VECTOR3D_FLT32(dnewV.x,dnewV.y,dnewV.z);
	}

	return Condition;
}
/*! 
* 	\fn      long NormalPosition::CalculNext(	const SPtComplet& D,SPtComplet &A,
									const H3_POINT2D_FLT64 dpx_Delta)
* 	\brief   calcul des valeurs metrique de points le long d'un parcours direct allant du point de depart au point d'arrive par increment 'dpx_Delta'
*				ne retient comme resultat de calcul que la derniere valeur, à savoir A
* 	\param   const SPtComplet& D : le point de depart (position pixel et metriques initialisées)  
* 	\param out  SPtComplet &A : le point d'arrivé (pos. pix initialisée avant d'entrer dans la fonction, le reste calculé)
* 	\param   const H3_POINT2D_FLT64 dpx_Delta :  l'increment (vecteur dans l'axe D vers A, sinon pb)
* 	\return  long
* 	\remarks 
*/ 
#define DIST_MAX (10L)//distance

/*! 
* 	\fn      long NormalPosition::CalculNext(	const SPtComplet& D,/* SPtComplet** Array,const H3_POINT2D_FLT64 dpx_Delta)
* 	\brief   calcul des valeurs metrique de points le long d'un parcours direct allant du point de depart au bord du masque par increment 'dpx_Delta'
*			le point de depart de l'integration est donnée en pixel (pxdepart). Il doit etre initialisé dans le tableau Array.
* 	\param   H3_POINT2D_INT32& pxDepart : index du point de depart (position pixel et metriques initialisées dans Array)  
* 	\param out  SPtComplet** Array : tableau DE LA TAILLE DU MASQUE initialisé à la position pxDepart (au moins)
* 	\param   const H3_POINT2D_INT8 ipx_Delta :  l'increment (vecteur dans l'axe duquel on souhaite faire le calcul _sens + direction_)
* 	\return  long
* 	\remarks si Array n'a pas les dimensions de m_Mask, il faut s'attendre à une grosse erreur (pointeur dans le vide). Pas de verification.
*/ 

size_t NormalPosition::CalculNext(	const H3_POINT2D_UINT32& pxDepart,
									const DataIn & DI,
									DataOut_2D& DO,
									const H3_POINT2D_FLT32& fpx_Delta0)const
{
	CString strFunction("CalculNext(2)");

	size_t NbPointsTraitesSup= 0L;
	const size_t nco= DI.paMask->GetCo(), nli= DI.paMask->GetLi();

	//check
	if(!((pxDepart.x<nco) && (pxDepart.y<nli) &&
		 (pxDepart.x>=0 ) && (pxDepart.y>=0 ) ))
	{
		H3DisplayWarning(strModule,strFunction,"Le point de départ n'est pas dans l'image");
		return 0L;
	}
	if((*(DO.paMask))(pxDepart.y,pxDepart.x) <= 0)
	{
		H3DisplayWarning(strModule,strFunction,"Le point de départ n'est pas dans le masque");
		return 0L;
	}
	if((*(DO.paMask))(pxDepart.y,pxDepart.x) != POINT_TRAITE)
	{
		H3DisplayWarning(strModule,strFunction,"Le point de départ n'est pas traité");
		return 0L;
	}
	
	//initialisation
	H3_POINT2D_FLT32 fpx_Delta(fpx_Delta0);

	//verification
	H3_POINT2D_UINT32 l_pxD= pxDepart;//pixel de depart

	//verification
	//[l_nexty,l_nextx] est le pix d'arrivé lors de l'integration
	long l_nexty= l_pxD.y+(long)fpx_Delta.y,	l_nextx= l_pxD.x+(long)fpx_Delta.x;
	if(!((l_nextx < nco) && (l_nexty < nli) && (l_nextx >= 0 ) && (l_nexty >= 0 ) ))
		return 0L;

	//initialisation
	__int8 Condition;

	Condition=  (*(DO.paMask))(l_nexty,l_nextx);
	bool cond1= (POINT_A_TRAITER == Condition);
	while( cond1 )
	{
		if(POINT_TRAITE == CalculPointVoisin(l_pxD.x + DI.nx  * l_pxD.y, l_nextx + DI.nx * l_nexty, DI, DO))
		{
			NbPointsTraitesSup++;
			//(*(DO.paMask))(l_nexty,l_nextx)= POINT_TRAITE;
 
			//on va voir le pixel suivant
			l_pxD.x= l_nextx;
			l_pxD.y= l_nexty;

			fpx_Delta += fpx_Delta0;

			l_nexty= pxDepart.y + (long)fpx_Delta.y;
			l_nextx= pxDepart.x + (long)fpx_Delta.x;

			if(!((l_nextx < nco) && (l_nexty < nli) &&
				 (l_nextx >= 0 ) && (l_nexty >= 0 ) ))
				Condition= HORS_MATRICE;
			else
				Condition= (*(DO.paMask))(l_nexty, l_nextx);

#if TEST			
	//essai: on recalule le point de depart à partir du point d'arrivée
	CalculPointVoisin(*pDtemp,fpx_Delta*(-1.0f), Ptemp);
	if(fabs(pDtempOld->pt.z-Ptemp.pt.z) > FLT_EPSILON)
	{
	/*CString msg;msg.Format("P0:x=%f y=%f z=%f\nP1:x=%f y=%f z=%f\nE :x=%g y=%g z=%g",
						   pDtempOld->pt.x,pDtempOld->pt.y,pDtempOld->pt.z,
						   Ptemp.pt.x,Ptemp.pt.y,Ptemp.pt.z,
						   pDtempOld->pt.x-Ptemp.pt.x,pDtempOld->pt.y-Ptemp.pt.y,pDtempOld->pt.z-Ptemp.pt.z);
	AfxMessageBox(msg);*/
	gInfo(Ptemp.px.y,Ptemp.px.x)= pDtempOld->pt.z-Ptemp.pt.z;
	}
#endif

		}
		else
		{
			Condition= POINT_TRAITE_SS_SUCCES;
			(*(DO.paMask))(l_nexty, l_nextx)= POINT_TRAITE_SS_SUCCES;
		}

		cond1= (POINT_A_TRAITER == Condition) ;
	}


	return NbPointsTraitesSup;
}

//il faut que le 1er point de DO soit initialisé
size_t NormalPosition::CalculNext(	const H3_POINT2D_UINT32& pxDepart,
									const DataIn& DI,
									DataOut_1D& DO,
									const H3_POINT2D_FLT32& fpx_Delta0)const
{
	CString strFunction("CalculNext(2)");

	size_t NbPointsTraitesSup= 0L;
	const size_t NbPointsTraitesMax= DO.paMask->GetSize();
	if(NbPointsTraitesMax < 2) return 0;

	//check
	if(!((pxDepart.x<DI.nx) && (pxDepart.y<DI.ny) &&
		 (pxDepart.x>=0 ) && (pxDepart.y>=0 ) ))
	{
		H3DisplayWarning(strModule,strFunction,"Le point de départ n'est pas dans l'image");
		return 0L;
	}
	if((*(DO.paMask))[0] <= 0)
	{
		H3DisplayWarning(strModule,strFunction,"Le point de départ n'est pas dans le masque");
		return 0L;
	}
	if(POINT_TRAITE != (*(DO.paMask))[0])
	{
		H3DisplayWarning(strModule,strFunction,"Le point de départ n'est pas traité");
		return 0L;
	}
	
	//initialisation
	H3_POINT2D_FLT32 fpx_Delta(fpx_Delta0);

	//verification
	H3_POINT2D_UINT32 l_pxD=pxDepart;//pixel de depart

	//verification
	//[l_nexty,l_nextx] est le pix d'arrivé lors de l'integration
	long l_nexty= l_pxD.y+(long)fpx_Delta.y,	l_nextx= l_pxD.x+(long)fpx_Delta.x;
	if(!((l_nextx<(long)DI.nx) && (l_nexty<(long)DI.ny) && (l_nextx>=0 ) && (l_nexty>=0 ) ))
		return 0L;

	//initialisation
	__int8 Condition;

	Condition= (*(DO.paMask))[0];
	bool cond1= (POINT_A_TRAITER == Condition);
	while( cond1 )
	{
		if(POINT_TRAITE == CalculPointVoisin(l_nextx+DI.nx*l_nexty,DI,NbPointsTraitesSup,NbPointsTraitesSup+1,DO))
		{
			++NbPointsTraitesSup;
			(*(DO.paMask))[NbPointsTraitesSup]= POINT_TRAITE;
 
			//on va voir le pixel suivant
			l_pxD.x= l_nextx;
			l_pxD.y= l_nexty;

			fpx_Delta += fpx_Delta0;

			l_nexty = l_pxD.y + (long)fpx_Delta.y;
			l_nextx = l_pxD.x + (long)fpx_Delta.x;

			if(!((l_nextx<(long)DI.nx) && (l_nexty<(long)DI.ny) &&
				 (l_nextx>=0 )   && (l_nexty>=0 ) ))
				 Condition= HORS_MATRICE;
			else
			{
				//Condition=(*(DO.paMask))(l_nexty,l_nextx);
			}

#if TEST			
	//essai: on recalule le point de depart à partir du point d'arrivée
	CalculPointVoisin(*pDtemp, fpx_Delta*(-1.0f), Ptemp);

	if(fabs(pDtempOld->pt.z-Ptemp.pt.z)>FLT_EPSILON)
	{
	/*CString msg;msg.Format("P0:x=%f y=%f z=%f\nP1:x=%f y=%f z=%f\nE :x=%g y=%g z=%g",
						   pDtempOld->pt.x,pDtempOld->pt.y,pDtempOld->pt.z,
						   Ptemp.pt.x,Ptemp.pt.y,Ptemp.pt.z,
						   pDtempOld->pt.x-Ptemp.pt.x,pDtempOld->pt.y-Ptemp.pt.y,pDtempOld->pt.z-Ptemp.pt.z);
	AfxMessageBox(msg);*/
	gInfo(Ptemp.px.y,Ptemp.px.x)=pDtempOld->pt.z-Ptemp.pt.z;
	}
#endif
		}
		else
		{
			Condition= POINT_TRAITE_SS_SUCCES;
			(*(DO.paMask))[NbPointsTraitesSup]= POINT_TRAITE_SS_SUCCES;
		}

		cond1= ((POINT_A_TRAITER==Condition) && (NbPointsTraitesSup+1<NbPointsTraitesMax)) ;
	}

	return NbPointsTraitesSup;
}

/*! 
* 	\fn      : NormalPosition::Integr_2
* 	\brief   : calcul la position d'un point A connaissant celle d'un point D
* 	\param   : const SPtComplet& D : point de départ completement initialisé
* 	\param   : SPtComplet& A : point d'arrivé. Position pixel initialisée
* 	\return  : long : nombre d'etape effectuées pour aller de D à A
* 	\remarks : integre sur un chemin rectiligne dans l'image
*/ 
/*
size_t NormalPosition::Integr_2(H3_POINT2D_UINT32& Depart_pix,H3_POINT2D_UINT32& Arriv_pix, const DataIn& DI, DataOut_0D& DO)const
{
	double d_dx=Arriv_pix.x-Depart_pix.x;
	double  d_dy=Arriv_pix.y-Depart_pix.y;
	double dmax_xy=__max(fabs(d_dx),fabs(d_dy));

	H3_POINT2D_FLT64 Sens;

	if(dmax_xy<=1)//pas besoin de normaliser
		Sens=H3_POINT2D_FLT64(d_dx,d_dy);
	else
		Sens=H3_POINT2D_FLT64(d_dx/dmax_xy,d_dy/dmax_xy);
		

	unsigned long nbPointTraite=0;
	const unsigned long nbPoints=__max( 2 , floor(dmax_xy+1.5) );

	DataOut_1D DO_1D(nbPoints);

	nbPointTraite= CalculNext(Depart_pix,DI,DO_1D,Sens);

CString msg;
msg.Format(_T("nbTraité:%d nbPtsATraiter=%d"),nbPointTraite,nbPoints);
H3DebugInfo(strModule,_T("Integr_2"),msg);

	DO.Pos =(*(DO_1D.paPos))[nbPointTraite];
	DO.N   =(*(DO_1D.paN))[nbPointTraite];
	DO.Mask=(*(DO_1D.paMask))[nbPointTraite];
//si nbPoints != nbPointTraite >> le calcul n'est pas arrivé au bout >> pb (quelques points (à priori sur les bords) n'ont pas été traités)
	return nbPointTraite;
}
*/
/*! 
* 	\fn      : NormalPosition::Integr_
* 	\brief   : 
* 	\param   : CPoint Depart_pix : 
* 	\return  : long
* 	\remarks : 
*/ 

size_t NormalPosition::Integr_(H3_POINT2D_UINT32& Depart_pix, const DataIn& DI, DataOut_2D& DO)const
{
	//constante utile
	const long sz_float= sizeof(float);
	const size_t nCo=		 DI.paMask->GetCo();
	const size_t nLi=		 DI.paMask->GetLi();

	H3_POINT2D_INT32 Sens(0L, 0L);
	size_t index= Depart_pix.x + Depart_pix.y * nCo;

	size_t nbPointTraite= 0, nbPointsTraitesSup;

	//en haut
	Sens= H3_POINT2D_INT32(0L,-1L);	
	nbPointsTraitesSup= CalculNext(Depart_pix, DI, DO, Sens);
	nbPointTraite += nbPointsTraitesSup;

	//en bas
	Sens= H3_POINT2D_INT32(0L,1L);
	nbPointsTraitesSup= CalculNext(Depart_pix, DI, DO, Sens);
	nbPointTraite += nbPointsTraitesSup;

	H3_ARRAY_INT32 MaskColon;
	MaskColon= (*(DO.paMask)).GetAt(0L, Depart_pix.x, nLi, 1L);

	//on cherche les points traités
	H3_ARRAY_FLT32 PixTraite= MaskColon.Find(POINT_TRAITE);	
	H3_POINT2D_UINT32 pxDep(Depart_pix.x, 0L);
	unsigned long i;

	//à droite
	Sens=H3_POINT2D_INT32(1L,0L);
	for (i=0; i<PixTraite.GetSize(); i++){
		pxDep.y= (unsigned long)PixTraite[i];
		nbPointsTraitesSup= CalculNext(pxDep, DI, DO, Sens);
		nbPointTraite += nbPointsTraitesSup;
	}
	//à gauche
	Sens=H3_POINT2D_INT32(-1L,0L);
	for (i=0; i<PixTraite.GetSize(); i++){
		pxDep.y= (unsigned long)PixTraite[i];
		nbPointsTraitesSup= CalculNext(pxDep, DI, DO, Sens);
		nbPointTraite += nbPointsTraitesSup;
	}

	return nbPointTraite;
}


/*! 
* 	\fn      : NormalPositionMap::NouvelleRef_fct
* 	\brief   : Apres traitement d'une partie des points, il se peut que l'integration s'arete alors que tous les points n'ont pas ete traité
*	\			on demande alors un nouveau point de départ.
* 	\return  : CPoint : point de depart renvoyé par la fonction. Il n'est pas traité mais touche au moins un point traité
*	\et est entouré de au moins 10 (d) points valides dans toutes les directions
* 	\remarks : fct appelée par NormalPositionMap::NouvelleRef, traitement principal de NouvelleRef
*/

H3_POINT2D_INT32 NormalPosition::NouvelleRef_fct(const H3_ARRAY2D_INT8& aMask,const H3_ARRAY_UINT32& Points_a_traiter,long dist)const
{
	const size_t nCo = aMask.GetCo(),		nLi = aMask.GetLi();

	const size_t sz = Points_a_traiter.GetSize();

	if(0 == sz) 
	{
		return(H3_POINT2D_INT32(-1,-1));
	}

	for (long i = 0; i < sz; i++){

		//recherche d'un point dont les (2d+1)*(2d+1) voisins sont valides
		//long X=long(Points_a_traiter[i])%(mMask.GetCo()), Y=long((Points_a_traiter[i]-(X))/mMask.GetCo());
		//long X=long(Points_a_traiter(i))%(Mask.szX)+1, Y=long((Points_a_traiter(i)-(X-1))/Mask.szX+1);
		long Y = long( floor(float(Points_a_traiter[i]) / nCo));
		long X = long( (Points_a_traiter[i]) -Y * nCo);

		//on cherche un point de départ à une distance d des bords 1:de l'image 2:du masque 
		bool Cond1=( (X >= dist) && (Y >= dist) && (nCo-X > dist) && (nLi-Y > dist) );
		if(Cond1) 
		{
			bool Cond2 = true;
			//voir les points voisins
			long Pos= (Y)*nCo + (X);//pointeur sur le point retenu
			__int8* pPos= aMask.GetData() + Pos;
			long offset1,offset2;

			for (long j= -dist; (j <= dist) && (Cond2); j++){
				offset1=j*nCo;

				for(long k=-dist;(k<=dist) && (Cond2);k++){
					offset2=offset1+k;
					Cond2=(Cond2 && (pPos[offset2]==POINT_A_TRAITER || pPos[offset2]==POINT_TRAITE));
				}
			}

			if (Cond2){

				/*
				long offset1,offset2;
				for (j=-1;(j<=1);j++){
					offset1=j*mMask.GetCo();
					for(long k=-1;(k<=1);k++){
						offset2=offset1+k;
						if(pPos[offset2]==POINT_TRAITE) return(CPoint(X+k,Y+j));
					}
				}*/
				offset1=nCo;
				if		(pPos[-1]==POINT_TRAITE)		{
					//printf("1	%d %d\n",X-1,Y);

					return(H3_POINT2D_INT32(X-1, Y));
				}
				else if (pPos[1]==POINT_TRAITE)			{
					//printf("2	%d %d\n",X+1,Y);
					return(H3_POINT2D_INT32(X+1, Y));
				}
				else if (pPos[-offset1]==POINT_TRAITE)	{
					//printf("3	%d %d\n",X,Y-1);
					return(H3_POINT2D_INT32(X, Y-1));
				}
				else if	(pPos[offset1] ==POINT_TRAITE)	{
					//printf("4	%d %d\n",X,Y+1);
					return(H3_POINT2D_INT32(X, Y+1));
				}
			}
		}
	}

	return((H3_POINT2D_INT32(-1,-1)));
}


/*! 
* 	\fn      : NormalPositionMap::NouvelleRef
* 	\brief   : Apres traitement d'une partie des points, il se peut que l'integration s'arrete alors que tous les points n'ont pas ete traité
*	\			on demande alors un nouveau point de départ.
* 	\return  : CPoint : point de depart renvoyé par la fonction. Il n'est pas traité mais touche au moins un point traité
*	\et est entouré de au moins 10 (DimZoneRechercheRef) points valides dans toutes les directions
* 	\remarks : 
*/ 

H3_POINT2D_INT32 NormalPosition::NouvelleRef(const H3_ARRAY2D_INT8& aMask,long & DimZoneRechercheRef)const
{
	CString strFunction("NouvelleRef()");

	H3_POINT2D_INT32 retPoint(-1,-1); 

	H3_ARRAY_UINT32 Points_a_traiter=aMask.Find((__int8)POINT_A_TRAITER);
	long sz=Points_a_traiter.GetSize();

	if(sz==0) 
	{
		return(retPoint);
	}

	bool continu=(DimZoneRechercheRef>0);
	while (continu)
	{
		retPoint=NouvelleRef_fct(aMask,Points_a_traiter,DimZoneRechercheRef);
		if(retPoint.x==-1 && retPoint.y==-1)
		{
			//H3DebugInfo(strModule,strFunction,"Aucun nouveau point trouvé");//appel frequent
			DimZoneRechercheRef=long (DimZoneRechercheRef/2) ;
			continu=(DimZoneRechercheRef>0);
		}
		else
		{
			//H3DebugInfo(strModule,strFunction,"nouveau point "+intToCString(retPoint.x)+" "+intToCString(retPoint.y));//appel frequent
			continu=false;
		}
	}

	return(retPoint);
}



