// Extrinsic_param.cpp: implementation of the CExtrinsic_param class.
//
//////////////////////////////////////////////////////////////////////

#include "stdafx.h"
#include "H3DSLRCalibration.h"
#include "Extrinsic_param.h"
#include "H3AppToolsDecl.h"
#include "H3Camera.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

#define MAX_CHANGE_in_COMP_EXT_REFINE (1e-10)
#define MIN_NBTARGET (5)
static CString strModule("Extrinsic_param");

#define H3MAX_ITER_HOMOGRAPHY (10)
#define H3MIN_TARGET (5)				//valeur arbitraire

#define DEFAULT_CALC_NAME "Calc"

#define METHOD0 1
#define METHOD1 0
#define METHOD2 0

//	La fonction XTransX réécrite pour X=[A,Z,Ka;Z,A,La]
//	A, matrice n*3
//	Z, matrice n*3 pleine de 0
//	Ka=[-k[i]*A[i,j]], k matrice n*1
//	La=[-l[i]*A[i,j]], l matrice n*1
//	legeres modifs cv 210606 (nom des variables)
template <class TYPE> static 
H3_MATRIX SpecialXTransX(H3_MATRIX& A,H3_MATRIX& k,H3_MATRIX& l)
{
	CString strFunction("SpecialXTransX");

	long Aco=A.GetCo(), Ali=A.GetLi();
	long i,j,f;

	if(k.GetLi()!=Ali || l.GetLi()!=Ali)
	{
		//error
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError("CorrespList",strFunction,"Error kl ligne");
		#else
			AfxMessageBox("CorrespList, fct SpecialXTransX2 : Error kl ligne");
		#endif
		AfxThrowUserException();
		return H3_MATRIX(0,0);
	}
	if(k.GetCo()!=1 || l.GetCo()!=1)
	{
		//error
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError("CorrespList",strFunction,"Error kl colonne");
		#else
			AfxMessageBox("CorrespList, fct SpecialXTransX2 : Error kl colonne");
		#endif
		AfxThrowUserException();
		return H3_MATRIX(0,0);
	}
	if(Aco!=3)
	{
		//error
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugInfo("CorrespList",strFunction,"Error A");
		#else
			AfxMessageBox("CorrespList, fct SpecialXTransX2 : Error A");
		#endif
		AfxThrowUserException();

		return H3_MATRIX(0,0);
	}

	H3_MATRIX ATA=Mat_XTransX(A);	//3*3
	H3_MATRIX kATA(Aco,Aco),lATA(Aco,Aco);
	H3_MATRIX k2l2ATA(Aco,Aco);
	kATA.Fill(0);
	lATA.Fill(0);
	k2l2ATA.Fill(0);

	TYPE *pk=k.GetData(),*pl=l.GetData();
	TYPE *pAf0=A.GetData();	//debut ligne f de la matrice A,initialisé pour f=0
	TYPE *pAi=(TYPE *)pAf0,*pAj=(TYPE *)pAf0;

	H3_MATRIX XTX(3*Aco,3*Aco);

	const TYPE *pkATA0=kATA.GetData(),*plATA0=lATA.GetData();
	const TYPE *pk2l2ATA0=k2l2ATA.GetData();
	TYPE *pkATA,*plATA;
	TYPE *pk2l2ATA;	
	TYPE Afi,AfiAfj;

	for(f=0;f<Ali;f++)
	{
		pkATA=(TYPE *)pkATA0;
		plATA=(TYPE *)plATA0;
		pk2l2ATA=(TYPE *)pk2l2ATA0;
		for(i=0;i<Aco;i++)
		{
			Afi=(*(pAi++));
			pAj=pAf0;
			for(j=0;j<Aco;j++)
			{
				AfiAfj=Afi*(*(pAj++));
				(*(pkATA++)) -= (*(pk))*AfiAfj;
				(*(plATA++)) -= (*(pl))*AfiAfj;
				(*(pk2l2ATA++)) += ((*(pk))*(*(pk))+(*(pl))*(*(pl)))*AfiAfj;
			}
		}
		pAf0=pAi;
		pk++;pl++;
	}

	XTX.Fill(0);
	XTX.SetAt(0,0,ATA);
	XTX.SetAt(3,3,ATA);
	XTX.SetAt(0,6,kATA.Trans());
	XTX.SetAt(3,6,lATA.Trans());
	XTX.SetAt(6,0,kATA);
	XTX.SetAt(6,3,lATA);
	XTX.SetAt(6,6,k2l2ATA);

	return XTX;
}

//	La fonction Y=Mat_Mult(X1',X2) réécrite pour
//	X1=[A,Z,Kao;Z,A,Lao] , matrice 2n*9
//	X2=[B1;B2]
//	A, matrice n*3
//	Z, matrice n*3 pleine de 0
//	Kao=[-k[i]*A[i,j]], k matrice n*1
//	Lao=[-l[i]*A[i,j]], l matrice n*1
//	B1,B2, matrices n*m
template <class TYPE> static 
H3_MATRIX SpecialMat_Mult(H3_MATRIX& A,H3_MATRIX& k,H3_MATRIX& l,H3_MATRIX& B1,H3_MATRIX& B2)
{
	long Aco=A.GetCo(), Ali=A.GetLi(), Bco=B1.GetCo(), Bli=B1.GetLi();
	long i,j,f;

	if(k.GetLi()!=Ali || l.GetLi()!=Ali)
	{
		//error
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugInfo("CorrespList","fct SpecialMat_Mult","Error kl ligne");
		#endif
		AfxThrowUserException();
	}
	if(k.GetCo()!=1 || l.GetCo()!=1)
	{
		//error
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugInfo("CorrespList","fct SpecialMat_Mult","Error kl colonne");
		#endif
		AfxThrowUserException();
	}
	if(Aco!=3)
	{
		//error
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugInfo("CorrespList","fct SpecialMat_Mult","Error A ligne");
		#endif
		AfxThrowUserException();
	}
	if(Ali!=Bli)
	{
		//error
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugInfo("CorrespList","fct SpecialMat_Mult","Error AB");
		#endif
		AfxThrowUserException();
	}
	if((B2.GetLi()!=Bli)||(B2.GetCo()!=Bco))
	{
		//error
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugInfo("CorrespList","fct SpecialMat_Mult","Error B1 B2");
		#endif
		AfxThrowUserException();
	}

	H3_MATRIX Y(9,Bco);
	H3_MATRIX ATB1=Mat_XTransY(A,B1);	
	H3_MATRIX ATB2=Mat_XTransY(A,B2);
	H3_MATRIX klATB(Aco,Bco);

	const TYPE *pklATB0=klATB.GetData();
	TYPE *pklATB;
	TYPE *pk=k.GetData(),*pl=l.GetData();
	TYPE *pA=A.GetData();
	TYPE *pB1=B1.GetData(),*pB10=B1.GetData();
	TYPE *pB2=B2.GetData(),*pB20=B2.GetData();
	klATB.Fill(0);

	TYPE Afi;

	for(f=0;f<Ali;f++)
	{
		pklATB=(TYPE*)pklATB0;
		for(i=0;i<Aco;i++)
		{
			Afi=(*(pA++));
			pB1=pB10;
			pB2=pB20;
			for(j=0;j<Bco;j++)
			{
				(*(pklATB++)) -= Afi*( (*pk)*(*(pB1++)) + (*pl)*(*(pB2++)) );
			}
		}
		pk++;pl++;
		pB10=pB1;
		pB20=pB2;
	}

	Y.SetAt(0,0,ATB1);
	Y.SetAt(3,0,ATB2);
	Y.SetAt(6,0,klATB);

	return Y;
}

//	Normalisation des données metriques (par rapport à z) et calcul du pixel moyen
static	bool MetricNormalisation(const CL_ARRAY2D& MetricData,CL_ARRAY& XX,CL_ARRAY& YY)
{
	CString strFunction("MetricNormalisation");
	H3DebugInfo(strModule,strFunction,"");

	long co=MetricData.GetCo(),li=MetricData.GetLi();

	if(li!=3 && li!=2)
	{
		H3DebugError(strModule,strFunction,"la donnée entrée n'a pas un nombre de lignes convenable");
	}
	XX=CL_ARRAY(co);
	YY=CL_ARRAY(co);

	CL_TYPE* px_m=MetricData.GetLine(0);
	CL_TYPE* py_m=MetricData.GetLine(1);
	CL_TYPE* pz_m;
	if(li==3) pz_m=MetricData.GetLine(2);
	else      pz_m=nullptr;

	CL_TYPE* p_xx=XX.GetData();
	CL_TYPE* p_yy=YY.GetData();

	long i;

	if(pz_m!=nullptr)
	{
		if((*pz_m)!=0)	//si le premier élément est nul, ils le sont tous a priori
		{
			i=0;
			while(i<co)
			{
				//normalisation des données metriques
				(*(p_xx++))=(*(px_m++))/(*pz_m);
				(*(p_yy++))=(*(py_m++))/(*pz_m);
				pz_m++;
				i++;
			}
		}
		else
		{
			i=0;
			while(i<co)
			{
				//normalisation des données metriques
				(*(p_xx++))=(*(px_m++));
				(*(p_yy++))=(*(py_m++));
				i++;
			}
		}
	}
	else
	{
		i=0;
		while(i<co)
		{
			//normalisation des données metriques
			(*(p_xx++))=(*(px_m++));
			(*(p_yy++))=(*(py_m++));
			i++;
		}
	}
	return true;
}

//	initialisation de mn
//	mn=Hnorm*(pixel_x(0:n); pixel_y(0:n);1(0:n))
static bool GetMN(const CRotation& Hnorm, const CL_ARRAY2D& Pixel ,Matrix& mn)
{
	CString strFunction("GetMN");
	H3DebugInfo(strModule,strFunction,"");

	long co=Pixel.GetCo(),li=Pixel.GetLi(),i;
	if(li!=2)
	{
		H3DebugError(strModule, strFunction, "données non valides");
		return false;
	}
	mn=Matrix(3,co);

	//	initialisation de mn
	//	mn=(*pHnorm)*[(*px);(*py);1]

	double *Lignex,*Ligney,*Lignez,*R;
	double x,y;
	Lignex=mn.GetLine(0);
	Ligney=mn.GetLine(1);
	Lignez=mn.GetLine(2);
	R=Hnorm.GetData();

	double	R11=R[0],R12=R[1],R13=R[2],
			R21=R[3],R22=R[4],R23=R[5],
			R31=R[6],R32=R[7],R33=R[8];

	CL_TYPE* px_p=Pixel.GetLine(0);
	CL_TYPE* py_p=Pixel.GetLine(1);

	i=0;
	while(i<co)
	{
		x=(*(px_p++));
		y=(*(py_p++));
		(*(Lignex++))=R11*x+R12*y+R13;
		(*(Ligney++))=R21*x+R22*y+R23;
		(*(Lignez++))=R31*x+R32*y+R33;
		i++;
	}
	return true;
}

static bool GetHREM(const CL_ARRAY& XX,const CL_ARRAY& YY,const Matrix& mn,CRotation& Hrem)
{
	long nb=XX.GetSize(),i;

	CL_TYPE* px2=XX.GetData();
	CL_TYPE* py2=YY.GetData();

	double* Lignex=mn.GetLine(0);
	double* Ligney=mn.GetLine(1);
	double* Lignez=mn.GetLine(2);
	i=0;

	CL_MATRIX A(nb,2),AA(nb,3),k(nb,1),l(nb,1);
	CL_TYPE *pA=A.GetData(),*pAA=AA.GetData(),*pk=k.GetData(),*pl=l.GetData();

	while(i<nb)
	{
		(*(pA++))=(*px2);
		(*(pA++))=(*py2);
		(*(pk++))=(*Lignex);
		(*(pl++))=(*Ligney);

		(*(pAA++))=(*px2);
		(*(pAA++))=(*py2);
		(*(pAA++))=1;

		i++;
		px2++;py2++;
		Lignex++;Ligney++;
	}

	CL_MATRIX test=SpecialXTransX(AA,k,l);

	//decomposition de L (matrice 9*9) en éléments simples
	CL_MATRIX U(9,9);
	CL_MATRIX W(9,1);
	CL_MATRIX V(9,9);

	test.SVDcmp(U,W,V);
	
	CL_TYPE *pv;
	double *ph;
	pv=V.GetData();
	pv+=8;
	ph=Hrem.GetData();
	
	for(i=0;i<9;i++)
	{
		(*(ph++))=(*pv);
		pv+=9;
	}
	Hrem /= Hrem(2,2);

	return true;
}

static bool GetMREP_ERR(const CRotation& H, const CL_ARRAY& Xnew_x,const CL_ARRAY& Xnew_y, const CL_ARRAY2D& xn, CL_ARRAY2D& mrep, CL_ARRAY& m_err)
{
	CL_TYPE *pmrepx,*pmrepy,*pmrepz;
	CL_TYPE *perr;
	CL_TYPE *px_p,*py_p;
	CL_TYPE *px2,*py2;
	double *pH=H.GetData();

	long i;
	long Imax=Xnew_x.GetSize();

	perr=m_err.GetData();
	
	pmrepx=mrep.GetLine(0);
	pmrepy=mrep.GetLine(1);
	pmrepz=mrep.GetLine(2);
	px_p=xn.GetLine(0);
	py_p=xn.GetLine(1);
	px2=Xnew_x.GetData();
	py2=Xnew_y.GetData();
	//

	i=0;
	while(i<Imax)
	{
		//rempli mrep=H*M
		(*pmrepx)=(pH[0]*(*px2)+pH[1]*(*py2)+pH[2]);
		(*pmrepy)=(pH[3]*(*px2)+pH[4]*(*py2)+pH[5]);
		(*pmrepz)=(pH[6]*(*px2)+pH[7]*(*py2)+pH[8]);

		(*pmrepx) /=(*pmrepz);
		(*pmrepy) /=(*pmrepz);
																		
		//rempli m_err
		(*(perr++))=(*px_p)-(*pmrepx);
		(*(perr++))=(*py_p)-(*pmrepy);

		px_p++;py_p++;
		pmrepx++;pmrepy++;pmrepz++;
		px2++;py2++;
		i++;
	}
	return true;
}

