#pragma once
#include <vector>

class CImageInfoClass
{
public:
	__declspec(dllexport) CImageInfoClass(void);
	__declspec(dllexport) virtual ~CImageInfoClass(void);
    __declspec(dllexport) void AllocateData(int aNbImg, int aSizeX, int aSizeY);
    __declspec(dllexport) void FreeData();

protected:
	int m_nbImages;     // Nombre total d'images
	int m_ImageWidth;
	int m_ImageHeight;

public:
	std::vector<BYTE*> m_Images; 
    int GetNbImages()const;
    int GetSizeX()const;
	int GetSizeY()const;

    BYTE* m_pMask = NULL;
};

/////////////////////////////////////////////////////////////////////////////////////////////

class CImageInfoClassInput : public CImageInfoClass
{
public:
	__declspec(dllexport) void AllocateData(int aNbPeriods, int aNbImgX, int aNbImgY, int aSizeX, int aSizeY);
    __declspec(dllexport) void FreePartialData(int period, char direction);
    __declspec(dllexport) void FreePartialData(char direction);
    int GetNbImX()const;
	int GetNbImY()const;
    int GetNbPeriods() const;
public:
	int m_NbHorizontalFrange; // nb images pour calcul de phase Y pour une période
    int m_NbVerticalFrange;	  // nb images pour calcul de phase X pour une période
    int m_nbPeriods;
};
