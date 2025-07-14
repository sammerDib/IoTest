#ifndef RBT_TYPEDEFS_H //W.Iff: um mehrmaliges Einbinden zu verhindern


#include "Nag_Compatibility.h" //definition of CComplex 
#include "MATRIX.H" //contains VArray




typedef struct {
	double dgrad; // Einfallswinkel in Grad
	double drad; // Einfallswinkel in Rad
} Winkel;


typedef struct {
	CComplex n;
} UnMod;


typedef struct {
	double dHalfWidth;
	double dCentroid;
	CComplex n1;
	CComplex n2;
} Bin;

typedef struct { 
	double dsli;
	double deli;
	double dsre;
	double dere;
	CComplex n1;
	CComplex n2;
} Trapez;

typedef struct {
	double dPosMax;
	CComplex n1;
	CComplex n2;
} Sin;

typedef struct {
	int iNo;
	int *iGFTyp;
	double *dxoben;
	double *dxunten;
	double *dBaseLine;
	double *dModDepth;
	CComplex *CIndex_l;
	Vector VIndex_l; //ref. index on the left
	Vector VEps_l; //epsilon on the left
} MultGF;


#define GF_X_LINEAR 1
#define GF_X_HALF_SINE	2


typedef struct {
	BOOL breparam;
	int iTyp;
	double dBaseLine;
	double dModDepth;
	UnMod sUnMod;
	Bin sBin;
	Trapez sTrapez;
	Sin sSin;
	MultGF sMultGF;
	int iIRKSteps;
	int iSLayers;
	CComplex n1;
	CComplex n2;
} CellLayer;


typedef struct {
	int iNoCells; // Anzahl identischer Zellen in Superzelle = Anzahl Wiederholungen
	int iNoCellLayers; // Anzahl Modulationsschichten pro Zelle
	CellLayer *sSCLayer;
} SuperCell;


class SSMM {
public:
	bool bhom; //homogeneous layer?
	int iN = 0;
	//double dPreFacG; 
	//Matrix D;  Matrix G;  Matrix G_inv;
	Matrix G_Eps;  Matrix G_1_div_Eps;
	Matrix G_Eps_inv;  Matrix G_1_div_Eps_inv; 
	Matrix MModes_to_EH_TE;
	Matrix MModes_to_EH_TM;
	Matrix M_TE_adj; //MModes_to_EH_TE_adj
	Matrix M_TM_adj; //MModes_to_EH_TM_adj
	Vector VDiag_TE_up, VDiag_TE_down, VDiag_TM_up, VDiag_TM_down;
	Vector Vkz_TE, Vkz_TM;
	Vector VExp_TE, VExp_TM;
	cppc cEps_B; //basis medium
	//SSMM(int iNN = 1); 
	void Init(int iN); 
	//~SSMM();
};


class SSMM_Interface {
public:
	int iNoAp = 0;
	SSMM *sm_upper = NULL;
	SSMM *sm_lower = NULL;
	Matrix *Aperture = NULL;
	cppc *cEps_upper = NULL; cppc *cEps_lower = NULL; 
	SSMM_Interface(int iNNoAp = 0, int iDim = 0);
	void Init(int iNNoAp, int iDim); 
	~SSMM_Interface();
};


class SSMM_Grating { //class for spline-spline-modal-method, 22.7.17
public:
	int iDim = 0; //no of basis functions
	int iNoSections = 0; //no sections with same discretis. 
	int *iNoDx = NULL;   double *dDx = NULL; 

	SSMM smCld;
	SSMM *smLayers = NULL;
	SSMM smSub;
	int iNo_Layers = 0;
	double dPreFacG;
	Matrix FT;
	Matrix FT_inv;
	Matrix D;  Matrix G;  Matrix G_inv;
	SSMM_Interface *smInt = NULL;
	SSMM_Grating(int iNNo_Layers, int iDimm, bool bPreconditioner);
	void Init(int iNNo_Layers, int iDimm);
	void Init(int iNoSurf, double *dxSurf, int iNoDxInp[], int *iDim);
	~SSMM_Grating();
};


