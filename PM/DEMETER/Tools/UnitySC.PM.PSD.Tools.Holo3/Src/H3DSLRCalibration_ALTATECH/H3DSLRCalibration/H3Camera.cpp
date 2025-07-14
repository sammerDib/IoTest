/*! 
* 	\file    C:\...\H3Camera.cpp
* 	\brief   
* 	\version 
* 	\author  S Jaminion
* 	\date    15/11/2007
* 	\remarks 
*/ 


// H3Camera.cpp: implementation of the CH3Camera class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "H3DSLRCalibration.h"

#include "H3Camera.h"
#include "H3AppToolsDecl.h"
#include "H3Target.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

static CString strModule("H3Camera");
#define TAILLE_MIRE_X 10	// nombre d'intersection en X
#define TAILLE_MIRE_Y 10	// nombre d'intersection en Y

#define PAS_MIRE_X 10f		// pas en X en mm
#define PAS_MIRE_Y 10f		// pas en Y en mm

//#define	DEFAULT_CAM_FC0	(2818.22558f)
//#define	DEFAULT_CAM_FC1	(2818.54322f)
//#define	DEFAULT_CAM_CC0	(620.18239f)
//#define	DEFAULT_CAM_CC1	(457.47695f)
//#define	DEFAULT_CAM_KC0	(-0.38f)
//#define	DEFAULT_CAM_KC1	(0.33f)
//#define	DEFAULT_CAM_KC2	(0.00255f)
//#define	DEFAULT_CAM_KC3	(0.00521f)
//#define	DEFAULT_CAM_KC4	(0.0f)
//#define	DEFAULT_ALPHAC	(0.0f)

//#define DEFAULT_CALIB_PIX_ERR (0.0f)

//#define	DEFAULT_ISKC1 (1)
//#define	DEFAULT_ISKC2 (1) 
//#define	DEFAULT_ISKC3 (1)
//#define	DEFAULT_ISKC4 (1)
//#define	DEFAULT_ISKC5 (0)
//#define	DEFAULT_ISALPHA (0)
//#define	DEFAULT_ISRC (1)

#define DEFAULT_COEF_STD	3.0f
#define DEFAULT_CALIB_NAME			"Calib"

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////
// fct static
//d'apres la fonction comp_error_calib de matlab
//////////////////////////////////////////////////////////////////////
static bool comp_error_calib(CH3Camera& Cam_IN,
					CH3Array<strPos>& A_Pos,
					CL_ARRAY2D& err_std)
{
	unsigned long NbEl=0,kk;
	for(kk=0;kk<A_Pos.GetSize();kk++)
	{
		if(_finite(A_Pos[kk].m_ExtP.m_omc(0)) && A_Pos[kk].mb_isActive)
			NbEl+=A_Pos[kk].m_CList.MetricData.GetCo();
	}
	CL_ARRAY2D x(2,NbEl),y(2,NbEl),ytemp,delta;
	NbEl=0;
	for(kk=0;kk<A_Pos.GetSize();kk++)
	{
		if(_finite(A_Pos[kk].m_ExtP.m_omc(0)) && A_Pos[kk].mb_isActive)
		{
			x.SetAt(0,NbEl,A_Pos[kk].m_CList.Pixel);

			A_Pos[kk].m_ExtP.projectPoints2(A_Pos[kk].m_CList.MetricData,Cam_IN,ytemp);
			y.SetAt(0,NbEl,ytemp);
			// Err = ecart Pixel mesuré - Pixel théorique
			A_Pos[kk].m_CList.Err=A_Pos[kk].m_CList.Pixel-ytemp;

			NbEl+=A_Pos[kk].m_CList.MetricData.GetCo();
		}
	}
	delta=x-y;
	CL_ARRAY2D deltax(1,NbEl),deltay(1,NbEl);
	CL_TYPE *p1=delta.GetLine(0),*p2=delta.GetLine(1),*px=deltax.GetData(),*py=deltay.GetData();
	while(NbEl--)
	{
		(*(px++))=(*(p1++));
		(*(py++))=(*(p2++));
	}

	CL_ARRAY res(7),resx(7),resy(7);
	if((err_std.GetLi()!=7)||(err_std.GetCo()!=3)) err_std=CL_ARRAY2D(7,3);

	resx=deltax.GetStatistics();
	resy=deltay.GetStatistics();
	res=delta.GetStatistics();

	for(kk=0;kk<7;kk++)
	{
		err_std(kk,0)=resx[kk];
		err_std(kk,1)=resy[kk];
		err_std(kk,2)=res[kk];
	}
	return true;
}

static bool extract_parameters(const H3_ARRAY_FLT64& Solution, H3_ARRAY_FLT64* pSolution_erreur, CH3Camera& Cam, CH3Array< strPos >& A_Pos)
{
	long nbIma=A_Pos.GetSize(),kk,j;

	if (Solution.GetSize()!=15+6*nbIma) return false;
	(Cam).fc[0]=		(Solution)[0];
	(Cam).fc[1]=		(Solution)[1];
	(Cam).cc[0]=		(Solution)[2];
	(Cam).cc[1]=		(Solution)[3];
	(Cam).alpha_c=		(Solution)[4];

	Cam.kc[0]=		Solution[5];
	Cam.kc[1]=		Solution[6];
	Cam.kc[2]=		Solution[7];
	Cam.kc[3]=		Solution[8];
	Cam.kc[4]=		Solution[9];

	Matrix omckk(3,1),Mtemp;
	Matrix Tckk(3,1);
	Matrix KK(3,3),Hkk;
	KK(0)=(Cam).fc[0];
	KK(1)=(Cam).fc[0]*(Cam).alpha_c;
	KK(2)=(Cam).cc[0];
	KK(3)=0;
	KK(4)=(Cam).fc[1];
	KK(5)=(Cam).cc[1];
	KK(6)=0;
	KK(7)=0;
	KK(8)=1;

	for(kk=0;kk<nbIma;kk++)
	{
		if(A_Pos[kk].mb_isActive)
		{
			j=15+6*kk;

			omckk(0)=		Solution[j++];
			omckk(1)=		Solution[j++];
			omckk(2)=		Solution[j++];
			A_Pos[kk].m_ExtP.m_omc=omckk;

			Tckk(0)=		Solution[j++];
			Tckk(1)=		Solution[j++];
			Tckk(2)=		Solution[j++];
			A_Pos[kk].m_ExtP.m_Tc=Tckk;

			A_Pos[kk].m_ExtP.m_Rc=CRotation(A_Pos[kk].m_ExtP.m_omc.Rodrigues(Mtemp));

			Mtemp=A_Pos[kk].m_ExtP.m_Rc;
			Mtemp.SetAt(0,2,Tckk);
			Hkk=KK*Mtemp;

			A_Pos[kk].m_CList.H=Hkk/Hkk(8);
		}
		else
		{
			A_Pos[kk].m_ExtP.m_omc.Fill(H3NaN);
			A_Pos[kk].m_ExtP.m_Tc.Fill(H3NaN);
			A_Pos[kk].m_ExtP.m_Rc.Fill(H3NaN);
			A_Pos[kk].m_CList.H.Fill(H3NaN);
		}
	}

	if(pSolution_erreur!=nullptr)
	{
		Cam.fc_erreur[0]=	(*pSolution_erreur)[0];
		Cam.fc_erreur[1]=	(*pSolution_erreur)[1];
		Cam.cc_erreur[0]=	(*pSolution_erreur)[2];
		Cam.cc_erreur[1]=	(*pSolution_erreur)[3];
		Cam.alpha_c_erreur=(*pSolution_erreur)[4];

		Cam.kc_erreur[0]=	(*pSolution_erreur)[5];
		Cam.kc_erreur[1]=	(*pSolution_erreur)[6];
		Cam.kc_erreur[2]=	(*pSolution_erreur)[7];
		Cam.kc_erreur[3]=	(*pSolution_erreur)[8];
		Cam.kc_erreur[4]=	(*pSolution_erreur)[9];
	}

	return true;
}
/*! 
* 	\fn      CH3Camera::CH3Camera(unsigned long _nx,unsigned long _ny):nx(_nx),ny(_ny)
* 	\author  S Jaminion
* 	\brief   : initialise par defaut les paramètres intrinsèque 
* 	\param   unsigned long _nx : 
* 	\param   unsigned long _ny : 
* 	\return  
* 	\remarks 
*/ 

CH3Camera::CH3Camera(size_t _nLi,size_t _nCo):nx(_nCo),ny(_nLi)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	InitDefault(nx, ny);
}

CH3Camera::~CH3Camera()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

}

