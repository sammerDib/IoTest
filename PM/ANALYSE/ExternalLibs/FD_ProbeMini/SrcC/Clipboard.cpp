/*
 * $Id: Clipboard.cpp 5752 2007-10-04 09:36:25Z n-combe $
 */

#include "../StdAfx.h"
#include "Clipboard.h"

/**
 * Copie le texte str dans le presse-papier.
 *
 * @param owner    fenetre parent
 * @param *str     chaine de caracteres
 *
 * @return true en cas de succes, false en cas d'erreur
 */
bool clipCopyText(HWND owner, char * str)
{
	bool returnVal = true;

	if (!owner || !str)
	{
		logMessage(LOG_GENERAL, LOG_DEBUG, 0, "clipCopyText null parameter");
		return false;
	}

	if (OpenClipboard(owner))
	{
		HGLOBAL hglbCopy = GlobalAlloc(GMEM_MOVEABLE, sizeof(char) * (strlen(str) + 1)); 

		if (hglbCopy == NULL) 
		{ 
			CloseClipboard(); 
			returnVal = false; 
		}
		else
		{
			// Lock the handle and copy the text to the buffer. 
			LPTSTR lptstrCopy = (LPTSTR) GlobalLock(hglbCopy); 
			memcpy(lptstrCopy, str, sizeof(char) * (strlen(str) + 1)); 
			GlobalUnlock(hglbCopy); 

			EmptyClipboard();
			SetClipboardData(CF_TEXT, hglbCopy);
			CloseClipboard();

			logMessage(LOG_GENERAL, LOG_INFO, LOG_MSGBOX, "The data has been copied to the clipboard.\nUse the command PASTE (Ctrl-V) in the external program");
		}
	}
	else
	{
		logMessage(LOG_GENERAL, LOG_WARNING, 0, "Clipboard not available");
		returnVal = false;
	}

	return returnVal;
}

/**
 * Copie une image (capture d'ecran) dans le presse papier.
 *
 * @param   owner   fenetre parent
 * @param   hwnd    la fenetre qu'il faut copier
 * @param  *rect    rectangle de la fenetre qu'il faut copier
 *
 * @return true en cas de succes, false en cas d'erreur
 */
bool clipCopyScreenshot(HWND owner, HWND hwnd, RECT *rect)
{
	bool returnVal = true;

	if (!owner || !hwnd)
	{
		logMessage(LOG_GENERAL, LOG_DEBUG, 0, "clipCopyScreenshot null parameter");
		return false;
	}

	if (!OpenClipboard(owner))
	{
		logMessage(LOG_GENERAL, LOG_WARNING, 0, "Clipboard not available");
		return false;
	}

	// Rectangle de selection
	RECT r;
	if (rect)
	{
		r = *rect;
		// Decale le rectangle de selection sur les coordonnees abs de la fenetre
		RECT wndRect;
		GetWindowRect(hwnd, &wndRect);
		OffsetRect(&r, wndRect.left, wndRect.top);
	}
	else
	{
		GetWindowRect(hwnd, &r);
	}

	wFlashWindow(hwnd, &r, 3, 200);

	HDC hdc = GetDC(HWND_DESKTOP);
	HDC hdcMem = CreateCompatibleDC(hdc);
	HBITMAP hBitmap = CreateCompatibleBitmap(hdc, r.right - r.left, r.bottom - r.top);
	if(hBitmap)
	{
		SelectObject(hdcMem, hBitmap);
		BitBlt(hdcMem, 0, 0, r.right - r.left, r.bottom - r.top, hdc, r.left, r.top, SRCCOPY);
		EmptyClipboard();
		SetClipboardData(CF_BITMAP, hBitmap);
		CloseClipboard();

		logMessage(LOG_GENERAL, LOG_INFO, LOG_MSGBOX, "The window content was copied to the clipboard.");
	}

	DeleteDC(hdcMem);
	ReleaseDC(hwnd, hdc);

	return returnVal;
}
