#ifndef _MATRIX_H
#define _MATRIX_H

#ifdef __BORLANDC__
#include <complex.h>
typedef complex cppc;
#else
#define _USE_MATH_DEFINES
#include <complex>
using namespace std;
typedef complex<double> cppc;
#endif

#include "Nag_Compatibility.h" //definition of CComplex 
#include "Defines.h" 


//basic mathematics

int iSum(int iDim, int *pArray);
double dSum(int iDim, double *pArray); 
cppc cSum(int iDim, cppc *pArray); 

void qsort(cppc*, int, int);


bool bParabolicFit_3(double dx, double *pdx, cppc *pcf, cppc *pcCoeff0, cppc *pcCoeff1, cppc *pcCoeff2); //parabola defined by 3 points 
bool bParabolicFit_3(double dx, double *pdx, double *pdf, double *pdCoeff0, double *pdCoeff1, double *pdCoeff2); //parabola defined by 3 points ;
bool bParabolicFit_4(double dx, int iIndexLL, int iIndexMax, double *pdx, cppc *pValue, cppc *pca0, cppc *pca1, cppc *pca2); //parabola defined by 4 points
bool bParabolicFit_4(double dx, int iIndexLL, int iIndexMax, double *pdx, double *pValue, double *pca0, double *pca1, double *pca2); //parabola defined by 4 points 


class SMatrix;
class dMatrix;
class Matrix;
class SVector;
class VArray; 


class iVector { //Vector of integer values
public:
	int Nown;
	int *Data;
	iVector(int Mown = 1);
	~iVector();
	iVector& operator = (const iVector&);
	iVector& operator = (const int);
	iVector& operator += (const int& d);  
	iVector& operator -= (const int& d); 
	iVector& operator *= (const int& d);  
	iVector& operator /= (const int& d); 
	iVector(const iVector&);
	void vResetValue(); 
};


class dVector { //Vector of double values
public: 
	int Nown;
	double *Data; 
	//int iNoEntries; int *pIndexEntries; //for efficient representation of sparse matrices: number of non-zero entries and their index 
	dVector(int Mown = 1);
	
	void Init(int Mown = 1);  
	~dVector();
	dVector& operator = (const dVector&);  
	dVector& operator = (const double);
	dVector& operator += (const double& d);
	dVector& operator -= (const double& d);
	dVector& operator *= (const double& d);
	dVector& operator /= (const double& d);
	dVector& operator += (const dVector& dV); 
	dVector& operator -= (const dVector& dV);
	dVector& operator *= (const dMatrix& dM); //dVector& dVector::operator *= (const dMatrix& dM) 
	dVector operator * (const dMatrix& dM); 
	dVector(const dVector&); 
	double norm(); 
	double norm_sq(); 
}; 


class Vector {
  public:
    int Nown;
	 cppc* Data;
	 Vector(int Mown = 1);
	 void Init(int Mown = 1);
	 Vector( const Vector *M ); 
     Vector( const Vector& );
	 Vector( const Vector&, const Vector& ); 
	 Vector( const Vector&, long iFirst, long iLast ); 
	 Vector(const VArray&); 
	 Vector( const VArray& VA, const VArray& VB );
	 Vector( const VArray& VA, const VArray& VB, const VArray& VC );
    ~Vector();
	void VAToVec(const VArray& VA) {  Vector C(VA); *this = C;  }
	void VAToVec(const VArray& VA, const VArray& VB) {  Vector C(VA, VB); *this = C;  }
	void VAToVec(const VArray& VA, const VArray& VB, const VArray& VC){  Vector C(VA, VB, VC); *this = C;  }

    Vector& operator = (const Vector&);
	bool operator == (const Vector&); 
    cppc& operator() (int i) const {return Data[i];}

	Vector operator = (const double& d) {long i;   for(i=0; i<(long)this->Nown; i++)  this->Data[i].real(d), this->Data[i].imag(0);   return *this;}  
	Vector operator * (const double& c) {Vector C(*this); long i;   for(i=0; i<(long)C.Nown; i++)  C.Data[i] *= c;  return C;} 
	Vector operator *= (const double& c) {long i;   for(i=0; i<(long)this->Nown; i++)  this->Data[i] *= c;  return *this;} 

