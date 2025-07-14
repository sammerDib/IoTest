#include "StdAfx.h"
#include "ImageInfoClass.h"

/////////////////////////////////////////////////////////////////////////////////////////////
// CImageInfoClass
/////////////////////////////////////////////////////////////////////////////////////////////
CImageInfoClass::CImageInfoClass(void)
{
	m_nbImages= m_ImageWidth= m_ImageHeight=0;
}

CImageInfoClass::~CImageInfoClass(void)
{
    FreeData();
}

__declspec(dllexport) void CImageInfoClass::AllocateData(int aNbImg, int aSizeX, int aSizeY)
{
    m_nbImages = aNbImg;
    m_ImageWidth = aSizeX;
    m_ImageHeight = aSizeY;

    long imageSize = m_ImageWidth * m_ImageHeight;

    m_Images.resize(m_nbImages);
    for (int i = 0; i < m_nbImages; i++)
        m_Images[i] = NULL;
}


__declspec(dllexport) void CImageInfoClass::FreeData()
{
    for (int i = 0; i < m_Images.size(); i++)
    {
        if (m_Images[i] != NULL)
        {
            delete[] m_Images[i];
            m_Images[i] = NULL;
        }
    }
    m_nbImages = 0;
    m_Images.clear();
}

int CImageInfoClass::GetNbImages()const
{
    return m_nbImages;
}

int CImageInfoClass::GetSizeX()const
{
    return m_ImageWidth;
}

int CImageInfoClass::GetSizeY()const
{
    return m_ImageHeight;
}
/////////////////////////////////////////////////////////////////////////////////////////////
// CImageInfoClassInput
/////////////////////////////////////////////////////////////////////////////////////////////
__declspec(dllexport) void CImageInfoClassInput::AllocateData(int aNbPeriods, int aNbImgX, int aNbImgY, int aSizeX, int aSizeY)
{
    m_NbVerticalFrange = aNbImgX;
    m_NbHorizontalFrange = aNbImgY;
    m_nbImages = (aNbImgX + aNbImgY)*aNbPeriods;
    m_nbPeriods = aNbPeriods;

    m_ImageWidth = aSizeX;
    m_ImageHeight = aSizeY;

    CImageInfoClass::AllocateData(m_nbImages, aSizeX, aSizeY);
}

__declspec(dllexport) void CImageInfoClassInput::FreePartialData(int period, char direction)
{
    int i0, i1;
    if (direction == 'X')
    {
        i0 = period * m_NbHorizontalFrange;
        i1 = i0 + m_NbHorizontalFrange;
    }
    else
    {
        i0 = m_nbPeriods * m_NbHorizontalFrange + period * m_NbVerticalFrange;
        i1 = i0 + m_NbVerticalFrange;
    }

    for (int i = i0; i < i1; i++)
    {
        delete m_Images[i];
        m_Images[i] = NULL;
    }
}

__declspec(dllexport) void CImageInfoClassInput::FreePartialData(char direction)
{
    int i0, i1;
    if (direction == 'X')
    {
        i0 = 0;
        i1 = i0 + m_nbPeriods * m_NbHorizontalFrange;
    }
    else
    {
        i0 = m_nbPeriods * m_NbHorizontalFrange;
        i1 = i0 + m_nbPeriods * m_NbVerticalFrange;
    }

    for (int i = i0; i < i1; i++)
    {
        delete m_Images[i];
        m_Images[i] = NULL;
    }
}


int CImageInfoClassInput::GetNbImX()const
{
	return m_NbVerticalFrange ;
}

int CImageInfoClassInput::GetNbImY()const
{
    return 	m_NbHorizontalFrange;
}

int CImageInfoClassInput::GetNbPeriods()const
{
    return 	m_nbPeriods;
}
