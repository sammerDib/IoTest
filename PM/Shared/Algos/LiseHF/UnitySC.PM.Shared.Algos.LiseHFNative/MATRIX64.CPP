#include "RBT.h" //contains all remaining headers 
#include "Matrix.h"
 

const cppc j_(0,1);
const double nz_ = 1.e-15;

#include <math.h>

//basic mathematics

int bSum(int iDim, bool *pArray) { 
	if (pArray == NULL)
		return 0;
	int iSum = 0;
	for (int i = 0; i < iDim; i++)
		if(pArray[i] == true) 
		iSum++; 
	return iSum;
}

int iSum(int iDim, int *pArray) {
	if (pArray == NULL)
		return 0; 
	int iSum = 0;
	for (int i = 0; i < iDim; i++)
		iSum += pArray[i];
	return iSum; 
}

double dSum(int iDim, double *pArray) {
	if (pArray == NULL)
		return 0.0; 
	double dSum = 0.0; 
	for (int i = 0; i < iDim; i++)
		dSum += pArray[i];
	return dSum; 
}

cppc cSum(int iDim, cppc *pArray) {
	if (pArray == NULL)
		return 0.0;
	cppc cSum = 0.0;
	for (int i = 0; i < iDim; i++)
		cSum += pArray[i];
	return cSum;
}


// implementations of the class VArray (array of Vectors)

VArray::VArray( int m, int n ) 
{
	int i;
	Mown = m; 
	Data = new Vector [Mown]; 
	for(i=0; i<Mown; i++)
		Data[i] = Vector(n); //data field is initialized with zeros automatically
}

VArray::VArray(const VArray& Input)
{
	int i;
	Mown = Input.Mown; 
	Data = new Vector [Mown]; 
	for(i=0; i<Mown; i++) //Data[i] = Input.Data[i]; //copy all vectors
	{
		if(Data[i].Nown != Input.Data[i].Nown)
		{
			if(Data[i].Nown)
				delete [] Data[i].Data; 
			Data[i].Nown = Input.Data[i].Nown;
			Data[i].Data = new cppc [Data[i].Nown];
		}
		memcpy( Data[i].Data, Input.Data[i].Data, Data[i].Nown * sizeof(cppc) ); 
	}
}

VArray::VArray( const Vector& V, long iDimSingleVec, long iNoSingleVec )
{
	long i;
	VArray VA( iNoSingleVec, iDimSingleVec ); 

	this->Mown = 0;  this->Data = NULL; //init. 

	for(i=0; i<(long)VA.Mown; i++) //copy
		memcpy(  (VA.Data + i)->Data,  V.Data + i*iDimSingleVec,  iDimSingleVec * sizeof(cppc)  ); 
	*this = VA;
}

VArray::VArray( const Vector& V, long iDimSingleVec, long iNoSingleVecInV, bool bFirstHalf )
{
	long i;
	VArray VA( iNoSingleVecInV/2, iDimSingleVec ); 

	this->Mown = 0;  this->Data = NULL; //init. 

	for(i=0; i<(long)VA.Mown; i++) //copy
	{
		if(bFirstHalf) //down:
			memcpy(  (VA.Data + i)->Data,  V.Data  +  i*iDimSingleVec,  iDimSingleVec * sizeof(cppc)  ); 
		else //up:
			memcpy(  (VA.Data + i)->Data,  V.Data  +  (iNoSingleVecInV/2 + i)*iDimSingleVec,  iDimSingleVec * sizeof(cppc)  ); 
	}
	*this = VA;
}

VArray::VArray( const Vector& V, long iDimSingleVec, long iNoVecInVA, long iSectionNo ) 
{
	long i;
	VArray VA( iNoVecInVA, iDimSingleVec ); 

	this->Mown = 0;  this->Data = NULL; //init.  

	for(i=0; i<(long)VA.Mown; i++) //copy
		memcpy(  (VA.Data + i)->Data,  V.Data  +  (iSectionNo*iNoVecInVA + i)*iDimSingleVec,  iDimSingleVec * sizeof(cppc)  ); 
	*this = VA;
}

VArray::~VArray() 
{
	if (Data)
		for (int i = 0; i < Mown; i++) {
			delete[] Data[i].Data;  Data[i].Data = NULL;  Data[i].Nown = 0; 
		}
		delete[] Data; 	Data = NULL;  Mown = 0; 
}

VArray& VArray::operator = (const VArray& M) 
{
	int i;
	if (Mown != M.Mown) 
	{
	  if (Mown) 
		  delete[] Data; 
	  Mown = M.Mown; 
	  Data = new Vector [Mown];
	}
	for(i=0; i<Mown; i++) //Data[i] = M.Data[i]; 
	{
		if(Data[i].Nown != M.Data[i].Nown)
		{
			if(Data[i].Nown)
				delete [] Data[i].Data; 
			Data[i].Nown = M.Data[i].Nown;
			Data[i].Data = new cppc [Data[i].Nown];
		}
		memcpy( Data[i].Data, M.Data[i].Data, Data[i].Nown * sizeof(cppc) ); 
	}
	return *this;
}

VArray& VArray::operator += (const VArray& M) 
{
	int i, j, iDimVec;
	CComplex  *CThis = NULL,  *CM = NULL;
	
	if(Mown != M.Mown)
		return *this;

	for(i=0; i<Mown; i++)
	{
		iDimVec = Data[i].Nown;
		CThis = reinterpret_cast<CComplex*>( Data[i].Data ); 
		CM = reinterpret_cast<CComplex*>( M.Data[i].Data );
		for(j=0; j<iDimVec; j++) //Data[i].Data[j] += M.Data[i].Data[j];
		{
			CThis->re += CM->re,  
			CThis->im += CM->im; 
			CThis++, CM++; 
		}
	}
	return *this;
}

VArray& VArray::operator -= (const VArray& M) 
{
	int i, j, iDimVec;
	CComplex  *CThis = NULL,  *CM = NULL;
	
	if(Mown != M.Mown)
		return *this;

	for(i=0; i<Mown; i++)
	{
		iDimVec = Data[i].Nown;
		CThis = reinterpret_cast<CComplex*>( Data[i].Data ); 
		CM = reinterpret_cast<CComplex*>( M.Data[i].Data );
		for(j=0; j<iDimVec; j++) //Data[i].Data[j] -= M.Data[i].Data[j];
		{
			CThis->re -= CM->re,  
			CThis->im -= CM->im; 
			CThis++, CM++; 
		}			
	}
	return *this;
}

VArray& VArray::operator *= (const cppc& c) 
{
	int i, j, iDimVec;
	CComplex  *CThis = NULL;
	double dConstRe = c.real(),  dConstIm = c.imag(),  dThisRe, dThisIm; 
	
	for(i=0; i<Mown; i++)
	{
		iDimVec = Data[i].Nown; 
		CThis = reinterpret_cast<CComplex*>( Data[i].Data ); 
		for(j=0; j<iDimVec; j++) //Data[i].Data[j] *= c;
		{
			dThisRe = CThis->re,  dThisIm = CThis->im; 
			CThis->re  =  dThisRe * dConstRe  -  dThisIm * dConstIm;
			CThis->im  =  dThisRe * dConstIm  +  dThisIm * dConstRe; 
			CThis++;
		}			
	}
	return *this;
}







//implementations of class iVector 

iVector::iVector(int Mown) {
	Nown = Mown; Data = new int[Nown];
};

iVector::~iVector() {
	delete[] Data; Data = NULL; Nown = 0;
};

iVector& iVector::operator = (const iVector& M) {
	if (Nown != M.Nown) { if (Nown) delete[] Data; Nown = M.Nown; Data = new int[Nown]; }
	memcpy(Data, M.Data, Nown*sizeof(int));
	return *this;
}

iVector::iVector(const iVector& M) {
	Nown = M.Nown;
	Data = new int[Nown];
	memcpy(Data, M.Data, Nown * sizeof(int));
}

iVector& iVector::operator = (const int d) {
	long i;
	for (i = 0; i<(long)this->Nown; i++)
		this->Data[i] = d;
	return *this;
}

iVector& iVector::operator += (const int& d) {
	long i;
	for (i = 0; i < (long) this->Nown; i++)
		this->Data[i] += d;
	return *this;
}

iVector& iVector::operator -= (const int& d) {
	long i; 
	for (i = 0; i < (long) this->Nown; i++)
		this->Data[i] -= d;
	return *this;
}

iVector& iVector::operator *= (const int& d) {
	long i;
	for (i = 0; i < (long) this->Nown; i++)
		this->Data[i] *= d;
	return *this;
}

iVector& iVector::operator /= (const int& d) {
	long i;
	for (i = 0; i < (long) this->Nown; i++)
		this->Data[i] /= d; 
	return *this;
}

void iVector::vResetValue() {
	for (int i = 0; i < (int)Nown; i++) {
		if (Data[i] > 0)
			Data[i] -= 1;  
	}
};
 




//implementations of class dVector

dVector::dVector(int Mown) { 
	Nown = Mown; Data = new double[Nown];  
};


void dVector::Init(int Mown) {
	delete[] Data; Nown = Mown;  
	Data = new double[Nown]; 
	for (int i = 0; i < Nown; i++)
		Data[i] = 0.0; 
}; 

dVector::~dVector() { 
	delete[] Data; 
	Data = NULL; Nown = 0;  
};

dVector& dVector::operator = (const dVector& M) {
	if (Nown != M.Nown) {
		if (Data) 
			delete[] Data; 
		Data = new double[M.Nown];
	}
	Nown = M.Nown, 
		memcpy(Data, M.Data, Nown * sizeof(double));
	return *this; 
}

dVector::dVector(const dVector& M) {
	Nown = M.Nown;  
	Data = new double[Nown];  memcpy(Data, M.Data, Nown * sizeof(double));
}

dVector& dVector::operator = (const double d) {
	int i; 
	for (i = 0; i < Nown; i++)
		Data[i] = d;
	return *this; 
}

dVector& dVector::operator += (const double& d) {
	int i;
	for (i = 0; i < Nown; i++)
		Data[i] += d;
	return *this; 
}

dVector& dVector::operator -= (const double& d) {
	int i;
	for (i = 0; i < Nown; i++)
		Data[i] -= d;
	return *this;
}

dVector& dVector::operator *= (const double& d) {
	long i;
	for (i = 0; i < Nown; i++)
		Data[i] *= d; 
	return *this;
}

dVector& dVector::operator /= (const double& d) {
	long i;
	for (i = 0; i < Nown; i++)
		Data[i] /= d; 
	return *this;
}

dVector& dVector::operator += (const dVector& dV) {
	double  *D1 = Data,  *D2 = dV.Data,  *D1_end = Data + Nown; 
	if (Nown != dV.Nown) 
		return *this; 
	while (D1 < D1_end) { 
		*D1 += *D2;
		D1++, D2++; 
	}
	return *this; 
}

dVector& dVector::operator -= (const dVector& dV) {
	double  *D1 = Data, *D2 = dV.Data, *D1_end = Data + Nown;
	if (Nown != dV.Nown)
		return *this;
	while (D1 < D1_end) {
		*D1 -= *D2;
		D1++, D2++;
	}
	return *this;
}

dVector& dVector::operator *= (const dMatrix& dM) { //calc dVector = dM * dVector;   W. Iff, 7.7.2021 
	if (Nown != dM.Next) 
		return *this; 
	int i, j;
	double *pdData_Temp = new double[dM.Nown], dTemp, *dpM_Data_i = NULL;
	for (i = 0; i < dM.Nown; i++) { //for all lines of dM: 
		dTemp = 0.0;  dpM_Data_i = dM.Data + i * dM.Next; 
		for (j = 0; j < Nown; j++) //calc. scalar product: 
			dTemp += dpM_Data_i[j] * Data[j]; 
		pdData_Temp[i] = dTemp; 
	}//i
	delete[] Data; 
	Data = pdData_Temp;  Nown = dM.Nown; 
	return *this; 
}

dVector dVector::operator * (const dMatrix& dM) {
	dVector C(dM.Nown); 
	if (Nown != dM.Next) 
		return C; 
	double *vData, *vm = dM.Data, *vc = C.Data, *vcDataStop = C.Data + C.Nown, *vDataStop = Data + Nown;
	
	for (vc = C.Data; vc < vcDataStop; vc++) { //for all lines of dM 
		*vc = 0.0;   
		for (vData = Data; vData < vDataStop; vData++) 
		{
			*vc += (*vm) * (*vData); 
			vm++;   
		}
	}//end for all lines of dM
	return C; 
}

double dVector::norm() {
	int i; 
	double dNormTmp = 0.0, *pdDataTemp = Data;
	for (i = 0; i < Nown; i++) { 
		dNormTmp += (*pdDataTemp) * (*pdDataTemp); 
		pdDataTemp++; 
	}
	dNormTmp = sqrt(dNormTmp); 
	return dNormTmp; 
} 

double dVector::norm_sq() {
	int i;
	double dNormSq = 0.0, *pdDataTemp = Data;
	for (i = 0; i < Nown; i++) {
		dNormSq += (*pdDataTemp) * (*pdDataTemp);
		pdDataTemp++;
	}
	return dNormSq; 
}



// implementations of the class Vector


Vector::Vector(int Mown) {
	Nown = Mown;
	if (Mown > 0)
		Data = new cppc[Nown];
	else
		Data = NULL; 
}

void Vector::Init(int Mown) { 
	delete[] Data; Nown = Mown; 
	if (Mown > 0)
		Data = new cppc[Nown];
	else
		Data = NULL; 
}

Vector::Vector( const Vector *M ) {
	if( M )
	{
	  Nown = M->Nown; 
	  Data = new cppc[Nown];
	  memcpy( Data, M->Data, Nown*sizeof(cppc) ); 
	}
	else
	{
		Nown = 0;  
		Data = NULL; 
	}
}

Vector::Vector(const Vector& M) {
  Nown = M.Nown; 
  Data = new cppc[Nown];
  memcpy(Data, M.Data, Nown*sizeof(cppc));
}

Vector::Vector( const Vector& V1, const Vector& V2 ) {
  Nown = V1.Nown + V2.Nown; 
  Data = new cppc[Nown];
  memcpy( Data,  V1.Data,  V1.Nown*sizeof(cppc) );
  memcpy( Data+V1.Nown,  V2.Data,  V2.Nown*sizeof(cppc) );
}

Vector::Vector( const Vector& V, long iFirst, long iLast )
{
	Nown = iLast-iFirst + 1; //last entry - first entry + 1 
	Data = new cppc[Nown];
	memcpy( Data, V.Data+iFirst, Nown*sizeof(cppc) ); 
}

Vector::Vector( const VArray& VA )
{
	long i, iDimVec = VA.Data[0].Nown,  iDimTot = VA.Mown * VA.Data[0].Nown;
	Nown = iDimTot;  Data = new cppc[iDimTot];

	for(i=0; i<(long)VA.Mown; i++)
		memcpy(  Data + i*iDimVec,  VA.Data[i].Data,  iDimVec * sizeof(cppc)  ); 
}

Vector::Vector( const VArray& VA, const VArray& VB ) 
{
	long i,  iDimVecA = VA.Data[0].Nown,  iDimVecB = VB.Data[0].Nown;   
	long  iDimTot  =  (long) VA.Mown * VA.Data[0].Nown  +  (long) VB.Mown * VB.Data[0].Nown;
	Nown = iDimTot;  Data = new cppc[iDimTot];

	for(i=0; i<(long)VA.Mown; i++)
		memcpy(  Data + i*iDimVecA,  VA.Data[i].Data,  iDimVecA * sizeof(cppc)  ); 
	for(i=0; i<(long)VB.Mown; i++)
		memcpy(  Data + VA.Mown*iDimVecA + i*iDimVecB,  VB.Data[i].Data,  iDimVecB * sizeof(cppc)  ); 
}

Vector::Vector( const VArray& VA, const VArray& VB, const VArray& VC ) 
{
	long i,  iDimVecA = VA.Data[0].Nown,  iDimVecB = VB.Data[0].Nown,   iDimVecC = VC.Data[0].Nown;   
	long  iDimTot  =  (long) VA.Mown * VA.Data[0].Nown  +  (long) VB.Mown * VB.Data[0].Nown  +  (long) VC.Mown * VC.Data[0].Nown;
	Nown = iDimTot;  Data = new cppc[iDimTot];

	for(i=0; i<(long)VA.Mown; i++)
		memcpy(  Data + i*iDimVecA,  VA.Data[i].Data,  iDimVecA * sizeof(cppc)  ); 
	for(i=0; i<(long)VB.Mown; i++)
		memcpy(  Data + VA.Mown*iDimVecA + i*iDimVecB,  VB.Data[i].Data,  iDimVecB * sizeof(cppc)  ); 
	for(i=0; i<(long)VC.Mown; i++)
		memcpy(  Data + VA.Mown*iDimVecA + VB.Mown*iDimVecB + i*iDimVecC,  VC.Data[i].Data,  iDimVecC * sizeof(cppc)  );
}

Vector::~Vector() {
	delete[] Data;
}

Vector& Vector::operator = (const Vector& M) {
  if (Nown != M.Nown) {
	  if (Nown) 
		  delete[] Data; 
	  Nown = M.Nown; 
	  Data = new cppc[Nown];
  }
  memcpy(Data, M.Data, Nown*sizeof(cppc));
  return *this;
}

bool Vector::operator == (const Vector& M) {
	int i;
	if (Nown != M.Nown)
		return false; 
	for (i = 0; i < Nown; i++) {
		if (Data[i] != M.Data[i])
			return false;
	}
	return true; 
}

Vector& Vector::operator += (const Vector& M) {
  cppc *D1 = Data;
  cppc *D2 = M.Data;
  cppc *D1end = Data + Nown;
  if ((*this).Nown != M.Nown) return *this; // exception ???
  while (D1 < D1end) *D1++ += *D2++;
  return *this;
}

Vector& Vector::operator -= (const Vector& M) {
  cppc *D1 = Data;
  cppc *D2 = M.Data;
  cppc *D1end = Data + Nown;
  if ((*this).Nown != M.Nown) return *this; // exception ???
  while (D1 < D1end) *D1++ -= *D2++;
  return *this;
}

Vector Vector::operator = (const cppc& c) {
	long i, iDim = (long)this->Nown; 
	CComplex *CTmp = reinterpret_cast<CComplex*>(this->Data), C;
	
	C.re = c.real();  C.im = c.imag(); 
	for(i=0; i<iDim; i++)  
	{
		*CTmp = C;  
		CTmp++;
	}

	return *this;
}

Vector Vector::operator *= (const cppc& c) {
	long i, iDim = (long)this->Nown; 
	double dcRe = c.real(), dcIm = c.imag();
	CComplex *CData = reinterpret_cast <CComplex*>(this->Data); 
	double dDataRe, dDataIm;

	for(i=0; i<iDim; i++) //this->Data[i] *= c;  
	{
		dDataRe = CData->re;  dDataIm = CData->im; 
		CData->re = dDataRe * dcRe - dDataIm * dcIm; 
		CData->im = dDataRe * dcIm + dDataIm * dcRe;
		CData++;
	}
	return *this;
} 

cppc Vector::operator * (const Vector& B) {
  if (Nown != B.Nown) return 0.; // exception ???
  cppc tc = 0.; double ta, taa, tb, tbb, *va, *vb, *vc, *vd;
  //for (int i=0; i<Nown; i++) tc += B.Data[i]*conj(Data[i]);
  va = reinterpret_cast <double*>(Data); vb = reinterpret_cast <double*>(B.Data);
  vc = reinterpret_cast <double*>(&tc); vd=va+2*Nown; 
  while (va<vd) {ta = *va++; taa = *va++; tb = *vb++; tbb = *vb++; *vc += ta*tb+taa*tbb; *(vc+1) += ta*tbb-taa*tb;} 
  return tc;
}

Vector Vector::operator * (const Matrix& B) {
  Vector C(B.Next);
  if (Nown != B.Nown) return C; // exception ???
  double *va, *vb, *vc, *vaa, *vbb, *vcc, ta, taa, tb, tbb;
  va = reinterpret_cast <double*>(Data); vb = reinterpret_cast <double*>(B.Data); 
  vc = reinterpret_cast <double*>(C.Data); vbb = vb+2*B.Nown*B.Next;
  for (vcc = vc+2*B.Next; vc<vcc; vc+=2) {
	*vc = *(vc+1) = 0.; 
	for (vaa=vb; vaa<vbb; vaa += 2*B.Next) 
	  {ta = *va++; taa = *va++; tb = *vaa; tbb = *(vaa+1); *vc += ta*tb-taa*tbb; *(vc+1) += ta*tbb+taa*tb;}
	va -= 2*Nown; vb += 2;
  }
  return C;
}



void Vector::GMResG(const Matrix &M) {
	const int knn = 1000;
	double *va, *vb, tvr, tvi, tv1r, tv1i, tv2r, tv2i;
	Vector B(Nown),  BB(Nown), R(*this);
	Vector *V[knn+2], *W[knn+2];
	int k, kk = 0, i, j;
	double tv, D[knn+2], max, lmax;
	cppc tc;
	for (i=0; i<Nown; i++) BB.Data[i] = 0.; 

	k = 0; V[0] = new Vector(Nown); W[0] = new Vector(Nown); *V[0] = R; goto lbl3;
	do {
		tv = 0.; for (i=0; i<Nown; i++) {tc = W[k]->Data[i]; tv += abs(tc*tc);} D[k] = 1./tv;
		tc = 0.; for (i=0; i<Nown; i++) tc += R.Data[i]*conj(W[k]->Data[i]); tc *= D[k];   	   
		for (i=0; i<Nown; i++) BB.Data[i] += V[k]->Data[i]*tc; 
		for (i=0; i<Nown; i++) B.Data[i] = W[k]->Data[i]*tc; R -= B; 
		max = 0; for (i=0; i<Nown; i++) {tv = abs(B.Data[i]); if (tv > max) max = tv;}
		if (k) {
			if (max <= 1.e-10) goto lbl1; 
			//if (((max > aa*lmax)&&(k >= kn))||(k > knn)) {for (i=0; i<=k; i++) {delete W[i]; delete V[i];} goto lbl2;} 
		}
		lmax = max; kk++; k++; 
		V[k] = new Vector(Nown); W[k] = new Vector(Nown); 
		*V[k] = *W[k-1]; 
		//cout<<k<<" "<<max<<endl; getch();
lbl3:
		*W[k] = (*V[k])*M;
		B = *W[k]; 
		for (j=0; j<k; j++) {
			va = reinterpret_cast <double*>(W[j]->Data); vb = reinterpret_cast <double*>(B.Data); tvr = tvi = 0.;
			for (i=0; i<Nown; i++) 
				{tv1r = *va++; tv2r = *vb++; tv1i = *va++; tv2i = *vb++; tvr += tv1r*tv2r + tv1i*tv2i; tvi += tv2i*tv1r - tv2r*tv1i;}
			tc = cppc(tvr,tvi); tc *= D[j]; tvr = tc.real(); tvi = tc.imag();
			va = reinterpret_cast <double*>(W[j]->Data); vb = reinterpret_cast <double*>(W[k]->Data); 
			for (i=0; i<Nown; i++) {tv1r = *va++; tv1i = *va++; *vb++ -= tvr*tv1r - tvi*tv1i; *vb++ -= tvr*tv1i + tvi*tv1r;}
			va = reinterpret_cast <double*>(V[j]->Data); vb = reinterpret_cast <double*>(V[k]->Data); 
			for (i=0; i<Nown; i++) {tv1r = *va++; tv1i = *va++; *vb++ -= tvr*tv1r - tvi*tv1i; *vb++ -= tvr*tv1i + tvi*tv1r;} 
		}
	} while (1);
lbl1:
	for (i=0; i<k; i++) {delete W[i]; delete V[i];}
	//fprintf( Fres, "Function GMResG: kk = %d,  residual = %g \n", kk, max );  //cout<<kk<<" error: "<<max<<endl;
	*this = BB;
}

void Vector::GMResG(void *PP, Vector (*mul)(const Vector&, void*)) {
	//cout.precision(14);
	const int kn = 1000;
	double *va, *vb, tvr, tvi, tv1r, tv1i, tv2r, tv2i;
	Vector B(Nown), BB(Nown), R(*this);
	Vector *V[kn+2], *W[kn+2];
	int k, kk = 0, i, j;
	double tv, D[kn+2], max;
	cppc tc;
	kk = 0; for (i=0; i<Nown; i++) BB.Data[i] = 0.; 
lbl2:
	k = 0; V[0] = new Vector(Nown); W[0] = new Vector(Nown); *V[0] = R;
	goto lbl3;
	do {
		tv = 0.; for (i=0; i<Nown; i++) {tc = W[k]->Data[i]; tv += abs(tc*tc);} D[k] = 1./tv;
		tc = 0.; for (i=0; i<Nown; i++) tc += R.Data[i]*conj(W[k]->Data[i]); tc *= D[k];   	   
		for (i=0; i<Nown; i++) BB.Data[i] += V[k]->Data[i]*tc;
		for (i=0; i<Nown; i++) R.Data[i] -= W[k]->Data[i]*tc; 
		max = 0; for (i=0; i<Nown; i++) {tv = abs(R.Data[i]); if (tv > max) max = tv;}
		if (max <= 1.e-10) goto lbl1;
		if (k > kn) {for (i=0; i<=k; i++) {delete W[i]; delete V[i];} goto lbl2;}
		kk++; k++; V[k] = new Vector(Nown); W[k] = new Vector(Nown); *V[k] = *W[k-1]; 
//		cout<<k<<" err "<<max<<endl; //getch();
lbl3:
		B = *W[k] = mul(*V[k],PP);
		for (j=0; j<k; j++) {
			va = reinterpret_cast <double*>(W[j]->Data); vb = reinterpret_cast <double*>(B.Data); tvr = tvi = 0.;
			for (i=0; i<Nown; i++) 
				{tv1r = *va++; tv2r = *vb++; tv1i = *va++; tv2i = *vb++; tvr += tv1r*tv2r + tv1i*tv2i; tvi += tv2i*tv1r - tv2r*tv1i;}
			tvr *= D[j]; tvi *= D[j];
			va = reinterpret_cast <double*>(W[j]->Data); vb = reinterpret_cast <double*>(W[k]->Data); 
			for (i=0; i<Nown; i++) {tv1r = *va++; tv1i = *va++; *vb++ -= tvr*tv1r - tvi*tv1i; *vb++ -= tvr*tv1i + tvi*tv1r;}
			va = reinterpret_cast <double*>(V[j]->Data); vb = reinterpret_cast <double*>(V[k]->Data); 
			for (i=0; i<Nown; i++) {tv1r = *va++; tv1i = *va++; *vb++ -= tvr*tv1r - tvi*tv1i; *vb++ -= tvr*tv1i + tvi*tv1r;} 
		}
	} while (1);
lbl1:
	for (i=0; i<=k; i++) {delete W[i]; delete V[i];}
	//fprintf( Fres, "Function GMResG: kk = %d,  residual = %g \n", kk, max );  //cout<<kk<<" error: "<<max<<endl;
	*this = BB;
}



void Vector::GMResAG(const Matrix& M) {
	const int knn = 1000;
	int i, k, kk;
	double tv;
	double *pv1, *pv2, tvr, tvi, tv1r, tv1i, tv2r, tv2i;
	cppc tc, ss[knn], cc[knn], gg[knn+1], y[knn], *h1, *h2;
	Vector Z(*this), *V[knn+1], *H[knn+1];

	for (i=0; i<knn+1; i++) gg[i] = 0.;

	kk = 0; V[0] = new Vector (Nown);
	gg[0] = tv = Z.norm();
	if (tv>1.e-10) tv = 1./tv;
	for (i=0; i<Nown; i++) V[0]->Data[i] = tv*Z(i);

	do {
		V[kk+1] = new Vector(Nown); H[kk] = new Vector(kk+2);

		*V[kk+1] = (*V[kk])*M;
		for (k=0; k<kk+1; k++) {
			H[kk]->Data[k] = tc = (*V[k])*(*V[kk+1]); tvr = tc.real(); tvi = tc.imag();
			pv1 = reinterpret_cast<double*>(V[kk+1]->Data); pv2 = reinterpret_cast<double*>(V[k]->Data);
			for (i=0; i<Nown; i++) {tv1r = *pv2++; tv1i = *pv2++; *pv1++ -= (tvr*tv1r - tvi*tv1i); *pv1++ -= (tvr*tv1i + tvi*tv1r);}
			//for (i=0; i<Nown; i++) V[kk+1]->Data[i] -= tc*V[k]->Data[i];
		}

		tv = (*V[kk+1]).norm();
		H[kk]->Data[kk+1] = tv;
		if ( fabs(tv)>1.e-10 ) {tv = 1./tv; for (i=0; i<Nown; i++) V[kk+1]->Data[i] *= tv;}

		h1 = h2 = H[kk]->Data; h2++;
		for (k=0; k<kk; k++, h1++, h2++) {
			*h1 = cc[k]*(tc = *h1) + ss[k]* *h2;
			*h2 = -conj(ss[k])*tc + cc[k]* *h2;
		}

		h1 = h2 = H[kk]->Data + kk; h2++;
		if ( abs(*h2)<1.e-10 ) {ss[kk] = 0.; cc[kk] = 1.;}
		else if ( abs(*h1)<1.e-10 ) {ss[kk] = conj(*h2)/abs(*h2); cc[kk] = 0.;}
		else {
			tv = 1./sqrt( std::norm(*h1) + std::norm(*h2) );
			cc[kk] = tv*abs(*h1); ss[kk] = tv*conj(*h2)*(*h1)/abs(*h1);
		}
		*h1 = cc[kk]*(tc = *h1) + ss[kk]* *h2;
		*h2 = -conj(ss[kk])*tc + cc[kk]* *h2;
		gg[kk+1] = -gg[kk]*conj(ss[kk]); gg[kk] *= cc[kk];

		tv = abs(gg[kk+1]);
		//cout<<kk<<" residual "<<tv<<endl;
		kk++;
	} while ( (kk<knn) && (tv>1.e-10) );

	//cout<<kk<<" resudial: "<<abs(gg[kk+1])<<endl;
	for (k=kk-1; k>=0; k--) {
		y[k] = gg[k];
		for (i=kk-1; i>k; i--) y[k] -= y[i]*H[i]->Data[k];
		y[k] /= H[k]->Data[k];
	}
	for (i=0; i<Nown; i++) Z.Data[i] = 0.;
	for (k=0; k<kk; k++) {
		tvr = y[k].real(); tvi = y[k].imag();
		pv1 = reinterpret_cast<double*>(Z.Data); pv2 = reinterpret_cast<double*>(V[k]->Data);
		for (i=0; i<Nown; i++) {
			tv2r = *pv2++; tv2i = *pv2++;
			*pv1++ += tvr*tv2r - tvi*tv2i; *pv1++ += tvr*tv2i + tvi*tv2r;
		}
//		for (i=0; i<Nown; i++) Z.Data[i] += y[k]*V[k]->Data[i];
	}
	for (k=0; k<kk; k++) {delete V[k]; delete H[k];} delete V[kk];
	*this = Z;
}