	Vector operator = (const cppc& c); 
	Vector operator + (const cppc& c) {Vector C(*this); long i;   for(i=0; i<(long)C.Nown; i++)  C.Data[i] += c;  return C;} 
	Vector operator - (const cppc& c) {Vector C(*this); long i;   for(i=0; i<(long)C.Nown; i++)  C.Data[i] -= c;  return C;} 
	Vector operator * (const cppc& c) {Vector C(*this); long i;   for(i=0; i<(long)C.Nown; i++)  C.Data[i] *= c;  return C;} 
	Vector operator / (const cppc& c) {Vector C(*this); long i;   for(i=0; i<(long)C.Nown; i++)  C.Data[i] /= c;  return C;} 
	Vector operator *= (const cppc& c); 
	Vector operator /= (const cppc& c) {long i;   for(i=0; i<(long)this->Nown; i++) this->Data[i] /= c;   return *this;} 

    Vector& operator += (const Vector&);
    Vector& operator -= (const Vector&);

    cppc operator * (const Vector&);
	Vector operator | (const Vector& B) { Vector C(*this); 
        for (int i = 0; i < Nown; i++) 
         C.Data[i] = Data[i] * B.Data[i]; 
         return C; }; //Vector C = "Diag-matrix this" * Vector B
	Vector operator * (const Matrix& B);
	Vector& operator *= (const Matrix& B) {*this = (*this)*B; return *this;}
	Vector& operator /= (const Matrix&);
    Vector& operator %= (const Matrix&);
    Vector operator + (const Vector& B) {Vector C(*this); C += B; return C;}
    Vector operator - (const Vector& B) {Vector C(*this); C -= B; return C;}
    Vector operator / (const Matrix& B) {Vector C(*this); C /= B; return C;}
    Vector operator % (const Matrix& B) {Vector C(*this); C %= B; return C;}

    double norm(int nn = 0) const;
	double norm_(int nn = 0) const;
	double dMinRePart(int nn = 0);  

	Vector BCG(const Matrix&);
	Vector BCG_(const Matrix&);
	void GMResG(const Matrix&);
	void GMResG(void*, Vector (*mul)(const Vector&, void*));
	void GMResAG(const Matrix&);
	void GMResAG( void *PP,  Vector (*mul)(const Vector&, void*),  int MaxNoRestarts, int MaxNoIter, double Tol, BOOL *bFailure,
					  int *NoIter,  double (*GetPrecisionDemand)(const Vector&, void*), void *Arg, BOOL *bDoTolCheck, Vector *InitGuess, 
					  BOOL bOutput ); 
	void GMResAG_SVec( void *PP,  Vector (*mul)(const Vector&, void*),  int MaxNoRestarts, int MaxNoIter, double Tol, BOOL *bFailure,
					  int *NoIter,  double (*GetPrecisionDemand)(const Vector&, void*), void *Arg, BOOL *bDoTolCheck, Vector *InitGuess );  
	void cudaGMResAG(void*, void(*mul)(void*, void*));
	void GMResAH(const Matrix&);
	void GMResAH(void*, Vector (*mul)(const Vector&, void*));

	void fDampedNewton( int (*fCalcJacAndFuVal)(const Vector *Vx, void *vExtraInfo, void *vJacobi, Vector *VFuVal), 
				   void *vExtraInfo,  void *vJacobi,   Vector (*fApplJac)( const Vector& Vx, void *vJacobi ), 
				   double dFacMin,  int *iItNoNewton,  int iItNoMaxNewton,  double dTol,  int iItNoMaxGMRES,  int *iNoGMRESItTot,   
				   BOOL *bFail,  double (*NormOfNonlinTerm)(const Vector&, void*), void *vArgPrecisionDemand ) ;

    Vector roots();
    cppc poly(cppc);
    Vector pcoeff();
    void qsort();
    Vector polynom(const Vector&);