CH3Camera & CH3Camera::operator=(const CH3Camera& Cam)
{
	if(this==&Cam)
		return *this;

	nx=Cam.nx;
	ny=Cam.ny;

	unsigned long i;
	for(i=0;i<2;i++)
	{
		cc[i]=Cam.cc[i];
		cc_erreur[i]=Cam.cc_erreur[i];

		fc[i]=Cam.fc[i];
		fc_erreur[i]=Cam.fc_erreur[i];

		pix_erreur[i]=Cam.pix_erreur[i];
	}
	for(i=0;i<5;i++)
	{
		kc[i]=Cam.kc[i];
		kc_erreur[i]=Cam.kc_erreur[i];
		m_is_dist[i]=Cam.m_is_dist[i];
	}
	alpha_c=Cam.alpha_c;
	alpha_c_erreur=Cam.alpha_c_erreur;

	m_fCalibPixelErr=Cam.m_fCalibPixelErr;
	
	m_strUnit=Cam.m_strUnit;
	mb_is_alpha=Cam.mb_is_alpha;
	mb_is_initialised=Cam.mb_is_initialised;
	mb_recompute_extrinsic=Cam.mb_recompute_extrinsic;

	return *this;
}


/////////////////////////////////////////////////////////////////////////////
/// \fn      CH3Camera::Init(int _nx, int _ny) 
/// \brief   Initialise les paramètres de la caméra
/// \param   int _nx
/// \param   int _ny
/// \author  MT
/////////////////////////////////////////////////////////////////////////////
void CH3Camera::InitDefault(size_t _nx, size_t _ny) 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());

	nx = _nx;
	ny = _ny;

	double focal_lenght=10;	//mm
	double pixel_size=0.02;	//mm

	//initialisation des parametres
	cc[0]=nx/2-0.5;	//+0.5 pour faire comme Matlab mais inutile car en C les indices commencent à 0
	cc[1]=ny/2-0.5;
	double FOVRad=0.305432619099008f;	//35/2 degres exprime en radian;


	fc[0]=(nx/2/::tan(FOVRad));
	fc[1]=(nx/2/::tan(FOVRad));

	alpha_c=0;
	kc[0]=kc[1]=kc[2]=kc[3]=kc[4]=0;

	mb_recompute_extrinsic=1;

	m_nReprojectionSigma=4;
#if 1
	short i;
	for(i=0;i<5;i++)
	{
		m_is_dist[i]=true;
		kc[i]=0;
	}
	m_is_dist[4]=false;
	mb_is_initialised=false;
	mb_is_alpha=0;	//=1: compute skew
#else
	LoadCalib("",0);//modifie nx ny f c ...
#endif

	fc_erreur[0]=0;
	fc_erreur[1]=0;
	cc_erreur[0]=0;
	cc_erreur[1]=0;
	pix_erreur[0]=0;
	pix_erreur[1]=0;
	alpha_c_erreur=0;
	kc_erreur[0]=kc_erreur[1]=kc_erreur[2]=kc_erreur[3]=kc_erreur[4]=0;
	m_fCalibPixelErr=-1;
}

void CH3Camera::Init(size_t _nx, size_t _ny,const stParamIntrin& P_I, const stParamIntrinErr& P_Ierr) 
{
	//AFX_MANAGE_STATE(AfxGetStaticModuleState());
	size_t i;

	nx = _nx;
	ny = _ny;

	//initialisation des parametres
	cc[0]=P_I.acc[0];	//+0.5 pour faire comme Matlab mais inutile car en C les indices commencent à 0
	cc[1]=P_I.acc[1];

	fc[0]=P_I.afc[0];
	fc[1]=P_I.afc[1];

	alpha_c=P_I.aalphac[0];
	for(i=0;i<5;i++)
		kc[i]=P_I.akc[i];

	mb_recompute_extrinsic=1;

	mb_is_alpha=(fabs(alpha_c)> FLT_EPSILON);	//=1: compute skew

	cc_erreur[0]=P_Ierr.acc_err[0];
	cc_erreur[1]=P_Ierr.acc_err[1];

	fc_erreur[0]=P_Ierr.afc_err[0];
	fc_erreur[1]=P_Ierr.afc_err[1];

	pix_erreur[0]=P_Ierr.apix_err[0];
	pix_erreur[1]=P_Ierr.apix_err[1];

	alpha_c_erreur=P_Ierr.aalphac_err[0];

	for(i=0;i<5;i++)
		kc_erreur[i]=P_Ierr.akc_err[i];

	m_fCalibPixelErr=-1;
}

/***************************************************************************/
//D'apres
//normalise
//Pixelin est un tableau de n elements en ligne et 2 colonnes: x et y
/***************************************************************************/

/*! 
* 	\fn      bool CH3Camera::normalise(const H3_ARRAY2D_FLT64 &Pixelin, H3_ARRAY2D_FLT64 &Pixelout) const
* 	\author  V Chalvidan	
* 	\brief   Calcul les coordonnées  pixel dans une repere retinien normalisé parfait
* 	\param   const H3_ARRAY2D_FLT64 &Pixelin : Coordonnées pixels
* 	\param   H3_ARRAY2D_FLT64 &Pixelout : coordonnées  pixel dans une repere retinien normalisé parfait
* 	\return  bool
* 	\remarks 
*/ 

bool CH3Camera::normalise(const H3_ARRAY2D_FLT64 &Pixelin, H3_ARRAY2D_FLT64 &Pixelout) const
{
	size_t nb_pixel=Pixelin.GetCo();
	if( (Pixelout.GetCo()!=nb_pixel) || (Pixelin.GetLi()!=2) || (Pixelout.GetLi()!=2) ) return false;

	H3_ARRAY2D_FLT64 pixel_distort(2,nb_pixel);
	size_t i=nb_pixel;
	double *ppix1x,*ppix1y,*ppix2x,*ppix2y;
	const double cc0=cc[0],cc1=cc[1],fc0=fc[0],fc1=fc[1];

	ppix1x=(double*)Pixelin.GetLine(0);
	ppix1y=(double*)Pixelin.GetLine(1);
	ppix2x=pixel_distort.GetLine(0);
	ppix2y=pixel_distort.GetLine(1);
	while(i--)
	{
		(*(ppix2x++))=( (*(ppix1x++)) - cc0 )/ fc0;//x
		(*(ppix2y++))=( (*(ppix1y++)) - cc1 )/ fc1;//y
	}
	if(mb_is_alpha)
	{
		i=nb_pixel;
		ppix2x=pixel_distort.GetLine(0);
		ppix2y=pixel_distort.GetLine(1);
		while(i--){
			(*(ppix2x++)) -= alpha_c*(*(ppix2y++));//x
		}
	}
	if(kc[0]*kc[0]+kc[1]*kc[1]+kc[2]*kc[2]+kc[3]*kc[3]+kc[4]*kc[4]<FLT_EPSILON)
	{	//pas de distortion
		Pixelout=pixel_distort;
		return true;
	}
	else
		return compensate_distortion_oulu(pixel_distort,Pixelout);

}

/*! 
* 	\fn      bool CH3Camera::compensate_distortion_oulu(const H3_ARRAY2D_FLT64 &Pixelin, H3_ARRAY2D_FLT64 &Pixelout) const
* 	\author  V Chalvidan
* 	\brief   
* 	\param   const H3_ARRAY2D_FLT64 &Pixelin : pixel normalisé//CV210510 en fait position dans le plan retinien
* 	\param   H3_ARRAY2D_FLT64 &Pixelout : pixel normalisé  corrigé des aberations camera
* 	\return  bool
* 	\remarks 
*/ 

bool CH3Camera::compensate_distortion_oulu(const H3_ARRAY2D_FLT64 &Pixelin, H3_ARRAY2D_FLT64 &Pixelout) const
{
	size_t nb_pixel=Pixelin.GetCo();
	if( (Pixelout.GetCo()!=nb_pixel) || (Pixelin.GetLi()!=2) || (Pixelout.GetLi()!=2) ) return false;

	double *ppix0x,*ppix0y,*ppix1,*ppix2;
	size_t i,kk;
	double x,y,r2,k_radial,dx,dy,x2,y2,xy;
	double k1=kc[0],k2=kc[1],k3=kc[4],p1=kc[2],p2=kc[3];
	
	ppix1=(double*)Pixelin.GetData();
	ppix2=(double*)Pixelout.GetData();

	i=2*nb_pixel;
	while(i--)
	{
		(*(ppix2++))= (*(ppix1++)); //initial guess
	}

	for(kk=0;kk<20;kk++)
	{
		ppix0x=(double*)Pixelin.GetData();
		ppix0y=ppix0x+nb_pixel;
		ppix1=(double*)Pixelout.GetData();
		ppix2=ppix1+nb_pixel;

		i=nb_pixel;
	
		while(i--)
		{
			x=(*ppix1);
			y=(*ppix2);
			x2=x*x;
			y2=y*y;
			xy=x*y;
			r2=x2+y2;
			k_radial=((k3*r2+k2)*r2+k1)*r2+1;
			dx=2*p1*xy+p2*(r2+2*x2);
			dy=p1*(r2+2*y2)+2*p2*xy;
			(*ppix1++)=((*(ppix0x++))-dx)/k_radial;
			(*ppix2++)=((*(ppix0y++))-dy)/k_radial;
		}	
	}
	return true;	
}

