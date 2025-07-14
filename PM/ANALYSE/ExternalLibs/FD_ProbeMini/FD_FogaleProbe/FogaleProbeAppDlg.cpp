// FogaleProbeAppDlg.cpp : implementation file
//

#include "stdafx.h"
#include "FogaleProbeApp.h"
#include "FogaleProbeAppDlg.h"

#include "FogaleProbeCalibrate.h"

#include "FogaleProbe.h"
#include "FogaleProbeParamID.h"
#include "FogaleProbeReturnValues.h"

#include "..\FD_LISE_General\sc_fixed_array.h" // for test

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

/////////////////////////////////////////////////////////////////////////////
// CAboutDlg dialog used for App About

bool bWindowOpened;

class CAboutDlg : public CDialog
{
public:
	CAboutDlg();

// Dialog Data
	//{{AFX_DATA(CAboutDlg)
	enum { IDD = IDD_ABOUTBOX };
	//}}AFX_DATA

	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(CAboutDlg)
	protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	//}}AFX_VIRTUAL

// Implementation
protected:
	//{{AFX_MSG(CAboutDlg)
	//}}AFX_MSG
	DECLARE_MESSAGE_MAP()
};

CAboutDlg::CAboutDlg() : CDialog(CAboutDlg::IDD)
{
	//{{AFX_DATA_INIT(CAboutDlg)
	//}}AFX_DATA_INIT
}

void CAboutDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CAboutDlg)
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CAboutDlg, CDialog)
	//{{AFX_MSG_MAP(CAboutDlg)
		// No message handlers
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

/////////////////////////////////////////////////////////////////////////////
// CFogaleProbeAppDlg dialog

CFogaleProbeAppDlg::CFogaleProbeAppDlg(CWnd* pParent /*=NULL*/)
	: CDialog(CFogaleProbeAppDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CFogaleProbeAppDlg)
	m_labelVersion = _T("");
	m_ReturnFunction = _T("");
	//}}AFX_DATA_INIT
	// Note that LoadIcon does not require a subsequent DestroyIcon in Win32
	m_hIcon = AfxGetApp()->LoadIcon(IDR_MAINFRAME);
}

void CFogaleProbeAppDlg::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	//{{AFX_DATA_MAP(CFogaleProbeAppDlg)
	DDX_Text(pDX, IDC_LABELVERSION, m_labelVersion);
	DDV_MaxChars(pDX, m_labelVersion, 128);
	DDX_Text(pDX, IDC_RETURN_FUNCTION, m_ReturnFunction);
	//}}AFX_DATA_MAP
}

BEGIN_MESSAGE_MAP(CFogaleProbeAppDlg, CDialog)
	//{{AFX_MSG_MAP(CFogaleProbeAppDlg)
	ON_WM_SYSCOMMAND()
	ON_WM_PAINT()
	ON_WM_QUERYDRAGICON()
	ON_BN_CLICKED(IDC_GETVERSION, OnGetversion)
	ON_BN_CLICKED(IDC_PROBEINIT, OnProbeinit)
	ON_BN_CLICKED(IDC_GETTHICKNESS, OnGetthickness)
	ON_BN_CLICKED(IDC_BUTTON4, OnGetRawSignal)
	ON_BN_CLICKED(IDC_DEFINESAMPLE, OnDefinesample)
	ON_BN_CLICKED(IDC_START, OnStart)
	ON_BN_CLICKED(IDC_STOP, OnStop)
	ON_BN_CLICKED(IDC_CLOSEPROBE, OnCloseprobe)
	ON_BN_CLICKED(IDC_START2, OnDoSettings)
	ON_BN_CLICKED(IDC_BUTTON8, OnOpenSettingWindow)
	ON_BN_CLICKED(IDC_BUTTON1, OnCloseSettingWindow)
	ON_BN_CLICKED(IDC_LABELVERSION, OnLabelversion)
	ON_BN_CLICKED(IDC_BUTTON9, OnUpdateSettingWindow)
	ON_BN_CLICKED(IDC_DOPOWERTEST, OnDoPowerTest)
	ON_BN_CLICKED(IDC_DOXYTEST, OnDoXYTest)
	ON_BN_CLICKED(IDC_DOFOCUSTEST, OnDoFocusTest)
	ON_BN_CLICKED(IDC_DOMATTEST, OnDoMatTest)
	ON_BN_CLICKED(IDC_DOCALTEST, OnDoCalTest)
	ON_BN_CLICKED(IDC_PROBE_1, OnBnClickedProbe1)
	ON_BN_CLICKED(IDC_PROBE_2, OnBnClickedProbe2)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()


// Global declaration of probe ID
int ProbeID = 0;

// Function to display an error message box with the name of the function
void DisplayError(char* FunctionCall)
{
	// temporary string to store the name of function returning an error
	char cTemp[1024];
	// write in cTemp string the message to display
	sprintf_s(cTemp,"Function %s returning an error code",FunctionCall);
	// display in a message box the message
	MessageBox(0,cTemp,"Fogale Probe error",0);
}