void Vector::GMResAG( void *PP,  Vector (*mul)(const Vector&, void*),  int MaxNoRestarts, int MaxNoIter, double Tol, BOOL *bFailure,
					  int *NoIter,  double (*GetPrecisionDemand)(const Vector&, void*), void *Arg, BOOL *bDoTolCheck, Vector *InitGuess, 
					  BOOL bOutput  )    
//Algebraic version of GMRES WITH restarts.  Solves A*x = b,  with b = *this.Data,  A*x is given by *mul.  PP: pointer to matrix A, 
//mul: function performing the operation A*x.   This function is used by W. since 20.5.13
//MaxNoRestarts: max. no. of permitted restarts, MaxNoIter: max. permitted no. of GMRES-iterations, Tol: tolerance
//(demanded precision), *bFailure: reports success of GMRES. InitGuess: pointer to initial guess; if NULL: no init. guess is given 
//Introduced by W. at 22.7.13
{
	bool bRestart;
	const int knn = 1000; //No. of iterations
	int i, k, kk, NoRestarts = -1; //counts number of restarts done
	double tv;
	double *pv1, *pv2, 
		   tvr, tvi, tv1r, tv1i, tv2r, tv2i; //temp var real, ~imag
	double dResidualInit;
	cppc tc, ss[knn], cc[knn], //ss: sinus, cc; cosine
		 gg[knn+1], y[knn], //"H * z(k) = d",  here: H * y = gg 
		 *h1, *h2; //2 entries in a column of H
	Vector Z(Nown), ZZ(Nown), //Z: will be the solution later, ZZ right side b of eq.,  Nown: dimension (No. of unknowns)
		   *V[knn+1], *H[knn+1]; //V: contains basis vectors, H: Hessenberg matrix 


	*bFailure = FALSE;  *NoIter = 0;  //initialization

	if(InitGuess) //set init. value for Z
		Z = *InitGuess; //usage of pre-knowledge
	else
	{
		for (i=0; i<Nown; i++) //init guess: zero
			Z.Data[i] = 0.; 
	}
	if(InitGuess) //set righ side
		ZZ = *this - mul(Z,PP); //ZZ = b - A * x_solution
	else
		ZZ = *this; //ZZ = b
	dResidualInit = ZZ.norm();
	
    //if(bOutput) fprintf( Fres, "GMRES, dResidualInit: %g \n", dResidualInit ); 

	if( dResidualInit <= 0.01 * Tol ) //alternatively: if( dResidualInit <= Tol ).  Initial guess Z = solution here => exit GMRES
	{
		*this = Z;  return; 
	}

	do {	// Loop over restarts
		for (i=0; i<knn+1; i++) 
			gg[i] = 0.;	//initializ. of d(k)
		kk = 0; 
		V[0] = new Vector(Nown);  gg[0] = tv = ZZ.norm(); //first residual, here equal to right side |b|
		if (tv > DBL_MIN) 
			tv = 1./tv; 
		else
			break;
		for (i=0; i<Nown; i++) 
			V[0]->Data[i] = tv * ZZ.Data[i]; //normalize basis vector v_0

		do {  // Loop over GMRES-iterations
			V[kk+1] = new Vector(Nown); H[kk] = new Vector(kk+2); //alloc next column of V and H

			//fPrint_c_matr( reinterpret_cast<CComplex*>((*V[kk]).Data), 1, (*V[kk]).Nown, "Vec. before GMRES-Mult.: \n" );
			////ctstart = clock(); /* Start d. Zeitmessung */
			*V[kk+1] = mul(*V[kk],PP); // MATRIX - VECTOR - MULT:  Generate new basis vector
			////ctend = clock();   dtime_Tot  =  (double)(ctend-ctstart) / (double)CLOCKS_PER_SEC; //total comp. time
			////if(bRes)
				////fprintf(Fres, "\n\n\n Total computation time: %fs \n", dtime_Tot);			
			//fPrint_c_matr( reinterpret_cast<CComplex*>((*V[kk+1]).Data), 1, (*V[kk+1]).Nown, "Vec. after GMRES-Mult.: \n" );
			
			for (k=0; k<kk+1; k++) {  // Arnoldi-process: subtract all projections of new vector kk onto old vectors k: 
				H[kk]->Data[k] = tc = (*V[k]) * (*V[kk+1]);   tvr = tc.real();  tvi = tc.imag();
				pv1 = reinterpret_cast<double*>(V[kk+1]->Data);   pv2 = reinterpret_cast<double*>(V[k]->Data);
				for (i=0; i<Nown; i++) {  //for all components:
					tv1r = *pv2++;  tv1i = *pv2++;  
					*pv1++ -= (tvr*tv1r - tvi*tv1i);  
					*pv1++ -= (tvr*tv1i + tvi*tv1r); 
				}
				//for (i=0; i<Nown; i++) V[kk+1]->Data[i] -= tc*V[k]->Data[i]; //this is the meaning of paragraph above
			}

			H[kk]->Data[kk+1] = tv = (*V[kk+1]).norm();	//remaining ortho. comp. of new vector
			if ( fabs(tv) > DBL_MIN ) { //avoid div. by 0
				tv = 1./tv; 
				for (i=0; i<Nown; i++) //normalize new basis vect.
					V[kk+1]->Data[i] *= tv; 
			} //normalize new vect.
			//else: V[kk+1]->Data[...] is close to zero; if this line is passed, it will be the last iteration and  V[kk+1] won't be needed.

			h1 = h2 = H[kk]->Data;  h2++; //apply old Givens-rot. to last column (old Givens-rot. exist not before second iterat.)
			for (k=0;   k < kk;   k++, h1++, h2++) {  //this is not passed in first iteration (for kk=0)
				*h1  =  cc[k] * (tc = *h1)  +  ss[k] * *h2; 
				*h2  =  -conj(ss[k]) * tc   +  cc[k] * *h2; 
			} 

			h1 = h2 = H[kk]->Data + kk;  h2++;  //determine new Givens-rot.
			if ( abs(*h2) <= DBL_MIN ) {
				ss[kk] = 0.;  
				cc[kk] = 1.;
			} 
			else if ( abs(*h1) <= DBL_MIN ) {
				ss[kk] = conj(*h2) / abs(*h2);   
				cc[kk] = 0.;
			}
			else {
				tv  =  1.  /  sqrt( std::norm(*h1) + std::norm(*h2) );  //std::norm(~) means (~)²  
				cc[kk] = tv * abs(*h1); 
				ss[kk] = tv * conj(*h2) * (*h1) / abs(*h1);
			} 
			*h1  =  cc[kk] * (tc = *h1)  +  ss[kk] * *h2;  //apply new Givens-rot. to last column H[kk]
			*h2  =  -conj(ss[kk]) * tc   +  cc[kk] * *h2; 
			gg[kk+1]  =  -gg[kk] * conj(ss[kk]);  //apply new Givens-rot. to right side
			gg[kk]   *=  cc[kk]; 
			tv  =  abs(gg[kk+1]); //cout<<kk<<" residual "<<tv<<endl; //calc. residual tv
			kk++;  (*NoIter)++;

			//Check, whether earlier stop of GMRES makes sense (makes a lager error tolerance sense?) :
			if( GetPrecisionDemand  &&  bDoTolCheck ) //if estimation of accuracy demand is desired:
			{
				if( *bDoTolCheck  &&  tv < 0.1*dResidualInit ) //if accuracy is sufficient for calc. of estimated accuracy demand:
				{
					double ReasonableTol;
					Vector Zt(Z); //Zt: "Z temporary", is only needed for this short query here 
					
					for (k=kk-1; k>=0; k--) //solve "H * z(k) = d"  (here: solve H * y = gg ),  H has triang. structure
					{ 
						y[k] = gg[k]; 
						for (i=kk-1;  i>k;  i--) 
							y[k] -= y[i] * H[i]->Data[k]; 
						y[k] /= H[k]->Data[k]; 
					}//end for 
					for (k=0; k<kk; k++) {  //"x(k) = x(0) + V_k * z(k), here: x(k) = V_k * z(k)";  calc. Z = V_k * y
						tvr = y[k].real();   tvi = y[k].imag();
						pv1 = reinterpret_cast<double*>(Zt.Data);   pv2 = reinterpret_cast<double*>(V[k]->Data);
						for (i=0; i<Nown; i++) { 
							tv2r = *pv2++;  
							tv2i = *pv2++;
							*pv1++ += tvr*tv2r - tvi*tv2i; 
							*pv1++ += tvr*tv2i + tvi*tv2r;
						}
						//for (i=0; i<Nown; i++) Z.Data[i] += y[k]*V[k]->Data[i]; //this is the meaning of the paragraph above
					}//end for
					ReasonableTol = GetPrecisionDemand( Zt, Arg );
					if( ReasonableTol < sqrt(Tol) ) //if ReasonableTol will be smaller than Tol in the future, its calculation is not beneficial here 
						*bDoTolCheck = FALSE;		//any more. Let's avoid its calculation in the future. 
					if( Tol < ReasonableTol )
						Tol = ReasonableTol;
					GetPrecisionDemand = NULL, bDoTolCheck = NULL; //we do this check only once in a GMRES-call.
				}//end if ... == TRUE
			}//end if ... != NULL
		
			//if(bOutput) fprintf( Fres, "GMResAG: No. iter. (with restarts), No. iter without restarts. and residual:  %d \t %d \t %g \n", *NoIter, kk, tv );  //cout<<jk<<" residual: "<<tv<<endl;

		} while (  (kk < knn)  &&  (kk < MaxNoIter)  &&  (tv > Tol)  );  //end of loop over GMRES-iterations

		//if(bOutput) fprintf( Fres, "Function GMResAG: Number of iterations (with restarts) = %d,  residual = %g \n", *NoIter, tv );  //cout<<jk<<" residual: "<<tv<<endl;

		for (k=kk-1; k>=0; k--) //solve "H * z(k) = d"  (here: solve H * y = gg ),  H has triang. structure
		{ 
			y[k] = gg[k]; 
			for (i=kk-1;  i>k;  i--) 
				y[k] -= y[i] * H[i]->Data[k]; 
			y[k] /= H[k]->Data[k]; 
		} 
		for (k=0; k<kk; k++) {  //"x(k) = x(0) + V_k * z(k), here: x(k) = V_k * z(k)";  calc. Z = V_k * y
			tvr = y[k].real();   tvi = y[k].imag();
			pv1 = reinterpret_cast<double*>(Z.Data);   pv2 = reinterpret_cast<double*>(V[k]->Data);
			for (i=0; i<Nown; i++) { 
				tv2r = *pv2++;  
				tv2i = *pv2++;
				*pv1++ += tvr*tv2r - tvi*tv2i; 
				*pv1++ += tvr*tv2i + tvi*tv2r;
			}
			//for (i=0; i<Nown; i++) Z.Data[i] += y[k]*V[k]->Data[i]; //this is the meaning of the paragraph above
		}
		for (k=0; k<kk; k++) {
			delete V[k];  delete H[k];
		} 
		delete V[kk];

		NoRestarts++;
		tv > Tol && NoRestarts < MaxNoRestarts  ?  (bRestart = TRUE)  :  (bRestart = FALSE);
		if(bRestart) //do following time consuming operation only, if ZZ will be needed
			ZZ = *this - mul(Z,PP); //ZZ = b - A * x_solution
	} while ( bRestart );  //end of loop over restarts
	
	if( tv > Tol )
	{
		*bFailure = TRUE;   bError_g = TRUE; 
	}
		
	*this = Z;
}


void Vector::GMResAH(const Matrix& M) {
	const int knn = 500;
	int i, k, kk;
	double tv, nv[knn+1];
	cppc tc, ss[knn], cc[knn], gg[knn+1], y[knn], *h1, *h2;
	Vector Z(*this), *V[knn+1], *H[knn+1];

	for (i=0; i<knn+1; i++) gg[i] = 0.;

	V[0] = new Vector(Nown);
	*V[0] = Z; if (abs(Z(0))>1.e-10) V[0]->Data[0] += Z.norm()*Z(0)/abs(Z(0));
	tc = (*V[0])*(*V[0]); nv[0] = 1./tc.real(); tc = (*V[0])*Z; tc *= 2.*nv[0];
	for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[0]->Data[i];
	gg[0] = Z(0);
	tc = -2.*conj(V[0]->Data[0])*nv[0];
	for (i=0; i<Nown; i++) Z.Data[i] = tc*V[0]->Data[i]; Z.Data[0] += 1.;
	Z = Z*M;
	tc = (*V[0])*Z; tc *= 2.*nv[0];
	for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[0]->Data[i];

	kk = 0;
	do {
		V[kk+1] = new Vector(Nown);
		for (i=0; i<kk+1; i++) V[kk+1]->Data[i] = 0.;
		for (i=kk+1; i<Nown; i++) V[kk+1]->Data[i] = Z(i);
		if ( abs(Z(kk+1))>1.e-10 ) V[kk+1]->Data[kk+1] += Z.norm(kk+1)*Z(kk+1)/abs(Z(kk+1));
		tc = (*V[kk+1])*(*V[kk+1]); nv[kk+1] = 1./tc.real(); tc = (*V[kk+1])*Z; tc *= 2.*nv[kk+1];
		for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[kk+1]->Data[i];

		H[kk] = new Vector(kk+2);
		for (i=0; i<kk+2; i++) {H[kk]->Data[i] = Z(i);}// cout<<" H "<<i<<" "<<Z(i)<<endl;}

		tc = -2.*conj(V[kk+1]->Data[kk+1])*nv[kk+1];
		for (i=0; i<Nown; i++) Z.Data[i] = tc*V[kk+1]->Data[i]; Z.Data[kk+1] += 1.;
		for (k=kk; k>=0; k--) {tc = (*V[k])*Z; tc *= 2.*nv[k]; for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[k]->Data[i];}
		Z = Z*M;
		for (k=0; k<kk+2; k++) {tc = (*V[k])*Z; tc *= 2.*nv[k]; for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[k]->Data[i];}

		h1 = h2 = H[kk]->Data; h2++;
		for (k=0; k<kk; k++, h1++, h2++) {
			*h1 = cc[k]*(tc = *h1) + ss[k]* *h2;
			*h2 = -conj(ss[k])*tc + cc[k]* *h2;
		}

		h1 = h2 = H[kk]->Data + kk; h2++;
		if ( abs(*h2)<1.e-10 ) {ss[kk] = 0.; cc[kk] = 1.;}
		else if ( abs(*h1)<1.e-10 ) {ss[kk] = conj(*h2)/abs(*h2); cc[kk] = 0.;}
		else {
			tv = 1./sqrt( std::norm(*h1) + std::norm(*h2) );
			cc[kk] = tv*abs(*h1); ss[kk] = tv*conj(*h2)*(*h1)/abs(*h1);
		}
		*h1 = cc[kk]*(tc = *h1) + ss[kk]* *h2;
		*h2 = -conj(ss[kk])*tc + cc[kk]* *h2;
		gg[kk+1] = -gg[kk]*conj(ss[kk]); gg[kk] *= cc[kk];

		tv = abs(gg[kk+1]);
		//cout<<kk<<" residual "<<tv<<endl;
		kk++;
	} while ( (kk<knn) && (tv>1.e-10) );

	//fprintf( Fres, "Function GMResAH: kk = %d,  residual = %g \n", kk, abs(gg[kk+1]) );  //cout<<kk<<" resudial: "<<abs(gg[kk+1])<<endl;

	for (k=kk-1; k>=0; k--) {
		y[k] = gg[k];
		for (i=kk-1; i>k; i--) y[k] -= y[i]*H[i]->Data[k];
		y[k] /= H[k]->Data[k];
	}
	for (i=0; i<Nown; i++) Z.Data[i] = 0.;
	for (k=kk-1; k>=0; k--) {
		Z.Data[k] += y[k];
		tc = (*V[k])*Z; tc *= 2.*nv[k]; for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[k]->Data[i];
	}
	for (k=0; k<kk; k++) {delete V[k]; delete H[k];} delete V[kk];
	*this = Z;
}

void Vector::GMResAH(void *PP, Vector (*mul)(const Vector&, void*)) {
	const int knn = 500;
	int i, k, kk;
	double tv, nv[knn+1];
	double *pv1, *pv2, tvr, tvi, tv1r, tv1i;
	cppc tc, ss[knn], cc[knn], gg[knn+1], y[knn], *h1, *h2;
	Vector Z(*this), *V[knn+1], *H[knn+1];

	for (i=0; i<knn+1; i++) gg[i] = 0.;

	V[0] = new Vector(Z);
	if (abs(Z(0))>1.e-10) V[0]->Data[0] += Z.norm()*Z(0)/abs(Z(0));
	else V[0]->Data[0] += Z.norm();
	tc = (*V[0])*(*V[0]); nv[0] = 1./tc.real(); tc = (*V[0])*Z; tc *= 2.*nv[0];
	for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[0]->Data[i];
	gg[0] = Z(0);
	tc = -2.*conj(V[0]->Data[0])*nv[0];
	for (i=0; i<Nown; i++) Z.Data[i] = tc*V[0]->Data[i]; Z.Data[0] += 1.;
	Z = mul(Z,PP);
	tc = (*V[0])*Z; tc *= 2.*nv[0];
	for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[0]->Data[i];

	kk = 0;
	do {
		V[kk+1] = new Vector(Nown);
		for (i=0; i<kk+1; i++) V[kk+1]->Data[i] = 0.;
		for (i=kk+1; i<Nown; i++) V[kk+1]->Data[i] = Z(i);
		if ( abs(Z(kk+1))>1.e-10 ) V[kk+1]->Data[kk+1] += Z.norm(kk+1)*Z(kk+1)/abs(Z(kk+1));
		else V[kk+1]->Data[kk+1] += Z.norm(kk+1);
		tc = (*V[kk+1])*(*V[kk+1]); nv[kk+1] = 1./tc.real(); tc = (*V[kk+1])*Z; tc *= 2.*nv[kk+1];
		pv1 = reinterpret_cast<double*>(Z.Data); pv2 = reinterpret_cast<double*>(V[kk+1]->Data); tvr = tc.real(); tvi = tc.imag();
		for (i=0; i<Nown; i++) {tv1r = *pv2++; tv1i = *pv2++; *pv1++ -= tvr*tv1r - tvi*tv1i; *pv1++ -= tvr*tv1i + tvi*tv1r;}
		//for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[kk+1]->Data[i];

		H[kk] = new Vector(kk+2);
		for (i=0; i<kk+2; i++) {H[kk]->Data[i] = Z(i);}

		tc = -2.*conj(V[kk+1]->Data[kk+1])*nv[kk+1];
		pv1 = reinterpret_cast<double*>(Z.Data); pv2 = reinterpret_cast<double*>(V[kk+1]->Data); tvr = tc.real(); tvi = tc.imag();
		for (i=0; i<Nown; i++) {tv1r = *pv2++; tv1i = *pv2++; *pv1++ = tvr*tv1r - tvi*tv1i; *pv1++ = tvr*tv1i + tvi*tv1r;}
		//for (i=0; i<Nown; i++) Z.Data[i] = tc*V[kk+1]->Data[i];
		Z.Data[kk+1] += 1.;
		for (k=kk; k>=0; k--) {
			tc = (*V[k])*Z; tc *= 2.*nv[k];
			pv1 = reinterpret_cast<double*>(Z.Data); pv2 = reinterpret_cast<double*>(V[k]->Data); tvr = tc.real(); tvi = tc.imag();
			for (i=0; i<Nown; i++) {tv1r = *pv2++; tv1i = *pv2++; *pv1++ -= tvr*tv1r - tvi*tv1i; *pv1++ -= tvr*tv1i + tvi*tv1r;}
			//for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[k]->Data[i];
		}
		Z = mul(Z,PP);
		for (k=0; k<kk+2; k++) {
			tc = (*V[k])*Z; tc *= 2.*nv[k];
			pv1 = reinterpret_cast<double*>(Z.Data); pv2 = reinterpret_cast<double*>(V[k]->Data); tvr = tc.real(); tvi = tc.imag();
			for (i=0; i<Nown; i++) {tv1r = *pv2++; tv1i = *pv2++; *pv1++ -= tvr*tv1r - tvi*tv1i; *pv1++ -= tvr*tv1i + tvi*tv1r;}
			//for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[k]->Data[i];
		}

		h1 = h2 = H[kk]->Data; h2++;
		for (k=0; k<kk; k++, h1++, h2++) {
			*h1 = cc[k]*(tc = *h1) + ss[k]* *h2;
			*h2 = -conj(ss[k])*tc + cc[k]* *h2;
		}

		h1 = h2 = H[kk]->Data + kk; h2++;
		if ( abs(*h2)<1.e-10 ) {ss[kk] = 0.; cc[kk] = 1.;}
		else if ( abs(*h1)<1.e-10 ) {ss[kk] = conj(*h2)/abs(*h2); cc[kk] = 0.;}
		else {
			tv = 1./sqrt( std::norm(*h1) + std::norm(*h2) );
			cc[kk] = tv*abs(*h1); ss[kk] = tv*conj(*h2)*(*h1)/abs(*h1);
		}
		*h1 = cc[kk]*(tc = *h1) + ss[kk]* *h2;
		*h2 = -conj(ss[kk])*tc + cc[kk]* *h2;
		gg[kk+1] = -gg[kk]*conj(ss[kk]); gg[kk] *= cc[kk];

		tv = abs(gg[kk+1]);
		//cout<<kk<<" residual "<<tv<<endl;
		kk++;
	} while ( (kk<knn) && (tv>1.e-10) );
 
	//fprintf( Fres, "Function GMResAH: kk = %d,  residual = %g \n", kk, abs(gg[kk+1]) );  //cout<<kk<<" resudial: "<<abs(gg[kk+1])<<endl;

	for (k=kk-1; k>=0; k--) {
		y[k] = gg[k];
		for (i=kk-1; i>k; i--) y[k] -= y[i]*H[i]->Data[k];
		y[k] /= H[k]->Data[k];
	}
	for (i=0; i<Nown; i++) Z.Data[i] = 0.;
	for (k=kk-1; k>=0; k--) {
		Z.Data[k] += y[k]; tc = (*V[k])*Z; tc *= 2.*nv[k];
		pv1 = reinterpret_cast<double*>(Z.Data); pv2 = reinterpret_cast<double*>(V[k]->Data); tvr = tc.real(); tvi = tc.imag();
		for (i=0; i<Nown; i++) {tv1r = *pv2++; tv1i = *pv2++; *pv1++ -= tvr*tv1r - tvi*tv1i; *pv1++ -= tvr*tv1i + tvi*tv1r;}
//		for (i=0; i<Nown; i++) Z.Data[i] -= tc*V[k]->Data[i];
	}
	for (k=0; k<kk; k++) {delete V[k]; delete H[k];} delete V[kk];
	*this = Z;
}

Vector& Vector::operator /= (const Matrix& A) {
  if (A.Next != A.Nown) return *this; // exception ???
  if (A.Nown != Nown) return *this; // exception ???
  int i, ii;
  Matrix AA(Nown); 
  for (i=0; i<Nown; i++) for (ii=0; ii<Nown; ii++) AA.Data[i*Nown+ii] = A.Data[ii*Nown+i]; 
  *this %= AA; return *this;
}

Vector& Vector::operator %= (const Matrix& A) {
  if (A.Next != A.Nown) return *this; // exception ???
  if (A.Nown != Nown) return *this; // exception ???
  double MaxMod,NewMod;
  const double NearZero = 1E-15;
  Matrix R(A);
  int i, ii, j, jj;
  double *vva, *vvb, *vvc, *vv, va, vb, vc, vd, tv;
  cppc *a = R.Data, *b = Data, *aa, *bb, *aaa = R.Data+Nown*Nown-1, ra; 
    
  for(i=0; i<Nown-1; i++,a+=Nown+1,b++) {
	MaxMod = abs(*a); j = i; vva = reinterpret_cast <double*>(a+Nown);
	for (ii=i+1; ii<Nown; ii++,vva+=2*Nown) {
      va = *vva; vb = *(vva+1); NewMod = sqrt(va*va+vb*vb);
	  if (MaxMod < NewMod) {MaxMod = NewMod; j = ii;}
    }
	if (MaxMod <= NearZero) return *this; // exception ???
	jj = 2*(Nown-i);
	if (j > i) {
	  vva = reinterpret_cast <double*>(a); vvc = reinterpret_cast <double*>(a+(j-i)*Nown); vv = vvc+jj;
	  while(vvc < vv) {tv = *vva; *vva++ = *vvc; *vvc++ = tv;}
	  vva = reinterpret_cast <double*>(b); vvc = reinterpret_cast <double*>(b+(j-i)); 
	  tv = *vva; *vva++ = *vvc; *vvc++ = tv; tv = *vva; *vva++ = *vvc; *vvc++ = tv;
	}
	jj -= 2; 
	//ra = -1./(*a);
	va = a->real(); vb = a->imag(); tv = va*va+vb*vb; ra = cppc(-va/tv,vb/tv);
	for (aa=a+Nown,bb=b+1; aa<aaa; aa+=Nown,bb++) {
	  va = ra.real(); vb = ra.imag(); 
	  tv = aa->real(); vd = aa->imag(); vc = va*tv-vb*vd; vd = va*vd+vb*tv;
      vva = reinterpret_cast <double*>(a+1); vvc = reinterpret_cast <double*>(aa+1); vv = vvc+jj;   	
	  while(vvc < vv) {va = *vva++; vb = *vva++; *vvc++ += va*vc-vb*vd; *vvc++ += va*vd+vb*vc;}
	  vva = reinterpret_cast <double*>(b); vvc = reinterpret_cast <double*>(bb);    	
	  va = *vva++; vb = *vva; *vvc++ += va*vc-vb*vd; *vvc += va*vd+vb*vc;
	}
  }
  for (i=Nown-1; i>=0; i--,a-=Nown+1,b--) {
	//ra = 1./(*a); vc = ra.real(); vd = ra.imag(); 
    va = a->real(); vb = a->imag(); tv = va*va+vb*vb; vc = va/tv; vd = -vb/tv;
	vvc = reinterpret_cast <double*>(b); 	
    va = *vvc; vb = *(vvc+1); *vvc++ = va*vc-vb*vd; *vvc = va*vd+vb*vc;
	vvb = reinterpret_cast <double*>(R.Data+i);    
    for (bb=Data; bb<b; vvb+=2*Nown,bb++) {
      vc = *vvb; vd = *(vvb+1); 
      vva = reinterpret_cast <double*>(b); vvc = reinterpret_cast <double*>(bb);   	
	  va = *vva++; vb = *vva; *vvc++ -= va*vc-vb*vd; *vvc -= va*vd+vb*vc;
	}
  }
  return *this;
}

double Vector::norm(int nn) const {
	if ( nn >= Nown ) return 0.;
	double tv = 0., tvr, tvi, *pv;
	pv = reinterpret_cast<double*>(this->Data) + 2*nn;
	for (int i=nn; i<Nown; i++) {tvr = *pv++; tvi = *pv++; tv += tvr*tvr + tvi*tvi;}
	return sqrt(tv);
}

double Vector::dMinRePart(int nn) {
	int i; 
	double dMin = DBL_MAX; 
	if (nn >= Nown) return 0.0; 
	for(i=nn; i<Nown; i++) 
		if( dMin > Data[i].real() )
			dMin = Data[i].real(); 
	return dMin; 
}

cppc Vector::poly(cppc x) {
  cppc s;
  int i;
  if (abs(x) < 1) {s = x; for (i=Nown-1; i>0; i--) {s += Data[i]; s *= x;} s += Data[0];}
  else {s = Data[0]/x; for (i=1; i<Nown; i++) {s += Data[i]; s /= x;} s += 1;}
  return s;
}

Vector Vector::pcoeff() {
  int i, j;
  Vector A(Nown), B(Nown);
  for (j=1; j<Nown; j++) A.Data[j] = 0.; A.Data[0] = 1.;
  for (i=0; i<Nown; i++) {
    B.Data[0] = 0.; for (j=1; j<Nown; j++) B.Data[j] = A.Data[j-1];
    for (j=0; j<=i; j++) B.Data[j] -= A.Data[j]*Data[i];
    for (j=0; j<Nown; j++) A.Data[j] = B.Data[j];
  }
  return A;
}

Vector Vector::roots() {
  Vector D(Nown);
  if (Nown == 1) {D.Data[0] = -Data[0]; return D;}
  int i, j;
  Matrix M(Nown);
  for (i=0; i<Nown; i++) for (j=0; j<Nown; j++) M.Data[i*Nown+j] = 0.;
  for (i=0; i<Nown; i++) M.Data[i] = -Data[Nown-1-i]; for (i=1; i<Nown; i++) M.Data[i*Nown+i-1] = 1.;
  D = M.diag(); D.qsort(); 
  return D;
}

void qsort(cppc *A, int iLo, int iHi) {
  int Lo, Hi;
  double Mid;
  cppc tc;
  Lo = iLo; Hi = iHi; Mid = real(A[(Lo+Hi)/2]);
  do {
    while (real(A[Lo]) < Mid) Lo++; while (real(A[Hi]) > Mid) Hi--;
    if (Lo <= Hi) {tc = A[Lo]; A[Lo++] = A[Hi]; A[Hi--] = tc;}
  } while (Lo <= Hi);
  if (Hi > iLo) qsort(A,iLo,Hi); if (Lo < iHi) qsort(A,Lo,iHi);
}

