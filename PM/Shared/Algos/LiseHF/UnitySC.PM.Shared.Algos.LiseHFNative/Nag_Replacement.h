// This file replaces the file "Exports.h" of the NAG-library, which has bveen removed.

#ifndef NAG_REPLACEMENT
#define NAG_REPLACEMENT

#include "Nag_Compatibility.h" 
#include "FFT.h" 


//My code: 

// Alloc & Free
int fAlloc_d_vect(double **dArray, int iDim);
int fAlloc_c_vect(CComplex **CVector, int iDim);
int fAlloc_d_sqmatr(double **dMatrix, int iDim);
int fAlloc_c_sqmatr(CComplex **CMatrix, int iDim);
int fAlloc_I_vect(Integer **iVector, int iDim);
void fFree_I_vect(Integer *iVarName);
void fFree_d_vect(double *dVarName);
void fFree_c_vect(CComplex *CVarName);
int fAlloc_d_matr(double **dMatrix, int iDim_r, int iDim_c);
int fAlloc_i_vect(int **iVector, int iDim);
void fFree_i_vect(int *iVarName);

//CComplex arithmetics:
CComplex fcd(CComplex z1, CComplex z2); //W: "fcd for fcomplex_divide", returns z1/z2
double fcre(CComplex z); //"fc real"
double fcim(CComplex z); //"fc imag"
CComplex fc(double x, double y); //"fcomplex", W.: returns (x + i * y)
CComplex fca(CComplex z1, CComplex z2); //"fc add", returns z1+z2
CComplex fcs(CComplex z1, CComplex z2); //"fc subtract", returns z1-z2
CComplex fcm(CComplex z1, CComplex z2); //"fc multiply", returns z1*z2
double fcabs(CComplex z); //"fcomplex abs"
double frsq (double x); //f real square
CComplex fcsq (CComplex x); //f complex square
double fcmodsq(CComplex x); //"f complex modulus squared": modulus of complex number squared
CComplex fcipow(CComplex z, double i); //"fcomplex i power", calc. of z^î
CComplex fcsqrt(CComplex z); //"fcomplex square root".  fcsqrt(-1 + 0*i) = i,  fcsqrt(-1 + 10^-16 * i) = i,  fcsqrt(-1 - 10^-16 * i) = -i.
CComplex fcexp(CComplex z); //"fcomplex exp"
CComplex fcconj(CComplex z); //"fcomplex conjugated"
double fcarg(CComplex z); //"fcomplex argument"
void fSwap(CComplex *Ca, CComplex *Cb);
int iSign(int i); 
int iSign(double i); 
double dSign(double dx); 
double dSqrt(double dx);  

//Fast functions for small matrices:
int fInvert2x2CMatrix( CComplex CM[2][2], CComplex CInv[2][2], char *szText, int iNoChar );
void C2x2MatrTimes2x2Matr (CComplex C1[2][2], CComplex C2[2][2], CComplex CRes[2][2]);


//Prof T.'s code: 

//matrix inv.
int fInvertComplexMatrix(CComplex CMatrixPointer[], int iDim, char *szText, int iNoChar );

//matrix matrix mult.
void fMultiplySquareComplexMatrices(CComplex *CInput1, CComplex *CInput2, CComplex *COutput, int iDim);

//matrix vector product:
void fCMatrVectProd(CComplex *CMatrixInput, CComplex *CVectorInput, CComplex *CVectorOutput, int iDim); // komplexes Matrix-Vektor Produkt

//FFT
int fFFT_Real( int iDim, double *dVec, CComplex *CVec, char *szText, int iNoChar );
bool bSetFFTDim(int& iDim, int& iPower); 
int fFFT_cppc(BOOL bInverse, unsigned int iDim, Complex *pIn, double *pDoubleIn, Complex *pOut, double *pDoubleOut, BOOL bSwapOutput); 
int fFFT_Compl ( BOOL bInverse, long iDim, CComplex *CData, char *szText, int iNoChar ); 
int fFFT_Compl_Hanke ( BOOL bInverse, long iDim, double *dBufRe, double *dBufIm, CComplex *CData, char *szText, int iNoChar ); 



#endif NAG_REPLACEMENT