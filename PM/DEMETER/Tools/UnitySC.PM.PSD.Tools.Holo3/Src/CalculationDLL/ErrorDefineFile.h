//#if !defined(ERRORDEFINEFILE_H_INCLUDED)
//#define ERRORDEFINEFILE_H_INCLUDED
#pragma once


// TO_HOLO3 : implemente here all error message. 
//#define NO_FRGR_ERROR						0x00000000
//#define ERR_FRGR_CALCULATION_BASE_CODE		0x01000000
//#define ERR_FRGR_SAMPLE_ERROR				ERR_FRGR_CALCULATION_BASE_CODE + 0x00000001	// for sample
#define WM_FRGR_UNKNOWN_ERROR				-1
#define WM_FRGR_NO_ERROR					WM_USER+0
#define WM_FRGR_PROCESSING_ERROR			WM_USER+2

#define H3_IM_FLT_REALLOC_ERROR				WM_USER+10
#define H3_CALCULATION_CONSTRUCT_ERROR		WM_USER+11
#define H3_IM_FLT_GETSTAT1_ERROR			WM_USER+13
#define H3_IM_FLT_GETSTAT2_ERROR			WM_USER+14
#define H3_IM_BYT_REALLOC_ERROR				WM_USER+15
#define H3_CALCULATION_FILTRE1_ERROR		WM_USER+16
#define H3_CALCULATION_FILTRE2_ERROR		WM_USER+17
#define H3_CALCULATION_FILTRE3_ERROR		WM_USER+18
#define H3_CALCULATION_FIT1_ERROR			WM_USER+19
#define H3_CALCULATION_FIT2_ERROR			WM_USER+20
#define H3_CALCULATION_FIT3_ERROR			WM_USER+21
#define H3_CALCULATION_DEGAU_ERROR			WM_USER+22
#define H3_CALCULATION_INTENSITY_ERROR		WM_USER+23
#define H3_CALCULATION_PHASE1_ERROR			WM_USER+24
#define H3_CALCULATION_PHASE2_ERROR			WM_USER+25
#define H3_CALCULATION_MASK_ERROR			WM_USER+26
#define H3_CALCULATION_INIT1_ERROR			WM_USER+27
#define H3_CALCULATION_INIT2_ERROR			WM_USER+28
#define H3_CALCULATION_INIT3_ERROR			WM_USER+29
#define H3_CALCULATION_INIT4_ERROR			WM_USER+30
#define H3_CALCULATION_DERIVX1_ERROR		WM_USER+31
#define H3_CALCULATION_DERIVX2_ERROR		WM_USER+32
#define H3_CALCULATION_DERIVX3_ERROR		WM_USER+33
#define H3_CALCULATION_DERIVX4_ERROR		WM_USER+34
#define H3_CALCULATION_DERIVY1_ERROR		WM_USER+35
#define H3_CALCULATION_DERIVY2_ERROR		WM_USER+36
#define H3_CALCULATION_DERIVY3_ERROR		WM_USER+37
#define H3_CALCULATION_DERIVY4_ERROR		WM_USER+38
#define FRGR_TREAT_COPY_ERROR				WM_USER+39
#define FRGR_TREAT_GETIMAGE_ERROR			WM_USER+40

#define MSG_NO_ERROR		"no error"