void Vector::qsort() {
  int Lo, Hi;
  double Mid;
  cppc tc;
  Lo = 0; Hi = Nown-1; Mid = real(Data[(Lo+Hi)/2]);
  do {
    while (real(Data[Lo]) < Mid) Lo++; while (real(Data[Hi]) > Mid) Hi--;
    if (Lo <= Hi) {tc = Data[Lo]; Data[Lo++] = Data[Hi]; Data[Hi--] = tc;}
  } while (Lo <= Hi);
  if (Hi > 0) ::qsort(Data,0,Hi); if (Lo < Nown-1) ::qsort(Data,Lo,Nown-1);
}

void vQsortData(double *A, int *piTrackingNo, int iLo, int iHi) {  //W.Iff, 23.6.2021  
	int Lo, Hi, iTemp; 
	double Mid, tc;
	Lo = iLo; Hi = iHi; Mid = A[(Lo + Hi) / 2];
	do {
		while (A[Lo] < Mid) Lo++; while (A[Hi] > Mid) Hi--;
		if (Lo <= Hi) { 
			iTemp = piTrackingNo[Lo];  tc = A[Lo];
			piTrackingNo[Lo] = piTrackingNo[Hi];  A[Lo++] = A[Hi];
			piTrackingNo[Hi] = iTemp;  A[Hi--] = tc;
		}
	} while (Lo <= Hi);
	if (Hi > iLo) vQsortData(A, piTrackingNo, iLo, Hi); if (Lo < iHi) vQsortData(A, piTrackingNo, Lo, iHi);
} 

void vQsortData(int iDim, double *dValue, int *piTrackingNo) {  //W.Iff, 23.6.2021 
	int Lo, Hi, iTemp;
	double Mid, tc;
	for (int i = 0; i < iDim; i++)
		piTrackingNo[i] = i;
	Lo = 0; Hi = iDim - 1; Mid = dValue[(Lo + Hi) / 2];
	do {
		while (dValue[Lo] < Mid) Lo++; while (dValue[Hi] > Mid) Hi--; 
		if (Lo <= Hi) { 
			iTemp = piTrackingNo[Lo]; tc = dValue[Lo];
			piTrackingNo[Lo] = piTrackingNo[Hi]; dValue[Lo++] = dValue[Hi];
			piTrackingNo[Hi] = iTemp; dValue[Hi--] = tc;
		}
	} while (Lo <= Hi);
	if (Hi > 0) vQsortData(dValue, piTrackingNo, 0, Hi); if (Lo < iDim - 1) vQsortData(dValue, piTrackingNo, Lo, iDim - 1);
}




//---------------------------------------------------------------- Interpolation ---------------------------------------------------------------


bool bParabolicFit_3(double dx, double *pdx, cppc *pcf, cppc *pcCoeff0, cppc *pcCoeff1, cppc *pcCoeff2) //parabola defined by 3 points 
{
	static double dM[9], dDelta, dInv[9], dDenominator, d1_div_Det; //matrix should have double data type for faster calc. 

	dDelta = pdx[0] - dx;		dM[0] = 1.0;   dM[1] = dDelta;   dM[2] = dDelta * dDelta;
	dDelta = pdx[1] - dx;		dM[3] = 1.0;   dM[4] = dDelta;   dM[5] = dDelta * dDelta;
	dDelta = pdx[2] - dx;		dM[6] = 1.0;   dM[7] = dDelta;   dM[8] = dDelta * dDelta;

	//calc M^(-1) :
	dDenominator = dM[0] * dM[4] * dM[8] + dM[1] * dM[5] * dM[6] + dM[2] * dM[3] * dM[7] - dM[6] * dM[4] * dM[2] - dM[7] * dM[5] * dM[0] - dM[8] * dM[3] * dM[1];
	if (fabs(dDenominator) <= FLT_MIN)
		return false;
	d1_div_Det = 1.0 / dDenominator;
	//fitted coefficients or rather value at x are Result = Inv * Fu : 
	if (pcCoeff0) {
		dInv[0] = d1_div_Det * (dM[4] * dM[8] - dM[5] * dM[7]);
		dInv[1] = d1_div_Det * (dM[2] * dM[7] - dM[1] * dM[8]);
		dInv[2] = d1_div_Det * (dM[1] * dM[5] - dM[2] * dM[4]);
		pcCoeff0->real(dInv[0] * pcf[0].real() + dInv[1] * pcf[1].real() + dInv[2] * pcf[2].real());
		pcCoeff0->imag(dInv[0] * pcf[0].imag() + dInv[1] * pcf[1].imag() + dInv[2] * pcf[2].imag());
	}
	if (pcCoeff1) {
		dInv[3] = d1_div_Det * (dM[5] * dM[6] - dM[3] * dM[8]);
		dInv[4] = d1_div_Det * (dM[0] * dM[8] - dM[2] * dM[6]);
		dInv[5] = d1_div_Det * (dM[2] * dM[3] - dM[0] * dM[5]);
		pcCoeff1->real(dInv[3] * pcf[0].real() + dInv[4] * pcf[1].real() + dInv[5] * pcf[2].real());
		pcCoeff1->imag(dInv[3] * pcf[0].imag() + dInv[4] * pcf[1].imag() + dInv[5] * pcf[2].imag());
	}
	if (pcCoeff2) {
		dInv[6] = d1_div_Det * (dM[3] * dM[7] - dM[4] * dM[6]);
		dInv[7] = d1_div_Det * (dM[1] * dM[6] - dM[0] * dM[7]);
		dInv[8] = d1_div_Det * (dM[0] * dM[4] - dM[1] * dM[3]);
		pcCoeff2->real(dInv[6] * pcf[0].real() + dInv[7] * pcf[1].real() + dInv[8] * pcf[2].real());
		pcCoeff2->imag(dInv[6] * pcf[0].imag() + dInv[7] * pcf[1].imag() + dInv[8] * pcf[2].imag());
	}
	return true;
}


bool bParabolicFit_3(double dx, double *pdx, double *pdf, double *pdCoeff0, double *pdCoeff1, double *pdCoeff2) //parabola defined by 3 points 
{
	static double dM[9], dDelta, dInv[9], dDenominator, d1_div_Det; //matrix should have double data type for faster calc. 

	dDelta = pdx[0] - dx;		dM[0] = 1.0;   dM[1] = dDelta;   dM[2] = dDelta * dDelta;
	dDelta = pdx[1] - dx;		dM[3] = 1.0;   dM[4] = dDelta;   dM[5] = dDelta * dDelta;
	dDelta = pdx[2] - dx;		dM[6] = 1.0;   dM[7] = dDelta;   dM[8] = dDelta * dDelta;

	//calc M^(-1) :
	dDenominator = dM[0] * dM[4] * dM[8] + dM[1] * dM[5] * dM[6] + dM[2] * dM[3] * dM[7] - dM[6] * dM[4] * dM[2] - dM[7] * dM[5] * dM[0] - dM[8] * dM[3] * dM[1];
	if (fabs(dDenominator) <= FLT_MIN)
		return false;
	d1_div_Det = 1.0 / dDenominator;
	//fitted coefficients or rather value at x are Result = Inv * Fu : 
	if (pdCoeff0) {
		dInv[0] = d1_div_Det * (dM[4] * dM[8] - dM[5] * dM[7]);
		dInv[1] = d1_div_Det * (dM[2] * dM[7] - dM[1] * dM[8]);
		dInv[2] = d1_div_Det * (dM[1] * dM[5] - dM[2] * dM[4]);
		*pdCoeff0 = dInv[0] * pdf[0] + dInv[1] * pdf[1] + dInv[2] * pdf[2]; 
	}
	if (pdCoeff1) {
		dInv[3] = d1_div_Det * (dM[5] * dM[6] - dM[3] * dM[8]);
		dInv[4] = d1_div_Det * (dM[0] * dM[8] - dM[2] * dM[6]);
		dInv[5] = d1_div_Det * (dM[2] * dM[3] - dM[0] * dM[5]);
		*pdCoeff1 = dInv[3] * pdf[0] + dInv[4] * pdf[1] + dInv[5] * pdf[2]; 
	}
	if (pdCoeff2) {
		dInv[6] = d1_div_Det * (dM[3] * dM[7] - dM[4] * dM[6]);
		dInv[7] = d1_div_Det * (dM[1] * dM[6] - dM[0] * dM[7]);
		dInv[8] = d1_div_Det * (dM[0] * dM[4] - dM[1] * dM[3]);
		*pdCoeff2 = dInv[6] * pdf[0] + dInv[7] * pdf[1] + dInv[8] * pdf[2]; 
	}
	return true;
}


bool bParabolicFit_4(double dx, int iIndexLL, int iIndexMax, double *pdx, cppc *pValue, cppc *pca0, cppc *pca1, cppc *pca2) //parabola defined by 4 points 
//Result is a0 (interpolated function value at dx)
{
	static double dFuReal[3], dFuImag[3], dM[9], dInv[9], d1_div_Det, dDenominator;
	static double dDelta, dWeightL, dWeightR, dDeltaL, dDeltaR;

	dFuReal[0] = pValue[1].real(), dFuReal[1] = pValue[2].real(), dFuReal[2] = 0.0;
	dFuImag[0] = pValue[1].imag(), dFuImag[1] = pValue[2].imag(), dFuImag[2] = 0.0;

	dDelta = pdx[1] - dx;
	dM[0] = 1.0;  dM[1] = dDelta;  dM[2] = dDelta * dDelta;
	dDelta = pdx[2] - dx;
	dM[3] = 1.0;  dM[4] = dDelta;  dM[5] = dDelta * dDelta;
	dM[6] = 1.0;  dM[7] = 0.0;     dM[8] = 0.0;
	if (iIndexLL >= 0) {
		dWeightL = (pdx[2] - dx) / (pdx[2] - pdx[1]);   dDeltaL = pdx[0] - dx;
		dM[7] += dWeightL * dDeltaL;   dM[8] += dWeightL * dDeltaL * dDeltaL;
		dFuReal[2] += dWeightL * pValue[0].real();  dFuImag[2] += dWeightL * pValue[0].imag();
	}
	int iIndexRR = iIndexLL + 3;
	if (iIndexRR <= iIndexMax) {
		dWeightR = (dx - pdx[1]) / (pdx[2] - pdx[1]);   dDeltaR = pdx[3] - dx;
		dM[7] += dWeightR * dDeltaR;   dM[8] += dWeightR * dDeltaR * dDeltaR;
		dFuReal[2] += dWeightR * pValue[3].real();  dFuImag[2] += dWeightR * pValue[3].imag();
	}

	//calc M^(-1) :
	dDenominator = dM[0] * dM[4] * dM[8] + dM[1] * dM[5] * dM[6] + dM[2] * dM[3] * dM[7] - dM[6] * dM[4] * dM[2] - dM[7] * dM[5] * dM[0] - dM[8] * dM[3] * dM[1];
	if (fabs(dDenominator) <= FLT_MIN)
		return false;
	d1_div_Det = 1.0 / dDenominator;
	//value a0 and parabolic coeff a0, a1, a2 at x is Result = Inv * Fu : 
	if (pca0) {
		dInv[0] = d1_div_Det * (dM[4] * dM[8] - dM[5] * dM[7]);
		dInv[1] = d1_div_Det * (dM[2] * dM[7] - dM[1] * dM[8]);
		dInv[2] = d1_div_Det * (dM[1] * dM[5] - dM[2] * dM[4]);
		pca0->real(dInv[0] * dFuReal[0] + dInv[1] * dFuReal[1] + dInv[2] * dFuReal[2]);
		pca0->imag(dInv[0] * dFuImag[0] + dInv[1] * dFuImag[1] + dInv[2] * dFuImag[2]);
	}
	if (pca1) {
		dInv[3] = d1_div_Det * (dM[5] * dM[6] - dM[3] * dM[8]);
		dInv[4] = d1_div_Det * (dM[0] * dM[8] - dM[2] * dM[6]);
		dInv[5] = d1_div_Det * (dM[2] * dM[3] - dM[0] * dM[5]);
		pca1->real(dInv[3] * dFuReal[0] + dInv[4] * dFuReal[1] + dInv[5] * dFuReal[2]);
		pca1->imag(dInv[3] * dFuImag[0] + dInv[4] * dFuImag[1] + dInv[5] * dFuImag[2]);
	}
	if (pca2) {
		dInv[6] = d1_div_Det * (dM[3] * dM[7] - dM[4] * dM[6]);
		dInv[7] = d1_div_Det * (dM[1] * dM[6] - dM[0] * dM[7]);
		dInv[8] = d1_div_Det * (dM[0] * dM[4] - dM[1] * dM[3]);
		pca2->real(dInv[6] * dFuReal[0] + dInv[7] * dFuReal[1] + dInv[8] * dFuReal[2]);
		pca2->imag(dInv[6] * dFuImag[0] + dInv[7] * dFuImag[1] + dInv[8] * dFuImag[2]);
	}

	return true;
}


bool bParabolicFit_4(double dx, int iIndexLL, int iIndexMax, double *pdx, double *pValue, double *pca0, double *pca1, double *pca2) //parabola defined by 4 points 
//Result is a0 (interpolated function value at dx)
{
	static double dFuReal[3], dM[9], dInv[9];
	double dDelta, dWeightL = 0.0, dWeightR = 0.0, dDeltaL, dDeltaR, dScaling_Fac, d1_div_Det, dDenominator;

	dFuReal[0] = pValue[1], dFuReal[1] = pValue[2], dFuReal[2] = 0.0;

	dDelta = pdx[1] - dx;
	dM[0] = 1.0;  dM[1] = dDelta;  dM[2] = dDelta * dDelta;
	dDelta = pdx[2] - dx;
	dM[3] = 1.0;  dM[4] = dDelta;  dM[5] = dDelta * dDelta;
	dM[6] = 0.0;  dM[7] = 0.0;     dM[8] = 0.0;
	if (iIndexLL >= 0) {
		dWeightL = (pdx[2] - dx) / (pdx[2] - pdx[1]);   dDeltaL = pdx[0] - dx;
		dM[6] += dWeightL;   dM[7] += dWeightL * dDeltaL;   dM[8] += dWeightL * dDeltaL * dDeltaL;
		dFuReal[2] += dWeightL * pValue[0];
	}
	int iIndexRR = iIndexLL + 3;
	if (iIndexRR <= iIndexMax) {
		dWeightR = (dx - pdx[1]) / (pdx[2] - pdx[1]);   dDeltaR = pdx[3] - dx;
		dM[6] += dWeightR;   dM[7] += dWeightR * dDeltaR;   dM[8] += dWeightR * dDeltaR * dDeltaR;
		dFuReal[2] += dWeightR * pValue[3];
	}
	if (abs(dM[6]) > NUM_NOISE) {
		dScaling_Fac = 1.0 / dM[6];  	dM[6] = 1.0;   dM[7] *= dScaling_Fac;   dM[8] *= dScaling_Fac;   dFuReal[2] *= dScaling_Fac;
	}
	else if (pdx[2] - pdx[1] > FLT_MIN) {
		if (pca0 != NULL)  	*pca0 = dFuReal[2] * (dx - pdx[1]) / (pdx[2] - pdx[1]) + dFuReal[1] * (pdx[2] - dx) / (pdx[2] - pdx[1]);
		if (pca1 != NULL)	*pca1 = (dFuReal[2] - dFuReal[1]) / (pdx[2] - pdx[1]);
		if (pca2 != NULL)	*pca2 = 0.0;
		return false;
	}
	else {
		if (pca0 != NULL)  *pca0 = dFuReal[1];
		if (pca1 != NULL)  *pca1 = 0.0;
		if (pca2 != NULL)  *pca2 = 0.0;
		return false;
	}

	//calc M^(-1) :
	dDenominator = dM[0] * dM[4] * dM[8] + dM[1] * dM[5] * dM[6] + dM[2] * dM[3] * dM[7] - dM[6] * dM[4] * dM[2] - dM[7] * dM[5] * dM[0] - dM[8] * dM[3] * dM[1];
	if (fabs(dDenominator) <= FLT_MIN)
		return false;
	d1_div_Det = 1.0 / dDenominator;
	//value a0 and parabolic coeff a0, a1, a2 at x is Result = Inv * Fu : 
	if (pca0) {
		dInv[0] = d1_div_Det * (dM[4] * dM[8] - dM[5] * dM[7]);
		dInv[1] = d1_div_Det * (dM[2] * dM[7] - dM[1] * dM[8]);
		dInv[2] = d1_div_Det * (dM[1] * dM[5] - dM[2] * dM[4]);
		*pca0 = dInv[0] * dFuReal[0] + dInv[1] * dFuReal[1] + dInv[2] * dFuReal[2];
	}
	if (pca1) {
		dInv[3] = d1_div_Det * (dM[5] * dM[6] - dM[3] * dM[8]);
		dInv[4] = d1_div_Det * (dM[0] * dM[8] - dM[2] * dM[6]);
		dInv[5] = d1_div_Det * (dM[2] * dM[3] - dM[0] * dM[5]);
		*pca1 = dInv[3] * dFuReal[0] + dInv[4] * dFuReal[1] + dInv[5] * dFuReal[2];
	}
	if (pca2) {
		dInv[6] = d1_div_Det * (dM[3] * dM[7] - dM[4] * dM[6]);
		dInv[7] = d1_div_Det * (dM[1] * dM[6] - dM[0] * dM[7]);
		dInv[8] = d1_div_Det * (dM[0] * dM[4] - dM[1] * dM[3]);
		*pca2 = dInv[6] * dFuReal[0] + dInv[7] * dFuReal[1] + dInv[8] * dFuReal[2];
	}

	return true;
}




// implementations of the class SVector

SVector::SVector(int mown) {
  Mown = mown; Data[0] = new Vector(mown); Data[1] = new Vector(mown);
}
		
SVector::SVector(const SVector& SM) {
  Mown = SM.Mown; Data[0] = new Vector(Mown); *(Data[0]) = SM(0); Data[1] = new Vector(Mown); *(Data[1]) = SM(1);
}

SVector::~SVector() {delete Data[0]; delete Data[1];}

SVector& SVector::operator = (const SVector& SD) {
  if (Mown != SD.Mown) {
    Mown = SD.Mown; delete Data[0]; delete Data[1]; 
	Data[0] = new Vector(SD(0)); Data[1] = new Vector(SD(1));
  }
  else {*(Data[0]) = SD(0); *(Data[1]) = SD(1);}
  return *this;		
}

SVector SVector::operator * (const SMatrix& B) {
  SVector C(Mown);
  if (Mown != B.Mown) return C; // exception ???
  *C.Data[0] = (*Data[0])*B(0,1)+(*Data[1])*B(1,1); *C.Data[1] = (*Data[0])*B(0,0)+(*Data[1])*B(1,0); 
  return C;
}

SVector SVector::operator * (const SMatrixD& B) {
  int i, j; 
  cppc tc;
  SVector C(Mown);
  if (Mown != B.Mown) return C; // exception ???
  for (j=0; j<2; j++) for (i=0; i<Mown; i++) C.Data[j]->Data[i] = Data[0]->Data[i]*B(0,i,1-j)+Data[1]->Data[i]*B(1,i,1-j);
  return C;
}

// implementations of the class dMatrix 


dMatrix::dMatrix(int Mown, int Mext) {
	Nown = Mown;
	Next = (Mext) ? Mext : Mown;
	Data = new double[Nown*Next]; 
}

dMatrix::dMatrix(const dMatrix& M) {
	Nown = M.Nown; Next = M.Next;
	Data = new double[Nown*Next];
	memcpy(Data, M.Data, Nown*Next*sizeof(double));
}
dMatrix::~dMatrix() { 
	delete[] Data; 
}

void dMatrix::Init(int Mown, int Mext) {
	delete[] Data; Nown = Mown, Next = Mext;
	Data = new double[Nown * Next]; 
}

void dMatrix::operator = (const double& d) {
	int iDim = Nown * Next;
	for (int i = 0; i < iDim; i++)
		Data[i] = d;
}

dMatrix& dMatrix::operator *= (const double& d) {
	int iDim = Nown * Next; 
	for (int i = 0; i < iDim; i++) {
		Data[i] *= d;  
	}
	return *this;
}

dMatrix& dMatrix::operator = (const dMatrix& M) {
	if ((Nown != M.Nown) || (Next != M.Next))
	{
		if (Nown*Next) delete[] Data; Nown = M.Nown; Next = M.Next; Data = new double[Nown*Next];
	}
	memcpy( Data, M.Data, Nown * Next * sizeof(double) ); //for (int i = 0; i<Nown*Next; i++) Data[i] = M.Data[i];
	return *this;
}

dMatrix dMatrix::operator * (const dMatrix& B) {
	dMatrix C(Nown, B.Next); 
	if (Next != B.Nown)
		return C; 
	int i, j, iNext_B = B.Next; //save some calc. time and multiply only non-zero entries
	double *pda = NULL, *pda_Start = NULL, *pda_Stop = NULL, *pdb = NULL, *pdc = C.Data, dTemp; 
	
	for (i = 0; i < Nown; i++) {
		pda_Start = Data + i * Next;   pda_Stop = Data + (i + 1) * Next;  
		for (j = 0; j < iNext_B; j++) {
			pda = pda_Start,  pdb = B.Data + j,  dTemp = 0.0; 
			while (pda < pda_Stop) {
				dTemp += (*pda) * (*pdb);
				pda++, pdb += iNext_B;
			}
			*pdc = dTemp;  pdc++; 
		}//j 
	}//i 
	return C;
}


void dMatrix::calc_M_adj_x_M(const dMatrix& A_adj) { //calc. Matrix^t x Matrix - meaning A_adj x A - and exploit the symmetry for acceleration
	if (Nown * Next != A_adj.Nown * A_adj.Nown) {
		delete[] Data;  Data = new double[A_adj.Nown * A_adj.Nown];
	}
	Nown = Next = A_adj.Nown; 

	int i, j, *pIndex_start = new int[Nown], *pIndex_stop = new int[Nown], iIndexStart, iIndexStop; //save some calc. time and multiply only non-zero entries
	double *pda_adj = NULL, *pda_Stop = NULL, *pda_Temp = NULL, *pdb_Temp = NULL, *pdb = NULL, *pdc = NULL, dTemp;

	for (i = 0; i < Nown; i++) { //find non-zero entries  
		pda_adj = A_adj.Data + i * A_adj.Next; 
		for (j = 0; j < A_adj.Next - 1; j++)
			if (pda_adj[j] != 0)
				break;
		pIndex_start[i] = j;
		for (j = A_adj.Next - 1; j > 0; j--)
			if (pda_adj[j] != 0)
				break;
		pIndex_stop[i] = j;
	}
	pdc = Data;
	for (i = 0; i < Nown; i++) {
		for (j = i; j < Next; j++) {
			iIndexStart = max(pIndex_start[i], pIndex_start[j]), iIndexStop = min(pIndex_stop[i], pIndex_stop[j]);   
			pda_Temp = A_adj.Data + i * A_adj.Next;  pdb_Temp = A_adj.Data + j * A_adj.Next;
			pda_adj = pda_Temp + iIndexStart,  pdb = pdb_Temp + iIndexStart,  pda_Stop = pda_Temp + iIndexStop,  dTemp = 0.0;
			while (pda_adj <= pda_Stop) {
				dTemp += (*pda_adj) * (*pdb);
				pda_adj++, pdb++;
			}
			pdc[j] = dTemp;  
		}//j 
		pdc += Next; 
	}//i 
	pdc = Data;
	for (i = 0; i < Nown; i++) {
		for (j = 0; j < i; j++)
			pdc[i * Next + j] = pdc[j * Next + i]; 
	}

	delete[] pIndex_start, delete[] pIndex_stop;  
}


dMatrix& dMatrix::operator /= (const dMatrix& A_) { 
	//Apply householder transformations as on p.124, "Grundlagen der Numerischen Mathematik und des Rechnens", 3rd edition, from Martin Hanke-Bourgeois.  By W. Iff at 7.7.2021
	if (this == &A_) return *this; 
	if (A_.Next != A_.Nown) return *this; 
	if (A_.Nown != Next) return *this; 
	
	int i, j, k, A_Nown = A_.Nown, A_Next = A_.Next; 
	double *Data_A = new double[A_Nown * A_Next];  memcpy(Data_A, A_.Data, A_Nown * A_Next * sizeof(double)); 
	double d2Norm_ai, dAbs_ai0, *Data_Ai = NULL, *Data_Aii = NULL, *Data_Aii_Tmp = NULL, *v = new double[A_Nown], dNorm_v, dBeta,
		*Data_Aj = NULL, *Data_Aij = NULL, dProj_Aj, dProj_j, dTemp;
	double *vTransp_x_Ai = new double[A_Next], *beta_x_v = new double[A_Nown], *Data_i = NULL, *Data_j = NULL, *Data_ij = NULL, 
		*vTransp_x_Data_i = new double[Next], *Data_ji = NULL; 
	
	for (i = 0; i < A_Next; i++) { 
		Data_Ai = Data_A + i * A_Next;    Data_Aii_Tmp = Data_Aii = Data_Ai + i;      Data_i = Data + i * Next;
		// 1. Calc Householder transformation: 
		dAbs_ai0 = fabs(*Data_Aii);  d2Norm_ai = 0.0;  //calc modulus of ai1 and ||ai||_2 
		for (j = i; j < Next; j++) {
			d2Norm_ai += (*Data_Aii_Tmp) * (*Data_Aii_Tmp);
			Data_Aii_Tmp += A_Next;
		} 
		d2Norm_ai = sqrt(d2Norm_ai); dNorm_v = 1.0 / d2Norm_ai;
		Data_Aii_Tmp = Data_Aii;
		for (j = i; j < A_Nown; j++) { //calc vector v
			v[j] = (*Data_Aii_Tmp) * dNorm_v;
			Data_Aii_Tmp += A_Next;
		}
		if (dAbs_ai0 > FLT_MIN) 
			v[i] += (*Data_Aii) / dAbs_ai0;
		else 
			v[i] += 1.0; 
		dBeta = 1.0 / (1.0 + dAbs_ai0 / d2Norm_ai); //calc scalar beta 
		// 2. Apply projection: 
		for (j = i; j < A_Next; j++) { //Left side: calc v^t * Ai: for all columns j
			Data_Aij = Data_Ai + j;  dProj_Aj = 0.0;
			for (k = i; k < A_Nown; k++) { //for all entries k 
				dProj_Aj += v[k] * (*Data_Aij);
				Data_Aij += A_Next; 
			}//k
			vTransp_x_Ai[j] = dProj_Aj;
		}//j
		for (j = 0; j < Next; j++) { //Right side: calc v^t * Data_i: for all columns j
			Data_ij = Data_i + j;  dProj_j = 0.0;
			for (k = i; k < Nown; k++) { //for all entries k 
				dProj_j += v[k] * (*Data_ij);
				Data_ij += Next; 
			}//k
			vTransp_x_Data_i[j] = dProj_j;
		}//j 
		// 3. Execute the mirroring
		for (j = i; j < A_Nown; j++) // calc beta * v 
			beta_x_v[j] = dBeta * v[j]; 
		for (j = i; j < A_Next; j++) { //Left side: Ai = Ai - (beta * v) * (v^t * Ai): 
			dProj_Aj = vTransp_x_Ai[j];  Data_Aj = Data_Ai + j; //for each column of new Ai: 
			for (k = i; k < A_Nown; k++) { //for all entries k  
				*Data_Aj -= beta_x_v[k] * dProj_Aj;
				Data_Aj += A_Next;
			}//k
		}//j 
		for (j = 0; j < Next; j++) { //Left side: Ai = Ai - (beta * v) * (v^t * Ai): 
			dProj_j = vTransp_x_Data_i[j];  Data_j = Data_i + j; //for each column of new Ai: 
			for (k = i; k < Nown; k++) { //for all entries k  
				*Data_j -= beta_x_v[k] * dProj_j;
				Data_j += Next; 
			}//k
		}//j 
	}//i 

	// 4. Invert right upper triang. matrix R (see "Numerik für Ingenieure und Naturwissenschaftler", p.66 by Dahmen and Reusken): 
	for (i = 0; i < Next; i++) { //for all right sides (vectors) i of the equation: 
		Data_i = Data + i;
		for (j = Next - 1; j >= 0; j--) { // go through all entries of the right-hand vector i:  
			dTemp = 0.0;  Data_Aj = Data_A + j * A_Next;
			for (k = j + 1; k < Nown; k++) //calc what to subtract from the j. component of the vector:  
				dTemp += Data_Aj[k] * Data_i[k * Next];
			Data_ji = Data_i + j * Next;
			*Data_ji = (*Data_ji - dTemp) / Data_Aj[j]; //subtract and divide; overwrite j. component of vector with the result 
			if (Data_Aj[j] == 0.0)
				*Data_ji = DBL_MAX; 
		}//j 
	}//i

	delete[] Data_A; 
	delete[] v;
	delete[] vTransp_x_Ai;
	delete[] beta_x_v;
	delete[] vTransp_x_Data_i; 
	return *this;
}


void dMatrix::vSetToUnityMat() { 
	int i, iStop = min(Nown, Next); 
	double *dDataTemp = Data, *dDataStop = Data + Nown * Next; 
	while (dDataTemp < dDataStop) {
		*dDataTemp = 0.0;  
		dDataTemp++; 
	}
	for (i = 0; i < iStop; i++)
		Data[i*Next + i] = 1.0; 
}

void dMatrix::transp(dMatrix& M) {
	int i, iIndexLine, j, NownTemp = M.Next, NextTemp = M.Nown;
	double *DataTemp = new double[NownTemp * NextTemp];
	for (i = 0; i < NownTemp; i++) {
		iIndexLine = i * NextTemp;
		for (j = 0; j < NextTemp; j++)
			DataTemp[iIndexLine + j] = M.Data[j*NownTemp + i];
	} 
	delete[] Data;  Data = DataTemp;   Nown = NownTemp, Next = NextTemp;
}