/*! 
* 	\fn      bool CH3Camera::compensate_distortion_oulu_px(const H3_ARRAY2D_FLT64 &Pixelin, H3_ARRAY2D_FLT64 &Pixelout) const
* 	\author  V Chalvidan
* 	\brief   
* 	\param   const H3_ARRAY2D_FLT64 &Pixelin : pixel CCD
* 	\param   H3_ARRAY2D_FLT64 &Pixelout : pixel CCD  corrigé des aberations camera
* 	\return  bool
* 	\remarks 
*/
bool CH3Camera::compensate_distortion_oulu_px(const H3_ARRAY2D_FLT64 &Pixelin, H3_ARRAY2D_FLT64 &Pixelout) const
{
	long nb_pixel=Pixelin.GetCo();
	if( (Pixelout.GetCo()!=nb_pixel) || (Pixelin.GetLi()!=2) || (Pixelout.GetLi()!=2) ) return false;

	H3_ARRAY2D_FLT64 retinienPixelin(2,nb_pixel),retinienPixelout(2,nb_pixel);

	double *ppix0x,*ppix0y,*ppix1,*ppix2;

	const double cc0=cc[0],cc1=cc[1];
	const double fc0=fc[0],fc1=fc[1];

	size_t i;

	//on projette les pixel sur le plan retinien
	ppix0x=(double*)Pixelin.GetData();
	ppix0y=ppix0x+nb_pixel;
	ppix1=(double*)retinienPixelin.GetData();
	ppix2=ppix1+nb_pixel;
	for(i=0;i<nb_pixel;i++)
	{
		(*ppix1++)=((*(ppix0x++))-cc0)/fc0;
		(*ppix2++)=((*(ppix0y++))-cc1)/fc1;
	}

	//correction
	compensate_distortion_oulu_px(retinienPixelin, Pixelout);

	//retour en pixel
	ppix0x=(double*)Pixelout.GetData();
	ppix0y=ppix0x+nb_pixel;
	for(i=0;i<nb_pixel;i++)
	{
		(*ppix0x) *= fc0;
		(*ppix0x) += cc0;

		(*ppix0y) *= fc1;
		(*ppix0y) += cc1;

		ppix0x++;
		ppix0y++;
	}
	return true;
}

/*! 
* 	\fn      bool CH3Camera::calibrage(CH3Array< strPos >& A_Pos )
* 	\author  S Jaminion
* 	\brief   Suppression des images en automatique
* 	\param     : Structure contenant pixel/metric/omc/tc
* 	\return  bool
* 	\remarks 
*/ 

bool CH3Camera::calibrage(CH3Array< strPos >& A_Pos )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("calibrage");
	H3DebugInfo(strModule,strFunction,"");
	
	// Calibration filtrage automatique
	bool bkcmodified=false;
	size_t nImaAct=A_Pos.GetSize();
	size_t nImaActReel=nImaAct;

	CString msg;
	if(calib_optim(A_Pos))
	{
		size_t nbPoints;
		//Suppression des images qui ont un ecart type trop important,ie  3*ecart type std
		for ( size_t ll=0;ll<nImaAct;ll++)
		{
			if (A_Pos[ll].mb_isActive)
			{
				H3_ARRAY_FLT32 res(7);
				res=A_Pos[ll].m_CList.Err.GetStatistics();
				if (res[6]>m_nReprojectionSigma*m_fCalibPixelErr)//version SJ
				{
					A_Pos[ll].mb_isActive=false;
					nImaActReel--;
				}
			}
		}

		//Suppression des images dont un point au moins 
		// a un ecart type trop important,ie  3*ecart type std
		bool bRemoveImage=true;
		size_t kk,i;
		while (bRemoveImage)
		{
			bRemoveImage=false;
			for (kk=0;kk<nImaAct;kk++)
			{
				if (A_Pos[kk].mb_isActive)
				{
					nbPoints=A_Pos[kk].m_CList.Err.GetSize();
					for(i=0;i<nbPoints;i++)
					{
						if(fabs(A_Pos[kk].m_CList.Err[i])>m_nReprojectionSigma*m_fCalibPixelErr) //version SJ 
						{
							bRemoveImage=true;
							A_Pos[kk].mb_isActive=false;
							i=nbPoints;
							nImaActReel--;
						}
					}
				}
			}

			if (nImaActReel<5)
			{
				AfxMessageBox("Il reste moins de 5 images : probleme de définition de mire");
				return false;
			}
			if (bRemoveImage)
			{
				calib_optim(A_Pos);
			}

		}
		//verification que l'erreur est 3 fois plus petite pour kc, sinon kc[i]=0
		for (size_t ikc=0;ikc<5;ikc++)
		{
			if (kc_erreur[ikc]>fabs(kc[ikc]))
			{
				bkcmodified=true;
				m_is_dist[ikc]=0;
				kc[ikc]=0;
			}
		}
		if(bkcmodified)
		{
			calib_optim(A_Pos);		
		}
		return true;

	}
	else
	{
		return false;
	}

	return false;
}

/*! 
* 	\fn      bool CH3Camera::calib_iter(CH3Array< strPos >& A_Pos )
* 	\author  V Chalvidan
* 	\brief   
* 	\param     : Structure contenant pixel/metric/omc/tc
* 	\return  bool
* 	\remarks  coeur du calibrage
*/ 

