/// 
///	\file    H3Array.h
///	\brief   Implementation de la classe CH3Array et des fonctions associees
///	\version 1.0.6.2
///	\author  E.COLON
///	\date    01/01/2002
///	\remarks 
/// 

// Modifications :
// VC 01/03/2008 - ajout declarations const
// EC 22/05/2008 - transfert des fonctions math dans la classe parent quand possible
//               - ajout fonctions abs,fabs,acos,asin,atan
// EC 04/06/2008 - ajout de l'operateur <<


#ifndef CH3ARRAY__INCLUDED_
#define CH3ARRAY__INCLUDED_

#include <iostream>
#include <fstream>
#include <algorithm>
#include <type_traits>
#include "limits.h"
#include "float.h"
#include "H3BaseDef.h"
#include "H3Point2D.h"
#include "H3Point3D.h"
#include "H3Vector2D.h"
#include "H3Vector3D.h"
#include "H3Rect.h"
#include "H3RGB24.h"

//#pragma warning (once : 4018 4244)

#define H3_ARRAY_FILE_VERSION	100	

#define H3_ARRAY				CH3Array< TYPE >
// 
#define H3_ARRAY_INT8			CH3Array< H3_INT8 >
#define H3_ARRAY_INT16			CH3Array< H3_INT16 >
#define H3_ARRAY_INT32			CH3Array< H3_INT32 >
#define H3_ARRAY_UINT8			CH3Array< H3_UINT8 >
#define H3_ARRAY_UINT16			CH3Array< H3_UINT16 >
#define H3_ARRAY_UINT32			CH3Array< H3_UINT32 >
#define H3_ARRAY_FLT32			CH3Array< H3_FLT32 >
#define H3_ARRAY_FLT64			CH3Array< H3_FLT64 >

// Complexes
#define H3_ARRAY_CPXFLT32		CH3Array< H3_CPXFLT32 >
#define H3_ARRAY_CPXFLT64		CH3Array< H3_CPXFLT64 >

// Points 2D
#define H3_ARRAY_PT2D    		CH3Array< H3_POINT2D       >
#define H3_ARRAY_PT2DINT32		CH3Array< H3_POINT2D_INT32 >
#define H3_ARRAY_PT2DUINT32		CH3Array< H3_POINT2D_UINT32 >

#define H3_ARRAY_PT2DFLT32		CH3Array< H3_POINT2D_FLT32 >
#define H3_ARRAY_PT2DFLT64		CH3Array< H3_POINT2D_FLT64 >

// Points 3D
#define H3_ARRAY_PT3D    		CH3Array< H3_POINT3D       >
#define H3_ARRAY_PT3DINT32		CH3Array< H3_POINT3D_INT32 >
#define H3_ARRAY_PT3DFLT32		CH3Array< H3_POINT3D_FLT32 >
#define H3_ARRAY_PT3DFLT64		CH3Array< H3_POINT3D_FLT64 >

// Vecteurs 3D
#define H3_ARRAY_V3DINT32		CH3Array< H3_VECTOR3D_INT32 >
#define H3_ARRAY_V3DFLT32		CH3Array< H3_VECTOR3D_FLT32 >
#define H3_ARRAY_V3DFLT64		CH3Array< H3_VECTOR3D_FLT64 >

// Couleurs
#define H3_ARRAY_RGB24			CH3Array< H3_RGB24 >

#define H3_CHECKALL_MODE 0

using namespace std;


//////////////////////////////////////////////////////////////////////
// Classe pour avoir une base commune entre toutes les Images/Matrices
class CH3GenericArray
{
public:
    // Taille de données en bits
    virtual int GetTypeSize() const = 0;
    virtual bool IsFloatingPoint() const = 0;
    virtual void* GetDataAsVoid() const = 0;
    // Retourne le nombre d'elements
    virtual size_t GetSize() const = 0;
    // Retourne la taille en bytes
    virtual size_t GetSizeByte() const = 0;
    virtual ~CH3GenericArray() {}
};

//////////////////////////////////////////////////////////////////////
///
///	\class   CH3Array 
///	\brief   tableau monodimensionnel de donnees
///	\author  E.COLON
///	\date    01/01/2002
///	\bug     
///	\remarks 
///
template <class TYPE>
class CH3Array : public virtual CH3GenericArray
{
public:
    virtual int GetTypeSize() const { return sizeof(TYPE) * 8; }
    virtual void* GetDataAsVoid() const { return m_pData; }

    // Redimensionnement
    bool Resize(size_t nSize);

    // recherche et remplacement
    H3_ARRAY_UINT32 Find(TYPE a)const;
    long Replace(TYPE OldValue, TYPE NewValue);


    // Conversion de types
    operator H3_ARRAY_FLT32()const;
    operator H3_ARRAY_PT2DFLT32()const;
    operator H3_ARRAY_INT32()const;

    // Interrogation attribus
    TYPE GetMedian()const;
    TYPE GetSum()const;
    // Retourne le nombre d'elements
    virtual size_t GetSize() const;
    // Retourne la taille en bytes
    virtual size_t GetSizeByte() const;
    const char* GetDataFormat()const;
    TYPE* GetData()const;
    bool IsEmpty() const;
    CH3Array<TYPE> GetStatistics(long nPopPercent = 100)const;
    virtual bool IsFloatingPoint()const;
    bool IsComplex()const;
    bool IsSigned()const;

    // Methodes de tri
    bool Sort(long nMethod = 0);
    bool BubbleSort();
    bool ShakerSort();
    bool QuickSort();

    void HoldData(bool bHold = true);
    void LinkData(const size_t nSize, const TYPE* const pData);

    // Operateurs logiques
    void Not() {};


    // Methodes et operateurs arithmetiques
    CH3Array<TYPE> operator +(const TYPE Value)const;
    void operator +=(const TYPE Value);
    CH3Array<TYPE> operator -(const TYPE Value)const;
    void operator -=(const TYPE Value);
    CH3Array<TYPE> operator *(const TYPE Value)const;
    void operator *=(const TYPE Value);
    CH3Array<TYPE> operator /(const TYPE Value)const;
    void operator /=(const TYPE Value);
    CH3Array<TYPE> operator %(const TYPE Value)const;
    void operator %=(const TYPE value);
    CH3Array<TYPE> operator +(const CH3Array<TYPE>& Src)const;
    void operator +=(const CH3Array<TYPE>& Src);
    CH3Array<TYPE> operator -(const CH3Array<TYPE>& Src)const;
    void operator -=(const CH3Array<TYPE>& Src);
    CH3Array<TYPE> operator *(const CH3Array<TYPE>& Src)const;
    void operator *=(const CH3Array<TYPE>& Src);
    CH3Array<TYPE> operator /(const CH3Array<TYPE>& Src)const;
    void operator /=(const CH3Array<TYPE>& Src);
    CH3Array<TYPE> operator %(const CH3Array<TYPE>& Src)const;
    void operator %=(const CH3Array<TYPE>& Src);

    // Fonctions mathematiques
    void pow(const TYPE Value);
    void sqrt();
    void cos();
    void sin();
    void tan();
    void acos();
    void asin();
    void atan();
    void abs();
    void fabs();
    void rand();
    void _floor();


    // Methodes de copie
    void Copy(const CH3Array<TYPE>& src);
    CH3Array<TYPE>& operator =(const CH3Array<TYPE>& Src);
    void CopyElements(TYPE* pDest, const TYPE* pSrc, size_t nCount);

