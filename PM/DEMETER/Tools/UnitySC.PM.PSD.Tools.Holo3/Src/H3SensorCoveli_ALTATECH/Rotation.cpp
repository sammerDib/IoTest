// Exact copy of Rotation.cpp in H3DSLRCalibration.
// Ideally, this version should be removed, but the linker doesn't seem to agree with that... ;)

// Rotation.cpp: implementation of the CRotation class.
//
//////////////////////////////////////////////////////////////////////


#include "stdafx.h"
#include "Rotation.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[]=__FILE__;
#define new DEBUG_NEW
#endif

static CString strModule("Rotation");
//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////
CRotation::CRotation()
{
	m_nLi = 3;
	m_nCo = 3;
	Alloc(m_nLi, m_nCo);

	//AngleDefined=false;
	long i;
	long j;
	for(i=0;i<m_nLi;i++)
	{
		for(j=0;j<m_nCo;j++)
		{
			if(i == j)
				(*this)(i,j)=1.0;
			else
				(*this)(i,j)=0.0;
		}
	}
}

CRotation::CRotation(const CRotation& R):Matrix(R)
{
	//AngleDefined=R.AngleDefined;

	//if(AngleDefined){
	//	RoThetaPhi.x=R.RoThetaPhi.x;
	//	RoThetaPhi.y=R.RoThetaPhi.y;
	//	RoThetaPhi.z=R.RoThetaPhi.z;
	//}
}

CRotation::CRotation(const Matrix &M):Matrix(3,3)
{
	//AngleDefined=false;

	long i;
	long j;
	for(i=0;i<m_nLi;i++){
		for(j=0;j<m_nCo;j++){
			if(i<m_nLi && j<m_nCo)
				(*this)(i,j)=M(i,j);
			else
				(*this)(i,j)=0;
		}
	}
}

//CRotation::CRotation(const Rad ro, const Rad theta, const Rad phi):Matrix(3,3)
//{
//	RoThetaPhi.x=ro;
//	RoThetaPhi.y=theta;
//	RoThetaPhi.z=phi;
//
//	AngleDefined=true;
//
//	defineData();
//}


//CRotation::CRotation(const Triplet& T):Matrix(3,3)
//{
//	CRotation(T.x,T.y,T.z);
//}

CRotation::CRotation(const double* const mat):Matrix(3,3)
{
	//AngleDefined=false;

	for(short i=0;i<9;i++) m_pData[i]=mat[i];
}

CRotation::~CRotation()
{
}

//CH3Triplet< double > CRotation::GetAngle()
//{
//	return RoThetaPhi;
//}

CRotation CRotation::Trans()const
{
	return CRotation((*this).Matrix::Trans());
}

CRotation CRotation::operator* (const CRotation &R)const
{
	return CRotation((*this).Matrix::operator*((const Matrix)R));	
}

CRotation CRotation::operator* (const double x)const
{
	return CRotation((*this).Matrix::operator*(x));	
}

CRotation CRotation::operator= (const CRotation &R)
{
	return CRotation((*this).Matrix::operator=((const Matrix)R));	
}

/*
CLASS_TYPE
H3_MATRIX CRotation::operator* (const H3_MATRIX &M)
{
	return H3_MATRIX< TYPE >((*this).Matrix::operator*((const Matrix)R));	
}
*/
//cv121206: tres surprenant (ro et theta autour de z et dans le sens opposé à la normale, phi autour de y et opp. norm)
//			je pense qu'il faur faire rozZ*rotY*rotZ << à verifier
//cv270510: donc correction
//cv010610: plus correction du signe
//void CRotation::defineData()
//{
//	if(AngleDefined){
//		if (m_pData==nullptr ) 
//			m_pData=new double[9];
//		double c,s;
//		double	ro=RoThetaPhi.x,
//				theta=RoThetaPhi.y,
//				phi=RoThetaPhi.z;
//
//		c= ::cos(ro);s= ::sin(ro);
//		//double mat1[9]={c,-s,0,s,c,0,0,0,1};
//		double mat1[9]={c,s,0,-s,c,0,0,0,1};
//		CRotation R1(mat1);
//
//		c= ::cos(theta);s= ::sin(theta);
//		//double mat2[9]={c,0,s,0,1,0,-s,0,c};
//		double mat2[9]={c,0,-s,0,1,0,s,0,c};
//		CRotation R2(mat2);
//
//		c= ::cos(phi);s= ::sin(phi);
//		//double mat3[9]={c,-s,0,s,c,0,0,0,1};
//		double mat3[9]={c,s,0,-s,c,0,0,0,1};
//		CRotation R3(mat3);
//
//		CRotation R=R3*R2*R1;
//			
//		for(short i=0;i<9;i++) m_pData[i]=R.m_pData[i];
//	}
//}

