#ifndef CH3MATRIX__INCLUDED_
#define CH3MATRIX__INCLUDED_
//Class CH3Matrix
//auteur EC:version 100
//modif VC :version 101
// date modif: mars 2004//modifs reportées le 210205
// type modif:	la fct Mat_MeanSquare modifiait la matrice appelante ->> elle ne la modifie plus
//				ajout de fonctions de decomposition en elements singulier Mat_svdcmp, Mat_svbksb Mat_MeanSquare2 pythag
//
#pragma warning( once : 4018 4244) 

#include "H3Array2D.h"
//class CString;//ajout cv 2.1.12 pour compilation mx file matlab

#define H3_MATRIX_FILE_VERSION	101

#define H3_MATRIX			CH3Matrix< TYPE>
// Entiers signes
#define H3_MATRIX_INT8		CH3Matrix< H3_INT8 >
#define H3_MATRIX_INT16		CH3Matrix< H3_INT16 >
#define H3_MATRIX_INT32		CH3Matrix< H3_INT32 >

// Entiers non signes
#define H3_MATRIX_UINT8		CH3Matrix< H3_UINT8 >
#define H3_MATRIX_UINT16	CH3Matrix< H3_UINT16 >
#define H3_MATRIX_UINT32	CH3Matrix< H3_UINT32 >

#define H3_MATRIX_FLT32		CH3Matrix< H3_FLT32 >
#define H3_MATRIX_FLT64		CH3Matrix< H3_FLT64 >
#define H3_MATRIX_FLT80		CH3Matrix< H3_FLT80 >
#define H3_MATRIX_CPXFLT32	CH3Matrix< H3_CPXFLT32 >
#define H3_MATRIX_CPXFLT64	CH3Matrix< H3_CPXFLT64 >

#define CLASS_TYPE template <class TYPE >
#define SQR(a) ((a)*(a))
#define SGN(a) (a)>0 ? 1: ((a)==0 ? 0: -1)
#define SIGN(a,b) ((b) >= 0.0 ? ::fabs(a) : -::fabs(a))


///////////////////////////////////////////////////////////////////////////////////
// class CH3Array2D
CLASS_TYPE class CH3Matrix:
	public H3_ARRAY2D
{  
public:
	// Methodes de copie
  	H3_MATRIX & operator =(const H3_MATRIX & Src);
//	H3_MATRIX & operator =(const CH3Array2D<TYPE> & Src);

	// Conversion de types
	operator H3_MATRIX_FLT32()const;
	operator H3_MATRIX_FLT64()const;
	operator H3_MATRIX_FLT80()const;

	// Operateurs
	H3_MATRIX operator *(const H3_MATRIX & Src)const;//la * matricielle ne se fait pas comme celle d'un Array2D
	H3_MATRIX operator /(const H3_MATRIX & Src)const{return H3_MATRIX((*this).H3_ARRAY2D::operator/(H3_ARRAY2D(Src)));};//CV 210205
	H3_MATRIX operator -(const H3_MATRIX & Src)const{return H3_MATRIX((*this).H3_ARRAY2D::operator-(H3_ARRAY2D(Src)));};//CV 090404
	H3_MATRIX operator +(const H3_MATRIX & Src)const{return H3_MATRIX((*this).H3_ARRAY2D::operator+(H3_ARRAY2D(Src)));};//CV 090404
	//H3_MATRIX&operator =(const H3_MATRIX & Src){return H3_MATRIX((*this).H3_ARRAY2D::operator=(H3_ARRAY2D(Src)));};//CV 130404

	H3_MATRIX operator *(const TYPE t)const{return H3_MATRIX((*this).H3_ARRAY2D::operator*(t));};
	H3_MATRIX operator /(const TYPE t)const{return H3_MATRIX((*this).H3_ARRAY2D::operator/(t));};
	H3_MATRIX operator +(const TYPE t)const{return H3_MATRIX((*this).H3_ARRAY2D::operator+(t));};
	H3_MATRIX operator -(const TYPE t)const{return H3_MATRIX((*this).H3_ARRAY2D::operator-(t));};

    void DiffByColumns();
    void DiffByLines();

    H3_MATRIX transpose()const;
    bool ReSetDims(long nLi, long nCo);
    H3_MATRIX VectorMatrixFromIndiceMatrix(H3_MATRIX& Src);
    H3_MATRIX VectorMatrixFromLineColumsMatrixes(H3_MATRIX& Li, H3_MATRIX& Co);

    bool fLoadBIN(FILE* Stream);
    bool fSaveBIN(FILE* Stream);

    bool fLoadHBF(FILE* Stream);
    bool fSaveHBF(FILE* Stream);

	H3_MATRIX Trans()const;
	H3_MATRIX Inv()const;
	H3_MATRIX Inv2()const;
	TYPE CoFact(long i,long j)const;
	TYPE Det()const;
	TYPE DiagMul()const;
	TYPE Minor(long i,long j)const;
	H3_MATRIX SubMat(long i,long j)const;
    H3_MATRIX SubMat(long i, long j, long iNum, long jNum)const;
    //choleski decomposition pour matrice symetrique definie positive
	H3_MATRIX choldc(bool cond=true,H3_MATRIX& p=H3_MATRIX()) const;
	H3_MATRIX cholsl(const H3_MATRIX& b,const H3_MATRIX& p) const;
	H3_MATRIX chols_inv1(const H3_MATRIX& p) const;
	H3_MATRIX chols_inv() const;
	H3_MATRIX inv_chols() const;
	H3_MATRIX inv_chols1() const;

	//
	bool SVDcmp(H3_MATRIX& U,H3_MATRIX& W,H3_MATRIX& V)const;
	TYPE Norm()const;
	TYPE Cond()const;
	H3_MATRIX Rodrigues(H3_MATRIX& dout=H3_MATRIX(0,0))const;
	H3_MATRIX reshape(size_t L,size_t C){return H3_MATRIX((*this).H3_ARRAY2D::reshape(L,C));};
	H3_MATRIX eye(void)const;
	// Constructeurs/Destructeurs
	CH3Matrix(size_t NbCo):H3_ARRAY2D(1,NbCo){};
	CH3Matrix(const H3_ARRAY2D & src):H3_ARRAY2D(src) {};
	CH3Matrix(const CH3Matrix & src):H3_ARRAY2D(src) {};
	CH3Matrix(size_t NbLi,size_t NbCo):H3_ARRAY2D(NbLi,NbCo){};
	CH3Matrix():H3_ARRAY2D(){};
	virtual ~CH3Matrix() {};

private:
    int GetSize_HBF(FILE* stream, size_t& sizeX, size_t& sizeY);
    int GetSize_BIN(FILE* stream, size_t& sizeX, size_t& sizeY);
};
//utilitaires
CLASS_TYPE TYPE pythag( TYPE a, TYPE b);
CLASS_TYPE H3_MATRIX Mat_svbksb(const H3_MATRIX& U,const H3_MATRIX& W,const H3_MATRIX& V,const H3_MATRIX& B);	
CLASS_TYPE H3_MATRIX Mat_MeanSquare2(const H3_MATRIX& A,const H3_MATRIX& B);
CLASS_TYPE H3_MATRIX Mat_MeanSquare_chols(const H3_MATRIX &A,const H3_MATRIX &B);
CLASS_TYPE	H3_MATRIX	Mat_Cross(const H3_MATRIX& U,const H3_MATRIX& V);
CLASS_TYPE	H3_MATRIX	Mat_Dot(const H3_MATRIX& U,const H3_MATRIX& V);
//H3_MATRIX_FLT64	Mat_eye(unsigned long n);
//H3_MATRIX_FLT32	Mat_eye(unsigned long n);
//modif cv 2.1.12 pour compilateur mx file matlab//void WinAffiche(const H3_MATRIX_FLT64 M, const CString title=_T(""));
void WinAffiche(const H3_MATRIX_FLT64 M, CString title);

CLASS_TYPE H3_MATRIX Mat_lSolve_Durbin(const H3_MATRIX &A,const H3_MATRIX &B );
CLASS_TYPE H3_MATRIX Mat_Durbin(const H3_MATRIX & R );
CLASS_TYPE H3_MATRIX Mat_BackSubs1(const H3_MATRIX &A, H3_MATRIX &B, H3_MATRIX &X,const H3_MATRIX &P,long xcol);
CLASS_TYPE H3_MATRIX Mat_SymToeplz(const H3_MATRIX &R );
CLASS_TYPE long Mat_Lu( H3_MATRIX & A, H3_MATRIX & P);
CLASS_TYPE H3_MATRIX Mat_XTransX(const H3_MATRIX & X);
CLASS_TYPE H3_MATRIX Mat_XTransY(const H3_MATRIX &X,const H3_MATRIX &Y);
CLASS_TYPE H3_MATRIX Mat_XXTrans(const H3_MATRIX & X) ;
CLASS_TYPE H3_MATRIX Mat_XYTrans(const H3_MATRIX &X,const H3_MATRIX &Y);
CLASS_TYPE H3_MATRIX Mat_MeanSquare(const H3_MATRIX &A,const H3_MATRIX &B);
CLASS_TYPE H3_MATRIX Mat_lSolve(const H3_MATRIX &A,const H3_MATRIX &B );
CLASS_TYPE H3_MATRIX Mat_Mul(const H3_MATRIX & A, const H3_MATRIX & B);
CLASS_TYPE H3_MATRIX Mat_Trans(const H3_MATRIX & A);
CLASS_TYPE H3_MATRIX Mat_Inv(const H3_MATRIX & A);
CLASS_TYPE H3_MATRIX Mat_Inv2(const H3_MATRIX & A);
CLASS_TYPE TYPE Mat_DiagMul(const H3_MATRIX & A);
CLASS_TYPE TYPE Mat_CoFact(const H3_MATRIX & A,const long i,const long j);
CLASS_TYPE TYPE Mat_Minor(const H3_MATRIX & A,const long i,const long j);
CLASS_TYPE TYPE Mat_Det(const H3_MATRIX & Src);
CLASS_TYPE H3_MATRIX Mat_SubMat(const H3_MATRIX & A,const size_t i,const size_t j);
CLASS_TYPE H3_MATRIX Mat_CopyCo(const H3_MATRIX & A,const size_t j);
CLASS_TYPE H3_MATRIX Mat_CopyLi(const H3_MATRIX & A,const size_t i);