static bool GetJ(const CL_ARRAY& Xnew_x,const CL_ARRAY& Xnew_y, const CL_ARRAY2D& mrep, CL_ARRAY2D &J)
{
	CL_TYPE *pJ=J.GetData();
	CL_TYPE *px2=Xnew_x.GetData();
	CL_TYPE *py2=Xnew_y.GetData();
	CL_TYPE *pmrepx=mrep.GetLine(0);
	CL_TYPE *pmrepy=mrep.GetLine(1);
	CL_TYPE *pmrepz=mrep.GetLine(2);
	size_t size=Xnew_x.GetSize();
	double x,y,z;

	for (size_t i=0; i<size; i++)
	{
		x=(*px2)/(*pmrepz);
		y=(*py2)/(*pmrepz);
		z=1/(*pmrepz);
		(*(pJ++))=-x;
		(*(pJ++))=-y;
		(*(pJ++))=-z;
		(*(pJ++))=0;
		(*(pJ++))=0;
		(*(pJ++))=0;
		(*(pJ++))=x*(*pmrepx);
		(*(pJ++))=y*(*pmrepx);

		(*(pJ++))=0;
		(*(pJ++))=0;
		(*(pJ++))=0;
		(*(pJ++))=-x;
		(*(pJ++))=-y;
		(*(pJ++))=-z;
		(*(pJ++))=x*(*pmrepy);
		(*(pJ++))=y*(*pmrepy);

		px2++;py2++;
		pmrepx++;pmrepy++;pmrepz++;
	}
	return true;
}

//////////////////////////////////////////////////////////
//compute_Homography
//auteur cv d'apres fct homonyme pour matlab
//mars 04
//////////////////////////////////////////////////////////
static CRotation compute_Homography(const CL_ARRAY2D& xn,const CL_ARRAY& Xnew_x,const CL_ARRAY& Xnew_y, CRotation& out_Hnorm, CRotation& out_inv_Hnorm,PARAM_EXTRINSIC_STRUCT param)
{
	CString strFunction=_T("compute_Homography"),msg;
	H3DebugInfo(strModule,strFunction,"in");

	//verification
	//inutile si le constructeur ou SetPixel et SetMetric ont ete utilisés
	long PixelCo=xn.GetCo();
	if(PixelCo<param.nMinTarget)
	{
		msg.Format("le nombre de cibles détectées est de %d, inférieur au minimum requis (%d)",
			PixelCo,param.nMinTarget);
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,msg);
		#else
			AfxMessageBox(msg);
		#endif
		AfxThrowUserException();
	}
	if(Xnew_x.GetSize()!=PixelCo || Xnew_y.GetSize()!=PixelCo)
	{
		msg.Format("données de tailles distinctes (%d %d %d)",
			PixelCo,Xnew_x.GetSize(),Xnew_y.GetSize());
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,msg);
		#else
			AfxMessageBox(msg);
		#endif
		AfxThrowUserException();
	}
	//fin verif
	
	long i=0;
	CL_TYPE meanx=0,meany=0;

	CL_TYPE* px_p=xn.GetLine(0);
	CL_TYPE* py_p=xn.GetLine(1);
	while(i<PixelCo)
	{
		meanx += (*(px_p++));
		meany += (*(py_p++));
		i++;
	}
	meanx /= PixelCo;
	meany /= PixelCo;

	//prenormalisation
	i=0;
	px_p=xn.GetLine(0);
	py_p=xn.GetLine(1);

	CL_TYPE scxx=0,scyy=0;
	while(i<PixelCo)
	{
		scxx += fabs( (*px_p)-meanx );
		scyy += fabs( (*py_p)-meany );

		(px_p++);
		(py_p++);
		i++;
	}
	scxx /= PixelCo;
	scyy /= PixelCo;

	double hnorm[9]={1/scxx,0,-meanx/scxx,0,1/scyy,-meany/scyy,0,0,1};
	out_Hnorm=CRotation(hnorm);

	double ihnorm[9]={scxx,0,meanx,0,scyy,meany,0,0,1};
	out_inv_Hnorm=CRotation(ihnorm);

	Matrix mn(3,PixelCo);
	//initialisation de mn
	//mn=(*pHnorm)*[(*px);(*py);1]
	if(!GetMN(out_Hnorm, xn ,mn)){
		H3DebugError(strModule,strFunction, "1");
		return CRotation();
	}

	//Compute the homography between m and mn

	//Build the Matrix
	CRotation Hrem;
	if(!GetHREM(Xnew_x,Xnew_y,mn,Hrem)){
		H3DebugError(strModule,strFunction, "2");
		return CRotation();
	}

	CRotation H=out_inv_Hnorm*Hrem;
	
	if(PixelCo>4)
	{
		CL_MATRIX mrep(3,PixelCo);
		CL_MATRIX MMM(3,PixelCo);
		CL_MATRIX JA(PixelCo,3),JK(PixelCo,1),JL(PixelCo,1);
		CL_MATRIX m_err(2*PixelCo,1);
		CL_MATRIX hh_innov(8,1),hhv(8,1),mat_temp(8,8);
#if METHOD0
		CL_MATRIX J(2*PixelCo,8);
		J.Fill(0);
#endif

		CL_TYPE *phhv;
		phhv=hhv.GetData();
		for(i=0;i<8;i++)
		{
			phhv[i]=H[i];
		}

		for(short iter=0;iter<param.nMaxIterHomography;iter++)
		{
			
			if(!GetMREP_ERR( H,Xnew_x,Xnew_y,xn,mrep,m_err))
			{
				H3DebugError(strModule,strFunction, "3");
				return CRotation();
			}
			
#if METHOD0
			//construction de J
			if(!GetJ(Xnew_x,Xnew_y,mrep,J))
			{
				H3DebugError(strModule,strFunction, "2");
				return CRotation();
			}
#endif

#if METHOD2
			CL_TYPE *pJA=JA.GetData();
			CL_TYPE *pJK=JK.GetData();
			CL_TYPE *pJL=JL.GetData();
			px2=pXX;
			py2=pYY;
			pmrepx=mrep.GetLine(0);
			pmrepy=mrep.GetLine(1);
			pmrepz=mrep.GetLine(2);
			i=validTarget;
			CL_TYPE x,y,z;

			i=0;
			while(i<PixelCo){//cv 16/11/05 version +rapide
					x=(*px2)/(*pmrepz);
					y=(*py2)/(*pmrepz);
					z=1/(*pmrepz);

					(*(pJA++))=-x;
					(*(pJA++))=-y;
					(*(pJA++))=-z;
					(*(pJK++))=(*pmrepx);
					(*(pJL++))=(*pmrepy);					
				px2++;py2++;
				pmrepx++;pmrepy++;pmrepz++;
				i++;			
			}
#endif
			
#if METHOD0 //solution 0
			try{
				hh_innov=Mat_MeanSquare(J,m_err);
				if(hh_innov.GetSize()==0)
				{
					H3DebugError(strModule,strFunction,"echec à l'inversion");
					hh_innov=H3_MATRIX_FLT32(8,1);
					hh_innov.Fill(0);
				}
				iter=(short)param.nMaxIterHomography;
			}
			catch(...){}
#elif METHOD1	//solution 1
			Matrix JTJ=Mat_XTransX(J);
			Matrix JTE=J.Trans()*m_err;

			Matrix JTJ=SpecialXTransX(JA,JK,JL);
			JTJ=JTJ.GetAt(0,0,8,8);
				
			Matrix JTE=SpecialMat_Mult(JA,JK,JL,m_errx,m_erry);
			JTE=JTE.GetAt(0,0,8,1);
			
			try{
				hh_innov=JTJ.Inv()*JTE;
			}
			catch(...){}
#else //(METHOD2)solution 2
			
			Matrix JTJ=SpecialXTransX(JA,JK,JL);
			JTJ=JTJ.GetAt(0,0,8,8);
			
			Matrix JTE=SpecialMat_Mult(JA,JK,JL,m_errx,m_erry);
			JTE=JTE.GetAt(0,0,8,1);

			try{
				hh_innov=Mat_MeanSquare_chols(JTJ,JTE);
			}
			catch(...){
				#if defined(H3APPTOOLSDECL_H__INCLUDED_)
				H3DebugError(strModule,strFunction,"JTJ non inversible");
				#endif
				AfxThrowUserException();
			}
#endif

			hhv=hhv-hh_innov;

			for(i=0;i<8;i++)
			{
				H[i]=phhv[i];
			}
			H[8]=1;
		}
	}
	return H;
}

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CExtrinsic_param::CExtrinsic_param()
{
	m_param.nMaxIterHomography=H3MAX_ITER_HOMOGRAPHY;
	m_param.nMinTarget=H3MIN_TARGET;

	m_omc.ReAlloc(3,1);
	m_Tc.ReAlloc(3,1);

	m_omc.Fill(0);
	m_Rc=m_omc.Rodrigues();
	m_Tc.Fill(0);
}

CExtrinsic_param::CExtrinsic_param(const Matrix& omc, const Matrix& Tc):m_omc(omc),m_Tc(Tc)
{
	H3_MATRIX_FLT64 tmp;
	m_Rc=m_omc.Rodrigues(tmp);
}

CExtrinsic_param::~CExtrinsic_param()
{
}

//////////////////////////////////////////////////////////
//compute_ext_calib
//auteur cv d'apres fct homonyme pour matlab
//avril 04
//////////////////////////////////////////////////////////
/*! 
* 	\fn      bool CExtrinsic_param::compute_ext_calib(CCorrespList2& CL, const CH3Camera& Cam,long MaxIter,double* pCond,double thresh_cond)
* 	\author  V Chalvidan
* 	\brief   
* 	\param   CCorrespList2& CL : Liste pixel-coordonnée métrique
* 	\param   const CH3Camera& Cam : Camera
* 	\param   long MaxIter : nombre maximum d'iterations
* 	\param   double* pCond : 
* 	\param   double thresh_cond : Seuil de condition d'élimination d'une image
* 	\return  bool
* 	\remarks 
*/ 

bool CExtrinsic_param::compute_ext_calib(CCorrespList2& CL, const CH3Camera& Cam,long MaxIter,double* pCond,double thresh_cond)
{
	CString strFunction("compute_ext_calib");
	H3DebugInfo(strModule,strFunction,"");


 	if((!Cam.cc[0]) || (!Cam.cc[1]) || (!Cam.fc[0])) return false;
	m_bPixelNorm=true;
	if(compute_extrinsic_init(CL,Cam,m_bPixelNorm) ) 
		return compute_extrinsic_refine(CL,Cam,MaxIter,pCond);
	else
	{
		H3DebugError(strModule,strFunction,"echec lors de l'initialisation");
		return false;
	}
}

//////////////////////////////////////////////////////////
//compute_extrinsic
//auteur cv d'apres fct homonyme pour matlab
//mars 04
//////////////////////////////////////////////////////////
/*! 
* 	\fn      bool CExtrinsic_param::compute_extrinsic(CCorrespList2& CL, const CH3Camera& Cam,long MaxIter,double* pCond,double thresh_cond)
* 	\author  V Chalvidan
* 	\brief   
* 	\param   CCorrespList2& CL : Liste pixel-métrique faite avec les params en entrées  Pix et Metric
* 	\param   const CH3Camera& Cam : 
* 	\param   long MaxIter : nombre maximum d'iteration
* 	\param   double* pCond : 
* 	\param   double thresh_cond : seuil maximum d'elimination d'une image
* 	\return  bool
* 	\remarks 
*/
bool CExtrinsic_param::compute_extrinsic(const H3_ARRAY2D_FLT64& Pix,const H3_ARRAY2D_FLT64& Metric, const CH3Camera& Cam,long MaxIter, double* pCond, double thresh_cond)
{
	CCorrespList2 CL(Pix,Metric);
	if(CL.Pixel.GetSize()!=Pix.GetSize())
		return false;

	return compute_extrinsic(CL,Cam,MaxIter,pCond,thresh_cond);

}

bool CExtrinsic_param::compute_extrinsic(CCorrespList2& CL, const CH3Camera& Cam,long MaxIter,double* pCond,double thresh_cond)
{
	static CString strFunction("compute_extrinsic");

	if(CL.Pixel.GetCo()<MIN_NBTARGET)
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"nb cibles inferieur au minimum");
		#endif
		return false;
	}

	if((!Cam.cc[0]) && (!Cam.cc[1]) && (!Cam.fc[0])) return false;
	m_bPixelNorm=true;

	if(compute_extrinsic_init(CL,Cam,m_bPixelNorm))
	{
		if(!compute_extrinsic_refine(CL,Cam,MaxIter,pCond))
		{
			H3DebugError(strModule,strFunction,"echec lors de refine");
			return false;
		}
	}
	else
	{
		H3DebugError(strModule,strFunction,"echec lors de l'initialisation");
		return false;
	}

	double ptemp1[9]={m_Rc(0),m_Rc(1),m_Tc(0),m_Rc(3),m_Rc(4),m_Tc(1),m_Rc(6),m_Rc(7),m_Tc(2)};
	CL.H=CRotation(ptemp1);
	double ptemp2[9]={	Cam.fc[0],Cam.alpha_c*Cam.fc[0],Cam.cc[0],
						0,Cam.fc[1],Cam.cc[1],
						0,0,1};
	CRotation Rtemp(ptemp2);
	CL.H = Rtemp * CL.H;

	//projection des points metriques dans l'espace pixel avec les parametres déterminés
	CL_ARRAY2D pixel(2,CL.m_nTarget);

	projectPoints2((CL.MetricData),Cam,pixel);

	//mesure des erreurs
	CL.Err= H3_ARRAY2D_FLT64(2,CL.m_nTarget);

	if (CL.Err.GetSize()<2)
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"CL.Err.GetSize()<2");
		#endif
		return false;
	}
	if (pixel.GetSize()<2)
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"pixel.GetSize()<2");
		#endif
		return false;
	}
	if (CL.Pixel.GetSize()<2)
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"CL.Pixel.GetSize()<2");
		#endif
		return false;
	}

	//calcul de l'ecart pixel mesuré/pixel calculé
	size_t size=CL.m_nTarget;
	CL_TYPE *p10,*p20,*p30,*p11,*p21,*p31;

	p10=CL.Err.GetLine(0);	 
	p20=pixel.GetLine(0);			
	p30=CL.Pixel.GetLine(0);	
	p11=CL.Err.GetLine(1);
	p21=pixel.GetLine(1);
	p31=CL.Pixel.GetLine(1);

	for (size_t i=0; i<size; i++)
	{
		(*(p10++)) = (*(p20++))-(*(p30++));
		(*(p11++)) = (*(p21++))-(*(p31++));
	}

	return true;
}

