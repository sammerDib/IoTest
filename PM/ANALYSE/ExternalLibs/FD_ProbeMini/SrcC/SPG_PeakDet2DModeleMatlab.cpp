
#include "SPG_General.h"

#ifdef SPG_General_USEPEAKDET2D

#include "SPG_Includes.h"

#include "SPG_SysInc.h"

#include <float.h>
#include <stdio.h>
#include <string.h>

#if 1 //1 = normal 0 = debug des valeurs des matrices
#define DBG(m)
#else
#define DBG(m) {char Msg[1024];PrintMatrix(Msg,m,#m);MessageBox(0,Msg,#m,0);}

void SPG_CONV PrintMatrix(char* Msg, Matrix& M, char* MatrixName)
{
	//char Coeff[64];
	//strcpy(Msg,MatrixName);
	//strcat(Msg,"=");
	Msg[0]=0;
	int SizeX=M.Ncols();
	int SizeY=M.Nrows();
	sprintf(Msg,"Lignes:%i Colonnes:%i\r\n",SizeY,SizeX);
	for(int j=1;j<=V_Min(SizeY,7);j++)
	{
		if(j>1) strcat(Msg,"\r\n");
		for(int i=1;i<=V_Min(SizeX,7);i++)
		{
			//Coeff[0]=0;
			CF_GetString(Msg,M(j,i),6);
			//strcat(Msg,Coeff);
			strcat(Msg,"\t");
		}
	}
	return;
}
#endif


/*
void SPG_CONV SPG_PeakModeleLoadMatlabParams(SPG_CONFIGFILE& CFG, MATLAB_PARAM& p)
{
	CHECK(CFG.Etat==0,"SPG_PeakModeleLoadMatlabParams",return);

	//parametres géométriques, les charge depuis le fichier de parametre s'il existe (il sera créé sinon)
	CFG_GetFloatDC(CFG,p.radiusc,	60,	
		"Radius of canon");

	CFG_GetFloatDC(CFG,p.camera_z,	38,	
		"z-Ofset of the camera virtual focus");

	CFG_GetFloatDC(CFG,p.dfocal,	-200,
		"distance of the focal to the main axis, positve value -> above");

	CFG_GetFloatDC(CFG,p.dccd,		5,	
		"distance of the focal to the main axis, positve value -> above");

	CFG_GetFloatDC(CFG,p.CCDWidth,	22.5,
		"largeur totale, ccd centré");

	CFG_GetFloatDC(CFG,p.CCDHeight, 16.8677,
		"hauteur totale, ccd centré");

	CFG_GetFloatDC(CFG,p.YReverse,	1,	
		"Sens de l'axe Y de l'image, +1 ou -1");

	//trois faisceaux par laser 2.011 1.946 1.889
	CFG_GetFloatDC(CFG,p.LaserIncidence[0],	2.011,	
		"Incidence du faisceau laser");
	CFG_GetFloatDC(CFG,p.LaserIncidence[1],	1.946,	
		"Incidence du faisceau laser");
	CFG_GetFloatDC(CFG,p.LaserIncidence[2],	1.889,	
		"Incidence du faisceau laser");

	//trois faisceaux par laser tous à 30° (en tenant compte de la géométrie
	//du système il faut entrer les paramètres suivants: 1.129 1.256 1.135)
	CFG_GetFloatDC(CFG,p.LaserOuverture[0],	1.129,	
		"Incidence du faisceau laser");
	CFG_GetFloatDC(CFG,p.LaserOuverture[1],	1.256,	
		"Incidence du faisceau laser");
	CFG_GetFloatDC(CFG,p.LaserOuverture[2],	1.135,	
		"Incidence du faisceau laser");

	CFG_GetFloatDC(CFG,p.LaserLensloc[0],	82.47,	
		"Incidence du faisceau laser");
	CFG_GetFloatDC(CFG,p.LaserLensloc[1],	65.82,	
		"Incidence du faisceau laser");
	CFG_GetFloatDC(CFG,p.LaserLensloc[2],	52.19,	
		"Incidence du faisceau laser");

	//six lasers (tous les 60°)
#define CFG_GetAngle(i) CFG_GetFloatDC(CFG,p.LaserAngle[i],	i*60*V_PI/180,	"Angle de rotation du faisceau laser par rapport au laser #0");
#define CFG_GetTilt(i) CFG_GetFloatDC(CFG,p.LaserTilt[i],	0,	"Angle de tilt du faisceau laser par rapport a l'horizontale");

	CFG_GetAngle(0);CFG_GetAngle(1);CFG_GetAngle(2);CFG_GetAngle(3);CFG_GetAngle(4);CFG_GetAngle(5);
	CFG_GetTilt(0);CFG_GetTilt(1);CFG_GetTilt(2);CFG_GetTilt(3);CFG_GetTilt(4);CFG_GetTilt(5);

	return;
}
*/