// function initialisaiton, execute the initialisation of probe when user clic on Init button of MFC 
void CFogaleProbeAppDlg::OnProbeinit() 
{
#ifdef NOHARDWARE

	ProbeID = FPInitialize(0, 0, 0, 0);
	//ProbeID = FPInitialize("Probe_Configuration_File.txt", 0, 0, 0);
	//ProbeID = FPInitialize("Config_SimuNoLin.txt", 0, 0, 0);
	//ProbeID = FPInitialize("SensorCHROMATIC.txt", 0, 0, 0);
	//ProbeID = FPInitialize("Config_DBL_CHROMATIC_1SN.txt", 0, 0, 0);

#else
	
	//////////////////////////////////////////////////////////
	// Example of use of FPInitialize
	//////////////////////////////////////////////////////////
	// Initialization of Probe with default configuration file
	// Default configuration file : Probe_Configuration_File.txt
	// the function return the Probe ID, which has been initialized with the configuration file
	//////////////////////////////////////////////////////////
	// Function return 0 if the initialization fail and the number (Probe ID) of identifiant if the initialization success
	
	//ProbeID = FPInitialize(0, 0, 0, 0);
	//ProbeID = FPInitialize("Probe_Configuration_File.txt", 0, 0, 0);
	//ProbeID = FPInitialize("Config_LDL_7DA_003.txt", 0, 0, 0);
	//ProbeID = FPInitialize("DoubleLISEED.txt", 0, 0, 0);
	//ProbeID = FPInitialize("SensorLISEED.txt", 0, 0, 0);
	//ProbeID = FPInitialize("Config_DBL_CHROMATIC_1SN.txt", 0, 0, 0);
	ProbeID = FPInitialize("SensorSPIRO.txt", 0, 0, 0);

#endif


	char cTypeTemp[1024];
	char cTypeSN[1024];
	double dRange = 0.0;
	int iFrequency = 0;
	double dGainMin = 0.0;
	double dGainMax = 0.0;
	double dGainStep = 0.0;
	FPGetSystemCaps(ProbeID,cTypeTemp,cTypeSN,&dRange,&iFrequency,&dGainMin,&dGainMax,&dGainStep);

	// Temporary string to store and display on the MFC interface the number of probe ID
	char cTemp[1024];
	// write in the string the number of probe ID
	sprintf_s(cTemp,"ID:%i",ProbeID);
	// display on the user interface the number of Probe ID
	CFogaleProbeAppDlg::SetDlgItemText(IDC_RETURN_FUNCTION,(LPCTSTR)cTemp);

	/*
	OnStart();
	OnDefinesample();
	OnGetthickness();
	*/
}

// Macro definition to write or read string in the different label of the user interface
#define RdTx(v,RID) {char MT[32];memset(MT,0,32);CFogaleProbeAppDlg::GetDlgItemText(RID,(LPTSTR)MT,31);v=atof(MT);}
#define WrTx(c,RID) {CFogaleProbeAppDlg::SetDlgItemText(RID,(LPTSTR)c);}
#define WrTxVal(d,s,RID) {char MT[32];sprintf_s(MT,s,d);CFogaleProbeAppDlg::SetDlgItemText(RID,(LPTSTR)MT);}

int CFogaleProbeAppDlg::StuffedTxbox(int RID) 
{
	char MT[32];memset(MT,0,32);CFogaleProbeAppDlg::GetDlgItemText(RID,(LPTSTR)MT,31);
	return strlen(MT);
}

