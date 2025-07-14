/// 
///	\file    H3Array2D.h
///	\brief   Implementation de la classe CH3Array2D et des fonctions associees
///	\version 1.0.6.2
///	\author  E.COLON
///	\date    01/01/2002
///	\remarks 
/// 

// Modifications :
// EC 06/01/2005 - Ajout Fonctions de tri BubleSortRows,ShakerSortRow et QuickSortRows
// EC 05/01/2005 - Ajout Fonctions Max,Min,Cos,Sin,Tan,Pow
// VC 21/02/2005 - Ajout de controles et de la fonction %
// SJ 30/10/2007 - Ajout de fonction et de const partout
// VC 01/03/2008 - ajout declarations const
// EC 22/05/2008 - transfert des fonctions math dans la classe parent quand possible
//               - ajout fonctions abs,fabs,acos,asin,atan

#ifndef CH3ARRAY2D__INCLUDED_
#define CH3ARRAY2D__INCLUDED_

#include <assert.h>
#include "H3Array.h"
#include "freeImagePlus.h"

#define H3_ARRAY2D_FILE_VERSION	100

//////////////////////////////////////////////////////////////////////
// Generique
#define H3_ARRAY2D				CH3Array2D< TYPE >

// Entiers signes
#define H3_ARRAY2D_INT8			CH3Array2D< H3_INT8 >
#define H3_ARRAY2D_INT16		CH3Array2D< H3_INT16 >
#define H3_ARRAY2D_INT32		CH3Array2D< H3_INT32 >

// Entiers non signes
#define H3_ARRAY2D_UINT8		CH3Array2D< H3_UINT8 >
#define H3_ARRAY2D_UINT16		CH3Array2D< H3_UINT16 >
#define H3_ARRAY2D_UINT32		CH3Array2D< H3_UINT32 >

// Reels
#define H3_ARRAY2D_FLT32		CH3Array2D< H3_FLT32 >
#define H3_ARRAY2D_FLT64		CH3Array2D< H3_FLT64 >
#define H3_ARRAY2D_FLT80		CH3Array2D< H3_FLT80 >

// Complexes
#define H3_ARRAY2D_CPXFLT32		CH3Array2D< H3_CPXFLT32 >
#define H3_ARRAY2D_CPXFLT64		CH3Array2D< H3_CPXFLT64 >

// Points 2D
#define H3_ARRAY2D_PT2DINT32	CH3Array2D< H3_POINT2D_INT32 >
#define H3_ARRAY2D_PT2DFLT32	CH3Array2D< H3_POINT2D_FLT32 >
#define H3_ARRAY2D_PT2DFLT64	CH3Array2D< H3_POINT2D_FLT64 >

// Points 3D
#define H3_ARRAY2D_PT3DINT32	CH3Array2D< H3_POINT3D_INT32 >
#define H3_ARRAY2D_PT3DFLT32	CH3Array2D< H3_POINT3D_FLT32 >
#define H3_ARRAY2D_PT3DFLT64	CH3Array2D< H3_POINT3D_FLT64 >

// Vecteurs 3D
#define H3_ARRAY2D_V3DINT32		CH3Array2D< H3_VECTOR3D_INT32 >
#define H3_ARRAY2D_V3DFLT32		CH3Array2D< H3_VECTOR3D_FLT32 >
#define H3_ARRAY2D_V3DFLT64		CH3Array2D< H3_VECTOR3D_FLT64 >

// Couleurs
#define H3_ARRAY2D_RGB24		CH3Array2D< H3_RGB24 >
class CRect;//ajout cv 2.1.12 pour compilation mx file matlab


//////////////////////////////////////////////////////////////////////
// Classe pour avoir une base commune entre toutes les Images/Matrices
#pragma warning(disable : 4250)
class CH3GenericArray2D : public virtual CH3GenericArray
{
public:
    size_t GetLi() const { return m_nLi; }
    size_t GetCo() const { return m_nCo; }
    virtual ~CH3GenericArray2D() {}

protected:
    size_t m_nLi;		// Nombre de lignes
    size_t m_nCo;		// Nombre de colonnes
};

//////////////////////////////////////////////////////////////////////
///	\class   CH3Array2D 
///	\brief   tableau bi-dimensionnel de donnees
///	\author  E.COLON
///	\date    01/01/2002
///	\bug     
///	\remarks 
///
template <class TYPE >
class CH3Array2D :
    public CH3Array < TYPE >, public CH3GenericArray2D
{
public:
    void LinkData(size_t nLi, size_t nCo, TYPE* pData);
    void LinkData(CH3Array2D<TYPE>& Sr);
    static void TransferDataOwnership(CH3Array2D<TYPE>& Src, CH3Array2D<TYPE>& Dst);

    H3_ARRAY2D reshape(size_t L, size_t C) const;
    H3_ARRAY2D ReSample(const CRect Rect, const size_t nStepLi, const size_t nStepCo) const;

    bool Resize(size_t nLi, size_t nCo);

    TYPE GetSumCo(long nCo)const;
    TYPE GetSumLi(long nLi)const;

    CH3Array<TYPE> GetAtCo(long nCo)const;
    CH3Array<TYPE> GetAtLi(long nLi)const;
    bool SetAtCo(long nCo, CH3Array<TYPE>& Src);
    bool SetAtLi(long nLi, CH3Array<TYPE>& Src);

    bool Set(const TYPE* pSrc, size_t nLi, size_t nCo, size_t nPitch);
    bool Get(const TYPE* pDest, size_t nLi, size_t nCo, size_t nPitch);

    void FlipUD();
    void FlipLR();

    // Fonctions de tri
    bool SortRows(long nCo, long nMethod = 0);
    bool QuickSortRows(long nCo);
    bool ShakerSortRows(long nCo);
    bool BubbleSortRows(long nCo);
    bool SortRowsByIndex(H3_ARRAY_UINT32& Index);

    // Interrogation attributs
    bool IsValid(size_t nLi, size_t nCo)const;
    TYPE* GetLine(size_t i) const;

    // Conversion de types
    operator H3_ARRAY2D_CPXFLT32()const;
    operator H3_ARRAY2D_CPXFLT64()const;
    operator H3_ARRAY2D_UINT8()const;
    operator H3_ARRAY2D_UINT16()const;
    operator H3_ARRAY2D_UINT32()const;
    operator H3_ARRAY2D_INT8()const;
    operator H3_ARRAY2D_INT16()const;
    operator H3_ARRAY2D_INT32()const;
    operator H3_ARRAY2D_FLT32()const;
    operator H3_ARRAY2D_FLT64()const;
    operator H3_ARRAY2D_FLT80()const;

    TYPE& operator ( )(size_t nLi, size_t nCo);
    TYPE   operator ( )(size_t nLi, size_t nCo) const;
    TYPE& operator ( )(size_t nItem);

    void SetAt(size_t nLi, size_t nCo, TYPE Value);
    void SetAt(size_t nLi, size_t nCo, const CH3Array2D<TYPE>& Src);

    CH3Array2D<TYPE> GetAt(size_t nLi, size_t nCo, size_t nNbLi, size_t nNbCo)const;
    CH3Array2D<TYPE> GetAt(H3_RECT_UINT32 rc)const;

    TYPE GetAt(size_t nLi, size_t nCo)const;
    CH3Array<TYPE> GetAt(const H3_ARRAY_PT2DUINT32& ptList)const;

    CH3Array<TYPE> GetProfil(const H3_POINT2D_INT32 pt1, const H3_POINT2D_INT32 pt2)const;
    H3_ARRAY_PT2DUINT32 GetProfilPoints(const H3_POINT2D_INT32 pt1, const H3_POINT2D_INT32 pt2)const;

    CH3Array2D<TYPE> Trans()const;

    // Methodes d'E/S
    void Display(const char* pszMsg = nullptr)const;
    bool LoadRAW(const char* pszFilename, long nLi, long nCo);
    bool LoadASCII(const char* pszFilename);

    bool SaveASCII(const CString& filename)const;

    /// <summary>
    /// Saves .hbf.
    /// </summary>
    bool Save(const char* pszFilename)const;
    bool Load(const char* pszFilename);

    bool fSave(const FILE* Stream)const;
    bool fLoad(const FILE* Stream);
    bool fSaveHeader(const FILE* Stream)const;
    bool fLoadHeader(const FILE* Stream);

    /// <summary>
    /// Exports a copy in Freeimage format.
    /// Float .tif saving: fipImage.save("c:\\temp\\test.tif", TIFF_NONE);
    /// </summary>
    fipImage CopyToNewFreeimage() const;

    // Valeurs maxi/mini
    TYPE Max(const long nLi, const long nCo)const;//attn: -1 est un arg valide
    TYPE Min(const long nLi, const long nCo)const;
    void GetStat(float& min, float& max, float& mean, float& std, const H3_ARRAY2D_UINT8* pMask = NULL) const;

    // Methodes et operateur arithmetiques
    CH3Array2D<TYPE> operator +(const TYPE Value)const;
    CH3Array2D<TYPE> operator -(const TYPE Value)const;
    CH3Array2D<TYPE> operator *(const TYPE Value)const;
    CH3Array2D<TYPE> operator /(const TYPE Value)const;

    CH3Array2D<TYPE> operator +(const CH3Array2D<TYPE>& Src)const;
    CH3Array2D<TYPE> operator -(const CH3Array2D<TYPE>& Src)const;
    CH3Array2D<TYPE> operator *(const CH3Array2D<TYPE>& Src)const;
    CH3Array2D<TYPE> operator /(const CH3Array2D<TYPE>& Src)const;
    CH3Array2D<TYPE> operator %(const CH3Array2D<TYPE>& Src)const;

    // Methodes de copie
    void Copy(const CH3Array2D<TYPE>& src);
    CH3Array2D<TYPE>& operator =(const CH3Array2D& Src);

    // Constructeurs/Destructeurs
    CH3Array2D(const H3_ARRAY_PT2D& Pt);
    CH3Array2D(size_t nLi, size_t nCo, TYPE* pData);
    CH3Array2D(const CH3Array2D& src);
    CH3Array2D(size_t nLi = 0, size_t nCo = 0);
    virtual ~CH3Array2D();

    // Allocation liberation memoire
    void InitMembers();
    bool Alloc(size_t nLi, size_t nCo);
    bool ReAlloc(size_t nLi, size_t nCo);
    void Free();
};

