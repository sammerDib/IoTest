#ifndef RBT_DEFINES_H //W.Iff: um mehrmaliges Einbinden zu vermeiden


/* Glob Flags */

//#define __SERIAL__ //for licensing (serial number)
//#define __DEBUG__ // Detaillierte Ausgabe
#define DLL	


/* Defines für RBT */


#define FLOAT_PRECISION 1.0e-7 //more accurate calc. is not required in case of good modelling; W., 25.9.13

///* [[ Toeplitz ]] */
//#define CToep(I,J) CToeplitz[I*iDim1+J]
//#define CIToep(I,J) CInvToeplitz[I*iDim1+J]
//#define CIToepI(I,J) CInvToeplitzI[I*iDim1+J]
//
//	/* obsolet - nur noch für sin */
//	#define dReToep(I,J) dReToeplitzM[I*iDim1+J]
//	#define dImToep(I,J) dImToeplitzM[I*iDim1+J]
//	#define dReIToep(I,J) dReInvToeplitz[I*iDim1+J]
//	#define dImIToep(I,J) dImInvToeplitz[I*iDim1+J]
//	#define dReToepI(I,J) dReToeplitzI[I*iDim1+J]
//	#define dImToepI(I,J) dImToeplitzI[I*iDim1+J]
//	#define dReIToepI(I,J) dReInvToeplitzI[I*iDim1+J]
//	#define dImIToepI(I,J) dImInvToeplitzI[I*iDim1+J]
//
///* nicht-binäre Profile */
//#define CEDiff(I,J) CEpsDiff[I*iDim1+J]
//#define CNxE(I,J) CNxEps[I*iDim1+J]
//#define CNyE(I,J) CNyEps[I*iDim1+J]
//#define CQe_11(I,J) CQeps_11[I*iDim1+J]
//#define CQe_22(I,J) CQeps_22[I*iDim1+J]
//#define CQe_33(I,J) CQeps_33[I*iDim1+J]
//#define CNxx(I,J) CNxNx[I*iDim1+J]
//#define CNxy(I,J) CNxNy[I*iDim1+J]
//#define CNyy(I,J) CNyNy[I*iDim1+J]
//#define CQe_22_i(I,J) CQeps_22_inv[I*iDim1+J]
//#define CQp_12i(I,J) CQp12_22_inv[I*iDim1+J]
//#define CQp_2i1(I,J) CQp22_inv_21[I*iDim1+J]
//#define CQpip(I,J) CQp_inv_p[I*iDim1+J]
//
//#define TYP_INT		0
//#define TYP_DOUBLE	1
//#define TYP_CHAR	2
//#define EPSILON		0
//#define INVEPSILON	1
//#define MODE_pTE	0
//#define MODE_sTM	1
//#define MOD_HOM		0
//#define MOD_BIN		1
//#define MOD_TRAP	2
//#define MOD_SIN		3
//#define MOD_MULTGF	4
//#define MOD_SC		5
//
////Media for the Psi-Matrices - W., 16.7.12:
//#define CLD 1 //Cladding, medium of incidence
//#define BASIS 2 //Basis medium (usually chosen so that it represents an average refrect. index)
//#define SUB 3 //Substrate; medium at bottom
//
////Propagation direction: - W., 21.7.12
#define SUB_TO_CLD				0 //propagation from Substrate to Cladding
#define CLD_TO_SUB				1 //	"		from Cladding to Substrate
#define SUB_AND_CLD_TO_MIDDLE	2 //	"		from Cld. & Substr. to middle of grating 
//
///* S-Matrix konisch */
//#define CZMat(I,J) CZMatrix[I*iDim2+J]
//#define CT_11(I,J) CTMatrix_11[I*iDim2+J]
//#define CT_12(I,J) CTMatrix_12[I*iDim2+J]
//#define CT_21(I,J) CTMatrix_21[I*iDim2+J]
//#define CT_22(I,J) CTMatrix_22[I*iDim2+J]
//#define CSI(I,J) CSIntm[I*iDim2+J]
///* T-Blockmatrixarrays */
//#define CTBS_11(I,J,K,L) CTBlockSav_11[(iZL_SMesh[I]+J)*iDim2*iDim2+K*iDim2+L]
//#define CTBS_12(I,J,K,L) CTBlockSav_12[(iZL_SMesh[I]+J)*iDim2*iDim2+K*iDim2+L]
//#define CTBS_21(I,J,K,L) CTBlockSav_21[(iZL_SMesh[I]+J)*iDim2*iDim2+K*iDim2+L]
//#define CTBS_22(I,J,K,L) CTBlockSav_22[(iZL_SMesh[I]+J)*iDim2*iDim2+K*iDim2+L]
//
///* Propagationsmatrix */
//#define CPF(I,J) CPropF[I*(iDimI)+J] //W: "CComplex Propagate F": matrix for prop. of vector F of Fourier coeff., see thesis (2.37a)
//#define CPF_DH(I,J) CPropF_DH[(I*iDimI)+J] //W: matrix for propagation of D and H (instead of E and H)
////W. 15.7.12: Vector a of el. & mag. fields: a el. down, a h down, a el. up, a h up. to propagate between Subst. and Cladding:
//#define Ca(I) Caeh[I] //dim* 4*(2N+1)
///* impliziter RK */
///* Integrationsmatrix für IRK_M */
//#define CFnm(I,J) CFInt[I*(iDimI)+J]
///* IRK-Matrizen */
//#define CR1n(I,J) CR1[I*(iDimI)+J]
//#define CR2i(I,J) CR2inv[I*(iDimI)+J]
//#define CM1(I,J) CMat1[I*(iDimI)+J]
//#define CM2(I,J) CMat2[I*(iDimI)+J]
//#define CK1n(I,J) CK1[I*(iDimI)+J]
//#define CK2n(I,J) CK2[I*(iDimI)+J]
//#define CFn(I,J) CF[I*(iDimI)+J]
//
//
///* sStapel aus sStapelInp generieren */
//#define dBLV(I,J) dBLVec[I*iRegNo_Max+J]
//#define dMDV(I,J) dMDVec[I*iRegNo_Max+J]
//

