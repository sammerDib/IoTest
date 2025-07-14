//This file includes all headers required in the entire project and is the only one to be included into the ~.c and ~.cpp files 

#ifndef RBT_H //W.Iff: um mehrmaliges Einbinden zu verhindern



// Includes:

//a) standard headers:
#include "AllWinHeaders.h"
//b) project specific headers:
#include "Defines.h"
#include "Nag_Compatibility.h" //definition of CComplex 
#include "Nag_Replacement.h" //from basic project: def of CComplex (Compatibility.h) etc, mathematical tools 
#include "RBT.Defines.h" //#define DLL etc
#include "MATRIX.H" //data type VArray
#include "RBT.Typedefs.h" //structs
#include "RBT.Globals.h" //glob vars
//#include "Splines.h" //spline basis fu. 
//#include "Tschebycheff.h" //Tschebycheff poly





	/* functions for RBT */

	int iRigoDiff(  int iUnitOfAllLengthes, int iUnitOfAllAngles, Radiation sRadiation, double dLengthOfPeriod, 
						RBTList_R sRBTList, int iDim, BOOL bDampGibbsPhenomenon, 
						RBTReturnDataptr pReturnData, BOOL bOutputIntoZeissFile, char *szPathZeissResultFile, 
						BOOL bDetermineEigensystemAndDetOfCReflAndCTrans, BOOL bWriteEigenvectorsToFile, 
						BOOL (*pPointerTobCalcIncAndExitRayCoordFrameForRBT) (double dxDirectionRBT[3], double dNormal[3], 
							   BOOL bTransmission, double de0Inc[3], double de0Out[3],
							  double dsInc[3], double dpInc[3], double dsOut[3], double dpOut[3]), 
						BOOL bUseRayBasedCoordSystFromPolRaytr  ); //called by main()

	/* Alloc */
