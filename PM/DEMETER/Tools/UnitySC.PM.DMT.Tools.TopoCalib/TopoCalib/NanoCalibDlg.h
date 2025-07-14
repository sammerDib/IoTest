
// NanoCalibDlg.h : fichier d'en-tête
//

#pragma once

#include "DiagTab.h"
#include "DlgCalibCam.h"
#include "DlgCalibSys.h"
#include "DlgMesure.h"

// boîte de dialogue CNanoCalibDlg
class CNanoCalibDlg : public CDialogEx
{
// Construction
public:
	CNanoCalibDlg(CWnd* pParent = NULL);	// constructeur standard

// Données de boîte de dialogue
	enum { IDD = IDD_NANOCALIB_DIALOG };

	protected:
	virtual void DoDataExchange(CDataExchange* pDX);	// Prise en charge de DDX/DDV


// Implémentation
protected:
	HICON m_hIcon;

	// Fonctions générées de la table des messages
	virtual BOOL OnInitDialog();
    
    bool DoSilentSysCalib();
    bool DoSilentCamCalib();
	afx_msg void OnSysCommand(UINT nID, LPARAM lParam);
	afx_msg void OnPaint();
	afx_msg HCURSOR OnQueryDragIcon();
	DECLARE_MESSAGE_MAP()
public:
	CDiagTab m_ctlrTab;
	CDlgCalibCam m_oDlgCalibCam;
	CDlgCalibSys m_oDlgCalibSys;
	CDlgMesure	 m_oDlgMesure;

    bool m_isSilent;
};