double dMatrix::norm()
{
	int i, iSize = Nown * Next;  double dNormTemp = 0.0, dRe;
	for (i = 0; i < iSize; i++) { 
		dRe = Data[i]; 
		dNormTemp += dRe * dRe;
	}
	dNormTemp = sqrt(dNormTemp); 
	return dNormTemp; 
}


// implementations of the class Matrix 


Matrix::Matrix(int Mown,int Mext) {
	Nown = Mown;
	Next = (Mext) ? Mext : Mown;       
	Data = new cppc[Nown*Next];
}

Matrix::Matrix(const Matrix& M) {
  Nown = M.Nown; Next = M.Next;
  Data = new cppc[Nown*Next];
  //for (int i=0; i<Nown*Next; i++) Data[i] = M.Data[i];
  memcpy(Data, M.Data, Nown*Next*sizeof(cppc));
}

Matrix::~Matrix() {delete[] Data;}

Matrix& Matrix::operator *= (const double& d) {
	CComplex *CData = reinterpret_cast<CComplex*>(Data);
	for (int i = 0; i < Nown * Next; i++) {
		CData->re *= d;  CData->im *= d;  CData++;
	} 
	return *this;
}

Matrix& Matrix::operator *= (const cppc& c) {
	for (int i = 0; i < Nown * Next; i++)
		Data[i] *= c; 
	return *this;
} 

void Matrix::Init(int Mown, int Mext) { 
	delete[] Data; Nown = Mown, Next = Mext; 
	Data = new cppc[Nown * Next]; 
}

Matrix Matrix::operator * (const cppc& c) 
//W. Iff at 7.6.2017
{ 
	Matrix C(Nown, Next);
		for (int i = 0; i < Nown; i++)
			for (int j = 0; j < Nown; j++)
				C.Data[i*Nown + j] = c * Data[i*Nown + j];
	return C;
}

Matrix& Matrix::operator = (const Matrix& M) {
  if ((Nown != M.Nown)||(Next != M.Next))
    {if (Nown*Next) delete[] Data; Nown = M.Nown; Next = M.Next; Data = new cppc[Nown*Next];}
  for (int i=0; i<Nown*Next; i++) Data[i] = M.Data[i];
  //memcpy(Data, M.Data, Nown*Next*sizeof(cppc));
  return *this;
}
  
void Matrix::operator = (const cppc& c) {
	int iDim = Nown * Next; 
	for (int i = 0; i < iDim; i++)
		Data[i] = c;
}

void Matrix::operator |= (const cppc& c) {
	int iDim = Nown * Next;
	for (int i = 0; i < iDim; i++)
		Data[i] = 0.0;
	for (int i = 0; i < Nown; i++)
		Data[i * Next + i] = c;
} 

Matrix& Matrix::operator += (const Matrix& M) {
  cppc *D1 = Data;
  cppc *D2 = M.Data;
  cppc *D1end = Data + Nown*Next;
  if ((*this).Nown != M.Nown) return *this; // exception ???
  if ((*this).Next != M.Next) return *this; // exception ???
  while (D1 < D1end) *D1++ += *D2++;
  return *this;
}

Matrix& Matrix::operator -= (const Matrix& M) {
  cppc *D1 = Data;
  cppc *D2 = M.Data;
  cppc *D1end = Data + Nown*Next;
  if ((*this).Nown != M.Nown) return *this; // exception ???
  if ((*this).Next != M.Next) return *this; // exception ???
  while (D1 < D1end) *D1++ -= *D2++;
  return *this;
}

Vector Matrix::operator * (const Vector& B) const {
  Vector C(Nown);
  if (Next != B.Nown) 
	  return C; // exception ??? 
  if (Nown != Next) {
	  int i, j, iIndexLine;
	  for (i = 0; i < Nown; i++) { //for all lines
		  iIndexLine = i * Next;  C.Data[i] = 0.0; 
		  for (j = 0; j < Next; j++) //for all columns 
			  C.Data[i] += Data[iIndexLine + j] * B.Data[j];  
	  } 
	  return C; // exception ??? 
  }
  int tt = 2*Nown;
  double *va, *vb, *vc, *vbb, *vcc, ta, taa, tb, tbb;
  va = reinterpret_cast <double*>(Data); vb = reinterpret_cast <double*>(B.Data); 
  vc = reinterpret_cast <double*>(C.Data); 
  for (vcc = vc+2*Nown; vc<vcc; vc+=2) {
	*vc = *(vc+1) = 0.; 
	for (vbb=vb+2*Next; vb<vbb;) 
	  {ta = *va++; taa = *va++; tb = *vb++; tbb = *vb++; *vc += ta*tb-taa*tbb; *(vc+1) += ta*tbb+taa*tb;}
	vb -= tt;
  }
  return C;
}

/*/
Matrix Matrix::operator * (const Matrix& B) {
  Matrix C(Nown, B.Next);
  if (Next != B.Nown) return C; // exception ???
  bool rk = false;
  int ni = Nown, nj = Next, nk = B.Next;
  zXeqAmB(C.Data,B.Data,Data,&ni,&nj,&nk,&rk);
  return C;
}
/*/

Matrix Matrix::operator * (const Matrix& B) 
{
	  Matrix C(Nown, B.Next);
	  if (Next != B.Nown) 
		  return C; // exception ???
	  int tt = 2*Nown*C.Next, tc = 2*C.Next, t = 2*Next*C.Next;
	  double *va, *vb, *vc, *vaa, *vbb, *vcc, tva, tvaa, tvb, tvbb;
	  va = reinterpret_cast <double*>(Data); vb = reinterpret_cast <double*>(B.Data); vc = reinterpret_cast <double*>(C.Data); 
	  for (vcc = vc+tt; vc<vcc;) *vc++ = *vc++ = 0.; vcc = vc-tt+tc;
	  for (vbb=va+2*Nown*Next; va<vbb; vcc+=tc, vb-=t) for (vaa=va+2*Next; va<vaa;) {
		tva = *va++; tvaa = *va++; vc = vcc-tc; 
		while (vc < vcc) {
		  tvb = *vb++; tvbb = *vb++; *vc++ += tva*tvb-tvaa*tvbb; *vc++ += tva*tvbb+tvaa*tvb; 
		}
	  }
	  return C;
}

void Matrix::Diag_x_M_x_Diag(bool bInv_l, Vector *VDiag_l, Vector *VDiag_r, bool bInv_r) {
	int i, j;
	if (VDiag_l != NULL) {
		cppc cl, *cDataTmp = Data;
		for (i = 0; i < Nown; i++) { //next line
			if (bInv_l)
				cl = 1.0 / VDiag_l->Data[i];
			else
				cl = VDiag_l->Data[i];  
			for (j = 0; j < Next; j++) {
				*cDataTmp *= cl;
				cDataTmp++; //next column (or line) 
			}
		}
	}//end if mult from left 
	if (VDiag_r != NULL) {
		cppc cr, *cDataTmp;
		for (i = 0; i < Next; i++) { //next column
			cDataTmp = Data + i;  
			if (bInv_r)
				cr = 1.0 / VDiag_r->Data[i];
			else
				cr = VDiag_r->Data[i]; 
			for (j = 0; j < Nown; j++) {
				*cDataTmp *= cr;
				cDataTmp += Next; //next line
			}
		}
	}//end if mult from right 
}

//
Matrix& Matrix::operator /= (const Matrix& A) {
  if (this == &A) return *this; // exception ???
  if (A.Next != A.Nown) return *this; // exception ???
  if (A.Nown != Next) return *this; // exception ???
  int i, ii;
  Matrix R(Next,Nown), AA(Next);
  for (i=0; i<Next; i++) for (ii=0; ii<Nown; ii++) R.Data[i*Nown+ii] = Data[ii*Next+i];
  for (i=0; i<Next; i++) for (ii=0; ii<Next; ii++) AA.Data[i*Next+ii] = A.Data[ii*Next+i]; 
  R %= AA;
  for (i=0; i<Nown; i++) for (ii=0; ii<Next; ii++) Data[i*Next+ii] = R.Data[ii*Nown+i];
  return *this;
}
//
Matrix& Matrix::operator %= (const Matrix& A) 
{
	if (this == &A) return *this; // exception ???
	if (A.Next != A.Nown) return *this; // exception ???
	if (A.Nown != Nown) return *this; // exception ???
	
	double MaxMod, NewMod;
	const double NearZero = 1E-15;
	Matrix R(A);
	int i, ii, j, jj, t = 2 * Next;
	double *vva, *vvb, *vvc, *vv, va, vb, vc, vd, tv;
	cppc *a = R.Data, *b = Data, *aa, *bb, *aaa = R.Data + Nown*Nown - 1, ra;

	for (i = 0; i < Nown - 1; i++, a += Nown + 1, b += Next) {
		MaxMod = abs(*a); j = i; vva = reinterpret_cast <double*>(a + Nown);
		for (ii = i + 1; ii < Nown; ii++, vva += 2 * Nown) {
			va = *vva; vb = *(vva + 1); NewMod = sqrt(va*va + vb*vb);
			if (MaxMod < NewMod) { MaxMod = NewMod; j = ii; }
		}
		if (MaxMod <= NearZero) return *this; // exception ???
		jj = 2 * (Nown - i);
		if (j > i) {
			vva = reinterpret_cast <double*>(a); vvc = reinterpret_cast <double*>(a + (j - i)*Nown); vv = vvc + jj;
			while (vvc < vv) { tv = *vva; *vva++ = *vvc; *vvc++ = tv; }
			vva = reinterpret_cast <double*>(b); vvc = reinterpret_cast <double*>(b + (j - i)*Next); vv = vvc + t;
			while (vvc < vv) { tv = *vva; *vva++ = *vvc; *vvc++ = tv; }
		}
		jj -= 2;
		//ra = -1./(*a);
		va = a->real(); vb = a->imag(); tv = va*va + vb*vb; ra = cppc(-va / tv, vb / tv);
		for (aa = a + Nown, bb = b + Next; aa < aaa; aa += Nown, bb += Next) {
			va = ra.real(); vb = ra.imag();
			tv = aa->real(); vd = aa->imag(); vc = va*tv - vb*vd; vd = va*vd + vb*tv;
			vva = reinterpret_cast <double*>(a + 1); vvc = reinterpret_cast <double*>(aa + 1); vv = vvc + jj;
			while (vvc < vv) { va = *vva++; vb = *vva++; *vvc++ += va*vc - vb*vd; *vvc++ += va*vd + vb*vc; }
			vva = reinterpret_cast <double*>(b); vvc = reinterpret_cast <double*>(bb); vv = vvc + t;
			while (vvc < vv) { va = *vva++; vb = *vva++; *vvc++ += va*vc - vb*vd; *vvc++ += va*vd + vb*vc; }
		}
	}
	for (i = Nown - 1; i >= 0; i--, a -= Nown + 1, b -= Next) {
		//ra = 1./(*a); vc = ra.real(); vd = ra.imag(); 
		va = a->real(); vb = a->imag(); tv = va*va + vb*vb; vc = va / tv; vd = -vb / tv;
		vvc = reinterpret_cast <double*>(b); vv = vvc + t;
		while (vvc < vv) { va = *vvc; vb = *(vvc + 1); *vvc++ = va*vc - vb*vd; *vvc++ = va*vd + vb*vc; }
		vvb = reinterpret_cast <double*>(R.Data + i);
		for (bb = Data; bb < b; vvb += 2 * Nown, bb += Next) {
			vc = *vvb; vd = *(vvb + 1);
			vva = reinterpret_cast <double*>(b); vvc = reinterpret_cast <double*>(bb); vv = vvc + t;
			while (vvc < vv) { va = *vva++; vb = *vva++; *vvc++ -= va*vc - vb*vd; *vvc++ -= va*vd + vb*vc; }
		}
	}
	return *this;
}


cppc Matrix::det() {
  int i, ii, j, jj = Nown;
  Matrix R(*this);
  cppc tc, tcc;
  double tv, tvv, *va, *vc, *vd, tvr, tvi, tar, tai, tcr, tci, tbr = 1.;
  va = reinterpret_cast <double*>(R.Data);
  for (i=0; i<jj-1; i++) {
	vc = va; tar = *va; tai = *(va+1); tv = tar*tar+tai*tai; j = i;
	for (ii=i+1; ii<jj; ii++) {vc += 2*jj; tcr = *vc; tci = *(vc+1); tvv = tcr*tcr+tci*tci; if (tvv > tv) {j = ii; tv = tvv;}}
	if (j > i) {
	  vc = va+2*jj*(j-i); vd = va; tbr = -tbr;
	  for (ii=i; ii<jj; ii++) {tcr = *vc; tci = *(vc+1); *vc++ = *vd; *vc++ = *(vd+1); *vd++ = tcr; *vd++ = tci;}
	  tar = *va; tai = *(va+1); tv = tar*tar+tai*tai; 
	}
    tar /= tv; tai /= tv; vc = va+2*jj;
    for (ii=i+1; ii<jj; ii++) {
	  vd = va+2; tvr = *vc++; tvi = *vc++; tcr = tar*tvr+tai*tvi; tci = tar*tvi-tai*tvr;
	  for (j=i+1; j<jj; j++) {tvr = *vd++; tvi = *vd++; *vc++ -= tcr*tvr-tci*tvi; *vc++ -= tcr*tvi+tci*tvr;} 
      vc += 2*i;
	}
	va += 2*jj+2;
  } 
  tcr = (*va)*tbr; tci = (*(va+1))*tbr;
  for (i=1; i<jj; i++) {va -= 2*(jj+1); tv = tcr*(*va)-tci*(*(va+1)); tci = tci*(*va)+tcr*(*(va+1)); tcr = tv;}
  return cppc(tcr,tci);
}

Vector Matrix::PVector(cppc la) {
  int i;
  Matrix A(*this); 
  Vector V(Nown);
  double tv;
  for (i=0; i<Nown; i++) {A.Data[i*(Nown+1)] -= la; V.Data[i] = 1.;}
  V %= A; tv = 0.;
  for (i=0; i<Nown; i++) tv += abs(V.Data[i]*V.Data[i]); tv = sqrt(tv);
  for (i=0; i<Nown; i++) V.Data[i] /= tv;
  return V;
}

Vector Matrix::LVector(cppc la) {
  int i;
  Matrix A(*this); 
  Vector V(Nown);
  double tv;
  for (i=0; i<Nown; i++) {A.Data[i*(Nown+1)] -= la; V.Data[i] = 1.;}
  V /= A; tv = 0.;
  for (i=0; i<Nown; i++) tv += abs(V.Data[i]*V.Data[i]); tv = sqrt(tv);
  for (i=0; i<Nown; i++) V.Data[i] /= tv;
  return V;
}

Matrix Matrix::hess() const {
  int i, ii, k, j, jj = Nown;
  Matrix R(*this); 
  cppc tc, tcc, tcs, tcr;
  double tv, tvv, *cs, *vcs, *va, *vb, *vaa, *vbb, tva, tvb, tvc, tvd, tvp, tvq, tvr, tvs;
  cs = new double[2*jj]; va = reinterpret_cast <double*>(R.Data); vaa = va-2;
  for (i=1; i<jj-1; i++) {
    vaa += 2*jj+2; tva = *vaa; tvb = *(vaa+1); tv = tva*tva+tvb*tvb; vbb = vaa; k = i;
	for (j=i+1; j<jj; j++) {vbb += 2*jj; tvc = *vbb; tvd = *(vbb+1); if ((tvv = tvc*tvc+tvd*tvd) > tv) {tv = tvv; k = j;}}
	if (k != i) {
	  vb = vaa; vbb = vaa+2*jj*(k-i);
	  for (ii=i-1; ii<jj; ii++) {tvv = *vb; *vb++ = *vbb; *vbb++ = tvv; tvv = *vb; *vb++ = *vbb; *vbb++ = tvv;}
	  tva = *vaa; tvb = *(vaa+1);  
	} 
	tva /= tv; tvb /= -tv; vbb = vaa+2*jj; vcs = cs; 
	for (j=i+1; j<jj; j++) {
      tvc = *vbb; *vbb++ = 0.; tvd = *vbb; *vbb++ = 0.; 
	  *vcs++ = (tvr = tva*tvc-tvb*tvd); *vcs++ = (tvs = tva*tvd+tvb*tvc); vb = vaa+2;
	  for (ii=i; ii<jj; ii++) {
		tvp = *vb++; tvq = *vb++; *vbb++ -= tvr*tvp-tvs*tvq; *vbb++ -= tvs*tvp+tvr*tvq;
	  } vbb += 2*i-2; 
	}
	if (k != i) {
	  vb = va+2*i; vbb = va+2*k;
	  for (ii=0; ii<jj; ii++) 
	    {tv = *vb; *vb++ = *vbb; *vbb++ = tv; tv = *vb; *vb = *vbb; *vbb = tv; vb += 2*jj-1; vbb += 2*jj-1;}
	} 
	vb = va+2*i;
	for (j=0; j<jj; j++) {
	  vbb = vb+2; vcs = cs; 
	  for (ii=i+1; ii<jj; ii++) 
	    {tvr = *vcs++; tvs = *vcs++; tvp = *vbb++; tvq = *vbb++; *vb += tvr*tvp-tvs*tvq; *(vb+1) += tvs*tvp+tvr*tvq;} 
	  vb += 2*jj;
    }
  }
  delete[] cs; return R;
}

Matrix Matrix::hessg() const {
  int i, ii, j, jj = Nown;
  Matrix R(*this); 
  cppc tc, tcc, tcs, tcr;
  double tv, tvv, *cs, *vcs, *va, *vb, *vaa, *vbb, tva, tvb, tvc, tvd, tvp, tvq, tvs;
  cs = new double[4*jj]; va = reinterpret_cast <double*>(R.Data); vaa = va;
  for (i=1; i<jj-1; i++) {
    vaa += 2*jj; tva = *vaa++; tvb = *vaa; *vaa++ = 0.; tv = tva*tva+tvb*tvb; vcs = cs; vbb = vaa;
	for (j=i+1; j<jj; j++) {
      vbb += 2*(jj-1); tvc = *vbb; *vbb++ = 0.; tvd = *vbb; *vbb++ = 0.; tvv = sqrt(tv += tvc*tvc+tvd*tvd); 
	  *vcs++ = (tva /= tvv); *vcs++ = (tvb /= tvv); *vcs++ = (tvc /= tvv); *vcs++ = (tvd /= tvv); vb = vaa;
	  for (ii=i; ii<jj; ii++) {
		tvp = tva*(*vb)+tvb*(*(vb+1))+tvc*(*vbb)+tvd*(*(vbb+1)); tvq = tva*(*(vb+1))-tvb*(*vb)+tvc*(*(vbb+1))-tvd*(*vbb);
		tvs = tva*(*(vbb+1))+tvb*(*vbb)-tvc*(*(vb+1))-tvd*(*vb); *vbb++ = tva*(*vbb)-tvb*(*(vbb+1))-tvc*(*vb)+tvd*(*(vb+1)); 
		*vbb++ = tvs; *vb++ = tvp; *vb++ = tvq;
	  } vbb -= 2*(jj-i); tva = tvv; tvb = 0.;
	}
	*(vaa-2) = tvv; vb = va+2*i;
	for (j=0; j<jj; j++) {
	  vbb = vb+2; vcs = cs; 
	  for (ii=i+1; ii<jj; ii++) {
        tva = *vcs++; tvb = *vcs++; tvc = *vcs++; tvd = *vcs++; tv = *vbb; 
	    tvp = tva*(*vb)-tvb*(*(vb+1))+tvc*(*vbb)-tvd*(*(vbb+1)); tvq = tva*(*(vb+1))+tvb*(*vb)+tvd*(*vbb)+tvc*(*(vbb+1));
	    tv = tva*(*(vbb+1))-tvb*(*vbb)+tvd*(*vb)-tvc*(*(vb+1)); *vbb++ = tva*(*vbb)+tvb*(*(vbb+1))-tvc*(*vb)-tvd*(*(vb+1)); 
	    *vbb++ = tv; *vb = tvp; *(vb+1) = tvq; 
	  } 
	  vb += 2*jj;
    }
  }
  delete[] cs; return R;
}

Matrix Matrix::hess(Matrix& Q) const {
  int i, ii, k, j, jj = Nown;
  Matrix R(*this); 
  cppc tc, tcc, tcs, tcr;
  double tv, tvv, *cs, *vcs, *va, *vb, *vq, *vaa, *vbb, *vbq, tva, tvb, tvc, tvd, tvp, tvq, tvr, tvs;
  cs = new double[2*jj]; va = reinterpret_cast <double*>(R.Data); vaa = va-2;
  vq = reinterpret_cast <double*>(Q.Data); 
  for (i=1; i<jj-1; i++) {
    vaa += 2*jj+2; tva = *vaa; tvb = *(vaa+1); tv = tva*tva+tvb*tvb; vbb = vaa; k = i;
	for (j=i+1; j<jj; j++) {vbb += 2*jj; tvc = *vbb; tvd = *(vbb+1); if ((tvv = tvc*tvc+tvd*tvd) > tv) {tv = tvv; k = j;}}
	if (k != i) {
	  vb = vaa; vbb = vaa+2*jj*(k-i);
	  for (ii=i-1; ii<jj; ii++) {tvv = *vb; *vb++ = *vbb; *vbb++ = tvv; tvv = *vb; *vb++ = *vbb; *vbb++ = tvv;}
	  tva = *vaa; tvb = *(vaa+1);  
	} 
	tva /= tv; tvb /= -tv; vbb = vaa+2*jj; vcs = cs; 
	for (j=i+1; j<jj; j++) {
      tvc = *vbb; *vbb++ = 0.; tvd = *vbb; *vbb++ = 0.; 
	  *vcs++ = (tvr = tva*tvc-tvb*tvd); *vcs++ = (tvs = tva*tvd+tvb*tvc); vb = vaa+2;
	  for (ii=i; ii<jj; ii++) {
		tvp = *vb++; tvq = *vb++; *vbb++ -= tvr*tvp-tvs*tvq; *vbb++ -= tvs*tvp+tvr*tvq;
	  } vbb += 2*i-2; 
	}
	if (k != i) {
	  vb = va+2*i; vbb = va+2*k; 
	  for (ii=0; ii<jj; ii++) 
	    {tv = *vb; *vb++ = *vbb; *vbb++ = tv; tv = *vb; *vb = *vbb; *vbb = tv; vb += 2*jj-1; vbb += 2*jj-1;}
	  vb = vq+2*i; vbb = vq+2*k; 
	  for (ii=0; ii<jj; ii++) 
	    {tv = *vb; *vb++ = *vbb; *vbb++ = tv; tv = *vb; *vb = *vbb; *vbb = tv; vb += 2*jj-1; vbb += 2*jj-1;}
	} 
	vb = va+2*i; vbq = vq+2*i;
	for (j=0; j<jj; j++) {
	  vbb = vb+2; vcs = cs; 
	  for (ii=i+1; ii<jj; ii++) 
	    {tvr = *vcs++; tvs = *vcs++; tvp = *vbb++; tvq = *vbb++; *vb += tvr*tvp-tvs*tvq; *(vb+1) += tvs*tvp+tvr*tvq;} 
	  vbb = vbq+2; vcs = cs; 
	  for (ii=i+1; ii<jj; ii++) 
	    {tvr = *vcs++; tvs = *vcs++; tvp = *vbb++; tvq = *vbb++; *vbq += tvr*tvp-tvs*tvq; *(vbq+1) += tvs*tvp+tvr*tvq;} 
	  vb += 2*jj; vbq += 2*jj;
    }
  }
  delete[] cs; return R;
}

Matrix Matrix::hessg(Matrix& Q) const {
  int i, ii, j, jj = Nown;
  Matrix R(*this); 
  cppc tc, tcc, tcs, tcr;
  double tv, tvv, *cs, *vcs, *va, *vb, *vq, *vaa, *vbb, *vaq, *vbq, tva, tvb, tvc, tvd, tvp, tvq, tvs;
  cs = new double[4*jj]; va = reinterpret_cast <double*>(R.Data); vaa = va;
  vq = reinterpret_cast <double*>(Q.Data); vaa = va;
  for (i=1; i<jj-1; i++) {
    vaa += 2*jj; tva = *vaa++; tvb = *vaa; *vaa++ = 0.; tv = tva*tva+tvb*tvb; vcs = cs; vbb = vaa;
	for (j=i+1; j<jj; j++) {
      vbb += 2*(jj-1); tvc = *vbb; *vbb++ = 0.; tvd = *vbb; *vbb++ = 0.; tvv = sqrt(tv += tvc*tvc+tvd*tvd); 
	  *vcs++ = (tva /= tvv); *vcs++ = (tvb /= tvv); *vcs++ = (tvc /= tvv); *vcs++ = (tvd /= tvv); vb = vaa;
	  for (ii=i; ii<jj; ii++) {
		tvp = tva*(*vb)+tvb*(*(vb+1))+tvc*(*vbb)+tvd*(*(vbb+1)); tvq = tva*(*(vb+1))-tvb*(*vb)+tvc*(*(vbb+1))-tvd*(*vbb);
		tvs = tva*(*(vbb+1))+tvb*(*vbb)-tvc*(*(vb+1))-tvd*(*vb); *vbb++ = tva*(*vbb)-tvb*(*(vbb+1))-tvc*(*vb)+tvd*(*(vb+1)); 
		*vbb++ = tvs; *vb++ = tvp; *vb++ = tvq;
	  } vbb -= 2*(jj-i); tva = tvv; tvb = 0.;
	}
	*(vaa-2) = tvv; vb = va+2*i; vaq = vq+2*i;
	for (j=0; j<jj; j++) {
	  vbb = vb+2; vbq = vaq+2; vcs = cs; 
	  for (ii=i+1; ii<jj; ii++) {
        tva = *vcs++; tvb = *vcs++; tvc = *vcs++; tvd = *vcs++; tv = *vbb; 
	    tvp = tva*(*vb)-tvb*(*(vb+1))+tvc*(*vbb)-tvd*(*(vbb+1)); tvq = tva*(*(vb+1))+tvb*(*vb)+tvd*(*vbb)+tvc*(*(vbb+1));
	    tv = tva*(*(vbb+1))-tvb*(*vbb)+tvd*(*vb)-tvc*(*(vb+1)); *vbb++ = tva*(*vbb)+tvb*(*(vbb+1))-tvc*(*vb)-tvd*(*(vb+1)); 
	    *vbb++ = tv; *vb = tvp; *(vb+1) = tvq; 
		tvp = tva*(*vaq)-tvb*(*(vaq+1))+tvc*(*vbq)-tvd*(*(vbq+1)); tvq = tva*(*(vaq+1))+tvb*(*vaq)+tvd*(*vbq)+tvc*(*(vbq+1));
	    tv = tva*(*(vbq+1))-tvb*(*vbq)+tvd*(*vaq)-tvc*(*(vaq+1)); *vbq++ = tva*(*vbq)+tvb*(*(vbq+1))-tvc*(*vaq)-tvd*(*(vaq+1)); 
	    *vbq++ = tv; *vaq = tvp; *(vaq+1) = tvq; 
	  } 
	  vb += 2*jj; vaq += 2*jj;
    }
  }
  delete[] cs; return R;
}

Matrix Matrix::qrs() const {
  int i, ii, jj = Nown, k, kk, *pr;
  Matrix R(*this); 
  cppc tc, tcc, tcs, tcr, tca, tcd;
  double tv, tvv, *va, *vb, *vbb, *vaa, *vcs, tva, tvb, tvc, tvd, tvr, tvs, tvp, tvq, *cs, tjn = 0.; 
  k = 0; kk = jj-2; cs = new double[2*jj]; vcs = cs; vaa = va = reinterpret_cast <double*>(R.Data);  
  pr = new int[jj];
  do {   
	do {va += 2*jj; if (k < jj-1) k++; else goto lbl1; tvr = *va++; tvq = *va++;} 
	while ((fabs(tvr) < nz_)&&(fabs(tvq) < nz_)); k--; va -= 2*(jj+1); vb = vaa+2*((kk+1)*jj+kk);
	while ((kk >= k)&&((fabs(*vb) < nz_)&&(fabs(*(vb+1)) < nz_))) {kk--; vb -= 2*jj+2;} if (kk < k) goto lbl1; else vb += 3;
	//cout<<k<<" "<<kk<<" "<<abs(R(k+1,k))<<" "<<abs(R(kk+1,kk))<<endl; getch();
	tvs = *vb--; tvr = *vb--; tvq = *vb--; tvp = *vb; vb -= 2*jj-3; tvd = *vb--; tvc = *vb--; tvb = *vb--; tva = *vb; 
	tvs = 0.5*(tvb-tvs); tvr = 0.5*(tva-tvr);  // tca
	tv = tvr*tvr-tvs*tvs+tvp*tvc-tvq*tvd; tvv = 2.*tvr*tvs+tvp*tvd+tvq*tvc; tvc = sqrt(tv*tv+tvv*tvv); 
	if (tvc < 1.e-30) tvp = tvq = tvc; else
	if (tv < 0) {tvq = sqrt(0.5*(tvc-tv)); tvp = 0.5*tvv/tvq;} else {tvp = sqrt(0.5*(tv+tvc)); tvq = 0.5*tvv/tvp;}  // tcd
	tvc = tvr-tvp; tvd = tvs-tvq; tvr += tvp; tvs += tvq;	if (tvc*tvc+tvd*tvd > tvr*tvr+tvs*tvs) {tvr = tvc; tvs = tvd;} // tc
    tvr = tva-tvr; tvs = tvb-tvs; vb = va; for (ii=k; ii<=kk+1; ii++) {*vb++ -= tvr; *vb -= tvs; vb += 2*jj+1;} vcs = cs;
	for (i=k; i<=kk; i++) {
	  vbb = (vb = va+2*(i-k)*(jj+1))+2*jj; tva = *vb; tvb = *(vb+1); tvc = *vbb; tvd = *(vbb+1); 
	  tv = tva*tva+tvb*tvb; tvv = tvc*tvc+tvd*tvd; 
	  if (tv < tvv) {
		*vcs++ = (tvp = (tva*tvc+tvb*tvd)/tvv); *vcs++ = (tvq = (tvb*tvc-tva*tvd)/tvv);
		pr[i] = 1; *vb++ = *vbb; *vb++ = *(vbb+1); *vbb++ = 0.; *vbb++ = 0.; 
		for (ii=i+1; ii<=kk+1; ii++) {
		  tvc = *vbb; tvd = *(vbb+1); tva = *vb; tvb = *(vb+1);   
		  *vb++ = tvc; *vb++ = tvd; *vbb++ = tva-(tvp*tvc-tvq*tvd); *vbb++ = tvb-(tvq*tvc+tvp*tvd); 
	    } 
	  }
	  else {
		pr[i] = 0; *vbb++ = 0.; *vbb++ = 0.; vb += 2; 
		*vcs++ = (tvp = (tva*tvc+tvb*tvd)/tv); *vcs++ = (tvq = (tva*tvd-tvb*tvc)/tv);
		for (ii=i+1; ii<=kk+1; ii++) {tvc = *vb++; tvd = *vb++; *vbb++ -= tvp*tvc-tvq*tvd; *vbb++ -= tvq*tvc+tvp*tvd;} 
	  }
	} vcs = cs;
    for (i=k; i<=kk+1; i++) { 
      vbb = (vb = va+2*(i-k)*(jj+1))+2; 
	  if (i > k) {
		vb -= 2; vbb -= 2; tvp = *vcs++; tvq = *vcs++; 
		if (pr[i-1]) {
		  tva = *vb; tvb = *(vb+1); tvc = *vbb; tvd = *(vbb+1); 
		  *vb++ = tvc+tvp*tva-tvq*tvb; *vb++ = tvd+tvq*tva+tvp*tvb; *vbb++ = tva; *vbb++ = tvb;
		}
		else {tvc = *vbb++; tvd = *vbb++; *vb++ += tvp*tvc-tvq*tvd; *vb++ += tvq*tvc+tvp*tvd;}
	  }
	  for (ii=i; ii<=kk; ii++) {
        tvp = *vcs++; tvq = *vcs++; 
		if (pr[ii]) {
		  tva = *vb; tvb = *(vb+1); tvc = *vbb; tvd = *(vbb+1); 
		  *vb++ = tvc+tvp*tva-tvq*tvb; *vb++ = tvd+tvq*tva+tvp*tvb; *vbb++ = tva; *vbb++ = tvb;
		}
		else {tvc = *vbb++; tvd = *vbb++; *vb++ += tvp*tvc-tvq*tvd; *vb++ += tvq*tvc+tvp*tvd;}
	  } 
      vcs -= 2*(kk+1-i); tjn += kk-i+2;
	} 
  	vb = va; for (ii=k; ii<=kk+1; ii++) {*vb++ += tvr; *vb += tvs; vb += 2*jj+1;}
	//for (i=0; i<jj; i++) for (j=0; j<jj; j++) cout<<i<<" "<<j<<" "<<R.Data[i*jj+j]<<endl;
  } while (1); 
lbl1:
  //cout<<tjn/double(jj*jj)/jj<<endl;
  delete[] pr; delete[] cs; return R;
}