void CFogaleProbeAppDlg::OnDefinesample() 
{
	char* Name = "Type_0";
	char* SampleNumber = "Lot_0";


	if(!StuffedTxbox(IDC_EDT_SOURCEVALUE))
	{
		WrTx("2.0" ,IDC_EDT_SOURCEVALUE);
	}

	if(!StuffedTxbox(IDC_EDT_THICKNESS1))
	{
		/*
		//simu
		WrTx("5000" ,IDC_EDT_THICKNESS1); WrTx("1000",IDC_EDT_TOLERANCE1); WrTx("1.519785",IDC_EDT_INDEX1);
		WrTx("10000",IDC_EDT_THICKNESS2); WrTx("2000",IDC_EDT_TOLERANCE2); WrTx("1.0",IDC_EDT_INDEX2);
		WrTx("1000" ,IDC_EDT_THICKNESS3); WrTx("500" ,IDC_EDT_TOLERANCE3); WrTx("1.519785",IDC_EDT_INDEX3);
		WrTx("4000" ,IDC_EDT_THICKNESS4); WrTx("1000",IDC_EDT_TOLERANCE4); WrTx("1.519785",IDC_EDT_INDEX4);
		*/
		/*
		//NOSAMPLE
		WrTx("50000" ,IDC_EDT_THICKNESS1); WrTx("50000",IDC_EDT_TOLERANCE1); WrTx("1.0",IDC_EDT_INDEX1);
		WrTx("50000",IDC_EDT_THICKNESS2); WrTx("50000",IDC_EDT_TOLERANCE2); WrTx("1.0",IDC_EDT_INDEX2);
		WrTx("50000" ,IDC_EDT_THICKNESS3); WrTx("50000" ,IDC_EDT_TOLERANCE3); WrTx("1.0",IDC_EDT_INDEX3);
		WrTx("50000" ,IDC_EDT_THICKNESS4); WrTx("50000",IDC_EDT_TOLERANCE4); WrTx("1.0",IDC_EDT_INDEX4);
		*/
		/*
		//Etalon FG25
		WrTx("5000" ,IDC_EDT_THICKNESS1); WrTx("500",IDC_EDT_TOLERANCE1); WrTx("1.519785",IDC_EDT_INDEX1);
		WrTx("25000",IDC_EDT_THICKNESS2); WrTx("1000",IDC_EDT_TOLERANCE2); WrTx("1.0",IDC_EDT_INDEX2);
		WrTx("6000" ,IDC_EDT_THICKNESS3); WrTx("500" ,IDC_EDT_TOLERANCE3); WrTx("1.519785",IDC_EDT_INDEX3);
		*/
		/*
		//Accelero Thales Capot/Airgap/Membrane
		WrTx("430" ,IDC_EDT_THICKNESS1); WrTx("10",IDC_EDT_TOLERANCE1); WrTx("3.68",IDC_EDT_INDEX1);
		WrTx("22.5",IDC_EDT_THICKNESS2); WrTx("6",IDC_EDT_TOLERANCE2); WrTx("1.0",IDC_EDT_INDEX2);
		WrTx("60" ,IDC_EDT_THICKNESS3); WrTx("20" ,IDC_EDT_TOLERANCE3); WrTx("3.68",IDC_EDT_INDEX3);
		//WrTx("3" ,IDC_EDT_THICKNESS4); WrTx("3" ,IDC_EDT_TOLERANCE4); WrTx("1.0",IDC_EDT_INDEX4);
		//WrTx("400" ,IDC_EDT_THICKNESS5); WrTx("100" ,IDC_EDT_TOLERANCE5); WrTx("3.68",IDC_EDT_INDEX5);
		*/
		/*	
		//Etalon Zerodur 20
		WrTx("159000" ,IDC_EDT_THICKNESS1); WrTx("10000",IDC_EDT_TOLERANCE1); WrTx("1.00027",IDC_EDT_INDEX1);
		WrTx("5000",IDC_EDT_THICKNESS2); WrTx("1000",IDC_EDT_TOLERANCE2); WrTx("1.54",IDC_EDT_INDEX2);
		WrTx("20000" ,IDC_EDT_THICKNESS3); WrTx("2000" ,IDC_EDT_TOLERANCE3); WrTx("1.00027",IDC_EDT_INDEX3);
		WrTx("5000" ,IDC_EDT_THICKNESS4); WrTx("1000" ,IDC_EDT_TOLERANCE4); WrTx("1.54",IDC_EDT_INDEX4);
		*/
		//Double ed test
		/*
		WrTx("3000" ,IDC_EDT_THICKNESS1); WrTx("3000",IDC_EDT_TOLERANCE1); WrTx("1.0",IDC_EDT_INDEX1);
		WrTx("306",IDC_EDT_THICKNESS2); WrTx("10",IDC_EDT_TOLERANCE2); WrTx("3.68",IDC_EDT_INDEX2);
		WrTx("1" ,IDC_EDT_THICKNESS3); WrTx("1" ,IDC_EDT_TOLERANCE3); WrTx("0",IDC_EDT_INDEX3);
		WrTx("980" ,IDC_EDT_THICKNESS4); WrTx("100" ,IDC_EDT_TOLERANCE4); WrTx("1.5",IDC_EDT_INDEX4);
		WrTx("3000" ,IDC_EDT_THICKNESS5); WrTx("3000" ,IDC_EDT_TOLERANCE5); WrTx("1.0",IDC_EDT_INDEX5);
		WrTx("2.5" ,IDC_EDT_SOURCEVALUE);
		*/
		
		//Lame de verre etalon
		WrTx("40000" ,IDC_EDT_THICKNESS1); WrTx("30000",IDC_EDT_TOLERANCE1); WrTx("1.00026",IDC_EDT_INDEX1);
		WrTx("1000" ,IDC_EDT_THICKNESS2); WrTx("400",IDC_EDT_TOLERANCE2); WrTx("1.519785",IDC_EDT_INDEX2);
		
		/*
		//Objectif Edmund 54855 au LI200 logo edmund en bas
		WrTx("3200" ,IDC_EDT_THICKNESS1); WrTx("300",IDC_EDT_TOLERANCE1); WrTx("1.519785",IDC_EDT_INDEX1);
		WrTx("18800",IDC_EDT_THICKNESS2); WrTx("300",IDC_EDT_TOLERANCE2); WrTx("1.0",IDC_EDT_INDEX2);
		WrTx("2300" ,IDC_EDT_THICKNESS3); WrTx("300",IDC_EDT_TOLERANCE3); WrTx("1.519785",IDC_EDT_INDEX3);
		*/
		/*		
		//Objectif Edmund 54855 logo Edmund en haut
		WrTx("6000" ,IDC_EDT_THICKNESS1); WrTx("2000",IDC_EDT_TOLERANCE1); WrTx("1.0027",IDC_EDT_INDEX1);
		WrTx("3800",IDC_EDT_THICKNESS2); WrTx("100",IDC_EDT_TOLERANCE2); WrTx("1.0",IDC_EDT_INDEX2);
		WrTx("1100" ,IDC_EDT_THICKNESS3); WrTx("100",IDC_EDT_TOLERANCE3); WrTx("1.519785",IDC_EDT_INDEX3);
		WrTx("2140" ,IDC_EDT_THICKNESS4); WrTx("100",IDC_EDT_TOLERANCE4); WrTx("1.519785",IDC_EDT_INDEX4);
		*/
		/*
		//Objectif Edmund 54855 logo Edmund en bas au gros collimateur
		WrTx("600" ,IDC_EDT_THICKNESS4); WrTx("100",IDC_EDT_TOLERANCE4); WrTx("1.519785",IDC_EDT_INDEX4);
		WrTx("8800",IDC_EDT_THICKNESS3); WrTx("100",IDC_EDT_TOLERANCE3); WrTx("1.0",IDC_EDT_INDEX3);
		WrTx("1100" ,IDC_EDT_THICKNESS2); WrTx("100",IDC_EDT_TOLERANCE2); WrTx("1.519785",IDC_EDT_INDEX2);
		WrTx("2140" ,IDC_EDT_THICKNESS1); WrTx("100",IDC_EDT_TOLERANCE1); WrTx("1.519785",IDC_EDT_INDEX1);
		*/
		/*
		//Wafer Ingrid vias
		WrTx("478" ,IDC_EDT_THICKNESS1); WrTx("50",IDC_EDT_TOLERANCE1); WrTx("1",IDC_EDT_INDEX1);
		WrTx("80",IDC_EDT_THICKNESS2); WrTx("40",IDC_EDT_TOLERANCE2); WrTx("1",IDC_EDT_INDEX2);
		WrTx("700" ,IDC_EDT_THICKNESS3); WrTx("300",IDC_EDT_TOLERANCE3); WrTx("3.6",IDC_EDT_INDEX3);
		//il faut saturer le premier pic (5V) et focaliser plus bas que la surface avec le gros collimateur
		*/
		/*
		//Wafer Ingrid Vias sur TMAP
		WrTx("48",IDC_EDT_THICKNESS2); WrTx("10",IDC_EDT_TOLERANCE2); WrTx("1",IDC_EDT_INDEX2);
		WrTx("747" ,IDC_EDT_THICKNESS3); WrTx("10",IDC_EDT_TOLERANCE3); WrTx("3.68",IDC_EDT_INDEX3);
		*/

		/*
		//Cale en Sitall (mesurée au double décimètre 50.0037mm, indice estimé par transparence 1.54033)
		WrTx("50000" ,IDC_EDT_THICKNESS1); WrTx("2000",IDC_EDT_TOLERANCE1); WrTx("1.5403",IDC_EDT_INDEX1);
		*/

		/*
		//Lentille demo labo optique N-BK7/N-SF5 11.10201 0.01733 6.0209
		WrTx("11100" ,IDC_EDT_THICKNESS1); WrTx("500",IDC_EDT_TOLERANCE1); WrTx("1.5197846",IDC_EDT_INDEX1);
		WrTx("20",IDC_EDT_THICKNESS2); WrTx("10",IDC_EDT_TOLERANCE2); WrTx("1.5",IDC_EDT_INDEX2);
		WrTx("6000" ,IDC_EDT_THICKNESS3); WrTx("500",IDC_EDT_TOLERANCE3); WrTx("1.6686506",IDC_EDT_INDEX3);
		WrTx("1.0" ,IDC_EDT_SOURCEVALUE);
		*/

		
		//Lame de verre in mode distance
		//WrTx("500" ,IDC_EDT_THICKNESS1); WrTx("100",IDC_EDT_TOLERANCE1); WrTx("1.00027",IDC_EDT_INDEX1);
		//WrTx("2000",IDC_EDT_THICKNESS2); WrTx("10",IDC_EDT_TOLERANCE2); WrTx("1.5197846",IDC_EDT_INDEX2);

	}

	// Définition de l'échantillon
	double Th[32];double Tol[32];double Index[32];double Type[32];

	int nt = 0; 
	if(StuffedTxbox(IDC_EDT_THICKNESS1)) {RdTx(Th[nt],IDC_EDT_THICKNESS1); RdTx(Tol[nt],IDC_EDT_TOLERANCE1); RdTx(Index[nt],IDC_EDT_INDEX1); Type[nt]=0;nt++;}
	if(StuffedTxbox(IDC_EDT_THICKNESS2)) {RdTx(Th[nt],IDC_EDT_THICKNESS2); RdTx(Tol[nt],IDC_EDT_TOLERANCE2); RdTx(Index[nt],IDC_EDT_INDEX2); Type[nt]=0;nt++;}
	if(StuffedTxbox(IDC_EDT_THICKNESS3)) {RdTx(Th[nt],IDC_EDT_THICKNESS3); RdTx(Tol[nt],IDC_EDT_TOLERANCE3); RdTx(Index[nt],IDC_EDT_INDEX3); Type[nt]=0;nt++;}
	if(StuffedTxbox(IDC_EDT_THICKNESS4)) {RdTx(Th[nt],IDC_EDT_THICKNESS4); RdTx(Tol[nt],IDC_EDT_TOLERANCE4); RdTx(Index[nt],IDC_EDT_INDEX4); Type[nt]=0;nt++;}
	if(StuffedTxbox(IDC_EDT_THICKNESS5)) {RdTx(Th[nt],IDC_EDT_THICKNESS5); RdTx(Tol[nt],IDC_EDT_TOLERANCE5); RdTx(Index[nt],IDC_EDT_INDEX5); Type[nt]=0;nt++;}

	double SourcePower; RdTx(SourcePower,IDC_EDT_SOURCEVALUE);


	int retvalue = FPDefineSample(ProbeID, Name, SampleNumber, Th, Tol, Index, Type, nt, SourcePower, 0.2);
	return;
}