void simurays(MATLAB_VAR& m, MATLAB_PARAM& p)
{
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Dacon:
//
// Main MATLAB script, simulation of what the CCD see
// and analytic expression of the curves on the CCD
//
// Author: Bruno: 03/Feb/2005
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Cylindrical Canon geometry description
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Radius of the canon
//
float radiusc=p.radiusc;

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Laser beams geometry description
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//float deltaradius=radiusc-60;
//float pi=4*atan(1);
float lensangles=p.LaserIncidence[ANGLE_FROM_ID(p.LaserID)][INCIDENCE_FROM_ID(p.LaserID)];//[45 40]*pi/180; // angles of the beam, 40 + 45 degrees
float lensloc=p.LaserLensloc[INCIDENCE_FROM_ID(p.LaserID)];//[0 0]; // the z-location where the beams meets the main z-axis
float ouverture=p.LaserOuverture[INCIDENCE_FROM_ID(p.LaserID)];//[50 45]*pi/180; // opening angles of the beams

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Camera geometry description
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//
// z-Ofset of the camera virtual focus
//
float camera_z=p.camera_z;
float dfocal=p.dfocal; // distance of the focal to the main axis, positve value -> above
             // negative value -> bellow
float dccd=p.dccd; // distance of the focal to the main axis, positve value -> above
        // negative value -> bellow
        
float dlooking=dccd; // camera will look only for points with d>dlooking,
               // where d is the distance to the cross section plane of the
               // lens containing the z-axis, (d is the distance of the target
               // when traveling parallel to the lens axis until meeting
               // the z=0 axis

float nnorm=dccd-dfocal;

float ccd_hsize=p.CCDWidth*nnorm/(radiusc-dfocal);//22.5
float ccd_vsize=p.CCDHeight*nnorm/(radiusc-dfocal);//16.8677
               
//
// These parameters are the dimension of the virtual CCD
// The parameter dx is used to generate the x-Grid on the CCD
//
TypCCDBox ccdbox;
ccdbox.xmin=-ccd_hsize; ccdbox.xmax=ccd_hsize;
ccdbox.ymin=-ccd_vsize; ccdbox.ymax=ccd_vsize;
ccdbox.nx=p.CCDSizeX;
ccdbox.ny=p.CCDSizeY;
ccdbox.dx=1; // x-step
ccdbox.noflip=(dfocal>dccd);

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Rotational angle of the camera (with respect to the x-axis)
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
CHECK(p.LaserNumberingDirection==0,"Parameter LaserNumberingDirection must be +1 or -1",p.LaserNumberingDirection=1);
float theta=p.CameraAngleRotationRad-p.LaserRefAngle+V_Signe(p.LaserNumberingDirection)*ANGLE_FROM_ID(p.LaserID)*V_DPI/LASER_ANGLE_COUNT;//p.LaserAngle[ANGLE_FROM_ID(p.LaserID)];//signes à vérifier
float phi=p.CameraAngleTiltRad;//80?

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//fxy=dfocal*exp(i*theta);

float fx=dfocal*sin(phi)*cos(theta);//real(fxy);
float fy=dfocal*sin(phi)*sin(theta);//imag(fxy);
float fz=camera_z+dfocal*cos(phi);

//
// Define the vector that is normal to the virtual CCD face,
// the length of this vector is the distance from the focal
// to the CCCD
//
//nxy=nnorm*exp(i*theta);
float nx=nnorm*sin(phi)*cos(theta);//real(nxy);
float ny=nnorm*sin(phi)*sin(theta);//imag(nxy);
float nz=nnorm*cos(phi);

//
// compute the vector c parallel to the canon
//
//C=[0 0 1]';
//float C[3]; C[0]=0;C[1]=0;C[2]=1;
	ColumnVector C(3);
	C(1)=0;
	C(2)=0;
	C(3)=1;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Loop over the beams
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

//for klaser=0:5 // laser beam number, 0-5
// for beamkind=1:length(lensangles) // loop over the beam kind

  float theta1=lensangles;//(beamkind);
  float dz1=lensloc;//(beamkind);
  
//
// Two axis that belongs to the plane of the laser beam
//

  //on suppose klaser=0
  float m1=1;//exp(i*klaser*pi/3);
  float m2si=1;//sqrt(-1)*m1;

  float uz=cos(theta1);
  m1=sin(theta1)*m1;
  
  float ux=m1;//real(m1);
  float uy=0;//imag(m1);
  float vx=0;//real(m2);
  float vy=m2si;//imag(m2);
  float vz=0;

  float wx=uy*vz-uz*vy;
  float wy=uz*vx-ux*vz;
  float wz=ux*vy-uy*vx;

  vx=cos(p.LaserTilt[ANGLE_FROM_ID(p.LaserID)])*vx+sin(p.LaserTilt[ANGLE_FROM_ID(p.LaserID)])*wx;
  vy=cos(p.LaserTilt[ANGLE_FROM_ID(p.LaserID)])*vy+sin(p.LaserTilt[ANGLE_FROM_ID(p.LaserID)])*wy;
  vz=cos(p.LaserTilt[ANGLE_FROM_ID(p.LaserID)])*vz+sin(p.LaserTilt[ANGLE_FROM_ID(p.LaserID)])*wz;

  //
  // The coordinates the point where the laser beam plane goes through
  //
  ColumnVector M(3);
  M(1)=0;
  M(2)=0;
  M(3)=dz1;

// Look the intersection of the laser beam plane with the canon axis
  /*
  A=[0 ux vx; ...
     0 uy vy; ...
     -1 uz vz];
  rhs=[-Mx; -My; -Mz];
  
  sol=A\rhs;
  */

  Matrix A(3,3);
  A(1,1)=0;  A(1,2)=ux; A(1,3)=vx;
  A(2,1)=0;  A(2,2)=uy; A(2,3)=vy;
  A(3,1)=-1; A(3,2)=uz; A(3,3)=vz;

  DBG(A);

  //cout<<A;

  ColumnVector rsh(3);
  rsh=-M;

  ColumnVector sol(3);


  sol=A.i()*rsh;

  DBG(sol);//vecteur nul

  //
  // Here is the center of the elipsoid
  //
  //N=[0; 0; 1];
  ColumnVector N(3);
  N(1)=0;
  N(2)=0;
  N(3)=sol(1);
  
  //
  // Compute the two main axis of the ellipsoid
  //
  //U=[ux uy uz]';
  ColumnVector U(3);
  U(1)=ux; 
  U(2)=uy;
  U(3)=uz;

  //V=[vx vy vz]';
  ColumnVector V(3);
  V(1)=vx; 
  V(2)=vy;
  V(3)=vz;

  float ps_cu=(C.t()*U).AsScalar(); // scalar product
  float ps_cv=(C.t()*V).AsScalar(); // scalar product

  ColumnVector P(3);
  ColumnVector Q(3);
  ColumnVector R(3);
  P=U*ps_cu+V*ps_cv;
  Q=V*ps_cu-U*ps_cv;
  R=C-P;

  DBG(P);
  DBG(Q);
  DBG(R);
  
  float Pnorm=sqrt((P.t()*P).AsScalar());
  float Rnorm=sqrt((R.t()*R).AsScalar());
  
  float h=Rnorm*Pnorm;
  float scale=radiusc/h;
  P=P*scale;
  Q=Q/Pnorm*radiusc;
  
  /*
  float Px=P(1);  float Py=P(2);  float Pz=P(3);
  float Qx=Q(1);  float Qy=Q(2);  float Qz=Q(3);
  */
  
  //
  // Compute the normal to the two side-axes that delimited the laser beam
  //
  DBG(U);
  DBG(V);

  float alpha=ouverture/2;
  ColumnVector NLEFT(3);
  ColumnVector NRIGHT(3);
  NLEFT=V*cos(alpha)+U*sin(alpha);
  NRIGHT=V*-cos(alpha)+U*sin(alpha);
  float psleft=(NLEFT.t()*M).AsScalar();
  float psright=(NRIGHT.t()*M).AsScalar();
  
  //NCONSTRAINT=[NLEFT NRIGHT sign(nnorm)*[nx ny nz]']; // The last constraint is needed because
                                          // the lens is looking from one side

  Matrix NCONSTRAINT(3,3);

  NCONSTRAINT.Column(1)=NLEFT;
  NCONSTRAINT.Column(2)=NRIGHT;

  ColumnVector NXYZ(3);
  NXYZ(1)=nx;
  NXYZ(2)=ny;
  NXYZ(3)=nz;
  NCONSTRAINT.Column(3)=NXYZ*(nnorm>0?1:-1);


  //psconstraint=[psleft psright nnorm*dlooking];
  RowVector psconstraint(3);
  psconstraint(1)=psleft;
  psconstraint(2)=psright;
  psconstraint(3)=nnorm*dlooking;

  DBG(NCONSTRAINT);
  DBG(psconstraint);

  //
  // This function return the boolean flag for points X that belong to the
  // intersection of n half-space { X: <X,Ni> > psi }, where Ni, and psi are stored
  // respectively in the ith column of the 3xn matrix NCONSTRAINT and
  // 1xn vector psconstraint
  //
  //constraintfcn=@(X) (all(NCONSTRAINT'*X-ndgrid(psconstraint,X(1,:))>0, 1));

  //
  // Perform the projection on the CCD
  //

  prjcam(m,fx,fy,fz,nx,ny,nz,N,P,Q,ccdbox,NCONSTRAINT,psconstraint);
  
// end for beamkind
//end for klaser
  return;
}

