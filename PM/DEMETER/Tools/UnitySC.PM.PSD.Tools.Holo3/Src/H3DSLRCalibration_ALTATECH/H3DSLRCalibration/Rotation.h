// Rotation.h: interface for the CRotation class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_ROTATION_H__F87F1BA7_8E0D_11D4_9B12_006097707C94__INCLUDED_)
#define AFX_ROTATION_H__F87F1BA7_8E0D_11D4_9B12_006097707C94__INCLUDED_

#if _MSC_VER >= 1000
#pragma once
#endif // _MSC_VER >= 1000

#include "H3Matrix.h"

#define Rad long double
#define Triplet CH3Triplet< double >
#define Matrix CH3Matrix< double >

class CRotation:public Matrix  
{
public:
	/// <summary>
	/// Identity.
	/// </summary>
	CRotation();

	//CRotation(const Rad ro, const Rad theta=0, const Rad phi=0);
	//CRotation(const Triplet& T);
	CRotation(const CRotation &R);
	CRotation(const Matrix &M);
	CRotation(const double * const mat);

	virtual ~CRotation();

	CRotation operator*(const CRotation &R)const;
	CRotation operator*(const double x)const;

	CRotation operator=(const CRotation &R);

	double Trace();
	double Det();
	bool Check();
//private:
//	void defineData();
//
//public:
	//Triplet GetAngle();
	CRotation Trans()const;
	CRotation Inv(){return Trans();};

public:
    /// <summary>
    /// 3* angles to rotation matrix.
    /// </summary>
    CH3Matrix<double> Rodrigues(CH3Matrix<double>& dout);

	/// <summary>
	/// Rotation matrix to 3* angles.
	/// </summary>
	Matrix Rodrigues();

	//RoThetaPhi (older code not used anymore)
	//on tourne d'abord autour de z d'un angle ro
	//puis:				autour de y d'un angle theta
	//enfin:			autour de z d'un angle phi (Euler: on tourne à nouveau autour de z) 
	//Triplet RoThetaPhi;
	//bool AngleDefined;
};

#endif // !defined(AFX_ROTATION_H__F87F1BA7_8E0D_11D4_9B12_006097707C94__INCLUDED_)
