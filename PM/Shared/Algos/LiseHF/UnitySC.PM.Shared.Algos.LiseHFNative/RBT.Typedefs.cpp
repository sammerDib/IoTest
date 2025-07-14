#include "RBT.Typedefs.h" 


void SSMM::Init(int iNN)
{
	iN = iNN;
	//D.Init(iN, iN);  G.Init(iN, iN);  G_inv.Init(iN, iN);  
	G_Eps.Init(iN, iN);  G_1_div_Eps.Init(iN, iN);  G_Eps_inv.Init(iN, iN);  G_1_div_Eps_inv.Init(iN, iN);
	M_TE_adj.Init(iN, iN); M_TM_adj.Init(iN, iN);  MModes_to_EH_TE.Init(iN, iN);  MModes_to_EH_TM.Init(iN, iN);
	Vkz_TE.Init(iN);  Vkz_TM.Init(iN);  VExp_TE.Init(iN);  VExp_TM.Init(iN);  VDiag_TE_up.Init(iN);  VDiag_TM_up.Init(iN);  VDiag_TE_down.Init(iN);  VDiag_TM_down.Init(iN);
}

SSMM_Interface::SSMM_Interface(int iNNoAp, int iDim)
{
	iNoAp = iNNoAp;
	if (iNNoAp > 0 && iDim > 0) {
		Aperture = new Matrix[iNoAp];
		for (int i = 0; i < iNoAp; i++)
			Aperture[i].Init(iDim, iDim);
		cEps_upper = new cppc[iNoAp];  cEps_lower = new cppc[iNNoAp];
	}
	else {
		Aperture = NULL;  cEps_upper = NULL;  cEps_lower = NULL; 
	}
}

void SSMM_Interface::Init(int iNNoAp =  0, int iDim = 0)
{
	iNoAp = iNNoAp;
	delete[] Aperture;  Aperture = new Matrix[iNoAp];
	for (int i = 0; i < iNoAp; i++)
		Aperture[i].Init(iDim, iDim);
	delete[] cEps_upper; cEps_upper = new cppc[iNoAp];  delete[] cEps_lower; cEps_lower = new cppc[iNNoAp];
}

SSMM_Interface::~SSMM_Interface() { 
	for (int i = 0; i<iNoAp; i++)
	{
		delete[] Aperture;  Aperture = NULL;
		delete[] cEps_upper;  cEps_upper = NULL;  delete[] cEps_lower;  cEps_lower = NULL;
	}
	iNoAp = 0; 
}

SSMM_Grating::SSMM_Grating(int iNNo_Layers, int iDimm, bool bPreconditioner)
{
	iNo_Layers = iNNo_Layers;  iDim = iDimm;
	if (iNo_Layers > 0 && iDim > 0) {
		D.Init(iDim, iDim);  G.Init(iDim, iDim);  G_inv.Init(iDim, iDim);
		smLayers = new SSMM[iNo_Layers];
	}
	if (bPreconditioner)
		smInt = new SSMM_Interface[iNo_Layers + 1];
	else
		smInt = NULL;
}

void SSMM_Grating::Init(int iNNo_Layers, int iDimm)
{
	iNo_Layers = iNNo_Layers;  iDim = iDimm;
	D.Init(iDim, iDim);  G.Init(iDim, iDim);  G_inv.Init(iDim, iDim);
	delete[] smLayers;  smLayers = new SSMM[iNo_Layers];
	smInt = new SSMM_Interface[iNo_Layers + 1];
}

void SSMM_Grating::Init(int iNoSurf, double *dxSurf, int iNoDxInp[], int *iDim)
{
	double Pi2 = 2.0 * PI; 
	iNoSections = iNoSurf + 1; 
	iNoDx = new int[iNoSurf + 1];  memcpy(iNoDx, iNoDxInp, (iNoSurf + 1) * sizeof(int));   dDx = new double[iNoSurf + 1];

	*iDim = 0; 
	for (int i = 0; i <= iNoSurf; i++)
		*iDim += iNoDx[i];
	G.Init(*iDim, *iDim);  G_inv.Init(*iDim, *iDim);
	delete[] smLayers;  smLayers = new SSMM[iNo_Layers];

	if (iNoSurf == 0) { //homogeneous medium
		dDx[0] = Pi2 / iNoDx[0];  
		return;
	} 
 
	for (int i = 1; i < iNoSurf; i++)
		dDx[i] = Pi2 * (dxSurf[i] - dxSurf[i - 1]) / (double)iNoDx[i];
	dDx[0] = dDx[iNoSurf] = Pi2 * (1.0 + dxSurf[0] - dxSurf[iNoSurf - 1]) / (double)(iNoDx[0] + iNoDx[iNoSurf]);
}

SSMM_Grating::~SSMM_Grating() {
	//discretis.:
	delete[] iNoDx; iNoDx = NULL;   delete[] dDx; dDx = NULL; 
	//layers
	delete[] smLayers;
	smLayers = NULL;
	//interfaces:
	if (smInt != NULL) {
		for (int i = 0; i < iNo_Layers + 1; i++) {
			smInt[i].~SSMM_Interface();
		}
		delete[] smInt;
		smInt = NULL;
	}
}