Matrix Matrix::qrsg() const {
  int i, ii, jj = Nown, k, kk;
  Matrix R(*this); 
  cppc tc, tcc, tcs, tcr, tca, tcd;
  double tv, tvv, *va, *vb, *vbb, *vaa, *vcs, tva, tvb, tvc, tvd, tvr, tvs, tvp, tvq, *cs, tjn = 0.; 
  k = 0; kk = jj-2; cs = new double[4*jj]; vcs = cs; vaa = va = reinterpret_cast <double*>(R.Data);  
  do {   
	do {va += 2*jj; if (k < jj-1) k++; else goto lbl1; tvr = *va++; tvq = *va++;} 
	while ((fabs(tvr) < nz_)&&(fabs(tvq) < nz_)); k--; va -= 2*(jj+1); vb = vaa+2*((kk+1)*jj+kk);
	while ((kk >= k)&&((fabs(*vb) < nz_)&&(fabs(*(vb+1)) < nz_))) {kk--; vb -= 2*jj+2;} if (kk < k) goto lbl1; else vb += 3;
	//cout<<k<<" "<<kk<<" "<<abs(R(k+1,k))<<" "<<abs(R(kk+1,kk))<<endl; getch();
	tvs = *vb--; tvr = *vb--; tvq = *vb--; tvp = *vb; vb -= 2*jj-3; tvd = *vb--; tvc = *vb--; tvb = *vb--; tva = *vb; 
	tvs = 0.5*(tvb-tvs); tvr = 0.5*(tva-tvr);  // tca
	tv = tvr*tvr-tvs*tvs+tvp*tvc-tvq*tvd; tvv = 2.*tvr*tvs+tvp*tvd+tvq*tvc; tvc = sqrt(tv*tv+tvv*tvv); 
	if (tvc < 1.e-30) tvp = tvq = tvc; else
	if (tv < 0) {tvq = sqrt(0.5*(tvc-tv)); tvp = 0.5*tvv/tvq;} else {tvp = sqrt(0.5*(tv+tvc)); tvq = 0.5*tvv/tvp;}  // tcd
	tvc = tvr-tvp; tvd = tvs-tvq; tvr += tvp; tvs += tvq;	if (tvc*tvc+tvd*tvd > tvr*tvr+tvs*tvs) {tvr = tvc; tvs = tvd;} // tc
    tvr = tva-tvr; tvs = tvb-tvs; vb = va; for (ii=k; ii<=kk+1; ii++) {*vb++ -= tvr; *vb -= tvs; vb += 2*jj+1;} 
	for (i=k; i<=kk; i++) {
	  vbb = (vb = va+2*(i-k)*(jj+1))+2*jj;
	  tva = *vb; tvb = *(vb+1); tvc = *vbb; tvd = *(vbb+1); tvv = sqrt(tva*tva+tvb*tvb+tvc*tvc+tvd*tvd); 
	  *vcs++ = (tva /= tvv); *vcs++ = (tvb /= tvv); *vcs++ = (tvc /= tvv); *vcs++ = (tvd /= tvv); 
      *vb++ = tvv; *vb++ = 0.; *vbb++ = 0.; *vbb++ = 0.;
	  for (ii=i+1; ii<=kk+1; ii++) {
		tvp = tva*(*vb)+tvb*(*(vb+1))+tvc*(*vbb)+tvd*(*(vbb+1)); tvq = tva*(*(vb+1))-tvb*(*vb)+tvc*(*(vbb+1))-tvd*(*vbb);
		tv = tva*(*(vbb+1))+tvb*(*vbb)-tvc*(*(vb+1))-tvd*(*vb); *vbb++ = tva*(*vbb)-tvb*(*(vbb+1))-tvc*(*vb)+tvd*(*(vb+1)); 
		*vbb++ = tv; *vb++ = tvp; *vb++ = tvq; 
	  } tjn += kk-i+1;
	} vcs -= 4*(kk+1-k);
    for (i=k; i<=kk+1; i++) { 
      vbb = (vb = va+2*(i-k)*(jj+1))+2;
	  if (i > k) {
		vb -= 2; vbb -= 2;
	    tva = *vcs++; tvb = *vcs++; tvc = *vcs++; tvd = *vcs++; tv = *vbb;
        *vb++ = tvc*tv; *vb++ = tvd*tv; *vbb++ = tva*tv; if (i == kk+1) tvv = *vbb; *vbb++ = -tvb*tv;	  
		if (i == kk+1) {*(vb-2) += -tvd*tvv; *(vb-1) += tvc*tvv; *(vbb-2) += tvb*tvv; *(vbb-1) += tva*tvv;}
	  }
	  for (ii=i; ii<=kk; ii++) {
        tva = *vcs++; tvb = *vcs++; tvc = *vcs++; tvd = *vcs++; tv = *vbb; 
	    tvp = tva*(*vb)-tvb*(*(vb+1))+tvc*(*vbb)-tvd*(*(vbb+1)); tvq = tva*(*(vb+1))+tvb*(*vb)+tvd*(*vbb)+tvc*(*(vbb+1));
		tv = tva*(*(vbb+1))-tvb*(*vbb)+tvd*(*vb)-tvc*(*(vb+1)); *vbb++ = tva*(*vbb)+tvb*(*(vbb+1))-tvc*(*vb)-tvd*(*(vb+1)); 
		*vbb++ = tv; *vb++ = tvp; *vb++ = tvq; 
	  } 
      vcs -= 4*(kk+1-i); tjn += kk-i+2;
	} vcs -= 4*(kk+1-k); 
  	vb = va; for (ii=k; ii<=kk+1; ii++) {*vb++ += tvr; *vb += tvs; vb += 2*jj+1;}
  } while (1); 
lbl1:
  //cout<<tjn/double(jj*jj)/jj<<endl;
  delete[] cs; return R;
}

Matrix Matrix::qrsg(const Vector& D, Matrix& Q) const {
  int i, ii, jj = Nown, k, kk, iii;
  Matrix R(*this); 
  cppc tc, tcc, tcs, tcr, tca, tcd;
  double tv, tvv, *va, *vb, *vq, *vbb, *vaa, *vcs, *vqa, *vqb, tva, tvb, tvc, tvd, tvr, tvs, tvp, tvq, *cs, tjn = 0.; 
  vaa = va = reinterpret_cast <double*>(R.Data); vq = reinterpret_cast <double*>(Q.Data);  
  k = 0; cs = new double[4*jj]; 
  for (kk=jj-2; kk>=0; kk--) {   
	tc = D.Data[kk+1]; tvr = tc.real();  tvs = tc.imag();
lbl1:   
	vb = va; for (ii=k; ii<=kk+1; ii++) {*vb++ -= tvr; *vb -= tvs; vb += 2*jj+1;} vcs = cs;
	for (i=k; i<=kk; i++) {
	  vbb = (vb = va+2*(i-k)*(jj+1))+2*jj;
	  tva = *vb; tvb = *(vb+1); tvc = *vbb; tvd = *(vbb+1); tvv = sqrt(tva*tva+tvb*tvb+tvc*tvc+tvd*tvd); 
	  *vcs++ = (tva /= tvv); *vcs++ = (tvb /= tvv); *vcs++ = (tvc /= tvv); *vcs++ = (tvd /= tvv); 
      *vb++ = tvv; *vb++ = 0.; *vbb++ = 0.; *vbb++ = 0.;
	  for (ii=i+1; ii<jj; ii++) {
		tvp = tva*(*vb)+tvb*(*(vb+1))+tvc*(*vbb)+tvd*(*(vbb+1)); tvq = tva*(*(vb+1))-tvb*(*vb)+tvc*(*(vbb+1))-tvd*(*vbb);
		tv = tva*(*(vbb+1))+tvb*(*vbb)-tvc*(*(vb+1))-tvd*(*vb); *vbb++ = tva*(*vbb)-tvb*(*(vbb+1))-tvc*(*vb)+tvd*(*(vb+1)); 
		*vbb++ = tv; *vb++ = tvp; *vb++ = tvq; 
	  } tjn += jj-i-1;
	} 
	for (i=0; i<=kk+1; i++) { 
      iii = (i > k) ? i-1 : k; vcs = (i > k+1) ? cs+4*(i-k-1) : cs;
	  vbb = (vb = vaa+2*(i*jj+iii))+2;
	  for (ii=iii; ii<=kk; ii++) {
        tva = *vcs++; tvb = *vcs++; tvc = *vcs++; tvd = *vcs++; tv = *vbb; 
	    tvp = tva*(*vb)-tvb*(*(vb+1))+tvc*(*vbb)-tvd*(*(vbb+1)); tvq = tva*(*(vb+1))+tvb*(*vb)+tvd*(*vbb)+tvc*(*(vbb+1));
		tv = tva*(*(vbb+1))-tvb*(*vbb)+tvd*(*vb)-tvc*(*(vb+1)); *vbb++ = tva*(*vbb)+tvb*(*(vbb+1))-tvc*(*vb)-tvd*(*(vb+1)); 
		*vbb++ = tv; *vb++ = tvp; *vb++ = tvq; 
	  } 
      tjn += kk-iii+1;
	}
	for (i=0; i<jj; i++) { 
      vcs = cs; vqb = (vqa = vq+2*(i*jj+k))+2;
	  for (ii=k; ii<=kk; ii++) {
        tva = *vcs++; tvb = *vcs++; tvc = *vcs++; tvd = *vcs++; tv = *vqb; 
	    tvp = tva*(*vqa)-tvb*(*(vqa+1))+tvc*(*vqb)-tvd*(*(vqb+1)); tvq = tva*(*(vqa+1))+tvb*(*vqa)+tvd*(*vqb)+tvc*(*(vqb+1));
		tv = tva*(*(vqb+1))-tvb*(*vqb)+tvd*(*vqa)-tvc*(*(vqa+1)); *vqb++ = tva*(*vqb)+tvb*(*(vqb+1))-tvc*(*vqa)-tvd*(*(vqa+1)); 
		*vqb++ = tv; *vqa++ = tvp; *vqa++ = tvq; 
	  } 
      //tjn += kk-iii+1;
	} 
	vb = va; for (ii=k; ii<=kk+1; ii++) {*vb++ += tvr; *vb += tvs; vb += 2*jj+1;}
	vb = vaa+2*((kk+1)*jj+kk); //cout<<k<<" "<<kk<<" "<<*vb<<" "<<*(vb+1)<<endl; getch();
	if ((fabs(*vb) > nz_)||(fabs(*(vb+1)) > nz_)) {
      vb += 3; tvs = *vb--; tvr = *vb--; tvq = *vb--; tvp = *vb; vb -= 2*jj-3; 
	  tvd = *vb--; tvc = *vb--; tvb = *vb--; tva = *vb; tvs = 0.5*(tvb-tvs); tvr = 0.5*(tva-tvr);  // tca
	  tv = tvr*tvr-tvs*tvs+tvp*tvc-tvq*tvd; tvv = 2.*tvr*tvs+tvp*tvd+tvq*tvc; tvc = sqrt(tv*tv+tvv*tvv); 
	  if (tvc < 1.e-30) tvp = tvq = tvc; else
	  if (tv < 0) {tvq = sqrt(0.5*(tvc-tv)); tvp = 0.5*tvv/tvq;} else {tvp = sqrt(0.5*(tv+tvc)); tvq = 0.5*tvv/tvp;}  // tcd
	  tvc = tvr-tvp; tvd = tvs-tvq; tvr += tvp; tvs += tvq;	if (tvc*tvc+tvd*tvd > tvr*tvr+tvs*tvs) {tvr = tvc; tvs = tvd;} // tc
      tvr = tva-tvr; tvs = tvb-tvs; goto lbl1; 	
	}
  } 
  //cout<<tjn/double(jj*jj)/jj<<endl;
  delete[] cs; return R;
}

Matrix Matrix::vect(const Vector& D) const {
  int i, j, ii, jj = Nown; cppc tc; 
  double tv, *va, *vb, *vbb, *vaa, *vd, *vdd, tva, tvb, tvc, tvd; 
  Matrix M(*this); va = reinterpret_cast <double*>(M.Data); vd = reinterpret_cast <double*>(D.Data); 
  vb = reinterpret_cast <double*>(Data); va += 2*jj*jj; vd += 2*jj;
  for (i=jj-1; i>=0; i--) {
    vd -= 2; *(--va) = 0.; vdd = vd; *(--va) = 1.;
	for (j=i+1; j<jj; j++) {
	  vbb = vb+2*(i*jj+j)+1; tvb = *vbb--; tva = *vbb--; vaa = va+2*(jj*(j-1-i)+j-i); vdd += 2;
	  for (ii=j-1; ii>i; ii--) 
	    {tvd = *vbb--; tvc = *vbb--; tva += tvc*(*vaa)-tvd*(*(vaa+1)); tvb += tvd*(*vaa)+tvc*(*(vaa+1)); vaa -= 2*jj;}  

	  tvc = (*vdd)-(*vd); tvd = (*(vdd+1))-(*(vd+1)); tv = tvc*tvc+tvd*tvd; 
	  *vaa++ = (tva*tvc+tvb*tvd)/tv; *vaa = (tvb*tvc-tva*tvd)/tv;
	}
	va -= 2*jj;
  }
  return M;
} 

Matrix Matrix::vects(const Vector& D) const {
  int i, j, k, jj = Nown; cppc tc; 
  double tv, tvv, *va, *vb, *vbb, *vaa, *vd, tva, tvb, tvc, tvd, tvq; 
  Matrix M(jj); va = reinterpret_cast <double*>(M.Data); vd = reinterpret_cast <double*>(D.Data); 
  vb = reinterpret_cast <double*>(Data); //tvv = 1.; for (k=0; k<jj/5; k++) tvv *= 0.1;
  for (k=0; k<jj; k++) {
    vaa = va+2*(jj*(jj-1)+k); *vaa++ = tvv = 1.; *vaa = 0.;
    for (i=jj-2; i>=0; i--) {
      vaa = va+2*(jj*(i+1)+k); vbb = vb+2*(jj*(i+1)+(i+1)); tvc = (*vd)-(*vbb++); tvd = (*(vd+1))-(*vbb++);
	  tva = (*vaa)*tvc-(*(vaa+1))*tvd; tvb = (*vaa)*tvd+(*(vaa+1))*tvc;
	  for (j=i+2; j<jj; j++) 
	    {vaa += 2*jj; tva -= (*vaa)*(*vbb)-(*(vaa+1))*(*(vbb+1)); tvb -= (*vaa)*(*(vbb+1))+(*(vaa+1))*(*vbb); vbb += 2;}
	  vaa = va+2*(jj*i+k); vbb = vb+2*(jj*(i+1)+i); tvc = *vbb++; tvd = *vbb; tv = tvc*tvc+tvd*tvd;
	  *vaa++ = tvq = (tva*tvc+tvb*tvd)/tv; *vaa = tva =(tvb*tvc-tva*tvd)/tv; tvv += tvq*tvq+tva*tva;
	  if (tvv > 1.e30) {
		tv = 1./sqrt(tvv); vaa = va+2*(jj*i+k); tvv = 1.;
	    for (j=i; j<jj; j++) {*vaa++ *= tv; *vaa *= tv; vaa += 2*jj-1;} 
	  }
	}
	tv = 1./sqrt(tvv); vaa = va+2*k; for (j=0; j<jj; j++) {*vaa++ *= tv; *vaa *= tv; vaa += 2*jj-1;} vd += 2; 
  }
  return M;
}

Vector Matrix::diag_() const {
  Vector D(Nown); Matrix R(Nown), Q(Nown);
  R = hess(); R = R.qrs(); for (int i=0; i<Nown; i++) D.Data[i] = R.Data[i*Nown+i]; 
  return D;
}

Vector Matrix::diag() const {
  Vector D(Nown); Matrix R(Nown), Q(Nown);
  R = hessg(); R = R.qrsg(); for (int i=0; i<Nown; i++) D.Data[i] = R.Data[i*Nown+i]; 
  return D;
}

Vector Matrix::diag_(Matrix& M) const {
  int i, j, jj = Nown; 
  cppc tc; 
  Vector D(jj); Matrix Q(jj), R(jj), T(jj);
  for (i=0; i<jj; i++) {for (j=0; j<jj; j++) Q.Data[i*jj+j] = 0.; Q.Data[i*jj+i] = 1.;} 
  R = hess(Q); T = R.qrs(); for (j=0; j<jj; j++) D.Data[j] = T.Data[jj*j+j]; 
  M = R.vects(D); M = Q*M; 
  return D;
}

Vector Matrix::diag(Matrix& M) const {
  int i, j, jj = Nown; 
  cppc tc;
  Vector D(jj); Matrix Q(jj), R(jj), T(jj);
  for (i=0; i<jj; i++) {for (j=0; j<jj; j++) Q.Data[i*jj+j] = 0.; Q.Data[i*jj+i] = 1.;} 
  R = hessg(Q); T = R.qrsg(); for (j=0; j<jj; j++) D.Data[j] = T.Data[jj*j+j]; 
  R = R.qrsg(D,Q); M = R.vect(D); M = Q*M; 
  //M = R.vects(D); M = Q*M; 
  return D;
}

Matrix Matrix::transp()
{
	int i, j; 
	Matrix M(Next, Nown);  

	for (i = 0; i < Nown; i++) //rows of A
		for (j = 0; j < Next; j++) //cols of A
			M.Data[M.Next * j + i] = Data[i*Next + j];
	return M;
}

void vSet_M_adjoint(Matrix& M, Matrix& M_adj)
{
	int i, j, iIndexLine;
	for (i = 0; i < M.Nown; i++) { //for all lines 
		iIndexLine = i * M.Next;
		for (j = 0; j < M.Next; j++) //for all columns  
			M_adj.Data[M_adj.Next * j + i] = conj(M.Data[iIndexLine + j]);
	} 
}

void Matrix::vNormalizeColumns()
{
	int i, j, iRows = Nown, iCols = Next;
	cppc cData, cEntry, cPhase_M;
	cppc cNorm = 0.0;
	double dNorm;

	for (i = 0; i < iCols; i++) {
		dNorm = 0.0;
		for (j = 0; j < iRows; j++) {
			cData = Data[j * iCols + i];  dNorm += cData.real() * cData.real() + cData.imag() * cData.imag();
		}
		dNorm = sqrt(dNorm);
		for (j = 0; j < iRows; j++) { 
			cEntry = Data[j * iCols + i];
			cPhase_M = cEntry / abs(cEntry);
			if (abs(cEntry) > 1.0e-4 * dNorm) 
				break;
		}
		cNorm = 1.0 / (cPhase_M * dNorm);
		for (j = 0; j < iRows; j++)
			Data[j * iCols + i] *= cNorm; 
	}
}

double Matrix::norm()
{
	int i;  double dNorm = 0.0, dRe, dIm; 
	for (i = 0; i < Nown * Next; i++) {
		dRe = Data[i].real(), dIm = Data[i].imag(); 
		dNorm += dRe * dRe + dIm * dIm; 
	}
	dNorm = sqrt(dNorm);
	return dNorm; 
}

void Matrix::vSetToUnityMat()
{
	int i, iDim = Nown * Next, iSmallerDim = Nown > Next ? Next : Nown;  
	for (i = 0; i < iDim; i++)
		Data[i] = 0.0; 
	for (i = 0; i < iSmallerDim; i++)
		Data[i * Next + i] = 1.0; 
}

Matrix MGetUnityMat(int iDim)
{
	Matrix M(iDim, iDim);
	int i, j;

	for (i = 0; i < iDim; i++)
		for (j = 0; j < iDim; j++)
			M.Data[i*iDim + j] = 0.0;
	for (i = 0; i < iDim; i++)
		M.Data[i*iDim + i] = 1.0;
	return M; 
}

// class MArray:

MArray::MArray(int m, int n, int nn)
{
	Mown = m;
	Data = new Matrix[Mown];
	for (int i = 0; i<Mown; i++)
		Data[i] = Matrix(n, nn); //data field is initialized with zeros automatically
}

MArray::~MArray()
{
	if (Data)
		for (int i = 0; i < Mown; i++) {
			delete[] Data[i].Data;  Data[i].Data = NULL;  Data[i].Nown = 0;  Data[i].Next = 0;
		}
	delete[] Data; 	Data = NULL;  Mown = 0;
}


// implementations of the class SMatrix
SMatrix::SMatrix(int mown) {
  Mown = mown; Data[0][0] = new Matrix(mown); Data[0][1] = new Matrix(mown); Data[1][0] = new Matrix(mown); Data[1][1] = new Matrix(mown); 
}

SMatrix::SMatrix(const SMatrix& SM) {
  Mown = SM.Mown; 
  Data[0][0] = new Matrix(Mown); *(Data[0][0]) = SM(0,0); Data[0][1] = new Matrix(Mown); *(Data[0][1]) = SM(0,1);
  Data[1][0] = new Matrix(Mown); *(Data[1][0]) = SM(1,0); Data[1][1] = new Matrix(Mown); *(Data[1][1]) = SM(1,1);
}

SMatrix::SMatrix(const SMatrixD& SD) {
  int i, j;
  Mown = SD.Mown;
  Data[0][0] = new Matrix(Mown); Data[0][1] = new Matrix(Mown);
  Data[1][0] = new Matrix(Mown); Data[1][1] = new Matrix(Mown);
  for (i=0; i<Mown; i++) {
    for (j=0; j<Mown; j++)
        Data[0][0]->Data[i*Mown+j] = Data[1][0]->Data[i*Mown+j] =
        Data[0][1]->Data[i*Mown+j] = Data[1][1]->Data[i*Mown+j] = 0.;
    Data[0][0]->Data[i*Mown+i] = SD(0,i,0); Data[0][1]->Data[i*Mown+i] = SD(0,i,1);
    Data[1][0]->Data[i*Mown+i] = SD(1,i,0); Data[1][1]->Data[i*Mown+i] = SD(1,i,1);
  }
}

SMatrix::~SMatrix() {delete Data[0][0]; delete Data[1][0]; delete Data[0][1]; delete Data[1][1];
}

SMatrix& SMatrix::operator = (const SMatrix& SM) {
  Mown = SM.Mown;
  *(Data[0][0]) = SM(0,0); *(Data[0][1]) = SM(0,1);
  *(Data[1][0]) = SM(1,0); *(Data[1][1]) = SM(1,1);
  return *this;
}

SMatrix& SMatrix::operator = (const SMatrixD& SD) {
  int i, j;
  if (Mown != SD.Mown) {
    Mown = SD.Mown;
    delete Data[0][0]; delete Data[0][1]; delete Data[1][0]; delete Data[1][1];
    Data[0][0] = new Matrix(Mown); Data[0][1] = new Matrix(Mown);
    Data[1][0] = new Matrix(Mown); Data[1][1] = new Matrix(Mown);
  }
  for (i=0; i<Mown; i++) {
    for (j=0; j<Mown; j++)
        Data[0][0]->Data[i*Mown+j] = Data[1][0]->Data[i*Mown+j] =
        Data[0][1]->Data[i*Mown+j] = Data[1][1]->Data[i*Mown+j] = 0.;
    Data[0][0]->Data[i*Mown+i] = SD(0,i,0); Data[0][1]->Data[i*Mown+i] = SD(0,i,1);
    Data[1][0]->Data[i*Mown+i] = SD(1,i,0); Data[1][1]->Data[i*Mown+i] = SD(1,i,1);
  }
  return *this;
}

void SMatrix::cut(const SMatrix& B) {
  int i, ii, j, jj, s;
  if (Mown == B.Mown) {*this = B; return;}
  if (Mown > B.Mown) for (j=0; j<2; j++) for (jj=0; jj<2; jj++) {
    s = Mown/2-B.Mown/2;
    for (i=0; i<Mown; i++) for (ii=0; ii<Mown; ii++) Data[j][jj]->Data[i*Mown+ii] = 0;
    for (i=0; i<B.Mown; i++) for (ii=0; ii<B.Mown; ii++) Data[j][jj]->Data[(i+s)*Mown+ii+s] = B.Data[j][jj]->Data[i*B.Mown+ii];
  }
  else for (j=0; j<2; j++) for (jj=0; jj<2; jj++)  {
    s = B.Mown/2-Mown/2;
    for (i=0; i<Mown; i++) for (ii=0; ii<Mown; ii++) Data[j][jj]->Data[i*Mown+ii] = B.Data[j][jj]->Data[(i+s)*B.Mown+ii+s];
  }
}


SMatrix SMatrix::operator * (const SMatrix& B) {
  SMatrix C(Mown);
  int i, ii, no;
  if (Mown != B.Mown) return C; // exception ???
  no = Mown;
  Matrix B1(no), B2(no), B3(no);
  B3 = B(0,0) * (*this)(1,1);
  for (i=0; i<no; i++)
    for (ii=0; ii<no; ii++)
      if (i == ii) B3.Data[i*no+ii] = 1.-B3(i,ii);
      else B3.Data[i*no+ii] = -B3(i,ii);
  B1 = (*this)(0,1); B1 /= B3;
  B3 = B1*B(0,0); B2 = B3*(*this)(1,0);
  B2 += (*this)(0,0); *(C.Data[0][0]) = B2;
  B3 = B1*B(0,1); *(C.Data[0][1]) = B3;

  B3 = (*this)(1,1)*B(0,0);
  for (i=0; i<no; i++)
    for (ii=0; ii<no; ii++)
      if (i == ii) B3.Data[i*no+ii] = 1.-B3(i,ii);
      else B3.Data[i*no+ii] = -B3(i,ii);
  B1 = B(1,0); B1 /= B3;
  B3 = B1*(*this)(1,1); B2 = B3*B(0,1); B2 += B(1,1); *(C.Data[1][1]) = B2;
  B3 = B1*(*this)(1,0); *(C.Data[1][0]) = B3;
  return C;
}

SMatrix SMatrix::operator & (const SMatrix& B) {
  SMatrix C(Mown);
  int i, j, no;
  if (Mown != B.Mown) return C; // exception ???
  no = Mown;
  Matrix AA(no);
  AA = B(0,0)*(*this)(1,1);
  for (i=0; i<no; i++) for (j=0; j<no; j++) AA.Data[i*no+j] = (i == j) ? 1.-AA(i,j) : -AA(i,j);
  *(C.Data[0][0]) = (*this)(0,1)/AA;
  *(C.Data[0][1]) = *(C.Data[0][0])*B(0,0);
  AA = (*this)(1,1)*B(0,0);
  for (i=0; i<no; i++) for (j=0; j<no; j++) AA.Data[i*no+j] = (i == j) ? 1.-AA(i,j) : -AA(i,j);
  *(C.Data[1][1]) = B(1,0)/AA;
  *(C.Data[1][0]) = *(C.Data[1][1])*(*this)(1,1);
  return C;
}

SMatrix SMatrix::operator ^ (const SMatrix& B) {
  SMatrix C(Mown);
  int i, j, no;
  if (Mown != B.Mown) return C; // exception ???
  no = Mown;
  Matrix AA(no);
  AA = B(0,0)*(*this)(1,1);
  for (i=0; i<no; i++) for (j=0; j<no; j++)
      if (i == j) AA.Data[i*no+j] = 1.-AA(i,j); else AA.Data[i*no+j] = -AA(i,j);
  *(C.Data[0][1]) = B(0,1)%AA; *(C.Data[0][0]) = B(0,0)%AA*(*this)(1,0);
  AA = (*this)(1,1)*B(0,0);
  for (i=0; i<no; i++) for (j=0; j<no; j++)
      if (i == j) AA.Data[i*no+j] = 1.-AA(i,j); else AA.Data[i*no+j] = -AA(i,j);
  *(C.Data[1][0]) = (*this)(1,0)%AA; *(C.Data[1][1]) = (*this)(1,1)%AA*B(0,1);
  return C;
}