bool CH3Camera::calib_iter(CH3Array< strPos >& A_Pos )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("calib_iter");
	H3DebugInfo(strModule,strFunction,"");

	CString msg;
	double alpha_smooth=0.4;
	double thresh_cond=1e6;
	double change=1;
	size_t i,j,kk,iter=0;
	size_t i0,i1,j0,j1;
	const long MaxIter=15, MaxIter2=20;	//MaxIter=30
	Matrix Mtemp;
	
	size_t nbList=A_Pos.GetSize();
	size_t nbPointskk=0;
	mb_recompute_extrinsic=(nbList<100);	//si plus de N images, on ne recalcule pas les parametres extrinseques

	//fix pb
	if(!m_is_dist[0]) m_is_dist[1] = false;
	if(!m_is_dist[1]) m_is_dist[4] = false;

	if(!mb_is_initialised){
		if(!init_param(A_Pos))
			return false;
	}
	//initialisation des parametres extrinseques
	size_t nbValid=nbList;
	for(i=0;i<nbList;i++)
	{
		if(A_Pos[i].mb_isActive)
			if(!A_Pos[i].m_ExtP.compute_ext_calib((A_Pos[i].m_CList),(*this),20,nullptr,thresh_cond)){
				A_Pos[i].mb_isActive=false;
				nbValid--;
			}
	}
	if(nbValid==0)	//peu etre que si nbValid != nbList il y a un pb
		return false;

	nbList=nbValid;

	//initialization of the global parameter vector
	H3_ARRAY_FLT64 init_param(15+6*nbList),param,param_innov,param_up;
	init_param[0]=fc[0];
	init_param[1]=fc[1];
	init_param[2]=cc[0];
	init_param[3]=cc[1];
	init_param[4]=alpha_c;
	init_param[5]=kc[0];
	init_param[6]=kc[1];
	init_param[7]=kc[2];
	init_param[8]=kc[3];
	init_param[9]=kc[4];

	for(i=10;i<15;i++)
	{
		init_param[i]=0;
	}

	for(i=0,j=15;i<nbList;i++)
	{
		if(A_Pos[i].mb_isActive)
		{
			init_param[j++]=A_Pos[i].m_ExtP.m_omc(0);
			init_param[j++]=A_Pos[i].m_ExtP.m_omc(1);
			init_param[j++]=A_Pos[i].m_ExtP.m_omc(2);

			init_param[j++]=A_Pos[i].m_ExtP.m_Tc(0);
			init_param[j++]=A_Pos[i].m_ExtP.m_Tc(1);
			init_param[j++]=A_Pos[i].m_ExtP.m_Tc(2);
		}
		else j+=6;
	}
	//Main optimization___________________________________________________________________
		
	param=init_param;

	CH3Camera CurrentCam(ny, nx);

	while((change>1e-9)&&(iter<MaxIter))    //________________________________________début
	{
		fc[0]=param[0];	fc[1]=param[1];
		cc[0]=param[2];	cc[1]=param[3];
		alpha_c=param[4];
		kc[0]=param[5];	kc[1]=param[6];	kc[2]=param[7];	kc[3]=param[8];	kc[4]=param[9];
		
		//compute the size of the Jacobian matrice
		//JJ3 est symetrique definie positive si inversible
		Matrix JJ3(15+6*nbList,15+6*nbList),ex3(15+6*nbList,1);
		Matrix JJ2_inv;
		JJ3.Fill(0);
		ex3.Fill(0);

		for(kk=0;kk<nbList;kk++)
		{
			
			if(A_Pos[kk].mb_isActive)
			{

				nbPointskk=(A_Pos[kk].m_CList.MetricData).GetCo();
				i=15+6*kk;
				A_Pos[kk].m_ExtP.m_omc(0)=param[i++];
				A_Pos[kk].m_ExtP.m_omc(1)=param[i++];
				A_Pos[kk].m_ExtP.m_omc(2)=param[i++];
				A_Pos[kk].m_ExtP.m_Tc(0)=param[i++];
				A_Pos[kk].m_ExtP.m_Tc(1)=param[i++];
				A_Pos[kk].m_ExtP.m_Tc(2)=param[i];
				A_Pos[kk].m_ExtP.m_Rc=A_Pos[kk].m_ExtP.m_omc.Rodrigues(Mtemp);

				 H3_ARRAY2D_FLT64	loc_Pix(2,nbPointskk),
						loc_dxdom(2*nbPointskk,3),//2*NbPts;3
						loc_dxdT(2*nbPointskk,3),//2*NbPts;3
						loc_dxdk(2*nbPointskk,5),//2*NbPts;5
						loc_dxdalpha(2*nbPointskk,1),//2*NbPts;1
						loc_dxdf(2*nbPointskk,2),//2*NbPts;2
						loc_dxdc(2*nbPointskk,2);//2*NbPts;2


				Matrix ex_kk;
				Matrix A(2*nbPointskk,10),B(2*nbPointskk,6);
				
				if(!A_Pos[kk].m_ExtP.projectPoints2(
					(A_Pos[kk].m_CList.MetricData),
					(*this),
					loc_Pix,
					&loc_dxdom,
					&loc_dxdT,
					&loc_dxdf,
					&loc_dxdc,
					&loc_dxdk,
					&loc_dxdalpha	))
				{
					#if defined(H3APPTOOLSDECL_H__INCLUDED_)
					H3DebugError(strModule,strFunction,"projection impossible");
					#endif

					return false;
				}

				ex_kk=CL_MATRIX((A_Pos[kk].m_CList.Pixel) - (loc_Pix));
				ex_kk=ex_kk.Trans();//facilite les utilisation future et en particulier resize

				A.SetAt(0,0,loc_dxdf);
				A.SetAt(0,2,loc_dxdc);
				A.SetAt(0,4,loc_dxdalpha);
				A.SetAt(0,5,loc_dxdk);
				Matrix At(A);
				A=A.Trans();

				B.SetAt(0,0,loc_dxdom);
				B.SetAt(0,3,loc_dxdT);
				Matrix Bt(B);
				B=B.Trans();

				JJ3.SetAt(0,0, JJ3.GetAt(0,0,10,10)+A*At );
				JJ3.SetAt(15+6*kk,15+6*kk, B*Bt);

				JJ3.SetAt(0,15+6*kk, A*Bt);
				JJ3.SetAt(15+6*kk,0, B*At);

				ex3.SetAt(0,0, ex3.GetAt(0,0,10,1)+ A*ex_kk.reshape(ex_kk.GetSize(),1) );
				ex3.SetAt(15+6*kk,0, B*ex_kk.reshape(ex_kk.GetSize(),1) );

			}
		}

		size_t nbActiveElements=0,nbActiveIntPar=0;
		H3_ARRAY_UINT8 ActiveElements(15+6*nbList);
		ActiveElements.Fill(0);
		for(kk=0;kk<4;kk++)
		{
			ActiveElements[kk]=1;
			nbActiveElements++;
		}
		if(mb_is_alpha)
		{	//alpha_c
			nbActiveElements++;
			ActiveElements[4]=1;
		}
		for(kk=0;kk<5;kk++)
		{
			if(m_is_dist[kk])
			{
				ActiveElements[kk+5]=1;
				nbActiveElements++;
			}
		}
		for(kk=0;kk<nbList;kk++)
		{
			if(A_Pos[kk].mb_isActive)
			{
				nbActiveElements+=6;
				j=15+6*kk;
				for(i=0;i<6;i++)
				{
					ActiveElements[j++]=1;
				}
			}
		}

		Matrix JJ3b(nbActiveElements,nbActiveElements),ex3b(nbActiveElements,1);
		H3_ARRAY_FLT64 paramb(nbActiveElements);

		for(i0=0,i1=0;i0<15+6*nbList;i0++)
		{ //il y aurait peut etre interet à faire deux boucles
			if(ActiveElements[i0])			//une pour selectionner les lignes l'autres pour les colonnes
			{			
				for(j0=i0,j1=i1;j0<15+6*nbList;j0++)
				{
					if(ActiveElements[j0])
					{
						JJ3b(i1,j1)=JJ3(i0,j0);
						JJ3b(j1,i1)=JJ3(j0,i0);
						j1++;
					}
				}
				ex3b(i1)=ex3(i0);
				paramb[i1]=param[i0];
				i1++;
			}
		}

		//facon 1
		JJ2_inv=JJ3b.Inv();
		if(JJ2_inv.GetSize()==0)
		{
			H3DebugError(strModule,strFunction,"JJ3 non inversible par 1");
			return false;
		}
		else{
			H3DebugInfo(strModule,strFunction,"JJ3 est inversible par 1");
		}

		JJ3=JJ3b;
		ex3=ex3b;
		double alpha_smooth2=1.0-pow(1.0-alpha_smooth,double(iter+1));

		param_innov=(JJ2_inv*ex3)*alpha_smooth2;
		param_up=paramb+param_innov;

		for(i0=0,i1=0;i0<15+6*nbList;i0++)
		{
			if(ActiveElements[i0])
			{
				param.SetAt(i0,param_up[i1]);
				i1++;
			}
		}

		CurrentCam.fc[0] = param[0];
		CurrentCam.fc[1] = param[1];
		CurrentCam.cc[0] = param[2];
		CurrentCam.cc[1] = param[3];
		CurrentCam.alpha_c = param[4];
		CurrentCam.kc[0] = param[5];
		CurrentCam.kc[1] = param[6];
		CurrentCam.kc[2] = param[7];
		CurrentCam.kc[3] = param[8];
		CurrentCam.kc[4] = param[9];

		//Change on the intrinsic parameters
		Matrix Mchange1(4,1),Mchange2(4,1);

		Mchange1(0,0)=CurrentCam.fc[0]-fc[0];
		Mchange1(1,0)=CurrentCam.fc[1]-fc[1];
		Mchange1(2,0)=CurrentCam.cc[0]-cc[0];
		Mchange1(3,0)=CurrentCam.cc[1]-cc[1];

		Mchange2(0,0)=CurrentCam.fc[0];
		Mchange2(1,0)=CurrentCam.fc[1];
		Mchange2(2,0)=CurrentCam.cc[0];
		Mchange2(3,0)=CurrentCam.cc[1];

		change=Mchange1.Norm()/Mchange2.Norm();
		//second step: optional
		bool check_cond=true;
		if(mb_recompute_extrinsic)
		{
			for(kk=0;kk<nbList;kk++)
			{
				if(A_Pos[kk].mb_isActive)
				{
					double Cond;
					CExtrinsic_param CurrentEP;
					CurrentEP.compute_ext_calib(A_Pos[kk].m_CList, CurrentCam, MaxIter2, &Cond,thresh_cond);
					if(check_cond)
					{
						if(Cond>thresh_cond)
						{
							A_Pos[kk].mb_isActive=false;
							CString msg;
							msg.Format("La vue %d presente un défaut et ne sera pas considérée dans la suite du calcul",kk);
#if defined(H3APPTOOLSDECL_H__INCLUDED_)
							H3DebugWarning(strModule,strFunction,msg);
#endif
						}
					}
					param.SetAt(15+6*kk,CurrentEP.m_omc.GetAt(0,0,3,1));
					param.SetAt(18+6*kk,CurrentEP.m_Tc.GetAt(0,0,3,1));
				}
			}
		}
		iter++;
	}
