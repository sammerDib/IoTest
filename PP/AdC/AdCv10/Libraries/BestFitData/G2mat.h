#ifndef G2MAT_H__INCLUDED_
#define G2MAT_H__INCLUDED_

#include <limits>
#include <string>
using namespace std;

static float GetFPNaN()
{
	//1 bit de signe à 0 ou 1,
	//8 bits d'exposant à 1,
	//23 bits de mantisse, mantisse non nulle(mantisse nulle pour infini)
	//si la mantisse commence par 1: Quiet NaN
	//si la mantisse commence par 0: Signal NaN (erreur)
	float Value;
	long *pf=(long *)&Value;
	*pf=0x7FC00000;
	return Value;
}
static double GetFPdNaN()
{
	//1 bit de signe à 0 ou 1,
	//11 bits d'exposant à 1,(7FF)
	//52 bits de mantisse, mantisse non nulle(mantisse nulle pour infini)
	//si la mantisse commence par 1: Quiet NaN
	//si la mantisse commence par 0: Signal NaN (erreur)
	//ATTENTION: NaN peut etre > ou < à un double avec Visual C++ au 19/04/07
	double Value;
	unsigned long nan[2]={0xffffffff, 0x7fffffff};
	Value = *( double* )nan;
	return Value;
}

const double dNaN=GetFPdNaN();
const float  fNaN=GetFPNaN();

const double PI=3.14159265358979;
const double TWO_PI=6.28318530717959;
const float fPI=(3.1415927f);
const float fTWO_PI=(6.2831853f);

#define MATRIX_T		TMat< TYPE >

// Entiers non signes
#define MATRIX_UINT8	TMat< byte > 
#define MATRIX_FLT		TMat< float >
#define MATRIX_DBL		TMat< double >

#define CLASS_TYPE  template< class TYPE >

CLASS_TYPE class TMat
{

public:
	// Methodes de copie
	MATRIX_T & operator =(const MATRIX_T & Src);

	// Operateurs
	MATRIX_T operator *(const MATRIX_T & Src)const;//la * matricielle ne se fait pas comme celle d'un Array2D
	MATRIX_T operator *(const TYPE Src)const;
	MATRIX_T operator -(const MATRIX_T & Src)const;
	MATRIX_T operator -(const TYPE & Src)const;
	MATRIX_T operator +(const MATRIX_T & Src)const;
	MATRIX_T operator +(const TYPE & Src)const;
	MATRIX_T operator /(const float &Src);
	TYPE  operator[](unsigned long nIndex) const;
	TYPE& operator[](unsigned long nIndex);	

	TYPE & operator ( )(long nLi, long nCo);
	TYPE   operator ( )(long nLi, long nCo) const;
	TYPE & operator ( )(long nItem);

	void CopyElements(TYPE* pDest, TYPE* pSrc, int nCount);
	void CopyElements(TYPE* pDest,const TYPE* pSrc, int nCount);

	TYPE* GetData()const;
	void InitMembers();
	bool Alloc(long nLi,long nCo);
	bool ReAlloc(long nLi,long nCo);
	void Copy(const MATRIX_T & src);
	void Free();
	bool IsEmpty();
	bool ReSetDims(long nLi,long nCo);
	MATRIX_T transpose()const;

	TYPE GetAt(long nLi,long nCo)const;

	unsigned long GetCo()const;
	unsigned long GetLi()const;
	long GetSize()const;

	void Fill(TYPE Value);
	MATRIX_T Inv()const;
	
	TMat(const TMat & src);
	TMat(long NbLi,long NbCo);
	TMat();
	~TMat() {Free();};

private:
	unsigned long m_nLi; //Nb line
	unsigned long m_nCo; //Nb Column
	TYPE * m_pData;

};

