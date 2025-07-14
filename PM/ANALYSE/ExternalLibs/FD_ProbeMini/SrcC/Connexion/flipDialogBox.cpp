
#include "..\SPG_General.h"

#ifdef SPG_General_USECONFIGFILEDLG

#include "..\SPG_Includes.h"

#undef NONLS
#include <windows.h>

#include "flipDialogBox.h"

#include <stdlib.h>
#include <memory.h>
#include <tchar.h>

flipDialogTemplate::flipDialogTemplate(LPCSTR caption, DWORD style, int x, int y, int w, int h,
    LPCSTR font, WORD fontSize)
{

    usedBufferLength = sizeof(DLGTEMPLATE);
    totalBufferLength = usedBufferLength;

    dialogTemplate = (DLGTEMPLATE*)malloc(totalBufferLength);

    dialogTemplate->style = style;
    
    if (font != NULL)
    {
        dialogTemplate->style |= DS_SETFONT;
    }
    
    dialogTemplate->x     = x;
    dialogTemplate->y     = y;
    dialogTemplate->cx    = w;
    dialogTemplate->cy    = h;
    dialogTemplate->cdit  = 0;
    
    dialogTemplate->dwExtendedStyle = 0;

    // The dialog box doesn't have a menu or a special class

    AppendData(_T("\0"), 2);
    AppendData(_T("\0"), 2);

    // Add the dialog's caption to the template

    AppendString(caption);

    if (font != NULL)
    {
        AppendData(&fontSize, sizeof(WORD));
        AppendString(font);
    }
            
}

void flipDialogTemplate::AddComponent(LPCSTR type, LPCSTR caption, DWORD style, DWORD exStyle,
    int x, int y, int w, int h, WORD id)
{

    DLGITEMTEMPLATE item;

    item.style = style;
    item.x     = x;
    item.y     = y;
    item.cx    = w;
    item.cy    = h;
    item.id    = id;

    item.dwExtendedStyle = exStyle;

    AppendData(&item, sizeof(DLGITEMTEMPLATE));
    
    AppendString(type);
    AppendString(caption);

    WORD creationDataLength = 0;
    AppendData(&creationDataLength, sizeof(WORD));

    // Increment the component count
    
    dialogTemplate->cdit++;

}

void flipDialogTemplate::AddButton(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
    int w, int h, WORD id)
{

    AddStandardComponent(0x0080, caption, style, exStyle, x, y, w, h, id);

    WORD creationDataLength = 0;
    AppendData(&creationDataLength, sizeof(WORD));

}

void flipDialogTemplate::AddEditBox(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
    int w, int h, WORD id)
{

    AddStandardComponent(0x0081, caption, style, exStyle, x, y, w, h, id);

    WORD creationDataLength = 0;
    AppendData(&creationDataLength, sizeof(WORD));

}

void flipDialogTemplate::AddStatic(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
    int w, int h, WORD id)
{

    AddStandardComponent(0x0082, caption, style, exStyle, x, y, w, h, id);

    WORD creationDataLength = 0;
    AppendData(&creationDataLength, sizeof(WORD));

}

void flipDialogTemplate::AddListBox(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
    int w, int h, WORD id)
{

    AddStandardComponent(0x0083, caption, style, exStyle, x, y, w, h, id);

    WORD creationDataLength = 0;
    AppendData(&creationDataLength, sizeof(WORD));

}

void flipDialogTemplate::AddScrollBar(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
    int w, int h, WORD id)
{

    AddStandardComponent(0x0084, caption, style, exStyle, x, y, w, h, id);

    WORD creationDataLength = 0;
    AppendData(&creationDataLength, sizeof(WORD));

}

void flipDialogTemplate::AddComboBox(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
    int w, int h, WORD id)
{

    AddStandardComponent(0x0085, caption, style, exStyle, x, y, w, h, id);

    WORD creationDataLength = 0;
    AppendData(&creationDataLength, sizeof(WORD));

}

/**
 * Returns a pointer to the Win32 dialog template which the object
 * represents. This pointer may become invalid if additional
 * components are added to the template.
 */

flipDialogTemplate::operator const DLGTEMPLATE*() const
{
    return dialogTemplate;
}

flipDialogTemplate::~flipDialogTemplate()
{
    free(dialogTemplate);
}

void flipDialogTemplate::AddStandardComponent(WORD type, LPCSTR caption, DWORD style,
    DWORD exStyle, int x, int y, int w, int h, WORD id)
{

    DLGITEMTEMPLATE item;

    // DWORD algin the beginning of the component data
    
    AlignData(sizeof(DWORD));

    item.style = style;
    item.x     = x;
    item.y     = y;
    item.cx    = w;
    item.cy    = h;
    item.id    = id;

    item.dwExtendedStyle = exStyle;

    AppendData(&item, sizeof(DLGITEMTEMPLATE));
    
    WORD preType = 0xFFFF;
    
    AppendData(&preType, sizeof(WORD));
    AppendData(&type, sizeof(WORD));

    AppendString(caption);

    // Increment the component count
    
    dialogTemplate->cdit++;

}

void flipDialogTemplate::AlignData(int size)
{

    int paddingSize = usedBufferLength % size;
    
    if (paddingSize != 0)
    {
        EnsureSpace(paddingSize);
        usedBufferLength += paddingSize;
    }

}

void flipDialogTemplate::AppendString(LPCSTR string)
{

    int length = MultiByteToWideChar(CP_ACP, 0, string, -1, NULL, 0);

    WCHAR* wideString = (WCHAR*)malloc(sizeof(WCHAR) * length);
    MultiByteToWideChar(CP_ACP, 0, string, -1, wideString, length);

    AppendData(wideString, length * sizeof(WCHAR));
    free(wideString);

}

void flipDialogTemplate::AppendData(void* data, int dataLength)
{

    EnsureSpace(dataLength);

    memcpy((char*)dialogTemplate + usedBufferLength, data, dataLength);
    usedBufferLength += dataLength;

}

void flipDialogTemplate::EnsureSpace(int length)
{

    if (length + usedBufferLength > totalBufferLength)
    {
    
        totalBufferLength += length * 2;

        void* newBuffer = malloc(totalBufferLength);
        memcpy(newBuffer, dialogTemplate, usedBufferLength);
        
        free(dialogTemplate);
        dialogTemplate = (DLGTEMPLATE*)newBuffer;

    }
}


#endif
