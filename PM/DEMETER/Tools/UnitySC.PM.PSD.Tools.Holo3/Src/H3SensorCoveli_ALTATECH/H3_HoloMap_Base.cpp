
#include "stdafx.h"
#include "Coveli.h"
#include "H3_HoloMap_Base.h"
#include "H3ImageToolsDecl.h"
#include "NormalPosition.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

extern CCoveliApp theApp;
#define DEFAULT_CFG_FILE _T("H3_HoloMap_Base.cfg")
static const CString strModule("CH3_HoloMap_Base");

#define CALIB_ARRET_OPERATEUR 1
#define CALIB_BAD_IMAGE_DIM   2
#define CALIB_CAM_ERROR		  3
#define CALIB_ERROR_UW        4

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
/*
Constructeur
*/
CH3_HoloMap_Base::CH3_HoloMap_Base()
{
	CString strFunction("CH3_HoloMap_Base"),msg;

	m_Step=100;

	H3SetDebugLevel(m_nFileLog);
	if(m_nFileLog!=0)
		H3SetDebugFile(m_strFileLog);
}

CH3_HoloMap_Base::~CH3_HoloMap_Base()
{
}

/*! 
* 	\fn      int CH3_HoloMap_Base::Calibrer(const CH3Array< H3_ARRAY2D_FLT32 > & aUWMirrorX, const CH3Array< H3_ARRAY2D_FLT32 > & aUWMirrorY, const CH3Array< H3_ARRAY2D_UINT8 > & aVideoMireXY, const H3_ARRAY_FLT32 & aMirrorHeight, const H3_ARRAY_FLT32 & aMireXYHeight)
* 	\brief	: calibrage camera puis calibrage systeme
* 	\param	:
* 	\return  
* 	\remarks 
*/
int CH3_HoloMap_Base::Calibrer(SCalibResults& SC,
							   const CH3Camera& Cam,//camera calibrée ailleurs
							   const CExtrinsic_param& ep_ObjRef_camFrame,	//position du wafer ref dans rep camera
							   const H3_ARRAY2D_FLT32 & Mes_MirrorX, //pour determiner la position du wafer ref
							   const H3_ARRAY2D_FLT32 & Mes_MirrorY, //pour determiner la position du wafer ref
							   const H3_ARRAY2D_UINT8 & Mes_MirrorMask)//pour determiner la position du wafer ref
{
	
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("Calibrer(1)"), msg;
	H3DebugInfo(strModule,strFunction,"");
	CString strMsg("Impossible de calibrer.");

	CWaitCursor wait2;

	//verifier les données
	size_t i,j,k;
	const size_t nLi=Mes_MirrorMask.GetLi(),nCo=Mes_MirrorMask.GetCo();

	if(	nLi!=Mes_MirrorX.GetLi() || nCo!=Mes_MirrorX.GetCo() ||
		nLi!=Mes_MirrorY.GetLi() || nCo!=Mes_MirrorY.GetCo() )
	{
		H3DebugError(strModule,strFunction,"les images de phase n'ont pas toutes la meme taille 2");
		return CALIB_BAD_IMAGE_DIM;
	}

	//fin verifications
	CExtrinsic_param EP_imMireHMAP;//la position de l'image de la mire HMAP (image/au miroir ref) / camera
		
	//relever de quelques pts sur l'image Metrique
	unsigned long StepLi=m_Step,StepCo=m_Step, nbValidPhase=0;
	for(i=StepLi;i<nLi;i+=StepLi)
		for(j=StepCo;j<nCo;j+=StepCo)
			nbValidPhase += (Mes_MirrorMask(i,j)!=0L);

	if(nbValidPhase<36){
		return CALIB_ERROR_UW;//il faut au moins 6*6 >> pas alignés et distant...
	}
	H3_ARRAY2D_FLT64 Metric(3,nbValidPhase),PixRef(2,nbValidPhase);

	for(i=StepLi,k=0;i<nLi;i+=StepLi)
		for(j=StepCo;j<nCo;j+=StepCo){
			if(Mes_MirrorMask(i,j)){
				PixRef(0,k)=(double)j;
				PixRef(1,k)=(double)i;

				Metric(0,k)=Mes_MirrorX(i,j);//coor metric sur la mire HMap ds le rep MireHMap
				Metric(1,k)=Mes_MirrorY(i,j);
				Metric(2,k)=0;
				k++;
			}
		}

	EP_imMireHMAP.compute_extrinsic(PixRef,Metric,Cam);//position de l'image de la mire par le miroir ref

	//CExtrinsic_param EP_MireHMAP;//la position de la mire HMAP / camera
	//1 point PMire de la mire HMAP est vue après reflexion sur l'objet
	//si PMire est exprimé dans le ref obj (x, y dans le plan obj) comme PMire_objRef
	//alors son image est iPMire_objRef(PMire_objRef.x,PMire_objRef.y, (-1)*PMire_objRef.z)
	//et reciproquement
	CRotation rObjRef2Cam=ep_ObjRef_camFrame.m_Rc;
	CRotation rImMiHM2Cam=EP_imMireHMAP.m_Rc;

	H3_MATRIX_FLT64 tObjRef2Cam=ep_ObjRef_camFrame.m_Tc;
	H3_MATRIX_FLT64 tImMiHM2Cam=EP_imMireHMAP.m_Tc;

	//passage de "l'image de la mire" au miroir ref
	CRotation rImMiHM2ObjRef=rObjRef2Cam.Trans() * rImMiHM2Cam;

	H3_MATRIX_FLT64 tImMiHM2ObjRef=rObjRef2Cam.H3_MATRIX_FLT64::Trans()*(tImMiHM2Cam-tObjRef2Cam);

	//passage de la mireHMap au miroir ref

	//par rapport à la transformation precedente (ImMiHM2ObjRef), et dans un repere ou (x,y) est dans le plan du miroir ref
	//le vecteur rotation est symetrique (signe de z change) et le sens de rotation change
	//le vecteur translation est symetrique
	CRotation rObjRef2imMiHM=rImMiHM2ObjRef.Trans();
	H3_MATRIX_FLT64 omc_rObjRef2imMiHM=rObjRef2imMiHM.Rodrigues();
	H3_MATRIX_FLT64 omc_rObjRef2MiHM=omc_rObjRef2imMiHM;
	omc_rObjRef2MiHM[0] *= -1;
	omc_rObjRef2MiHM[1] *= -1;
	CRotation rMiHM2ObjRef=omc_rObjRef2MiHM.Rodrigues();
	rMiHM2ObjRef=rMiHM2ObjRef.Trans();

	H3_MATRIX_FLT64 tMiHM2ObjRef=tImMiHM2ObjRef;
	tMiHM2ObjRef[2]*=-1;
	CExtrinsic_param EP_MiHM2ObjRef(rMiHM2ObjRef.Rodrigues(),tMiHM2ObjRef);

	//passage de la mireHMap à la camera
	CRotation rMiHM2Cam=rObjRef2Cam*rMiHM2ObjRef;
	H3_MATRIX_FLT64 tMiHM2Cam=rObjRef2Cam.H3_MATRIX_FLT64::operator*( tMiHM2ObjRef);
	tMiHM2Cam += tObjRef2Cam;

	//ep_mireHMap dans le repere camera
	CExtrinsic_param EP_MireHMAP(rMiHM2Cam.Rodrigues(),tMiHM2Cam);

	//passage de la camera à l'objet ref
	CRotation rCam2ObjRef=rObjRef2Cam.Trans();
	H3_MATRIX_FLT64 tCam2ObjRef=rCam2ObjRef.H3_MATRIX_FLT64::operator*(tObjRef2Cam*(-1));
	CExtrinsic_param EP_Cam2ObjRef(rCam2ObjRef.Rodrigues(),tCam2ObjRef);

/////////////////////////////////////////////////////////Initialisation
	SC.ep_MireHMAP=EP_MireHMAP;//le seul element nouveau
	//SC.ep_ObjRef=ep_ObjRef_camFrame;

	//les calculs se font dans le repere objet
	mCam=Cam;
	m_ep_Cam=EP_Cam2ObjRef;
	m_ep_MireHMAP=EP_MiHM2ObjRef;

	//pas forcement necessaire au moment du calibrage, mais à faire avant une mesure
	//donc si on veut calibrer puis mesurer il faut le faire quand meme
	bool b1=m_NP.SetParams(Cam,EP_MiHM2ObjRef,EP_Cam2ObjRef);
	if(b1!=true)
		return 1;

	return 0;
}