// Operateur de copie, voir egalement fonction Copy
/*
CLASS_TYPE 
H3_MATRIX & H3_MATRIX::operator =(const CH3Array2D<TYPE> & Src)
{
	if (this==&Src) return *this;

	Copy(Src);

	return *this;
}
*/

CLASS_TYPE H3_MATRIX H3_MATRIX::VectorMatrixFromLineColumsMatrixes(H3_MATRIX& Li, H3_MATRIX& Co)
{
    H3_MATRIX resultMatrix(Li.GetSize(), Co.GetSize());
    for (long i = 0; i < Li.GetSize(); i++)
    {
        for (long j = 0; j < Co.GetSize(); j++)
        {
            resultMatrix(i, j) = (*this)((long)Li(i), (long)Co(j));
        }
    }
    return(resultMatrix);
}

CLASS_TYPE H3_MATRIX H3_MATRIX::VectorMatrixFromIndiceMatrix(H3_MATRIX& Src)
{
    H3_MATRIX resultMatrix((*this).GetSize(), 1);
    for (long i = 0; i < Src.GetSize(); i++)
    {
        resultMatrix(i) = (*this)((long)Src(i));
    }
    return(resultMatrix);
}

CLASS_TYPE
bool H3_MATRIX::ReSetDims(long nLi, long nCo)
{
    m_nLi = nLi;
    m_nCo = nCo;
    return(true);
}

CLASS_TYPE H3_MATRIX H3_MATRIX::transpose()const
{
    H3_MATRIX resultMatrix(m_nCo, m_nLi);
    unsigned long i, j;
    for (i = 0; i < m_nLi; i++)
    {
        for (j = 0; j < m_nCo; j++)
        {
            resultMatrix(j, i) = (*this)(i, j);
        }
    }
    return(resultMatrix);
}

CLASS_TYPE
void CH3Matrix<TYPE>::DiffByColumns()
{
    // A(diffcol(n)) = A(Col(n+1)) - A(Col(n))
    //
    if (m_pData != nullptr && m_nLi * m_nCo > 0)
    {
        for (unsigned long i = 0; i < m_nLi; i++)
        {
            for (unsigned long j = 0; j < m_nCo; j++)
            {
                long lidx = i * m_nCo; // index line
                if (j == m_nCo - 1) // dernière col
                {
                    m_pData[lidx + j] = (TYPE)0;
                }
                else
                {
                    //A(diffcol(n)) = A(Col(n+1)) - A(Col(n))
                    m_pData[lidx + j] = m_pData[lidx + j + 1] - m_pData[lidx + j];
                }

            }
        }
    }
}

CLASS_TYPE
void CH3Matrix<TYPE>::DiffByLines()
{
    // A(diffLine(n)) = A(Line(n+1)) - A(Line(n))
    //
    if (m_pData != nullptr && m_nLi * m_nCo > 0)
    {
        for (unsigned long i = 0; i < m_nLi; i++)
        {
            for (unsigned long j = 0; j < m_nCo; j++)
            {
                long lidx = i * m_nCo; // index line
                if (i == m_nLi - 1) // dernière line
                {
                    m_pData[lidx + j] = (TYPE)0;
                }
                else
                {
                    // A(diffLine(n)) = A(Line(n+1)) - A(Line(n))
                    m_pData[lidx + j] = m_pData[lidx + m_nCo + j] - m_pData[lidx + j];
                }

            }
        }
    }
}

CLASS_TYPE
bool CH3Matrix<TYPE>::fLoadBIN(FILE* Stream)
{
    GetSize_BIN(Stream, m_nLi, m_nCo);
    Alloc(m_nLi, m_nCo);
    if (Stream)
    {
        if (m_nLi * m_nCo == 0)
        {
            return true;
        }

        if (m_nLi * m_nCo > 0)
        {

            if (fread(m_pData, sizeof(TYPE), m_nLi * m_nCo, Stream) == m_nLi * m_nCo)
            {
                return true;
            }
        }
    }
    return false;
}

CLASS_TYPE
bool CH3Matrix<TYPE>::fSaveBIN(FILE* stream)
{
    if (stream)
    {
        ::fwrite(&m_nCo, sizeof(int), 1, stream);
        ::fwrite(&m_nLi, sizeof(int), 1, stream);
        if (m_nLi * m_nCo == 0)
        {
            return true;
        }

        if (::fwrite(m_pData, sizeof(TYPE), m_nLi * m_nCo, stream) == m_nLi * m_nCo)
        {
            return true;
        }
    }
    return false;
}

CLASS_TYPE
int CH3Matrix<TYPE>::GetSize_HBF(FILE* stream, size_t& sizeX, size_t& sizeY)
{
    char temp;
    fpos_t endStringPosition;
    fpos_t streamOrigin = 0;

    fgetpos(stream, &endStringPosition);
    fread(&temp, 1, 1, stream);
    bool bNoReadErr = true;
    while (temp != '\0' && bNoReadErr)
    {
        bNoReadErr = (0 != fread(&temp, sizeof(char), 1, stream));
    }

    /*#ifdef _DEBUG
        fgetpos(stream,&endStringPosition);
        char * pcTypeval = new char[endStringPosition];
        fsetpos(stream,&streamOrigin);
        fread(pcTypeval,sizeof(char),endStringPosition,stream);
        CString  test;
        test.Format("%s",pcTypeval);
        delete [] pcTypeval;
    #endif*/

    //On connaît la longueur de chaine à récupérer/exclure
    long dummy;
    fread(&dummy, sizeof(unsigned int), 1, stream);
    fread(&sizeY, sizeof(unsigned int), 1, stream);
    fread(&sizeX, sizeof(unsigned int), 1, stream);

    return(0);
}

CLASS_TYPE
int CH3Matrix<TYPE>::GetSize_BIN(FILE* stream, size_t& sizeX, size_t& sizeY)
{
    size_t t;

    //On connaît la longueur de chaine à récupérer/exclure
    fread(&sizeY, sizeof(int), 1, stream);
    fread(&sizeX, sizeof(int), 1, stream);
    return(0);
}

CLASS_TYPE
bool CH3Matrix<TYPE>::fLoadHBF(FILE* Stream)
{
    GetSize_HBF(Stream, m_nCo, m_nLi);
    Alloc(m_nLi, m_nCo);
    if (Stream)
    {
        if (m_nLi * m_nCo == 0)
        {
            return true;
        }

        if (m_nLi * m_nCo > 0)
        {
            size_t ttt = sizeof(TYPE);
            size_t ss = sizeof(float);
            if (fread(m_pData, sizeof(TYPE), m_nLi * m_nCo, Stream) == m_nLi * m_nCo)
            {
                return true;
            }
        }
    }
    return false;
}

CLASS_TYPE
bool CH3Matrix<TYPE>::fSaveHBF(FILE* stream)
{
    if (stream)
    {
        //HEADER :
        //example if TYPE = float =:
        //class CH3Array2D<float> *
        const char* cTypename = typeid(TYPE).name();
        CString csHeader;
        csHeader.Format(_T("class CH3Array2D<%s> *"), LPCSTR(typeid(TYPE).name()));

        ::fwrite(csHeader.GetBuffer(), sizeof(char), csHeader.GetLength(), stream);
        csHeader.ReleaseBuffer();
        // write null terminating char
        ::fwrite(_T("\0"), sizeof(char), 1, stream);
        long dummy = 100;
        ::fwrite(&dummy, sizeof(unsigned int), 1, stream);
        ::fwrite(&m_nLi, sizeof(unsigned int), 1, stream);
        ::fwrite(&m_nCo, sizeof(unsigned int), 1, stream);

        if (m_nLi * m_nCo == 0)
        {
            return true;
        }

        if (::fwrite(m_pData, sizeof(TYPE), m_nLi * m_nCo, stream) == m_nLi * m_nCo)
        {
            return true;
        }
    }
    return false;
}

CLASS_TYPE H3_MATRIX & H3_MATRIX::operator =(const H3_MATRIX & Src)
{
	if (this==&Src) return *this;

	Copy(Src);

	return *this;
}


//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur de conversion de types
//////////////////////////////////////////////////////////////////////////////////////
// Conversion vers H3_FLT32
CLASS_TYPE H3_MATRIX::operator H3_MATRIX_FLT32()const 
{
	H3_MATRIX_FLT32 Temp(m_nLi,m_nCo);
	TYPE *pSrc=m_pData;
	H3_FLT32 *pDest=Temp.GetData();

	unsigned long i=m_nSize;
	while (i--)
		*pDest++=(H3_FLT32)(*pSrc++);

	return Temp;
}

