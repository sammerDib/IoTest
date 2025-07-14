
/* Notice that the beginning and the end of the profil can't be filtered i.e. :
The first (sizeframe-1)/2 points can't be filtered and the last (sizeframe-1)/2 points also.
To obtain this we could interpolate (sizeframe-1)/2 points at each edge of  the profil 
which would be used to construct the first filtered points and the last.
Nevertheless interpolate would be dangerous if sizeframe is big and could mistake our final result.
*/

#include "stdafx.h"
#include "NanoNoiseIndex.h"
#include "NanoNoiseIndexDlg.h"
#include  <memory>
#include <Eigen/Dense>
#include <Eigen/LU>
#include <fstream>
#include <iostream>
#include <string>
#include "math.h"
#include <stdio.h>
#include "CProfil.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif

using namespace Eigen;
using namespace std;

static bool g_bDbgMode = false;

CProfil CProfil::sgolay( int i_f,int i_k)
{
	i_k = i_k+1;
	i_f = i_f-(i_f-1)%2;
	MatrixXd coeffs(i_f,1);
	MatrixXd sol(i_f,1);
	MatrixXd X_dJac(i_f,i_k);
	MatrixXd X_dX_dJacm(i_k,i_k);
	MatrixXd X_dX_dJact(i_k,i_f);
	MatrixXd y_inter(i_f,1);
	MatrixXd y_liss(m_ntaille,1);
	MatrixXd X_dX_dJacinter(i_k,i_f);

	const int dim = i_f;
	const int degre = i_k;
	int ending = m_ntaille-i_f;

	for (int decalage=0;decalage<ending;decalage++)
	{
		//write X_dJacobian to obtain convolution coeff
		for (int i=0;i<i_f;i++)
		{
			for (int j=0;j<degre;j++)
			{
				X_dJac (i,j) = pow ((double)i-(double)(i_f-1)/2.0 ,(double) j);                  //((double)i+decalage-((double)(i_f/2)+decalage))^(double)j;
			}
		}
		//X_dX_dJact
		for (int i=0;i<i_f;i++)
		{
			for (int j=0;j<degre;j++)
			{
				X_dX_dJact (j,i) = X_dJac (i,j);
			}
		}
		// (transpose(J)*J) (X_dX_dJact*X_dJac)*x=X_dX_dJact*y_inter;
		//initialisation of X_dX_dJacm
		for (int i=0;i<degre;i++)
		{
			for (int j=0;j<degre;j++)
			{
				X_dX_dJacm(i,j) = 0;
			}
		}
		X_dX_dJacm = X_dX_dJact * X_dJac;

		for (int i=0;i<i_f;i++)
		{
			y_inter(i) = this->m_vdVecteur[i+decalage];
		}

		//solving by system
		sol = X_dX_dJact*y_inter; // sol = X_dX_dJact*y_inter
		coeffs = X_dX_dJacm.fullPivLu().solve(sol); //solveur needed

		//solving with inversing
		// 		X_dX_dJacinter = X_dX_dJacm.inverse()*X_dX_dJact;
		// 		coeffs = X_dX_dJacinter*y_inter;

		y_liss(decalage+(i_f+1)/2) = coeffs(0);


		//print to debug
		// 		string path7 = "D:\\devHC\\data\\X_dX_dJacinter.txt";
		// 		FILE * pfile7;
		// 		errno_t err7 = fopen_s(&pfile7,path7.c_str(),"w+");
		// 		for (int j=0;j<i_f;j++)
		// 		{
		// 			for (int i=0;i<i_k;i++)
		// 			{
		// 				fprintf (pfile7, "%f\n", X_dX_dJacinter(i,j)); 
		// 			}
		// 		}
		// 		fclose(pfile7);
		// 
		// 		string path6 = "D:\\devHC\\data\\coeffs.txt";
		// 		FILE * pfile6;
		// 		errno_t err6 = fopen_s(&pfile6,path6.c_str(),"w+");
		// 		for (int j=0;j<i_k;j++)
		// 		{
		// 			fprintf (pfile6, "%f\n", coeffs(j)); 
		// 		}
		// 		fclose(pfile6);
		// 
		// 
		// 		string path = "D:\\devHC\\data\\X_dJac.txt";
		// 		FILE * pfile;
		// 		errno_t err = fopen_s(&pfile,path.c_str(),"w+");
		// 		for (int j=0;j<i_k;j++)
		// 		{
		// 			for (int i=0;i<i_f;i++)
		// 			{
		// 					fprintf (pfile, "%f\n", X_dJac(i,j)); 
		// 			}
		// 		}
		// 		fclose(pfile);
		// 
		// 		string path5 = "D:\\devHC\\data\\y_inter.txt";
		// 		FILE * pfile5;
		// 		errno_t err5 = fopen_s(&pfile5,path5.c_str(),"w+");
		// 		for (int j=0;j<i_f;j++)
		// 		{
		// 			fprintf (pfile5, "%f\n", y_inter(j)); 
		// 		}
		// 		fclose(pfile5);

	}
	//return
	
	if (g_bDbgMode)
	{
		string path4 = "Dbg_liss.txt";
		FILE * pfile4;
		errno_t err4 = fopen_s(&pfile4,path4.c_str(),"w+");
		for (int j=i_f;j<m_ntaille-i_f;j++)
		{
			fprintf (pfile4, "%lf;%lf\n", y_liss(j),m_vdVecteur[j]); 
		}
		fclose(pfile4);
	}
	
	for (int i=(i_f+1)/2;i<this->m_ntaille-(i_f+1)/2;i++)
	{
		this->m_vdVecteur[i] = y_liss(i);
	}

	return *this;
}