	//Functions for compatibility with old C-code:   W., 25.7.13
	void GetVal(const CComplex *CSrc);
	void SetVal(CComplex *CDest);
	void SetVal( cppc cVal );
};
//Erfahrungen mit Vektor:
//Va(iDim2) mitten in einer Funktion allokiert keinen Speicher, funktioniert offenbar nur in Verbindung mit einer Definition: 
//Vector Va1(50).  Explizite Freigabe bei Verlassen der Funktion unnoetig. Scheint wie bei einem array ("int Va1[50];") zu sein. 
//Va1 = Va2 kopiert nicht die Zeiger zu den Daten sondern allokiert offenbar und kopiert dann die Daten

typedef  Vector *Vptr; //pointer to a Vector


void vQsortData(double *A, int *piTrackingNo, int iLo, int iHi);
void vQsortData(int iDim, double *dValue, int *piTrackingNo);


struct GSM_Arg_IRK2 { 
	int L; //layer no. 
	int s; //slice no. 
	double dDz; //thickness of slice (integrat. step size) * weight 
	long iN; //no. of diff orders
	Vector *VExpMid; 
};

struct GSM_Arg { 
	long iN, iDimTE, iDim; //no. of diff orders;  dim TE,  dim TE & TM
	int L; //Layer no. 
	double dH; //height of Layer
	int iNoS; //No. of slices 
	double dDz ; //slice thickness (integrat. step size) 
	Vector  *VTddL, *VRudL, *VTuuL, *VRduL,   *VTddU, *VRudU, *VTuuU, *VRduU;  //Fresnel-coeff for Lower border L & Upper border U
	Vector  *VExpDzHalf, *VExpDz, *VExpH, *VExp2H; //exponential factors e^(i*k_z*...)
	Vector  *VMR; //"Vector Multiple Reflection Factor": 1 / ( 1 - r_L * r_U * VExp2H )
};


class VArray { //Vector-Array - resembles SVector a bit
  public:
    int Mown; //no. of pointers;
    Vptr Data;

	void Init(){ 
		Mown = 0; Data = NULL; 
	}
	VArray(int  m = 1, int n = 1); //alloc array of Vectors 
    VArray(const VArray&);
	VArray( const Vector& V, long iDimSingleVec, long iNoSingleVec );
	VArray( const Vector& V, long iDimSingleVec, long iNoSingleVec, bool bFirstHalf );	
	VArray( const Vector& V, long iDimSingleVec, long iNoVecInVA, long iSectionNo ) ;	
	~VArray();
	void VecToVA( const Vector& V, long iDimSingleVec, long iNoSingleVec )	{ VArray VA( V, iDimSingleVec, iNoSingleVec); *this = VA; } 
	void VecToVA( const Vector& V, long iDimSingleVec, long iNoSingleVecInV, bool bFirstHalf ) { VArray VA( V, iDimSingleVec, iNoSingleVecInV, bFirstHalf ); *this = VA; } 
    void VecToVA( const Vector& V, long iDimSingleVec, long iNoVecInVA, long iSectionNo  ) { VArray VA( V, iDimSingleVec, iNoVecInVA, iSectionNo ); *this = VA; }
	VArray& operator = (const VArray&);
	void operator = (const cppc& c){ for (int i=0; i < Mown; i++) Data[i] = c; };
	VArray& operator += (const VArray&); 
	VArray& operator -= (const VArray&); 
	VArray& operator *= (const cppc& c); 
	double norm() { double dNorm = 0.0; for (int i = 0; i < Mown; i++) 
		{ dNorm += Data[i].norm(); } 
		return dNorm; };
};

struct SVecArg {
	int iN;
	int L; 
	int isBot, isTop; //slices numbers of lowermost & uppermost slice
	int iNoS, iDelta_s; //No. of S-Vector planes between Segments; No. of slices in a segment - except for uppermost Segment  
	BOOL bStartWithCdu;
};


class SMatrix;
class SMatrixD;