//////////////////////////////////////////////////////////
//compute_extrinsic_init
//auteur cv d'apres fct homonyme pour matlab
//mars 04
//////////////////////////////////////////////////////////

/*! 
* 	\fn      bool CExtrinsic_param::compute_extrinsic_init(CCorrespList2& CL,
											  const CH3Camera& Cam,bool& m_bPixelNorm)
* 	\author  V Chalvidan
* 	\brief   
* 	\param   CCorrespList& CL : 
* 	\param   const CH3Camera& Cam : 
* 	\param   bool& m_bPixelNorm : booleen afin d'utiliser la pixel norm ou non
* 	\return  bool
* 	\remarks 
*/ 

bool CExtrinsic_param::compute_extrinsic_init(CCorrespList2& CL, const CH3Camera& Cam, bool& m_bPixelNorm)
{
	CString strFunction("compute_extrinsic_init");
	H3DebugInfo(strModule,strFunction,"in");

	long NbPoints=CL.Pixel.GetCo();
	long NbValidPoints=CL.validPixel.GetCo();

	long validTarget=CL.validMetricData.GetCo();

	//Preparation du calcul
	H3_ARRAY2D_FLT64 xn(2,validTarget);
	Cam.normalise(CL.validPixel,xn);

	if(!CL.m_3D)
	{
		CString msg;
		msg.Format(" %d %d",CL.m_V.GetLi(),CL.m_V.GetCo());
		H3DebugInfo(strModule,strFunction,_T("flat ")+msg);

		//transform the plane to bring it to the z=0 plane
		CRotation R_transform(H3_MATRIX_FLT64(CL.m_V.Trans()));

		H3_MATRIX_FLT64 Rtemp(H3_MATRIX_FLT64(R_transform.GetAt(0,2,2,1)));
		if(Rtemp.Norm()<1e-6)
			R_transform=CRotation();
		if(R_transform.Det()<0)
			R_transform=CRotation((H3_MATRIX_FLT64)R_transform*(-1));

		//T_transform=-(R_transform)*Xmean;
		Matrix T_transform(3,1);
		Matrix Xmean(3,1);
		Xmean[0]=-CL.m_MetricMean[0];Xmean[1]=-CL.m_MetricMean[1];Xmean[2]=-CL.m_MetricMean[2];
		T_transform=(Matrix)R_transform*Xmean;

		//Xnew=R_transform*X+T_transform*ones(...)
		double tx=T_transform(0,0),ty=T_transform(1,0),tz=T_transform(2,0);

		H3_ARRAY2D_FLT64 Xnew=(Matrix)R_transform*(Matrix)CL.validMetricData;
		H3_ARRAY_FLT64 Xnew_x(NbValidPoints);
		H3_ARRAY_FLT64 Xnew_y(NbValidPoints);

		CL_TYPE* pX=CL.MetricData.GetLine(0);
		CL_TYPE* pY=CL.MetricData.GetLine(1);
		CL_TYPE* pZ=CL.MetricData.GetLine(2);

		double *p_Xn1x,*p_Xn1y,*p_Xn1z;
		double *p_Xn2x,*p_Xn2y;

		p_Xn1x=Xnew.GetLine(0);
		p_Xn1y=Xnew.GetLine(1);
		p_Xn1z=Xnew.GetLine(2);

		p_Xn2x=Xnew_x.GetData();
		p_Xn2y=Xnew_y.GetData();

		size_t size=NbValidPoints;

		for (size_t i=0; i<size; i++)
		{
			(*p_Xn1x)+=tx;
			(*p_Xn1y)+=ty;
			(*p_Xn1z)+=tz;

			(*p_Xn2x)=(*p_Xn1x);
			(*p_Xn2y)=(*p_Xn1y);

			p_Xn1x++;p_Xn1y++;p_Xn1z++;
			p_Xn2x++;p_Xn2y++;
		}

		CRotation temp1,temp2;
		CRotation H;
		try{
			//Ajout SJ 11-07 : cette fonction est appellé a plusieurs endroits : dans init_param de la classe
			//Camera : ceux sont des pixels non normalisés qui sont utilisés.
			if (m_bPixelNorm)
			{
				H=compute_Homography(xn,Xnew_x,Xnew_y,temp1,temp2,m_param);
				if(H[0]==0 && H[4]==0 && H[8]==0)
				{
					H3DebugInfo(strModule,strFunction,"calcul de l'homographie impossible 0");
					return false;
				}
			}
			else
			{
				H=compute_Homography(CL.validPixel,Xnew_x,Xnew_y,temp1,temp2,m_param);
				if(H[0]==0 && H[4]==0 && H[8]==0)
				{
					H3DebugInfo(strModule,strFunction,"calcul de l'homographie impossible 1");
					return false;
				}
			}
		}
		catch(...)
		{
			H3DebugInfo(strModule,strFunction,"calcul de l'homographie impossible");
			return false;
		}

		double sc=	( ::sqrt(H(0,0)*H(0,0)+H(1,0)*H(1,0)+H(2,0)*H(2,0))
					+ ::sqrt(H(0,1)*H(0,1)+H(1,1)*H(1,1)+H(2,1)*H(2,1)))/2;

		H=CRotation(H/sc);
		CL.H=H;
		CL.m_bHinitialised=true;

		Matrix u1=H.GetAt(0,0,3,1),u2=H.GetAt(0,1,3,1),u3(3,1),tmp;
		tmp=Mat_Dot(u1,u1);

		u1 /= sqrt(tmp[0]);
		tmp=Mat_Dot(u1,u2);

		u2 -= u1*(tmp[0]);
		tmp=Mat_Dot(u2,u2);

		u2 /= sqrt(tmp[0]);

		u3=Mat_Cross(u1,u2);

		double d_temp[9]={u1(0),u2(0),u3(0),u1(1),u2(1),u3(1),u1(2),u2(2),u3(2)};
		CRotation RRR(d_temp);

		Matrix Mtemp;

		m_omc=RRR.Rodrigues(Mtemp);
		
		m_Rc=CRotation(m_omc.Rodrigues(Mtemp));
		m_Tc= Matrix(H.GetAt(0,2,3,1));

		m_Tc +=m_Rc.Matrix::operator*(T_transform);
		m_Rc = m_Rc*R_transform;

		m_omc=m_Rc.Rodrigues(Mtemp);
		m_Rc=CRotation(m_omc.Rodrigues(Mtemp));

	}
	else
	{	//structure non plane
		//J matrice 2n*12 avec une structure compliquée
		H3DebugInfo(strModule,strFunction,"not flat");
		long i,j;
		Matrix n=Matrix(CL.Pixel);
		Matrix X=Matrix(CL.MetricData);
		Matrix XXt=Mat_XXTrans(X);
		Matrix nxX=X;
		Matrix nyX=X;

		long Xco=X.GetCo(),Xli=X.GetLi();
		double *pnxX=nxX.GetData(),*pnyX=nyX.GetData();
		double *pnx,*pny;
		for(j=0;j<Xli;j++)
		{
			pnx=xn.GetData();
			pny=pnx+Xco;
			for(i=0;i<Xco;i++)
			{
				(*(pnxX++)) *= (*(pnx++));
				(*(pnyX++)) *= (*(pny++));
			}
		}

		Matrix nxXXt=Mat_XYTrans(nxX,X);
		Matrix nyXXt=Mat_XYTrans(nyX,X);
		Matrix nxXnxXt=Mat_XXTrans(nxX);
		Matrix nyXnyXt=Mat_XXTrans(nyX);

		double sum_Xx=0,sum_Xy=0,sum_Xz=0;
		double sum_nx_Xx=0,sum_nx_Xy=0,sum_nx_Xz=0;
		double sum_ny_Xx=0,sum_ny_Xy=0,sum_ny_Xz=0;
		double sum_nx=0,sum_ny=0;
		double sum_nx2=0,sum_ny2=0;
		double sum_n2_Xx=0,sum_n2_Xy=0,sum_n2_Xz=0;

		double *pXx=X.GetData(),*pXy=pXx+Xco,*pXz=pXy+Xco;
		double *pnxXx=nxX.GetData(),*pnxXy=pnxXx+Xco,*pnxXz=pnxXy+Xco;
		double *pnyXx=nyX.GetData(),*pnyXy=pnyXx+Xco,*pnyXz=pnyXy+Xco;
		pnx=xn.GetData();pny=pnx+Xco;

		for(j=0;j<Xco;j++)
		{
			sum_Xx    += (*pXx);   sum_Xy	 += (*pXy);	  sum_Xz	+= (*pXz); 
			sum_nx_Xx += (*pnxXx); sum_nx_Xy += (*pnxXy); sum_nx_Xz += (*pnxXz);
			sum_ny_Xx += (*pnyXx); sum_ny_Xy += (*pnyXy); sum_ny_Xz += (*pnyXz);
			sum_nx += (*pnx); sum_ny += (*pny);
			sum_nx2 += (*pnx)*(*pnx); sum_ny2 += (*pny)*(*pny);
			sum_n2_Xx += (*pnx)*(*pnxXx)+(*pny)*(*pnyXx);
			sum_n2_Xy += (*pnx)*(*pnxXy)+(*pny)*(*pnyXy);
			sum_n2_Xz += (*pnx)*(*pnxXz)+(*pny)*(*pnyXz);

			pXx++;pXy++;pXz++;
			pnxXx++;pnxXy++;pnxXz++;
			pnyXx++;pnyXy++;pnyXz++;
			pnx++;pny++;
		}

		Matrix JTJ(12,12);
		JTJ.Fill(0);

		//remplissage à la main ou presque
		CH3Array2D< Matrix > M(4,4);
		double *pM;
		for(i=0;i<3;i++)
		{
			for(j=0;j<=i;j++)
			{
				M(i,j)=Matrix(3,3);
				pM=M(i,j).GetData();
				pM[0]=pM[4]=XXt(i,j);
				pM[1]=pM[3]=0;
				pM[2]=pM[6]=-nxXXt(i,j);
				pM[5]=pM[7]=-nyXXt(i,j);
				pM[8]=nxXnxXt(i,j)+nyXnyXt(i,j);
			}
		}
		for(i=0;i<3;i++)
		{
			for(j=0;j<i;j++)
			{
				M(j,i)=M(i,j);
			}
		}

		M(0,3)=Matrix(3,3);
		M(0,3)[0]=sum_Xx;		M(0,3)[1]=0;			M(0,3)[2]=-sum_nx_Xx;
		M(0,3)[3]=0	;			M(0,3)[4]=sum_Xx;		M(0,3)[5]=-sum_ny_Xx;
		M(0,3)[6]=-sum_nx_Xx;	M(0,3)[7]=-sum_ny_Xx;	M(0,3)[8]=sum_n2_Xx;

		M(1,3)=Matrix(3,3);
		M(1,3)[0]=sum_Xy;		M(1,3)[1]=0;			M(1,3)[2]=-sum_nx_Xy;
		M(1,3)[3]=0	;			M(1,3)[4]=sum_Xy;		M(1,3)[5]=-sum_ny_Xy;
		M(1,3)[6]=-sum_nx_Xy;	M(1,3)[7]=-sum_ny_Xy;	M(1,3)[8]=sum_n2_Xy;

		M(2,3)=Matrix(3,3);
		M(2,3)[0]=sum_Xz;		M(2,3)[1]=0;			M(2,3)[2]=-sum_nx_Xz;
		M(2,3)[3]=0	;			M(2,3)[4]=sum_Xz;		M(2,3)[5]=-sum_ny_Xz;
		M(2,3)[6]=-sum_nx_Xz;	M(2,3)[7]=-sum_ny_Xz;	M(2,3)[8]=sum_n2_Xz;

		M(3,3)=Matrix(3,3);
		M(3,3)[0]=Xco	;		M(3,3)[1]=0;			M(3,3)[2]=-sum_nx;
		M(3,3)[3]=0	;			M(3,3)[4]=Xco	;		M(3,3)[5]=-sum_ny;
		M(3,3)[6]=-sum_nx;		M(3,3)[7]=-sum_ny;		M(3,3)[8]=sum_nx2+sum_ny2;

		M(3,0)=M(0,3).Trans();
		M(3,1)=M(1,3).Trans();
		M(3,2)=M(2,3).Trans();

		for(i=0;i<4;i++)
		{
			for(j=0;j<4;j++)
			{
				JTJ.SetAt(3*i,3*j,M(i,j));
			}
		}

		Matrix U(12,12),V(12,12),W(1,12);
		JTJ.SVDcmp(U,W,V);

		CRotation RR;
		Matrix tmp;
		for(i=0;i<9;i++)
			RR[i]=V(i,11);
		RR=RR.Trans();

		if(RR.Det()<0)
		{
			V.SetAt(0,11,V.GetAt(0,11,12,1)*(-1));
			RR=RR*(-1);
		}

		Matrix Ur(3,3),Wr(1,3),Vr(3,3);
		RR.SVDcmp(Ur,Wr,Vr);

		m_Rc=Mat_XYTrans(Ur,Vr);

		Matrix temp=V.GetAt(0,11,9,1);
		double normtemp=0,normR=0;
		for(i=0;i<9;i++)
		{
			normtemp += temp[i]*temp[i];
			normR += m_Rc[i]*m_Rc[i];
		}
		normtemp=::sqrt(normtemp);
		normR=::sqrt(normR);

		m_Tc=V.GetAt(9,11,3,1)/(normtemp/normR);

		m_omc=m_Rc.Rodrigues(tmp);
		m_Rc=m_omc.Rodrigues(tmp);
	}
	H3DebugInfo(strModule,strFunction,"out");	
	return true;
}

