// This file replaces the file "Exports.h" of the NAG-library, which has bveen removed.

//#include "stdafx.h" 
#include "Nag_Replacement.h"
#include "Nag_Compatibility.h" 
#include <math.h>

#include "RBT.Globals.h" 


//My code: 

// Alloc & Free

int fAlloc_I_vect(Integer **iVector, int iDim) //"alloc Integer vector "
{
	int iexit_status = 0;
	*iVector = (Integer *)malloc(iDim*sizeof(int)); 
	if (!*iVector)
		iexit_status = 1;
	return(iexit_status);
}

int fAlloc_i_vect(int **iVector, int iDim) //"alloc int vector "
{
	int iexit_status = 0;
	*iVector = (int *)malloc(iDim*sizeof(int)); 
	if (!*iVector)
		iexit_status = 1;
	return(iexit_status);
}

int fAlloc_d_vect(double **dArray, int iDim) //"alloc double vector "
{
	int iexit_status = 0;
	*dArray = (double *)malloc(iDim*sizeof(double)); 
	if (!*dArray)
		iexit_status = 1;
	return(iexit_status);
}

int fAlloc_c_vect(CComplex **CVector, int iDim) //"alloc complex vector "
{
	int iexit_status = 0;
	*CVector = (CComplex *)malloc(iDim*sizeof(CComplex)); 
	if (!*CVector)
		iexit_status = 1;
	return(iexit_status);
}

int fAlloc_d_sqmatr(double **dMatrix, int iDim) //"alloc double square matrix "
{
	int iexit_status = 0;
	*dMatrix = (double *)malloc(iDim*iDim*sizeof(double)); 
	if (!*dMatrix)
		iexit_status = 1;
	return(iexit_status);
}

int fAlloc_c_sqmatr(CComplex **CMatrix, int iDim) //"alloc complex square matrix "
{
	int iexit_status = 0;
	int isize;
	isize = iDim*iDim*sizeof(CComplex);

	*CMatrix = (CComplex *)malloc(iDim*iDim*sizeof(CComplex));
	if (!*CMatrix)
		iexit_status = 1;
	return(iexit_status);
}

int fAlloc_d_matr(double **dMatrix, int iDim_r, int iDim_c) //"alloc double matrix "
{
	int iexit_status = 0;
	*dMatrix = (double *)malloc(iDim_r*iDim_c*sizeof(double)); 
	if (!*dMatrix)
		iexit_status = 1;
	return(iexit_status);
}

void fFree_I_vect(Integer *iVarName) //"free Integer vector"
{
	free(iVarName); 
}

void fFree_i_vect(int *iVarName) //"free int vector"
{
	free(iVarName); 
}

void fFree_d_vect(double *dVarName) //"free double vector"
{
	free(dVarName);
}

void fFree_c_vect(CComplex *CVarName) //"free complex vector"
{
	free(CVarName); 
}



//CComplex arithmetics:


CComplex fcd(CComplex z1, CComplex z2) //W: "fcd for fcomplex_divide", returns z1/z2
{
	CComplex Cresult;
	double drel, dNenner;

	if (fabs(z2.re) >= fabs(z2.im))
	{
		drel = z2.im/z2.re;
		dNenner = z2.re+drel*z2.im;
		Cresult.re = (z1.re+drel*z1.im)/dNenner;
		Cresult.im = (z1.im-drel*z1.re)/dNenner;
	}
	else
	{
		drel = z2.re/z2.im;
		dNenner = z2.im+drel*z2.re;
		Cresult.re = (z1.re*drel+z1.im)/dNenner;
		Cresult.im = (z1.im*drel-z1.re)/dNenner;
	}

	return Cresult;
}


double fcre(CComplex z) //"fc real"
{
	return z.re;
}


double fcim(CComplex z) //"fc imag"
{
	return z.im;
}