/*! 
* 	\fn      int CH3_HoloMap_Base::init(const CH3Camera & Cam, const CExtrinsic_param& ep_ObjRef_CamFrame, const CExtrinsic_param& ep_MireHMAP_CamFrame)
* 	\brief	 point d'altitude connu a milieu de l'image
* 	\param	:
* 	\return  
* 	\remarks 
*/
int CH3_HoloMap_Base::init(const CH3Camera & Cam, const CExtrinsic_param& waferToCam, const CExtrinsic_param& screenToCam)
{
	//passage de la camera à l'objet ref
	CRotation rCam2ObjRef= waferToCam.m_Rc.Trans();
	H3_MATRIX_FLT64 tCam2ObjRef=rCam2ObjRef.H3_MATRIX_FLT64::operator*(waferToCam.m_Tc*(-1));
	CExtrinsic_param EP_Cam2ObjRef(rCam2ObjRef.Rodrigues(),tCam2ObjRef);

	//passage de la mire ref à l'objet ref
	//passage de la camera à l'objet ref
	CRotation rMireHMAP2ObjRef=rCam2ObjRef* screenToCam.m_Rc;
	H3_MATRIX_FLT64 tMireHMAP2ObjRef=rCam2ObjRef.H3_MATRIX_FLT64::operator*(screenToCam.m_Tc)+tCam2ObjRef;
	CExtrinsic_param EP_MiHM2ObjRef(rMireHMAP2ObjRef.Rodrigues(),tMireHMAP2ObjRef);

	mCam=Cam;
	m_ep_Cam=EP_Cam2ObjRef;
	m_ep_MireHMAP=EP_MiHM2ObjRef;

	bool b1=m_NP.SetParams(Cam,EP_MiHM2ObjRef,EP_Cam2ObjRef);
	if(b1!=true)
		return 1;
	else return 0;
}