//////////////////////////////////////////////////////////
//compute_extrinsic_refine
//auteur cv d'apres fct homonyme pour matlab
//mars 04
//////////////////////////////////////////////////////////
/*! 
* 	\fn      bool CExtrinsic_param::compute_extrinsic_refine(CCorrespList& CL,const CH3Camera& Cam,long MaxIter, double* pCond,double thresh_cond)
* 	\author  V Chalvidan
* 	\brief   
* 	\param   CCorrespList2& CL : liste correspondance pixel/coordonnées metriques
* 	\param   const CH3Camera& Cam : 
* 	\param   long MaxIter : nombre max d'iteration
* 	\param   double* pCond : 
* 	\param   double thresh_cond : Seuil de condition d'élimination d'une image
* 	\return  bool
* 	\remarks 
*/ 

bool CExtrinsic_param::compute_extrinsic_refine(CCorrespList2& CL, const CH3Camera& Cam, long MaxIter, double* pCond, double thresh_cond)
{
	CString strFunction("compute_extrinsic_refine");
	H3DebugInfo(strModule,strFunction,"in");

	double change = 1,cond;
	long iter=0,ii,jj;
	long nbPt=(CL.MetricData).GetCo();
	H3_MATRIX_FLT64 dxdom(2*nbPt,3),dxdT(2*nbPt,3);
	H3_MATRIX_FLT64 ex2(2*nbPt,1),JJ_T_ex2(6,1);
	H3_MATRIX_FLT64 JJ(2*nbPt,6);
	H3_MATRIX_FLT64 U2,V2,W2;
	H3_ARRAY2D_FLT64 x(2,nbPt),ex(2,nbPt);
	H3_MATRIX_FLT64 param(6,1),param_innov(6,1),param_up(6,1);
	H3_MATRIX_FLT64 temp;
	double norm_param_up;
	double *pex_x,*pex_y,*pex2;

	long vers=1;
	param.SetAt(0,0,m_omc);
	param.SetAt(3,0,m_Tc);

	while((change>MAX_CHANGE_in_COMP_EXT_REFINE)&&(iter<MaxIter))
	{
		if(!projectPoints2((CL.MetricData),Cam,x,&dxdom,&dxdT))
		{
			CString ErrorMsg;
			ErrorMsg.Format("CExtrinsic_param::compute_extrinsic_refine: Erreur1");
			throw ErrorMsg;
		}
		
		JJ.SetAt(0,0,dxdom);
		JJ.SetAt(0,3,dxdT);
		cond=JJ.Cond();
		

		H3_MATRIX_FLT64 L_JJ,p_JJ(6,1);
		if(vers==3)
		{
			//on utilise la propriété de JJ=[dxdom,dxdT] pour calculer JJ'*JJ
			H3_MATRIX_FLT64 dxdomTdxdom=Mat_XTransX(dxdom);
			H3_MATRIX_FLT64 dxdT_T_dxdT=Mat_XTransX(dxdT);
			H3_MATRIX_FLT64 dxdT_T_dxdom=Mat_XTransY(dxdT,dxdom);
			
			H3_MATRIX_FLT64 JTJ(6,6);
			JTJ.SetAt(0,0,dxdomTdxdom);
			JTJ.SetAt(3,3,dxdT_T_dxdT);
			JTJ.SetAt(3,0,dxdT_T_dxdom);
			JTJ.SetAt(0,3,dxdT_T_dxdom.Trans());

			cond=::sqrt(JTJ.Cond());

			//test utilisation du fait que JTJ est symetrique		
			try
			{
				L_JJ=JTJ.choldc(false,p_JJ);
			}
			catch(...)
			{
				#if defined(H3APPTOOLSDECL_H__INCLUDED_)
				H3DebugError(strModule,strFunction,"matrice non symetrique definie positive");
				#endif

				change=0;
				continue;
			}
		}

		if(cond > thresh_cond)
		{
			change=0;
		}
		else
		{
			ex=(CL.Pixel)-x;
			size_t size=nbPt;
			ii=0;jj=0;
			pex_x=ex.GetData();
			pex_y=pex_x+ex.GetCo();
			pex2=ex2.GetData();
			for (size_t i=0; i<size; i++)
			{
				(*(pex2++))=(*(pex_x++));
				(*(pex2++))=(*(pex_y++));
			}
			try
			{
				if(vers==1)
				{
					Matrix Old_pi=param_innov;
					param_innov=Mat_MeanSquare(JJ,ex2);

					if(param_innov.GetSize()==0)
					{
						param_innov=Old_pi*0;
						change=0;
						continue;
					}
				}

				if(vers==3)
					param_innov=L_JJ.cholsl(Mat_XTransY(JJ,ex2),p_JJ);
			}
			catch(...)
			{
				#if defined(H3APPTOOLSDECL_H__INCLUDED_)
				H3DebugError(strModule,strFunction,"param_innov non calculable");
				#endif

				change=0;
				continue;
			}

			param_up=param +param_innov;

			norm_param_up=param_up.Norm();
			if(norm_param_up==0)
				change=0;
			else
				change=param_innov.Norm()/norm_param_up;
			param=param_up;

			m_omc=Matrix(param.GetAt(0,0,3,1));
			m_Tc= Matrix(param.GetAt(3,0,3,1));
		}
		iter++;
	}
	if(pCond !=nullptr) (*pCond)=cond;
	m_Rc=CRotation(m_omc.Rodrigues(temp));
	return true;
}

//////////////////////////////////////////////////////////
//rigid_motion
//auteur cv d'apres fct homonyme pour matlab
//mars 04
//////////////////////////////////////////////////////////
H3_ARRAY2D_FLT64 CExtrinsic_param::rigid_motion(const H3_ARRAY2D_FLT64& X, H3_ARRAY2D_FLT64 *p_dYdom, H3_ARRAY2D_FLT64 *p_dYdT)
{
	//p_dYdom pointe sur un tableau 3*(*pX).GetCo() lignes * 3 colonnes
	//p_dYdT  pointe sur un tableau 3*(*pX).GetCo() lignes * 3 colonnes
	long nbPt=X.GetCo(),i,j,i0,i1,i2;

	//verifications:
	if(	((*p_dYdom).GetLi()!=3*nbPt)|| ((*p_dYdT).GetLi()!=3*nbPt) || 
		((*p_dYdom).GetCo()!=3)		|| ((*p_dYdT).GetCo()!=3) ) return false;

	H3_MATRIX_FLT64 dRdom(9,3);

	CRotation R(m_omc.Rodrigues(dRdom));
	double	R00=R(0,0),R01=R(0,1),R02=R(0,2),
			R10=R(1,0),R11=R(1,1),R12=R(1,2),
			R20=R(2,0),R21=R(2,1),R22=R(2,2);
	double tx=m_Tc(0),ty=m_Tc(1),tz=m_Tc(2);	

	H3_ARRAY2D_FLT64 Y(3,nbPt);
	double x,y,z;
	double *px0,*py0,*pz0,*px1,*py1,*pz1;

	px0=X.GetLine(0);
	py0=X.GetLine(1);
	pz0=X.GetLine(2);

	px1=Y.GetLine(0);
	py1=Y.GetLine(1);
	pz1=Y.GetLine(2);

	for(i=0;i<nbPt;i++)
	{
		x=(*(px0++));
		y=(*(py0++));
		z=(*(pz0++));
		(*(px1++))=R00*x+R01*y+R02*z +tx;
		(*(py1++))=R10*x+R11*y+R12*z +ty;
		(*(pz1++))=R20*x+R21*y+R22*z +tz;
	}

	if((p_dYdom!=nullptr)&&(p_dYdT!=nullptr))
	{

		Matrix dydR(3*nbPt,9);
		dydR.Fill(0);
		p_dYdT->Fill(0);

		for(j=0;j<nbPt;j++)
		{
			i0=3*j;
			i1=i0+1;
			i2=i1+1;
			dydR(i0,0)=dydR(i1,1)=dydR(i2,2)=X(0,j);
			dydR(i0,3)=dydR(i1,4)=dydR(i2,5)=X(1,j);
			dydR(i0,6)=dydR(i1,7)=dydR(i2,8)=X(2,j);
			
			(*p_dYdT)(i0,0)=(*p_dYdT)(i1,1)=(*p_dYdT)(i2,2)=1;
		}
		(*p_dYdom) = dydR*(dRdom);
	}
	return Y;
}