typedef struct {
	BOOL breparam;
	int iTyp;
	double dBaseLine;
	double dModDepth;
	UnMod sUnMod;
	Bin sBin;
	Trapez sTrapez;
	Sin sSin;
	MultGF sMultGF;
	bool bbinary; 
	SuperCell sSC;
	int iIRKSteps;
	int iSLayers;
	int iNoS_GSM; //no. of GSM-slices at the moment (it varies due to extrapol. procedure)
	CComplex n1;
	CComplex n2;
	VArray VAEps; //[[epsilon]]: pointer to "Vector of Toeplitz matrices for epsilon"
	VArray VAInvEps; //[[epsilon]]: pointer to "Vector of Toeplitz matrices for epsilon" 
	VArray VANxNx; //"Vector pointer to Toeplitz matrix for Normal component Nx²" 
	VArray VANxNz; //"Vector pointer to Toeplitz matrix for Normal component NxNz in GSM"  
	VArray VANzNz; //1 - NzNz 
	VArray VAEpsPC; //[[epsilon]]: pointer to "Vector of Toeplitz matrices for epsilon". "PC" means use in case of precond.
	VArray VAInvEpsPC; //[[epsilon]]: pointer to "Vector of Toeplitz matrices for epsilon" "PC" means use in case of precond.
	VArray VANxNxPC; //"Vector pointer to Toeplitz matrix for Normal component Nx²" "PC" means use in case of precond.
	VArray VANxNzPC; //"Vector pointer to Toeplitz matrix for Normal component NxNz in GSM"  "PC" means use in case of precond.
	VArray VANzNzPC; //1 - NzNz "PC" means use in case of precond.
	VArray VAEpsDense; //[[epsilon]]: pointer to "Vector of Toeplitz matrices for epsilon"
	VArray VAInvEpsDense; //[[epsilon]]: pointer to "Vector of Toeplitz matrices for epsilon" 
	VArray VANxNxDense; //"Vector pointer to Toeplitz matrix for Normal component Nx²" 
	VArray VANxNzDense; //"Vector pointer to Toeplitz matrix for Normal component NxNz in GSM"  
	VArray VANzNzDense; //1 - NzNz 
	VArray *VA_Aperture;
	BOOL bExtrapol; //if TRUE, midpoint rule + extrapolation => order 4; otherwise: order 2
	cppc cEpsB_L; //Layer basis med
	cppc cEpsB_Top; //basis med above Layer (between layers)
	cppc cEpsB_Bot; 
	//SSMM *sm; //spline-spline-modal-method, 12.6.17
} Layer;

typedef struct {
	double *dyKoord;
	double *dxKoorda;
	double *dxKoordb;
	double *dxOrd;
} StrOut;

typedef struct {
	int iNo;
	CComplex *CDielF;
	cppc *cEps; //epsilon
	CComplex *CInvDF;
	cppc *cIEps; //inverse epsilon 
} DielFkt;