CComplex fc(double x, double y) //"fcomplex", W.: returns (x + i * y)
{
	CComplex CNumber;

	CNumber.re = x;
	CNumber.im = y;
	return CNumber;
}


CComplex fca(CComplex z1, CComplex z2) //"fc add", returns z1+z2
{
	CComplex Csum;

	Csum.re = z1.re+z2.re; 
	Csum.im = z1.im+z2.im; 
	return Csum;
}


CComplex fcs(CComplex z1, CComplex z2) //"fc subtract", returns z1-z2
{
	CComplex Cdiff;

	Cdiff.re = z1.re-z2.re; 
	Cdiff.im = z1.im-z2.im; 
	return Cdiff;
}


CComplex fcm(CComplex z1, CComplex z2) //"fc multiply", returns z1*z2
{
	CComplex Cresult;

	Cresult.re = z1.re*z2.re - z1.im*z2.im;
	Cresult.im = z1.re*z2.im + z1.im*z2.re;
	return Cresult;
}


double fcabs(CComplex z)//"fcomplex abs"
{
	double dAbs;
	double dx, dy, dtemp;

	dx = fabs(z.re);  dy = fabs(z.im);
	if (dx == 0)
		dAbs = dy;
	else if (dy == 0)
		dAbs = dx;
	else if (dx > dy)
	{
		dtemp = dy/dx;
		dAbs = dx*sqrt(1.0+dtemp*dtemp);
	}
	else
	{
		dtemp = dx/dy;
		dAbs = dy*sqrt(1.0+dtemp*dtemp);
	}

	return dAbs;
}


double frsq (double x) //f real square
{
	return x*x;
}


CComplex fcsq (CComplex x) //f complex square
{
	return fcm(x, x);
}


double fcmodsq(CComplex x) //"f complex modulus squared": modulus of complex number squared
{
	return  x.re * x.re  +  x.im * x.im;
}


CComplex fcipow(CComplex z, double i) //"fcomplex i power", calc. of z^î
{
	CComplex Cresult;
	double dBetrag, dPhase, dDummy1, dDummy2;

	dBetrag = fcabs(z);
	if (fcre(z) == 0) // z rein imaginär
	  {
	    if (fcim(z) > 0) // z positiv imaginär
	      dPhase = PI/2;
	    else if (fcim(z) < 0) // z negativ imaginär
	      dPhase = -PI/2;
	    else // z = 0
	      dPhase = 0;
	  }
	else // z nicht rein imaginär
	  dPhase = atan2(z.im,z.re);	
	dDummy1 = pow(dBetrag, i); 
	dDummy2 = dPhase * i;
	Cresult.re = dDummy1*cos(dDummy2);
	Cresult.im = dDummy1*sin(dDummy2);

	return Cresult;
}


CComplex fcsqrt(CComplex z) //"fcomplex square root".  fcsqrt(-1 + 0*i) = i,  fcsqrt(-1 + 10^-16 * i) = i,  fcsqrt(-1 - 10^-16 * i) = -i.
{
	CComplex Cresult;
	double x,y,w,r;
	
	// aus Numerical Recipes:
	if ((z.re == 0.0) && (z.im == 0.0))
	{
		Cresult.re=0.0;
		Cresult.im=0.0;
	} 
	else 
	{
		x=fabs(z.re);
		y=fabs(z.im);
		if (x >= y) 
		{
			r=y/x;
			w=sqrt(x)*sqrt(0.5*(1.0+sqrt(1.0+r*r)));
		} 
		else 
		{
			r=x/y;
			w=sqrt(y)*sqrt(0.5*(r+sqrt(1.0+r*r)));
		}
		if (z.re >= 0.0) 
		{
			Cresult.re=w;
			Cresult.im=z.im/(2.0*w);
		} else {
			Cresult.im=(z.im >= 0) ? w : -w;
			Cresult.re=z.im/(2.0*Cresult.im);
		}
    }

	return Cresult;
}