// Function call when user click on "Start" button
void CFogaleProbeAppDlg::OnStart() 
{
	//////////////////////////////////////////////////////////
	// Example of use of FPStartSingleShotAcq
	//////////////////////////////////////////////////////////
	// fonction start the probe associate to the Probe ID in single shot acquisition
	//////////////////////////////////////////////////////////
	// Function return FP_OK if the start of probe success and FP_FAIL if the start fail

	char Type[256];
	double Range;
	char SerialNumber[256];
	int Frequency;
	double VMin;
	double VMax;
	double VStep;
	int r = FPGetSystemCaps(ProbeID,Type,SerialNumber,&Range,&Frequency,&VMin,&VMax,&VStep);

	char Type2[256];
	r=FPGetParam(ProbeID,Type2,FPID_C_TYPE);

	char SerialNumber2[256];
	r=FPGetParam(ProbeID,SerialNumber2,FPID_C_SERIAL);

	double Range2;
	r=FPGetParam(ProbeID,&Range2,FPID_D_RANGE);

	int RetValue = FPStartSingleShotAcq(ProbeID);

	// test on RetValue if the function success or fail
	if(RetValue != FP_OK)
	{
		// call function DisplayError to display a message box with the name of function which return an error code 
		DisplayError("FPStartSingleShotAcq");
	}
}