/* Makros */

	/* TESTSUCCESS */
	#ifdef __DEBUG__					
		#define TESTSUCCES(Text) {						\
				if (ireturn_status != 0)				\
				{										\
					printf( #Text);						\
					fprintf(Fres, #Text);				\
					iexit_status = 1; bError_g = TRUE;	\
					goto END;							\
				}										\
		}
	#else // kein DEBUG	:							//Geht bei DLL printf, also Ausgabe auf den Bildschirm? Nein!
		#ifdef DLL //W.Iff  #Text darf kein Komma enthalten !!!
			#define TESTSUCCES(Text) {						\
					if (ireturn_status != 0)				\
					{										\
						fprintf(Fres, #Text);				\
						iexit_status = 1; bError_g = TRUE;	\
						goto END;							\
					}										\
			}
		#else //keine DLL:
			#define TESTSUCCES(Text) {				\
					if (ireturn_status != 0)		\
					{								\
						if (bRes)					\
							fprintf(Fres, #Text);	\
						iexit_status = 1;			\
						goto END;					\
					}								\
			}
		#endif //DLL
	#endif // DEBUG

//following Macro  resebles TESTSUCCES, but can output 2 strings in one single call; useful, if functions from NAG_Wrapper-dll are called:
#define TESTSUC_NAG(szText1, szText2) {						\
					if (ireturn_status != 0)				\
					{										\
						fprintf(Fres, szText1);				\
						fprintf(Fres, szText2);				\
						iexit_status = 1; bError_g = TRUE;	\
						goto END;							\
					}										\
				}

#define rcC reinterpret_cast<CComplex*>


//#define CAEH_VERSION	4293 //version for saving/loading Caeh;  W.,17.7.12
//#define CHECK_SAVING_SUCCESS	5924 //W., 17.7.12
//
//
////solar emissivity project - definitions of the materials
//#define SI				1	//a possible substrate
//#define AL				2	//metallic substrate
//#define MO				3	//metallic substrate
//#define TI_AL_288N2		4	//rather metallic
//#define TI_AL_550N2		5	//semiconducting
//#define AL2_O3			6	//dielectric
//#define AIR				7	
//
//#define ENERGY_BALANCE_ACCURACY		1.0e-3
//
////definition of the modus (the way, how to interpret VParam or VOptParam) and definition of the grating type 
//#define BINARY_1D			1	//Sidewall thicknesses are pre-defined and not optimized
//#define BINARY_1D_SIDES		2	//optimiz. also of sidewall thicknesses
//#define BINARY_2D			3	//the same for 2D
//#define BINARY_2D_SIDES		4
//#define CHANDEZON_1D		5	
//#define CHANDEZON_2D		6  


#define RBT_DEFINES_H //W.Iff
#endif //RBT_DEFINES_H not defined