#ifdef DLL //W.Iff : Definition von structs, die bei der Uebergabe von Argumenten an RBT und der Rueckgabe von Argumenten 
			//von RBT benutzt werden:
	//types of profile of layers and stratums
	#define NOT_YET_DEFINED	-1
	#define UNMOD			0	//unmodulated
	#define GF_X			4	//multi boundary surface (GF: Grenzflaeche, German) with reference to x-direction


		
	// -------------------------------- einfallende Strahlung  --------------------------------------------
	
	typedef struct
	{
		double dLambda; //units depend on iUnitOfAllLengthes in RBT-setup-pointer
		double dTheta;
		double dPhi;
		double dNormalComp; //TE
		double dParallelComp; //TM
		double dPhaseNormal;  //phase TE
		double dPhaseParallel; //phase TM
	} Radiation;
	typedef Radiation *Radiationptr;


	//struct CComplex hier nicht definieren, ist bereits an anderer Stelle von Stefan auf die gleiche Weise definiert worden !



	// ------------------ im Folgenden stets Bezug auf x-Richtung: -------------------------



	typedef struct tagUnMod_R {	//Unmodulated kind of layer:
		int iMatNo; //material number  
		CComplex CRefrInd; //user-inputted wavelength-independent refractive index
	} UnMod_R;	//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef UnMod_R *UnMod_Rptr;


	typedef struct {
		double dRelatBroadnessBump;//dRelat...=Relative  Stegbreite
		double dRelatPosMidpointBump;//x-Position des Mittelpunkts des Stegs
		CComplex CRefrIndGroove;//n im Graben
		CComplex CRefrIndBump;//n im Steg
	} Binary_R;	//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef Binary_R *Binary_Rptr;

	typedef struct { 
		double dRelat_xCoordBottomLeft;//dRelat...=Relative
		double dRelat_xCoordTopLeft;
		double dRelat_xCoordBottomRight;
		double dRelat_xCoordTopRight;
		CComplex CRefrIndGroove;//n im Graben
		CComplex CRefrIndBump;//n im Steg
	} Trapezium_R;	//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef Trapezium_R *Trapezium_Rptr;

	typedef struct {
		//dRelat_xPosMidpointBump=0.0 : relative x-Koordinate bei max. Funktionswert des Sinus; nicht vom Nutzer waehlbar!
		CComplex CRefrIndGroove;//n im Graben
		CComplex CRefrIndBump;//n im Steg
	} Sin_R;	//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef Sin_R *Sin_Rptr;


	typedef struct tagSingleSurface_xDirection
	//Add-on at 31.5.2011 by W.Iff: Now, the user can choose between a constant, wavelength-independent refractive index
	//(iInputMode CONST_REF_IND (0)) and a real material with dispersion used for the calculations (e.g. pulse simulation): 
	//iInputMode RAYTR_GLASS_CAT (1) <=> material from Raytrace glass catalogue, 
	//iInputMode METAL_CAT (2) <=> material from metal catalogue.
	{
		int iGF_xType;//1 oder 2: 1:linear, 2: 1/2-Sinus
		
		double dRelat_xCoordGF_xBottom;//relative x-Koord. der Grenzflaeche
		double dRelat_xCoordGF_xTop;

		//Different ways of inputting index of refraction on the left side (in -x-direction) of the boundary surface:
		int iMatNoLeft; //material number on the left 
		CComplex CRefrIndLeft;//inputted index of refraction on the left side
		
		struct tagSingleSurface_xDirection *pPrevGF_x;
		struct tagSingleSurface_xDirection *pNextGF_x;	
	}GF_x_R;	//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef GF_x_R *GF_x_Rptr;



	typedef struct tagMultBoundarySurfaces_xDirectionList//Aneinanderreihung (in x-Richtung) von Grenzflaechen  (in einer Layer y=const)
	{
		int iNumberGF_x;//Anzahl Grenzflaechen innerhalb einer Periode
		GF_x_Rptr pGF_x; //zeigt auf 1.Element der Liste mit Grenzflaechen
	}MultGF_xList_R;//Liste mit x_Grenzflaechen			//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef MultGF_xList_R *MultGF_xList_Rptr;




	// ------------- im Folgenden Bezug auf y-Richtung: --------------------------




	typedef struct tagRC_yStratumOfRepUnit_R //ein Listenelement "tagRepetitionCell in y Direction Stratum of Repetition Unit";
	{
		int iType;//0-4 zulaesssig: unmod, binaer, Trapezium_R, sin,  MultGF_xList_R. -1 bedeutet undefiniert (pProfileOfStratum=NULL)
		double dyCoordBottom;																	
		double dyCoordTop;
		void *pProfileOfStratum;//Zeigt auf ein Struct mit Info ueber Profil (UnMod_R,Binary_R etc.)
		int iNumberIRKSteps;																				
		int iNumberSMatrixLayers;
		struct tagRC_yStratumOfRepUnit_R *pNextStratum;
		struct tagRC_yStratumOfRepUnit_R *pPrevStratum;	
	}RC_yStratumOfRepUnit_R;	//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef RC_yStratumOfRepUnit_R *RC_yStratumOfRepUnit_Rptr;



	typedef struct RepetitionCell_yDirectionList_R
	{
		int iNumberOfRepetitionUnits;//die identischen Units werden uebereinandergeestapelt.
		int iNumberOfStratumsPerRepUnit;//Anzahl Schichten innerhalb einer Wiederholungseinheit.
		RC_yStratumOfRepUnit_Rptr pRC_yStratum; //zeigt auf Liste, die Information ueber Aufbau einer Wiederholungseinheit enthaelt.
	}RC_yList_R;//Liste mit RepetitionCell-Schichten		//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef RC_yList_R *RC_yList_Rptr;



	typedef struct tagRBTLayer_R
	{
		int iType;//0 bis 5 zulaessig: unmod,binaer,trapez,sinus,MultGF,RepetitionCell_yDirection.  -1 bedeutet undefiniert 
		double dyCoordBottom;																			//(pProfileOfLayer=NULL)
		double dyCoordTop;
		void *pProfileOfLayer;//zeigt auf struct UnMod_R oder Binary_R etc.
		int iNumberIRKSteps;//Anzahl IRK-Schritte																			
		int iNumberSMatrixLayers;//Anzahl S-Matrix-Schichten
		BOOL bExtrapol; //if true: use GSM (midpoint rule) plus extrapol. => order 4 with respect to integration direction
		CComplex CRefInd_b; //refract. index of basis medium inside layer 
		struct tagRBTLayer_R *pPrevLayer;
		struct tagRBTLayer_R *pNextLayer;	
	}RBTLayer_R;//Struct fuer Schicht mit Zellen		//_R fuer Raytrace; zur Abgrenzung von RBT-Code
	typedef RBTLayer_R *RBTLayer_Rptr;



	typedef struct{
	//Has been created for data which are produced/calculated only in case of Polaris. Raytracing at grating.
	//Is not needed for usage of Rigodif as single application (Raytrace->Main-menue->Rigodif).

	//quantities desired for calculations:
	double dReferenceR_Phi00;	//This variables would be pointers if we needed more diffracrion orders simultanously
	double dReferenceR_Phi01;
	double dReferenceR_Phi10;
	double dReferenceR_Phi11;

	double dReferenceR_UpsideDown_Phi00;		//Not all quantities are in used at the moment: ~Phi01, ~Phi10 not.
	double dReferenceR_UpsideDown_Phi01;
	double dReferenceR_UpsideDown_Phi10;
	double dReferenceR_UpsideDown_Phi11;

	double dReferenceT_Phi00;	//T Transmission , R:Reflection; Not all quantities are usuallly in use simultaniously.
	double dReferenceT_Phi01;
	double dReferenceT_Phi10;
	double dReferenceT_Phi11;

	double dReferenceT_UpsideDown_Phi00;
	double dReferenceT_UpsideDown_Phi01;
	double dReferenceT_UpsideDown_Phi10;
	double dReferenceT_UpsideDown_Phi11;

	//evaluation quantities:
	int iParserfunctionErrors; // Number of parserfunction errors
	int iGrazingRays;	//Number of ...
	int iBetaTooSmall;
	int iEvanescentRays;
	int iRaysAtTooLargePeriod;
	int iEnergyNotConserved; // Number of Rays for which RBT has violated the energy conservation (accuracy is user-defined).
	int iResultsAreNotRepresentableNumbers; //Number of rays at a DOE for which the RBT-results contain "#QNAN" and "#IND"
										//because of numerical errors
	
	double dMinAveragePhase;
	double dPeriodAtMinAveragePhase;
	double dxAtMinAveragePhase;
	double dyAtMinAveragePhase;
	double dThetaAtMinAveragePhase;
	double dPhiAtMinAveragePhase;

	double dMaxAveragePhase;
	double dPeriodAtMaxAveragePhase;
	double dxAtMaxAveragePhase;
	double dyAtMaxAveragePhase;
	double dThetaAtMaxAveragePhase;
	double dPhiAtMaxAveragePhase;

	double dMinRetardance;
	double dPeriodAtMinRetardance;
	double dxAtMinRetardance;
	double dyAtMinRetardance;
	double dThetaAtMinRetardance;
	double dPhiAtMinRetardance;

	double dMaxRetardance;
	double dPeriodAtMaxRetardance;
	double dxAtMaxRetardance;
	double dyAtMaxRetardance;
	double dThetaAtMaxRetardance;
	double dPhiAtMaxRetardance;

	//Look up-table for already calculated rays could be also defined here. To Do!!

	}CalcResults; //wurde speziell fuer Polarisationsraytracing an diffraktiven optischen Elementen eingefuehrt
	typedef CalcResults *CalcResultsptr;



	typedef struct{
		//Has been created for data which are edited/needed only in case of Polaris. Raytracing at grating.
		//Is not needed for usage of Rigodif as single application (Raytrace->Main-menue->Rigodif).

		BOOL bReserveVariable_unused; //perhaps needed in the future
		BOOL bCalcAllRaysWithoutRBT;//Sinnvoll, wenn z.B. die visuellen Strahlen in Raytrace getract werden. Dann wird RBT-nicht gebraucht.
		double dMaxAllowedAngleRayToNormalInDegree;
		double dMaxPeriodForPolarisCalcInLambdaVac; //Rays at periods greater this value are not calculated. E.g. rays at the middle of a DOE with
		//circular symmetry cannot be calculated properly by an 1-dim-calc-method.
		double dMaxPeriodForRigCalcInLambdaVac; //in order to avoid a very long calculation time
		double dPeriod1InLambdaVac;
		int iN1;		//iN as a  function of the period. Supporting points are user defined. iN(dPeriod) is interpolated linearly.
		double dPeriod2InLambdaVac;
		int iN2;
		double dPeriod3InLambdaVac; //not in use up to now
		int iN3;					//	"
		double dPeriodForReferencePhasesInLambdaVac; //used to avoid jumps in the phases by 2Pi. (RBT represents phases only in interval ]-Pi;Pi] )
		//could be user defined. 
		BOOL bDampGibbsPhenomenon_PolRaytr;//has always to be copied into WholeRBTSetup_R->bDampGibbsPhenomenon before calculation
		double dUpperAllowedLimitSumOfEff; //user-defined.  ~ is >= 1.
		double dLowerAllowedLimitSumOfEff; //user-defined.  ~ is <= 1.
		CalcResults *pResults;
	}CalcData; //wurde speziell fuer Polarisations-Raytracing an diffraktiven optischen Elementen eingefuehrt
	typedef CalcData *CalcDataptr;



	typedef struct tagRBTList_R //contains information on whole diffracting structure (GRATING)
	//Allowing dispersion: Wavelength-dependent refract. index for cladding, substrate and some types of layers has been 
	//introduced (added) by W. Iff at 27.3.2011
	{
		int iModus; 
		int iMatCld; //material no. cladding
		int iMatSub; //material no. substrate
		double dRefrIndexCld; //inputted refractive index of the cladding.
		CComplex CRefrIndexSubstr; //refract. index of substrate		
		int iNumberOfLayers;
		RBTLayer_R *pLayer; //Points to all remaining grating data, hence to 0. element of "tagRBTLayer_R"
	}RBTList_R; //list of layers	//_R stands for Raytrace; for dissociation from RBT-typedefs
	typedef RBTList_R *RBTList_Rptr;