// Function call when user click on "GetThickness" button
void CFogaleProbeAppDlg::OnGetthickness() 
{
	// definition of Number of thickness
	int NbThickness = 8;
	// definition of the array of thickness to get the value measured
	double Thickness[8], Quality[8];
	// loop for the initialization of the array, all thickness equal to zero
	for (int i=0; i<NbThickness; i++)
	{
		Thickness[i] = 0.0;
		Quality[i] = 0.0;
	}

	//////////////////////////////////////////////////////////
	// Example of use of FPGetThickness
	//////////////////////////////////////////////////////////
	// Use the probe ID to have the value measured by the probe
	// The function return the array of thickness measure and the quality associate
	// 
	//////////////////////////////////////////////////////////
	// Function return FP_OK if the function sucsess and FP_FAIL if the function fail
	// Check the Quality value to have an indicator of confidence, if the value equal to zero, the measurement is not good

	int RetValue = FPGetThickness(ProbeID, Thickness, Quality,NbThickness);

	//double ArrayAmplitude[8];
	//memset(ArrayAmplitude,0,8);

	// get param pour tester la fonction d'amplitude peaks
	//FPGetParam(ProbeID,ArrayAmplitude,FPID_D_AMPLITUDEPEAKS);

	// test on RetValue if the function success 
	if(RetValue != FP_OK)
	{
		// call function DisplayError to display a message box with the name of function which return an error code 
		DisplayError("FPGetThickness");
	}

	// command to write in the user interface the value measured of quality, and thickness
	WrTxVal(Thickness[0],"%.2f",IDC_THICKNESS1);
	WrTxVal(Thickness[1],"%.2f",IDC_THICKNESS2);
	WrTxVal(Thickness[2],"%.2f",IDC_THICKNESS3);
	WrTxVal(Thickness[3],"%.2f",IDC_THICKNESS4);
	WrTxVal(Thickness[4],"%.2f",IDC_THICKNESS5);
	WrTxVal(Quality[0],"%.2f",IDC_QUAL1);
	WrTxVal(Quality[1],"%.2f",IDC_QUAL2);
	WrTxVal(Quality[2],"%.2f",IDC_QUAL3);
	WrTxVal(Quality[3],"%.2f",IDC_QUAL4);
	WrTxVal(Quality[4],"%.2f",IDC_QUAL5);


	/*
	int retvalue; //return value for FPGet/FPSet is FP_OK, FP_UNAVAILABLE, FP_FAIL
	double rawnoisebase;
	retvalue = FPGetParam(ProbeID,&rawnoisebase,FPID_D_RAWNOISEBASE);
	rawnoisebase*=2;
	retvalue = FPSetParam(ProbeID,&rawnoisebase,FPID_D_RAWNOISEBASE);

	double scannerlength;
	retvalue = FPGetParam(ProbeID,&scannerlength,FPID_D_SCANNERLENGTH_UM);
	scannerlength/=2;
	retvalue = FPSetParam(ProbeID,&scannerlength,FPID_D_SCANNERLENGTH_UM);
	*/

	// definition of string log to get the log information from the dll
	char Log[4096];

	//////////////////////////////////////////////////////////
	// Example of use of FPGetParam with FPID_C_LOG
	//////////////////////////////////////////////////////////
	// the function return in variable Log all the information about internal National Instrument log 
	// 
	//////////////////////////////////////////////////////////
	// Function return FP_OK if you get the param and FP_FAIL if the function don't return the param

	int retvalue = FPGetParam(ProbeID,Log,FPID_C_LOG);
	//MessageBox(Log,"OnGetthickness",MB_OK);
}


void CFogaleProbeAppDlg::OnDoPowerTest() 
{
	void* nonzero=(void*)(-1);
	FPSetParam(ProbeID,nonzero,FPID_I_AUTOTESTPOWER);
}

void CFogaleProbeAppDlg::OnDoXYTest() 
{
	void* nonzero=(void*)(-1);
	//FPSetParam(ProbeID,nonzero,FPID_I_AUTOTESTXY);
	FPSetParam(ProbeID,nonzero,FPID_I_AUTOTESTSTAGE);
}

void CFogaleProbeAppDlg::OnDoFocusTest() 
{
	void* nonzero=(void*)(-1);
	FPSetParam(ProbeID,nonzero,FPID_I_AUTOTESTFOCUS);
}