SMatrix SMatrix::operator * (const SMatrixD& B) {
  int i, j;
  SMatrix C(Mown);
  Matrix AA(Mown), BB(Mown);
  if (Mown != B.Mown) return C; // exception ???
  cppc B1, B2;

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -B(0,i,0)*(*this)(1,i,1,j);
    AA.Data[j*Mown+j] += 1;
  }
  BB = *Data[0][1]; BB /= AA;
  for (j=0; j<Mown; j++)
    for (i=0; i<Mown; i++)
      AA.Data[i*Mown+j] = BB(i,j)*B(0,j,0);
  *C.Data[0][0] = (*this)(0,0); *C.Data[0][0] += AA*(*this)(1,0);
  for (j=0; j<Mown; j++)
    for (i=0; i<Mown; i++)
      C.Data[0][1]->Data[i*Mown+j] = BB(i,j)*B(0,j,1);

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -(*this)(1,i,1,j)*B(0,j,0);
    AA.Data[j*Mown+j] += 1;
  }
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) BB.Data[i*Mown+j] = 0;
    BB.Data[j*Mown+j] = B(1,j,0);
  }
  BB /= AA; AA = BB*(*this)(1,1);
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) C.Data[1][1]->Data[i*Mown+j] = AA(i,j)*B(0,j,1);
    C.Data[1][1]->Data[j*Mown+j] += B(1,j,1);
  }
  *C.Data[1][0] = BB*(*this)(1,0);
  return C;
}

SMatrix SMatrix::operator & (const SMatrixD& B) {
  int i, j;
  SMatrix C(Mown);
  Matrix AA(Mown);
  if (Mown != B.Mown) return C; // exception ???
  cppc B1, B2;

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -B(0,i,0)*(*this)(1,i,1,j);
    AA.Data[j*Mown+j] += 1.;
  }
  *C.Data[0][0] = *Data[0][1]/AA;
  for (j=0; j<Mown; j++)
    for (i=0; i<Mown; i++)
      C.Data[0][1]->Data[i*Mown+j] = C(0,i,0,j)*B(0,j,0);

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -(*this)(1,i,1,j)*B(0,j,0);
    AA.Data[j*Mown+j] += 1.;
  }
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) C.Data[1][1]->Data[i*Mown+j] = 0.;
    C.Data[1][1]->Data[j*Mown+j] = B(1,j,0);
  }
  *C.Data[1][1] /= AA; *C.Data[1][0] = C(1,1)*(*Data[1][1]);
  return C;
}

SMatrix SMatrix::operator ^ (const SMatrixD& B) {
  int i, j;
  SMatrix C(Mown);
  Matrix AA(Mown);
  if (Mown != B.Mown) return C; // exception ???
  cppc B1, B2;

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -B(0,i,0)*(*this)(1,i,1,j);
    AA.Data[j*Mown+j] += 1;
  }
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) C.Data[1][1]->Data[j*Mown+i] = C.Data[1][0]->Data[j*Mown+i] = 0;
    C.Data[0][1]->Data[j*Mown+j] = B(0,j,1);
  }
  *C.Data[0][1] %= AA;
  for (j=0; j<Mown; j++)
    for (i=0; i<Mown; i++) C.Data[0][0]->Data[j*Mown+i] = B(0,j,0)*(*this)(1,j,0,i);
  *C.Data[0][0] %= AA ;

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -(*this)(1,i,1,j)*B(0,j,0);
    AA.Data[j*Mown+j] += 1;
  }
  *C.Data[1][0] = (*this)(1,0)%AA;
  for (j=0; j<Mown; j++)
    for (i=0; i<Mown; i++) C.Data[1][1]->Data[j*Mown+i] = (*this)(1,j,1,i)*B(0,i,1);
  *C.Data[1][1] %= AA;
  return C;
}

SMatrix SMatrix::operator * (const double d) {
  SMatrix C(Mown);
  int i, ii;
  for (i=0; i<Mown; i++)
    for (ii=0; ii<Mown; ii++) {
      C.Data[1][1]->Data[i*Mown+ii] = Data[1][1]->Data[i*Mown+ii]*exp(j_*double(ii-i)*d);
      C.Data[1][0]->Data[i*Mown+ii] = Data[1][0]->Data[i*Mown+ii]*exp(j_*double(ii-i)*d);
      C.Data[0][1]->Data[i*Mown+ii] = Data[0][1]->Data[i*Mown+ii]*exp(j_*double(ii-i)*d);
      C.Data[0][0]->Data[i*Mown+ii] = Data[0][0]->Data[i*Mown+ii]*exp(j_*double(ii-i)*d);
    }
  return C;
}

SMatrix& SMatrix::operator *= (const double d) {
  int i, ii;
  for (i=0; i<Mown; i++)
    for (ii=0; ii<Mown; ii++) {
      Data[1][1]->Data[i*Mown+ii] *= exp(j_*double(ii-i)*d);
      Data[1][0]->Data[i*Mown+ii] *= exp(j_*double(ii-i)*d);
      Data[0][1]->Data[i*Mown+ii] *= exp(j_*double(ii-i)*d);
      Data[0][0]->Data[i*Mown+ii] *= exp(j_*double(ii-i)*d);
    }
  return *this;
}

SMatrix SMatrix::ST() {
  SMatrix T(Mown);
  int i, ii;
  for (i=0; i<Mown; i++) {
    for (ii=0; ii<Mown; ii++)
      {T.Data[1][0]->Data[i*Mown+ii] = 0; T.Data[0][0]->Data[i*Mown+ii] = -Data[0][1]->Data[i*Mown+ii];}
    T.Data[1][0]->Data[i*Mown+i] = 1;
  }
  *T.Data[1][0] /= (*Data[1][1]);
  *T.Data[1][1] = *T.Data[1][0]*(*Data[1][0]);
  *T.Data[0][1] = *Data[0][0]+*T.Data[0][0]*(*T.Data[1][1]);
  *T.Data[0][0] *= *T.Data[1][0];
  return T;
}

SMatrix SMatrix::STT() {
  SMatrix T(Mown);
  int i, ii;
  for (i=0; i<Mown; i++) {
    for (ii=0; ii<Mown; ii++) T.Data[0][1]->Data[i*Mown+ii] = 0;
    T.Data[0][1]->Data[i*Mown+i] = 1;
  }
  *T.Data[0][1] /= (*Data[0][0]);
  *T.Data[0][0] = *T.Data[0][1]*(*Data[0][0]);
  *T.Data[1][1] = *Data[1][1]*(*T.Data[0][1]);
  for (i=0; i<Mown; i++) for (ii=0; ii<Mown; ii++) T.Data[1][0]->Data[i*Mown+ii] = -T(1,i,1,ii);
  *T.Data[1][0] = *Data[1][0]+*T.Data[1][0]*(*Data[0][0]);
  return T;
}

// implementations of the class SMatrixD

SMatrixD::SMatrixD(int mown) {
  Mown = mown; Data[0][0] = new Vector(mown); Data[0][1] = new Vector(mown);
  Data[1][0] = new Vector(mown); Data[1][1] = new Vector(mown); 
}

SMatrixD::SMatrixD(const SMatrixD& SM) {
  Mown = SM.Mown; 
  Data[0][0] = new Vector(Mown); *(Data[0][0]) = SM(0,0); Data[0][1] = new Vector(Mown); *(Data[0][1]) = SM(0,1);
  Data[1][0] = new Vector(Mown); *(Data[1][0]) = SM(1,0); Data[1][1] = new Vector(Mown); *(Data[1][1]) = SM(1,1);
}

SMatrixD::~SMatrixD() {delete Data[0][0]; delete Data[1][0]; delete Data[0][1]; delete Data[1][1];}

SMatrixD& SMatrixD::operator = (const SMatrixD& SD) {
  if (Mown != SD.Mown) {
    Mown = SD.Mown;
    delete Data[0][0]; delete Data[0][1]; delete Data[1][0]; delete Data[1][1];
    Data[0][0] = new Vector(SD(0,0)); Data[0][1] = new Vector(SD(0,1));
    Data[1][0] = new Vector(SD(1,0)); Data[1][1] = new Vector(SD(1,1));
  }
  else {*(Data[0][0]) = SD(0,0); *(Data[0][1]) = SD(0,1); *(Data[1][0]) = SD(1,0); *(Data[1][1]) = SD(1,1);}
  return *this;
}

SMatrixD SMatrixD::operator * (const SMatrixD& B) {
  SMatrixD C(Mown);
  if (Mown != B.Mown) return C; // exception ???
  cppc B1, B2;

  for (int i=0; i<Mown; i++) {
    B2 = 1.-B(0,i,0)*(*this)(1,i,1);
    B1 = (*this)(0,i,1)/B2;
    C.Data[0][0]->Data[i] = (*this)(0,i,0)+B1*B(0,i,0)*(*this)(1,i,0);
    C.Data[0][1]->Data[i] = B1*B(0,i,1);
    B1 = B(1,i,0)/B2;
    C.Data[1][1]->Data[i] = B(1,i,1)+B1*(*this)(1,i,1)*B(0,i,1);
    C.Data[1][0]->Data[i] = B1*(*this)(1,i,0);
  }
  return C;
}

SMatrixD SMatrixD::operator & (const SMatrixD& B) {
  SMatrixD C(Mown);
  if (Mown != B.Mown) return C; // exception ???
  cppc B1;

  for (int i=0; i<Mown; i++) {
    B1 = 1.-B(0,i,0)*(*this)(1,i,1);
    C.Data[0][1]->Data[i] = (C.Data[0][0]->Data[i] = (*this)(0,i,1)/B1)*B(0,i,0);
    C.Data[1][0]->Data[i] = (C.Data[1][1]->Data[i] = B(1,i,0)/B1)*(*this)(1,i,1);
  }
  return C;
}

SMatrixD SMatrixD::operator ^ (const SMatrixD& B) {
  SMatrixD C(Mown);
  if (Mown != B.Mown) return C; // exception ???
  cppc B1;

  for (int i=0; i<Mown; i++) {
    B1 = 1.-B(0,i,0)*(*this)(1,i,1);
    C.Data[0][1]->Data[i] = B(0,i,1)/B1;
    C.Data[0][0]->Data[i] = B(0,i,0)/B1*(*this)(1,i,0);
    C.Data[1][0]->Data[i] = (*this)(1,i,0)/B1;
    C.Data[1][1]->Data[i] = (*this)(1,i,1)/B1*B(0,i,1);
  }
  return C;
}

SMatrix SMatrixD::operator * (const SMatrix& B) {
  int i, j;
  SMatrix C(Mown);
  Matrix AA(Mown), BB(Mown);
  if (Mown != B.Mown) return C; // exception ???

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -B(0,i,0,j)*(*this)(1,j,1);
    AA.Data[j*Mown+j] += 1.;
  }
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) BB.Data[i*Mown+j] = 0.;
    BB.Data[j*Mown+j] = (*this)(0,j,1);
  }
  BB /= AA; AA = BB*B(0,0);
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) C.Data[0][0]->Data[i*Mown+j] = AA(i,j)*(*this)(1,j,0);
    C.Data[0][0]->Data[j*Mown+j] += (*this)(0,j,0);
  }
  *C.Data[0][1] = BB*B(0,1);

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -(*this)(1,i,1)*B(0,i,0,j);
    AA.Data[j*Mown+j] += 1.;
  }
  BB = B(1,0); BB /= AA;
  for (j=0; j<Mown; j++)
    for (i=0; i<Mown; i++)
      AA.Data[i*Mown+j] = BB(i,j)*(*this)(1,j,1);
  *C.Data[1][1] = B(1,1); *C.Data[1][1] += AA*B(0,1);
  for (j=0; j<Mown; j++)
    for (i=0; i<Mown; i++)
      C.Data[1][0]->Data[i*Mown+j] = BB(i,j)*(*this)(1,j,0);
  return C;
}

SMatrix SMatrixD::operator & (const SMatrix& B) {
  int i, j;
  SMatrix C(Mown);
  Matrix AA(Mown);
  if (Mown != B.Mown) return C; // exception ???
  cppc B1, B2;

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -B(0,i,0,j)*(*this)(1,j,1);
    AA.Data[j*Mown+j] += 1.;
  }
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) C.Data[0][0]->Data[i*Mown+j] = 0.;
    C.Data[0][0]->Data[j*Mown+j] = (*this)(0,j,1);
  }
  *C.Data[0][0] /= AA; *C.Data[0][1] = *C.Data[0][0]*B(0,0);

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -(*this)(1,i,1)*B(0,i,0,j);
    AA.Data[j*Mown+j] += 1.;
  }
  *C.Data[1][1] = B(1,0)/AA;
  for (j=0; j<Mown; j++)
    for (i=0; i<Mown; i++)
      C.Data[1][0]->Data[i*Mown+j] = C(1,i,1,j)*(*this)(1,j,1);
  return C;
}

SMatrix SMatrixD::operator ^ (const SMatrix& B) {
  int i, j;
  SMatrix C(Mown);
  Matrix AA(Mown);
  if (Mown != B.Mown) return C; // exception ???
  cppc B1, B2;

  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -B(0,i,0,j)*(*this)(1,j,1);
    AA.Data[j*Mown+j] += 1.;
  }
  *C.Data[0][1] = B(0,1)%AA;
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) C.Data[0][0]->Data[j*Mown+i] = B(0,j,0,i)*(*this)(1,i,0);
  }
  *C.Data[0][0] %= AA;
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) AA.Data[i*Mown+j] = -(*this)(1,i,1)*B(0,i,0,j);
    AA.Data[j*Mown+j] += 1.;
  }
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) C.Data[1][0]->Data[j*Mown+i] = 0.;
    C.Data[1][0]->Data[j*Mown+j] = (*this)(1,j,0);
  }
  *C.Data[1][0] %= AA;
  for (j=0; j<Mown; j++) {
    for (i=0; i<Mown; i++) C.Data[1][1]->Data[j*Mown+i] = (*this)(1,j,1)*B(0,j,1,i);
  }
  *C.Data[1][1] %= AA;
  return C;
}

SMatrixD SMatrixD::ST() {
  SMatrixD T(Mown);
  int i;
  cppc tc;
  for (i=0; i<Mown; i++) {
    tc = T.Data[1][1]->Data[i] = 1./(*this)(1,i,1);
    T.Data[1][0]->Data[i] = tc*(*this)(1,i,0);
    T.Data[0][1]->Data[i] = (*this)(0,i,1)*tc;
    T.Data[0][0]->Data[i] = (*this)(0,i,0)-T(0,i,1)*(*this)(1,i,0);
  }
  return T;
}

SMatrixD SMatrixD::STT() {
  SMatrixD T(Mown);
  int i;
  cppc tc;
  for (i=0; i<Mown; i++) {
    tc = T.Data[0][1]->Data[i] = 1./(*this)(0,i,1);
    T.Data[0][0]->Data[i] = tc*(*this)(0,i,0);
    T.Data[1][1]->Data[i] = (*this)(1,i,1)*tc;
    T.Data[1][0]->Data[i] = (*this)(1,i,0)-T(1,i,1)*(*this)(0,i,0);
  }
  return T;
}

// QMatrix implementations

QMatrix QMatrix::operator * (const QMatrix& B) {
  QMatrix M;
  cppc tv;
  tv = 1.-Data[1][1]*B.Data[0][0];
  M.Data[0][0] = Data[0][0]+Data[0][1]*B.Data[0][0]*Data[1][0]/tv;
  M.Data[0][1] = Data[0][1]*B.Data[0][1]/tv;
  M.Data[1][0] = Data[1][0]*B.Data[1][0]/tv;
  M.Data[1][1] = B.Data[1][1]+Data[1][1]*B.Data[0][1]*B.Data[1][0]/tv;
  return M;
}

QMatrix QMatrix::operator & (const QMatrix& B) {
  QMatrix M;
  cppc tv;
  tv = 1.-Data[1][1]*B.Data[0][0];
  M.Data[0][0] = Data[0][1]/tv;
  M.Data[0][1] = Data[0][1]*B.Data[0][0]/tv;
  M.Data[1][0] = B.Data[1][0]*Data[1][1]/tv;
  M.Data[1][1] = B.Data[1][0]/tv;
  return M;
}


QMatrix QMatrix::operator ^ (const QMatrix& B) {
  QMatrix M;
  cppc tv;
  tv = 1.-Data[1][1]*B.Data[0][0];
  M.Data[0][0] = Data[1][0]*B.Data[0][0]/tv;
  M.Data[0][1] = B.Data[0][1]/tv;
  M.Data[1][0] = Data[1][0]/tv;
  M.Data[1][1] = B.Data[0][1]*Data[1][1]/tv;
  return M;
}








//####################################################  Matrix inversion by Householder by Prof. T.  ############################################



Matrix vr(const Matrix& A, const Matrix& X) { //Householder 
  int i, ii, j, jj = A.Nown;
  Matrix R(A); Matrix B(X);
  cppc tc, tcc;
  double tv, tvv, *va, *vb, *vc, tvr, tvi, tcr, tci;
  va = reinterpret_cast <double*>(R.Data); vb = reinterpret_cast <double*>(B.Data); 
  for (i=0; i<jj-1; i++) {
	tv = 0.; for (vc=va+2,j=i+1; j<jj; j++) {tvr = *vc++; tv += tvr*tvr; tvi = *vc++; tv += tvi*tvi;} tvv = tv;
	tvr = *va; tv += tvr*tvr; tvi = *(va+1); tv += tvi*tvi; tv = sqrt(tv); tcc = cppc(tvr,tvi);
	tcc -= (R.Data[i*jj+i] = -exp(cppc(0.,1.)*arg(tcc))*tv); tvr = tcc.real(); tvi = tcc.imag();
	tvv += tvr*tvr+tvi*tvi; tvv = -2./tvv; vc = va; va += 2;
    for (ii=i+1; ii<jj; ii++) {
      vc += 2*jj; tcr = (*vc)*tvr; tci = -(*vc++)*tvi; tci += (*vc)*tvr; tcr += (*vc++)*tvi; 
	  for (j=i+1; j<jj; j++) {tcr += (*vc)*(*va); tci -= (*vc++)*(*(va+1)); tci += (*vc)*(*va++); tcr += (*vc++)*(*va++);}
	  tcr *= tvv; tci *= tvv; va -= 2*(jj-i-1); vc -= 2*(jj-i);
	  *vc++ += tvr*tcr-tvi*tci; *vc++ += tvr*tci+tvi*tcr; 
	  for (j=i+1; j<jj; j++) {*vc++ += (*va)*tcr-(*(va+1))*tci; *vc++ += (*va)*tci+(*(va+1))*tcr; va += 2;} 
	  va -= 2*(jj-i-1); vc -= 2*(jj-i);
	} 
	for (ii=0; ii<jj; ii++) {  
	  tcr = (*vb)*tvr; tci = -(*vb++)*tvi; tci += (*vb)*tvr; tcr += (*vb++)*tvi; 
	  for (j=i+1; j<jj; j++) {tcr += (*vb)*(*va); tci -= (*vb++)*(*(va+1)); tci += (*vb)*(*va++); tcr += (*vb++)*(*va++);}
	  tcr *= tvv; tci *= tvv; va -= 2*(jj-i-1); vb -= 2*(jj-i);
	  *vb++ += tvr*tcr-tvi*tci; *vb++ += tvr*tci+tvi*tcr; 
	  for (j=i+1; j<jj; j++) {*vb++ += (*va)*tcr-(*(va+1))*tci; *vb++ += (*va)*tci+(*(va+1))*tcr; va += 2;} 
	  vb += 2*i; va -= 2*(jj-i-1);    
	}
	va += 2*jj; vb -= 2*jj*jj-2;
  }
  for (i=jj-1; i>=0; i--) {
    tvr = *va; tvi = *(va+1); tvv = tvr*tvr+tvi*tvi; 
	for (ii=0; ii<jj; ii++) {
	  tcr = *(vb+1); tci = -(*(vb+1) = (tvr*tcr-tvi*(*vb))/tvv); tcr = -(*vb = (tvi*tcr+tvr*(*vb))/tvv); 
	  for (j=0; j<i; j++) {va -= 2; *(--vb) += tcr*(*(va+1))+tci*(*va); *(--vb) += tcr*(*va)-tci*(*(va+1));}
	  vb += 2*jj+2*i; va += 2*i;
	}
	vb -= 2*jj*jj+2; va -= 2*(jj+1);	
  }
  return B;
}



//#############################################################  Code written by W.  ############################################################



//############################################################# Least Squares Methods ###############################################################

// Implementations of calsss MinData_double (data characterizing the data set at a minimum):  

MinData_double::MinData_double(int Nown) { 
	bLocMin = false;  
	dVParam.Init(Nown);  dVStandardDev.Init(Nown);  
	dNorm = DBL_MAX; 
};

MinData_double::MinData_double(const MinData_double& Min) {
	bLocMin = Min.bLocMin;
	dVParam = Min.dVParam, dVStandardDev = Min.dVStandardDev;   
	dNorm = Min.dNorm;
};

void MinData_double::Init(int Nown) { 
	bLocMin = false;
	dVParam.Init(Nown);  dVStandardDev.Init(Nown);
	dNorm = DBL_MAX;
};

MinData_double& MinData_double::operator = (const MinData_double& Min) {
	bLocMin = Min.bLocMin;
	dVParam = Min.dVParam, dVStandardDev = Min.dVStandardDev; 
	dNorm = Min.dNorm; 
	return *this; 
}; 

MinData_double::~MinData_double() {
}; 

MinData_double *pAllocMinDataArray(int iNo_MinData, int iDim_OptParam)
{
	MinData_double *pMinData = new MinData_double [iNo_MinData]; 
	for (int i = 0; i < iNo_MinData; i++)
		pMinData[i].Init(iDim_OptParam);
	return pMinData; 
};

void vDeallocMinDataArray(MinData_double **ppMinData) {
	delete[](*ppMinData);   *ppMinData = NULL; 
}; 

int fNewVect( Vector **V, long iDim )
{
	int iexit_status = 0;

	*V = new Vector(iDim);
	if( ! (*V) )
		iexit_status = 1;
	return iexit_status;
}


//#######################################  Functions for compatibility of C and C++ vectors  #########################################



void Vector::GetVal(const CComplex *CSrc)
//This function copies data from CSource to VDest.Data.  Dimensions must match!  W., 22.7.13 
//Geht nicht: void fCopyComplexToVector( Vector VDest, CComplex *CSource ), denn VDest ist nur eine neu angelegte Kopie des Vectors!
//Einzelnen Wert setzen: cDest->real(CSource->re);  cDest->imag(CSource->im); 
//Einzelnen Wert holen: cDest->real();  cDest->imag(); memcpy ist aber effizienter.
//size_t sizeC = sizeof(CComplex), sizec = sizeof(complex<double>);  sizeC = sizec holds.
{
	CComplex *CDest = reinterpret_cast<CComplex*> (this->Data);
		
	memcpy( CDest, CSrc, this->Nown * sizeof(CComplex) );
}



void Vector::SetVal(CComplex *CDest)
//This function copies data from VSource.Data to CDest.  Dimensions must match!  W., 22.7.13 
{
	CComplex *CSrc = reinterpret_cast<CComplex*> (this->Data);
	
	memcpy( CDest, CSrc, this->Nown * sizeof(CComplex) );
}



void Vector::SetVal( cppc cVal )
//This function copies data from VSource.Data to CDest.  Dimensions must match!  W., 22.7.13 
{
	long i; 
	for( i=0; i<(long)this->Nown; i++ )  
		this->Data[i] = cVal;
}



//FT

void vSetFTinvMatrix(Matrix &FT_inv, Matrix &FT)
//Sets up all FT and FT_inv matrix entries explicitely.  W. 14.9.2017 
{
	int j; 
	int i, iIndex, iIndex2;
	double dk, dExp;
	for (j = -(int)(FT_inv.Next / 2);  j <= (int)(FT_inv.Next / 2);  j++) {
		dk = j*(2.0*PI) / (double)FT_inv.Nown;
		int j_Index = j + (int)(FT_inv.Next / 2);
		for (i = 0; i < FT_inv.Nown; i++)
		{
			dExp = (double)i * dk;
			iIndex = i * FT_inv.Next + j_Index;
			FT_inv.Data[iIndex].real(cos(dExp));  FT_inv.Data[iIndex].imag(sin(dExp));
			iIndex2 = j_Index * FT_inv.Next + i; 
			FT.Data[iIndex2].real(FT_inv.Data[iIndex].real());  FT.Data[iIndex2].imag(-FT_inv.Data[iIndex].imag());
		}
	}//end for j
}



//---------------------------------------------------------- Least Squares Methods --------------------------------------------------------------

 