void CProfil::penalisation(double dseuil)
{
	CProfil v;
	v.m_vdVecteur.resize(this->m_ntaille);
	v.m_ntaille = m_vdVecteur.size();
	for (int i=0;i<m_vdVecteur.size();i++)
	{
		 v.m_vdVecteur[i] = this->m_vdVecteur[i] - dseuil;
		if (v.m_vdVecteur[i] < 0.0)
		{
			v.m_vdVecteur[i] = 0.0;
		}
		v.m_vdVecteur[i] = exp (v.m_vdVecteur[i]);
		this->m_vdVecteur[i] = this->m_vdVecteur[i] * v.m_vdVecteur[i];
	}
}

double CProfil::mean()
{
	double somme = 0.0;
	for (int i=0;i<m_vdVecteur.size();i++)
	{
		somme = somme + this->m_vdVecteur[i];
	}
	somme = somme/(double)m_vdVecteur.size();
	return somme;
}

void CProfil::diff(CProfil v)
{
	if (m_vdVecteur.size() == v.m_vdVecteur.size() )
	{	
		for (int i=0;i<v.m_vdVecteur.size();i++)
		{
			this->m_vdVecteur[i] = fabs(this->m_vdVecteur[i] - v.m_vdVecteur[i]); 
		}
	}
}

CProfil::CProfil()
{
	this->m_ntaille = 0;
	m_bLoadSuccess = false;
}

CProfil::~CProfil()
{
	m_vdVecteur.clear();
}

CProfil::CProfil(CString p_sFilePath, int p_nFilterFrameSize)
{
	m_bLoadSuccess = false;
	/*	CStdioFile*/

	FILE * pfile;
	float l_fx = 0;
	errno_t err = fopen_s(&pfile,(LPCTSTR) p_sFilePath,"r+");

	if(err)
	{
		if(pfile == NULL)
		{
			AfxMessageBox("Error : could not open profile file");
			return ;
		}
	}
	else
	{
		int i = 1; int n = 1;
		int valeur_debut =7;// reading begin line 8
		m_bLoadSuccess = true;
		while(n > 0 && m_bLoadSuccess)
		{
			if(i > valeur_debut)
			{
				float fval = 0.0;
				n = fscanf( pfile, "%f;%f;\n",&l_fx , &fval);
				m_vdVecteur.push_back(fval);
				i++;
				m_ntaille = i-7;
			}
			else
			{
				char line[100];
				if( fgets( line, 100, pfile ) == NULL)
				{
					AfxMessageBox("ERROR in  fgets( line, 100, pfile )");
					m_bLoadSuccess = false;
				}
				i++;
			}
		}
			
		fclose(pfile);
		m_ntaille = m_vdVecteur.size();
		if(m_bLoadSuccess)
			reflectborders(p_nFilterFrameSize);
	}
}

void CProfil::reflectborders(int p_nFilterFrameSize)
{
	// on recopie en debut et fin de vecteur la reflection des bords de vecteur pour absorber la limitation du au filtre sgolay
	int relectsize =  p_nFilterFrameSize-(p_nFilterFrameSize-1)%2;
	int i=0;
	// end border reflection
	for (i=m_ntaille-1; i>=m_ntaille-relectsize; i--)
	{
		double d= m_vdVecteur[i];
		m_vdVecteur.push_back(m_vdVecteur[i]);
	}
	vector<double> vtemp;
	for (i=relectsize-1; i>=0; i--)
	{
		vtemp.push_back(m_vdVecteur[i]);
	}
	m_vdVecteur.insert(m_vdVecteur.begin(),vtemp.begin(),vtemp.end());
	m_ntaille = m_vdVecteur.size();

	if (g_bDbgMode)
	{
		string path4 = "Dbg_reflect.txt";
		FILE * pfile4;
		errno_t err4 = fopen_s(&pfile4,path4.c_str(),"w+");
		for (int j=0;j<m_ntaille;j++)
		{
			fprintf (pfile4, "%lf\n", m_vdVecteur[j]); 
		}
		fclose(pfile4);
	}
}

void CProfil::ecriture()
{
	string path = "D:\\devHC\\data\\logvec.txt";
	FILE * pfile;
	errno_t err = fopen_s(&pfile,path.c_str(),"w+");
	for (int i=0;i<this->m_ntaille;i++)
	{
		fprintf (pfile, "%f\n", this->m_vdVecteur[i]);  
	}
	fclose(pfile);
}

void CProfil::VectSetTaille(int n)
{
	this->m_ntaille = n;
}
void CProfil::VectSetValues(int position,double valeur)
{
	this->m_vdVecteur[position] = valeur;
}

double CProfil::ComputeNoiseIndex(LPCTSTR p_csEditFile,int p_nFilterFrameSize,int p_nFilterOrder, double p_dSeuil)
{
	double dNoiseIndex = 0.0;
	CProfil o_x;
	CProfil o_y;
	CProfil o_z;
	o_x = CProfil((LPCTSTR)p_csEditFile,p_nFilterFrameSize);
	if(o_x.IsLoaded())
	{
		o_z = CProfil((LPCTSTR)p_csEditFile,p_nFilterFrameSize);
		if(o_z.IsLoaded())
		{
			o_y = o_x.sgolay(p_nFilterFrameSize, p_nFilterOrder);
			o_y.diff(o_z);
			if(p_dSeuil>=0.0)
				o_y.penalisation(p_dSeuil);
			dNoiseIndex = o_y.mean();
		}
	}
	return dNoiseIndex;
}

void CProfil::SetDbgMode( bool bDbg )
{
	g_bDbgMode = bDbg;
}