void constraintfcn(Matrix& X, Matrix& NCONSTRAINT, RowVector& psconstraint, RowVector& Valid)
{//met a zero yValid si hors contrainte, laisse la valeur antérieure sinon
	//DbgCHECK(1,"entering constraintfcn");
	DBG(NCONSTRAINT);
	DBG(psconstraint);
	CHECK(NCONSTRAINT.Ncols()!=psconstraint.Ncols(),"constraintfcn",return);

	int Xcols=X.Ncols();
	int NCONSTRAINTcols=NCONSTRAINT.Ncols();

	for(int n=1;n<=Xcols;n++)
	{
		V_VECT XColumn;
		XColumn.x=X(1,n);
		XColumn.y=X(2,n);
		XColumn.z=X(3,n);
		for(int m=1;m<=NCONSTRAINTcols;m++)
		{
			V_VECT NConstraintColumn;
			NConstraintColumn.x=NCONSTRAINT(1,m);
			NConstraintColumn.y=NCONSTRAINT(2,m);
			NConstraintColumn.z=NCONSTRAINT(3,m);
			float produitscalaire;

			V_ScalVect(NConstraintColumn,XColumn,produitscalaire);

			if(produitscalaire-psconstraint(m)<0)
			{
				Valid(n)=0;
				break;
			}
		}
	}
	return;
}