    // Methodes de remplissage
    void Fill(TYPE Value);

    // Methodes d'E/S
    void Display(const char* pszMsg = nullptr)const;
    bool LoadRAW(const char* pszFileName);
    bool SaveRAW(const char* pszFileName)const;
    bool fLoadRAW(const FILE* Stream);
    bool fSaveRAW(const FILE* Stream)const;
    bool fSave(const FILE* Stream)const;
    bool fLoad(const FILE* Stream);
    bool SaveASCII(const CString& filename)const;
    bool LoadASCII(const char* pszFilename);

    // Manipulation d'elements
    const TYPE& operator[](const size_t nIndex) const;
    TYPE& operator[](const size_t nIndex);
    TYPE& ElementAt(size_t nIndex);
    CH3Array<TYPE> GetAt(const size_t nIndex, const size_t nSize)const;
    TYPE GetAt(const size_t nIndex) const;
    void SetAt(const size_t nIndex, const TYPE Value);
    void SetAt(const size_t nIndex, const CH3Array<TYPE>& AValue);

    // Allocation liberation memoire
    void InitMembers();
    bool Alloc(const size_t nSize);
    bool ReAlloc(const size_t nSize);
    void Free();

    // Constructeurs et destructeur
    CH3Array(const size_t nSize, const TYPE* const pData);
    CH3Array(const CH3Array& src);
    CH3Array(TYPE FirstValue, TYPE Step, TYPE LastValue);
    CH3Array(const size_t nSize, TYPE Value);
    CH3Array(size_t nSize = 0);
    virtual ~CH3Array();

    friend ostream& operator<< (ostream& os, H3_ARRAY& A);

protected:
    bool m_bHold;		// Indique que les donnees sont conservees en memoire
    size_t m_nSize;		// Nombre d'elements //un jour: const ulong size; *const_cast<ulong*>(&m_nSize) = 0;//
    TYPE* m_pData;		// Pointeur vers les elements
};

template <class TYPE>
H3_ARRAY sqrt(const H3_ARRAY& Src);
template <class TYPE>
H3_ARRAY stats(const H3_ARRAY& Src, long nPopPercent = 100);
template <class TYPE>
TYPE Sum(const H3_ARRAY& Src);


template <class TYPE>
ostream& operator<< (ostream& os, H3_ARRAY& A)
{
    for (size_t i = 0; i < A.GetSize(); i++)
    {
        os << A[i] << endl;
    }
    return os;
}

template <class TYPE>
void CH3Array<TYPE>::LinkData(const size_t nSize, const TYPE* const pData)
{
    Free();

    m_nSize = nSize;
    m_pData = (TYPE*)pData;
    HoldData(true);
}

template <class TYPE>
void CH3Array<TYPE>::HoldData(bool bHold)
{
    m_bHold = bHold;
}


// Remplace la valeur OldValue par la valeur NewValue et retourne le nombre
// d'elements remplacés
// EC 22/08/07
template <class TYPE>
long CH3Array<TYPE>::Replace(TYPE OldValue, TYPE NewValue)
{
    H3_ARRAY_UINT32 p = (*this).Find(OldValue);
    for (size_t i = 0; i < p.GetSize(); i++)
    {
        (*this)[p[i]] = NewValue;
    }
    return true;
}


// Trouve les indices de position de toutes les valeurs a
template <class TYPE>
H3_ARRAY_UINT32 CH3Array<TYPE>::Find(TYPE a) const
{
    H3_ARRAY_UINT32 Tmp(GetSize());
    size_t k = 0;

    TYPE* pSrc = GetData();
    for (size_t i = 0; i < GetSize(); i++)
    {
        if (*pSrc == a) Tmp[k++] = i;
        pSrc++;
    }
    return Tmp.GetAt(0, k);
}

// idem
template <>
inline H3_ARRAY_UINT32 CH3Array<H3_FLT32>::Find(H3_FLT32 a) const
{
    H3_ARRAY_UINT32 Tmp(GetSize());
    size_t k = 0;

    if (_isnan(a))
    {
        for (size_t i = 0; i < m_nSize; i++)
        {
            if (_isnan((*this)[i]))
                Tmp[k++] = (unsigned __int32)i;//attn:v64
        }
    }
    else
    {
        for (size_t i = 0; i < m_nSize; i++)
        {
            if (!_isnan((*this)[i]))
                if ((*this)[i] == a)
                    Tmp[k++] = (unsigned __int32)i;//attn:v64
        }
    }

    return Tmp.GetAt(0, k);
}

// Specialisation methode find pour le type H3_CPXFLT32
// Attention doit être declaré inline pour eviter les definitions
// multiples à l'edition de lien
// EC 22/08/07
template <>
inline H3_ARRAY_UINT32 CH3Array<H3_CPXFLT32>::Find(H3_CPXFLT32 a) const
{
    H3_ARRAY_UINT32 Tmp(GetSize());
    size_t k = 0;

    if (_isnan(a.re) || _isnan(a.im))
    {
        for (size_t i = 0; i < m_nSize; i++)
        {
            if (_isnan((*this)[i].re) || _isnan((*this)[i].im))
                Tmp[k++] = (unsigned __int32)i;//attn:v64
        }
    }
    else
    {
        for (size_t i = 0; i < m_nSize; i++)
        {
            if ((!_isnan((*this)[i].re)) && (!_isnan((*this)[i].im)))
                if ((*this)[i] == a)
                {
                    Tmp[k++] = (unsigned __int32)i;//attn:v64
                }
        }
    }

    return Tmp.GetAt(0, k);
}

// Specialisation methode find pour le type H3_CPXFLT64
// EC 22/08/07
template <>
inline H3_ARRAY_UINT32 CH3Array<H3_CPXFLT64>::Find(H3_CPXFLT64 a) const
{
    H3_ARRAY_UINT32 Tmp(GetSize());
    size_t k = 0;

    if (_isnan(a.re) || _isnan(a.im))
    {
        for (size_t i = 0; i < m_nSize; i++)
        {
            if (_isnan((*this)[i].re) || _isnan((*this)[i].im))
                Tmp[k++] = (unsigned __int32)i;//attn:v64
        }
    }
    else
    {
        for (size_t i = 0; i < m_nSize; i++)
        {
            if ((*this)[i] == a)
            {
                Tmp[k++] = (unsigned __int32)i;//attn:v64
            }
        }
    }

    return Tmp.GetAt(0, k);
}

// Conversion vers H3_ARRAY_PT2DFLT32
template <class TYPE>
CH3Array<TYPE>::operator H3_ARRAY_PT2DFLT32()const
{
    H3_ARRAY_PT2DFLT32 Temp(m_nSize);
    H3_POINT2D_FLT32* pDest = Temp.GetData();
    TYPE* pSrc = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        (*(pDest++)) = (H3_POINT2D_FLT32)(*(pSrc++));
    }

    return Temp;
}

// Retourne vrai si le type de donnee est du type float ou double
template <class TYPE>
bool CH3Array<TYPE>::IsFloatingPoint()const
{
    return std::is_floating_point<TYPE>::value;
}

// Retourne vrai si le type de donnee est du type complex
template <class TYPE>
bool CH3Array<TYPE>::IsComplex()const
{
    const type_info& t = typeid(TYPE);

    if (strstr(t.name(), "complex"))
        return true;

    return false;
}

