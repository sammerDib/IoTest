#include "resource.h"		// symboles principaux
#include <fstream>
#include <iostream>
#include <string>
#include  <memory>
#include "math.h"
#include <stdio.h>
#include <vector>
#include "stdafx.h"

using namespace std;

class CProfil 
{	
public:
	CProfil(CString p_sFilePath, int nframesize);
	virtual ~CProfil();;
	CProfil();

	double ComputeNoiseIndex(LPCTSTR p_csEditFile,int p_nFilterFrameSize,int p_nFilterOrder, double p_dSeuil);
	bool IsLoaded(){return m_bLoadSuccess;}

	static void SetDbgMode(bool bDbg);

private :
	vector<double> m_vdVecteur;
	bool	m_bLoadSuccess;

	int m_ntaille;
	void VectSetTaille(int n);
	void VectSetValues(int position, double valeur);
	CProfil sgolay(int f,int k);
	void  diff(CProfil v);
	void penalisation(double dseuil);
	double mean();
	void ecriture();
	void reflectborders(int p_nFilterFrameSize);
};