//______________________________________________________________Fin Optim	
	//Computation of the error of estimation

	//extracion of the final intrinsic and extrinsic parameters
	extract_parameters(param,nullptr,(*this),A_Pos);

	//recompute the error
	CL_ARRAY2D std;
	Matrix JJ3(15+6*nbList,15+6*nbList);
	H3_ARRAY_FLT64 ActiveElements(15+6*nbList);

	comp_error_calib((*this),A_Pos,std);
	double sigma_x=std(6,0),sigma_y=std(6,1),sigma_xy=std(6,2);
	m_fCalibPixelErr=sigma_xy;

	JJ3.Fill(0);
	ActiveElements.Fill(0);
	long nbActiveElements=0;
	for(kk=0;kk<4;kk++)
	{
		ActiveElements[kk]=1;
		nbActiveElements++;
	}
	if(mb_is_alpha)
	{	//alpha_c
		nbActiveElements++;
		ActiveElements[4]=1;
	}
	for(kk=0;kk<5;kk++)
	{
		if(m_is_dist[kk])
		{
			ActiveElements[kk+5]=1;
			nbActiveElements++;
		}
	}
	for(kk=0;kk<nbList;kk++)
	{
		if(A_Pos[kk].mb_isActive)
		{
			nbActiveElements += 6;
			j=15+6*kk;
			for(i=0;i<6;i++)
			{
				ActiveElements[j++]=1;
			}
			nbPointskk=(A_Pos[kk].m_CList.MetricData).GetCo();
			i=15+6*kk;
			A_Pos[kk].m_ExtP.m_omc(0)=param[i++];
			A_Pos[kk].m_ExtP.m_omc(1)=param[i++];
			A_Pos[kk].m_ExtP.m_omc(2)=param[i++];
			A_Pos[kk].m_ExtP.m_Tc(0)=param[i++];
			A_Pos[kk].m_ExtP.m_Tc(1)=param[i++];
			A_Pos[kk].m_ExtP.m_Tc(2)=param[i];
			A_Pos[kk].m_ExtP.m_Rc=A_Pos[kk].m_ExtP.m_omc.Rodrigues(Mtemp);

			H3_ARRAY2D_FLT64	loc_Pix(2,nbPointskk),
					loc_dxdom(2*nbPointskk,3),
					loc_dxdT(2*nbPointskk,3),
					loc_dxdk(2*nbPointskk,5),
					loc_dxdalpha(2*nbPointskk,1),
					loc_dxdf(2*nbPointskk,2),
					loc_dxdc(2*nbPointskk,2);

			Matrix A(2*nbPointskk,10),B(2*nbPointskk,6),ABt(10,6);
			
			if(!A_Pos[kk].m_ExtP.projectPoints2(
				(A_Pos[kk].m_CList.MetricData),
				(*this),
				loc_Pix,
				&loc_dxdom,
				&loc_dxdT,
				&loc_dxdf,
				&loc_dxdc,
				&loc_dxdk,
				&loc_dxdalpha	))
			{
				#if defined(H3APPTOOLSDECL_H__INCLUDED_)
				H3DebugError(strModule,strFunction,"projection impossible _2");
				#endif

				return false;
			}
			A.SetAt(0,0,loc_dxdf);
			A.SetAt(0,2,loc_dxdc);
			A.SetAt(0,4,loc_dxdalpha);
			A.SetAt(0,5,loc_dxdk);
			Matrix At(A);
			A=A.Trans();

			B.SetAt(0,0,loc_dxdom);
			B.SetAt(0,3,loc_dxdT);
			Matrix Bt(B);
			B=B.Trans();

			ABt=A*Bt;

			JJ3.SetAt(0,0, JJ3.GetAt(0,0,10,10)+A*At );
			JJ3.SetAt(15+6*kk,15+6*kk, B*Bt);

			JJ3.SetAt(0,15+6*kk, ABt);
			JJ3.SetAt(15+6*kk,0, ABt.Trans());
		}
	}

	Matrix JJ3b(nbActiveElements,nbActiveElements),JJ2_inv;
	H3_ARRAY_FLT64 paramb(nbActiveElements);
	for(i0=0,i1=0;i0<15+6*nbList;i0++)
	{	//il y aurait peut etre interet à faire deux boucles
		if(ActiveElements[i0])
		{	//une pour selectionner les lignes l'autres pour les colonnes
			for(j0=i0,j1=i1;j0<15+6*nbList;j0++)
			{
				if(ActiveElements[j0])
				{
					JJ3b(i1,j1)=JJ3(i0,j0);
					JJ3b(j1,i1)=JJ3(j0,i0);
					j1++;
				}
			}
			paramb[i1]=param[i0];
			i1++;
		}
	}
	
	try
	{
		JJ2_inv=JJ3b.inv_chols1();
	}
	catch(...)
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"JJ3 non inversible _2");
		#endif

		return false;
	}

	H3_ARRAY_FLT64 solution_error(15+6*nbList);
	solution_error.Fill(0);
	for(i0=0,i1=0;i0<15+6*nbList;i0++)
	{	//il y aurait peut etre interet à faire deux boucles
		if(ActiveElements[i0])
		{	//une pour selectionner les lignes l'autres pour les colonnes
			solution_error[i0]=3*sqrt(JJ2_inv(i1,i1))*sigma_xy;
			i1++;
		}
	}
	extract_parameters(param,&solution_error,(*this),A_Pos);

	pix_erreur[0]=std(6,0);
	pix_erreur[1]=std(6,1);

	CString PixelERR;
	PixelERR.Format("Erreur entre les positions pixel mesurées"\
		"et celles calculées\nerrX=%f\terrY=%f\n"\
		"erreur minx=%f\tmaxx=%f\n"\
		"erreur miny=%f\tmaxy=%f\n",
		std(6,0),std(6,1),std(1,0),std(2,0),std(1,1),std(2,1));
	#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugInfo(strModule,strFunction,PixelERR);
	#else
		AfxMessageBox(PixelERR);
	#endif
	return true;
}

/***************************************************************************/
//bool CCamera::init_param(CH3Array< strPos >& A_Pos)
//D'apres
//init_intrinsic_param
/***************************************************************************/

/*! 
* 	\fn      bool CH3Camera::init_param(CH3Array< strPos >& A_Pos)
* 	\author  V Chalvidan
* 	\brief   
* 	\param     : Structure contenant pixel/metric/omc/tc
* 	\return  bool
* 	\remarks 
*/ 

bool CH3Camera::init_param(CH3Array< strPos >& A_Pos)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("init_param");
	H3DebugInfo(strModule,strFunction,"");

	size_t szA=A_Pos.GetSize();
	size_t kk,jj,ll;
	long nbActive=0;

	for(kk=0;kk<szA;kk++)
	{
		if(A_Pos[kk].mb_isActive) nbActive++;
	}

	Matrix A(2*nbActive,2);
	Matrix B(2*nbActive,1);
	CRotation H,hNorm,iNorm;
	double temp[9]={1,0,-cc[0],0,1,-cc[1],0,0,1};

	CRotation Sub_cc(temp);

	double a1,a2,a3,a4,b1,b2,b3,b4,c1,c2,c3,c4,n1,n2,n3,n4;
	double tmp11,tmp12,tmp13,tmp21,tmp22,tmp23;

	for(kk=0,jj=0;jj<szA;jj++)
	{
		if(A_Pos[jj].mb_isActive)
		{
			bool bPixelNorm=false;
			if(!A_Pos[jj].m_ExtP.compute_extrinsic_init(A_Pos[jj].m_CList,(*this),bPixelNorm))
				return false;
			if (!A_Pos[jj].m_CList.m_bHinitialised) 
			{
				CString msg;
				msg.Format("Probleme d'initialisation : calibrage arrete");

				#if defined(H3APPTOOLSDECL_H__INCLUDED_)
					H3DebugError(strModule,strFunction,msg);
				#else
					#if H3_CHECKALL_MODE
						AfxMessageBox(strModule + msg);
					#endif
				#endif
			}

			CRotation Hkk(Sub_cc*(A_Pos[jj].m_CList.H));

			if(Hkk.GetSize()==0 )
			{
				return false;
			}
			if(!_finite(Hkk(0)))
			{
				return false;
			}

			//extract vanishing point
			n1=sqrt(Hkk(0)*Hkk(0)+Hkk(3)*Hkk(3)+Hkk(6)*Hkk(6));
			n2=sqrt(Hkk(1)*Hkk(1)+Hkk(4)*Hkk(4)+Hkk(7)*Hkk(7));

			tmp11=(Hkk(0)+Hkk(1))/2;
			tmp12=(Hkk(3)+Hkk(4))/2;
			tmp13=(Hkk(6)+Hkk(7))/2;
			tmp21=(Hkk(0)-Hkk(1))/2;
			tmp22=(Hkk(3)-Hkk(4))/2;
			tmp23=(Hkk(6)-Hkk(7))/2;

			n3=sqrt(tmp11*tmp11+tmp12*tmp12+tmp13*tmp13);
			n4=sqrt(tmp21*tmp21+tmp22*tmp22+tmp23*tmp23);

			a1=Hkk(0)/n1;	b1=Hkk(3)/n1;	c1=Hkk(6)/n1;
			a2=Hkk(1)/n2;	b2=Hkk(4)/n2;	c2=Hkk(7)/n2;
			a3=tmp11/n3;	b3=tmp12/n3;	c3=tmp13/n3;
			a4=tmp21/n4;	b4=tmp22/n4;	c4=tmp23/n4;

			ll=4*kk;
			A(ll++)=a1*a2;
			A(ll++)=b1*b2;
			A(ll++)=a3*a4;
			A(ll  )=b3*b4;

			ll=2*kk;
			B(ll++)=-c1*c2;
			B(ll  )=-c3*c4;

			kk++;
		}
	}
	try
	{
		Matrix tmp=Mat_MeanSquare(A,B);
		fc[0]=1/sqrt(fabs(tmp(0)));
		fc[1]=1/sqrt(fabs(tmp(1)));
	}
	catch(...)
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugInfo(strModule,strFunction,"Erreur");
		#else
			AfxMessageBox("Camera::InitParam Erreur");
		#endif
	}

	mb_is_initialised=true;
	return true;
}