class SVector {
  public:
    int Mown;
    Vector *Data[2];

	SVector(int n = 1);
    SVector(const SVector&);
	~SVector();

    cppc operator() (int i, int ii) const {return Data[i]->Data[ii];}
    Vector operator() (int i) const {return *Data[i];}

    SVector& operator = (const SVector&);

    SVector operator * (const SMatrix&);
	SVector operator * (const SMatrixD&);
    SVector& operator *= (const SMatrix& B) {return (*this) = (*this)*B;}
    SVector& operator *= (const SMatrixD& B) {return (*this) = (*this)*B;}
};


class dMatrix {
public:
	int Nown, Next;
	double *Data; 
	dMatrix(int Mown = 1, int Mext = 0);
	dMatrix(const dMatrix&);
	~dMatrix();
	void Init(int Mown, int Mext);
	void operator = (const double&);
	dMatrix& operator *= (const double&); 
	dMatrix operator * (const double& c);
	dMatrix& operator = (const dMatrix&);
	double& operator() (int i, int j) const { return Data[i*Next + j]; } 
	dMatrix& operator *= (const dMatrix& B) { *this = (*this)*B; return *this; } 
	dMatrix operator * (const dMatrix&);
	void calc_M_adj_x_M(const dMatrix&); //do Matrix multiplication: M^t x M and exploit the symmetry 
	dMatrix& operator /= (const dMatrix&); 
	void vSetToUnityMat(); 
	void transp(dMatrix& M); //transposed
	double norm();
}; 


class Matrix {
  public:
    int Nown,Next;
	 cppc* Data;
	 Matrix(int Mown = 1, int Mext = 0);
   Matrix(const Matrix&);
    ~Matrix();
	void Init(int Mown, int Mext); 

	Matrix& operator *= (const double&); 
	Matrix& operator *= (const cppc&);
	Matrix operator * (const cppc& c); 
    Matrix& operator = (const Matrix&);
	void operator = (const cppc&); 
	void operator |= (const cppc&);

    cppc& operator() (int i, int j) const {return Data[i*Next + j];}

    Matrix& operator += (const Matrix&);
    Matrix& operator -= (const Matrix&);

    Vector operator * (const Vector&) const;
    Matrix operator * (const Matrix&);
	void Diag_x_M_x_Diag(bool bInv_l, Vector *VDiag_l, Vector *VDiag_r, bool bInv_r);

    Matrix& operator *= (const Matrix& B) {*this = (*this)*B; return *this;}
    Matrix& operator /= (const Matrix&);
    Matrix& operator %= (const Matrix&);
    Matrix operator + (const Matrix& B)
      {Matrix C(*this); C += B; return C;}
    Matrix operator - (const Matrix& B)
      {Matrix C(*this); C -= B; return C;}
    Matrix operator / (const Matrix& B)
      {Matrix C(*this); C /= B; return C;}
    Matrix operator % (const Matrix& B)
      {Matrix C(*this); C %= B; return C;}

    cppc det();
    Vector PVector(cppc);
	Vector LVector(cppc);

	Matrix hess() const;
	Matrix hessg() const;
	Matrix hess(Matrix& Q) const;
	Matrix hessg(Matrix& Q) const;
	Matrix qrs() const;
	Matrix qrsg() const;
	Matrix qrsg(const Vector& D, Matrix& Q) const;
	Matrix vect(const Vector& D) const;
	Matrix vects(const Vector& D) const;

	Vector diag_() const;
    Vector diag_(Matrix&) const;

	Vector diag() const;
    Vector diag(Matrix&) const;

	Matrix transp();
	//Matrix inv(Matrix& A); 
	
	void vNormalizeColumns(); 
	double norm(); 
	void vSetToUnityMat(); 
};

Matrix MGetUnityMat(int iDim);
void vSet_M_adjoint(Matrix& M, Matrix& M_adj); 


// class MArray:

typedef  Matrix *Mptr; //pointer to a Vector

class MArray { //Vector-Array - resembles SVector a bit
public:
	int Mown; //no. of pointers;
	Mptr Data;