H3_ARRAY2D_FLT32 CExtrinsic_param::rigid_motion(const H3_ARRAY2D_FLT32& X, H3_ARRAY2D_FLT32 *p_dYdom, H3_ARRAY2D_FLT32 *p_dYdT)
{
	//p_dYdom pointe sur un tableau 3*(*pX).GetCo() lignes * 3 colonnes
	//p_dYdT  pointe sur un tableau 3*(*pX).GetCo() lignes * 3 colonnes
	long nbPt=X.GetCo(),i,j,i0,i1,i2;

	//verifications:
	if(	((*p_dYdom).GetLi()!=3*nbPt)|| ((*p_dYdT).GetLi()!=3*nbPt) || 
		((*p_dYdom).GetCo()!=3)		|| ((*p_dYdT).GetCo()!=3) ) return false;

	H3_MATRIX_FLT64 dRdom(9,3);

	CRotation R(m_omc.Rodrigues(dRdom));
	float	R00=R(0,0),R01=R(0,1),R02=R(0,2),
			R10=R(1,0),R11=R(1,1),R12=R(1,2),
			R20=R(2,0),R21=R(2,1),R22=R(2,2);
	float tx=m_Tc(0),ty=m_Tc(1),tz=m_Tc(2);

	H3_ARRAY2D_FLT32 Y(3,nbPt);//Y=R*X+T
	float x,y,z;
	float *px0,*py0,*pz0,*px1,*py1,*pz1;

	px0=X.GetLine(0);
	py0=X.GetLine(1);
	pz0=X.GetLine(2);

	px1=Y.GetLine(0);
	py1=Y.GetLine(1);
	pz1=Y.GetLine(2);

	for(i=0;i<nbPt;i++)
	{
		x=(*(px0++));
		y=(*(py0++));
		z=(*(pz0++));
		(*(px1++))=R00*x+R01*y+R02*z +tx;
		(*(py1++))=R10*x+R11*y+R12*z +ty;
		(*(pz1++))=R20*x+R21*y+R22*z +tz;
	}

	if((p_dYdom!=nullptr)&&(p_dYdT!=nullptr))
	{

		Matrix dydR(3*nbPt,9);
		dydR.Fill(0);
		p_dYdT->Fill(0);

		for(j=0;j<nbPt;j++)
		{
			i0=3*j;
			i1=i0+1;
			i2=i1+1;
			dydR(i0,0)=dydR(i1,1)=dydR(i2,2)=X(0,j);
			dydR(i0,3)=dydR(i1,4)=dydR(i2,5)=X(1,j);
			dydR(i0,6)=dydR(i1,7)=dydR(i2,8)=X(2,j);
			
			(*p_dYdT)(i0,0)=(*p_dYdT)(i1,1)=(*p_dYdT)(i2,2)=1;
		}
		(*p_dYdom) = dydR*(dRdom);
	}
	return Y;
}
//////////////////////////////////////////////////////////
//projectPoints2
//auteur cv d'apres fct homonyme pour matlab
//mars 04
//	pMetrique_IN	3;NbPts
//	p_dxpdom		2*NbPts;3
//	p_dxpdT			2*NbPts;3
//	p_dxpdk			2*NbPts;5
//	p_dxpdalpha		2*NbPts;1
//	p_dxpdf			2*NbPts;2
//	p_dxpdc			2*NbPts;2			
//
//////////////////////////////////////////////////////////
bool CExtrinsic_param::projectPoints2(const H3_ARRAY2D_FLT64& Metrique_IN,
									  const CH3Camera & Cam_IN,
									  H3_ARRAY2D_FLT64& Pix_OUT,
									  H3_ARRAY2D_FLT64* p_dxpdom,
									  H3_ARRAY2D_FLT64* p_dxpdT,
									  H3_ARRAY2D_FLT64* p_dxpdf,
									  H3_ARRAY2D_FLT64* p_dxpdc,
									  H3_ARRAY2D_FLT64* p_dxpdk,
									  H3_ARRAY2D_FLT64* p_dxpdalpha)
{
	CString strFunction("projectPoints2 (Flt64)");

	long nbPt=Metrique_IN.GetCo(),i,j,k_0,k_1,k_2,ii0,ii1;
	bool is_dxpdom=(p_dxpdom !=nullptr);
	bool is_dxpdT= (p_dxpdT !=nullptr);
	bool is_dxpdk= (p_dxpdk !=nullptr);
	bool is_dxpdalpha=(p_dxpdalpha !=nullptr);
	bool is_dxpdf= (p_dxpdf !=nullptr);
	bool is_dxpdc= (p_dxpdc !=nullptr);

	//verification
	if( (Pix_OUT.GetCo()!=nbPt) ||(Pix_OUT.GetLi()!=2) )
		Pix_OUT.ReAlloc(2,nbPt);
	if(	(Metrique_IN.GetLi()!=3) )
	{
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error1");
		#endif
		return false;
	}
	if (is_dxpdom)
		if(((*p_dxpdom).GetLi()!=2*nbPt) || ((*p_dxpdom).GetCo()!=3)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error2");
		#endif
		return false;
	}
	if (is_dxpdT)
		if(((*p_dxpdT).GetLi()!=2*nbPt) || ((*p_dxpdT).GetCo()!=3)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error3");
		#endif
		return false;
	}
	if (is_dxpdk)
		if(((*p_dxpdk).GetLi()!=2*nbPt) || ((*p_dxpdk).GetCo()!=5)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error4");
		#endif
		return false;
	}
	if (p_dxpdalpha !=nullptr)
		if(((*p_dxpdalpha).GetLi()!=2*nbPt) || ((*p_dxpdalpha).GetCo()!=1)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error5");
		#endif
		return false;
	}
	if (is_dxpdf)
		if(((*p_dxpdf).GetLi()!=2*nbPt) || ((*p_dxpdf).GetCo()!=2)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error6");
		#endif
		return false;
	}
	if (is_dxpdc)
		if(((*p_dxpdc).GetLi()!=2*nbPt) || ((*p_dxpdc).GetCo()!=2)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error7");
		#endif
		return false;
	}
	
	H3_ARRAY2D_FLT64 dYdom(3*nbPt,3), dYdT(3*nbPt,3);

	H3_ARRAY_FLT64 dcdistdom(3),dcdistdT(3),dcdistdk(5);
	double dxdom_0_0,dxdom_0_1,dxdom_0_2,dxdom_1_0,dxdom_1_1,dxdom_1_2;
	double dxdT_0_0, dxdT_0_1, dxdT_0_2, dxdT_1_0, dxdT_1_1, dxdT_1_2;
	double dr2dT_0,dr2dT_1,dr2dT_2;
	double dr4dT_0,dr4dT_1,dr4dT_2;
	double dr6dT_0,dr6dT_1,dr6dT_2;
	double dr2dom_0,dr2dom_1,dr2dom_2;
	double dr4dom_0,dr4dom_1,dr4dom_2;
	double dr6dom_0,dr6dom_1,dr6dom_2;
	double dcdistdom_0,dcdistdom_1,dcdistdom_2;
	double dcdistdT_0,dcdistdT_1,dcdistdT_2;
	double dcdistdk_0,dcdistdk_1,dcdistdk_2=0,dcdistdk_3=0,dcdistdk_4;

	double x,y,iz,x2,y2;
	double r2,r4,r6,cdist;
	double xd1,yd1,xd2,yd2;
	double a1,a2,a3,delta_x,delta_y;
	double aa,bb,cc;

	double dxd1dom_0_0,dxd1dom_0_1,dxd1dom_0_2,dxd1dom_1_0,dxd1dom_1_1,dxd1dom_1_2;
	double dxd1dT_0_0, dxd1dT_0_1, dxd1dT_0_2, dxd1dT_1_0, dxd1dT_1_1, dxd1dT_1_2;
	double dxd2dom_0_0,dxd2dom_0_1,dxd2dom_0_2,dxd2dom_1_0,dxd2dom_1_1,dxd2dom_1_2;
	double dxd2dT_0_0, dxd2dT_0_1, dxd2dT_0_2, dxd2dT_1_0, dxd2dT_1_1, dxd2dT_1_2;
	double dxd3dom_0_0,dxd3dom_0_1,dxd3dom_0_2;
	double dxd3dT_0_0, dxd3dT_0_1, dxd3dT_0_2;

	double	dxd1dk_0_0,dxd1dk_0_1,dxd1dk_0_2=0,dxd1dk_0_3=0,dxd1dk_0_4,
			dxd1dk_1_0,dxd1dk_1_1,dxd1dk_1_2=0,dxd1dk_1_3=0,dxd1dk_1_4;
	double	dxd2dk_0_0,dxd2dk_0_1,dxd2dk_0_2=0,dxd2dk_0_3=0,dxd2dk_0_4,
			dxd2dk_1_0,dxd2dk_1_1,dxd2dk_1_2=0,dxd2dk_1_3=0,dxd2dk_1_4;
	double	dxd3dk_0_0,dxd3dk_0_1,dxd3dk_0_2=0,dxd3dk_0_3=0,dxd3dk_0_4;
	double  dxd3dalpha_0,dxd3dalpha_1;
	double  ddelta_xdom_0_0,ddelta_xdom_0_1,ddelta_xdom_0_2,ddelta_xdom_1_0,ddelta_xdom_1_1,ddelta_xdom_1_2;
	double  ddelta_xdT_0_0, ddelta_xdT_0_1, ddelta_xdT_0_2, ddelta_xdT_1_0, ddelta_xdT_1_1, ddelta_xdT_1_2;
	double	ddelta_xdk_0_0,ddelta_xdk_0_1,ddelta_xdk_0_2,ddelta_xdk_0_3,ddelta_xdk_0_4,
			ddelta_xdk_1_0,ddelta_xdk_1_1,ddelta_xdk_1_2,ddelta_xdk_1_3,ddelta_xdk_1_4;
	//premiere initialisation
	H3_ARRAY2D_FLT64 Y=rigid_motion(Metrique_IN,&dYdom,&dYdT);
	
	//parametres camera
	double k1=Cam_IN.kc[0],k2=Cam_IN.kc[1],k3=Cam_IN.kc[2],k4=Cam_IN.kc[3],k5=Cam_IN.kc[4];
	double alpha=Cam_IN.alpha_c;
	double c_0=Cam_IN.cc[0], c_1=Cam_IN.cc[1];
	double fc_0=Cam_IN.fc[0],fc_1=Cam_IN.fc[1];

	bool is_k3ork4=Cam_IN.m_is_dist[3]||Cam_IN.m_is_dist[4];
	bool is_alpha=Cam_IN.mb_is_alpha;

	for(i=0;i<nbPt;i++)
	{
		j=0;
		k_0=3*i; k_1=k_0+1;k_2=k_0+2;
		iz=1.0/Y(2,i);
		x=Y(0,i)*iz;
		y=Y(1,i)*iz;
		x2=x*x;
		y2=y*y;
		bb=-x*iz;
		cc=-y*iz;

		r2=x2+y2;
		r4=r2*r2;
		r6=r2*r4;

		{	//le minimum: calcul de la projection pixel
			cdist=((r2*k5+k2)*r2+k1)*r2+1;
			xd1=x*cdist;
			yd1=y*cdist;

			if(is_k3ork4)
			{
				a1=2*x*y;
				a2=r2+2*x2;
				a3=r2+2*y2;

				delta_x=k3*a1+k4*a2;
				delta_y=k3*a3+k4*a1;

				xd2=xd1+delta_x;
				yd2=yd1+delta_y;
			}
			else
			{
				xd2=xd1;
				yd2=yd1;
			}
			if(is_alpha)
			{	//Add Skew
				Pix_OUT(0,i)=(xd2+alpha*yd2)*fc_0+c_0;
				Pix_OUT(1,i)=yd2*fc_1+c_1;
			}
			else{
				Pix_OUT(0,i)=xd2*fc_0+c_0;
				Pix_OUT(1,i)=yd2*fc_1+c_1;
			}
		}	//fin du minimum

		{
			dxdom_0_0=iz*dYdom(k_0,0)+bb*dYdom(k_2,0);
			dxdom_0_1=iz*dYdom(k_0,1)+bb*dYdom(k_2,1);
			dxdom_0_2=iz*dYdom(k_0,2)+bb*dYdom(k_2,2);
			dxdom_1_0=iz*dYdom(k_1,0)+cc*dYdom(k_2,0);
			dxdom_1_1=iz*dYdom(k_1,1)+cc*dYdom(k_2,1);
			dxdom_1_2=iz*dYdom(k_1,2)+cc*dYdom(k_2,2);

			dxdT_0_0=iz*dYdT(k_0,0)+bb*dYdT(k_2,0);
			dxdT_0_1=iz*dYdT(k_0,1)+bb*dYdT(k_2,1);
			dxdT_0_2=iz*dYdT(k_0,2)+bb*dYdT(k_2,2);
			dxdT_1_0=iz*dYdT(k_1,0)+cc*dYdT(k_2,0);
			dxdT_1_1=iz*dYdT(k_1,1)+cc*dYdT(k_2,1);
			dxdT_1_2=iz*dYdT(k_1,2)+cc*dYdT(k_2,2);
		}		
	
		{
			dr2dom_0=2*(x*dxdom_0_0+y*dxdom_1_0);
			dr2dom_1=2*(x*dxdom_0_1+y*dxdom_1_1);
			dr2dom_2=2*(x*dxdom_0_2+y*dxdom_1_2);

			dr2dT_0=2*(x*dxdT_0_0+y*dxdT_1_0);
			dr2dT_1=2*(x*dxdT_0_1+y*dxdT_1_1);
			dr2dT_2=2*(x*dxdT_0_2+y*dxdT_1_2);

			dr4dom_0=dr2dom_0*(r2*2);dr4dom_1=dr2dom_1*(r2*2);dr4dom_2=dr2dom_2*(r2*2);
			dr4dT_0= dr2dT_0* (r2*2);dr4dT_1= dr2dT_1* (r2*2);dr4dT_2= dr2dT_2* (r2*2);

			dr6dom_0=dr4dom_0*(r2*(1.5));dr6dom_1=dr4dom_1*(r2*(1.5));dr6dom_2=dr4dom_2*(r2*(1.5));
			dr6dT_0= dr4dT_0* (r2*(1.5));dr6dT_1= dr4dT_1* (r2*(1.5));dr6dT_2= dr4dT_2* (r2*(1.5));
		}
		
		{			
			dcdistdom_0= dr2dom_0*k1+dr4dom_0*k2+dr6dom_0*k5;
			dcdistdom_1= dr2dom_1*k1+dr4dom_1*k2+dr6dom_1*k5;
			dcdistdom_2= dr2dom_2*k1+dr4dom_2*k2+dr6dom_2*k5;
			
			dcdistdT_0= dr2dT_0*k1+dr4dT_0*k2+dr6dT_0*k5;
			dcdistdT_1= dr2dT_1*k1+dr4dT_1*k2+dr6dT_1*k5;
			dcdistdT_2= dr2dT_2*k1+dr4dT_2*k2+dr6dT_2*k5;

			dcdistdk_0=r2;
			dcdistdk_1=r4;
			dcdistdk_4=r6;			
		}

		{				
			{
			dxd1dom_0_0=x*dcdistdom_0+dxdom_0_0*cdist;
			dxd1dom_0_1=x*dcdistdom_1+dxdom_0_1*cdist;
			dxd1dom_0_2=x*dcdistdom_2+dxdom_0_2*cdist;

			dxd1dom_1_0=y*dcdistdom_0+dxdom_1_0*cdist;
			dxd1dom_1_1=y*dcdistdom_1+dxdom_1_1*cdist;
			dxd1dom_1_2=y*dcdistdom_2+dxdom_1_2*cdist;
			}

			{
			dxd1dT_0_0=x*dcdistdT_0 + dxdT_0_0*cdist;
			dxd1dT_0_1=x*dcdistdT_1 + dxdT_0_1*cdist;
			dxd1dT_0_2=x*dcdistdT_2 + dxdT_0_2*cdist;

			dxd1dT_1_0=y*dcdistdT_0 + dxdT_1_0*cdist;
			dxd1dT_1_1=y*dcdistdT_1 + dxdT_1_1*cdist;
			dxd1dT_1_2=y*dcdistdT_2 + dxdT_1_2*cdist;
			}
			
			if(is_dxpdk || (!is_k3ork4))
			{
			dxd1dk_0_0 = x*dcdistdk_0;
			dxd1dk_0_1 = x*dcdistdk_1;
			dxd1dk_0_4 = x*dcdistdk_4;

			dxd1dk_1_0 = y*dcdistdk_0;
			dxd1dk_1_1 = y*dcdistdk_1;
			dxd1dk_1_4 = y*dcdistdk_4;
			}
		}

		if(is_k3ork4)
		{

			aa=2*k3*y+6*k4*x;
			bb=2*(k3*x+k4*y);
			cc=6*k3*y+2*k4*x;

			ddelta_xdom_0_0=aa*dxdom_0_0+bb*dxdom_1_0;
			ddelta_xdom_0_1=aa*dxdom_0_1+bb*dxdom_1_1;
			ddelta_xdom_0_2=aa*dxdom_0_2+bb*dxdom_1_2;

			ddelta_xdom_1_0=bb*dxdom_0_0+cc*dxdom_1_0;
			ddelta_xdom_1_1=bb*dxdom_0_1+cc*dxdom_1_1;
			ddelta_xdom_1_2=bb*dxdom_0_2+cc*dxdom_1_2;

			ddelta_xdT_0_0=aa*dxdT_0_0+bb*dxdT_1_0;
			ddelta_xdT_0_1=aa*dxdT_0_1+bb*dxdT_1_1;
			ddelta_xdT_0_2=aa*dxdT_0_2+bb*dxdT_1_2;

			ddelta_xdT_1_0=bb*dxdT_0_0+cc*dxdT_1_0;
			ddelta_xdT_1_1=bb*dxdT_0_1+cc*dxdT_1_1;
			ddelta_xdT_1_2=bb*dxdT_0_2+cc*dxdT_1_2;

			dxd2dom_0_0=dxd1dom_0_0+ddelta_xdom_0_0;
			dxd2dom_0_1=dxd1dom_0_1+ddelta_xdom_0_1;
			dxd2dom_0_2=dxd1dom_0_2+ddelta_xdom_0_2;
			dxd2dom_1_0=dxd1dom_1_0+ddelta_xdom_1_0;
			dxd2dom_1_1=dxd1dom_1_1+ddelta_xdom_1_1;
			dxd2dom_1_2=dxd1dom_1_2+ddelta_xdom_1_2;

			dxd2dT_0_0=dxd1dT_0_0+ddelta_xdT_0_0;
			dxd2dT_0_1=dxd1dT_0_1+ddelta_xdT_0_1;
			dxd2dT_0_2=dxd1dT_0_2+ddelta_xdT_0_2;
			dxd2dT_1_0=dxd1dT_1_0+ddelta_xdT_1_0;
			dxd2dT_1_1=dxd1dT_1_1+ddelta_xdT_1_1;
			dxd2dT_1_2=dxd1dT_1_2+ddelta_xdT_1_2;

			if(is_dxpdk)
			{

			ddelta_xdk_0_0=ddelta_xdk_0_1=0; ddelta_xdk_0_2=a1; ddelta_xdk_0_3=a2; ddelta_xdk_0_4=0; 
			ddelta_xdk_1_0=ddelta_xdk_1_1=0; ddelta_xdk_1_2=a3; ddelta_xdk_1_3=a1; ddelta_xdk_1_4=0; 

			dxd2dk_0_0=dxd1dk_0_0+ddelta_xdk_0_0;
			dxd2dk_0_1=dxd1dk_0_1+ddelta_xdk_0_1;
			dxd2dk_0_2=dxd1dk_0_2+ddelta_xdk_0_2;
			dxd2dk_0_3=dxd1dk_0_3+ddelta_xdk_0_3;
			dxd2dk_0_4=dxd1dk_0_4+ddelta_xdk_0_4;
			dxd2dk_1_0=dxd1dk_1_0+ddelta_xdk_1_0;
			dxd2dk_1_1=dxd1dk_1_1+ddelta_xdk_1_1;
			dxd2dk_1_2=dxd1dk_1_2+ddelta_xdk_1_2;
			dxd2dk_1_3=dxd1dk_1_3+ddelta_xdk_1_3;
			dxd2dk_1_4=dxd1dk_1_4+ddelta_xdk_1_4;
			}
		}
		else
		{
			delta_x=delta_y=0;

			ddelta_xdom_0_0=ddelta_xdom_0_1=ddelta_xdom_0_2=
			ddelta_xdom_1_0=ddelta_xdom_1_1=ddelta_xdom_1_2=0;

			ddelta_xdT_0_0=ddelta_xdT_0_1=ddelta_xdT_0_2=
			ddelta_xdT_1_0=ddelta_xdT_1_1=ddelta_xdT_1_2=0;

			if(is_dxpdk)
			{
			ddelta_xdk_0_0=ddelta_xdk_0_1=ddelta_xdk_0_2=ddelta_xdk_0_3=ddelta_xdk_0_4=0; 
			ddelta_xdk_1_0=ddelta_xdk_1_1=ddelta_xdk_1_2=ddelta_xdk_1_3=ddelta_xdk_1_4=0; 
			}

			dxd2dom_0_0=dxd1dom_0_0;
			dxd2dom_0_1=dxd1dom_0_1;
			dxd2dom_0_2=dxd1dom_0_2;
			dxd2dom_1_0=dxd1dom_1_0;
			dxd2dom_1_1=dxd1dom_1_1;
			dxd2dom_1_2=dxd1dom_1_2;

			dxd2dT_0_0=dxd1dT_0_0;
			dxd2dT_0_1=dxd1dT_0_1;
			dxd2dT_0_2=dxd1dT_0_2;
			dxd2dT_1_0=dxd1dT_1_0;
			dxd2dT_1_1=dxd1dT_1_1;
			dxd2dT_1_2=dxd1dT_1_2;

			dxd2dk_0_0=dxd1dk_0_0;
			dxd2dk_0_1=dxd1dk_0_1;
			dxd2dk_0_2=dxd1dk_0_2;
			dxd2dk_0_3=dxd1dk_0_3;
			dxd2dk_0_4=dxd1dk_0_4;
			dxd2dk_1_0=dxd1dk_1_0;
			dxd2dk_1_1=dxd1dk_1_1;
			dxd2dk_1_2=dxd1dk_1_2;
			dxd2dk_1_3=dxd1dk_1_3;
			dxd2dk_1_4=dxd1dk_1_4;
		}

		if(is_alpha)
		{	//Add Skew
			dxd3dom_0_0=dxd2dom_0_0 + alpha* dxd2dom_1_0;
			dxd3dom_0_1=dxd2dom_0_1 + alpha* dxd2dom_1_1;
			dxd3dom_0_2=dxd2dom_0_2 + alpha* dxd2dom_1_2;

			dxd3dT_0_0=dxd2dT_0_0 + alpha* dxd2dT_1_0;
			dxd3dT_0_1=dxd2dT_0_1 + alpha* dxd2dT_1_1;
			dxd3dT_0_2=dxd2dT_0_2 + alpha* dxd2dT_1_2;

			if(is_dxpdk)
			{
			dxd3dk_0_0=dxd2dk_0_0 + alpha* dxd2dk_1_0;
			dxd3dk_0_1=dxd2dk_0_1 + alpha* dxd2dk_1_1;
			dxd3dk_0_2=dxd2dk_0_2 + alpha* dxd2dk_1_2;
			dxd3dk_0_3=dxd2dk_0_3 + alpha* dxd2dk_1_3;
			dxd3dk_0_4=dxd2dk_0_4 + alpha* dxd2dk_1_4;
			}

			dxd3dalpha_0=yd2;
			dxd3dalpha_1=0;
		}
		else
		{
			dxd3dom_0_0=dxd2dom_0_0 ;
			dxd3dom_0_1=dxd2dom_0_1 ;
			dxd3dom_0_2=dxd2dom_0_2 ;

			dxd3dT_0_0=dxd2dT_0_0 ;
			dxd3dT_0_1=dxd2dT_0_1 ;
			dxd3dT_0_2=dxd2dT_0_2 ;

			if(is_dxpdk)
			{
			dxd3dk_0_0=dxd2dk_0_0 ;
			dxd3dk_0_1=dxd2dk_0_1 ;
			dxd3dk_0_2=dxd2dk_0_2 ;
			dxd3dk_0_3=dxd2dk_0_3 ;
			dxd3dk_0_4=dxd2dk_0_4 ;

			}

			dxd3dalpha_0=yd2;
			dxd3dalpha_1=0;
		}

		{	//elements retournés par la fct
			ii0=2*i;
			ii1=ii0+1;
			
			if(is_dxpdom)
			{
				(*p_dxpdom)(ii0,0)=fc_0*dxd3dom_0_0;
				(*p_dxpdom)(ii0,1)=fc_0*dxd3dom_0_1;
				(*p_dxpdom)(ii0,2)=fc_0*dxd3dom_0_2;
				(*p_dxpdom)(ii1,0)=fc_1*dxd2dom_1_0;
				(*p_dxpdom)(ii1,1)=fc_1*dxd2dom_1_1;
				(*p_dxpdom)(ii1,2)=fc_1*dxd2dom_1_2;
			}
			if(is_dxpdT)
			{
				(*p_dxpdT)(ii0,0)=fc_0*dxd3dT_0_0;
				(*p_dxpdT)(ii0,1)=fc_0*dxd3dT_0_1;
				(*p_dxpdT)(ii0,2)=fc_0*dxd3dT_0_2;
				(*p_dxpdT)(ii1,0)=fc_1*dxd2dT_1_0;
				(*p_dxpdT)(ii1,1)=fc_1*dxd2dT_1_1;
				(*p_dxpdT)(ii1,2)=fc_1*dxd2dT_1_2;
			}
			if(is_dxpdk)
			{
				(*p_dxpdk)(ii0,0)=fc_0*dxd3dk_0_0;
				(*p_dxpdk)(ii0,1)=fc_0*dxd3dk_0_1;
				(*p_dxpdk)(ii0,2)=fc_0*dxd3dk_0_2;
				(*p_dxpdk)(ii0,3)=fc_0*dxd3dk_0_3;
				(*p_dxpdk)(ii0,4)=fc_0*dxd3dk_0_4;
				
				(*p_dxpdk)(ii1,0)=fc_1*dxd2dk_1_0;
				(*p_dxpdk)(ii1,1)=fc_1*dxd2dk_1_1;
				(*p_dxpdk)(ii1,2)=fc_1*dxd2dk_1_2;
				(*p_dxpdk)(ii1,3)=fc_1*dxd2dk_1_3;
				(*p_dxpdk)(ii1,4)=fc_1*dxd2dk_1_4;
			}
			if(is_dxpdalpha)
			{
				(*p_dxpdalpha)(ii0)=fc_0*dxd3dalpha_0;
				(*p_dxpdalpha)(ii1)=fc_1*dxd3dalpha_1;
			}
		}
	}

	if(is_dxpdf)
	{
		for(i=0,j=0;i<nbPt;i++)
		{		
			(*p_dxpdf)(j,0)=(Pix_OUT(0,i)-c_0)/fc_0;  (*p_dxpdf)(j,1)=0;
			j++;
			(*p_dxpdf)(j,0)=0;				(*p_dxpdf)(j,1)=(Pix_OUT(1,i)-c_1)/fc_1;
			j++;
		}
	}

	if(is_dxpdc)
	{
		for(i=0,j=0;i<nbPt;i++)
		{
			(*p_dxpdc)(j++)=1;	(*p_dxpdc)(j++)=0;
			(*p_dxpdc)(j++)=0;	(*p_dxpdc)(j++)=1;
		}
	}

	return true;
}