// Retourne vrai si le type de donnee est signe
template <class TYPE>
bool CH3Array<TYPE>::IsSigned()const
{
    const type_info& t = typeid(TYPE);

    if (strstr(t.name(), "unsigned"))
        return false;

    return true;
}

// Retourne la valeur somme des elements
template <class TYPE>
TYPE CH3Array<TYPE>::GetSum()const
{
    return sum(*this);
}

// Retourne la valeur mediane
template <class TYPE>
TYPE CH3Array<TYPE>::GetMedian()const
{
    // Calcul de la valeur mediane
    CH3Array Tmp(*this);
    Tmp.Sort();
    return Tmp[Tmp.GetSize() / 2];
}

template <class TYPE>
CH3Array<TYPE> CH3Array<TYPE>::GetStatistics(long nPopPercent)const
{
    return stats(*this, nPopPercent);
}

// Conversion vers H3_FLT32
template <class TYPE>
CH3Array<TYPE>::operator H3_ARRAY_FLT32()const
{
    H3_ARRAY_FLT32 Temp(m_nSize);
    H3_FLT32* pDest = Temp.GetData();
    TYPE* pSrc = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_FLT32)(*pSrc++);
    }

    return Temp;
}

// Conversion vers H3_INT32
template <class TYPE>
CH3Array<TYPE>::operator H3_ARRAY_INT32()const
{
    H3_ARRAY_INT32 Temp(m_nSize);
    H3_INT32* pDest = Temp.GetData();
    TYPE* pSrc = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_INT32)(*pSrc++);
    }

    return Temp;
}


//////////////////////////////////////////////////////////////////////////////////////
// Groupe des constructeurs/Destructeurs
//////////////////////////////////////////////////////////////////////////////////////
// Construction du tableau TYPE de nSize elements non initialises
template <class TYPE>
inline CH3Array<TYPE>::CH3Array(const size_t nSize)
{
    Alloc(nSize);
}

// Construction du tableau TYPE de nSize elements initialises avec 
// la valeur Value
template <class TYPE>
CH3Array<TYPE>::CH3Array(const size_t nSize, TYPE Value)
{
    if (Alloc(nSize))
        Fill(Value);
}

// Construction du tableau TYPE initialise avec des valeurs
// comprises entre FirstValue et LastValue par pas de step.
// La taille du tableau est fonction de l'etendue et du pas.
template <>
inline CH3Array<H3_CPXFLT32>::CH3Array(H3_CPXFLT32 cfFirstValue, H3_CPXFLT32 cfStep, H3_CPXFLT32 cfLastValue)
{
    InitMembers();

    size_t nSize = 0;


    // Je ne sais pas faire autrement si tout le monde est complexe
    if (cfStep.im != 0 || cfLastValue.im != 0)
    {
        return;
    }

    float Low = min(cfFirstValue.re, cfLastValue.re);
    float High = max(cfFirstValue.re, cfLastValue.re);
    float Step = cfStep.re;

    if (Step != 0)
        nSize = (size_t)((High - Low) / Step) + 1;
    else
        nSize = (size_t)((High - Low)) + 1;

    if (Alloc(nSize))
    {
        float Value = cfFirstValue.re;
        for (size_t i = 0; i < m_nSize; i++)
        {
            (*this)[i] = H3_CPXFLT32(Value, cfFirstValue.im);
            Value += Step;
        }
    }
}


// Construction du tableau TYPE initialise avec des valeurs
// comprises entre FirstValue et LastValue par pas de step.
// La taille du tableau est fonction de l'etendue et du pas.
template <>
inline CH3Array<H3_CPXFLT64>::CH3Array(H3_CPXFLT64 cdFirstValue, H3_CPXFLT64 cdStep, H3_CPXFLT64 cdLastValue)
{
    InitMembers();

    size_t nSize = 0;

    // Je ne sais pas faire autrement si tout le monde est complexe
    if (cdStep.im != 0 || cdLastValue.im != 0)
    {
        return;
    }

    double Low = min(cdFirstValue.re, cdLastValue.re);
    double High = max(cdFirstValue.re, cdLastValue.re);
    double Step = cdStep.re;

    if (Step != 0)
        nSize = (size_t)((High - Low) / Step) + 1;
    else
        nSize = (size_t)((High - Low)) + 1;

    if (Alloc(nSize))
    {
        double Value = cdFirstValue.re;
        for (size_t i = 0; i < m_nSize; i++)
        {
            (*this)[i] = H3_CPXFLT64(Value, cdFirstValue.im);
            Value += Step;
        }
    }
}

// Construction du tableau TYPE initialise avec des valeurs
// comprises entre FirstValue et LastValue par pas de step.
// La taille du tableau est fonction de l'etendue et du pas.
template <class TYPE>
inline CH3Array<TYPE>::CH3Array(TYPE FirstValue, TYPE Step, TYPE LastValue)
{
    InitMembers();

    size_t nSize = 0;

    TYPE Low = min(FirstValue, LastValue);
    TYPE High = max(FirstValue, LastValue);

    if (Step != 0)
        nSize = (size_t)((High - Low) / Step) + 1;
    else
        nSize = (size_t)((High - Low)) + 1;

    if (Alloc(nSize))
    {
        TYPE Value = FirstValue;
        for (size_t i = 0; i < m_nSize; i++)
        {
            m_pData[i] = Value;
            Value += Step;
        }
    }
}

// Construction d'un tableau de nSize elements <TYPE> initialise en fonction
template <class TYPE>
inline CH3Array<TYPE>::CH3Array(const size_t nSize, const TYPE* const pData)//:CH3Array<TYPE> (nSize)
{
    if (Alloc(nSize))
    {
        for (size_t i = 0; i < GetSize(); i++)
            (*this)[i] = pData[i];
    }
}

// Constructeur de copie
template <class TYPE>
inline CH3Array<TYPE>::CH3Array(const CH3Array& src)
{
    if (this != &src)
    {
        Alloc(src.m_nSize);
        CopyElements(m_pData, src.m_pData, src.m_nSize);
    }
}

// Destructeur
template <class TYPE>
inline CH3Array<TYPE>::~CH3Array()
{
    Free();
}

// Initialisation des variables membres
template <class TYPE>
inline void CH3Array<TYPE>::InitMembers()
{
    m_bHold = false;
    m_nSize = 0;
    m_pData = nullptr;
}

// Allocation d'un tableau de nSize element
template <class TYPE>
inline bool CH3Array<TYPE>::Alloc(const size_t nSize)
{
    InitMembers();

    if (nSize > 0)
    {
        try {
            m_pData = new TYPE[nSize];
            if (m_pData != nullptr)
            {
                m_nSize = nSize;
                return true;
            }
        }
        catch (CException* e) {
            CString msg;
            msg.Format(_T("CH3Array:Pb lors de l'allocation de %d élément(s)"), nSize);
            AfxMessageBox(msg);
            e->Delete();
            return false;
        }
    }
    return false;
}