template <class TYPE>
H3_ARRAY2D sqrt(const H3_ARRAY2D& Src);
template <class TYPE>
H3_ARRAY2D Min(const H3_ARRAY2D& Src1, const H3_ARRAY2D& Src2);
template <class TYPE>
H3_ARRAY2D Max(const H3_ARRAY2D& Src1, const H3_ARRAY2D& Src2);
template <class TYPE>
H3_ARRAY2D Floor(const H3_ARRAY2D& Src1);

template <class TYPE>
fipImage CH3Array2D<TYPE>::CopyToNewFreeimage() const
{
    FREE_IMAGE_TYPE type;
    switch (sizeof(TYPE))
    {
    case 4:
        type = FIT_FLOAT;
        break;
    default:
        type = FIT_BITMAP;
        break;
    }

    fipImage pRet(type, GetCo(), GetLi(), sizeof(TYPE) * 8);

    TYPE* pSource = m_pData;
    size_t lineSize_bytes = GetCo() * sizeof(TYPE);
    for (int l = GetLi(); (--l) >= 0;)// Reverse line order to change origin from top left to bottom left (freeImage).
    {
        memcpy(pRet.getScanLine(l), pSource, lineSize_bytes); // Line copy, padding excluded.
        pSource += GetCo();
    }

    return pRet;
}

template <class TYPE>
void CH3Array2D<TYPE>::LinkData(size_t nLi, size_t nCo, TYPE* pData)
{
    CH3Array<TYPE>::LinkData(nLi * nCo, pData);
    m_nLi = nLi;
    m_nCo = nCo;
}

template <class TYPE>
void CH3Array2D<TYPE>::LinkData(CH3Array2D<TYPE>& Src)
{
    CH3Array2D<TYPE>::LinkData(Src.m_nLi, Src.m_nCo, Src.m_pData);
}

template <class TYPE>
void CH3Array2D<TYPE>::TransferDataOwnership(CH3Array2D<TYPE>& Src, CH3Array2D<TYPE>& Dst)
{
    assert(!Src.m_bHold);
    Dst.LinkData(Src);
    Src.m_bHold = true;
    Dst.m_bHold = false;
}

// Redimensionne une matrice en concervant les donnees initiales
// Si la nouvelle dimension est plus petite que la dimension initiale
// alors seuls les premiers elements sont copiés. Les nouveaux 
// elements crées ne sont pas initialisés.
template <class TYPE>
bool CH3Array2D<TYPE>::Resize(size_t nLi, size_t nCo)
{
    if (nLi != m_nLi || nCo != m_nCo)
    {
        CH3Array2D Tmp(*this);
        if (ReAlloc(nLi, nCo))
        {
            SetAt(0, 0, Tmp);
        }
    }
    return true;
}

//redimensionne une matrice
template <class TYPE>
H3_ARRAY2D H3_ARRAY2D::reshape(size_t L, size_t C) const
{
    if ((L * C) == (m_nLi * m_nCo)) {
        H3_ARRAY2D Ret(*this);
        Ret.m_nLi = L;
        Ret.m_nCo = C;

        return Ret;
    }
    else {
        return H3_ARRAY2D(0, 0);
        //en fait: throw error
    }
}