//	int fAlloc();
//	int fAlloc_CPropF_and_NiNj();
//	int fAlloc_mem_for_total_output();

	/* Berechnungsfunktion */
	int fCalcRBT(); // Berechnungsfunktion RBT
	int fCalcCaeh(); //Berechnungsfunktion; calc. without matrices: only Caeh

	/* Init */
	void fSetSqMatrToZero (CComplex *CMatr, int iDim);	void fSetSqMatrToUnity (CComplex *CMatr, int iDim);
	void fSetCVectToZero( CComplex *CV, long iDim );
	void fSetCVect( cppc cVal, long iDim );
	void fSetdVectToZero( double *dV, long iDim );
	void fExtractCSubMatr(CComplex *CMatr, CComplex *CSubMatr, int iDimLarge, int iDimSmall);
	void fInitConstants();
	void fInitDimensions(int iDim);
	int fInitPsiAndPsiInv (CComplex *Cbeta, CComplex CRefrInd, CComplex CDielF, BOOL bAlsoInvPsi,
						CComplex *Cp, CComplex *Cqe, CComplex *Cqh,  CComplex *CFakEz, CComplex *CFakHz, CComplex *C1_div_2qe, CComplex *C1_div_2qh );
	int fInitCalc(void);
	int fBWInit(void);
	int fCalcCbeta ( CComplex *CRefrInd, CComplex *Cbeta, CComplex *pDataToReturn_CKy ); //W., 18.8.12
	int fBWProj();
	void fBubbleSort(double *dVect, int iDim, int *iNoEqualElements);
	void fCalcForbiddenEpsilons();
	BOOL fIsEpsilonForbidden(CComplex CEps);
	int fAllocAndReadExactSolution( CComplex **CEz_T_Exact, CComplex **CHz_T_Exact,  CComplex **CEz_R_Exact, CComplex **CHz_R_Exact, int iDim );
	int fInput_CA_Ez_CA_Hz_CB_Ez_CB_Hz( char *szPath, CComplex *CA_Ez, CComplex *CA_Hz, CComplex *CB_Ez, CComplex *CB_Hz); //W., 17.7.12

	/* PropMat */
	void fInitCPropF();
	int fGetPropMat_bin_1DCM( int iCurLay, CComplex *CExp );
	void fCalcPropMatrInRayleighBasis ( CComplex *CExp, CComplex *CDiag_Rayleigh);
	void fFourierTrans_NP(int iType); // Fouriertrafo komplexe Brechungsindizes
	void fFourierTrans_Mbin(int iType, int iCurLay); // Fouriertrafo komplexe Brechungsindizes
	void fFourierTrans_Mult(int iType, int iDim, double *dxOrd, CComplex *CRes); // Fouriertrafo komplexe Brechungsindizes
	void fFourierTrans_Mult_Apertures(double *dxOrd, VArray *VA_Res); 

	//Psi-Matrizen, SG thesis, section 2.6
	void fPsi_Times_Vect ( Vector Vp, Vector Vqe, Vector Vqh, 
					   Vector Vect1, Vector Vect2, Vector Vect3, Vector Vect4, 
					   Vector *VRes1, Vector *VRes2, Vector *VRes3, Vector *VRes4 ); 
	int fPsi_Times_Caeh(int iMedium); //W: functions only for field vector, not for T-matrices implemented
	void fCMatr_Times_Psi (CComplex *CMatr, CComplex *Cp, CComplex *Cqe, CComplex *Cqh);
	int fInvPsi_Times_Vect(int iMedium, CComplex *CVect);
	int fInvPsi_Times_Caeh(int iMedium);
	void fCMatr_Times_InvPsi (CComplex *CMatr, CComplex *C1_div_2qe, CComplex *C1_div_2qh, CComplex *Cp_div_qe, CComplex *Cp_div_qh);
	void fInvPsi_Times_CMatr(CComplex *CMatr, CComplex *C1_div_2qe, CComplex *C1_div_2qh, CComplex *Cp_div_qe, CComplex *Cp_div_qh);
	void fPsi_Times_CMatr( CComplex *CpVect, CComplex *CqeVect, CComplex *CqhVect, CComplex *CM );
	int fNewPsiTimesVect( CComplex *CTe1,  CComplex *CTe2,  CComplex *CTm1,  CComplex *CTm2,  CComplex *Ca);
	int fMatrixTimesNewPsi( CComplex *CMat,  CComplex *CTe1,  CComplex *CTe2,  CComplex *CTm1,  CComplex *CTm2 );
	int fNewInvPsiTimesVect( CComplex *CTe1,  CComplex *CTe2,  CComplex *CTm1,  CComplex *CTm2,  CComplex *Ceh);
	int fNewPsiInvTimesMatrix( CComplex *CTe1,  CComplex *CTe2,  CComplex *CTm1,  CComplex *CTm2, CComplex *CMat );
	int fWritePsiAndInvPsiToFile( char *szPath, CComplex *CTe1, CComplex *CTe2, CComplex *CTm1, CComplex *CTm2,  
								CComplex *CInvTe1, CComplex *CInvTe2, CComplex *CInvTm1, CComplex *CInvTm2  );
	int fReadPsiAndInvPsiFromFile( char *szPath, CComplex *CTe1, CComplex *CTe2, CComplex *CTm1, CComplex *CTm2,  
								CComplex *CInvTe1, CComplex *CInvTe2, CComplex *CInvTm1, CComplex *CInvTm2  );
	int fWriteDiagsToFile( char *szPath, CComplex *CEH, CComplex *CDH, CComplex *CRayl );
	int fReadDiagsFromFile( char *szPath, CComplex *CEH, CComplex *CDH, CComplex *CRayl );
	int fDiagonaliseWithinDiffOrders( CComplex *CMatr, char *szPathPsiFile, char *szPropMatrName, double y, BOOL bEH_Field  );
	void fExpTimesVect(CComplex *CExp, double dDelta_y, CComplex *CVect, int iDim); //e^(CExp*dDelta_y)*CVect. W., 13.8.12
	int fSetDiscretisPoints(CComplex CEps1, CComplex CEps2, CComplex **CRes, int *iSizeOfCRes);
	int fCalcDiscretisForEpsilon( CComplex CEps1, CComplex CEps2, CComplex **CRes, int *iSizeOfCRes );
	int fTestPsiAndInvPsi();
	int fGetCPFForBasisMedChange (CComplex CEpsStart, CComplex CEpsStop, double dDeltaEta, BOOL bCalcDerivOfT );
	int fChangeBasisMed(CComplex CEpsStart, CComplex CEpsStop);
	int fCalcSMatrFresnel( CComplex CEpsBelow, CComplex CEpsAbove, BOOL bIncFromBelow, BOOL bIncFromAbove, 
					   CComplex *CRes_dd, CComplex *CRes_ud, CComplex *CRes_du, CComplex *CRes_uu );
	void f2x2BlockDiag_x_MatrOrVect( CComplex *CA, CComplex *Cv, int iNoCols_v, CComplex *CRes );
	void fMatrOrVect_x_2x2BlockDiag( CComplex *Cv, int iNoLns_v, CComplex *CA, CComplex *CRes );
	int fAddFresnelScatSurfToS_g( CComplex CEpsBelow, CComplex CEpsAbove );

	/* Propagation */
	void fOutputLayerNoAndSLayerNo(int L, int s); //output of current L and s
	int fPropagate_Caeh_1Layer(int L, double *dymesh); //propagation of Caeh for 1 layer
	int fPropagate_Caeh_1Layer_inv(int L, double *dymesh); //propagation of Caeh for 1 layer in opposite direction
	int fPropagate_CM(); // Propagation Konisch
	int fPropagate_Caeh(); //only for propagat. of vector of Rayleigh-coeff.
	void fCopySReflAndSTrans();

	/* Integration routines */
	void fInitCFInt();
	void fInitS();
	int fIntegrate_Caeh_bin(); //W.: for binary profiles
	void fSetSijFromS( CComplex **CSuu,  CComplex **CSud,  CComplex **CSdu,  CComplex **CSdd,  CComplex *CS );
	void fOutputSMatrBlocks( CComplex *CSdd, CComplex *CSdu, CComplex *CSud, CComplex *CSuu, char *szText );
	int CalcDerivOfSWithNAG( CComplex *CSdd, CComplex *CSud, CComplex *CSuu, 
						 CComplex *CDerivSdd, CComplex *CDerivSdu, CComplex *CDerivSud, CComplex *CDerivSuu );
	int fGetPropMat( double y, int iCurLay, CComplex *CExp );
	int iCalcDerivOfS_Or_T( double y, BOOL bFirstCalc, BOOL bNew_y_Val, void *vExtraInfo, CComplex *CFuVal, CComplex *CDeriv );
	int fCalcDerivForInt( CComplex *CFuVal, CComplex *CDeriv );
	int fIntegrate_IRK_bin ( double dIntStart, double dIntEnd, int iCurLay ); // IRK für binäre Profile
	int fIntegrate_Sud_and_Sdd_IRK2 ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay, CComplex *CS );
	int fInt_T_IRK2 ( CComplex *CF, CComplex *CKTemp, BOOL bFirstStep, double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay );
	int fInt_T_IRK_Extrapol ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay );
	int fIntegrate_IRK(double dIntStart, double dIntEnd, int iCurLay);
	int fExpEuler1 ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay );
	int fImprovedEuler ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay );
	int fIntegrate ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay, BOOL bChangeBasisMed, CComplex *CEpsStart, CComplex *CEpsStop );
	int fIntegrate4SBlocks ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay, BOOL bStartWithUnity );
	int fClassRK_InGeneral( double dIntStart, double dIntEnd, int iIRKsteps, void *vExtraInfo, CComplex *CFuVal, long lDim, 
			  void (*vSetFuToUnity)(CComplex *CFuVal), 
			  int (*iCalcDeriv)(double y, BOOL bFirstCalc, void *vExtraInfo, CComplex *CFuVal, CComplex *CDeriv) );
	int fImprovedEuler_InGeneral( double dIntStart, double dIntEnd, int iIRKsteps, void *vExtraInfo, CComplex *CFuVal, long lDim, 
			  int (*iCalcDeriv)(double y, BOOL bFirstCalc, BOOL bNew_y_Val, void *vExtraInfo, CComplex *CFuVal, CComplex *CDeriv) );
	int fProp_ExpEuler1WithExp ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay );
	int f2ndOrderTMatrixExp ( double dDelta_y, CComplex *CDiag, CComplex *CFIntTmp );
	int fProp_IRK4WithExp ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay );
	int fInt_Sud_Sdd_ExplEuler1WithExp( CComplex *CS_ud, CComplex *CS_dd, double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay, double dEta );
	int fSud_Sdd_ViaExtrapol( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay );
	int fInt_Sud_and_Sdd_IRK2_DmpNewton ( double dIntStart, double dIntEnd, int iIRKsteps, int iCurLay );
	cppc fIntSimpson(double dxStart, double dxStop, double *dx, cppc *cf, int iNoPoints); 


	/* Berechnung Felder */
	int fCalc_ExHxEyHy_from_EzHz( CComplex *CEz, CComplex *CHz, BOOL bUpwards, int iMedium,
							  CComplex *CEx, CComplex *CEy, CComplex *CHx, CComplex *CHy );
	int fDiffrctdAmp_NP_1DCM_SMatrix();

	//Ausgabe Felder
	void fWrite_Rayl_z_coeff_ASCII ( CComplex *CA_Ez, CComplex *CA_Hz, CComplex *CB_Ez, CComplex *CB_Hz ); //W., 21.7.12
	int fOutput_CA_Ez_CA_Hz_CB_Ez_CB_Hz( char *szPath, CComplex *CA_Ez, CComplex *CA_Hz, CComplex *CB_Ez, CComplex *CB_Hz ); //W., 17.7.12 
	
	/* Dealloc */
	//void fDealloc(); // Deallokierung
	//void fDealloc_CPropF_and_NiNj();
	//void fDealloc_mem_for_total_output();

	/* Mathematik */
	/* Lineare Algebra */
	//Matrices
	cppc fcppc(double re, double im);
	cppc fC_To_cppc(CComplex CIn);
	CComplex fcppc_To_C(cppc cIn);
	void vMatrixIdentReal( double *dpSource, double *dpDestination, int iDim); // Identität zweier reeller Matrizen (quadratisch)
	void vMatrixIdentComplex( CComplex *CSource,  CComplex *CDestination, int iDim); // Identität zweier komplexer Matrizen
	void fMultSqCMatr( CComplex *C1In, CComplex *C2In,  CComplex *CTemp,  CComplex *CRes,  int iDim );
	void fMult3SqCMatr( CComplex *C1In, CComplex *C2In, CComplex *C3In,   CComplex *CTemp1, CComplex *CTemp2,   CComplex *CRes,   int iDim );
	void fExpTimesSqMatrTimesExp( CComplex *CExpL, CComplex *CM, CComplex *CExpR, int iDim, CComplex *CRes );
	void fRealLinKombOf2CVec( double a, CComplex *Ca, double b, CComplex *Cb, CComplex *CRes, long lDim );
	void fRealLinKombOf2CMatrFor4Mat( double a, CComplex *Ca1, CComplex *Ca2, CComplex *Ca3, CComplex *Ca4,   
								  double b, CComplex *Cb1, CComplex *Cb2, CComplex *Cb3, CComplex *Cb4,
								  CComplex *CRes1, CComplex *CRes2, CComplex *CRes3, CComplex *CRes4,  int iRowDim, int iColDim );
	void fAddMatrices( CComplex *CM1,   CComplex *CM2,  int iNoLns, int iNoCols,   CComplex *CRes );
	void fAddConstTimesUnity( CComplex CConst, CComplex *CM, int iDim );
	void fMatrix1MinusMatrix2( CComplex *CM1,   CComplex *CM2,  int iNoLns, int iNoCols,   CComplex *CRes );
	void fNegMatrix1MinusMatrix2( CComplex *CM1,   CComplex *CM2,  int iNoLns, int iNoCols,   CComplex *CRes );
	void fSqMatrixPlusDiag( CComplex *CM, CComplex *CDiag, int iDim );
	void fSqMatrixMinusDiag( CComplex *CM, CComplex *CDiag, int iDim );
	void fRealScalarTimesMatrix( double ds,   CComplex *CM,  int iNoLns, int iNoCols,   CComplex *CRes );
	void fExtract_T_ij_FromCPropF( CComplex *CT_11,  CComplex *CT_12,  CComplex *CT_21,  CComplex *CT_22 );
	void vMatrixIdentReal_rc( double *dpSource, double *dpDestination, int iDim_r, int iDim_c); //Identität zweier reeller (rxc)-Matrizen
	//Vectors	
	double f2Norm( CComplex *CV, int iNoLns );
	double f2NormOfDiff( CComplex *CV1, CComplex *CV2, int iNoLns );
	void fSetCVToZero( CComplex *CV, int iNoLns );
	void fV2EqV1( CComplex *CV2, CComplex *CV1, long lNoLns );
	void fSxV( double dScal, CComplex *CV1, long lNoLns, CComplex *CRes );
	void fAddVect( CComplex *CV1,   CComplex *CV2,  long lNoLns, CComplex *CRes );
	void fV1MinusV2( CComplex *CV1,   CComplex *CV2,  long lNoLns, CComplex *CRes );
	//void fReSxCVPlusReSxCV( double dScal1, CComplex *CV1, double dScal2, CComplex *CV2, int iNoLns, CComplex *CRes );
	void fCVInPlusSxCMxCV( CComplex *CVIn,   double dScal, CComplex *CM, CComplex *CV, CComplex *CVt,   int iNoLns );
	void fCVInPlusS1xCM1xCV1PlusS2xCM2xCV2( CComplex *CVIn,   double dScal1, CComplex *CM1, CComplex *CV1, 
														 double dScal2, CComplex *CM2, CComplex *CV2,   CComplex *CVt,   int iNoLns );
	/* Diagonale Matrizen */
	void fAddComplexDiagMat(CComplex *CVec1, CComplex *CVec2, CComplex *CResVec, int iDim);
	void fMult4x4BlockDiagMat(CComplex *CZVec1, CComplex *CZVec2, CComplex *CZVec3, CComplex *CZVec4, CComplex *CSVec1, CComplex *CSVec2, CComplex *CSVec3, CComplex *CSVec4, CComplex *CResVec, int iDim);
	void fMultiplyComplexDiagMat(CComplex *CVec1, CComplex *CVec2, CComplex *CResVec, int iDim);
	void fCalcNorm( CComplex *C1, CComplex *C2, int iDim, double *dRes );



	//############################################################ cpp-code ######################################################

	bool fInvert_2_x_2_mat(cppc *cIn, cppc *cOut);



	/* Funktionen nur für RBT und DLL*/

		/* Eingabe */
		#ifdef DLL //W.Iff
			GF_x_Rptr	pGetGF_xFromList(MultGF_xList_Rptr pMultGF_xList, int iPosition);	//W.Iff
			RC_yStratumOfRepUnit_Rptr	pGetStratumFromList(RC_yList_Rptr pList, int iPosition);
			RBTLayer_Rptr	pGetLayerFromList(RBTList_Rptr pList, int iPosition);
			int  fReadInputArguments(int iUnitOfAllLengthes, int iUnitOfAllAngles, Radiation sRadiation, double dLengthOfPeriod,  
							RBTList_R sRBTList, int iDim);
		#else // keine DLL: 
			int fReadInp(FILE *Ffilename); // Auslesen des Eingabefiles ~.inp
			int fInit(FILE *Ffilename); // Initialisierung aus Eingabefile ; ist gar nicht im Gebrauch!
			int freadvariable(FILE *Ffilename, char cpBezeichnung[], void *vpTarget, int iType); // Eingabevariable lesen
			int ffindmark(FILE *Ffilename, char cpBezeichnung[]); // fuer das Auslesen des Eingabefiles ~.inp noetig
		#endif //keine DLL

		/* Reparametrisierung */
		int fGensStapel4Calc();

		/* Ausgabe */
		void fParamout();
		void fStrucOut(int iCurLay); // Ausgabe der Strukturparameter
		void fOutp_H_CalcStruc(int iCurLay); // Ausgabe berechnete Struktur (header)
		void fOutp_CalcStruc(int iCurLay, double dBaseKoord, int ifold, double dStep, double *dxvec, double dxl, double dxr); // Ausgabe berechnete Struktur
		void fOutp_SavCalcStruc(int iCurLay, int iCurCell, int iCurCellLay, int iNoSaveKoords); // Strukturausgabe für Zellenschichten ohne Int.
		int fOutput(); // Numerische Ausgabe; wird auch bei DLL-Anwendung benutzt !
		void fPlot_Vec(Vector *Vec, char *szText, ofstream *file);
		void fPrint_Vec(Vector *Vec, char *szText, ofstream *file);
		void fPrint_VA(VArray *VA, char *szText, ofstream *file); 
		void fPrint_c_matr (CComplex *CMatr, int iNoRows, int iNoColumns, char *szText); //print  complex matrix to Fres
		void fPrint_c_matr (CComplex *CMatr, int iNoRows, int iNoColumns, char *szText, BOOL bOutput);  
		void fPrint_d_matr (double *dMatr, int iNoRows, int iNoColumns, char *szText);
		void fPrint_c_matr(Matrix *M, char *szText, ofstream *file); 
		int fWriteRayl_z_Coeff_binarily();
		void fPrintNumerErrorNoncon( fstream *fOut, CComplex *CEz_R_Exact,  CComplex *CHz_R_Exact,  CComplex *CEz_T_Exact,  CComplex *CHz_T_Exact );
		void fPrintMaxMatrEntry( CComplex *CMatr, int iNoRows, int iNoCols, FILE *File, BOOL bOnlyNondiagEntries );
		void fPrintMaxDiagEntry( CComplex *CMatr, int iNoRows, FILE *File );
		int fWriteFewDiffOrd( CComplex *CEzR, CComplex *CEzT, CComplex *CHzR, CComplex *CHzT, int iN_Calc, int iN_Write );
		int fReadFewDiffOrd( Vector *VEz, Vector *VHz, int iN_Read );
		void fErrorForFewDifford( Vector *VEz, Vector *VHz, int iN_Max, double *EzError, double *HzError );

		/* nicht-binäre Profile */
		int fGetPropMat_1DCM( double y, int iCurLay, CComplex *CExp );
		//int fGetNormToep(double y0, int iCurLay); //W.Iff, Dec. 2009: A bad implementation by S. Glaubrecht.
		int fGetNormToep_Mult( int iDim, double *dxOrd, int iCurLay, cppc *cNxNx, cppc *cNxNy, cppc *cNyNy, long iNoFCoeff );

		void fFourierTrans_Loc(int iType, double da, double db);
		
		//Interpolation
		int fLinInterpolInSections(int iNoBrkp, double *dX_Br, double *dNxNx_Br, double *dNxNy_Br, double *dNyNy_Br,
						int iFFTDim, double *dx_FFTNode, double *dNxNx, double *dNxNy, double *dNyNy); //W. Iff 1.12.2009
		double fHat( double dx );
		double dIntegralOfHat1Step( double dxStart, double dxStop );
		void fInitTableIntOfNormHat();
		double fCalcIntOfNormHatAtX( double dx );
		int fInterpolAngleByHatFu( int iNoBrkp, double *dX_Br, double *dNxNx_Br, double *dNxNy_Br, int iFFTDim, 
						   double *dx_FFTNode, double *dNxNxVec, double *dNxNyVec );
		void fNevilleAitken( double *dx, CComplex *Cf, int iNoPoints, long lVecDim, double dxRes );

		//rounding
		int fround(double dZahl);

		/* Ausgabe */
		#ifndef DLL //W.Iff
			void fPrepNumFile(char *cName, FILE **Fp, char *clabel, char *cext); // kein .nop-Output bei DLL
		#endif //keine DLL

		/* Ausgabe f. Zeiss */
			void fPrintZergData(FILE *Filep); // Ausgabe im Zeiss .erg Format -auch bei DLL moeglich
		#ifndef DLL //W.Iff
			void fPrepZergFile(char *cName, FILE **Fp, char *clabel, char *cext);
		#endif //keine DLL













		// #####################################################  Fast_S_Algo  ####################################################



		// -------------------------------- maths -----------------------------------------
		
		void fcppc_x_Vec( cppc c, Vector *Vec ); 
		void fVecPlusVecPlusVec( Vector *Vec1, Vector *Vec2, Vector *Vec3, Vector *VRes );
		void fVecPlusVecMinusVec( Vector *Vec1, Vector *Vec2, Vector *Vec3, Vector *VRes );
		void fDiag_x_Vec( Vector *VDiag, Vector *Vec, Vector *VRes ); 
		int fShuffle ( long iDim, CComplex *CData );
		int fFFT( BOOL bInverse, long iDim, cppc *cData, char *szText, int iNoChar ); 
		void fToep_x_TETM_Vec_Fast(Vector *VCirc, Vector *VecTETM, Vector *VResTETM);
		void fToep_x_Vec_Fast( Vector *VCirc, Vector *Vec, double *dFFT_Buf_Re, double *dFFT_Buf_Im, Vector *VBuffer, Vector *VRes ); 
		Vector fToep_x_Vec( const Vector& vVec, void *vToeplitz ); 
		Vector fToep_x_Vec_FFTs_inversed( const Vector& vVec, void *vToeplitz ); 
		Vector fToep_x_Vec_FFTs_inversed_2( const Vector& vVec, void *vToeplitz ); 
		void fToep_x_Vec_DirectSpace( const Vector *Vec, double dSign, Vector *VDiag, BOOL bTakeInverse,
							  double dFac_x_Vec, Vector *VExtendedVec, Vector *VRes ); 
		void fToep_x_Vec_DirectSpace_Tilted(  const Vector *Vec,    
									  double dSign1, Vector *VDiag1, BOOL bTakeInverse1, Vector *VNormal1, 
									  double dSign2, Vector *VDiag2, BOOL bTakeInverse2, Vector *VNormal2,  double dFac_x_Vec,  
									  Vector  *VExtendedVec,  Vector *VRes  ); 
		void fToep_x_Vec_DirectSpace_MatrixW( const Vector *VEx, const Vector *VEz, 
									  Vector *VEps, Vector *VInvEps, Vector *VNxNx,  Vector *VNxNz,  Vector *VNzNz,   
									  Vector  *VExtendedVecEx,  Vector  *VExtendedVecEz,  Vector *VResEx,  Vector *VResEz );  
		void fTestToeplMult();
		Vector fToepInv_x_Vec( Vector Vec, Vector *VToepInv, BOOL *bFailure );
		void fcppc_x_Vec( cppc cScal, Vector *Vec, long iDim );
		void fSet_Vec_Zero( Vector *Vec);
		void fSet_cppc_Zero( cppc *cData, long iDim );
		long iRound( double dx );
		
		// ----------------------- Multiple reflection series & S-Vector -------------------------
		

		//void fAlloc_WPM_or_GSM();
		//void fDealloc_WPM_or_GSM(); 
		//void fInitCalc_WPM_or_GSM();
		//void fNullify( cppc *cField, long iNoSets, long iDim ); 
		//void vInit_ParamForbDoCirc(struct ParamForbDoCirc *sArguments);
		//int fPrepareNextLayer(int L, double *dDeltayS, BOOL bDense);
		//BOOL fPrepareCalcAtLayer( int L );
		//BOOL fAllocAndPrepareCalcAtMGF( int L ); 
		//void fFreeDielF_MGF();
		//void fOutputIncInOutFormat( double dDirection[3],   CComplex CEx, CComplex CEz,   CComplex CHx, CComplex CHz );
		//void fOutputEAndH( CComplex *Ca, char *szText );
		//void fOutputEH_cppc( Vector *Va, char *szText );
		//void CalcAndOutputResidual(CComplex *CaI, int iStepNo, int iNoOfS, double *dResidual, double *dResidualTot);
		//void CalcAndOutputResidual_cppc( cppc *caI, int iStepNo, int iNoOfS, double *dResidual, double *dResidualTot);
		//void fGetSVecNorm( Vector *V0, Vector *V1, int iStepNo, double *dResidualTot ); 
		
		// ---------------------------------------- Output ------------------------
		
		void fOutputResInOutFormat( BOOL bRefl,   CComplex *CEx, CComplex *CEy, CComplex *CEz,   CComplex *CHx, CComplex *CHy, CComplex *CHz,  
								 CComplex *Cas, CComplex *Cap,   double *dEffic, double dEfficTot );
		Vector mult(const Vector& cx);
		
		// ----------------------------- WPM functions ------------------------------------ 
		
		void fSetInDirectSpace( int iMode, double dx0, double dx1, long iDim,  cppc *c0, cppc *c1, cppc *cRes ); 
		BOOL fSetEpsInDirectSpace( double *dxSurf, cppc *cEpsSurf, long iNoSurf, long iDim,  cppc *cRes, BOOL *bInterpolAtPixel ); 
		cppc fCalcBetaRBT( cppc cEps, cppc cMu, long iOrder, long iN );
		CComplex fCalc_ky_C( cppc cEps, cppc cMu, long iOrder );
		void fCalc_p_qe_qh(  cppc cEps, cppc cMu, long iOrder,  cppc *cp, cppc *cqe, cppc *cqh  );
		void fEH_From_a(  cppc caeDown, cppc cahDown, cppc caeUp, cppc cahUp,   cppc cp, cppc cqe, cppc cqh,   
						  cppc *cEx, cppc *cEz, cppc *cHx, cppc *cHz  );
		void fFresnelEqIncFromAbove(  cppc cEps1, cppc cMu1, cppc cEps3, cppc cMu3,  Vector *V1Inc, Vector *V3Out, Vector *V1Out  );
		void fFresnelEqIncFromBelow(  cppc cEps1, cppc cMu1, cppc cEps3, cppc cMu3,  Vector *V3Inc, Vector *V1Out, Vector *V3Out  );
		void fEHFromEzHz(  cppc *cEz, cppc *cHz,  long iDim, cppc cEps, cppc cMu,  BOOL bUpwards,  cppc *cEx, cppc *cHx  ); 
		void fProp( double dDelta_y,  cppc cEpsGF_x, cppc cMu,  Vector *VT );
		void fRecomb( double dx1, double dx2, BOOL bSetAlsoV1, BOOL bSetAlsoV2,   Vector *V0,  Vector *V1,  Vector *V2  );
		void fRecombRig(  double dx1, double dx2, BOOL bSetAlsoV1, BOOL bSetAlsoV2,   Vector *V0,  Vector *V1,  Vector *V2,
				  BOOL bEzHz  ); 
		void fIntDH( double dDelta_y,  cppc cEpsGF_x1, cppc cEpsGF_x2, cppc cMu,  Vector *V0,  double dx1, double dx2 );
		void fPropHH( cppc cEps, cppc cMu, Vector *V, double dDy ); 
		void fPropRigHH( cppc cEps, cppc cMu, Vector *V, double dDy, BOOL bPropEzHz );
		void fGetExHxFromEzHz( Vector *V, cppc cEps, BOOL bDownwardProp, BOOL bGetDx ); 
		void fGetEzHzFromExHx( Vector *V, cppc cEps, BOOL bDownwardProp, BOOL bDxGiven );
		void fNonDiagDeltayTE ( BOOL bRecomb, double dDy,  cppc cEpsGF_x1, cppc cEpsGF_x2, Vector *V0, Vector *V1,  double dx1, double dx2,  
					    cppc cEpsBasis );
		void fImplEulerTE( double dDy,  cppc cEpsGF_x1, cppc cEpsGF_x2, Vector *V0, Vector *V1,  double dx1, double dx2,  cppc cEpsBasis );
		void fIRK2TE1Step ( double dDy, cppc cEpsGF_x1, cppc cEpsGF_x2, Vector *V0, Vector *V1, double dx1, double dx2, cppc cEpsBasis, BOOL bRecomb );
		void fIRK2TE ( double dyStart, double dyStop, int iNoSteps,  int iLayerType, cppc cEpsGF_x1, cppc cEpsGF_x2, 
			    Vector *V0, Vector *V1, BOOL bRecomb, BOOL bUseDx, double dx1, double dx2 );
		void fPropBin ( double dyStart, double dyStop, int iNoSteps,  int iLayerType, cppc cEpsGF_x1, cppc cEpsGF_x2, cppc cMu1, cppc cMu2,  
			    Vector *V0, Vector *V1, BOOL bRecomb, BOOL bUseDx, double dx1, double dx2 ); 
		void fAddToCldOrSub( double dx0, double dx1,  Vector *V0, Vector *V1, Vector *VCldOrSub );
		void fDirect_To_k_Space( Vector *V ); 
		void f_k_To_Direct_Space( Vector *V ); 
		
		// --------------------------------  main-functions  &  S-vector main-functions  ------------------------------
		
		//Vector VDoCircWPM( const Vector& VaI, void *vArguments );
		//Vector VDoCirculations( const Vector& VaI, void *vArguments );
		//BOOL fFast_S_Algo();
		//BOOL fFast_S_Algo_WPM();
		//BOOL fFast_S_Algo_GSM();
		
		// ------------------------------- num. Int. (iterative) -------------------------------------------------------
		
		BOOL fDoOneIRK4IterSubStep( CComplex *Ca0,   
					double dWeight1, CComplex *CM1, CComplex *Ca1, CComplex *CM2, CComplex *Ca2,   
					double dWeight2, CComplex *CM3, CComplex *Ca3, CComplex *CM4, CComplex *Ca4,   CComplex *CRes );
		BOOL fSVectorIRK4Expl( CComplex *Ca0u, CComplex *Ca3d, BOOL bStartFromAbove, double dyStart, double dyEnd, int iCurLay,
							   CComplex *Ca0d, CComplex *Ca3u );



		// ########################################  GSM  ################################################

		void fQ_EH_x_a( Vector *VaDown, Vector *VaUp, long iN, cppc cEpsB, cppc cMuB,   
			    Vector *VEx, Vector *VEy, Vector *VEz,  Vector *VHx, Vector *VHy, Vector *VHz );
		void fP_EH_x_EH( Vector *VEx, Vector *VEy, Vector *VEz,  Vector *VHx, Vector *VHy, Vector *VHz, 
			    long iN, cppc cEpsB, cppc cMuB,  Vector *VaDown, Vector *VaUp );
		void fFresnelGSM( cppc cEpsU, cppc cEpsL, Vector *VaUpInc, Vector *VaDownInc, long iN,   Vector *VaUpOut, Vector *VaDownOut );
		void fCalc_kz_AndExp( Vector *Vkz, BOOL bCalc_kz, Vector *VExp, BOOL bTE_And_TM, cppc cEps, double dDz, long iN );
		void fPropGSM_Isotr(Vector *Va, BOOL bTE_And_TM, cppc cEps, double dDz, long iN);
		Vector fPropGSM( Vector *Va, cppc cEpsTE, cppc cEpsTM, double dDz, long iN );
		Vector fPropGSM_DownAndUp( const Vector& Va, cppc cEpsTE, cppc cEpsTM, double dDzDown, double dDzUp, long iN ); 
		long fGetDimInDirectSpace (long iN); 
		long fGetCircDim (long iN);
		void fCalcCirc( long i2N_FCoeff, long i2N_FCoeff_PC, Vector *VFCoeff, Vector *VCirc );
		void fGet_x_Coord_RBT( double y, int L, double *dxOrd );
		int fSetEpsAndInvEpsAndN( int L, BOOL bDense );
		void fPrepareEpsAndN( long iDim );
		void fDeleteEpsAndN();
		
		bool fSetEpsAndInvEps_SSMM_all_L();
		bool fSetEpsAndInvEps_SSMM(int L, ofstream& fOut);
		void fSetIntForPrecond2(ofstream& fOut); 
		void fScatAtInterface_SSMM_down(Vector& Va_TE_down_upper, Vector& Va_TM_down_upper, void *vData,
			Vector& Va_TE_down_lower, Vector& Va_TM_down_lower, Vector& Va_TE_up_upper, Vector& Va_TM_up_upper); 
		void fScatAtInterface_SSMM_up(Vector& Va_TE_up_lower, Vector& Va_TM_up_lower, void *vData,
			Vector& Va_TE_up_upper, Vector& Va_TM_up_upper, Vector& Va_TE_down_lower, Vector& Va_TM_down_lower); 
		void fScatAtLayer_SSMM(int L, int iN, Vector *VaUpInc, Vector *VaDownInc, Vector *VaUpOut, Vector *VaDownOut); 

		void fDeltaEps_x_E( int s, Vector *VEps, Vector *VInvEps, Vector *VNxNx, Vector *VNxNz, Vector *VNzNz,   
			   Vector *VEx, Vector *VEy, Vector *VEz, BOOL *bFailure );
		void fScatAmplAtDeltaEps( int L, int s, double dDz, int iN,   Vector *VaUpInc, Vector *VaDownInc,   Vector *VaUpOut, Vector *VaDownOut );
		Vector f1MinusRPVQ_1Slice( const Vector& Va, void *vArg ); 
		void fScatAtSliceLs( int L, int s, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		Vector fScatAmplAtDeltaEps_DownAndUp( int L, int s, double dDz, int iN, const Vector& Va );




		//################################### S-Vector algo for slices s ################################################

		int fCalc_s_Segmentation_s( int isBot, int isTop, int *iDelta_s );
		void fCalc_s_OfSegments_s( int isBot, int iNoVecInVA, int iDelta_s,  int *isLowestSegTop, int *isUpmostSegBot );
		VArray fCud_s( int L, int isBot, int isTop, int iNoS, int iDelta_s, int iN, VArray VADown, Vector *VBot, BOOL bInitVBot );
		VArray fCdu_s( int L, int isBot, int isTop, int iNoS, int iDelta_s, int iN, VArray VAUp, Vector *VTop, BOOL bInitVTop );
		Vector fScatAtOuterTop_s( int L, int isTop, int isBot, int iN,  Vector *VaDownInc, Vector *VTrans );
		Vector fScatAtOuterBot_s( int L, int isTop, int isBot, int iN,  Vector *VaUpInc, Vector *VTrans );
		Vector fMultCudCdu_s( const Vector& VInput, void *vArg );
		Vector fMultCudAndCdu_s( const Vector& VInput, void *vArg );
		void fScatExpl_s( int L, int isBot, int isTop, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		void fScatImplCompressed_s( int L, int isBot, int isTop, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		void fScatImpl_s( int L, int isBot, int isTop, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		void fScat_s( int L, int isBot, int isTop, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		
		// ------------------------------------ Preconditioner for S-Vec_s ----------------------------------------------

		void fSetSubarray_s( VArray *VA_N, int iN, int iN_Sub,  VArray *VA_NSub );
		void fSetArray_s( VArray *VA_NSub, int iN, int iN_Sub,  VArray *VA_N );
		void fPrecond_CudCdu_s( struct SVecArg sArg, VArray *VADown_N );
		void fPrecond_CudAndCdu_s( struct SVecArg sArg, VArray *VADown_N, VArray *VAUp_N );


		//################################### S-Vector algo for Layers L ################################################

		void fExpolForLayer_L( int L, int iN, Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		int fCalc_s_Segmentation_L( int isBot, int isTop, int *iDelta_s );
		void fCalc_s_OfSegments_L( int isBot, int iNoVecInVA, int iDelta_s,  int *isLowestSegTop, int *isUpmostSegBot );
		VArray fCud_L( int isBot, int isTop, int iNoS, int iDelta_s, int iN, VArray VADown, Vector *VBot, BOOL bInitVBot );
		VArray fCdu_L( int isBot, int isTop, int iNoS, int iDelta_s, int iN, VArray VAUp, Vector *VTop, BOOL bInitVTop );
		Vector fScatAtOuterTop_L( int isTop, int isBot, int iN,  Vector *VaDownInc, Vector *VTrans );
		Vector fScatAtOuterBot_L( int isTop, int isBot, int iN,  Vector *VaUpInc, Vector *VTrans );
		Vector fMultCudCdu_L( const Vector& VInput, void *vArg );
		Vector fMultCudAndCdu_L( const Vector& VInput, void *vArg );
		void fScatExpl_L( int isBot, int isTop, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		void fScatImplCompressed_L( int isBot, int isTop, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		void fScatImplCompressedDownInc_L( int isBot, int isTop, int iN,  Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		void fScatImpl_L( int isBot, int isTop, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );
		void fScatImpl_L_InitGuess( int isBot, int isTop, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut ); 
		void fScat_L( int L_Bot, int L_Top, int iN,  Vector *VaUpInc, Vector *VaDownInc,  Vector *VaUpOut, Vector *VaDownOut );

		// ------------------------------------ Preconditioner for S-Vec_L ----------------------------------------------

		void fSetSubarray_L( VArray *VA_N, int iN, int iN_Sub,  VArray *VA_NSub );
		void fSetArray_L( VArray *VA_NSub, int iN, int iN_Sub,  VArray *VA_N );
		void fPrecond_CudCdu_L( struct SVecArg sArg, VArray *VADown_N );
		void fPrecond_CudAndCdu_L( struct SVecArg sArg, VArray *VADown_N, VArray *VAUp_N ); 


	//####################################################################  WPM - version 25.2.2017  ###################################################################

	void fWPM_2017(int L, int iN, Vector *VaUpInc, Vector *VaDownInc, Vector *VaUpOut, Vector *VaDownOut); 
	void fWPM_2017_ExpFirst(int L, int iN, Vector *VaUpInc, Vector *VaDownInc, Vector *VaUpOut, Vector *VaDownOut); 
	void fWPM_2017_refl_free(int L, int iN, Vector *VaUpInc, Vector *VaDownInc, Vector *VaUpOut, Vector *VaDownOut); 
	void fWPM_2017_refl_free_version2(int L, int iN, Vector *VaUpInc, Vector *VaDownInc, Vector *VaUpOut, Vector *VaDownOut);





		
	//----------------------------------------------------------------------------------------------------------------------------------------------
	//------------  Functions from ReadInput.cpp - e.g. for generation of the solar apectrum and the grating for solar thermal emissivity control ("_Sol")  ---------------

	//void vDeleteRBTList(RBTList_Rptr pRBTList);
	//RBTList_Rptr pGetGratingBinOpt( struct OptInfo sInfo, double *dTop_nm, double *dBot_nm, double *pHalfWidths_nm ); 
	//RBTList_Rptr pGetGratingSinOpt( struct OptInfo sInfo, double *dTop_nm, double *dBot_nm );  
	BOOL bReadNextLine_ASCII(ifstream *pfFile, string *pstrBuffer, int *pLineNo); 
	bool bReadNextLine(ifstream *pfFile, string *pstr, const char **ppc, int *pLineNo);
	BOOL bReadNoLines( ifstream *pfFile, int *pNoLines, int *pLineNo ); 
	//BOOL bReadRefInd( ifstream *pfFile, int *pLineNo, int iNoLines, struct RefIndAtLambda *pData ); 
	//BOOL bReadSolSpec( ifstream *pfFile, int *pLineNo, int iNoLines, struct Spectrum *pData );
	//double dSpecIntBlackBody( double dLambda_nm ) ; //calculate the spectral intensity of a black body
	//BOOL bInitSpecAndRefIndAtLambda(); 
	//void vFreeSpecRefIndAtLambda();
	//BOOL bCalcRefIndMat( int iMatNo, double dLambda_nm, CComplex *CRefInd ); //Set refractive index of material;
	//BOOL bSetRefIndAndDiscretisOfGrating( double dLambda_nm, RBTList_Rptr pGrating ); //Set refractive index of all materials in grating 
	//RBTList_Rptr pGenerateTestGratingASML(); 
	//RBTList_Rptr pGenerateTestGrating(int *iNoDx, int *iDim);

	//------------------------------- Merit function & facilities -------------------------------------------------------------------------------

	double dPeriodFromParams( int iModus, const Vector& VParam, int iNoLayers, double pHalfWidths[] ); 
	double dEffic_Sol( void *pInfo, const Vector& VParam, bool *pSuc ); //merit fu. 

	//----------------------------  functions for efficiency calculation of solar absorbers  -----------------------------------------------------

	void vInitEffTable( struct EffAtLambda *pEffTable, int iN );
	void vFreeEffTable( struct EffAtLambda *pEffTable ); 
	BOOL bCalcWeightedAbs( double *pWeightedAbs, struct EffAtLambda sEffTable ); 
	BOOL bCalcWeightedEmiss1D_And_2D( double *pWeightedEmiss, Radiation sRadiation, RBTList_R sGrating, double dPeriod, struct EffAtLambda *pEffTable ); 
	BOOL bCalcWeightedEmiss( double *pWeightedEmiss, Radiation sRadiation, RBTList_R sGrating, double dPeriod, struct EffAtLambda *pEffTable );
	BOOL bCalcTotHemisphEmiss( Radiation sRadiation, RBTList_R sGrating, double dPeriod, double *pTotHemisphEmiss, struct EffAtLambda *pEffTable );

	//--------------------------------------------  functions for optimization  -----------------------------------------------------

	Vector VNumericGrad( void *pInfo, double (*dMeritFu)( void *pInfo, const Vector& VParam, bool *pSuc ), const Vector& VParam, const Vector& VParamMin, 
					 const Vector& VParamMax, const Vector& VStepSize, const bool pCalcDeriv[], const double dTolFac, bool *pSuc,  
					 Vector *pParamLower, Vector *pParamUpper, Vector *pFuLower, Vector *pFuUpper ); 
	double dLineSearchMin(void *pInfo, double(*dMeritFu)(void *pInfo, const Vector& VParam, bool *pSuc), const Vector& VParam, const double dFuValStart,
		const Vector& VGrad, const Vector& VParamMin, const Vector& VParamMax, bool *pOptParam, bool *pSuc, bool *pDoRestart);
	Vector Nonlin_CG_Method( void *pInfo, Vector VParamInp, Vector VParamMin, Vector VParamMax,  Vector VStepSize, double dTolFac, bool *pOptParam, bool *pSuc,    
						 double dMeritFu(void *pInfo, const Vector& VParam, bool *pSuc) );   



#define RBT_H //W.Iff
#endif //RBT_H not defined