/*! 
* 	\fn      bool CExtrinsic_param::projectPoints2(const H3_ARRAY2D_FLT32& Metrique_IN,
									  const CH3Camera & Cam_IN,
									  H3_ARRAY2D_FLT32& Pix_OUT,
									  H3_ARRAY2D_FLT32* p_dxpdom,
									  H3_ARRAY2D_FLT32* p_dxpdT,
									  H3_ARRAY2D_FLT32* p_dxpdf,
									  H3_ARRAY2D_FLT32* p_dxpdc,
									  H3_ARRAY2D_FLT32* p_dxpdk,
									  H3_ARRAY2D_FLT32* p_dxpdalpha)
* 	\author  S Jaminion
* 	\brief   projection de coordonnées métriques dans le plan retinien normalisé parfait
* 	\param   const H3_ARRAY2D_FLT32& Metrique_IN : coordonnées metriques dans un repere monde
* 	\param   const CH3Camera & Cam_IN : Structure contenant les parametres intrinseques de la cam
* 	\param   H3_ARRAY2D_FLT32& Pix_OUT : coord pixel normalise parfait
* 	\param   H3_ARRAY2D_FLT32* p_dxpdom : 
* 	\param   H3_ARRAY2D_FLT32* p_dxpdT : 
* 	\param   H3_ARRAY2D_FLT32* p_dxpdf : 
* 	\param   H3_ARRAY2D_FLT32* p_dxpdc : 
* 	\param   H3_ARRAY2D_FLT32* p_dxpdk : 
* 	\param   H3_ARRAY2D_FLT32* p_dxpdalpha : 
* 	\return  bool
* 	\remarks 
*/ 