//	--------------------------- Datenstrukturen fuer die Rueckgabe der Ergebnisse ------------------------------

   

	typedef struct tagRBTReturnData //Enthaelt Information ueber gesamte beugende Struktur
	{
		//1 .) k-Vektoren der reflektierten und transmittierten Strahlung

		double *dKx; // = alpha
		CComplex *CKy_R; // = beta 1 (fuer Reflexion)
		CComplex *CKy_T; // = beta 3 (fuer Transmission)
		double dKz; // = gamma

		//2.) Ergebnisse fuer fundamentale Eingangspolarisationen TE und TM:

		//Rayleigh-Koeffizienten:
		CComplex *CRayl_Ex_R_TE;		//TE
		CComplex *CRayl_Ey_R_TE;
		CComplex *CRayl_Ez_R_TE;	//E-Felder
		CComplex *CRayl_Ex_T_TE;
		CComplex *CRayl_Ey_T_TE;
		CComplex *CRayl_Ez_T_TE;

		CComplex *CRayl_Hx_R_TE;
		CComplex *CRayl_Hy_R_TE;
		CComplex *CRayl_Hz_R_TE; //H-Felder
		CComplex *CRayl_Hx_T_TE; 
		CComplex *CRayl_Hy_T_TE;
		CComplex *CRayl_Hz_T_TE;

		CComplex *CRayl_Ex_R_TM;		//TM
		CComplex *CRayl_Ey_R_TM;
		CComplex *CRayl_Ez_R_TM; //E-Felder
		CComplex *CRayl_Ex_T_TM;
		CComplex *CRayl_Ey_T_TM;
		CComplex *CRayl_Ez_T_TM;

		CComplex *CRayl_Hx_R_TM;
		CComplex *CRayl_Hy_R_TM;
		CComplex *CRayl_Hz_R_TM;	//H-Felder
		CComplex *CRayl_Hx_T_TM;
		CComplex *CRayl_Hy_T_TM;
		CComplex *CRayl_Hz_T_TM;

		//Unit direction-vector s for the diffracted orders (for reflection and transmission)
		double *dSx_R; //reflection
		double *dSy_R;
		double *dSz_R;
		double *dSx_T; //transmission
		double *dSy_T;
		double *dSz_T;

		//Komplexe Amplituden der E-Felder im strahlbasierten sp-System
		CComplex *CAmp_Es_R_TE, *CAmp_Ep_R_TE;
		CComplex *CAmp_Es_T_TE, *CAmp_Ep_T_TE;
		CComplex *CAmp_Es_R_TM, *CAmp_Ep_R_TM;
		CComplex *CAmp_Es_T_TM, *CAmp_Ep_T_TM;

		//Effizienzen
		double *dEff_R_TE;	//TE
		double dSumOfEff_R_TE;
		double *dEff_T_TE;
		double dSumOfEff_T_TE;
		double dSumOfEff_TE;
		double *dEff_R_TM;	//TM
		double dSumOfEff_R_TM;
		double *dEff_T_TM;
		double dSumOfEff_T_TM;
		double dSumOfEff_TM;

		//3.) Ergebnisse fuer die eingegebene Eingangspolarisation (also fuer den eingegebenen Jonesvektor):

		//Rayleigh-Koeffizienten:
		CComplex *CRayl_Ex_R; 
		CComplex *CRayl_Ey_R;
		CComplex *CRayl_Ez_R; //E-Felder
		CComplex *CRayl_Ex_T;
		CComplex *CRayl_Ey_T;
		CComplex *CRayl_Ez_T;

		CComplex *CRayl_Hx_R;
		CComplex *CRayl_Hy_R;
		CComplex *CRayl_Hz_R; //H-Felder
		CComplex *CRayl_Hx_T; 
		CComplex *CRayl_Hy_T;
		CComplex *CRayl_Hz_T;

		//Komplexe Amplituden der E-Felder im strahlbasierten sp-System
		CComplex *CAmp_Es_R, *CAmp_Ep_R;
		CComplex *CAmp_Es_T, *CAmp_Ep_T;
	
		//Effizienzen
		double *dEff_R;
		double dSumOfEff_R;
		double *dEff_T;
		double dSumOfEff_T;
		double dSumOfEff;

		//4.) Information wheter calculation is canceled due to a too small |dk_y,n| , n integer, -iN < n < iN
		BOOL bCalcCanceledDueToSmallBeta; //Any beta very close (beta>3.2e-5) to zero in any medium is a problem !!
									//Concern numerical inaccuracy when calculating small beta=sqrt(dK²-dk_x²-dk_z²) !!
		BOOL bReducedAccuracyDueToSmallBeta; //Is TRUE if an accuracy of only 2-6 valid digits is exspected (in 32-Bit-Version)
									//due to a small Beta in cladding or substrate. Calculation is still carried out.

		//5.) Data refering to the S-matrix blocks S_12, S_22, that is CSud_g and CSdd_g in RBT.DiffrctAmp_NP_1DCM_SMatrix.c
		//These data are calculated for alleviation of physical interpretation.
		//The determinant is always written as	dModulus_of_det * 2^iExp_of_det_to_basis_2 * e^(i*dPhase_of_det_in_rad).
		//The exponential representation is used in order to avoid overflow.
		BOOL bTotal_Success_in_calc_of_data_of_S12_and_S22; //success in total in calculation of determinant and eigensystem of
		//S_12 & S_22 using different methods
		double dModulus_of_det_CRefl_LU;		//_LU emphasizes calculation by using LU-factorisation
		Integer iExp_of_det_to_basis_2_CRefl_LU;
		double dPhase_of_det_in_rad_CRefl_LU;
		double dModulus_of_det_CTrans_LU;
		Integer iExp_of_det_to_basis_2_CTrans_LU;
		double dPhase_of_det_in_rad_CTrans_LU;
		double dModulus_of_det_CRefl_QR;		//_QR emphasizes calculation by using QR-factorisation
		Integer iExp_of_det_to_basis_2_CRefl_QR;
		double dModulus_of_det_CTrans_QR;
		Integer iExp_of_det_to_basis_2_Trans_QR;
		CComplex *CEigenvalues_CRefl;		//Eigensystem of CSud_g (CRefl in former times)
		double dModulus_of_det_CRefl_EV;		//_EV emphasizes calculation by using the eigenvalues
		Integer	iExp_of_det_to_basis_2_CRefl_EV;						 
		double dPhase_of_det_in_rad_CRefl_EV;							 
		CComplex *CEigenvalues_CTrans;		//Eigensystem of CSdd_g (called CTrans in former times)
		double dModulus_of_det_CTrans_EV;
		Integer	iExp_of_det_to_basis_2_CTrans_EV;
		double dPhase_of_det_in_rad_CTrans_EV;

		//6.) benoetigte Rechenzeit:
		double dTimeInSec;

	}RBTReturnData;
	typedef RBTReturnData *RBTReturnDataptr;