// Redimensionne une matrice en concervant les donnees initiales
// Si la nouvelle dimension est plus petite que la dimension initiale
// alors seuls les premiers elements sont copiés. Les nouveaux 
// elements crées ne sont pas initialisés.
template <class TYPE>
bool CH3Array<TYPE>::Resize(size_t nSize)
{
    if (nSize != m_nSize)
    {
        CH3Array Tmp(*this);

        if (ReAlloc(nSize))
        {
            size_t nCount = __min(nSize, Tmp.GetSize());
#if 1//pour voir
            for (size_t i = 0; i < nCount; i++)
            {
                m_pData[i] = Tmp[i];
            }
#else
            //test cv190711
            TYPE* pSrc = Tmp.GetData();
            memcpy(m_pData, pSrc, nCount * sizeof(TYPE));
#endif
            return true;
        }
        else
            return false;
    }
    return true;
}

// Allocation d'un tableau de nSize element
template <class TYPE>
bool CH3Array<TYPE>::ReAlloc(size_t nSize)
{
    if (nSize != m_nSize)
    {
        Free();
        return Alloc(nSize);
    }
    return true;
}

// Desallocation memoire
template <class TYPE>
void CH3Array<TYPE>::Free()
{
    if (!m_bHold && m_pData && m_nSize > 0)
    {
        delete[] m_pData;
        m_pData = nullptr;
        m_nSize = 0;
    }
}

//////////////////////////////////////////////////////////////////////////////////////
// Operateurs logique
//////////////////////////////////////////////////////////////////////////////////////
template <> void CH3Array<H3_UINT8>::Not()
{
    H3_UINT8* p = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ~(*p);
        p++;
    }
}
template <> void CH3Array<H3_UINT16>::Not()
{
    H3_UINT16* p = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ~(*p);
        p++;
    }
}
template <> void CH3Array<H3_UINT32>::Not()
{
    H3_UINT32* p = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ~(*p);
        p++;
    }
}
template <> void CH3Array<H3_INT8>::Not()
{
    H3_INT8* p = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ~(*p);
        p++;
    }
}
template <> void CH3Array<H3_INT16>::Not()
{
    H3_INT16* p = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ~(*p);
        p++;
    }
}
template <> void CH3Array<H3_INT32>::Not()
{
    H3_INT32* p = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ~(*p);
        p++;
    }
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE>
CH3Array<TYPE> CH3Array<TYPE>::operator +(const TYPE Value)const
{
    // Ca ca marche ...
    H3_ARRAY Tmp(*this);
    TYPE* p = Tmp.m_pData;

    size_t size = Tmp.m_nSize;
    for (size_t i = 0; i < size; i++)
    {
        (*p++) += Value;
    }

    return Tmp;
}


template <class TYPE>
void CH3Array<TYPE>::operator +=(const TYPE Value)
{
    TYPE* p = m_pData;
    for (size_t i = 0; i < m_nSize; i++)
    {
        (*p++) += Value;
    }
}

template <class TYPE>
CH3Array<TYPE> CH3Array<TYPE>::operator -(const TYPE Value)const
{
    H3_ARRAY Tmp(*this);
    TYPE* p = Tmp.m_pData;

    size_t size = Tmp.m_nSize;
    for (size_t i = 0; i < size; i++)
    {
        (*p++) -= Value;
    }

    return Tmp;
}

template <class TYPE>
void CH3Array<TYPE>::operator -=(const TYPE Value)
{
    TYPE* p = m_pData;
    for (size_t i = 0; i < m_nSize; i++)
    {
        (*p++) -= Value;
    }
}

template <class TYPE>
CH3Array<TYPE> CH3Array<TYPE>::operator *(const TYPE Value)const
{
    H3_ARRAY Tmp(*this);
    TYPE* p = Tmp.m_pData;

    size_t size = Tmp.m_nSize;
    for (size_t i = 0; i < size; i++)
    {
        (*p++) *= Value;
    }

    return Tmp;
}

template <class TYPE>
void CH3Array<TYPE>::operator *=(const TYPE Value)
{
    TYPE* p = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        (*p++) *= Value;
    }
}

template <class TYPE>
CH3Array<TYPE> CH3Array<TYPE>::operator /(const TYPE Value)const
{
    H3_ARRAY Tmp(*this);
    TYPE* p = Tmp.m_pData;

    size_t size = Tmp.m_nSize;
    for (size_t i = 0; i < size; i++)
    {
        (*p++) /= Value;
    }

    return Tmp;
}

template <class TYPE>
void CH3Array<TYPE>::operator /=(const TYPE Value)
{
    TYPE* p = m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        (*p++) /= Value;
    }
}

template <class TYPE>
void CH3Array<TYPE>::operator +=(const CH3Array<TYPE>& Src)
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator += (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif
    TYPE* pDest = m_pData;
    TYPE* pSrc = Src.m_pData;
    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ += *pSrc++;
    }
}

template <class TYPE>
CH3Array<TYPE>  CH3Array<TYPE>::operator +(const CH3Array<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator + (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif
    CH3Array Tmp(*this);
    Tmp += Src;
    return Tmp;
}

template <class TYPE>
void CH3Array<TYPE>::operator -=(const CH3Array<TYPE>& Src)
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format("2 CH3Array::operator -= (CH3Array): attention: tableaux de taille distincte");
        AfxMessageBox(msg);
    }
#endif
    TYPE* pDest = m_pData;
    TYPE* pSrc = Src.m_pData;
    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ -= *pSrc++;
    }
}

template <class TYPE>
CH3Array<TYPE>  CH3Array<TYPE>::operator -(const CH3Array<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator - (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif
    CH3Array Tmp(*this);
    Tmp -= Src;
    return Tmp;
}

template <class TYPE>
void CH3Array<TYPE>::operator *=(const CH3Array<TYPE>& Src)
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator *= (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif
    TYPE* pDest = m_pData;
    TYPE* pSrc = Src.m_pData;

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ *= *pSrc++;
    }
}

template <class TYPE>
CH3Array<TYPE>  CH3Array<TYPE>::operator *(const CH3Array<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator * (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif
    CH3Array Tmp(*this);
    Tmp *= Src;
    return Tmp;
}

template <class TYPE>
void CH3Array<TYPE>::operator /=(const CH3Array<TYPE>& Src)
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator /= (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif	
    TYPE* pDest = m_pData;
    TYPE* pSrc = Src.m_pData;
    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ /= *pSrc++;
    }
}

template <class TYPE>
CH3Array<TYPE>  CH3Array<TYPE>::operator /(const CH3Array<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator / (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif
    CH3Array Tmp(*this);
    Tmp /= Src;
    return Tmp;
}

/////////////////////////////////////////////VC 17/02/03
template <class TYPE>
void CH3Array<TYPE>::operator %=(const CH3Array<TYPE>& Src)
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator %= (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif	
    TYPE* pDest = m_pData;
    TYPE* pSrc = Src.m_pData;
    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest = *pDest - floor(*pDest / *pSrc) * *pSrc;
        pDest++; pSrc++;
    }
}

template <class TYPE>
CH3Array<TYPE>  CH3Array<TYPE>::operator %(const CH3Array<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if (Src.m_nSize != m_nSize) {
        CString msg;
        msg.Format(_T("2 CH3Array::operator % (CH3Array): attention: tableaux de taille distincte"));
        AfxMessageBox(msg);
    }
#endif
    CH3Array Tmp(*this);
    Tmp %= Src;
    return Tmp;
}

template <class TYPE>
void CH3Array<TYPE>::operator %=(const TYPE value)
{
    TYPE* pDest = m_pData;
    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest = *pDest - floor(*pDest / value) * value;
        pDest++;
    }
}

