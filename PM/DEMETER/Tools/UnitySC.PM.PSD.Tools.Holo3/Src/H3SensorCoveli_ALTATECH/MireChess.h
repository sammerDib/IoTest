// MireChess.h: interface for the CMireChess class.
//
//////////////////////////////////////////////////////////////////////

#if !defined(AFX_MireChess_H__1669AD3F_F19B_48FC_8745_F219CDDE8EE2__INCLUDED_)
#define AFX_MireChess_H__1669AD3F_F19B_48FC_8745_F219CDDE8EE2__INCLUDED_

#if _MSC_VER > 1000
#pragma once
#endif // _MSC_VER > 1000

class CMireChess  
{
public:
	CMireChess();
	virtual ~CMireChess();
	//Nombre de points sur la mire
	long GetNbPtsX()const;
	void SetNbPtsX(long nNbPtsX);
	long GetNbPtsY()const;
	void SetNbPtsY(long nNbPtsY);
	//Pas de la mire
	float GetStepX()const;
	void SetStepX(float fStepX);
	float GetStepY()const;
	void SetStepY(float fStepY);
	//Couleur des points
	long GetPtsColor()const;
	void SetPtsColor(long nPtsColor);
	// lecture ecriture Setting
	bool LoadSettings(CString strFileName, CString strSection);
	bool SaveSettings(CString strFileName, CString strSection);
	
private:
	float m_fStepX;		// Nombre de pas en X
	float m_fStepY;		// Nombre de pas en Y
	long m_nNbPtsX;		// Nombre de point en X
	long m_nNbPtsY;		// Nombre de point en Y
	long m_nPtsColor;	// Couleur des point
};

#endif // !defined(AFX_MireChess_H__1669AD3F_F19B_48FC_8745_F219CDDE8EE2__INCLUDED_)