#define MSG_WM_FRGR_UNKNOWN_ERROR				_T("Unknown Error")
#define MSG_WM_FRGR_NO_ERROR					_T("No Error")
#define MSG_WM_FRGR_PROCESSING_ERROR			_T("Processing Error")
#define MSG_WM_FRGR_IMAGE_READY					_T("Image Ready")
#define MSG_H3_IM_FLT_REALLOC_ERROR				_T("Unable to allocate float image")
#define MSG_H3_CALCULATION_CONSTRUCT_ERROR		_T("Unable to construct Calculation class")
#define MSG_H3_IM_FLT_GETSTAT1_ERROR			_T("Error in statistic computation (N°1)")
#define MSG_H3_IM_FLT_GETSTAT2_ERROR			_T("Error in statistic computation (N°2)")
#define MSG_H3_IM_BYT_REALLOC_ERROR				_T("Unable to allocate byte image")
#define MSG_H3_CALCULATION_FILTRE1_ERROR		_T("Error in filtering (N°1)")
#define MSG_H3_CALCULATION_FILTRE2_ERROR		_T("Error in filtering (N°2)")
#define MSG_H3_CALCULATION_FILTRE3_ERROR		_T("Error in filtering (N°3)")
#define MSG_H3_CALCULATION_FIT1_ERROR			_T("Error in fitting (N°1)")
#define MSG_H3_CALCULATION_FIT2_ERROR			_T("Error in fitting (N°2)")
#define MSG_H3_CALCULATION_FIT3_ERROR			_T("Error in fitting (N°3)")
#define MSG_H3_CALCULATION_DEGAU_ERROR			_T("Error in removing polynomial function")
#define MSG_H3_CALCULATION_INTENSITY_ERROR		_T("Error in computing intensity map")
#define MSG_H3_CALCULATION_PHASE1_ERROR			_T("Error in computing phase map (N°1)")
#define MSG_H3_CALCULATION_PHASE2_ERROR			_T("Error in computing pahse map (N°2)")
#define MSG_H3_CALCULATION_MASK_ERROR			_T("Error in computing mask")
#define MSG_H3_CALCULATION_INIT1_ERROR			_T("Error in calculation class initialisation (N°1)")
#define MSG_H3_CALCULATION_INIT2_ERROR			_T("Error in calculation class initialisation (N°2)")
#define MSG_H3_CALCULATION_INIT3_ERROR			_T("Error in calculation class initialisation (N°3)")
#define MSG_H3_CALCULATION_INIT4_ERROR			_T("Error in calculation class initialisation (N°4)")
#define MSG_H3_CALCULATION_DERIVX1_ERROR		_T("Error in X derivative computation (N°1)")
#define MSG_H3_CALCULATION_DERIVX2_ERROR		_T("Error in X derivative computation (N°2)")
#define MSG_H3_CALCULATION_DERIVX3_ERROR		_T("Error in X derivative computation (N°3)")
#define MSG_H3_CALCULATION_DERIVX4_ERROR		_T("Error in X derivative computation (N°4)")
#define MSG_H3_CALCULATION_DERIVY1_ERROR		_T("Error in Y derivative computation (N°1)")
#define MSG_H3_CALCULATION_DERIVY2_ERROR		_T("Error in Y derivative computation (N°2)")
#define MSG_H3_CALCULATION_DERIVY3_ERROR		_T("Error in Y derivative computation (N°3)")
#define MSG_H3_CALCULATION_DERIVY4_ERROR		_T("Error in Y derivative computation (N°4)")
#define MSG_FRGR_TREAT_COPY_ERROR				_T("Error in treatement copy")
#define MSG_FRGR_TREAT_GETIMAGE_ERROR			_T("Error while getting images")



// registry
#define ERR_GLOBAL_NO_ERROR					0x00000000
#define ERR_REGISTRY_BASE_CODE				0x00000100

#define ERR_KEY_SUCCESS						ERR_GLOBAL_NO_ERROR
#define ERR_KEY_QUERY						ERR_REGISTRY_BASE_CODE+0x00010000
#define ERR_KEY_SET							ERR_REGISTRY_BASE_CODE+0x00020000
#define ERR_KEY_CLOSE						ERR_REGISTRY_BASE_CODE+0x00030000
#define ERR_KEY_CREATE						ERR_REGISTRY_BASE_CODE+0x00040000
#define ERR_KEY_OPEN						ERR_REGISTRY_BASE_CODE+0x00050000
#define ERR_REGISTRY						ERR_REGISTRY_BASE_CODE+0x00060000


///Gestion des messages d'erreurs

void SetLastTestError(int nError);
void GetLastTestError(int &nIDError, CString &strErrorMessage);
void GetLastTestError(CString* sErrorMessage);

//#endif
// 