// Conversion vers H3_FLT64
CLASS_TYPE H3_MATRIX::operator H3_MATRIX_FLT64()const 
{
	H3_MATRIX_FLT64 Temp(m_nLi,m_nCo);
	TYPE *pSrc=m_pData;
	H3_FLT64 *pDest=Temp.GetData();

	unsigned long i=m_nSize;
	while (i--)
		*pDest++=(H3_FLT64)(*pSrc++);

	return Temp;
}

// Conversion vers H3_FLT80
CLASS_TYPE H3_MATRIX::operator H3_MATRIX_FLT80()const 
{
	H3_MATRIX_FLT80 Temp(m_nLi,m_nCo);
	TYPE *pSrc=m_pData;
	H3_FLT80 *pDest=Temp.GetData();

	unsigned long i=m_nSize;
	while (i--)
		*pDest++=(H3_FLT80)(*pSrc++);

	return Temp;
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques
//////////////////////////////////////////////////////////////////////////////////////
CLASS_TYPE H3_MATRIX H3_MATRIX::operator *(const  H3_MATRIX & Src)const
{
	return Mat_Mul((*this),Src);
}

//////////////////////////////////////////////////////////////////////////////////////
// Methodes de calcul matriciel
//////////////////////////////////////////////////////////////////////////////////////
CLASS_TYPE H3_MATRIX H3_MATRIX::Trans()const 
{
	return Mat_Trans((*this));
}

CLASS_TYPE H3_MATRIX H3_MATRIX::Inv()const 
{
	return Mat_Inv((*this));
}

CLASS_TYPE H3_MATRIX H3_MATRIX::Inv2()const 
{
	return Mat_Inv2((*this));
}

CLASS_TYPE TYPE H3_MATRIX::DiagMul()const 
{
	return Mat_DiagMul((*this));
}

CLASS_TYPE TYPE H3_MATRIX::CoFact(long i,long j)const 
{
	return Mat_CoFact((*this),i,j);
}

CLASS_TYPE TYPE H3_MATRIX::Minor(long i,long j)const 
{
	return Mat_Minor((*this),i,j);
}

CLASS_TYPE TYPE H3_MATRIX::Det()const 
{
	return Mat_Det((*this));
}

CLASS_TYPE H3_MATRIX H3_MATRIX::SubMat(long i,long j)const 
{
	return Mat_SubMat((*this),i,j);
}

CLASS_TYPE H3_MATRIX H3_MATRIX::SubMat(long i, long j, long iNum, long jNum)const
{
    long xNumElts = 0;
    long yNumElts = 0;

    //calcul du nombre d'éléments demandés

    if (jNum < 0)
    {
        xNumElts = m_nCo;
    }
    else
    {
        xNumElts = jNum;
    }

    if (iNum < 0)
    {
        yNumElts = m_nLi;
    }
    else
    {
        yNumElts = iNum;
    }

    //déclaration de la matrice demandée 
    H3_MATRIX resultMatrix(yNumElts, xNumElts);

    for (int yIndex = i; yIndex < i + yNumElts; yIndex++)
    {
        for (int xIndex = j; xIndex < j + xNumElts; xIndex++)
        {
            resultMatrix(yIndex - i, xIndex - j) = (*this)(yIndex, xIndex);
        }
    }

    return(resultMatrix);
}

/////////////////////////////////////////////////////////////////////////////
// Fonctions
/////////////////////////////////////////////////////////////////////////////

/*
*-----------------------------------------------------------------------------
*	funct:	mat_backsubs1
*	desct:	back substitution
*	given:	A = square matrix A (LU composite)
*		!! B = column matrix B (attention!, see comen)
*		!! X = place to put the result of X
*		P = Permutation vector (after calling mat_lu)
*		xcol = column of x to put the result
*	retrn:	column matrix X (of AX = B)
*	comen:	B will be overwritten
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_BackSubs1(const H3_MATRIX &A, H3_MATRIX &B, H3_MATRIX &X ,const H3_MATRIX &P,long xcol)
{
    long i, j, k;
    size_t n;
	TYPE sum;
	
	long Pk0,Pi0;//cv250308
	TYPE B_Pk0_0;

	n = A.GetCo();	

	for (k=0; k<n; k++)
	{
		Pk0=(long)P(k,0);
		B_Pk0_0=B(Pk0,0);
		for (i=k+1; i<n; i++)
		{
			Pi0=(long)P(i,0);
			B(Pi0,0) -= A(Pi0,k) * B_Pk0_0;
		}
	}

    X(n-1,xcol) = B((long)P(n-1,0),0) / A((long)P(n-1,0),n-1);
	for (k=n-2; k>=0; k--)
	{
		sum = 0.0;
		Pk0=(long)P(k,0);
		for (j=k+1; j<n; j++)
		{
			sum += A(Pk0,j) * X(j,xcol);
		}
		X(k,xcol) = (B(Pk0,0) - sum) / A(Pk0,k);
	}

/*	n = A.GetCo();

	for (k=0; k<n; k++)
	{
		for (i=k+1; i<n; i++)
			B((long)P(i,0),0) -= A((long)P(i,0),k) * B((long)P(k,0),0);
	}

    X(n-1,xcol) = B((long)P(n-1,0),0) / A((long)P(n-1,0),n-1);
	for (k=n-2; k>=0; k--)
	{
		sum = 0.0;
		for (j=k+1; j<n; j++)
		{
			sum += A((long)P(k,0),j) * X(j,xcol);
		}
		X(k,xcol) = (B((long)P(k,0),0) - sum) / A((long)P(k,0),k);
	}*/

	return (X);
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_lsolve_durbin
*	desct:	Solve simultaneous linear eqns using
*		Levinson-Durbin algorithm
*
*		This function solve the linear eqns Ax = B:
*
*		|  v0   v1   v2  .. vn-1 | |  a1   |    |  v1   |
*		|  v1   v0   v1  .. vn-2 | |  a2   |    |  v2   |
*		|  v2   v1   v0  .. vn-3 | |  a3   |  = |  ..   |
*		|  ...                   | |  ..   |    |  ..   |
*		|  vn-1 vn-2 ..  .. v0   | |  an   |    |  vn   |
*
*	domain:	where A is a symmetric Toeplitz matrix and B
*		in the above format (related to A)
*
*	given:	A, B
*	retrn:	x (of Ax = B)
*
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_lSolve_Durbin(const H3_MATRIX &A,const H3_MATRIX &B ) 
{
	long n = A.GetLi();
	H3_MATRIX R(n+1,1);
	
	for (long i=0; i<n; i++)
	{
		R(i,0) = A(i,0);
	}
	R(n,0) = B(n-1,0);

	return Mat_Durbin(R);
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_durbin
*	desct:	Levinson-Durbin algorithm
*
*		This function solve the linear eqns Ax = B:
*
*		|  v0   v1   v2  .. vn-1 | |  a1   |    |  v1   |
*		|  v1   v0   v1  .. vn-2 | |  a2   |    |  v2   |
*		|  v2   v1   v0  .. vn-3 | |  a3   |  = |  ..   |
*		|  ...                   | |  ..   |    |  ..   |
*		|  vn-1 vn-2 ..  .. v0   | |  an   |    |  vn   |
*
*		where A is a symmetric Toeplitz matrix and B
*		in the above format (related to A)
*
*	given:	R = autocorrelated matrix (v0, v1, ... vn) (dim (n+1) x 1)
*	retrn:	x (of Ax = B)
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_Durbin(const H3_MATRIX & R )
{
	long p = R.GetLi() - 1;
	H3_MATRIX W(p+2,1);
	H3_MATRIX E(p+2,1);
	H3_MATRIX K(p+2,1);
	H3_MATRIX A(p+2,p+2,);

	W(0,0) = R(1,0);
	E(0,0) = R(0,0);

	for (long i=1; i<=p; i++)
	{
		K(i,0) = W(i-1,0) / E(i-1,0);
		E(i,0) = E(i-1,0) * (1.0 - K(i,0) * K(i,0));

		A(i,i) = -K(i,0);

		long i1 = i-1;
		if (i1 >= 1)
		{
			for (long j=1; j<=i1; j++)
			{
				long ji = i - j;
				A(j,i) = A(j,i1) - K(i,0) * A(ji,i1);
			}
		}

		if (i != p)
		{
			W(i,0) = R(i+1,0);
			for (long j=1; j<=i; j++)
				W(i,0) += A(j,i) * R(i-j+1,0);
		}
	}

	H3_MATRIX X(p,1);
	for (i=0; i<p; i++)
	{
		X(i,0) = -A(i+1,p);
	}

	return (X);
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_SymToeplz
*	desct:	create a n x n symmetric Toeplitz matrix from
*		a n x 1 correlation matrix
*	given:	R = correlation matrix (n x 1)
*	retrn:	the symmetric Toeplitz matrix
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_SymToeplz(const H3_MATRIX &R )
{
	long n = R.GetLi();
	H3_MATRIX T(n,n);

	for (long i=0; i<n; i++)
		for (long j=0; j<n; j++)
		{
			T(i,j) = R(abs(i-j),0);
		}
	return (T);
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_lu
*	desct:	in-place LU decomposition with partial pivoting
*	given:	!! A = square matrix (n x n) !ATTENTION! see commen
*		P = permutation vector (n x 1)
*	retrn:	number of permutation performed
*		-1 means suspected singular matrix
*	comen:	A will be overwritten to be a LU-composite matrix
*
*	note:	the LU decomposed may NOT be equal to the LU of
*		the orignal matrix a. But equal to the LU of the
*		rows interchanged matrix.
*-----------------------------------------------------------------------------
*/
CLASS_TYPE long Mat_Lu( H3_MATRIX &A, H3_MATRIX &P)
{
	long i, j, k, n;
    long maxi;
	TYPE c, c1,tmp;
	long p;
	long Pk0,Pi0;//cv250308
	TYPE A_Pi0_k;

	n = A.GetCo();

	for (p=0,i=0; i<n; i++)
	{
		P(i,0)=(TYPE)i;
	}

	for (k=0; k<n; k++)
	{
	// --- partial pivoting ---
		for (i=k, maxi=k, c=0.0; i<n; i++)
		{
			c1 = ::fabs( A( (long)P(i,0) , k) );
			if (c1 > c)
			{
				c = c1;
				maxi = i;
			}
		}

		//	row exchange, update permutation vector
		if (k != maxi)
		{
			p++;
			tmp = P(k,0);
			P(k,0) = P(maxi,0);
			P(maxi,0) = tmp;
		}

		// suspected singular matrix
	    if ( A((long)P(k,0),k) == 0.0 )
			return -1;

		Pk0=(long)P(k,0);
		for (i=k+1; i<n; i++)
		{
			Pi0=(long)P(i,0);
			// --- calculate m(i,j) ---
			A(Pi0,k) /= A(Pk0,k);//cv250308
			//A((long)P(i,0),k) = A((long)P(i,0),k)/A((long)P(k,0),k);

			A_Pi0_k=A(Pi0,k);

			// --- elimination ---
			for (j=k+1;j<n; j++)
			{
				A(Pi0,j) -= A_Pi0_k * A(Pk0,j);//cv250308
				//A((long)P(i,0),j) -= A((long)P(i,0),k) * A((long)P(k,0),j);
			}
		}
	}

	return p;
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_backsubs1
*	desct:	back substitution
*	given:	A = square matrix A (LU composite)
*		!! B = column matrix B (attention!, see comen)
*		!! X = place to put the result of X
*		P = Permutation vector (after calling mat_lu)
*		xcol = column of x to put the result
*	retrn:	column matrix X (of AX = B)
*	comen:	B will be overwritten
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_BackSubs1(const H3_MATRIX &A,const H3_MATRIX &B,const H3_MATRIX &X,const H3_MATRIX &P,int xcol)
{
	long i, j, k, n;
	TYPE sum;
	long Pk0;

	n = A.GetCo();

	for (k=0; k<n; k++)
	{
		Pk0=(long)P(k,0);
		for (i=k+1; i<n; i++)
			B((long)P(i,0),0) -= A((long)P(i,0),k) * B(Pk0,0);
	}

    X(n-1,xcol) = B((long)P(n-1,0),0) / A((long)P(n-1,0),n-1);
	for (k=n-2; k>=0; k--)
	{
		sum = 0.0;
		Pk0=(long)P(k,0);
		for (j=k+1; j<n; j++)
		{
			sum += A(Pk0,j) * X(j,xcol);
		}
		X(k,xcol) = (B(Pk0,0) - sum) / A(Pk0,k);
	}

	return (X);
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_lsolve
*	desct:	solve linear equations
*	given:	a = square matrix A
*		b = column matrix B
*	retrn:	column matrix X (of AX = B)
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_lSolve(const H3_MATRIX &a,const H3_MATRIX &b )
{
	long n=a.GetCo();
	H3_MATRIX A(a);
	H3_MATRIX B(b);
	H3_MATRIX X(n,1);X.Fill(0);
	H3_MATRIX P(n,1);

	if (Mat_Lu(A, P)==-1)
	{
		printf("Attention matrice singuliere\n");
	}
	Mat_BackSubs1(A,B,X,P,0);

	return (X);
}

/*
Resolution AX=B par la methode des moindres carres
Calcul X=INV(At*A)*At*B
avec
A[n,m]
B[n,1]
X[m,1]
*/
//CLASS_TYPE H3_MATRIX Mat_MeanSquare(H3_MATRIX &A,H3_MATRIX &B)
CLASS_TYPE H3_MATRIX Mat_MeanSquare(const H3_MATRIX &A0,const H3_MATRIX &B)//modif cv 210205
{
	H3_MATRIX A(A0);//evite que la matrice entrée ne soit modifiée
	H3_MATRIX C(1,A.GetCo());C.Fill(0);
	TYPE Cj,Ci;
	{
		long i,j;
		// Chercher les coefs par lesquels diviser chaque colonne de A
		for (j=0;j<A.GetCo();j++)
		{

			for (i=0;i<A.GetLi();i++)				
				C[j]=__max(::fabs(A(i,j)),C[j]);
		}
	
		// Diviser chaque colonne par le coef trouve
		for (j=0;j<A.GetCo();j++){
			Cj=C[j];
			for (i=0;i<A.GetLi();i++)
				A(i,j)/=Cj;
		}
	}

	// Calculer AT*A
	H3_MATRIX ATA=Mat_XTransX(A);

	// Calculer INV(AT*A)
	H3_MATRIX ATAI=Mat_Inv(ATA);

	if (ATAI.IsEmpty())
		return H3_MATRIX(0,0);

	// Calculer PT*Z
	ATA=Mat_XTransY(A,B);

	// Calculer INV(AT*A)*AT*B
	H3_MATRIX S=ATAI*ATA;

	// Corriger chaque element de S avec le coef trouve precedemment
	{
		long i,j;
		for (i=0;i<S.GetLi();i++){
			Ci=C[i];
			for (j=0;j<S.GetCo();j++)
				S(i,j)/=Ci;
		}
	}

	return S;
}

// X trans * X
CLASS_TYPE H3_MATRIX Mat_XTransX(const H3_MATRIX & X)
{
	size_t  n=X.GetLi();
	size_t  m=X.GetCo();

	H3_MATRIX XTX(m,m);

	//modif cv nov05: apres test c'est plus rapide dans ce sens
	XTX.Fill(0);
	TYPE Xki;
	for (size_t  k=0;k<n;k++){	
		for (size_t  i=0;i<m;i++){		
			Xki=X(k,i);
			for (size_t  j=0;j<=i;j++){			
				XTX(i,j) += (Xki * X(k,j));
			}
		}
	}
	//et en utilisant la symetrie du résultat
	for (size_t  i=0;i<m;i++){
		for (size_t  j=0;j<i;j++){
			XTX(j,i)=XTX(i,j);
		}
	}

	return XTX;
}

// X trans * Y
CLASS_TYPE H3_MATRIX Mat_XTransY(const H3_MATRIX &X,const H3_MATRIX &Y)
{
	const size_t Xli=X.GetLi(),Yli=Y.GetLi();
	if (Xli!=Yli)
		return H3_MATRIX(0,0);

	const size_t Xco=X.GetCo(),Yco=Y.GetCo();
	H3_MATRIX XTY(Xco,Yco);

	//modif cv nov05: apres test c'est plus rapide dans ce sens
	XTY.Fill(0);
	TYPE *pX=X.GetData();
	TYPE *pY,*pY0=Y.GetData();
	TYPE *pXTY,*pXTY0=XTY.GetData();

	size_t i,j,k;

	for (k=0;k<Xli;k++){	
		pXTY=pXTY0;
		for (i=0;i<Xco;i++){		
			//Xki=X(k,i);
			pY=pY0;
			for (j=0;j<Yco;j++){			
				//XTY(i,j) += (Xki * Y(k,j));
				(*(pXTY++)) += (*pX)*(*(pY++));
			}
			pX++;
		}
		pY0=pY;
	}

	return XTY;
}
// X  * Xtrans
CLASS_TYPE H3_MATRIX Mat_XXTrans(const H3_MATRIX & X)
{
	size_t n=X.GetLi();
	size_t m=X.GetCo();
	size_t i,j,k;

	H3_MATRIX XXT(n,n);

	XXT.Fill(0);
	TYPE *pXik0=(double*)X.GetData(), *pXjk0=(double*)X.GetData();
	TYPE *pXik,*pXjk;
	
	for (i=0;i<n;i++){	
		pXjk   = pXjk0;
		for (j=0;j<=i;j++){
			pXik=pXik0;
			for (k=0;k<m;k++){						
				XXT(i,j) += (*(pXik++)) * (*(pXjk++));
			}
		}		
		pXik0 += m;
	}
	//et en utilisant la symetrie du résultat
	for (i=0;i<n;i++){
		for (j=0;j<i;j++){
			XXT(j,i)=XXT(i,j);
		}
	}

	return XXT;
}

// X  * Ytrans
CLASS_TYPE H3_MATRIX Mat_XYTrans(const H3_MATRIX &X,const H3_MATRIX &Y)
{
	CString strFunction("Mat_XYTrans");

	size_t i,j,k;
	size_t Xco=X.GetCo(),Xli=X.GetLi(),Yco=Y.GetCo(),Yli=Y.GetLi();

	if (Xco!=Yco){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
		H3DebugError("",strFunction,"Xco!=Yco");
		#else
			#if H3_CHECKALL_MODE
				AfxMessageBox("Mat_XYTrans Xco!=Yco");
			#endif
		#endif
		return H3_MATRIX(0,0);
	}
	
	H3_MATRIX XYT(Xli,Yli);

	XYT.Fill(0);
	TYPE *pXik,*pXik0=(double*)X.GetData();
	TYPE *pYjk,*pYjk0=(double*)Y.GetData();
	TYPE *pXYT=XYT.GetData();
	
	for (i=0;i<Xli;i++){
		pYjk=pYjk0;
		for (j=0;j<Yli;j++){
			pXik=pXik0;
			for (k=0;k<Xco;k++){			
				(*pXYT) += (*(pXik++)) * (*(pYjk++));
			}
			pXYT++;
		}		
		pXik0 += Xco;
	}

	return XYT;
}

CLASS_TYPE H3_MATRIX Mat_Mul(const H3_MATRIX & A,const H3_MATRIX & B)
{
#if H3_CHECKALL_MODE
	if(A.GetCo()!=B.GetLi()){
		CString msg;
		msg.Format("MATRIX::Mat_Mul (MATRIX): attention: tableaux de taille inappropriée");
		AfxMessageBox(msg);
	}
#endif
	const size_t Ali=A.GetLi(),Aco=A.GetCo(),Bli=B.GetLi(),Bco=B.GetCo();////modif cv nov05: apres test c'est plus rapide en utilisant des variables

	size_t i,j,k;
		
	//modif cv nov05: apres test c'est plus rapide
	H3_MATRIX C(Ali,Bco);
	C.Fill(0);
	TYPE Aik;

	//encore plus rapide comme ca
	TYPE *pA=A.GetData(),*pB=B.GetData(),*pC=C.GetData(),*pB0=B.GetData(),*pCi=C.GetData();
	for (i=0; i<Ali; i++){
		pB=pB0;
		for (k=0; k<Aco; k++){
			Aik=(*(pA++));			
			pC=pCi;
			for (j=0; j<Bco; j++){						
				(*(pC++)) += Aik*(*(pB++));
			}
		}
		pCi=pC;
	}
	return (C);
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_trans
*	desct:	transpose of a matrix
*	given:	A = matrix A to be transposed
*	retrn:	allocated matrix for A^t
*	comen:
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_Trans(const H3_MATRIX & A)
{
	size_t nbli=A.GetLi();
	size_t nbco=A.GetCo();

	H3_MATRIX At(nbco,nbli);

	for (size_t i=0; i<nbco; i++)
		for (size_t j=0; j<nbli; j++)
			At(i,j) = A(j,i);

	return (At);
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_inv
*	desct:	find inverse of a matrix
*	given:	a = square matrix a
*	retrn:	square matrix Inverse(A)
*		NULL = fails, singular matrix, or malloc() fails
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_Inv(const H3_MATRIX & Src)
{
	size_t n = Src.GetCo();
	H3_MATRIX A(Src);
	H3_MATRIX B(n,1);
	H3_MATRIX C(n,n);
	H3_MATRIX P(n,1);
	
	//	- LU-decomposition -
	//	also check for singular matrix
	if (Mat_Lu(A, P) == -1)
	{
		return H3_MATRIX(0,0);
	}

	for (size_t i=0; i<n; i++)
	{
		B.Fill(0);
		B(i,0) = 1.0;
		Mat_BackSubs1( A, B, C, P, i );
	}

	return (C);
}

CLASS_TYPE H3_MATRIX Mat_Inv2(const H3_MATRIX & Src)
{
	const size_t MatVal=Src.GetLi();
	if(Src.GetCo()!=MatVal)
		return(H3_MATRIX(0,0));
	size_t i,j,k;
	//1 normalisation
	H3_MATRIX_FLT64 MatMaxCo(1,MatVal),MatMaxLi(MatVal,1);
	MatMaxCo.Fill(0.0);
	MatMaxLi.Fill(0.0);

	TYPE *pSrc=Src.GetData();
	for(i=0L,k=0L;i<MatVal;i++){
		for(j=0L;j<MatVal;j++,k++){
			MatMaxCo[j]=__max(MatMaxCo[j],fabs(*pSrc));
			MatMaxLi[i]=__max(MatMaxLi[i],fabs(*pSrc));
		}
		pSrc++;
	}
	MatMaxCo.sqrt();
	MatMaxLi.sqrt();
	
	H3_MATRIX_FLT64 Src_tmp(Src);
	for(i=0L;i<MatVal;i++)
		for(j=0L;j<MatVal;j++)
			Src_tmp(i,j)/=MatMaxCo[j]*MatMaxLi[i];	
	
	//2 inversion
	//H3_MATRIX_FLT64 iPtxP=PtxP.Inv();
	H3_MATRIX_FLT64 iSrc_tmp=Src_tmp.Inv();	
	
	if (!iSrc_tmp.IsEmpty())
	{
		//à verifier si matrice non symetrique
		//3 denormalisation
		for(i=0L;i<MatVal;i++)
			for(j=0L;j<MatVal;j++)
				iSrc_tmp(i,j)/=MatMaxCo[i]*MatMaxLi[j];
	}
	
	return (iSrc_tmp);
}

CLASS_TYPE TYPE Mat_DiagMul(const H3_MATRIX & A)
{
	TYPE Result=1;

	for (size_t i=0; i<A.GetLi(); i++)
	{
		Result *= A(i,i);
	}

	return Result;
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_cofact
*	desct:	find cofactor
*	given:	A = a square matrix,
*		i=row, j=col
*	retrn:	the cofactor of Aij
*-----------------------------------------------------------------------------
*/
CLASS_TYPE TYPE Mat_CoFact(const H3_MATRIX & A,size_t i,size_t j)
{
	TYPE signa[2] = {1.0, -1.0};
	return signa[(i+j)%2] * A(i,j) * A.Minor(i,j);
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_minor
*	desct:	find minor
*	given:	A = a square matrix,
*		i=row, j=col
*	retrn:	the minor of Aij
*-----------------------------------------------------------------------------
*/
CLASS_TYPE TYPE Mat_Minor(const H3_MATRIX & A,size_t i,size_t j)
{
	H3_MATRIX S=A.SubMat(i,j);
	return S.Det();
}

CLASS_TYPE TYPE Mat_Det(const H3_MATRIX & Src)
{
	TYPE signa[2] = {1.0, -1.0};

	size_t n;
	long i,j;
	TYPE result=0;

	n = Src.GetLi();
	H3_MATRIX A(Src);
	H3_MATRIX P(n,1);

	// take a LUP-decomposition
	i=Mat_Lu(A,P);

	// Si la matrice est singuliere
	if (i==-1)
		return 0;
	
	// normal case: |A| = |L||U||P|
	// |L| = 1,
	// |U| = multiplication of the diagonal
	// |P| = +-1
	result = 1.0;
	for (j=0; j<A.GetLi(); j++)
	{
		result *= A((long)P(j,0),j);
	}
	result *= signa[i%2];

	return (result);
}

CLASS_TYPE H3_MATRIX Mat_SubMat(const H3_MATRIX & A,size_t i,size_t j)
{
	size_t m, m1, p, p1;
	size_t nbli=A.GetLi();
	size_t nbco=A.GetCo();

	H3_MATRIX S(A.GetLi()-1,A.GetCo()-1);

	for (m=m1=0; m<nbli; m++)
	{
		if (m==i) continue;
		for (p=p1=0; p<nbco; p++)
		{
			if (p==j) continue;
			S(m1,p1) = A(m,p);
			p1++;
		}
		m1++;
	}

	return S;
}

/*
*-----------------------------------------------------------------------------
*	funct:	mat_col
*	desct:	extraction d'une colonne
*	given:	A  matrix,
*	retrn:	la colonne j
*-----------------------------------------------------------------------------
*/
CLASS_TYPE H3_MATRIX Mat_CopyCo(const H3_MATRIX & A,long j)
{
	if (j<0 || j>A.GetCo()-1)
		return H3_MATRIX(0,0);

	H3_MATRIX S(A.GetLi(),1);
	for (size_t i=0;i<A.GetLi();i++)
	{
		S[i]=A(i,j);
	}

	return S;
}

CLASS_TYPE H3_MATRIX Mat_CopyLi(const H3_MATRIX & A,long i)
{
	if (i<0 || i>A.GetLi()-1)
		return H3_MATRIX(0,0);

	H3_MATRIX S(1,A.GetCo());
	for (size_t j=0;j<A.GetCo();j++)
	{
		S[j]=A(i,j);
	}

	return S;
}

/***************************************************************
Singular Value Decomposition
auteur CV d'apres Numerical recipies in C
calcul des valeurs singulieres de (*this): (*this)=U*W*V'
avec (*this): matrice m*n
U: matrice m*n à colonnes orthogonales
W: matrice diagonale >> l'element retourné ne contient que la diagonale
V: matrice n*n à colonnes orthogonales

//si U seule est necessaire, on peut ne passer qu'un argument à la fonction
****************************************************************/
CLASS_TYPE
bool H3_MATRIX::SVDcmp(H3_MATRIX& U,H3_MATRIX& W,H3_MATRIX& V)const
{
	CString strFunction=_T("Mat_svdcmp");
//	CString msg;
//	AfxMessageBox(_T("in ")+strFunction);
	size_t m=(*this).GetLi(),n=(*this).GetCo();
	//U
	U=(*this);//
//	msg.Format("U\n%f\t%f\t%f\n%f\t%f\t%f\n%f\t%f\t%f",U(0,0),U(0,1),U(0,2),U(1,0),U(1,1),U(1,2),U(2,0),U(2,1),U(2,2));
//	AfxMessageBox(msg);
	//V
	if(V.GetLi()!=n || V.GetCo()!=n){
		V=H3_MATRIX(n,n);
	}

	//W
	if(W.GetLi()!=1 || W.GetCo()!=n ){
		W= CH3Matrix(1,n);
	}

	bool flag;
	long i;
	long its,j,k,jj,l,nm;
	double anorm=0,c,s,scale=0;
	TYPE x,y,z,f,g=0,h;
	TYPE *rv1;
	rv1=new TYPE[n];

	//boucle1
	for(i=0;i<n;i++){
		l=i+1;
		rv1[i]=scale*g;
		g=s=scale=(double)0.0;
		if(i<m){
			for(k=i;k<m;k++) scale += ::fabs(U(k,i));
			if(scale){
				for(k=i;k<m;k++){
					U(k,i) /= scale;
					s += (U(k,i))*(U(k,i));
				}
				f=U(i,i);
				g=-SIGN(::sqrt(s),f);
				h=f*g-s;
				U(i,i)=f-g;
				for(j=l;j<n;j++){
					for(s=0,k=i;k<m;k++) s += U(k,i)*U(k,j);
					f=s/h;
					for(k=i;k<m;k++) U(k,j) += f*U(k,i);
				}
				for(k=i;k<m;k++) U(k,i) *= scale;
			}
		}
		W[i]=scale*g;//W(i)=scale*g;
		g=s=scale=0.0;
		if(i<m && i!=(n-1)){
			for(k=l;k<n;k++) scale += ::fabs(U(i,k));
			if(scale){
				for(k=l;k<n;k++){
					U(i,k)/=scale;
					s += (U(i,k))*(U(i,k));
				}
				f=U(i,l);
				g=-SIGN(::sqrt(s),f);
				h=f*g-s;
				U(i,l)=f-g;
				for(k=l;k<n;k++) rv1[k]=(U(i,k)) /h;
				for(j=l;j<m;j++){
					for(s=0,k=l;k<n;k++) s += (U(j,k)) * (U(i,k));
					for(k=l;k<n;k++) (U(j,k)) += s*rv1[k];
				}
				for(k=l;k<n;k++) (U(i,k)) *= scale;
			}
		}
		anorm=__max(anorm,(::fabs(W[i]+::fabs(rv1[i]) )));//anorm=__max(anorm,(fabs( W(i)+fabs(rv1[i]) )));
	}
	//boucle2 
	for(i=n-1;i>=0;i--){
		if(i<n-1){
			if(g){
				for(j=l;j<n;j++) V(j,i)=U(i,j)/U(i,l)/g;
				for(j=l;j<n;j++){
					for(s=0,k=l;k<n;k++) s += U(i,k)*V(k,j);
					for(k=l;k<n;k++) V(k,j) += s*V(k,i);
				}
			}
			for(j=l;j<n;j++) V(i,j)=V(j,i)=0.0;
		}
		V(i,i)=1.0;
		g=rv1[i];
		l=i;
	}
	//boucle3
	for(i=__min(m,n)-1;i>=0;i--){
		l=i+1;
		g=W[i];//g=W(i);
		for(j=l;j<n;j++) U(i,j)=0;
		if(g){
			g=1.0/g;
			for(j=l;j<n;j++){
				for(s=0.0,k=l;k<m;k++) s += U(k,i)*U(k,j);
				f=(s/U(i,i))*g;
				for(k=i;k<m;k++) U(k,j) += f*U(k,i);
			}
			for(j=i;j<m;j++) U(j,i) *= g;
		}
		else{
			for(j=i;j<m;j++)U(j,i)=0.0;
		}
		++(U(i,i));
	}
	//boucle4
	for(k=n-1;k>=0;k--){
		for(its=1;its<=30;its++){
			flag=true;
			for(l=k;l>=0;l--){
				nm=l-1;
				if(double(::fabs(rv1[l])+anorm)==anorm){
					flag=false;
					break;
				}
				if(double(::fabs(W[nm])+anorm)==anorm) break;//if(double(fabs(W(nm))+anorm)==anorm) break;
			}
			if(flag){
				c=0.0;
				s=1.0;
				for(i=l;i<=k;i++){
					f=s*rv1[i];
					rv1[i] *= c;
					if(double(::fabs(f)+anorm)==anorm) break;
					g=W[i];//g=W(i);
					h=pythag(f,g);
					W[i]=h;//W(i)=h;
					h=1.0/h;
					c=g*h;
					s=-f*h;
					for(j=0;j<m;j++){
						y=U(j,nm);
						z=U(j,i);
						U(j,nm)=y*c+z*s;
						U(j,i)=z*c-y*s;
					}
				}
			}
			z=W[k];//z=W(k);
			if(l==k){
				if(z<0.0){
					W[k]=-z;//W(k)=-z;
					for(j=0;j<n;j++) V(j,k) *= -1;//V(j,k)=-V(j,k);
					//for(j=0;j<n;j++) U(k,j)=-U(k,j);//pour coherence avec matlab
				}
				break;
			}
			if(its==30){
				//error: pas de convergence
				AfxMessageBox("error: pas de convergence");
				return false;
			}
			x=W[l];//x=W(l);
			nm=k-1;
			y=W[nm];//y=W(nm)
			g=rv1[nm];
			h=rv1[k];
			f=((y-z)*(y+z)+(g-h)*(g+h))/(2.0*h*y);
			g=pythag(f,(TYPE)1.0);
			f=((x-z)*(x+z)+h*((y/(f+SIGN(g,f)))-h))/x;
			c=s=1.0;
			for(j=l;j<=nm;j++){
				i=j+1;
				g=rv1[i];
				y=W[i];//y=W(i);
				h=s*g;
				g=c*g;
				z=pythag(f,h);
				rv1[j]=z;
				c=f/z;
				s=h/z;
				f=x*c+g*s;
				g=g*c-x*s;
				h=y*s;
				y *= c;
				for(jj=0; jj<n;jj++){
					x=V(jj,j);
					z=V(jj,i);
					V(jj,j)=x*c+z*s;
					V(jj,i)=z*c-x*s;
				}
				z=pythag(f,h);
				W[j]=z;//W(j)=z;
				if(z){
					z=1.0/z;
					c=f*z;
					s=h*z;
				}
				f=c*g+s*y;
				x=c*y-s*g;
				for(jj=0;jj<m;jj++){
					y=U(jj,j);
					z=U(jj,i);
					U(jj,j)=y*c+z*s;
					U(jj,i)=z*c-y*s;
				}
			}
			rv1[l]=0.0;
			rv1[k]=f;
			W[k]=x;//W(k)=x;
		}	
	}
	delete[] rv1;
//ordonner W //ajout cv le 200404
	//ceci n'est pas fait dans le corps de la fonction >> pb de traduction de C vers C++?
	CH3Array2D< TYPE > Cu(m,1),Cv(n,1);
	TYPE Wi;
	bool cond=true;
	while(cond){
		cond=false;
		for(i=0; i<n-1;i++){
			if(W[i]<W[i+1]){//if(W(i)<W(i+1)){
				Cu=U.GetAt(0,i,m,1);
				U.SetAt(0,i,U.GetAt(0,i+1,m,1));
				U.SetAt(0,i+1,Cu);

				Wi=W[i];//Wi=W(i);
				W[i]=W[i+1];//W(i)=W(i+1);
				W[i+1]=Wi;//W(i+1)=Wi;

				//Lv=V.GetAt(i,0,1,n);
				//V.SetAt(i,0,V.GetAt(i+1,0,1,n));
				//V.SetAt(i+1,0,Lv);
				Cv=V.GetAt(0,i,n,1);
				V.SetAt(0,i,V.GetAt(0,i+1,n,1));
				V.SetAt(0,i+1,Cv);

				cond=true;
			}
		}
	}

	U *= -1;//=U*(-1);//pour coherence avec matlab//cv250308
	V *= -1;//=V*(-1);

	return true;
}
/*******************************************************
pythag
auteur: cv d'apres Recipies in C
mars 2004
calcul sqrt(a2+b2)
********************************************************/

CLASS_TYPE TYPE pythag( TYPE a, TYPE b)
{
	double absa=fabs(a),absb=::fabs(b);
	if(absa>absb) return absa*::sqrt(1.0+SQR(absb/absa));
	else return(absb==0.0 ? 0.0 : absb*::sqrt(1.0+SQR(absa/absb)));
}
/*******************************************************
Mat_svbksb
auteur: cv d'apres Recipies in C
mars 2004
Resolution de A.x=B
A=U*W*V' (utilise SVDcmp) matrice m*n
B: matrice m*1
output: X: matrice n*1
********************************************************/
CLASS_TYPE H3_MATRIX Mat_svbksb(const H3_MATRIX& U,const H3_MATRIX& W,const H3_MATRIX& V,const H3_MATRIX& B)
{
	size_t nU=U.GetCo(),mU=U.GetLi(),nB=B.GetCo();
	size_t i,j,k;
	double s;
	H3_ARRAY2D temp(nU,nB);
	for(k=0;k<nB;k++){
		for(j=0;j<nU;j++){
			s=0.0;
			if(W[j]){//if(W(j)){
				for(i=0;i<mU;i++) s += U(i,j)*B(i,k);
				s /= W[j];//s /= W(j);
			}
			temp(j,k)=s;
		}
	}
	H3_MATRIX X(nU,nB);
	for(k=0;k<nB;k++){
		for(j=0;j<nU;j++){
			s=0.0;
			for(i=0;i<nU;i++) s += V(j,i)*temp(i,k);
			X(j,k)=s;
		}
	}

	return X;
}

/*******************************************************
Mat_meansquare2
auteur: cv d'apres Recipies in C
mars 2004
Resolution de A.x=B
A= matrice m*n
B: matrice m*1
output: X: matrice n*1

 nb: le facteur 1e-9 est à determiner à l'usage (1e-6 dans la doc)
********************************************************/
#define H3_MATRIX_FACTOR 1e-6
CLASS_TYPE H3_MATRIX Mat_MeanSquare2(const H3_MATRIX& A,const H3_MATRIX& B)
{
	size_t j;
	size_t n=A.GetCo(),m=A.GetLi();
	bool isSingular=false;

	H3_MATRIX U(m,n),V(n,n),W(1,n);

	A.SVDcmp(U,W,V);
	TYPE wmax=0,w;
	for(j=0;j<n;j++){
		w=::fabs(W[j]);//w=fabs(W(j));
		if(w>wmax) wmax=w;
	}

	TYPE wmin=wmax*H3_MATRIX_FACTOR;
	for(j=0;j<n;j++){
		w=::fabs(W[j]);//w=fabs(W(j));
		if(w<wmin) {
			W[j]=0;//W(j)=0;
			isSingular=true;
		}
	}
#if H3_CHECKALL_MODE
	if(isSingular) AfxMessageBox("Mat_MeanSquare2: matrice singuliere");
#endif
	H3_MATRIX R=Mat_svbksb(U,W,V,B);

	return (R);
}
#undef H3_MATRIX_FACTOR
/*******************************************************
Norm
auteur: cv
mars 2004
A une matrice
A=U*W*V' (Mat_svdcmp)
A.Norm=plus grand element de W
********************************************************/
CLASS_TYPE TYPE H3_MATRIX::Norm()const
{
	size_t n=(*this).GetCo(),m=(*this).GetLi();

	H3_MATRIX U(m,n),V(n,n),W(1,n);

	(*this).SVDcmp(U,W,V);
	return W[0];
}

/*******************************************************
Cond
auteur: cv
mars 2004
A une matrice
A=U*W*V' (Mat_svdcmp)
A.Cond=plus grand element de W / plus petit
une condition grande indique à priori une matrice singuliere
********************************************************/
CLASS_TYPE TYPE H3_MATRIX::Cond()const
{
	size_t j;
	size_t n=(*this).GetCo(),m=(*this).GetLi();

	//H3_MATRIX U,V,W;
	H3_MATRIX U(m,n),V(n,n),W(1,n);

	(*this).SVDcmp(U,W,V);
	TYPE wmax=::fabs(W[0]),wmin=::fabs(W[0]),w;//TYPE wmax=fabs(W(0)),wmin=fabs(W(0)),w;
	for(j=1;j<n;j++){
		w=::fabs(W[j]);//w=fabs(W(j));
		if(w>wmax) wmax=w;
		if(w<wmax) wmin=w;
	}
	return (wmax/wmin);
}
/*******************************************************
Cross
auteur: cv
mars 2004
A et B deux matrices
A cross B =C
colonne i de C= produit vectoriel de la colonne i de A avec la i de B 
********************************************************/
CLASS_TYPE	H3_MATRIX	Mat_Cross(const H3_MATRIX& U,const H3_MATRIX& V)
{
	size_t L=U.GetLi(),C=U.GetCo(),i,j;//si L<2: pb
	if(( L!= V.GetLi())||( C!= V.GetCo())){
		return H3_MATRIX();
	}
	H3_MATRIX ret(L,C);
	ret.Fill(0);;
	for(j=0;j<C;j++){
		for(i=0;i<L-2;i++){	
			 ret(i,j) = (U(i+1,j))*(V(i+2,j))-(V(i+1,j))*(U(i+2,j));
		}
	}
	for(j=0;j<C;j++){
		i=L-2;
		{	
			 ret(i,j) = (U(i+1,j))*(V(0,j))-(V(i+1,j))*(U(0,j));
		}
		i=L-1;
		{	
			 ret(i,j) = (U(0,j))*(V(1,j))-(V(0,j))*(U(1,j));
		}
	}
	return ret;
}
/*******************************************************
Mat_Dot
auteur: cv
mars 2004
A et B deux matrices
A dot B =C
C(1,i)= produit scalaire de la colonne i de A avec la i de B 
********************************************************/
CLASS_TYPE	H3_MATRIX	Mat_Dot(const H3_MATRIX& U,const H3_MATRIX& V)
{
	size_t L=U.GetLi(),C=U.GetCo(),i,j;
	if(( L!= V.GetLi())||( C!= V.GetCo())){
//		throw();
		return H3_MATRIX();
	}
	H3_MATRIX ret(1,C);
	ret.Fill(0);
	TYPE temp;
	for(j=0;j<C;j++){
		temp=0;
		for(i=0;i<L;i++){	
			 temp += (U(i,j))*(V(i,j));
		}
		ret[j]=temp;//ret(j)=temp;
	}
	return ret;
}
/*******************************************************
Rodrigues
auteur: cv
mars 2004
d'apres fonction homonyme pour Matlab
********************************************************/

CLASS_TYPE	H3_MATRIX H3_MATRIX::Rodrigues(CH3Matrix& dout)const
{
	if((m_nLi!=3)||(m_nCo!=1)) return CH3Matrix(3,3);
	if((dout.GetLi() != 9)|| (dout.GetCo() != 3)) dout=CH3Matrix(9,3);

	double x=m_pData[0],y=m_pData[1],z=m_pData[2];
	double theta=::sqrt(double(x*x+y*y+z*z));

	CH3Matrix out(3,3);

	dout.Fill(0);

	if(theta<DBL_EPSILON){

//SJ		out=Mat_eye(3);
		CH3Matrix out(3,3);

		out.Fill(0);
		out.SetAt(0,0,1);
		out.SetAt(1,1,1);
		out.SetAt(2,2,1);

		dout(1,2)=dout(5,0)=dout(6,2)=1;
		dout(2,1)=dout(3,2)=dout(7,0)=-1;
		
		return(out);
	}
	else{
		double alpha=::cos(theta),beta=::sin(theta),gamma=1-alpha;
		CH3Matrix omega=(*this)/theta;
		CH3Matrix omegav(3,3);
		CH3Matrix A(3,3);
		
		//SJ eye=Mat_eye(3);
		CH3Matrix eye(3,3);

		eye.Fill(0);
		eye.SetAt(0,0,1);
		eye.SetAt(1,1,1);
		eye.SetAt(2,2,1);

		CH3Matrix dm3din(4,3),dm2dm3(4,4),dm1dm2(21,4),dRdm1(9,21);
		double w1=omega(0),w2=omega(1),w3=omega(2);
		
		omegav.Fill(0);
		omegav(0,1)=-w3;
		omegav(0,2)= w2;
		omegav(1,0)= w3;
		omegav(1,2)=-w1;
		omegav(2,0)=-w2;
		omegav(2,1)= w1;

		A=omega*omega.Trans();

		if(dout.GetData() !=NULL){

			dm1dm2.Fill(0);
			dm1dm2(0,3)=-beta;
			dm1dm2(1,3)=alpha;
			dm1dm2(2,3)=beta;
			dm1dm2(8,0)=dm1dm2(9,1)=dm1dm2(4,2)=1;
			dm1dm2(10,0)=dm1dm2(5,1)=dm1dm2(6,2)=-1;
			dm1dm2(12,0)=2*w1;
			dm1dm2(16,1)=2*w2;
			dm1dm2(20,2)=2*w3;
			dm1dm2(13,1)=dm1dm2(15,1)=dm1dm2(14,2)=dm1dm2(18,2)=w1;
			dm1dm2(13,0)=dm1dm2(15,0)=dm1dm2(17,2)=dm1dm2(19,2)=w2;
			dm1dm2(14,0)=dm1dm2(18,0)=dm1dm2(17,1)=dm1dm2(19,1)=w3;

			dRdm1.Fill(0);
			dRdm1(0,0)=dRdm1(4,0)=dRdm1(8,0)=1;
			dRdm1(0,1)=omegav(0);
			dRdm1(1,1)=omegav(3);
			dRdm1(2,1)=omegav(6);
			dRdm1(3,1)=omegav(1);
			dRdm1(4,1)=omegav(4);
			dRdm1(5,1)=omegav(7);
			dRdm1(6,1)=omegav(2);
			dRdm1(7,1)=omegav(5);
			dRdm1(8,1)=omegav(8);
			dRdm1(0,3)=dRdm1(1,4)=dRdm1(2,5)=dRdm1(3,6)=dRdm1(4,7)=dRdm1(5,8)=dRdm1(6,9)=dRdm1(7,10)=dRdm1(8,11)=beta;
			dRdm1(0,2)=A(0);
			dRdm1(1,2)=A(3);
			dRdm1(2,2)=A(6);
			dRdm1(3,2)=A(1);
			dRdm1(4,2)=A(4);
			dRdm1(5,2)=A(7);
			dRdm1(6,2)=A(2);
			dRdm1(7,2)=A(5);
			dRdm1(8,2)=A(8);
			dRdm1(0,12)=dRdm1(1,13)=dRdm1(2,14)=dRdm1(3,15)=dRdm1(4,16)=dRdm1(5,17)=dRdm1(6,18)=dRdm1(7,19)=dRdm1(8,20)=gamma;
			
			dm2dm3.Fill(0);
			dm2dm3.SetAt(0,0,eye/theta);
			dm2dm3.SetAt(0,3,(*this)/(-theta*theta));
			dm2dm3(3,3)=1;

			dm3din.SetAt(0,0,eye);
			dm3din.SetAt(3,0,(*this).Trans()/theta);

			dout=dRdm1*dm1dm2*dm2dm3*dm3din;
		}

		out=eye;
		out *= alpha;
		out += (omegav * beta) + (A * gamma);

		return(out);
	}
}

CLASS_TYPE	H3_MATRIX H3_MATRIX::eye()const
{
	if((m_nLi==1)||(m_nCo==1)){
		long m=__max(m_nLi,m_nCo);
		H3_MATRIX Ret(m,m);
		Ret.Fill(0);
		for(long i=0;i<m;i++){
			Ret(i,i)=(*this)(i);
		}
		return Ret;
	}
	else{
		//throw...
		AfxMessageBox("H3_MATRIX::eye() : erreur");
		return H3_MATRIX();
	}
}

//d'apres Numerical Recipies in c
//cholesky decomposition
//si la matrice à tester est définie positive et symetrique ,
//cette routine construit sa décomposition de cholesky
//la matrice retournée RetMat n'occupe qu'un triangle inférieur
//RetMat*RetMat'=(*this)
//A.choldc(true,p) retourne la matrice triangulaire inf , p contient la diagonale
//A.choldc(false,p) retourne une matrice dont le triangle inf est la décomposition voulue
//   si on lui supperpose la diagonale p, le triangle sup (diag compris) correspond
//à la matrice de départ

CLASS_TYPE	H3_MATRIX H3_MATRIX::choldc(bool cond,H3_MATRIX& p) const
{
	CString strFunction("choldc");

	if(p.GetSize()!=m_nLi) p=H3_MATRIX(1,m_nLi);

	long i,j,k;
	double sum;
	H3_MATRIX RetMat(*this);

	TYPE* pR=RetMat.GetData(),*pRi,*pRj;
	long offseti,offsetj;

	for(i=0;i<m_nLi;i++){
		offseti=i*m_nCo;
		for(j=i;j<m_nCo;j++){
			offsetj=j*m_nCo;
			sum=pR[offseti+j];
			pRi=pR+offseti;
			pRj=pR+offsetj;
			for(k=i-1;k>=0;k--) sum -= pRi[k]*pRj[k];
			if(i==j){
				if(sum<=0.0){
					#if defined(H3APPTOOLSDECL_H__INCLUDED_)
						H3DebugError(strModule,strFunction,"la matrice n'est pas définie positive");
					#endif
					AfxThrowUserException();
				}
				p[i]=::sqrt(sum);
			}
			else{
				pR[offsetj+i]=sum/p[i];
			}
		}
	}

	if(cond){
		for(i=0;i<m_nLi;i++) RetMat(i,i)=p[i];

		for(i=0;i<m_nLi;i++){
			offseti=i*m_nCo;
			for(j=i+1;j<m_nCo;j++){
				pR[offseti+j]=0;
			}
		}
	}

	return RetMat;
}
//d'apres Numerical Recipies in c
//on cherche x verifiant A.x=b
//A matrice symetrique definie positive
//A s'ecrit =Ltheo*Ltheo'
//avec Ltheo est une matrice triangle inf (telle que retournée par cholc(true)
//en fait, on utilise L et p retournés par L=A.cholc(false,p)
//L matrice n*n
//b matrice n*1
//x matrice n*1 x est ce que l'on recherche
//on ecrit: x=L.cholsl(b,p)
CLASS_TYPE	H3_MATRIX H3_MATRIX::cholsl(const H3_MATRIX& b,const H3_MATRIX& p) const
{
	CString strFunction("cholsl");

	//verif
	long bco=b.GetCo(),bli=b.GetLi();
	if(bli!=m_nLi){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"la matrice b ne convient pas");
		#endif
		AfxThrowUserException();
	}
	if(p.GetSize()!=m_nLi){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"la matrice p ne convient pas");
		#endif
		AfxThrowUserException();
	}
	if(m_nCo!=m_nLi){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"la matrice n'est pas carrée");
		#endif
		AfxThrowUserException();
	}

	long i,k,offseti,index_co;
	double sum;
	H3_MATRIX x(m_nCo,bco);
	TYPE *pD=(double*)GetData();

	for(index_co=0;index_co<bco;index_co++){
		for(i=0;i<m_nLi;i++){//solve L.y=b, storing y in x
			offseti=i*m_nCo;
			for(sum=b(i,index_co),k=i-1;k>-1;k--) sum -= pD[offseti+k]*x(k,index_co);
			x(i,index_co)=sum/p[i];
		}

		for(i=m_nLi-1;i>-1;i--){//solve L'.x=y
			for(sum=x(i,index_co),k=i+1;k<m_nLi;k++) sum -= pD[k*m_nLi+i]*x(k,index_co);
			x(i,index_co)=sum/p[i];
		}
	}

	return x;
}

//si A est une matrice symetrique definie positive et L le resultat de
//L=A.choldc(false,p)
//L.chols_inv1(p) retourne inv(Ltheo) ou Ltheo=A.choldc(true)
//Ltheo*Ltheo'=A donc inv(A)=inv(Ltheo)'*inv(Ltheo)
CLASS_TYPE	H3_MATRIX H3_MATRIX::chols_inv1(const H3_MATRIX& p) const
{
	CString strFunction("chols_inv1");
	//verif
	if((this->GetLi())!=p.GetSize()){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"la diagonale est inapropriée");
		#endif
		AfxThrowUserException();
	}
	
	long i,j,k;
	double sum;
	H3_MATRIX iL=(*this);

	for(i=0;i<m_nLi;i++){
		iL(i,i)=1.0/p[i];
		for(j=i+1;j<m_nCo;j++){
			sum=0.0;
			for(k=i;k<j;k++) sum -= iL(j,k)*iL(k,i);
			iL(j,i)=sum/p[j];
		}
	}

	for(i=0;i<m_nLi;i++){
		for(j=i+1;j<m_nCo;j++){
			iL(i,j)=0;
		}
	}

	return iL;
}

//si A est une matrice symetrique definie positive et L le resultat de
//A.chols_inv() retourne inv(A) 
//principe: Ltheo=A.choldc(true)
//Ltheo*Ltheo'=A donc inv(A)=inv(Ltheo)'*inv(Ltheo)
//ATTENTION:si A n'est pas symetrique definie positive
//		le resultat n'est pas la matrice inverse de A
//		M une matrice, si M*M' est inversible, M*M' est symetrique definie positive
//nb:en fait, iL=inv(Ltheo) est calculé à partir de L=A.choldc(false,diag);
CLASS_TYPE	H3_MATRIX H3_MATRIX::chols_inv() const
{
	CString strFunction("chols_inv");
	//verif: pas de verif
	
	H3_MATRIX diag;

	H3_MATRIX L=(*this).choldc(false,diag);

	H3_MATRIX iL=L.chols_inv1(diag);

	return Mat_XTransX(iL);
}

//A une matrice
//A.x=Y
//on note iA la matrice (si elle existe ) tq x=iA.Y 
//iA=inv(A'*A)*A'
//B=A'*A est une matrice symetrique definie positive
//B peut donc etre inversée en utilisant choleski
//c'est ce que l'on va faire pour calculer iA qui est la valeur retournée
CLASS_TYPE	H3_MATRIX H3_MATRIX::inv_chols() const
{
	CString strFunction("inv_chols");

	if(m_nLi!=m_nCo){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"la matrice n'est pas carrée");
		#endif
		AfxThrowUserException();				
	}

	H3_MATRIX B=Mat_XTransX(*this);

	H3_MATRIX diag;

	H3_MATRIX L=B.choldc(false,diag);

	H3_MATRIX iL=L.chols_inv1(diag);

	H3_MATRIX iB=Mat_XTransX(iL);

	return Mat_XYTrans(iB,(*this));
}

//si A est une matrice symetrique definie positive 
//L le resultat de
//L=A.choldc(false,p)
//L.chols_inv1(p) retourne inv(Ltheo) ou Ltheo=A.choldc(true)
//Ltheo*Ltheo'=A donc inv(A)=inv(Ltheo)'*inv(Ltheo)
//on retourne inv(A)
CLASS_TYPE	H3_MATRIX H3_MATRIX::inv_chols1() const
{
	CString strFunction("inv_chols1");
	//verif //tres incomplet mais mieux que rien
	if((this->GetLi())!=(this->GetCo())){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"la matrice n'est pas carré");
		#endif
		AfxThrowUserException();
	}
	
	try{
		H3_MATRIX diag(m_nLi,1);
		H3_MATRIX L=(*this).choldc(false,diag);
		H3_MATRIX iL=L.chols_inv1(diag);
		return Mat_XTransX(iL);
	}
	catch(...){
		#if defined(H3APPTOOLSDECL_H__INCLUDED_)
			H3DebugError(strModule,strFunction,"la matrice n'est pas symetrique definie positive");
		#endif
		AfxThrowUserException();

		return H3_MATRIX(0,0);//jamais atteind mais evite un warning à la compilation
	}
}

#endif