/*! 
* 	\fn      int CH3_HoloMap_Base::Mesurer(SMesure& Mes,const H3_ARRAY2D_FLT32 & aOnMireHMapX, const H3_ARRAY2D_FLT32 & aOnMireHMapY, const H3_ARRAY2D_UINT8 & aMask)
* 	\brief	 point d'altitude connu a milieu de l'image
* 	\param	:
* 	\return  
* 	\remarks 
*/
int CH3_HoloMap_Base::Mesurer_Z0(SMesure& Mes,const H3_ARRAY2D_FLT32 & aOnMireHMapX, const H3_ARRAY2D_FLT32 & aOnMireHMapY, const H3_ARRAY2D_UINT8 & aMask)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("Mesurer_Z0()");
	CString strMsg("Impossible de mesurer.");
	H3DebugInfo(strModule,strFunction,"");

	CWaitCursor wait2;

	size_t nLi=aMask.GetLi();
	size_t nCo=aMask.GetCo();

	if(nLi*nCo==0L)
	{
		H3DebugError(strModule,strFunction,"La cartographie de Mesure est vide");
		// Erreur Code 3
		return 3;
	}

	Mes.aPts.ReAlloc(nLi,nCo);
	Mes.aPts.Fill(NaN);
	Mes.aNs.ReAlloc(nLi,nCo);
	Mes.aNs.Fill(NaN);
	Mes.aMask.ReAlloc(nLi,nCo);
	Mes.aMask.Fill(0);

	//Il faut transformer les données metriques en tenant compte des param extrinseques de la mire
	H3_ARRAY2D_FLT32 MesX(nLi,nCo),MesY(nLi,nCo),MesZ(nLi,nCo);
	double	R00=m_ep_MireHMAP.m_Rc[0],R01=m_ep_MireHMAP.m_Rc[1],R02=m_ep_MireHMAP.m_Rc[2],
			R10=m_ep_MireHMAP.m_Rc[3],R11=m_ep_MireHMAP.m_Rc[4],R12=m_ep_MireHMAP.m_Rc[5],
			R20=m_ep_MireHMAP.m_Rc[6],R21=m_ep_MireHMAP.m_Rc[7],R22=m_ep_MireHMAP.m_Rc[8];
	double	Tx=m_ep_MireHMAP.m_Tc[0],Ty=m_ep_MireHMAP.m_Tc[1],Tz=m_ep_MireHMAP.m_Tc[2];
	double x,y;
	size_t i,j,k;
	for(i=0,k=0;i<nLi;i++){
		for(j=0;j<nCo;j++,k++){
			if(aMask[k]){
				x=aOnMireHMapX[k];
				y=aOnMireHMapY[k];

				MesX[k]=R00*x+R01*y+Tx;
				MesY[k]=R10*x+R11*y+Ty;
				MesZ[k]=R20*x+R21*y+Tz;
			}
		}
	}

	//puis faire la mesure
	long nbpointsTraites=m_NP.ComputeMaps_Z0(MesX,MesY,MesZ,aMask,Mes.aPts,Mes.aNs,Mes.aMask);

	return 0;
}