//utilitaires
/*CLASS_TYPE  MATRIX_T Mat_BackSubs1(const MATRIX_T &A, MATRIX_T &B, MATRIX_T &X,const MATRIX_T &P,long xcol);
CLASS_TYPE  long Mat_Lu( MATRIX_T & A, MATRIX_T & P);

CLASS_TYPE  MATRIX_T Mat_Mul(const MATRIX_T & A, const MATRIX_T & B);
CLASS_TYPE  MATRIX_T Mat_Scalar_Mul(const MATRIX_T & A, const TYPE & B);
CLASS_TYPE  MATRIX_T Mat_Scalar_Add(const MATRIX_T & A, const TYPE & B);
CLASS_TYPE  MATRIX_T Mat_Div(const MATRIX_T & A,const TYPE Src);
CLASS_TYPE  MATRIX_T Mat_Add(const MATRIX_T & A,const MATRIX_T Src);
CLASS_TYPE  MATRIX_T Mat_Sub(const MATRIX_T & A,const MATRIX_T Src);
CLASS_TYPE  MATRIX_T Mat_Inv(const MATRIX_T & A);*/

//////////////////////////////////////////////////////////////////////////////////////
// Methodes et operateur arithmetiques
//////////////////////////////////////////////////////////////////////////////////////
CLASS_TYPE MATRIX_T Mat_Mul(const MATRIX_T & A,const MATRIX_T & B)
{
	const long Ali=A.GetLi(),Aco=A.GetCo(),Bli=B.GetLi(),Bco=B.GetCo();

	long i,j,k;

	MATRIX_T C(Ali,Bco);
	C.Fill(0);
	TYPE Aik;

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
CLASS_TYPE MATRIX_T Mat_Scalar_Mul(const MATRIX_T & A, const TYPE & B)
{
	MATRIX_T C(A.GetLi(),A.GetCo());
	TYPE *pA = A.GetData();
	for(int i=0;i<A.GetSize();i++)
	{
		C(i)=pA[i]*B;
	}
	return (C);
}
CLASS_TYPE MATRIX_T Mat_Scalar_Add(const MATRIX_T & A,const TYPE & B)
{
	MATRIX_T C(A.GetLi(),A.GetCo());
	TYPE *pA = A.GetData();
	for(int i=0;i<A.GetSize();i++)
	{
		C(i)=pA[i]+(TYPE)B;
	}
	return (C);
}
CLASS_TYPE MATRIX_T Mat_Div(const MATRIX_T & A,const TYPE Src)
{
	MATRIX_T C( A.GetLi() ,A.GetCo());
	C.Fill(0);
	TYPE *pA = A.GetData();
	for(int index=0;index < C.GetSize();index++)
	{
		C(index)=pA[index]/Src;
	}

	return C;
}
CLASS_TYPE MATRIX_T Mat_Add(const MATRIX_T & A,const MATRIX_T Src)
{
	MATRIX_T C( A.GetLi() ,A.GetCo());
	C.Fill(0);

	TYPE *pA = A.GetData();
	TYPE *pSrc = Src.GetData();
	for(int i=0;i<A.GetSize();i++)
	{
		C(i)=pA[i]+pSrc[i];
	}

	return(C);
}
CLASS_TYPE MATRIX_T Mat_Sub(const MATRIX_T & A,const MATRIX_T Src)
{
	MATRIX_T C( A.GetLi() ,A.GetCo());
	C.Fill(0);
	TYPE *pA = A.GetData();
	TYPE *pSrc = Src.GetData();
	for(int i=0;i<A.GetSize();i++)
	{
		C(i)=pA[i]-pSrc[i];
	}

	return(C);
}
CLASS_TYPE MATRIX_T Mat_BackSubs1(const MATRIX_T &A, MATRIX_T &B, MATRIX_T &X ,const MATRIX_T &P,long xcol)
{
	long i, j, k, n;
	TYPE sum;

	long Pk0,Pi0;
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

	return (X);
}
CLASS_TYPE MATRIX_T Mat_BackSubs1(const MATRIX_T &A,const MATRIX_T &B,const MATRIX_T &X,const MATRIX_T &P,int xcol)
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
CLASS_TYPE long Mat_Lu( MATRIX_T &A, MATRIX_T &P)
{
	long i, j, k, n;
	long maxi;
	TYPE c, c1,tmp;
	long p;
	long Pk0,Pi0;
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
			A(Pi0,k) /= A(Pk0,k);

			A_Pi0_k=A(Pi0,k);

			// --- elimination ---
			for (j=k+1;j<n; j++)
			{
				A(Pi0,j) -= A_Pi0_k * A(Pk0,j);
			}
		}
	}

	return p;
}
CLASS_TYPE MATRIX_T Mat_Inv(const MATRIX_T & Src)
{
	long n = Src.GetCo();
	MATRIX_T A(Src);
	MATRIX_T B(n,1);
	MATRIX_T C(n,n);
	MATRIX_T P(n,1);
	//	- LU-decomposition -
	//	also check for singular matrix
	if (Mat_Lu(A, P) == -1)
	{
		return MATRIX_T(0,0);
	}

	for (long i=0; i<n; i++)
	{
		B.Fill(0);
		B(i,0) = 1.0;
		Mat_BackSubs1( A, B, C, P, i );
	}

	return (C);
}
CLASS_TYPE TYPE Mat_Det(const MATRIX_T & Src)
{
	TYPE signa[2] = {1.0, -1.0};

	long i,j,n;
	TYPE result=0;

	n = Src.GetLi();
	MATRIX_T A(Src);
	MATRIX_T P(n,1);

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
/*****************************************/

CLASS_TYPE MATRIX_T::TMat()
{
	m_nLi=m_nCo=0L;
	m_pData=NULL;
}

CLASS_TYPE MATRIX_T::TMat( long nLi,long nCo )
{
	Alloc(nLi,nCo);
}

CLASS_TYPE MATRIX_T & MATRIX_T::operator=( const MATRIX_T & Src )
{
	if (this==&Src) return *this;

	Copy(Src);

	return *this;
}

CLASS_TYPE MATRIX_T MATRIX_T::operator*( const MATRIX_T & Src ) const
{
	return Mat_Mul((*this),Src);
}

CLASS_TYPE MATRIX_T ::TMat( const MATRIX_T & src )
{
	if (this!=&src)
	{
		if (Alloc(src.m_nLi,src.m_nCo))
			CopyElements(m_pData, src.m_pData, m_nLi*m_nCo);
	}
}

CLASS_TYPE MATRIX_T MATRIX_T::operator *(const TYPE Src)const
{
	return Mat_Scalar_Mul(*this,Src);
}

CLASS_TYPE MATRIX_T MATRIX_T::operator +(const TYPE & Src)const
{
	return(Mat_Scalar_Add(*this,Src));
}

CLASS_TYPE MATRIX_T MATRIX_T::operator -(const TYPE & Src)const
{
	return(Mat_Scalar_Add(*this,-Src));
}

CLASS_TYPE MATRIX_T MATRIX_T::Inv()const 
{
	return Mat_Inv((*this));
}

CLASS_TYPE
	TYPE MATRIX_T::operator[](unsigned long nIndex) const
{ 
	return m_pData[nIndex]; 
}

CLASS_TYPE
	TYPE& MATRIX_T::operator[](unsigned long nIndex)
{
	return m_pData[nIndex]; 
}

CLASS_TYPE
	void MATRIX_T::CopyElements(TYPE* pDest, TYPE* pSrc, int nCount)
{
	while (nCount--)
		( *(pDest++) ) = ( *(pSrc++) );
}

CLASS_TYPE
	void MATRIX_T::CopyElements(TYPE* pDest,const TYPE* pSrc, int nCount)
{
	while (nCount--)
		( *(pDest++) ) = ( *(pSrc++) );
}
// Retourne un pointeur sur les donnees
CLASS_TYPE
	TYPE* MATRIX_T::GetData()const
{ 
	return (TYPE*)m_pData; 
}

CLASS_TYPE
	inline	long MATRIX_T::GetSize()const
{
	return m_nLi*m_nCo;
}


//////////////////////////////////////////////////////////////////////////////////////
// Methodes et d'allocation et de liberation
//////////////////////////////////////////////////////////////////////////////////////
CLASS_TYPE
	void MATRIX_T::InitMembers()
{
	m_pData=NULL;
	m_nLi=m_nCo=0;
}

// Allocation d'un tableau de nLi*nCo elements
CLASS_TYPE
	bool MATRIX_T::Alloc(long nLi,long nCo)
{
	InitMembers();

	// Pour eviter d'allouer des tableaux de dimensions nulles
	if (nLi>0 && nCo>0)
	{
		try{
			m_pData= new TYPE [nLi*nCo] ;
			if(m_pData != NULL)
			{
				m_nLi=nLi;
				m_nCo=nCo;
				return true;
			}
		}
		catch(...)
		{
			// 			CString msg;
			// 			msg.Format("MATRIX_T:Pb lors de l'allocation de %d élément(s)",nLi*nCo);
			// 			AfxMessageBox(msg);
			// 			e->Delete() ;

			//			ASSERT(FALSE);
			//			TRACE("MATRIX_T:Pb lors de l'allocation de %d élément(s)",nLi*nCo);
			return false;
		}
	}
	else
	{
		if(nLi>0L)
			m_nLi=nLi;
		else
			m_nLi=0L;

		if(nCo>0L)
			m_nCo=nCo;
		else
			m_nCo=0L;

		m_pData=NULL;

		return true;
	}

	return false;
}
// ReAllocation d'un tableau de nLi*nCo elements
CLASS_TYPE
	bool MATRIX_T::ReAlloc(long nLi,long nCo)
{
	if (nLi!=m_nLi || nCo!=m_nCo)
	{
		Free();
		return Alloc(nLi,nCo);
	}
	return true;
}

// Desallocation memoire
CLASS_TYPE 
	void MATRIX_T::Free()
{
	if (m_pData && m_nLi*m_nCo>0)
	{
		delete [] m_pData;
		m_pData=NULL;
		m_nLi=m_nCo=0;
	}
	InitMembers();
}
// Copie les donnees de l'element src
CLASS_TYPE
	void MATRIX_T::Copy(const MATRIX_T & src)
{
	if (this != &src)
	{
		if (ReAlloc(src.m_nLi,src.m_nCo))
		{
			CopyElements(m_pData,src.m_pData,m_nLi*m_nCo);
		}
	}
}

CLASS_TYPE
	bool MATRIX_T::ReSetDims(long nLi,long nCo)
{
	m_nLi=nLi;
	m_nCo=nCo;
	return(true);
}

CLASS_TYPE
	inline	TYPE MATRIX_T::GetAt(long nLi,long nCo)const
{
	return (*this)[nLi*m_nCo+nCo];
}

CLASS_TYPE
	inline	TYPE   MATRIX_T::operator ( )(long nLi, long nCo) const
{
	return (*this)[nLi*m_nCo+nCo];
}

CLASS_TYPE
	inline	TYPE & MATRIX_T::operator ( )(long nItem)
{
	return (*this)[nItem];
}

CLASS_TYPE
	TYPE & MATRIX_T::operator ( )(long nLi, long nCo)
{
	return (*this)[nLi*m_nCo+nCo];
}

//CLASS_TYPE MATRIX_T & MATRIX_T::operator =(const MATRIX_T & Src)
CLASS_TYPE MATRIX_T MATRIX_T::operator /(const float &Src)
{
	return Mat_Div(*this,Src);

}

CLASS_TYPE MATRIX_T MATRIX_T::operator +(const MATRIX_T &Src)const
{
	return Mat_Add(*this,Src);
}

CLASS_TYPE MATRIX_T MATRIX_T::operator -(const MATRIX_T &Src)const
{
	return Mat_Sub(*this,Src);
}

// Initialise tout le tableau avec la valeur Value
CLASS_TYPE
	void MATRIX_T::Fill(TYPE Value)
{
	long nSize=m_nLi*m_nCo;
	if (m_pData && nSize>0)
	{
		for (long i=0;i<nSize;i++)
			m_pData[i]=Value;
	}
}

CLASS_TYPE
	inline	unsigned long MATRIX_T::GetCo()const
{
	return m_nCo;
}

CLASS_TYPE
	inline	unsigned long MATRIX_T::GetLi() const
{
	return m_nLi;
}

CLASS_TYPE
	inline	bool MATRIX_T::IsEmpty()
{
	return(m_pData==NULL || m_nLi*m_nCo==0L) ;
}







#endif //G2MAT_H__INCLUDED_