bool bFindMinNorm1Dim(bool *pOptParam, dVector *pdVUpperLimit, bool *pbUpperLimitHit, dVector *pdVLowerLimit, bool *pbLowerLimitHit, 
	double dTol, int iMinIter, int iMaxIter, int iNoSamples1Dim, double *pdxArray, double *pdNormSamples1Dim,
	double dF_sq, double &dFNext_sq, dVector &dVx, dVector &dVxNextTemp, dVector &dVxNext, dVector &dVs, dVector *pdVFNextTemp, dVector *pdVFNext,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, dVector& dVParam, dVector *pdVResult, dMatrix *pdMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, double &dPositionMin1Dim)
{
	bool bSuc1Dim; 
	int iIndexOfMin1Dim = 0, iIndexL, iIndexLL, i, j;
	double dStep1Dim = 1.0 / (double)(iNoSamples1Dim - 1);
	double dPositionMin1DimPrevious, dPositionMin1DimStart, dMinNorm1Dim = DBL_MAX, dMaxNorm1Dim = 0, da0, da1, da2, dIncrement;

	pdNormSamples1Dim[0] = dF_sq, pdNormSamples1Dim[iNoSamples1Dim - 1] = dFNext_sq;
	for (i = 1; i < iNoSamples1Dim - 1; i++) {
		dVxNextTemp = dVs; dVxNextTemp *= dStep1Dim;  dVxNextTemp += dVx;    pdxArray[i] = i * dStep1Dim;   //dVxNextTemp = dStep1Dim * dVs + dVx 
		for (j = 0; j < dVx.Nown; j++) {
			if (pdVLowerLimit)
				if (dVxNextTemp.Data[j] < pdVLowerLimit->Data[j]) {
					dVxNextTemp.Data[j] = pdVLowerLimit->Data[j];  if (pbLowerLimitHit) pbLowerLimitHit[j] = true;
				}
			if (pdVUpperLimit)
				if (dVxNextTemp.Data[j] > pdVUpperLimit->Data[j]) {
					dVxNextTemp.Data[j] = pdVUpperLimit->Data[j];  if (pbUpperLimitHit) pbUpperLimitHit[j] = true;
				}
		}//j
		vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, dVxNextTemp, pdVFNextTemp, NULL, pOptParam, &bSuc1Dim);
		if (bSuc1Dim == false)
			return false;
		pdNormSamples1Dim[i] = pdVFNextTemp->norm_sq(); 
	}
	for (i = 0; i < iNoSamples1Dim - 1; i++) {
		if (pdNormSamples1Dim[i] < dMinNorm1Dim) {
			pdNormSamples1Dim[i] = dMinNorm1Dim, iIndexOfMin1Dim = i;
		}
		dMaxNorm1Dim = max(dMaxNorm1Dim, pdNormSamples1Dim[i]); 
	}
	iIndexL = max(0, iIndexOfMin1Dim - 1);  iIndexL = min(iIndexL, iNoSamples1Dim - 1);
	dPositionMin1DimStart = dPositionMin1Dim = iIndexOfMin1Dim * dStep1Dim;
	bParabolicFit_3(dPositionMin1Dim, pdxArray + iIndexL, pdNormSamples1Dim + iIndexL, &da0, &da2, &da2);
	dPositionMin1Dim = max((iIndexOfMin1Dim - 0.5) * dStep1Dim, dPositionMin1Dim); //avoid overshooting
	dPositionMin1Dim = min((iIndexOfMin1Dim + 0.5) * dStep1Dim, dPositionMin1Dim); //avoid overshooting 
	dPositionMin1DimPrevious = dPositionMin1Dim; //initialization

	for (i = 0; i < iMaxIter; i++) { 
		iIndexL = (int)(dPositionMin1Dim / dStep1Dim);  iIndexLL = iIndexL - 1;  iIndexLL = max(0, iIndexLL);  iIndexLL = min(iIndexLL, iNoSamples1Dim - 3);
		bParabolicFit_4(dPositionMin1Dim, iIndexLL, iNoSamples1Dim - 1, pdxArray + iIndexLL, pdNormSamples1Dim + iIndexLL, &da0, &da1, &da2);
		if (da2 > NUM_NOISE) {
			dIncrement = -da1 / (2.0 * da2);
			dIncrement = max(-0.5*dStep1Dim, dIncrement);  dIncrement = min(dIncrement, 0.5*dStep1Dim);
			dPositionMin1Dim = dPositionMin1Dim + dIncrement;
			dPositionMin1Dim = max(dPositionMin1DimStart - dStep1Dim, dPositionMin1Dim);
			dPositionMin1Dim = min(dPositionMin1Dim, dPositionMin1DimStart + dStep1Dim);
			dPositionMin1Dim = max(0.0, dPositionMin1Dim);   dPositionMin1Dim = min(dPositionMin1Dim, 1.0);
			if (fabs(dPositionMin1Dim - dPositionMin1DimPrevious) < dTol * (dMaxNorm1Dim - da0) && i >= iMinIter) 
				break;
			dPositionMin1DimPrevious = dPositionMin1Dim;
		}//end if
	}//end for j 
	dVxNextTemp = dVs;  dVxNextTemp *= dPositionMin1Dim;  dVxNextTemp += dVx;  //dVxNextTemp = dPositionMin1Dim * dVs + dVx 
	for (j = 0; j < dVx.Nown; j++) {
		if (pdVLowerLimit)
			if (dVxNextTemp.Data[j] < pdVLowerLimit->Data[j]) {
				dVxNextTemp.Data[j] = pdVLowerLimit->Data[j];  if (pbLowerLimitHit) pbLowerLimitHit[j] = true;
			}
		if (pdVUpperLimit)
			if (dVxNextTemp.Data[j] > pdVUpperLimit->Data[j]) {
				dVxNextTemp.Data[j] = pdVUpperLimit->Data[j];  if (pbUpperLimitHit) pbUpperLimitHit[j] = true;
			}
	}//j
	vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, dVxNextTemp, pdVFNextTemp, NULL, pOptParam, &bSuc1Dim);
	if (bSuc1Dim == false)
		return false;
	dVxNext = dVxNextTemp;   *pdVFNext = *pdVFNextTemp;   dFNext_sq = pdVFNextTemp->norm(); dFNext_sq *= dFNext_sq; 
	return true;
}


 
bool bLevenbergMarquardt_double(dVector& dVx0, bool *bOptParam, dVector* pdVUpperLimit, bool *pbUpperLimitHit, dVector* pdVLowerLimit, 
	bool *pbLowerLimitHit, double dMu, bool bLinearityTest, double dTol, int iMinIterDLS, int iMaxIter, double dMin_Chance_DLS_Residual, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, dVector& dVParam, dVector *pdVResult, dMatrix *pdMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, MinData_double *psResult)
	//LevenbergMarquardt algorithm as in Dahmen Reusken, "Numerik für Ingenieure und Naturwissenschaftler" on p. 222 - 224.  
	//This is the most up-to-date and enhanced version of the Levenberg-Marquardt method in this project.   W. Iff, 30.8.2024 
{
	bool bSuc1 = true, bSuc2 = true, bStop = false;
	int i, j, iNoCalc_Vx;   
	const double  dBeta0 = 0.42, dBeta1 = 0.8; 
	double dNorm_init = DBL_MAX, dNormVF_Previous = DBL_MAX;  psResult->dNorm = DBL_MAX;
	double dMuSq, dRho, dF_sq, dFNext_sq, dFNextLinearized_sq, dDerivNorm = 0.0, dNumRho, dDenomRho = DBL_MAX; //We had dBeta0 = 0.3 before 12.8.2024
	double dNormOfMdFdxSq, dMinAllowedMuSq, dMinAllowedMu;
	iVector iVLowerLimitRealHit(dVx0.Nown); iVLowerLimitRealHit = 0;  iVector iVUpperLimitRealHit(dVx0.Nown); iVUpperLimitRealHit = 0;
	dVector dVF(iDimVF), dVs(dVx0.Nown), dVxNext(dVx0.Nown), dVFNext(iDimVF), dVFNextLinearized(iDimVF), dVDeriv(dVx0.Nown);

	dMatrix dMdFdx(iDimVF, dVx0.Nown), dMdFdx_adj(dVx0.Nown, iDimVF), dM_dFdx_adj_x_dFdx(dVx0.Nown, dVx0.Nown), dM_dFdx_adj_x_dFdx_Copy(dVx0.Nown, dVx0.Nown), 
		dMInverse(dVx0.Nown, dVx0.Nown); 
	//ofstream ofDLS_Mat("DLS_Mat.txt"), ofConvSpeed("ConvSpeed.txt"); 
	////ofstream fOutDLS("./results_1DTM/DLS_Convergence.txt");   fOutDLS << setprecision(12);   cout << setprecision(12);

	////cout << endl << "Start of Levenberg-Marquardt algo" << endl;
	psResult->dVParam = dVx0;   
//	ofDLS_Mat << endl << dVx0.Data[0] << "\t\t" << dVx0.Data[1] << "\t\t" << dVx0.Data[2] << "\t\t" << dVx0.Data[3] << "\t\t" << dVx0.Data[4] << endl; 
//	cout << endl << dVx0.Data[0] << "\t\t" << dVx0.Data[1] << "\t\t" << dVx0.Data[2] 
//		<< "\t\t" << dVx0.Data[3] << "\t\t" << dVx0.Data[4] 
//		<< endl; 
	if (pbUpperLimitHit) *pbUpperLimitHit = false;  if (pbLowerLimitHit) *pbLowerLimitHit = false;
	for (i = 0; i < iMaxIter; i++) {
		////cout << "Iteration no.: " << i << "\t ";  
		//1.) Calc. F, F': 
//		vPrint_dVec(ofDLS_Mat, "psResult->dVParam = ", psResult->dVParam); 
		vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, psResult->dVParam, &dVF, &dMdFdx, bOptParam, &bSuc1);
//		vPrint_dVec(ofDLS_Mat, "dVF = ", dVF); 
		dNormVF_Previous = psResult->dNorm;  psResult->dNorm = dVF.norm();  if (i == 0) dNorm_init = psResult->dNorm;  ////cout << "VF.norm() = " << pNormVF << "\t"; 
//		ofConvSpeed << i << "\t\t" << psResult->dNorm << "\t\t";  cout << i << "\t psResult->dNorm: " << psResult->dNorm;
		////cout << "MdFdx.norm() / (2 * PI / 0.6645) = " << MdFdx.norm() / (2 * PI / 0.6645) << "\t"; 
		//2.) Solve problem, calc s_i: 
//		vPrint_dMat(ofDLS_Mat, "dMdFdx = ", dMdFdx); 
		dMdFdx_adj.transp(dMdFdx); 
//		vPrint_dMat(ofDLS_Mat, "dMdFdx_adj = ", dMdFdx_adj);
		dM_dFdx_adj_x_dFdx_Copy = dM_dFdx_adj_x_dFdx = dMdFdx_adj * dMdFdx;
//		vPrint_dMat(ofDLS_Mat, "dM_dFdx_adj_x_dFdx = ", dM_dFdx_adj_x_dFdx); 
		dM_dFdx_adj_x_dFdx.calc_M_adj_x_M(dMdFdx_adj);
//		vPrint_dMat(ofDLS_Mat, "dM_dFdx_adj_x_dFdx = ", dM_dFdx_adj_x_dFdx); 
		iNoCalc_Vx = 0; 
	CALC_VX:
		if (iNoCalc_Vx > 500) //this means µ approx. 2^500 and µ² = approx. 2^1000 = approx. 1.0e301; 
			break; //ensure termination here (should never happen) 
		iNoCalc_Vx++;
		dM_dFdx_adj_x_dFdx = dM_dFdx_adj_x_dFdx_Copy; 
		dNormOfMdFdxSq = dM_dFdx_adj_x_dFdx.norm();  dMinAllowedMuSq = dVx0.Nown * dNormOfMdFdxSq * 0.01 * NUM_NOISE_FLOAT;  dMinAllowedMu = sqrt(dMinAllowedMuSq);
		if (dMu < dMinAllowedMu)
			dMu = dMinAllowedMu; //diag elements dMuSq must have certain size so that M_dFdx_adj_x_dFdx is always invertible.    
//		ofDLS_Mat << endl << dMu << endl;  cout << "\t dMu = " << dMu << "\t"; 
		dMuSq = dMu * dMu; 
		for (j = 0; j < dVx0.Nown; j++)
			dM_dFdx_adj_x_dFdx.Data[j * dVx0.Nown + j] += dMuSq;
//		vPrint_dMat(ofDLS_Mat, "dM_dFdx_adj_x_dFdx + dMuSq = ", dM_dFdx_adj_x_dFdx); 
		dMInverse.vSetToUnityMat();
		dMInverse /= dM_dFdx_adj_x_dFdx;
//		vPrint_dMat(ofDLS_Mat, "dMInverse = ", dMInverse); 
		dVs = dVF * dMdFdx_adj; //dVs = dMdFdx_adj * dVF; 
//		vPrint_dVec(ofDLS_Mat, "dVs = ", dVs); 
		dVs *= dMInverse; // dMInverse * dVs; 
//		vPrint_dVec(ofDLS_Mat, "dVs = ", dVs); 
		dVs *= -1;
		for (j = 0; j < dVx0.Nown; j++) {
			if (bOptParam[j] == false)
				dVs.Data[j] = 0.0; 
		}
//		vPrint_dVec(ofDLS_Mat, "dVs = ", dVs); 
		dVxNext = psResult->dVParam;  
		dVxNext += dVs; // dVxNext = dVx + dVs
//		vPrint_dVec(ofDLS_Mat, "dVxNext = ", dVxNext); 
		//3.) and 4.) Linearity check with dRho: 
		if (bLinearityTest) {
			dF_sq = psResult->dNorm;  dF_sq *= dF_sq;
			vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, dVxNext, &dVFNext, NULL, bOptParam, &bSuc2); 
			dFNext_sq = dVFNext.norm();  dFNext_sq *= dFNext_sq; 
			dVFNextLinearized = dVs * dMdFdx; // calc dVFNextLinearized = dVF + dMdFdx * dVs : 
			dVFNextLinearized += dVF; 
			dFNextLinearized_sq = dVFNextLinearized.norm();  dFNextLinearized_sq *= dFNextLinearized_sq;
			if (bSuc1 != true || bSuc2 != true) {
				psResult->dVParam = dVx0;  return false; //change no parameters
			}
			dNumRho = dF_sq - dFNext_sq;  dDenomRho = dF_sq - dFNextLinearized_sq;
			if ( fabs(dDenomRho) / ((dF_sq + dFNextLinearized_sq) / 2.0) <= 0.02 * NUM_NOISE_FLOAT ) {
				dRho = 1.0;   dDenomRho = 0.0;  //perfect linearity 
			} else 
				dRho = dNumRho / dDenomRho;  ////cout << "dRho - 1.0 = " << dRho - 1.0 << "\t"; 
			if (dVs.norm() <= MIN_STEP_DLS * psResult->dVParam.norm()) //ensure termination of the algo in case of persistent nonlinearities 
				dRho = 1.0;
		}
		else //linearity is assumed: 
			dRho = 1.0;
		//5.) Termination criteria:  
		dVDeriv = dVF * dMdFdx_adj; 
		dDerivNorm = dVDeriv.norm(); //this is always needed 
//		ofConvSpeed << dDerivNorm << endl;    cout << "\t dDerivNorm: " << dDerivNorm; 
		//4.) Linearity check with dRho:   
		if (dRho <= dBeta0) {
			dMu *= 2.0;  goto CALC_VX;
		}
		else { //step size is OK: 
			if (dRho >= dBeta1)
				dMu /= 2.0; //less damping  
			for (j = 0; j < dVx0.Nown; j++) {
				if (pdVLowerLimit) {
					if (dVxNext.Data[j] < pdVLowerLimit->Data[j]) {
						iVLowerLimitRealHit.Data[j] += 2;  dVxNext.Data[j] = pdVLowerLimit->Data[j]; 
						//if (iVLowerLimitRealHit.Data[j] >= 25) 
						//	dVDeriv.Data[j] = 0.0;
						if (pbLowerLimitHit) 
							*pbLowerLimitHit = true;
					}
				}//end if pVLowerLimit   
				if (pdVUpperLimit) {
					if (dVxNext.Data[j] > pdVUpperLimit->Data[j]) {
						iVUpperLimitRealHit.Data[j] += 2;   dVxNext.Data[j] = pdVUpperLimit->Data[j]; 
						//if (iVUpperLimitRealHit.Data[j] >= 25)  
						//	dVDeriv.Data[j] = 0.0;  
						if (pbUpperLimitHit) 
							*pbUpperLimitHit = true;
					}
				}//end if pVUpperLimit  
			}//end for j
			psResult->dVParam = dVxNext;   dDerivNorm = dVDeriv.norm(); //recalculation (update) 
//			cout << "\t\t" << psResult->dVParam.Data[0] << "\t\t" << psResult->dVParam.Data[1] << "\t\t" << psResult->dVParam.Data[2]
//				<< "\t\t" << psResult->dVParam.Data[3] << "\t\t" << psResult->dVParam.Data[4] 
//				<< endl; 
		}//end else step size is OK 
		////cout << " dDerivNorm = " << dDerivNorm << endl;   fOutDLS << i << "\t" << psResult->dNorm << "\t\t" << dDerivNorm << endl;  
		if (dDerivNorm < dTol  &&  fabs(1.0 - psResult->dNorm / dNormVF_Previous) < dMin_Chance_DLS_Residual  &&  i >= iMinIterDLS) //Stop if accuracy is sufficient!
			break;
		if (dVs.norm() <= MIN_STEP_DLS * psResult->dVParam.norm() && i >= iMinIterDLS) //Stop if step forward is marginal (algo has converged to something)
			break;
		if (bLinearityTest)
			if (dDerivNorm <= dTol &&  fabs(dDenomRho) <= NUM_NOISE && i >= iMinIterDLS) //Stop if perfect linearity or step forward is marginal  
				break;
		iVLowerLimitRealHit.vResetValue();   iVUpperLimitRealHit.vResetValue(); 
		int iMax = (int)(iMaxIter / 5.0) + 1; 
		for (j = 0; j < dVx0.Nown; j++) { 
			if (iVLowerLimitRealHit.Data[j] > iMax || iVUpperLimitRealHit.Data[j] > iMax)
				if ( fabs(1.0 - psResult->dNorm / dNormVF_Previous) < dMin_Chance_DLS_Residual )
					bStop = true; 
		}//end for j
		if (bStop && i >= iMinIterDLS)
			break;
	}//end for i  
//	cout << endl << "DLS result: " << psResult->dVParam.Data[0] << "\t\t" << psResult->dVParam.Data[1] << "\t\t" << psResult->dVParam.Data[2]
//		<< "\t\t" << psResult->dVParam.Data[3] << "\t\t" << psResult->dVParam.Data[4] 
//		<< endl; 
	//////cout << endl << "Levenberg-Marquardt algo: Summary:  " << i << " iterations;" << "   NormVF = " << psResult->dNorm << "   dDerivNorm = " << dDerivNorm << "\t" << endl;
	////fOutDLS << "# \t" << i << "\t" << psResult->dNorm << "\t" << dDerivNorm << "\t" << iNoCalc_Vx; 
	if (i < iMaxIter && dDerivNorm < 1.1 * dTol) //if we are not at a local maximum, where the derivative is 0 - as in case of a local minimum: 
	{
		if (psResult->dVStandardDev.Nown != dVx0.Nown)
			psResult->dVStandardDev.Init(dVx0.Nown); 
		dMatrix dMCovar(psResult->dVParam.Nown, psResult->dVParam.Nown);  double dSumSq, dSigmaSq;  int iNoDegeesOfFreedom = bSum(psResult->dVParam.Nown, bOptParam), m; 
		vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, psResult->dVParam, &dVF, &dMdFdx, bOptParam, &bSuc1);
		if (bSuc1 == false) {
			//////cout << endl << "Failure of damped least squares algorithm!" << endl;  	
			return false;
		}
		psResult->dNorm = dVF.norm();  dSumSq = psResult->dNorm * psResult->dNorm;  dSigmaSq = dSumSq / (dVF.Nown - iNoDegeesOfFreedom); 
		dMdFdx_adj.transp(dMdFdx); 
		dMCovar = dMdFdx_adj * dMdFdx;
		if (dSigmaSq > DBL_MIN) {
			dMCovar *= 1.0 / dSigmaSq;
			for (j = 0; j < dMCovar.Nown; j++) {
				if ( fabs(dMCovar.Data[j * dMCovar.Next + j]) < FLT_MIN ) 
					dMCovar.Data[j * dMCovar.Next + j] += copysign( FLT_MIN, dMCovar.Data[j * dMCovar.Next + j] ); 
			}//j
			dMInverse.vSetToUnityMat();
			dMInverse /= dMCovar;
			for (m = 0; m < (int)psResult->dVParam.Nown; m++) 
				psResult->dVStandardDev.Data[m] = sqrt(dMInverse(m, m));
		}//end if 
		else
			psResult->dVStandardDev = 0.0; 
		return true;
	}
	else { //no convergence, change nothing 
		//////cout << endl << "Failure of damped least squares algorithm!" << endl;  
		psResult->dNorm = dNorm_init; 
		return false;
	}
}




bool bLevenbergMarquardt_double2(dVector& dVx0, bool *bOptParam, dVector* pdVUpperLimit, bool *pbUpperLimitHit, dVector* pdVLowerLimit,
	bool *pbLowerLimitHit, double dMu, bool bLinearityTest, double dTol, int iMinIterDLS, int iMaxIter, double dMin_Chance_DLS_Residual, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, dVector& dVParam, dVector *pdVResult, dMatrix *pdMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, MinData_double *psResult)
	//LevenbergMarquardt algorithm as in Dahmen Reusken, "Numerik für Ingenieure und Naturwissenschaftler" on p. 222 - 224.  
	//This is the most up-to-date and enhanced version of the Levenberg-Marquardt method in this project.   W. Iff, 30.8.2024 
{
	bool bSuc1 = true, bSuc2 = true, bStop = false;
	int i, j, iNoCalc_Vx;
	const double  dBeta0 = 0.44, dBeta1 = 0.8;
	double dNorm_init = DBL_MAX, dNormVF_Previous = DBL_MAX;  psResult->dNorm = DBL_MAX;
	double dMuSq, dRho, dF_sq, dFNext_sq, dFNextLinearized_sq, dDerivNorm = 0.0, dNumRho, dDenomRho = DBL_MAX; //We had dBeta0 = 0.3 before 12.8.2024
	double dNormOfMdFdxSq, dMinAllowedMuSq, dMinAllowedMu;
	iVector iVLowerLimitRealHit(dVx0.Nown); iVLowerLimitRealHit = 0;  iVector iVUpperLimitRealHit(dVx0.Nown); iVUpperLimitRealHit = 0;
	dVector dVF(iDimVF), dVs(dVx0.Nown), dVxNext(dVx0.Nown), dVFNext(iDimVF), dVFNextLinearized(iDimVF), dVDeriv(dVx0.Nown);
	dMatrix dMdFdx(iDimVF, dVx0.Nown), dMdFdx_adj(dVx0.Nown, iDimVF), dM_dFdx_adj_x_dFdx(dVx0.Nown, dVx0.Nown), dM_dFdx_adj_x_dFdx_Copy(dVx0.Nown, dVx0.Nown),
		dMInverse(dVx0.Nown, dVx0.Nown);
	//1-dimensional optimization: 
	//int iMinIter1Dim = 1, iMaxIter1Dim = 100, iNoSamples1Dim = 5;
	//double dTol1Dim = NUM_NOISE_FLOAT, dPositionMin1Dim;
	//dVector dVxArray(iNoSamples1Dim), dVNormSamples1Dim(iNoSamples1Dim), dVxNextTemp(dVx0.Nown), dVFNextTemp(dVFNext.Nown); 
	//ofstream ofDLS_Mat("DLS_Mat.txt"), ofConvSpeed("ConvSpeed.txt"); 
	////ofstream fOutDLS("./results_1DTM/DLS_Convergence.txt");   fOutDLS << setprecision(12);   cout << setprecision(12);

	////cout << endl << "Start of Levenberg-Marquardt algo" << endl;
	psResult->dVParam = dVx0;
	//	ofDLS_Mat << endl << dVx0.Data[0] << "\t\t" << dVx0.Data[1] << "\t\t" << dVx0.Data[2] << "\t\t" << dVx0.Data[3] << "\t\t" << dVx0.Data[4] << endl; 
	//	cout << endl << dVx0.Data[0] << "\t\t" << dVx0.Data[1] << "\t\t" << dVx0.Data[2] 
	//		<< "\t\t" << dVx0.Data[3] << "\t\t" << dVx0.Data[4] 
	//		<< endl; 
	if (pbUpperLimitHit) *pbUpperLimitHit = false;  if (pbLowerLimitHit) *pbLowerLimitHit = false;
	for (i = 0; i < iMaxIter; i++) {
		////cout << "Iteration no.: " << i << "\t ";  
		//1.) Calc. F, F': 
		//		vPrint_dVec(ofDLS_Mat, "psResult->dVParam = ", psResult->dVParam); 
		vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, psResult->dVParam, &dVF, &dMdFdx, bOptParam, &bSuc1);
		//		vPrint_dVec(ofDLS_Mat, "dVF = ", dVF); 
		dNormVF_Previous = psResult->dNorm;  psResult->dNorm = dVF.norm();  if (i == 0) dNorm_init = psResult->dNorm;  ////cout << "VF.norm() = " << pNormVF << "\t"; 
		//		ofConvSpeed << i << "\t\t" << psResult->dNorm << "\t\t";  cout << i << "\t psResult->dNorm: " << psResult->dNorm;
		////cout << "MdFdx.norm() / (2 * PI / 0.6645) = " << MdFdx.norm() / (2 * PI / 0.6645) << "\t"; 
		//2.) Solve problem, calc s_i: 
		//		vPrint_dMat(ofDLS_Mat, "dMdFdx = ", dMdFdx); 
		dMdFdx_adj.transp(dMdFdx);
		//		vPrint_dMat(ofDLS_Mat, "dMdFdx_adj = ", dMdFdx_adj);
		dM_dFdx_adj_x_dFdx_Copy = dM_dFdx_adj_x_dFdx = dMdFdx_adj * dMdFdx;
		//		vPrint_dMat(ofDLS_Mat, "dM_dFdx_adj_x_dFdx = ", dM_dFdx_adj_x_dFdx); 
		dM_dFdx_adj_x_dFdx.calc_M_adj_x_M(dMdFdx_adj);
		//		vPrint_dMat(ofDLS_Mat, "dM_dFdx_adj_x_dFdx = ", dM_dFdx_adj_x_dFdx); 
		iNoCalc_Vx = 0;
	CALC_VX:
		if (iNoCalc_Vx > 500) //this means µ approx. 2^500 and µ² = approx. 2^1000 = approx. 1.0e301; 
			break; //ensure termination here (should never happen) 
		iNoCalc_Vx++;
		dM_dFdx_adj_x_dFdx = dM_dFdx_adj_x_dFdx_Copy;
		dNormOfMdFdxSq = dM_dFdx_adj_x_dFdx.norm();  dMinAllowedMuSq = dVx0.Nown * dNormOfMdFdxSq * 0.01 * NUM_NOISE_FLOAT;  dMinAllowedMu = sqrt(dMinAllowedMuSq);
		if (dMu < dMinAllowedMu)
			dMu = dMinAllowedMu; //diag elements dMuSq must have certain size so that M_dFdx_adj_x_dFdx is always invertible.    
		//		ofDLS_Mat << endl << dMu << endl;  cout << "\t dMu = " << dMu << "\t"; 
		dMuSq = dMu * dMu;
		for (j = 0; j < dVx0.Nown; j++)
			dM_dFdx_adj_x_dFdx.Data[j * dVx0.Nown + j] += dMuSq;
		//		vPrint_dMat(ofDLS_Mat, "dM_dFdx_adj_x_dFdx + dMuSq = ", dM_dFdx_adj_x_dFdx); 
		dMInverse.vSetToUnityMat();
		dMInverse /= dM_dFdx_adj_x_dFdx;
		//		vPrint_dMat(ofDLS_Mat, "dMInverse = ", dMInverse); 
		dVs = dVF * dMdFdx_adj; //dVs = dMdFdx_adj * dVF; 
		//		vPrint_dVec(ofDLS_Mat, "dVs = ", dVs); 
		dVs *= dMInverse; // dMInverse * dVs; 
		//		vPrint_dVec(ofDLS_Mat, "dVs = ", dVs); 
		dVs *= -1;
		for (j = 0; j < dVx0.Nown; j++) {
			if (bOptParam[j] == false)
				dVs.Data[j] = 0.0;
		}
		//		vPrint_dVec(ofDLS_Mat, "dVs = ", dVs); 
		dVxNext = psResult->dVParam;
		dVxNext += dVs; // dVxNext = dVx + dVs
		//		vPrint_dVec(ofDLS_Mat, "dVxNext = ", dVxNext); 
		//3.) and 4.) Linearity check with dRho: 
		dF_sq = psResult->dNorm;  dF_sq *= dF_sq; 
		if (bLinearityTest) {
			vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, dVxNext, &dVFNext, NULL, bOptParam, &bSuc2);
			dFNext_sq = dVFNext.norm();  dFNext_sq *= dFNext_sq;
			dVFNextLinearized = dVs * dMdFdx; // calc dVFNextLinearized = dVF + dMdFdx * dVs : 
			dVFNextLinearized += dVF;
			dFNextLinearized_sq = dVFNextLinearized.norm();  dFNextLinearized_sq *= dFNextLinearized_sq;
			if (bSuc1 != true || bSuc2 != true) {
				psResult->dVParam = dVx0;  return false; //change no parameters
			}
			dNumRho = dF_sq - dFNext_sq;  dDenomRho = dF_sq - dFNextLinearized_sq;
			if (fabs(dDenomRho) / ((dF_sq + dFNextLinearized_sq) / 2.0) <= 0.02 * NUM_NOISE_FLOAT) {
				dRho = 1.0;   dDenomRho = 0.0;  //perfect linearity 
			}
			else
				dRho = dNumRho / dDenomRho;  ////cout << "dRho - 1.0 = " << dRho - 1.0 << "\t"; 
			if (dVs.norm() <= MIN_STEP_DLS * psResult->dVParam.norm()) //ensure termination of the algo in case of persistent nonlinearities 
				dRho = 1.0;
		}
		else //linearity is assumed: 
			dRho = 1.0;
		//5.) Termination criteria:  
		dVDeriv = dVF * dMdFdx_adj;
		dDerivNorm = dVDeriv.norm(); //this is always needed 
		//		ofConvSpeed << dDerivNorm << endl;    cout << "\t dDerivNorm: " << dDerivNorm; 
		//4.) Linearity check with dRho:   
		if (dRho <= dBeta0) {
			dMu *= 2.0;  goto CALC_VX; //if too large step size
		}
		if (dRho >= dBeta1)
			dMu /= 2.0; //less damping  
		//bFindMinNorm1Dim(bOptParam, pdVUpperLimit, pbUpperLimitHit, pdVLowerLimit, pbLowerLimitHit, dTol1Dim, iMinIter1Dim, iMaxIter1Dim, iNoSamples1Dim, 
		//	dVxArray.Data, dVNormSamples1Dim.Data, dF_sq, dFNext_sq, psResult->dVParam, dVxNextTemp, dVxNext, dVs, &dVFNextTemp, &dVFNext,
		//	vCalc_F_or_dFdx, pInfoInt, pInfoSpec, pvAdditionalInfo, dPositionMin1Dim); //dVxNext = psResult->dVParam + dVs * dPositionMin1Dim; 
		for (j = 0; j < dVx0.Nown; j++) {
			if (pdVLowerLimit) {
				if (dVxNext.Data[j] < pdVLowerLimit->Data[j]) {
					iVLowerLimitRealHit.Data[j] += 2;  dVxNext.Data[j] = pdVLowerLimit->Data[j];
					//if (iVLowerLimitRealHit.Data[j] >= 25) 
					//	dVDeriv.Data[j] = 0.0;
					if (pbLowerLimitHit)
						*pbLowerLimitHit = true;
				}
			}//end if pVLowerLimit   
			if (pdVUpperLimit) {
				if (dVxNext.Data[j] > pdVUpperLimit->Data[j]) {
					iVUpperLimitRealHit.Data[j] += 2;   dVxNext.Data[j] = pdVUpperLimit->Data[j];
					//if (iVUpperLimitRealHit.Data[j] >= 25)  
					//	dVDeriv.Data[j] = 0.0;  
					if (pbUpperLimitHit)
						*pbUpperLimitHit = true;
				}
			}//end if pVUpperLimit  
		}//end for j
		psResult->dVParam = dVxNext;   dDerivNorm = dVDeriv.norm(); //recalculation (update) 
		//			cout << "\t\t" << psResult->dVParam.Data[0] << "\t\t" << psResult->dVParam.Data[1] << "\t\t" << psResult->dVParam.Data[2]
		//				<< "\t\t" << psResult->dVParam.Data[3] << "\t\t" << psResult->dVParam.Data[4] 
		//				<< endl;  
		////cout << " dDerivNorm = " << dDerivNorm << endl;   fOutDLS << i << "\t" << psResult->dNorm << "\t\t" << dDerivNorm << endl;  
		if (dDerivNorm < dTol  &&  fabs(1.0 - psResult->dNorm / dNormVF_Previous) < dMin_Chance_DLS_Residual  &&  i >= iMinIterDLS) //Stop if accuracy is sufficient!
			break;
		if (dVs.norm() <= MIN_STEP_DLS * psResult->dVParam.norm() && i >= iMinIterDLS) //Stop if step forward is marginal (algo has converged to something)
			break;
		if (bLinearityTest)
			if (dDerivNorm <= dTol &&  fabs(dDenomRho) <= NUM_NOISE && i >= iMinIterDLS) //Stop if perfect linearity or step forward is marginal  
				break;
		iVLowerLimitRealHit.vResetValue();   iVUpperLimitRealHit.vResetValue();
		int iMax = (int)(iMaxIter / 5.0) + 1;
		for (j = 0; j < dVx0.Nown; j++) {
			if (iVLowerLimitRealHit.Data[j] > iMax || iVUpperLimitRealHit.Data[j] > iMax)
				if (fabs(1.0 - psResult->dNorm / dNormVF_Previous) < dMin_Chance_DLS_Residual)
					bStop = true;
		}//end for j
		if (bStop && i >= iMinIterDLS)
			break;
	}//end for i  
	//	cout << endl << "DLS result: " << psResult->dVParam.Data[0] << "\t\t" << psResult->dVParam.Data[1] << "\t\t" << psResult->dVParam.Data[2]
	//		<< "\t\t" << psResult->dVParam.Data[3] << "\t\t" << psResult->dVParam.Data[4] 
	//		<< endl; 
	//////cout << endl << "Levenberg-Marquardt algo: Summary:  " << i << " iterations;" << "   NormVF = " << psResult->dNorm << "   dDerivNorm = " << dDerivNorm << "\t" << endl;
	////fOutDLS << "# \t" << i << "\t" << psResult->dNorm << "\t" << dDerivNorm << "\t" << iNoCalc_Vx; 
	if (i < iMaxIter && dDerivNorm < 1.1 * dTol) //if we are not at a local maximum, where the derivative is 0 - as in case of a local minimum: 
	{
		if (psResult->dVStandardDev.Nown != dVx0.Nown)
			psResult->dVStandardDev.Init(dVx0.Nown);
		dMatrix dMCovar(psResult->dVParam.Nown, psResult->dVParam.Nown);  double dSumSq, dSigmaSq;  int iNoDegeesOfFreedom = bSum(psResult->dVParam.Nown, bOptParam), m;
		vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, psResult->dVParam, &dVF, &dMdFdx, bOptParam, &bSuc1);
		if (bSuc1 == false) {
			//////cout << endl << "Failure of damped least squares algorithm!" << endl;  	
			return false;
		}
		psResult->dNorm = dVF.norm();  dSumSq = psResult->dNorm * psResult->dNorm;  dSigmaSq = dSumSq / (dVF.Nown - iNoDegeesOfFreedom);
		dMdFdx_adj.transp(dMdFdx);
		dMCovar = dMdFdx_adj * dMdFdx;
		if (dSigmaSq > DBL_MIN) {
			dMCovar *= 1.0 / dSigmaSq;
			for (j = 0; j < dMCovar.Nown; j++) {
				if (fabs(dMCovar.Data[j * dMCovar.Next + j]) < FLT_MIN)
					dMCovar.Data[j * dMCovar.Next + j] += copysign(FLT_MIN, dMCovar.Data[j * dMCovar.Next + j]);
			}//j
			dMInverse.vSetToUnityMat();
			dMInverse /= dMCovar;
			for (m = 0; m < (int)psResult->dVParam.Nown; m++)
				psResult->dVStandardDev.Data[m] = sqrt(dMInverse(m, m));
		}//end if 
		else
			psResult->dVStandardDev = 0.0;
		return true;
	}
	else { //no convergence, change nothing 
		//////cout << endl << "Failure of damped least squares algorithm!" << endl;  
		psResult->dNorm = dNorm_init;
		return false;
	}
}