	void Init(){
		Mown = 0; Data = NULL;
	}
	MArray(int  m = 1, int n = 1, int nn = 1); //alloc array of Vectors 
	~MArray();
};


class SMatrix {
  public:
    int Mown;
    Matrix *Data[2][2];

	SMatrix(int n = 1);
    SMatrix(const SMatrix&);
    SMatrix(const SMatrixD&);
    ~SMatrix();

    SMatrix& operator = (const SMatrix&);
    SMatrix& operator = (const SMatrixD&);
    void cut(const SMatrix&);
    Matrix& operator() (int i, int j) const {return *Data[i][j];}
    cppc operator() (int i, int ii, int j, int jj) const
      {return Data[i][j]->Data[Mown*ii+jj];}
    SMatrix operator * (const SMatrix&);
    SMatrix operator & (const SMatrix&);
    SMatrix operator ^ (const SMatrix&);
    SMatrix& operator *= (const SMatrix& B) {return (*this) = (*this)*B;}
    SMatrix& operator &= (const SMatrix& B) {return (*this) = (*this)&B;}
    SMatrix& operator ^= (const SMatrix& B) {return (*this) = (*this)^B;}
    SMatrix operator * (const SMatrixD&);
    SMatrix operator & (const SMatrixD&);
    SMatrix operator ^ (const SMatrixD&);
    SMatrix& operator *= (const SMatrixD& B) {return (*this) = (*this)*B;}
    SMatrix& operator &= (const SMatrixD& B) {return (*this) = (*this)&B;}
    SMatrix& operator ^= (const SMatrixD& B) {return (*this) = (*this)^B;}
    SMatrix operator * (const double);
    SMatrix& operator *= (const double);

    SMatrix ST();
    SMatrix STT();
};

class SMatrixD {
  public:
    int Mown;
    Vector *Data[2][2];

	SMatrixD(int n = 1);
    SMatrixD(const SMatrixD&);
	~SMatrixD();

    cppc operator() (int i, int ii, int j) const
      {return Data[i][j]->Data[ii];}
    Vector operator() (int i, int j) const {return *Data[i][j];}

    SMatrixD& operator = (const SMatrixD&);

    SMatrixD operator * (const SMatrixD&);
    SMatrixD operator & (const SMatrixD&);
    SMatrixD operator ^ (const SMatrixD&);
    SMatrixD& operator *= (const SMatrixD& B) {return (*this) = (*this)*B;}
    SMatrixD& operator &= (const SMatrixD& B) {return (*this) = (*this)&B;}
    SMatrixD& operator ^= (const SMatrixD& B) {return (*this) = (*this)^B;}
    SMatrix operator * (const SMatrix&);
    SMatrix operator & (const SMatrix&);
    SMatrix operator ^ (const SMatrix&);
	 
    SMatrixD ST();
    SMatrixD STT();
};

class QMatrix {
  public:
    cppc Data[2][2];
	QMatrix() {}
	QMatrix(QMatrix& Q) {Data[0][0] = Q.Data[0][0]; Data[0][1] = Q.Data[0][1]; Data[1][0] = Q.Data[1][0]; Data[1][1] = Q.Data[1][1]; };    
	cppc operator() (int i, int j) const {return Data[i][j];}

	QMatrix& operator = (const QMatrix& Q) {Data[0][0] = Q.Data[0][0]; Data[0][1] = Q.Data[0][1]; Data[1][0] = Q.Data[1][0]; Data[1][1] = Q.Data[1][1]; return *this;};    
    QMatrix operator * (const QMatrix&);
    QMatrix operator & (const QMatrix&);
    QMatrix operator ^ (const QMatrix&);
    QMatrix& operator *= (const QMatrix& B) {return (*this) = (*this)*B;}
    QMatrix& operator &= (const QMatrix& B) {return (*this) = (*this)&B;}
    QMatrix& operator ^= (const QMatrix& B) {return (*this) = (*this)^B;}
};




//####################################################  Matrix inversion by Householder by Prof. T.  ############################################