void prjcam(MATLAB_VAR& m,  
			float& fx, float& fy, float& fz, float& nx, float& ny, float& nz, ColumnVector& N, ColumnVector& P, ColumnVector& Q, TypCCDBox& ccdbox,
			Matrix& NCONSTRAINT, RowVector& psconstraint)
{
//function [Xp, Yp, Zp]=prjcam_minima(fx, fy, fz, nx, ny, nz, ...
//                             N, P, Q, ccdbox, constraintfcn, varargin)
//
//  function [Xp, Yp, Zp]=prjcam(fx, fy, fz, nx, ny, nz, ...
//                              N, P, Q, ccdbox, constraintfcn, varargin)
//
// Perform the projection on the CCD
//
// Author: Bruno 03/Feb/2006
//          Last update: 06/Feb, no need to normalize target ellipse at d=1
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
    
  // ccd dimensions, x-resolution, and flipflag 
  
  float xmin=ccdbox.xmin;
  float xmax=ccdbox.xmax;
//  float ymin=ccdbox.ymin;
//  float ymax=ccdbox.ymax;
//  float dx=ccdbox.dx;
  float noflip=ccdbox.noflip;
  
  //
  // Pcam is the orthonormal basis of the CCD, the three columns are
  // (1) CCD-x axis
  // (2) CCD-y axis (should be parallel to the canon axis)
  // (3) camera lens axis
  
  //[Pcam, nnorm]=cambasis(nx, ny, nz, noflip);

  Matrix Pcam(3,3);
  float nnorm;

  cambasis(m,Pcam,nnorm,nx,ny,nz,noflip);

  DBG(Pcam);

  
  //////////////////////////////////////////////////////////////////////////////////////////////////////
  // Compute the points by conic-projective method
  //////////////////////////////////////////////////////////////////////////////////////////////////////
    
  //
  // Compute the normal vector to the ellipse by vectorial product
  //
  
  //R=[P(2)*Q(3)-P(3)*Q(2); ...
  //   P(3)*Q(1)-P(1)*Q(3); ...
  //   P(1)*Q(2)-P(2)*Q(1)]; 
  ColumnVector R(3);
  R(1)=P(2)*Q(3)-P(3)*Q(2);
  R(2)=P(3)*Q(1)-P(1)*Q(3);
  R(3)=P(1)*Q(2)-P(2)*Q(1);

  DBG(R);
  
  //
  // virtual Focal point
  //
  ColumnVector F(3);//Row or Column ?
  //F=[fx; fy; fz];
  F(1)=fx;
  F(2)=fy;
  F(3)=fz;

  DBG(F);

  P=P/((P.t()*P).AsScalar());
  Q=Q/((Q.t()*Q).AsScalar());

  DBG(P);
  DBG(Q);
  
  ColumnVector NF(3);
  NF=(F-N);
  float PNF=(P.t()*NF).AsScalar(); // scalar products
  float QNF=(Q.t()*NF).AsScalar();
  float RNF=(R.t()*NF).AsScalar();

  if(abs(RNF)<FLT_EPSILON*R.NormFrobenius()) // Singular, camera orthogonal to the ellipse plane
      return;
  
  R=R/(-RNF);
  
//
// Matrix of the bilinear form centered around the focal
// the dimension of B is 1/L^2, where L is length unit
// This matrix is computed directly in the new basis attached to the CCD
//
  /*
  A1=Pcam' * [P+PNF*R Q+QNF*R R];
  A2=A1; A2(:,3)=-A2(:,3);
  BB=A1*A2';
  */

  Matrix A1(3,3);
  Matrix A2(3,3);
  Matrix TMPMAT(3,3);
  TMPMAT.Column(1)=P+R*PNF;
  TMPMAT.Column(2)=Q+R*QNF;
  TMPMAT.Column(3)=R;
  A1=Pcam.t()*TMPMAT;
  A2=A1;
  A2.Column(3)=-A2.Column(3);
  Matrix BB(3,3);
  BB=A1*A2.t();

  DBG(A1);
  DBG(A2);
  DBG(BB);
 
 //
 // Generate the x-Grid on the CCD
 //
 //  float xgrid=(xmin:dx:xmax);
 //  z=nnorm*ones(size(xgrid));
 
 //
 // find the coefficients a,b,c of the second order polynomial equation
 // (y is the unknow) of the conic
 //    a*y^2+ b*y + c = 0
 //

	RowVector xgrid(ccdbox.nx);
	RowVector z(ccdbox.nx);
	RowVector y(ccdbox.nx);
	RowVector yValid(ccdbox.nx);
	{for(int i=1;i<=ccdbox.nx;i++)
	{
		xgrid(i)=xmin+(i-1)*(xmax-xmin)/(ccdbox.nx-1);
#ifdef DebugFloat
		float xgridfloat=xgrid(i);
		CHECKFLOAT(xgridfloat,"xgrid(i)");
#endif
		z(i)=nnorm;

	   float a=BB(2,2);
	   float b=2*(BB(1,2)*(xgrid(i))+BB(3,2)*nnorm);
	   float c=(BB(1,1)*(xgrid(i))+(2*BB(3,1)*nnorm))*xgrid(i)+BB(3,3)*nnorm*nnorm;
 
	   float delta=b*b-4*a*c;
	   if(delta<0)
	   {
		   y(i)=0;
		   yValid(i)=0;
	   }
	   else
	   {
		   float sqrtd=sqrt(delta);
		   float s=(Pcam(3,2)>0?1:-1);
		   y(i)=(-b-s*sqrtd)/(2*a);
		   yValid(i)=1;
		   /*
		   if((y(i)>=ymin)&&(y(i)<=ymax))
		   {
		   }
		   else
		   {
			yValid(i)=0;
		   }
		   */
	   }
	}}

	DBG(xgrid);
	DBG(y);
	/*
	float lastY=y(768);
	int Valid=yValid(768);
	*/

	Matrix PointsCCD(3,ccdbox.nx);
	PointsCCD.Row(1)=xgrid;
	PointsCCD.Row(2)=y;
	PointsCCD.Row(3)=z;
#ifdef DebugFloat
	float fPointsCCD=PointsCCD(1,1);
	CHECKFLOAT(fPointsCCD,"PointsCCD(1,1)");
#endif

	//constraintfcn(PointsCCD,NCONSTRAINT,psconstraint,yValid);

	ColumnVector Rcam(3);
	Rcam=Pcam.t()*R;


	RowVector t(ccdbox.nx);
	t=Rcam.t()*PointsCCD;
	{for(int i=1;i<=ccdbox.nx;i++)
	{
		CHECK(t(i)==0,"Rcam.t()*PointsCCD(i)=0",continue);
		t(i)=1/t(i);
		float ftest=t(i);
		CHECKFLOAT(ftest,"t(i)");
	}}

 	Matrix TARGET(3,ccdbox.nx);
	ColumnVector TT(3);
	{for(int j=1;j<=ccdbox.nx;j++)
	{
		{for(int i=1;i<=3;i++)
		{
#ifdef DebugFloat
			float fPointsCCD=PointsCCD(i,j);
			CHECKFLOAT(fPointsCCD,"PointsCCD(i,j)");
			float ft=t(j);
			CHECKFLOAT(ft,"t(j)");
#endif

			TT(i)=PointsCCD(i,j)*t(j);

#ifdef DebugFloat
			float fTT=TT(i);
			CHECKFLOAT(fTT,"TT(i)");
#endif
		}}
		TARGET.Column(j)=F+Pcam*TT;

#ifdef DebugFloat
		float f=TARGET(2,j);
		CHECKFLOAT(f,"f");
#endif
	}}

#ifdef DebugFloat
	float f=TARGET(2,1);
	CHECKFLOAT(f,"f");
#endif

	constraintfcn(TARGET,NCONSTRAINT,psconstraint,yValid);

	/**********************************************

  COPIE DES RESULTATS EN DIRECTION DE LA STRUCTURE MATLAB_VAR& m

	**********************************************/

	CHECK(m.NumY!=ccdbox.nx,"prjcam",return);
	CHECK(m.Y==0,"prjcam",return);
	CHECK(m.Yvalid==0,"prjcam",return);
	{for(int i=1;i<=ccdbox.nx;i++)
	{
		m.Y[i-1]=(y(i)-ccdbox.ymin)*ccdbox.ny/(ccdbox.ymax-ccdbox.ymin);
		if(yValid(i)>0)
		{
			m.Yvalid[i-1]=1;

		}
		else
		{
			m.Yvalid[i-1]=0;
		}
	}}
	m.ccdbox=ccdbox;

	return;
}

