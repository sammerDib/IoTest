#include "stdafx.h"
#include "ImageJ.h"
#include "H3Array2D.h"
#include <mil.h>

// FDE debug
static int fdcount = 0;

static void execute(const char* lpApplicationName, char* lpCommandLine)
{
    STARTUPINFO si;
    PROCESS_INFORMATION pi;

    BOOL ok = CreateProcess(
        lpApplicationName, //LPCSTR     lpApplicationName,
        lpCommandLine,     //LPSTR      lpCommandLine,
        NULL,   //LPSECURITY_ATTRIBUTES lpProcessAttributes,
        NULL,   //LPSECURITY_ATTRIBUTES lpThreadAttributes,
        FALSE,  //BOOL                  bInheritHandles,
        /*NORMAL_PRIORITY_CLASS | CREATE_NO_WINDOW | DETACHED_PROCESS*/0,      //DWORD                 dwCreationFlags,
        NULL,   //LPVOID                lpEnvironment,
        NULL,   //LPCSTR                lpCurrentDirectory,
        &si,    //LPSTARTUPINFOA        lpStartupInfo,
        &pi     //LPPROCESS_INFORMATION lpProcessInformation
    );

    if (ok)
    {
        WaitForSingleObject(pi.hProcess, INFINITE);

        DWORD exitCode = 0xFDFDFDFD;
        BOOL result = GetExitCodeProcess(pi.hProcess, &exitCode);

        CloseHandle(pi.hProcess);
        CloseHandle(pi.hThread);
    }
    else
    {
        LPVOID lpMsgBuf;
        DWORD dw = GetLastError();
        FormatMessage(FORMAT_MESSAGE_ALLOCATE_BUFFER | FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
            NULL, dw, MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), (LPTSTR)&lpMsgBuf, 0, NULL);

        _tprintf(_T("CreateProcess() failed: %s\n\n"), (LPTSTR)lpMsgBuf);

        LocalFree(lpMsgBuf);
    }
}

template <class TYPE>
IMAGE_JDECL int ImageJ(const CH3Array2D<TYPE>& img)
{
    return ImageJ(img.GetData(), (int)img.GetCo(), (int)img.GetLi(), img.GetTypeSize(), img.IsFloatType());
}

int ImageJ(long milid)
{
    char filename[1024];
    snprintf(filename, sizeof(filename), "c:\\temp\\fdebug-%d.tif", fdcount);
    MbufSave(filename, milid);

    char cmd[1024];
    const char* viewer = "C:\\Program Files\\ImageJ\\ImageJ.exe\0\0\0";
    //snprintf(cmd, sizeof(cmd), "\"%s\" \"%s\"\0\0\0", viewer, filename);  // C'est la fête à la guillemet
    //execute(viewer, cmd);
    snprintf(cmd, sizeof(cmd), "START \"coucou\" \"%s\" \"%s\"", viewer, filename);  // C'est la fête à la guillemet
    system(cmd);

    return fdcount++;
}

int ImageJ(const void* data, int SizeX, int SizeY, int type, bool isfloat = false)
{
    if (isfloat)
        type |= M_FLOAT;
    else
        type |= M_UNSIGNED;

    MIL_ID milid;
    MbufAlloc2d(ImageJMilSystemID, SizeX, SizeY, type, M_IMAGE + M_PROC, &milid);
    MbufPut(milid, data);
    int ret = ImageJ(milid);
    MbufFree(milid);

    return ret;
}

int ImageJ(const BYTE* data, int SizeX, int SizeY) { return ImageJ(data, ImageJSizeX, ImageJSizeY, 8); }
int ImageJ(const WORD* data, int SizeX, int SizeY) { return ImageJ(data, ImageJSizeX, ImageJSizeY, 16); }
int ImageJ(const float* data, int SizeX, int SizeY) { return ImageJ(data, ImageJSizeX, ImageJSizeY, 32, /*isfloat:*/true); }
int ImageJ(const BYTE* data) { return ImageJ(data, ImageJSizeX, ImageJSizeY); }
int ImageJ(const WORD* data) { return ImageJ(data, ImageJSizeX, ImageJSizeY); }
int ImageJ(const float* data) { return ImageJ(data, ImageJSizeX, ImageJSizeY); }
int ImageJ(const H3_ARRAY2D_FLT32& img) { return ImageJ(img.GetData(), (int)img.GetCo(), (int)img.GetLi(), img.GetTypeSize(), img.IsFloatingPoint()); }
int ImageJ(const H3_ARRAY2D_UINT16& img) { return ImageJ(img.GetData(), (int)img.GetCo(), (int)img.GetLi(), img.GetTypeSize(), img.IsFloatingPoint()); }
int ImageJ(const H3_ARRAY2D_UINT8& img) { return ImageJ(img.GetData(), (int)img.GetCo(), (int)img.GetLi(), img.GetTypeSize(), img.IsFloatingPoint()); }