void CFogaleProbeAppDlg::OnDoMatTest() 
{

	//Here is an example to test and access to the material list

	// declaration of DWORD to estimate the time for reading the material list
	DWORD T=GetTickCount();

	// declaration of different 
	int NumMat; char Name[FPID_STRLEN]; double ng=-1;


	//////////////////////////////////////////////////////////
	// Example of use of FPGetParam with FPID_I_GETNUMMATERIALS
	//////////////////////////////////////////////////////////
	// the function return in variable NumMat the number of material
	// the third argument associate to have this information is FPID_I_GETNUMMATERIALS 
	//////////////////////////////////////////////////////////
	// Function return FP_OK if you get the param and FP_FAIL if the function don't return the param
	
	int RetValue = FPGetParam(ProbeID,&NumMat,FPID_I_GETNUMMATERIALS);

	if(RetValue==FP_OK)
	{
		for(int MatNr=0;MatNr<NumMat;MatNr++)
		{
			//////////////////////////////////////////////////////////
			// Example of use of FPGetParam with FPID_I_GETNUMMATERIALS
			//////////////////////////////////////////////////////////
			// the function return the material name with the second argument
			// the third argument associate to have this information is FPID_C_GETMATERIALNAME + the number of the material
			// here you must add the number of the material you want to have the name
			//
			//////////////////////////////////////////////////////////
			// Function return FP_OK if the start of probe success and FP_FAIL if the stop fail

			RetValue = FPGetParam(ProbeID,Name,FPID_C_GETMATERIALNAME+MatNr);
			
			
			//////////////////////////////////////////////////////////
			// Example of use of FPGetParam with FPID_D_GETMATERIALINDEX
			//////////////////////////////////////////////////////////
			// the function return the index with the second argument
			// the third argument associate to have this information is FPID_D_GETMATERIALINDEX + the number of the material
			// here you must add the number of the material you want to have the index
			//
			//////////////////////////////////////////////////////////
			// Function return FP_OK if the start of probe success and FP_FAIL if the stop fail

			RetValue = FPGetParam(ProbeID,&ng,FPID_D_GETMATERIALINDEX+MatNr);
		}
		
		char Msg[256];
		sprintf_s(Msg,"%i materials\nLast material: %s ng=%.7f\nTemps total:%ims",NumMat,Name,ng,GetTickCount()-T);
		MessageBox(Msg,"DoMatTest",0);
	}

	/*
	void* nonzero=(void*)(-1);
	FPSetParam(ProbeID,nonzero,FPID_I_AUTOTESTMAT);
	*/


/*	
	int NumMat;
	int RetValue = FPGetParam(ProbeID,&NumMat,FPID_I_GETNUMMATERIALS);

	int MatNr;
	for(MatNr=0;MatNr<NumMat;MatNr++)
	{
		char Name[FPID_STRLEN];
		RetValue = FPGetParam(ProbeID,Name,FPID_C_GETMATERIALNAME+MatNr);
		if(stricmp(Name,"sicristalline")==0) break;
	}
	if(MatNr!=NumMat)
	{
		FILE* F=fopen("siv.txt","wb+");
		for(float i=0;i<10;i+=0.1f)
		{
			FPDefineSample(ProbeID,"Sample Wafer","void",0,0,0,0,0,i,0);
			double ng;
			RetValue = FPGetParam(ProbeID,&ng,FPID_D_GETMATERIALINDEX+MatNr);
			fprintf(F,"%.1f\t%.6f\r\n",i,ng);
		}
		fclose(F);
	}
	
*/	

	/*
	int NumMat;
	int RetValue = FPGetParam(ProbeID,&NumMat,FPID_I_GETNUMMATERIALS);

	int MatNr;
	for(MatNr=0;MatNr<NumMat;MatNr++)
	{
		char Name[FPID_STRLEN];
		RetValue = FPGetParam(ProbeID,Name,FPID_C_GETMATERIALNAME+MatNr);
		if(stricmp(Name,"air")==0) break;
	}
	if(MatNr!=NumMat)
	{
		int PMIN=101000; int PMAX=102500; int PSTEP=20;
		int TMIN=18; int TMAX=38; int TSTEP=1;
		FILE* F=fopen("air.txt","wb+");

		//ligne d'entete
		fprintf(F,"P\\T\t\t");
		for(double t=TMIN;t<TMAX;t+=TSTEP)
		{
			fprintf(F,"%.1f\t",t);
		}
		fprintf(F,"\r\n");

		for(double p=PMIN;p<PMAX;p+=PSTEP)
		{
			fprintf(F,"%.1f\t\t",p);
		for(double t=TMIN;t<TMAX;t+=TSTEP)
		{
			//FPDefineSample(ProbeID,"Sample Wafer","void",0,0,0,0,0,i,0);
			FPSetParam(ProbeID,&p,FPID_D_PRESSUREEXT);
			FPSetParam(ProbeID,&t,FPID_D_TEMPEXT);
			double ng;
			RetValue = FPGetParam(ProbeID,&ng,FPID_D_GETMATERIALINDEX+MatNr);
			fprintf(F,"%.7f\t",ng);
		}
		fprintf(F,"\r\n");
		}
		fclose(F);
	}

	*/
/*
	double WorkingDistance;
	FPGetParam(ProbeID,&WorkingDistance,FPID_D_WORKINGDISTANCE);
	double Focus=150000;
	FPSetParam(ProbeID,&Focus,FPID_D_CT_SETFOCUS);
*/
	return;
}

void CFogaleProbeAppDlg::OnDoCalTest() 
{
	FPAGainLoopZ20(ProbeID);
}

void CFogaleProbeAppDlg::OnStop() 
{
	//////////////////////////////////////////////////////////
	// Example of use of FPStopSingleShotAcq
	//////////////////////////////////////////////////////////
	// fonction stop the probe associate to the Probe ID in single shot acquisition
	//////////////////////////////////////////////////////////
	// Function return FP_OK if the start of probe success and FP_FAIL if the stop fail

	int RetValue = FPStopSingleShotAcq(ProbeID);

	// test on RetValue if the function success or fail
	if(RetValue != FP_OK)
	{
		// call function DisplayError to display a message box with the name of function which return an error code 
		DisplayError("FPStopSingleShotAcq");
	}

}