/*! 
* 	\fn      bool CH3Camera::calib_optim(CH3Array< strPos >& A_Pos )
* 	\author  V Chalvidan
* 	\brief   
* 	\param     : 
* 	\return  bool
* 	\remarks appel fonction calib_iter
*/ 

bool CH3Camera::calib_optim(CH3Array< strPos >& A_Pos )
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("calib_optim");
	H3DebugInfo(strModule,strFunction,"");

	mb_recompute_extrinsic=(A_Pos.GetSize()<100);

	try
	{
		if(!calib_iter(A_Pos))
		{
			return false;
		}
	}
	catch(...)
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"Une erreur inattendue s'est produite");
			H3DebugError(strModule,strFunction,"la camera n'est pas initialisée");
		#else
			AfxMessageBox("Une erreur inattendue s'est produite");
			AfxMessageBox("CCamera::calib_optim\n la camera n'est pas initialisée");
		#endif

		return false;
	}
	return true;
}

// Cette fonction retourne l'unite de mesure (mm)
CString CH3Camera::GetUnit()
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	return m_strUnit;

}

BOOL CH3Camera::LoadCalib(CString strFileName,int Indice)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("LoadCalib");
	CString msg("in LoadCalib");

	CString strEntry;
	strEntry.Format("%s%d",DEFAULT_CALIB_NAME,Indice);

	nx=H3GetPrivProfileInt(strEntry,_T("nx"),strFileName);
	ny=H3GetPrivProfileInt(strEntry,_T("ny"),strFileName);

	fc[0]=H3GetPrivProfileFloat(strEntry,_T("fc0"),strFileName);
	fc[1]=H3GetPrivProfileFloat(strEntry,_T("fc1"),strFileName);

	cc[0]=H3GetPrivProfileFloat(strEntry,_T("cc0"),strFileName);
	cc[1]=H3GetPrivProfileFloat(strEntry,_T("cc1"),strFileName);

	kc[0]=H3GetPrivProfileFloat(strEntry,_T("kc0"),strFileName);
	kc[1]=H3GetPrivProfileFloat(strEntry,_T("kc1"),strFileName);
	kc[2]=H3GetPrivProfileFloat(strEntry,_T("kc2"),strFileName);
	kc[3]=H3GetPrivProfileFloat(strEntry,_T("kc3"),strFileName);
	kc[4]=H3GetPrivProfileFloat(strEntry,_T("kc4"),strFileName);

	alpha_c=H3GetPrivProfileFloat(strEntry,_T("alpha_c"),strFileName);
	m_fCalibPixelErr=H3GetPrivProfileFloat(strEntry,_T("fCalibPixelErr"),strFileName);

	m_is_dist[0]=(1==H3GetPrivProfileInt(strEntry,_T("iskc1"),strFileName))?true:false;
	m_is_dist[1]=(1==H3GetPrivProfileInt(strEntry,_T("iskc2"),strFileName))?true:false;
	m_is_dist[2]=(1==H3GetPrivProfileInt(strEntry,_T("iskc3"),strFileName))?true:false;
	m_is_dist[3]=(1==H3GetPrivProfileInt(strEntry,_T("iskc4"),strFileName))?true:false;
	m_is_dist[4]=(1==H3GetPrivProfileInt(strEntry,_T("iskc5"),strFileName))?true:false;
	mb_is_alpha=(1==H3GetPrivProfileInt(strEntry,_T("is_alpha"),strFileName))?true:false;
	mb_recompute_extrinsic=(1==H3GetPrivProfileInt(strEntry,_T("is_rc"),strFileName))?true:false;
	
	mb_is_initialised = true;
	//if(fc[0]==DEFAULT_CAM_FC0)
	//	if(fc[1]==DEFAULT_CAM_FC1)
	//		if(cc[0]==DEFAULT_CAM_CC0)
	//			if(cc[1]==DEFAULT_CAM_CC1)
	//				if(kc[0]==DEFAULT_CAM_KC0)
	//					if(kc[1]==DEFAULT_CAM_KC1)
	//						if(kc[2]==DEFAULT_CAM_KC2)
	//							if(kc[3]==DEFAULT_CAM_KC3) 
	//								if(kc[4]==DEFAULT_CAM_KC4) 
	//									if(alpha_c==DEFAULT_ALPHAC) 
	//									{
	//										// Erreur Code 3
	//										mb_is_initialised = false;
	//									}

    m_nReprojectionSigma = 4;// H3GetPrivProfileInt(strEntry, _T("ReprojectionSigma"), 4, strFileName); "ReprojectionSigma" does not seem not be part of the intrinsic cam parameters anymore.

	return true;
}