CComplex fcexp(CComplex z) //"fcomplex exp"
{
	double dExp = exp(z.re);
	CComplex CTemp, CResult;

	// aus Numerical Recipes
	CTemp.re = cos(z.im),  CTemp.im = sin(z.im);
	CResult.re = dExp * CTemp.re,  CResult.im = dExp * CTemp.im;  
	
	return CResult;
}


CComplex fcconj(CComplex z) //"fcomplex conjugated"
{
	CComplex CResult;

	CResult.re = z.re,  CResult.im = -z.im;
	return CResult;
}


double fcarg(CComplex z) //"fcomplex argument"
{
	return atan2(z.im, z.re);
}


void fSwap(CComplex *Ca, CComplex *Cb)
{
	CComplex CTemp;

	CTemp = *Ca;  *Ca = *Cb;  *Cb = CTemp;
}

int iSign(int i)
{
	if (i >= 0)
		return 1;
	else
		return -1;
}

int iSign(double i)
{
	if (i >= 0)
		return 1;
	else
		return -1;
}

double dSign(double dx)
{
	if (dx >= 0)
		return 1.0;
	else
		return -1.0;  
}


double dSqrt(double dx) //For cases in which dx < 0.0 should never happen, but might be slightly smaller than 0.0 due to numeric noise 
{
	if (dx <= 0.0)
		return 0.0;
	else
		return sqrt(dx);  
}




//##############################  Lin. algebra: Part 1: Fast functions for small matrices (e.g. 2x2-matrices) ###################################


int fInvert2x2CMatrix( CComplex CM[2][2], CComplex CInv[2][2], char *szText, int iNoChar )
//This function inverts a complex 2 x 2 matrixCM; result: CInv.  W., 30.12.12
{
	int ireturn_status = 0, iexit_status = 0;
	CComplex CFact,  CDet = fcs(  fcm(CM[0][0], CM[1][1]),  fcm(CM[1][0], CM[0][1])  );

	if( fcabs(CDet) < sqrt(NUM_NOISE) )
		ireturn_status = 1;
	TESTSUCCES_NAG_COMPATIBILITY( Error in fInvert2x2CMatrix: CDet is close to zero. Abort. \n )

	CFact = fcd( fc(1,0), CDet );

	CInv[0][0] = CM[1][1];								CInv[0][1] = fc( -CM[0][1].re, -CM[0][1].im );
	CInv[1][0] = fc( -CM[1][0].re, -CM[1][0].im );		CInv[1][1] = CM[0][0];

	CInv[0][0] = fcm(CFact, CInv[0][0]);  CInv[0][1] = fcm(CFact, CInv[0][1]);  
	CInv[1][0] = fcm(CFact, CInv[1][0]);  CInv[1][1] = fcm(CFact, CInv[1][1]);

END: return iexit_status;
}


void C2x2MatrTimes2x2Matr (CComplex C1[2][2], CComplex C2[2][2], CComplex CRes[2][2])
//This function computes  CRes = C1 * C2.   W., 30.12.12
{
	int m, n;

	for(m=0; m<2; m++)
		for(n=0; n<2; n++)
			CRes[m][n]   =   fca(   fcm(C1[m][0], C2[0][n]),   fcm(C1[m][1], C2[1][n])   );
}



//Prof T.'s code: 


//matrix inv. 
#include "MATRIX.H"