void CFogaleProbeAppDlg::OnCloseprobe() 
{
	//////////////////////////////////////////////////////////
	// Example of use of FPClose
	//////////////////////////////////////////////////////////
	// this function close a probe pointed by Probe ID
	// After the execution of this function, all the function call with the probe ID closed will return FP_FAIL 
	//////////////////////////////////////////////////////////
	// Function return FP_OK if the close of probe success and FP_FAIL if the close fail

	int RetValue  = FPClose(ProbeID);
	
	// test on RetValue if the function success or fail
	if(RetValue != FP_OK)
	{
		// call function DisplayError to display a message box with the name of function which return an error code 
		DisplayError("FPClose");
	}
}

BOOL CFogaleProbeAppDlg::DestroyWindow() 
{
	// TODO: Add your specialized code here and/or call the base class

	//////////////////////////////////////////////////////////
	// Example of use of FPDLLClose
	//////////////////////////////////////////////////////////
	// You need just to call the function DLLClose when you finish with the dll
	//
	//////////////////////////////////////////////////////////
	// 

	FPDLLClose();
	int r=CDialog::DestroyWindow();
	OutputDebugString("DEBUG CHECK START AT CFogaleProbeAppDlg::DestroyWindow\n");
	_CrtDumpMemoryLeaks();
	OutputDebugString("DEBUG CHECK STOP AT CFogaleProbeAppDlg::DestroyWindow\n");
	return r;
}

void CFogaleProbeAppDlg::OnDoSettings() 
{
	//////////////////////////////////////////////////////////
	// Example of use of FPDoSettings
	//////////////////////////////////////////////////////////
	// this function open a setting window where you can adjust :
	//	- Coupled power for LenScan system
	//	- Gain value for LISE ED system
	//  - Gain value and threshold for CHROMATIC
	// 
	//////////////////////////////////////////////////////////
	// Function return FP_OK if the setting window display FP_FAIL if the setting window don't open

	int RetValue = FPDoSettings(ProbeID);

	// test on RetValue if the function success or fail
	if(RetValue != FP_OK)
	{
		// call function DisplayError to display a message box with the name of function which return an error code 
		DisplayError("FPDoSettings");
	}
}

void CFogaleProbeAppDlg::OnOpenSettingWindow() 
{
	// TODO: Add your control notification handler code here.
	
	//////////////////////////////////////////////////////////
	// Example of use of FPOpenSettingsWindow
	//////////////////////////////////////////////////////////
	// this function open a setting window for the probe associate to the probe ID
	// this function just open the window and not refresh the graph
	// you must refresh the window by using the function FPUpdateSettingsWindow
	// 
	//////////////////////////////////////////////////////////
	// Function return FP_OK if the window open and FP_FAIL if the window don't open
	
	int RetValue = FPOpenSettingsWindow(ProbeID);
	bWindowOpened=true;

	// test on RetValue if the function success or fail
	if(RetValue != FP_OK)
	{
		// call function DisplayError to display a message box with the name of function which return an error code 
		DisplayError("FPOpenSettingsWindow");
	}
}

void CFogaleProbeAppDlg::OnCloseSettingWindow() 
{
	// TODO: Add your control notification handler code here
	
	//////////////////////////////////////////////////////////
	// Example of use of FPCloseSettingsWindow
	//////////////////////////////////////////////////////////
	// this function close a setting window for the probe associate to the probe ID
	// this function just close the window and not refresh the graph
	// you must refresh the window by using the function FPCloseSettingsWindow
	// 
	//////////////////////////////////////////////////////////
	// Function return FP_OK if the window close and FP_FAIL if the window don't close

	int RetValue = FPCloseSettingsWindow(ProbeID);
	bWindowOpened = false;

	// test on RetValue if the function success or fail
	if(RetValue != FP_OK)
	{
		// call function DisplayError to display a message box with the name of function which return an error code 
		DisplayError("FPCloseSettingsWindow");
	}
	
}

void CFogaleProbeAppDlg::OnLabelversion() 
{
	// TODO: Add your control notification handler code here
	m_labelVersion = "1";
}

void CFogaleProbeAppDlg::OnUpdateSettingWindow() 
{
	// TODO: Add your control notification handler code here
	if(bWindowOpened == true)
	{
		//////////////////////////////////////////////////////////
		// Example of use of FPUpdateSettingsWindow
		//////////////////////////////////////////////////////////
		// this function update a setting window for the probe associate to the probe ID
		// this function just refresh the graph in the window, by displaying the signal corresponding to the ProbeID
		// suppose that the window is already open by using function FPOpenSettingsWindow
		// 
		//////////////////////////////////////////////////////////
		// Function return FP_OK if the window close and FP_FAIL if the window don't close

		FPUpdateSettingsWindow(ProbeID);
	}
	else
	{
		int iNbSamples = 40000;
		double* dI = (double*)malloc(iNbSamples*sizeof(double));
		float StepX = 0.0;
		float fSatur = 0.0;
		// ajout des variables selected peaj pour debug
		double SelectedPeak[8];
		double DiscartedPeak[8];
		int NbPeak = 0;
		int NbDiscPeak = 0;
		FPGetRawSignal(ProbeID,"SiTuTrouvesLePassTuEsFort",dI,&iNbSamples,&StepX,1,&fSatur,SelectedPeak,&NbPeak,DiscartedPeak,&NbDiscPeak);
		free(dI);
	}
}