#endif //DLL


	



struct Parameter
{
	double dLambdaVak; // Vakuumwellenlänge
	Winkel sTheta; // Einfallswinkel
	Winkel sPhi; // konischer Winkel
	int	iBeugOrd; // max. Beugungsordnung N: -N..N (2N+1) Komponenten in case of Fourier method; No. of basis functions in case of spline method with exact boundary conditions
	double dKVak; // Vakuumwellenvektor
};


struct Geom
{
	double dPeriod_nm; //Periodenlänge in nm
	double dPeriod; // skalierte Periodenlänge 
	double dModDepth; // skalierte Modulationshöhe 
	double dLattVec; // skalierter Gittervektor
	double dCentroid; // skalierter Mittelpunkt des Stegs 
	double dHalfWidth; // skalierte halbe Breite des Stegs
	double dTotalHeight; // skalierte Gesamthöhe des Stacks
};

struct Brechungsindex
{
	CComplex CM1; // Brechungsindex Einfallsmedium
	CComplex CM2; // Brechungsindex Basismedium
	CComplex CM3; // Brechungsindex Ausfallsmedium
	CComplex CMG; // Brechungsindex Gittergraben
	CComplex CMS; // Brechungsindex Gittersteg
};

struct Epsilon
{
	CComplex CM1; // Dielektrische Funktion Einfallsmedium
	CComplex CM2; // Dielektrische Funktion Basismedium
	CComplex CM3; // Dielektrische Funktion Ausfallsmedium
	CComplex CMG; // Dielektrische Funktion Gittergraben
	CComplex CMS; // Dielektrische Funktion Gittersteg
	cppc cEps1; // Dielektrische Funktion Einfallsmedium
	cppc cEps3; // Dielektrische Funktion Ausfallsmedium
};