BOOL CH3Camera::SaveCalib(CString strFileName,int Indice)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("SaveCalib");
	CString msg("in SaveCalib");

	CString strEntry;
	strEntry.Format("%s%d",DEFAULT_CALIB_NAME,Indice);

	bool b=true;

	b&=H3WritePrivProfileInt(strEntry,_T("nx"),(int)nx,strFileName);
	b&=H3WritePrivProfileInt(strEntry,_T("ny"),(int)ny,strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("fc0"),fc[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("fc1"),fc[1],strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("cc0"),cc[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("cc1"),cc[1],strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("kc0"),kc[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("kc1"),kc[1],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("kc2"),kc[2],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("kc3"),kc[3],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("kc4"),kc[4],strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("alpha_c"),alpha_c,strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("fCalibPixelErr"),m_fCalibPixelErr,strFileName);

	b&=H3WritePrivProfileInt(strEntry,_T("iskc1"),m_is_dist[0],strFileName);
	b&=H3WritePrivProfileInt(strEntry,_T("iskc2"),m_is_dist[1],strFileName);
	b&=H3WritePrivProfileInt(strEntry,_T("iskc3"),m_is_dist[2],strFileName);
	b&=H3WritePrivProfileInt(strEntry,_T("iskc4"),m_is_dist[3],strFileName);
	b&=H3WritePrivProfileInt(strEntry,_T("iskc5"),m_is_dist[4],strFileName);
	b&=H3WritePrivProfileInt(strEntry,_T("is_alpha"),mb_is_alpha,strFileName);
	b&=H3WritePrivProfileInt(strEntry,_T("is_rc"),mb_recompute_extrinsic,strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("fc0_err"),fc_erreur[1],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("fc1_err"),fc_erreur[1],strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("cc0_err"),cc_erreur[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("cc1_err"),cc_erreur[1],strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("kc0_err"),kc_erreur[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("kc1_err"),kc_erreur[1],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("kc2_err"),kc_erreur[2],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("kc3_err"),kc_erreur[3],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("kc4_err"),kc_erreur[4],strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("alpha_c_err"),alpha_c_erreur,strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("pix_x_err"),pix_erreur[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("pix_y_err"),pix_erreur[1],strFileName);

	return b;
}

/*! 
* 	\fn      void CH3Camera::WinAffiche() const
* 	\author  V Chalvidan
* 	\brief   Affiche les parametres intrinseques de la camera
* 	\return  void
* 	\remarks 
*/ 

void CH3Camera::WinAffiche() const
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("WinAffiche");

	CString s;
	s.Format("Paramètres Caméra\n"\
				"Focale : %f  %f\n"\
				"Centre Optique : %f  %f\n"\
				"facteurs correctifs : %f  %f  %f  %f  %f\n"\
				"Angle colonne/vertical : %f\n\n"\
				"Err Focale : %f  %f\n"\
				"Err Centre Optique : %f  %f\n"\
				"Err facteurs correctifs : %f %f %f %f %f\n"\
				"Err Angle colonne/vertical : %f \n"\
				"Err Pixel : %f  %f",
				fc[0],fc[1],cc[0],cc[1],
				kc[0],kc[1],kc[2],kc[3],kc[4],
				alpha_c,
				fc_erreur[0],fc_erreur[1],cc_erreur[0],cc_erreur[1],
				kc_erreur[0],kc_erreur[1],kc_erreur[2],kc_erreur[3],kc_erreur[4],
				alpha_c_erreur,pix_erreur[0],pix_erreur[1]);
	H3DisplayInfo(s);
	H3DebugInfo(strModule, strFunction,s);
}

bool CH3Camera::MFCalib(H3_ARRAY_PT2DFLT32 *Pt,unsigned int nbImage,H3_ARRAY2D_FLT64 Metric)
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	CString strFunction("MFCalib");
	H3DebugInfo(strModule,strFunction,"");
	CH3Array< strPos > A_pos(nbImage);

	//Recherche des intersection de chaque image
	for (size_t i=0;i<nbImage;i++)
	{	
		H3_ARRAY2D_FLT32 Temp(2,Pt[i].GetSize());
		for (UINT u=0;u<Pt[i].GetSize();u++)
		{
			Temp(0,u)=Pt[i][u].x;
			Temp(1,u)=Pt[i][u].y;
		}

		A_pos[i].m_CList = CCorrespList2(Temp,Metric);
		if(A_pos[i].m_CList.H.GetSize()==0)
		{
			CString msg;
			msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 1");
			H3DebugWarning(strModule,strFunction,msg);
			A_pos[i].mb_isActive=false;

		}
		else
		{
			if(!_finite(A_pos[i].m_CList.H(0)))
			{
				CString msg;
				msg.Format("la relation points reperés / grille n'a pas pu etre realisée _ 2");
				H3DebugWarning(strModule,strFunction,msg);
				A_pos[i].mb_isActive=false;

			}
			else
				A_pos[i].mb_isActive=true;
		}
	}

	//Veritable  calibrage
	calibrage(A_pos);
	WinAffiche();

	return true;
}

bool CH3Camera::apply_distortion(const H3_ARRAY2D_FLT64& Pixelin, H3_ARRAY2D_FLT64& Pixelout) const	//plan retinien 
{
	AFX_MANAGE_STATE(AfxGetStaticModuleState());
	size_t nb_pixel=Pixelin.GetCo();
	if( (Pixelout.GetCo()!=nb_pixel) || (Pixelin.GetLi()!=2) || (Pixelout.GetLi()!=2) ) return false;

	double *ppix_inx,*ppix_iny,*ppix_outx,*ppix_outy;
	size_t i;
	double x,y,r2,k_radial,dx,dy,x2,y2,xy;
	double k1=kc[0],k2=kc[1],k3=kc[4],p1=kc[2],p2=kc[3];
		
	ppix_inx=(double*)Pixelin.GetData();
	ppix_iny=ppix_inx+nb_pixel;
	ppix_outx=(double*)Pixelout.GetData();
	ppix_outy=ppix_outx+nb_pixel;

	i=nb_pixel;

	while(i--){
		(*ppix_outx)=x=(*ppix_inx);
		(*ppix_outy)=y=(*ppix_iny);
		x2=x*x;
		y2=y*y;
		xy=x*y;
		r2=x2+y2;
		k_radial=((k3*r2+k2)*r2+k1)*r2+1;
		dx=2*p1*xy+p2*(r2+2*x2);
		dy=p1*(r2+2*y2)+2*p2*xy;
		(*ppix_outx)*=k_radial;
		(*ppix_outx)+=dx;

		(*ppix_outy)*=k_radial;
		(*ppix_outy)+=dy;

		ppix_inx++;
		ppix_iny++;

		ppix_outx++;
		ppix_outy++;
	}	

	return true;

}

stParamIntrin CH3Camera::GetParamIntrin()const
{
	stParamIntrin ParamIntrin;

	ParamIntrin.afc.ReAlloc(2);
	ParamIntrin.acc.ReAlloc(2);
	ParamIntrin.akc.ReAlloc(5);
	ParamIntrin.aalphac.ReAlloc(1);
	//Paramètres intrinsèques //etrange de definir 2* les memes variables
	ParamIntrin.afc[0]=fc[0];
	ParamIntrin.afc[1]=fc[1];
	ParamIntrin.acc[0]=cc[0];
	ParamIntrin.acc[1]=cc[1];
	ParamIntrin.akc[0]=kc[0];
	ParamIntrin.akc[1]=kc[1];
	ParamIntrin.akc[2]=kc[2];
	ParamIntrin.akc[3]=kc[3];
	ParamIntrin.akc[4]=kc[4];
	ParamIntrin.akc[0]=kc[0];
	ParamIntrin.akc[1]=kc[1];
	ParamIntrin.aalphac[0]=alpha_c;

	return ParamIntrin;
}

stParamIntrinErr CH3Camera::GetParamIntrinErr()const
{
	stParamIntrinErr ParamIntrinErr;

	ParamIntrinErr.afc_err.ReAlloc(2);
	ParamIntrinErr.acc_err.ReAlloc(2);
	ParamIntrinErr.akc_err.ReAlloc(5);
	ParamIntrinErr.aalphac_err.ReAlloc(1);
	ParamIntrinErr.apix_err.ReAlloc(2);

	ParamIntrinErr.afc_err[0] = fc_erreur[0];
	ParamIntrinErr.afc_err[1] = fc_erreur[1];
	ParamIntrinErr.acc_err[0] = cc_erreur[0];
	ParamIntrinErr.acc_err[1] = cc_erreur[1];
	ParamIntrinErr.akc_err[0] = kc_erreur[0];
	ParamIntrinErr.akc_err[1] = kc_erreur[1];
	ParamIntrinErr.akc_err[2] = kc_erreur[2];
	ParamIntrinErr.akc_err[3] = kc_erreur[3];
	ParamIntrinErr.akc_err[4] = kc_erreur[4];
	ParamIntrinErr.aalphac_err[0] = alpha_c_erreur;
	ParamIntrinErr.apix_err[0] = pix_erreur[0];
	ParamIntrinErr.apix_err[1] = pix_erreur[1];
	return ParamIntrinErr;
}

bool CH3Camera::Rect_Index(const Matrix& R, H3_ARRAY2D_FLT64 & outRectIndex , H3_ARRAY2D_FLT64 & outIndex , H3_ARRAY2D_FLT64 & outFactor)const
{
	size_t sz= nx*ny;

	if(outRectIndex.GetLi()!=2 || outIndex.GetCo()!=sz)
		return false;
	if(outIndex.GetLi()!=4 || outIndex.GetCo()!=sz)
		return false;
	if(outFactor.GetLi()!=4 || outFactor.GetCo()!=sz)
		return false;

	stParamIntrin P_I;
	P_I=GetParamIntrin();
	
	Matrix KK(3,3);	//Camera Matrix
	double KK0,KK1,KK2,KK4,KK5;
	KK.Fill(0);
	KK0=KK[0]=P_I.afc[0];
	KK1=KK[1]=P_I.afc[0]*P_I.aalphac[0];
	KK2=KK[2]=P_I.acc[0];
	KK4=KK[4]=P_I.afc[1];
	KK5=KK[5]=P_I.acc[1];
	KK[8]=1;

	Matrix iKK=KK.Inv();
	if(iKK.GetSize()!=9)
		return false;

	double iKK0=iKK[0],iKK1=iKK[1],iKK2=iKK[2],iKK3=iKK[3],iKK4=iKK[4],iKK5=iKK[5],iKK6=iKK[6],iKK7=iKK[7],iKK8=iKK[8];
	double valX0=iKK2,valY0=iKK5,valZ0=iKK8,valX,valY,valZ;

	size_t i,j;
	H3_ARRAY2D_FLT64 Ray(3,sz);
	double *pRayX=Ray.GetData(),*pRayY=pRayX+sz,*pRayZ=pRayY+sz;
	for(i=0;i<ny;i++)
	{
		valX=valX0;
		valY=valY0;
		valZ=valZ0;
		for(j=0;j<nx;j++)
		{
			(*pRayX++)=valX;
			(*pRayY++)=valY;
			(*pRayZ++)=valZ;

			valX+=iKK0;
			valY+=iKK3;
			valZ+=iKK6;
		}
		valX0+=iKK1;
		valY0+=iKK4;
		valZ0+=iKK7;
	}

	H3_ARRAY2D_FLT64 Ray2=R.Trans()*Ray;

	H3_ARRAY2D_FLT64 px(2,sz);
	double *px_x=px.GetData(),*px_y=px_x+sz;
	pRayX=Ray2.GetData();
	pRayY=pRayX+sz;
	pRayZ=pRayY+sz;

	for(i=0;i<sz;i++)
	{
			(*px_x++)=(*pRayX++)/(*pRayZ);
			(*px_y++)=(*pRayY++)/(*pRayZ);
			pRayZ++;
	}

	if(!apply_distortion(px,outRectIndex))
		return false;

	px_x=outRectIndex.GetData();
	px_y=px_x+sz;
	for(i=0;i<sz;i++)
	{
			(*px_x)=KK0*(*px_x)+KK1*(*px_y)+KK2;
			(*px_y)=KK4*(*px_y)+KK5;
			px_x++;
			px_y++;
	}

	H3_ARRAY2D_FLT64 px_0(2,sz);
	H3_ARRAY2D_FLT64 alpha(2,sz);
	px_x=outRectIndex.GetData();
	px_y=px_0.GetData();
	double *palpha_x=alpha.GetData(),*palpha_y;
	for(i=0;i<2*sz;i++)
	{
		(*px_y)=floor(*px_x);
		(*palpha_x)=(*px_x)-(*px_y);

		palpha_x++;
		px_x++;
		px_y++;
	}

	H3_ARRAY2D_FLT64 A(4,sz);
	H3_ARRAY2D_FLT64 Ind(4,sz);
	double *pA1=A.GetData(),*pA2=pA1+sz,*pA3=pA2+sz,*pA4=pA3+sz;
	double *pInd1=Ind.GetData(),*pInd2=pInd1+sz,*pInd3=pInd2+sz,*pInd4=pInd3+sz;
	palpha_x=alpha.GetData();
	palpha_y=palpha_x+sz;
	px_x=px_0.GetData();
	px_y=px_x+sz;

	for(i=0;i<sz;i++)
	{
		(*pA3)=(*pA1)=1-(*palpha_x);
		(*pA2)=1-(*palpha_y);
		(*pA1)*=(*pA2);
		(*pA2)*=(*palpha_x);
		(*pA3)*=(*palpha_y);
		(*pA4)=(*palpha_x)*(*palpha_y);

		(*pInd1)=(*px_y)*nx+(*px_x);	//nb: les indices matlab ne sont pas les memes
		(*pInd2)=(*pInd1)+1;
		(*pInd3)=(*pInd1)+nx;
		(*pInd4)=(*pInd3)+1;
	}
	return true;
}


//calcul des positions pixel dans une image de départ pour quelques positions pixels dans l'image rectifiée 
//inRectIndex(2, nbpix): pixel de l'image rectifié dont on veut connaitre les pixels d'origine
//R: matrice (rotation) pour passer du repere camera au repere camera rectifiée
//rmq: les params fc et cc de la camera rectifiée sont ceux de la camera d'origine
bool CH3Camera::Rect_Index(const Matrix& R, const H3_ARRAY2D_FLT64 & inRectIndex, H3_ARRAY2D_FLT64 & outRectIndex)const
{
	size_t sz= nx*ny;
	size_t nbPos=inRectIndex.GetCo();

	if(inRectIndex.GetLi()!=2 )
		return false;
	if(outRectIndex.GetLi()!=2 || outRectIndex.GetCo()!=nbPos)
		return false;

	////passage position pixel >> plan retinien: pos_ret=iKK*[px_x;px_y;1] pour la camera RECTIFIEE
	////ici la cam rectifiée a meme focale et centre que la camera non rectifié
	////pour changer ca , il faut un param supplementaire
	stParamIntrin P_I;
	P_I=GetParamIntrin();
	
	Matrix KK(3,3);//Matrix caracteristique de la Camera rectifiée
	double KK0,KK1,KK2,KK4,KK5;
	KK.Fill(0);
	KK0=KK[0]=P_I.afc[0];
	KK1=KK[1]=P_I.afc[0]*P_I.aalphac[0];
	KK2=KK[2]=P_I.acc[0];
	KK4=KK[4]=P_I.afc[1];
	KK5=KK[5]=P_I.acc[1];
	KK[8]=1;

	Matrix iKK=KK.Inv();
	if(iKK.GetSize()!=9)
		return false;

	double iKK0=iKK[0],iKK1=iKK[1],iKK2=iKK[2],iKK3=iKK[3],iKK4=iKK[4],iKK5=iKK[5],iKK6=iKK[6],iKK7=iKK[7],iKK8=iKK[8];
	double valX0=iKK2,valY0=iKK5,valZ0=iKK8;

	size_t i;
	H3_ARRAY2D_FLT64 Ray(3,nbPos);//les points dans le plan retinien de la camera rectifié
	double *pRayX=Ray.GetData(),*pRayY=pRayX+nbPos,*pRayZ=pRayY+nbPos;
	double *px_x=inRectIndex.GetData(),*px_y=px_x+nbPos;

	for(i=0;i<nbPos;i++){
		(*pRayX++)=iKK0*(*px_x)+iKK1*(*px_y)+iKK2;
		(*pRayY++)=iKK3*(*px_x)+iKK4*(*px_y)+iKK5;
		(*pRayZ++)=iKK6*(*px_x)+iKK7*(*px_y)+iKK8;

		px_x++;
		px_y++;
	}

	H3_ARRAY2D_FLT64 Ray2=R.Trans()*Ray;//les points exprimés dans le repere de la camera

	H3_ARRAY2D_FLT64 px(2,nbPos);
	px_x=px.GetData();
	px_y=px_x+nbPos;

	pRayX=Ray2.GetData();
	pRayY=pRayX+nbPos;
	pRayZ=pRayY+nbPos;

	for(i=0;i<nbPos;i++){//les points exprimés dans le repere de la camera sont projettés sur le plan retinien
			(*px_x++)=(*pRayX++)/(*pRayZ);
			(*px_y++)=(*pRayY++)/(*pRayZ);
			pRayZ++;
	}

	if(!apply_distortion(px,outRectIndex))//on applique la distortion
		return false;

	px_x=outRectIndex.GetData();
	px_y=px_x+nbPos;
	for(i=0;i<nbPos;i++){//et on obtient les positions pixels
			(*px_x)=KK0*(*px_x)+KK1*(*px_y)+KK2;
			(*px_y)=KK4*(*px_y)+KK5;
			px_x++;
			px_y++;
	}

	return true;
}

//calcul des positions pixel dans une image rectifié pour quelques positions pixels dans l'image d'origine
//projection des pixel sur le plan retinien
//si la camera a des defauts, correction des points dans le plan retinien
//modif du plan retinien (rotation) pour qu'il corresponde à la camera rectifié
//si celle ci devait avoir des defauts, il faudrait les appliquer(pour le moment, NON)
//projection sur le nouveau plan pixel

//R rotation plan retinien >> plan retinien rectifié
//KKrect Matrice caracteristique de la camera rectifiée [fx 0 cx;0 fy cy;0,0,1]
bool CH3Camera::iRect_Index(const Matrix& R,const Matrix &KKrect, const H3_ARRAY2D_FLT64 & inRectIndex, H3_ARRAY2D_FLT64 & outRectIndex)const
{
	size_t sz= nx*ny;
	size_t nbPos=inRectIndex.GetCo();

	if(inRectIndex.GetLi()!=2 )
		return false;
	if(outRectIndex.GetLi()!=2 || outRectIndex.GetCo()!=nbPos)
		return false;
	if(KKrect.GetLi()!=3 || KKrect.GetCo()!=3)
		return false;

	////passage position pixel >> plan retinien: pos_ret=iKK*[px_x;px_y;1]
	
	Matrix KK(3,3);//Camera Matrix
	double KK0,KK1,KK2,KK4,KK5;
	KK.Fill(0);
	KK0=KK[0]=fc[0];
	KK1=KK[1]=fc[0]*alpha_c;
	KK2=KK[2]=cc[0];
	KK4=KK[4]=fc[1];
	KK5=KK[5]=cc[1];
	KK[8]=1;

	Matrix iKK=KK.Inv();
	if(iKK.GetSize()!=9)
		return false;

	double iKK0=iKK[0],iKK1=iKK[1],iKK2=iKK[2],iKK3=iKK[3],iKK4=iKK[4],iKK5=iKK[5],iKK6=iKK[6],iKK7=iKK[7],iKK8=iKK[8];

	size_t i;
	H3_ARRAY2D_FLT64 Ray(2,nbPos);
	double *pRayX=Ray.GetData(),*pRayY=pRayX+nbPos;
	double *px_x=inRectIndex.GetData(),*px_y=px_x+nbPos;

	for(i=0;i<nbPos;i++){
		(*(pRayX++))=iKK0*(*px_x)+iKK1*(*px_y)+iKK2;
		(*(pRayY++))=iKK3*(*px_x)+iKK4*(*px_y)+iKK5;
		px_x++;
		px_y++;
	}

	//on corrige les distortions camera
	H3_ARRAY2D_FLT64 CorrectedRays(2,nbPos);
	if(!compensate_distortion_oulu(Ray,CorrectedRays))
		return false;

	//on passe dans le repere du plan retinien rectifié defini par R (passage repe retinien >> rep retinien rectifié)
	//on projette tout de suite sur le plan ret rect en divisant par Z
	//on utilise outRectIndex parce que c'est disponible
	double R0=R[0],R1=R[1],R2=R[2],R3=R[3],R4=R[4],R5=R[5],R6=R[6],R7=R[7],R8=R[8];
	pRayX=CorrectedRays.GetData();
	pRayY=pRayX+nbPos;
	px_x=outRectIndex.GetData();
	px_y=px_x+nbPos;
	double z;

	for(i=0;i<nbPos;i++){
		z=			(R6*(*pRayX)+R7*(*pRayY)+R8);
		(*(px_x++))=(R0*(*pRayX)+R1*(*pRayY)+R2)/z;
		(*(px_y++))=(R3*(*pRayX)+R4*(*pRayY)+R5)/z;
		
		pRayX++;
		pRayY++;
	}

	//si la camera rectifié devait avoir des aberations c'est ici qu'il faudrait les prendres en compte
	//apply_distortion

	//projection sur le plan pixel de la camera rectifié
	KK0=KKrect[0];
	KK2=KKrect[2];
	KK4=KKrect[4];
	KK5=KKrect[5];

	px_x=outRectIndex.GetData();
	px_y=px_x+nbPos;
	for(i=0;i<nbPos;i++){
		(*px_x)=KK0*(*px_x)+KK2;
		(*px_y)=KK4*(*px_y)+KK5;
		px_x++;
		px_y++;
	}

//NB: les deux dernieres operations peuvent se faire dans une seule boucle, mais pour tester c'est plus facile comme ca

	return true;
}