bool CExtrinsic_param::projectPoints2(const H3_ARRAY2D_FLT32& Metrique_IN,
									  const CH3Camera & Cam_IN,
									  H3_ARRAY2D_FLT32& Pix_OUT,
									  H3_ARRAY2D_FLT32* p_dxpdom,
									  H3_ARRAY2D_FLT32* p_dxpdT,
									  H3_ARRAY2D_FLT32* p_dxpdf,
									  H3_ARRAY2D_FLT32* p_dxpdc,
									  H3_ARRAY2D_FLT32* p_dxpdk,
									  H3_ARRAY2D_FLT32* p_dxpdalpha)
{
	CString strFunction("projectPoints2 (Flt32)");

	long nbPt=Metrique_IN.GetCo(),i,j,k_0,k_1,k_2,ii0,ii1;
	bool is_dxpdom=(p_dxpdom !=nullptr);
	bool is_dxpdT= (p_dxpdT !=nullptr);
	bool is_dxpdk= (p_dxpdk !=nullptr);
	bool is_dxpdalpha=(p_dxpdalpha !=nullptr);
	bool is_dxpdf= (p_dxpdf !=nullptr);
	bool is_dxpdc= (p_dxpdc !=nullptr);

	//verification
	if( (Pix_OUT.GetCo()!=nbPt) ||(Pix_OUT.GetLi()!=2) )
		Pix_OUT.ReAlloc(2,nbPt);
	if(	(Metrique_IN.GetLi()!=3) ){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error1");
		#endif
		return false;
	}
	if (is_dxpdom)
		if(((*p_dxpdom).GetLi()!=2*nbPt) || ((*p_dxpdom).GetCo()!=3)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error2");
		#endif
		return false;
	}
	if (is_dxpdT)
		if(((*p_dxpdT).GetLi()!=2*nbPt) || ((*p_dxpdT).GetCo()!=3)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error3");
		#endif
		return false;
	}
	if (is_dxpdk)
		if(((*p_dxpdk).GetLi()!=2*nbPt) || ((*p_dxpdk).GetCo()!=5)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error4");
		#endif
		return false;
	}
	if (p_dxpdalpha !=nullptr)
		if(((*p_dxpdalpha).GetLi()!=2*nbPt) || ((*p_dxpdalpha).GetCo()!=1)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error5");
		#endif
		return false;
	}
	if (is_dxpdf)
		if(((*p_dxpdf).GetLi()!=2*nbPt) || ((*p_dxpdf).GetCo()!=2)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error6");
		#endif
		return false;
	}
	if (is_dxpdc)
		if(((*p_dxpdc).GetLi()!=2*nbPt) || ((*p_dxpdc).GetCo()!=2)){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugWarning(strModule,strFunction,"error7");
		#endif
		return false;
	}
	
	H3_ARRAY2D_FLT32 dYdom(3*nbPt,3), dYdT(3*nbPt,3);

	H3_ARRAY_FLT32 dcdistdom(3),dcdistdT(3),dcdistdk(5);
	float dxdom_0_0,dxdom_0_1,dxdom_0_2,dxdom_1_0,dxdom_1_1,dxdom_1_2;
	float dxdT_0_0, dxdT_0_1, dxdT_0_2, dxdT_1_0, dxdT_1_1, dxdT_1_2;
	float dr2dT_0,dr2dT_1,dr2dT_2;
	float dr4dT_0,dr4dT_1,dr4dT_2;
	float dr6dT_0,dr6dT_1,dr6dT_2;
	float dr2dom_0,dr2dom_1,dr2dom_2;
	float dr4dom_0,dr4dom_1,dr4dom_2;
	float dr6dom_0,dr6dom_1,dr6dom_2;
	float dcdistdom_0,dcdistdom_1,dcdistdom_2;
	float dcdistdT_0,dcdistdT_1,dcdistdT_2;
	float dcdistdk_0,dcdistdk_1,dcdistdk_2=0,dcdistdk_3=0,dcdistdk_4;

	float x,y,iz,x2,y2;
	float r2,r4,r6,cdist;
	float xd1,yd1,xd2,yd2;
	float a1,a2,a3,delta_x,delta_y;
	float aa,bb,cc;

	float dxd1dom_0_0,dxd1dom_0_1,dxd1dom_0_2,dxd1dom_1_0,dxd1dom_1_1,dxd1dom_1_2;
	float dxd1dT_0_0, dxd1dT_0_1, dxd1dT_0_2, dxd1dT_1_0, dxd1dT_1_1, dxd1dT_1_2;
	float dxd2dom_0_0,dxd2dom_0_1,dxd2dom_0_2,dxd2dom_1_0,dxd2dom_1_1,dxd2dom_1_2;
	float dxd2dT_0_0, dxd2dT_0_1, dxd2dT_0_2, dxd2dT_1_0, dxd2dT_1_1, dxd2dT_1_2;
	float dxd3dom_0_0,dxd3dom_0_1,dxd3dom_0_2;
	float dxd3dT_0_0, dxd3dT_0_1, dxd3dT_0_2;

	float	dxd1dk_0_0,dxd1dk_0_1,dxd1dk_0_2=0,dxd1dk_0_3=0,dxd1dk_0_4,
			dxd1dk_1_0,dxd1dk_1_1,dxd1dk_1_2=0,dxd1dk_1_3=0,dxd1dk_1_4;
	float	dxd2dk_0_0,dxd2dk_0_1,dxd2dk_0_2=0,dxd2dk_0_3=0,dxd2dk_0_4,
			dxd2dk_1_0,dxd2dk_1_1,dxd2dk_1_2=0,dxd2dk_1_3=0,dxd2dk_1_4;
	float	dxd3dk_0_0,dxd3dk_0_1,dxd3dk_0_2=0,dxd3dk_0_3=0,dxd3dk_0_4;
	float  dxd3dalpha_0,dxd3dalpha_1;
	float  ddelta_xdom_0_0,ddelta_xdom_0_1,ddelta_xdom_0_2,ddelta_xdom_1_0,ddelta_xdom_1_1,ddelta_xdom_1_2;
	float  ddelta_xdT_0_0, ddelta_xdT_0_1, ddelta_xdT_0_2, ddelta_xdT_1_0, ddelta_xdT_1_1, ddelta_xdT_1_2;
	float	ddelta_xdk_0_0,ddelta_xdk_0_1,ddelta_xdk_0_2,ddelta_xdk_0_3,ddelta_xdk_0_4,
			ddelta_xdk_1_0,ddelta_xdk_1_1,ddelta_xdk_1_2,ddelta_xdk_1_3,ddelta_xdk_1_4;
	//premiere initialisation
	H3_ARRAY2D_FLT32 Y=rigid_motion(Metrique_IN,&dYdom,&dYdT);
	
	//parametres camera
	float k1=Cam_IN.kc[0],k2=Cam_IN.kc[1],k3=Cam_IN.kc[2],k4=Cam_IN.kc[3],k5=Cam_IN.kc[4];
	float alpha=Cam_IN.alpha_c;
	float c_0=Cam_IN.cc[0], c_1=Cam_IN.cc[1];
	float fc_0=Cam_IN.fc[0],fc_1=Cam_IN.fc[1];

	bool is_k3ork4=Cam_IN.m_is_dist[3]||Cam_IN.m_is_dist[4];
	bool is_alpha=Cam_IN.mb_is_alpha;

	for(i=0;i<nbPt;i++)
	{
		j=0;
		k_0=3*i; k_1=k_0+1;k_2=k_0+2;
		iz=1.0/Y(2,i);
		x=Y(0,i)*iz;
		y=Y(1,i)*iz;
		x2=x*x;
		y2=y*y;
		bb=-x*iz;
		cc=-y*iz;

		r2=x2+y2;
		r4=r2*r2;
		r6=r2*r4;

		{	//le minimum: calcul de la projection pixel
			cdist=((r2*k5+k2)*r2+k1)*r2+1;
			xd1=x*cdist;
			yd1=y*cdist;

			if(is_k3ork4)
			{
				a1=2*x*y;
				a2=r2+2*x2;
				a3=r2+2*y2;

				delta_x=k3*a1+k4*a2;
				delta_y=k3*a3+k4*a1;

				xd2=xd1+delta_x;
				yd2=yd1+delta_y;
			}
			else
			{
				xd2=xd1;
				yd2=yd1;
			}
			if(is_alpha)
			{	//Add Skew
				Pix_OUT(0,i)=(xd2+alpha*yd2)*fc_0+c_0;
				Pix_OUT(1,i)=yd2*fc_1+c_1;
			}
			else
			{
				Pix_OUT(0,i)=xd2*fc_0+c_0;
				Pix_OUT(1,i)=yd2*fc_1+c_1;
			}
		}	//fin du minimum

		{
			dxdom_0_0=iz*dYdom(k_0,0)+bb*dYdom(k_2,0);
			dxdom_0_1=iz*dYdom(k_0,1)+bb*dYdom(k_2,1);
			dxdom_0_2=iz*dYdom(k_0,2)+bb*dYdom(k_2,2);
			dxdom_1_0=iz*dYdom(k_1,0)+cc*dYdom(k_2,0);
			dxdom_1_1=iz*dYdom(k_1,1)+cc*dYdom(k_2,1);
			dxdom_1_2=iz*dYdom(k_1,2)+cc*dYdom(k_2,2);

			dxdT_0_0=iz*dYdT(k_0,0)+bb*dYdT(k_2,0);
			dxdT_0_1=iz*dYdT(k_0,1)+bb*dYdT(k_2,1);
			dxdT_0_2=iz*dYdT(k_0,2)+bb*dYdT(k_2,2);
			dxdT_1_0=iz*dYdT(k_1,0)+cc*dYdT(k_2,0);
			dxdT_1_1=iz*dYdT(k_1,1)+cc*dYdT(k_2,1);
			dxdT_1_2=iz*dYdT(k_1,2)+cc*dYdT(k_2,2);
		}		
		
		{
			dr2dom_0=2*(x*dxdom_0_0+y*dxdom_1_0);
			dr2dom_1=2*(x*dxdom_0_1+y*dxdom_1_1);
			dr2dom_2=2*(x*dxdom_0_2+y*dxdom_1_2);

			dr2dT_0=2*(x*dxdT_0_0+y*dxdT_1_0);
			dr2dT_1=2*(x*dxdT_0_1+y*dxdT_1_1);
			dr2dT_2=2*(x*dxdT_0_2+y*dxdT_1_2);

			dr4dom_0=dr2dom_0*(r2*2);dr4dom_1=dr2dom_1*(r2*2);dr4dom_2=dr2dom_2*(r2*2);
			dr4dT_0= dr2dT_0* (r2*2);dr4dT_1= dr2dT_1* (r2*2);dr4dT_2= dr2dT_2* (r2*2);

			dr6dom_0=dr4dom_0*(r2*(1.5));dr6dom_1=dr4dom_1*(r2*(1.5));dr6dom_2=dr4dom_2*(r2*(1.5));
			dr6dT_0= dr4dT_0* (r2*(1.5));dr6dT_1= dr4dT_1* (r2*(1.5));dr6dT_2= dr4dT_2* (r2*(1.5));
		}
		
		{			
			dcdistdom_0= dr2dom_0*k1+dr4dom_0*k2+dr6dom_0*k5;
			dcdistdom_1= dr2dom_1*k1+dr4dom_1*k2+dr6dom_1*k5;
			dcdistdom_2= dr2dom_2*k1+dr4dom_2*k2+dr6dom_2*k5;
			
			dcdistdT_0= dr2dT_0*k1+dr4dT_0*k2+dr6dT_0*k5;
			dcdistdT_1= dr2dT_1*k1+dr4dT_1*k2+dr6dT_1*k5;
			dcdistdT_2= dr2dT_2*k1+dr4dT_2*k2+dr6dT_2*k5;

			dcdistdk_0=r2;
			dcdistdk_1=r4;
			dcdistdk_4=r6;			
		}

		{				
			{
			dxd1dom_0_0=x*dcdistdom_0+dxdom_0_0*cdist;
			dxd1dom_0_1=x*dcdistdom_1+dxdom_0_1*cdist;
			dxd1dom_0_2=x*dcdistdom_2+dxdom_0_2*cdist;

			dxd1dom_1_0=y*dcdistdom_0+dxdom_1_0*cdist;
			dxd1dom_1_1=y*dcdistdom_1+dxdom_1_1*cdist;
			dxd1dom_1_2=y*dcdistdom_2+dxdom_1_2*cdist;
			}

			{
			dxd1dT_0_0=x*dcdistdT_0 + dxdT_0_0*cdist;
			dxd1dT_0_1=x*dcdistdT_1 + dxdT_0_1*cdist;
			dxd1dT_0_2=x*dcdistdT_2 + dxdT_0_2*cdist;

			dxd1dT_1_0=y*dcdistdT_0 + dxdT_1_0*cdist;
			dxd1dT_1_1=y*dcdistdT_1 + dxdT_1_1*cdist;
			dxd1dT_1_2=y*dcdistdT_2 + dxdT_1_2*cdist;
			}
			
			if(is_dxpdk || (!is_k3ork4))
			{
				dxd1dk_0_0 = x*dcdistdk_0;
				dxd1dk_0_1 = x*dcdistdk_1;
				dxd1dk_0_4 = x*dcdistdk_4;

				dxd1dk_1_0 = y*dcdistdk_0;
				dxd1dk_1_1 = y*dcdistdk_1;
				dxd1dk_1_4 = y*dcdistdk_4;
			}
		}
		if(is_k3ork4)
		{

			aa=2*k3*y+6*k4*x;
			bb=2*(k3*x+k4*y);
			cc=6*k3*y+2*k4*x;

			ddelta_xdom_0_0=aa*dxdom_0_0+bb*dxdom_1_0;
			ddelta_xdom_0_1=aa*dxdom_0_1+bb*dxdom_1_1;
			ddelta_xdom_0_2=aa*dxdom_0_2+bb*dxdom_1_2;

			ddelta_xdom_1_0=bb*dxdom_0_0+cc*dxdom_1_0;
			ddelta_xdom_1_1=bb*dxdom_0_1+cc*dxdom_1_1;
			ddelta_xdom_1_2=bb*dxdom_0_2+cc*dxdom_1_2;

			ddelta_xdT_0_0=aa*dxdT_0_0+bb*dxdT_1_0;
			ddelta_xdT_0_1=aa*dxdT_0_1+bb*dxdT_1_1;
			ddelta_xdT_0_2=aa*dxdT_0_2+bb*dxdT_1_2;

			ddelta_xdT_1_0=bb*dxdT_0_0+cc*dxdT_1_0;
			ddelta_xdT_1_1=bb*dxdT_0_1+cc*dxdT_1_1;
			ddelta_xdT_1_2=bb*dxdT_0_2+cc*dxdT_1_2;

			dxd2dom_0_0=dxd1dom_0_0+ddelta_xdom_0_0;
			dxd2dom_0_1=dxd1dom_0_1+ddelta_xdom_0_1;
			dxd2dom_0_2=dxd1dom_0_2+ddelta_xdom_0_2;
			dxd2dom_1_0=dxd1dom_1_0+ddelta_xdom_1_0;
			dxd2dom_1_1=dxd1dom_1_1+ddelta_xdom_1_1;
			dxd2dom_1_2=dxd1dom_1_2+ddelta_xdom_1_2;

			dxd2dT_0_0=dxd1dT_0_0+ddelta_xdT_0_0;
			dxd2dT_0_1=dxd1dT_0_1+ddelta_xdT_0_1;
			dxd2dT_0_2=dxd1dT_0_2+ddelta_xdT_0_2;
			dxd2dT_1_0=dxd1dT_1_0+ddelta_xdT_1_0;
			dxd2dT_1_1=dxd1dT_1_1+ddelta_xdT_1_1;
			dxd2dT_1_2=dxd1dT_1_2+ddelta_xdT_1_2;

			if(is_dxpdk)
			{

				ddelta_xdk_0_0=ddelta_xdk_0_1=0; ddelta_xdk_0_2=a1; ddelta_xdk_0_3=a2; ddelta_xdk_0_4=0; 
				ddelta_xdk_1_0=ddelta_xdk_1_1=0; ddelta_xdk_1_2=a3; ddelta_xdk_1_3=a1; ddelta_xdk_1_4=0; 

				dxd2dk_0_0=dxd1dk_0_0+ddelta_xdk_0_0;
				dxd2dk_0_1=dxd1dk_0_1+ddelta_xdk_0_1;
				dxd2dk_0_2=dxd1dk_0_2+ddelta_xdk_0_2;
				dxd2dk_0_3=dxd1dk_0_3+ddelta_xdk_0_3;
				dxd2dk_0_4=dxd1dk_0_4+ddelta_xdk_0_4;
				dxd2dk_1_0=dxd1dk_1_0+ddelta_xdk_1_0;
				dxd2dk_1_1=dxd1dk_1_1+ddelta_xdk_1_1;
				dxd2dk_1_2=dxd1dk_1_2+ddelta_xdk_1_2;
				dxd2dk_1_3=dxd1dk_1_3+ddelta_xdk_1_3;
				dxd2dk_1_4=dxd1dk_1_4+ddelta_xdk_1_4;
			}
		}
		else
		{
			delta_x=delta_y=0;

			ddelta_xdom_0_0=ddelta_xdom_0_1=ddelta_xdom_0_2=
			ddelta_xdom_1_0=ddelta_xdom_1_1=ddelta_xdom_1_2=0;

			ddelta_xdT_0_0=ddelta_xdT_0_1=ddelta_xdT_0_2=
			ddelta_xdT_1_0=ddelta_xdT_1_1=ddelta_xdT_1_2=0;

			if(is_dxpdk)
			{
			ddelta_xdk_0_0=ddelta_xdk_0_1=ddelta_xdk_0_2=ddelta_xdk_0_3=ddelta_xdk_0_4=0; 
			ddelta_xdk_1_0=ddelta_xdk_1_1=ddelta_xdk_1_2=ddelta_xdk_1_3=ddelta_xdk_1_4=0; 
			}

			dxd2dom_0_0=dxd1dom_0_0;
			dxd2dom_0_1=dxd1dom_0_1;
			dxd2dom_0_2=dxd1dom_0_2;
			dxd2dom_1_0=dxd1dom_1_0;
			dxd2dom_1_1=dxd1dom_1_1;
			dxd2dom_1_2=dxd1dom_1_2;

			dxd2dT_0_0=dxd1dT_0_0;
			dxd2dT_0_1=dxd1dT_0_1;
			dxd2dT_0_2=dxd1dT_0_2;
			dxd2dT_1_0=dxd1dT_1_0;
			dxd2dT_1_1=dxd1dT_1_1;
			dxd2dT_1_2=dxd1dT_1_2;

			dxd2dk_0_0=dxd1dk_0_0;
			dxd2dk_0_1=dxd1dk_0_1;
			dxd2dk_0_2=dxd1dk_0_2;
			dxd2dk_0_3=dxd1dk_0_3;
			dxd2dk_0_4=dxd1dk_0_4;
			dxd2dk_1_0=dxd1dk_1_0;
			dxd2dk_1_1=dxd1dk_1_1;
			dxd2dk_1_2=dxd1dk_1_2;
			dxd2dk_1_3=dxd1dk_1_3;
			dxd2dk_1_4=dxd1dk_1_4;
		}

		if(is_alpha)
		{	//Add Skew

			dxd3dom_0_0=dxd2dom_0_0 + alpha* dxd2dom_1_0;
			dxd3dom_0_1=dxd2dom_0_1 + alpha* dxd2dom_1_1;
			dxd3dom_0_2=dxd2dom_0_2 + alpha* dxd2dom_1_2;

			dxd3dT_0_0=dxd2dT_0_0 + alpha* dxd2dT_1_0;
			dxd3dT_0_1=dxd2dT_0_1 + alpha* dxd2dT_1_1;
			dxd3dT_0_2=dxd2dT_0_2 + alpha* dxd2dT_1_2;

			if(is_dxpdk)
			{
				dxd3dk_0_0=dxd2dk_0_0 + alpha* dxd2dk_1_0;
				dxd3dk_0_1=dxd2dk_0_1 + alpha* dxd2dk_1_1;
				dxd3dk_0_2=dxd2dk_0_2 + alpha* dxd2dk_1_2;
				dxd3dk_0_3=dxd2dk_0_3 + alpha* dxd2dk_1_3;
				dxd3dk_0_4=dxd2dk_0_4 + alpha* dxd2dk_1_4;
			}

			dxd3dalpha_0=yd2;
			dxd3dalpha_1=0;
		}
		else
		{

			dxd3dom_0_0=dxd2dom_0_0 ;
			dxd3dom_0_1=dxd2dom_0_1 ;
			dxd3dom_0_2=dxd2dom_0_2 ;

			dxd3dT_0_0=dxd2dT_0_0 ;
			dxd3dT_0_1=dxd2dT_0_1 ;
			dxd3dT_0_2=dxd2dT_0_2 ;

			if(is_dxpdk)
			{
				dxd3dk_0_0=dxd2dk_0_0 ;
				dxd3dk_0_1=dxd2dk_0_1 ;
				dxd3dk_0_2=dxd2dk_0_2 ;
				dxd3dk_0_3=dxd2dk_0_3 ;
				dxd3dk_0_4=dxd2dk_0_4 ;
			}

			dxd3dalpha_0=yd2;
			dxd3dalpha_1=0;
		}

		{	//elements retournés par la fct
			ii0=2*i;
			ii1=ii0+1;
			
			if(is_dxpdom)
			{
				(*p_dxpdom)(ii0,0)=fc_0*dxd3dom_0_0;
				(*p_dxpdom)(ii0,1)=fc_0*dxd3dom_0_1;
				(*p_dxpdom)(ii0,2)=fc_0*dxd3dom_0_2;
				(*p_dxpdom)(ii1,0)=fc_1*dxd2dom_1_0;
				(*p_dxpdom)(ii1,1)=fc_1*dxd2dom_1_1;
				(*p_dxpdom)(ii1,2)=fc_1*dxd2dom_1_2;
			}
			if(is_dxpdT)
			{
				(*p_dxpdT)(ii0,0)=fc_0*dxd3dT_0_0;
				(*p_dxpdT)(ii0,1)=fc_0*dxd3dT_0_1;
				(*p_dxpdT)(ii0,2)=fc_0*dxd3dT_0_2;
				(*p_dxpdT)(ii1,0)=fc_1*dxd2dT_1_0;
				(*p_dxpdT)(ii1,1)=fc_1*dxd2dT_1_1;
				(*p_dxpdT)(ii1,2)=fc_1*dxd2dT_1_2;
			}
			if(is_dxpdk)
			{
				(*p_dxpdk)(ii0,0)=fc_0*dxd3dk_0_0;
				(*p_dxpdk)(ii0,1)=fc_0*dxd3dk_0_1;
				(*p_dxpdk)(ii0,2)=fc_0*dxd3dk_0_2;
				(*p_dxpdk)(ii0,3)=fc_0*dxd3dk_0_3;
				(*p_dxpdk)(ii0,4)=fc_0*dxd3dk_0_4;
				
				(*p_dxpdk)(ii1,0)=fc_1*dxd2dk_1_0;
				(*p_dxpdk)(ii1,1)=fc_1*dxd2dk_1_1;
				(*p_dxpdk)(ii1,2)=fc_1*dxd2dk_1_2;
				(*p_dxpdk)(ii1,3)=fc_1*dxd2dk_1_3;
				(*p_dxpdk)(ii1,4)=fc_1*dxd2dk_1_4;
			}
			if(is_dxpdalpha)
			{
				(*p_dxpdalpha)(ii0)=fc_0*dxd3dalpha_0;
				(*p_dxpdalpha)(ii1)=fc_1*dxd3dalpha_1;
			}
		}
	}

	if(is_dxpdf)
	{
		for(i=0,j=0;i<nbPt;i++)
		{		
			(*p_dxpdf)(j,0)=(Pix_OUT(0,i)-c_0)/fc_0;  (*p_dxpdf)(j,1)=0;
			j++;
			(*p_dxpdf)(j,0)=0;				(*p_dxpdf)(j,1)=(Pix_OUT(1,i)-c_1)/fc_1;
			j++;
		}
	}
	if(is_dxpdc)
	{
		for(i=0;i<nbPt;i++)
		{
			(*(p_dxpdc++))=1;	(*(p_dxpdc++))=0;
			(*(p_dxpdc++))=0;	(*(p_dxpdc++))=1;
		}
	}
	return true;
}

