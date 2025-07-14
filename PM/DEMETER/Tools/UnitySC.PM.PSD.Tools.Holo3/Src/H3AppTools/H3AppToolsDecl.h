// H3AppMessageToolsDecl.h: declarations
//
//////////////////////////////////////////////////////////////////////

#if !defined(H3APPTOOLSDECL_H__INCLUDED_)
#define H3APPTOOLSDECL_H__INCLUDED_

#ifdef _DLL
#  ifdef AFX_H3APPTOOLS_H__5BCF1970_3975_4317_A596_48774862FCFC__INCLUDED_
#    define H3APPTOOLS_EXP extern "C++" __declspec(dllexport)
#  else
#    define H3APPTOOLS_EXP extern "C++" __declspec(dllimport)
#  endif
#else
#  define H3APPTOOLS_EXP extern "C++"
#endif

#include <list>
#include <vector>
#include "H3Matrix.h"


using namespace std;

/////////////////////////////////////////////////////////////////////////////
// Debug et messages
#define H3_DEBUG_ERROR				0x0001
#define H3_DEBUG_WARNING			0x0002
#define H3_DEBUG_INFO				0x0004
#define H3_DEBUG_MESSAGE			0x0008
#define H3_DEBUG_DISABLE			0x0000
#define H3_DEBUG_ENABLE				0x000F

/// <summary>
/// Calib folder structure.
/// </summary>
struct CalibPaths
{
    /// <summary>
    /// The calibration procedure creates a new folder, that should be pre initialized with default compute settings, and fills it with calibration data.
    /// </summary>
    LPCSTR _LastCalibPath = _T("C:\\UnitySC\\Psd\\Temp\\TopoCalib\\Result");
    LPCSTR _InputSettingsFile = _T("SensorData.txt");
    LPCSTR _CheckMesureFile = _T("CheckMesure.txt");

    LPCSTR _CamCalibSubfolder = _T("Calib_cam");
    LPCSTR _CamCalibIntrinsicParamsFile = _T("CalibCam_0.txt");
    LPCSTR _CamCalibLogSubfolder = _T("log");

    LPCSTR _SysCalibSubfolder = _T("Calib_Sys");
    LPCSTR _MatrixScreenToCamFile = _T("Res1.txt");
    LPCSTR _MatrixWaferToCameraFile = _T("EP_ref_CamFrame.txt");
    LPCSTR _PhaseReferenceXFile = _T("ResX.klib");
    LPCSTR _PhaseReferenceYFile = _T("ResY.klib");

    LPCSTR _UWPhiSubfolder = _T("Calib_UWPhi");
    LPCSTR _UWMirrorMaskFile = _T("UWMask.hbf");

    CString PhaseReferenceYPath(const CString& calibFolder)
    {
        return calibFolder + "\\" + _SysCalibSubfolder + "\\" + _PhaseReferenceYFile;
    }
    CString PhaseReferenceXPath(const CString& calibFolder)
    {
        return calibFolder + "\\" + _SysCalibSubfolder + "\\" + _PhaseReferenceXFile;
    }
    CString MatrixWaferToCameraPath(const CString& calibFolder)
    {
        return calibFolder + "\\" + _SysCalibSubfolder + "\\" + _MatrixWaferToCameraFile;
    }
    CString MatrixScreenToCamPath(const CString& calibFolder)
    {
        return calibFolder + "\\" + _SysCalibSubfolder + "\\" + _MatrixScreenToCamFile;
    }
    CString CamCalibLogFolder(const CString& calibFolder)
    {
        return CamCalibFolder(calibFolder) + "\\" + _CamCalibLogSubfolder;
    }
    CString CamCalibIntrinsicParamsPath(const CString& calibFolder)
    {
        return CamCalibFolder(calibFolder) + "\\" + _CamCalibIntrinsicParamsFile;
    }
    CString CamCalibFolder(const CString& calibFolder)
    {
        return calibFolder + "\\" + _CamCalibSubfolder;
    }
    CString InputSettingsPath(const CString& calibFolder)
    {
        return calibFolder + "\\" + _InputSettingsFile;
    }
    CString UWMirrorMaskPath(const CString& calibFolder)
    {
        return calibFolder + "\\" + _UWPhiSubfolder + "\\" + _UWMirrorMaskFile;
    }
};
H3APPTOOLS_EXP CalibPaths _CalibPaths;
//#define DEFAULT_SAVE_GLOBALTOPO_PATH		_T("C:\\AltaSight\\GlobalTopo")

//#define DEFAULT_SAVE_CALIB_PATH				_T("C:\\AltaSight\\GlobalTopo\\Calib_cam\\CalibCam_0.txt")
//#define DEFAULT_SAVE_EPCAMFRAM_PATH			_T("C:\\AltaSight\\GlobalTopo\\Calib_cam\\EP_ref_CamFrame.txt")

