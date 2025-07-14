#include "RBT.h" //all project specific headers 
#include "Olovia.h" 

#include <string>
#include <fstream>
#include <iostream>


//########################################################  STRUCTS FOR OLOVIA PROJECT  #####################################################################



struct sEffIndices {
	int iMaxD = 99;  double dD0 = 1.0, dDFactor1D = 1.0403065, dDFactorCirc = 1.030722407; //max index of arrax, smallest value of D, and incremental factor: D[i] = dD0 * dDFactor^i
	int iMaxLambda = 20;  double dLambda0 = 1.2005991, dDeltaLamda = 0.01427335; //max index of arrax, smallest value of wavelength, and increment: dLambda[j] = dLambda0 + j * dDeltaLamda  
	Matrix M_n_Eff1DTE;
	Matrix M_n_Eff1DTM;
	Matrix M_n_EffCirc;
	int iTypeOfHole = PLANAR;
	double dkMidInAir, dkMid_x_nEff;
};

struct sEffIndices sEffInd;



struct sReadLiseData {
	char *szFileName = NULL; double dzStart; double dzStop; int iNoOfLines; int iNoInt; int *iNoOfLines_a = NULL; int *iNoOfLines_b = NULL;
	double **ppzInt = NULL; double **ppDataInt = NULL;
};


struct sCalcDeltaIntSq {
	int iNoPoints = 0, iNoFFTPoints = 0, ikStart, ikStop;
	double *pzMeasured = NULL, *pIntMeasured = NULL;
	dVector *pdVk = NULL, *pdVkWindow = NULL; Vector *pVSpectrum = NULL; Vector *pVSpecWindow = NULL; Vector *pVConjPhase = NULL;
	double dOrigin_z_Glob, dApodisLeft, dzFreeSpace, dzTop, dzBottom, dzBottomMax, dzFreeSpaceBottom, dApodisRight, dPeriod, dDiameter; //dOrigin: middle of envelope, origin for Fourier transform
	double dLambdaMid, dkMidMeritFu, dDelta_z, dDelta_z_LarkinAlgo, dLc, dkStart, dkStop, dkWindowLeft, dkWindowRight, dDelta_k;
	dVector *pdV_x_or_k_weight = NULL; Vector *pVSpectrumSmoothened = NULL; int iTypeOfWeights = NO_WEIGHTS; //variables for weighted least squares 
};

void vDelete_sCalcDeltaIntSq(struct sCalcDeltaIntSq *p)
{
	delete[] p->pzMeasured, delete[] p->pIntMeasured;  delete[] p->pdVk, delete[] p->pVSpectrum;  delete[]  p->pVSpecWindow;  delete[] p->pVConjPhase;
	delete[] p->pdV_x_or_k_weight;  delete[] p->pVSpectrumSmoothened;
	p->pzMeasured = NULL, p->pIntMeasured = NULL, p->pdVk = NULL, p->pVSpectrum = NULL; p->pVSpecWindow = NULL; p->pVConjPhase = NULL;  p->pdV_x_or_k_weight = NULL;
	p->pVSpectrumSmoothened = NULL;
}

struct sInfoConicalCircTSV {
	double dzTop, dDepth, dDiameterBot, dDiameterTop;
};

struct sFPData {    //struct for storing loaded data to be used in the Fabry-Perot model
	dVector dVLambda; //wavelength for which we have rigorous results, e.g. 3
	double dBasisFac_D = 1.01;      //no of diameters for which we have results, e.g. 262
	dVector VD; //diameters
	Vector *pVrb = NULL;  //refl coeffic rb in Fabry-Perot formula
	//Vector Vrh; 
	Vector V_Eb_50xObj, V_Eb_20xObj, V_Eb_10xObj, V_Eb_5xObj;
	Vector Vth_50xObj, Vth_20xObj, Vth_10xObj, Vth_5xObj;
	Vector VEh_50xObj, VEh_20xObj, VEh_10xObj, VEh_5xObj;
	int iObjType = 0;
	int iNoStairs = 10;
	bool bConcludeDiam = false; //conclusion of diameters 
};

void vDeleteFPData(struct sFPData &sFP)
{
	sFP.dVLambda.Nown = 0;  delete[] sFP.dVLambda.Data;  sFP.dVLambda.Data = NULL;
	sFP.VD.Nown = 0;  delete[] sFP.VD.Data;  sFP.VD.Data = NULL;
	delete[] sFP.pVrb;  sFP.pVrb = NULL;

	sFP.V_Eb_50xObj.Nown = 0;  delete[] sFP.V_Eb_50xObj.Data;  sFP.V_Eb_50xObj.Data = NULL;
	sFP.V_Eb_20xObj.Nown = 0;  delete[] sFP.V_Eb_20xObj.Data;  sFP.V_Eb_20xObj.Data = NULL;
	sFP.V_Eb_10xObj.Nown = 0;  delete[] sFP.V_Eb_10xObj.Data;  sFP.V_Eb_10xObj.Data = NULL;
	sFP.V_Eb_5xObj.Nown = 0;  delete[] sFP.V_Eb_5xObj.Data;   sFP.V_Eb_5xObj.Data = NULL;

	sFP.Vth_50xObj.Nown = 0;  delete[] sFP.Vth_50xObj.Data;  sFP.Vth_50xObj.Data = NULL;
	sFP.Vth_20xObj.Nown = 0;  delete[] sFP.Vth_20xObj.Data;  sFP.Vth_20xObj.Data = NULL;
	sFP.Vth_10xObj.Nown = 0;  delete[] sFP.Vth_10xObj.Data;  sFP.Vth_10xObj.Data = NULL;
	sFP.Vth_5xObj.Nown = 0;   delete[] sFP.Vth_5xObj.Data;   sFP.Vth_5xObj.Data = NULL;

	sFP.VEh_50xObj.Nown = 0;  delete[] sFP.VEh_50xObj.Data;  sFP.VEh_50xObj.Data = NULL;
	sFP.VEh_20xObj.Nown = 0;  delete[] sFP.VEh_20xObj.Data;  sFP.VEh_20xObj.Data = NULL;
	sFP.VEh_10xObj.Nown = 0;  delete[] sFP.VEh_10xObj.Data;  sFP.VEh_10xObj.Data = NULL;
	sFP.VEh_5xObj.Nown = 0;   delete[] sFP.VEh_5xObj.Data;   sFP.VEh_5xObj.Data = NULL;
}

struct sFPData sFP;


//########################################################  READ OLOVIA PROJECT FILES  #####################################################################



bool bReadEffIndices(char *szPath, bool bRealPart, Matrix &MResult, bool bInitMResult)
{
	int iLineNo = 0, iNoOfLines, i, j, iPos;
	double dTmp;
	string strNextLine;
	const char *pcString = NULL;

	ifstream fIn(szPath);
	if (!fIn.good()) {
		//fprintf(Fres, "\n Error in bReadEffIndices()! Opening of Input-file failed! \n"); 
		return FALSE;
	}

	if (!bReadNoLines(&fIn, &iNoOfLines, &iLineNo)) {
		//fprintf(Fres, "\n Error in bReadEffIndices()! Reading of iNoPoints failed! \n");
		return FALSE;
	}
	if (iNoOfLines != NO_OF_WAVELENGTHS) {
		//fprintf(Fres, "\n Error in bReadEffIndices()! Reading of iNoPoints failed! \n");
		return FALSE;
	}
	Matrix M(NO_OF_WAVELENGTHS, NO_OF_DIAMETERS);

	for (i = 0; i < iNoOfLines; i++) {
		if (!bReadNextLine_ASCII(&fIn, &strNextLine, &iLineNo))
			return FALSE;
		pcString = strNextLine.c_str();
		for (j = 0; j < NO_OF_DIAMETERS; j++) {
			while (pcString[0] == ' ' || pcString[0] == '\t')
				pcString++;
			if (sscanf_s(pcString, "%lg	", &dTmp) != 1)
				return FALSE;
			if (bRealPart)
				M.Data[i * NO_OF_DIAMETERS + j].real(dTmp);
			else
				M.Data[i * NO_OF_DIAMETERS + j].imag(-dTmp); //we propagate by exp(-ikr) 
			while (pcString[0] != ' ' && pcString[0] != '\t' && pcString[0] != '\0')
				pcString++;
		}//end for j
	}
	if (!bReadNextLine_ASCII(&fIn, &strNextLine, &iLineNo))
		return FALSE;
	iPos = (int)strNextLine.find("&", 0); //find key word "&"
	if (iPos != 0)
		return FALSE;
	if (MResult.Nown != M.Nown || MResult.Next != M.Next || bInitMResult)
		MResult = M;
	else
		MResult += M;
	return TRUE;
}




void vCalcEffInd(double dDiameter, double &dk, cppc &cnEff, int iTypeOfHole)
{
	int i, ip1, j, jp1;
	double dDFactor, di, dj, dLambda, dWi, dWip1, dWj, dWjp1;
	Matrix *pM = NULL;
	cppc cM_j_i, cM_jp1_i, cM_j_ip1, cM_jp1_ip1;

	if (iTypeOfHole == ONE_DIM_TE)
		pM = &sEffInd.M_n_Eff1DTE;
	else if (iTypeOfHole == ONE_DIM_TM)
		pM = &sEffInd.M_n_Eff1DTM;
	else if (iTypeOfHole == CIRC)
		pM = &sEffInd.M_n_EffCirc;
	else {
		cout << endl << "Warning: An effective index of 1 is used." << endl;  cnEff = 1;  return;
	}

	if (iTypeOfHole == ONE_DIM_TE || iTypeOfHole == ONE_DIM_TM)
		dDFactor = sEffInd.dDFactor1D;
	else
		dDFactor = sEffInd.dDFactorCirc;
	double dLogdDFactor = log(dDFactor), dLogdD0 = log(sEffInd.dD0);

	di = (log(dDiameter) - dLogdD0) / dLogdDFactor;
	if (di <= 0.0)
		di = 0.0;
	i = (int)di;  ip1 = i + 1;
	if (ip1 >= NO_OF_DIAMETERS)
		di = i = ip1 = NO_OF_DIAMETERS - 1;
	dWip1 = di - (double)i;  dWi = 1.0 - dWip1;

	dLambda = 2.0 * (2.0 * PI) / dk;   dj = (dLambda - sEffInd.dLambda0) / sEffInd.dDeltaLamda;
	if (dj <= 0.0)
		dj = 0.0;
	j = (int)dj;  jp1 = j + 1;
	if (jp1 >= NO_OF_WAVELENGTHS)
		dj = j = jp1 = NO_OF_WAVELENGTHS - 1;
	dWjp1 = dj - (double)j;  dWj = 1.0 - dWjp1;

	cM_j_i = (*pM)(j, i);  cM_jp1_i = (*pM)(jp1, i);  cM_j_ip1 = (*pM)(j, ip1);  cM_jp1_ip1 = (*pM)(jp1, ip1);

	cnEff = dWi * (dWj * cM_j_i + dWjp1 * cM_jp1_i) +
		dWip1 * (dWj * cM_j_ip1 + dWjp1 * cM_jp1_ip1);
}




void vCalcEffInd(double dDiameter, dVector &dV_k, Vector &VnEff, int iTypeOfHole)
{
	int i, ip1, j, jp1, m;
	double dDFactor, di, dj, dLambda, dWi, dWip1, dWj, dWjp1;
	Matrix *pM = NULL;
	cppc cM_j_i, cM_jp1_i, cM_j_ip1, cM_jp1_ip1;

	if (iTypeOfHole == ONE_DIM_TE)
		pM = &sEffInd.M_n_Eff1DTE;
	else if (iTypeOfHole == ONE_DIM_TM)
		pM = &sEffInd.M_n_Eff1DTM;
	else if (iTypeOfHole == CIRC)
		pM = &sEffInd.M_n_EffCirc;
	else {
		cout << endl << "Warning: An effective index of 1 is used." << endl;  VnEff = 1;  return;
	}

	if (iTypeOfHole == ONE_DIM_TE || iTypeOfHole == ONE_DIM_TM)
		dDFactor = sEffInd.dDFactor1D;
	else
		dDFactor = sEffInd.dDFactorCirc;
	double dLogdDFactor = log(dDFactor), dLogdD0 = log(sEffInd.dD0);

	di = (log(dDiameter) - dLogdD0) / dLogdDFactor;
	if (di <= 0.0)
		di = 0.0;
	i = (int)di;  ip1 = i + 1;
	if (ip1 >= NO_OF_DIAMETERS)
		di = i = ip1 = NO_OF_DIAMETERS - 1;
	dWip1 = di - (double)i;  dWi = 1.0 - dWip1;

	for (m = 0; m < (int)dV_k.Nown; m++) {
		dLambda = 2.0 * (2.0 * PI) / dV_k.Data[m];   dj = (dLambda - sEffInd.dLambda0) / sEffInd.dDeltaLamda;
		if (dj <= 0.0)
			dj = 0.0;
		j = (int)dj;  jp1 = j + 1;
		if (jp1 >= NO_OF_WAVELENGTHS)
			dj = j = jp1 = NO_OF_WAVELENGTHS - 1;
		dWjp1 = dj - (double)j;  dWj = 1.0 - dWjp1;

		cM_j_i = (*pM)(j, i);  cM_jp1_i = (*pM)(jp1, i);  cM_j_ip1 = (*pM)(j, ip1);  cM_jp1_ip1 = (*pM)(jp1, ip1);
		VnEff.Data[m] = dWi * (dWj * cM_j_i + dWjp1 * cM_jp1_i) +
			dWip1 * (dWj * cM_j_ip1 + dWjp1 * cM_jp1_ip1);
	}
}






void vInit_sReadLiseData(struct sReadLiseData *s)
{
	s->iNoOfLines = 0;  s->iNoOfLines_a = NULL;  s->iNoOfLines_b = NULL;  s->ppzInt = NULL;  s->ppDataInt = NULL;  s->szFileName = NULL;
}

void vFree_sReadLiseData(struct sReadLiseData *s)
{
	for (int m = 0; m < s->iNoInt; m++) {
		free(s->ppzInt[m]), free(s->ppDataInt[m]);
		free(s->iNoOfLines_a), free(s->iNoOfLines_b), free(s->ppzInt), free(s->ppDataInt);
		s->iNoOfLines_a = NULL, s->iNoOfLines_b = NULL, s->ppzInt = NULL, s->ppDataInt = NULL;
	}
}


#ifdef OLOVIA_DEBUG
void vSaveEnvelope(char *szPath, int iNoOfEnv, double **pzPeak, double **pEnv, int *piDim)
{
	int m, i;
	ofstream fOut(szPath);

	fOut << " # " << iNoOfEnv << " & " << endl;  //enabling a possibility to check if the file is read later
	for (m = 0; m < iNoOfEnv; m++)
		fOut << " # " << piDim[m];

	int iMaxNoLines = 0;
	for (m = 0; m < iNoOfEnv; m++)
		if (piDim[m] > iMaxNoLines)
			iMaxNoLines = piDim[m];

	for (i = 0; i < iMaxNoLines; i++) {
		for (m = 0; m < iNoOfEnv; m++) {
			if (i <= piDim[m] - 1)
				fOut << pzPeak[m][i] << "\t" << pEnv[m][i] << "\t";
		}
		fOut << endl;
	}

	fOut << " # " << "&" << endl; //enabling a possibility to check if the file is read later
}
#endif 



BOOL bReadLiseFileSection(char szFileName[], double dPeriod, int *iNoOfLines, double **ppz, double **ppData, int *iTypeOfHole)
{
	int i, iPos, iLineNo = 0;
	double *pz, *pData;
	string strNextLine;
	ifstream  fFile(szFileName); //fFile("_x50_Xpol_subs_a_cpp.txt"); 
	if (!fFile.good()) {
		//fprintf(Fres, "\n Error! Opening of Input-file failed! \n");
		goto END;
	}

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
		return FALSE;
	if (strstr(strNextLine.c_str(), "PLANAR") != NULL) {
		*iTypeOfHole = PLANAR;
	}
	else if (strstr(strNextLine.c_str(), "1DTE") != NULL) {
		*iTypeOfHole = ONE_DIM_TE;
	}
	else if (strstr(strNextLine.c_str(), "1DTM") != NULL) {
		*iTypeOfHole = ONE_DIM_TM;
	}
	else if (strstr(strNextLine.c_str(), "CIRC") != NULL) {
		*iTypeOfHole = CIRC;
	}
	else
		goto END;

	if (!bReadNoLines(&fFile, iNoOfLines, &iLineNo)) {
		//fprintf(Fres, "\n Error! Reading of iNoPoints failed! \n");
		goto END;
	}
	delete[] * ppz;  delete[] * ppData;
	*ppz = pz = new double[*iNoOfLines], *ppData = pData = new double[*iNoOfLines];

	int iTmp;  //double dTmp[15];
	for (i = 0; i < *iNoOfLines; i++) {
		if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
			return FALSE;
		/*if (sscanf_s(strNextLine.c_str(), "%lg	%lg %lg   %lg %lg %lg   %lg %lg %lg   %lg %lg   %lg %lg %lg %lg ",
			dTmp + 0, dTmp + 1, dTmp + 2,   dTmp + 3, dTmp + 4, dTmp + 5,   dTmp + 6, dTmp + 7, dTmp + 8,   pz, pData,   dTmp + 11, dTmp + 12, dTmp + 13, dTmp + 14) != 15)
			return FALSE; */
		if (sscanf_s(strNextLine.c_str(), "%d	%lg %lg", &iTmp, pz, pData) != 3)
			return FALSE;
		////*pz *= 1.0e-6;  //z from the file is in microns
		pz++, pData++;
	}
	iPos = (int)strNextLine.find("&", 0); //find key word "&"
	if (iPos == (int)strNextLine.npos || iPos <= 3)
		return FALSE;

	pz = *ppz;
	if (pz[*iNoOfLines - 1] < pz[0]) { //motion of reference mirror in -z - direction 
		pData = *ppData; //cancel last operation 
		double *pzTmp = new double[*iNoOfLines], *pDataTmp = new double[*iNoOfLines];
		for (int j = 0; j < *iNoOfLines; j++) {
			pDataTmp[j] = pData[*iNoOfLines - 1 - j];  pzTmp[j] = pz[*iNoOfLines - 1 - j];
		}
		memcpy(pData, pDataTmp, sizeof(double) * (*iNoOfLines));  memcpy(pz, pzTmp, sizeof(double) * (*iNoOfLines));
	}

	if (pz[*iNoOfLines - 1] - pz[0] < dPeriod) {
		cout << endl << "Error: Input file does not cover entire computational domain!" << endl;  goto END;
	}
	return TRUE;

END:
	cout << endl << "Error in function bReadLiseFileSection()! \n Abandonment!" << endl;
	return FALSE;
}



BOOL bReadLiseFile(char szFileName[], double dzStart, double dPeriod, int& iNoPixelsInPeriod, double **ppz, double **ppData, int *iTypeOfHole)
{
	int i, iPos, iLineNo = 0, iNoOfLines, iIndexStartOfPeriod = -1;
	double *pz = NULL, *pData = NULL;
	string strNextLine;
	ifstream  fFile(szFileName); //fFile("_x50_Xpol_subs_a_cpp.txt"); 
	if (!fFile.good()) {
		//fprintf(Fres, "\n Error! Opening of Input-file failed! \n");
		goto END;
	}

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
		return FALSE;
	if (strstr(strNextLine.c_str(), "PLANAR") != NULL) {
		*iTypeOfHole = PLANAR;
	}
	else if (strstr(strNextLine.c_str(), "1DTE") != NULL) {
		*iTypeOfHole = ONE_DIM_TE;
	}
	else if (strstr(strNextLine.c_str(), "1DTM") != NULL) {
		*iTypeOfHole = ONE_DIM_TM;
	}
	else if (strstr(strNextLine.c_str(), "CIRC") != NULL) {
		*iTypeOfHole = CIRC;
	}
	else {
		cout << endl << "Undefined type of structure!" << endl;   goto END;
	}

	if (!bReadNoLines(&fFile, &iNoOfLines, &iLineNo)) {
		cout << endl << "Reading of number of lines failed!" << endl;
		//fprintf(Fres, "\n Error! Reading of iNoPoints failed! \n");
		goto END;
	}
	pz = new double[iNoOfLines], pData = new double[iNoOfLines];

	int iTmp;  iNoPixelsInPeriod = 0;  //double dTmp[15];  
	for (i = 0; i < iNoOfLines; i++) {
		if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo)) {
			cout << endl << "File read error in data line i = " << i << endl;   return FALSE;
		}
		/*if (sscanf_s(strNextLine.c_str(), "%lg	%lg %lg   %lg %lg %lg   %lg %lg %lg   %lg %lg   %lg %lg %lg %lg ",
		dTmp + 0, dTmp + 1, dTmp + 2,   dTmp + 3, dTmp + 4, dTmp + 5,   dTmp + 6, dTmp + 7, dTmp + 8,   pz, pData,   dTmp + 11, dTmp + 12, dTmp + 13, dTmp + 14) != 15)
		return FALSE; */
		if (sscanf_s(strNextLine.c_str(), "%d	%lg %lg", &iTmp, pz + i, pData + i) != 3) {
			cout << endl << "Numbers in data line i = " << i << " cannot be read. " << endl;   return FALSE;
		}
		////*pz *= 1.0e-6;  //z from the file is in microns
		if (pz[i] > dzStart && iIndexStartOfPeriod < 0)
			iIndexStartOfPeriod = i;
		if (pz[i] > dzStart && pz[i] < dzStart + dPeriod)
			iNoPixelsInPeriod++;
	}
	if (iIndexStartOfPeriod > 0) { //we take also the pixel at the start or left of the period - if existing
		iIndexStartOfPeriod--;  iNoPixelsInPeriod++;
	}
	if (iIndexStartOfPeriod + (iNoPixelsInPeriod + 1) < iNoOfLines - 1) //we take also the pixel at the end of the period or right of it - if existing  
		iNoPixelsInPeriod++;
	iPos = (int)strNextLine.find("&", 0); //find key word "&"
	if (iPos == (int)strNextLine.npos || iPos <= 3 || pz[iNoOfLines - 1] < pz[0]) { //if no concluding &-sign in file or the z-values are decreasing => error: 
		cout << endl << "Input file has wrong format or file read error! Abandonment!" << endl;  goto END;
	}

	delete[](*ppz);  delete[](*ppData);
	*ppz = new double[iNoPixelsInPeriod], *ppData = new double[iNoPixelsInPeriod];
	for (i = 0; i < iNoPixelsInPeriod; i++) {
		(*ppz)[i] = pz[iIndexStartOfPeriod + i];  (*ppData)[i] = pData[iIndexStartOfPeriod + i];
	}
	delete[] pz; delete[] pData;
	return TRUE;

END:
	cout << endl << "Error in function bReadLiseFile()! \n Abandonment!" << endl;
	delete[] pz; delete[] pData;  iNoPixelsInPeriod = 0;  return FALSE;
}







BOOL bReadFTFile(int *iNoOfLines, double **ppz, double **ppDataRe, double **ppDataIm)
{
	int i, iPos, iLineNo = 0, iTmp[2];
	double *pz, *pDataRe, *pDataIm, dTmp[6];
	string strNextLine;
	ifstream  fFile("FourierTrafo_for_-z-direction_pos_part.txt"); //fFile("FourierTrafoPosPart.txt");
	if (!fFile.good()) {
		//fprintf(Fres, "\n Error! Opening of Input-file failed! \n");
		goto END;
	}

	if (!bReadNoLines(&fFile, iNoOfLines, &iLineNo)) {
		//fprintf(Fres, "\n Error! Reading of iNoPoints failed! \n");
		goto END;
	}
	*ppz = pz = new double[*iNoOfLines], *ppDataRe = pDataRe = new double[*iNoOfLines], *ppDataIm = pDataIm = new double[*iNoOfLines];

	for (i = 0; i < *iNoOfLines; i++) {
		if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
			return FALSE;
		if (sscanf_s(strNextLine.c_str(), "%d %lg %lg %lg %lg %lg", iTmp, dTmp + 1, pz, pDataRe, pDataIm, dTmp + 5) != 6)
			return FALSE;
		//*pz *= 1.0e-6;  //z from the file is in microns
		*pz++, *pDataRe++, *pDataIm++;
	}
	iPos = (int)strNextLine.find("&", 0); //find key word "&"
	if (iPos == (int)strNextLine.npos || iPos <= 3)
		return FALSE;

	return TRUE;

END:
	//fprintf(Fres, "\n Error in function bReadFTFile()! \n Abandonment! \n");
	return FALSE;
}



//##########################################################  READ TABLES FOR CIRCULAR TSV IMPERFECTIONS  #####################################################



bool bRead_rb(const char *szFile, dVector &dVLambda, int &iNoDiam, Vector **ppVrb)
{
	string str;  const char *pc;  int iLineNo = 0;  Vector *pVrb = NULL;
	ifstream fIn(szFile);
	if (!fIn.good())
	{
		//fprintf(Fres, "\n Error! Opening of Input-file failed! \n");
		goto END;
	}

	if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
		goto END;
	if (sscanf_s(pc, " %d %d ", &dVLambda.Nown, &iNoDiam) != 2) //"3  262  &":  3 wavelengths, 262 diameters
		goto END;
	pc = strstr(pc, "&");  if (pc == NULL)   goto END;
	delete[] dVLambda.Data;  dVLambda.Data = new double[dVLambda.Nown];  dVLambda = DBL_MAX;  //all entries are invalid at the beginning  
	pVrb = new Vector[dVLambda.Nown * iNoDiam];

	for (int i = 0; i < (int)dVLambda.Nown; i++) //for all wavelengths i:
	{
		if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
			goto END;
		if (sscanf_s(pc, "%lg", dVLambda.Data + i) != 1) //"+1.20000 &":  wavelength
			goto END;
		pc = strstr(pc, "&");  if (pc == NULL || dVLambda.Data[i] > FLT_MAX)   goto END;

		for (int j = 0; j < iNoDiam; j++) //for all 262 diameters: 
		{
			int jj;  double dReal = DBL_MAX, dImag = DBL_MAX;  Vector *VTmp = pVrb + i * iNoDiam + j;
			if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
				goto END;
			if (sscanf_s(pc, "%d %d %lg %lg", &jj, &VTmp->Nown, &dReal, &dImag) != 4) //"loop variable, number of data points, first complex coeffic  Re + Im * i
				goto END;
			if (jj != j || VTmp->Nown < 2 || VTmp->Nown > 1000 || fabs(dReal) > FLT_MAX || fabs(dImag) > FLT_MAX)
				goto END;
			VTmp->Data = new cppc[VTmp->Nown];  *VTmp = DBL_MAX; //mark all entries as invalid at the beginning 
			VTmp->Data[0].real(dReal), VTmp->Data[0].imag(-dImag);
			for (int m = 1; m < (int)VTmp->Nown; m++) //read line of complex coeffic rb
			{
				pc = strstr(pc, "i");  if (pc == NULL)  goto END;
				pc++;  dReal = DBL_MAX, dImag = DBL_MAX;
				if (sscanf_s(pc, "%lg %lg", &dReal, &dImag) != 2) //other complex coeffic  Re + Im * i   in this line
					goto END;
				if (fabs(dReal) > FLT_MAX || fabs(dImag) > FLT_MAX)  goto END;
				VTmp->Data[m].real(dReal), VTmp->Data[m].imag(-dImag);
			}//end for m (all coeffic)
			pc = strstr(pc, "i");   if (pc == NULL)  goto END;
		}//end for j (all diameters)

		if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
			goto END;
		pc = strstr(pc, "&");  if (pc == NULL)  goto END; //check whether data stream is still OK
	}//end for i (all wavelengths)  

	delete[] * ppVrb;   *ppVrb = pVrb;
	return true;

END:
	//fprintf(Fres, "\n Error in line %d in the function reading the reflection coefficients()! \n Abandonment! \n", iLineNo); 
	dVLambda.Nown = 0;  delete[] dVLambda.Data;  dVLambda.Data = NULL;   iNoDiam = 0;
	delete[] * ppVrb;   *ppVrb = NULL;    delete[] pVrb;  pVrb = NULL;
	return FALSE;
}



bool bRead_Ebas_rb(char *szFile, char *pcObjType, dVector &dVLambda, int &iNoDiam, Vector **ppV_Ebas_rb)
{
	string str;  const char *pc;  int iLineNo = 0;  Vector *pV_Ebas_rb = NULL;
	ifstream fIn(szFile);
	if (!fIn.good())
	{
		//fprintf(Fres, "\n Error! Opening of Input-file failed! \n");
		goto END;
	}

	if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
		goto END;
	pc = strstr(pc, pcObjType);  if (pc == NULL)  goto END; //e.g. "50x objective" 
	if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
		goto END;
	if (sscanf_s(pc, " %d %d ", &dVLambda.Nown, &iNoDiam) != 2) //"3  262  &":  3 wavelengths, 262 diameters
		goto END;
	pc = strstr(pc, "&");  if (pc == NULL)   goto END; //a check whether the input stream is still OK here ...
	delete[] dVLambda.Data;  dVLambda.Data = new double[dVLambda.Nown];  dVLambda = DBL_MAX;  //all entries are invalid at the beginning  
	pV_Ebas_rb = new Vector[dVLambda.Nown * iNoDiam];

	for (int i = 0; i < (int)dVLambda.Nown; i++) //for all wavelengths i:
	{
		if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
			goto END;
		if (sscanf_s(pc, "%lg", dVLambda.Data + i) != 1) //"+1.20000 &":  wavelength
			goto END;
		pc = strstr(pc, "&");  if (pc == NULL || dVLambda.Data[i] > FLT_MAX)   goto END;

		for (int j = 0; j < iNoDiam; j++) //for all 262 diameters: 
		{
			int jj;  double dReal = DBL_MAX, dImag = DBL_MAX;  Vector *VTmp = pV_Ebas_rb + i * iNoDiam + j;
			if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
				goto END;
			if (sscanf_s(pc, "%d %d %lg %lg", &jj, &VTmp->Nown, &dReal, &dImag) != 4) //"loop variable, number of data points, first complex coeffic  Re + Im * i
				goto END;
			if (jj != j || VTmp->Nown < 2 || VTmp->Nown > 1000 || fabs(dReal) > FLT_MAX || fabs(dImag) > FLT_MAX)
				goto END;
			VTmp->Data = new cppc[VTmp->Nown];  *VTmp = DBL_MAX; //mark all entries as invalid at the beginning 
			VTmp->Data[0].real(dReal), VTmp->Data[0].imag(dImag);
			for (int m = 1; m < (int)VTmp->Nown; m++) //read line of complex coeffic rbrh
			{
				pc = strstr(pc, "i");  if (pc == NULL)  goto END;
				pc++;  dReal = DBL_MAX, dImag = DBL_MAX;
				if (sscanf_s(pc, "%lg %lg", &dReal, &dImag) != 2) //other complex coeffic  Re + Im * i   in this line
					goto END;
				if (fabs(dReal) > FLT_MAX || fabs(dImag) > FLT_MAX)  goto END;
				VTmp->Data[m].real(dReal), VTmp->Data[m].imag(dImag);
			}//end for m (all coeffic)
			pc = strstr(pc, "i");   if (pc == NULL)  goto END;
		}//end for j (all diameters)

		if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
			goto END;
		pc = strstr(pc, "&");  if (pc == NULL)  goto END; //check whether data stream is still OK 
	}//end for i (all wavelengths)  

	delete[] * ppV_Ebas_rb;   *ppV_Ebas_rb = pV_Ebas_rb;
	return true;

END:
	//fprintf(Fres, "\n Error in line %d in function bRead_rbrh()! \n Abandonment! \n", iLineNo);
	dVLambda.Nown = 0;  delete[] dVLambda.Data;  dVLambda.Data = NULL;   iNoDiam = 0;
	delete[] * ppV_Ebas_rb;   *ppV_Ebas_rb = NULL;    delete[] pV_Ebas_rb;  pV_Ebas_rb = NULL;
	return FALSE;
}



bool bRead_Eh_or_Eb_or_th_or_rh(const char *szFile, const char *pcObjType, dVector &dVLambda, int &iNoDiam, Vector &Vth_or_Eh)
{
	string str;  const char *pc;  int iLineNo = 0;
	ifstream fIn(szFile);
	if (!fIn.good())
	{
		//fprintf(Fres, "\n Error! Opening of Input-file failed! \n");
		goto END;
	}

	if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
		goto END;
	pc = strstr(pc, pcObjType);  if (pc == NULL)  goto END; //e.g. "50x objective" 
	if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
		goto END;
	if (sscanf_s(pc, " %d %d ", &dVLambda.Nown, &iNoDiam) != 2) //"3  262  &":  3 wavelengths, 262 diameters
		goto END;
	pc = strstr(pc, "&");  if (pc == NULL)   goto END; //a check whether the input stream is still OK here ...
	delete[] dVLambda.Data;  dVLambda.Data = new double[dVLambda.Nown];  dVLambda = DBL_MAX;  //all entries are invalid at the beginning  
	Vth_or_Eh.Nown = dVLambda.Nown * iNoDiam;  delete[] Vth_or_Eh.Data;  Vth_or_Eh.Data = new cppc[Vth_or_Eh.Nown];  Vth_or_Eh = DBL_MAX;

	for (int i = 0; i < (int)dVLambda.Nown; i++) //for all wavelengths i:
	{
		if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
			goto END;
		if (sscanf_s(pc, "%lg", dVLambda.Data + i) != 1) //"+1.20000 &":  wavelength
			goto END;
		pc = strstr(pc, "&");  if (pc == NULL || dVLambda.Data[i] > FLT_MAX)   goto END;

		for (int j = 0; j < iNoDiam; j++) //for all 262 diameters: 
		{
			double dReal = DBL_MAX, dImag = DBL_MAX;
			if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
				goto END;
			if (sscanf_s(pc, "%lg %lg", &dReal, &dImag) != 2) //"complex coeffic  Re + Im * i
				goto END;
			if (fabs(dReal) > FLT_MAX || fabs(dImag) > FLT_MAX)
				goto END;
			pc = strstr(pc, "i");   if (pc == NULL)  goto END;
			Vth_or_Eh.Data[i * iNoDiam + j].real(dReal);   Vth_or_Eh.Data[i * iNoDiam + j].imag(-dImag);
		}//end for j (all diameters)
		if (!bReadNextLine(&fIn, &str, &pc, &iLineNo))
			goto END;
		pc = strstr(pc, "&");  if (pc == NULL)  goto END; //check whether data stream is still OK 

	}//end for i (all wavelengths)  
	return true;

END:
	//fprintf(Fres, "\n Error in line %d in the function reading the reflection and transmiision coefficients! \n Abandonment! \n", iLineNo);
	dVLambda.Nown = 0;  delete[] dVLambda.Data;   dVLambda.Data = NULL;   iNoDiam = 0;
	Vth_or_Eh.Nown = 0;  delete[] Vth_or_Eh.Data;  Vth_or_Eh.Data = NULL;
	return FALSE;
}



bool bReadFPData()
{
	int iNoDiam = 0, i;

	//---------------------------------------------------  r_bas  -------------------------------------------------------------------------------------------------------

	if (!bRead_rb("./Imperfect_TSV/Circ_all_Obj_rb.txt", sFP.dVLambda, iNoDiam, &sFP.pVrb)) // rb * rh
		goto END;
	sFP;
	sFP.dVLambda.Data[0];			sFP.dVLambda.Data[2];
	sFP.pVrb[0].Data[0];			sFP.pVrb[0].Data[16];
	sFP.pVrb[3 * 262 - 1].Data[0];	sFP.pVrb[3 * 262 - 1].Data[216];   sFP.pVrb[3 * 262 - 1].Data[223];

	//---------------------------------------------------  E_bas  -------------------------------------------------------------------------------------------------------


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x05_Obj_Ebas.txt", "05x objective", sFP.dVLambda, iNoDiam, sFP.V_Eb_5xObj))  // Eb for 5x  
		goto END;
	sFP;
	sFP.dVLambda.Data[0];               sFP.dVLambda.Data[2];
	sFP.V_Eb_5xObj.Data[0];			    sFP.V_Eb_5xObj.Data[261];
	sFP.V_Eb_5xObj.Data[2 * 262];	    sFP.V_Eb_5xObj.Data[3 * 262 - 1];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x10_Obj_Ebas.txt", "10x objective", sFP.dVLambda, iNoDiam, sFP.V_Eb_10xObj))  // Eb for 10x  
		goto END;
	sFP;
	sFP.dVLambda.Data[0];               sFP.dVLambda.Data[2];
	sFP.V_Eb_10xObj.Data[0];			sFP.V_Eb_10xObj.Data[261];
	sFP.V_Eb_10xObj.Data[2 * 262];	    sFP.V_Eb_10xObj.Data[3 * 262 - 1];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x20_Obj_Ebas.txt", "20x objective", sFP.dVLambda, iNoDiam, sFP.V_Eb_20xObj))  // Eb for 20x  
		goto END;
	sFP;
	sFP.dVLambda.Data[0];               sFP.dVLambda.Data[2];
	sFP.V_Eb_20xObj.Data[0];			sFP.V_Eb_20xObj.Data[261];
	sFP.V_Eb_20xObj.Data[2 * 262];	    sFP.V_Eb_20xObj.Data[3 * 262 - 1];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x50_Obj_Ebas.txt", "50x objective", sFP.dVLambda, iNoDiam, sFP.V_Eb_50xObj))  // Eb for 50x  
		goto END;
	sFP;
	sFP.dVLambda.Data[0];               sFP.dVLambda.Data[2];
	sFP.V_Eb_50xObj.Data[0];			sFP.V_Eb_50xObj.Data[261];
	sFP.V_Eb_50xObj.Data[2 * 262];	    sFP.V_Eb_50xObj.Data[3 * 262 - 1];

	//------------------------------------------------  t_haut  ----------------------------------------------------------------------------------------------------------

	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x05_Obj_th.txt", "05x objective", sFP.dVLambda, iNoDiam, sFP.Vth_5xObj)) // th for 5x
		goto END;
	sFP;
	sFP.dVLambda.Data[0];  sFP.dVLambda.Data[2];
	sFP.Vth_5xObj.Data[0]; sFP.Vth_5xObj.Data[261];    sFP.Vth_5xObj.Data[262]; sFP.Vth_5xObj.Data[262 + 261];    sFP.Vth_5xObj.Data[2 * 262]; sFP.Vth_5xObj.Data[2 * 262 + 261];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x10_Obj_th.txt", "10x objective", sFP.dVLambda, iNoDiam, sFP.Vth_10xObj)) // th for 10x
		goto END;
	sFP;
	sFP.dVLambda.Data[0];   sFP.dVLambda.Data[2];
	sFP.Vth_10xObj.Data[0]; sFP.Vth_10xObj.Data[261];    sFP.Vth_10xObj.Data[262]; sFP.Vth_10xObj.Data[262 + 261];    sFP.Vth_10xObj.Data[2 * 262]; sFP.Vth_10xObj.Data[2 * 262 + 261];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x20_Obj_th.txt", "20x objective", sFP.dVLambda, iNoDiam, sFP.Vth_20xObj)) // th for 20x
		goto END;
	sFP;
	sFP.dVLambda.Data[0];			sFP.dVLambda.Data[2];
	sFP.Vth_20xObj.Data[0];			sFP.Vth_20xObj.Data[261];
	sFP.Vth_20xObj.Data[262];		sFP.Vth_20xObj.Data[262 + 261];
	sFP.Vth_20xObj.Data[2 * 262];	sFP.Vth_20xObj.Data[2 * 262 + 261];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x50_Obj_th.txt", "50x objective", sFP.dVLambda, iNoDiam, sFP.Vth_50xObj)) // th for 50x  
		goto END;
	sFP;
	sFP.dVLambda.Data[0];			sFP.dVLambda.Data[2];
	sFP.Vth_50xObj.Data[0];			sFP.Vth_50xObj.Data[261];
	sFP.Vth_50xObj.Data[262];		sFP.Vth_50xObj.Data[262 + 261];
	sFP.Vth_50xObj.Data[2 * 262];	sFP.Vth_50xObj.Data[2 * 262 + 261];

	//-------------------------------------------  E_haut  ---------------------------------------------------------------------------------------------------------------

	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x05_Obj_E_haut.txt", "05x objective", sFP.dVLambda, iNoDiam, sFP.VEh_5xObj)) // Eh for 5x 
		goto END;
	sFP;
	sFP.dVLambda.Data[0];  sFP.dVLambda.Data[2];
	sFP.VEh_5xObj.Data[0]; sFP.VEh_5xObj.Data[261];    sFP.VEh_5xObj.Data[262]; sFP.VEh_5xObj.Data[262 + 261];    sFP.VEh_5xObj.Data[2 * 262]; sFP.VEh_5xObj.Data[2 * 262 + 261];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x10_Obj_E_haut.txt", "10x objective", sFP.dVLambda, iNoDiam, sFP.VEh_10xObj)) // Eh for 10x
		goto END;
	sFP;
	sFP.dVLambda.Data[0];   sFP.dVLambda.Data[2];
	sFP.VEh_10xObj.Data[0]; sFP.VEh_10xObj.Data[261];    sFP.VEh_10xObj.Data[262]; sFP.VEh_10xObj.Data[262 + 261];    sFP.VEh_10xObj.Data[2 * 262]; sFP.VEh_10xObj.Data[2 * 262 + 261];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x20_Obj_E_haut.txt", "20x objective", sFP.dVLambda, iNoDiam, sFP.VEh_20xObj)) // Eh for 20x
		goto END;
	sFP;
	sFP.dVLambda.Data[0];			sFP.dVLambda.Data[2];
	sFP.VEh_20xObj.Data[0];			sFP.VEh_20xObj.Data[261];
	sFP.VEh_20xObj.Data[262];		sFP.VEh_20xObj.Data[262 + 261];
	sFP.VEh_20xObj.Data[2 * 262];	sFP.VEh_20xObj.Data[2 * 262 + 261];


	if (!bRead_Eh_or_Eb_or_th_or_rh("./Imperfect_TSV/Circ_x50_Obj_E_haut.txt", "50x objective", sFP.dVLambda, iNoDiam, sFP.VEh_50xObj)) // Eh for 50x
		goto END;
	sFP;
	sFP.dVLambda.Data[0];			sFP.dVLambda.Data[2];
	sFP.VEh_50xObj.Data[0];			sFP.VEh_50xObj.Data[261];
	sFP.VEh_50xObj.Data[262];		sFP.VEh_50xObj.Data[262 + 261];
	sFP.VEh_50xObj.Data[2 * 262];	sFP.VEh_50xObj.Data[2 * 262 + 261];

	sFP.VD.Nown = iNoDiam;   delete[] sFP.VD.Data;   sFP.VD.Data = new double[iNoDiam];
	for (i = 0; i < iNoDiam; i++)
		sFP.VD.Data[i] = pow(sFP.dBasisFac_D, i);
	return true;

END:
	cout << endl << "Loading of coupling coefficients failed. File load error! \n ";   vDeleteFPData(sFP);  return false;
}


//############################################################  INTERPOLATION FOR FABRY PEROT FORMULA  ###################################################################



bool bInterpol_h(int iD, double dh, Vector *pVecrb, cppc &crb)
{
	int j;
	static double dNumNoise = 0.1 * NUM_NOISE_FLOAT, dhArray[4];
	double dDepth = 0.5 * sFP.VD.Data[iD];  double dDelta_h = dDepth / (pVecrb->Nown - 1);  double dIndex_h = dh / dDelta_h;

	if (dIndex_h <= -dNumNoise) {
		crb = pVecrb->Data[0];  return false;
	}
	if (dIndex_h >= pVecrb->Nown - 1 + dNumNoise) {
		crb = pVecrb->Data[pVecrb->Nown - 1];  return false;
	}

	int iIndex_h = (int)(dIndex_h + 0.5);
	if (abs(iIndex_h - dIndex_h) <= dNumNoise) {
		crb = pVecrb->Data[iIndex_h];  return true;
	}

	int iIndex_L = (int)dIndex_h;  int iIndex_LL = iIndex_L - 1, iIndexR = iIndex_L + 1;   //(int)(-0.1) = 0 
	if (iIndex_L < 0 || iIndexR >(int)pVecrb->Nown - 1)
		return false;
	for (j = 0; j < 4; j++)
		dhArray[j] = (iIndex_LL + j) * dDelta_h;
	return bParabolicFit_4(dh, iIndex_LL, pVecrb->Nown - 1, dhArray, pVecrb->Data + iIndex_LL, &crb, NULL, NULL);
}


#define MACRO_ERROR_AT_(s) {cout << endl << "Calculation failure at the TSV "<< s <<" : The parameters are out of the considered parameter region!";  return false;}

bool bInterpol_FP_Coeff(int iLambda, double dD_Bot, double dD_Top, double dh, cppc& cEh, cppc& cth, cppc& cEb, cppc& crb)
{
	static double dNumNoiseFloat = 0.1 * NUM_NOISE_FLOAT;
	int j;   static Vector *pVec_rb = NULL;
	static cppc *pData_Eh = NULL, *pData_th = NULL, *pData_Eb = NULL; // , cArray_rb[4] = { DBL_MAX, DBL_MAX, DBL_MAX, DBL_MAX };

	if (dD_Top <= 1.0 - dNumNoiseFloat || dD_Top >= pow(1.01, sFP.VD.Nown - 1) + dNumNoiseFloat)
		MACRO_ERROR_AT_("TOP");
	if (dD_Bot <= 1.0 - dNumNoiseFloat || dD_Bot >= pow(1.01, sFP.VD.Nown - 1) + dNumNoiseFloat)
		MACRO_ERROR_AT_("BOT");
	double dDL_Bot = log(dD_Bot) / log(sFP.dBasisFac_D);   int iD_Bot = (int)(dDL_Bot + 0.5);  int iIndex_Bot = iLambda * sFP.VD.Nown + iD_Bot; // +0.5 ?
	double dDL_Top = log(dD_Top) / log(sFP.dBasisFac_D);   int iD_Top = (int)(dDL_Top + 0.5);  //int iIndex_Top = iLambda * sFP.VD.Nown + iD_Top; // +0.5 ? 
	if (iD_Top < 0 || iD_Top >(int)sFP.VD.Nown - 1)
		MACRO_ERROR_AT_("TOP");
	if (iD_Bot < 0 || iD_Bot >(int)sFP.VD.Nown - 1)
		MACRO_ERROR_AT_("BOT");

	if (sFP.iObjType == X5_OBJ) {
		pData_Eh = sFP.VEh_5xObj.Data;   pData_th = sFP.Vth_5xObj.Data;    pData_Eb = sFP.V_Eb_5xObj.Data;
	}
	else if (sFP.iObjType == X10_OBJ) {
		pData_Eh = sFP.VEh_10xObj.Data;  pData_th = sFP.Vth_10xObj.Data;   pData_Eb = sFP.V_Eb_10xObj.Data;
	}
	else if (sFP.iObjType == X20_OBJ) {
		pData_Eh = sFP.VEh_20xObj.Data;  pData_th = sFP.Vth_20xObj.Data;   pData_Eb = sFP.V_Eb_20xObj.Data;
	}
	else if (sFP.iObjType == X50_OBJ) {
		pData_Eh = sFP.VEh_50xObj.Data;  pData_th = sFP.Vth_50xObj.Data;   pData_Eb = sFP.V_Eb_50xObj.Data;
	}
	else {
		cout << endl << "The selected objective is unknown!";  return false;
	}
	pVec_rb = sFP.pVrb;

	//interpol of crb for h and then for D_Bot: 
	if (abs(iD_Bot - dDL_Bot) <= dNumNoiseFloat) { //take values with index iD from the tables: 
		if (bInterpol_h(iD_Bot, dh, pVec_rb + iIndex_Bot, crb) == false)
			MACRO_ERROR_AT_("BOT");
	}//end if no interpolation of D_Bot  
	else {
		int iDL_Bot = (int)(floor(dDL_Bot) + 0.5);
		if (iDL_Bot < 0 || iDL_Bot >= (int)sFP.VD.Nown - 1) //if diameter is not existing: keep in mind: iDR_Bot = iDL_Bot + 1 
			MACRO_ERROR_AT_("BOT");
		int iDLL_Bot = iDL_Bot - 1;   int iIndexLL_Bot = iLambda * sFP.VD.Nown + iDLL_Bot;  cppc cArray_rb[4];
		if (iDLL_Bot >= 0)
			if (bInterpol_h(iDLL_Bot, dh, pVec_rb + iIndexLL_Bot, cArray_rb[0]) == false)
				MACRO_ERROR_AT_("BOT");
		for (j = 1; j <= 2; j++)
			if (bInterpol_h(iDLL_Bot + j, dh, pVec_rb + iIndexLL_Bot + j, cArray_rb[j]) == false)  //interpolate for height h first...
				MACRO_ERROR_AT_("BOT");
		if (iDLL_Bot + 3 <= (int)sFP.VD.Nown - 1)
			if (bInterpol_h(iDLL_Bot + 3, dh, pVec_rb + iIndexLL_Bot + 3, cArray_rb[3]) == false)
				MACRO_ERROR_AT_("BOT");
		if (bParabolicFit_4(dD_Bot, iDLL_Bot, sFP.VD.Nown - 1, sFP.VD.Data + iDLL_Bot, cArray_rb, &crb, NULL, NULL) == false) //... then interpolation for Diameter
			MACRO_ERROR_AT_("BOT");
	}//end else D_Bot  

	/*
	//same for D_Top:
	if ( abs(iD_Top - dDL_Top) <= dNumNoiseFloat ) { //take values with index iD from the tables:
		cEh = pData_Eh[iIndex_Top];   cth = pData_th[iIndex_Top];   cEb = pData_Eb[iIndex_Top];
	}//end if no interpol. of D_Top
	else {
		int iDL_Top = (int)(floor(dDL_Top) + 0.5);
		if ( iDL_Top < 0 || iDL_Top >= (int)sFP.VD.Nown - 1 )
			MACRO_ERROR_AT_("TOP");
		int iDLL_Top = iDL_Top - 1;  int iIndexLL_Top = iLambda * sFP.VD.Nown + iDLL_Top;
		if (bParabolicFit_4(dD_Top, iDLL_Top, sFP.VD.Nown - 1, sFP.VD.Data + iDLL_Top, pData_Eh + iIndexLL_Top, &cEh, NULL, NULL) == false ||
			bParabolicFit_4(dD_Top, iDLL_Top, sFP.VD.Nown - 1, sFP.VD.Data + iDLL_Top, pData_th + iIndexLL_Top, &cth, NULL, NULL) == false ||
			bParabolicFit_4(dD_Top, iDLL_Top, sFP.VD.Nown - 1, sFP.VD.Data + iDLL_Top, pData_Eb + iIndexLL_Top, &cEb, NULL, NULL) == false)
			MACRO_ERROR_AT_("TOP");
	}//end else interpol. of D_Top
	*/

	double  dRadius = 0.5 * dD_Top, dWaist = 4.346; //6.24;   // dWaist *= sFP.dVLambda.Data[iLambda] / dLambdaMid;
	double dExp = dRadius / dWaist;
	cth = 1.0;  cEh = exp(-2.0 * dExp * dExp);  // cEh * (-0.5559) / (-0.5559) reflection coeffic is normalized to one 
	cEb = (cOne - cEh);  crb = crb / (-0.5559); //reflection coeffic is normalized to one 

	return true;
}


bool bInterpol_FP(dVector *pdVkWindow, double& dD_Bot, double& dD_Top, double& dh, cppc *pcEh, cppc *pcth, cppc *pcEb, cppc *pcrb)
{
	int i;   double dk, dDelta_k, dDelta_k_sq;
	static dVector dV_k(3);
	static cppc cEh[3], cth[3], cEb[3], crb[3], cEhCoeff[3], cthCoeff[3], cEbCoeff[3], crbCoeff[3];

	for (i = 0; i <= 2; i++) { //calc Eh, th, Eb, rb for all 3 lambda for subsequent fit of the k-dependence  
		if (bInterpol_FP_Coeff(i, dD_Bot, dD_Top, dh, cEh[i], cth[i], cEb[i], crb[i]) == false)
			return false;
		dV_k.Data[i] = 2 * PI / sFP.dVLambda.Data[i];
	}

	dk = (dV_k.Data[0] + dV_k.Data[2]) / 2.0; //the k-vector of the light is half the k-vector of the OCT fringes
	if (bParabolicFit_3(dk, dV_k.Data, cEh, cEhCoeff, cEhCoeff + 1, cEhCoeff + 2) == false ||
		bParabolicFit_3(dk, dV_k.Data, cth, cthCoeff, cthCoeff + 1, cthCoeff + 2) == false ||
		bParabolicFit_3(dk, dV_k.Data, cEb, cEbCoeff, cEbCoeff + 1, cEbCoeff + 2) == false ||
		bParabolicFit_3(dk, dV_k.Data, crb, crbCoeff, crbCoeff + 1, crbCoeff + 2) == false)
		return false;

	cout << endl << "i" << "\t" << "pdVkWindow->Data[i] / 2.0" << "\t" << "dDelta_k" << "\t" << "pcEh[i]" << "\t\t\t\t\t" << "pcth[i]" << "\t\t\t\t" << "pcEb[i]" << "\t\t\t\t\t" << "pcrb[i]";
	for (i = 0; i < (int)pdVkWindow->Nown; i++) {
		dDelta_k = pdVkWindow->Data[i] / 2.0 - dk;   dDelta_k_sq = dDelta_k * dDelta_k;
		pcEh[i] = cEhCoeff[0] + cEhCoeff[1] * dDelta_k + cEhCoeff[2] * dDelta_k_sq;
		pcth[i] = cthCoeff[0] + cthCoeff[1] * dDelta_k + cthCoeff[2] * dDelta_k_sq;
		pcEb[i] = cEbCoeff[0] + cEbCoeff[1] * dDelta_k + cEbCoeff[2] * dDelta_k_sq;
		pcrb[i] = crbCoeff[0] + crbCoeff[1] * dDelta_k + crbCoeff[2] * dDelta_k_sq;
		cout << endl << i << "\t" << pdVkWindow->Data[i] / 2.0 << "\t\t\t\t" << dDelta_k << "\t" << pcEh[i] << "\t\t" << pcth[i] << "\t\t" << pcEb[i] << "\t\t" << pcrb[i];
	}
	return true;
}


bool bInterpol_FP_with_scaling(dVector *pdVkWindow, double& dD_Bot, double& dD_Top, double& dh, cppc *pcEh, cppc *pcth, cppc *pcEb, cppc *pcrb)
{
	int i, j;   double dLambda, dk;
	static double dScalFactor[3], dArray_D_Bot[3], dArray_D_Top[3], dArray_h[3]; // , dArray_k[3];
	static cppc cEhArray[3], cthArray[3], cEbArray[3], crbArray[3];

	cout << endl << "i" << "\t" << "pdVkWindow->Data[i] / 2.0" << "\t" << "pcEh[i]" << "\t\t\t\t" << "pcth[i]" << "\t\t" << "pcEb[i]" << "\t\t\t" << "pcrb[i]";
	for (j = 0; j < (int)pdVkWindow->Nown; j++) { //for all frequencies: 
		dk = 0.5 * pdVkWindow->Data[j]; //the k-vector of the light is half the k-vector of the OCT fringes 
		dLambda = 2.0 * PI / dk;
		for (i = 0; i <= 2; i++) {
			dScalFactor[i] = sFP.dVLambda.Data[i] / dLambda;
			dArray_D_Bot[i] = dScalFactor[i] * dD_Bot;   dArray_D_Top[i] = dScalFactor[i] * dD_Top;	  dArray_h[i] = dScalFactor[i] * dh;
			//calc Eh, th, Eb, rb for all 3 lambda for subsequent fit of the k-dependence  
			if (bInterpol_FP_Coeff(i, dArray_D_Bot[i], dD_Top, dArray_h[i], cEhArray[i], cthArray[i], cEbArray[i], crbArray[i]) == false)
				return false;
			//dArray_k[i] = 2 * PI / sFP.dVLambda.Data[i]; //interpolation in dependence on lambda is better here 
		}
		if (bParabolicFit_3(dLambda, sFP.dVLambda.Data, cEhArray, pcEh + j, NULL, NULL) == false ||
			bParabolicFit_3(dLambda, sFP.dVLambda.Data, cthArray, pcth + j, NULL, NULL) == false ||
			bParabolicFit_3(dLambda, sFP.dVLambda.Data, cEbArray, pcEb + j, NULL, NULL) == false ||
			bParabolicFit_3(dLambda, sFP.dVLambda.Data, crbArray, pcrb + j, NULL, NULL) == false) //interpolation in dependence on lambda is better here 
			return false;
		//Output: 
		cout << endl << j << "\t" << dk << "\t\t\t\t" << pcEh[j] << "\t\t\t" << pcth[j] << "\t\t" << pcEb[j] << "\t\t" << pcrb[j];
	}//end for j
	cout << endl << "pcrb[78] = " << abs(pcrb[78]) << endl;
	return true;
}




//####################################################################  ENVELOPE DETECTION  ###################################################################


void vCalcEnvelope(double *pInt, double *dEnvelope, int iNoPoints)
{
	double dI2_I4, dI1_I3_I5, dRad;
	dEnvelope -= 2;
	for (int i = 2; i <= iNoPoints - 3; i++) { //the 2 leftmost and the 2 rightmost points of the envelope cannot be calculated (not defined)
		dI2_I4 = pInt[i - 1] - pInt[i + 1];  dI2_I4 *= dI2_I4;
		dI1_I3_I5 = (pInt[i - 2] - pInt[i]) * (pInt[i] - pInt[i + 2]);
		dRad = dI2_I4 - dI1_I3_I5;
		dEnvelope[i] = dSqrt(dRad);
	}
}


//###########################################################  PEAK DETECTION BY FOURIER TRANSFORM  ##############################################################


cppc cAnalytFT_of_Int(double *pz, double *pInt, int iNoPoints, double dk)
{
	int i;
	double dzRight = 0.0;
	cppc cFT = 0.0, cExpLeft = 0.0, cExpRight = 0.0, c2ik = 2.0 * cIm * dk;

	dzRight = 0.5 * (pz[0] + pz[1]);  cExpRight = exp(c2ik * dzRight);
	for (i = 1; i <= iNoPoints - 2; i++) {
		cExpLeft = cExpRight;
		dzRight = 0.5 * (pz[i] + pz[i + 1]);  cExpRight = exp(c2ik * dzRight);
		cFT += pInt[i] * (cExpRight - cExpLeft);
	}
	cFT *= 1.0 / c2ik;
	return cFT;
}


double dCalcAngleForCalcOfDepth(double *pz1, double *pInt1, int iNoPoints1, double *pz2, double *pInt2, int iNoPoints2, double dk)
{
	cppc cFT1, cFT2;
	double dAngle;

	cFT1 = cAnalytFT_of_Int(pz1, pInt1, iNoPoints1, dk), cFT2 = cAnalytFT_of_Int(pz2, pInt2, iNoPoints2, dk);
	dAngle = arg(cFT2 / cFT1);
	return dAngle;
}


void vCalcDepthByFT_of_Int(double *pz1, double *pInt1, int iNoPoints1, double *pz2, double *pInt2, int iNoPoints2, double dk,
	double dIntendedDepth, double dn_group, double *dResultingDepth)
{
	double dk_plus, dk_minus, dAngle_plus, dAngle_minus, dDeriv_k, dStepSize = 1.0e-2, dDelta_k;

	dDelta_k = (dStepSize * PI) / (2.0 * dn_group * dIntendedDepth);
	dk_plus = dk + dDelta_k;  dk_minus = dk - dDelta_k;
	dAngle_plus = dCalcAngleForCalcOfDepth(pz1, pInt1, iNoPoints1, pz2, pInt2, iNoPoints2, dk_plus);
	dAngle_minus = dCalcAngleForCalcOfDepth(pz1, pInt1, iNoPoints1, pz2, pInt2, iNoPoints2, dk_minus);
	dDeriv_k = (dAngle_plus - dAngle_minus) / (2.0 * dDelta_k);
	*dResultingDepth = dDeriv_k / (2.0 * dn_group);
}


//#############################################################  SPECTRUM  ################################################################


int iFind_z_At_z0(int iDim, double *pz, double dz0, int iPosition)
{
	int iIndexLeft = 0, iIndexMid = iDim / 2, iIndexRight = iDim - 1;
	while (iIndexRight > iIndexLeft + 1) {
		iIndexMid = (iIndexLeft + iIndexRight) / 2;
		if (pz[iIndexMid] > dz0)
			iIndexRight = iIndexMid;
		else if (pz[iIndexMid] < dz0)
			iIndexLeft = iIndexMid;
		else
			return iIndexMid;
	}
	if (iPosition == -1) //(bLeftOfz0)
		return iIndexLeft;
	else if (iPosition == +1)
		return iIndexRight;
	else {
		if (dz0 - pz[iIndexLeft] > pz[iIndexRight] - dz0)
			return iIndexRight;
		else
			return iIndexLeft;
	}
}



void vApodisation(double dStart, double dStop, double dWindowLeft, double dWindowRight, int iNoLines, double *pz, double *pData)
//apodisation with half-cosine to avoid Gibbs-ringing after Fourier trafo. 
{
#ifdef OLOVIA_DEBUG
	ofstream FOutApodis("Apodis_of_double_val.txt");
#endif
	double dApodisation = 0.0, dArg, dcos_arg, dzLeft = dWindowLeft - dStart, dzRight = dWindowRight - dStop;
	for (int j = 0; j < iNoLines; j++) {
		if (pz[j] < dStart || pz[j] > dStop) {
			dApodisation = 0.0;  pData[j] = 0.0;
#ifdef OLOVIA_DEBUG
			FOutApodis << pz[j] << "\t" << dApodisation << "\t" << pData[j] << endl;
#endif 
			continue;
		}
		if (pz[j] < dWindowLeft) {
			dArg = PI * (pz[j] - dStart) / dzLeft;  dcos_arg = cos(dArg);
			dApodisation = 1.0 - dcos_arg;
			dApodisation *= 0.5;  pData[j] *= dApodisation;
		}
		else if (pz[j] > dWindowRight) {
			dArg = PI * (pz[j] - dStop) / dzRight;   dcos_arg = cos(dArg);
			dApodisation = 1.0 - dcos_arg;
			dApodisation *= 0.5;  pData[j] *= dApodisation;
		}
		else
			dApodisation = 1.0;
#ifdef OLOVIA_DEBUG
		FOutApodis << pz[j] << "\t" << dApodisation << "\t" << pData[j] << endl;
#endif 
	}//end for j 
}



void vApodisation(double dStart, double dStop, double dWindowLeft, double dWindowRight, int iNoLines, double *pz, cppc *pData)
//apodisation with half-cosine to avoid Gibbs-ringing after Fourier trafo.  
{
#ifdef OLOVIA_DEBUG
	ofstream FOutApodis("Apodis_of_cppc_val.txt");
#endif 
	double dApodisation = 0.0, dArg, dcos_arg, dzLeft = dWindowLeft - dStart, dzRight = dWindowRight - dStop;
	for (int j = 0; j < iNoLines; j++) {
		if (pz[j] < dStart || pz[j] > dStop) {
			dApodisation = 0.0;  pData[j] = 0.0;
#ifdef OLOVIA_DEBUG
			FOutApodis << pz[j] << "\t\t" << dApodisation << "\t\t" << pData[j].real() << "\t\t" << pData[j].imag() << endl;
#endif 
			continue;
		}//end if 

		if (pz[j] < dWindowLeft) {
			dArg = PI * (pz[j] - dStart) / dzLeft;  dcos_arg = cos(dArg);
			dApodisation = 1.0 - dcos_arg;
			dApodisation *= 0.5;  pData[j] *= dApodisation;
		}
		else if (pz[j] > dWindowRight) {
			dArg = PI * (pz[j] - dStop) / dzRight;   dcos_arg = cos(dArg);
			dApodisation = 1.0 - dcos_arg;
			dApodisation *= 0.5;  pData[j] *= dApodisation;
		}
		else
			dApodisation = 1.0;
#ifdef OLOVIA_DEBUG
		FOutApodis << pz[j] << "\t\t" << dApodisation << "\t\t" << pData[j].real() << "\t\t" << pData[j].imag() << endl;
#endif 
	}//end for all lines j 
}


void vApodisOfIntensity(struct sCalcDeltaIntSq *p, bool bPlanarSubstrate)
{
	double dWindowRight, dStop;
	if (bPlanarSubstrate) {
		dWindowRight = p->dOrigin_z_Glob + p->dApodisLeft + 2.0 * p->dzFreeSpaceBottom;   dStop = dWindowRight + p->dApodisRight;
	}
	else {
		dWindowRight = p->dOrigin_z_Glob + p->dPeriod - p->dApodisRight;   dStop = p->dOrigin_z_Glob + p->dPeriod;
	}

	vApodisation(p->dOrigin_z_Glob, dStop, p->dOrigin_z_Glob + p->dApodisLeft, dWindowRight, p->iNoPoints, p->pzMeasured, p->pIntMeasured);
}


void vApodisOfSpectrum(struct sCalcDeltaIntSq *p)
{
	int i;
	vApodisation(p->dkStart, p->dkStop, p->dkWindowLeft, p->dkWindowRight, p->pdVk->Nown, p->pdVk->Data, p->pVSpectrum->Data);
	p->pVSpectrum->Data[p->pVSpectrum->Nown / 2] = 0.0;
	for (i = 1; i < (int)p->pVSpectrum->Nown / 2; i++)
		p->pVSpectrum->Data[p->pVSpectrum->Nown - i] = conj(p->pVSpectrum->Data[i]); //the spectrum always has this symmetry, since the intensity is real
}




void vFraction_of_FT_of_pattern(struct sCalcDeltaIntSq *p) //"_x50_Xpol_subs_b_cppc_14_2_18.txt" 
//This function computes a fraction of the fourier transform of the fringe pattern. The peaks are at 2*PI/(0.5*wavelength) = +/- 2*PI / (0.5*1.329) 
{
	int i, j, Index;
	double dStart = p->dOrigin_z_Glob, dStop = p->dOrigin_z_Glob + p->dPeriod, dPhaseR, dDzR, dMid = 0;
	cppc cExpL, cExpR, cTmp;
#ifdef OLOVIA_DEBUG 
	ofstream fOutFT("Fraction_of_FourierTrafo_for_+z-direction_apodis.txt");
#endif 

	int  ikStart = (int)(p->dkStart * p->dPeriod / (2.0 * PI) + 0.5), ikStop = (int)(p->dkStop * p->dPeriod / (2.0 * PI) + 0.5);  int iDim_k = ikStop - ikStart + 1;
	dVector Vk(iDim_k); Vector VSpectrum(iDim_k);

	for (i = ikStart; i <= ikStop; i++) {  //-N, ...., +N  or  -N, ..., +N-1; POSSIBLE ACCELERATION: we need the calc. only for few frequencies between k1 and k2 
		Index = i - ikStart;
		VSpectrum.Data[Index] = 0.0;  Vk.Data[Index] = (i * 2.0 * PI) / p->dPeriod;

		if (i != 0) {
			cExpR = 1.0;  //cout << cExpR << endl;
			for (j = 0; j < p->iNoPoints; j++) { //for all z_j with I(z_j) > zero; ACCELERATION: we need the calc. only for few z_j between z_min and z_max 
				if (j == p->iNoPoints - 1)
					dMid = p->pzMeasured[j] + 0.5 * (p->pzMeasured[j] - p->pzMeasured[j - 1]);
				else
					dMid = (p->pzMeasured[j] + p->pzMeasured[j + 1]) / 2.0;
				if (dMid <= dStart)
					continue;
				if (dMid > dStop)
					dMid = dStop;
				cExpL = cExpR; // - exp(-cIm * dPhaseL); // ... * e^(-ikz)
				dDzR = dMid - p->dOrigin_z_Glob; //- dStart;
				dPhaseR = Vk.Data[Index] * dDzR;
				cExpR = exp(-cIm * dPhaseR);   //cout << j << "\t" << dDzR << "\t" << dPhaseR << "\t" << cExpR << endl;
				cTmp = p->pIntMeasured[j] * (cExpR - cExpL);
				VSpectrum.Data[Index] += cTmp;
				if (p->pzMeasured[j] > dStop) {
					j++;  break;
				}
			}//end for all z_j
			if (dMid < dStop)
				VSpectrum.Data[Index] += p->pIntMeasured[j] * (exp(-cIm * Vk.Data[Index] * (dStop - p->dOrigin_z_Glob)) - cExpR); //e^(n2pi) - ...
			VSpectrum.Data[Index] /= (p->dPeriod * (-cIm) * Vk.Data[Index]);
		}
		else {
			cExpR = 0;  //cout << cExpR << endl;  
			for (j = 0; j < p->iNoPoints; j++) { //for all z_j
				if (j == p->iNoPoints - 1)
					dMid = p->pzMeasured[j] + 0.5 * (p->pzMeasured[j] - p->pzMeasured[j - 1]);
				else
					dMid = (p->pzMeasured[j] + p->pzMeasured[j + 1]) / 2.0;
				if (dMid <= dStart)
					continue;
				if (dMid > dStop)
					dMid = dStop;
				cExpL = cExpR; // - exp(-cIm * dPhaseL); // ... * e^(-ikz)
				cExpR = dMid - dStart; //cout << j << "\t" << dDzR << "\t" << dPhaseR << "\t" << cExpR << endl;
				cTmp = p->pIntMeasured[j] * (cExpR - cExpL);
				VSpectrum.Data[Index] += cTmp;
				if (p->pzMeasured[j] > dStop) {
					j++;  break;
				}
			}//end for all z_j
			if (dMid < dStop)
				VSpectrum.Data[Index] += p->pIntMeasured[j] * (p->dPeriod - cExpR); //e^(n2pi)=1
			VSpectrum.Data[Index] /= p->dPeriod;
		}
		//cout << i << "\t" << Index << "\t" << Vk.Data[Index] << "  \t" << VSpectrum.Data[Index] << endl;
	}//end for all k_i 
	double dCorrectionFac = 1.030512;  VSpectrum *= dCorrectionFac;  //dCorrectionFac results from an optimization of modulus of amplitudes; Correct numeric errors done by midpoint rule during calc. of spectrum:

#ifdef OLOVIA_DEBUG
	fOutFT << "#   2 * PI / 1.180 = " << 2 * PI / 1.180 << "   2Pi / 1.312 = " << 2 * PI / 1.312 << "   2 * PI / 1.440 = " << 2 * PI / 1.440
		<< "   2Pi / p = " << 2 * PI / p->dPeriod << endl;
	for (i = 0; i < (int)Vk.Nown; i++)
		fOutFT << ikStart + i << "\t" << Vk.Data[i] << "\t" << VSpectrum.Data[i].real() << "\t" << VSpectrum.Data[i].imag() << "\t" << abs(VSpectrum.Data[i]) << endl;
#endif 
	delete[] p->pdVk;  p->pdVk = new dVector[1];  delete[] p->pVSpectrum;  p->pVSpectrum = new Vector[1];
	p->pdVk->Init(p->iNoFFTPoints);   p->pVSpectrum->Init(p->iNoFFTPoints);
	for (i = ikStart; i <= ikStop; i++) {
		p->pVSpectrum->Data[i] = VSpectrum.Data[i - ikStart];   p->pVSpectrum->Data[p->iNoFFTPoints - i] = conj(VSpectrum.Data[i - ikStart]);
	}
	for (i = 0; i < p->iNoFFTPoints / 2; i++)
		p->pdVk->Data[i] = (i * 2.0 * PI) / p->dPeriod;
	for (i = -p->iNoFFTPoints / 2; i < 0; i++)
		p->pdVk->Data[p->iNoFFTPoints + i] = (i * 2.0 * PI) / p->dPeriod;
	return;
}



void vFraction_of_analyt_FT_of_pattern(double dStart, double dStop, int ikStart, int ikStop, double dPeriod, int iNoPoints, double *pzMeasured,
	double dOrigin_z_Glob, double *pIntMeasured, dVector *pdVk, Vector *pVSpectrum) //"_x50_Xpol_subs_b_cppc_14_2_18.txt" 
//This function computes a fraction of the fourier transform of the fringe pattern. The peaks are at 2*PI/(0.5*wavelength) = +/- 2*PI / (0.5*1.329) 
{
	int i, j, Index;
	double dPhaseR, dDzR, dMid = 0;
	cppc cExpL, cExpR, cTmp;

	int iDim_k = ikStop - ikStart + 1;  dVector Vk(iDim_k); Vector VSpectrum(iDim_k);

	for (i = ikStart; i <= ikStop; i++) {  //-N, ...., +N  or  -N, ..., +N-1; POSSIBLE ACCELERATION: we need the calc. only for few frequencies between k1 and k2 
		Index = i - ikStart;
		VSpectrum.Data[Index] = 0.0;  Vk.Data[Index] = (i * 2.0 * PI) / dPeriod;

		if (i != 0) {
			cExpR = 1.0;  //cout << cExpR << endl;
			for (j = 0; j < iNoPoints - 1; j++) { //for all z_j with I(z_j) > zero; ACCELERATION: we need the calc. only for few z_j between z_min and z_max   
				dMid = (pzMeasured[j] + pzMeasured[j + 1]) / 2.0;
				if (dMid > dStop)
					dMid = dStop;
				cExpL = cExpR; // - exp(-cIm * dPhaseL); // ... * e^(-ikz)
				dDzR = dMid - dOrigin_z_Glob; //- dStart;
				dPhaseR = Vk.Data[Index] * dDzR;
				cExpR = exp(-cIm * dPhaseR);   //cout << j << "\t" << dDzR << "\t" << dPhaseR << "\t" << cExpR << endl;
				cTmp = pIntMeasured[j] * (cExpR - cExpL);
				if (dMid <= dStart)
					continue;
				VSpectrum.Data[Index] += cTmp;
				if (pzMeasured[j] > dStop) {
					j++;  break;
				}
			}//end for all z_j
			if (dMid < dStop)
				VSpectrum.Data[Index] += pIntMeasured[j] * (exp(-cIm * Vk.Data[Index] * (dStop - dOrigin_z_Glob)) - cExpR); //e^(n2pi) - ...
			VSpectrum.Data[Index] /= (dPeriod * (-cIm) * Vk.Data[Index]);
		}
		else {
			cExpR = 0;  //cout << cExpR << endl;  
			for (j = 0; j < iNoPoints - 1; j++) { //for all z_j
				dMid = (pzMeasured[j] + pzMeasured[j + 1]) / 2.0;
				if (dMid > dStop)
					dMid = dStop;
				cExpL = cExpR; // - exp(-cIm * dPhaseL); // ... * e^(-ikz)
				cExpR = dMid - dStart; //cout << j << "\t" << dDzR << "\t" << dPhaseR << "\t" << cExpR << endl;
				cTmp = pIntMeasured[j] * (cExpR - cExpL);
				if (dMid <= dStart)
					continue;
				VSpectrum.Data[Index] += cTmp;
				if (pzMeasured[j] > dStop) {
					j++;  break;
				}
			}//end for all z_j
			if (dMid < dStop)
				VSpectrum.Data[Index] += pIntMeasured[j] * (dPeriod - cExpR); //e^(n2pi)=1
			VSpectrum.Data[Index] /= dPeriod;
		}
		//cout << i << "\t" << Index << "\t" << Vk.Data[Index] << "  \t" << VSpectrum.Data[Index] << endl;
	}//end for all k_i 

#ifdef OLOVIA_DEBUG  
	ofstream fOutFT("vFraction_of_analyt_FT_of_pattern.txt");
	fOutFT << "#   2 * PI / 1.180 = " << 2 * PI / 1.180 << "   2Pi / 1.312 = " << 2 * PI / 1.312 << "   2 * PI / 1.440 = " << 2 * PI / 1.440
		<< "   2Pi / p = " << 2 * PI / dPeriod << endl;
	for (i = 0; i < (int)Vk.Nown; i++)
		fOutFT << ikStart + i << "\t\t" << Vk.Data[i] << "\t\t" << VSpectrum.Data[i].real() << "\t\t" << VSpectrum.Data[i].imag() << "\t\t" << abs(VSpectrum.Data[i]) << endl;
#endif 
	//delete[] pdVk;  pdVk = new dVector[1];  delete[] pVSpectrum;  pVSpectrum = new Vector[1];
	*pdVk = Vk;  *pVSpectrum = VSpectrum;
	/*
	pdVk->Init(iNoFFTPoints);   pVSpectrum->Init(iNoFFTPoints);
	for (i = ikStart; i <= ikStop; i++) {
		pVSpectrum->Data[i] = VSpectrum.Data[i - ikStart];   pVSpectrum->Data[iNoFFTPoints - i] = conj(VSpectrum.Data[i - ikStart]);
	}
	for (i = 0; i < iNoFFTPoints / 2; i++)
		pdVk->Data[i] = (i * 2.0 * PI) / dPeriod;
	for (i = -iNoFFTPoints / 2; i < 0; i++)
		pdVk->Data[iNoFFTPoints + i] = (i * 2.0 * PI) / dPeriod;
	return;
	*/
}


void vInv_analyt_FT(dVector &dVk, Vector &VSpectrum, dVector &dVx, double dOrigin, double *pdFuVal)
//This function does an analytical inverse Fourier transform.  W. Iff, 16.1.2019 
{
#ifdef OLOVIA_DEBUG 
	ofstream ofOutSpec("vInv_analyt_FT_Spec.txt"), ofOutFuVal("vInv_analyt_FT_FuVal.txt");
#endif 
	int i, j;  cppc cTmp;  Vector VFuVal(dVx.Nown); VFuVal = 0.0;

	for (i = 0; i < (int)dVk.Nown; i++) { //for all frequencies: 
#ifdef OLOVIA_DEBUG 
		ofOutSpec << i << "\t\t" << VSpectrum.Data[i].real() << "\t\t" << VSpectrum.Data[i].imag() << endl;
#endif 
		for (j = 0; j < (int)dVx.Nown; j++) { //for all locations x
			cTmp = VSpectrum.Data[i] * exp(cIm * dVk.Data[i] * (dVx.Data[j] - dOrigin));
			VFuVal.Data[j] += cTmp.real();
		}
	}//end for all frequencies i 
	for (j = 0; j < (int)dVx.Nown; j++) { //for all locations x
		pdFuVal[j] = VFuVal.Data[j].real();
		if (abs(VFuVal.Data[j].imag()) > 0.001 * abs(VFuVal.Data[j].real()))
			cout << endl << "vInv_analyt_FT: im part: " << j << "  " << VFuVal.Data[j] << endl;
#ifdef OLOVIA_DEBUG 
		ofOutFuVal << j << "\t\t" << dVx.Data[j] << "\t\t" << pdFuVal[j] << endl;
#endif 
	}
}

void vGetDispersion(struct sCalcDeltaIntSq *psConjPhase)
{
	int i;
	psConjPhase->pVConjPhase = new Vector[1];  psConjPhase->pVConjPhase->Init(psConjPhase->pVSpectrum->Nown);
	for (i = 0; i < (int)psConjPhase->pVSpectrum->Nown; i++) {
		double dAbs = abs(psConjPhase->pVSpectrum->Data[i]);
		if (dAbs > 10.0 * DBL_MIN) {
			psConjPhase->pVConjPhase->Data[i] = psConjPhase->pVSpectrum->Data[i] / abs(psConjPhase->pVSpectrum->Data[i]);
			psConjPhase->pVConjPhase->Data[i] = conj(psConjPhase->pVConjPhase->Data[i]);
		}
		else
			psConjPhase->pVConjPhase->Data[i] = 0.0;
		psConjPhase->pVConjPhase->Data[i] *= exp(-cIm * psConjPhase->pdVk->Data[i] * psConjPhase->dzTop);
	}//end for
}


void vCancelDispersion(struct sCalcDeltaIntSq *psSpectrum, struct sCalcDeltaIntSq *psConjPhase)
{
	for (int i = 0; i < (int)psConjPhase->pVSpectrum->Nown; i++) {
		psSpectrum->pVSpectrum->Data[i] *= psConjPhase->pVConjPhase->Data[i];
	}//end for
}

//########################################################### Functions for Levenberg - Marquardt algorithm ###########################################################

void vCalcDeltaIntAndDeriv2Surf(double *pIntMeasured, dVector *dVk, Vector VSpectrum, Vector& VParam,
	Vector *pVDeltaInt, Matrix *pMDeltaIntDeriv, bool bOptParam[], bool *pSuc)
	//This function is for exactly 2 reflecting surfaces only!  
{
	*pSuc = false;
	if (pVDeltaInt == NULL || pIntMeasured == NULL) //pMDeltaIntDeriv may be NULL, the derivative is not always needed in practice anyway, but function value always
		return;

	int i, iDim = VSpectrum.Nown;
	double dDepth = VParam.Data[Z_BOT_LISE].real() - VParam.Data[Z_TOP_LISE].real();
	Vector VSpectrumTmp(iDim), VDeltaIntTopAmplDeriv(iDim), VDeltaIntTop_z_Deriv(iDim), VDeltaIntBotAmplDeriv(iDim), VDeltaIntBot_z_Deriv(iDim);

	//1.) model the peak at the top surface: 
	for (i = 0; i < iDim; i++) //propagation:  
		VSpectrumTmp.Data[i] = VSpectrum.Data[i] * exp(-cIm * dVk->Data[i] * VParam.Data[Z_TOP_LISE].real());
	fFFT_cppc(TRUE, iDim, VSpectrumTmp.Data, NULL, VDeltaIntTopAmplDeriv.Data, NULL, FALSE);
	if (bOptParam[Z_TOP_LISE]) {
		for (i = 0; i < iDim; i++)
			VSpectrumTmp.Data[i] = VParam.Data[AMPL_TOP_LISE] * VSpectrum.Data[i] *
			exp(-cIm * dVk->Data[i] * VParam.Data[Z_TOP_LISE].real()) * (-cIm * dVk->Data[i]);
		fFFT_cppc(TRUE, iDim, VSpectrumTmp.Data, NULL, VDeltaIntTop_z_Deriv.Data, NULL, FALSE);
	}
	//2.) model the peak at the bottom surface: 
	for (i = 0; i < iDim; i++) //propagation:  
		VSpectrumTmp.Data[i] = VSpectrum.Data[i] * exp(-cIm * dVk->Data[i] * VParam.Data[Z_TOP_LISE].real() - cIm * dVk->Data[i] * dDepth);
	fFFT_cppc(TRUE, iDim, VSpectrumTmp.Data, NULL, VDeltaIntBotAmplDeriv.Data, NULL, FALSE);
	if (bOptParam[Z_BOT_LISE]) {
		for (i = 0; i < iDim; i++)
			VSpectrumTmp.Data[i] = VParam.Data[AMPL_BOT_LISE] * VSpectrum.Data[i] *
			exp(-cIm * dVk->Data[i] * VParam.Data[Z_TOP_LISE].real() - cIm * dVk->Data[i] * dDepth) * (-cIm * dVk->Data[i]);
		fFFT_cppc(TRUE, iDim, VSpectrumTmp.Data, NULL, VDeltaIntBot_z_Deriv.Data, NULL, FALSE);
	}
	//3. each optimization param belongs to one line of MDeltaIntDeriv:  
	if (pMDeltaIntDeriv)
		for (i = 0; i < iDim; i++) {
			if (bOptParam[Z_TOP_LISE])
				pMDeltaIntDeriv->Data[pMDeltaIntDeriv->Next * i + Z_TOP_LISE] = VDeltaIntTop_z_Deriv.Data[i]; //column 0 <=> param. 0 (z at top) 
			if (bOptParam[Z_BOT_LISE])
				pMDeltaIntDeriv->Data[pMDeltaIntDeriv->Next * i + Z_BOT_LISE] = VDeltaIntBot_z_Deriv.Data[i]; //col 1 <=> param. 1 (z at bottom)  
			if (bOptParam[AMPL_TOP_LISE])
				pMDeltaIntDeriv->Data[pMDeltaIntDeriv->Next * i + AMPL_TOP_LISE] = VDeltaIntTopAmplDeriv.Data[i]; //col 2 <=> param. 2 (ampl at top) 
			if (bOptParam[AMPL_BOT_LISE])
				pMDeltaIntDeriv->Data[pMDeltaIntDeriv->Next * i + AMPL_BOT_LISE] = VDeltaIntBotAmplDeriv.Data[i]; //col 3 <=> param. 3 (ampl at bot)   
		}
	//4. calc function value F: 
	for (i = 0; i < iDim; i++)
		pVDeltaInt->Data[i] = VParam.Data[AMPL_TOP_LISE] * VDeltaIntTopAmplDeriv.Data[i] + VParam.Data[AMPL_BOT_LISE] * VDeltaIntBotAmplDeriv.Data[i] - pIntMeasured[i];
	//5. output: 
#ifdef OLOVIA_DEBUG 
	ofstream FOutInt("Calc_Intensity_Deriv_by_FFT.txt");
	for (i = 0; i < iDim; i++) { //for all pixels i
		FOutInt << i << "\t\t" << pVDeltaInt->Data[i].real() << "\t\t" << pVDeltaInt->Data[i].imag();
		if (pMDeltaIntDeriv) {
			FOutInt << "\t\t" << pMDeltaIntDeriv->Data[4 * i].real() << "\t\t" << pMDeltaIntDeriv->Data[4 * i].imag();
			FOutInt << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 1].real() << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 1].imag();
			FOutInt << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 2].real() << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 2].imag();
			FOutInt << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 3].real() << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 3].imag();
		}
		FOutInt << endl;
	} //end for i 
#endif 
	*pSuc = true;
}

void vCalcIntFuVal(double dzScale, double dzTop, double dDepth, double *pDepthUnused, double dDiameter, cppc cAmplTop, cppc cAmplBot,
	dVector &dVkWindow, cppc *cSpectrum, bool bMoveOnlyEnvelope, Vector &VTop, Vector&VBot, Vector *pVTotal)
	//"void CalculateIntensityFunctionValue". This function is for exactly 2 reflecting surfaces only!  VERY BASIC FUNCTION! ONLY SUITED FOR SIMPLE TSV DEPTH ESTIMATION!
{
	int i;
	double  dzTop_in_µm = dzScale * dzTop, dDepth_in_µm = dzScale * dDepth;
	cppc cDelta_k_Air, cDelta_k_Hole, cTmp;

	//0.) calc effective index inside hole:
	Vector VnEff(dVkWindow.Nown);   vCalcEffInd(dDiameter, dVkWindow, VnEff, sEffInd.iTypeOfHole);
	cppc cExpAir, cExpHole, cnEff_at_k_Mid;  vCalcEffInd(dDiameter, sEffInd.dkMidInAir, cnEff_at_k_Mid, sEffInd.iTypeOfHole);
	sEffInd.dkMid_x_nEff = cnEff_at_k_Mid.real() * sEffInd.dkMidInAir;

	//1. model the peak at the top and bottom surface - preparations: 
	for (i = 0; i < dVkWindow.Nown; i++) { //propagation:  
		cDelta_k_Air = dVkWindow.Data[i];   cDelta_k_Hole = dVkWindow.Data[i] * VnEff.Data[i];
		cDelta_k_Air -= sEffInd.dkMidInAir;
		if (bMoveOnlyEnvelope)
			cDelta_k_Hole -= sEffInd.dkMid_x_nEff;
		cExpAir = exp(-cIm * cDelta_k_Air * dzTop_in_µm);
		cExpHole = exp(-cIm * cDelta_k_Hole * dDepth_in_µm);

		//dLambda = 2.0 * PI / dVkWindow.Data[i];  bInterpol_FP(dLambda, dDiameter, dDiameter, dDepth_in_µm, cEh, cth, cEb, crb); 
		cTmp = cSpectrum[i] * cExpAir;
		VTop.Data[i] = cTmp; // * cEh;
		VBot.Data[i] = cTmp * cExpHole; // * cth * cEb * crb 

	}

	//2. calc function value F: 
	for (i = 0; i < dVkWindow.Nown; i++) {
		VTop.Data[i] *= cAmplTop;  VBot.Data[i] *= cAmplBot;
		if (pVTotal)
			pVTotal->Data[i] = VTop.Data[i] + VBot.Data[i];
	}

	//ratio a_bot / a_top for the publication: 
	if (pDepthUnused) {
		double dTmp_µm = *pDepthUnused * dzScale;
		cppc cPropFactor = exp(-cIm * cnEff_at_k_Mid * sEffInd.dkMidInAir * dTmp_µm);
		double  da_t = abs(cAmplTop), da_b = abs(cAmplBot) * abs(cPropFactor);   double dA_bot_div_A_top = da_b / da_t;
		cout << endl << "dA_bot_div_A_top = " << dA_bot_div_A_top << endl;
	}
	return;
}



double dCalcBotDepthFromAR(double dD_Bot, double dD_Top, double dTSV_Height) //all quantities in µm 
{
	static double dCoeff1 = 0.116514, dCoeff2 = 0.00232279;  //as in paper 
	double dD = 0.5 * (dD_Bot + dD_Top);
	double dAR = dTSV_Height / dD;
	double dDenom = 1.0 + dCoeff1 * dAR + dCoeff2 * dAR * dAR;
	double dExcentricity = 1.0 - 1.0 / dDenom;
	return dExcentricity * (0.5 * dD_Bot);
}


bool bCalcIntConeAmplOpt(sInfoConicalCircTSV *pTSV, struct sCalcDeltaIntSq *pSpec, Vector *pVResultTop, Vector *pVResultBot)
//Diffraction calculation at TSVs. This function is for 2 reflecting surfaces only. 
{
	static dVector *pdVkWindow;   pdVkWindow = pSpec->pdVkWindow;
	static cppc *pcSpectrum;  pcSpectrum = pSpec->pVSpectrum->Data + pSpec->ikStart;
	static int i, j;
	static cppc *pcResultTop, *pcResultBot;  pcResultTop = pVResultTop->Data, pcResultBot = pVResultBot->Data;
	static double dStepDown, dD1, dD2, dD3, dDeltaDiam, dj1, dj2, dj3;
	dStepDown = pTSV->dDepth / (double)sFP.iNoStairs, dDeltaDiam = (pTSV->dDiameterTop - pTSV->dDiameterBot) / (double)sFP.iNoStairs;
	static cppc cExpAir, cSpectcExpAir, cExp;
	static Vector Vaverage_n(pdVkWindow->Nown); Vaverage_n = 0; //initialization
	static double dz1 = 0.5 - (sqrt(15.0) / 10.0), dz2 = 0.5, dz3 = 0.5 + (sqrt(15.0) / 10.0), dWeight1 = 5.0 / 18.0, dWeight2 = 4.0 / 9.0, dWeight3 = 5.0 / 18.0;
	static Vector VnEff1(pdVkWindow->Nown), VnEff2(pdVkWindow->Nown), VnEff3(pdVkWindow->Nown),
		VEh(pdVkWindow->Nown), Vth(pdVkWindow->Nown), VEb(pdVkWindow->Nown), Vrb(pdVkWindow->Nown);
	static double dDepth_bot_µm;  dDepth_bot_µm = dCalcBotDepthFromAR(pTSV->dDiameterBot, pTSV->dDiameterTop, pTSV->dDepth);

	Vector VEh2(VEh.Nown), Vth2(Vth.Nown), VEb2(VEb.Nown), Vrb2(Vrb.Nown);
	//if (bInterpol_FP(pdVkWindow, pTSV->dDiameterBot, pTSV->dDiameterTop, dDepth_bot_µm, VEh2.Data, Vth2.Data, VEb2.Data, Vrb2.Data) == false)
	//	goto END; 
	if (bInterpol_FP_with_scaling(pdVkWindow, pTSV->dDiameterBot, pTSV->dDiameterTop, dDepth_bot_µm, VEh.Data, Vth.Data, VEb.Data, Vrb.Data) == false)
		goto END;
	for (i = 0; i < (int)pdVkWindow->Nown; i++) { //for all wavelengths:  
		cExpAir = exp(-cIm * pdVkWindow->Data[i] * pTSV->dzTop);   cSpectcExpAir = pcSpectrum[i] * cExpAir;
		pcResultTop[i] = cSpectcExpAir * VEh.Data[i];              //cSpectcExpAirEh; 
		pcResultBot[i] = cSpectcExpAir * Vth.Data[i] * VEb.Data[i] * Vrb.Data[i];  //cSpectcExpAir_thEbrb; 
	}//i

	for (j = 0; j < sFP.iNoStairs; j++) { //for all integration steps: calc effective index inside hole and propagate: 
		dj3 = j + dz3;   dD3 = pTSV->dDiameterBot + dj3 * dDeltaDiam;   vCalcEffInd(dD3, *pdVkWindow, VnEff3, sEffInd.iTypeOfHole); //top 
		dj2 = j + dz2;   dD2 = pTSV->dDiameterBot + dj2 * dDeltaDiam;   vCalcEffInd(dD2, *pdVkWindow, VnEff2, sEffInd.iTypeOfHole); //middle
		dj1 = j + dz1;   dD1 = pTSV->dDiameterBot + dj1 * dDeltaDiam;   vCalcEffInd(dD1, *pdVkWindow, VnEff1, sEffInd.iTypeOfHole); //bottom
		for (i = 0; i < (int)pdVkWindow->Nown; i++) //for all wavelengths:  
			Vaverage_n.Data[i] += VnEff1.Data[i] * dWeight1 + VnEff2.Data[i] * dWeight2 + VnEff3.Data[i] * dWeight3;
	}//j 
	for (i = 0; i < (int)pdVkWindow->Nown; i++) { //for all wavelengths:  
		cExp = exp(-cIm * pdVkWindow->Data[i] * Vaverage_n.Data[i] * dStepDown);
		pcResultBot[i] *= cExp;  //down- and upwards 
		if (i == 78)
			cout << endl << "cExp[78] = " << abs(cExp) << endl;
	}//i 

	return true;
END: cout << endl << "Failure of the electromagnetic calculation!"; return false;
}




bool bLevenbergMarquardtAmplOpt(Vector &Vx0, double dMu, double dTol, int iMaxIter, int iDimVF,
	bool(*bCalc_F)(sInfoConicalCircTSV *pTSV, struct sCalcDeltaIntSq *pSpec, Vector *pVResultTop, Vector *pVResultBot),
	sInfoConicalCircTSV *pTSV, struct sCalcDeltaIntSq *pInt, struct sCalcDeltaIntSq *pSpec, Vector& Vx, double& dNormVF)
	//LevenbergMarquardt algorithm as in Dahmen Reusken, "Numerik für Ingenieure und Naturwissenschaftler" on p. 222 - 224. 
{
	static int i, j, iIndex;
	static double dNormVF_Previous, dNorm_x, dMuSq, dVTmpNorm;
	static double dNormOfMdFdxSq, dMinAllowedMuSq, dMinAllowedMu, dAbs0, dAbs1;
	static Vector VFTop(iDimVF), VFBot(iDimVF), VTmp(Vx0.Nown), Vs(Vx0.Nown);
	static Matrix M_dFdx_adj_x_dFdx(Vx0.Nown, Vx0.Nown), MInv(Vx0.Nown, Vx0.Nown);
	static cppc ctt, ctb, cbt, cbb, ct0, c0t, cb0, c0b, c00;
	static cppc cat, cab, ccat, ccab, cOffDiag, cDiag, cPrefac;
	static double *pdTopRe, *pdTopIm, *pdBotRe, *pdBotIm, *pdI0Re, *pdI0Im;
	static double dTopRe, dTopIm, dBotRe, dBotIm, dI0Re, dI0Im;
	static double dttRe, dbbRe, d00Re, dtbRe, dtbIm, dt0Re, dt0Im, db0Re, db0Im;
	//ofstream fOutDLS("./results_1DTM/DLS_Convergence.txt");   fOutDLS << setprecision(12);   cout << setprecision(12);

	//Precalc of products Vector * Vector:
	if (bCalc_F(pTSV, pSpec, &VFTop, &VFBot) == false)
		return false;
	pdTopRe = reinterpret_cast<double*>(VFTop.Data);				pdTopIm = reinterpret_cast<double*>(VFTop.Data);				pdTopIm++;
	pdBotRe = reinterpret_cast<double*>(VFBot.Data);				pdBotIm = reinterpret_cast<double*>(VFBot.Data);				pdBotIm++;
	pdI0Re = reinterpret_cast<double*>(pInt->pVSpecWindow->Data);	pdI0Im = reinterpret_cast<double*>(pInt->pVSpecWindow->Data);	pdI0Im++;
	dttRe = 0, dbbRe = 0, d00Re = 0, dtbRe = 0, dtbIm = 0, dt0Re = 0, dt0Im = 0, db0Re = 0, db0Im = 0; //initialization 
	for (i = 0; i < (int)iDimVF; i++) {
		iIndex = 2 * i;
		dTopRe = pdTopRe[iIndex], dTopIm = pdTopIm[iIndex], dBotRe = pdBotRe[iIndex], dBotIm = pdBotIm[iIndex], dI0Re = pdI0Re[iIndex], dI0Im = pdI0Im[iIndex];
		dttRe += dTopRe * dTopRe + dTopIm * dTopIm;
		dbbRe += dBotRe * dBotRe + dBotIm * dBotIm;
		d00Re += dI0Re * dI0Re + dI0Im * dI0Im;
		dtbRe += dTopRe * dBotRe + dTopIm * dBotIm;   dtbIm += dTopRe * dBotIm - dTopIm * dBotRe;
		dt0Re += dTopRe * dI0Re + dTopIm * dI0Im;    dt0Im += dTopRe * dI0Im - dTopIm * dI0Re;
		db0Re += dBotRe * dI0Re + dBotIm * dI0Im;    db0Im += dBotRe * dI0Im - dBotIm * dI0Re;
	}
	ctt.real(dttRe), ctt.imag(0), cbb.real(dbbRe), cbb.imag(0), c00.real(d00Re), c00.imag(0);
	ctb.real(dtbRe), ctb.imag(dtbIm); 	cbt.real(dtbRe), cbt.imag(-dtbIm);
	ct0.real(dt0Re), ct0.imag(dt0Im); 	c0t.real(dt0Re), c0t.imag(-dt0Im);
	cb0.real(db0Re), cb0.imag(db0Im);  	c0b.real(db0Re), c0b.imag(-db0Im);

	//Precalc. Matrix:
	M_dFdx_adj_x_dFdx.Data[0] = ctt;	M_dFdx_adj_x_dFdx.Data[1] = ctb;  //(F´+) * (F´) add compl conj?
	M_dFdx_adj_x_dFdx.Data[2] = cbt;	M_dFdx_adj_x_dFdx.Data[3] = cbb;  //             add compl conj? 
	//Add damping:  
	dNormOfMdFdxSq = M_dFdx_adj_x_dFdx.norm();
	dMinAllowedMuSq = Vx0.Nown * dNormOfMdFdxSq * 0.01 * NUM_NOISE_FLOAT;  dMinAllowedMu = sqrt(dMinAllowedMuSq);
	if (dMu < dMinAllowedMu)
		dMu = dMinAllowedMu; //diag elements dMuSq must have certain size so that Matrix is always invertible.    
	dMuSq = dMu * dMu;
	for (j = 0; j < Vx0.Nown; j++)
		M_dFdx_adj_x_dFdx.Data[j * Vx0.Nown + j] += dMuSq;
	//Precalc Inverse: MInverse.vSetToUnityMat();  MInverse /= M_dFdx_adj_x_dFdx; : 
	cPrefac = 1.0 / (M_dFdx_adj_x_dFdx.Data[0] * M_dFdx_adj_x_dFdx.Data[3] - M_dFdx_adj_x_dFdx.Data[1] * M_dFdx_adj_x_dFdx.Data[2]);
	MInv.Data[0] = +cPrefac * M_dFdx_adj_x_dFdx.Data[3];	MInv.Data[1] = -cPrefac * M_dFdx_adj_x_dFdx.Data[1];
	MInv.Data[2] = -cPrefac * M_dFdx_adj_x_dFdx.Data[2];	MInv.Data[3] = +cPrefac * M_dFdx_adj_x_dFdx.Data[0];

	//cout << endl << "Start of Levenberg-Marquardt algo" << endl;
	Vx = Vx0;   dNormVF = DBL_MAX; //initializations 
	for (i = 0; i < iMaxIter; i++) {
		//cout << "Iteration no.: " << i << "\t ";  
		//1.) Calc. F, F': vCalc_F(..., Vx, ..., &VF, &MdFdx):  |F|² and (F´+) * (F´) : 
		dNormVF_Previous = dNormVF;  dNorm_x = Vx.norm();
		cat = Vx.Data[C_AMPL_TOP_LISE], cab = Vx.Data[C_AMPL_BOT_LISE], ccat = conj(Vx.Data[C_AMPL_TOP_LISE]), ccab = conj(Vx.Data[C_AMPL_BOT_LISE]);
		cOffDiag = ccab * cat*cbt - cat * c0t - cab * c0b, cDiag = ccat * cat*ctt + ccab * cab*cbb + c00;
		dNormVF = sqrt(cDiag.real() + 2.0 * cOffDiag.real()); // |F|²
		//VTmp = MdFdx_adj * VF;
		VTmp.Data[0] = cat * ctt + cab * ctb - ct0;
		VTmp.Data[1] = cat * cbt + cab * cbb - cb0;
		dVTmpNorm = VTmp.norm(); //needed later as termination criterion 
		//Vs = - MInv * VTmp;  VxNext = Vx + Vs;  recalculation (update)
		Vs.Data[0] = -MInv.Data[0] * VTmp.Data[0] - MInv.Data[1] * VTmp.Data[1];
		Vs.Data[1] = -MInv.Data[2] * VTmp.Data[0] - MInv.Data[3] * VTmp.Data[1];
		Vx.Data[0] = Vx.Data[0] + Vs.Data[0];
		Vx.Data[1] = Vx.Data[1] + Vs.Data[1];
		//5.) Termination criteria:  
		if ((Vs.norm() <= MIN_STEP_DLS * dNorm_x && i > 0) || (dVTmpNorm < dTol && 1.0 - dNormVF / dNormVF_Previous < 0.0001 * CHANGE_OF_DLS_RESIDUAL))
			break;
		//cout << " dDerivNorm = " << dDerivNorm << endl;   fOutDLS << i << "\t" << pNormVF << "\t\t" << dDerivNorm << endl;  
	}//end for i  
	//cout << endl << "Levenberg-Marquardt algo: Summary:  " << i << " iterations;" << "   NormVF = " << pNormVF << "   dDerivNorm = " << dDerivNorm << "\t" << endl;
	//fOutDLS << "# \t" << i << "\t" << dNormVF << "\t" << dDerivNorm << "\t" << iNoCalc_Vx; 

	dAbs0 = abs(Vx.Data[0]), dAbs1 = abs(Vx.Data[1]);
	if (dAbs0 > FLT_MIN && dAbs1 > FLT_MIN) {
		Vx.Data[0] /= dAbs0;  Vx.Data[1] /= dAbs1; //normalization
	}
	else {
		cout << endl << "Failure of damped least squares algorithm, too weak or inappropriate signal at the TSV top or bottom!" << endl;	return false;
	}
	//calc |F(x)|: 
	cat = Vx.Data[C_AMPL_TOP_LISE], cab = Vx.Data[C_AMPL_BOT_LISE], ccat = conj(Vx.Data[C_AMPL_TOP_LISE]), ccab = conj(Vx.Data[C_AMPL_BOT_LISE]);
	cOffDiag = ccab * cat*cbt - cat * c0t - cab * c0b, cDiag = ccat * cat*ctt + ccab * cab*cbb + c00;
	dNormVF = sqrt((cDiag.real() + 2.0 * cOffDiag.real()) * 2.0); // ... * 2.0 because at this point, we need to remember that there are not only the positive, but also the negative frequencies.

#ifdef OLOVIA_DEBUG 
	static ofstream ofDLS_Conv("ofDLS_Convergence.txt"); static int iCount = 0;
	ofDLS_Conv << iCount << "\t\t" << i << "\t\t" << dNormVF << "\t\t" << dAbs0 << "\t\t" << dAbs1 << endl;
	iCount++;
#endif 

	if (i < iMaxIter && dVTmpNorm < 100.0 * dTol) //if we are not at a local maximum, where the derivative is 0 - as in case of a local minimum: 
		return true;
	else { //no convergence
		cout << endl << "Failure of inner least squares algorithm, no convergence!" << endl;	return false;
	}
}

void vWeightInDirSpace(double dOrigin_z, double dDelta_z, int iNoFFTPoints, dVector &dVkWindow, int iIndexStart, cppc *pcSignalWindow, int iIncrementInp,
	dVector &dVWeightsDirSpace, cppc *pcResultFourier, int iIncrementOutp)
	//Prepare inv FFT into direct space and do inverse FFT => smoothened pattern on equidist. mesh in dir.space: 
{
#ifdef OLOVIA_DEBUG 
	ofstream ofFFT0("FFT0.txt"), ofFFT1("FFT1.txt"), ofFFT2("FFT2.txt"), ofFFT3("FFT3.txt");
#else
	dOrigin_z = 0;  dDelta_z = 0;
#endif 
	int i; 	Vector VSignalFourier(iNoFFTPoints);  dVector dV_SignalDirSpace(iNoFFTPoints);
	for (i = 0; i < (int)dVkWindow.Nown; i++) {
		VSignalFourier.Data[iIndexStart + i] = pcSignalWindow[i * iIncrementInp];
		if (iIndexStart + i > 0)
			VSignalFourier.Data[iNoFFTPoints - (iIndexStart + i)] = conj(pcSignalWindow[i * iIncrementInp]);
	}
#ifdef OLOVIA_DEBUG 
	for (i = 0; i < iNoFFTPoints; i++)
		ofFFT0 << i << "\t\t" << VSignalFourier.Data[i].real() << "\t\t" << VSignalFourier.Data[i].imag() << endl;
#endif 
	fFFT_cppc(TRUE, iNoFFTPoints, VSignalFourier.Data, NULL, NULL, dV_SignalDirSpace.Data, FALSE);
#ifdef OLOVIA_DEBUG 
	for (i = 0; i < iNoFFTPoints; i++)
		ofFFT1 << i << "\t\t" << dOrigin_z + i * dDelta_z << "\t\t" << dV_SignalDirSpace.Data[i] << endl;
#endif 

	//Calc. weighted differences (merit function) in direct space:  
	dVector dV_ResultDirSpace(iNoFFTPoints);
	for (i = 0; i < iNoFFTPoints; i++)  //Debug Info  
		dV_ResultDirSpace.Data[i] = dVWeightsDirSpace.Data[i] * dV_SignalDirSpace.Data[i]; //s.pzMeasured[i] << "\t\t" << dV_I_x_Reconstructed.Data[i];

	//Transform back to Fourier space: 
	Vector V_ResultTmp(iNoFFTPoints);
#ifdef OLOVIA_DEBUG 
	for (i = 0; i < iNoFFTPoints; i++)
		ofFFT2 << i << "\t\t" << dOrigin_z + i * dDelta_z << "\t\t" << dV_ResultDirSpace.Data[i] << endl;
#endif 
	fFFT_cppc(FALSE, iNoFFTPoints, NULL, dV_ResultDirSpace.Data, V_ResultTmp.Data, NULL, FALSE);
#ifdef OLOVIA_DEBUG 
	for (i = 0; i < iNoFFTPoints; i++)
		ofFFT3 << i << "\t\t" << V_ResultTmp.Data[i].real() << "\t\t" << V_ResultTmp.Data[i].imag() << endl;
#endif 
	for (i = 0; i < (int)dVkWindow.Nown; i++)
		pcResultFourier[i * iIncrementOutp] = V_ResultTmp.Data[iIndexStart + i];
}



void vCalcDeltaIntAndDeriv2Surf_No_k0_DirSpace(struct sCalcDeltaIntSq *pPlanar, struct sCalcDeltaIntSq *pTSV,
	Vector& VParam, Vector *pVDeltaInt, Matrix *pMDeltaIntDeriv, bool bOptParam[], bool bMoveOnlyEnvelope, bool *pSuc)
	//This function is for exactly 2 reflecting surfaces only!  
{
	*pSuc = false;
	if (pTSV->pVSpectrum->Data == NULL) //pMDeltaIntDeriv may be NULL, the derivative is not always needed in practice anyway, but function value always
		return;
	dVector *pdVkWindow = pPlanar->pdVkWindow;   int iNoFFTPoints = pTSV->iNoFFTPoints;   double dDiameter = pTSV->dDiameter;
	static dVector dVWeightsDirSpace(iNoFFTPoints);
	if (dVWeightsDirSpace.Nown != iNoFFTPoints) {
		delete[] dVWeightsDirSpace.Data;  dVWeightsDirSpace.Data = new double[iNoFFTPoints];  dVWeightsDirSpace.Nown = iNoFFTPoints;
	}

	int i;
	double dzTop = VParam.Data[Z_TOP_LISE].real(), dDepth = VParam.Data[DEPTH_LISE].real();
	cppc cTddTop = 1, cTuuTop = 1, cRudTop = 1, cRduTop = 1, cRudBot = 0;
	cppc  cAmplTop(VParam.Data[AMPL_RE_TOP_LISE].real(), VParam.Data[AMPL_IM_TOP_LISE].real()), cAmplBot(VParam.Data[AMPL_RE_BOT_LISE].real(), VParam.Data[AMPL_IM_BOT_LISE].real());
	cppc *pcSpectrum = pPlanar->pVSpectrum->Data + pPlanar->ikStart;   cppc cDelta_k_Air, cDelta_k_Hole, cNumerator, cDenom, cFabryPerotFactor;
	Vector VTop(pVDeltaInt->Nown), VBot(pVDeltaInt->Nown);

	//0.) calc effective index inside hole:
	Vector VnEff(pdVkWindow->Nown);   vCalcEffInd(dDiameter, *pdVkWindow, VnEff, sEffInd.iTypeOfHole);
	cppc cnEff_at_k_Mid;            vCalcEffInd(dDiameter, sEffInd.dkMidInAir, cnEff_at_k_Mid, sEffInd.iTypeOfHole);
	sEffInd.dkMid_x_nEff = cnEff_at_k_Mid.real() * sEffInd.dkMidInAir;

	//1. Weight the squares: 
	if (pMDeltaIntDeriv) { //if new iteration:  

		//Calc. weights in Fourier space: 
		vCalcIntFuVal(Z_SCALE, dzTop, dDepth, NULL, dDiameter, 1, 1, *pdVkWindow, pPlanar->pVSpectrumSmoothened->Data, false, VTop, VBot, NULL);
		VTop *= cAmplTop;  VBot *= cAmplBot;

		Vector VWeightsTotFourierTop(iNoFFTPoints), VWeightsTotFourierBot(iNoFFTPoints);
		for (i = 0; i < (int)pdVkWindow->Nown; i++) {
			VWeightsTotFourierTop.Data[pTSV->ikStart + i] = VTop.Data[i];
			VWeightsTotFourierBot.Data[pTSV->ikStart + i] = VBot.Data[i];
		}

		//Transform weights into direct space: 
		Vector VWeightsDirSpaceTop(iNoFFTPoints), VWeightsDirSpaceBot(iNoFFTPoints);
		fFFT_cppc(TRUE, iNoFFTPoints, VWeightsTotFourierTop.Data, NULL, VWeightsDirSpaceTop.Data, NULL, FALSE);
		fFFT_cppc(TRUE, iNoFFTPoints, VWeightsTotFourierBot.Data, NULL, VWeightsDirSpaceBot.Data, NULL, FALSE);

		//Summation in direct space: 
		for (i = 0; i < iNoFFTPoints; i++)
			dVWeightsDirSpace.Data[i] = abs(VWeightsDirSpaceTop.Data[i]) + abs(VWeightsDirSpaceBot.Data[i]); //abs, since we need only the envelope, no oscillations!
		//dVWeightsDirSpace *= 2.0; //since the values on the neg. half axis had been omitted in Fourier space. - obsolete, we do a normalization subsequently! 
		dVector dVWeightsDebug = dVWeightsDirSpace;

		//Normalization of weights: 
		double dSum = 0.0;
		for (i = 0; i < iNoFFTPoints; i++)
			dSum += dVWeightsDirSpace.Data[i];
		if (dSum <= FLT_MIN || dSum > FLT_MAX)
			dVWeightsDirSpace = 1.0;
		else
			dVWeightsDirSpace *= iNoFFTPoints / dSum;
#ifdef OLOVIA_DEBUG 
		ofstream ofWeightDirSpace("ofWeightDirSpace.txt");
		for (i = 0; i < iNoFFTPoints; i++)
			ofWeightDirSpace << i << "\t\t" << pPlanar->dOrigin_z_Glob + i * pPlanar->dDelta_z << "\t\t" <<
			abs(VWeightsDirSpaceTop.Data[i]) << "\t\t" << abs(VWeightsDirSpaceBot.Data[i]) << "\t\t" << dVWeightsDebug.Data[i] << "\t\t" << dVWeightsDirSpace.Data[i] << endl;
#endif 
	}//end if new iteration
	//dVWeightsDirSpace = 1.0; 

	//Calc function value F and weight F:  
	vCalcIntFuVal(Z_SCALE, dzTop, dDepth, NULL, dDiameter, 1, 1, *pdVkWindow, pcSpectrum, bMoveOnlyEnvelope, VTop, VBot, NULL);
	for (i = 0; i < (int)pVDeltaInt->Nown; i++)
		pVDeltaInt->Data[i] = VTop.Data[i] * cAmplTop + VBot.Data[i] * cAmplBot - pTSV->pVSpectrum->Data[pTSV->ikStart + i];
	//Vector V0 = *pVDeltaInt; 
	vWeightInDirSpace(pPlanar->dOrigin_z_Glob, pPlanar->dDelta_z, iNoFFTPoints, *pdVkWindow, pTSV->ikStart, pVDeltaInt->Data, 1, dVWeightsDirSpace, pVDeltaInt->Data, 1);
	//Vector V1 = *pVDeltaInt;
	//Vector V2(V0.Nown);  V2 = V1 - V0;  cout << endl << "V2.norm() = " << V2.norm() << endl;  

	//2. each optimization param belongs to one line of MDeltaIntDeriv:     
	if (pMDeltaIntDeriv) {
		double dFringePeriodAir = 2.0 * PI / sEffInd.dkMidInAir;  double dFringePeriodHole = 2.0 * PI / sEffInd.dkMid_x_nEff;
		dFringePeriodAir /= Z_SCALE;  dFringePeriodHole /= Z_SCALE;
		dFringePeriodAir *= 0.0625;  dFringePeriodHole *= 0.0625;
		Vector VTop1(pVDeltaInt->Nown), VBot1(pVDeltaInt->Nown), VTop2(pVDeltaInt->Nown), VBot2(pVDeltaInt->Nown),
			VTop3(pVDeltaInt->Nown), VBot3(pVDeltaInt->Nown), VTop4(pVDeltaInt->Nown), VBot4(pVDeltaInt->Nown);
		vCalcIntFuVal(Z_SCALE, dzTop + dFringePeriodAir, dDepth, NULL, dDiameter, 1, 1, *pdVkWindow, pcSpectrum, bMoveOnlyEnvelope, VTop1, VBot1, NULL);
		vCalcIntFuVal(Z_SCALE, dzTop - dFringePeriodAir, dDepth, NULL, dDiameter, 1, 1, *pdVkWindow, pcSpectrum, bMoveOnlyEnvelope, VTop2, VBot2, NULL);
		vCalcIntFuVal(Z_SCALE, dzTop, dDepth + dFringePeriodHole, NULL, dDiameter, 1, 1, *pdVkWindow, pcSpectrum, bMoveOnlyEnvelope, VTop3, VBot3, NULL);
		vCalcIntFuVal(Z_SCALE, dzTop, dDepth - dFringePeriodHole, NULL, dDiameter, 1, 1, *pdVkWindow, pcSpectrum, bMoveOnlyEnvelope, VTop4, VBot4, NULL);
		*pMDeltaIntDeriv = (cppc) 0.0;
		for (i = 0; i < (int)pVDeltaInt->Nown; i++) {
			if (bOptParam[Z_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + Z_TOP_LISE] =
				(cAmplTop * VTop1.Data[i] + cAmplBot * VBot1.Data[i] - cAmplTop * VTop2.Data[i] - cAmplBot * VBot2.Data[i]) / (2.0 * dFringePeriodAir); //column 0 <=> param. 0 (z at top) 
			if (bOptParam[DEPTH_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + DEPTH_LISE] =
				(cAmplTop * VTop3.Data[i] + cAmplBot * VBot3.Data[i] - cAmplTop * VTop4.Data[i] - cAmplBot * VBot4.Data[i]) / (2.0 * dFringePeriodHole); //col 1 <=> param. 1 (z at bottom)   
			if (bOptParam[AMPL_RE_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_RE_TOP_LISE] = VTop.Data[i]; //col 2 <=> param. 2 (ampl at top)    
			if (bOptParam[AMPL_RE_BOT_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_RE_BOT_LISE] = VBot.Data[i]; //col 3 <=> param. 3 (ampl at bot) 
			if (bOptParam[AMPL_IM_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_IM_TOP_LISE] = VTop.Data[i] * cIm; //col 4 <=> param. 4 (phase at top) 
			if (bOptParam[AMPL_IM_BOT_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_IM_BOT_LISE] = VBot.Data[i] * cIm; //col 5 <=> param. 5 (phase at bot) 
		}//end for i
		if (bOptParam[Z_TOP_LISE])
			vWeightInDirSpace(pPlanar->dOrigin_z_Glob, pPlanar->dDelta_z, iNoFFTPoints, *pdVkWindow, pTSV->ikStart, pMDeltaIntDeriv->Data + Z_TOP_LISE, pMDeltaIntDeriv->Next, dVWeightsDirSpace,
				pMDeltaIntDeriv->Data + Z_TOP_LISE, pMDeltaIntDeriv->Next);
		if (bOptParam[DEPTH_LISE])
			vWeightInDirSpace(pPlanar->dOrigin_z_Glob, pPlanar->dDelta_z, iNoFFTPoints, *pdVkWindow, pTSV->ikStart, pMDeltaIntDeriv->Data + DEPTH_LISE, pMDeltaIntDeriv->Next, dVWeightsDirSpace,
				pMDeltaIntDeriv->Data + DEPTH_LISE, pMDeltaIntDeriv->Next);
		if (bOptParam[AMPL_RE_TOP_LISE])
			vWeightInDirSpace(pPlanar->dOrigin_z_Glob, pPlanar->dDelta_z, iNoFFTPoints, *pdVkWindow, pTSV->ikStart, pMDeltaIntDeriv->Data + AMPL_RE_TOP_LISE, pMDeltaIntDeriv->Next, dVWeightsDirSpace,
				pMDeltaIntDeriv->Data + AMPL_RE_TOP_LISE, pMDeltaIntDeriv->Next);
		if (bOptParam[AMPL_RE_BOT_LISE])
			vWeightInDirSpace(pPlanar->dOrigin_z_Glob, pPlanar->dDelta_z, iNoFFTPoints, *pdVkWindow, pTSV->ikStart, pMDeltaIntDeriv->Data + AMPL_RE_BOT_LISE, pMDeltaIntDeriv->Next, dVWeightsDirSpace,
				pMDeltaIntDeriv->Data + AMPL_RE_BOT_LISE, pMDeltaIntDeriv->Next);
		if (bOptParam[AMPL_IM_TOP_LISE])
			vWeightInDirSpace(pPlanar->dOrigin_z_Glob, pPlanar->dDelta_z, iNoFFTPoints, *pdVkWindow, pTSV->ikStart, pMDeltaIntDeriv->Data + AMPL_IM_TOP_LISE, pMDeltaIntDeriv->Next, dVWeightsDirSpace,
				pMDeltaIntDeriv->Data + AMPL_IM_TOP_LISE, pMDeltaIntDeriv->Next);
		if (bOptParam[AMPL_IM_BOT_LISE])
			vWeightInDirSpace(pPlanar->dOrigin_z_Glob, pPlanar->dDelta_z, iNoFFTPoints, *pdVkWindow, pTSV->ikStart, pMDeltaIntDeriv->Data + AMPL_IM_BOT_LISE, pMDeltaIntDeriv->Next, dVWeightsDirSpace,
				pMDeltaIntDeriv->Data + AMPL_IM_BOT_LISE, pMDeltaIntDeriv->Next);
	}//end if calc. deriv 

	*pSuc = true;
}



void vCalcDeltaIntAndDeriv2Surf_No_k0_Fourier(/*Vector *pVWeights, */ cppc *pIntMeasuredFourier, int iIndexStart, dVector& dVkWindow, double dDiameter,
	Vector& VSpectrum, Vector& VParam, Vector *pVDeltaInt, Matrix *pMDeltaIntDeriv, bool bOptParam[], bool bMoveOnlyEnvelope, bool *pSuc)
	//This function is for exactly 2 reflecting surfaces only!  
{
	*pSuc = false;
	if (pIntMeasuredFourier == NULL) //pMDeltaIntDeriv may be NULL, the derivative is not always needed in practice anyway, but function value always
		return;

	int i;
	double dzTop = VParam.Data[Z_TOP_LISE].real(), dDepth = VParam.Data[DEPTH_LISE].real();
	cppc cTddTop = 1, cTuuTop = 1, cRudTop = 1, cRduTop = 1, cRudBot = 0;
	cppc  cAmplTop(VParam.Data[AMPL_RE_TOP_LISE].real(), VParam.Data[AMPL_IM_TOP_LISE].real()), cAmplBot(VParam.Data[AMPL_RE_BOT_LISE].real(), VParam.Data[AMPL_IM_BOT_LISE].real());
	cppc *cSpectrum = VSpectrum.Data + iIndexStart, cDelta_k_Air, cDelta_k_Hole, cNumerator, cDenom, cFabryPerotFactor;
	Vector VTop(pVDeltaInt->Nown), VBot(pVDeltaInt->Nown);

	//0.) calc effective index inside hole:
	Vector VnEff(dVkWindow.Nown);   vCalcEffInd(dDiameter, dVkWindow, VnEff, sEffInd.iTypeOfHole);
	cppc cnEff_at_k_Mid;            vCalcEffInd(dDiameter, sEffInd.dkMidInAir, cnEff_at_k_Mid, sEffInd.iTypeOfHole);
	sEffInd.dkMid_x_nEff = cnEff_at_k_Mid.real() * sEffInd.dkMidInAir;

	/*{
		ofstream ofMaxDepth("MaxDepth.txt");
		double dD, dk, dDepth1DTE, dConstant = log(10.0) * 1.329 / (4.0 * PI);
		cppc cnEff1DTE, cnEff1DTM, cnEffCirc, dDepth1DTM, dDepthCirc;
		for (int ii = 1; ii <= 11; ii++)
		{
			if (ii == 11)
				dD = 1.5;
			else
				dD = ii;
			dk = 2.0 * PI / 1.329;  dk *= 2.0; // k has to be input as fringe frequency!
			vCalcEffInd(dD, dk, cnEff1DTE, ONE_DIM_TE);
			vCalcEffInd(dD, dk, cnEff1DTM, ONE_DIM_TM);
			vCalcEffInd(dD, dk, cnEffCirc, CIRC);
			dDepth1DTE = dConstant / abs(cnEff1DTE.imag());    dDepth1DTM = dConstant / abs(cnEff1DTM.imag());    dDepthCirc = dConstant / abs(cnEffCirc.imag());
			ofMaxDepth << dD << "\t" << cnEff1DTE.imag() << "\t" << dDepth1DTE << "  \t" << cnEff1DTM.imag() << "\t" << dDepth1DTM << "\t" << cnEffCirc.imag() << "\t" << dDepthCirc << endl;
		}
	}*/

	//1. calc function value F:  
	vCalcIntFuVal(Z_SCALE, dzTop, dDepth, NULL, dDiameter, 1, 1, dVkWindow, cSpectrum, bMoveOnlyEnvelope, VTop, VBot, pVDeltaInt);
	for (i = 0; i < pVDeltaInt->Nown; i++)
		pVDeltaInt->Data[i] = VTop.Data[i] * cAmplTop + VBot.Data[i] * cAmplBot - pIntMeasuredFourier[iIndexStart + i];

	//2. each optimization param belongs to one line of MDeltaIntDeriv:     
	if (pMDeltaIntDeriv) {
		double dFringePeriodAir = 2.0 * PI / sEffInd.dkMidInAir;  double dFringePeriodHole = 2.0 * PI / sEffInd.dkMid_x_nEff;
		dFringePeriodAir /= Z_SCALE;  dFringePeriodHole /= Z_SCALE;
		dFringePeriodAir *= 0.0625;  dFringePeriodHole *= 0.0625;
		Vector VTop1(pVDeltaInt->Nown), VBot1(pVDeltaInt->Nown), VTop2(pVDeltaInt->Nown), VBot2(pVDeltaInt->Nown),
			VTop3(pVDeltaInt->Nown), VBot3(pVDeltaInt->Nown), VTop4(pVDeltaInt->Nown), VBot4(pVDeltaInt->Nown);
		vCalcIntFuVal(Z_SCALE, dzTop + dFringePeriodAir, dDepth, NULL, dDiameter, 1, 1, dVkWindow, cSpectrum, bMoveOnlyEnvelope, VTop1, VBot1, NULL);
		vCalcIntFuVal(Z_SCALE, dzTop - dFringePeriodAir, dDepth, NULL, dDiameter, 1, 1, dVkWindow, cSpectrum, bMoveOnlyEnvelope, VTop2, VBot2, NULL);
		vCalcIntFuVal(Z_SCALE, dzTop, dDepth + dFringePeriodHole, NULL, dDiameter, 1, 1, dVkWindow, cSpectrum, bMoveOnlyEnvelope, VTop3, VBot3, NULL);
		vCalcIntFuVal(Z_SCALE, dzTop, dDepth - dFringePeriodHole, NULL, dDiameter, 1, 1, dVkWindow, cSpectrum, bMoveOnlyEnvelope, VTop4, VBot4, NULL);
		*pMDeltaIntDeriv = (cppc) 0.0;
		for (i = 0; i < pVDeltaInt->Nown; i++) {
			if (bOptParam[Z_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + Z_TOP_LISE] =
				(cAmplTop * VTop1.Data[i] + cAmplBot * VBot1.Data[i] - cAmplTop * VTop2.Data[i] - cAmplBot * VBot2.Data[i]) / (2.0 * dFringePeriodAir); //column 0 <=> param. 0 (z at top) 
			if (bOptParam[DEPTH_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + DEPTH_LISE] =
				(cAmplTop * VTop3.Data[i] + cAmplBot * VBot3.Data[i] - cAmplTop * VTop4.Data[i] - cAmplBot * VBot4.Data[i]) / (2.0 * dFringePeriodHole); //col 1 <=> param. 1 (z at bottom)   
			if (bOptParam[AMPL_RE_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_RE_TOP_LISE] = VTop.Data[i]; //col 2 <=> param. 2 (ampl at top)    
			if (bOptParam[AMPL_RE_BOT_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_RE_BOT_LISE] = VBot.Data[i]; //col 3 <=> param. 3 (ampl at bot) 
			if (bOptParam[AMPL_IM_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_IM_TOP_LISE] = VTop.Data[i] * cIm; //col 4 <=> param. 4 (phase at top) 
			if (bOptParam[AMPL_IM_BOT_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_IM_BOT_LISE] = VBot.Data[i] * cIm; //col 5 <=> param. 5 (phase at bot)
			/*
			cDelta_k_Air = dVkWindow.Data[i] - sEffInd.dkMidInAir;  cDelta_k_Hole = dVkWindow.Data[i] * VnEff.Data[i] - sEffInd.dkMid_x_nEff;
			if (bOptParam[Z_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + Z_TOP_LISE] = cAmplTop * VTop.Data[i] * (-cIm) * cDelta_k_Air * Z_SCALE; //column 0 <=> param. 0 (z at top)
			if (bOptParam[DEPTH_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + DEPTH_LISE] = cAmplBot * VBot.Data[i] * (-cIm) * cDelta_k_Hole * Z_SCALE; //col 1 <=> param. 1 (z at bottom)
			if (bOptParam[AMPL_RE_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_RE_TOP_LISE] = VTop.Data[i]; //col 2 <=> param. 2 (ampl at top)
			if (bOptParam[AMPL_RE_BOT_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_RE_BOT_LISE] = VBot.Data[i]; //col 3 <=> param. 3 (ampl at bot)
			if (bOptParam[AMPL_IM_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_IM_TOP_LISE] = VTop.Data[i] * cIm; //col 4 <=> param. 4 (phase at top)
			if (bOptParam[AMPL_IM_BOT_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_IM_BOT_LISE] = VBot.Data[i] * cIm; //col 5 <=> param. 5 (phase at bot)
			*/
		}
	}//end if calc. deriv 

	/* ////
	if (pVWeights) {
		ofstream ofWeights("WeightsUsedByMLA.txt");
		ofWeights << "# no. of Weights: " << pVWeights->Nown << "  no. of squares: " << pVDeltaInt->Nown << endl;
		if (pVWeights->Nown != pVDeltaInt->Nown) {
			cout << endl << "ERROR IN WEIGHTED LEAST SQUARES: MISMATCH OF DIMENSIONS !!!" << endl;   *pSuc = false;   return;
		}
		cppc *pcWeights = pVWeights->Data;
		for (i = 0; i < pVDeltaInt->Nown; i++) {
			double dWeight = abs(pcWeights[i]);  //dWeight = 1.0;
			pVDeltaInt->Data[i] *= dWeight;   ofWeights << i << "\t\t" << dWeight << endl;
			if (pMDeltaIntDeriv) {
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + Z_TOP_LISE] *= dWeight;
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + DEPTH_LISE] *= dWeight;
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_RE_TOP_LISE] *= dWeight;
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_RE_BOT_LISE] *= dWeight;
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_IM_TOP_LISE] *= dWeight;
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_IM_BOT_LISE] *= dWeight;
			}//end if Deriv
		}//end for i
	}//end if pdWeights
	*/ ////

	*pSuc = true;
}




void vCalcDeltaIntAndDerivDLSFourierCone(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo,
	Vector& VParam, double *pdFuVal, Matrix *pMDeriv, bool bOptParam[], bool *pSuc)
	//This function is for exactly 2 reflecting surfaces only!  
{
	*pSuc = false;
	static struct sCalcDeltaIntSq *pInt, *pSpec;    pInt = (struct sCalcDeltaIntSq *)pInfoInt, pSpec = (struct sCalcDeltaIntSq *)pInfoSpectrum;
	if (pInt->pVSpectrum->Data == NULL) //pMDeltaIntDeriv may be NULL, the derivative is not always needed in practice anyway, but function value always 
		return;
	static bool bSuc[9] = { true, true, true, true, true, true, true, true, true };  static int i;
	static double dNorm[9], dDelta_D_Top, dDelta_D_Bot;   dDelta_D_Top = VParam.Data[D_TOP_LISE].real() * 0.05, dDelta_D_Bot = VParam.Data[D_BOT_LISE].real() * 0.05;
	static double dDeriv_z_top, dDeriv_Depth, dDeriv_D_Top, dDeriv_D_Bot;
	static sInfoConicalCircTSV sInfoTSV, sInfoTSV_Deriv;
	static Vector VcAmpIn(2), VcAmpOut(2);   VcAmpIn.Data[0] = VcAmpIn.Data[1] = 0;
	Vector *pVcAmp = (Vector *)pvAdditionalInfo;

	//1. initialization:   
	sInfoTSV.dDiameterTop = VParam.Data[D_TOP_LISE].real(), sInfoTSV.dzTop = Z_SCALE * VParam.Data[Z_TOP_LISE].real(),
		sInfoTSV.dDiameterBot = VParam.Data[D_BOT_LISE].real(), sInfoTSV.dDepth = Z_SCALE * VParam.Data[DEPTH_LISE].real();
#ifdef OLOVIA_DEBUG 
	static ofstream ofParam("TSV_Params.txt");
	ofParam << "\t\t" << sInfoTSV.dDiameterTop << "\t\t" << sInfoTSV.dzTop << "\t\t" << sInfoTSV.dDiameterBot << "\t\t" << sInfoTSV.dDepth << endl;
#endif 

	//2. calc function value F: 
	cout << endl << "Dt = " << sInfoTSV.dDiameterTop << "   zt = " << sInfoTSV.dzTop << "   Db = " << sInfoTSV.dDiameterBot << "   H = " << sInfoTSV.dDepth << endl;
	if (pdFuVal != NULL) {
		bSuc[0] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV, pInt, pSpec, VcAmpOut, dNorm[0]);
		*pdFuVal = dNorm[0];   pVcAmp->Data[C_AMPL_TOP_LISE] = VcAmpOut.Data[C_AMPL_TOP_LISE];   pVcAmp->Data[C_AMPL_BOT_LISE] = VcAmpOut.Data[C_AMPL_BOT_LISE];
	}

	//3. each optimization param belongs to one line of MDeltaIntDeriv:     
	if (pMDeriv) {
		double dFringePeriodAir = 2.0 * PI / sEffInd.dkMidInAir;   double dFringePeriodHole = dFringePeriodAir;   static cppc cnEff;
		vCalcEffInd(0.5 * (sInfoTSV.dDiameterTop + sInfoTSV.dDiameterBot), sEffInd.dkMidInAir, cnEff, sEffInd.iTypeOfHole);
		if (cnEff.real() > 0.5) // should be always the case 
			dFringePeriodHole /= cnEff.real();
		dFringePeriodAir *= 2;  dFringePeriodHole *= 2; //*= 0.0625; // 2 periods 

		//Z_TOP_LISE: 
		cout << endl << "zt+:" << endl;
		sInfoTSV_Deriv = sInfoTSV;  sInfoTSV_Deriv.dzTop += dFringePeriodAir;      //dzTop + dFringePeriodAir 
		bSuc[1] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV_Deriv, pInt, pSpec, VcAmpOut, dNorm[1]);
		cout << endl << "zt-:" << endl;
		sInfoTSV_Deriv = sInfoTSV;  sInfoTSV_Deriv.dzTop -= dFringePeriodAir;      //dzTop - dFringePeriodAir 
		bSuc[2] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV_Deriv, pInt, pSpec, VcAmpOut, dNorm[2]);
		//DEPTH_LISE: 
		cout << endl << "H+:" << endl;
		sInfoTSV_Deriv = sInfoTSV;  sInfoTSV_Deriv.dDepth += dFringePeriodHole;    //dDepth + dFringePeriodHole
		bSuc[3] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV_Deriv, pInt, pSpec, VcAmpOut, dNorm[3]);
		cout << endl << "H-:" << endl;
		sInfoTSV_Deriv = sInfoTSV;  sInfoTSV_Deriv.dDepth -= dFringePeriodHole;    //dDepth - dFringePeriodHole
		bSuc[4] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV_Deriv, pInt, pSpec, VcAmpOut, dNorm[4]);
		//D_TOP_LISE: 
		cout << endl << "Dt+:" << endl;
		sInfoTSV_Deriv = sInfoTSV;  sInfoTSV_Deriv.dDiameterTop += dDelta_D_Top;   //dDiameterTop + dDelta_D_Top
		bSuc[5] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV_Deriv, pInt, pSpec, VcAmpOut, dNorm[5]);
		cout << endl << "Dt-:" << endl;
		sInfoTSV_Deriv = sInfoTSV;  sInfoTSV_Deriv.dDiameterTop -= dDelta_D_Top;   //dDiameterTop - dDelta_D_Top
		bSuc[6] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV_Deriv, pInt, pSpec, VcAmpOut, dNorm[6]);
		//D_BOT_LISE: 
		cout << endl << "Db+:" << endl;
		sInfoTSV_Deriv = sInfoTSV;  sInfoTSV_Deriv.dDiameterBot += dDelta_D_Bot;   //dDiameterBot + dDelta_D_Bot 
		bSuc[7] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV_Deriv, pInt, pSpec, VcAmpOut, dNorm[7]);
		cout << endl << "Db-:" << endl;
		sInfoTSV_Deriv = sInfoTSV;  sInfoTSV_Deriv.dDiameterBot -= dDelta_D_Bot;   //dDiameterBot - dDelta_D_Bot 
		bSuc[8] = bLevenbergMarquardtAmplOpt(VcAmpIn, 0.0, 0.0001 * TOLERANCE_DLS, 10 * MAX_ITER_DLS, pSpec->pdVkWindow->Nown, bCalcIntConeAmplOpt,
			&sInfoTSV_Deriv, pInt, pSpec, VcAmpOut, dNorm[8]);

		//Calc of central differences:  
		*pMDeriv = (cppc) 0.0;
		if (bOptParam[Z_TOP_LISE]) {
			dDeriv_z_top = (dNorm[1] - dNorm[2]) / (2.0 * dFringePeriodAir);  	pMDeriv->Data[Z_TOP_LISE] = dDeriv_z_top; //column 1 <=> param. 1 (z at top) 
		}
		if (bOptParam[DEPTH_LISE]) {
			dDeriv_Depth = (dNorm[3] - dNorm[4]) / (2.0 * dFringePeriodHole);	pMDeriv->Data[DEPTH_LISE] = dDeriv_Depth; //col 3 <=> param. 3 (z at bottom)  
		}
		if (bOptParam[D_TOP_LISE]) {
			dDeriv_D_Top = (dNorm[5] - dNorm[6]) / (2.0 * dDelta_D_Top);		pMDeriv->Data[D_TOP_LISE] = dDeriv_D_Top; //col 0 <=> param. 0 (ampl at top)    
		}
		if (bOptParam[D_BOT_LISE]) {
			dDeriv_D_Bot = (dNorm[7] - dNorm[8]) / (2.0 * dDelta_D_Bot);		pMDeriv->Data[D_BOT_LISE] = dDeriv_D_Bot; //col 2 <=> param. 2 (ampl at bot) 
		}
#ifdef OLOVIA_DEBUG 
		ofstream ofDeriv("Deriv.txt");	ofDeriv << "\t\t" << dDeriv_D_Top << "\t\t" << dDeriv_z_top << "\t\t" << dDeriv_D_Bot << "\t\t" << dDeriv_Depth << endl;
#endif 
	}//end if calc. deriv 

	*pSuc = true;
	if (bSuc[0] == false)
		*pSuc = false;
	if (pMDeriv)
		for (i = 1; i <= 8; i++)
			if (bSuc[i] == false)
				*pSuc = false;
}

void vCalcDeltaIntAndDeriv2SurfFourier(cppc *pIntMeasuredFourier, dVector *dVkWindow, double dDiameter, int iIndexStart,
	Vector VSpectrum, Vector& VParam, Vector *pVDeltaInt, Matrix *pMDeltaIntDeriv, bool bOptParam[], bool *pSuc)
	//This function is for exactly 2 reflecting surfaces only!   
{
	*pSuc = false;
	if (pVDeltaInt == NULL || pIntMeasuredFourier == NULL) //pMDeltaIntDeriv may be NULL, the derivative is not always needed in practice anyway, but function value always
		return;

	int i;
	double dDepth = VParam.Data[Z_BOT_LISE].real() - VParam.Data[Z_TOP_LISE].real();
	cppc cDelta_k_Air, cDelta_k_Hole, *cSpectrum = VSpectrum.Data + iIndexStart;
	Vector VDeltaIntTopAmplDeriv(pVDeltaInt->Nown), VDeltaIntTop_z_Deriv(pVDeltaInt->Nown), VDeltaIntBotAmplDeriv(pVDeltaInt->Nown), VDeltaIntBot_z_Deriv(pVDeltaInt->Nown);

	//0.) calc effective index inside hole:
	Vector VnEff(dVkWindow->Nown);   vCalcEffInd(dDiameter, *dVkWindow, VnEff, sEffInd.iTypeOfHole);
	cppc cnEff_at_k_Mid;  vCalcEffInd(dDiameter, sEffInd.dkMidInAir, cnEff_at_k_Mid, sEffInd.iTypeOfHole);
	sEffInd.dkMid_x_nEff = cnEff_at_k_Mid.real() * sEffInd.dkMidInAir;

	//1.) model the peak at the top surface: 
	for (i = 0; i < pVDeltaInt->Nown; i++) { //propagation:  
		cDelta_k_Air = dVkWindow->Data[i] - sEffInd.dkMidInAir;
		VDeltaIntTopAmplDeriv.Data[i] = cSpectrum[i] * exp(-cIm * cDelta_k_Air  * VParam.Data[Z_TOP_LISE].real());
		////}
		/*if (bOptParam[Z_TOP_LISE])
			for (i = 0; i < pVDeltaInt->Nown; i++) {
				cDelta_k_Air = dVk->Data[iIndexStart + i] - dk0;
				VDeltaIntTop_z_Deriv.Data[i] = VParam.Data[AMPL_TOP_LISE] * VSpectrum.Data[iIndexStart + i] * exp(-cIm * cDelta_k_Air * VParam.Data[Z_TOP_LISE].real()) *
					(-cIm * cDelta_k_Air);
			} */
			//2.) model the peak at the bottom surface:  
			////for (i = 0; i < pVDeltaInt->Nown; i++) { //propagation: 
				////cDelta_k_Air = dVkWindow->Data[i] - sEffInd.dkMidInAir;
		cDelta_k_Hole = dVkWindow->Data[i] * VnEff.Data[i] - sEffInd.dkMid_x_nEff;
		VDeltaIntBotAmplDeriv.Data[i] = VDeltaIntTopAmplDeriv.Data[i] * exp(-cIm * cDelta_k_Hole * dDepth);
	}//end for 
	/*if (bOptParam[Z_BOT_LISE])
		for (i = 0; i < pVDeltaInt->Nown; i++) {
			cDelta_k_Air = dVk->Data[iIndexStart + i] - dk0;  cDelta_k_Hole = dVk->Data[iIndexStart + i] * sEffInd.pV_n_Eff1DTE_for_k.Data[i] - dk0;
			VDeltaIntBot_z_Deriv.Data[i] = VParam.Data[AMPL_BOT_LISE] * VSpectrum.Data[iIndexStart + i] *
				exp(-cIm * cDelta_k_Air * VParam.Data[Z_TOP_LISE].real() - cIm * cDelta_k_Hole * dDepth) * (-cIm * cDelta_k_Hole);
		}*/
		//3. each optimization param belongs to one line of MDeltaIntDeriv:  
	if (pMDeltaIntDeriv) {
		*pMDeltaIntDeriv = (cppc) 0.0;
		for (i = 0; i < pVDeltaInt->Nown; i++) {
			/*if (bOptParam[Z_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + Z_TOP_LISE] = VDeltaIntTop_z_Deriv.Data[i]; //column 0 <=> param. 0 (z at top)
			if (bOptParam[Z_BOT_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + Z_BOT_LISE] = VDeltaIntBot_z_Deriv.Data[i]; //col 1 <=> param. 1 (z at bottom) */
			if (bOptParam[AMPL_TOP_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_TOP_LISE] = VDeltaIntTopAmplDeriv.Data[i]; //col 2 <=> param. 2 (ampl at top) 
			if (bOptParam[AMPL_BOT_LISE])
				pMDeltaIntDeriv->Data[i * pMDeltaIntDeriv->Next + AMPL_BOT_LISE] = VDeltaIntBotAmplDeriv.Data[i]; //col 3 <=> param. 3 (ampl at bot)   
		}
	}
	//4. calc function value F:
	for (i = 0; i < pVDeltaInt->Nown; i++)
		pVDeltaInt->Data[i] = VParam.Data[AMPL_TOP_LISE] * VDeltaIntTopAmplDeriv.Data[i] + VParam.Data[AMPL_BOT_LISE] * VDeltaIntBotAmplDeriv.Data[i] -
		pIntMeasuredFourier[iIndexStart + i];
	//5. output:  
#ifdef OLOVIA_DEBUG 
	ofstream FOutInt("Calc_Intensity_Deriv_by_FFT.txt");
	for (i = 0; i < pVDeltaInt->Nown; i++) { //for all pixels i 
		FOutInt << i << "\t\t" << pVDeltaInt->Data[i].real() << "\t\t" << pVDeltaInt->Data[i].imag();
		/*if (pMDeltaIntDeriv) {
			FOutInt << "\t\t" << pMDeltaIntDeriv->Data[4 * i].real() << "\t\t" << pMDeltaIntDeriv->Data[4 * i].imag();
			FOutInt << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 1].real() << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 1].imag();
			FOutInt << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 2].real() << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 2].imag();
			FOutInt << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 3].real() << "\t\t" << pMDeltaIntDeriv->Data[4 * i + 3].imag();
		}*/
	} //end for i  
#endif 
	*pSuc = true;
}


void vDeltaIntAndDerivForDLS(void *pInfoInt, void *pInfoSpectrum, Vector& VParam, Vector *pVResult, Matrix *pMResult, bool bOptParam[], bool *pSuc)
//works in direct space; is heavy but accurate.
{
	struct sCalcDeltaIntSq *pInt = (struct sCalcDeltaIntSq *)pInfoInt, *p = (struct sCalcDeltaIntSq *)pInfoSpectrum;
	vCalcDeltaIntAndDeriv2Surf(pInt->pIntMeasured, p->pdVk, p->pVSpectrum, VParam, pVResult, pMResult, bOptParam, pSuc);
}


void vDeltaIntAndDerivForDLS_No_k0_Fourier(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, Vector& VParam, Vector *pVResult, Matrix *pMResult,
	bool *pbOptParam, bool *pSuc)
{
	pvAdditionalInfo = NULL;
	struct sCalcDeltaIntSq *pInt = (struct sCalcDeltaIntSq *)pInfoInt, *p = (struct sCalcDeltaIntSq *)pInfoSpectrum;
	bool bMoveOnlyEnvelope = (pbOptParam != NULL);
	if (p->iTypeOfWeights == WEIGHTS_IN_DIRECT_SPACE)
		vCalcDeltaIntAndDeriv2Surf_No_k0_DirSpace(p, pInt, VParam, pVResult, pMResult, pbOptParam, bMoveOnlyEnvelope, pSuc);
	/*else if (p->iTypeOfWeights == WEIGHTS_IN_FOURIER_SPACE)
		vCalcDeltaIntAndDeriv2Surf_No_k0_Fourier( p->pVSpectrumSmoothened, pInt->pVSpectrum->Data, pInt->ikStart, *p->pdVkWindow, pInt->dDiameter, *p->pVSpectrum, VParam,
			pVResult, pMResult, pbOptParam, bMoveOnlyEnvelope, pSuc ); */
	else
		vCalcDeltaIntAndDeriv2Surf_No_k0_Fourier(/*NULL,*/ pInt->pVSpectrum->Data, pInt->ikStart, *p->pdVkWindow, pInt->dDiameter, *p->pVSpectrum, VParam,
			pVResult, pMResult, pbOptParam, bMoveOnlyEnvelope, pSuc);
}



void vMeritFuForAmpl(void *pInfoInt, void *pInfoSpectrum, Vector& VParam, Vector *pVResult, Matrix *pMResult, bool bOptParam[], bool *pSuc)
{
	struct sCalcDeltaIntSq *pInt = (struct sCalcDeltaIntSq *)pInfoInt, *p = (struct sCalcDeltaIntSq *)pInfoSpectrum;
	vCalcDeltaIntAndDeriv2SurfFourier(pInt->pVSpectrum->Data, p->pdVkWindow, pInt->dDiameter, p->ikStart, p->pVSpectrum, VParam, pVResult, pMResult, bOptParam, pSuc);
}


//###################################################  Functions for search of global minimum of merit function #######################################################


double dCalcEnvelope(double *pNorm)
{
	double dDiff24, dDiff24Sq, dDiff13, dDiff35, dTmp, dEnv;
	pNorm--; //we start counting from 1 here as in K. G. Larkin "Efficient nonlinear algorithm for envelope detection in white light interferometry", but C++ starts from 0
	dDiff24 = pNorm[2] - pNorm[4];
	dDiff24Sq = dDiff24 * dDiff24;
	dDiff13 = pNorm[1] - pNorm[3];
	dDiff35 = pNorm[3] - pNorm[5];
	dTmp = dDiff24Sq - dDiff13 * dDiff35;
	dEnv = dSqrt(dTmp);
	return dEnv;
}


double dCalcPhaseInEnvelope(double *pNorm) //returns phase in Rad at central point [3] of array from [1], [2], ... , [5]
{
	double dDiff24, dDiff15, dNumerator, dNumeratorSq, dDenominator, dPhaseInRad;
	pNorm--;
	dDiff24 = pNorm[2] - pNorm[4];  dDiff15 = pNorm[1] - pNorm[5];
	dNumeratorSq = 4.0 * dDiff24 * dDiff24 - dDiff15 * dDiff15;
	dNumerator = dSqrt(dNumeratorSq);
	dDenominator = -pNorm[1] + 2.0 * pNorm[3] - pNorm[5];
	dPhaseInRad = atan2(dNumerator, dDenominator);
	return dPhaseInRad;
}


double dCalcFuWithoutEnv(double *pNorm)
{
	pNorm--; //we start counting from 1 here as in K. G. Larkin "Efficient nonlinear algorithm for envelope detection in white light interferometry", but C++ starts from 0 
	double dMeanVal = pNorm[1] / 4.0 + pNorm[3] / 2.0 + pNorm[5] / 4.0; //since pNorm[1] + pNorm[5] = -pNorm[3] (approx). Does not allow an averaging: pNorm[2] / 4.0 + pNorm[4] / 4.0 !
	return dMeanVal;
}


bool bCalcEnvelopeDataAt_x2(Vector& VxStart, int iParamNo, double dzStep, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, Vector& Vx, Vector *VF, Matrix *pMdFdx, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpectrum, double& dEnvelopeCenter, double& dPhaseCenter, double& dNormCenter)
{
	bool bSuc, bOptParam[4];   bOptParam[Z_TOP_LISE] = bOptParam[Z_BOT_LISE] = bOptParam[AMPL_TOP_LISE] = bOptParam[AMPL_BOT_LISE] = false;
	int i;
	double dDelta_z, pNorm[5];
	Vector VInput, VF(iDimVF);

	//Calc. points around starting value: 
	for (i = 0; i < 5; i++) {
		dDelta_z = (i - 2) * dzStep;   VInput = VxStart;  VInput.Data[iParamNo] += dDelta_z;
		vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, VInput, &VF, NULL, bOptParam, &bSuc);  pNorm[i] = VF.norm();
		if (bSuc == false)
			return false;
	}
	dNormCenter = pNorm[2];
	dEnvelopeCenter = dCalcEnvelope(pNorm); //Is even a bit worse: dFuValCenter = 0.25 * pEnv[0] + 0.5 * pEnv[1] + 0.25 * pEnv[2];  
	dPhaseCenter = dCalcPhaseInEnvelope(pNorm);
	return true;
}


bool bCalcEnvelopeDataAt_x234(Vector& VxStart, int iParamNo, double dzStep, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, Vector& Vx, Vector *VF, Matrix *pMdFdx, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpectrum, double& dEnvCenter, double &dDerivOfEnv)
{
	bool bSuc, bOptParam[4];   bOptParam[Z_TOP_LISE] = bOptParam[Z_BOT_LISE] = bOptParam[AMPL_TOP_LISE] = bOptParam[AMPL_BOT_LISE] = false;
	int i;
	double dDelta_z, pNorm[9], pEnv[5];
	Vector VInput, VF(iDimVF);

	//Calc. points around starting value: 
	for (i = 0; i < 9; i++) {
		dDelta_z = (i - 4) * dzStep;   VInput = VxStart;  VInput.Data[iParamNo] += dDelta_z;
		vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, VInput, &VF, NULL, bOptParam, &bSuc);  pNorm[i] = VF.norm();
		if (bSuc == false)
			return false;
	}
	for (i = 0; i < 5; i++)
		pEnv[i] = dCalcEnvelope(pNorm + i);

	dDerivOfEnv = (pEnv[4] - pEnv[0]) / (2.0 * dzStep); //The distance between 0 and 4 is one period, this is advantageous!  
	dEnvCenter = 0.25 * pEnv[1] + 0.5 * pEnv[2] + 0.25 * pEnv[3]; //Is even a bit worse: dFuValCenter = 0.25 * pEnv[0] + 0.5 * pEnv[2] + 0.25 * pEnv[4];   
	return true;
}


bool bCalcEnvelopeDataAt_x234(Vector& VxStart, int iParamNo, double dzStep, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, Vector& Vx, Vector *VF, Matrix *pMdFdx, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpectrum, double& dEnvCenter, double &dDerivOfEnv, double& dFuCenter, double& dDerivOfFu, double& dNormCenter, double& dPhaseCenter,
	double *pdDelta_z_ToNextMin)
{
	bool bSuc, bSucMin, bOptParam[4];   bOptParam[Z_TOP_LISE] = bOptParam[Z_BOT_LISE] = bOptParam[AMPL_TOP_LISE] = bOptParam[AMPL_BOT_LISE] = false;
	int i, iIndexMin = 4;
	double dDelta_z[9], pNorm[9], pAverage[5], pEnv[5], pFuVal[5], pPhase[5], dNormMin, dNormAtMin;
	Vector VInput, VF(iDimVF), VxMin(VxStart);
	struct sCalcDeltaIntSq *pSpec = (struct sCalcDeltaIntSq *) pInfoSpectrum;

	//Calc. points around starting value: 
	for (i = 0; i < 9; i++) {
		dDelta_z[i] = (i - 4) * dzStep;   VInput = VxStart;  VInput.Data[iParamNo] += dDelta_z[i];
		vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, VInput, &VF, NULL, bOptParam, &bSuc);  pNorm[i] = VF.norm();
		if (bSuc == false)
			return false;
	}
	dNormCenter = pNorm[4]; //norm at central value  

	for (i = 0; i < 5; i++) {
		pAverage[i] = dCalcFuWithoutEnv(pNorm + i);  pEnv[i] = dCalcEnvelope(pNorm + i);  pFuVal[i] = pAverage[i] - pEnv[i] / 2.0;  pPhase[i] = dCalcPhaseInEnvelope(pNorm + i);
	}

	dDerivOfEnv = (pEnv[4] - pEnv[0]) / (4.0 * dzStep); //The distance between 0 and 4 is one period, this is advantageous!  
	dEnvCenter = 0.25 * pEnv[1] + 0.5 * pEnv[2] + 0.25 * pEnv[3]; //Is even a bit worse: dFuValCenter = 0.25 * pEnv[0] + 0.5 * pEnv[2] + 0.25 * pEnv[4];  
	dDerivOfFu = (pFuVal[4] - pFuVal[0]) / (4.0 * dzStep);
	dFuCenter = 0.25 * pFuVal[1] + 0.5 * pFuVal[2] + 0.25 * pFuVal[3];
	dPhaseCenter = pPhase[2]; //Wrong: 0.25 * pPhase[1] + 0.5 * pPhase[2] + 0.25 * pPhase[3]; since pPhase is always >= 0 in current implementation 

	if (pdDelta_z_ToNextMin) {
		iIndexMin = 4;  dNormMin = DBL_MAX;
		for (i = 2; i <= 6; i++)
			if (pNorm[i] < dNormMin) {
				dNormMin = pNorm[i];  iIndexMin = i;
			}
		*pdDelta_z_ToNextMin = (pSpec->dLambdaMid / 2.0) * dSign(pNorm[3] - pNorm[5]) * abs(dPhaseCenter - PI) / (2.0 * PI);
		VxMin.Data[iParamNo] += *pdDelta_z_ToNextMin;
		//2.) Check whether minimum: 
		vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, VxMin, &VF, NULL, bOptParam, &bSucMin);  dNormAtMin = VF.norm();
		if (bSucMin != true) //reset value, if minimum search has failed:
			*pdDelta_z_ToNextMin = dDelta_z[iIndexMin];
		else if (dNormAtMin >= dNormMin)
			*pdDelta_z_ToNextMin = dDelta_z[iIndexMin];
	}//end if  
	return true;
}


bool bFindInitValFor_z_at_zc_With3FuVal(Vector VxStart, int iParamNo,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, Vector& VParam, Vector *pVResult, Matrix *pMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpectrum, Vector& Vx)
{
	struct sCalcDeltaIntSq *pInt = (struct sCalcDeltaIntSq *)pInfoInt;
	bool bSuc1, bSuc2, bSuc3, bSucMin, bOptParam[4];   bOptParam[Z_TOP_LISE] = bOptParam[Z_BOT_LISE] = bOptParam[AMPL_TOP_LISE] = bOptParam[AMPL_BOT_LISE] = false;
	double dNorml, dNormStart, dNormr, dNormMin, da, dDiffl, dDiffStart, dDiffr, db, dyTmp, dxlTmp, dxrTmp, dc; //"Start is at position 2 in middle", "l" is on the left, "r" on the right
	double dDelta_2Minus, dDelta_Minus, dDelta_Plus, dDelta;
	Vector Vxl(VxStart), Vxr(VxStart), VxMin(VxStart), VF(pInt->ikStop - pInt->ikStart + 1);

	//Calc. points around starting value:
	Vxl.Data[iParamNo] = VxStart.Data[iParamNo] * cIm;  Vxr.Data[iParamNo] = VxStart.Data[iParamNo] / cIm; //shift by +/- Pi/2 
	vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, Vxl, &VF, NULL, bOptParam, &bSuc1);      dNorml = VF.norm();
	vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, VxStart, &VF, NULL, bOptParam, &bSuc2);  dNormStart = VF.norm();
	vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, Vxr, &VF, NULL, bOptParam, &bSuc3);      dNormr = VF.norm();
	if (bSuc1 != true || bSuc2 != true || bSuc3 != true)
		goto END;
	//Conclude z-position of the local minimum:
	da = (dNorml + dNormr) / 2.0;
	dDiffl = dNorml - da;   dDiffStart = dNormStart - da;   dDiffr = dNormr - da;
	db = dSqrt(0.5 * dDiffl * dDiffl + dDiffStart * dDiffStart + 0.5 * dDiffr * dDiffr);
	dyTmp = (dNormStart - da) / db;  dxlTmp = -1.0 * (dNorml - da) / db;  dxrTmp = (dNormr - da) / db;
	dc = atan2(dyTmp, 0.5 * dxlTmp + 0.5 * dxrTmp);
	dDelta_2Minus = (-2.5 * PI - dc); 	dDelta_Minus = (-0.5 * PI - dc);   dDelta_Plus = (+1.5 * PI - dc);  //phase differences to next minima
	if (fabs(dDelta_2Minus) < fabs(dDelta_Minus) && fabs(dDelta_2Minus) < fabs(dDelta_Plus))
		dDelta = dDelta_2Minus;
	else if (fabs(dDelta_Minus) < fabs(dDelta_2Minus) && fabs(dDelta_Minus) < fabs(dDelta_Plus))
		dDelta = dDelta_Minus;
	else if (fabs(dDelta_Plus) < fabs(dDelta_2Minus) && fabs(dDelta_Plus) < fabs(dDelta_Minus))
		dDelta = dDelta_Plus;
	else
		dDelta = dDelta_Minus;
	VxMin.Data[iParamNo] = Vx.Data[iParamNo] * exp(cIm * dDelta); //multiply complex amplitude at top or bottom with appropriate phase factor to match the measurement data 
	vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, VxMin, &VF, NULL, bOptParam, &bSucMin);  dNormMin = VF.norm();
	if (bSucMin != true)
		goto END;
	//Identify / Check whether minimum:
	if (dNormStart <= dNorml && dNormStart <= dNormMin && dNormStart <= dNormr)
		Vx = VxStart;
	else if (dNorml <= dNormStart && dNorml <= dNormMin && dNorml <= dNormr)
		Vx = Vxl;
	else if (dNormr <= dNorml && dNormr <= dNormStart && dNormr <= dNormMin)
		Vx = Vxr;
	else
		Vx = VxMin;
	return true;
END:
	Vx = VxStart;  return false;
}


bool bFindInitValFor_z_at_zc(Vector& VxStart, int iParamNo, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, Vector& VParam, Vector *pVResult, Matrix *pMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpectrum, Vector& Vx)
{
	bool bSuc;
	bool bSucMin, bOptParam[4];   bOptParam[Z_TOP_LISE] = bOptParam[Z_BOT_LISE] = bOptParam[AMPL_TOP_LISE] = bOptParam[AMPL_BOT_LISE] = false;
	double dDelta_z, dUnsed, dPhaseCenter, dNormCenter, dNormMin;
	Vector VxMin(VxStart), VF(iDimVF);
	struct sCalcDeltaIntSq *pInt = (struct sCalcDeltaIntSq *)pInfoInt, *pSpec = (struct sCalcDeltaIntSq *)pInfoSpectrum;

	//1.) Calc. position of nearest local minimum: 
	bSuc = bCalcEnvelopeDataAt_x2(VxStart, iParamNo, pInt->dLambdaMid, iDimVF, vCalc_F_or_dFdx, pInfoInt, pInfoSpectrum, dUnsed, dPhaseCenter, dNormCenter);
	if (bSuc == false)
		return false;
	dDelta_z = (pSpec->dLambdaMid / 2.0) * (dSign(dPhaseCenter) * PI - dPhaseCenter) / (2.0 * PI);
	VxMin.Data[iParamNo] += dDelta_z;
	//2.) Check whether minimum: 
	vCalc_F_or_dFdx(pInfoInt, pInfoSpectrum, VxMin, &VF, NULL, bOptParam, &bSucMin);  dNormMin = VF.norm();
	if (bSucMin != true)
		return false;
	if (dNormMin >= dNormCenter)
		Vx = VxStart;
	else
		Vx = VxMin;
	return true;
}



bool bFindInitValFor_z_at_zc_2Surf(Vector& VxStart, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, Vector& VParam, Vector *pVResult, Matrix *pMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpectrum, Vector& Vx)
{
	bool bSuc[2];
	Vector Vx0(Vx), Vx1(Vx);
	bSuc[0] = bFindInitValFor_z_at_zc(VxStart, Z_TOP_LISE, iDimVF, vCalc_F_or_dFdx, pInfoInt, pInfoSpectrum, Vx0);
	if (bSuc[0])
		Vx.Data[Z_TOP_LISE] = Vx0.Data[Z_TOP_LISE];
	bSuc[1] = bFindInitValFor_z_at_zc(VxStart, Z_BOT_LISE, iDimVF, vCalc_F_or_dFdx, pInfoInt, pInfoSpectrum, Vx1);
	if (bSuc[1])
		Vx.Data[Z_BOT_LISE] = Vx1.Data[Z_BOT_LISE];
	return bSuc[0] && bSuc[1];
}


long iRound(double dx)
{
	double dResult;
	long lResult;

	if (dx > 0.0) {
		dResult = dx + 0.5;
		lResult = (long)(dResult);
		return lResult;
	}
	else {
		dResult = dx - 0.5;
		lResult = (long)(dResult);
		return lResult;
	}
}


void vCalc_LambdaMid_OfLiseSignal(struct sCalcDeltaIntSq *pInt)
//LevenbergMarquardt algorithm as in Dahmen Reusken, "Numerik für Ingenieure und Naturwissenschaftler" on p. 222 - 224. 
{
	int i, ikMid, iDelta_k, iIndexOfMax = iRound((pInt->ikStart + pInt->ikStop) / 2.0);
	double dMaxAmpl, dk0 = 0.0, dWeight, dSumOfWeights = 0.0;
	Vector VIntWithout_k0(pInt->iNoFFTPoints);

	//Calc. k0 of measured signal:  
	ikMid = iIndexOfMax;  iDelta_k = (pInt->ikStop - pInt->ikStart);
	dMaxAmpl = 0.0;
	for (i = ikMid - iDelta_k / 4; i <= ikMid + iDelta_k / 4; i++) { //search about +/- 8 % around expected central frequency:   
		if (dMaxAmpl < abs(pInt->pVSpectrum->Data[i])) {
			dMaxAmpl = abs(pInt->pVSpectrum->Data[i]);  iIndexOfMax = i;
		}
	}
	//for (i = iIndexOfMax - iDelta_k / 4; i <= iIndexOfMax + iDelta_k / 4; i++) { //integrate about +/- 8 % around max. contribution:  
	for (i = pInt->ikStart; i <= pInt->ikStop; i++) {
		dWeight = abs(pInt->pVSpectrum->Data[i]);
		dk0 += dWeight * pInt->pdVk->Data[i];  dSumOfWeights += dWeight;
	}
	dk0 /= dSumOfWeights;
	pInt->dLambdaMid = 2.0 * (2.0 * PI / dk0); //since the fringe period is labda / 2  
}





bool bCalc_dFdx_ddFddx_1D(int iParamNo, double dzCenter, struct MinData& sMinl, struct MinData& sMinc, struct MinData& sMinr, cppc cF, cppc& cdFdz, cppc& cddFddz)
//Calc. f', f'' by solving 3 x 3 lin. equ. A * x = b 
{
	Matrix M(3, 3), MInv = MGetUnityMat(3);  Vector Vx(3), Vb(3);  //solve A * x = b  
	M(0, 0) = 1.0;  M(0, 1) = sMinl.VParam.Data[iParamNo] - dzCenter;  M(0, 2) = 0.5 * M(0, 1) * M(0, 1);
	M(1, 0) = 1.0;  M(1, 1) = sMinc.VParam.Data[iParamNo] - dzCenter;  M(1, 2) = 0.5 * M(1, 1) * M(1, 1);
	M(2, 0) = 1.0;  M(2, 1) = sMinr.VParam.Data[iParamNo] - dzCenter;  M(2, 2) = 0.5 * M(2, 1) * M(2, 1);
	if (abs(M(0, 1) - M(1, 1)) < NUM_NOISE || abs(M(1, 1) - M(2, 1)) < NUM_NOISE || M(0, 1).real() >= M(1, 1).real() || M(1, 1).real() >= M(2, 1).real()) { //failure, if minima coincide; we need 3 different minima!
		cdFdz = 0;  cddFddz = 0;  return false;
	}
	Vb.Data[0] = sMinl.dNorm;
	Vb.Data[1] = sMinc.dNorm;
	Vb.Data[2] = sMinr.dNorm;
	double dAverageSizeEntryM = M.norm();  dAverageSizeEntryM *= dAverageSizeEntryM;  dAverageSizeEntryM /= (double)(3 * 3);  dAverageSizeEntryM = dSqrt(dAverageSizeEntryM);
	double dSizePoductOf3Entries = pow(dAverageSizeEntryM, 3);
	double dAbs = abs(M.det() / dSizePoductOf3Entries);
	if (dAbs < 100.0 * NUM_NOISE) {
		cdFdz = 0;  cddFddz = 0;  return false;
	}
	MInv /= M;
	/*if (fInvert_2_x_2_mat(M.Data, M.Data) == false) {
		cdFdz = 0;  cddFddz = 0;  return false;
	}*/
	Vx = MInv * Vb;
	cF = Vx.Data[0].real(), cdFdz = Vx.Data[1].real(), cddFddz = Vx.Data[2].real();
	return true;
}

void vSelectMinimaOfMeritFu(double dToleranceMerit, int& iNoMin, MinData_double **ppMin)
//1. 
{
	bool *pbMinIsGlob = NULL;
	int j, iCount;
	double dNormGlobMin;

	//Find norm (merit function) of global minimum in the array:
	dNormGlobMin = DBL_MAX;  iCount = 0;//initializ. 
	for (j = 0; j < iNoMin; j++) {
		if ((*ppMin)[j].bLocMin) {
			if ((*ppMin)[j].dNorm < dNormGlobMin)
				dNormGlobMin = (*ppMin)[j].dNorm;
			iCount++;
		}
	}//end for j
	if (iCount == 0) {  //should not happen
		iNoMin = 0;   delete[](*ppMin);  *ppMin = NULL;   return;
	}

	//count the number of "global minima" and set them: 
	iCount = 0;  pbMinIsGlob = new bool[iNoMin];
	for (j = 0; j < iNoMin; j++) {
		if ((*ppMin)[j].bLocMin && (*ppMin)[j].dNorm <= dNormGlobMin * (1.0 + dToleranceMerit)) {
			pbMinIsGlob[j] = true;  iCount++;
		}
		else
			pbMinIsGlob[j] = false;
	} //end for j 
	iCount = 0;
	for (j = 0; j < iNoMin; j++) {
		if (pbMinIsGlob[j]) {
			(*ppMin)[iCount] = (*ppMin)[j];
			iCount++;
		}
	}
	iNoMin = iCount;
	/*	//Avoid multiple fold storage of identical minima:
		dVector dVDiff((*ppMin)->dVParam.Nown);  double dDiff;
		for (i = 0; i < iNoMin; i++)
			for (j = i + 1; j < iNoMin; j++) {
				if (pGlobMin[j].bLocMin) {
					dVDiff = pGlobMin[j].dVParam; dVDiff -= pGlobMin[i].dVParam;  dDiff = dVDiff.norm();
					if (dDiff < dToleranceParam) //if about 1.0 nm difference or less:
						pGlobMin[j].bLocMin = false;  iCount--;
				}
			}
		delete[](*ppMin);  (*ppMin) = new MinData_double[iCount]; iCount = 0;
		for (i = 0; i < iNoMin; i++)
			if (pGlobMin[i].bLocMin) {
				(*ppMin)[iCount] = pGlobMin[i];  iCount++;
			}
		iNoMin = iCount;
	*/
	delete[] pbMinIsGlob;
}


void vSelectMinimaOfMeritFu(double dToleranceMerit, int& iNoMin, struct MinData **ppMin)
//2. 
{
	bool *pbMinIsGlob = NULL;
	int j, iCount;
	double dNormGlobMin;
	MinData *pGlobMin = NULL;

	//Find norm (merit function) of global minimum in the array:
	dNormGlobMin = DBL_MAX;  iCount = 0;//initializ. 
	for (j = 0; j < iNoMin; j++) {
		if ((*ppMin)[j].bLocMin) {
			if ((*ppMin)[j].dNorm < dNormGlobMin)
				dNormGlobMin = (*ppMin)[j].dNorm;
			iCount++;
		}
	}//end for j
	if (iCount == 0) {  //should not happen
		iNoMin = 0;   delete[](*ppMin);  *ppMin = NULL;   return;
	}
	//count the number of "global minima" andset them: 
	iCount = 0;  pbMinIsGlob = new bool[iNoMin];
	for (j = 0; j < iNoMin; j++) {
		if ((*ppMin)[j].bLocMin && (*ppMin)[j].dNorm <= dNormGlobMin * (1.0 + dToleranceMerit)) {
			pbMinIsGlob[j] = true;  iCount++;
		}
		else
			pbMinIsGlob[j] = false;
	} //end for j 
	pGlobMin = new struct MinData[iCount];  iCount = 0;
	for (j = 0; j < iNoMin; j++) {
		if (pbMinIsGlob[j]) {
			pGlobMin[iCount] = (*ppMin)[j];  iCount++;
		}
	}
	iNoMin = iCount;
	/*	//Avoid multiple fold storage of identical minima:
		Vector VDiff((*ppMin)->VParam.Nown);  double dDiff;
		for (i = 0; i < iNoMin; i++)
			for (j = i + 1; j < iNoMin; j++) {
				if (pGlobMin[j].bLocMin) {
					VDiff = pGlobMin[j].VParam; VDiff -= pGlobMin[i].VParam;  dDiff = VDiff.norm();
					if (dDiff < dToleranceParam) //if about 1.0 nm difference or less:
						pGlobMin[j].bLocMin = false;  iCount--;
				}
			}
		delete[](*ppMin);  (*ppMin) = new struct MinData[iCount]; iCount = 0;
		for (i = 0; i < iNoMin; i++)
			if (pGlobMin[i].bLocMin) {
				(*ppMin)[iCount] = pGlobMin[i];  iCount++;
			}
		iNoMin = iCount;
	*/
	delete[] pbMinIsGlob;  delete[] pGlobMin;
}



bool bFindGlobMin2Surf(int iTypeOfWeights, Vector& Vx0, double dzTopMin, double dzTopMax, double dDepthMin, double dDepthMax, int iNoEntries, double *pSamplingDistDepth,
	void *pInfoIntensity, void *pInfoSpectrum, Vector& VParamGlobMin, /*double& dNormGlobMin,*/ int& iNoGlobMin, Vector& VStandardDeviation,
	double &dResult_delta_z_bot_fringes)
{
	struct sCalcDeltaIntSq *pSpec = (struct sCalcDeltaIntSq *)pInfoSpectrum, *pInt = (struct sCalcDeltaIntSq *)pInfoIntensity;
	int i, j;  //int iNoEntries = 7; 
	const int iDimDLS = 6;
	double dSamplingDistDepth, dDeltaLowest, dMiddle;
	//double dAccuracy1Envelope = 0.5, dAccuracy2Envelope = 0.5; //consider maximaly 100% error in envelope, e.g. 40 000 instead of 20 000   
	struct MinData *pMin = NULL, *pGlobMin = NULL;
	Vector VParamTmp(iDimDLS), VResult(pInt->ikStop - pInt->ikStart + 1), VZero(Vx0.Nown);

	if (iNoEntries < 1)
		iNoEntries = 1;
	if (dzTopMin > -0.1)
		dzTopMin = -0.1;
	if (dzTopMax < 0.1)
		dzTopMax = 0.1;
	if (dDepthMin < 0.1)
		dDepthMin = 0.1;
	if (dDepthMin < dzTopMax)
		dDepthMin = dzTopMax;
	if (dDepthMax < dDepthMin + 0.1)
		return false;

	//1.) Initialization: 
	pMin = new struct MinData[iNoEntries];
	for (i = 0; i < iNoEntries; i++) {
		pMin[i].bLocMin = false, pMin[i].dNorm = DBL_MAX, pMin[i].VParam = Vx0, pMin[i].VStandardDev = VZero;
	}

	//2. a) Iterative search of all the minima with complete modeling and DLS: 
	double dLambdaMid = (pInt->dLambdaMid + pSpec->dLambdaMid) / 2.0;   double dFringePeriod = dLambdaMid / 2.0; //  dAbsTop, dAbsBot;
	pSpec->dkMidMeritFu = pInt->dkMidMeritFu = 2.0 * PI / dFringePeriod;   sEffInd.dkMidInAir = 2.0 * PI / (pInt->dLambdaMid * 0.5);  //sEffInd.dkMidInAir *= 1.05; //for a tolerance test

	bool bLowerLimitHit, bUpperLimitHit, bOptParam[iDimDLS];   for (j = 0; j < iDimDLS; j++) bOptParam[j] = false;

	if (iNoEntries > 1) {
		if (pSamplingDistDepth == NULL)
			dSamplingDistDepth = fabs(dDepthMax - dDepthMin) / (double)iNoEntries;
		else
			dSamplingDistDepth = fabs(*pSamplingDistDepth);
		if (dSamplingDistDepth <= DBL_MIN) { //step size at least 0.1 µm
			delete[] pGlobMin;  pGlobMin = NULL;  return false;
		}
		dDeltaLowest = -(iNoEntries * dSamplingDistDepth) / 2.0 + dSamplingDistDepth / 2.0;
		for (i = 0; i < iNoEntries; i++)
			if (pSamplingDistDepth == NULL) {
				dMiddle = (dDepthMax + dDepthMin) / 2.0;
				pMin[i].VParam.Data[DEPTH_LISE] = dMiddle + dDeltaLowest + i * dSamplingDistDepth;
			}
			else
				pMin[i].VParam.Data[DEPTH_LISE] = pMin[i].VParam.Data[DEPTH_LISE] + dDeltaLowest + i * dSamplingDistDepth; //we had iNoEntries = 7 and SAMPLING_DISTANCE_DEPTH = 2.0
	}

	clock_t ctstart, ctend;
	double dtime_Tot;
	ctstart = clock();
	for (i = 0; i < iNoEntries; i++) {

		Vector VStandardDevTmp(iDimDLS), VLowerLimit(VParamTmp.Nown), VUpperLimit(VParamTmp.Nown);

		VLowerLimit.Data[Z_TOP_LISE] = dzTopMin / Z_SCALE; // -4.0 / Z_SCALE;
		VUpperLimit.Data[Z_TOP_LISE] = dzTopMax / Z_SCALE; // +4.0 / Z_SCALE;

		VLowerLimit.Data[DEPTH_LISE] = dDepthMin / Z_SCALE;  // 4.01 / Z_SCALE;
		if (pMin[i].VParam.Data[DEPTH_LISE].real() / Z_SCALE <= VLowerLimit.Data[DEPTH_LISE].real())
			pMin[i].VParam.Data[DEPTH_LISE].real(VLowerLimit.Data[DEPTH_LISE].real() * Z_SCALE + 0.1);
		VUpperLimit.Data[DEPTH_LISE] = dDepthMax / Z_SCALE; // (pMin[6].VParam.Data[DEPTH_LISE] + 16.0) / Z_SCALE;
		if (pMin[i].VParam.Data[DEPTH_LISE].real() / Z_SCALE >= VUpperLimit.Data[DEPTH_LISE].real())
			pMin[i].VParam.Data[DEPTH_LISE].real(VUpperLimit.Data[DEPTH_LISE].real() * Z_SCALE - 0.1);

		VLowerLimit.Data[AMPL_RE_TOP_LISE] = -10.0;			VUpperLimit.Data[AMPL_RE_TOP_LISE] = 10.0; //all amplitudes should be between 0 and 1 
		VLowerLimit.Data[AMPL_IM_TOP_LISE] = -10.0;			VUpperLimit.Data[AMPL_IM_TOP_LISE] = 10.0;

		VLowerLimit.Data[AMPL_RE_BOT_LISE] = -10.0;			VUpperLimit.Data[AMPL_RE_BOT_LISE] = 10.0;
		VLowerLimit.Data[AMPL_IM_BOT_LISE] = -10.0;			VUpperLimit.Data[AMPL_IM_BOT_LISE] = 10.0;

		for (j = 0; j < iDimDLS; j++) bOptParam[j] = true;
		VParamTmp.Data[Z_TOP_LISE] = pMin[i].VParam.Data[Z_TOP_LISE] / Z_SCALE;	VParamTmp.Data[DEPTH_LISE] = pMin[i].VParam.Data[DEPTH_LISE] / Z_SCALE;
		VParamTmp.Data[AMPL_RE_TOP_LISE] = pMin[i].VParam.Data[AMPL_TOP_LISE].real();   VParamTmp.Data[AMPL_IM_TOP_LISE] = pMin[i].VParam.Data[AMPL_TOP_LISE].imag();
		VParamTmp.Data[AMPL_RE_BOT_LISE] = pMin[i].VParam.Data[AMPL_BOT_LISE].real();   VParamTmp.Data[AMPL_IM_BOT_LISE] = pMin[i].VParam.Data[AMPL_BOT_LISE].imag();

		if (iTypeOfWeights == WEIGHTS_IN_DIRECT_SPACE)
			pSpec->iTypeOfWeights = WEIGHTS_IN_FOURIER_SPACE; //first, optimize completely in Fourier space to get good initial values for the weights in direct space later 
		pMin[i].bLocMin = bLevenbergMarquardt(VParamTmp, bOptParam, &VUpperLimit, NULL, &bUpperLimitHit, &VLowerLimit, NULL, &bLowerLimitHit, MU_START_DLS_LISE, true,
			6.0e4 * TOLERANCE_DLS, MAX_ITER_DLS, pInt->ikStop - pInt->ikStart + 1, vDeltaIntAndDerivForDLS_No_k0_Fourier, pInfoIntensity, pInfoSpectrum,
			NULL, VParamTmp, &VStandardDevTmp, pMin[i].dNorm, true);
		if (pMin[i].bLocMin == true && iTypeOfWeights == WEIGHTS_IN_DIRECT_SPACE) { //success in first step: 
			pSpec->iTypeOfWeights = WEIGHTS_IN_DIRECT_SPACE; //secondly, optimize with weights in direct space and good initial values:  
			pMin[i].bLocMin = bLevenbergMarquardt(VParamTmp, bOptParam, &VUpperLimit, NULL, &bUpperLimitHit, &VLowerLimit, NULL, &bLowerLimitHit, MU_START_DLS_LISE, true,
				6.0e4 * TOLERANCE_DLS, MAX_ITER_DLS, pInt->ikStop - pInt->ikStart + 1, vDeltaIntAndDerivForDLS_No_k0_Fourier, pInfoIntensity, pInfoSpectrum,
				NULL, VParamTmp, &VStandardDevTmp, pMin[i].dNorm, true);
		}

		if (pMin[i].bLocMin == true) { //if loc min found: 
			//parameters: 
			pMin[i].VParam.Data[Z_TOP_LISE] = VParamTmp.Data[Z_TOP_LISE] * Z_SCALE;
			pMin[i].VParam.Data[DEPTH_LISE] = VParamTmp.Data[DEPTH_LISE] * Z_SCALE;
			pMin[i].VParam.Data[AMPL_TOP_LISE] = VParamTmp.Data[AMPL_RE_TOP_LISE].real() + cIm * VParamTmp.Data[AMPL_IM_TOP_LISE].real();
			pMin[i].VParam.Data[AMPL_BOT_LISE] = VParamTmp.Data[AMPL_RE_BOT_LISE].real() + cIm * VParamTmp.Data[AMPL_IM_BOT_LISE].real();
			//////cout << "VParam  =  (AMPL_TOP, Z_TOP, AMPL_BOT, DEPTH)  =  "
			//////	<< pMin[i].VParam.Data[AMPL_TOP_LISE] << "  " << pMin[i].VParam.Data[Z_TOP_LISE] << "  "
			//////	<< pMin[i].VParam.Data[AMPL_BOT_LISE] << "  " << pMin[i].VParam.Data[DEPTH_LISE] << endl; 
			//standard deviation: 
			pMin[i].VStandardDev.Data[Z_TOP_LISE] = VStandardDevTmp.Data[Z_TOP_LISE] * Z_SCALE;
			pMin[i].VStandardDev.Data[DEPTH_LISE] = VStandardDevTmp.Data[DEPTH_LISE] * Z_SCALE;
			pMin[i].VStandardDev.Data[AMPL_TOP_LISE] = VStandardDevTmp.Data[AMPL_RE_TOP_LISE].real() + cIm * VStandardDevTmp.Data[AMPL_IM_TOP_LISE].real();
			pMin[i].VStandardDev.Data[AMPL_BOT_LISE] = VStandardDevTmp.Data[AMPL_RE_BOT_LISE].real() + cIm * VStandardDevTmp.Data[AMPL_IM_BOT_LISE].real();
			//////cout << "VStandardDeviation  =  (Delta_AMPL_TOP, Delta_Z_TOP, Delta_AMPL_BOT, Delta_DEPTH)  =  "
			////// 	 << pMin[i].VStandardDev.Data[AMPL_TOP_LISE] << "  " << pMin[i].VStandardDev.Data[Z_TOP_LISE] << "  "
			//////	 << pMin[i].VStandardDev.Data[AMPL_BOT_LISE] << "  " << pMin[i].VStandardDev.Data[DEPTH_LISE] << endl;
		}//end if loc. min found 
	}//end for i   
	ctend = clock();
	dtime_Tot = (double)(ctend - ctstart) / (double)CLOCKS_PER_SEC;  cout << endl << "total computation time in seconds = " << dtime_Tot << endl;  //total comp. time

	//3.) POST-PROCESSING : Find global minimum in the array: 

	iNoGlobMin = iNoEntries;   pGlobMin = pMin;  pMin = NULL;
	vSelectMinimaOfMeritFu(10.0 * TOLERANCE_DLS, iNoGlobMin, &pGlobMin);
	if (iNoGlobMin <= 0) {
		delete[] pGlobMin;  pGlobMin = NULL;  return false;
	}

	//Averaging, if more than one parameter combination matches (should never happen):  
	VParamGlobMin = 0.0;   VStandardDeviation = 0.0;
	for (i = 0; i < iNoGlobMin; i++) {
		VParamGlobMin += pGlobMin[i].VParam;   VStandardDeviation += pGlobMin[i].VStandardDev;
	}
	if (iNoGlobMin > 1) { //this should not happen or is very unusual 
		VParamGlobMin /= (cppc)iNoGlobMin;     VStandardDeviation /= (cppc)iNoGlobMin;
		cout << endl << "Warning: more than one combination of parameters matches the measurement data! " << iNoGlobMin << " minima (solutions) have been found." << endl;
	}
	if (iNoGlobMin > 1) {
		Vector VStandardDev2(Vx0.Nown);
		for (i = 0; i < iNoGlobMin; i++)
			for (j = 0; j < (int)VParamGlobMin.Nown; j++) {
				double dDifference = abs(pGlobMin[i].VParam.Data[j] - VParamGlobMin.Data[j]);
				VStandardDev2.Data[j] += dDifference * dDifference;
			}
		VStandardDev2 *= 1.0 / (double)(iNoGlobMin - 1);
		for (j = 0; j < (int)VParamGlobMin.Nown; j++)
			VStandardDev2.Data[j] = sqrt(VStandardDev2.Data[j]);
		VStandardDeviation += VStandardDev2;
	}

	dResult_delta_z_bot_fringes = 0.0;
	//double dPhi_top = arg(VParamGlobMin.Data[AMPL_TOP_LISE]), dPhi_bot = arg(VParamGlobMin.Data[AMPL_BOT_LISE]);
	//double dTmp = fmod(dPhi_top - VParamGlobMin.Data[DEPTH_LISE].real() * sEffInd.dkMid_x_nEff - dPhi_bot, 2.0 * PI);
	//while (dTmp <= -PI)
	//	dTmp += 2.0 * PI;
	//while (dTmp > PI)
	//	dTmp -= 2.0 * PI; 
	//dResult_delta_z_bot_fringes = dTmp / sEffInd.dkMid_x_nEff; 
	double dLambdaEff = 2.0 * PI / sEffInd.dkMid_x_nEff;  	cout << endl << "dLambdaEff = " << dLambdaEff << endl;

	//correct the phase:
	//VParamGlobMin.Data[AMPL_BOT_LISE] *= exp(cIm * sEffInd.dkMid_x_nEff * (VParamGlobMin.Data[DEPTH_LISE].real() + dResult_delta_z_bot_fringes));

	//Plot merit fu.: 
	/*
	ofstream ofMeritFu("./results_1DTM/Merit.txt");
	bool bSuc = true;   double dDepth, dDelta_z, dNorm;
	for (j = -2400; j <= 2400; j++) {
		VParamTmp.Data[Z_TOP_LISE] = VParamGlobMin.Data[Z_TOP_LISE] / Z_SCALE;
		dDelta_z = j * 0.01 * dFringePeriod;   dDepth = VParamGlobMin.Data[DEPTH_LISE].real() + dResult_delta_z_bot_fringes + dDelta_z;
		VParamTmp.Data[DEPTH_LISE] = dDepth / Z_SCALE;
		VParamTmp.Data[AMPL_RE_TOP_LISE] = VParamGlobMin.Data[AMPL_TOP_LISE].real();   VParamTmp.Data[AMPL_IM_TOP_LISE] = VParamGlobMin.Data[AMPL_TOP_LISE].imag();
		VParamTmp.Data[AMPL_RE_BOT_LISE] = VParamGlobMin.Data[AMPL_BOT_LISE].real();   VParamTmp.Data[AMPL_IM_BOT_LISE] = VParamGlobMin.Data[AMPL_BOT_LISE].imag();
		vDeltaIntAndDerivForDLS_No_k0_Fourier(pInfoIntensity, pInfoSpectrum, VParamTmp, &VResult, NULL, NULL, &bSuc);
		dNorm = sqrt(2.0) * VResult.norm();
		if (bSuc)
			ofMeritFu << j << "\t\t" << dDepth << "\t\t" << dNorm << "\t\t" << dNorm * dNorm << endl;
	}
	*/

	delete[] pGlobMin;  pGlobMin = NULL;
	return true;
}



void vLimitTheStepSize(Vector &Vx, Vector &Vs, Vector &VxNext, void *pInfoSpectrum, double &dMu)
{
	struct sCalcDeltaIntSq *pSpec = (struct sCalcDeltaIntSq *)pInfoSpectrum;
	double  dkMin = 0.5 * pSpec->pdVkWindow->Data[0], dkMax = 0.5 * pSpec->pdVkWindow->Data[pSpec->pdVkWindow->Nown - 1];
	double  dLambdaMin = 2.0 * PI / dkMax, dLambdaMax = 2.0 * PI / dkMin;
	double  dScalMin = sFP.dVLambda.Data[0] / dLambdaMax, dScalMax = sFP.dVLambda.Data[sFP.dVLambda.Nown - 1] / dLambdaMin;
	double  dDMin = sFP.VD.Data[0] / dScalMin, dDMax = sFP.VD.Data[sFP.VD.Nown - 1] / dScalMax;
	dDMin *= 1.0 + STEP_SIZE_D;  dDMax *= 1.0 - STEP_SIZE_D;
	bool bBondaryHit = false, bStepTooLarge = false;

	if (VxNext.Data[D_TOP_LISE].real() < dDMin)
		VxNext.Data[D_TOP_LISE].real(dDMin), bBondaryHit = true;
	if (VxNext.Data[D_TOP_LISE].real() > dDMax)
		VxNext.Data[D_TOP_LISE].real(dDMax), bBondaryHit = true;

	if (VxNext.Data[D_BOT_LISE].real() < dDMin)
		VxNext.Data[D_BOT_LISE].real(dDMin), bBondaryHit = true;
	if (VxNext.Data[D_BOT_LISE].real() > dDMax)
		VxNext.Data[D_BOT_LISE].real(dDMax), bBondaryHit = true;
	/*
	dDMax = 1.25 * Vx.Data[D_TOP_LISE].real();
	if (VxNext.Data[D_TOP_LISE].real() > dDMax)
		VxNext.Data[D_TOP_LISE].real(dDMax), bStepTooLarge = true;
	dDMin = 0.75 * Vx.Data[D_TOP_LISE].real();
	if (VxNext.Data[D_TOP_LISE].real() < dDMin)
		VxNext.Data[D_TOP_LISE].real(dDMin), bStepTooLarge = true;

	dDMax = 1.25 * Vx.Data[D_BOT_LISE].real();
	if (VxNext.Data[D_BOT_LISE].real() > dDMax)
		VxNext.Data[D_BOT_LISE].real(dDMax), bStepTooLarge = true;
	dDMin = 0.75 * Vx.Data[D_BOT_LISE].real();
	if (VxNext.Data[D_BOT_LISE].real() < dDMin)
		VxNext.Data[D_BOT_LISE].real(dDMin), bStepTooLarge = true;
	*/
	double dzTopMax = Vx.Data[Z_TOP_LISE].real() + 1.0 / Z_SCALE;
	if (VxNext.Data[Z_TOP_LISE].real() > dzTopMax)
		VxNext.Data[Z_TOP_LISE].real(dzTopMax), bStepTooLarge = true;
	double dzTopMin = Vx.Data[Z_TOP_LISE].real() - 1.0 / Z_SCALE;
	if (VxNext.Data[Z_TOP_LISE].real() < dzTopMin)
		VxNext.Data[Z_TOP_LISE].real(dzTopMin), bStepTooLarge = true;

	double dzBotMax = Vx.Data[Z_BOT_LISE].real() + 1.0 / Z_SCALE;
	if (VxNext.Data[Z_BOT_LISE].real() > dzBotMax)
		VxNext.Data[Z_BOT_LISE].real(dzBotMax), bStepTooLarge = true;
	double dzBotMin = Vx.Data[Z_BOT_LISE].real() - 1.0 / Z_SCALE;
	if (VxNext.Data[Z_BOT_LISE].real() < dzBotMin)
		VxNext.Data[Z_BOT_LISE].real(dzBotMin), bStepTooLarge = true;

	if (bBondaryHit)
		dMu *= 2;
	else if (bStepTooLarge)
		dMu *= 2;

	Vs = VxNext - Vx;
}


bool bFindGlobMin2SurfCone(Vector& Vx0, double dzTopMin, double dzTopMax, double dDepthMin, double dDepthMax,
	void *pInfoIntensity, void *pInfoSpectrum, void *pvAdditionalInfo, Vector& VParamGlobMin, /*double& dNormGlobMin,*/ int& iNoGlobMin, Vector& VStandardDeviation,
	double &dResult_delta_z_bot_fringes)
{
	struct sCalcDeltaIntSq *pSpec = (struct sCalcDeltaIntSq *)pInfoSpectrum, *pInt = (struct sCalcDeltaIntSq *)pInfoIntensity;
	int i, j, iNoEntries = 3;
	double daDiamTopStart[] = { Vx0.Data[D_TOP_LISE].real(), 0.66 * Vx0.Data[D_TOP_LISE].real(), 1.5 * Vx0.Data[D_TOP_LISE].real() };
	double daDiamBotStart[] = { Vx0.Data[D_BOT_LISE].real(), 0.66 * Vx0.Data[D_BOT_LISE].real(), 1.5 * Vx0.Data[D_BOT_LISE].real() };
	struct MinData *pMin = NULL, *pGlobMin = NULL;
	Vector VParamTmp(Vx0.Nown), VZero(Vx0.Nown);

	//1.) Initialization: 

	if (dzTopMin > -0.1)
		dzTopMin = -0.1;
	if (dzTopMax < 0.1)
		dzTopMax = 0.1;
	if (dDepthMin < 0.1)
		dDepthMin = 0.1;
	if (dDepthMax < dDepthMin + 0.1)
		dDepthMax = dDepthMin + 0.1;
	if (Vx0.Data[DEPTH_LISE].real() <= dDepthMin)
		Vx0.Data[DEPTH_LISE].real(dDepthMin + 0.1);
	if (Vx0.Data[DEPTH_LISE].real() >= dDepthMax)
		Vx0.Data[DEPTH_LISE].real(dDepthMax - 0.1);
	double dDmin = sFP.VD.Data[1], dDmax = sFP.VD.Data[sFP.VD.Nown - 2];
	for (i = 0; i < iNoEntries; i++) {
		if (daDiamTopStart[i] < 1.1 * dDmin)
			daDiamTopStart[i] = 1.1 * dDmin;
		if (daDiamTopStart[i] > 0.9 * dDmax)
			daDiamTopStart[i] = 0.9 * dDmax;
		if (daDiamBotStart[i] < 1.1 * dDmin)
			daDiamBotStart[i] = 1.1 * dDmin;
		if (daDiamBotStart[i] > 0.9 * dDmax)
			daDiamBotStart[i] = 0.9 * dDmax;
	}

	pMin = new struct MinData[iNoEntries];
	for (i = 0; i < iNoEntries; i++) {
		pMin[i].bLocMin = false, pMin[i].dNorm = DBL_MAX, pMin[i].VParam = Vx0, pMin[i].VStandardDev = VZero;
		pMin[i].VParam.Data[D_TOP_LISE] = daDiamTopStart[i];   pMin[i].VParam.Data[D_BOT_LISE] = daDiamBotStart[i];
	}

	//2. a) Iterative search of all the minima with complete modeling and DLS: 

	double dLambdaMid = (pInt->dLambdaMid + pSpec->dLambdaMid) / 2.0;   double dFringePeriod = dLambdaMid / 2.0; //  dAbsTop, dAbsBot;
	pSpec->dkMidMeritFu = pInt->dkMidMeritFu = 2.0 * PI / dFringePeriod;   sEffInd.dkMidInAir = 2.0 * PI / (pInt->dLambdaMid * 0.5);  //sEffInd.dkMidInAir *= 1.05; //for a tolerance test
	bool bLowerLimitHit, bUpperLimitHit;

	clock_t ctstart, ctend;
	double dtime_Tot;
	ctstart = clock();

	Vector VStandardDevTmp(Vx0.Nown), VLowerLimit(Vx0.Nown), VUpperLimit(Vx0.Nown);
	VLowerLimit.Data[Z_TOP_LISE] = dzTopMin / Z_SCALE;   VUpperLimit.Data[Z_TOP_LISE] = dzTopMax / Z_SCALE;
	VLowerLimit.Data[DEPTH_LISE] = dDepthMin / Z_SCALE;  VUpperLimit.Data[DEPTH_LISE] = dDepthMax / Z_SCALE;
	VLowerLimit.Data[D_TOP_LISE] = sFP.VD.Data[1];		 VUpperLimit.Data[D_TOP_LISE] = sFP.VD.Data[sFP.VD.Nown - 2];
	VLowerLimit.Data[D_BOT_LISE] = sFP.VD.Data[1];		 VUpperLimit.Data[D_BOT_LISE] = sFP.VD.Data[sFP.VD.Nown - 2];

	for (i = 0; i < iNoEntries; i++) {
		VParamTmp.Data[Z_TOP_LISE] = pMin[i].VParam.Data[Z_TOP_LISE] / Z_SCALE;	    VParamTmp.Data[DEPTH_LISE] = pMin[i].VParam.Data[DEPTH_LISE] / Z_SCALE;
		VParamTmp.Data[D_TOP_LISE] = pMin[i].VParam.Data[D_TOP_LISE];               VParamTmp.Data[D_BOT_LISE] = pMin[i].VParam.Data[D_BOT_LISE];

		pMin[i].bLocMin = bLevenbergMarquardtScalar(VParamTmp, NULL, &VUpperLimit, NULL, &bUpperLimitHit, &VLowerLimit, NULL, &bLowerLimitHit, MU_START_DLS_LISE_DIAM, true,
			6.0e4 * TOLERANCE_DLS, MAX_ITER_DLS, pInt->ikStop - pInt->ikStart + 1, vCalcDeltaIntAndDerivDLSFourierCone, vLimitTheStepSize, pInfoIntensity, pInfoSpectrum,
			pvAdditionalInfo, VParamTmp, &VStandardDevTmp, pMin[i].dNorm);

		if (pMin[i].bLocMin == true) { //if loc min found: 
			//parameters: 
			pMin[i].VParam.Data[Z_TOP_LISE] = VParamTmp.Data[Z_TOP_LISE] * Z_SCALE;
			pMin[i].VParam.Data[DEPTH_LISE] = VParamTmp.Data[DEPTH_LISE] * Z_SCALE;
			pMin[i].VParam.Data[D_TOP_LISE] = VParamTmp.Data[D_TOP_LISE];
			pMin[i].VParam.Data[D_BOT_LISE] = VParamTmp.Data[D_BOT_LISE];
			//standard deviation: 
			pMin[i].VStandardDev.Data[Z_TOP_LISE] = VStandardDevTmp.Data[Z_TOP_LISE] * Z_SCALE;
			pMin[i].VStandardDev.Data[DEPTH_LISE] = VStandardDevTmp.Data[DEPTH_LISE] * Z_SCALE;
			pMin[i].VStandardDev.Data[D_TOP_LISE] = VStandardDevTmp.Data[D_TOP_LISE];
			pMin[i].VStandardDev.Data[D_BOT_LISE] = VStandardDevTmp.Data[D_BOT_LISE];
			//pMin[i].VStandardDev.Data[AMPL_TOP_LISE] = VStandardDevTmp.Data[AMPL_RE_TOP_LISE].real() + cIm * VStandardDevTmp.Data[AMPL_IM_TOP_LISE].real(); 
			//pMin[i].VStandardDev.Data[AMPL_BOT_LISE] = VStandardDevTmp.Data[AMPL_RE_BOT_LISE].real() + cIm * VStandardDevTmp.Data[AMPL_IM_BOT_LISE].real(); 
		}//end if loc. min found 
	}//end for i   
	ctend = clock();
	dtime_Tot = (double)(ctend - ctstart) / (double)CLOCKS_PER_SEC;  cout << endl << "total computation time in seconds = " << dtime_Tot << endl;  //total comp. time

	//3.) POST-PROCESSING : Find global minimum in the array: 

	iNoGlobMin = iNoEntries;   pGlobMin = pMin;  pMin = NULL;
	vSelectMinimaOfMeritFu(10.0 * TOLERANCE_DLS, iNoGlobMin, &pGlobMin);
	if (iNoGlobMin <= 0) {
		delete[] pGlobMin;  pGlobMin = NULL;  return false;
	}

	//Averaging, if more than one parameter combination matches (should never happen):  
	VParamGlobMin = 0.0;   VStandardDeviation = 0.0;
	for (i = 0; i < iNoGlobMin; i++) {
		VParamGlobMin += pGlobMin[i].VParam;   VStandardDeviation += pGlobMin[i].VStandardDev;
	}
	if (iNoGlobMin > 1) { //this should not happen or is very unusual 
		VParamGlobMin /= (cppc)iNoGlobMin;     VStandardDeviation /= (cppc)iNoGlobMin;
		cout << endl << "Warning: more than one combination of parameters matches the measurement data! " << iNoGlobMin << " minima (solutions) have been found." << endl;
	}
	if (iNoGlobMin > 1) {
		Vector VStandardDev2(Vx0.Nown);
		for (i = 0; i < iNoGlobMin; i++)
			for (j = 0; j < (int)VParamGlobMin.Nown; j++) {
				double dDifference = abs(pGlobMin[i].VParam.Data[j] - VParamGlobMin.Data[j]);
				VStandardDev2.Data[j] += dDifference * dDifference;
			}
		VStandardDev2 *= 1.0 / (double)(iNoGlobMin - 1);
		for (j = 0; j < (int)VParamGlobMin.Nown; j++)
			VStandardDev2.Data[j] = sqrt(VStandardDev2.Data[j]);
		VStandardDeviation += VStandardDev2;
	}

	dResult_delta_z_bot_fringes = 0.0;
	//Correct the phase: not ready yet!
	//double dPhi_top = arg(VParamGlobMin.Data[AMPL_TOP_LISE]), dPhi_bot = arg(VParamGlobMin.Data[AMPL_BOT_LISE]);
	//double dTmp = fmod(dPhi_top - VParamGlobMin.Data[DEPTH_LISE].real() * sEffInd.dkMid_x_nEff - dPhi_bot, 2.0 * PI);
	//while (dTmp <= -PI)
	//	dTmp += 2.0 * PI;
	//while (dTmp > PI)
	//	dTmp -= 2.0 * PI;
	//dResult_delta_z_bot_fringes = dTmp / sEffInd.dkMid_x_nEff;
	double dLambdaEff = 2.0 * PI / sEffInd.dkMid_x_nEff;   cout << endl << "dLambdaEff = " << dLambdaEff << endl;
	//VParamGlobMin.Data[AMPL_BOT_LISE] *= exp(cIm * sEffInd.dkMid_x_nEff * (dHeightTSV + dResult_delta_z_bot_fringes)); 

	//Plot merit fu.: 
	//ofstream ofMeritFu("./results_1DTM/Merit.txt");
	//bool bSucMerit = true;   double dDepth, dDelta_z, dNorm;
	//for (j = -2400; j <= 2400; j++) {
	//	VParamTmp.Data[Z_TOP_LISE] = VParamGlobMin.Data[Z_TOP_LISE] / Z_SCALE;
	//	dDelta_z = j * 0.01 * dFringePeriod;   dDepth = VParamGlobMin.Data[DEPTH_LISE].real() + dResult_delta_z_bot_fringes + dDelta_z;
	//	VParamTmp.Data[DEPTH_LISE] = dDepth / Z_SCALE;
	//	VParamTmp.Data[D_TOP_LISE] = VParamGlobMin.Data[D_TOP_LISE].real();   VParamTmp.Data[D_BOT_LISE] = VParamGlobMin.Data[D_BOT_LISE].imag();
	//	vCalcDeltaIntAndDerivDLSFourierCone(pInfoIntensity, pInfoSpectrum, NULL, VParamTmp, &VResult, NULL, NULL, &bSucMerit); 
	//	dNorm = sqrt(2.0) * VResult.norm();
	//	if (bSucMerit)
	//		ofMeritFu << j << "\t\t" << dDepth << "\t\t" << dNorm << "\t\t" << dNorm * dNorm << endl;
	//}

	delete[] pGlobMin;  pGlobMin = NULL;
	return true;
}


//################################################################### Olovia band pass filter ###################################################################



void vSmoothenSpectrum(struct sCalcDeltaIntSq *p)
{
	int i;
	//double  dkMid = 2.0 * 2.0 * PI / p->dLambdaMid;
	cppc cExp;

	for (i = p->ikStart; i <= p->ikStop; i++) {
		cExp = exp(cIm * p->pdVk->Data[i] * p->dzTop);
		p->pVSpectrum->Data[i] *= cExp;
	}
	/*vChebyFit((p->dkStart + p->dkStop) / 2.0, (p->dkStop - p->dkStart) / 2.0, 30, p->pdVk->Data, *p->pVSpectrum, true); //a non-essential smoothening step
	cExp = exp(-cIm * dkMid * dDelta_z);
	for (i = p->ikStart; i <= p->ikStop; i++) {
		p->pVSpectrum->Data[i] *= cExp;
		p->pVSpectrum->Data[p->iNoFFTPoints - i] = conj(p->pVSpectrum->Data[i]);
	}*/
}


void vGenerateTSV(struct sCalcDeltaIntSq *p)
{
	int i;
	cppc cExp, cTmp;

	for (i = p->ikStart; i <= p->ikStop; i++) {
		cExp = exp(-cIm * p->pdVk->Data[i] * 13.1);
		cTmp = p->pVSpectrum->Data[i] * cExp;
		p->pVSpectrum->Data[i] += cTmp;
	}
	for (i = 1; i < (int)p->pVSpectrum->Nown / 2; i++)
		p->pVSpectrum->Data[p->pVSpectrum->Nown - i] = conj(p->pVSpectrum->Data[i]); //the spectrum always has this symmetry, since the intensity is real
}



void vBandPass(struct sCalcDeltaIntSq *p, struct sCalcDeltaIntSq *pDispersionData, bool bPlanarSubstrate)
{
	pDispersionData = NULL;
	int i;
#ifdef OLOVIA_DEBUG
	ofstream ofIntApodis("./results_1DTM/ofIntApodis.txt"), ofSpec("./results_1DTM/ofSpec.txt"), ofFiltered("./results_1DTM/ofFiltered.txt");
#endif 

	//0.) Set k-values inside tranmission window of filter: 
	p->pdVkWindow = new dVector[1];  p->pdVkWindow->Init(p->ikStop - p->ikStart + 1);
	for (i = p->ikStart; i <= p->ikStop; i++)
		p->pdVkWindow->Data[i - p->ikStart] = i * 2.0 * PI / p->dPeriod;
	p->dDelta_k = 2.0 * PI / p->dPeriod;
	//1.) Apodisation of intensity pattern: 
	vApodisOfIntensity(p, bPlanarSubstrate);
#ifdef OLOVIA_DEBUG
	for (i = 0; i < p->iNoPoints; i++) //Debug Info
		ofIntApodis << i << "\t" << p->pzMeasured[i] << "\t\t" << p->pIntMeasured[i] << endl;
#endif 
	//2.) Calculate spectrum + dispersion + dispersion compensation + do apodisation 
	vFraction_of_FT_of_pattern(p);
	vApodisOfSpectrum(p);
	//if (bPlanarSubstrate == false)
	//	vGenerateTSV(p); 
	//vSmoothenSpectrum(p); 
	//dispersion:
	/*if (pDispersionData == NULL) {
		vGetDispersion(p);
		vCancelDispersion(p, p);
	}
	else
		vCancelDispersion(p, pDispersionData);  */
		//end dispersion  
#ifdef OLOVIA_DEBUG 
	for (i = 0; i < (int)p->pVSpectrum->Nown; i++) //Debug Info 
		ofSpec << i << "\t" << p->pdVk->Data[i] << "\t\t" << p->pVSpectrum->Data[i].real() << "\t" << p->pVSpectrum->Data[i].imag() << endl;
#endif 
	delete[] p->pIntMeasured;  p->pIntMeasured = new double[p->pVSpectrum->Nown];  delete[] p->pzMeasured;  p->pzMeasured = new double[p->pVSpectrum->Nown];
	p->iNoPoints = p->pVSpectrum->Nown;
	//3.) Inverse FFT => smoothened pattern on equidist. mesh  
	fFFT_cppc(TRUE, p->pVSpectrum->Nown, p->pVSpectrum->Data, NULL, NULL, p->pIntMeasured, FALSE);
	for (i = 0; i < (int)p->pVSpectrum->Nown; i++)
		p->pzMeasured[i] = i * p->dDelta_z + p->dOrigin_z_Glob;
#ifdef OLOVIA_DEBUG 
	for (i = 0; i < p->iNoPoints; i++) //Debug Info 
		ofFiltered << i << "\t" << p->pzMeasured[i] << "\t\t" << p->pIntMeasured[i] << endl;
#endif  
	//4.) Optimiz const. prefactor of spectrum: 
	/*double dDeltaIntSq;  Vector VParam1D(1), VGrad1D(1), VParamMin1D, VParamMax1D;  bool bOptParam1D = true, bSuccess1D, bDoRestart1D;
	VParam1D = 1.0;  VGrad1D = -1.0;  VParamMin1D = 1.0;  VParamMax1D = 1.1;
	*p->VSpectrum *= VParam1D.Data[AMPL_LISE];  VParam1D.Data[AMPL_LISE] = 1.0; //reset optimiz. parameter
	//5.) Quality check of result:
	dDeltaIntSq = dOptSpectrum1ParamMerit((void*)p, VParam1D, &bSuccess1D); */
	//5. calc center frequency 
	vCalc_LambdaMid_OfLiseSignal(p);

	p->pVSpecWindow = new Vector[1];  p->pVSpecWindow->Init(p->ikStop - p->ikStart + 1);
	for (i = 0; i < (int)p->pVSpecWindow->Nown; i++)
		p->pVSpecWindow->Data[i] = p->pVSpectrum->Data[p->ikStart + i];
}


void vBandPassForWeights(struct sCalcDeltaIntSq *p)
{
	int i;
	//0.) Embed data to Fourier-interpolate into a larger array of zeros: 
	dVector dVkWindow(p->pdVkWindow->Nown + 2), dV_weight_re(p->pdVkWindow->Nown + 2), dV_weight_im(p->pdVkWindow->Nown + 2);
	dVkWindow.Data[0] = p->pdVkWindow->Data[0] - p->dDelta_k;   dV_weight_re.Data[0] = 0.0;  dV_weight_im.Data[0] = 0.0;
	for (i = 0; i < (int)p->pdVkWindow->Nown; i++) {
		dVkWindow.Data[1 + i] = p->pdVkWindow->Data[i];
		dV_weight_re.Data[1 + i] = p->pVSpectrumSmoothened->Data[i].real();   dV_weight_im.Data[1 + i] = p->pVSpectrumSmoothened->Data[i].imag();
	}
	dVkWindow.Data[1 + i] = dVkWindow.Data[i] + p->dDelta_k;  dV_weight_re.Data[1 + i] = 0.0;   dV_weight_im.Data[1 + i] = 0.0;

	//1.) Calculate spectrum 
	int iDim = dV_weight_re.Nown;  int iDimHalf = (int)(iDim / 2.0 + 0.25);
	double dPeriod = iDim * p->dDelta_k;   dVector dVk(1);  Vector VSpectrumRe(1), VSpectrumIm(1);
	double dOrigin_z_glob = dVkWindow.Data[0] - 0.5 * p->dDelta_k;
	//fFFT_cppc(FALSE, iDim, NULL, p->pdV_weight->Data, NULL, p->pdV_weight->Data, FALSE);
	vFraction_of_analyt_FT_of_pattern(dOrigin_z_glob, dOrigin_z_glob + dPeriod, -iDimHalf, iDim - iDimHalf - 1, dPeriod, iDim, dVkWindow.Data,
		dOrigin_z_glob, dV_weight_re.Data, &dVk, &VSpectrumRe); //take pdVkWindow or pdVk here? 
	vFraction_of_analyt_FT_of_pattern(dOrigin_z_glob, dOrigin_z_glob + dPeriod, -iDimHalf, iDim - iDimHalf - 1, dPeriod, iDim, dVkWindow.Data,
		dOrigin_z_glob, dV_weight_im.Data, &dVk, &VSpectrumIm); //take pdVkWindow or pdVk here? 

	//2.) Cut away  noise: 
	double dDelta_k = 2.0 * PI / dPeriod;
	double dkStartWindow = 1.0 * (-0.5) * iDim * dDelta_k;  double dkWindowLeft = 0.0 * (-0.5) * iDim * dDelta_k;
	double dkStopWindow = -dkStartWindow;  double dkWindowRight = -dkWindowLeft;
	vApodisation(dkStartWindow, dkStopWindow, dkWindowLeft, dkWindowRight, iDim, dVk.Data, VSpectrumRe.Data);
	vApodisation(dkStartWindow, dkStopWindow, dkWindowLeft, dkWindowRight, iDim, dVk.Data, VSpectrumIm.Data);

	//3.) Inverse FFT => smoothened pattern on equidist. mesh  
	//fFFT_cppc(TRUE, p->pVSpectrum->Nown, p->pVSpectrum->Data, NULL, NULL, p->pIntMeasured, FALSE); 
	vInv_analyt_FT(dVk, VSpectrumRe, dVkWindow, dOrigin_z_glob, dV_weight_re.Data);
	vInv_analyt_FT(dVk, VSpectrumIm, dVkWindow, dOrigin_z_glob, dV_weight_im.Data);

	//4.) Copy data into original array: 
#ifdef OLOVIA_DEBUG 
	ofstream ofOut("vBandPassForWeights.txt");
	for (i = 0; i < (int)p->pdVkWindow->Nown; i++) {
		ofOut << i << "\t" << p->pdVkWindow->Data[i] << "\t\t" << p->pVSpectrumSmoothened->Data[i].real() << "\t\t" << p->pVSpectrumSmoothened->Data[i].imag() << "\t\t";
		p->pdVkWindow->Data[i] = dVkWindow.Data[1 + i];
		ofOut << p->pdVkWindow->Data[i] << "\t\t" << p->pVSpectrumSmoothened->Data[i].real() << "\t\t" << p->pVSpectrumSmoothened->Data[i].imag() << endl;
	}
#endif 
}


//################################################################### Olovia error estimates ###################################################################


void vWeightedAverageAndStdErr(Vector& VStdErr, Vector& VParam, cppc& cAverage, double& dStdDev)
{
	if (VParam.Nown <= 1) {
		cAverage = VParam.Data[0], dStdDev = abs(VStdErr.Data[0]);  return;
	}
	cAverage = 0.0;  dStdDev = 0.0;

	int j;
	double dSumOfWeights = 0.0, dSq1, dSq2, dVar = 0.0;
	dVector dVWeight(VStdErr.Nown); //variance

	for (j = 0; j < VParam.Nown; j++) {
		dVWeight.Data[j] = 1.0 / abs(VStdErr.Data[j]);  dVWeight.Data[j] *= dVWeight.Data[j];
		dSumOfWeights += dVWeight.Data[j];
	}
	dVWeight /= dSumOfWeights;
	for (j = 0; j < VParam.Nown; j++)
		cAverage += dVWeight.Data[j] * VParam.Data[j];
	for (j = 0; j < VParam.Nown; j++) {
		dSq1 = abs(VParam.Data[j] - cAverage);  dSq1 *= dSq1;
		dSq2 = abs(VStdErr.Data[j]);  dSq2 *= dSq2;
		dVar += dVWeight.Data[j] * (dSq1 + dSq2);
	}
	dStdDev = sqrt(dVar / (VParam.Nown - 1.0));
}

//#####################################################  Spectroscopic reflectometry  #####################################################

bool bReadSingleAvantesFile(char szFileName[], string &sComment, double &dIntegrationTime, int &iNoAverages, double &dSmoothingNoPixels, string &sSpectrometerName,
	int iNoPoints, double *pWavelength_nm, double *pSpectrum)
{
	bool bSuc = true;
	int i, iLineNo = 0;
	char *pCharacter = NULL;
	string strNextLine;
	ifstream  fFile(szFileName);
	if (!fFile.good()) {
		cout << "\n Error! Opening of Input-file failed! \n"; goto END;
	}

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))  goto END;
	pCharacter = (char*)strstr(strNextLine.c_str(), "Integration time [ms]:");
	if (pCharacter == NULL) {
		sComment = strNextLine; //the first line contains a comment
		if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))	goto END;
		pCharacter = (char*)strstr(strNextLine.c_str(), "Integration time [ms]:");
		if (pCharacter == NULL)	  goto END;
	}
	else
		sComment = "";
	pCharacter += 22;
	if (sscanf_s(pCharacter, "%lg", &dIntegrationTime) != 1)   goto END;

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))	goto END;
	pCharacter = (char*)strstr(strNextLine.c_str(), "Averaging Nr. [scans]:");
	if (pCharacter == NULL)	goto END;
	pCharacter += 22;  if (sscanf_s(pCharacter, "%d", &iNoAverages) != 1) goto END;

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))	goto END;
	pCharacter = (char*)strstr(strNextLine.c_str(), "Smoothing Nr. [pixels]:");
	if (pCharacter == NULL)	  goto END;
	pCharacter += 23;   if (sscanf_s(pCharacter, "%lg", &dSmoothingNoPixels) != 1)	 goto END;

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))	goto END;
	pCharacter = (char*)strstr(strNextLine.c_str(), "Data measured with spectrometer [name]:");
	if (pCharacter == NULL)	 goto END;
	pCharacter += 39;
	sSpectrometerName = pCharacter;

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))	goto END;
	if (strstr(strNextLine.c_str(), "Wave") == NULL)	goto END;

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))	goto END;
	if (strstr(strNextLine.c_str(), "[nm]") == NULL)	goto END;

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo)) {
		cout << endl << "File read error in data line i = 0" << endl;   goto END;
	}
	if (sscanf_s(strNextLine.c_str(), "%lg;  %lg", pWavelength_nm, pSpectrum) != 2)	 bSuc = false;

	for (i = 0; i < iNoPoints; i++) {
		if (i > 0 || bSuc == false) {
			if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo)) {
				cout << endl << "File read error in data line i = " << i << endl;   goto END;
			}
			if (sscanf_s(strNextLine.c_str(), "%lg;  %lg", pWavelength_nm + i, pSpectrum + i) != 2) {
				cout << endl << "Numbers in data line i = " << i << " cannot be read completely. " << endl;    goto END;
			}
		}
	}//end for 

	if (pWavelength_nm[0] > 420.0) {
		cout << endl << "Input file has wrong format or file read error! Abandonment!" << endl;  goto END;
	}
	for (i = 1; i < iNoPoints; i++) {
		if (pWavelength_nm[i] <= pWavelength_nm[i - 1]) {
			cout << endl << "Input file has wrong format or file read error! Abandonment!" << endl;  goto END;
		}
	}
	if (pWavelength_nm[iNoPoints - 1] < 890.0) {
		cout << endl << "Input file has wrong format or file read error! Abandonment!" << endl;  goto END;
	}

	return true;
END:
	cout << endl << "Error in function bReadSingleAvantesFile()! \n Abandonment!" << endl;	return FALSE;
}


bool bRead_Dark_Reference_Sample_And_Summarize_In_One_File(char szFileNameDark[], char szFileNameReference[], char szFileNameSample[],
	char szOutputFileName[], int iNoPoints, double *pWavelength_nm, double *pDark, double *pReference, double *pSpectrum)
{
	int i, iNoAverages[3];
	double dIntegrationTimeDark, dIntegrationTimeReference, dIntegrationTimeSample, dSmoothingNoPixels[3], dMultiplierDark, dMultiplierReference, dMultiplierSample, dMin;
	string strNextLine, sComment_0, sComment_1, sComment_2, sSpectroName_0, sSpectroName_1, sSpectroName_2;
	ofstream ofAvantes(szOutputFileName); //for debug output: output and check the data 
	if (!ofAvantes.good()) {
		cout << "\n Error! Opening of Input-file failed! \n"; goto END;
	}
	if (bReadSingleAvantesFile(szFileNameSample, sComment_0, dIntegrationTimeSample, iNoAverages[0], dSmoothingNoPixels[0], sSpectroName_0, iNoPoints, pWavelength_nm, pSpectrum) == false ||
		bReadSingleAvantesFile(szFileNameDark, sComment_1, dIntegrationTimeDark, iNoAverages[1], dSmoothingNoPixels[1], sSpectroName_1, iNoPoints, pWavelength_nm, pDark) == false ||
		bReadSingleAvantesFile(szFileNameReference, sComment_2, dIntegrationTimeReference, iNoAverages[2], dSmoothingNoPixels[2], sSpectroName_2, iNoPoints, pWavelength_nm, pReference) == false)
		goto END;

	//Applied format: sample, dark, reference: 
	ofAvantes << "# " << sComment_0 << endl;
	ofAvantes << "#Integration time [ms]: " << dIntegrationTimeSample << endl;
	ofAvantes << "#Averaging Nr. [scans]: " << iNoAverages[0] << "\t" << iNoAverages[1] << "\t" << iNoAverages[2] << endl;
	ofAvantes << "#Smoothing Nr. [pixels]: " << dSmoothingNoPixels[0] << "\t" << dSmoothingNoPixels[1] << "\t" << dSmoothingNoPixels[2] << endl;
	ofAvantes << "#Data measured with spectrometer [name]: " << sSpectroName_0 << "\t" << sSpectroName_1 << "\t" << sSpectroName_2 << endl;
	ofAvantes << "#Wave    Sample    Dark      Reference " << endl;
	ofAvantes << "#[nm]    [counts]  [counts]  [counts] " << endl;
	ofAvantes << "#" << endl;

	//Applied format: wavelength/nm, sample, dark, reference: 
	dMin = min(dIntegrationTimeSample, min(dIntegrationTimeDark, dIntegrationTimeReference));
	dMultiplierDark = dMin / dIntegrationTimeDark, dMultiplierReference = dMin / dIntegrationTimeReference;  dMultiplierSample = dMin / dIntegrationTimeSample;
	for (i = 0; i < iNoPoints; i++)
		ofAvantes << pWavelength_nm[i] << "\t\t" << pSpectrum[i] * dMultiplierSample << "\t\t" << pDark[i] * dMultiplierDark << "\t\t" << pReference[i] * dMultiplierReference << endl;

	return true;
END:
	cout << endl << "Error in function bReadLiseFile()! \n Abandonment!" << endl;	return FALSE;
}

BOOL bReadNextLine_ASCII(ifstream* pfFile, string* pstrBuffer, int* pLineNo)
//Read the next non-empty line from file *pflFile in ASCII-Format into *pstrBuffer. *pLineNo: number of line currently read
//(needed for error-handling). This Function returns FALSE in case of failure, TRUE in case of success.	 W.Iff, April 2011
{
	char aszIsEmpty[32767]; //for checking if the read line is empty.
	int iReturnValue; //for checking if the read line is empty.

	if (pfFile == NULL || pstrBuffer == NULL || pLineNo == NULL)
		return FALSE;
	while (!pfFile->eof())
	{
		getline(*pfFile, *pstrBuffer, '\n'); (*pLineNo)++;
		iReturnValue = sscanf_s(pstrBuffer->c_str(), "%s", aszIsEmpty, (unsigned int) sizeof(aszIsEmpty)); //detect empty lines and skip them
		if (iReturnValue > 0 && iReturnValue != EOF) //if (next) non-empty line (line contains more than blanks and tabs)
			return TRUE;
	}
	return FALSE;
	//fprintf(Fres, "\n Error in function bReadNextLine_ASCII()! Reading of line no. %d failed! \n", *pLineNo);  return FALSE;
}

bool bReadNextLine(ifstream* pfFile, string* pstr, const char** ppc, int* pLineNo)
{
	const char* pc = NULL;
	if (bReadNextLine_ASCII(pfFile, pstr, pLineNo) != TRUE)
		return false;
	pc = pstr->c_str();
	while (pc[0] == ' ' || pc[0] == '\t')
		pc++;
	*ppc = pc;
	return true;
}



BOOL bReadNoLines(ifstream* pfFile, int* pNoLines, int* pLineNo)
{
	string strBuffer;
	unsigned int iPos, iPos2;

	if (!bReadNextLine_ASCII(pfFile, &strBuffer, pLineNo))
		return FALSE;
	iPos = (int)strBuffer.find("#", 0); //find key word "#"
	if (iPos == strBuffer.npos)
		return FALSE;
	if (sscanf_s(strBuffer.c_str() + iPos + 1, "%d", pNoLines) != 1) //read no. of lines / data points given
		return FALSE;
	iPos2 = (int)strBuffer.find("&", 0); //find key word "&"
	if (iPos2 < iPos + 3)
		return FALSE;

	return TRUE;
}

bool bReadAvantesFile(const char* szFileName, int iNoPoints, double *pWavelength_nm, double *pDark, double *pReference, double *pSpectrum)
{
	bool bSuc;
	int i, iLineNo = 0;
	string strNextLine;
#ifdef OLOVIA_DEBUG
	ofstream ofAvantes("ofAvantesTextFile.txt"); //for debug output: output and check the data 
#endif 
	ifstream  fFile(szFileName);
	if (!fFile.good()) {
		cout << "\n Error! Opening of Input-file failed! \n"; goto END;
	}

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo)) //the first line contains the comments 
		return false;
	if (strstr(strNextLine.c_str(), "Integration time [ms]:") == NULL) {
		if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
			return false;
	}
	if (strstr(strNextLine.c_str(), "Integration time [ms]:") == NULL)
		goto END;
	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
		return false;
	if (strstr(strNextLine.c_str(), "Averaging Nr. [scans]:") == NULL)
		goto END;
	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
		return false;
	if (strstr(strNextLine.c_str(), "Smoothing Nr. [pixels]:") == NULL)
		goto END;
	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
		return false;
	if (strstr(strNextLine.c_str(), "Data measured with spectrometer [name]:") == NULL)
		goto END;
	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
		return false;
	if (strstr(strNextLine.c_str(), "Wave") == NULL)
		goto END;
	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo))
		return false;
	if (strstr(strNextLine.c_str(), "[nm]") == NULL)
		goto END;

	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo)) {
		cout << endl << "File read error in data line i = 0" << endl;   return false;
	}
	if (sscanf_s(strNextLine.c_str(), "%lg %lg %lg %lg", pWavelength_nm, pSpectrum, pDark, pReference) == 4)
		bSuc = true;
	else
		bSuc = false;

	for (i = 0; i < iNoPoints; i++) {
		if (i > 0 || bSuc == false) {
			if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo)) {
				cout << endl << "File read error in data line i = " << i << endl;   return false;
			}
			if (sscanf_s(strNextLine.c_str(), "%lg %lg %lg %lg", pWavelength_nm + i, pSpectrum + i, pDark + i, pReference + i) != 4) {
				cout << endl << "Numbers in data line i = " << i << " cannot be read completely. " << endl;   return false;
			}
		}
	}

	if (pWavelength_nm[0] > 420.0) {
		cout << endl << "Input file has wrong format or file read error! Abandonment!" << endl;  goto END;
	}
	for (i = 1; i < iNoPoints; i++) {
		if (pWavelength_nm[i] <= pWavelength_nm[i - 1]) {
			cout << endl << "Input file has wrong format or file read error! Abandonment!" << endl;  goto END;
		}
	}
	if (pWavelength_nm[iNoPoints - 1] < 890.0) {
		cout << endl << "Input file has wrong format or file read error! Abandonment!" << endl;  goto END;
	}

#ifdef OLOVIA_DEBUG
	ofAvantes << "# i \t wavelength \t sample \t dark  \t\t reference" << endl; //Debug output: output and check the data
	for (i = 0; i < iNoPoints; i++)
		ofAvantes << i << "\t" << pWavelength_nm[i] << "\t\t" << pSpectrum[i] << "\t\t" << pDark[i] << "\t\t" << pReference[i] << "\t\t" << endl;
#endif 

	return true;

END:
	cout << endl << "Error in function bReadLiseFile()! \n Abandonment!" << endl;	return false;
}





double dInterpolateDiscreteFu(int iNoPoints, double *pz, double *pdf, int iInterpol_Order, double dz_Sample)
{
	//Function for the interpolation of a discrete function pdf(z). The z-values must be in ascending order. Interpolation order: 1 (linear) or 2 (quadratic)   
	//W. Iff, 18.10.2021 
	if (iNoPoints < 1 || pz == NULL || pdf == NULL)
		return DBL_MAX;
	if (iInterpol_Order < 1 || iNoPoints < 2)
		return pdf[0];
	double dDelta_z = (pz[iNoPoints - 1] - pz[0]) / ((double)iNoPoints - 1.0);
	int j = (int)((dz_Sample - pz[0]) / dDelta_z);
	if (iInterpol_Order < 2 || iNoPoints < 3) //interpolate linearly between adjacent neighbours: 
		return pdf[j] + (dz_Sample - pz[j]) * (pdf[j + 1] - pdf[j]) / (pz[j + 1] - pz[j]);
	//Interpolate quadratically (do best parabolic fit of 3 or 4 given data points):  
	double df_Sample;
	if (j > 0)
		j--;
	bParabolicFit_4(dz_Sample, j, iNoPoints - 1, pz + j, pdf + j, &df_Sample, NULL, NULL);
	return df_Sample;
}



void vInterpolateSpectrum(int iNoPoints, double *pzMeasured, double *pIntMeasured, int iNoFFTPoints, double dSamplingOffset, double dDelta_k_Apodis_interval,
	double dEmbeddingfactor, int iInterpol_Order, double *pkEquidist1, double *pIntEqudist1, double &dkMax, double& dDelta_k_Equidist)
	//Function for the interpolation of the spectrometer values. 1st line: input arguments; 2nd line: output arguments.  W. Iff, 20.5.2021  
{
	int i1, i, j1 = -1;
	double dkStart = pzMeasured[0] - dDelta_k_Apodis_interval; // dApodisInterval * (pzMeasured[1] - pzMeasured[0]);
	double dkStop = pzMeasured[iNoPoints - 1] + dDelta_k_Apodis_interval; // dApodisInterval * (pzMeasured[iNoPoints - 1] - pzMeasured[iNoPoints - 2])

	dkMax = dkStop * dEmbeddingfactor;		dDelta_k_Equidist = dkMax / (double)(iNoFFTPoints);

	for (i1 = 0; i1 < iNoFFTPoints; i1++)
		pkEquidist1[i1] = (i1 + dSamplingOffset) * dDelta_k_Equidist;
	i1 = (int)(dkStart / dDelta_k_Equidist - dSamplingOffset); i1++;
	for (i = 0; i < i1; i++)
		pIntEqudist1[i] = 0.0;
	while (pkEquidist1[i1] <= pzMeasured[0]) {
		pIntEqudist1[i1] = pIntMeasured[0] * (pkEquidist1[i1] - dkStart) / (pzMeasured[0] - dkStart);
		i1++;
	}
	if (iInterpol_Order == 1) { //interpolate linearly between adjacent neighbours: 
		while (pkEquidist1[i1] < pzMeasured[iNoPoints - 1]) {
			while (pzMeasured[j1 + 1] <= pkEquidist1[i1])
				j1++;
			pIntEqudist1[i1] = pIntMeasured[j1] + (pkEquidist1[i1] - pzMeasured[j1]) * (pIntMeasured[j1 + 1] - pIntMeasured[j1]) / (pzMeasured[j1 + 1] - pzMeasured[j1]);
			i1++;
		}
	}
	else {  //interpolate quadratically (do best parabolic fit of 4 given data points):  
		while (pkEquidist1[i1] < pzMeasured[iNoPoints - 1]) {
			while (pzMeasured[j1 + 2] < pkEquidist1[i1])
				j1++;
			bParabolicFit_4(pkEquidist1[i1], j1, iNoPoints - 1, pzMeasured + j1, pIntMeasured + j1, pIntEqudist1 + i1, NULL, NULL);
			i1++;
		}
	}
	while (pkEquidist1[i1] < dkStop) {
		pIntEqudist1[i1] = pIntMeasured[iNoPoints - 1] * (dkStop - pkEquidist1[i1]) / (dkStop - pzMeasured[iNoPoints - 1]);
		i1++;
	}
	for (i = i1; i < iNoFFTPoints; i++)
		pIntEqudist1[i] = 0.0;
}



void vInterpol_of_order_1_or_2(int iNoPoints, double *pzMeasured, double *pIntMeasured, int iInterpol_Order, int iDimInterpol, double *pz, double *pResult)
//Function for the interpolation of 1st and 2nd order.  W. Iff, 15.5.2022  
{
	int i1 = 0, j1 = -1;
	double dFirstDeriv;

	if (iInterpol_Order < 0)
		iInterpol_Order = 0;
	if (iInterpol_Order > 2)
		iInterpol_Order = 2;

	if (iNoPoints <= 0) {
		for (i1 = 0; i1 < iDimInterpol; i1++)  pResult[i1] = 0.0;  return;
	}
	else if (iNoPoints == 1 || iInterpol_Order == 0) {
		for (i1 = 0; i1 < iDimInterpol; i1++)  pResult[i1] = pIntMeasured[0];  return;
	}
	else if (iNoPoints == 2)
		iInterpol_Order = 1;

	/*	if (iInterpol_Order == 1)
			dFirstDeriv = (pIntMeasured[1] - pIntMeasured[0]) / (pzMeasured[1] - pzMeasured[0]);
		else
			bParabolicFit_4(pzMeasured[0], -1, iNoPoints - 1, pzMeasured, pIntMeasured, NULL, &dFirstDeriv, NULL);  */
	dFirstDeriv = 0.0;
	while (pz[i1] <= pzMeasured[0] && i1 < iDimInterpol) {
		pResult[i1] = pIntMeasured[0] + dFirstDeriv * (pz[i1] - pzMeasured[0]);
		i1++;
	}

	if (iInterpol_Order == 1) { //interpolate linearly between adjacent neighbours: 
		while (pz[i1] < pzMeasured[iNoPoints - 1] && i1 < iDimInterpol) {
			while (pzMeasured[j1 + 1] < pz[i1])
				j1++;
			pResult[i1] = pIntMeasured[j1] + (pz[i1] - pzMeasured[j1]) * (pIntMeasured[j1 + 1] - pIntMeasured[j1]) / (pzMeasured[j1 + 1] - pzMeasured[j1]);
			i1++;
		}
	}//end if lin. interpol.
	else {  //interpolate quadratically (do best parabolic fit of 4 given data points):  
		while (pz[i1] < pzMeasured[iNoPoints - 1] && i1 < iDimInterpol) {
			while (pzMeasured[j1 + 2] < pz[i1])
				j1++;
			bParabolicFit_4(pz[i1], j1, iNoPoints - 1, pzMeasured + j1, pIntMeasured + j1, pResult + i1, NULL, NULL);
			i1++;
		}
	}//end else quadratic. interpol.

/*	if (iInterpol_Order == 1)
		dFirstDeriv = (pIntMeasured[iNoPoints - 1] - pIntMeasured[iNoPoints - 2]) / (pzMeasured[iNoPoints - 1] - pzMeasured[iNoPoints - 2]);
	else {
		j1 = iNoPoints - 3;
		bParabolicFit_4(pzMeasured[iNoPoints - 1], j1, iNoPoints - 1, pzMeasured + j1, pIntMeasured + j1, NULL, &dFirstDeriv, NULL);
	}  */
	dFirstDeriv = 0.0;
	while (i1 < iDimInterpol) {
		pResult[i1] = pIntMeasured[iNoPoints - 1] + dFirstDeriv * (pz[i1] - pzMeasured[iNoPoints - 1]);
		i1++;
	}
}



void vFFT_of_Spectrum_4thOrder(int iNoPoints, double *pk_Spectrum, double *pSpectrum, int iNoFFTPoints, double dDelta_k_Apodis_interval, double dEmbeddingFactor,
	double &dDelta_k_Equidist, cppc *pFT_Spectrum, double *pAbs_FT_Spectrum, double &dkMax, double &dDelta_z, double *pz)
	//This function does the FFT of the non-equidistant spectrometer data, which have to be interpolated first. The Fourier transform is then done 
	//by the sum of 2 FFTs on 2 grids, resulting ideally in 4th-order convergence of the Fourier transform (Gaussian integration scheme of 4th order). W. Iff, 20.5.2021 
{
	int i;
	double *pkEquidist1 = new double[iNoFFTPoints], *pkEquidist2 = new double[iNoFFTPoints],
		*pIntEqudist1 = new double[iNoFFTPoints], *pIntEqudist2 = new double[iNoFFTPoints];
	double dSamplingPos1 = 0.2113248654, dSamplingPos2 = 0.7886751346;

	//interpolation:
	vInterpolateSpectrum(iNoPoints, pk_Spectrum, pSpectrum, iNoFFTPoints, dSamplingPos1, dDelta_k_Apodis_interval, dEmbeddingFactor, 2, pkEquidist1, pIntEqudist1, dkMax, dDelta_k_Equidist);
	vInterpolateSpectrum(iNoPoints, pk_Spectrum, pSpectrum, iNoFFTPoints, dSamplingPos2, dDelta_k_Apodis_interval, dEmbeddingFactor, 2, pkEquidist2, pIntEqudist2, dkMax, dDelta_k_Equidist);

	//FFT: 
	double dkSift1 = dSamplingPos1 * dDelta_k_Equidist, dkSift2 = dSamplingPos2 * dDelta_k_Equidist;
	cppc *pcFFT1 = new cppc[iNoFFTPoints], *pcFFT2 = new cppc[iNoFFTPoints];

	fFFT_cppc(FALSE, iNoFFTPoints, NULL, pIntEqudist1, pcFFT1, NULL, FALSE);
	fFFT_cppc(FALSE, iNoFFTPoints, NULL, pIntEqudist2, pcFFT2, NULL, FALSE);
	for (i = 0; i < iNoFFTPoints; i++) {
		pcFFT1[i] *= exp(-cIm * dDelta_k_Equidist*dkSift1);  pcFFT2[i] *= exp(-cIm * dDelta_k_Equidist*dkSift2);
		pFT_Spectrum[i] = 0.5 * (pcFFT1[i] + pcFFT2[i]);
	}
	if (pAbs_FT_Spectrum != NULL)
		for (i = 0; i < iNoFFTPoints; i++)
			pAbs_FT_Spectrum[i] = abs(pFT_Spectrum[i]);
	dDelta_z = 2.0 * PI / dkMax;
	if (pz != NULL)
		for (i = 0; i < iNoFFTPoints; i++)
			pz[i] = i * dDelta_z;

	delete[] pkEquidist1;  delete[] pkEquidist2;  delete[] pIntEqudist1;  delete[] pIntEqudist2;  delete[] pcFFT1;  delete[] pcFFT2;
}


void vFFT_of_Spectrum_2ndOrder(int iNoPoints, double *pk_Spectrum, double *pSpectrum, int iNoFFTPoints, bool bDataOnFFTGrid, double dOffset, double dDelta_k_Apodis_interval,
	double dEmbeddingFactor, double dScalingFactor_k, double &dDelta_k_Equidist, cppc *pFT_Spectrum, double *pAbs_FT_Spectrum, double &dkMax, double &dDelta_z, double *pz)
	//This function does the FFT of the non-equidistant spectrometer data, which have to be interpolated first. W. Iff, 16.6.2021 
{
	if (iNoPoints <= 0 || iNoFFTPoints <= 0) {
		dDelta_k_Equidist = dkMax = dDelta_z = 0.0;  return;
	}
	else if (iNoPoints == 1 || iNoFFTPoints == 1) {
		if (pFT_Spectrum != NULL) pFT_Spectrum[0] = 0.0;   if (pAbs_FT_Spectrum != NULL) pAbs_FT_Spectrum[0] = 0.0;
	}
	bool bInterpolation;
	int i;
	double *pkEquidist = new double[iNoFFTPoints], *pIntEqudist = new double[iNoFFTPoints];

	//interpolation:
	if (bDataOnFFTGrid && iNoPoints == iNoFFTPoints || iNoPoints <= 2) {
		bInterpolation = false;   iNoFFTPoints = iNoPoints;
		memcpy(pIntEqudist, pSpectrum, iNoFFTPoints * sizeof(double));   memcpy(pkEquidist, pk_Spectrum, iNoPoints);
		dkMax = pk_Spectrum[iNoFFTPoints - 1] * (double)iNoFFTPoints / (double)(iNoFFTPoints - 1);   dDelta_k_Equidist = dkMax / (double)iNoFFTPoints;
	}
	else {
		bInterpolation = true;
		vInterpolateSpectrum(iNoPoints, pk_Spectrum, pSpectrum, iNoFFTPoints, 0.0, dDelta_k_Apodis_interval, dEmbeddingFactor, 2, pkEquidist, pIntEqudist, dkMax, dDelta_k_Equidist);
#ifdef OLOVIA_DEBUG 
		ofstream ofInterpolatedSpec("InterpolatedSpec.txt"); int nn;
		for (nn = 0; nn < iNoFFTPoints; nn++)
			ofInterpolatedSpec << nn << " \t" << pkEquidist[nn] << " \t" << pIntEqudist[nn] << endl;
#endif 
	}
	//FFT: 
	fFFT_cppc(FALSE, iNoFFTPoints, NULL, pIntEqudist, pFT_Spectrum, NULL, FALSE);   dDelta_z = 2.0 * PI / dkMax;
	if (abs(dOffset) > NUM_NOISE && bInterpolation == false) { //if data on a shifted grid have been given to FFT instead of grid containing the origin, correct this now:
		double dkSift = dOffset * dDelta_k_Equidist;
		for (i = 0; i < iNoFFTPoints; i++)
			pFT_Spectrum[i] *= exp(-cIm * (i*dDelta_z * dkSift));
	}
	if (pAbs_FT_Spectrum != NULL)
		for (i = 0; i < iNoFFTPoints; i++)
			pAbs_FT_Spectrum[i] = abs(pFT_Spectrum[i]);
	dDelta_z *= dScalingFactor_k; //dDelta_z /= 2.0;  //we need the depth, not the optical path length;  later, divide here also by n_eff !   
	if (pz != NULL)
		for (i = 0; i < iNoFFTPoints; i++)
			pz[i] = i * dDelta_z;
#ifdef OLOVIA_DEBUG 
	ofstream of_FT_of_Spectrum_2nd_Order("FT_of_Spectrum_2nd_Order.txt");
	for (int nn = 0; nn < iNoFFTPoints; nn++) {
		of_FT_of_Spectrum_2nd_Order << nn << " \t" << nn * dDelta_z << " \t" << pFT_Spectrum[nn].real() << " \t" << pFT_Spectrum[nn].imag() << " \t"
			<< abs(pFT_Spectrum[nn]) << endl;
	}
#endif 
	delete[] pkEquidist;  delete[] pIntEqudist;
}


void vInv_FFT_of_Spectrum_2ndOrder(int iNoFFTPoints, double dDelta_k_Equidist, cppc *pFFT, bool bDataOnFFTGrid, double dOffset, double dScalingFactor, double dz_Equidist,
	int iNoPoints, double *pz, double *pResult)
	//This function does the FFT of the non-equidistant spectrometer data, which have to be interpolated first. W. Iff, 16.6.2021 
{
	if (iNoPoints <= 0 || iNoFFTPoints <= 0)
		return;

	bool bInterpolation;
	int i;
	double *pIntEqudist = new double[iNoFFTPoints];

	if (bDataOnFFTGrid && iNoPoints == iNoFFTPoints || iNoPoints <= 2)
		bInterpolation = false;
	else
		bInterpolation = true;

	if (abs(dOffset) > NUM_NOISE && bInterpolation == false) { //if data on a shifted grid have been given to FFT instead of grid containing the origin, correct this now:
		double dzSift = dOffset * (pz[1] - pz[0]), dDelta_k = dDelta_k_Equidist / dScalingFactor;
		for (i = 0; i < iNoFFTPoints; i++)
			pFFT[i] *= exp(+cIm * (i*dDelta_k*dzSift));
	}

	fFFT_cppc(TRUE, iNoFFTPoints, pFFT, NULL, NULL, pIntEqudist, FALSE);
#ifdef OLOVIA_DEBUG 
	ofstream inv_FT_of_Spectrum("inv_FT_of_Spectrum.txt");
	for (int nn = 0; nn < iNoFFTPoints; nn++) {
		inv_FT_of_Spectrum << nn << " \t" << pIntEqudist[nn] << endl;
	}
#endif 

	if (bInterpolation) {
		double *pzEquidist = new double[iNoFFTPoints];
		for (i = 0; i < iNoFFTPoints; i++)
			pzEquidist[i] = i * dz_Equidist;
		vInterpol_of_order_1_or_2(iNoFFTPoints, pzEquidist, pIntEqudist, 2, iNoPoints, pz, pResult);
#ifdef OLOVIA_DEBUG 
		ofstream ofInterpolatedSpec("InterpolatedSpec.txt");
		for (int nn = 0; nn < iNoPoints; nn++)
			ofInterpolatedSpec << nn << " \t" << pz[nn] << " \t" << pResult[nn] << endl;
#endif  
		delete[] pzEquidist;
	}
	else
		memcpy(pResult, pIntEqudist, iNoFFTPoints * sizeof(double));
	delete[] pIntEqudist;
}


void vLowpass_Spectrum(int iNoPoints, double *pk_Spectrum, double *pSpectrum, int iNoFFTPoints, bool bDataOnFFTGrid, double dOffset, double dEmbeddingFactor,
	double dWidthGauss_reciprocal_space, bool bNormalizedWidth, double *pResult)
{
	if (dWidthGauss_reciprocal_space >= 1.0 / NUM_NOISE_FLOAT) {
		memcpy(pResult, pSpectrum, iNoPoints * sizeof(double));   return;
	}

	int i;
	double dDelta_k_Equidist, dkMax, dDelta_z, dWeight, dExpCoeff;
	cppc *pFFT = new cppc[iNoFFTPoints];

	vFFT_of_Spectrum_2ndOrder(iNoPoints, pk_Spectrum, pSpectrum, iNoFFTPoints, bDataOnFFTGrid, dOffset, 0.0, dEmbeddingFactor, 1.0, dDelta_k_Equidist, pFFT, NULL,
		dkMax, dDelta_z, NULL);
	if (bNormalizedWidth)
		dExpCoeff = 1.0 / (dWidthGauss_reciprocal_space * dWidthGauss_reciprocal_space);
	else
		dExpCoeff = (dDelta_z * dDelta_z) / (dWidthGauss_reciprocal_space * dWidthGauss_reciprocal_space);
	for (i = 1; i < iNoFFTPoints / 2; i++) {
		dWeight = exp(-(i * i) * dExpCoeff);
		pFFT[i] *= dWeight;   pFFT[iNoFFTPoints - i] *= dWeight;
	}
	pFFT[iNoFFTPoints / 2] = 0;

	vInv_FFT_of_Spectrum_2ndOrder(iNoFFTPoints, dDelta_z, pFFT, bDataOnFFTGrid, dOffset, 1.0, dDelta_k_Equidist, iNoPoints, pk_Spectrum, pResult);
#ifdef OLOVIA_DEBUG 
	ofstream ofLowpass("Lowpass.txt");
	for (i = 0; i < iNoPoints; i++)
		ofLowpass << " \t " << i << " \t " << pk_Spectrum[i] << " \t " << pSpectrum[i] << " \t " << pResult[i] << "\n";
#endif 
	delete[] pFFT;
}


void vApplyHighPass(double dxLeft, double dxRight, int iDim, double *pdx, double *pdInput, double *pdOutput)
{
	int i = 0;
	double dWeight, dkFilter = PI / (dxRight - dxLeft);
	while (pdx[i] < dxLeft && i < iDim) {
		pdOutput[i] = 0;  i++;
	}
	while (pdx[i] < dxRight && i < iDim) {
		dWeight = 0.5 - 0.5 * cos(dkFilter * (pdx[i] - dxLeft));
		pdOutput[i] = dWeight * pdInput[i];  i++;
	}
	while (i < iDim) {
		pdOutput[i] = pdInput[i];  i++;
	}
}



void vApplyLowPass(double dxLeft, double dxRight, int iDim, double *pdx, double *pdInput, double *pdOutput)
{
	int i = 0;
	double dWeight, dkFilter = PI / (dxRight - dxLeft);
	while (pdx[i] < dxLeft && i < iDim) {
		pdOutput[i] = pdInput[i];  i++;
	}
	while (pdx[i] < dxRight && i < iDim) {
		dWeight = 0.5 + 0.5 * cos(dkFilter * (pdx[i] - dxLeft));
		pdOutput[i] = dWeight * pdInput[i];  i++;
	}
	while (i < iDim) {
		pdOutput[i] = 0;  i++;
	}
}



bool bValidateRecordedSpectra(int iNo_pixels, double *pWavelength_nm, double *pDark, double *pReference, double *pSpectrum, 
	double dThreshold_Dark, double dRelative_Threshold, double dMin_Permitted_Power, double dMax_Peritted_Power_Desity, stringstream *pssMessages)
{
	int i; 
	double dSumOfDark = 0.0, dSumOfReference = 0.0, dSumOfSpectrum = 0.0;

	if (pDark == NULL) 
		return false;

	if (pWavelength_nm[0] > 420.0) {
		*pssMessages << "Too large wavelength! Input has wrong format or data readout error! Abandonment!" << endl;  return false;
	}
	for (i = 1; i<iNo_pixels; i++) {
		if (pWavelength_nm[i] <= pWavelength_nm[i - 1]) {
			*pssMessages << "Wavelengths not sorted! Input has wrong format or data readout error! Abandonment!" << endl;  return false;
		}
	}
	if (pWavelength_nm[iNo_pixels - 1] < 900.0) { 
		*pssMessages << "Insufficient wavelength range! Input has wrong format or data readout error! Abandonment!" << endl;  return false;
	}

	for (i = 0; i < iNo_pixels; i++) {
		if (pDark[i] >= dMax_Peritted_Power_Desity) {
			*pssMessages << "Invalid data: Oversaturated Dark spectrometer!" << endl;  return false;
		}
		dSumOfDark += pDark[i]; 
	} 
	if (dSumOfDark < dThreshold_Dark) {
		*pssMessages << "Invalid data: Missing dark spectrum!" << endl;  return false;
	}

	if (pReference != NULL) {
		for (i = 0; i < iNo_pixels; i++) {
			if (pReference[i] >= dMax_Peritted_Power_Desity) {
				*pssMessages << "Invalid data: Oversaturated Reference spectrometer!" << endl;  return false;
			}
			dSumOfReference += pReference[i];
		}//i
		if (dSumOfReference < dRelative_Threshold * dSumOfDark) {
			*pssMessages << "Invalid data: Missing or too small reference spectrum!" << endl;  return false;
		}
		if (dSumOfReference - dSumOfDark < dMin_Permitted_Power) {
			*pssMessages << "Invalid data: Too low power or missing reference spectrum!" << endl;  return false;
		}
	}//end if pReference  

    // Note RTi 12/03/2025
    // in case of normalized signal with intensity factor a spectrum could be larger than dMax_Peritted_Power_Desity
    // check if signal is oversatured will take place outside this calculation DLL
    // So avoid this check in the loop below
    bool SkipOversaturatedSpectrumCheck = true;
	if (pSpectrum != NULL) {
        
        if (SkipOversaturatedSpectrumCheck)
        {
            //  introduce behavior with TTM normalized signal which can be potential above dMax_Peritted_Power_Desity
            for (i = 0; i < iNo_pixels; i++) {
                dSumOfSpectrum += pSpectrum[i];
            }
        }
        else
        {
            // original behavior
            for (i = 0; i < iNo_pixels; i++) {
                if (pSpectrum[i] >= dMax_Peritted_Power_Desity) {
                    *pssMessages << "Invalid data: Oversaturated spectrometer!" << endl;  return false;
                }
                dSumOfSpectrum += pSpectrum[i];
            }
        }
		
		if (dSumOfSpectrum < dRelative_Threshold * dSumOfDark) {
			*pssMessages << "Invalid data: Missing or too small spectrum!" << endl;  return false;
		}
		if (dSumOfSpectrum - dSumOfDark < dMin_Permitted_Power) {
			*pssMessages << "Invalid data: Too low power or missing spectrum!" << endl;  return false; 
		}
	}//end if pSpectrum  

	return true; 
} 



void vPreprocess_spectra(int iNo_pixels, double *pWavelength_nm, double *pDark_spectrum, double *pReference_spectrum_in, double *pSpectrum_in,
	dVector &dVk_spectrum, double *pReference_spectrum_out, double *pSpectrum_out)
{
	int i;
	for (i = 0; i < iNo_pixels; i++) {
		dVk_spectrum.Data[i] = (2.0 * PI) / (0.001 * pWavelength_nm[iNo_pixels - 1 - i]); //we need microns
		pReference_spectrum_out[i] = pReference_spectrum_in[iNo_pixels - 1 - i] - pDark_spectrum[iNo_pixels - 1 - i];
		pSpectrum_out[i] = pSpectrum_in[iNo_pixels - 1 - i] - pDark_spectrum[iNo_pixels - 1 - i];
	}
#ifdef OLOVIA_DEBUG 
	ofstream ofSk("Sk.txt");
	for (i = 0; i < iNo_pixels; i++)
		ofSk << i << " \t" << dVk_spectrum.Data[i] << " \t" << pSpectrum_in[iNo_pixels - 1 - i] << " \t" << pReference_spectrum_in[iNo_pixels - 1 - i]
		<< " \t" << pDark_spectrum[iNo_pixels - 1 - i] << endl;
#endif 
}


void vSmoothenData(double dSigma, int iNo_pixels, double *pk, double *pData_in, double *pData_out)
{
	int i, i1, i2, i3;  const int iFactor = 10;  const int iDim_Half_Gauss = (20 + 5) * iFactor + 1;
	double dExponent, di3_minus_i1, dHalfGauss[iDim_Half_Gauss];

	dHalfGauss[0] = 1.0;
	i1 = 20 * iFactor;
	for (i = 1; i <= i1; i++) {
		dExponent = i / (double)(10 * iFactor);
		dHalfGauss[i] = exp(-dExponent * dExponent);
	}
	i2 = i1 + 1;  i3 = i1 + 5 * iFactor;  di3_minus_i1 = i3 - i1;
	for (i = i2; i < i3; i++) {
		dHalfGauss[i] = dHalfGauss[i1] * (i3 - i) / di3_minus_i1;
	}
	dHalfGauss[i3] = 0.0;
#ifdef OLOVIA_DEBUG
	ofstream ofGauss("Gauss.txt");
	for (i = 0; i <= i3; i++)
		ofGauss << i << "\t" << dHalfGauss[i] << endl;
#endif

	int j, iIndex, djMax, iAdditionalDim;
	double dkj, *pDelta_k = new double[iNo_pixels], dDelta_k_min, dSigmaMax = 2.5 * dSigma, *pSpectrum_in_extended = NULL, *pSpectrum_in2 = NULL, dNorm;

	pDelta_k[0] = pk[1] - pk[0];
	for (i = 1; i <= iNo_pixels - 2; i++)
		pDelta_k[i] = (pk[i + 1] - pk[i - 1]) / 2.0;
	pDelta_k[iNo_pixels - 1] = pk[iNo_pixels - 1] - pk[iNo_pixels - 2];
	dDelta_k_min = min(pDelta_k[0], pDelta_k[iNo_pixels - 1]);   djMax = (int)(dSigmaMax / dDelta_k_min);
	iAdditionalDim = djMax + 1;   pSpectrum_in_extended = new double[iNo_pixels + 2 * iAdditionalDim];
	for (i = 0; i < iAdditionalDim; i++)
		pSpectrum_in_extended[i] = 0.0;
	memcpy(pSpectrum_in_extended + iAdditionalDim, pData_in, iNo_pixels * sizeof(double));
	for (i = iAdditionalDim + iNo_pixels; i < 2 * iAdditionalDim + iNo_pixels; i++)
		pSpectrum_in_extended[i] = 0.0;
	pSpectrum_in2 = pSpectrum_in_extended + iAdditionalDim;
	for (i = 0; i < iNo_pixels; i++) {
		pData_out[i] = pSpectrum_in2[i] * dHalfGauss[0];  dNorm = dHalfGauss[0];    djMax = (int)(dSigmaMax / pDelta_k[i]);
		for (j = 1; j <= djMax; j++) {
			dkj = j * pDelta_k[i];    iIndex = (int)(10 * iFactor * dkj / dSigma + 0.5);
			pData_out[i] += dHalfGauss[iIndex] * (pSpectrum_in2[i + j] + pSpectrum_in2[i - j]);   dNorm += 2.0 * dHalfGauss[iIndex];
		}
		pData_out[i] /= dNorm;
	}//end for i 
	delete[] pDelta_k;  delete[] pSpectrum_in_extended;
#ifdef OLOVIA_DEBUG
	ofstream ofData("SmoothenedData.txt");
	for (i = 0; i < iNo_pixels; i++)
		ofData << i << "\t" << pk[i] << "\t\t" << pData_in[i] << "\t\t" << pData_out[i] << endl;
#endif 
}


void vNormalizeReflection(int iNoPixels, double *pReference, double *pSpectrum, double dRelative_Threshold, double dAbsolute_Threshold, double *pNormalized_Refl)
{
	int i, i1;
	double dAverageReference = 0.0, dThreshold;
	for (i = 0; i < iNoPixels; i++) //calc. threshold 
		dAverageReference += pReference[i];
	dAverageReference /= (double)iNoPixels;
	dRelative_Threshold *= dAverageReference;
	dThreshold = max(dRelative_Threshold, dAbsolute_Threshold);
	i1 = (int)(i * 0.75);
	for (i = i1; i >= 0; i--) //do normalization: 
		if (pReference[i] > dThreshold)
			pNormalized_Refl[i] = pSpectrum[i] / pReference[i];
		else {
			pNormalized_Refl[i] = 0.0;
			i--;  break;
		}
	while (i >= 0)
		pNormalized_Refl[i--] = 0.0;
	for (i = i1 + 1; i < iNoPixels; i++) //normalize: 
		if (pReference[i] > dThreshold)
			pNormalized_Refl[i] = pSpectrum[i] / pReference[i];
		else {
			pNormalized_Refl[i] = 0.0;
			i++;  break;
		}
	while (i < iNoPixels)
		pNormalized_Refl[i++] = 0.0;
}


void vIntegrate_By_Midpoint(double dz_Left, double dz_Right, double dDelta_z, int iDim, double *pz, double *pf, double &dSum)
//W. Iff, 23.6.2021
{
	int i, iIndexLeft, iIndexRight;
	double dfLeft, dfRight, dFractionLeft, fFractionRight, dIncrementLeft, dIncrementRight;
	iIndexLeft = (int)(dz_Left / dDelta_z) + 1;
	iIndexRight = (int)(dz_Right / dDelta_z);
	if (iIndexRight > iDim - 1)
		iIndexRight = iDim - 1;
	dSum = 0.5 * pf[iIndexLeft];
	for (i = iIndexLeft + 1; i < iIndexRight; i++) {
		dSum += pf[i];
	}
	dSum += 0.5 * pf[iIndexRight];
	dSum *= dDelta_z;
	if (iIndexLeft - 1 >= 0) {
		dFractionLeft = (pz[iIndexLeft] - dz_Left) / dDelta_z;     dfLeft = dFractionLeft * pf[iIndexLeft - 1] + (1 - dFractionLeft) * pf[iIndexLeft];
		dIncrementLeft = (dFractionLeft  * dDelta_z) * (dfLeft + pf[iIndexLeft]) / 2.0;      dSum += dIncrementLeft;
	}
	if (iIndexRight + 1 < iDim) {
		fFractionRight = (dz_Right - pz[iIndexRight]) / dDelta_z;   dfRight = fFractionRight * pf[iIndexRight + 1] + (1 - fFractionRight) * pf[iIndexRight];
		dIncrementRight = (fFractionRight * dDelta_z) * (dfRight + pf[iIndexRight]) / 2.0;    dSum += dIncrementRight;
	}
}


void vSignal_Minus_Background(int iDim, double *pz, double *pf, double dAmplitude_Background, double dWeight, double *pf_Minus_Noise)
{
	int i;
	double dBackground;

	pf_Minus_Noise[0] = pf[0];
	for (i = 1; i < iDim; i++) {
		dBackground = dAmplitude_Background / pz[i];
		pf_Minus_Noise[i] = pf[i] - dWeight * dBackground;
	}
#ifdef OLOVIA_DEBUG
	ofstream ofSignal("signal.txt");
	for (i = 0; i < iDim / 2; i++)
		ofSignal << i << " \t" << pz[i] << "  \t" << pf[i] << " \t" << pf_Minus_Noise[i] << endl;
#endif
}



void vFindMaximaOf_f(double dStart, double dStop, double dDelta_z, int iDim, double *pdx, double *pdf,
	int &iNoMax, double **ppPositions_of_peaks, double **ppAmplitudes_of_peaks)
	//Find the local maxima of a function f(x). The maxima may be somewhere between the data points given by pdx, pdf.   W. Iff, 23.6.21 
{
	int i, j, jj, iStart, iStop, iIndexLL, iIndex;
	iVector iVMax(iDim - 1); iVMax = 0; //this vector registers whether there is a maximum in the interval between z_i and z_i+1 

	iStart = (int)(dStart / dDelta_z);   iStop = (int)(dStop / dDelta_z) + 1;
	iStart = max(iStart, 1);   iStop = min(iStop, iDim - 2);
	for (i = iStart; i <= iStop; i++) {
		if (pdf[i] >= pdf[i - 1] && pdf[i] >= pdf[i + 1]) {
			if (pdf[i - 1] > pdf[i + 1]) //is the max on the left or the right? 
				iVMax.Data[i - 1] = 1;
			else //is the max on the left or the right?  
				iVMax.Data[i] = 1;
		}//end if
	}//i  
	iNoMax = 0;
	for (i = iStart; i <= iStop; i++) { //start counting from 1; (the interval on the very left will always contain an uninteresting maximum) 
		if (iVMax.Data[i] > 0)
			iNoMax++;
	}
	iVector iVMax2(iNoMax);
	j = 0;
	for (i = iStart; i <= iStop; i++) {
		if (iVMax.Data[i] > 0) {
			iVMax2.Data[j] = i;  j++;
		}
	}
#ifdef OLOVIA_DEBUG
	ofstream ofMax("max.txt");
	for (j = 0; j < iNoMax; j++) {
		i = iVMax2.Data[j];
		ofMax << j << "\t" << i << "\t" << pdx[i] << "\t\t" << pdx[i + 1] << "\t\t" << pdf[i] << "\t\t" << pdf[i + 1] << endl;
	}
#endif

	dVector dVPositionOfMaxTemp(iNoMax), dVAmplOfMaxTemp(iNoMax);
	double dGradientLeft, dGradientRight, dFractionLeft, fFractionRight, da0, da1, da2, dCorrection_in_z;
	for (j = 0; j < iNoMax; j++) {
		i = iVMax2.Data[j];
		dGradientLeft = (pdf[i + 1] - pdf[i - 1]) / (pdx[i + 1] - pdx[i - 1]);
		dGradientRight = (pdf[i + 2] - pdf[i]) / (pdx[i + 2] - pdx[i]);
		if (dGradientLeft < 0)
			dVPositionOfMaxTemp.Data[j] = pdx[i];
		else if (dGradientRight > 0)
			dVPositionOfMaxTemp.Data[j] = pdx[i + 1];
		else {
			dFractionLeft = dGradientLeft / (dGradientLeft - dGradientRight);   fFractionRight = 1.0 - dFractionLeft;
			dVPositionOfMaxTemp.Data[j] = pdx[i] * fFractionRight + pdx[i + 1] * dFractionLeft;
		}
		iIndexLL = i - 1;
		//bParabolicFit_4( dVPositionOfMaxTemp.Data[i], iIndexLL, iDim - 1, pdx + iIndexLL, pdf + iIndexLL, &da0, NULL, NULL );   dVAmplOfMaxTemp.Data[i] = da0;  
		bParabolicFit_4(dVPositionOfMaxTemp.Data[j], iIndexLL, iDim - 1, pdx + iIndexLL, pdf + iIndexLL, &da0, &da1, &da2);   dVAmplOfMaxTemp.Data[j] = da0;
		if (abs(da2) > NUM_NOISE) {
			dCorrection_in_z = -da1 / (2.0 * da2);
			if (abs(dCorrection_in_z) < dDelta_z) {
				dVPositionOfMaxTemp.Data[j] += dCorrection_in_z;
				dVAmplOfMaxTemp.Data[j] = da2 * dCorrection_in_z * dCorrection_in_z + da1 * dCorrection_in_z + da0;
			}
		}//end if abs 
	}//i 

	*ppAmplitudes_of_peaks = new double[iNoMax], *ppPositions_of_peaks = new double[iNoMax];
	iVector iVTrackingNo(iNoMax);   vQsortData(iNoMax, dVAmplOfMaxTemp.Data, iVTrackingNo.Data);
	for (jj = 0; jj < iNoMax; jj++) {
		iIndex = iNoMax - 1 - jj; // we take the largest entries - they are on the right side of the sorted array 
		(*ppAmplitudes_of_peaks)[jj] = dVAmplOfMaxTemp.Data[iIndex]; // dVAmplOfMaxTemp is now sorted 
		(*ppPositions_of_peaks)[jj] = dVPositionOfMaxTemp.Data[iVTrackingNo.Data[iIndex]]; // dVPositionOfMaxTemp has not been sorted 
	}
}




void vSet_Static_Weights_In_Direct_Space_TSV(int iDim, double dDelta_z, double dzWindowLeft, double dzWindowRight, double dHalf_Width_of_Peak,
	bool bNewPeakDetection, double *pdMeasurement, double dThreshold, int& iGlobal_Start, int& iGlobal_Stop, double *pdWeightsTemp)
{
	bool bSinusoidalWeights = false;  
	int i;   
	double dWidthOfEdgesOfCompWindow = 1.0 * dHalf_Width_of_Peak;  //dWidthOfEdgesOfCompWindow = 0.33 * dHalf_Width_of_Peak;  

	if (bNewPeakDetection)
		dWidthOfEdgesOfCompWindow *= 2.0 / 3.0; 

	//1) Determine computational window for merit function:  
	iGlobal_Start = (int)((dzWindowLeft - dWidthOfEdgesOfCompWindow) / dDelta_z);   
	iGlobal_Stop = (int)((dzWindowRight + dWidthOfEdgesOfCompWindow) / dDelta_z) + 1; 
	//2) Set weights in comp. window to 1:   
	if (bNewPeakDetection) {
		dThreshold = max(dThreshold, NUM_NOISE_FLOAT); 
		if (bSinusoidalWeights) {
			double  dLowerThreshold = 0.0 * dThreshold,  dUpperThreshold = 2.0 * dThreshold; 
			double  dAverage = (dLowerThreshold + dUpperThreshold) / 2.0, dDifference = dUpperThreshold - dLowerThreshold;
			double dk = PI / dDifference; 
			for (i = iGlobal_Start; i <= iGlobal_Stop; i++) {
				if (pdMeasurement[i] <= dLowerThreshold)
					pdWeightsTemp[i] = 0.0;
				else if (pdMeasurement[i] >= dUpperThreshold)
					pdWeightsTemp[i] = 1.0;
				else
					pdWeightsTemp[i] = 0.5 + 0.5 * sin(dk * (pdMeasurement[i] - dAverage));
			}//i
		}//end if sinusoidal weights
		else { //linear weights: 
			double dLowerThreshold = 0.0 * dThreshold, dUpperThreshold = 2.0 * dThreshold; //0.1 * dThreshold, and 1.9 * dThreshold; 
			double dBaseWeight = 0.3;  double dLinWeight = 1.0 - dBaseWeight;
			for (i = iGlobal_Start; i <= iGlobal_Stop; i++) {
				if (pdMeasurement[i] <= dLowerThreshold)
					pdWeightsTemp[i] = dBaseWeight;
				else if (pdMeasurement[i] >= dUpperThreshold)
					pdWeightsTemp[i] = 1.0;
				else
					pdWeightsTemp[i] = dBaseWeight + dLinWeight * (pdMeasurement[i] - dLowerThreshold) / (dUpperThreshold - dLowerThreshold);
			}//i
		}//end else linear weights
	}//end if new detection 
	else 
		for (i = iGlobal_Start; i <= iGlobal_Stop; i++) 
			pdWeightsTemp[i] = 1.0; 
	
	double dSumOfStaticWeights = 0.0; 
	for (i = iGlobal_Start; i <= iGlobal_Stop; i++)
		dSumOfStaticWeights += pdWeightsTemp[i];
	for (i = iGlobal_Start; i <= iGlobal_Stop; i++)
		pdWeightsTemp[i] /= dSumOfStaticWeights; 
	
	//3) Set weights in comp. window to 0:   
	for (i = 0; i < iGlobal_Start; i++)
		pdWeightsTemp[i] = 0.0; 
	for (i = iGlobal_Stop + 1; i < iDim; i++)
		pdWeightsTemp[i] = 0.0; 
#ifdef OLOVIA_DEBUG  
	ofstream ofWeights_For_DLS("Weights for DLS.txt");
	for (i = 0; i < iDim; i++)
		ofWeights_For_DLS << i << "\t" << i * dDelta_z << "\t" << pdWeightsTemp[i] << endl;
#endif
}




void vConvert_Double_To_cppc(int iDim, double *pRe, double *pIm, cppc *pComplex)
{
	int i;
	for (i = 0; i < iDim; i++) {
		pComplex[i].real(pRe[i]), pComplex[i].imag(pIm[i]);
	}
}



void vCalc_Fresnel_Coeff_Multilayer(int iNo_layers_top, Multilayer *pMulti)
{
	int i, j;
	cppc  cTrans_dd, cTrans_uu, cReflTop;

	pMulti->dFresnel_rtrt = 1.0;  //initialisation
	for (i = 1; i <= iNo_layers_top; i++) {
		pMulti->dVAmplFresnel.Data[i - 1] = abs((pMulti->VRef_Ind.Data[i - 1] - pMulti->VRef_Ind.Data[i]) / (pMulti->VRef_Ind.Data[i - 1] + pMulti->VRef_Ind.Data[i]));
		if (i == 1)
			pMulti->dFresnel_rtrt *= pMulti->dVAmplFresnel.Data[i - 1];
		cTrans_dd = 2.0 * pMulti->VRef_Ind.Data[i] / (pMulti->VRef_Ind.Data[i] + pMulti->VRef_Ind.Data[i + 1]);
		cTrans_uu = 2.0 * pMulti->VRef_Ind.Data[i + 1] / (pMulti->VRef_Ind.Data[i] + pMulti->VRef_Ind.Data[i + 1]);
		for (j = i; j > 0; j--) {
			pMulti->dVAmplFresnel.Data[j - 1] *= abs(cTrans_dd * cTrans_uu);
		}//j
		if (i < iNo_layers_top) {
			pMulti->dFresnel_rtrt *= abs(cTrans_dd);  pMulti->dFresnel_rtrt *= abs(cTrans_uu);
		}
	}//i 
	pMulti->dVAmplFresnel.Data[iNo_layers_top] =
		abs((pMulti->VRef_Ind.Data[iNo_layers_top + 1] - pMulti->VRef_Ind.Data[iNo_layers_top]) / (pMulti->VRef_Ind.Data[iNo_layers_top + 1] + pMulti->VRef_Ind.Data[iNo_layers_top]));

	cReflTop = (pMulti->VRef_Ind.Data[iNo_layers_top] - pMulti->VRef_Ind.Data[iNo_layers_top + 1]) / (pMulti->VRef_Ind.Data[iNo_layers_top] + pMulti->VRef_Ind.Data[iNo_layers_top + 1]);
	pMulti->dFresnel_rtrt *= abs(cReflTop);
}



void vCalc_Attenuations_Of_TSV_Peaks(int iNoLayers, double *pz_Peaks, Fresnel_TSV *pFresnel_TSV)
{
	int m;
	double dThickness_Layer;
	Multilayer *pMulti_on_Top = pFresnel_TSV->pMulti;

	pMulti_on_Top->dAttenuation_total = 1.0;   pMulti_on_Top->dVAttenuationAtInterfaces.Data[iNoLayers - 1] = 1.0;
	for (m = iNoLayers - 1; m >= 1; m--) {
		dThickness_Layer = (pz_Peaks[m] - pz_Peaks[m - 1]) / pMulti_on_Top->VRef_Ind.Data[m].real();
		pMulti_on_Top->dVAttenuationInLayers.Data[m] = exp(-(2.0 * PI / pMulti_on_Top->dLambda_Center) * pMulti_on_Top->VRef_Ind.Data[m].imag() * dThickness_Layer * 2.0);
		pMulti_on_Top->dAttenuation_total *= pMulti_on_Top->dVAttenuationInLayers.Data[m];
		pMulti_on_Top->dVAttenuationAtInterfaces.Data[m] = pMulti_on_Top->dAttenuation_total;
	}
}


void vCalc_Signal_In_Direct_Space_TSV_Layered(LISE_HF_Dir_Space *psInfoSpace, Fresnel_TSV *pFresnel, double *pz_Peaks, double dAmplSubst, double dFWHM,
	double dAmplBackgroundCenter, double dDerivBackgroundCenter, double dAmplMultilayer, int iPeakStart, int iPeakStop, double *pdModulus_of_FT)
{
	int i, j, iNoLayersOnTop = pFresnel->pMulti->dVAmplFresnel.Nown - 1, iIndex_Left_Left, iIndex_Left, iIndex_Right, iIndex_Right_Right, iStart, iStop;
	double dz, dk, dHalf_cos, dz_Peak_glob = 0.0, dz_Peak_loc, dAmpl = 0.0, dOpt_Thickness_Layers = 0.0;

	for (i = 0; i < psInfoSpace->iDim_z; i++)
		pdModulus_of_FT[i] = 0;

	vCalc_Attenuations_Of_TSV_Peaks(iNoLayersOnTop, pz_Peaks, pFresnel);
	dOpt_Thickness_Layers += pz_Peaks[iNoLayersOnTop] - pz_Peaks[0];

	for (j = iPeakStart; j <= iPeakStop; j++) { //maximally for (j = 0; j <= iNoLayersOnTop + 3; j++): Peaks from TSV/silicon, and peaks from layers on top 
		if (j <= iNoLayersOnTop) {
			dz_Peak_glob = pz_Peaks[j];					dAmpl = dAmplSubst * pFresnel->pMulti->dVAmplFresnel.Data[j] * pFresnel->pMulti->dVAttenuationAtInterfaces.Data[j];
		}
		else if (j == iNoLayersOnTop + 1) {
			dz_Peak_glob = 1 * dOpt_Thickness_Layers;   dAmpl = dAmplMultilayer;
		}
		else if (j == iNoLayersOnTop + 2) {
			dz_Peak_glob = 2 * dOpt_Thickness_Layers;   dAmpl = dAmplMultilayer * pFresnel->pMulti->dFresnel_rtrt * pFresnel->pMulti->dAttenuation_total;
		}
		else if (j == iNoLayersOnTop + 3) {
			dz_Peak_glob = 3 * dOpt_Thickness_Layers;   dAmpl = dAmplMultilayer * pow(pFresnel->pMulti->dFresnel_rtrt * pFresnel->pMulti->dAttenuation_total, 2.0);
		}
		else
			break;

		dz_Peak_loc = dz_Peak_glob - psInfoSpace->iStart * psInfoSpace->dDelta_z; //transition from global coordinates to local ones in the considered window
		dAmpl *= psInfoSpace->dAmpl_TSV_Si_Init; //due to previous normalization 

		iIndex_Left = (int)(dz_Peak_loc / psInfoSpace->dDelta_z);   iIndex_Left_Left = (int)((dz_Peak_loc - dFWHM) / psInfoSpace->dDelta_z) + 1;   dk = PI / dFWHM;
		if (iIndex_Left_Left > iIndex_Left)
			iIndex_Left_Left = iIndex_Left;
		iStart = min(iIndex_Left, psInfoSpace->iDim_z - 1);  iStop = max(iIndex_Left_Left, 0);
		for (i = iStart; i >= iStop; i--) {
			dz = i * psInfoSpace->dDelta_z;  	dHalf_cos = 0.5 + 0.5 * cos((dz - dz_Peak_loc) * dk);
			pdModulus_of_FT[i] += dHalf_cos * dAmpl;
		}
		iIndex_Right = iIndex_Left + 1;   iIndex_Right_Right = (int)((dz_Peak_loc + dFWHM) / psInfoSpace->dDelta_z);
		if (iIndex_Right_Right < iIndex_Right)
			iIndex_Right_Right = iIndex_Right;
		iStart = max(iIndex_Right, 0);  iStop = min(iIndex_Right_Right, psInfoSpace->iDim_z - 1);
		for (i = iStart; i <= iStop; i++) {
			dz = i * psInfoSpace->dDelta_z;  	dHalf_cos = 0.5 + 0.5 * cos((dz - dz_Peak_loc) * dk);
			pdModulus_of_FT[i] += dHalf_cos * dAmpl;
		}

	}//j 
	if (dAmplBackgroundCenter > NUM_NOISE_FLOAT) {
		double dzLeft = psInfoSpace->iStart * psInfoSpace->dDelta_z, dzRight = psInfoSpace->iStop * psInfoSpace->dDelta_z;  double dzCenter = (dzLeft + dzRight) / 2.0, df;
		double dExp = -dzCenter * dDerivBackgroundCenter / dAmplBackgroundCenter;
		dAmpl = dAmplBackgroundCenter * pow(dzCenter, dExp);
		for (i = psInfoSpace->iStart; i < psInfoSpace->iStop; i++) {
			dz = i * psInfoSpace->dDelta_z;   df = dAmpl / pow(dz, dExp);   pdModulus_of_FT[i] += df;
		}
	}
	ofstream ofSignal("CompSignal.txt");
	for (i = 0; i < psInfoSpace->iDim_z; i++)
		ofSignal << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pdModulus_of_FT[i] << endl;
}



void vCalc_Signal_In_Direct_Space_LISE_ED(sAdditionalData_LISE_ED *psInfo, double *pOptParam, int iPeakStart, int iPeakStop, double *pModel)
{
	int i, j, iIndex_Left_Left, iIndex_Left, iIndex_Right, iIndex_Right_Right, iStart, iStop;
	double dz, dk, dHalf_cos, dz_Peak_glob = 0.0, dz_Peak_loc, dAmpl = 0.0, dFWHM = (double)(psInfo->dFWHM_Peak);  
	LISE_HF_Dir_Space *psInfoSpace = (LISE_HF_Dir_Space *)(psInfo->pDirSpace);

	for (i = 0; i < psInfoSpace->iDim_z; i++)
		pModel[i] = 0.0;

	for (j = iPeakStart; j <= iPeakStop; j++) { //maximally for (j = 0; j < iNo_layers + 3; j++) 
		dz_Peak_glob = pOptParam[2 * j];		dAmpl = pOptParam[2 * j + AMPL_TSV];
		if (dAmpl <= NUM_NOISE)
			continue; 
		dz_Peak_loc = dz_Peak_glob - psInfoSpace->iStart * psInfoSpace->dDelta_z; //transition from global coordinates to local ones in the considered calc. window
		dAmpl *= psInfoSpace->dAmpl_TSV_Si_Init; //due to previous normalization 

		iIndex_Left = (int)(dz_Peak_loc / psInfoSpace->dDelta_z);   iIndex_Left_Left = (int)((dz_Peak_loc - dFWHM) / psInfoSpace->dDelta_z) + 1;   dk = PI / dFWHM;
		if (iIndex_Left_Left > iIndex_Left)
			iIndex_Left_Left = iIndex_Left;
		iStart = min(iIndex_Left, psInfoSpace->iDim_z - 1);  iStop = max(iIndex_Left_Left, 0);
		for (i = iStart; i >= iStop; i--){
			dz = i * psInfoSpace->dDelta_z;  	dHalf_cos = 0.5 + 0.5 * cos((dz - dz_Peak_loc) * dk);
			pModel[i] += dHalf_cos * dAmpl;
		}
		iIndex_Right = iIndex_Left + 1;   iIndex_Right_Right = (int)((dz_Peak_loc + dFWHM) / psInfoSpace->dDelta_z);
		if (iIndex_Right_Right < iIndex_Right)
			iIndex_Right_Right = iIndex_Right;
		iStart = max(iIndex_Right, 0);  iStop = min(iIndex_Right_Right, psInfoSpace->iDim_z - 1);
		for (i = iStart; i <= iStop; i++){
			dz = i * psInfoSpace->dDelta_z;  	dHalf_cos = 0.5 + 0.5 * cos((dz - dz_Peak_loc) * dk);
			pModel[i] += dHalf_cos * dAmpl;
		}
	}//j 

#ifdef OLOVIA_DEBUG
	ofstream ofSignal("CompSignal.txt");
	for (i = 0; i < psInfoSpace->iDim_z; i++)
		ofSignal << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pModel[i] << endl;
#endif
}



void vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ(sAdditionalData_LISE_ED *psInfo, double *pdOptParam, int iNoPeaks, bool bUpdateWeights, 
	double *pdData, double *pdModelMinusData)
//_WLSQ means weighted least squares.  W. Iff, 23.7.2024 
{
	int i, j, iIndex_Left[4], iIndex_Right[4]; 
	double dz, dHalf_cos, dz_Peak_glob = 0.0, dz_Peak_loc, dAmpl = 0.0, dTemp, dFWHM = (double)(psInfo->dFWHM_Peak);  double dk = PI / dFWHM; 
	LISE_HF_Dir_Space *psInfoSpace = (LISE_HF_Dir_Space *)(psInfo->pDirSpace);
	double *pdStaticWeights = psInfoSpace->pdStaticWeights, *pdDynamicWeights = psInfoSpace->pdDynamicWeights; 
	const double dWeightingFactor = 2.0 / 3.0;
	double *pdWeightsInTotal = psInfoSpace->pdWeightsInTotal;
#ifdef OLOVIA_DEBUG
	ofstream ofSignal("CompSignal.txt"), ofWeightsInTotal("WeightsInTotal.txt"), ofWeightedResult("WeightedResult.txt");
#endif 

	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		pdModelMinusData[i] = +0.0;		pdDynamicWeights[i] = +0.0;
	}

	for (j = 0; j < iNoPeaks; j++) {
		dz_Peak_glob = pdOptParam[2 * j];		dAmpl = pdOptParam[2 * j + AMPL_TSV];

		dz_Peak_loc = dz_Peak_glob - psInfoSpace->iStart * psInfoSpace->dDelta_z; //transition from global coordinates to local ones in the considered calc. window
		dAmpl *= psInfoSpace->dAmpl_TSV_Si_Init; //due to previous normalization 

		//calc. left side of peak: 
		iIndex_Left[3] = (int)(dz_Peak_loc / psInfoSpace->dDelta_z);	  iIndex_Left[3] = min(iIndex_Left[3], psInfoSpace->iDim_z - 1);
		iIndex_Left[2] = (int)((dz_Peak_loc - dFWHM / 2.0) / psInfoSpace->dDelta_z) + 1;     iIndex_Left[2] = max(iIndex_Left[2], 0);
		iIndex_Left[1] = iIndex_Left[2] - 1;     iIndex_Left[1] = min(iIndex_Left[1], psInfoSpace->iDim_z - 1);
		iIndex_Left[0] = (int)((dz_Peak_loc - dFWHM) / psInfoSpace->dDelta_z) + 1;     iIndex_Left[0] = max(iIndex_Left[0], 0);

		for (i = iIndex_Left[3]; i >= iIndex_Left[2]; i--){ 
			dz = i * psInfoSpace->dDelta_z;  	dTemp = cos((dz - dz_Peak_loc) * dk);    // pdDynamicWeights[i] += dTemp * dTemp;
			dHalf_cos = 0.5 + 0.5 * dTemp;  dTemp = dHalf_cos * dAmpl;   pdModelMinusData[i] += dTemp * dTemp;
		}
		for (i = iIndex_Left[1]; i >= iIndex_Left[0]; i--){
			dz = i * psInfoSpace->dDelta_z;  	dTemp = cos((dz - dz_Peak_loc) * dk);    
			dHalf_cos = 0.5 + 0.5 * dTemp;  dTemp = dHalf_cos * dAmpl;   pdModelMinusData[i] += dTemp * dTemp;
		}

		//calc. right side of peak:  
		iIndex_Right[0] = iIndex_Left[3] + 1;    iIndex_Right[0] = max(iIndex_Right[0], 0);
		iIndex_Right[1] = (int)((dz_Peak_loc + dFWHM / 2.0) / psInfoSpace->dDelta_z);   iIndex_Right[1] = min(iIndex_Right[1], psInfoSpace->iDim_z - 1);
		iIndex_Right[2] = iIndex_Right[1] + 1;   iIndex_Right[2] = max(iIndex_Right[2], 0);
		iIndex_Right[3] = (int)((dz_Peak_loc + dFWHM) / psInfoSpace->dDelta_z);   iIndex_Right[3] = min(iIndex_Right[3], psInfoSpace->iDim_z - 1);
		
		for (i = iIndex_Right[0]; i <= iIndex_Right[1]; i++){
			dz = i * psInfoSpace->dDelta_z;  	dTemp = cos((dz - dz_Peak_loc) * dk);    // pdDynamicWeights[i] += dTemp * dTemp;
			dHalf_cos = 0.5 + 0.5 * dTemp;    dTemp = dHalf_cos * dAmpl;   pdModelMinusData[i] += dTemp * dTemp;
		}
		for (i = iIndex_Right[2]; i <= iIndex_Right[3]; i++){
			dz = i * psInfoSpace->dDelta_z;  	dTemp = cos((dz - dz_Peak_loc) * dk);    
			dHalf_cos = 0.5 + 0.5 * dTemp;    dTemp = dHalf_cos * dAmpl;   pdModelMinusData[i] += dTemp * dTemp;
		}
	}//end for all peaks j 
	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		pdModelMinusData[i] = sqrt(pdModelMinusData[i]);	// pdDynamicWeights[i] = sqrt(pdDynamicWeights[i]);
#ifdef OLOVIA_DEBUG
		ofSignal << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << 
			pdStaticWeights[i] << "\t\t" << pdDynamicWeights[i] << "\t\t" << pdData[i] << "\t\t" << pdModelMinusData[i] << endl;
#endif
	}//i
	if (bUpdateWeights) {
		psInfoSpace->dSumOfWeights = 0.0; 
		for (i = 0; i < psInfoSpace->iDim_z; i++) {
			pdWeightsInTotal[i] = pdStaticWeights[i] + dWeightingFactor * pdDynamicWeights[i];   psInfoSpace->dSumOfWeights += pdWeightsInTotal[i];
		}//i
		for (i = 0; i < psInfoSpace->iDim_z; i++)
			pdWeightsInTotal[i] /= psInfoSpace->dSumOfWeights;
	}//end if update

	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		pdModelMinusData[i] = (pdModelMinusData[i] - pdData[i]) * pdWeightsInTotal[i];
#ifdef OLOVIA_DEBUG
		ofWeightsInTotal << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pdWeightsInTotal[i] << "\t\t" << pdModelMinusData[i] << endl;
#endif
	}//i
#ifdef OLOVIA_DEBUG
	ofWeightsInTotal << endl << "dSumOfWeights: " << psInfoSpace->dSumOfWeights << endl;   
	cout << endl << "dSumOfWeights: " << psInfoSpace->dSumOfWeights << "\t\t";
	double dNormSq = 0.0; 
	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		ofWeightedResult << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pdModelMinusData[i] << endl;     
		dNormSq += pdModelMinusData[i] * pdModelMinusData[i];
	}
	ofWeightedResult << endl << "dNormSq: " << dNormSq << endl;		cout << "dNormSq: " << dNormSq << endl;
#endif
} 





void vIntAndDeriv_TSV_Dir_Space_LISE_ED_Mode(void *pInfoInt, void *pvInfo, void *pvAdditionalInfo,
	dVector& dVParam, dVector *pVDeltaInt, dMatrix *pdMDeriv, bool bOptParam[], bool *pSuc)
	//All depths + Amplitudes + FWHM are optimized. W.Iff, 1.5.2022
{
	int i, j, iIndex, iNoLayers = *((int*)pvAdditionalInfo);
	double *pdDeltaInt = pVDeltaInt->Data, dTemp;
	double dDelta_Depth = 0.005; 
	sAdditionalData_LISE_ED *psInfo = (sAdditionalData_LISE_ED *)pvInfo;  	LISE_HF_Dir_Space *psInfoSpace = (LISE_HF_Dir_Space *)psInfo->pDirSpace;
	double  *pdIntensity = (double*)pInfoInt, *pdWeightsInTotal = psInfoSpace->pdWeightsInTotal; //use of local coordinates within the window 
#ifdef OLOVIA_DEBUG 
	ofstream ofMerit("Merit.txt");
#endif 

	vCalc_Signal_In_Direct_Space_LISE_ED(psInfo, dVParam.Data, 0, iNoLayers - 1, pdDeltaInt);
	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		#ifdef OLOVIA_DEBUG 
		ofMerit << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pdIntensity[i] << "\t\t" << pdDeltaInt[i] << 
			"\t\t" << pdWeightsInTotal[i] << "\t\t" << pdDeltaInt[i] - pdIntensity[i] << endl; 
		#endif 
		pdDeltaInt[i] -= pdIntensity[i];  pdDeltaInt[i] *= pdWeightsInTotal[i];
	}

	if (pdMDeriv) { //calc derivatives:
		dVector dVDelta_Plus(psInfoSpace->iDim_z), dVDelta_Minus(psInfoSpace->iDim_z), dVDeriv(psInfoSpace->iDim_z);  
		*pdMDeriv = 0.0; //initialization 

		for (i = 0; i < iNoLayers; i++) { //depths (TSV, layers):
			iIndex = 2 * i;
			if (bOptParam[iIndex]) {
				dTemp = dVParam.Data[iIndex];   dVParam.Data[iIndex] = dTemp + dDelta_Depth;
				vCalc_Signal_In_Direct_Space_LISE_ED(psInfo, dVParam.Data, 0, iNoLayers - 1, dVDelta_Plus.Data);
				dVParam.Data[iIndex] = dTemp - dDelta_Depth;
				vCalc_Signal_In_Direct_Space_LISE_ED(psInfo, dVParam.Data, 0, iNoLayers - 1, dVDelta_Minus.Data);
				dVParam.Data[iIndex] = dTemp; //restore original value
				for (j = 0; j < psInfoSpace->iDim_z; j++) {
					pdMDeriv->Data[iIndex] = pdWeightsInTotal[j] * (dVDelta_Plus.Data[j] - dVDelta_Minus.Data[j]) / (2.0 * dDelta_Depth);
					iIndex += pdMDeriv->Next;
				}
			}//end if
		}//end for i 

		for (i = 0; i < iNoLayers; i++) {
			iIndex = 2 * i + AMPL_TSV; 
			if (bOptParam[iIndex]) { //amplitude of substrate:
				dTemp = dVParam.Data[iIndex];   dVParam.Data[iIndex] = 1.0;
				vCalc_Signal_In_Direct_Space_LISE_ED(psInfo, dVParam.Data, i, i, dVDeriv.Data);
				dVParam.Data[iIndex] = dTemp;
				for (j = 0; j < psInfoSpace->iDim_z; j++) {
					pdMDeriv->Data[iIndex] = pdWeightsInTotal[j] * dVDeriv.Data[j];
					iIndex += pdMDeriv->Next;
				}
			}//end if bOptParam...
		}//end for i

	}//end if pMDeriv   
	*pSuc = true;
}



void vIntAndDeriv_TSV_Dir_Space_LISE_HF_WLSQ(void *pInfoInt, void *pvInfo, void *pvAdditionalInfo,
	dVector& dVParam, dVector *pVDeltaInt, dMatrix *pdMDeriv, bool bOptParam[], bool *pSuc)
	//This function calculates merit function and derivatives for the WLSQ (weighted least squares procedure).  
	//All depths + Amplitudes are optimized. W. Iff, 25.7.2024  
{
	int i, j, iIndex, iNoLayers = *((int*)pvAdditionalInfo);
	double *pdDeltaInt = pVDeltaInt->Data, dTemp, dParamValuePlus, dParamValueMinus;
	const double dDelta_Depth = 0.004, dDelta_Ampl = 0.001;
	sAdditionalData_LISE_ED *psInfo = (sAdditionalData_LISE_ED *)pvInfo;  	LISE_HF_Dir_Space *psInfoSpace = (LISE_HF_Dir_Space *)psInfo->pDirSpace;
	double  *pdIntensity = (double*)pInfoInt; //use of local coordinates within the window 
#ifdef OLOVIA_DEBUG 
	ofstream ofMerit("Merit.txt");
#endif 

	vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ( psInfo, dVParam.Data, iNoLayers, false, pdIntensity, pdDeltaInt ); //pdMDeriv==NULL ? false : true
#ifdef OLOVIA_DEBUG 
	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		ofMerit << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pdIntensity[i] << "\t\t" << pdDeltaInt[i] << endl; 
	}
#endif 

	if (pdMDeriv) { //calc derivatives:
		dVector dVDelta_Plus(psInfoSpace->iDim_z), dVDelta_Minus(psInfoSpace->iDim_z), dVDeriv(psInfoSpace->iDim_z); 
		*pdMDeriv = 0.0; //initialization 

		for (i = 0; i < iNoLayers; i++) { //depths (TSV, layers):
			iIndex = 2 * i;
			if (bOptParam[iIndex]) {
				dTemp = dVParam.Data[iIndex];   dVParam.Data[iIndex] = dTemp + dDelta_Depth;
				vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ( psInfo, dVParam.Data, iNoLayers, false, pdIntensity, dVDelta_Plus.Data );
				dVParam.Data[iIndex] = dTemp - dDelta_Depth;
				vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ( psInfo, dVParam.Data, iNoLayers, false, pdIntensity, dVDelta_Minus.Data );
				dVParam.Data[iIndex] = dTemp; //restore original value
				for (j = 0; j < psInfoSpace->iDim_z; j++) {
					pdMDeriv->Data[iIndex] = (dVDelta_Plus.Data[j] - dVDelta_Minus.Data[j]) / (2.0 * dDelta_Depth);
					iIndex += pdMDeriv->Next; 
				}
			}//end if
		}//end for i 

		for (i = 0; i < iNoLayers; i++) {
			iIndex = 2 * i + AMPL_TSV;
			if (bOptParam[iIndex]) { //amplitude of substrate:
				dTemp = dVParam.Data[iIndex];   dParamValuePlus = dVParam.Data[iIndex] = dTemp + dDelta_Ampl;
				vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ(psInfo, dVParam.Data, iNoLayers, false, pdIntensity, dVDelta_Plus.Data);
				dParamValueMinus = dVParam.Data[iIndex] = max(dTemp - dDelta_Ampl, +0.0); 
				vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ(psInfo, dVParam.Data, iNoLayers, false, pdIntensity, dVDelta_Minus.Data);
				dVParam.Data[iIndex] = dTemp; //restore original value
				for (j = 0; j < psInfoSpace->iDim_z; j++) {
					pdMDeriv->Data[iIndex] = (dVDelta_Plus.Data[j] - dVDelta_Minus.Data[j]) / (dParamValuePlus - dParamValueMinus); 
					iIndex += pdMDeriv->Next; 
				}
			}//end if bOptParam...
		}//end for i

	}//end if pMDeriv   
	*pSuc = true;
}





void vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ2(sAdditionalData_LISE_ED *psInfo, double *pdOptParam, int iNoPeaks, bool bUpdateWeights,
	double *pdData, double *pdModelMinusData)
	//W. Iff, 31.8.2024 
{
	int i, j, iIndex_Left[4], iIndex_Right[4];
	double dz, dHalf_cos, dz_Peak_glob = 0.0, dz_Peak_loc, dAmpl = 0.0, dTemp, dFWHM = (double)(psInfo->dFWHM_Peak);  double dk = PI / dFWHM;
	double dIntegral, dNormaliz, dAverageValue, dPeakHeightEstimate, dThreshold;  const double dRelativeThreshold = 0.52;
	LISE_HF_Dir_Space *psInfoSpace = (LISE_HF_Dir_Space *)(psInfo->pDirSpace);
	double *pdCosine = psInfoSpace->pdCosine, dFactor1, dFactor2, *pdWeightsSinglePeak = psInfoSpace->pdWeightsSinglePeak; 
	double *pdStaticWeights = psInfoSpace->pdStaticWeights, *pdDynamicWeights = psInfoSpace->pdDynamicWeights;
	const double dWeightingFactorStatic = 0.0, dWeightingFactorDynamic = 1.0;
	double *pdWeightsInTotal = psInfoSpace->pdWeightsInTotal;
#ifdef OLOVIA_DEBUG
	ofstream ofSignal("CompSignal.txt"), ofWeightsInTotal("WeightsInTotal.txt"), ofWeightedResult("WeightedResult.txt");
#endif 

	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		pdModelMinusData[i] = +0.0;		pdDynamicWeights[i] = +0.0;
	}
	for (j = 0; j < iNoPeaks; j++) {
		for (i = 0; i < psInfoSpace->iDim_z; i++) {
			pdCosine[i] = 0.0, pdWeightsSinglePeak[i] = 0.0; //not needed but it makes debugging easier
		}
		dIntegral = 0.0, dNormaliz = 0.0;

		dz_Peak_glob = pdOptParam[2 * j];		dAmpl = pdOptParam[2 * j + AMPL_TSV];

		dz_Peak_loc = dz_Peak_glob - psInfoSpace->iStart * psInfoSpace->dDelta_z; //transition from global coordinates to local ones in the considered calc. window
		dAmpl *= psInfoSpace->dAmpl_TSV_Si_Init; //due to previous normalization 

		//calc. left side of peak: 
		iIndex_Left[3] = (int)(dz_Peak_loc / psInfoSpace->dDelta_z);	  iIndex_Left[3] = min(iIndex_Left[3], psInfoSpace->iDim_z - 1);
		iIndex_Left[2] = (int)((dz_Peak_loc - dFWHM / 2.0) / psInfoSpace->dDelta_z) + 1;     iIndex_Left[2] = max(iIndex_Left[2], 0);
		iIndex_Left[1] = iIndex_Left[2] - 1;     iIndex_Left[1] = min(iIndex_Left[1], psInfoSpace->iDim_z - 1);
		iIndex_Left[0] = (int)((dz_Peak_loc - dFWHM) / psInfoSpace->dDelta_z) + 1;     iIndex_Left[0] = max(iIndex_Left[0], 0);

		for (i = iIndex_Left[3]; i >= iIndex_Left[2]; i--){
			dz = i * psInfoSpace->dDelta_z;		dTemp = cos((dz - dz_Peak_loc) * dk);    
			dHalf_cos = 0.5 + 0.5 * dTemp;		pdCosine[i] = dHalf_cos * dAmpl;	dNormaliz += pdCosine[i]; dIntegral += pdCosine[i] * pdData[i];
			pdModelMinusData[i] += pdCosine[i] * pdCosine[i];
		}
		for (i = iIndex_Left[1]; i >= iIndex_Left[0]; i--){
			dz = i * psInfoSpace->dDelta_z;  	dTemp = cos((dz - dz_Peak_loc) * dk);
			dHalf_cos = 0.5 + 0.5 * dTemp;		pdCosine[i] = dHalf_cos * dAmpl;	dNormaliz += pdCosine[i]; dIntegral += pdCosine[i] * pdData[i];
			pdModelMinusData[i] += pdCosine[i] * pdCosine[i];
		}

		//calc. right side of peak:  
		iIndex_Right[0] = iIndex_Left[3] + 1;    iIndex_Right[0] = max(iIndex_Right[0], 0);
		iIndex_Right[1] = (int)((dz_Peak_loc + dFWHM / 2.0) / psInfoSpace->dDelta_z);   iIndex_Right[1] = min(iIndex_Right[1], psInfoSpace->iDim_z - 1);
		iIndex_Right[2] = iIndex_Right[1] + 1;   iIndex_Right[2] = max(iIndex_Right[2], 0);
		iIndex_Right[3] = (int)((dz_Peak_loc + dFWHM) / psInfoSpace->dDelta_z);   iIndex_Right[3] = min(iIndex_Right[3], psInfoSpace->iDim_z - 1);

		for (i = iIndex_Right[0]; i <= iIndex_Right[1]; i++){
			dz = i * psInfoSpace->dDelta_z;  	dTemp = cos((dz - dz_Peak_loc) * dk);    
			dHalf_cos = 0.5 + 0.5 * dTemp;		pdCosine[i] = dHalf_cos * dAmpl;   dNormaliz += pdCosine[i]; dIntegral += pdCosine[i] * pdData[i];
			pdModelMinusData[i] += pdCosine[i] * pdCosine[i];
		}
		for (i = iIndex_Right[2]; i <= iIndex_Right[3]; i++){
			dz = i * psInfoSpace->dDelta_z;  	dTemp = cos((dz - dz_Peak_loc) * dk);
			dHalf_cos = 0.5 + 0.5 * dTemp;		pdCosine[i] = dHalf_cos * dAmpl;   dNormaliz += pdCosine[i]; dIntegral += pdCosine[i] * pdData[i];
			pdModelMinusData[i] += pdCosine[i] * pdCosine[i];
		}
		dAverageValue = dIntegral / dNormaliz;  dPeakHeightEstimate = dAverageValue * 4.0 / 3.0;  dThreshold = dRelativeThreshold * dPeakHeightEstimate;
		dNormaliz = 0.0; 
		for (i = iIndex_Left[0]; i <= iIndex_Right[3]; i++) {
			dFactor1 = max(pdCosine[i] - 0.07 * dAmpl, 0.0);   dFactor2 = max(pdData[i] - dThreshold, 0.0);
			pdWeightsSinglePeak[i] = dFactor1 * dFactor2;    
			dNormaliz += pdWeightsSinglePeak[i]; //do we need normaliz of single peak weights ? 
		}//i
		for (i = iIndex_Left[0]; i <= iIndex_Right[3]; i++)
			pdWeightsSinglePeak[i] /= dNormaliz; 
		for (i = iIndex_Left[0]; i <= iIndex_Right[3]; i++)
			pdDynamicWeights[i] += pdWeightsSinglePeak[i] * pdWeightsSinglePeak[i];
	}//end for all peaks j 
	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		pdModelMinusData[i] = sqrt(pdModelMinusData[i]);	 pdDynamicWeights[i] = sqrt(pdDynamicWeights[i]);
#ifdef OLOVIA_DEBUG
		ofSignal << i << "\t";
		ofSignal << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t";
		ofSignal << pdStaticWeights[i] << "\t\t"; 
		ofSignal << pdDynamicWeights[i] << "\t\t";
		ofSignal << pdData[i] << "\t\t";
		ofSignal << pdModelMinusData[i] << endl;
#endif
	}//i
	if (bUpdateWeights) {
		psInfoSpace->dSumOfWeights = 0.0;
		for (i = 0; i < psInfoSpace->iDim_z; i++) {
			pdWeightsInTotal[i] = dWeightingFactorStatic * pdStaticWeights[i] + dWeightingFactorDynamic * pdDynamicWeights[i];   
			psInfoSpace->dSumOfWeights += pdWeightsInTotal[i];
		}//i
		for (i = 0; i < psInfoSpace->iDim_z; i++)
			pdWeightsInTotal[i] /= psInfoSpace->dSumOfWeights;
	}//end if update

	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		pdModelMinusData[i] = (pdModelMinusData[i] - pdData[i]) * pdWeightsInTotal[i];
#ifdef OLOVIA_DEBUG
		ofWeightsInTotal << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pdWeightsInTotal[i] << "\t\t" << pdModelMinusData[i] << endl;
#endif
	}//i
#ifdef OLOVIA_DEBUG
	ofWeightsInTotal << endl << "dSumOfWeights: " << psInfoSpace->dSumOfWeights << endl;
	cout << endl << "dSumOfWeights: " << psInfoSpace->dSumOfWeights << "\t\t";
	double dNormSq = 0.0;
	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		ofWeightedResult << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pdModelMinusData[i] << endl;
		dNormSq += pdModelMinusData[i] * pdModelMinusData[i];
	}
	ofWeightedResult << endl << "dNormSq: " << dNormSq << endl;		cout << "dNormSq: " << dNormSq << endl;
#endif
}





void vIntAndDeriv_TSV_Dir_Space_LISE_HF_WLSQ2(void *pInfoInt, void *pvInfo, void *pvAdditionalInfo,
	dVector& dVParam, dVector *pVDeltaInt, dMatrix *pdMDeriv, bool bOptParam[], bool *pSuc)
	//This function calculates merit function and derivatives for the WLSQ2 (weighted least squares procedure 2).  
	//All depths + Amplitudes are optimized.  W. Iff, 31.8.2024 
{
	int i, j, iIndex, iNoLayers = *((int*)pvAdditionalInfo);
	double *pdDeltaInt = pVDeltaInt->Data, dTemp, dParamValuePlus, dParamValueMinus;
	const double dDelta_Depth = 0.004, dDelta_Ampl = 0.001;
	sAdditionalData_LISE_ED *psInfo = (sAdditionalData_LISE_ED *)pvInfo;  	LISE_HF_Dir_Space *psInfoSpace = (LISE_HF_Dir_Space *)psInfo->pDirSpace;
	double  *pdIntensity = (double*)pInfoInt; //use of local coordinates within the window 
#ifdef OLOVIA_DEBUG 
	ofstream ofMerit("Merit.txt");
#endif 

	vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ2(psInfo, dVParam.Data, iNoLayers, psInfoSpace->bUpdateWeights, pdIntensity, pdDeltaInt); 
	psInfoSpace->bUpdateWeights = false; //only one update per Levenberg-Marquardt call or per iteration maximally 
#ifdef OLOVIA_DEBUG 
	for (i = 0; i < psInfoSpace->iDim_z; i++) {
		ofMerit << i << "\t" << (i + psInfoSpace->iStart) * psInfoSpace->dDelta_z << "\t\t" << pdIntensity[i] << "\t\t" << pdDeltaInt[i] << endl;
	}
#endif 

	if (pdMDeriv) { //calc derivatives:
		dVector dVDelta_Plus(psInfoSpace->iDim_z), dVDelta_Minus(psInfoSpace->iDim_z), dVDeriv(psInfoSpace->iDim_z);
		*pdMDeriv = 0.0; //initialization 

		for (i = 0; i < iNoLayers; i++) { //depths (TSV, layers):
			iIndex = 2 * i;
			if (bOptParam[iIndex]) {
				dTemp = dVParam.Data[iIndex];   dVParam.Data[iIndex] = dTemp + dDelta_Depth;
				vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ2(psInfo, dVParam.Data, iNoLayers, false, pdIntensity, dVDelta_Plus.Data);
				dVParam.Data[iIndex] = dTemp - dDelta_Depth;
				vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ2(psInfo, dVParam.Data, iNoLayers, false, pdIntensity, dVDelta_Minus.Data);
				dVParam.Data[iIndex] = dTemp; //restore original value
				for (j = 0; j < psInfoSpace->iDim_z; j++) {
					pdMDeriv->Data[iIndex] = (dVDelta_Plus.Data[j] - dVDelta_Minus.Data[j]) / (2.0 * dDelta_Depth);
					iIndex += pdMDeriv->Next;
				}
			}//end if
		}//end for i 

		for (i = 0; i < iNoLayers; i++) {
			iIndex = 2 * i + AMPL_TSV;
			if (bOptParam[iIndex]) { //amplitude of substrate:
				dTemp = dVParam.Data[iIndex];   dParamValuePlus = dVParam.Data[iIndex] = dTemp + dDelta_Ampl;
				vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ2(psInfo, dVParam.Data, iNoLayers, false, pdIntensity, dVDelta_Plus.Data);
				dParamValueMinus = dVParam.Data[iIndex] = max(dTemp - dDelta_Ampl, +0.0);
				vCalc_Signal_In_Direct_Space_LISE_HF_WLSQ2(psInfo, dVParam.Data, iNoLayers, false, pdIntensity, dVDelta_Minus.Data);
				dVParam.Data[iIndex] = dTemp; //restore original value
				for (j = 0; j < psInfoSpace->iDim_z; j++) {
					pdMDeriv->Data[iIndex] = (dVDelta_Plus.Data[j] - dVDelta_Minus.Data[j]) / (dParamValuePlus - dParamValueMinus);
					iIndex += pdMDeriv->Next;
				}
			}//end if bOptParam... 
		}//end for i

	}//end if pMDeriv   
	*pSuc = true;
}







void vCalc_Optical_Thickness_Of_Multilayer(int iNo_layers, double *pGeom_thicknesses, double *pRef_index_re, double *pOptical_thicknesses, double &dThickness_total)
{
	int i;
	dThickness_total = 0.0;   //total optical thickness of stack of layers 
	if (pGeom_thicknesses == NULL || pRef_index_re == NULL) //if there are no layers: 
		return;
	for (i = 0; i < iNo_layers; i++) {
		pOptical_thicknesses[i] = pRef_index_re[i] * pGeom_thicknesses[i];   dThickness_total += pOptical_thicknesses[i];
	}
}


double dCalc_Effective_Index(double TSV_Diameter, double dLabda_Mid_um)
//This function should be improved later when the effective indices are computed more accurately for visible light 
{
	double dNormalized_Diam = (TSV_Diameter / 6.0) * (1.31 / dLabda_Mid_um);
	double dDenominator = pow(dNormalized_Diam, 1.585);
	double dEff_Ind = 1.0 + 0.01 * 1.0 / dDenominator;
	return dEff_Ind;
}


bool bCalc_Geom_From_Optical_Depths_TSV(int iNo_TSV_layers, double *pOptical_thicknesses, double *pOptical_thickness_tolerances, double *pRef_index_re, double dEff_Index_TSV,
	double &dTotal_optical_depth, double &dTotal_geom_depth, double &dTotal_optical_tolerance, double *pGeom_layer_thicknesses)
{
	if (iNo_TSV_layers <= 0)
		return false;
	int i;
	dTotal_optical_depth = 0.0;
	for (i = 0; i < iNo_TSV_layers; i++)
		dTotal_optical_depth += pOptical_thicknesses[i];
	dTotal_geom_depth = dTotal_optical_depth / dEff_Index_TSV;

	dTotal_optical_tolerance = 0;
	if (pOptical_thickness_tolerances != NULL)
		for (i = 0; i < iNo_TSV_layers; i++)
			dTotal_optical_tolerance += pOptical_thickness_tolerances[i];

	if (pGeom_layer_thicknesses != NULL) {
		for (i = iNo_TSV_layers - 1; i > 0; i--)
			pGeom_layer_thicknesses[i] = pOptical_thicknesses[i] / pRef_index_re[i];
		pGeom_layer_thicknesses[0] = dTotal_geom_depth;
		for (i = iNo_TSV_layers - 1; i > 0; i--)
			pGeom_layer_thicknesses[0] -= pGeom_layer_thicknesses[i];
	}
	return true;
}


bool bCalc_Optical_From_Geom_Depths_TSV(double dEff_Index_TSV, int iNo_TSV_layers, double *pGeom_layer_thicknesses, double *pRef_index_re, double *pOptical_layer_thicknesses)
{
	if (iNo_TSV_layers <= 0 || pGeom_layer_thicknesses == NULL || pOptical_layer_thicknesses == NULL)
		return false;
	int i;
	for (i = iNo_TSV_layers - 1; i > 0; i--)
		pOptical_layer_thicknesses[i] = pGeom_layer_thicknesses[i] * pRef_index_re[i];
	double dTotal_geom_depth = 0;
	for (i = 0; i < iNo_TSV_layers; i++)
		dTotal_geom_depth += pGeom_layer_thicknesses[i];

	pOptical_layer_thicknesses[0] = dTotal_geom_depth * dEff_Index_TSV;
	for (i = iNo_TSV_layers - 1; i > 0; i--)
		pOptical_layer_thicknesses[0] -= pOptical_layer_thicknesses[i];
	return true;
}


bool bApplyConstraints(int iNo_layers, double *pOptical_thicknesses, double *pOptical_thickness_tolerances, double *pComputed_optical_thicknesses)
{
	bool bSuccess = true;
	int i;
	for (i = 0; i < iNo_layers; i++) {
		if (pComputed_optical_thicknesses[i] < pOptical_thicknesses[i] - pOptical_thickness_tolerances[i] ||
			pComputed_optical_thicknesses[i] > pOptical_thicknesses[i] + pOptical_thickness_tolerances[i])
			bSuccess = false;
		if (pComputed_optical_thicknesses[i] < 0.0)
			bSuccess = false;
	}
	return bSuccess;
}




bool bApplyConstraints2(int iNo_layers, double *pOptical_thicknesses, double *pOptical_thickness_tolerances, double *pComputed_optical_thicknesses, 
	double dFWHM)
{
	bool bSuccess = true;
	int i;
	for (i = 0; i < iNo_layers; i++) {
		if (pComputed_optical_thicknesses[i] < pOptical_thicknesses[i] - pOptical_thickness_tolerances[i] - dFWHM * 0.66 ||
			pComputed_optical_thicknesses[i] > pOptical_thicknesses[i] + pOptical_thickness_tolerances[i] + dFWHM * 0.66)
			bSuccess = false;
		if (pComputed_optical_thicknesses[i] < dFWHM * 0.2)
			bSuccess = false;
	}
	return bSuccess;
}





void vSetGridForGridSearch(int iNoParams, double dzWindowRight, double dStepsize_z, double *pParamValues, double *pTolerances, dMatrix &dMGrid)
{
	int i, j, iDim = 1;    iVector iV_Dim(iNoParams);
	dVector dVStepSize(iNoParams);    dMatrix dM_z_Values(iNoParams, (int)(dzWindowRight / dStepsize_z) + 2);  dM_z_Values = 0.0;

	for (i = 0; i < iNoParams; i++) {
		iV_Dim.Data[i] = (int)(2.0 * pTolerances[i] / dStepsize_z) + 1;  dVStepSize.Data[i] = (2.0 * pTolerances[i]) / ((double)iV_Dim.Data[i]);  iDim *= iV_Dim.Data[i];
		for (j = 0; j < iV_Dim.Data[i]; j++)
			dM_z_Values(i, j) = pParamValues[i] - pTolerances[i] + (0.5 + j) * dVStepSize.Data[i]; //discretisation  
	}
	dMGrid.Nown = iDim, dMGrid.Next = iNoParams, delete[] dMGrid.Data; dMGrid.Data = new double[dMGrid.Nown * dMGrid.Next];  //dMatrix dMGrid(iDim, iNoParams);  
	for (j = 0; j < iNoParams; j++)
		dMGrid(0, j) = dM_z_Values(j, 0);
	for (i = 1; i < iDim; i++) {
		for (j = 0; j < iNoParams; j++)
			dMGrid.Data[i * iNoParams + j] = dMGrid.Data[(i - 1) * iNoParams + j];
		dMGrid.Data[i * iNoParams + 0] += dVStepSize.Data[0];
		for (j = 0; j < iNoParams - 1; j++) {
			if (dMGrid.Data[i * iNoParams + j] > pParamValues[j] + pTolerances[j]) {
				dMGrid.Data[i * iNoParams + j] = dM_z_Values(j, 0);  //reset 
				dMGrid.Data[i * iNoParams + (j + 1)] += dVStepSize.Data[j + 1];
			}//end if 
		}//layers j
	}//parameter combinations i 

#ifdef OLOVIA_DEBUG
	ofstream ofGridSearch("GridSearch.txt");
	ofGridSearch << "dM_z_Values(i, j) : " << endl;
	for (i = 0; i < dM_z_Values.Nown; i++) {
		for (j = 0; j < dM_z_Values.Next; j++)
			ofGridSearch << dM_z_Values(i, j) << " \t";
		ofGridSearch << endl;
	}//end for i
	ofGridSearch << endl << endl << "dMGrid(i, j) : " << endl;
	for (i = 0; i < dMGrid.Nown; i++) {
		for (j = 0; j < dMGrid.Next; j++)
			ofGridSearch << dMGrid(i, j) << " \t";
		ofGridSearch << endl;
	}//end for i 
#endif  
}



void vCheckInitValuesOfGridSearch(int iNo_layers, sAdditionalData_LISE_ED *pInfo, int iDim_z, double *pdFuVal,  
	double dThreshold_peak, bool bNewPeakDetection, MinData_double *pMinInit)
{
	int i, j, iNo_detected_peaks, iIndexL, iIndexR;
	double dDelta_z_Half = 0.5 * pInfo->pDirSpace->dDelta_z; 
	double dSafetyMargin = 1.0 - cos(dDelta_z_Half * PI / pInfo->dFWHM_Peak);  dThreshold_peak *= 1.0 - (2.0 * dSafetyMargin); //dSafetyMargin is approx. 0.98 
	double dHalfWidthOfPeak; 
	
	if (bNewPeakDetection)
		dHalfWidthOfPeak = 0.3 * pInfo->dFWHM_Peak; 
	else 
		dHalfWidthOfPeak = 0.6 * pInfo->dFWHM_Peak; 
	iNo_detected_peaks = 0; 
	for (i = 0; i < iNo_layers; i++) {
		iIndexL = (int)((pMinInit->dVParam.Data[2 * i] - dHalfWidthOfPeak) / dDelta_z_Half);		iIndexL = max(0, iIndexL);
		iIndexR = (int)((pMinInit->dVParam.Data[2 * i] + dHalfWidthOfPeak) / dDelta_z_Half) + 1;	iIndexR = min(iIndexR, iDim_z - 1);
		for (j = iIndexL; j <= iIndexR; j++) {
			if (pdFuVal[j] >= dThreshold_peak) {
				iNo_detected_peaks++;  break; 
			}
		}//j
	}//i
	if (iNo_detected_peaks < iNo_layers) 
		pMinInit->bLocMin = false;
}



bool bGridSearch_LISE_ED_MODE(int iNo_layers, double dzWindowRight, double *pOpticalThicknesses, double *pOptThicknessTolerances,
	int iNoOptParam, bool *pOptParam, double dTolerance_DLS, double *pModulus_FT_local, sAdditionalData_LISE_ED *pInfo, MinData_double *pMinResult)
{
	int i, j, iIndexOfGlobMin = -1;  double dNormOfGlobMin = DBL_MAX, dz_Position_Peak, dStepsize_z = 0.48 * pInfo->dFWHM_Peak;
	dMatrix dMGrid;

	vSetGridForGridSearch(iNo_layers, dzWindowRight, dStepsize_z, pOpticalThicknesses, pOptThicknessTolerances, dMGrid);

	bool bPositiveLayerThickness = false;
	dVector dVCompOpticalThicknesses(iNo_layers);   MinData_double *pMin = pAllocMinDataArray(dMGrid.Nown, iNoOptParam);
	for (i = 0; i < dMGrid.Nown; i++) { //for all initial values 
		dz_Position_Peak = 0.0;
		for (j = 0; j < iNo_layers; j++) {
			if (dMGrid(i, j) < -10.0 * NUM_NOISE_FLOAT) //if a layer thickness is clearly negative (smaller than -0.1 nm) 
				bPositiveLayerThickness = false;
			else {
				bPositiveLayerThickness = true;
				dz_Position_Peak += dMGrid(i, j);   pMin[i].dVParam.Data[2 * j] = dz_Position_Peak;   pMin[i].dVParam.Data[2 * j + AMPL_TSV] = 0.0;
			}
		}//end for all layer j
		if (bPositiveLayerThickness == true) {
			pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, NULL, NULL, MU_START_DLS_LISE, true, dTolerance_DLS, 3, 300, 1.0e-6,
				pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_ED_Mode, (void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
			if (pMin[i].bLocMin == true) {
				dVCompOpticalThicknesses.Data[0] = pMin[i].dVParam.Data[0];
				for (j = 1; j < iNo_layers; j++)
					dVCompOpticalThicknesses.Data[j] = pMin[i].dVParam.Data[2 * j] - pMin[i].dVParam.Data[2 * (j - 1)];
				if (bApplyConstraints(iNo_layers, pOpticalThicknesses, pOptThicknessTolerances, dVCompOpticalThicknesses.Data) == true)
					if (pMin[i].dNorm < dNormOfGlobMin) {
						dNormOfGlobMin = pMin[i].dNorm;   iIndexOfGlobMin = i;
					}
			}//end if loc. min. 
		}//end if positive layer thickness 
	}//end for all init. values  
	if (iIndexOfGlobMin > -1)
		*pMinResult = pMin[iIndexOfGlobMin];
	vDeallocMinDataArray(&pMin);
	if (iIndexOfGlobMin > -1) return true;   else  return false;
}




bool bGridSearch_LISE_HF(int iNo_layers, double dzWindowRight, double *pOpticalThicknesses, double *pOptThicknessTolerances,
	double *pModulus_FT_local, sAdditionalData_LISE_ED *pInfo,
	int iFT_dim, double *pz_glob, double *pModulus_of_FT_glob, double dThreshold_peak, bool bPeakDetectionOnRight, bool bNewPeakDetection, 
	MinData_double *pMinResult)
{
	bool *pOptParam = new bool[pMinResult->dVParam.Nown]; 
	int i, j, iIndexOfGlobMin = -1, iNoValidInitVectors = 0;  
	double dNormOfGlobMin = DBL_MAX, dz_Position_Peak, dStepsize_z;  
	dVector dVLowerLimit(pMinResult->dVParam.Nown);  dVLowerLimit = 0.0; 
	dMatrix dMGrid;
	
	bNewPeakDetection ? (dStepsize_z = 0.3 * pInfo->dFWHM_Peak) : (dStepsize_z = 0.48 * pInfo->dFWHM_Peak); 
	vSetGridForGridSearch(iNo_layers, dzWindowRight, dStepsize_z, pOpticalThicknesses, pOptThicknessTolerances, dMGrid);

	dVector dVzInterpol(2 * iFT_dim - 1), dVInterpolVal(2 * iFT_dim - 1); 
	for (i = 0; i < iFT_dim - 1; i++) {
		dVzInterpol.Data[2 * i] = pz_glob[i];  dVInterpolVal.Data[2 * i] = pModulus_of_FT_glob[i];
		dVzInterpol.Data[2 * i + 1] = pz_glob[i] + pInfo->pDirSpace->dDelta_z/2.0;
		int iIndexLL = (int)(dVzInterpol.Data[2 * i + 1] / pInfo->pDirSpace->dDelta_z) - 1;   iIndexLL = max(iIndexLL, 0);
		if (!bParabolicFit_4(dVzInterpol.Data[2 * i + 1], iIndexLL, iFT_dim - 1, pz_glob + iIndexLL, pModulus_of_FT_glob + iIndexLL,
			dVInterpolVal.Data + 2 * i + 1, NULL, NULL))
			dVInterpolVal.Data[2 * i + 1] = max(pModulus_of_FT_glob[i], pModulus_of_FT_glob[i + 1]);
	}
	dVzInterpol.Data[2 * i] = pz_glob[i];  dVInterpolVal.Data[2 * i] = pModulus_of_FT_glob[i];

	dVector dVCompOpticalThicknesses(iNo_layers);   MinData_double *pMin = pAllocMinDataArray(dMGrid.Nown, 2 * iNo_layers); 
	for (i = 0; i < dMGrid.Nown; i++) { //for all initial values 
		pMin[i].bLocMin = true;   dz_Position_Peak = 0.0;
		for (j = 0; j < iNo_layers; j++) {
			if (dMGrid(i, j) < 0.2 * pInfo->dFWHM_Peak) { //if a layer thickness is clearly negative (smaller than -0.1 nm) 
				pMin[i].bLocMin = false;  break; 
			}
			else {
				dz_Position_Peak += dMGrid(i, j);   pMin[i].dVParam.Data[2 * j] = dz_Position_Peak;   pMin[i].dVParam.Data[2 * j + AMPL_TSV] = 0.0;
			}
		}//end for all layers j 
		if (pMin[i].bLocMin == true)
			vCheckInitValuesOfGridSearch(iNo_layers, pInfo, dVInterpolVal.Nown, dVInterpolVal.Data, dThreshold_peak, bNewPeakDetection, pMin + i);
		else
			continue; 
		if (pMin[i].bLocMin == true) { //if initial values are OK: 
			iNoValidInitVectors++; 
			double dTolerance_DLS = 0.0015; //DLS means damped least squares; dTolerance_DLS was 28.0 before Jul. 2024 for old peak fitting
			for (j = 0; j < iNo_layers; j++) {
				pOptParam[2 * j] = false;   pOptParam[2 * j + AMPL_TSV] = true;  //optimization of initial amplitudes 
			}
			if (bNewPeakDetection == true)
				pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 2.0*MU_START_DLS_LISE,
				true, 0.5, 2, 210, 1.0e-2, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_HF_WLSQ,
				(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
			else
				pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 2.0*MU_START_DLS_LISE,
				true, 0.5, 2, 210, 1.0e-2, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_ED_Mode,
				(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
			for (j = 0; j < iNo_layers; j++) 
				pOptParam[2 * j] = true;   //optimization of amplitudes and peak positions  
			if (bNewPeakDetection == true)
				pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 100.0 * MU_START_DLS_LISE,
				true, dTolerance_DLS, 3, 210, 2.0e-6, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_HF_WLSQ,
					(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
			else 
				pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 10.0 * MU_START_DLS_LISE,
				true, dTolerance_DLS, 3, 210, 2.0e-6, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_ED_Mode,
				(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i); 			
			if (pMin[i].bLocMin == true) { //if successful calc. 
				dVCompOpticalThicknesses.Data[0] = pMin[i].dVParam.Data[0];
				for (j = 1; j < iNo_layers; j++)
					dVCompOpticalThicknesses.Data[j] = pMin[i].dVParam.Data[2 * j] - pMin[i].dVParam.Data[2 * (j - 1)];
				for (j = 0; j < iNo_layers; j++) {
					if ( dVCompOpticalThicknesses.Data[j] < 0.5 * pInfo->dFWHM_Peak || 3.0 * pMin[i].dVStandardDev.Data[2 * j] > fabs(dVCompOpticalThicknesses.Data[j]) )
						pMin[i].bLocMin = false; 
				}
				if (bApplyConstraints(iNo_layers, pOpticalThicknesses, pOptThicknessTolerances, dVCompOpticalThicknesses.Data) == false)
					pMin[i].bLocMin = false;
				if (pMin[i].bLocMin == true) {
					int iNo_detected_peaks = 0;   double dAmplitude;    
					for (j = 0; j < iNo_layers; j++) {
						int iIndexLL = (int)(pMin[i].dVParam.Data[2 * j] / pInfo->pDirSpace->dDelta_z) - 1; 
						bParabolicFit_4(pMin[i].dVParam.Data[2 * j], iIndexLL, iFT_dim - 1, pz_glob + iIndexLL, pModulus_of_FT_glob + iIndexLL,
							&dAmplitude, NULL, NULL);
						if (pMin[i].dVParam.Data[2 * j + AMPL_TSV] * pInfo->pDirSpace->dAmpl_TSV_Si_Init > 0.48 * dAmplitude  &&  dAmplitude >= dThreshold_peak)
							iNo_detected_peaks++;
					}
					if (iNo_detected_peaks < iNo_layers)
						pMin[i].bLocMin = false;
				}//end if bLocMin 
			}//end if successful calc. 
		}//end if initial values are OK 
	}//end for all init. values i  
#ifdef OLOVIA_DEBUG
	cout << endl << "iNoValidInitVectors = " << iNoValidInitVectors << endl; 
#endif

	if (bPeakDetectionOnRight) {
		double dTotal_TSV_Depth = 0.0, dPrecision_z_LSQ = 0.004; //ensured precision of least squares procedure for calc of z-values
		for (i = 0; i < dMGrid.Nown; i++) { //for all initial values - find rightmost peak
			if (pMin[i].bLocMin)
				dTotal_TSV_Depth = max(dTotal_TSV_Depth, pMin[i].dVParam.Data[2 * (iNo_layers - 1)]);
		}
		for (i = 0; i < dMGrid.Nown; i++) { //for all initial values - check whether the rightmost peak has been detected 
			if (pMin[i].bLocMin)
				if (pMin[i].dVParam.Data[2 * (iNo_layers - 1)] < dTotal_TSV_Depth - dPrecision_z_LSQ)
					pMin[i].bLocMin = false;
		}
	}//end if rightmost peak detection 

	for (i = 0; i < dMGrid.Nown; i++) { //for all initial values - find least square solution among eligible solutions 
		if (pMin[i].bLocMin == true)
			if (pMin[i].dNorm < dNormOfGlobMin) {
				dNormOfGlobMin = pMin[i].dNorm;   iIndexOfGlobMin = i;
			}
	}//end for i
	if (iIndexOfGlobMin > -1)
		*pMinResult = pMin[iIndexOfGlobMin];
	vDeallocMinDataArray(&pMin);    delete[] pOptParam;  
	if (iIndexOfGlobMin > -1) return true;   else  return false;
}






bool bGridSearch_LISE_HF2(int iNo_layers, double dzWindowRight, double *pOpticalThicknesses, double *pOptThicknessTolerances,
	double *pModulus_FT_local, sAdditionalData_LISE_ED *pInfo,
	int iFT_dim, double *pz_glob, double *pModulus_of_FT_glob, double dThreshold_peak, bool bPeakDetectionOnRight, bool bNewPeakDetection,
	MinData_double *pMinResult) 
{
	bool *pOptParam = new bool[pMinResult->dVParam.Nown];
	int i, j, iIndexOfGlobMin = -1, iNoValidInitVectors = 0;
	double dNormOfGlobMin = DBL_MAX, dz_Position_Peak, dStepsize_z;
	dVector dVLowerLimit(pMinResult->dVParam.Nown);  dVLowerLimit = 0.0;
	dMatrix dMGrid;
	//variables for outer iterations: 
	int k, iMaxOuterIter = 80;  double dDiff, dLengthDiff, dStepsize, dTolOuterIter = 0.001, dStepSingleLayer = 0.0;
	const double dCriticalLength = 0.003; 
	dVector dVDeltaParam(pMinResult->dVParam.Nown);

	bNewPeakDetection ? (dStepsize_z = 0.36 * pInfo->dFWHM_Peak) : (dStepsize_z = 0.48 * pInfo->dFWHM_Peak);
	vSetGridForGridSearch(iNo_layers, dzWindowRight, dStepsize_z, pOpticalThicknesses, pOptThicknessTolerances, dMGrid);

	dVector dVzInterpol(2 * iFT_dim - 1), dVInterpolVal(2 * iFT_dim - 1);
	for (i = 0; i < iFT_dim - 1; i++) {
		dVzInterpol.Data[2 * i] = pz_glob[i];  dVInterpolVal.Data[2 * i] = pModulus_of_FT_glob[i];
		dVzInterpol.Data[2 * i + 1] = pz_glob[i] + pInfo->pDirSpace->dDelta_z / 2.0;
		int iIndexLL = (int)(dVzInterpol.Data[2 * i + 1] / pInfo->pDirSpace->dDelta_z) - 1;   iIndexLL = max(iIndexLL, 0);
		if (!bParabolicFit_4(dVzInterpol.Data[2 * i + 1], iIndexLL, iFT_dim - 1, pz_glob + iIndexLL, pModulus_of_FT_glob + iIndexLL,
			dVInterpolVal.Data + 2 * i + 1, NULL, NULL))
			dVInterpolVal.Data[2 * i + 1] = max(pModulus_of_FT_glob[i], pModulus_of_FT_glob[i + 1]);
	}
	dVzInterpol.Data[2 * i] = pz_glob[i];  dVInterpolVal.Data[2 * i] = pModulus_of_FT_glob[i];

	dVector dVCompOpticalThicknesses(iNo_layers);   MinData_double *pMin = pAllocMinDataArray(dMGrid.Nown, 2 * iNo_layers);
	MinData_double *pMinCopy = pAllocMinDataArray(dMGrid.Nown, 2 * iNo_layers);  
	for (i = 0; i < dMGrid.Nown; i++) { //for all initial values i
		pMin[i].bLocMin = true;		pMinCopy[i].bLocMin = false;	dz_Position_Peak = 0.0; 
		for (j = 0; j < iNo_layers; j++) {
			if (dMGrid(i, j) < 0.2 * pInfo->dFWHM_Peak) { //if a layer thickness is clearly negative (smaller than -0.1 nm) 
				pMin[i].bLocMin = false;  break;
			}
			else {
				dz_Position_Peak += dMGrid(i, j);   pMin[i].dVParam.Data[2 * j] = dz_Position_Peak;   pMin[i].dVParam.Data[2 * j + AMPL_TSV] = 0.0;
			}
		}//end for all layers j 
		if (pMin[i].bLocMin == false)
			continue;
		vCheckInitValuesOfGridSearch(iNo_layers, pInfo, dVInterpolVal.Nown, dVInterpolVal.Data, dThreshold_peak, bNewPeakDetection, pMin + i);
		if (pMin[i].bLocMin == false)
			continue;

		iNoValidInitVectors++;
		for (j = 0; j < pInfo->pDirSpace->iDim_z; j++) //reset weights to static weights:
			pInfo->pDirSpace->pdWeightsInTotal[j] = pInfo->pDirSpace->pdStaticWeights[j]; 
		double dTolerance_DLS = 0.0015; //DLS means damped least squares; dTolerance_DLS was 28.0 before Jul. 2024 for old peak fitting
		for (j = 0; j < iNo_layers; j++) {
			pOptParam[2 * j] = false;   pOptParam[2 * j + AMPL_TSV] = true;  //optimization of initial amplitudes 
		}
		if (bNewPeakDetection == true)
			pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 2.0*MU_START_DLS_LISE,
			true, 0.5, 2, 210, 1.0e-2, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_HF_WLSQ,
			(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
		else
			pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 2.0*MU_START_DLS_LISE,
			true, 0.5, 2, 210, 1.0e-2, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_ED_Mode,
			(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
		for (j = 0; j < iNo_layers; j++)
			pOptParam[2 * j] = true;   //optimization of amplitudes and peak positions  
		if (bNewPeakDetection == true)
			pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 100.0 * MU_START_DLS_LISE,
			true, dTolerance_DLS, 3, 210, 2.0e-6, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_HF_WLSQ,
			(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
		else
			pMin[i].bLocMin = bLevenbergMarquardt_double(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 10.0 * MU_START_DLS_LISE,
			true, dTolerance_DLS, 3, 210, 2.0e-6, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_ED_Mode,
			(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
		pMinCopy[i] = pMin[i]; 
		if (pMin[i].bLocMin == false)
			continue;

		if (bNewPeakDetection) {

			if (pMin[i].bLocMin == true) { 
				for (j = 0; j < i; j++) { //avoid calc. again with same init. values 
					if (pMinCopy[j].bLocMin == true) { //if loc. min. j
						dLengthDiff = 0.0;
						for (k = 0; k < iNo_layers; k++) {
							dDiff = pMin[i].dVParam.Data[2 * k] - pMinCopy[j].dVParam.Data[2 * k];   dLengthDiff += dDiff * dDiff;
						}//k
						dLengthDiff = sqrt(dLengthDiff);
						if (dLengthDiff < dCriticalLength)
							pMin[i].bLocMin = false; 
					}//end if loc. min. j
				}//j
			}//end if loc. min.
			if (pMin[i].bLocMin == false)
				continue;

			dVCompOpticalThicknesses.Data[0] = pMin[i].dVParam.Data[0]; //pMinTemp = pMin[i];  
			for (j = 1; j < iNo_layers; j++)
				dVCompOpticalThicknesses.Data[j] = pMin[i].dVParam.Data[2 * j] - pMin[i].dVParam.Data[2 * (j - 1)];
			if (bApplyConstraints2(iNo_layers, pOpticalThicknesses, pOptThicknessTolerances, dVCompOpticalThicknesses.Data, pInfo->dFWHM_Peak) == false)
				pMin[i].bLocMin = false;
			if (pMin[i].bLocMin == true) {
				int iNo_detected_peaks = 0;   double dAmplitude;
				for (j = 0; j < iNo_layers; j++) {
					int iIndexLL = (int)(pMin[i].dVParam.Data[2 * j] / pInfo->pDirSpace->dDelta_z) - 1;
					bParabolicFit_4(pMin[i].dVParam.Data[2 * j], iIndexLL, iFT_dim - 1, pz_glob + iIndexLL, pModulus_of_FT_glob + iIndexLL,
						&dAmplitude, NULL, NULL);
					if (pMin[i].dVParam.Data[2 * j + AMPL_TSV] * pInfo->pDirSpace->dAmpl_TSV_Si_Init > 0.48 * dAmplitude * 0.5  &&  dAmplitude >= dThreshold_peak * 0.5)
						iNo_detected_peaks++;
				}
				if (iNo_detected_peaks < iNo_layers)
					pMin[i].bLocMin = false;
			}//end if bLocMin of first calc. 
			if (pMin[i].bLocMin == false)
				continue;

			dStepSingleLayer = 0.0; 
			for (k = 0; k < iMaxOuterIter; k++) {
				dVDeltaParam = pMin[i].dVParam;   pInfo->pDirSpace->bUpdateWeights = true; 
				pMin[i].bLocMin = bLevenbergMarquardt_double2(pMin[i].dVParam, pOptParam, NULL, NULL, &dVLowerLimit, NULL, 200.0 * MU_START_DLS_LISE,
					true, dTolerance_DLS, 3, 210, 2.0e-6, pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_HF_WLSQ2,
					(void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMin + i);
				if (pMin[i].bLocMin == true) { //if successful 2nd calc. 
					dVCompOpticalThicknesses.Data[0] = pMin[i].dVParam.Data[0];
					for (j = 1; j < iNo_layers; j++)
						dVCompOpticalThicknesses.Data[j] = pMin[i].dVParam.Data[2 * j] - pMin[i].dVParam.Data[2 * (j - 1)];

					if (bApplyConstraints2(iNo_layers, pOpticalThicknesses, pOptThicknessTolerances, dVCompOpticalThicknesses.Data, pInfo->dFWHM_Peak) == false)
						pMin[i].bLocMin = false;
					if (pMin[i].bLocMin == true) {
						int iNo_detected_peaks = 0;   double dAmplitude;
						for (j = 0; j < iNo_layers; j++) {
							int iIndexLL = (int)(pMin[i].dVParam.Data[2 * j] / pInfo->pDirSpace->dDelta_z) - 1;
							bParabolicFit_4(pMin[i].dVParam.Data[2 * j], iIndexLL, iFT_dim - 1, pz_glob + iIndexLL, pModulus_of_FT_glob + iIndexLL,
								&dAmplitude, NULL, NULL);
							if (pMin[i].dVParam.Data[2 * j + AMPL_TSV] * pInfo->pDirSpace->dAmpl_TSV_Si_Init > 0.48 * dAmplitude * 0.5  &&  dAmplitude >= dThreshold_peak * 0.66)
								iNo_detected_peaks++;
						}//j
						if (iNo_detected_peaks < iNo_layers)
							pMin[i].bLocMin = false;
					}//end if
					if (pMin[i].bLocMin == false)
						break;

					dVDeltaParam -= pMin[i].dVParam;    dStepsize = 0.0;   
					for (j = 0; j < iNo_layers; j++) {
						dStepsize += dVDeltaParam.Data[2 * j] * dVDeltaParam.Data[2 * j];  
						dStepSingleLayer = max(dStepSingleLayer, fabs(dVDeltaParam.Data[2 * j]));  
					}//j
					dStepsize = sqrt(dStepsize); 
					if (dStepsize < dTolOuterIter)
						break; 

				}//end if successful 2nd calc. 
				if (pMin[i].bLocMin == false)
					break;
			}//end for k
			if (k >= iMaxOuterIter || dStepSingleLayer > 0.67 * pInfo->dFWHM_Peak)
				pMin[i].bLocMin = false;
		}//end if bNewPeakDetection 
		if (pMin[i].bLocMin == false)
			continue; 

		dVCompOpticalThicknesses.Data[0] = pMin[i].dVParam.Data[0];
		for (j = 1; j < iNo_layers; j++)
			dVCompOpticalThicknesses.Data[j] = pMin[i].dVParam.Data[2 * j] - pMin[i].dVParam.Data[2 * (j - 1)];
		for (j = 0; j < iNo_layers; j++) {
			if (dVCompOpticalThicknesses.Data[j] < 0.5 * pInfo->dFWHM_Peak || 3.0 * pMin[i].dVStandardDev.Data[2 * j] > fabs(dVCompOpticalThicknesses.Data[j]))
				pMin[i].bLocMin = false;
		}
		if (bApplyConstraints(iNo_layers, pOpticalThicknesses, pOptThicknessTolerances, dVCompOpticalThicknesses.Data) == false)
			pMin[i].bLocMin = false;
		if (pMin[i].bLocMin == true) {
			int iNo_detected_peaks = 0;   double dAmplitude;
			for (j = 0; j < iNo_layers; j++) {
				int iIndexLL = (int)(pMin[i].dVParam.Data[2 * j] / pInfo->pDirSpace->dDelta_z) - 1;
				bParabolicFit_4(pMin[i].dVParam.Data[2 * j], iIndexLL, iFT_dim - 1, pz_glob + iIndexLL, pModulus_of_FT_glob + iIndexLL,
					&dAmplitude, NULL, NULL);
				if (pMin[i].dVParam.Data[2 * j + AMPL_TSV] * pInfo->pDirSpace->dAmpl_TSV_Si_Init > 0.48 * dAmplitude  &&  dAmplitude >= dThreshold_peak)
					iNo_detected_peaks++;
			}
			if (iNo_detected_peaks < iNo_layers)
				pMin[i].bLocMin = false;
		}//end if bLocMin of first calc. 

	}//end for all init. values i 
 
#ifdef OLOVIA_DEBUG
	cout << endl << "iNoValidInitVectors = " << iNoValidInitVectors << endl;
#endif

	if (bPeakDetectionOnRight) {
		double dTotal_TSV_Depth = 0.0, dPrecision_z_LSQ = 0.004; //ensured precision of least squares procedure for calc of z-values
		for (i = 0; i < dMGrid.Nown; i++) { //for all initial values - find rightmost peak
			if (pMin[i].bLocMin)
				dTotal_TSV_Depth = max(dTotal_TSV_Depth, pMin[i].dVParam.Data[2 * (iNo_layers - 1)]);
		}
		for (i = 0; i < dMGrid.Nown; i++) { //for all initial values - check whether the rightmost peak has been detected 
			if (pMin[i].bLocMin)
				if (pMin[i].dVParam.Data[2 * (iNo_layers - 1)] < dTotal_TSV_Depth - dPrecision_z_LSQ)
					pMin[i].bLocMin = false;
		}
	}//end if rightmost peak detection 

	for (i = 0; i < dMGrid.Nown; i++) { //for all initial values - find least square solution among eligible solutions 
		if (pMin[i].bLocMin == true)
			if (pMin[i].dNorm < dNormOfGlobMin) {
				dNormOfGlobMin = pMin[i].dNorm;   iIndexOfGlobMin = i;
			}
	}//end for i
	if (iIndexOfGlobMin > -1)
		*pMinResult = pMin[iIndexOfGlobMin];
	vDeallocMinDataArray(&pMin);	vDeallocMinDataArray(&pMinCopy);    delete[] pOptParam;
	if (iIndexOfGlobMin > -1) return true;   else  return false;
}







bool bOptimizePeakPositions_LISE_ED_Mode(int iNo_layers, double *pOpticalThicknesses, double *pOptThicknessTolerances,
	bool *pOptParam, double dTolerance_DLS, double *pModulus_FT_local, sAdditionalData_LISE_ED *pInfo, MinData_double *pMinResult)
{
	int i;
	dVector dVCompOpticalThicknesses(iNo_layers);

	pMinResult->bLocMin = bLevenbergMarquardt_double(pMinResult->dVParam, pOptParam, NULL, NULL, NULL, NULL, MU_START_DLS_LISE, true, dTolerance_DLS, 3, 300, 1.0e-6,
		pInfo->pDirSpace->iDim_z, vIntAndDeriv_TSV_Dir_Space_LISE_ED_Mode, (void*)pModulus_FT_local, (void*)pInfo, (void*)&iNo_layers, pMinResult);
	if (pMinResult->bLocMin == false)
		return false;
	dVCompOpticalThicknesses.Data[0] = pMinResult->dVParam.Data[0];
	for (i = 1; i < iNo_layers; i++)
		dVCompOpticalThicknesses.Data[i] = pMinResult->dVParam.Data[2 * i] - pMinResult->dVParam.Data[2 * (i - 1)];
	if (bApplyConstraints(iNo_layers, pOpticalThicknesses, pOptThicknessTolerances, dVCompOpticalThicknesses.Data) == false)
		return false;
	return true;
}




bool bFindPeaks_LISE_HF_LSQ(int iFT_dim, double dDelta_z, double dzWindowLeft, double dzWindowRight, 
	double *pz, double *pModulus_of_FT, int iNo_layers, double *pOpticalThicknesses, double *pComputed_optical_thicknesses, 
	double *pOptThicknessTolerances, double *pAmplitudes, double dMaxAmplitude, double dThreshold_peak, double dFWHM_peak,
	bool bPeakDetectionOnRight, bool bNewPeakDetection, double &dAsymptStdErr, double &dNormalizedResidual, double &dQuality, 
	int &iNo_detected_peaks, stringstream *pssMessages)
//Find the peaks in the spectrum precisely by least squares procedure (LSQ). 7.8.2022
{
	//1) Set weights for least squares procedure: 
	int iStart, iStop;
	dVector dV_StaticWeights(iFT_dim);

	vSet_Static_Weights_In_Direct_Space_TSV(iFT_dim, dDelta_z, dzWindowLeft, dzWindowRight, dFWHM_peak,
		bNewPeakDetection, pModulus_of_FT, dThreshold_peak, iStart, iStop, dV_StaticWeights.Data);
	LISE_HF_Dir_Space sDirSpace(iStart, iStop, dDelta_z, dV_StaticWeights.Data);    sAdditionalData_LISE_ED sInfo(&sDirSpace, dFWHM_peak);

	//2a) Calc. 2-norm of function to fit (for normalisation of residual later):   
	int i, iNoOptParam = 2 * iNo_layers; //layer thicknesses (depths), amplitudes, FWHM of the peaks in addition.    
	bool bSuccess = false;
	double dMinAmplitude = 0.0;    
	double *pModulus_FT_local = pModulus_of_FT + sDirSpace.iStart, dNorm_FT_in_window; //pModulus of FT in local coordinates  
	dVector dV_Modulus_FT_in_window(sDirSpace.iDim_z); //intensity in computational window 
	MinData_double sMin(iNoOptParam); 
	
	if (dMaxAmplitude < NUM_NOISE) {
		*pssMessages << "Error : Max. amplitude is below NOISE zone - cannot analyse signal." << endl;   return false;
	}
	sDirSpace.dAmpl_TSV_Si_Init = dMaxAmplitude;  //max Amplitude will be normalized to 1 for least squares procedure 
	for (i = 0; i < sDirSpace.iDim_z; i++) {
		dV_Modulus_FT_in_window.Data[i] = pModulus_FT_local[i] * sDirSpace.pdWeightsInTotal[i]; 
	}
	dNorm_FT_in_window = dV_Modulus_FT_in_window.norm(); 

	//2b) Apply least squares procedure(s):  
	bSuccess = bGridSearch_LISE_HF2(iNo_layers, dzWindowRight, pOpticalThicknesses, pOptThicknessTolerances,
		pModulus_FT_local, &sInfo, iFT_dim, pz, pModulus_of_FT, dThreshold_peak, bPeakDetectionOnRight, bNewPeakDetection, &sMin);  
	bSuccess == true ? (iNo_detected_peaks = iNo_layers) : (iNo_detected_peaks = 0);  
	//3. Output and evaluation of the results:  
	if (bSuccess) {
		pComputed_optical_thicknesses[0] = sMin.dVParam.Data[0];   pAmplitudes[0] = sMin.dVParam.Data[AMPL_TSV] * sDirSpace.dAmpl_TSV_Si_Init;
		for (i = 1; i < iNo_layers; i++) {
			pComputed_optical_thicknesses[i] = sMin.dVParam.Data[2 * i] - sMin.dVParam.Data[2 * (i - 1)];
			pAmplitudes[i] = sMin.dVParam.Data[2 * i + AMPL_TSV] * sDirSpace.dAmpl_TSV_Si_Init;
		}
		dAsymptStdErr = 0.0;
		for (i = 0; i < iNo_layers; i++)
			dAsymptStdErr += sMin.dVStandardDev.Data[2 * i] * sMin.dVStandardDev.Data[2 * i];
		dAsymptStdErr = sqrt(dAsymptStdErr / iNo_layers);   
		dNormalizedResidual = sMin.dNorm / dNorm_FT_in_window;
		dMinAmplitude = 0.0;
		for (i = 0; i < iNo_layers; i++)
			dMinAmplitude += 1.0 / (pAmplitudes[i] * pAmplitudes[i] + FLT_MIN); 
		dMinAmplitude /= iNo_layers;
		dMinAmplitude = 1.0 / sqrt(dMinAmplitude);
		dQuality = sqrt(dMinAmplitude) / (dNormalizedResidual + NUM_NOISE);
	}//end if bSuccess 
	else {
		*pssMessages << "Error: Not all interfaces (layers) have been detected successfully." << endl;
		*pssMessages << "You have the following possibilities: Modify the threshold, modify the tolerances for the depths (thicknesses), "
			"and model overlapping or neighboured peaks by 2 interfaces (layers)." << endl; 
			*pssMessages << "The failure happens with the following input: " << endl; 
			*pssMessages << "Signal processing version: " << bNewPeakDetection << ", rightmost peak detection: " << bPeakDetectionOnRight << endl; 
			*pssMessages << "Peak threshold: " << dThreshold_peak << ", FWHM: " << dFWHM_peak << ", no.layers: " << iNo_layers << endl; 
			*pssMessages << "Optical thicknesses and tolerances (Input): " << endl; 
		for (i = 0; i < iNo_layers; i++) 
			*pssMessages << pOpticalThicknesses[i] << " \t " << pOptThicknessTolerances[i] << endl;
		*pssMessages << endl << "z in micron and signal: " << endl;
		for (i = iStart; i <= iStop; i++) { 
			*pssMessages << i * dDelta_z << " \t " << pModulus_FT_local[i - iStart] << endl;
		}//i
		*pssMessages << endl; 
	}//end else 
	return bSuccess; 
}





bool bIdentify_Glob_Min(int& iNoMin, MinData_double **ppMin, double dToleranceMerit, MinData_double *pMinResult, dVector *dV_Difference_Between_Solutions,
	char szAdditional_message[], int iNo_char)
	//POST-PROCESSING : Find global minimum in the array of local minima.  W. Iff, 11.10.2021 
{
	bool bSuccess = true;   int i, j;   double dConfidenceInterval1 = 3.0, dConfidenceInterval2 = 10.0;

	vSelectMinimaOfMeritFu(dToleranceMerit, iNoMin, ppMin);
	if (iNoMin <= 0)
		return false;

	//Average, if more than one parameter combination matches:  
	MinData_double *pMin = *ppMin;
	pMinResult->dVParam = 0.0;   pMinResult->dVStandardDev = 0.0;   pMinResult->dNorm = 0;
	for (i = 0; i < iNoMin; i++) {
		pMinResult->dVParam += pMin[i].dVParam;   pMinResult->dNorm += pMin[i].dNorm * pMin[i].dNorm;
		for (j = 0; j < pMinResult->dVParam.Nown; j++)
			pMinResult->dVStandardDev.Data[j] += pMin[i].dVStandardDev.Data[j] * pMin[i].dVStandardDev.Data[j];
	}
	pMinResult->dVParam /= (double)iNoMin;     pMinResult->dNorm = sqrt(pMinResult->dNorm / (double)iNoMin);
	for (j = 0; j < pMinResult->dVParam.Nown; j++)
		pMinResult->dVStandardDev.Data[j] = sqrt(pMinResult->dVStandardDev.Data[j] / (double)iNoMin);

	if (iNoMin > 1) {
		double dDifference;  dVector dVStandardDev2(pMinResult->dVParam.Nown);
		for (j = 0; j < (int)pMinResult->dVParam.Nown; j++) {
			dVStandardDev2.Data[j] = 0.0;   dV_Difference_Between_Solutions->Data[j] = 0.0;
			for (i = 0; i < iNoMin; i++) {
				dDifference = abs(pMin[i].dVParam.Data[j] - pMinResult->dVParam.Data[j]);
				dV_Difference_Between_Solutions->Data[j] = max(dV_Difference_Between_Solutions->Data[j], dDifference);
				dVStandardDev2.Data[j] += dDifference * dDifference;
			}
			dVStandardDev2.Data[j] /= (double)(iNoMin - 1);
			dVStandardDev2.Data[j] = sqrt(dVStandardDev2.Data[j]);
		}
		for (j = 0; j < (int)pMinResult->dVParam.Nown; j++) {
			if (dV_Difference_Between_Solutions->Data[j] > dConfidenceInterval1 * abs(pMinResult->dVStandardDev.Data[j]))
				sprintf_s(szAdditional_message, iNo_char * sizeof(char), " Warning: more than one combination of parameters matches the measurement data! ");
			if (dV_Difference_Between_Solutions->Data[j] > dConfidenceInterval2 * abs(pMinResult->dVStandardDev.Data[j])) {
				sprintf_s(szAdditional_message, iNo_char * sizeof(char), " More than one combination of parameters matches the measurement data! ");
				bSuccess = false;
			}
		}
		for (j = 0; j < pMinResult->dVParam.Nown; j++)
			pMinResult->dVStandardDev.Data[j] = sqrt(pMinResult->dVStandardDev.Data[j] * pMinResult->dVStandardDev.Data[j] + dVStandardDev2.Data[j] * dVStandardDev2.Data[j]);
	}
	else
		*dV_Difference_Between_Solutions = 0.0;
	return bSuccess;
}



void vSetInitVectorsForDLS(int iNo_analyzed_peaks, double *pdPositions_of_peaks, double *pdAmplitudes_of_peaks, double dFWHM,
	int iNo_layers_on_top, double *pGeom_layer_thicknesses, Fresnel_TSV *pFresnel,
	double dThresholdPeakSubst, double dThresholdPeakTop, int& iNoOfInitVectors, dVector **ppdV_InitVector)
{
	int i, j, m, iIndex = 0, iNoInitValuesLeft = 0, iNoInitValuesRight = 0, iNoOfInitVectorsTemp = (iNo_layers_on_top + 1) * (iNo_layers_on_top + 1) + 1;
	double dOpticalThickness_Tot = 0.0, dDelta_z, dMappingFactor, dz_Expected_Peak;
	double dz_Average_Measured = 0.0, dSum_Of_Weights_Measured = 0.0, dz_Center_Expected_Loc_Coord = 0.0, dSum_Of_Weights_Expected = 0.0;
	dVector dVOptical_Thicknesses(iNo_layers_on_top), dVDifference(iNo_layers_on_top), dVInitValuesLeft(iNo_analyzed_peaks), dVInitValuesRight(iNo_analyzed_peaks);
	dVector *pdV_InitVector = new dVector[iNoOfInitVectorsTemp];
	for (i = 0; i < iNoOfInitVectorsTemp; i++)
		pdV_InitVector[i].Init(iNo_layers_on_top + 1);

	for (i = 0; i < iNo_layers_on_top; i++) {
		dVOptical_Thicknesses.Data[i] = pGeom_layer_thicknesses[i] * pFresnel->pMulti->VRef_Ind.Data[i].real();   dOpticalThickness_Tot += dVOptical_Thicknesses.Data[i];
	}//i
	for (i = 0; i <= iNo_layers_on_top; i++) {
		if (pdAmplitudes_of_peaks[i] > dThresholdPeakSubst) {
			dVInitValuesLeft.Data[iNoInitValuesLeft] = pdPositions_of_peaks[i];  iNoInitValuesLeft++;  //peaks may get smaller to the right in general
		}
	}//i
	for (i = iNo_layers_on_top; i >= 0; i--) {
		if (pdAmplitudes_of_peaks[i] > dThresholdPeakTop) {
			dVInitValuesRight.Data[iNoInitValuesRight] = pdPositions_of_peaks[i];  iNoInitValuesRight++;
		}
	}//i 
	for (i = 0; i < iNoInitValuesLeft; i++) {
		for (j = 0; j < iNoInitValuesRight; j++) {
			dDelta_z = dVInitValuesRight.Data[j] - dVInitValuesLeft.Data[i];
			if (dDelta_z > -NUM_NOISE_FLOAT) { //if valid intial values
				pdV_InitVector[iIndex].Data[0] = dVInitValuesLeft.Data[i];   pdV_InitVector[iIndex].Data[iNo_layers_on_top] = dVInitValuesRight.Data[j];
				if (dDelta_z < dFWHM) { //non-resolvable peaks 
					pdV_InitVector[iIndex].Data[0] -= dFWHM / 4.0;   pdV_InitVector[iIndex].Data[iNo_layers_on_top] += dFWHM / 4.0;
					dDelta_z = pdV_InitVector[iIndex].Data[iNo_layers_on_top] - pdV_InitVector[iIndex].Data[0];
				}
				dMappingFactor = dOpticalThickness_Tot / dDelta_z;  dz_Expected_Peak = dVInitValuesLeft.Data[i];
				for (m = 1; m < iNo_layers_on_top; m++) {
					dz_Expected_Peak += dVOptical_Thicknesses.Data[m] * dMappingFactor;  pdV_InitVector[iIndex].Data[m] = dz_Expected_Peak;
				}//m 		
				iIndex++;
			}//end if valid intial values
		}//j 
	}//i

	for (i = 0; i <= iNo_analyzed_peaks; i++) {
		dz_Average_Measured += pdAmplitudes_of_peaks[i] * pdPositions_of_peaks[i];   dSum_Of_Weights_Measured += pdAmplitudes_of_peaks[i];
	}
	dz_Average_Measured /= dSum_Of_Weights_Measured;
	dSum_Of_Weights_Expected = pFresnel->pMulti->dVAmplFresnel.Data[0]; //the local origin is at the interface between substrate and coatings
	for (i = 0; i <= iNo_layers_on_top; i++) {
		dz_Center_Expected_Loc_Coord += dVOptical_Thicknesses.Data[i] * pFresnel->pMulti->dVAmplFresnel.Data[1 + i];
		dSum_Of_Weights_Expected += pFresnel->pMulti->dVAmplFresnel.Data[i];
	}
	dz_Center_Expected_Loc_Coord /= dSum_Of_Weights_Expected;   dz_Expected_Peak = dz_Average_Measured - dz_Center_Expected_Loc_Coord;
	for (i = 0; i <= iNo_layers_on_top; i++) {
		pdV_InitVector[iIndex].Data[i] = dVOptical_Thicknesses.Data[i] - dz_Expected_Peak;
	}
	iIndex++;

	*ppdV_InitVector = new dVector[iIndex];   iNoOfInitVectors = iIndex;
	for (i = 0; i < iIndex; i++)
		(*ppdV_InitVector)[i] = pdV_InitVector[i];
	delete[] pdV_InitVector;
}




void vCheck_Input_Of_LISE_HF_DLL_2023(double dTSV_diam, int iNo_layers, double *pOptical_thicknesses, double *pOptical_thickness_tolerances, double *pRef_index_re,
	double *pRef_index_im, int iNo_pixels, double *pWavelength_nm, double *pDark_spectrum, double *pReference_spectrum, double *pSpectrum, int iFT_dim,
	int iOperating_Mode, double dThreshold_valid_signal, double dThreshold_peak, double dz_Resolution, stringstream *pssMessages,
	double *pModulus_of_FT, double *pz, double *pComputed_optical_thicknesses,
	double *pAmplitudes_of_peaks, double *pComputed_geom_thicknesses, bool &bFT_possible, bool &bSignal_analysis_possible)
{
	if (dTSV_diam < MIN_TSV_DIAMETER && iOperating_Mode != FT_ONLY_MODE) {
		*pssMessages << "Error: missing implementation for TSV diameters smaller than " << MIN_TSV_DIAMETER << " um" << endl;
		bSignal_analysis_possible = false;
	}
	if ((iNo_layers <= 0 || pOptical_thicknesses == NULL || pOptical_thickness_tolerances == NULL || pRef_index_re == NULL || pRef_index_im == NULL) &&
		iOperating_Mode != FT_ONLY_MODE) {
		*pssMessages << "Error: missing layer data (refractive indices or thicknesses)!" << endl;  bSignal_analysis_possible = false;
	}
	if (iNo_pixels < MIN_NO_PIXELS) {
		*pssMessages << "Error: too low number of detector pixels" << endl;   bFT_possible = bSignal_analysis_possible = false;
	}
	if (pWavelength_nm == NULL || pDark_spectrum == NULL || pReference_spectrum == NULL || pSpectrum == NULL) {
		*pssMessages << "Error: missing spectrometer data!" << endl;   bFT_possible = bSignal_analysis_possible = false;
	}
	if (iFT_dim < MIN_NO_PIXELS) {
		*pssMessages << "Error: Too small dimension for FFT!" << endl;  bFT_possible = bSignal_analysis_possible = false;
	}
	if (iOperating_Mode != FT_ONLY_MODE  &&  iOperating_Mode != GRID_SEARCH_MODE) {
		*pssMessages << "Error: unknown operational mode!" << endl;  bFT_possible = bSignal_analysis_possible = false;
	}
	if ((dThreshold_valid_signal <= NUM_NOISE || dThreshold_peak <= NUM_NOISE || dThreshold_peak >= 1.0 - NUM_NOISE_FLOAT) && iOperating_Mode != FT_ONLY_MODE) {
		*pssMessages << "Error: Too low or too high threshold for detection of signal!" << endl;  bSignal_analysis_possible = false;
	}
	if (dz_Resolution < 0 || dz_Resolution > 10.0 * MAX_DETECTABLE_DEPTH) {
		*pssMessages << "Error: Inadmissable input for z-resolution!" << endl;  bFT_possible = bSignal_analysis_possible = false;
	}
	if (pModulus_of_FT == NULL || pz == NULL) {
		*pssMessages << "Error: Missing array for modulus of FT or z-values!";  bFT_possible = bSignal_analysis_possible = false;
	}
	if ((pComputed_optical_thicknesses == NULL || pAmplitudes_of_peaks == NULL || pComputed_geom_thicknesses == NULL) && iOperating_Mode != FT_ONLY_MODE) {
		*pssMessages << "Missing arrays for output of results!" << endl;   bSignal_analysis_possible = false;
	}
}





void vLISE_HF_main(double dTSV_diam, int iNo_layers, double *pOptical_thicknesses, double *pOptical_thickness_tolerances, double *pRef_index_re, double *pRef_index_im,
	int iNo_pixels, double *pWavelength_nm, double *pDark_spectrum, double *pReference_spectrum, double *pSpectrum, double *pSpectrumPlanarLayers, int &iFT_dim,
	int iOperating_Mode, bool bPeakDetectionOnRight, bool bNewPeakDetection,  
	double dThreshold_valid_signal, double dThreshold_peak, double dz_Resolution, bool bDebugOutput, int iDebugFileNo,
	double &dAsymptStdErr, double &dNormalizedResidual, double &dQuality, stringstream *pssMessages, double *pModulus_of_FT, double *pz,
	int &iNo_detected_peaks, double *pComputed_optical_thicknesses, double *pAmplitudes_of_peaks, double *pComputed_geom_thicknesses,
	bool &bFT_done, bool &bSignal_analysis_done, double& dMaxPeakFFTAmplitudeinWindow)
//This function estimates the TSV depth from dark-, reference and TSV-spectrum. Documentation on this function can also be found on   
//"\\fmo-srv-sight.altatechsc.local\Nanovision\3MET2431_LiseHF_high_AR_structures\LiseHF_software". All lengths are in micrometers apart from the wavelength values 
//provided by the spectrometer in nm (they are converted in this program into micrometer).  
//Function arguments: Line 1 - 3 contain the input, line 4 - 5 contain the output.    
//dTSV_diam: diameter of the TSV (input),    iNo_layers: number of layers in total, including the TSV (the TSV is treated as a layer regarding data 
//structures; we count from the lowermost to the uppermost layer: the lowermost layer has the index [0]; the uppermost layer has the index [no. of layers - 1];  
//pOptical_thicknesses: optical thicknesses of these layers similar to the LISE-ED and FLARE sensor,    pOptical_thickness_tolerances: the optical thicknesses found by 
//this data analysis must be within pOptical_thicknesses[i] - pOptical_thickness_tolerances[i] and pOptical_thicknesses[i] + pOptical_thickness_tolerances[i]
//for layer no. i - otherwise, the result is invalid (same as for the LISE-ED and FLARE);     pRef_index_re: real part of the refractive indices of these layers (input);      
//pRef_index_re[0] is the medium surrounding the TSV,    pRef_index_im: imag part of these refractive indices (input),    iNo_pixels: Number of spectrometer pixels (number 
//of data points),    pWavelength_nm: wavelength values at these spectrometer pixels (in nm),    pDark_spectrum: spectrum recorded without any sample below the objective  
// (input),     pReference_spectrum: spectrum recorded at the planar substrate (silicon) at the reference position in advance,    pSpectrum: spectrum from the TSV,  
//pSpectrumPlanarLayers: Not yet used currently. The spectrum from the planar multilayer structure next to the TSV may be stored here.  
//iFT_dim: dimension of the Fourier-transformed data field (input) - the spectrum is embedded into a larger array of the dimension iFT_dim before the FFT is done,     
//iOperating_Mode: if 1 (GRID_SEARCH_MODE), data processing and output are as in case of the FLARE sensor (model-based approach); 
//more options can be provided in the future;    dThreshold_valid_signal: results are invalid when the highest peak of the FT to be 
//analyzed is smaller than dThreshold_valid_signal * peak height at z = 0,    dThreshold_peak: a local maximum of the FFT is treated as meaningful peak, when it has 
//at least the amplitude dThreshold_peak * amplitude of highest peak in the considered window (z-region);     double dz_Resolution: desired remaining z-resolution of the 
//FFT after smoothening; 0.2 µm: slight smoothening; 1.0 µm: strong smoothening; the smoothening removes noise and artefacts so that artificial peaks disappear;     
//bDebugOutput: if true, all in- and output is written to a file "LISE_HF_log_no_...txt";  iDebugFileNo: Number of this debug file.  
//dAsymptStdErr: asymptotic standard error (similar to a standard deviation) of the computed layer thicknesses (output),    dNormalizedResidual (output): residual 
//given by the Levenberg-Marquardt fitting procedure / 2-norm of function in the window to be fitted. A smaller value means a better fit (better match between simulation 
//and measurement),  dQuality: sqrt( amplitude of smallest fitted peaks) / (dNormalizedResidual + 1.0e-12); this is a measure for the signal quality,    
//*pssMessages: contains error messages in case of errors as well as warnings (output), pModulus_of_FT: 
//array containing the modulus of the entries of the Fourier transfom (output) - must have been allocated in the calling higher-level function already,  pz: array containing 
//the z-values which belong to the Fourier-transformed data (output) - must have been allocated before already,  iNo_detected_peaks: number of peaks in the spectrum, which 
//are in the considered z-window and above the threshold,   pComputed_optical_thicknesses: optical thicknesses of the layers obtained from the signal analysis (output),     
//pAmplitudes_of_peaks: amplitudes of the analysed peaks in the spectrum (output),  pComputed_geom_thicknesses: geometical thicknesses of the layers obtained from 
//the signal analysis (e.g. depth of the TSV in the silicon substrate); the TSV in silicon is counted as a layer here.   
//bFT_done: if true, the Fourier transform has been done successfully; bSignal_analysis_done:  if true, the signal analysis has been done successfully. 
//The user may discard the results of this function when dAsymptStdErr exceeds a certain user-defined threshold or the Qualíty (output) is too low. In that case, 
//it may be better to do another measurement at a slightly different position at the TSV and record another spectrum.   
//W. Iff, 30.8.2024 
{
	//Part 0: Checks, initialisations, debug output: 
	dMaxPeakFFTAmplitudeinWindow = 0.0;

	//0. Checks:  
	bFT_done = true;  //initialisation with true - set to false later in case of failure   
	bSignal_analysis_done = (iOperating_Mode == FT_ONLY_MODE ? false : true);  //initialisation with true - set to false later in case of failure 
	vCheck_Input_Of_LISE_HF_DLL_2023(dTSV_diam, iNo_layers, pOptical_thicknesses, pOptical_thickness_tolerances, pRef_index_re,
		pRef_index_im, iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, iFT_dim,
		iOperating_Mode, dThreshold_valid_signal, dThreshold_peak, dz_Resolution, pssMessages, 
		pModulus_of_FT, pz, pComputed_optical_thicknesses, pAmplitudes_of_peaks, pComputed_geom_thicknesses, bFT_done, bSignal_analysis_done);
	if (bFT_done == false)
		return;

	//1. Initialisations: 
	int i;
	dAsymptStdErr = DBL_MAX, dNormalizedResidual = DBL_MAX, dQuality = 0.0, iNo_detected_peaks = 0;
	if (pAmplitudes_of_peaks != NULL)
		for (i = 0; i < iNo_layers; i++) pAmplitudes_of_peaks[i] = 0.0;

	//2. Debug output:  
	ofstream ofDebug;	stringstream ssFileName;  ssFileName << "LISE_HF_log_no_" << iDebugFileNo << ".txt";
	if (bDebugOutput) {
		ofDebug.open(ssFileName.str());   //string str;  str.assign("LISE_HF_log_");    	
		ofDebug << "DLL - Input:" << endl;
		ofDebug << "dTSV_diam: " << dTSV_diam << ",  iNo_layers: " << iNo_layers << endl;
		if (iOperating_Mode != FT_ONLY_MODE && pOptical_thicknesses != NULL && pOptical_thickness_tolerances != NULL && pRef_index_re != NULL && pRef_index_im != NULL)
			for (i = 0; i < iNo_layers; i++) {
				ofDebug << "Layer no. " << i << ",  pOptical_thicknesses[i]: " << pOptical_thicknesses[i] <<
					",    pOptical_thickness_tolerances[i]: " << pOptical_thickness_tolerances[i] <<
					",  pRef_index_re[i]: " << pRef_index_re[i] << ",  pRef_index_im[i]: " << pRef_index_im[i] << endl;
			}//i			
		ofDebug << "iNo_pixels: " << iNo_pixels;
		ofDebug << ",  pWavelength_nm[0]: " << pWavelength_nm[0] << ",  pWavelength_nm[1]: " << pWavelength_nm[1] << endl << "pDark_spectrum[1600]: " <<
			pDark_spectrum[1600] << ",  pReference_spectrum[1600]: " << pReference_spectrum[1600] << ",  pSpectrum[1600]: " << pSpectrum[1600] << endl;

		ofDebug << "iFT_dim: " << iFT_dim << ",   iOperating_Mode: " << iOperating_Mode << ",  dThreshold_valid_signal: " << dThreshold_valid_signal <<
			",  dThreshold_peak: " << dThreshold_peak << endl << "dz_Resolution: " << dz_Resolution << endl << endl;
		ofDebug << "pWavelength_nm[i] \t" << "pSpectrum[i] \t\t" << "pDark_spectrum[i] \t" << "pReference_spectrum[i] \t";
		if (pSpectrumPlanarLayers != NULL)
			ofDebug << "  pSpectrumPlanarLayers[i]";
		ofDebug << endl;
		for (i = 0; i < iNo_pixels; i++) {
			ofDebug << pWavelength_nm[i] << "\t\t\t" << pSpectrum[i] << "\t\t\t" << pDark_spectrum[i] << "\t\t\t" << pReference_spectrum[i];
			if (pSpectrumPlanarLayers != NULL)
				ofDebug << "\t\t" << pSpectrumPlanarLayers[i];
			ofDebug << endl;
		}//end for i
		ofDebug.close();
	}//end if bDebugOutput 

	//Part I of function: Fourier transform (FT)
	//3. Checks - also of the recorded spectra and initializations: 
	double dThresholdDark = (double)iNo_pixels, dRelative_Threshold = 1.2, dMinPermittedPower = (double)iNo_pixels * 100.0, dMaxPerittedPowerDesity = 65535.0;
	if (bValidateRecordedSpectra(iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, dThresholdDark, dRelative_Threshold,
		dMinPermittedPower, dMaxPerittedPowerDesity, pssMessages) == false) {
		bFT_done = bSignal_analysis_done = false;   return;
	}

	//4.a Preprocess spectrometer data: 
	dVector dVk_spectrum(iNo_pixels), dVPreproc_Reference(iNo_pixels), dVPreproc_Spectrum(iNo_pixels), dVSpectralWeight(iNo_pixels), dVNormalized_Refl(iNo_pixels);
	vPreprocess_spectra(iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, dVk_spectrum, dVPreproc_Reference.Data, dVPreproc_Spectrum.Data);
#ifdef OLOVIA_DEBUG	
	ofstream ofPreproc("S_Preproc.txt");
	for (int nn = 0; nn < iNo_pixels; nn++) {
		ofPreproc << nn << " \t" << dVk_spectrum.Data[nn] << " \t" << dVPreproc_Reference.Data[nn] << " \t" << dVPreproc_Spectrum.Data[nn] << endl;
	}
#endif 
	//4.b smoothen data: 
	double dSigma_k = 0.67 * (dVk_spectrum.Data[iNo_pixels - 1] - dVk_spectrum.Data[iNo_pixels - 2]);
	dVector dVPreproc_Spectrum_Smooth(iNo_pixels), dVPreproc_Reference_Smooth(iNo_pixels);
	vSmoothenData(dSigma_k, iNo_pixels, dVk_spectrum.Data, dVPreproc_Spectrum.Data, dVPreproc_Spectrum_Smooth.Data);
	vSmoothenData(dSigma_k, iNo_pixels, dVk_spectrum.Data, dVPreproc_Reference.Data, dVPreproc_Reference_Smooth.Data);

	//5. Utilize the reference spectrum:
	double dRelative_Threshold_Signal = 0.01, dAbsoluteThreshold = 20.0;
	vNormalizeReflection(iNo_pixels, dVPreproc_Reference_Smooth.Data, dVPreproc_Spectrum_Smooth.Data, dRelative_Threshold_Signal, dAbsoluteThreshold, dVNormalized_Refl.Data);
#ifdef OLOVIA_DEBUG	
	ofstream ofNormalizedRefl("NormalizedRefl.txt");
	for (int nn = 0; nn < dVNormalized_Refl.Nown; nn++) {
		ofNormalizedRefl << nn << " \t" << dVk_spectrum.Data[nn] << " \t" << dVPreproc_Spectrum_Smooth.Data[nn] << " \t" << dVPreproc_Reference_Smooth.Data[nn]
			<< " \t" << dVNormalized_Refl.Data[nn] << endl;
	}
#endif 

	//6. Calc. weight function: 
	vLowpass_Spectrum(iNo_pixels, dVk_spectrum.Data, dVPreproc_Reference_Smooth.Data, iFT_dim, false, 0.0, 1.1, 20, true, dVSpectralWeight.Data);
#ifdef OLOVIA_DEBUG
	ofstream ofSpectralWeight("Weights.txt");
	for (i = 0; i < iNo_pixels; i++)
		ofSpectralWeight << i << " \t " << dVk_spectrum.Data[i] << " \t " << dVSpectralWeight.Data[i] << " \t " << dVNormalized_Refl.Data[i] << " \t "
		<< dVNormalized_Refl.Data[i] * dVSpectralWeight.Data[i] << endl;
#endif 
	for (i = 0; i < iNo_pixels; i++)
		dVNormalized_Refl.Data[i] *= dVSpectralWeight.Data[i];

	//7. FFT: 
	double dk_equidist, dkMax, dDelta_z, dApodis_Interval = 20.0 * (dVk_spectrum.Data[1] - dVk_spectrum.Data[0]), dWidthGauss_reciprocal_space;
	dVector dVModulus_of_FT_accurate(iFT_dim);  Vector VDummy(iFT_dim);
	vFFT_of_Spectrum_2ndOrder(iNo_pixels, dVk_spectrum.Data, dVNormalized_Refl.Data, iFT_dim, false, 0.0, dApodis_Interval, 2.0, 1.0 / 2.0, dk_equidist, VDummy.Data,
		dVModulus_of_FT_accurate.Data, dkMax, dDelta_z, pz);
	dWidthGauss_reciprocal_space = iFT_dim * dDelta_z / (dz_Resolution + NUM_NOISE);
	vLowpass_Spectrum(iFT_dim, pz, dVModulus_of_FT_accurate.Data, iFT_dim, true, 0.0, 1.0, dWidthGauss_reciprocal_space, true, pModulus_of_FT);
	if (iOperating_Mode == FT_ONLY_MODE)
	{
		// search max peak to display threshold peaks
		double dOptical_thickness_total = dSum(iNo_layers, pOptical_thicknesses);
		double dOptical_tolerance_total = dSum(iNo_layers, pOptical_thickness_tolerances);
		double dzWindowLeft_FFTmode = pOptical_thicknesses[0] - pOptical_thickness_tolerances[0];
		double dzWindowRight_FFTmode = dOptical_thickness_total + dOptical_tolerance_total;
		dzWindowRight_FFTmode = max(dzWindowLeft_FFTmode + NUM_NOISE_FLOAT, dzWindowRight_FFTmode);
		dzWindowRight_FFTmode = min((iFT_dim / 2 - 2) * dDelta_z - NUM_NOISE_FLOAT, dzWindowRight_FFTmode);
		int iNo_all_peaks_FFTmode;
		double* pPositions_all_peaks_sorted_FFTmode = NULL, *pAmplitudes_all_peaks_sorted_FFTmode = NULL;
		vFindMaximaOf_f(dzWindowLeft_FFTmode, dzWindowRight_FFTmode, dDelta_z, iFT_dim, pz, pModulus_of_FT, iNo_all_peaks_FFTmode, &pPositions_all_peaks_sorted_FFTmode, &pAmplitudes_all_peaks_sorted_FFTmode);
		dMaxPeakFFTAmplitudeinWindow = pAmplitudes_all_peaks_sorted_FFTmode[0];
		return;
	}

	//Part II of function: Signal analysis
	//8a) Further checks of input needed for data analysis 
	if (bSignal_analysis_done == false)
		return;
	double dFWHM_peak;  //0.86; //1.0; //dFWHM_peak = 0.77; 
	bNewPeakDetection == true ? (dFWHM_peak = 0.77) : (dFWHM_peak = 0.86);  
	double dOptical_thickness_total = dSum(iNo_layers, pOptical_thicknesses), dOptical_tolerance_total = dSum(iNo_layers, pOptical_thickness_tolerances), dGeom_thickness_total;
	Multilayer Multi_on_top(iNo_layers);   Multi_on_top.dLambda_Center = 0.560;
	Fresnel_TSV Fres_TSV(Multi_on_top);
	Fres_TSV.dEffective_Index_TSV = dCalc_Effective_Index(dTSV_diam, Fres_TSV.dLambda_Center);

	if (dOptical_thickness_total + dOptical_tolerance_total + 2 * dFWHM_peak > MAX_DETECTABLE_DEPTH) {
		*pssMessages << "Error: The maximum detectable depth is " << MAX_DETECTABLE_DEPTH << " µm." << endl; 
		bSignal_analysis_done = false;   return;
	}
	
	if (pOptical_thicknesses[0] - pOptical_thickness_tolerances[0] - dFWHM_peak < 2.0 * dFWHM_peak) {
		*pssMessages << "Error: Optical thickness of lowest layer not large enough for this signal processing method." << endl;
		bSignal_analysis_done = false;  return;
	}
	double dzWindowLeft = pOptical_thicknesses[0] - pOptical_thickness_tolerances[0], dzWindowRight = dOptical_thickness_total + dOptical_tolerance_total;
	dzWindowRight = max(dzWindowLeft + NUM_NOISE_FLOAT, dzWindowRight);
	if (dzWindowRight > MAX_DETECTABLE_DEPTH) {
		*pssMessages << "Error: The maximum detectable depth is " << MAX_DETECTABLE_DEPTH << " µm";  bSignal_analysis_done = false; return;
	}
	dzWindowRight = min((iFT_dim / 2 - 2) * dDelta_z - NUM_NOISE_FLOAT, dzWindowRight);

	//8b) Search for peaks, peak fitting for better reproducibility and assessment of the measurement quality: 
	bool bCalcSuccess;
	int iNo_all_peaks;
	double *pPositions_all_peaks_sorted = NULL, *pAmplitudes_all_peaks_sorted = NULL;

	vFindMaximaOf_f(dzWindowLeft, dzWindowRight, dDelta_z, iFT_dim, pz, pModulus_of_FT, iNo_all_peaks, &pPositions_all_peaks_sorted, &pAmplitudes_all_peaks_sorted);
#ifdef OLOVIA_DEBUG
	ofstream ofSortedList("SortedListOfPeaks.txt");
	for (i = 0; i < iNo_all_peaks; i++)
		ofSortedList << i << " \t" << pPositions_all_peaks_sorted[i] << " \t" << pAmplitudes_all_peaks_sorted[i] << endl;
#endif  
	dMaxPeakFFTAmplitudeinWindow = pAmplitudes_all_peaks_sorted[0];
	if (pAmplitudes_all_peaks_sorted[0] < dThreshold_valid_signal * pModulus_of_FT[0]) {
		*pssMessages << "Too small signal: FT[0]: " << pModulus_of_FT[0] << ", signal threshold: " << dThreshold_valid_signal <<
			", highest peak: " << pAmplitudes_all_peaks_sorted[0] << endl;   bSignal_analysis_done = false;
		//goto END; 
	}
	else
	{
		bCalcSuccess = bFindPeaks_LISE_HF_LSQ(iFT_dim, dDelta_z, dzWindowLeft, dzWindowRight, pz, pModulus_of_FT, iNo_layers, pOptical_thicknesses,
				pComputed_optical_thicknesses, pOptical_thickness_tolerances, pAmplitudes_of_peaks, pAmplitudes_all_peaks_sorted[0], dThreshold_peak * pAmplitudes_all_peaks_sorted[0],
				dFWHM_peak, bPeakDetectionOnRight, bNewPeakDetection, dAsymptStdErr, dNormalizedResidual, dQuality, iNo_detected_peaks, pssMessages);
		if (bCalcSuccess)
			bCalcSuccess = bCalc_Geom_From_Optical_Depths_TSV(iNo_layers, pComputed_optical_thicknesses, pOptical_thickness_tolerances, pRef_index_re, Fres_TSV.dEffective_Index_TSV,
				dOptical_thickness_total, dGeom_thickness_total, dOptical_tolerance_total, pComputed_geom_thicknesses);
		if (bCalcSuccess == false) {
			bSignal_analysis_done = false;
			*pssMessages << "TSV diameter: " << dTSV_diam << " um" << endl << "Refractive indices(re and im) :" << endl; 
			for (i = 0; i < iNo_layers; i++)
				*pssMessages << pRef_index_re[i] << " \t " << pRef_index_im[i] << endl;  
		}
		if (bDebugOutput) {
			ofDebug.open(ssFileName.str(), std::ios_base::app);
			ofDebug << endl << "DLL - Output: " << endl;
			if (bCalcSuccess)
				ofDebug << "dAsymptStdErr: " << dAsymptStdErr << ",  dNormalizedResidual: " << dNormalizedResidual << ",  dQuality: " << dQuality << endl;
			ofDebug << "szAdditional_message: " << pssMessages->str() << endl << "pModulus_of_FT[0]: " << pModulus_of_FT[0] <<
				",  pModulus_of_FT[1]: " << pModulus_of_FT[1] << ",  pz[0]: " << pz[0] << ",  pz[1]: " << pz[1] << endl;
			if (bCalcSuccess) {
				ofDebug << "iNo_detected_peaks: " << iNo_detected_peaks << endl;
				for (i = 0; i < iNo_detected_peaks; i++)
					ofDebug << "i = " << i << ":  pComputed_optical_thicknesses[i]: " << pComputed_optical_thicknesses[i] <<
					",  pAmplitudes_of_peaks[i]: " << pAmplitudes_of_peaks[i] << endl;
				if (pComputed_geom_thicknesses != NULL)
					for (i = 0; i < iNo_detected_peaks; i++)
						ofDebug << "pComputed_geom_thicknesses[i]: " << pComputed_geom_thicknesses[i] << endl;
			}//end if success   
			ofDebug << "bSuccess: " << bCalcSuccess << endl;   ofDebug.close();
		}//end if output 
	}
	//END: 
	delete[] pPositions_all_peaks_sorted, delete[] pAmplitudes_all_peaks_sorted;  return;
}



//##################################################################  Metrology utilities  ##########################################################################



void vAverageAndStandardDev()
{
	int i;  const int iDim = 11;
	double dAverage = 0.0, df[iDim], dDelta[iDim], dSigma = 0.0;
	//df[0] = 56.5991, df[1] = 56.6194, df[2] = 56.6072, df[3] = 56.6213, df[4] = 56.6553, df[5] = 56.6140, df[6] = 56.5871, df[7] = 56.5793, df[8] = 56.5878, df[9] = 56.5735, df[10] = 56.6071; 
	//df[0] = 58.7374, df[1] = 58.7376, df[2] = 58.7368, df[3] = 58.7371, df[4] = 58.7375, df[5] = 58.7374, df[6] = 58.7386, df[7] = 58.7385, df[8] = 58.7397, df[9] = 58.7401; 
	//df[0] = 60.4441, df[1] = 60.4595, df[2] = 60.4633, df[3] = 60.4691, df[4] = 60.4732, df[5] = 60.4775, df[6] = 60.4834, df[7] = 60.4841, df[8] = 60.4862, df[9] = 60.4920;  
	df[0] = 78.9694, df[1] = 78.9699, df[2] = 78.9696, df[3] = 78.9696, df[4] = 78.9697, df[5] = 78.9701, df[6] = 78.9702, df[7] = 78.9705, df[8] = 78.9705, df[9] = 78.9701, df[10] = 78.9705;

	for (i = 0; i < iDim; i++)
		dAverage += df[i];
	dAverage /= (double)iDim;
	for (i = 0; i < iDim; i++) {
		dDelta[i] = df[i] - dAverage;
		dDelta[i] *= dDelta[i];
		dSigma += dDelta[i];
	}
	dSigma /= (double)(iDim - 1);
	dSigma = sqrt(dSigma);
	cout << "Average: " << dAverage << endl << endl << "Sigma: " << dSigma << endl << endl << "3-Sigma: " << 3.0 * dSigma;
}


void vCalcContrastAtSiLayer()
//calculate the contrast in the spectrum of LISE-HF or similar sensors at a layer of silicon.  W.Iff, 14.10.2021 
{
	int i;
	double dWavelength;
	double dk;
	double dz[3];   dz[1] = 4.0; //layer thicknesses in µm 
	double dTemp, dTempSq, dTemp2, dE[100], dE_plus, dE_minus, dI_plus, dI_minus, dDeltaInt, dI_Average, dContrast;
	cppc cud[3], cdu[3], cdd[3], cuu[3], cDifference[3], cSum[3], cPropFac[3], cPropFac2[3]; //cud: Fresnel-coeff for refl of downgoing light etc.
	cppc cn[3];  //refractive index n 
	//With the ref. ind. from Aspnes and Studna (1983)  
	//dWavelength = 0.45;    cn[0].real(1.5), cn[0].imag(0.0);     cn[1].real(4.6784), cn[1].imag(0.14851);    cn[2].real(1.0), cn[2].imag(0.0);  
	//dWavelength = 0.50;    cn[0].real(1.5), cn[0].imag(0.0);     cn[1].real(4.2992), cn[1].imag(0.070425);   cn[2].real(1.0), cn[2].imag(0.0);
	//dWavelength = 0.55;    cn[0].real(1.5), cn[0].imag(0.0);     cn[1].real(4.0870), cn[1].imag(0.040882);   cn[2].real(1.0), cn[2].imag(0.0); 
	//With the ref. ind. from Schinke (2015)  
	//dWavelength = 0.45;    cn[0].real(1.5), cn[0].imag(0.0);     cn[1].real(4.6730), cn[1].imag(0.095362);    cn[2].real(1.0), cn[2].imag(0.0);  
	//dWavelength = 0.50;    cn[0].real(1.5), cn[0].imag(0.0);     cn[1].real(4.2890), cn[1].imag(0.048542);   cn[2].real(1.0), cn[2].imag(0.0);
	dWavelength = 0.55;    cn[0].real(1.5), cn[0].imag(0.0);     cn[1].real(4.0730), cn[1].imag(0.028038);   cn[2].real(1.0), cn[2].imag(0.0);

	for (i = 0; i <= 1; i++) {
		cDifference[i] = cn[i + 1] - cn[i];   cSum[i] = cn[i + 1] + cn[i];
		cud[i] = cDifference[i] / cSum[i];
		cdd[i] = 2.0 * cn[i + 1] / cSum[i];
		cdu[i] = -cud[i];
		cuu[i] = 2.0 * cn[i] / cSum[i];
		cout << (cud[i] * conj(cud[i]) * cn[i + 1].real() + cdd[i] * conj(cdd[i]) * cn[i].real()) / cn[i + 1].real() << "\t\t"
			<< (cdu[i] * conj(cdu[i]) * cn[i].real() + cuu[i] * conj(cuu[i]) * cn[i + 1].real()) / cn[i].real() << endl;
	}
	i = 1;
	dk = 2.0 * PI / dWavelength;   cPropFac[i] = exp(cIm * dk * cn[i] * dz[i]);   cPropFac2[i] = cPropFac[i] * cPropFac[i];

	dE[0] = abs(cud[1]);   dE[1] = abs(cPropFac2[1] * cuu[1] * cud[0] * cdd[1]);
	dE_plus = dE[0] + dE[1];  dE_minus = dE[0] - dE[1];   dI_plus = dE_plus * dE_plus;  dI_minus = dE_minus * dE_minus;
	dDeltaInt = dI_plus - dI_minus;
	dTemp = abs(cPropFac2[1] * cud[0] * cdu[1]);   dTempSq = dTemp * dTemp;   dTemp2 = 1.0 / (1.0 - dTempSq);
	dI_Average = dE[0] * dE[0] + dE[1] * dE[1] * dTemp2;
	dContrast = dDeltaInt / dI_Average;
}


//----------------------------------------------------- Out-of -date code for spectroscopic reflectometry ------------------------------------------------



void vFT_of_Spectro(struct sSpectro_Data *p) //"_x50_Xpol_subs_b_cppc_14_2_18.txt"  
//This function computes a fraction of the fourier transform of the fringe pattern. The peaks are at 2*PI/(0.5*wavelength) = +/- 2*PI / (0.5*1.329) 
{
	int i, j, Index;
	double dPhaseR, dDzR;
	cppc cExpL, cExpR, cTmp, cDelta;
	ofstream fOutFT("Spectro_FT_Eval.txt");

	int  ikStart = 0, ikStop = (int)((p->iNoPoints - 1) / 2);  int iDim_k = ikStop - ikStart + 1;
	dVector Vk(iDim_k); Vector VSpectrum(iDim_k);

	p->dPeriod = p->pzMeasured[p->iNoPoints - 1] + 0.5 * (p->pzMeasured[p->iNoPoints - 1] - p->pzMeasured[p->iNoPoints - 2]);
	for (i = ikStart; i <= ikStop; i++) {  //-N, ...., +N  or  -N, ..., +N-1; POSSIBLE ACCELERATION: we need the calc. only for few frequencies between k1 and k2 
		Index = i - ikStart;   VSpectrum.Data[Index] = 0.0;   Vk.Data[Index] = (i * 2.0 * PI) / p->dPeriod;
		dDzR = p->pzMeasured[0] - 0.5 * (p->pzMeasured[1] - p->pzMeasured[0]);   //cout << cExpR << endl;  
		if (i != 0) {
			dPhaseR = Vk.Data[Index] * dDzR;
			cExpR = exp(-cIm * dPhaseR);
			for (j = 0; j < p->iNoPoints; j++) { //for all z_j with I(z_j) > zero; ACCELERATION: we need the calc. only for few z_j between z_min and z_max 
				if (j == p->iNoPoints - 1)
					dDzR = p->pzMeasured[j] + 0.5 * (p->pzMeasured[j] - p->pzMeasured[j - 1]);
				else
					dDzR = (p->pzMeasured[j] + p->pzMeasured[j + 1]) / 2.0;
				cExpL = cExpR; // - exp(-cIm * dPhaseL); // ... * e^(-ikz)
				dPhaseR = Vk.Data[Index] * dDzR;
				cExpR = exp(-cIm * dPhaseR);   //cout << j << "\t" << dDzR << "\t" << dPhaseR << "\t" << cExpR << endl;
				cDelta = cExpR - cExpL;
				cTmp = p->pIntMeasured[j] * cDelta;
				VSpectrum.Data[Index] += cTmp;
			}//end for all z_j 
			VSpectrum.Data[Index] /= (-cIm * Vk.Data[Index]);
			VSpectrum.Data[Index] /= p->dPeriod;
		}
		else {
			cExpR = dDzR;
			for (j = 0; j < p->iNoPoints; j++) { //for all z_j
				if (j == p->iNoPoints - 1)
					dDzR = p->pzMeasured[j] + 0.5 * (p->pzMeasured[j] - p->pzMeasured[j - 1]);
				else
					dDzR = (p->pzMeasured[j] + p->pzMeasured[j + 1]) / 2.0;
				cExpL = cExpR; // - exp(-cIm * dPhaseL); // ... * e^(-ikz)
				cExpR = dDzR; //cout << j << "\t" << dDzR << "\t" << dPhaseR << "\t" << cExpR << endl;
				cDelta = cExpR - cExpL;
				cTmp = p->pIntMeasured[j] * cDelta;
				VSpectrum.Data[Index] += cTmp;
			}//end for all z_j 
			VSpectrum.Data[Index] /= p->dPeriod;
		}
		//cout << i << "\t" << Index << "\t" << Vk.Data[Index] << "  \t" << VSpectrum.Data[Index] << endl;
	}//end for all k_i 

	fOutFT << "# i   " << "   z_i/2.0:  " << "   Abs(Counts):  " << "   Re(Counts):   " << "   Im(Counts):   " << endl;
	for (i = 0; i < (int)Vk.Nown; i++)
		fOutFT << ikStart + i << "\t" << Vk.Data[i] / 2.0 << "\t\t" << abs(VSpectrum.Data[i]) << "\t\t" << VSpectrum.Data[i].real() << "\t\t" << VSpectrum.Data[i].imag() << endl;
	delete[] p->pdVk;  p->pdVk = new dVector[1];  delete[] p->pdVAbsFT;  p->pdVAbsFT = new dVector[1];
	p->pdVk->Init(ikStop - ikStart);   p->pdVAbsFT->Init(ikStop - ikStart);
	for (i = ikStart; i <= ikStop; i++) {
		p->pdVAbsFT->Data[i - ikStart] = abs(VSpectrum.Data[i - ikStart]);   p->pdVk->Data[i - ikStart] = (i * 2.0 * PI) / p->dPeriod;
	}
	return;
}





BOOL bReadFFTResultFile(char szFile1[], char szFile2[], char szFile3[], char szFile4[], char szFile5[])
{
	int i, j, iLineNo[5] = { 0, 0, 0, 0, 0 };
	double *pz = NULL, *pData = NULL;
	string strNextLine[5];
	ifstream  fFile1(szFile1), fFile2(szFile2), fFile3(szFile3), fFile4(szFile4), fFile5(szFile5);
	ofstream ofFFTAverage("ofFFTAverage.txt");
	if (!fFile1.good() || !fFile2.good() || !fFile3.good() || !fFile4.good() || !fFile5.good()) {
		//fprintf(Fres, "\n Error! Opening of input-file failed! \n");
		goto END;
	}

	if (!bReadNextLine_ASCII(&fFile1, strNextLine + 0, iLineNo + 0)) //first line with comment 
		return FALSE;
	if (!bReadNextLine_ASCII(&fFile2, strNextLine + 1, iLineNo + 1)) //first line with comment 
		return FALSE;
	if (!bReadNextLine_ASCII(&fFile3, strNextLine + 2, iLineNo + 2)) //first line with comment 
		return FALSE;
	if (!bReadNextLine_ASCII(&fFile4, strNextLine + 3, iLineNo + 3)) //first line with comment 
		return FALSE;
	if (!bReadNextLine_ASCII(&fFile5, strNextLine + 4, iLineNo + 4)) //first line with comment 
		return FALSE;

	for (j = 0; j < 5; j++)
		if (strstr(strNextLine[j].c_str(), "Im(Counts):") == NULL)
			goto END;

	pz = new double[1024], pData = new double[1024];

	for (i = 0; i < 1024; i++) {
		int iCount;  double azTmp[5], aDataTmp[5];

		if (!bReadNextLine_ASCII(&fFile1, strNextLine + 0, iLineNo + 0)) //first line with comment 
			return FALSE;
		if (!bReadNextLine_ASCII(&fFile2, strNextLine + 1, iLineNo + 1)) //first line with comment 
			return FALSE;
		if (!bReadNextLine_ASCII(&fFile3, strNextLine + 2, iLineNo + 2)) //first line with comment 
			return FALSE;
		if (!bReadNextLine_ASCII(&fFile4, strNextLine + 3, iLineNo + 3)) //first line with comment 
			return FALSE;
		if (!bReadNextLine_ASCII(&fFile5, strNextLine + 4, iLineNo + 4)) //first line with comment  
			return FALSE;

		for (j = 0; j < 5; j++) {
			if (sscanf_s(strNextLine[j].c_str(), "%d %lg %lg", &iCount, azTmp + j, aDataTmp + j) != 3) {
				cout << endl << "Numbers in data line i = " << i << " cannot be read. " << endl;   return FALSE;
			}
			if (iCount != i)
				return FALSE;
		}
		for (j = 1; j < 5; j++) {
			if (fabs(azTmp[0] - azTmp[j]) > FLOAT_PRECISION)
				return FALSE;
		}
		pz[i] = azTmp[0];  pData[i] = 0.0;
		for (j = 0; j < 5; j++) {
			pData[i] += aDataTmp[j];
		}
		pData[i] /= 5.0;
	}

	ofFFTAverage << "# i \t z/nm \t\t counts" << endl;
	for (i = 0; i < 1024; i++) {
		ofFFTAverage << i << "\t" << pz[i] << "\t\t" << pData[i] << endl;
	}

	delete[] pz;  delete[] pData;  return TRUE;

END:
	cout << endl << "Error in function bReadLiseFile()! \n Abandonment!" << endl;
	delete[] pz;  delete[] pData;  return FALSE;
}





//-------------------------------------------------------------- Code for calculation of the reflection  ---------------------------------------------------------


struct sReadSpecData {
	char *szFileName = NULL; double dzStart; double dzStop; int iNoOfLines; int iNoInt; int *iNoOfLines_a = NULL; int *iNoOfLines_b = NULL;
	double **ppzInt = NULL; double **ppDataInt = NULL;
};


struct sCalcDeltaSpecSq {
	int iNoPoints = 0, iNoFFTPoints = 0, ikStart, ikStop;
	double *pzMeasured = NULL, *pIntMeasured = NULL;
	dVector *pdVk = NULL, *pdVkWindow = NULL; Vector *pVSpectrum = NULL; Vector *pVSpecWindow = NULL; Vector *pVConjPhase = NULL;
	double dOrigin_z_Glob, dApodisLeft, dzFreeSpace, dzTop, dzBottom, dzBottomMax, dzFreeSpaceBottom, dApodisRight, dPeriod, dDiameter; //dOrigin: middle of envelope, origin for Fourier transform
	double dLambdaMid, dkMidMeritFu, dDelta_z, dDelta_z_LarkinAlgo, dLc, dkStart, dkStop, dkWindowLeft, dkWindowRight, dDelta_k;
	dVector *pdV_x_or_k_weight = NULL; Vector *pVSpectrumSmoothened = NULL; int iTypeOfWeights = NO_WEIGHTS; //variables for weighted least squares 
};

void vDelete_sCalcDeltaSpecSq(struct sCalcDeltaSpecSq *p)
{
	delete[] p->pzMeasured, delete[] p->pIntMeasured;  delete[] p->pdVk, delete[] p->pVSpectrum;  delete[]  p->pVSpecWindow;  delete[] p->pVConjPhase;
	delete[] p->pdV_x_or_k_weight;  delete[] p->pVSpectrumSmoothened;
	p->pzMeasured = NULL, p->pIntMeasured = NULL, p->pdVk = NULL, p->pVSpectrum = NULL; p->pVSpecWindow = NULL; p->pVConjPhase = NULL;  p->pdV_x_or_k_weight = NULL;
	p->pVSpectrumSmoothened = NULL;
}

struct sInfoConicalCircSpecTSV {
	double dzTop, dDepth, dDiameterBot, dDiameterTop;
};



void vPreCalcReflMatOfTSV(int iNoLayers, cppc *pRefInd, double *pDiameter, dVector *pdVkVac, Matrix *pMEffInd, Matrix *pMk_z)
//cppc *pFresnel_dd, cppc *pFresnel_ud, cppc *pFresnel_du, cppc *pFresnel_uu)
//Calc. for one wavelength only: 
{
	int i, j;
	double dLambda0 = 1.2005991, dLambda, dRatio, dDiameterCalc;
	cppc nl, nu;
	// Calc k_z: 
	for (j = 0; j <= pMk_z->Nown; j++) { //for all frequencies:
		(*pMk_z)(j, 0) = pRefInd[0] * pdVkVac->Data[j];  (*pMEffInd)(j, 0) = pRefInd[0];
		for (i = 0; i <= iNoLayers; i++) { //for all layers: 
			dLambda = (2.0 * PI / pdVkVac->Data[0]);  dRatio = dLambda0 / dLambda;
			dDiameterCalc = dRatio * pDiameter[i + 1];
			if (i < iNoLayers)
				vCalcEffInd(dDiameterCalc, pdVkVac->Data[j], (*pMEffInd)(j, i + 1), CIRC);
			else
				(*pMEffInd)(j, i + 1) = pRefInd[i + 1];
			(*pMk_z)(j, i + 1) = (*pMEffInd)(j, i + 1) * pdVkVac->Data[0];
			// Calc Frenel coeff for perpendic incidence:  
			/*if (pFresnel_dd) {
				nl = pMEffInd->Data[j, i];
				nu = pMEffInd->Data[j, i + 1];
				pFresnel_dd[i] = 2.0 * nu / (nu + nl);
				pFresnel_ud[i] = (nu - nl) / (nu + nl);
				pFresnel_du[i] = (nl - nu) / (nu + nl);
				pFresnel_uu[i] = 2.0 * nl / (nu + nl);
				}*/
		}//i
	}//j
}


void vCalcReflMatOfMultilayer(int iNoLayers, double *dThickness, Matrix *pMk_z, double *pkz_mid_re, cppc &pcIncField,
	cppc *pAmplud, cppc *pAmpldu, cppc *pReflCoeff, double *pReflPower)
	// Calc reflection for all wavelengths: 
{
	int i, j;
	for (j = 0; j < pMk_z->Nown; j++) { //for all frequencies:
		//Calc exponential factors exp(i k_z z):
		//init: lowest interface = lowest Fresnel coeff: 
		pReflCoeff[j] = pAmplud[0];
		for (i = 1; i < iNoLayers; i++) { //for all layers:  
			// add next homogeneous region:
			cppc cExp = exp(cIm * ((*pMk_z)(j, i + 1) - pkz_mid_re[i + 1]) * (dThickness[i] * 2.0));
			pReflCoeff[j] *= cExp;
			// add next interface: 
			//pReflCoeff = pFresnel_ud[i] + pFresnel_uu[i] * pReflCoeff * 1.0 / (1.0 - pFresnel_du[i] * pReflCoeff) * pFresnel_dd[i]; 
			//pReflCoeff = pAmplud[i] + pReflCoeff * 1.0 / (1.0 - pAmpldu[i] * pReflCoeff); 
			if (i > 1)
				pReflCoeff[j] = pAmplud[i] + pReflCoeff[j] * 1.0 / (1.0 - pAmpldu[i - 1] * pReflCoeff[j]);
			else
				pReflCoeff[j] = pAmplud[i] + pReflCoeff[j]; //1-fold refl at TSV bottom; 2-fold refl is negligible. 
		}//i
		cppc cReflField = pReflCoeff[j] * pcIncField;
		double dRe = cReflField.real(), dIm = cReflField.imag();
		pReflPower[j] = dRe * dRe + dIm * dIm;
	}//j

}
void vCalcEllipseParamOfSpot(double dxGauss, double dyGauss, double dRadius, double *pdImageSub, dVector  *pdV_Grid_x_Sub, dVector  *pdV_Grid_y_Sub,
	double &dRatioOfAxisOfEllipse, double &dAngleOfEllipse)
	//This function evaluates dRatioOfAxisOfEllipse, meaning the ellipticity of the spot and dAngleOfEllipse (orintation of this ellipse), measured 
	//(counter-clockwise) in degree starting from the +x-axis. The purpose here is the analysis of the spot on the camera, which should be ideally round,  
	//and its optical aberrations.  W. Iff, 13.5.2024 
{
	int i, j, iIndex = 0;
	double dIntegral_0_deg = 0, dIntegral_45_deg = 0, dIntegral_90_deg = 0, dIntegral_135_deg = 0, d2Phi, dSumOfWeights = 0;
	double dDelta_x, dDelta_y, dDelta_x_45_deg, dDelta_y_45_deg;
	for (i = 0; i < pdV_Grid_y_Sub->Nown; i++) { // for all lines i in the image:
		dDelta_y = pdV_Grid_y_Sub->Data[i] - dyGauss;
		for (j = 0; j < pdV_Grid_x_Sub->Nown; j++) { //for all entries j in the line i:
			dDelta_x = pdV_Grid_x_Sub->Data[j] - dxGauss;
			if (sqrt(dDelta_x * dDelta_x + dDelta_y * dDelta_y) < 4.5 * dRadius) {
				dIntegral_0_deg += abs(dDelta_x) * pdImageSub[iIndex];   dIntegral_90_deg += abs(dDelta_y) * pdImageSub[iIndex];
				dDelta_x_45_deg = (dDelta_x + dDelta_y) / sqrt(2.0);  dDelta_y_45_deg = (-dDelta_x + dDelta_y) / sqrt(2.0);
				dIntegral_45_deg += abs(dDelta_x_45_deg) * pdImageSub[iIndex];   dIntegral_135_deg += abs(dDelta_y_45_deg) * pdImageSub[iIndex];
				dSumOfWeights += pdImageSub[iIndex];
			}
			iIndex++;
		}//for all entries j in the line i
	}//i (end for all lines in the image) 
	dIntegral_0_deg /= dSumOfWeights, dIntegral_45_deg /= dSumOfWeights, dIntegral_90_deg /= dSumOfWeights, dIntegral_135_deg /= dSumOfWeights;
	double dAverage = (dIntegral_0_deg + dIntegral_45_deg + dIntegral_90_deg + dIntegral_135_deg) / 4.0;
	double dAmplEllipse = sqrt(pow(dIntegral_0_deg - dAverage, 2) + pow(dIntegral_45_deg - dAverage, 2));
	dRatioOfAxisOfEllipse = (dAverage + dAmplEllipse) / (dAverage - dAmplEllipse);
	if (dRatioOfAxisOfEllipse > NUM_NOISE) {
		d2Phi = atan2(dIntegral_45_deg - dAverage, dIntegral_0_deg - dAverage);  dAngleOfEllipse = (d2Phi / 2.0) * 180.0 / PI;
	}
	else
		dAngleOfEllipse = 0;
}


void vBeamProfilerMeritFu(void *pInfoInt, void *pvInfo, void *pvAdditionalInfo, dVector& dVParam, dVector *pVDeltaInt, dMatrix *pdMDeriv, bool bOptParam[], bool *pSuc)
//This function calculates function values and derivatives (if needed) for the iterative solver belonging to the beam profiler. 
//Lengths are in µm.    W. Iff, 13.5.2024
{
	bOptParam = NULL;
	int i, j;
	double  dMaxIntensity = ((dVector *)pvAdditionalInfo)->Data[0], dAmplScalingFactor = ((dVector *)pvAdditionalInfo)->Data[1];
	double dAmpl = dVParam.Data[0], dx = dVParam.Data[1], dy = dVParam.Data[2], dRadius = dVParam.Data[3], dBackground = dVParam.Data[4];
	double *pdImage = (double *)pInfoInt;  double *pdDeltaInt = pVDeltaInt->Data;
	double *pdDeriv_Ampl = NULL, *pdDeriv_x = NULL, *pdDeriv_y = NULL, *pdDeriv_Radius = NULL, *pdDeriv_Background = NULL;
	if (pdMDeriv != NULL) {
		pdDeriv_Ampl = pdMDeriv->Data, pdDeriv_x = pdMDeriv->Data + 1, pdDeriv_y = pdMDeriv->Data + 2, pdDeriv_Radius = pdMDeriv->Data + 3,
			pdDeriv_Background = pdMDeriv->Data + 4;
	}
	struct sGrid_xy *psGridSub = (sGrid_xy*)pvInfo;   dVector  *pdV_Grid_x = psGridSub->pdV_Grid_x, *pdV_Grid_y = psGridSub->pdV_Grid_y;

	double  dRadius_sq = dRadius * dRadius, dMinus2_div_R_sq = -2.0 / dRadius_sq, d2_div_dRadius_pow3 = 2.0 / (dRadius*dRadius*dRadius),
		dDelta_x, dDelta_x_sq, dDelta_y, dDelta_y_sq, dDelta_r_sq, dExp, dAmpl_x_Exp, dFuValue;
	for (i = 0; i < pdV_Grid_y->Nown; i++) { // for all lines i in the image:
		dDelta_y = pdV_Grid_y->Data[i] - dy;   dDelta_y_sq = dDelta_y * dDelta_y;
		for (j = 0; j < pdV_Grid_x->Nown; j++) { //for all entries j in the line i:
			dDelta_x = pdV_Grid_x->Data[j] - dx;   dDelta_x_sq = dDelta_x * dDelta_x;  dDelta_r_sq = dDelta_x_sq + dDelta_y_sq;
			dExp = exp(-dDelta_r_sq / dRadius_sq); // exp^[-(x²+y²)/R²] 
			dAmpl_x_Exp = dAmpl * dAmplScalingFactor * dExp;
			//Set function value and derivatives: 
			dFuValue = dAmpl_x_Exp + dBackground;
			if (dFuValue <= dMaxIntensity) { //normal case:
				*pdDeltaInt = dFuValue - (*pdImage); // A * exp^[-(x²+y²)/R²]  
				if (pdMDeriv != NULL) {
					*pdDeriv_Ampl = dExp * dAmplScalingFactor; // exp^[-(x²+y²)/R²]  
					*pdDeriv_x = dAmpl_x_Exp * dMinus2_div_R_sq * dDelta_x * (-1.0); // A * exp^[-(x²+y²)/R²] * (-1/R²) * 2x 
					*pdDeriv_y = dAmpl_x_Exp * dMinus2_div_R_sq * dDelta_y * (-1.0); // A * exp^[-(x²+y²)/R²] * (-1/R²) * 2y 
					*pdDeriv_Radius = dAmpl_x_Exp * dDelta_r_sq * d2_div_dRadius_pow3; // A * exp^[-(x²+y²)/R²] * (-1) * (x²+y²) (-2/R³) 
					*pdDeriv_Background = 1.0;
				}
			}//end if normal case
			else { //oversaturated pixel: 
				*pdDeltaInt = dMaxIntensity - (*pdImage);
				if (pdMDeriv != NULL) {
					*pdDeriv_Ampl = 0.0;   *pdDeriv_x = 0.0;   *pdDeriv_y = 0.0;   *pdDeriv_Radius = 0.0;   *pdDeriv_Background = 0.0;
				}
			}//end else oversaturated pixel 
			//prepare for next pixel (next entry in the vector)
			pdDeltaInt++, pdImage++;
			if (pdMDeriv != NULL) {
				pdDeriv_Ampl += pdMDeriv->Next, pdDeriv_x += pdMDeriv->Next, pdDeriv_y += pdMDeriv->Next, pdDeriv_Radius += pdMDeriv->Next;
				pdDeriv_Background += pdMDeriv->Next;
			}
		}//for all entries j in the line i
	}//i (end for all lines in the image) 
	*pSuc = true;
}


bool bExtractSubimage(int iNoLines, double dPixelSize_y, int iNoColumns, double dPixelSize_x, int *piImage, int iOffsetNoLines, int iOffsetNoColumns,
	int iNoLinesSub, int iNoColumnsSub, int **ppiImageSub, dVector *pdV_x_Sub, dVector *pdV_y_Sub)
	//This function extracts a sub-image from the centre of the image. The outer regions of the image are cut away symetrically. This reduces the amount of
	//data to be processed and the computation time. We may concentrate on the image centre because the spot to be fit by a Gaussian later is approximately
	//on the optical axis. At least, it is usually only some micron away fom it. If it is further away, we first align the beam better before recording an 
	//image and fitting the Gaussian.  W. Iff, 7.3.2024 
{
	if (ppiImageSub == NULL)
		return false;
	if (piImage == NULL || *ppiImageSub == NULL || iNoLines < iNoLinesSub || iNoColumns < iNoColumnsSub || pdV_x_Sub == NULL || pdV_y_Sub == NULL)
		return false;
	int i, iIndexStart = iOffsetNoLines * iNoColumns + iOffsetNoColumns, iDeltaIndex = iNoColumns - iNoColumnsSub, j, *piImageSub = *ppiImageSub;
	double  dOffset_x = dPixelSize_x * (double)(iNoColumnsSub - 1) / 2.0, dOffset_y = dPixelSize_y * (double)(iNoLinesSub - 1) / 2.0;

	piImage += iIndexStart;
	for (i = 0; i < iNoLinesSub; i++) { // for all lines in the image 
		for (j = 0; j < iNoColumnsSub; j++) { //for all entries in the line i 
			*piImageSub = *piImage;
			piImageSub++;  piImage++;
		}//j
		piImage += iDeltaIndex;
	}//i
	for (i = 0; i < iNoLinesSub; i++) // for all lines in the image 
		pdV_y_Sub->Data[i] = dOffset_y - i * dPixelSize_y;
	for (j = 0; j < iNoColumnsSub; j++) // for all columns in the image 
		pdV_x_Sub->Data[j] = j * dPixelSize_x - dOffset_x;
	return true;
}


bool bBeamProfiler(int iNoLines, int iNoLinesSub, double dPixelSize_y, int iNoColumns, int iNoColumnsSub, double dPixelSize_x, int *piImage,
	double *pdImageSub, double &dAmpl, double &dxGauss, double &dyGauss, double &dRadius, double &dBackground, double dMaxIntensity,
	double &dNorm, double &dWeightedNorm, double &dRatioOfAxisOfEllipse, double &dAngleOfEllipse)
	//This function fits a Gaussian into the LISE-HF or LISE-ED spot on the camera intensity image to retrieve the exact spot location and radius in order to facilitate
	//the optical alignment by technicians. "Sub" means sub-image. Such sub-image is extracted from the image for the Gaussian fit of the intensity. Applying the fit to the 
	//entire image is not needed because the spot is always in the centre of the image or at least close to the red crosshair target. In addition, the computation time is 
	//reduced by use of the sub-image. The unit for the length here is micrometer.  First line: Input variables; second and 3rd line: output. 
	//This function works with a local coordinate frame. The origin is the centre of the camera image denoted by "int *piImage", this means the centre of the red crosshair target. 
	//From this origin, the "+x"-direction is to the right (increasing column index), and the "-y"-direction is towards the bottom (increasing line index).  
	//dWeightedNorm is an indication for the difference between fit and measured spot; it is normalized by the number of pixels and the max. intensity per pixel. It should
	//ideally be zero and indicates aberrations of the measured spot. dRatioOfAxisOfEllipse and dAngleOfEllipse detect and charaterize undesired elliptic distortions
	//of the measured spot due to optical aberrations.   W.Iff, 13.5.2024
{
	int iOffsetNoLines = (iNoLines - iNoLinesSub) / 2, iOffsetNoColumns = (iNoColumns - iNoColumnsSub) / 2; //The image centre is taken. The offset is on the outer left and right...
	int iNoPixelsSub = iNoLinesSub * iNoColumnsSub;
	int *piImageSub = new int[iNoPixelsSub], i, j;

	dVector dV_Grid_x_Sub(iNoColumnsSub), dV_Grid_y_Sub(iNoLinesSub);
	struct sGrid_xy sGridSub = { &dV_Grid_x_Sub, &dV_Grid_y_Sub };

	if (bExtractSubimage(iNoLines, dPixelSize_y, iNoColumns, dPixelSize_x, piImage, iOffsetNoLines, iOffsetNoColumns,
		iNoLinesSub, iNoColumnsSub, &piImageSub, &dV_Grid_x_Sub, &dV_Grid_y_Sub) == false) {
		delete[] piImageSub;  return false;
	}
	for (i = 0; i < iNoPixelsSub; i++) //conversion to double for the numeric fitting procedure 
		pdImageSub[i] = (double)piImageSub[i];

	const int iNoFitParam = 5;
	bool pbOptParam[iNoFitParam] = { true, true, true, true, true }, pbUpperLimitHit[iNoFitParam] = { false, false, false, false, false },
		pbLowerLimitHit[iNoFitParam] = { false, false, false, false, false }, bSuc;
	double dAmplScalingFactor = 10.0; //we like to have matrix entries in the same order of magnitude in our iterative method below. 
	dVector dVParam(iNoFitParam), dVUpperLimit(iNoFitParam), dVLowerLimit(iNoFitParam);
	MinData_double *pMin = pAllocMinDataArray(1, iNoFitParam);
	dVParam.Data[0] = dAmpl / dAmplScalingFactor, dVParam.Data[1] = dxGauss, dVParam.Data[2] = dyGauss, dVParam.Data[3] = dRadius, dVParam.Data[4] = dBackground;
	dVector dVAdditionalInfo(2);  dVAdditionalInfo.Data[0] = dMaxIntensity, dVAdditionalInfo.Data[1] = dAmplScalingFactor;
	dVLowerLimit.Data[0] = NUM_NOISE_FLOAT; 						dVUpperLimit.Data[0] = 1.0 / NUM_NOISE;  //limitation of spot amplitude
	dVLowerLimit.Data[1] = dV_Grid_x_Sub.Data[0];					dVUpperLimit.Data[1] = dV_Grid_x_Sub.Data[iNoColumnsSub - 1]; //limitation of x
	dVLowerLimit.Data[2] = dV_Grid_y_Sub.Data[iNoLinesSub - 1];		dVUpperLimit.Data[2] = dV_Grid_y_Sub.Data[0]; //limitation of "-y" 
	dVLowerLimit.Data[3] = min(dPixelSize_x, dPixelSize_y);			dVUpperLimit.Data[3] = max(dVUpperLimit.Data[1] - dVLowerLimit.Data[1], dVUpperLimit.Data[2] - dVLowerLimit.Data[2]); //limitation of radius
	dVLowerLimit.Data[4] = 0.0;										dVUpperLimit.Data[4] = dMaxIntensity;

	bSuc = bLevenbergMarquardt_double(dVParam, pbOptParam, &dVUpperLimit, pbUpperLimitHit, &dVLowerLimit, pbLowerLimitHit,
		600.0, true, 30.0, 5, 400, 1.0e-7, iNoPixelsSub, vBeamProfilerMeritFu, (void *)pdImageSub, (void *)(&sGridSub), (void *)(&dVAdditionalInfo), pMin);

	dAmpl = pMin->dVParam.Data[0] * dAmplScalingFactor, dxGauss = pMin->dVParam.Data[1], dyGauss = pMin->dVParam.Data[2], dRadius = pMin->dVParam.Data[3],
		dBackground = pMin->dVParam.Data[4];

	vCalcEllipseParamOfSpot(dxGauss, dyGauss, dRadius, pdImageSub, &dV_Grid_x_Sub, &dV_Grid_y_Sub, dRatioOfAxisOfEllipse, dAngleOfEllipse);

	int iIndex = 0;
	double  dRadius_sq = dRadius * dRadius, dDelta_x, dDelta_x_sq, dDelta_y, dDelta_y_sq, dDelta_r_sq, dAmpl_x_Exp, dFuValue;
	double dWeight, dSumOfWeights = 0;  dNorm = 0, dWeightedNorm = 0;
	for (i = 0; i < dV_Grid_y_Sub.Nown; i++) { // for all lines i in the image:
		dDelta_y = dV_Grid_y_Sub.Data[i] - dyGauss;   dDelta_y_sq = dDelta_y * dDelta_y;
		for (j = 0; j < dV_Grid_x_Sub.Nown; j++) { //for all entries j in the line i:
			dDelta_x = dV_Grid_x_Sub.Data[j] - dxGauss;   dDelta_x_sq = dDelta_x * dDelta_x;  dDelta_r_sq = dDelta_x_sq + dDelta_y_sq;
			dAmpl_x_Exp = dAmpl * exp(-dDelta_r_sq / dRadius_sq); // exp^[-(x²+y²)/R²];
			dFuValue = dAmpl_x_Exp + dBackground;
			if (dFuValue <= dMaxIntensity) //normal case:
				pdImageSub[iIndex] = pdImageSub[iIndex] - dFuValue; // A * exp^[-(x²+y²)/R²]  
			else //oversaturated pixel: 
				pdImageSub[iIndex] = pdImageSub[iIndex] - dMaxIntensity;
			dWeight = dAmpl_x_Exp / dFuValue;  dSumOfWeights += dWeight;
			dNorm += pdImageSub[iIndex] * pdImageSub[iIndex];    dWeightedNorm += pdImageSub[iIndex] * pdImageSub[iIndex] * dWeight;
			iIndex++;
		}//for all entries j in the line i
	}//i (end for all lines in the image) 
	dNorm /= (dV_Grid_y_Sub.Nown * dV_Grid_x_Sub.Nown);    dWeightedNorm /= dSumOfWeights;
	dNorm = sqrt(dNorm);   dWeightedNorm = sqrt(dWeightedNorm);
	dNorm /= dMaxIntensity, dWeightedNorm /= dMaxIntensity;

	delete[] piImageSub;   vDeallocMinDataArray(&pMin);
	return bSuc;
}


void vCalcAverageAndBroadnessOfSpectrum(dVector *pdVk, dVector *pdVRef_minus_Dark, double &dAverageWavelength, double &dRelativeSpectralBroadness)
//This function is made for the characterization of the spectrum during the optical alignment check of the LISE-HF. The purpose is to characterize the spectrum 
//of each LISE-HF in terms of average wavelength and spectral broadness in real time during the optical alignment so that we can make our sensors match the 
//specifications so that they become spectrally more uniform from tool to tool.		W. Iff, 5.3.2024. 
{
	int i;
	double dk_Average = 0.0, dSumOfSpectrum = 0.0, dDelta_k, dSpectrumAverage, dk_StandardDev = 0.0;

	for (i = 0; i < pdVk->Nown; i++) {
		dk_Average += pdVk->Data[i] * pdVRef_minus_Dark->Data[i];   dSumOfSpectrum += pdVRef_minus_Dark->Data[i];
	}
	dk_Average /= dSumOfSpectrum;    dAverageWavelength = 2.0 * PI / dk_Average;

	for (i = 0; i < pdVk->Nown; i++) {
		dDelta_k = pdVk->Data[i] - dk_Average;     dk_StandardDev += dDelta_k * dDelta_k * pdVRef_minus_Dark->Data[i];
	}
	dSpectrumAverage = dSumOfSpectrum / (double)pdVk->Nown;    dk_StandardDev = dk_StandardDev / dSpectrumAverage;
	dk_StandardDev = sqrt(dk_StandardDev / (double)(pdVk->Nown - 1));
	dRelativeSpectralBroadness = dk_StandardDev / dk_Average;
}

bool bCharacterizeSpectrum(int iNoPixels, double *pdWavelength, double *pdDark, double *pdReference,
	double &dAverageWavelength, double &dRelativeSpectralBroadness, double &dAverageWavelengthEquidistSampl, double &dRelativeSpecBroadnessEquidistSampl)
	//This function is made for the characterization of the spectrum during the optical alignment check of the LISE-HF. The purpose is to characterize the spectrum 
	//of each LISE-HF in terms of average wavelength and spectral broadness in real time during the optical alignment so that we can make our sensors match the 
	//specifications so that they become spectrally more uniform from tool to tool. There are 2 ways to calculate the average wavelength and spectral broadness: 
	//The non-equidistant sampling in k-space given by the spectrometer pixels and the equidistant sampling in k-space obtained from interpolation. We try both and compare.  
	//W. Iff, 5.3.2024;  revision at 11.6.2024. 
{
	if (iNoPixels < 4 || pdWavelength == NULL || pdDark == NULL || pdReference == NULL)
		return false;
	if (pdWavelength[iNoPixels - 1] <= pdWavelength[0] + NUM_NOISE_FLOAT)
		return false;

	int i, j;
	double dOverallReferencePower = 0.0, dOverallDarkPower = 0.0, dOverallSignalPower;
	for (i = 0; i < iNoPixels; i++) {
		if (pdDark[i] >= 0.9 * MAX_PERMITTED_POWER_PER_PIXEL || pdReference[i] >= 0.9 * MAX_PERMITTED_POWER_PER_PIXEL)
			return false;
		dOverallReferencePower += pdReference[i];  dOverallDarkPower += pdDark[i];
	}
	dOverallSignalPower = dOverallReferencePower - dOverallDarkPower;
	if (dOverallSignalPower < iNoPixels * MIN_PERMITTED_AVERAGE_POWER_PER_PIXEL || dOverallSignalPower < dOverallDarkPower)
		return false;

	int iDimEquidist = 2048, iIndexLeftLeft;
	double dDelta_k, dWeightLeft, dWeightRight;
	dVector dVk(iNoPixels), dVRef_minus_Dark(iNoPixels), dVRef_minus_Dark_Equidist(iDimEquidist), dVk_Equidist(iDimEquidist);

	for (i = 0; i < iNoPixels; i++) {
		dVRef_minus_Dark.Data[iNoPixels - 1 - i] = pdReference[i] - pdDark[i];   dVk.Data[iNoPixels - 1 - i] = 2 * PI / pdWavelength[i];
	}
	vCalcAverageAndBroadnessOfSpectrum(&dVk, &dVRef_minus_Dark, dAverageWavelength, dRelativeSpectralBroadness);

	dVk_Equidist.Data[0] = dVk.Data[0];  dVk_Equidist.Data[iDimEquidist - 1] = dVk.Data[iNoPixels - 1]; //let's be exact here (see also below)
	dDelta_k = (dVk_Equidist.Data[iDimEquidist - 1] - dVk_Equidist.Data[0]) / (double)(iDimEquidist - 1);
	for (i = 1; i < iDimEquidist - 1; i++)
		dVk_Equidist.Data[i] = dVk_Equidist.Data[0] + i * dDelta_k;
	i = 0;
	while (dVk_Equidist.Data[i] <= dVk.Data[1]) {
		dWeightRight = (dVk_Equidist.Data[i] - dVk.Data[0]) / (dVk.Data[1] - dVk.Data[0]);  dWeightLeft = 1.0 - dWeightRight;
		dVRef_minus_Dark_Equidist.Data[i] = dWeightLeft * dVRef_minus_Dark.Data[0] + dWeightRight * dVRef_minus_Dark.Data[1];
		i++;
	}
	j = 2;
	while (dVk_Equidist.Data[i] < dVk.Data[iNoPixels - 2]) { //there are at least 2 data points left and 2 data points right:
		while (dVk.Data[j] < dVk_Equidist.Data[i])
			j++;
		iIndexLeftLeft = j - 2;
		if (bParabolicFit_4(dVk_Equidist.Data[i], iIndexLeftLeft, iNoPixels - 1, dVk.Data + iIndexLeftLeft, dVRef_minus_Dark.Data + iIndexLeftLeft,
			dVRef_minus_Dark_Equidist.Data + i, NULL, NULL) == false)
			return false;
		i++;
	}
	while (i < iDimEquidist) {
		dWeightRight = (dVk_Equidist.Data[i] - dVk.Data[iNoPixels - 2]) / (dVk.Data[iNoPixels - 1] - dVk.Data[iNoPixels - 2]);  dWeightLeft = 1.0 - dWeightRight;
		dVRef_minus_Dark_Equidist.Data[i] = dWeightLeft * dVRef_minus_Dark.Data[iNoPixels - 2] + dWeightRight * dVRef_minus_Dark.Data[iNoPixels - 1];
		i++;
	}
	vCalcAverageAndBroadnessOfSpectrum(&dVk_Equidist, &dVRef_minus_Dark_Equidist, dAverageWavelengthEquidistSampl, dRelativeSpecBroadnessEquidistSampl);

	return true;
}

bool bEvaluateArrayTestOn_35553_Grid(double dStepSize, double *pdTSV_Depth, double dStandardStepSize,
	double &dAverage, double &dGrad_45_deg, double &dGrad_135_deg, double &dModulusOfGrad, double &dDefocus)
	//This function evaluates the results from the array test on the grid specified below. The array test determines the measured TSV depths in dependence on the 
	//spot positions at the TSV. Before the array test, the user centres the spot on the TSV (position 10 below). Then, he presses the button for the array test 
	//(x-y-analysis). Subsequently, FPMS / Analyse measures the TSV depth at the positions 0, 1, ..., 20, saves the results in the array "*pdTSV_Depth" and calls 
	//bEvaluateArrayTestOn_35553_Grid(...), which calculates average and gradient from it, which is needed for optical alignment of the LISE-HF. 
	//The x-y-grid for the array test is the following: 
	//		0	1	2 
	//	3	4	5	6	7
	//	8	9	10	11	12   
	//	13	14	15	16	17		
	//		18	19	20
	//The number of points indicates the sequence of the measurements and entries in the array here. Point 10 is exactly centred on the TSV. The x-y-grid is equidistant  
	//and consists of squares of the size  dStepSize x dStepSize. From this, the average and the gradient of the measured TSV depth is calculated in 45° and 135° direction 
	//(direction of the x-y-alignment screws) as well as the defocus. In the future, we may calculate also higher-order aberrations. 
	//Important convention: The measured depth has to be set to FLOAT_MAX for invalid data points (failed measurement points).  
	//1st line of arguments of this function: input,  2nd line: output (results).		W. Iff, 2.5.2024
{
	int i;
	double dx[21], dy[21], dx_45_deg[21], dy_135_deg[21], dSqrt2 = sqrt(2.0), dRadiusMax, dRadius, dZernikePoly_2_0[21];
	double dNormalizationDefoc = dStandardStepSize * dStandardStepSize * 136.0 / 21.0; //for normalization of the defocus term on the given 35553-grid
	dMatrix dM(21, 4);

	dx[0] = -dStepSize, dx[1] = 0, dx[2] = dStepSize;
	for (i = 3; i <= 7; i++) {
		dx[i + 10] = dx[i + 5] = dx[i] = (i - 5.0) * dStepSize;
	}
	dx[18] = -dStepSize, dx[19] = 0, dx[20] = dStepSize;

	dy[0] = dy[1] = dy[2] = 2 * dStepSize;
	for (i = 3; i <= 7; i++) {
		dy[i] = dStepSize;   dy[i + 5] = 0;  dy[i + 10] = -dStepSize;
	}
	dy[18] = dy[19] = dy[20] = -2 * dStepSize;

	for (i = 0; i <= 20; i++) {
		dx_45_deg[i] = dx[i] / dSqrt2 + dy[i] / dSqrt2,
			dy_135_deg[i] = -dx[i] / dSqrt2 + dy[i] / dSqrt2;
	}

	dRadiusMax = dStepSize * sqrt(136.0 / 21.0); //Only this dRadiusMax leads to fulfillment of the orthogonality condition between the constant function "1" and  
	for (i = 0; i <= 20; i++) {																//the defocus basis function on this given discrete grid of 21 points.
		dRadius = sqrt(dx[i] * dx[i] + dy[i] * dy[i]);
		dZernikePoly_2_0[i] = 2.0 * dRadius * dRadius - dRadiusMax * dRadiusMax;
		dZernikePoly_2_0[i] /= dNormalizationDefoc;
	}

	for (i = 0; i <= 20; i++) {
		dM.Data[4 * i] = 1.0, dM.Data[4 * i + 1] = dx_45_deg[i], dM.Data[4 * i + 2] = dy_135_deg[i], dM.Data[4 * i + 3] = dZernikePoly_2_0[i];
	}

	dVector dVf(21);

	for (i = 0; i < 21; i++) {
		if (pdTSV_Depth[i] < NUM_NOISE_FLOAT || abs(pdTSV_Depth[i]) > MAX_DETECTABLE_DEPTH)
			return false; //We exclude invalid data points (failed or incomplete array tests) from the evaluation
		dVf.Data[i] = pdTSV_Depth[i];
	}//i

	dMatrix dM_transp(4, 21); dM_transp.transp(dM);
	dMatrix dM_transp_x_dM(dM_transp);
	dM_transp_x_dM *= dM;
	dMatrix dM_inv(4, 4); dM_inv.vSetToUnityMat();  dM_inv /= dM_transp_x_dM;
	dVector dVResult(dVf);  dVResult *= dM_transp;  dVResult *= dM_inv;

	dAverage = dVResult.Data[0], dGrad_45_deg = dVResult.Data[1], dGrad_135_deg = dVResult.Data[2], dDefocus = dVResult.Data[3];
	dModulusOfGrad = sqrt(dGrad_45_deg * dGrad_45_deg + dGrad_135_deg * dGrad_135_deg);
	return true;
}

bool bRead_FPMS_Camera_image(char *szFileName, int &iNoLines, int &iNoColumns, int **ppIntensityImage, char szAdditional_message[], int iNo_char)
//This function reads a camera image, which has been recorded in FPMS as png and subsequently been converted and saved by Matlab.  W. Iff, 7.3.2024  
{
	bool bSuc = true;
	int i, j, iLineNo = 0;
	char *szPointerTemp = NULL;
	string strNextLine;
	ifstream  fFile(szFileName);
	sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image File: %s", szFileName);
	if (!fFile.good()) {
		sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image Not good");
		cout << "\n Error! Opening of Input-file failed! \n"; return false;
	}
	sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image Step 0");
	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo)) //the first line contains the comments 
		return false;

	sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image Step 1");
	if (sscanf_s(strNextLine.c_str(), "%d lines, %d columns", &iNoLines, &iNoColumns) != 2)
		return false;

	sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image Step 2 %i %i %i line %s", iNoLines, iNoColumns, iLineNo, strNextLine.c_str());
	int *pIntensityImage = new int[iNoLines * iNoColumns];  int *pIntensityImageTemp = pIntensityImage;
	for (i = 0; i < iNoLines; i++) {
		bSuc = bSuc && bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo);

		if (bSuc) {
			szPointerTemp = (char*)strNextLine.c_str();

			for (j = 0; j < iNoColumns - 1; j++) {
				if (sscanf_s(szPointerTemp, "%d", pIntensityImageTemp) != 1) {
					sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image error A line %i column %i", i, j);
					bSuc = false;   goto END;
				}
				if ((szPointerTemp = strstr(szPointerTemp, ",")) == NULL) {
					sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image error B line %i column %i", i, j);
					bSuc = false;   goto END;
				}
				pIntensityImageTemp++;   szPointerTemp++;
			}//j
			if (sscanf_s(szPointerTemp, "%d", pIntensityImageTemp) != 1) {
				sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image error C line %i column %i", i, j);
				bSuc = false;   goto END;
			}
			pIntensityImageTemp++;
		}//end if bSuc (next line)
		else
			goto END;
	}//end for i 
	sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image Step 3: %s", szFileName);
	if (!bReadNextLine_ASCII(&fFile, &strNextLine, &iLineNo)) { //last line 
		bSuc = false;   goto END;
	}
	sprintf_s(szAdditional_message, iNo_char * sizeof(char), "bRead_FPMS_Camera_image Step 4: %s", szFileName);
	if (strstr(strNextLine.c_str(), "end") == NULL)
		bSuc = false;

END:
	if (bSuc)
		*ppIntensityImage = pIntensityImage;
	else
		delete[] pIntensityImage;
	return bSuc;
}

extern "C"
{
	__declspec(dllexport) bool LISE_HF_Compute(double dTSV_diam, int iNo_layers, double *pOptical_thicknesses, double *pOptical_thickness_tolerances, double *pRef_index_re, double *pRef_index_im,
		int iNo_pixels, double *pWavelength_nm, double *pDark_spectrum, double *pReference_spectrum, double *pSpectrum, int &iFT_dim,
		int iOperating_Mode, bool bPeakDetectionOnRight, bool bNewPeakDetection,
		double dThreshold_valid_signal, double dThreshold_peak, double dz_Resolution, bool bDebugOutput, int iDebugFileNo,
		double &dAsymptStdErr, double &dNormalizedResidual, double &dQuality, char szAdditional_message[], int iNo_char, double *pModulus_of_FT, double *pz,
		int &iNo_detected_peaks, double *pComputed_optical_thicknesses, double *pAmplitudes_of_peaks, double *pComputed_geom_thicknesses) 
	{

		bool bFT_done = false;
		bool bSignal_analysis_done = false;
		double dMaxPeakFFTAmplitudeinWindow = 0;
		stringstream ssMessage;

		vLISE_HF_main(dTSV_diam, iNo_layers, pOptical_thicknesses, pOptical_thickness_tolerances, pRef_index_re, pRef_index_im,
			iNo_pixels, pWavelength_nm, pDark_spectrum, pReference_spectrum, pSpectrum, NULL, iFT_dim,
			iOperating_Mode, bPeakDetectionOnRight, bNewPeakDetection,
			dThreshold_valid_signal, dThreshold_peak, dz_Resolution, bDebugOutput, iDebugFileNo,
			dAsymptStdErr, dNormalizedResidual, dQuality, &ssMessage, pModulus_of_FT, pz,
			iNo_detected_peaks, pComputed_optical_thicknesses, pAmplitudes_of_peaks, pComputed_geom_thicknesses, bFT_done, bSignal_analysis_done, dMaxPeakFFTAmplitudeinWindow);

		string sMessage(ssMessage.str());  //in Olovia dll
		const char *pcString = sMessage.c_str();
		size_t size = sMessage.size();  //this is the same as sMessage.length();
		if (iNo_char > (signed int) size) {
			memcpy(szAdditional_message, pcString, size * sizeof(char));
			szAdditional_message[size] = '\0';
		}

		return bSignal_analysis_done;
	}

	__declspec(dllexport) bool LISE_HF_BeamProfiler(char* szImageFile, double dPixelSize_y, double dPixelSize_x, double &dAmpl, double &dxGauss, double &dyGauss, double &dRadius, double& dBackground, double& dMaxIntensity, double &dNorm, double &dWeightedNorm,
		double &dRatioOfAxisOfEllipse, double &dAngleOfEllipse, char szAdditional_message[], int iNo_char)
	{
		//This function tests and shows how to use the function which fits a Gaussian into the LISE-HF or LISE-ED spot on the camera intensity image. 
		//The unit for the length is micrometer. The origin of the local coordinate frame is in the centre of the camera image ( = centre of the red 
		//crosshair taget). The Gaussian spot is usually at or at least very close to the red crosshair target (centre of the image). 
		//See bRead_FPMS_Camera_image(...) for further details. 	W. Iff, 13.5.2024

		//"H:/Beam-images 1-3-24/With 50x VIS obj/30dB 40ms-100um above focus.txt";
		//"H:/Beam-images 1-3-24/Without obj/30dB 6ms-centred.txt";  
		//"H:/Beam-images 1-3-24/Without obj/30dB 65ms-oversaturated.txt";  //file with no of pixels in x, y and matrix of intensity values
		int iNoLines, iNoColumns, *piImage = NULL;
		int iNoLinesSub;
		int iNoColumnsSub;
		double *pdImageSub = NULL;
		dBackground = 6.5;
		dMaxIntensity = 255.0;
		dNorm = DBL_MAX;
		dAmpl = 255.0 * 0.7;
		dWeightedNorm = DBL_MAX;

		bool result = false;

		sprintf_s(szAdditional_message, iNo_char * sizeof(char), "Open File: %s", szImageFile);
		//The rest follows all from the input above:  
		bool bRead_FPMS_Camera_image(char *szFileName, int &iNoLines, int &iNoColumns, int **ppIntensityImage, char szAdditional_message[], int iNo_char);
		if (bRead_FPMS_Camera_image(szImageFile, iNoLines, iNoColumns, &piImage, szAdditional_message, iNo_char) == false)
			goto END;

		iNoLinesSub = (int)(iNoLines * 0.5 + 0.5);
		iNoLinesSub = 2 * (iNoLinesSub / 2 + 1);   iNoColumnsSub = iNoLinesSub;
		dxGauss = 0.0, dyGauss = 0.0; //The Gaussian spot is approx. at the centre of the image.
		dRadius = 11.0;
		pdImageSub = new double[iNoLinesSub * iNoColumnsSub];
		sprintf_s(szAdditional_message, iNo_char * sizeof(char), "Start Computing %i %i", iNoLines, iNoColumns);
		result = bBeamProfiler(iNoLines, iNoLinesSub, dPixelSize_y, iNoColumns, iNoColumnsSub, dPixelSize_x, piImage, pdImageSub, dAmpl, dxGauss, dyGauss, dRadius, dBackground, dMaxIntensity, dNorm, dWeightedNorm, dRatioOfAxisOfEllipse, dAngleOfEllipse);

		sprintf_s(szAdditional_message, iNo_char * sizeof(char), "End Computing %i %i", iNoLines, iNoColumns);
	END:
		delete[] piImage;
		delete[] pdImageSub;
		return result;
	}

	__declspec(dllexport) bool LISE_HF_BeamProfilerExtended(char* szImageFile, double dPixelSize_y, double dPixelSize_x, double &dAmpl, double &dxGauss, double &dyGauss,
		double &dxyGauss_45deg, double &dxyGauss_135deg, double &dRadius, double& dBackground, double& dMaxIntensity, double &dNorm, double &dWeightedNorm,
		double &dRatioOfAxisOfEllipse, double &dAngleOfEllipse, char szAdditional_message[], int iNo_char, int &iNoLinesSub, int &iNoColumnsSub, double *pdImageSub)
	{
		//This function tests and shows how to use the function which fits a Gaussian into the LISE-HF or LISE-ED spot on the camera intensity image. 
		//The unit for the length is micrometer. The origin of the local coordinate frame is in the centre of the camera image ( = centre of the red 
		//crosshair taget). The Gaussian spot is usually at or at least very close to the red crosshair target (centre of the image). 
		//See bRead_FPMS_Camera_image(...) for further details. 	
		//We need to output the spot position in the normal x-y-coordinate system for apps eng. and a 45° rotated system (direction of the screws) for optical alignment.
		//W. Iff, 14.7.2024 

		//"H:/Beam-images 1-3-24/With 50x VIS obj/30dB 40ms-100um above focus.txt";
		//"H:/Beam-images 1-3-24/Without obj/30dB 6ms-centred.txt";  
		//"H:/Beam-images 1-3-24/Without obj/30dB 65ms-oversaturated.txt";  //file with no of pixels in x, y and matrix of intensity values
		int iNoLines, iNoColumns, *piImage = NULL;
		//int iNoLinesSub; int iNoColumnsSub;  double *pdImageSub = NULL;  
		dBackground = 6.5;
		dMaxIntensity = 255.0;
		dNorm = DBL_MAX;
		dAmpl = 255.0 * 0.7;
		dWeightedNorm = DBL_MAX;

		bool result = false;

		sprintf_s(szAdditional_message, iNo_char * sizeof(char), "Open File: %s", szImageFile);
		//The rest follows all from the input above:  
		bool bRead_FPMS_Camera_image(char *szFileName, int &iNoLines, int &iNoColumns, int **ppIntensityImage, char szAdditional_message[], int iNo_char);
		if (bRead_FPMS_Camera_image(szImageFile, iNoLines, iNoColumns, &piImage, szAdditional_message, iNo_char) == false)
			goto END;

		iNoLinesSub = (int)(iNoLines * 0.5 + 0.5);
		iNoLinesSub = 2 * (iNoLinesSub / 2 + 1);   iNoColumnsSub = iNoLinesSub;
		dxGauss = 0.0, dyGauss = 0.0; //The Gaussian spot is approx. at the centre of the image.
		dRadius = 11.0;
		//pdImageSub = new double[iNoLinesSub * iNoColumnsSub];
		sprintf_s(szAdditional_message, iNo_char * sizeof(char), "Start Computing %i %i", iNoLines, iNoColumns);
		result = bBeamProfiler(iNoLines, iNoLinesSub, dPixelSize_y, iNoColumns, iNoColumnsSub, dPixelSize_x, piImage, pdImageSub, dAmpl, dxGauss, dyGauss, dRadius, dBackground, dMaxIntensity, dNorm, dWeightedNorm, dRatioOfAxisOfEllipse, dAngleOfEllipse);
		dxyGauss_45deg = (dxGauss + dyGauss) / sqrt(2.0), dxyGauss_135deg = (dyGauss - dxGauss) / sqrt(2.0); //_deg means degree

		sprintf_s(szAdditional_message, iNo_char * sizeof(char), "End Computing %i %i", iNoLines, iNoColumns);
	END:
		delete[] piImage;
		return result;
	}


	__declspec(dllexport) bool LISE_HF_bCharacterizeSpectrum(int iNoPixels, double *pdWavelength, double *pdDark, double *pdReference,
		double &dAverageWavelength, double &dRelativeSpectralBroadness, double &dAverageWavelengthEquidistSampl,
		double &dRelativeSpecBroadnessEquidistSampl, char szAdditional_message[], int iNo_char)
	{
		//This function shows how to use "bCharacterizeSpectrum(...)" for the characterization of the spectrum during the optical alignment check of the LISE-HF. 
		//The purpose is to characterize the spectrum of each LISE-HF in terms of average wavelength and spectral broadness in real time during the optical 
		//alignment so that we can make our sensors match the specifications so that they become spectrally more uniform from tool to tool.   W. Iff, 5.3.2024.

		bool bSuccess = bCharacterizeSpectrum(iNoPixels, pdWavelength, pdDark, pdReference,
			dAverageWavelength, dRelativeSpectralBroadness, dAverageWavelengthEquidistSampl, dRelativeSpecBroadnessEquidistSampl);
		if (bSuccess == false)
			sprintf_s(szAdditional_message, iNo_char * sizeof(char), "Failure. Please check your signal and its magnitude.");
		return bSuccess;
	}

	__declspec(dllexport) bool LISE_HF_bEvaluateArrayTest(double dStepSize, double *pdTSV_Depth, double dStandardStepSize,
		double &dAverage, double &dGrad_45_deg, double &dGrad_135_deg, double &dModulusOfGrad, double &dDefocus)
	{
		//This function is needed for the real-time array test for optical alignment of the LISE-HF.	W. Iff, 2.5.2024

		double adTSV_Depth[21];

		for (int i = 0; i < 21; i++)
		{
			adTSV_Depth[i] = pdTSV_Depth[i];
		}


		return bEvaluateArrayTestOn_35553_Grid(dStepSize, adTSV_Depth, dStandardStepSize, dAverage, dGrad_45_deg,
			dGrad_135_deg, dModulusOfGrad, dDefocus);
	}
}