int fInvertComplexMatrix( CComplex CMatrixPointer[], int iDim, char *szText, int iNoChar )
{
	BOOL bUseHouseholder = TRUE, bTest = TRUE; //Householder or Gauss; Check result of inversion: yes/no 
	int iIndex, iexit_status = 0, i, j; 
	Matrix MIn(iDim, iDim), MUnity(iDim, iDim), MOut(iDim, iDim);
	CComplex  *CTestInp = new CComplex [iDim*iDim],  *CTestResult = new CComplex [iDim*iDim]; 
	double dNorm = 0.0; //calc of 2-norm of A * A^-1 as a check 

	if(bTest)
		{ CTestInp = new CComplex [iDim*iDim],  CTestResult = new CComplex [iDim*iDim];  memcpy( CTestInp, CMatrixPointer, iDim*iDim*sizeof(CComplex) ); }   
	
	for(i=0; i<iDim; i++)
		MUnity.Data[i*iDim + i] = cOne;
	//copy to MIn:
	for(i=0; i<iDim; i++)
		for(j=0; j<iDim; j++)
		{
			iIndex = i*iDim + j;
			MIn.Data[iIndex].real(CMatrixPointer[iIndex].re),  MIn.Data[iIndex].imag(CMatrixPointer[iIndex].im); 
		}

	if(bUseHouseholder)
		MOut = vr(MIn, MUnity); //matrix inversion by Householder method
	else
		MOut = MUnity/MIn; //matrix inversion by Gauss method 
	
	//Copy from MOut:
	for(i=0; i<iDim; i++)
		for(j=0; j<iDim; j++)
		{
			iIndex = i*iDim + j;
			CMatrixPointer[iIndex].re = MOut.Data[iIndex].real(),  CMatrixPointer[iIndex].im = MOut.Data[iIndex].imag(); 
		}

	if(bTest) //Check in the following how accurately the inverse has been computed: 
	{
		fMultiplySquareComplexMatrices( CTestInp, CMatrixPointer, CTestResult, iDim ); //A * A^-1 
		for(i=0; i<iDim; i++)
			CTestResult[i*iDim+i].re -= 1.0; //subtract unity matrix, result should be zero matrix
		for(i=0; i<iDim; i++)
			for(j=0; j<iDim; j++)
				{ iIndex = i*iDim+j;  dNorm += CTestResult[iIndex].re * CTestResult[iIndex].re + CTestResult[iIndex].im * CTestResult[iIndex].im; } 
		dNorm = sqrt(dNorm); 
		//fprintf( Fres, "\n Check of matrix inversion: 2-Norm of inverse matrix of dimension %d * %d = %+18.15g \n", iDim, iDim, dNorm ); 
		if(dNorm > 1.0e-5)
		{
            iexit_status = 1; 
            //fprintf( Fres, "\n Error in matrix inversion, function fInvertComplexMatrix(...): Accuracy is less than 1.0e-5! Abandonment! \n"); 
        }
	}//end if bTest

	if(bTest)
		{ delete [] CTestInp,  delete [] CTestResult; }  
	return iexit_status;
}


//matrix matrix mult.
void fMultiplySquareComplexMatrices(CComplex *CInput1, CComplex *CInput2, CComplex *COutput, int iDim)
{
	int i, j; 
	Matrix MIn1(iDim, iDim), MIn2(iDim, iDim), MOut(iDim, iDim);

	//copy to MIn:
	for(i=0; i<iDim; i++)
	{
		for(j=0; j<iDim; j++)
		{
			int iIndex = i*iDim + j;
			MIn1.Data[iIndex].real(CInput1[iIndex].re),  MIn1.Data[iIndex].imag(CInput1[iIndex].im); 
			MIn2.Data[iIndex].real(CInput2[iIndex].re),  MIn2.Data[iIndex].imag(CInput2[iIndex].im); 
		}
	}
	MOut = MIn1 * MIn2; //matrix mult.
	for(i=0; i<iDim; i++)
	{
		for(j=0; j<iDim; j++)
		{
			int iIndex = i*iDim + j;
			COutput[iIndex].re = MOut.Data[iIndex].real(),  COutput[iIndex].im = MOut.Data[iIndex].imag(); 
		}
	}
}