struct InvEpsilon
{
	CComplex InvEps_S; // Inverses Epsilon Steg
	CComplex InvEps_G; // Inverses Epsilon Graben
	CComplex InvInvEps_S; // Inverses Epsilon^(-1) Steg
	CComplex InvInvEps_G; // Inverses Epsilon^(-1) Graben
};

struct Blochwinkel
{
	double dalpha_0; //only kx_0 is buffered in WPM 
	double *dalpha; // k-Vektorkomponente alpha
	CComplex *Cbeta_1; // k-Vektorkomponente beta Einfallsmedium
	CComplex *Cbeta_2; // k-Vektorkomponente beta Basismedium
	CComplex *Cbeta_3; // k-Vektorkomponente beta Ausfallsmedium
	double dgamma0; // k-Vektorkomponente gamma0
	double *dForbiddenEpsilon; //Values of permittivity leading to Beta_n=0 or K_j²-gamma_0² = 0; dimension: iDim1+1
};

struct IntParam
{
	int iIRKsteps; // Anzahl der Integrationsschritte pro (S-Matrix-)Schicht
};

struct JonesVektor
{
	double dNPinc; // Komponente des Jonesvektors senkrecht zur Einfallsebene
	double dPPinc; // Komponente des Jonesvektors in der Einfallsebene
	double dPhaseN; // phase of N-component (TE)  //W.Iff: verwendete Einheit: rad
	double dPhaseP; //phase of P-component (TM)
};