int CH3_HoloMap_Base::Mesurer_Zreal(SMesure& Mes,const H3_ARRAY2D_FLT32 & aOnMireHMap_X, const H3_ARRAY2D_FLT32 & aOnMireHMap_Y, const H3_ARRAY2D_UINT8 & aOnMireHMap_MASK, const size_t nPixRefX, const size_t nPixRefY, const float fRefZ)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("Mesurer_Z0()");
	CString strMsg("Impossible de mesurer.");
	H3DebugInfo(strModule,strFunction,"");

	CWaitCursor wait2;

	size_t nLi=aOnMireHMap_MASK.GetLi();
	size_t nCo=aOnMireHMap_MASK.GetCo();

	if(nLi*nCo==0L)
	{
		H3DebugError(strModule,strFunction,"La cartographie de Mesure est vide");
		// Erreur Code 3
		return 3;
	}

	Mes.aPts.ReAlloc(nLi,nCo);
	Mes.aPts.Fill(NaN);
	Mes.aNs.ReAlloc(nLi,nCo);
	Mes.aNs.Fill(NaN);
	Mes.aMask.ReAlloc(nLi,nCo);
	Mes.aMask.Fill(POINT_A_NE_PAS_TRAITER);

	//Il faut transformer les données metriques en tenant compte des param extrinseques de la mire
	H3_ARRAY2D_FLT32 MesX(nLi,nCo),MesY(nLi,nCo),MesZ(nLi,nCo);
	double	R00=m_ep_MireHMAP.m_Rc[0],R01=m_ep_MireHMAP.m_Rc[1],R02=m_ep_MireHMAP.m_Rc[2],
			R10=m_ep_MireHMAP.m_Rc[3],R11=m_ep_MireHMAP.m_Rc[4],R12=m_ep_MireHMAP.m_Rc[5],
			R20=m_ep_MireHMAP.m_Rc[6],R21=m_ep_MireHMAP.m_Rc[7],R22=m_ep_MireHMAP.m_Rc[8];
	double	Tx=m_ep_MireHMAP.m_Tc[0],Ty=m_ep_MireHMAP.m_Tc[1],Tz=m_ep_MireHMAP.m_Tc[2];
	double x,y;
	size_t i,j,k;
	for(i=0,k=0;i<nLi;i++){
		for(j=0;j<nCo;j++,k++){
			if(aOnMireHMap_MASK[k]){
				x=aOnMireHMap_X[k];
				y=aOnMireHMap_Y[k];

				MesX[k]=R00*x+R01*y+Tx;
				MesY[k]=R10*x+R11*y+Ty;
				MesZ[k]=R20*x+R21*y+Tz;
				
				Mes.aMask[k]= POINT_A_TRAITER;
			}
		}
	}

	//puis faire la mesure

	long nbpointsTraites=m_NP.ComputeMaps_Z(MesX, MesY, MesZ,
											Mes.aMask,
											nPixRefX, nPixRefY, fRefZ,
											Mes.aPts, Mes.aNs, Mes.aMask);

	return 0;
}