Matrix vr(const Matrix& A, const Matrix& X);   //Householder 



//#############################################################  Code written by W.  ###############################################################

int fNewVect( Vector **V, long iDim );

//FT

void vSetFTinvMatrix(Matrix &FT_inv, Matrix &FT); 


//############################################################# Least Squares Methods ###############################################################

class MinData_double { //data characterizing the data set at a minimum  
public:
	bool bLocMin; 
	dVector dVParam;  dVector dVStandardDev; 
	double dNorm; //merit function value at minimum or envelope at maximum of merit function. 
	//Functions: 
	MinData_double(int Nown = 1);  
	MinData_double(const MinData_double&); 
	void Init(int Nown = 1); 
	MinData_double& operator = (const MinData_double&);
	~MinData_double(); 
};
MinData_double *pAllocMinDataArray(int iNo_MinData, int iDim_OptParam);
void vDeallocMinDataArray(MinData_double **ppMinData); 


struct MinData {
	bool bLocMin;
	Vector VParam;  Vector VStandardDev;
	double dNorm; //merit function value at minimum or envelope at maximum of merit function. 
};

struct sGrid_xy { dVector* pdV_Grid_x; dVector* pdV_Grid_y; };

#define MIN_STEP_DLS 1.0e-14 //stop iterating when relative change of optimization parameters is less than MIN_STEP_DLS  
#define CHANGE_OF_DLS_RESIDUAL 1.0e-6 //residual should change less than CHANGE_OF_DLS_RESIDUAL before stopping damped least squares (DLS)

bool bLevenbergMarquardt_double(dVector& dVx0, bool *bOptParam, dVector* pdVUpperLimit, bool *pbUpperLimitHit, dVector* pdVLowerLimit,
	bool *pbLowerLimitHit, double dMu, bool bLinearityTest, double dTol, int iMinIterDLS, int iMaxIter, double dMin_Chance_DLS_Residual, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, dVector& dVParam, dVector *pdVResult, dMatrix *pdMResult, bool bOptParam[], bool *pSuc), 
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, MinData_double *psResult);

bool bLevenbergMarquardt_double2(dVector& dVx0, bool *bOptParam, dVector* pdVUpperLimit, bool *pbUpperLimitHit, dVector* pdVLowerLimit,
	bool *pbLowerLimitHit, double dMu, bool bLinearityTest, double dTol, int iMinIterDLS, int iMaxIter, double dMin_Chance_DLS_Residual, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, dVector& dVParam, dVector *pdVResult, dMatrix *pdMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, MinData_double *psResult); 

bool bLevenbergMarquardt(Vector Vx0, bool bOptParam[], Vector* pVUpperLimit, dVector *pdVUpperModulus, bool *pbUpperLimitHit, Vector* pVLowerLimit, dVector *pdVLowerModulus,
	bool *pbLowerLimitHit, double dMu, bool bLinearityTest, double dTol, int iMaxIter, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, Vector& VParam, Vector *pVResult, Matrix *pMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, Vector& Vx, Vector *pVxStandardDev, double& pNormVF, bool bPlusComplexConj);

bool bLevenbergMarquardtScalar(Vector Vx0, bool bOptParam[], Vector* pVUpperLimit, dVector *pdVUpperModulus, bool *pbUpperLimitHit, Vector* pVLowerLimit, dVector *pdVLowerModulus,
	bool *pbLowerLimitHit, double dMu, bool bLinearityTest, double dTol, int iMaxIter, int iDimVF_Nown,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, Vector& VParam, double *pdResult, Matrix *pMResult, bool bOptParam[], bool *pSuc),
	void vLimitTheStepSize(Vector &Vx, Vector &Vs, Vector &VxNext, void *pInfoSpectrum, double &dMu),
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, Vector& Vx, Vector *pVxStandardDev, double& dNormF); 


void vPrint_dVec(ofstream & of, char *szText, dVector & dV); 
void vPrint_dMat(ofstream & of, char *szText, dMatrix & dM); 


#endif 