double  CRotation::Det()
{
//	if(!DataDefined) defineData();
	return(	 m_pData[0]*m_pData[4]*m_pData[8]
			+m_pData[1]*m_pData[5]*m_pData[6]
			+m_pData[2]*m_pData[3]*m_pData[7]
			-m_pData[2]*m_pData[4]*m_pData[6]
			-m_pData[1]*m_pData[3]*m_pData[8]
			-m_pData[0]*m_pData[5]*m_pData[7]);
}


double  CRotation::Trace()
{
//	if(!DataDefined) defineData();
	return(	 m_pData[0]+m_pData[4]+m_pData[8]);
}

CH3Matrix< double > CRotation::Rodrigues(CH3Matrix< double > &dout)
{
	//verification matrice rotation
	if (!(*this).Check()){
		CH3Matrix< double > RetNaN(3,1);
		RetNaN.Fill(NaN);
		return RetNaN;
	}

	double tr=(Trace()-1)/2,tr_=tr;

	Matrix dthetadR(1,9);//dtrdR(dtemp);
	dthetadR.Fill(0);
	dthetadR[0]=dthetadR[4]=dthetadR[8]=0.5;

	CH3Matrix< double > dvardr(5,9),dvar2dvar(4,5),domega2dvar2(3,4);

	if (tr_>1)  tr_=1.0-DBL_EPSILON;//permet de calculer acos()
	if (tr_<-1) tr_=-1.0+DBL_EPSILON;
	double theta=::acos(tr_),dthetadtr;
	double s= ::sin(theta),c= ::cos(theta);

	if (s>1e-5){
		double vth,dvthdtheta;
		Matrix om1(3,1),om,out;

		dthetadtr=-1/ ::sqrt(1-tr_*tr_);
		dthetadR *= dthetadtr;
		vth=0.5/s;
		dvthdtheta=-vth*c/s;
		om1(0)=(m_pData[7]-m_pData[5]);
		om1(1)=(m_pData[2]-m_pData[6]);
		om1(2)=(m_pData[3]-m_pData[1]);
		om=om1*vth;
		out=om*theta;

		dvardr.Fill(0);
		dvardr(0,5)=dvardr(1,6)=dvardr(2,1)=1;
		dvardr(0,7)=dvardr(1,2)=dvardr(2,3)=1;
		dvardr.SetAt(3,0,dthetadR*dvthdtheta);
		dvardr.SetAt(4,0,dthetadR);

/*		dvar2dvar.Fill(0);
		dvar2dvar.SetAt(0,0,Mat_eye(3)*vth);
		dvar2dvar.SetAt(0,3,om1);
		dvar2dvar(3,4)=1;

		domega2dvar2.SetAt(0,0,Mat_eye(3)*theta);
		domega2dvar2.SetAt(0,3,om);
*/
//SJ
		H3_ARRAY2D_FLT64 MATEYE=H3_ARRAY2D_FLT64(3,3);
		MATEYE.Fill(0);
		MATEYE.SetAt(0,0,1.F);
		MATEYE.SetAt(1,1,1.F);
		MATEYE.SetAt(2,2,1.F);

		dvar2dvar.Fill(0);
		dvar2dvar.SetAt(0,0,MATEYE*vth);
		dvar2dvar.SetAt(0,3,om1);
		dvar2dvar(3,4)=1;

		domega2dvar2.SetAt(0,0,MATEYE*theta);
		domega2dvar2.SetAt(0,3,om);

		dout=domega2dvar2*dvar2dvar*dvardr;
		return out;
	}
	else{
		if (tr>0){
			CH3Matrix< double > out(3,1);
			out.Fill(0);

			return(out);
		}
		else{
			CH3Matrix< double > out(3,1);
			out(0)=(::sqrt((m_pData[0]+1)/2));
			out(1)=(::sqrt((m_pData[1]+1)/2)*2*(m_pData[1]>=0)-1);
			out(2)=(::sqrt((m_pData[2]+1)/2)*2*(m_pData[2]>=0)-1);
			out = out*theta;

			return out;
		}
	}	
}