struct ReflEH_Point //one point in direct space
{
	long i; //Node number, i >= 0, i <= iDim;  x_i = i * delta_x
	cppc cEx; 
	cppc cEz; 
	cppc cHx; 
	cppc cHz; 
};

struct ReflEH_Plane //one layer in direct space
{
	int iL; //layer no.
	int iS; //slice no. within layer
	long iNo; //no. of points
	struct ReflEH_Point *pP; //pointer to points
};

struct ReflEH
{
	int iNo; //no. of planes
	int iCur; //current plane
	struct ReflEH_Plane *pP; //pointer to IRK-planes inside layers
	struct ReflEH_Plane *pL; //pointer to planes surrounding the layers
};

struct ParamForbDoCirc  //See chapter about S-vector algo in W.'s thesis. "C_" stands for circulation here.  9.6.13
{
	int iStart;  int iStop;
	BOOL bUnityMinusC; //calc. (1-C) * ...
	int iMode[6]; //Modus, kind of propagation to do
	int iNoOfS; //No. of inserted S-vector planes
	double *dResidual;  double *dResidualTot;  
	Vector *VaCld; //field in Cladding
	Vector *VaSub; //field in Substrate
	struct ReflEH sRefl; //for buffering reflected light 
	int iexit_status;
};

//See chapter about S-vector algo in W.'s thesis. "C_" stands for circulation here.  9.6.13