H3APPTOOLS_EXP CString H3LoadString(UINT nID);
H3APPTOOLS_EXP bool H3EnableError(bool bEnable);
H3APPTOOLS_EXP bool H3IsErrorEnabled();
H3APPTOOLS_EXP bool H3EnableWarning(bool bEnable);
H3APPTOOLS_EXP bool H3IsWarningEnabled();
H3APPTOOLS_EXP bool H3EnableInfo(bool bEnable);
H3APPTOOLS_EXP bool H3IsInfoEnabled();

H3APPTOOLS_EXP long H3DisplayError(const CString& strModule, const CString& strFunction, const CString& strMessage, bool bDisplaySysError = false);
H3APPTOOLS_EXP long H3DisplayError(const CString& strMessage);

H3APPTOOLS_EXP long H3DisplayWarning(const CString& strModule, const CString& strFunction, const CString& strMessage);
H3APPTOOLS_EXP long H3DisplayWarning(const CString& strMessage);
H3APPTOOLS_EXP long H3DisplayInfo(const CString& strModule, const CString& strFunction, const CString& strMessage);
H3APPTOOLS_EXP long H3DisplayInfo(const CString& strMessage);
H3APPTOOLS_EXP long H3DisplayMessage(const CString& strModule, const CString& strFunction, const CString& strTitle, const CString& strMessage, UINT nType);
H3APPTOOLS_EXP long H3DisplayMessage(const CString& strTitle, const CString& strMessage, UINT nType);
H3APPTOOLS_EXP void H3DebugMessage(const CString& strModule, const CString& strFunction, const CString& strMessage);
H3APPTOOLS_EXP void H3DebugInfo(const CString& strModule, const CString& strFunction, const CString& strMessage);
H3APPTOOLS_EXP void H3DebugWarning(const CString& strModule, const CString& strFunction, const CString& strMessage);
H3APPTOOLS_EXP void H3DebugError(const CString& strModule, const CString& strFunction, const CString& strMessage);
H3APPTOOLS_EXP void H3SetDebugFile(const CString& strDebugFile);
H3APPTOOLS_EXP CString H3GetDebugFile();
H3APPTOOLS_EXP long H3SetDebugLevel(long nLevel);
H3APPTOOLS_EXP long H3GetDebugLevel();

H3APPTOOLS_EXP bool H3CreateDirectory(const CString& strPathName, LPSECURITY_ATTRIBUTES lpSecurityAttributes);
H3APPTOOLS_EXP bool H3ReformatPath(CString& strDest, const CString& strSrc);
H3APPTOOLS_EXP bool H3ValidPath(const CString& strPath);
H3APPTOOLS_EXP bool H3DirectoryExist(const CString& strPathName);
H3APPTOOLS_EXP bool H3FileExist(const CString& strFilename);
H3APPTOOLS_EXP bool H3FileList(list<CString>& strFileList, const CString& strSrcFilter, long nMode = 0);
H3APPTOOLS_EXP bool H3DeleteFile(const CString& strFileFilter);
H3APPTOOLS_EXP long H3FileLength(const CString& strFilename);
H3APPTOOLS_EXP void H3SplitPath(const CString& strPath, CString& strDrive, CString& strDir, CString& strName, CString& strExt);
H3APPTOOLS_EXP CString H3FileName(const CString& strFilename);
H3APPTOOLS_EXP CString H3FileExt(const CString& strFilename);
H3APPTOOLS_EXP CString H3FilePath(const CString& strFilename);

H3APPTOOLS_EXP bool H3GetDiskFreeSpaceEx(LPCTSTR lpDirectoryName, double& FreeBytesAvailableToCaller, double& TotalNumberOfBytes, double& TotalNumberOfFreeBytes);
H3APPTOOLS_EXP double H3GetSysDiskFreeSpace();

H3APPTOOLS_EXP CString H3ByteToCString(double Bytes);

H3APPTOOLS_EXP CString H3GetLongPathName(const CString& strShortPath);

H3APPTOOLS_EXP CString H3GetTempFilename(const CString& strPrefix);
H3APPTOOLS_EXP CString H3GetTempPath();

H3APPTOOLS_EXP bool H3fWriteCOleDateTime(FILE* Stream, COleDateTime& OleDateTime);
H3APPTOOLS_EXP COleDateTime H3fReadCOleDateTime(FILE* Stream);

H3APPTOOLS_EXP bool H3fWriteLong(FILE* Stream, long Value);
H3APPTOOLS_EXP long H3fReadLong(FILE* Stream);

H3APPTOOLS_EXP bool H3fWriteFloat(FILE* Stream, float Value);
H3APPTOOLS_EXP float H3fReadFloat(FILE* Stream);

H3APPTOOLS_EXP bool H3fWriteDouble(FILE* Stream, double Value);
H3APPTOOLS_EXP double H3fReadDouble(FILE* Stream);

H3APPTOOLS_EXP bool H3fWriteCString(FILE* Stream, const CString& str);
H3APPTOOLS_EXP CString H3fReadCString(FILE* Stream);

H3APPTOOLS_EXP bool H3fWriteCString2(FILE* Stream, const CString& str);
H3APPTOOLS_EXP CString H3fReadCString2(FILE* Stream);