//matrix vector product:
void fCMatrVectProd( CComplex *CMatrixInput, CComplex *CVectorInput, CComplex *CVectorOutput, int iDim ) // komplexes Matrix-Vektor Produkt
{
	int i, j; 
	CComplex  *COut = (CComplex *)malloc( iDim*sizeof(CComplex) ),  *CTmpVecIn = NULL,  CResTmp; //buffer final result in COut, before overwriting CVectorOutput 
	//and possibly CVectorInput!

	for(i=0; i<iDim; i++) //for all lines
	{
		CTmpVecIn = CVectorInput;  CResTmp.re = 0.0,  CResTmp.im = 0.0; //next line, reset input vector  
		for(j=0; j<iDim; j++) //for all columns
		{
			CResTmp = fca( CResTmp, fcm(*CMatrixInput,*CTmpVecIn) );
			CMatrixInput++; CTmpVecIn++; 
		}
		COut[i] = CResTmp; //store result for the line
	}
	memcpy( CVectorOutput, COut, iDim*sizeof(CComplex) );  
	free(COut);
}


 
#include "FFT.h" 

//FFT
int fFFT_Real( int iDim, double *dVec, CComplex *CVec, char *szText, int iNoChar )
{
	int i, iexit_status = 0;
	Complex *CIn = new Complex [iDim], *COut = new Complex [iDim]; 

	int iPower = 0, iSize = 1; //TEST17  2^iPower = iSize
	while (iSize < iDim && iPower < 1000)
		iSize *= 2, iPower++;
	if (iPower >= 1000 || iSize != iDim)
	{
		cout << endl << "ERROR IN FFT !" << endl; iexit_status = 1; return iexit_status;
	}
	FFT Data(2, iPower); 

	for(i=0; i<iDim; i++)
		{ CIn[i].real(dVec[i]),  CIn[i].imag(0.0); } 
	Data.d(CIn, COut); 
	for(i=0; i<iDim; i++) 
		{ CVec[i].re = COut[i].real(),  CVec[i].im = -COut[i].imag(); } 

	delete[] CIn,  delete[] COut; 
	return iexit_status;
}


bool bSetFFTDim(int& iDim, int& iPower)
{
	int iSize = 1; iPower = 0; //TEST17  2^iPower = iSize
	while (iSize < iDim && iPower < 1000) 
		iSize *= 2, iPower++;
	if (iPower >= 1000)
	{
		cout << endl << "ERROR IN FFT !" << endl;  return false; 
	} 
	else {
		iDim = iSize;  return true; 
	}
}