void cambasis(MATLAB_VAR& m,  
			  Matrix& Pcam, float& nnorm, float& nx, float& ny, float& nz, 
			  float& noflipx)
{
//function [Pcam, nnorm, nx, ny, nz]=cambasis(nx, ny, nz, noflipx)
//
// function [Pcam, nnorm, nx, ny, nz]=cambasis(nx, ny, nz, noflipx)
//
// Pcam is antidirect basis, unless if noflipx flag is given
//
// Typically, for the virtual focale point located behind the CCD, 
// noflipx should be 0 (default value), otherwise noflipx should be 1
//
// return the orthonormal basis of the CCD, the three columns are
// (1) CCD-x axis
// (2) CCD-y axis (should be almost parallel to the canon axis)
// (3) camera lens axis
//
// nnorm: norm of the normal vector
// output (nx, ny, nz) will be normalized
//
// Author: Bruno: 03/Feb/2005
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

  //
  // Compute the polar coordinate of the lens axis
  //
  nnorm=sqrt(nx*nx+ny*ny+nz*nz);
  nx=nx/nnorm;
  ny=ny/nnorm;
  nz=nz/nnorm;  

  float R=(1+4*FLT_EPSILON)*sqrt(nx*nx+ny*ny);
  float sinphi=R;
  float phi;
  if(R>=1)
  {
	sinphi=1;
	phi=V_PI/2;
  }
  else if(R<=-1)
  {
	  sinphi=-1;
	  phi=-V_PI/2;
  }
  else
  {
	phi=asin(sinphi);
  }

  float theta=atan2(ny,nx);//variables locales à cette fonction n'ayant rien a voir avec theta de simurays

  //
  // Pcam is the orthonormal basis of the CCD, the three columns are
  // (1) CCD-x axis
  // (2) CCD-y axis (should be parallel to the canon axis)
  // (3) camera lens axis
  // Pcam is antidirect basis, unless if noflipx flag is given
  Pcam(1,1)=sin(theta);	Pcam(1,2)=-cos(phi)*cos(theta);	Pcam(1,3)=sin(phi)*cos(theta);
  Pcam(2,1)=-cos(theta);Pcam(2,2)=-cos(phi)*sin(theta);	Pcam(2,3)=sin(phi)*sin(theta);
  Pcam(3,1)=0;			Pcam(3,2)=sin(phi);				Pcam(3,3)=cos(phi);
    
  if(noflipx) // determinant = 1
  {
      //Pcam(:,1)=-Pcam(:,1);
	  Pcam.Column(1)=-Pcam.Column(1);
  }

  DBG(Pcam);
  return;
}


#endif