//H3APPTOOLS_EXP float H3GetFPNaN();
//H3APPTOOLS_EXP float H3Round(float v);
//H3APPTOOLS_EXP double H3Round(double v);

H3APPTOOLS_EXP float H3Round(float fValue, unsigned long n = 0);
H3APPTOOLS_EXP double H3Round(double dValue, unsigned long n = 0);
H3APPTOOLS_EXP float H3Trunc(float fValue, unsigned long n = 0);
H3APPTOOLS_EXP double H3Trunc(double dValue, unsigned long n = 0);


//H3APPTOOLS_EXP void H3Tic();
//H3APPTOOLS_EXP DWORD H3Toc();
//H3APPTOOLS_EXP DWORD H3Toc(const CString &strMsg);

H3APPTOOLS_EXP CString H3Value2CString(const char* pszFormat1, const char* pszFormat2, double v);
H3APPTOOLS_EXP CString H3Value2CString(const char* pszFormat, double v);


#define H3WritePrivProfileInt		H3WritePrivProfile
#define H3WritePrivProfileFloat		H3WritePrivProfile
#define H3WritePrivProfileString	H3WritePrivProfile
#define H3WritePrivProfileDouble	H3WritePrivProfile

H3APPTOOLS_EXP vector <double> H3GetPrivProfileVectorDouble(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const vector <double>& values, const CString& strFilename);
H3APPTOOLS_EXP vector <H3_POINT2D_FLT64> H3GetPrivProfileVector2DF64(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const vector <H3_POINT2D_FLT64> pts, const CString& strFilename);
H3APPTOOLS_EXP H3_POINT2D_FLT64 H3GetPrivProfile2DF64(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const H3_POINT2D_FLT64& pt, const CString& strFilename);
H3APPTOOLS_EXP H3_RECT_FLT64 H3GetPrivProfileRectF64(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const H3_RECT_FLT64& rc, const CString& strFilename);
//H3APPTOOLS_EXP H3_MATRIX_FLT64 H3GetPrivProfileMatrixF64(const CString &strSection,const CString &strEntry,H3_MATRIX_FLT64 &matDefault,const CString &strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, H3_MATRIX_FLT64& mat, const CString& strFilename);
H3APPTOOLS_EXP H3_RECT_INT32 H3GetPrivProfileRectInt32(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const H3_RECT_INT32& rc, const CString& strFilename);
H3APPTOOLS_EXP H3_POINT3D_FLT64 H3GetPrivProfile3DF64(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const H3_POINT3D_FLT64& pt, const CString& strFilename);
H3APPTOOLS_EXP CString H3GetPrivProfileString(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const CString& strString, const CString& strFilename);
H3APPTOOLS_EXP float H3GetPrivProfileFloat(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const float& fValue, const CString& strFilename);
H3APPTOOLS_EXP double H3GetPrivProfileDouble(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const double& fValue, const CString& strFilename);
//H3APPTOOLS_EXP bool H3GetPrivProfile(const CString &strSection,const CString &strEntry,const bool &bDefault,const CString &strFilename);
//H3APPTOOLS_EXP bool H3WritePrivProfile(const CString &strSection,const CString &strEntry,const bool &bValue,const CString &strFilename);
H3APPTOOLS_EXP int H3GetPrivProfileInt(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const int& nValue, const CString& strFilename);
H3APPTOOLS_EXP long H3GetPrivProfileLong(const CString& strSection, const CString& strEntry, const CString& strFilename);
H3APPTOOLS_EXP bool H3WritePrivProfile(const CString& strSection, const CString& strEntry, const long& nValue, const CString& strFilename);

H3APPTOOLS_EXP CString H3GetCurrentTimeString();
H3APPTOOLS_EXP long H3AddFileNamesToCombo(CComboBox& DestCombo, const CString& strSrcFile);


H3APPTOOLS_EXP bool H3IsProcessRunning(char* processName);

H3APPTOOLS_EXP long H3RegSetString(HKEY hKey, CString strSubKey, CString strEntry, CString strValue);
H3APPTOOLS_EXP CString H3RegQueryString(HKEY hKey, CString strSubKey, CString strEntry, CString strDefValue);
H3APPTOOLS_EXP long H3RegSetDWORD(HKEY hKey, CString strSubKey, CString strEntry, DWORD dwValue);
H3APPTOOLS_EXP DWORD H3RegQueryDWORD(HKEY hKey, CString strSubKey, CString strEntry, DWORD dwDefValue);

/// <summary>
/// Creates a folder if it does not exist yet, and if no file with the same name exists.
/// throws exception in case of error.
/// Recursive: creates all root folders that may be required.
/// </summary>
H3APPTOOLS_EXP void CreateFolder(const CString& folderPath);

/// <summary>
/// Creates all folders and subfolders needed for a file, since fstream does not create folders. (!)
/// throws exception in case of error.
/// </summary>
H3APPTOOLS_EXP void CreateFolderForFile(const CString& filePath);

#define _H3MODULE H3FileName(CString(__FILE__))


#endif


