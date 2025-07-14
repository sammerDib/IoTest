/*
 * $Id: Clipboard.h 5752 2007-10-04 09:36:25Z n-combe $
 */

#ifndef _CLIPBOARD_H_
#define _CLIPBOARD_H_

bool clipCopyText(HWND owner, char * str);
bool clipCopyScreenshot(HWND owner, HWND hwnd, RECT *rect);

#endif // def (_CLIPBOARD_H_)