/* H3_ARRAY2D_UINT32 sort2D(const H3_ARRAY_PT3DFLT32& PosMM, float stepX,float stepY)
*  brief
*  param
*  remarks 
*/
static H3_ARRAY2D_UINT32 sort2D(const H3_ARRAY_PT3DFLT32& PosMM, float stepX,float stepY)
{
	CString strFunction("sort2D()");
	H3DebugInfo(strModule,strFunction,"");

	float minX=PosMM[0].x,minY=PosMM[0].y,maxX=PosMM[0].x,maxY=PosMM[0].y,x,y;
	long i;
	for(i=1L;i< PosMM.GetSize();i++)
	{
		y=PosMM[i].y;
		if(x<minX) minX=x;
		else{
			if(x>maxX) maxX=x;
		}			
		if(y<minY) minY=y;
		else{
			if(y>maxY) maxY=y;
		}
	}
	H3_ARRAY2D_UINT32 tab((H3_UINT32)floor((maxY-minY+0.5)/stepY),(H3_UINT32)floor((maxX-minX+0.5)/stepX));
	tab.Fill(ULONG_MAX);
	for(i=0L;i< PosMM.GetSize();i++){
		size_t x=(size_t)floor((PosMM[i].x-minX+0.5)/stepX);
		size_t y=(size_t)floor((PosMM[i].y-minY+0.5)/stepY);
		tab(y, x)=(H3_UINT32)i;
	}
	return tab;
}

/* H3_ARRAY2D_UINT32 sort2D(const H3_ARRAY2D_FLT32& PosMM, float stepX,float stepY)
*  brief
*  param
*  remarks 
*/
static H3_ARRAY2D_UINT32 sort2D(const H3_ARRAY2D_FLT32& PosMM, float stepX,float stepY)
{
	CString strFunction("sort2D()");
	H3DebugInfo(strModule,strFunction,"");

	if(PosMM.GetCo()<2)
	{
		H3DisplayError(strModule,strFunction,"Nombre de colonnes incorrect.");
		H3_ARRAY2D_UINT32 retArray(0,0);
		return retArray;
	}

	float minX=PosMM(0,0),minY=PosMM(0,1),maxX=PosMM(0,0),maxY=PosMM(0,1),x,y;
	long i;
	for(i=1L;i<PosMM.GetLi();i++)
	{
		x=PosMM(i,0);
		y=PosMM(i,1);
		if(x<minX) minX=x;
		else{
			if(x>maxX) maxX=x;
		}			
		if(y<minY) minY=y;
		else{
			if(y>maxY) maxY=y;
		}
	}
    long szXtab = (long)floor((maxX - minX) / stepX + 0.5) + 1;
    long szYtab = (long)floor((maxY - minY) / stepY + 0.5) + 1;
	H3_ARRAY2D_UINT32 tab(szYtab,szXtab);
	tab.Fill(ULONG_MAX);
	for(i=0L;i< PosMM.GetLi();i++){
        size_t x=(size_t)floor((PosMM(i,0)-minX)/stepX+0.5);
        size_t y=(size_t)floor((PosMM(i,1)-minY)/stepY+0.5);

		tab(y,x)=i;
	}
	return tab;
}