bool bLevenbergMarquardt(Vector Vx0, bool bOptParam[], Vector* pVUpperLimit, dVector *pdVUpperModulus, bool *pbUpperLimitHit, Vector* pVLowerLimit, dVector *pdVLowerModulus, 
	bool *pbLowerLimitHit, double dMu, bool bLinearityTest, double dTol, int iMaxIter, int iDimVF,
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, Vector& VParam, Vector *pVResult, Matrix *pMResult, bool bOptParam[], bool *pSuc),
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, Vector& Vx, Vector *pVxStandardDev, double& pNormVF, bool bPlusComplexConj)
//LevenbergMarquardt algorithm as in Dahmen Reusken, "Numerik für Ingenieure und Naturwissenschaftler" on p. 222 - 224. 
{ 
	bool bSuc1 = true, bSuc2 = true, bStop = false;
	int i, j, iNoCalc_Vx = 0; 
	double dNormVF_Previous = DBL_MAX;  pNormVF = DBL_MAX; 
	double dMuSq, dRho, dF_sq, dFNext_sq, dFNextLinearized_sq, dBeta0 = 0.2, dBeta1 = 0.8, dDerivNorm = 0.0, dNumRho, dDenomRho = DBL_MAX; //dx_Solution = 1357.7799945
	double dNormOfMdFdxSq, dMinAllowedMuSq, dMinAllowedMu, dPlusComplexConj = bPlusComplexConj == true ? 2.0 : 1.0;
	iVector iVLowerLimitRealHit(Vx0.Nown), iVLowerLimitImagHit(Vx0.Nown), iVLowerLimitModulusHit(Vx0.Nown); iVLowerLimitRealHit = 0, iVLowerLimitImagHit = 0, iVLowerLimitModulusHit = 0; 
	iVector iVUpperLimitRealHit(Vx0.Nown), iVUpperLimitImagHit(Vx0.Nown), iVUpperLimitModulusHit(Vx0.Nown); iVUpperLimitRealHit = 0, iVUpperLimitImagHit = 0, iVUpperLimitModulusHit = 0; 
	Vector VF(iDimVF), VTmp(Vx0.Nown), Vs(Vx0.Nown), VxNext(Vx0.Nown), VFNext(iDimVF), VFNextLinearized(iDimVF), VDeriv(Vx0.Nown); 
	Matrix MdFdx(iDimVF, Vx0.Nown), MdFdx_adj(Vx0.Nown, iDimVF), M_dFdx_adj_x_dFdx(Vx0.Nown, Vx0.Nown), MInverse(Vx0.Nown, Vx0.Nown); 
	////ofstream fOutDLS("./results_1DTM/DLS_Convergence.txt");   fOutDLS << setprecision(12);   cout << setprecision(12);

	////cout << endl << "Start of Levenberg-Marquardt algo" << endl;
	Vx = Vx0;  
	if (pbUpperLimitHit) *pbUpperLimitHit = false;  if (pbLowerLimitHit) *pbLowerLimitHit = false; 
	for (i = 0; i < iMaxIter; i++) {  
		////cout << "Iteration no.: " << i << "\t ";  
		//1.) Calc. F, F': 
		vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, Vx, &VF, &MdFdx, bOptParam, &bSuc1);
		dNormVF_Previous = pNormVF;  pNormVF = sqrt(dPlusComplexConj) * VF.norm();   ////cout << "VF.norm() = " << pNormVF << "\t";
		////cout << "MdFdx.norm() / (2 * PI / 0.6645) = " << MdFdx.norm() / (2 * PI / 0.6645) << "\t"; 
		//2.) Solve problem, calc s_i: 
		vSet_M_adjoint(MdFdx, MdFdx_adj);  
		M_dFdx_adj_x_dFdx = MdFdx_adj * MdFdx;  
		if (bPlusComplexConj)
			for (j = 0; j < M_dFdx_adj_x_dFdx.Nown * M_dFdx_adj_x_dFdx.Next; j++)
				M_dFdx_adj_x_dFdx.Data[j] = dPlusComplexConj * M_dFdx_adj_x_dFdx.Data[j].real();
	CALC_VX:
		iNoCalc_Vx++; 
		dNormOfMdFdxSq = M_dFdx_adj_x_dFdx.norm();  dMinAllowedMuSq = Vx0.Nown * dNormOfMdFdxSq * 0.1 * NUM_NOISE_FLOAT;  dMinAllowedMu = sqrt(dMinAllowedMuSq);
		if (dMu < dMinAllowedMu)
			dMu = dMinAllowedMu; //diag elements dMuSq must have certain size so that M_dFdx_adj_x_dFdx is always invertible.    
		dMuSq = dMu * dMu;  
		for (j = 0; j < Vx0.Nown; j++) 
			M_dFdx_adj_x_dFdx.Data[j * Vx0.Nown + j] += dMuSq;
		MInverse.vSetToUnityMat(); 
		MInverse /= M_dFdx_adj_x_dFdx;
		VTmp = MdFdx_adj * VF;  
		if(bPlusComplexConj)
			for(j = 0; j < VTmp.Nown; j++)
				VTmp.Data[j] = dPlusComplexConj * VTmp.Data[j].real();
		Vs = MInverse * VTmp; 
		Vs *= -1; 
		VxNext = Vx + Vs;  
		//3.) and 4.) Linearity check with dRho: 
		if (bLinearityTest) {
			dF_sq = pNormVF;  dF_sq *= dF_sq;
			vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, VxNext, &VFNext, NULL, bOptParam, &bSuc2);
			dFNext_sq = VFNext.norm();
			dFNext_sq *= dFNext_sq * dPlusComplexConj;
			VFNextLinearized = VF + MdFdx * Vs;  dFNextLinearized_sq = VFNextLinearized.norm();  dFNextLinearized_sq *= dPlusComplexConj * dFNextLinearized_sq;
			if (bSuc1 != true || bSuc2 != true) {
				Vx = Vx0;  return false; //change no parameters
			}
			dNumRho = dF_sq - dFNext_sq;  dDenomRho = dF_sq - dFNextLinearized_sq;
			dRho = dNumRho / dDenomRho;  ////cout << "dRho - 1.0 = " << dRho - 1.0 << "\t"; 
		}
		else //linearity is assumed: 
			dRho = 1.0; 
		//5.) Termination criteria:  
		VDeriv = MdFdx_adj * VF;  
		if (bPlusComplexConj)
			for (j = 0; j < VDeriv.Nown; j++)
				VDeriv.Data[j] = dPlusComplexConj * VDeriv.Data[j].real();
		dDerivNorm = VDeriv.norm(); //this is always needed 
		if (bLinearityTest)
			if (/*dNumRho == 0.0 ||*/ dDenomRho == 0.0) //Stop if step forward is marginal (algo has converged) - meaning if Vs or MdFdx is too small!
				break;  
		if (  (Vs.norm() <= MIN_STEP_DLS * Vx.norm() && i > 0)  ||  (dDerivNorm < dTol / 10.0 && 1.0 - pNormVF / dNormVF_Previous < CHANGE_OF_DLS_RESIDUAL)  )
			break; 
		//4.) Linearity check with dRho:  
		if (dRho <= dBeta0) {
			dMu *= 2.0;  goto CALC_VX; 
		}
		else { //step size is OK: 
			if (dRho >= dBeta1) 
				dMu /= 2.0; //less damping  
			for (j = 0; j < Vx0.Nown; j++) { 
				if (pVLowerLimit) {
					if (VxNext.Data[j].real() < pVLowerLimit->Data[j].real()) {
						iVLowerLimitRealHit.Data[j] += 2;  VxNext.Data[j].real(pVLowerLimit->Data[j].real());
						if (iVLowerLimitRealHit.Data[j] >= 3) VDeriv.Data[j].real(0.0); 
						if (pbLowerLimitHit) *pbLowerLimitHit = true;
					}
					if (VxNext.Data[j].imag() < pVLowerLimit->Data[j].imag()) {
						iVLowerLimitImagHit.Data[j] += 2;   VxNext.Data[j].imag(pVLowerLimit->Data[j].imag());  
						if (iVLowerLimitImagHit.Data[j] >= 3) VDeriv.Data[j].imag(0.0);  if (pbLowerLimitHit) *pbLowerLimitHit = true;
					}
				}//end if pVLowerLimit   
				if (pdVLowerModulus) 
					if (abs(VxNext.Data[j]) < pdVLowerModulus->Data[j]) {
						iVLowerLimitModulusHit.Data[j] += 2; 
						if (abs(VxNext.Data[j]) <= NUM_NOISE)
							VxNext.Data[j] = pdVLowerModulus->Data[j]; 
						else 
							VxNext.Data[j] *= pdVLowerModulus->Data[j] / abs(VxNext.Data[j]);  
						if (iVLowerLimitModulusHit.Data[j] >= 3) VDeriv.Data[j] = 0.0;  if (pbLowerLimitHit) *pbLowerLimitHit = true;
					}
				if (pVUpperLimit) { 
					if (VxNext.Data[j].real() > pVUpperLimit->Data[j].real()) { 
						iVUpperLimitRealHit.Data[j] += 2;   VxNext.Data[j].real(pVUpperLimit->Data[j].real());  
						if (iVUpperLimitRealHit.Data[j] >= 3)  VDeriv.Data[j].real(0.0);  if (pbUpperLimitHit) *pbUpperLimitHit = true;
					}
					if (VxNext.Data[j].imag() > pVUpperLimit->Data[j].imag()) {
						iVUpperLimitImagHit.Data[j] += 2;  VxNext.Data[j].imag(pVUpperLimit->Data[j].imag());  
						if (iVUpperLimitImagHit.Data[j] >= 3)  VDeriv.Data[j].imag(0.0);  if (pbUpperLimitHit) *pbUpperLimitHit = true;
					}
				}//end if pVUpperLimit  
				if (pdVUpperModulus) 
					if (abs(VxNext.Data[j]) > pdVUpperModulus->Data[j]) {
						iVUpperLimitModulusHit.Data[j] += 2;  VxNext.Data[j] *= pdVUpperModulus->Data[j] / abs(VxNext.Data[j]);  
						if (iVUpperLimitModulusHit.Data[j] >= 3) VDeriv.Data[j] = 0.0;  if (pbUpperLimitHit) *pbUpperLimitHit = true;
					}
			}//end for j
			Vx = VxNext;   dDerivNorm = VDeriv.norm(); //recalculation (update) 
		}//end else step size is OK 
		////cout << " dDerivNorm = " << dDerivNorm << endl;   fOutDLS << i << "\t" << pNormVF << "\t\t" << dDerivNorm << endl;  
		if ( dDerivNorm < dTol  &&  1.0 - pNormVF / dNormVF_Previous < CHANGE_OF_DLS_RESIDUAL ) //Stop if accuracy is sufficient!
			break; 
		iVLowerLimitRealHit.vResetValue();  iVLowerLimitImagHit.vResetValue();  iVLowerLimitModulusHit.vResetValue();   
		iVUpperLimitRealHit.vResetValue();  iVUpperLimitImagHit.vResetValue();  iVUpperLimitModulusHit.vResetValue(); 
		for (j = 0; j < Vx0.Nown; j++) {
			int iMax = (int)(iMaxIter / 50.0 + 3); // + 2  
			if( iVLowerLimitRealHit.Data[j] > iMax || iVLowerLimitImagHit.Data[j] > iMax || iVLowerLimitModulusHit.Data[j] > iMax ||
				iVUpperLimitRealHit.Data[j] > iMax || iVUpperLimitImagHit.Data[j] > iMax || iVUpperLimitModulusHit.Data[j] > iMax ) 
				bStop = true; 
		}//end for 
		if (bStop)
			break; 
	}//end for i  
	//////cout << endl << "Levenberg-Marquardt algo: Summary:  " << i << " iterations;" << "   NormVF = " << pNormVF << "   dDerivNorm = " << dDerivNorm << "\t" << endl;
	////fOutDLS << "# \t" << i << "\t" << pNormVF << "\t" << dDerivNorm << "\t" << iNoCalc_Vx; 
	if (i < iMaxIter && dDerivNorm < 10.0 * dTol) //if we are not at a local maximum, where the derivative is 0 - as in case of a local minimum: 
	{
		if (pVxStandardDev == NULL) 
			return true; 
		Matrix MCovar(Vx.Nown, Vx.Nown);  double dSumSq, dSigmaSq;  int m, n; 
		vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, Vx, &VF, &MdFdx, bOptParam, &bSuc1);
		if (bSuc1 == false) {
			//////cout << endl << "Failure of damped least squares algorithm!" << endl;  	
			return false;
		}
		pNormVF = sqrt(dPlusComplexConj) * VF.norm();  dSumSq = pNormVF * pNormVF;  dSigmaSq = dSumSq / (VF.Nown - Vx.Nown);
		vSet_M_adjoint(MdFdx, MdFdx_adj); 
		MCovar = MdFdx_adj * MdFdx;
		for (m = 0; m < (int)Vx.Nown; m++) //add complex conj.:
			for (n = 0; n < (int)Vx.Nown; n++)
				MCovar(m, n) = MCovar(m, n).real() * 2.0; 
		if (dSigmaSq > DBL_MIN) {
			MCovar *= 1.0 / dSigmaSq;
			MInverse.vSetToUnityMat();
			MInverse /= MCovar;
			for (m = 0; m < (int)Vx.Nown; m++)
				pVxStandardDev->Data[m] = sqrt(MInverse(m, m)); 
		}
		else 
			*pVxStandardDev = 0.0; 
		return true;
	}
	else { //no convergence, change nothing 
		//////cout << endl << "Failure of damped least squares algorithm!" << endl;  	
		return false;
	} 
}

bool bLevenbergMarquardtScalar(Vector Vx0, bool bOptParam[], Vector* pVUpperLimit, dVector *pdVUpperModulus, bool *pbUpperLimitHit, Vector* pVLowerLimit, dVector *pdVLowerModulus,
	bool *pbLowerLimitHit, double dMu, bool bLinearityTest, double dTol, int iMaxIter, int iDimVF_Nown, 
	void(*vCalc_F_or_dFdx)(void *pInfoInt, void *pInfoSpectrum, void *pvAdditionalInfo, Vector& VParam, double *pdResult, Matrix *pMResult, bool bOptParam[], bool *pSuc),
	void vLimitTheStepSize(Vector &Vx, Vector &Vs, Vector &VxNext, void *pInfoSpectrum, double &dMu), 
	void *pInfoInt, void *pInfoSpec, void *pvAdditionalInfo, Vector& Vx, Vector *pVxStandardDev, double& dNormF)
	//LevenbergMarquardt algorithm as in Dahmen Reusken, "Numerik für Ingenieure und Naturwissenschaftler" on p. 222 - 224, but for a scalar merit function. 
{
	bool bSuc1 = true, bSuc2 = true, bStop = false;
	int i, j, iNoCalc_Vx = 0;
	double dNormF_Previous = DBL_MAX;  dNormF = DBL_MAX;
	double dMuSq, dRho, dF_sq, dFNext_sq, dFNextLinearized_sq, dBeta0 = 0.2, dBeta1 = 0.8, dDerivNorm = 0.0, dNumRho, dDenomRho = DBL_MAX; //dx_Solution = 1357.7799945
	double dNormOfMdFdxSq, dMinAllowedMuSq, dMinAllowedMu; // , dPlusComplexConj = bPlusComplexConj == true ? 2.0 : 1.0;
	iVector iVLowerLimitRealHit(Vx0.Nown), iVLowerLimitImagHit(Vx0.Nown), iVLowerLimitModulusHit(Vx0.Nown); iVLowerLimitRealHit = 0, iVLowerLimitImagHit = 0, iVLowerLimitModulusHit = 0;
	iVector iVUpperLimitRealHit(Vx0.Nown), iVUpperLimitImagHit(Vx0.Nown), iVUpperLimitModulusHit(Vx0.Nown); iVUpperLimitRealHit = 0, iVUpperLimitImagHit = 0, iVUpperLimitModulusHit = 0;
	double dF, dFNext = INT_MAX, dFNextLinearized; 
	Vector VTmp(Vx0.Nown), Vs(Vx0.Nown), VxNext(Vx0.Nown), VDeriv(Vx0.Nown); 
	Matrix MdFdx(1, Vx0.Nown), MdFdx_adj(Vx0.Nown, 1), M_dFdx_adj_x_dFdx(Vx0.Nown, Vx0.Nown), M_dFdx_adj_x_dFdx_damped(Vx0.Nown, Vx0.Nown), MInverse(Vx0.Nown, Vx0.Nown);
	////ofstream fOutDLS("./results_1DTM/DLS_Convergence.txt");   fOutDLS << setprecision(12);   cout << setprecision(12);

	////cout << endl << "Start of Levenberg-Marquardt algo" << endl;
	Vx = Vx0;
	if (pbUpperLimitHit) *pbUpperLimitHit = false;  if (pbLowerLimitHit) *pbLowerLimitHit = false;
	for (i = 0; i < iMaxIter; i++) {
		cout << endl << "Iteration no.: " << i << "\t " << endl;  
		//1.) Calc. F, F': 
		if (i > 0 && bLinearityTest == true) {
			vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, Vx, NULL, &MdFdx, bOptParam, &bSuc1);  dF = dFNext; 
		}
		else 
			vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, Vx, &dF, &MdFdx, bOptParam, &bSuc1); 
		if (bSuc1 != true) {
			Vx = Vx0;  return false; //change no parameters
		}
		dNormF_Previous = dNormF;  dNormF = dF;   ////cout << "VF.norm() = " << pNormVF << "\t";
		////cout << "MdFdx.norm() / (2 * PI / 0.6645) = " << MdFdx.norm() / (2 * PI / 0.6645) << "\t"; 
		//2.) Solve problem, calc s_i: 
		vSet_M_adjoint(MdFdx, MdFdx_adj);
		M_dFdx_adj_x_dFdx = MdFdx_adj * MdFdx;  
		dNormOfMdFdxSq = M_dFdx_adj_x_dFdx.norm();  dMinAllowedMuSq = Vx0.Nown * dNormOfMdFdxSq * 0.1 * NUM_NOISE_FLOAT;  dMinAllowedMu = sqrt(dMinAllowedMuSq); 
	CALC_VX:
		iNoCalc_Vx++;
		if (dMu < dMinAllowedMu)
			dMu = dMinAllowedMu; //diag elements dMuSq must have certain size so that M_dFdx_adj_x_dFdx is always invertible.    
		dMuSq = dMu * dMu;
		for (j = 0; j < Vx0.Nown * Vx0.Nown; j++) //reset matrix:
			M_dFdx_adj_x_dFdx_damped.Data[j] = M_dFdx_adj_x_dFdx.Data[j];  
		for (j = 0; j < Vx0.Nown; j++) //add damping:
			M_dFdx_adj_x_dFdx_damped.Data[j * Vx0.Nown + j] += dMuSq;
		MInverse.vSetToUnityMat();
		MInverse /= M_dFdx_adj_x_dFdx_damped;
		for (j = 0; j < Vx0.Nown; j++)
			VTmp.Data[j] = MdFdx_adj.Data[j] * dF;
		Vs = MInverse * VTmp;
		Vs *= -1;
		VxNext = Vx + Vs;
		vLimitTheStepSize(Vx, Vs, VxNext, pInfoSpec, dMu);
		//3.) and 4.) Linearity check with dRho: 
		if (bLinearityTest) {
			dF_sq = dNormF;  dF_sq *= dF_sq;
			vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, VxNext, &dFNext, NULL, bOptParam, &bSuc2);
			dFNext_sq = dFNext * dFNext;
			dFNextLinearized = dF; 
			for (j = 0; j < Vx0.Nown; j++)
				dFNextLinearized += MdFdx.Data[j].real() * Vs.Data[j].real(); //All considered quantities are real here since the merit function F and the parameters s are real.   
			dFNextLinearized_sq = dFNextLinearized * dFNextLinearized; 
			if (bSuc2 != true) {
				Vx = Vx0;  return false; //change no parameters
			}
			dNumRho = dF_sq - dFNext_sq;  dDenomRho = dF_sq - dFNextLinearized_sq;
			dRho = dNumRho / dDenomRho;  ////cout << "dRho - 1.0 = " << dRho - 1.0 << "\t"; 
		}
		else //linearity is assumed: 
			dRho = 1.0;
		//5.) Termination criteria:  
		for (j = 0; j <Vx0.Nown ; j++)
			VDeriv.Data[j] = MdFdx_adj.Data[j] * dF; 
		dDerivNorm = VDeriv.norm(); //this is always needed 
		if (bLinearityTest)
			if (/*dNumRho == 0.0 ||*/ dDenomRho == 0.0) //Stop if step forward is marginal (algo has converged) - meaning if Vs or MdFdx is too small!
				break;
		if ((Vs.norm() <= MIN_STEP_DLS * Vx.norm() && i > 0) || (dDerivNorm < dTol / 10.0 && 1.0 - dNormF / dNormF_Previous < CHANGE_OF_DLS_RESIDUAL))
			break;
		//4.) Linearity check with dRho:  
		if (dRho <= dBeta0) { 
			cout << endl << "repeat CALC_VX:" << endl; 
			dMu *= 2.0;  goto CALC_VX;
		}
		else { //step size is OK: 
			if (dRho >= dBeta1)
				dMu /= 2.0; //less damping  
			for (j = 0; j < Vx0.Nown; j++) {
				if (pVLowerLimit) {
					if (VxNext.Data[j].real() < pVLowerLimit->Data[j].real()) {
						iVLowerLimitRealHit.Data[j] += 2;  VxNext.Data[j].real(pVLowerLimit->Data[j].real());
						if (iVLowerLimitRealHit.Data[j] >= 3) VDeriv.Data[j].real(0.0);
						if (pbLowerLimitHit) *pbLowerLimitHit = true;
					}
					if (VxNext.Data[j].imag() < pVLowerLimit->Data[j].imag()) {
						iVLowerLimitImagHit.Data[j] += 2;   VxNext.Data[j].imag(pVLowerLimit->Data[j].imag());
						if (iVLowerLimitImagHit.Data[j] >= 3) VDeriv.Data[j].imag(0.0);  if (pbLowerLimitHit) *pbLowerLimitHit = true;
					}
				}//end if pVLowerLimit   
				if (pdVLowerModulus)
					if (abs(VxNext.Data[j]) < pdVLowerModulus->Data[j]) {
						iVLowerLimitModulusHit.Data[j] += 2;
						if (abs(VxNext.Data[j]) <= NUM_NOISE)
							VxNext.Data[j] = pdVLowerModulus->Data[j];
						else
							VxNext.Data[j] *= pdVLowerModulus->Data[j] / abs(VxNext.Data[j]);
						if (iVLowerLimitModulusHit.Data[j] >= 3) VDeriv.Data[j] = 0.0;  if (pbLowerLimitHit) *pbLowerLimitHit = true;
					}
				if (pVUpperLimit) {
					if (VxNext.Data[j].real() > pVUpperLimit->Data[j].real()) {
						iVUpperLimitRealHit.Data[j] += 2;   VxNext.Data[j].real(pVUpperLimit->Data[j].real());
						if (iVUpperLimitRealHit.Data[j] >= 3)  VDeriv.Data[j].real(0.0);  if (pbUpperLimitHit) *pbUpperLimitHit = true;
					}
					if (VxNext.Data[j].imag() > pVUpperLimit->Data[j].imag()) {
						iVUpperLimitImagHit.Data[j] += 2;  VxNext.Data[j].imag(pVUpperLimit->Data[j].imag());
						if (iVUpperLimitImagHit.Data[j] >= 3)  VDeriv.Data[j].imag(0.0);  if (pbUpperLimitHit) *pbUpperLimitHit = true;
					}
				}//end if pVUpperLimit  
				if (pdVUpperModulus)
					if (abs(VxNext.Data[j]) > pdVUpperModulus->Data[j]) {
						iVUpperLimitModulusHit.Data[j] += 2;  VxNext.Data[j] *= pdVUpperModulus->Data[j] / abs(VxNext.Data[j]);
						if (iVUpperLimitModulusHit.Data[j] >= 3) VDeriv.Data[j] = 0.0;  if (pbUpperLimitHit) *pbUpperLimitHit = true;
					}
			}//end for j
			Vx = VxNext;   dDerivNorm = VDeriv.norm(); //recalculation (update) 
		}//end else step size is OK 
		////cout << " dDerivNorm = " << dDerivNorm << endl;   fOutDLS << i << "\t" << pNormVF << "\t\t" << dDerivNorm << endl;  
		if (dDerivNorm < dTol  &&  1.0 - dNormF / dNormF_Previous < CHANGE_OF_DLS_RESIDUAL) //Stop if accuracy is sufficient!
			break;
		iVLowerLimitRealHit.vResetValue();  iVLowerLimitImagHit.vResetValue();  iVLowerLimitModulusHit.vResetValue();
		iVUpperLimitRealHit.vResetValue();  iVUpperLimitImagHit.vResetValue();  iVUpperLimitModulusHit.vResetValue();
		for (j = 0; j < Vx0.Nown; j++) {
			int iMax = (int)(iMaxIter / 50.0 + 3); // + 2  
			if (iVLowerLimitRealHit.Data[j] > iMax || iVLowerLimitImagHit.Data[j] > iMax || iVLowerLimitModulusHit.Data[j] > iMax ||
				iVUpperLimitRealHit.Data[j] > iMax || iVUpperLimitImagHit.Data[j] > iMax || iVUpperLimitModulusHit.Data[j] > iMax)
				bStop = true;
		}//end for 
		if (bStop)
			break;
	}//end for i  
	//////cout << endl << "Levenberg-Marquardt algo: Summary:  " << i << " iterations;" << "   NormVF = " << pNormVF << "   dDerivNorm = " << dDerivNorm << "\t" << endl;
	////fOutDLS << "# \t" << i << "\t" << pNormVF << "\t" << dDerivNorm << "\t" << iNoCalc_Vx; 
	if (i < iMaxIter && dDerivNorm < 10.0 * dTol) //if we are not at a local maximum, where the derivative is 0 - as in case of a local minimum: 
	{
		if (pVxStandardDev == NULL)
			return true;
		Matrix MCovar(Vx.Nown, Vx.Nown);  double dSumSq, dSigmaSq;  int m, n;
		vCalc_F_or_dFdx(pInfoInt, pInfoSpec, pvAdditionalInfo, Vx, &dF, &MdFdx, bOptParam, &bSuc1);
		if (bSuc1 != true)  	
			return false;   
		dSumSq = dNormF * dNormF;  dSigmaSq = dSumSq / (iDimVF_Nown - Vx.Nown);
		vSet_M_adjoint(MdFdx, MdFdx_adj);
		MCovar = MdFdx_adj * MdFdx;
		for (m = 0; m < (int)Vx.Nown; m++) //add complex conj.:
			for (n = 0; n < (int)Vx.Nown; n++)
				MCovar(m, n) = MCovar(m, n).real() * 2.0;
		if (dSigmaSq > DBL_MIN) {
			MCovar *= 1.0 / dSigmaSq;
			MInverse.vSetToUnityMat();
			MInverse /= MCovar;
			for (m = 0; m < (int)Vx.Nown; m++)
				pVxStandardDev->Data[m] = sqrt(MInverse(m, m));
		}
		else
			*pVxStandardDev = 0.0;
		return true;
	}
	else { //no convergence, change nothing 
		//////cout << endl << "Failure of damped least squares algorithm!" << endl;  	
		return false;
	}
}

void vPrint_dVec(ofstream & of, char *szText, dVector & dV)
{
	of << endl;
	while (*szText != 0) {
		of << *szText;
		szText++;
	}
	of << endl; 
	int n;
	for (n = 0; n < dV.Nown; n++)  {
		of << dV.Data[n] << "\t\t";
		of << endl; 
	}
	of << endl << endl;
}


void vPrint_dMat( ofstream & of, char *szText, dMatrix & dM )
{
	of << endl;
	while (*szText != 0) {
		of << *szText;
		szText++; 
	}
	of << endl;
	int n, nn; 
	for (n = 0; n < dM.Nown; n++)  {
		for (nn = 0; nn < dM.Next; nn++)
			of << dM.Data[n * dM.Next + nn] << "\t\t";
		of << endl;
	}
	of << endl << endl;
}