CExtrinsic_param CExtrinsic_param::operator =(const CExtrinsic_param& Src)
{
	if(this==&Src) return *this;

	m_omc=Src.m_omc;
	m_Rc=Src.m_Rc;
	m_Tc=Src.m_Tc;

	return *this;
}

CExtrinsic_param::CExtrinsic_param(const CExtrinsic_param& Src)
{
	if(this!=&Src){
		m_omc=Src.m_omc;
		m_Rc=Src.m_Rc;
		m_Tc=Src.m_Tc;
	}
}

void CExtrinsic_param::test()
{
	if ((fabs(m_omc[0])>=1.57)||(fabs(m_omc[1])>=1.57))
	{
		double d_temp[9]={0,-1,0,-1,0,0,0,0,-1};
		CRotation RRR(d_temp);
		m_Rc*=RRR;
		m_omc=m_Rc.Rodrigues();
	}
}

BOOL CExtrinsic_param::LoadCalib(CString strFileName,int Indice)
{
	CString strEntry;
	strEntry.Format("%s%d",DEFAULT_CALC_NAME,Indice);

    return Load(strFileName, strEntry);
}

BOOL CExtrinsic_param::Load(CString strFileName,CString strEntry)
{
	CString strFunction("Load");
	CString msg("in Load");

	#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		msg.Format("in");
		H3DebugInfo(strModule,strFunction,msg);
	#else
		msg.Format("CExtrinsic_param::in LoadCalib");
		AfxMessageBox(msg);
	#endif

	m_param.nMaxIterHomography=H3GetPrivProfileInt(strEntry,_T("MAX_ITER_HOMOGRAPHY"),strFileName);
	m_param.nMinTarget=H3GetPrivProfileInt(strEntry,_T("MIN_TARGET"),strFileName);

	m_omc.ReAlloc(3,1);
	m_Tc.ReAlloc(3,1);
	m_Rc.ReAlloc(3,3);
	m_omc[0]=H3GetPrivProfileFloat(strEntry,_T("omc0"),strFileName);
	m_omc[1]=H3GetPrivProfileFloat(strEntry,_T("omc1"),strFileName);
	m_omc[2]=H3GetPrivProfileFloat(strEntry,_T("omc2"),strFileName);

	m_Rc[0]=H3GetPrivProfileFloat(strEntry,_T("Rc00"),strFileName);
	m_Rc[1]=H3GetPrivProfileFloat(strEntry,_T("Rc01"),strFileName);
	m_Rc[2]=H3GetPrivProfileFloat(strEntry,_T("Rc02"),strFileName);
	m_Rc[3]=H3GetPrivProfileFloat(strEntry,_T("Rc10"),strFileName);
	m_Rc[4]=H3GetPrivProfileFloat(strEntry,_T("Rc11"),strFileName);
	m_Rc[5]=H3GetPrivProfileFloat(strEntry,_T("Rc12"),strFileName);
	m_Rc[6]=H3GetPrivProfileFloat(strEntry,_T("Rc20"),strFileName);
	m_Rc[7]=H3GetPrivProfileFloat(strEntry,_T("Rc21"),strFileName);
	m_Rc[8]=H3GetPrivProfileFloat(strEntry,_T("Rc22"),strFileName);

	m_Tc[0]=H3GetPrivProfileFloat(strEntry,_T("Tc0"),strFileName);
	m_Tc[1]=H3GetPrivProfileFloat(strEntry,_T("Tc1"),strFileName);
	m_Tc[2]=H3GetPrivProfileFloat(strEntry,_T("Tc2"),strFileName);
	
	if( fabs(m_omc[0])<FLT_EPSILON && fabs(m_omc[1])<FLT_EPSILON && fabs(m_omc[2])<FLT_EPSILON && 
		fabs(m_Tc[0])<FLT_EPSILON && fabs(m_Tc[1])<FLT_EPSILON && fabs(m_Tc[2])<FLT_EPSILON &&
		(m_Rc[0]>1.00001))
        //Erreur Code 4
        return false;
	else
		return TRUE;
}

BOOL CExtrinsic_param::SaveCalib(CString strFileName,int Indice)
{
	CString strEntry;
	strEntry.Format("%s%d",DEFAULT_CALC_NAME,Indice);

    return Save(strFileName, strEntry);
}

BOOL CExtrinsic_param::Save(CString strFileName,CString strEntry)
{
	CString strFunction("Save");
	CString msg("in Save");

	#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugInfo(strModule,strFunction,msg);
	#else
		AfxMessageBox(msg);
	#endif

	bool b=true;

	b&=H3WritePrivProfileInt(strEntry,_T("MAX_ITER_HOMOGRAPHY"),m_param.nMaxIterHomography,strFileName);
	b&=H3WritePrivProfileInt(strEntry,_T("MIN_TARGET"),m_param.nMinTarget,strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("omc0"),m_omc[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("omc1"),m_omc[1],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("omc2"),m_omc[2],strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("Rc00"),m_Rc[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Rc01"),m_Rc[1],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Rc02"),m_Rc[2],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Rc10"),m_Rc[3],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Rc11"),m_Rc[4],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Rc12"),m_Rc[5],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Rc20"),m_Rc[6],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Rc21"),m_Rc[7],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Rc22"),m_Rc[8],strFileName);

	b&=H3WritePrivProfileFloat(strEntry,_T("Tc0"),m_Tc[0],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Tc1"),m_Tc[1],strFileName);
	b&=H3WritePrivProfileFloat(strEntry,_T("Tc2"),m_Tc[2],strFileName);

	return b;
}

