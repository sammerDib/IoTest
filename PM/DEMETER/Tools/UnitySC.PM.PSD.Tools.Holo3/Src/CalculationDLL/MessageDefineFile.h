//#if !defined(MESSAGEDEFINEFILE_H_INCLUDED)
//#define MESSAGEDEFINEFILE_H_INCLUDED

#pragma once

// Define Global for all component
#define CAMERA_SIZE_X		4872
#define CAMERA_SIZE_Y		3248

#define IMG_SIZE_X			4872
#define IMG_SIZE_Y			3248

#define MAX_NUMBER_THREAD					2
#define KEY_NAME "SOFTWARE"

#define WM_BASE_CALCULATION_MSG				WM_USER + 0x0000
#define WM_BASE_ACQUISITION_MSG				WM_USER + 0x0050

// TO_HOLO3 : implemente here all message use by mother view.
#define WM_FRGR_IMAGE_READY					1
#define WM_FRGR_ACQUITION_RUNNING			3
#define WM_FRGR_CALCUL_RUNNING				4
#define WM_FRGR_SAVE_RUNNING				5


//#endif
