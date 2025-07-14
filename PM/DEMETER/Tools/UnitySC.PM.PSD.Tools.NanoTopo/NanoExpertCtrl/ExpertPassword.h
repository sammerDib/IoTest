#pragma once


// Boîte de dialogue CExpertPassword

class CExpertPassword : public CDialog
{
	DECLARE_DYNAMIC(CExpertPassword)

public:
	CExpertPassword(CWnd* pParent = NULL);   // constructeur standard
	virtual ~CExpertPassword();

// Données de boîte de dialogue
	enum { IDD = IDD_PSSWD };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // Prise en charge de DDX/DDV

	DECLARE_MESSAGE_MAP()
	virtual void OnOK();
public:
	CString m_csPwd;
};