template <class TYPE>
CH3Array<TYPE>  CH3Array<TYPE>::operator %(const  TYPE value)const
{
    CH3Array Tmp(*this);
    Tmp %= value;
    return Tmp;
}
/////////////////////////////////////////////

// Retourne TRUE si le buffer est vide
template <class TYPE>
inline bool CH3Array<TYPE>::IsEmpty() const
{
    if (m_pData && m_nSize > 0)
        return false;

    return true;
}

// Initialise tout le tableau avec la valeur Value
template <class TYPE>
void CH3Array<TYPE>::Fill(TYPE Value)
{
    if (m_pData && m_nSize > 0)
    {
        for (size_t i = 0; i < m_nSize; i++)
            m_pData[i] = Value;
    }
}

// Retourne l'element a la position nIndex
template <class TYPE>
inline TYPE CH3Array<TYPE>::GetAt(size_t nIndex) const
{
#if H3_CHECKALL_MODE
    if ((nIndex >= m_nSize) || (nIndex < 0)) {
        CString msg;
        msg.Format(_T("1 CH3Array::GetAt ( ): nIndex=%d\tmax=%d"), nIndex, m_nSize - 1);
        AfxMessageBox(msg);
    }
#endif
    return m_pData[nIndex];
}

template <class TYPE>
CH3Array<TYPE> CH3Array<TYPE>::GetAt(const size_t nIndex, const size_t nSize) const
{
#if H3_CHECKALL_MODE
    if (((nIndex + nSize) > m_nSize)) {//if(((nIndex+nSize)>m_nSize)||(nIndex<0)){
        CString msg;
        msg.Format(_T("2 CH3Array::GetAt ( ): nIndex+nSize=%d\tmax=%d"), nIndex + nSize, m_nSize);
        AfxMessageBox(msg);
    }
#endif
    //CH3Array<TYPE> Dest(nSize);

    // Nombre de lignes à copier
    size_t n = 0;
    if ((size_t)(nIndex + nSize) <= m_nSize)
        n = nSize;
    else
        n = m_nSize - nIndex;//et si nIndex>GetSize?

    CH3Array<TYPE> Dest(n);

    // Copie
    for (size_t i = 0, j = nIndex; i < n; i++, j++)
    {
        Dest[i] = (*this)[j];//Dest[i]=(*this)[nIndex+i];
    }

    return Dest;
}

// Place valeur Value a la position nIndex. La fonction suppose
// que la position nIndex est valide
template <class TYPE>
inline void CH3Array<TYPE>::SetAt(size_t nIndex, TYPE Value)
{
#if H3_CHECKALL_MODE
    if (nIndex > m_nSize) {
        CString msg;
        msg.Format(_T("CH3Array::SetAt= (): débordement de la zone prévue"));
        AfxMessageBox(msg);
    }
#endif
    m_pData[nIndex] = Value;
}

template <class TYPE>//cv280404
inline void CH3Array<TYPE>::SetAt(size_t nIndex, const CH3Array<TYPE>& AValue)
{
#if H3_CHECKALL_MODE
    if ((AValue.m_nSize + nIndex) > m_nSize) {
        CString msg;
        msg.Format(_T("CH3Array::SetAt= (CH3Array): débordement de la zone prévue"));
        AfxMessageBox(msg);
    }
#endif
    TYPE* pT = (TYPE*)GetData() + nIndex;
    TYPE* pA = (TYPE*)AValue.GetData();
    size_t j = 0;

    for (j = 0; j < AValue.GetSize(); j++) {
        m_pData[nIndex + j] = AValue.m_pData[j];
    }
}

// Retourne une reference à l'element à la position nIndex
// La fonction suppose que la position nIndex est valide
template <class TYPE>
inline TYPE& CH3Array<TYPE>::ElementAt(size_t nIndex)
{
#if H3_CHECKALL_MODE
    if ((nIndex) >= m_nSize) {
        CString msg;
        msg.Format(_T("CH3Array::ElementAt(nIndex): débordement de la zone prévue"));
        AfxMessageBox(msg);
    }
#endif
    return m_pData[nIndex];
}

// Retourne l'element a la position nIndex
// Voir fonction GetAt()
template <class TYPE>
inline const TYPE& CH3Array<TYPE>::operator[](size_t nIndex) const
{
#if H3_CHECKALL_MODE
    if (nIndex >= m_nSize) {
        CString msg;
        msg.Format(_T("CH3Array::[]const : débordement de la zone prévue"));
        AfxMessageBox(msg);
    }
#endif
    return m_pData[nIndex];
}