//use of explicit procedure:
#define C_INC		0  
#define C_EXPLICIT	1

//use of implicit procedure:
#define C_INC_IMPLICIT  -1
#define C_IMPLICIT		 2
#define C_REFL_OUT		 3
#define C_TRANS_OUT		 4

//New S-Vector algo version at 1.5.14:
#define EXPL  10
#define IMPL_COMPRESSED  15
#define IMPL_COMPRESSED_DOWN_INC  20
#define IMPL  25
//for slices s:
#define EXPL_S  10
#define IMPL_COMPRESSED_S  15
//#define IMPL_COMPRESSED_DOWN_INC_S  20 //this mode makes no sense here, listet only for sake of completeness
#define IMPL_S  25
//for Layers L:
#define EXPL_L  10
#define IMPL_COMPRESSED_L  15
#define IMPL_COMPRESSED_DOWN_INC_L  20
#define IMPL_L  25
#define IMPL_L_INIT_GUESS  30

//WPM:
#define SET   0
#define ADD   1

//Polarization: 
#define TE_POL	0
#define TM_POL	1
#define CONICAL	2

#define UNUSED -1; //value of unused quantities




// ---------------------------------------  Structs for the solar thermal project  -------------------------------------

	struct Spectrum
	{
		double dLambda_nm; //wavelength in nm (nm is used in program for wavelength throughout)
		double dVal; //value of spectral intensity of solar irradiation   
	};


	struct RefIndAtLambda
	{
		double dLambda_nm; //wavelength in nm
		CComplex CRefInd; 
	};


	struct EffAtLambda
	{
		int iNoPoints; 
		double *pLambda; 
		double *pEff; //average over TE & TM 
	};



// ------------------------------------- Structs for the optimization of gratings ---------------------------------


	struct OptInfo
	{
		int iModus; 
		int iNoLayers; 
		double (*dCalcPeriod)(int iModus, const Vector& VParam, int iNoLayers, double *pHalfWidths); 
		int *iMatNo;
	}; 



	struct sLambdaNode //struct for integration over wavelength: specification of the forbidden integration region. W.Iff, 30.9.15
	{
		BOOL bExisting; //is this node existing?
		double dLambdaLeft; //left border around Rayleigh anomaly  
		double dLambdaMid; //Wavelength at Rayleigh anomaly 
		double dLambdaRight; //right border around Rayleigh anomaly   
	};


	struct sLambdaInt //struct for integrqtion over wavelength
	{
		double dLambda;
		double dWeightLeft;		//weights for integrqtion over lambda
		double dWeightRight; 
	};



#define RBT_TYPEDEFS_H //W.Iff
#endif //RBT_TYPEDEFS_H not defined