void CFogaleProbeAppDlg::OnGetRawSignal()
{
	// Not available for OEM version

	//double Buffer[100000];
	int NumSample = 0;
	float StepX = 0.0;
	float fSaturationThreshold = 0.0;
	double SelectedPeak[8];
	double DiscartedPeak[8];
	int NbPeak = 0;
	int NbDiscPeak = 0;
	FPGetRawSignal(ProbeID,"SiTuTrouvesLePassTuEsFort",0,&NumSample,&StepX,1,&fSaturationThreshold,0,&NbPeak,0,&NbDiscPeak);
	double* Buffer=(double*)malloc(NumSample*sizeof(double));
	char Msg[256];
	sprintf_s(Msg,"NumSample:%i NumPeaks:%i NumDiscarded:%i",NumSample,NbPeak,NbDiscPeak);
	MessageBox(Msg,"FogaleProbe:GetRawSignal",MB_OK);
	FPGetRawSignal(ProbeID,"SiTuTrouvesLePassTuEsFort",Buffer,&NumSample,&StepX,1,&fSaturationThreshold,SelectedPeak,&NbPeak,DiscartedPeak,&NbDiscPeak);
	//"SiTuTrouvesLePassTuEsFort"
	free(Buffer);
}


/////////////////////////////////////////////////////////////////////////////
// CFogaleProbeAppDlg message handlers

BOOL CFogaleProbeAppDlg::OnInitDialog()
{
	CDialog::OnInitDialog();

	// Add "About..." menu item to system menu.

	// IDM_ABOUTBOX must be in the system command range.
	ASSERT((IDM_ABOUTBOX & 0xFFF0) == IDM_ABOUTBOX);
	ASSERT(IDM_ABOUTBOX < 0xF000);

	CMenu* pSysMenu = GetSystemMenu(FALSE);
	if (pSysMenu != NULL)
	{
		CString strAboutMenu;
		strAboutMenu.LoadString(IDS_ABOUTBOX);
		if (!strAboutMenu.IsEmpty())
		{
			pSysMenu->AppendMenu(MF_SEPARATOR);
			pSysMenu->AppendMenu(MF_STRING, IDM_ABOUTBOX, strAboutMenu);
		}
	}

	// Set the icon for this dialog.  The framework does this automatically
	//  when the application's main window is not a dialog
	SetIcon(m_hIcon, TRUE);			// Set big icon
	SetIcon(m_hIcon, FALSE);		// Set small icon
	
	// TODO: Add extra initialization here

	//////////////////////////////////////////////////////////
	// Example of use of FPDLLClose
	//////////////////////////////////////////////////////////
	// You need just to init the dll and access to the different function
	//
	//////////////////////////////////////////////////////////
	// 

	FPDLLInit();
	bWindowOpened = false;
	return TRUE;  // return TRUE  unless you set the focus to a control

}

void CFogaleProbeAppDlg::OnSysCommand(UINT nID, LPARAM lParam)
{
	if ((nID & 0xFFF0) == IDM_ABOUTBOX)
	{
		CAboutDlg dlgAbout;
		dlgAbout.DoModal();
	}
	else
	{
		CDialog::OnSysCommand(nID, lParam);
	}
}

// If you add a minimize button to your dialog, you will need the code below
//  to draw the icon.  For MFC applications using the document/view model,
//  this is automatically done for you by the framework.

void CFogaleProbeAppDlg::OnPaint() 
{
	if (IsIconic())
	{
		CPaintDC dc(this); // device context for painting

		SendMessage(WM_ICONERASEBKGND, (WPARAM) dc.GetSafeHdc(), 0);

		// Center icon in client rectangle
		int cxIcon = GetSystemMetrics(SM_CXICON);
		int cyIcon = GetSystemMetrics(SM_CYICON);
		CRect rect;
		GetClientRect(&rect);
		int x = (rect.Width() - cxIcon + 1) / 2;
		int y = (rect.Height() - cyIcon + 1) / 2;

		// Draw the icon
		dc.DrawIcon(x, y, m_hIcon);
	}
	else
	{
		CDialog::OnPaint();
	}
}

// The system calls this to obtain the cursor to display while the user drags
//  the minimized window.
HCURSOR CFogaleProbeAppDlg::OnQueryDragIcon()
{
	return (HCURSOR) m_hIcon;
}

void write11thelement( float* _f, int size)
{
    SC_CACHED_ARRAY( float, f, _f, size );
    f[11] = 2;
    return;
}

void checked_array_test()
{
    float f[10];
    write11thelement(f,10);
    return;
}

void CFogaleProbeAppDlg::OnGetversion() 
{

	//////////////////////////////////////////////////////////
	// Example of use of FPGetVersion
	//////////////////////////////////////////////////////////
	// this function return the version of the dll
	// 
	//////////////////////////////////////////////////////////
	// Function return only the version

	int iVersion = FPGetVersion();

	// declaration of a string	
	char cTemp[128];
	// write in the string the name associate
	sprintf_s(cTemp,"Version : %i",iVersion);
	// send to the user interface the information
	CFogaleProbeAppDlg::SetDlgItemText(IDC_LABELVERSION,(LPCTSTR)cTemp);

    checked_array_test();
}

void CFogaleProbeAppDlg::OnBnClickedProbe1()
{
	// TODO : ajoutez ici le code de votre gestionnaire de notification de contrôle
	int Probe1 = 0;
	void* nonzero=&Probe1;
	FPSetParam(ProbeID,nonzero,FPID_I_SETCURRENTPROBE);

	double toto[4]; memset(toto,0,4*sizeof(double));
	FPSetParam(ProbeID,toto,FPID_D_CALIBRATE_TOTAL_TH);
}

void CFogaleProbeAppDlg::OnBnClickedProbe2()
{
	// TODO : ajoutez ici le code de votre gestionnaire de notification de contrôle
	int Probe2 = 1;
	void* nonzero=&Probe2;
	FPSetParam(ProbeID,nonzero,FPID_I_SETCURRENTPROBE);
}