// Retourne une reference à l'element à la position nIndex
// La fonction suppose que la position nIndex est valide
// Voir fonction ElementAt()
template <class TYPE>
inline TYPE& CH3Array<TYPE>::operator[](size_t nIndex)
{
#if H3_CHECKALL_MODE
    if (nIndex >= m_nSize) {
        CString msg;
        msg.Format(_T("CH3Array::[] : débordement de la zone prévue"));
        AfxMessageBox(msg);
    }
#endif
    return m_pData[nIndex];
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateurs de copie
//////////////////////////////////////////////////////////////////////////////////////
// Copie les donnees de l'element src
template<class TYPE>
void CH3Array<TYPE>::Copy(const CH3Array<TYPE>& src)
{
    if (this != &src)
    {
        Free();
        if (Alloc(src.m_nSize))
            CopyElements(m_pData, src.m_pData, m_nSize);
    }
}

// Operateur de copie, voir egalement fonction Copy
template <class TYPE>
/*inline*/ CH3Array<TYPE>& CH3Array<TYPE>::operator =(const CH3Array<TYPE>& Src)
{
    if (this == &Src) return *this;

    Copy(Src);
    return *this;
}

// Fonction generique de copie de nCount elements de type TYPE de pSrc vers pDest
template<class TYPE>
void CH3Array<TYPE>::CopyElements(TYPE* pDest, const TYPE* pSrc, size_t nCount)
{
    //H3Tic();
#if 0
    for (size_t i = 0; i < nCount; i++)
    {
        (*(pDest++)) = (*(pSrc++));
    }
#else
    //test cv190711
    memcpy(pDest, pSrc, nCount * sizeof(TYPE));
#endif
    //H3Toc("2:");
}


//////////////////////////////////////////////////////////////////////////////////////
// Groupe attributs
//////////////////////////////////////////////////////////////////////////////////////
// Retourne une chaine de caractere definissant le type des donnees
template <class TYPE>
const char* CH3Array<TYPE>::GetDataFormat()const
{
    const type_info& t = typeid(TYPE);
    return t.name();
}

// Retourne un pointeur sur les donnees
template<class TYPE>
inline TYPE* CH3Array<TYPE>::GetData()const
{
    return m_pData;
}

// Retourne le nombre d'elements
template <class TYPE>
inline size_t CH3Array<TYPE>::GetSize()const
{
    return m_nSize;
}

template <class TYPE>
inline size_t CH3Array<TYPE>::GetSizeByte()const
{
    return m_nSize * sizeof(TYPE);
}

//////////////////////////////////////////////////////////////////////////////////////
// Groupe de fonctions E/S
//////////////////////////////////////////////////////////////////////////////////////
// Affiche le contenu vers la console de sortie
template <class TYPE>
void CH3Array<TYPE>::Display(const char* pszMsg)const
{
    if (pszMsg) cout << pszMsg;
    cout << GetSize() << " " << GetDataFormat() << endl;
    for (size_t i = 0; i < GetSize(); i++)
        cout << (*this)[i] << " ";
    cout << endl;
}

template <class TYPE>
bool CH3Array<TYPE>::SaveRAW(const char* pszFileName)const
{
    FILE* Stream = nullptr;
    if (Stream = fopen(pszFileName, "wb"))
    {
        bool bRetValue = fSaveRAW(Stream);
        fclose(Stream);
        return bRetValue;
    }

    return false;
}

template <class TYPE>
bool CH3Array<TYPE>::LoadRAW(const char* pszFileName)
{
    FILE* Stream = NULL;
    if (Stream = fopen(pszFileName, "rb"))
    {
        bool bRetValue = fLoadRAW(Stream);
        fclose(Stream);
        return bRetValue;
    }

    return false;
}

template <class TYPE>
bool CH3Array<TYPE>::fLoadRAW(const FILE* Stream)
{
    if (Stream)
    {
        if (m_nSize == 0)
        {
            return true;
        }

        if (m_nSize > 0)
        {
            if (::fread(m_pData, sizeof(TYPE), m_nSize, (FILE*)Stream) == m_nSize)
            {
                return true;
            }
        }
    }
    return false;
}

template <class TYPE>
bool CH3Array<TYPE>::fSaveRAW(const FILE* Stream)const
{
    if (Stream)
    {
        if (::fwrite(m_pData, sizeof(TYPE), m_nSize, (FILE*)Stream) != m_nSize)
        {
            return false;
        }
    }
    return true;
}

template <class TYPE>
bool CH3Array<TYPE>::fLoad(const FILE* Stream)
{
    if (!Stream)
        return false;

    // Lire le type et le verifier
    const type_info& t = typeid(this);

    string strName = t.name();//cv200308
    H3ChangeTypeIdName(strName);
    size_t nTypeNameLen = strName.length() + 1;

    //unsigned long nTypeNameLen=strlen(t.name())+1;
    char* pszTypeName = new char[nTypeNameLen];
    if (fread(pszTypeName, sizeof(char), nTypeNameLen, (FILE*)Stream) != nTypeNameLen)
    {
        delete[] pszTypeName;
        return false;
    }
    pszTypeName[nTypeNameLen - 1] = '\0';

    //if (strcmp(pszTypeName,t.name())!=0)
    if (strName != string(pszTypeName))
    {
        delete[] pszTypeName;
        return false;
    }

    delete[] pszTypeName;

    // Lire la version
    unsigned long nVersion;
    if (fread(&nVersion, sizeof(unsigned long), 1, (FILE*)Stream) != 1)
    {
        return false;
    }

    if (nVersion != H3_ARRAY_FILE_VERSION)
    {
        return false;
    }

    // Lire les dimensions
    size_t nSize;
    unsigned __int32 ui;
    if (fread(&ui, sizeof(unsigned __int32), 1, (FILE*)Stream) != 1)//pb unsigned long / size_t : pour compatibilité v32 v64
    {
        return false;
    }
    nSize = (size_t)ui;

    // Charger les donnees
    if (ReAlloc(nSize) == false)
    {
        return false;
    }

    return fLoadRAW(Stream);
}

template <class TYPE>
bool CH3Array<TYPE>::fSave(const FILE* Stream)const
{
    // Ecrire type
    // EC 03/08/07 pour compatibilite VC6 avec VC8
    {
        const type_info& t = typeid(this);
        string strName = t.name();
        H3ChangeTypeIdName(strName);
        ::fwrite(strName.c_str(), sizeof(char), strName.length() + 1, (FILE*)Stream);
    }

    unsigned __int32 nVersion = H3_ARRAY_FILE_VERSION;
    ::fwrite(&nVersion, sizeof(unsigned __int32), 1, (FILE*)Stream);

    unsigned __int32 ul = (unsigned __int32)m_nSize;
    ::fwrite(&ul, sizeof(unsigned __int32), 1, (FILE*)Stream);//pb unsigned long / size_t : pour compatibilité v32 v64//cv

    return fSaveRAW(Stream);
}

template <class TYPE>
TYPE sum(const H3_ARRAY& Src)
{
    TYPE TheSum = 0;

    TYPE* pSrc = Src.GetData();
    size_t nCount = Src.GetSize();
    for (size_t i = 0; i < nCount; i++)
    {
        TheSum += *pSrc++;
    }

    return TheSum;
}

//cv 130812 pb coherence gestion de tableau vide /plein de Nan
//Et si les entrées sont des entiers???
template <class TYPE>
H3_ARRAY stats(const H3_ARRAY& Src, long nPopPercent)
{
    nPopPercent = min(nPopPercent, 100);
    nPopPercent = max(nPopPercent, 1);
    size_t nSize = Src.GetSize();

    if (nSize == 0) {
        CH3Array<TYPE> Res(7);
        Res.Fill(NaN);
        Res[0] = 0;
        return Res;
    }

    size_t a = nSize * nPopPercent / 100;
    size_t nStep = nSize / a;

    if (Src.IsFloatingPoint())
    {
        long N = 0;
        double SumXi = 0;
        double SumXi2 = 0;
        double Max = -DBL_MAX;
        double Min = DBL_MAX;
        double Mean;
        double Std;
        TYPE* pData = Src.GetData();
        long nCount = nSize;
        for (size_t i = 0; i < nSize; i += nStep)
        {
            double Xi = *pData;
            if (!_isnan(Xi))
            {
                N++;
                SumXi += Xi;
                SumXi2 += Xi * Xi;
                Max = __max(Max, Xi);
                Min = __min(Min, Xi);
            }
            pData += nStep;
        }

        if (N > 0)
        {
            Mean = SumXi / ((double)N);
            Std = sqrt((SumXi2 / (double)N) - Mean * Mean);
        }
        else
        {
            Min = 0;
            Max = 0;
            SumXi = 0;
            SumXi2 = 0;
            Mean = 0;
            Std = 0;
        }

        CH3Array<TYPE> Res(7);
        Res[0] = (TYPE)N;
        Res[1] = (TYPE)Min;
        Res[2] = (TYPE)Max;
        Res[3] = (TYPE)SumXi;
        Res[4] = (TYPE)SumXi2;
        Res[5] = (TYPE)Mean;
        Res[6] = (TYPE)Std;

        return Res;
    }
    else
    {
        long N = 0;
        double SumXi = 0;
        double SumXi2 = 0;
        double Max = -DBL_MAX;
        double Min = DBL_MAX;
        double Mean;
        double Std;
        TYPE* pData = Src.GetData();
        for (size_t i = 0; i < nSize; i += nStep)
        {
            double Xi = *pData;
            N++;
            SumXi += Xi;
            SumXi2 += Xi * Xi;
            Max = __max(Max, Xi);
            Min = __min(Min, Xi);
            pData += nStep;
        }

        if (N > 0)
        {
            Mean = SumXi / ((double)N);
            Std = sqrt((SumXi2 / (double)N) - Mean * Mean);
        }
        else
        {
            Min = 0;
            Max = 0;
            SumXi = 0;
            SumXi2 = 0;
            Mean = 0;
            Std = 0;
        }

        CH3Array<TYPE> Res(7);
        Res[0] = (TYPE)N;
        Res[1] = (TYPE)Min;
        Res[2] = (TYPE)Max;
        Res[3] = (TYPE)SumXi;
        Res[4] = (TYPE)SumXi2;
        Res[5] = (TYPE)Mean;
        Res[6] = (TYPE)Std;

        return Res;
    }
}

///////////////////////////////////////////////////////////////////
// Cette fonction trie les lignes de la matrice dans l'ordre croissant
// en utilisant l'algorithme du ShakerSort
// La fonction retourne false en cas d'echec
template <class TYPE>
bool CH3Array<TYPE>::ShakerSort()
{
    long n = m_nSize;

    long g = 0;
    long d = n - 1;
    long k = 0;
    long i;

    do
    {
        for (i = g; i < d; i++)
        {
            if ((*this)[i] > (*this)[i + 1])
            {
                k = i;
                TYPE tmp = (*this)[i];
                (*this)[i] = (*this)[i + 1];
                (*this)[i + 1] = tmp;
            }
        }
        d = k;
        for (i = d; i >= g; i--)
        {
            if ((*this)[i] > (*this)[i + 1])
            {
                k = i;
                TYPE tmp = (*this)[i];
                (*this)[i] = (*this)[i + 1];
                (*this)[i + 1] = tmp;
            }
        }
        g = k + 1;
    } while (g <= d);

    return true;
}

///////////////////////////////////////////////////////////////////
// Cette fonction trie les lignes de la matrice dans l'ordre croissant
// en utilisant l'algorithme du BubleSort
// La fonction retourne false en cas d'echec
template <class TYPE>
bool CH3Array<TYPE>::BubbleSort()
{
    long n = m_nSize;

    long j = n - 1;
    bool bSwap;
    do
    {
        bSwap = false;
        for (long i = 0; i < j; i++)
        {
            if ((*this)[i] > (*this)[i + 1])
            {
                bSwap = true;
                TYPE tmp = (*this)[i];
                (*this)[i] = (*this)[i + 1];
                (*this)[i + 1] = tmp;
            }
        }
        j--;
    } while (bSwap);

    return true;
}

///////////////////////////////////////////////////////////////////
// Fonction Pivoter effectuant le pivotage du tableau a[] entre les 
// indices start et end. Le pivotage se fait en place, directement 
// dans le tableau a. 
// Le pivot est choisi comme le premier element : a[start].
// La fonction retourne la position du pivot après pivotage.
template <class TYPE>
static long QuickSortSwing(H3_ARRAY& Src, size_t start, size_t end)
{
    size_t pivot = start, j = end;
    // A chaque étape on cherche à avoir les éléments de start à pivot-1
    // plus petits que le pivot, ceux de j+1 à end plus grands que le pivot,
    // les éléments restants de pivot+1 à j étant à faire pivoter.
    while (pivot < j)
    {
        register TYPE* pSrcPivot = &Src[pivot];
        register TYPE* pSrcJ = &Src[j];

        if (*pSrcPivot < *pSrcJ)
        {
            // l'élément j est déjà à sa place.
            j--;
        }
        else
        {
            // échange de a[pivot] et a[j]
            {
                register TYPE tmp = *pSrcPivot;
                *pSrcPivot = *pSrcJ;
                *pSrcJ = tmp;
            }

            // on incrémente la position du pivot
            pivot++;

            pSrcPivot++;
            // on stocke le pivot en a[pivot+1]
            {
                register TYPE tmp = *pSrcPivot;
                *pSrcPivot = *pSrcJ;
                *pSrcJ = tmp;
            }
        }
    }
    return pivot;
}

///////////////////////////////////////////////////////////////////
template <class TYPE>
static void QuickSort(H3_ARRAY& Src, long start, long end)
{
    // a[] contient un seul élément : on a fini !
    if (end <= start) return;

    // pivotage du tableau
    long pivot = QuickSortSwing(Src, start, end);

    // tri réccursif de la partie gauche de a[]
    QuickSort(Src, start, pivot - 1);

    // tri réccursif de la partie droite de a[]
    QuickSort(Src, pivot + 1, end);
}

///////////////////////////////////////////////////////////////////
// Cette fonction trie les lignes de la matrice dans l'ordre croissant
// des valeurs en utilisant l'alogorithme du QuickSort
// La fonction retourne false en cas d'echec
template <class TYPE>
bool CH3Array<TYPE>::QuickSort()
{
    ::QuickSort(*this, 0, m_nSize - 1);
    return true;
}

///////////////////////////////////////////////////////////////////
// Cette fonction trie les lignes de la matrice dans l'ordre croissant
// des valeurs en utilisant l'alogorithme defini par le parametre nMethod
// nMethod=0 QuickSort (defaut)
// nMethod=1 BubleSort
// nMethod=2 ShakerSort
// La fonction retourne false en cas d'echec
template <class TYPE>
bool CH3Array<TYPE>::Sort(long nMethod)
{
    switch (nMethod)
    {
    case 1:
        return BubbleSort();
        break;
    case 2:
        return ShakerSort();
        break;
    case 0:
    default:
        return QuickSort();
        break;
    }

    return true;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3Array<TYPE>::LoadASCII(const char *pszFilename)
///	\brief   chargement depuis un fichier ASCII
/// \details chargement des elements depuis un fichier ASCII. La fonction 
///  charge toutes les valeurs reelles presentes dans le fichier ASCII quelle
///  que soit l'ogranisation ligne/colonnes.
///	\param   pszFilename chemin et nom du fichier source
///	\retval	 true succes de la fonction
///	\retval	 false echec de la fonction
/// \remarks Ne fonctionne que sur les valeurs reelles (complexes pas pris en charge)
/// \see	 SaveASCII()
///	\author  E.COLON
template <class TYPE >
bool CH3Array<TYPE>::LoadASCII(const char* pszFilename)
{
    FILE* stream = nullptr;
    size_t nSize = 0;
    double value;

    if ((stream = fopen(pszFilename, "r")) == nullptr)
    {
        return false;
    }

    // Parcourir le fichier pour compter le nombre de valeurs
    fseek(stream, 0L, SEEK_SET);
    while (!feof(stream))
    {
        int numread = fscanf(stream, "%lf", &value);

        // Verifier si une valeur a ete lue correctement
        if (numread == 0)
        {
            fclose(stream);
            return false;
        }

        if (numread != EOF)
        {
            nSize++;
        }
    }

    // Relire le fichier et stocker les valeurs
    ReAlloc(nSize);

    fseek(stream, 0L, SEEK_SET);

    TYPE* p = GetData();
    for (size_t i = 0; i < nSize; i++)
    {
        fscanf(stream, "%lf", &value);
        *p++ = value;
    }

    // Si on arrive ici, s'est que tout s'est bien passe !
    fclose(stream);
    return true;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3Array<TYPE>::SaveASCII(const char *pszFilename)
///	\brief   enregistrement vers un fichier ASCII
/// \details enregistrement des elements vers un fichier ASCII. Les valeurs
///  sont séparées par un espace.
///	\param   pszFilename chemin et nom du fichier destination
///	\retval	 true succes de la fonction
///	\retval	 false echec de la fonction
/// \remarks Ne fonctionne que sur les valeurs reelles (complexes pas pris en charge)
/// \see	 SaveASCII()
///	\author  E.COLON
template <class TYPE >
bool CH3Array<TYPE>::SaveASCII(const CString& filename)const
{
    CreateFolderForFile(filename);
    fstream f(filename, ios_base::out | ios_base::trunc);
    if (f.is_open())
    {
        for (size_t n = 0; n < m_nSize; n++)
        {
            f << m_pData[n] << endl;
        }

        f.close();
        return true;
    }
    return false;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::sqrt()
///	\brief   Calcule la racine carree de chaque element du tableau
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::sqrt()
{
    TYPE* p = GetData();
    size_t size = GetSize();
    for (size_t i = 0; i < size; i++)
    {
        *p = ::sqrt(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY cos(const H3_ARRAY &aSrc)
///	\brief   retourne la racine carree de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY sqrt(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();

    for (size_t i = 0; i < size; i++)
    {
        (*(pDest++)) = ::sqrt(*(pSrc++));
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::cos()
///	\brief   Calcule le cosinus de chaque element du tableau
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::cos()
{
    TYPE* p = GetData();
    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ::cos(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::acos()
///	\brief   Calcule le acosinus de chaque element du tableau
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::acos()
{
    TYPE* p = GetData();
    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ::acos(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY cos(const H3_ARRAY &aSrc)
///	\brief   retourne le cosinus de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY cos(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();

    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::cos(*pSrc++);
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY acos(const H3_ARRAY &aSrc)
///	\brief   retourne le acosinus de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY acos(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();
    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::acos(*pSrc++);
    }
    return aDest;
}


/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::sin()
///	\brief   Calcule le sinus de chaque element du tableau
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::sin()
{
    TYPE* p = GetData();
    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ::sin(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::sin()
///	\brief   Calcule le sinus de chaque element du tableau
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::asin()
{
    size_t size = GetSize();
    TYPE* p = GetData();
    for (size_t i = 0; i < size; i++)
    {
        *p = ::asin(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY sin(const H3_ARRAY &aSrc)
///	\brief   retourne le sinus de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY sin(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();
    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::sin(*pSrc++);
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY asin(const H3_ARRAY &aSrc)
///	\brief   retourne le asinus de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY asin(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();
    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::asin(*pSrc++);
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::tan()
///	\brief   Calcule la tangente de chaque element du tableau
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::tan()
{
    TYPE* p = GetData();
    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ::tan(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::atan()
///	\brief   Calcule la arctangente de chaque element du tableau
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::atan()
{
    size_t size = GetSize();
    TYPE* p = GetData();
    for (size_t i = 0; i < size; i++)
    {
        *p = ::atan(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY tan(const H3_ARRAY &aSrc)
///	\brief   retourne la tangente de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY tan(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();
    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::tan(*pSrc++);
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY atan(const H3_ARRAY &aSrc)
///	\brief   retourne l'arctangente de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY atan(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();
    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::atan(*pSrc++);
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::pow(const TYPE Value)
///	\brief   Eleve le contenu du tableau à la puissance spécifiée
/// \details Cette fonction eleve le contenu du tableau à la puissance 'Value'.
/// \param	 Value puissance
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::pow(const TYPE Value)
{
    TYPE* p = GetData();
    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = ::pow(*p, Value);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY pow(H3_ARRAY &Src,const TYPE Value)
///	\brief   Eleve le contenu du tableau à la puissance spécifiée
/// \details Cette fonction retourne de elements du tableau eleves à la 
///			 puissance 'Value' passe. Les dimensions du tableau retourne sont
///	         identiques à celles du tableau Src.
///	\retval  H3_ARRAY aSrc^Value
/// \see	 CH3Array<TYPE>::pow()
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY pow(const H3_ARRAY& aSrc, const TYPE Value)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();
    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::pow(*pSrc++, Value);
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::abs()
///	\brief   calcule la valeur abs de chaque element
/// \see	 abs()
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::abs()
{
    TYPE* p = GetData();

    if (IsFloatingPoint())
    {
        for (size_t i = 0; i < m_nSize; i++)
        {
            *p = ::fabs(*p);
            p++;
        }
    }
    else
    {
        for (size_t i = 0; i < m_nSize; i++)
        {
            *p = ::abs(*p);
            p++;
        }
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::fabs()
///	\brief   calcule la valeur abs de chaque element
/// \see	 abs()
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::fabs()
{
    size_t size = GetSize();
    TYPE* p = GetData();
    for (size_t i = 0; i < size; i++)
    {
        *p = ::fabs(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY abs(H3_ARRAY &aSrc)
///	\brief   Retourne la valeur absolue
/// \Param	 aSrc tableau d'entree 	
///	\retval  H3_ARRAY valeur absolue de 'aSrc'
/// \see	 CH3Array<TYPE>::abs()
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY abs(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();

    if (aSrc.IsFloatingPoint())
    {
        for (size_t i = 0; i < size; i++)
        {
            *pDest++ = ::fabs(*pSrc++);
        }
    }
    else
    {
        for (size_t i = 0; i < size; i++)
        {
            *pDest++ = ::abs(*pSrc++);
        }
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY fabs(H3_ARRAY &aSrc)
///	\brief   Retourne la valeur absolue
/// \Param	 aSrc tableau d'entree 	
///	\retval  H3_ARRAY valeur absolue de 'aSrc'
/// \see	 CH3Array<TYPE>::abs()
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY fabs(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();
    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::fabs(*pSrc++);
    }
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      void CH3Array<TYPE>::rand()
///	\brief   Remplissage avec des valeurs aleatoires
/// \details Cette fonction remplit le tableau avec des valeurs aleatoires
///          comprises entre 0 et RAND_MAX.
/// \see	 
///	\author  E.COLON
/// 
template <class TYPE>
void CH3Array<TYPE>::rand()
{
    TYPE* p = GetData();
    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = (TYPE)::rand();	// ca ne marche que pour les type int, float, long etc
        // et pas sur les CPoint2D, 3D ..., complex
        //p->rand();
        p++;
    }



    // remarque EC la fonction rand retourne un entier, elle n'est pas definie pour
    // tous les types, il faudrait voir ca
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY _floor(H3_ARRAY &aSrc)
///	\brief   Retourne la partie entiere
/// \Param	 aSrc tableau d'entree 	
///	\retval  H3_ARRAY partie entiere de 'aSrc'
/// \see	 CH3Array<TYPE>::_floor()
///	\author  CV
/// 
template <class TYPE>
void CH3Array<TYPE>::_floor()
{
    TYPE* p = GetData();
    for (size_t i = 0; i < m_nSize; i++)
    {
        *p = floor(*p);
        p++;
    }
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY _floor(H3_ARRAY &aSrc)
///	\brief   Retourne la partie entiere
/// \Param	 aSrc tableau d'entree 	
///	\retval  H3_ARRAY partie entie de 'aSrc'
/// \see	 CH3Array<TYPE>::_floor()
///	\author  CV 290212
/// 
template <class TYPE>
H3_ARRAY _floor(const H3_ARRAY& aSrc)
{
    size_t size = aSrc.GetSize();
    H3_ARRAY aDest(size);
    TYPE* pDest = aDest.GetData();
    TYPE* pSrc = aSrc.GetData();
    for (size_t i = 0; i < size; i++)
    {
        *pDest++ = ::floor(*pSrc++);
    }
    return aDest;
}

#endif


