/*!
* 	\file    H3AppTools.cpp
* 	\brief   Routines dinitialisation de la DLL
* 	\version 1.0.5.0
* 	\author  E.COLON
* 	\date    04/09/2007
* 	\remarks
*/

#include "stdafx.h"
#include "H3AppTools.h"
#include "H3AppToolsDecl.h"
#include <direct.h>
#include <io.h>
#include <fcntl.h>
#include <math.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

static CString strModule(_T("CH3AppMessageToolsApp"));

CalibPaths _CalibPaths;

extern bool g_bErrorDisplay;
extern bool g_bWarningDisplay;
extern bool g_bInfoDisplay;
extern CString g_strDebugFile;
extern long g_nDebugLevel;

void CreateFoldersHelper(const CString& fullPath, bool includesLeafFolder)
{
    CString& path = (CString&)fullPath;

    char DirName[256];
    char* p = path.GetBuffer();

    char* q = DirName;
    while (*p)
    {
        if (('\\' == *p) || ('/' == *p))
        {
            if (':' != *(p - 1))
            {
                if (PathFileExists(DirName))
                {
                    if (!PathIsDirectory(DirName))
                    {
                        // Part of the path is a file!
                        path.ReleaseBuffer();
                        throw std::runtime_error("Cannot create " + path + ", " + CString(q) + " is a file!");
                    }
                }
                else if (!CreateDirectory(DirName, NULL))
                {
                    path.ReleaseBuffer();
                    throw std::runtime_error("Cannot create " + CString(q));
                }
            }
        }
        *q++ = *p++;
        *q = '\0';
    }
    path.ReleaseBuffer();

    if (includesLeafFolder)
    {
        if (':' != *(q - 1))
        {
            if (PathFileExists(DirName))
            {
                if (!PathIsDirectory(DirName))
                {
                    // Part of the path is a file!
                    throw std::runtime_error("Cannot create " + path + ", " + CString(q) + " is a file!");
                }
            }
            else if (!CreateDirectory(DirName, NULL))
            {
                throw std::runtime_error("Cannot create " + CString(q));
            }
        }
    }
}

void CreateFolder(const CString& folderPath)
{
    CreateFoldersHelper(folderPath, true);
}

void CreateFolderForFile(const CString& filePath)
{
    CreateFoldersHelper(filePath, false);
}