// Redimensionne un tableau en prelevant une region d'interet
// et dans cette region d'interet en sous échantillonnant
//ATTENTION: modif le 200511 CV pour prendre en compte vrai CRect
//ReSample_old remplace la fonction ReSample d'avant cette date
template <class TYPE>
H3_ARRAY2D H3_ARRAY2D::ReSample(const CRect Rect, const size_t nStepLi, const size_t nStepCo)const
{
    try {
        //verif
        if (Rect.top<0 || Rect.top>m_nLi ||
            Rect.bottom<Rect.top || Rect.bottom>m_nLi ||
            Rect.left  <0 || Rect.left  >m_nCo ||
            Rect.right <Rect.left || Rect.right >m_nCo)
        {
            CString strFunction("ReSample");
            CString msg;
            msg.Format("ROI inadaptée _ Utiliser ReSample_old pour code d'avant le 20/05/11");

#if defined(H3APPMESSAGETOOLSDECL_H__INCLUDED_)
            H3DebugError(_T("H3_ARRAY2D"), strFunction, msg);
#else
            AfxMessageBox(msg);
#endif

            return H3_ARRAY2D(0, 0);
        }

        //la fonction
        H3_ARRAY2D retA((Rect.bottom - Rect.top) / nStepLi + 1, (Rect.right - Rect.left) / nStepCo + 1);
        size_t li, co;
        TYPE* pSrc0 = GetData() + Rect.top * m_nCo + Rect.left;
        TYPE* pSrc;
        TYPE* pDest = retA.GetData();

        for (li = Rect.top; li < Rect.bottom; li += nStepLi)
        {
            pSrc = pSrc0;
            for (co = Rect.left; co < Rect.right; co += nStepCo)
            {
                (*(pDest++)) = (*pSrc);
                pSrc += nStepCo;
            }
            pSrc0 += nStepLi * m_nCo;
        }

        return retA;

    }
    catch (...) {
        CString strFunction("ReSample");
        CString msg;
        msg.Format("pour les programmes créés avant le 20/05/11 , remplacer l'appel à la fonction par ReSample_old");
#if defined(H3APPMESSAGETOOLSDECL_H__INCLUDED_)
        H3DebugError(_T("H3_ARRAY2D"), strFunction, msg);
#else
        AfxMessageBox(msg);
#endif
        return H3_ARRAY2D(0, 0);
    }
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::Trans()const
{
    CH3Array2D Tmp(GetCo(), GetLi());

    size_t nbli = GetLi();
    size_t nbco = GetCo();

    for (size_t li = 0; li < nbli; li++)
        for (size_t co = 0; co < nbco; co++)
            Tmp(co, li) = (*this)(li, co);

    return Tmp;
}

template <class TYPE >
bool CH3Array2D <TYPE>::Load(const char* pszFilename)
{
    bool bRetValue = false;
    FILE* Stream = NULL;
    errno_t errno;
    errno = fopen_s(&Stream, pszFilename, "rb");
    if (errno == 0 && Stream != NULL)
    {
        if (fLoad(Stream))
        {
            bRetValue = true;
        }
        else {
            CString strFunction(_T("Load"));
            CString msg;
            msg.Format(_T("format fichier inadapté: %s"), pszFilename);

            AfxMessageBox(msg);
        }
        fclose(Stream);
    }
    else {
        CString strFunction(_T("Load"));
        CString msg;
        msg.Format(_T("Fichier inexistant: %s"), pszFilename);
        AfxMessageBox(msg);
    }

    return bRetValue;
}

template <class TYPE >
bool CH3Array2D <TYPE>::fLoad(const FILE* Stream)
{
    if (!fLoadHeader(Stream))
        return false;

    return fLoadRAW(Stream);
}

template <class TYPE >
bool CH3Array2D <TYPE>::fLoadHeader(const FILE* Stream)
{
    if (!Stream)
        return false;

    // Lire le type et le verifier
    // EC 03/08/07 pour compatibilite VC6 avec VC8
    {
        const type_info& t = typeid(this);

        string strName = t.name();
        H3ChangeTypeIdName(strName);

        size_t i = strName.find(" __ptr64", 0);//en 64 bit la signature de classe change un peu//cv
        if (i != string::npos)
            strName = strName.substr(0, i);

        size_t nTypeNameLen = strName.length() + 1;
        char* pszTypeName = new char[nTypeNameLen];
        if (fread(pszTypeName, sizeof(char), nTypeNameLen, (FILE*)Stream) != nTypeNameLen)
        {
            delete[] pszTypeName;
            return false;
        }
        pszTypeName[nTypeNameLen - 1] = '\0';

        bool b1 = (strName != string(pszTypeName));

        if (b1)
        {
            delete[] pszTypeName;
            return false;
        }
        delete[] pszTypeName;
    }

    // Lire la version
    {
        __int32 nVersion;
        if (fread(&nVersion, sizeof(__int32), 1, (FILE*)Stream) != 1)//if (fread(&nVersion,sizeof(unsigned long),1,(FILE *)Stream)!=1)//cv
            return false;

        if (nVersion != H3_ARRAY2D_FILE_VERSION)
            return false;
    }

    // Lire les dimensions
    unsigned __int32 ui;//pb compatibilité v32 et v64 bits (ancienne def: unsigned long...)
    size_t nLi;
    if (fread(&ui, sizeof(unsigned __int32), 1, (FILE*)Stream) != 1)//if (fread(&nLi,sizeof(unsigned long),1,(FILE *)Stream)!=1)//cv
        return false;
    nLi = (size_t)ui;
    size_t nCo;
    if (fread(&ui, sizeof(unsigned __int32), 1, (FILE*)Stream) != 1)//if (fread(&nCo,sizeof(unsigned long),1,(FILE *)Stream)!=1)//cv
        return false;
    nCo = (size_t)ui;


    // Adapter les dimensions du buffer
    if (ReAlloc(nLi, nCo) == false)
        return false;

    return true;
}

template <class TYPE >
bool CH3Array2D <TYPE>::Save(const char* pszFilename)const
{
    bool bRetValue = false;
    FILE* Stream = NULL;
    errno_t errno;
    errno = fopen_s(&Stream, pszFilename, "wb");
    if (errno == 0 && Stream != NULL)
    {
        if (fSave(Stream))
        {
            bRetValue = true;
        }
        fclose(Stream);
    }

    return bRetValue;
}

template <class TYPE >
bool CH3Array2D <TYPE>::fSave(const FILE* Stream)const
{
    if (!fSaveHeader(Stream))
        return false;

    // Ecrire donnees
    return fSaveRAW(Stream);
}

template <class TYPE >
bool CH3Array2D <TYPE>::fSaveHeader(const FILE* Stream)const
{
    if (Stream == nullptr)
        return false;

    // Ecrire type
    // EC 03/08/07 pour compatibilite VC6 avec VC8
    {
        const type_info& t = typeid(this);
        string strName = t.name();
        H3ChangeTypeIdName(strName);

        size_t i = strName.find(" __ptr64", 0);//en 64 bit la signature de classe change un peu//cv
        if (i != string::npos)
            strName = strName.substr(0, i);

        ::fwrite(strName.c_str(), sizeof(char), strName.length() + 1, (FILE*)Stream);
    }

    // Ecrire la version
    __int32 nVersion = H3_ARRAY2D_FILE_VERSION;
    ::fwrite(&nVersion, sizeof(__int32), 1, (FILE*)Stream);//::fwrite(&nVersion,sizeof(size_t),1,(FILE *)Stream);//cv

    // Ecrire les dimensions
    ::fwrite(&m_nLi, sizeof(__int32), 1, (FILE*)Stream);//::fwrite(&m_nLi,sizeof(size_t),1,(FILE *)Stream);//cv
    ::fwrite(&m_nCo, sizeof(__int32), 1, (FILE*)Stream);//::fwrite(&m_nCo,sizeof(size_t),1,(FILE *)Stream);//cv


    return true;
}



//////////////////////////////////////////////////////////////////////////////////////
// Groupe de fonctions Constructeurs/Destructeurs
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE >
CH3Array2D <TYPE>::CH3Array2D(size_t nLi, size_t nCo)
{
    Alloc(nLi, nCo);
}

// Constructeur de copie
template <class TYPE>
inline CH3Array2D<TYPE>::CH3Array2D(const CH3Array2D& src)
{
    if (this != &src)
    {
        if (Alloc(src.m_nLi, src.m_nCo))
            CopyElements(m_pData, src.m_pData, src.m_nSize);
    }
}

// Construction d'un tableau de nLi*nCo elements <TYPE> initialise
// avec les donnees pointees par pData
template <class TYPE >
CH3Array2D <TYPE>::CH3Array2D(size_t nLi, size_t nCo, TYPE* pData) :
    CH3Array<TYPE>(nLi* nCo, pData)
{
    m_nLi = nLi;
    m_nCo = nCo;
}

//Constructeur de Array_Pt2D vers Array2D
template <class TYPE >
H3_ARRAY2D::CH3Array2D(const H3_ARRAY_PT2D& Pt)
{
    long i, nbPt = Pt.GetSize();

    (*this) = H3_ARRAY2D(nbPt, 2);
    TYPE* data = GetData();
    for (i = 0; i < nbPt; i++) {
        (*(data++)) = Pt[i].x;
        (*(data++)) = Pt[i].y;
    }
}

// Destructeur ... No comment
template <class TYPE>
CH3Array2D<TYPE>::~CH3Array2D()
{
    Free();
}

// Initialisation des variables membres
template <class TYPE>
inline void CH3Array2D<TYPE>::InitMembers()
{
    CH3Array<TYPE>::InitMembers();
    m_nLi = m_nCo = 0;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et d'allocation et de liberation
//////////////////////////////////////////////////////////////////////////////////////
// Allocation d'un tableau de nLi*nCo elements
template <class TYPE>
bool CH3Array2D<TYPE>::Alloc(size_t nLi, size_t nCo)
{
    InitMembers();

    // Pour eviter d'allouer des tableaux de dimensions nulles
    if (nLi > 0 && nCo > 0)
    {
        if (CH3Array<TYPE>::Alloc(nLi * nCo))
        {
            m_nLi = nLi;
            m_nCo = nCo;
            return true;
        }
    }

    return false;
}

// ReAllocation d'un tableau de nLi*nCo elements
template <class TYPE>
bool CH3Array2D<TYPE>::ReAlloc(size_t nLi, size_t nCo)
{
    if (nLi != m_nLi || nCo != m_nCo)
    {
        Free();
        return Alloc(nLi, nCo);
    }
    return true;
}


// Desallocation memoire
template <class TYPE>
inline void CH3Array2D<TYPE>::Free()
{
    CH3Array<TYPE>::Free();
    m_nLi = m_nCo = 0;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator +(const TYPE Value)const
{
    CH3Array2D Tmp(*this);
    Tmp += Value;
    return Tmp;
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator -(const TYPE Value)const
{
    CH3Array2D Tmp(*this);
    Tmp -= Value;
    return Tmp;
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator *(const TYPE Value)const
{
    CH3Array2D Tmp(*this);
    Tmp *= Value;
    return Tmp;
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator /(const TYPE Value)const
{
    CH3Array2D Tmp(*this);
    Tmp /= Value;
    return Tmp;
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator +(const CH3Array2D<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if ((Src.m_nLi != m_nLi) || (Src.m_nCo != m_nCo)) {
        CString msg;
        msg.Format("CH3Array2D::operator + (CH3Array2D): attention: matrices de taille distincte");
        AfxMessageBox(msg);
    }
#endif
    CH3Array2D Tmp(*this);
    Tmp += Src;
    return Tmp;
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator -(const CH3Array2D<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if ((Src.m_nLi != m_nLi) || (Src.m_nCo != m_nCo)) {
        CString msg;
        msg.Format("CH3Array2D::operator - (CH3Array2D): attention: matrices de taille distincte");
        AfxMessageBox(msg);
    }
#endif
    CH3Array2D Tmp(*this);
    Tmp -= Src;
    return Tmp;
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator *(const CH3Array2D<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if ((Src.m_nLi != m_nLi) || (Src.m_nCo != m_nCo)) {
        CString msg;
        msg.Format("CH3Array2D::operator * (CH3Array2D): attention: matrices de taille distincte");
        AfxMessageBox(msg);
    }
#endif
    CH3Array2D Tmp(*this);
    Tmp *= Src;
    return Tmp;
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator /(const CH3Array2D<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if ((Src.m_nLi != m_nLi) || (Src.m_nCo != m_nCo)) {
        CString msg;
        msg.Format("CH3Array2D::operator / (CH3Array2D): attention: matrices de taille distincte");
        AfxMessageBox(msg);
    }
#endif
    CH3Array2D Tmp(*this);
    Tmp /= Src;
    return Tmp;
}

template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::operator %(const CH3Array2D<TYPE>& Src)const
{
#if H3_CHECKALL_MODE
    if ((Src.m_nLi != m_nLi) || (Src.m_nCo != m_nCo)) {
        CString msg;
        msg.Format("CH3Array2D::operator % (CH3Array2D): attention: matrices de taille distincte");
        AfxMessageBox(msg);
    }
#endif
    CH3Array2D Tmp(*this);
    Tmp %= Src;
    return Tmp;
}

// Macro utilisee par GetProfil 
#define _CMP(res,x,y)		\
{							\
	if (x<y) res=1;			\
	else if (x>y) res=-1;	\
		else res=0;			\
}

template <class TYPE>
H3_ARRAY_PT2DUINT32 CH3Array2D<TYPE>::GetProfilPoints(
    H3_POINT2D_INT32 pt1,
    H3_POINT2D_INT32 pt2)const
{
    unsigned long x1 = pt1.x;
    unsigned long y1 = pt1.y;
    unsigned long x2 = pt2.x;
    unsigned long y2 = pt2.y;

    if (y1 >= 0 && y1 < GetLi() &&
        y2 >= 0 && y2 < GetLi() &&
        x1 >= 0 && x1 < GetCo() &&
        x2 >= 0 && x2 < GetCo())
    {
        unsigned long dxx = ::abs((long)(x2 - x1));
        unsigned long dyy = ::abs((long)(y2 - y1));
        long s1; _CMP(s1, x1, x2);
        long s2; _CMP(s2, y1, y2);

        unsigned long n = __max(dxx, dyy) + 1;
        H3_ARRAY_PT2DUINT32 Buffer(n);

        long k = 0;
        if (k < n)
        {
            Buffer[k].x = x1;
            Buffer[k].y = y1;
            k++;
        }
        long ech = 0;
        if (dyy > dxx)
        {
            unsigned long tmp = dxx;
            dxx = dyy;
            dyy = tmp;
            ech = 1;
        }

        long v = 2 * dyy - dxx;

        //Algorithme de Bresenham
        long x = x1, y = y1;
        for (unsigned long i = 1; i <= dxx; i++)
        {
            while (v >= 0)
            {
                if (ech)
                    x += s1;
                else
                    y += s2;

                v = v - 2 * dxx;
            }

            if (ech) y += s2;
            else x += s1;
            v = v + 2 * dyy;

            if (k < n)
            {
                Buffer[k].x = x;
                Buffer[k].y = y;
                k++;
            }
        }

        return Buffer;
    }

    return H3_ARRAY_PT2DUINT32(0);
}

template <class TYPE>
CH3Array<TYPE> CH3Array2D<TYPE>::GetProfil(
    H3_POINT2D_INT32 pt1,
    H3_POINT2D_INT32 pt2)const
{
    unsigned long x1 = pt1.x;
    unsigned long y1 = pt1.y;
    unsigned long x2 = pt2.x;
    unsigned long y2 = pt2.y;

    if (y1 >= 0 && y1 < GetLi() &&
        y2 >= 0 && y2 < GetLi() &&
        x1 >= 0 && x1 < GetCo() &&
        x2 >= 0 && x2 < GetCo())
    {
        unsigned long dxx = ::abs((long)(x2 - x1));
        unsigned long dyy = ::abs((long)(y2 - y1));
        long s1; _CMP(s1, x1, x2);
        long s2; _CMP(s2, y1, y2);

        unsigned long n = __max(dxx, dyy) + 1;
        CH3Array<TYPE> Buffer(n);

        long k = 0;
        if (k < n)
        {
            Buffer[k++] = (*this)[y1 * m_nCo + x1];
        }

        long ech = 0;
        if (dyy > dxx)
        {
            unsigned long tmp = dxx;
            dxx = dyy;
            dyy = tmp;
            ech = 1;
        }

        long v = 2 * dyy - dxx;

        //Algorithme de Bresenham
        long x = x1, y = y1;
        for (unsigned long i = 1; i <= dxx; i++)
        {
            while (v >= 0)
            {
                if (ech)
                    x += s1;
                else
                    y += s2;

                v = v - 2 * dxx;
            }

            if (ech) y += s2;
            else x += s1;
            v = v + 2 * dyy;

            if (k < n)
            {
                Buffer[k++] = (*this)[y * m_nCo + x];
            }
        }

        return Buffer;
    }

    return CH3Array<TYPE>(0);
}


//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateurs de copie
//////////////////////////////////////////////////////////////////////////////////////
// Copie les donnees de l'element src
template<class TYPE>
void CH3Array2D<TYPE>::Copy(const CH3Array2D<TYPE>& src)
{
    if (this != &src)
    {
        if (ReAlloc(src.m_nLi, src.m_nCo))
        {
            CopyElements(m_pData, src.m_pData, m_nSize);
        }
    }
}

// Operateur de copie, voir egalement fonction Copy
template <class TYPE>
CH3Array2D<TYPE>& CH3Array2D<TYPE>::operator =(const CH3Array2D& Src)
{
    if (this == &Src) return *this;

    Copy(Src);

    return *this;
}

// Retourne true si les coordonnees nLi,nCo sont valides
template <class TYPE>
bool CH3Array2D<TYPE>::IsValid(size_t nLi, size_t nCo)const
{
    if (nLi > 0 && nLi < m_nLi &&
        nCo>0 && nCo < m_nCo)
        return true;
    else
        return false;
}

template <class TYPE>
TYPE CH3Array2D<TYPE>::GetAt(size_t nLi, size_t nCo)const
{
#if H3_CHECKALL_MODE
    if ((nLi >= m_nLi) || (nLi < 0)) {
        CString msg;
        msg.Format("1 CH3Array2D::GetAt(long nLi,long nCo): nLi=%d max=%d", nLi, m_nLi - 1);
        AfxMessageBox(msg);
    }
    if ((nCo >= m_nCo) || (nCo < 0)) {
        CString msg;
        msg.Format("1 CH3Array2D::GetAt(long nLi,long nCo): nCo=%d max=%d", nCo, m_nCo - 1);
        AfxMessageBox(msg);
    }
#endif
    return (*this)[nLi * m_nCo + nCo];
}

// Retourne une reference sur l'element (nLi,nCo), aucune verification
// sur la validite de la position n'est faite
template <class TYPE>
inline TYPE& CH3Array2D<TYPE>::operator ( )(size_t nLi, size_t nCo)
{
#if H3_CHECKALL_MODE
    if ((nLi >= m_nLi) || (nLi < 0)) {
        CString msg;
        msg.Format("2 CH3Array2D::operator ( ): nLi=%d max=%d", nLi, m_nLi - 1);
        AfxMessageBox(msg);
    }
    if ((nCo >= m_nCo) || (nCo < 0)) {
        CString msg;
        msg.Format("2 CH3Array2D::operator ( ): nCo=%d max=%d", nCo, m_nCo - 1);
        AfxMessageBox(msg);
    }
#endif
    return (*this)[nLi * m_nCo + nCo];
}

template <class TYPE>
inline TYPE   CH3Array2D<TYPE>::operator ( )(size_t nLi, size_t nCo) const
{
#if H3_CHECKALL_MODE
    if ((nLi >= m_nLi) || (nLi < 0)) {
        CString msg;
        msg.Format("2b CH3Array2D::operator ( ): nLi=%d max=%d", nLi, m_nLi - 1);
        AfxMessageBox(msg);
    }
    if ((nCo >= m_nCo) || (nCo < 0)) {
        CString msg;
        msg.Format("2b CH3Array2D::operator ( ): nCo=%d max=%d", nCo, m_nCo - 1);
        AfxMessageBox(msg);
    }
#endif
    return (*this)[nLi * m_nCo + nCo];
}

template <class TYPE>
inline TYPE& CH3Array2D<TYPE>::operator ( )(size_t nItem)
{
#if H3_CHECKALL_MODE
    if ((nItem >= m_nLi * m_nCo) || (nItem < 0)) {
        CString msg;
        msg.Format("3 CH3Array2D::operator ( ): nItem=%d max=%d", nItem, m_nLi * m_nCo - 1);
        AfxMessageBox(msg);
    }
#endif
    return (*this)[nItem];
}

template <class TYPE>
inline void CH3Array2D<TYPE>::SetAt(
    size_t nLi,
    size_t nCo,
    const TYPE Value)
{
#if H3_CHECKALL_MODE
    if ((nLi >= m_nLi) || (nLi < 0)) {
        CString msg;
        msg.Format(_T("4 CH3Array2D::SetAt ( ): nLi=%d max=%d"), nLi, m_nLi - 1);
        AfxMessageBox(msg);
    }
    if ((nCo >= m_nCo) || (nCo < 0)) {
        CString msg;
        msg.Format(_T("4 CH3Array2D::SetAt ( ): nCo=%d max=%d"), nCo, m_nCo - 1);
        AfxMessageBox(msg);
    }
#endif
    m_pData[nLi * m_nCo + nCo] = Value;
}

// Incruste Src en nLi,nCo
template <class TYPE>
void CH3Array2D<TYPE>::SetAt(
    size_t nLi,
    size_t nCo,
    const CH3Array2D<TYPE>& Src)
{
#if H3_CHECKALL_MODE
    if (((nLi + Src.m_nLi) > m_nLi) || (nLi < 0)) {
        CString msg;
        msg.Format(_T("5 CH3Array2D::SetAt ( ): nLi_0=%d\tnLi_end=%d\tmax=%d"), nLi, nLi + Src.m_nLi - 1, m_nLi - 1);
        AfxMessageBox(msg);
    }
    if (((nCo + Src.m_nCo) > m_nCo) || (nCo < 0)) {
        CString msg;
        msg.Format(_T("5 CH3Array2D::SetAt ( ): nCo_0=%d\tnCo_end=%d\tmax=%d"), nCo, nCo + Src.m_nCo - 1, m_nCo - 1);
        AfxMessageBox(msg);
    }
#endif
    // Nombre de lignes à copier
    size_t nbli = 0;
    if (nLi + Src.GetLi() <= GetLi())
        nbli = Src.GetLi();
    else
        nbli = GetLi() - nLi;

    // Nombre de colonnes à copier
    size_t nbco = 0;
    if (nCo + Src.GetCo() <= GetCo())
        nbco = Src.GetCo();
    else
        nbco = GetCo() - nCo;

    // Copie//cv 250308
    size_t offset0 = nLi * m_nCo + nCo, offsetSrc0 = 0;
    size_t offset, offsetSrc;
    TYPE* pData = GetData();
    TYPE* pDataSrc = Src.GetData();

    size_t li, co;
    for (li = 0; li < nbli; li++)
    {
        offset = offset0;
        offsetSrc = offsetSrc0;
        for (co = 0; co < nbco; co++)
        {
            pData[offset++] = pDataSrc[offsetSrc++];
        }
        offsetSrc0 += Src.m_nCo;
        offset0 += m_nCo;
    }
}

template <class TYPE>
CH3Array<TYPE> CH3Array2D<TYPE>::GetAt(
    const H3_ARRAY_PT2DUINT32& ptList)const
{

    CH3Array<TYPE > Values(ptList.GetSize());
    for (size_t i = 0; i < ptList.GetSize(); i++)
    {
        size_t x = ptList[i].x;
        size_t y = ptList[i].y;

        size_t Offset = y * m_nCo + x;
        if (Offset >= m_nSize)
            return CH3Array<TYPE>(0);

        Values[i] = m_pData[Offset];
    }

    return Values;
}

// Incruste Src en nLi,nCo
template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::GetAt(
    H3_RECT_UINT32 rc)const
{
    return GetAt(rc.top, rc.left, rc.Height(), rc.Width());
}

// Incruste Src en nLi,nCo
template <class TYPE>
CH3Array2D<TYPE> CH3Array2D<TYPE>::GetAt(
    size_t nLi, size_t nCo,
    size_t nNbLi, size_t nNbCo)const
{
#if H3_CHECKALL_MODE
    if (((nLi + nNbLi) > m_nLi) || (nLi < 0)) {
        CString msg;
        msg.Format(_T("6 CH3Array2D::SetAt ( ): nLi_0=%d\tnLi_end=%d\tmax=%d"), nLi, nLi + nNbLi - 1, m_nLi - 1);
        AfxMessageBox(msg);
    }
    if (((nCo + nNbCo) > m_nCo) || (nCo < 0)) {
        CString msg;
        msg.Format(_T("6 CH3Array2D::SetAt ( ): nCo_0=%d\tnCo_end=%d\tmax=%d"), nCo, nCo + nNbCo - 1, m_nCo - 1);
        AfxMessageBox(msg);
    }
#endif
    // Nombre de lignes à copier
    size_t nbli = 0;
    if (nLi + nNbLi <= m_nLi)
        nbli = nNbLi;
    else
        nbli = GetLi() - nLi;

    // Nombre de colonnes à copier
    size_t nbco = 0;
    if (nCo + nNbCo <= m_nCo)
        nbco = nNbCo;
    else
        nbco = GetCo() - nCo;

    CH3Array2D Dest(nbli, nbco);

    // Copie//cv250308
    TYPE* pDest = Dest.GetData();
    TYPE* pSrc0 = GetData() + nLi * m_nCo + nCo;
    TYPE* pSrc;
    for (size_t li = 0; li < nbli; li++)
    {
        pSrc = pSrc0;
        for (size_t co = 0; co < nbco; co++)
        {
            (*(pDest++)) = (*(pSrc++));
        }
        pSrc0 += m_nCo;
    }

    return Dest;
}

//////////////////////////////////////////////////////////////////////////////////////
// Groupe de fonctions E/S
//////////////////////////////////////////////////////////////////////////////////////
template <class TYPE>
void CH3Array2D<TYPE>::Display(const char* pszMsg)const
{
    if (pszMsg) cout << pszMsg;
    cout << GetLi() << "*" << GetCo() << " " << GetDataFormat() << endl;
    for (size_t li = 0; li < m_nLi; li++)
    {
        for (size_t co = 0; co < m_nCo; co++)
        {
            cout << m_pData[li * m_nCo + co] << " ";
        }
        cout << endl;
    }
}

// EC 05/01/05, Probleme a corriger :
// attention cette fonction ne sauvegarde pas toutes les decimales 
// des nombres reels. 
template <class TYPE >
bool CH3Array2D<TYPE>::SaveASCII(const CString& filename)const
{
    CreateFolderForFile(filename);
    fstream f(filename, ios_base::out | ios_base::trunc);
    if (f.is_open())
    {
        size_t k = 0;
        for (size_t nLi = 0; nLi < m_nLi; nLi++)
        {
            for (size_t nCo = 0; nCo < m_nCo; nCo++)
            {
                f << m_pData[k++] << " ";
            }
            f << endl;
        }

        f.close();
        return true;
    }
    return false;
}

template <class TYPE >
bool CH3Array2D<TYPE>::LoadRAW(const char* pszFilename, long nLi, long nCo)
{
    FILE* Stream = fopen(pszFilename, "rb");
    if (Stream)
    {
        ReAlloc(nLi, nCo);
        fLoadRAW(Stream);
        fclose(Stream);
        return true;
    }
    return false;
}

template <class TYPE >
bool CH3Array2D<TYPE>::LoadASCII(const char* pszFilename)
{
    FILE* stream = nullptr;
    size_t  nbli = 0, nbco = 0, nco = 0;
    double value;

    if ((stream = fopen(pszFilename, "r")) == nullptr)
    {
        return false;
    }

    // Parcourir le fichier pour compter le nombre de lignes et
    // de colonnes
    fseek(stream, 0L, SEEK_SET);

    while (!feof(stream))
    {
        long numread = fscanf(stream, "%lf", &value);

        // Verifier si une valeur a ete lue correctement
        if (numread == 0)
        {
            fclose(stream);
            return false;
        }

        if (numread != EOF)
        {
            nco++;

            // chercher le prochain caractere interressant autre
            // qu'un espace ou une tabulation
            char ch = (char)fgetc(stream);
            while ((ch == ' ') || (ch == '\t'))
                ch = (char)fgetc(stream);

            if (ch != EOF)
                fseek(stream, -1L, SEEK_CUR);

            // si fin de ligne ou EOF alors verifier si le nombre d'elements trouves
            // dans la ligne est constant et incrementer le nombre de lignes
            if (ch == '\n' || ch == EOF)
            {
                if (nbli == 0) nbco = nco;
                if (nco != nbco)
                {
                    fclose(stream);
                    return false;
                }
                nco = 0;
                nbli++;
            }
        }
    }

    // Relire le fichier et stocker les valeurs dans la structure matrice
    ReAlloc(nbli, nbco);

    fseek(stream, 0L, SEEK_SET);

    TYPE* p = GetData();
    for (size_t i = 0; i < nbli * nbco; i++)
    {
        fscanf(stream, "%lf", &value);
        *p++ = value;
    }

    // Si on arrive ici, s'est que tout s'est bien passe !
    fclose(stream);
    return true;
}

//////////////////////////////////////////////////////////////////////////////////////
// Groupe de fonctions d'interrogation
//////////////////////////////////////////////////////////////////////////////////////
// Retourne le nombre de lignes de la matrice
template <class TYPE >
TYPE* CH3Array2D <TYPE>::GetLine(size_t i) const
{
#if H3_CHECKALL_MODE
    if ((i >= m_nLi) || (i < 0)) {
        CString strFunction("GetLine");
        CString msg;
        msg.Format(_T("2 CH3Array2D::GetLine ( ): i=%d max=%d"), i, m_nLi - 1);
#if defined(H3APPTOOLSDECL_H__INCLUDED_)
        H3DebugError(_T("H3_ARRAY2D"), strFunction, msg);
#else
        AfxMessageBox(msg);
#endif
    }
#endif
    //return m_Line[i];
    return (TYPE*)(GetData() + i * m_nCo);
}


//////////////////////////////////////////////////////////////////////////////////////
// Operateurs de conversion de types
//////////////////////////////////////////////////////////////////////////////////////
// Conversion vers H3_UINT8
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_UINT8()const
{
    H3_ARRAY2D_UINT8 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_UINT8* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_UINT8)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers H3_INT8
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_INT8()const
{
    H3_ARRAY2D_INT8 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_INT8* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_INT8)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers H3_UINT16
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_UINT16()const
{
    H3_ARRAY2D_UINT16 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_UINT16* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_UINT16)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers H3_INT16
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_INT16()const
{
    H3_ARRAY2D_INT16 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_INT16* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_INT16)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers H3_UINT32
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_UINT32()const
{
    H3_ARRAY2D_UINT32 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_UINT32* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_UINT32)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers H3_UINT32
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_INT32()const
{
    H3_ARRAY2D_INT32 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_INT32* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_INT32)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers H3_FLT32
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_FLT32()const
{
    H3_ARRAY2D_FLT32 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_FLT32* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_FLT32)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers H3_FLT64
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_FLT64()const
{
    H3_ARRAY2D_FLT64 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_FLT64* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_FLT64)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers H3_FLT80
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_FLT80()const
{
    H3_ARRAY2D_FLT80 Temp(m_nLi, m_nCo);
    TYPE* pSrc = m_pData;
    H3_FLT80* pDest = Temp.GetData();

    for (size_t i = 0; i < m_nSize; i++)
    {
        *pDest++ = (H3_FLT80)(*pSrc++);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers complex<float>
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_CPXFLT32()const
{
    H3_ARRAY2D_CPXFLT32 Temp(m_nLi, m_nCo);
    for (size_t i = 0; i < GetSize(); i++)
    {
        H3_CPXFLT32 v = H3_CPXFLT32((*this)[i], 0);
        Temp(i) = H3_CPXFLT32((*this)[i], 0);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Conversion vers complex<double>
template <class TYPE>
CH3Array2D<TYPE>::operator H3_ARRAY2D_CPXFLT64()const
{
    H3_ARRAY2D_CPXFLT64 Temp(m_nLi, m_nCo);
    for (long i = 0; i < GetSize(); i++)
    {
        Temp(i) = H3_CPXFLT64((*this)[i], 0);
    }

    return Temp;
}

///////////////////////////////////////////////////////////////////
// Cette fonction retourne la valeur maxi.
// nLi=-1 nCo=mmm retourne la valeur maxi de la colonne mmm
// nLi=nnn nCo=-1 retourne la valeur maxi de la ligne nnn
// nLi=nCo=-1 retourne la valeur mini de la matrice
// nLi=nnn nCo=mmm retourne la valeur de la ligne nnn colonne mmm
template <class TYPE>
TYPE CH3Array2D<TYPE>::Max(const long nLi, const long nCo)const
{
    long nStep, nFirst;

    if ((nLi >= GetLi()) ||
        (nCo >= GetCo()))
    {
        return 0;
    }

    if (nLi < 0)
    {
        if (nCo < 0)
        {
            nStep = 1;
            nFirst = 0;
        }
        else
        {
            nStep = GetCo();
            nFirst = nCo;
        }
    }
    else
    {
        if (nCo < 0)
        {
            nStep = 1;
            nFirst = nLi * GetCo();
        }
        else
        {
            return (*this)(nLi, nCo);
        }
    }

    double theMax = -DBL_MAX;
    long nSize = GetSize();
    for (long i = nFirst; i < nSize; i += nStep)
    {
        theMax = __max(theMax, (*this)[i]);
    }

    return theMax;
}

///////////////////////////////////////////////////////////////////
// Cette fonction retourne la valeur mini.
// nLi=-1 nCo=mmm retourne la valeur mini de la colonne mmm
// nLi=nnn nCo=-1 retourne la valeur mini de la ligne nnn
// nLi=nCo=-1 retourne la valeur mini de la matrice
// nLi=nnn nCo=mmm retourne la valeur de la ligne nnn colonne mmm
template <class TYPE>
TYPE CH3Array2D<TYPE>::Min(const long nLi, const long nCo)const
{
    long nStep, nFirst;

    if ((nLi >= GetLi()) ||
        (nCo >= GetCo()))
    {
        return 0;
    }

    if (nLi < 0)
    {
        if (nCo < 0)
        {
            nStep = 1;
            nFirst = 0;
        }
        else
        {
            nStep = GetCo();
            nFirst = nCo;
        }
    }
    else
    {
        if (nCo < 0)
        {
            nStep = 1;
            nFirst = nLi * GetCo();
        }
        else
        {
            return (*this)(nLi, nCo);
        }
    }

    double theMin = DBL_MAX;
    long nSize = GetSize();
    for (long i = nFirst; i < nSize; i += nStep)
    {
        theMin = __min(theMin, (*this)[i]);
    }

    return theMin;
}

///////////////////////////////////////////////////////////////////
// Cette fonction retourne la valeur mini/maxi/mean/stdev.
template <class TYPE>
void CH3Array2D<TYPE>::GetStat(float& min, float& max, float& mean, float& std, const H3_ARRAY2D_UINT8* pMask)const
{
    long i = m_nSize, j = 0L;

    std = 0.0f;
    mean = 0.0f;

    float val;
    unsigned long nb = 0;

    if (pMask == NULL)
    {
        max = min = m_pData[0];
        while (i--)
        {
            val = m_pData[j];
            std += val * val;
            mean += val;
            nb++;
            min = __min(val, min);
            max = __max(val, max);
            j++;
        }
        mean /= nb;
        std /= nb;
        std = sqrtf(std - mean * mean);
    }
    else
    {
        assert(pMask->GetCo() == m_nCo || pMask->GetLi() == m_nLi);

        min = FLT_MAX;
        max = FLT_MIN;
        while (i--)
        {
            if ((*pMask)[j])
            {
                val = m_pData[j];
                std += val * val;
                mean += val;
                nb++;
                min = __min(val, min);
                max = __max(val, max);
            }
            j++;
        }

        mean /= nb;
        std /= nb;
        std = sqrtf(std - mean * mean);
    }
}

///////////////////////////////////////////////////////////////////
// Cette fonction echange les lignes en concervant les colonnes
// (effet miroir horizontal)
//  X = 1 4      devient  3 6
//      2 5               2 5
//      3 6               1 4
template <class TYPE>
void CH3Array2D<TYPE>::FlipUD()
{
    CH3Array<TYPE> TmpRow(m_nCo);
    TYPE* pTmpRow = TmpRow.GetData();

    size_t N = m_nLi;
    size_t M = m_nCo;
    size_t N2 = N / 2;
    size_t nCount = sizeof(TYPE) * M;
    TYPE* pSrc = GetData();			// @ premiere ligne
    TYPE* pDest = GetData() + (N - 1) * M;	// @ derniere ligne
    for (size_t i = 0; i < N2; i++)
    {
        memcpy(pTmpRow, pSrc, nCount);
        memcpy(pSrc, pDest, nCount);
        memcpy(pDest, pTmpRow, nCount);

        pSrc += M;					// Ligne suivante
        pDest -= M;					// Ligne precedente
    }
}

///////////////////////////////////////////////////////////////////
// Cette fonction echange les colones en concervant les lignes
// (effet miroir vertical)
//  X = 1 4      devient  3 6
//      2 5               2 5
//      3 6               1 4
template <class TYPE>
void CH3Array2D<TYPE>::FlipLR()
{
    size_t i, j;
    TYPE* pT, * pM, * pM0;
    H3_ARRAY2D M(m_nLi, m_nCo);

    pT = GetData();
    pM0 = M.GetLine(1) - 1;

    for (i = 0; i < m_nLi; i++)
    {
        pM = pM0;
        for (j = 0; j < m_nCo; j++)
            (*(pM--)) = (*(pT++));
        pM0 += m_nCo;
    }

    *this = M;
}

///////////////////////////////////////////////////////////////////
// Cette fonction retourne les elements de la colonne nCo
template <class TYPE>
CH3Array<TYPE> CH3Array2D<TYPE>::GetAtCo(long nCo)const
{
    if (nCo < 0 || nCo >= m_nCo)
        return CH3Array<TYPE>(0);

    CH3Array<TYPE> Dest(m_nLi);
    TYPE* pSrc = GetData() + nCo;
    TYPE* pDest = Dest.GetData();
    long nCount = m_nLi;
    while (nCount--)
    {
        *pDest = *pSrc;
        pDest++;
        pSrc += m_nCo;
    }

    return Dest;
}

///////////////////////////////////////////////////////////////////
// Cette fonction fixe les elements de la colonne nCo
template <class TYPE>
bool CH3Array2D<TYPE>::SetAtCo(long nCo, CH3Array<TYPE>& Src)
{
    if (nCo < 0 || nCo >= m_nCo)
        return false;

    if (Src.GetSize() != m_nLi)
        return false;


    TYPE* pSrc = Src.GetData();
    TYPE* pDest = GetData() + nCo;
    long nCount = m_nLi;
    while (nCount--)
    {
        *pDest = *pSrc;
        pDest += m_nCo;
        pSrc++;
    }

    return true;
}


///////////////////////////////////////////////////////////////////
// Cette fonction retourne les elements de la ligne nLi
template <class TYPE>
CH3Array<TYPE> CH3Array2D<TYPE>::GetAtLi(long nLi)const
{
    if (nLi < 0 || nLi >= m_nLi)
        return CH3Array<TYPE>(0);

    CH3Array<TYPE> Dest(m_nCo);
    TYPE* pSrc = GetData() + m_nCo * nLi;
    TYPE* pDest = Dest.GetData();
    long nCount = m_nCo;
    while (nCount--)
    {
        *pDest++ = *pSrc++;
    }

    return Dest;
}

///////////////////////////////////////////////////////////////////
// Cette fonction fixe les elements de la ligne nLi
template <class TYPE>
bool CH3Array2D<TYPE>::SetAtLi(long nLi, CH3Array<TYPE>& Src)
{
    if (nLi < 0 || nLi >= m_nLi)
        return false;

    if (Src.GetSize() != m_nCo)
        return false;

    TYPE* pSrc = Src.GetData();
    TYPE* pDest = GetData() + m_nCo * nLi;
    long nCount = m_nCo;
    while (nCount--)
    {
        *pDest++ = *pSrc++;
    }

    return true;
}

///////////////////////////////////////////////////////////////////
// Cette fonction retourne la somme des elements d'une colonne
template <class TYPE>
TYPE CH3Array2D<TYPE>::GetSumCo(long nCo)const
{
    if (nCo < 0 || nCo >= m_nCo)
        return 0;

    TYPE TheSum = 0;

    TYPE* p = GetData() + m_nCo;
    long nCount = m_nLi;
    while (nCount--)
    {
        TheSum += *p;
        p += m_nCo;
    }

    return TheSum;
}

///////////////////////////////////////////////////////////////////
// Cette fonction retourne la somme des elements d'une ligne
template <class TYPE>
TYPE CH3Array2D<TYPE>::GetSumLi(long nLi)const
{
    if (nLi < 0 || nLi >= m_nLi)
        return 0;

    TYPE TheSum = 0;

    TYPE* p = GetData() + nLi * m_nCo;
    long nCount = m_nLi;
    while (nCount--)
    {
        TheSum += *p;
        p++;
    }

    return TheSum;
}

///////////////////////////////////////////////////////////////////
// Cette fonction trie les lignes de la matrice dans l'ordre croissant
// des valeurs specifiees sur la colonne nCo en utilisant l'alogorithme
// du BubleSort
// La fonction retourne false en cas d'echec
template <class TYPE>
bool CH3Array2D<TYPE>::BubbleSortRows(long nCo)
{
    // Verification
    if ((nCo >= m_nCo) || (nCo < 0))
        return false;

    // Creer un tableau contenant les adresses des elements de la colonne
    H3_ARRAY_UINT32 Index(m_nLi);
    for (long i = 0, off = nCo; i < m_nLi; i++, off += m_nCo)
        Index[i] = off;

    // Trier, a la fin de cette operation le vecteur index donne l'ordre
    // dans lequel les elements doivent etres lus pour etres classes
    long n = m_nLi;
    long m = m_nCo;

    long j = n - 1;
    bool bSwap;
    do
    {
        bSwap = false;
        for (long i = 0; i < j; i++)
        {
            if ((*this)[Index[i]] > (*this)[Index[i + 1]])
            {
                bSwap = true;
                H3_UINT32 tmp = Index[i];
                Index[i] = Index[i + 1];
                Index[i + 1] = tmp;
            }
        }
        j--;
    } while (bSwap);

    // Modifier l'index pour le faire pointer sur les n°ligne
    Index /= m_nCo;

    // Reorganiser les lignes
    return SortRowsByIndex(Index);
}

///////////////////////////////////////////////////////////////////
// Cette fonction trie les lignes de la matrice dans l'ordre croissant
// des valeurs specifiees sur la colonne nCo en utilisant l'alogorithme
// du ShakerSort
// La fonction retourne false en cas d'echec
template <class TYPE>
bool CH3Array2D<TYPE>::ShakerSortRows(long nCo)
{
    // Creer un tableau contenant les adresses des elements de la colonne
    H3_ARRAY_UINT32 Index(m_nLi);
    {
        for (long i = 0, off = nCo; i < m_nLi; i++, off += m_nCo)
            Index[i] = off;
    }

    // Trier, a la fin de cette operation le vecteur index donne l'ordre
    // dans lequel les elements doivent etres lus pour etres classes
    long n = m_nLi;
    long m = m_nCo;
    long g = 0;
    long d = n - 1;
    long k = 0;
    long i;

    do
    {
        for (i = g; i < d; i++)
        {
            if ((*this)[Index[i]] > (*this)[Index[i + 1]])
            {
                k = i;
                H3_UINT32 tmp = Index[i];
                Index[i] = Index[i + 1];
                Index[i + 1] = tmp;
            }
        }
        d = k;
        for (i = d; i >= g; i--)
        {
            if ((*this)[Index[i]] > (*this)[Index[i + 1]])
            {
                k = i;

                H3_UINT32 tmp = Index[i];
                Index[i] = Index[i + 1];
                Index[i + 1] = tmp;
            }
        }
        g = k + 1;
    } while (g <= d);

    // Modifier l'index pour le faire pointer sur les n°ligne
    Index /= m_nCo;

    // Reorganiser les lignes
    return SortRowsByIndex(Index);
}


///////////////////////////////////////////////////////////////////
// Cette fonction reorganise les lignes selon l'ordre defini dans
// le vecteur Index
// Si Index=3 0 2 1 et This=10 11 alors resultat=40 41 
//                          20 21                10 11
//                          30 31                30 31
//                          40 41			     20 21		 		
template <class TYPE>
bool CH3Array2D<TYPE>::SortRowsByIndex(H3_ARRAY_UINT32& Index)
{
    if (Index.GetSize() != m_nLi)
        return false;

    // Reorganiser les donnees selon l'index
    CH3Array2D<TYPE> TmpBuf = (*this);
    {
        for (long i = 0; i < m_nLi; i++)
        {
            long i1 = Index[i];
            for (long j = 0; j < m_nCo; j++)
            {
                (*this)(i, j) = TmpBuf(i1, j);
            }
        }
    }

    return true;
}

// ///////////////////////////////////////////////////////////////////
// Fonction Pivoter effectuant le pivotage du tableau a[] entre les indices
// start et end. Le pivotage se fait en place, directement dans le tableau a.
// Le pivot est choisi comme le premier element : a[start].
// La fonction retourne la position du pivot après pivotage.
template <class TYPE>
static long QuickSortSwing(H3_ARRAY2D& Src, H3_ARRAY_UINT32& Index, long start, long end)
{
    long m = Src.GetCo();
    long pivot = start, j = end;
    // A chaque étape on cherche à avoir les éléments de start à pivot-1
    // plus petits que le pivot, ceux de j+1 à end plus grands que le pivot,
    // les éléments restants de pivot+1 à j étant à faire pivoter.
    H3_UINT32* pIndexPivot = &Index[pivot];
    H3_UINT32* pIndexJ = &Index[j];
    while (pivot < j)
    {
        pIndexJ = &Index[j];
        if (Src[*pIndexPivot] < Src[*pIndexJ])
        {
            j--;                  // l'élément j est déjà à sa place.
        }
        else
        {
            // échange de a[pivot] et a[j]
            H3_UINT32 tmp = *pIndexPivot;
            *pIndexPivot = *pIndexJ;
            *pIndexJ = tmp;

            // on incrémente la position du pivot
            pivot++;

            pIndexPivot = &Index[pivot];

            // on stocke le pivot en a[pivot+1]
            tmp = *pIndexPivot;
            *pIndexPivot = *pIndexJ;
            *pIndexJ = tmp;
        }
    }
    return pivot;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      static void QuickSort(H3_ARRAY2D &Src,H3_ARRAY_UINT32 &Index,long start,long end)
///	\brief   fonction utilisee par QuickSortRows()
/// \Detail  
/// \Param	 Src 	
/// \Param	 Index 	
/// \Param	 start 	
/// \Param	 end 	
/// \see	 QuickSortRows()
///	\author  E.COLON
/// 
template <class TYPE>
static void QuickSort(H3_ARRAY2D& Src, H3_ARRAY_UINT32& Index, long start, long end)
{
    // a[] contient un seul élément : on a fini !
    if (end <= start) return;

    // pivotage du tableau
    long pivot = QuickSortSwing(Src, Index, start, end);

    // tri réccursif de la partie gauche de a[]
    QuickSort(Src, Index, start, pivot - 1);

    // tri réccursif de la partie droite de a[]
    QuickSort(Src, Index, pivot + 1, end);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3Array2D<TYPE>::QuickSortRows(long nCo)
///	\brief   tri 'QuickSort' sur une colonne
/// \Detail  Cette fonction trie les lignes de la matrice dans l'ordre croissant
///          des valeurs specifiees sur la colonne nCo en utilisant l'alogorithme
///          du QuickSort.
/// \Param	 nCo indice de la colonne a trier 	
///	\retval  true succes
///	\retval  false echec
/// \see	 SortRows()
///	\author  E.COLON
/// 
template <class TYPE>
bool CH3Array2D<TYPE>::QuickSortRows(long nCo)
{
    // Verification
    if ((nCo >= m_nCo) || (nCo < 0))
        return false;

    // Creer un tableau contenant les adresses des elements de la colonne
    H3_ARRAY_UINT32 Index(m_nLi);
    {
        for (long i = 0, off = nCo; i < m_nLi; i++, off += m_nCo)
            Index[i] = off;
    }

    // Trier, a la fin de cette operation le vecteur index donne l'ordre
    // dans lequel les elements doivent etres lus pour etres classes
    ::QuickSort(*this, Index, 0, m_nLi - 1);

    // Modifier l'index pour le faire pointer sur les n°ligne
    Index /= m_nCo;

    // Reorganiser les lignes
    return SortRowsByIndex(Index);
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      bool CH3Array2D<TYPE>::SortRows(long nCo,long nMethod)
///	\brief   tri sur une colonne
/// \Detail  Cette fonction trie les lignes de la matrice dans l'ordre croissant
///          des valeurs specifiees sur la colonne nCo en utilisant l'alogorithme
///          defini par le parametre nMethod.
/// \Param	 nCo indice de la colonne a trier 	
/// \Param   nMethod methode de trie a utiliser
///			   nMethod=0 QuickSort (defaut)
///            nMethod=1 BubleSort
///            nMethod=2 ShakerSort
///	\retval  true succes
///	\retval  false echec
/// \see	 QuickSortRows(),ShakerSortRows()
///	\author  E.COLON
/// 
template <class TYPE>
bool CH3Array2D<TYPE>::SortRows(long nCo, long nMethod)
{
    switch (nMethod)
    {
    case 1:
        return BubbleSortRows(nCo);
        break;
    case 2:
        return ShakerSortRows(nCo);
        break;
    case 0:
    default:
        return QuickSortRows(nCo);
        break;
    }

    return true;
}


/////////////////////////////////////////////////////////////////////////////
///	\fn     bool CH3Array2D<TYPE>::Copy(const TYPE *pSrc,long nSizeX,long nSizeY,long nPitch)
///	\brief  Copie les données pointées par pSrc dans le tableau
/// \Param	pSrc adresse des donnees sources
/// \Param	nCo nombre de colonnes
/// \Param	nLi nombre de lignes
/// \Param	nPitch nombre d'elements effectifs pour passer d'une ligne à
///         l'autre (nPitch>=nCo)
///	\retval  true succes
///	\retval  false echec
///	\author  E.COLON
/// 
template <class TYPE>
bool CH3Array2D<TYPE>::Set(const TYPE* pSrc, size_t nLi, size_t nCo, size_t nPitch)
{
    if (nPitch < nCo)
    {
        Free();
        return false;
    }

    ReAlloc(nLi, nCo);

    for (size_t i = 0; i < nLi; i++)
    {
        TYPE* pD = GetData() + i * m_nCo;
        TYPE* pS = (TYPE*)pSrc + i * nPitch;
        for (size_t j = 0; j < nCo; j++)
        {
            *pD++ = *pS++;
        }
    }

    return true;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn     bool CH3Array2D<TYPE>::Get(const TYPE *pDest,long nLi,long nCo,long nPitch)
///	\brief  Copie les données du tableau vers la zone memoire spécifiée
/// \Param	pDest adresse destination
/// \Param	nCo nombre de colonnes
/// \Param	nLi nombre de lignes
/// \Param	nPitch nombre d'elements effectifs pour passer d'une ligne à
///         l'autre (nPitch>=nCo)
///	\retval  true succes
///	\retval  false echec
///	\author  E.COLON
/// 
template <class TYPE>
bool CH3Array2D<TYPE>::Get(const TYPE* pDest, size_t nLi, size_t nCo, size_t nPitch)
{
    if (nPitch < nCo)
    {
        Free();
        return false;
    }

    size_t nMinLi = __min(nLi, m_nLi);
    size_t nMinCo = __min(nCo, m_nCo);

    TYPE* pSrc = GetData();
    for (size_t i = 0; i < nMinLi; i++)
    {
        TYPE* pD = (TYPE*)pDest + i * nPitch;
        TYPE* pS = (TYPE*)GetData() + i * m_nCo;
        for (size_t j = 0; j < nMinCo; j++)
        {
            *pD++ = *pS++;
        }
    }

    return true;
}


/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY2D sqrt(const H3_ARRAY2D &aSrc)
///	\brief   retourne la racine carree de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D sqrt(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.sqrt();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY2D cos(const H3_ARRAY2D &aSrc)
///	\brief   retourne le cosinus de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D cos(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.cos();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY2D acos(const H3_ARRAY2D &aSrc)
///	\brief   retourne l'arccosinus de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D acos(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.acos();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY2D sin(const H3_ARRAY2D &aSrc)
///	\brief   retourne le sinus de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D sin(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.sin();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY2D sin(const H3_ARRAY2D &aSrc)
///	\brief   retourne l'arcsinus de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D asin(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.asin();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY tan(const H3_ARRAY2D &aSrc)
///	\brief   retourne la tangente de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D tan(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.tan();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY atan(const H3_ARRAY2D &aSrc)
///	\brief   retourne l'arctangente de chaque elements du tableau
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D atan(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.atan();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY2D pow(H3_ARRAY2D &Src,const TYPE Value)
///	\brief   Eleve le contenu du tableau à la puissance spécifiée
/// \details Cette fonction retourne de elements du tableau eleves à la 
///			 puissance 'Value' passe. Les dimensions du tableau retourne sont
///	         identiques à celles du tableau Src.
///	\retval  H3_ARRAY2D aSrc^Value
/// \see	 CH3Array<TYPE>::pow()
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D pow(const H3_ARRAY2D& aSrc, const TYPE Value)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.pow(Value);
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY abs(H3_ARRAY2D &aSrc)
///	\brief   Retourne la valeur absolue
/// \Param	 aSrc tableau d'entree 	
///	\retval  H3_ARRAY valeur absolue de 'aSrc'
/// \see	 CH3Array<TYPE>::abs()
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D abs(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.abs();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY2D fabs(H3_ARRAY2D &aSrc)
///	\brief   Retourne la valeur absolue
/// \Param	 aSrc tableau d'entree 	
///	\retval  H3_ARRAY valeur absolue de 'aSrc'
/// \see	 CH3Array<TYPE>::abs()
///	\author  E.COLON
/// 
template <class TYPE>
H3_ARRAY2D fabs(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest.fabs();
    return aDest;
}

/////////////////////////////////////////////////////////////////////////////
///	\fn      H3_ARRAY2D Floor(const H3_ARRAY2D &aSrc)
///	\brief   retourne la partie entiere de chaque elements du tableau
///	\author  CV 290212
/// 
template <class TYPE>
H3_ARRAY2D Floor(const H3_ARRAY2D& aSrc)
{
    H3_ARRAY2D aDest(aSrc);
    aDest._floor();
    return aDest;
}

#endif