CH3Matrix< double > CRotation::Rodrigues()
{
	CH3Matrix< double > temp;
	//verification matrice rotation
	if (!(*this).Check()) return CH3Matrix< double >(3,1);

	double tr=(Trace()-1)/2,tr_=tr;

	Matrix dthetadR(1,9);//dtrdR(dtemp);
	dthetadR.Fill(0);
	dthetadR[0]=dthetadR[4]=dthetadR[8]=0.5;

	CH3Matrix< double > dvardr(5,9),dvar2dvar(4,5),domega2dvar2(3,4);

	if (tr_>1)  tr_=1.0-DBL_EPSILON;//permet de calculer acos()
	if (tr_<-1) tr_=-1.0+DBL_EPSILON;
	double theta=::acos(tr_),dthetadtr;
	double s= ::sin(theta),c= ::cos(theta);

	if (s>1e-5){
		double vth,dvthdtheta;
		Matrix om1(3,1),om,out;

		dthetadtr=-1/ ::sqrt(1-tr_*tr_);
		dthetadR *= dthetadtr;
		vth=0.5/s;
		dvthdtheta=-vth*c/s;
		om1(0)=(m_pData[7]-m_pData[5]);
		om1(1)=(m_pData[2]-m_pData[6]);
		om1(2)=(m_pData[3]-m_pData[1]);
		om=om1*vth;
		out=om*theta;

		dvardr.Fill(0);
		dvardr(0,5)=dvardr(1,6)=dvardr(2,1)=1;
		dvardr(0,7)=dvardr(1,2)=dvardr(2,3)=1;
		dvardr.SetAt(3,0,dthetadR*dvthdtheta);
		dvardr.SetAt(4,0,dthetadR);

/*		dvar2dvar.Fill(0);
		dvar2dvar.SetAt(0,0,Mat_eye(3)*vth);
		dvar2dvar.SetAt(0,3,om1);
		dvar2dvar(3,4)=1;

		domega2dvar2.SetAt(0,0,Mat_eye(3)*theta);
		domega2dvar2.SetAt(0,3,om);
*/
		H3_ARRAY2D_FLT64 MATEYE=H3_ARRAY2D_FLT64(3,3);
		MATEYE.Fill(0);
		MATEYE.SetAt(0,0,1.F);
		MATEYE.SetAt(1,1,1.F);
		MATEYE.SetAt(2,2,1.F);
		dvar2dvar.Fill(0);
		dvar2dvar.SetAt(0,0,MATEYE*vth);
		dvar2dvar.SetAt(0,3,om1);
		dvar2dvar(3,4)=1;

		domega2dvar2.SetAt(0,0,MATEYE*theta);
		domega2dvar2.SetAt(0,3,om);



		temp=domega2dvar2*dvar2dvar*dvardr;

		return out;
	}
	else{
		if (tr>0){
			CH3Matrix< double > out(3,1);
			out.Fill(0);

			return(out);
		}
		else{
			CH3Matrix< double > out(3,1);
			out(0)=(::sqrt((m_pData[0]+1)/2));
			out(1)=(::sqrt((m_pData[4]+1)/2)*(2*(m_pData[1]>=0)-1));
			out(2)=(::sqrt((m_pData[8]+1)/2)*(2*(m_pData[2]>=0)-1));
			out = out*theta;

			return out;
		}
	}	
}

bool  CRotation::Check()
{
	CRotation eye;
	Matrix rtemp=(*this).Trans()*(*this)-eye;//ce n'est pas une rotation
	double test=rtemp.Norm();
	return (test< 1e-5);
}