int fFFT_cppc(BOOL bInverse, unsigned int iDim, Complex *pIn, double *pDoubleIn, Complex *pOut, double *pDoubleOut, BOOL bSwapOutput)
//Check and debug this! W., 13.9.2017
{
	bool bErrorInImagPart = false;
	int iexit_status = 0;
	unsigned int i;
	Complex *CIn = new Complex[iDim], *COut = new Complex[iDim];

	unsigned int iPower = 0, iSize = 1; //TEST17  2^iPower = iSize
	while (iSize < iDim && iPower < 1000)
		iSize *= 2, iPower++;
	if (iPower >= 1000 || iSize != iDim)
	{
		//cout << endl << "ERROR IN FFT !" << endl; 
		iexit_status = 1; return iexit_status;
	}
	FFT Data(2, iPower);
	//void fPrint_c_matr(CComplex *CMatr, int iNoRows, int iNoColumns, char *szText, BOOL bOutput);
	//fPrint_c_matr(CData, 1, iDim, "FFT-input: \n ", TRUE);  //TEST17
	if (pIn)
		for (i = 0; i<iDim; i++)
			CIn[i] = conj(pIn[i]);  //CIn[i].real(pIn[i].real()), CIn[i].imag(-pIn[i].imag()); //TEST17 
	else
		for (i = 0; i<iDim; i++)
			CIn[i] = pDoubleIn[i]; 

	if (bInverse) {
		Data.i(CIn, COut);
		for (i = 0; i < iDim; i++)
			COut[i] *= iDim; 
	}
	else {
		Data.d(CIn, COut); 
		for (i = 0; i < iDim; i++)
			COut[i] /= iDim;
	}

	if(pOut)
		if (bSwapOutput) 
			for (i = 0; i < iDim / 2; i++) {
				pOut[i] = conj(COut[iDim / 2 + i]);  pOut[iDim / 2 + i] = conj(COut[i]); 
			}
		else
			for (i = 0; i<iDim; i++) 
				pOut[i] = conj(COut[i]);  //VDataOut.Data[i].real(COut[i].real()), VDataOut.Data[i].imag(-COut[i].imag());
	else
		if (bSwapOutput)
			for (i = 0; i < iDim / 2; i++) {
				pDoubleOut[i] = COut[iDim / 2 + i].real();  pDoubleOut[iDim / 2 + i] = COut[i].real();
				if (  abs(COut[i].imag()) > 2.0e-4 * abs(COut[i].real())  ||  abs(COut[iDim / 2 + i].imag()) > NUM_NOISE_FLOAT * abs(COut[iDim / 2 + i].real())  )
					bErrorInImagPart = true;
			}
		else 
			for (i = 0; i < iDim; i++) {
				pDoubleOut[i] = COut[i].real(); 
				if (abs(COut[i].imag()) > 2.0e-4 * abs(COut[i].real()))
					bErrorInImagPart = true;
			}
	//if(bErrorInImagPart)
	//	cout << endl << " WARNING IN FFT: imag part > 2.0e-4 * real part in real FFT!" << endl;

	delete[] CIn; delete[] COut;
	//fPrint_c_matr(CData, 1, iDim, "FFT-output: \n ", TRUE);  //TEST17 
	return iexit_status;
} 



int fFFT_Compl ( BOOL bInverse, long iDim, CComplex *CData, char *szText, int iNoChar )
{
	int i, iexit_status = 0; 
	Complex *CIn = new Complex [iDim],  *COut = new Complex [iDim]; 

	int iPower = 0, iSize = 1; //TEST17  2^iPower = iSize
	while (iSize < iDim && iPower < 1000)
		iSize *= 2, iPower++;
	if (iPower >= 1000 || iSize != iDim)
	{
		cout << endl << "ERROR IN FFT !" << endl; iexit_status = 1; return iexit_status;
	}
	FFT Data(2, iPower); 
	//void fPrint_c_matr(CComplex *CMatr, int iNoRows, int iNoColumns, char *szText, BOOL bOutput);
	//fPrint_c_matr(CData, 1, iDim, "FFT-input: \n ", TRUE);  //TEST17

	for(i=0; i<iDim; i++)
		{ CIn[i].real(CData[i].re),  CIn[i].imag(-CData[i].im); } //TEST17
	if(bInverse)	
		Data.i(CIn, COut); 
	else
		Data.d(CIn, COut); 
	for(i=0; i<iDim; i++) 
		{ CData[i].re = COut[i].real(),  CData[i].im = -COut[i].imag(); } 

	delete[] CIn; delete[] COut; 
	//fPrint_c_matr(CData, 1, iDim, "FFT-output: \n ", TRUE);  //TEST17
	return iexit_status;
}


int fFFT_Compl_Hanke ( BOOL bInverse, long iDim, double *dBufRe, double *dBufIm, CComplex *CData, char *szText, int iNoChar )
{ 
	int iexit_status = fFFT_Compl( bInverse, iDim, CData, szText, iNoChar ); 
	//if (bInverse) //iFFT:
	//{
	//	double dFac = 1.0 / (double)iDim; //(eq. 53.2, Hanke: "1/N")
	//	for (int i = 0; i < iDim; i++)
	//	{
	//		CData[i].re *= dFac;  CData[i].im *= dFac;
	//	}
	//}
	//else //FFT: 
	//	dFac = (double)iDim; //(eq. (53.2), Hanke: no "1/N") 
	
	return iexit_status;
} 


