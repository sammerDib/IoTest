/**
*** Project: Unity SW (?)
*** Organisation : Unity SC / Fogale Optique
***
*** File: VSI_StackImages.c
***
*** Object : *** 	Various implementation and basic array tools
***          for VSI StackImages processing
***
*** Date:			02/05/2023
*** Last update:    29/06/2023
*** Author : Bruno Luong
***
**/

#include "VSI_StackImages.h"

#if defined(_OPENMP)
#include "omp.h"
#endif

//#define DEBUG_ON_FILE 1

#ifdef MATLAB
#include "mex.h"
#define DEBUG_ON_FILE 0
#define PRINTF mexPrintf
#else
#define PRINTF printf
#endif

/* Set to 1 to have tracking of dynamic memory allocation then
  call dbg_memstate() to display the activities and summrize */
#define DEBUG_MEMORY 0

static unsigned char SingleNaN_bytes[4] = { 0, 0, 192, 255 };
static unsigned char DoubleNaN_bytes[8] = { 0, 0, 0, 0, 0, 0, 248, 255 };
static unsigned char SingleInf_bytes[4] = { 0, 0, 128, 127 };
static unsigned char DoubleInf_bytes[8] = { 0, 0, 0, 0, 0, 0, 240, 127 };

#define NAN_SGL (*((float*)(SingleNaN_bytes)))
#define INF_SGL (*((float*)(SingleInf_bytes)))

#define NAN_DBL (*((double*)(DoubleNaN_bytes)))
#define INF_DBL (*((double*)(DoubleInf_bytes)))

#define ISNAN(x)	(x!=x)

#ifndef NAN
#define NAN NAN_DBL
#endif
#ifndef INF
#define INF INF_DBL
#endif

#ifndef PI
#define PI	    3.14159265358979323846264338327950288419716939937510
#endif
#define PI2	    6.28318530717958647692528676655900576839433879875021
#define IPI2    0.15915494309189533576888376337251436203445964574046

#ifndef max
#define max(a,b)					(((a) > (b)) ? (a) : (b))
#endif

#ifndef min
#define min(a,b)					(((a) < (b)) ? (a) : (b))
#endif

#define CEIL(x)						ceil(x)
#define FLOOR(x)					floor(x)
#define ROUND(x)					round(x)

#if DEBUG_MEMORY == 1
/* Capture malloc and free to check correctness */
#define MALLOC_VSI(n)				dbg_malloc(n)
#define REALLOC_VSI(p,n)			dbg_realloc(p,n)
#define FREE_VSI(p)					dbg_free(p)

#else
#ifdef UNITY_SW
#define MALLOC_VSI(n)				malloc(n)
#define REALLOC_VSI(p,n)			realloc(p,n)
#define FREE_VSI(p)					if (p) free(p)
#else
#define MALLOC_VSI(n)				malloc(n)
#define REALLOC_VSI(p,n)			realloc(p,n)
#define FREE_VSI(p)					if (p) free(p)
#endif

#endif

/**********************************************************************
* Implementation of Debug memory
* Keep track of malloc and free
**********************************************************************/

typedef struct
{
    void* p;
    size_t n;							    // memory bytes size
    UInt8 allocated;						// 1: allocated, 0: not allocated or freed
} MemoryTrackRecord;

/* This part is used to save array into text file for debugging */
static MemoryTrackRecord* DEBUG_M = NULL;
static int M_NREC = 0;
static int M_ALLOC_REC = 0;

void* dbg_malloc(size_t n)
{
    void* p;
    p = malloc(n);

	if (M_NREC >= M_ALLOC_REC)
	{
        /* Grow array if it is too small */
		M_ALLOC_REC = max(M_NREC+1, 2 * M_ALLOC_REC);
		DEBUG_M = realloc(DEBUG_M, sizeof(MemoryTrackRecord) * M_ALLOC_REC);
	}
	DEBUG_M[M_NREC].p = p;
	DEBUG_M[M_NREC].n = n;
	DEBUG_M[M_NREC].allocated = 1;
    M_NREC++;
	return p;
}

void dbg_free(void *p)
{
    int i;
    VSI_BOOL found;
    if (p != NULL)
    {
        free(p);

        found = 0;
        for (i = 0; i < M_NREC; i++)
        {
            found = (p == DEBUG_M[i].p) && (DEBUG_M[i].allocated);
            if (found)
            {
                DEBUG_M[i].allocated = 0;
                break;
            }
        }
        if (!found)
        {
            PRINTF("WARNING: free on non invalid pointer\n");
        }
    }
}

void* dbg_realloc(void *pold, size_t n)
{
    void* p;
    p = dbg_malloc(n);
    if (p && pold)
        memcpy(p, pold, n);
    dbg_free(pold);
    return p;
}

void VSI_dbg_memstate()
{
	int i;
    size_t leakblocks, totalblocks;
    long int totalbytes, leakbytes;

    totalbytes = leakbytes = 0;
    totalblocks = leakblocks = 0;
	for (i = 0; i < M_NREC; i++)
	{
        totalbytes += (long int)DEBUG_M[i].n;
        totalblocks++;
        if (DEBUG_M[i].allocated)
        {
            leakbytes += (long int)DEBUG_M[i].n;
            leakblocks++;
        }
        PRINTF("block #%i, address %p, size %i, allocate %i\n",
            i, DEBUG_M[i].p, (int)DEBUG_M[i].n, (int)DEBUG_M[i].allocated);
	}
    PRINTF("MEMSTATE summarize:\n");
    PRINTF("\tleak blocks  = %i/%i\n", (int)leakblocks, (int)totalblocks);
    PRINTF("\tleak bytes   = %li/%li\n", leakbytes, totalbytes);
}

/* This part is used to save array into text file for debugging */
static double* DEBUG_A = NULL;
static int A_BYTES = 0;
static int A_ALLOC_BYTES = 0;
static ArrayType A_TYPE = VSI_REAL;

void DEBUG_CLOSE()
{
    if (DEBUG_M != NULL)
        free(DEBUG_M);
    DEBUG_M = NULL;
    M_NREC = M_ALLOC_REC = 0;

    if (DEBUG_A != NULL)
        free(DEBUG_A);
    DEBUG_A = NULL;
    A_BYTES = A_ALLOC_BYTES = 0;
}

void DEBUG_STORE_RAW(const double* a, int n, ArrayType Type)
{
    int abytes;
    A_TYPE = Type;
    if (A_TYPE == VSI_REAL)
        abytes = n * sizeof(double);
    else
        abytes = 2 * n * sizeof(double);

    A_BYTES = abytes;
    if (A_BYTES > A_ALLOC_BYTES)
    {
        A_ALLOC_BYTES = max(A_BYTES, 2 * A_ALLOC_BYTES);
        DEBUG_A = realloc(DEBUG_A, A_ALLOC_BYTES);
    }
    memcpy(DEBUG_A, a, A_BYTES);
}

void DEBUG_STORE_NUM(const NumArrayType* a)
{
    DEBUG_STORE_RAW(a->data.Pr, a->n, a->Type);
}

void DEBUG_STORE_IMAGE(const DOUBLE_RM_Image* Im)
{
    DEBUG_STORE_RAW(Im->Data, Im->nx * Im->ny, VSI_REAL);
}

ErrorFlagType DEBUG_SAVE2FILE(char* filename)
{
    FILE* fid;
    int n, i;
    if (DEBUG_A == NULL)
        return VSI_OK_FLAG;
    fopen_s(&fid, filename, "wb");
    if (fid != NULL)
    {
        if (A_TYPE == VSI_REAL)
        {
            n = A_BYTES / sizeof(double);
            for (i = 0; i < n; i++)
            {
                fprintf(fid, "%1.10f\n", DEBUG_A[i]);
            }
        }
        else
        {
            n = A_BYTES / (2 * sizeof(double));
            for (i = 0; i < n; i++)
            {
                fprintf(fid, "%1.10f,\t%1.10f\n", DEBUG_A[2 * i], DEBUG_A[2 * i + 1]);
            }
        }
        fclose(fid);
        return VSI_OK_FLAG;
    }
    else return VSI_FILE_ISSUE;
}

VSI_API(ErrorFlagType) VSI_Get_Version(VSI_VERSION_TAG *Tag)
{
	memcpy(Tag, The_VSI_Version, sizeof(VSI_VERSION_TAG));
	return VSI_OK_FLAG;
}

// Initialization  VSI modules 
VSI_API(ErrorFlagType) VSI_InitModule(void)
{
	return VSI_OK_FLAG;
}

// Close VSI modules 
VSI_API(ErrorFlagType) VSI_CloseModule(void)
{
    DEBUG_CLOSE();

	return VSI_OK_FLAG;
}

VSI_API(ErrorFlagType) VSI_DefaultOptions(VSI_StackImages_opt_Type *options)
{

	memset(options, 0, sizeof(VSI_StackImages_opt_Type));

    options->OutputSelectionMask        = DEF_OUTPUTSELECTIONMASK;
	options->PhaseMethod				= DEF_PHASEMETHOD;
	options->AmplitudeMethod			= DEF_AMPLITUDEMETHOD;
	options->ResamplingMethod			= DEF_RESAMPLINGMETHOD;
	options->FrequencyMethod			= DEF_FREQUENCYMETHOD;
	options->WindowType					= DEF_WINDOWTYPE;
	options->Ewidth						= DEF_EWIDTH;
	options->SpectrumSkewness			= DEF_SPECTRUMSKEWNESS;
	options->nwin						= DEF_NWIN;
	options->PeakMethod					= DEF_PEAKMETHOD;
	options->maxiter					= DEF_GOLDENSEARCH_MAXITER;
	options->LoRelwidth					= DEF_LORELWIDTH;
	options->HiRelwidth					= DEF_HIRELWIDTH;
	options->PeakDetectionRelThreshold	= DEF_PEAKDETECTIONTHRESHOLD;
    options->UseFixLambda               = DEF_USEFIXLAMBDA;
	options->MaskThreshold				= DEF_MASKTHRESHOLD;
    options->AmplitudeCmpWindow         = DEF_AMPLITUDECMPWINDOW;
	options->phasewinsize				= DEF_PHASEWINSIZE;
	options->SlopeCorrectionMethod      = DEF_SLOPECORRECTIONMETHOD;
	options->Verbose_Level				= DEF_VERBOSE_LEVEL;
	options->GraphicalMask				= DEF_GRAPHICAL_MASK;

	return VSI_OK_FLAG;
}


inline int Dblsign(double val) { return ((0.0 < val) - (val < 0.0)); }

double MinDblArray(const double* a, int n, int* pimin)
{
    int i, imin = 0;
    double res;
    res = +INF;
    for (i = 0; i < n; i++)
        if (a[i] < res) res = a[imin = i];
    if (pimin) *pimin = imin;
    return res;
}

double MaxDblArray(const double* a, int n, int* pimax)
{
    int i, imax = 0;
    double res;
    res = -INF;
    for (i = 0; i < n; i++)
        if (a[i] > res) res = a[imax = i];
    if (pimax) *pimax = imax;
    return res;
}

double FractionalMaxDblArray(const double* a, int n, double* pimax)
{
    int jmax;
    double amax, sm, s0, sp, Q1, Q2, di;

    amax = MaxDblArray(a, n, &jmax);
    if (n < 3)
    {
        if (pimax) *pimax = (double)jmax;
        return amax;
    }
    jmax = min(jmax, n - 2);
    jmax = max(jmax, 1);

    sm = a[jmax - 1];
    s0 = a[jmax + 0];
    sp = a[jmax + 1];
    Q1 = 0.5 * (sp + sm) - s0;
    Q2 = 0.5 * (sp - sm);
    di = ((Q1 < 0) ? -Q2 / (2.0 * Q1) : 0.0);
    di = max(di, -1.0);
    di = min(di, +1.0);
    amax = amax + 0.5 * di * Q2;
    if (pimax) *pimax = ((double)jmax + di);
    return amax;
}

void DblComplexArrayLinPhaseRotate(ComplexDouble* z, int n,
    double phi1, double phi9)
    /*************************************************************************
     *
     * z_rot = DblComplexArrayLinPhaseRotate(z, n, phi1, phi9)
     *
     * Compute
     *  n is length(z)
     *  phi = linspace(phi1,phi9,length(z));
     *  z_rot = z .* exp(1i*phi);
     ************************************************************************/
{
    double* pz, * po;
    double zr, zi;
    int i;
    double dphi, phi, c, s;

    dphi = (n > 1) ? (phi9 - phi1) / (double)(n - 1) : 0;
    phi = phi1;
    po = (double*)z;
    pz = (double*)z;
    phi = phi1;
    for (i = n; i--; )
    {
        c = cos(phi);
        s = sin(phi);
        zr = *pz++;
        zi = *pz++;
        *po++ = zr * c - zi * s;
        *po++ = zr * s + zi * c;
        phi += dphi;
    }
}

double DblArraySum(const double* a, int n)
{
    double res;
    res = 0;
    while (n--) res += *(a++);
    return res;
}
double DblArrayDot(const double* a, const double* b, int n)
{
    double res;
    res = 0;
    while (n--) res += *(a++) * *(b++);
    return res;
}
double DblArrayFirstMoment(const double* a, int n, double scale)
{
    double res, s;
    res = 0;
    s = -0.5 * (n - 1);
    while (n--) res += (s++) * *(a++);
    return (scale * res);
}
void DblArrayPlus(const double* a, const double* b, double* res, int n)
{
    while (n--) *(res++) = *(a++) + *(b++);
}
void DblArrayMinus(const double* a, const double* b, double* res, int n)
{
    while (n--) *(res++) = *(a++) - *(b++);
}
void DblArrayMult(const double* a, const double* b, double* res, int n)
{
    while (n--) *(res++) = *(a++) * *(b++);
}
void DblArrayDiv(const double* a, const double* b, double* res, int n)
{
    while (n--) *(res++) = *(a++) / *(b++);
}
void DblArraySuperposition(const double* a, const double* b, double* res, int n, double ca, double cb)
{
    while (n--) *(res++) = ca * *(a++) + cb * *(b++);
}
void DblArraySin(const double* a, double* res, int n)
{
    while (n--) *(res++) = sin(*(a++));
}
void DblArrayCos(const double* a, double* res, int n)
{
    while (n--) *(res++) = cos(*(a++));
}
void DblArrayAtan2(const double* a, const double* b, double* res, int n)
{
    while (n--) *(res++) = atan2(*(a++),*(b++));
}
void DblArrayAbs(const double* a, double* res, int n)
{
    while (n--) *(res++) = (double)fabs(*(a++));
}
void DblArrayScale(const double* a, double* res, int n, double s)
{
    while (n--) *(res++) = s * (*(a++));
}
void DblArrayInverse(const double* a, double* res, int n, double s)
{
    while (n--) *(res++) = s / (*(a++));
}
void DblArrayShift(const double* a, double* res, int n, double s)
{
    while (n--) *(res++) = s + (*(a++));
}
void DblArrayReverseShift(const double* a, double* res, int n, double s)
{
    while (n--) *(res++) = s - (*(a++));
}
void DblArrayROUND(const double* a, double* res, int n)
{
    while (n--) *(res++) = (double)ROUND(*(a++));
}
void DblArrayCEIL(const double* a, double* res, int n)
{
    while (n--) *(res++) = (double)CEIL(*(a++));
}
void DblArrayFLOOR(const double* a, double* res, int n)
{
    while (n--) *(res++) = (double)FLOOR(*(a++));
}
void DblArrayCLIP(const double* a, double* res, int n, double lo, double up)
{
    double x;
    while (n--)
    {
        x = *(a++);
        if (x < lo) x = lo;
        else if (x > up) x = up;
        *(res++) = x;
    }
}
void DblArraySign(const double* a, double* res, int n)
{
    double x, y;
    while (n--)
    {
        x = *(a++);
        if (x < 0) y = -1;
        else if (x > 0) y = 1;
        else y = 0;
        *(res++) = y;
    }
}
void DblComplexArrayMult(const ComplexDouble* za, const ComplexDouble* zb,
    ComplexDouble* zab, int n)
{
    double* a, * b, * ab;
    double ar, ai, br, bi;
    int i;

    a = (double*)za;
    b = (double*)zb;
    ab = (double*)zab;
    for (i = n; i--; )
    {
        ar = *a++;
        ai = *a++;
        br = *b++;
        bi = *b++;
        *ab++ = ar * br - ai * bi;
        *ab++ = ar * bi + ai * br;
    }
}
void DblArrayComplex(const double* pr, const double* pi, ComplexDouble* pz, int n)
{
    double* c;
    int i;
    c = (double*)pz;
    for (i = n; i--; )
    {
        *c++ = *pr++;
        *c++ = *pi++;
    }
}
void DblComplexArrayFromModulePhase(const double* A, const double* phase, ComplexDouble* z, int n)
/*************************************************************************
*
* z_rot = DblComplexArrayLinPhaseRotate(z, n, phi1, phi9)
*
* Compute
*  n is length(z)
*  z = A .* exp(1i*phi);
************************************************************************/
{
    double* zz;
    int i;
    zz = (double*)z;
    for (i = n; i--; )
    {
        *zz++ = *A * cos(*phase);
        *zz++ = *A * sin(*phase);
        A++; phase++;
    }
}
void DblArrayFill(double* a, int n, double value)
{
    while (n--) *(a++) = value;
}
void DblArrayLinspace(double* a, int n, double a0, double aend)
{
    int i;
    double dx;
    if (n == 1)
        *a = a0;
    else
    {
        dx = (aend - a0) / (double)(n - 1);
        for (i = 0; i < n; i++) a[i] = a0 + (double)i * dx;
    }
}
void DblArrayCopy(const double* a, double* res, int n)
{
    memmove(res, a, n * sizeof(double));
}
void DblComplexArrayCopy(const ComplexDouble* a, ComplexDouble* res, int n)
{
    DblArrayCopy((double*)a, (double*)res, 2 * n);
}
void DblArrayInplaceFlip(double* A, int n)
{
    double* a1, * a9, t;
    int i;

    a1 = A;
    a9 = a1 + n;
    for (i = n / 2; i--; )
    {
        t = *a1;
        *a1++ = *(--a9);
        *a9 = t;
    }
}
void InplaceDblArrayFlip(NumArrayType* A)
{
    double* a1, * a9, t;
    ComplexDouble* z1, * z9, zt;
    int i, n;

    n = A->n;
    if (A->Type == VSI_REAL)
    {
        a1 = A->data.Pr;
        a9 = a1 + n;
        for (i = n / 2; i--; )
        {
            t = *a1;
            *a1++ = *(--a9);
            *a9 = t;
        }
    }
    else
    {
        z1 = A->data.Pc;
        z9 = z1 + n;
        for (i = n / 2; i--; )
        {
            zt = *z1;
            *z1++ = *(--z9);
            *z9 = zt;
        }
    }
}

void SqrDblModulus(const NumArrayType* Az, double* A2)
/* Compute the square of modulus of a complex array Az */
{
    ComplexDouble* Pc;
    double* Pr;
    int i, nAz;

    nAz = Az->n;
    if (Az->Type == VSI_COMPLEX)
    {
        Pc = Az->data.Pc;
        for (i = nAz; i--;)
        {
            *(A2++) = Pc->real * Pc->real + Pc->imag * Pc->imag;
            Pc++;
        }
    }
    else
    {
        Pr = Az->data.Pr;
        for (i = nAz; i--;)
        {
            *(A2++) = *Pr * *Pr;
            Pr++;
        }
    }
}

void SqrtDblArray(const double* A2, int nA, double* A)
/* Compute the square root of modulus of real array A2,
 * return the resut in A */
{
    int i;
    for (i = nA; i--;) *A++ = sqrt(*A2++);
}

void SqrDblArray(const double* A, int nA, double* A2)
/* Compute the square of A,
 * return the resut in A */
{
    int i;
    for (i = nA; i--;)
    { 
        *A2++ = *A * *A;
        A++;
    }
}

void InplaceDblComplexArrayConj(NumArrayType* A)
{
    double* ai;
    int i, n;
    if (A->Type == VSI_REAL) return;
    ai = A->data.Pr + 1;
    n = A->n;
    for (i = n; i--; )
    {
        *ai = -(*ai);
        ai += 2;
    }
}
void DblArrayCumsum(const double* A, double* C, int n)
{
    double s;
    int i;
    s = 0.0;
    for (i = n; i--;) *C++ = (s += *A++);
}

/*	Non-square matrix transpose of matrix of size r x c (column varies first)
    and base address A; the result is allocated and returned in *p_At size c x r */
ErrorFlagType MatrixTranspose(double* A, int r, int c, double **p_At)
{
    int size, i;
    double *At;

    if (p_At != NULL)
    {
        size = r * c;
        *p_At = At = MALLOC_VSI(sizeof(double) * size);
        if (At == NULL) return VSI_OUTOFMEMORY;

#if defined(_OPENMP)
#pragma omp parallel for default(none) \
    schedule(static, 32) \
    shared(A, At, r, c)
#endif		
		for (i = 0; i < r; i++)
		{
			double* Ai = A + i;
            double* Ati = At + i*c;
			for (int j = c; j--; )
			{
				*Ati++ = *Ai;
				Ai += r;
			}
		}
	}

    return VSI_OK_FLAG;
} /* MatrixTranspose */

#define IDXINF 0x7FFFFFFF
void conv_rectangle(const double* A, int nA, int n, int side, double* B, double* CumsumA) {
    /*************************************************************************
     * B = conv_rectangle(A, n, side)
     * Perform 1D convolution with rectangular pulse.
     * side takes values of -1 or 1
     * A[] is column vector
     * CumsumA is temporary array of length >= length(A) = nA
     *
     * NOTE: If A input is NULL, conv_rectangle assumes CumsumA[] already
     * contains the cumulative sum of (implicit) A and the cumulative
     * calculation is carried out internally.
     * Otherwise CumsumA[] is populated from A[];
     *
     * i.e., B = conv(a,ones(n,1),'same')) for side == -1 (size(B) is [nA,1])
     * or    B = conv(ones(n,1),a,'same')) for side == 1  (size(B) is [n,1])
     ************************************************************************/

    int rsmaller, skrink;
    int nB, nC;
    int i, m, p, f;
    int nh, nt, nb, mxnb;
    int ih0, ib0, it0;
    double* C, * C0, * Cn, s, Clast;

    /* Ugly stuffs, derived from a inspiring night
       Just take it as granted if you can't get into it */

    if (side < 0)
    {
        p = n / 2;
        m = n - p;
        f = nA - m;
        mxnb = nA;
        nB = nA;
    }
    else
    {
        f = nA / 2;
        m = n - f;
        p = m - (nA % 2);
        mxnb = IDXINF;
        nB = n;
    }

    rsmaller = n < nA;
    skrink = p < 0;

    if (rsmaller)
    {
        ih0 = n - m;
        nh = m;
        it0 = nA - n;
        nt = f - it0;
        ib0 = -m * skrink;
        nb = it0 + (m + p) * skrink;
    }
    else
    {
        ih0 = n - m;
        nh = nA - ih0;
        nb = n - nA;
        if (nb > mxnb) nb = mxnb;
        it0 = 0;
        nt = f;
    }

    C0 = C = CumsumA;
    /* If A not provided, CumsumA contains cumulative sum of A */
    if (A != NULL)
    {
        /* Cumulative sum */
        nC = nA;
        if (skrink)
        {
            /* skrink the integration to the strict minimum  */
            A -= m;
            C -= m;
            nC += (m + p);
        }
        s = 0;
        for (i = nC; i--;)
        {
            s += *A++;
            *C++ = s;
        }
        Clast = *(C - 1);
    }
    else Clast = *(C0 + nA - 1);

    C = C0 + ih0;
    if (nh > 0) { memcpy(B, C, nh * sizeof(double)); B += nh; }

    if (rsmaller)
    {
        C = C0 + ib0;
        Cn = C + n;
        if (nb > 0) for (i = nb; i--;) *(B++) = *(Cn++) - *(C++);
    }
    else
    {
        if (nb > 0) for (i = nb; i--;) *(B++) = Clast;
    }

    C = C0 + it0;
    if (nt > 0) for (i = nt; i--;) *(B++) = Clast - *(C++);

} /* conv_rectangle */

#define CVDIM 2
void conv2_rectangle(const double* A_IN, int mA, int nA,
	                 int nKernel, int side, double* B_OUT, double* TMP) {
	/*************************************************************************
	* MATLAB MEX ROUTINE conv2_rectangle_mex.c
	*
	* B = conv2_rectangle(A_IN, nKernel, side)
	* Perform 2D convolution with rectangular pulse
	* side is -1 or 1
	* A is a double 2D array of size mA x nA
	* i.e.,    B = conv(A,ones(nKernel),'same')) for side == -1
	* or       B = conv(ones(nKernel),A,'same')) for side == 1
    * 
    * TMP is working buffer of length max(mA,nA)
	*
	* Author: Bruno Luong <b.luong@fogale.com>
	* History
	*  OriginAl: 22/June/2021
	************************************************************************/

	int rsmaller[CVDIM], pneg[CVDIM];
	int szA[CVDIM], szB[CVDIM], nC;
	int k, j, i, ni, nj;
	int numelA, sA, sB, tA, tB, NI;
	int n[CVDIM], m[CVDIM], p[CVDIM], f[CVDIM];
	int nh[CVDIM], nt[CVDIM], nb[CVDIM], mxnb[CVDIM];
	int ih0[CVDIM], ib0[CVDIM], it0[CVDIM];
	int ifh[CVDIM], ifb[CVDIM], ift[CVDIM], ilast[CVDIM], ilastmax[CVDIM];
	double *B, *C, *B0, *C0, *Cn, s, Clast;
    const double* A,  *A0;
	int iC0, iC9;

    szA[0] = mA;
    szA[1] = nA;

    n[1] = n[0] = nKernel;;

	/* Preparation */
	for (k = 0; k < CVDIM; k++)
	{
		ifh[k] = 0;

		ilastmax[k] = (side < 0) ? szA[k] - 1 : n[k] - 1;
		ilast[k] = ilastmax[k];

		if (side < 0)
		{
			p[k] = n[k] / 2;
			m[k] = n[k] - p[k];
			f[k] = szA[k] - m[k];
			mxnb[k] = szA[k];
		}
		else
		{
			f[k] = szA[k] / 2;
			m[k] = n[k] - f[k];
			p[k] = m[k] - (szA[k] % 2);
			mxnb[k] = IDXINF;
		}

		rsmaller[k] = n[k] < szA[k];
		if (rsmaller[k])
		{
			nh[k] = m[k];
			ih0[k] = n[k] - nh[k];

			it0[k] = szA[k] - n[k];
			nt[k] = f[k] - it0[k];

			pneg[k] = p[k] < 0;
			ib0[k] = -m[k] * pneg[k];
			nb[k] = it0[k] + (m[k] + p[k])*pneg[k];
		}
		else
		{
			ih0[k] = n[k] - m[k];
			nh[k] = szA[k] - ih0[k];

			nb[k] = n[k] - szA[k];
			if (nb[k] > mxnb[k]) nb[k] = mxnb[k];

			it0[k] = 0;
			nt[k] = f[k];
		}

		if (nh[k] < 0) nh[k] = 0;
		ifb[k] = ifh[k] - nh[k];
		if (ifb[k] < 0) ifb[k] = 0;
		if (nb[k] < 0) nb[k] = 0;
		ift[k] = ifb[k] - nb[k];
		if (ift[k] < 0) ift[k] = 0;

		nh[k] -= ifh[k];
		nb[k] -= ifb[k];
		nt[k] -= ift[k];

		if (ifh[k] > 0) ih0[k] += ifh[k];
		if (ifb[k] > 0) ib0[k] += ifb[k];
		if (ift[k] > 0) it0[k] += ift[k];
	}

	/* Compute size of the output B */
	numelA = 1;
	for (k = 0; k < CVDIM; k++)
	{
		szB[k] = ilast[k] - ifh[k] + 1;
		numelA *= szA[k];
	}

	/* Calculation */
	A0 = A_IN;
	B0 = B_OUT;
	for (k = 0; k < CVDIM; k++)
	{
		nj = numelA / szA[k];
		C0 = TMP;
		switch (k)
		{
		case 0:
			sA = sB = 1;
			tA = szA[0];
			tB = szB[0];
			break;
		case 1:
			A0 = B0;
			sA = sB = szB[0];
			tA = tB = 1;
			break;
		}
		/* skrink the calculation to the strict minimum  */
		if (side > 0)
		{
			iC0 = ifh[k] + f[k] - n[k];
			iC9 = f[k] + 1 + ilast[k];
		}
		else
		{
			iC0 = ifh[k] - (n[k] + 1) / 2;
			iC9 = ilast[k] + 1 + (n[k] / 2);
		}
		if (iC0 < 0) iC0 = 0;
		if (iC9 > szA[k]) iC9 = szA[k];
		nC = iC9 - iC0;

		for (j = 0; j < nj; j++)
		{
			A = A0 + j * tA + iC0 * sA;
			B = B0 + j * tB;
			C = C0 + iC0;

			/* Cumulative sum */
			s = 0.0;
			for (i = nC; i--;)
			{
				s += *A;
				*C++ = s;
				A += sA;
			}

			Clast = *(C - 1);

			NI = szB[k];

			/* Head */
			ni = nh[k];
			if (ni > NI) ni = NI;
			if (ni > 0)
			{
				C = C0 + ih0[k];
				for (i = ni; i--;)
				{
					*B = *(C++);
					B += sB;
				}
				NI -= ni;
			}

			/* Body */
			ni = nb[k];
			if (ni > NI) ni = NI;
			if (ni > 0)
			{
				if (rsmaller[k])
				{
					C = C0 + ib0[k];
					Cn = C + n[k];
					for (i = ni; i--;)
					{
						*B = *(Cn++) - *(C++);
						B += sB;
					}
				}
				else
				{
					for (i = ni; i--;)
					{
						*B = Clast;
						B += sB;
					}
				}
				NI -= ni;
			}

			/* Tail */
			ni = nt[k];
			if (ni > NI) ni = NI;
			if (ni > 0)
			{
				C = C0 + it0[k];
				for (i = ni; i--;)
				{
					*B = Clast - *(C++);
					B += sB;
				}
				NI -= ni;
			}
		}
	}
}

ErrorFlagType Dblconv1(const double* a, int na, const double* b, int nb,
    double* c, int* pint_nc, int shape)
    /* Naive implementation by sliding sum,
       efficient when A or B is small (~10 elemenst) */
{
    int nc, i, j, j1, j2, jmax, nanb;
    double s;
    const double* pa, * pb;

    switch (shape)
    {
    case VALID_SHAPE:
        j1 = nb - 1;
        j2 = na;
        break;
    case FULL_SHAPE:
        j1 = 0;
        j2 = na + nb - 1;
        break;
    case SAME_SHAPE:
        j1 = nb / 2;
        j2 = j1 + na;
        break;
    }

    if (pint_nc)
    {
        /* Compute length of the output of requested */
        nc = j2 - j1;
        if (nc < 0) nc = 0;
        *pint_nc = nc;
    }

    na--; nb--;
    jmax = min(na, nb);
    for (j = j1; j < jmax; j++)
    {
        i = j + 1;
        s = 0;
        pa = a;
        pb = b + j;
        while (i--) s += *(pa++) * *(pb--);
        *(c++) = s;
    }
    if (nb <= na)
        for (; j < nb; j++)
        {
            i = j + 1;
            s = 0;
            pa = a;
            pb = b + j;
            while (i--) s += *(pa++) * *(pb--);
            *(c++) = s;
        }
    else
    {
        jmax = min(nb, j2);
        na++;
        for (; j < jmax; j++)
        {
            i = na;
            s = 0;
            pa = a;
            pb = b + j;
            while (i--) s += *(pa++) * *(pb--);
            *(c++) = s;
        }
        na--;
    }
    b += nb;
    a -= nb;
    nb++;
    for (; j < na; j++)
    {
        i = nb;
        s = 0;
        pa = a + j;
        pb = b;
        while (i--) s += *(pa++) * *(pb--);
        *(c++) = s;
    }
    nanb = na + nb;
    jmax = min(nanb, j2);
    for (; j < jmax; j++)
    {
        i = nanb - j;
        s = 0;
        pa = a + j;
        pb = b;
        while (i--) s += *(pa++) * *(pb--);
        *(c++) = s;
    }
    for (; j < j2; j++) *(c++) = 0;

    return VSI_OK_FLAG;
} /* Dblconv1 */

/*************************************************************************
 * lemire_nd_maxengine(A, dimA, window, shapeflag, MAXVAL, dimOut)
 *
 * PURPOSE: multiple 1D max running/filtering
 * Similar to LEMIRE_ENGINE but working on the second dimension, while
 * looping along the first and third dimensions. This MEX is used for
 * the engine for multidimensional min/max filtering
 *
 * INPUTS
 *  A: 3D double array
 *  dimA: dimension of A
 *  window: scalar, size of the sliding window, must be >= 1
 *  shapeflag: double scalar: 0, 1, 2 resp. for full, same and valid shape
 *
 * OUTPUTS
 *  *MAXVAL: 3D double array, memory allocated by lemire_nd_maxengine
 *  dimOut: dimension of MAXVAL
 *  For "valid" shape
 *  maxval: running max, vectors of dimension (length(A)-window+1), i.e.,
 *      maxval(:,1,:) is max(A(:,1:win,:))
 *      maxval(:,2,:) is max(A(:,2:win+1,:))
 *      ...
 *      maxval(:,end,:) is max(A(:,end-window+1:end,:))
 *  For "Full" shape (with shapeflag)
 *  For "Same" shape output has the same dimension as A
 *  output MAXVAL has dimension (length(A)+window-1), correspond to all
 *  positions of sliding window that is intersect with A
 *
 * Algorithm: Lemire's "STREAMING MAXIMUM-MINIMUM FILTER USING NO MORE THAN
 * THREE COMPARISONS PER ELEMENT" Nordic Journal of Computing, Volume 13,
 * Number 4, pages 328-339, 2006.
 *
 ************************************************************************/
ErrorFlagType lemire_nd_maxengine(double *A, const int dimA[3], int WINDOW, int SHAPE,
    double ** MAXVAL, int dimOut[3])
{
    /* Data pointers, which one are used depends on the class of A */
    //double* adouble, * valdouble;

    int n, window;
    int imax, margin;
    int  lstart, size;
    int*  WedgeBuffer; /* wedge */
   // int nWedge; /* wedge number of elements (0 is empty wedge) */
    //int Wedgefirst, Wedgelast; /* Indices of two ends of the wedge */
    int shape;

    int p, q, pq, jk;
    int stepA, stepMinMax;

    void* adata, * valdata;

	shape = SHAPE;

	p = dimA[0];
	n = dimA[1];
	q = dimA[2];

    /* Get the window size, cast it in mwSize */
    window = WINDOW;
    margin = window - 1;

    /* This parameters configure for three cases:
     * - full scan (minimum 1-element intersecting with window), or
     * - same scan (output has the same size as input)
     * - valid scan (full overlapping with window) */
    if (shape == FULL_SHAPE) {
        dimOut[1] = n + margin;
        lstart = -(int)(margin);
    }
    else if (shape == SAME_SHAPE) {
        dimOut[1] = n;
        lstart = -(int)(margin / 2);
    }
    else { /* if (shape==VALID_SHAPE) */
        dimOut[1] = n - margin;
        lstart = 0;
    }

    /* The last index to be scanned */
    imax = (dimOut[1] + margin) + lstart;

    /* Populate the dimensions of the output arrays */
    dimOut[0] = p;
    dimOut[2] = q;

    /* Allocate the output, the caller should takecare of free it */
    valdata = *MAXVAL = MALLOC_VSI(dimOut[0] * dimOut[1] * dimOut[2] * sizeof(double));
    if (valdata == NULL) return VSI_OUTOFMEMORY;

	/* Jump step of the third dimension */
	stepA = p * n; /* for A */
	stepMinMax = p * dimOut[1]; /* step for output */

    /* Pointer to the input array */
	adata = A;

    /* Allocate wedges buffers for L and U, each is size (window+1) */
    size = (int)(window + 1);
    WedgeBuffer = MALLOC_VSI(size * p * q * sizeof(int));
    if (WedgeBuffer == NULL)
    {
        FREE_VSI(*MAXVAL);
        *MAXVAL = NULL;
        return VSI_OUTOFMEMORY;
    }

    /* number of max run-lengths */
    pq = p * q;

/* This is the lemire engine
   the outer parallel loop works on combined 1st and 3rd dimensions */
/* Note: j -> third dimension
 *       k -> first dimension
 *       i -> second (working) dimension */
//#if defined(_OPENMP)
//#pragma(omp parallel for default(none) private(i, j, k, jk) \
//	private(adouble, valdouble, Wedge) \
//	private(nWedge, Wedgefirst, Wedgelast, left, pleft) \
//	private(linidx) \
//	schedule(static) \
//	shared(adata, valdata, stepA, stepMinMax, lstart, \
//		p, pq, n, size, imax, WedgeBuffer, window))
//#endif

#if defined(_OPENMP)
#pragma omp parallel for schedule(dynamic, 16) firstprivate(adata, valdata, WedgeBuffer) shared(stepA, stepMinMax, lstart, p, pq, n, size, imax, window)
#endif
	for (jk = 0; jk < pq; jk++) {
		int j = jk / p;
		int k = jk % p;
		double* adouble = (double*) adata + j * stepA + k;
		double* valdouble = (double*) valdata + j * stepMinMax + k;
		int* Wedge = WedgeBuffer + (j * p + k) * size;
		int nWedge = 0;
		int Wedgefirst = 0;
		int Wedgelast = -1;
		int left = -(int)(window);
		int pleft = 0;
		for (int i = 1; i < n; i++) {
			left++;
			if (left >= lstart) {
				int linidx = p * (nWedge ? Wedge[Wedgefirst] : i - 1);
				valdouble[pleft] = adouble[linidx];
				pleft += p;
			}
			if (adouble[p * i] > adouble[p * (i - 1)]) {
				while (nWedge) {
					if (adouble[p * i] <= adouble[p * Wedge[Wedgelast]]) {
						if (left == Wedge[Wedgefirst]) {
							nWedge--;
							if ((++Wedgefirst) == size) Wedgefirst = 0;
						}
						break;
					}
					nWedge--;
					if ((--Wedgelast) < 0) Wedgelast += size;
				}
			}
			else {
				nWedge++;
				if ((++Wedgelast) == size) Wedgelast = 0;
				Wedge[Wedgelast] = i - 1;
				if (left == Wedge[Wedgefirst]) {
					nWedge--;
					if ((++Wedgefirst) == size) Wedgefirst = 0;
				}
			}
		}
		for (int i = n; i <= imax; i++) {
			left++;
			int linidx = p * (nWedge ? Wedge[Wedgefirst] : n - 1);
			valdouble[pleft] = adouble[linidx];
			pleft += p;
			nWedge++;
			if ((++Wedgelast) == size) Wedgelast = 0;
			Wedge[Wedgelast] = n - 1;
			if (left == Wedge[Wedgefirst]) {
				nWedge--;
				if ((++Wedgefirst) == size) Wedgefirst = 0;
			}
		}
	}

    FREE_VSI(WedgeBuffer);

    return VSI_OK_FLAG;

} /* lemire_nd_maxengine */

/*
PURPOSE : two dimensional max(dilatation) filtering

INPUTS :
     A : ND arrays, logical and all numeric classes are supported
     WINDOW : size of the sliding window.The value must be >= 1
     SHAPE : 'full' 'same' 'valid
        'full' - returns the full size arrays,
     'same' - returns the central part of the filtering output
        that is the same size as A.
     'valid' - returns only those parts of the filter that are computed
        without the padding edges.
OUTPUTS:
    *MAXVAL: 2D double array, memory allocated by maxfilter
    dimOut: dimension of MAXVAL
 -'full', the size is size(A) + (WINDOW - 1)
 -'same', the size is size(A)
 -'valid', the size is size(A) - WINDOW + 1
*/
#define IMAGE_DIM 2
ErrorFlagType maxfilter(double* A, const int dimA[2], int WINDOW, int SHAPE,
    double **MAXVAL, int dimOut[2])
{
    int dim;
    int dimtmp[3];
    double *Ain, *Aout;
    ErrorFlagType Flag;

    *MAXVAL = NULL; /* default value if error occurs */
    dimtmp[0] = dimtmp[1] = 0;

    /* loop max run-length successively on both dimensions */
    Ain = A;
    for (dim = 1; dim <= IMAGE_DIM; dim++)
    {
        if (dim == 1)
        {
            dimtmp[0] = 1;
            dimtmp[1] = dimA[0];
            dimtmp[2] = dimA[1];
        }
        else // (dim == 2) since IMAGE_DIM is 2
        {
            dimtmp[0] = dimA[0];
            dimtmp[1] = dimA[1];
            dimtmp[2] = 1;
        }
        
        Flag = lemire_nd_maxengine(Ain, dimtmp, WINDOW, SHAPE, &Aout, dimtmp);

        /* recursive free and updating */
        if (Ain != A) FREE_VSI(Ain);
        Ain = Aout;

        if (Flag < 0) return Flag;

    }

    /* Assign outputs */
    dimOut[0] = dimtmp[0];
    dimOut[1] = dimtmp[1];
    *MAXVAL = Aout;

    return VSI_OK_FLAG;

} /* maxfilter */

/* interface for sorting array of double numbers */ 
int _cmp_Double(const void* pa, const void* pb)
{
    double a, b;
    a = *((double*)(pa));
    b = *((double*)(pb));
    if (a < b)
        return -1;
    else
        return +1;
} /* _cmp_DL_TravelingDist */

int _cmp_Byte(const void* pa, const void* pb)
{
    UInt8 a, b;
    a = *((UInt8*)(pa));
    b = *((UInt8*)(pb));
    if (a < b)
        return -1;
    else
        return +1;
}

void Copy_DoubleArray(double* dest, const double* source, UInt32 n)
{
    memcpy(dest, source, n * sizeof(double));
}

/* Comput the median */
/* TBD check what happens with NaN */
ErrorFlagType MedianDblArray(const double* a, int n, double* Median)
{
    double *Tmp;
    /* Allocate temporary array where data are sorted */
    Tmp = MALLOC_VSI(sizeof(double) * n);
    if (Tmp == NULL) return VSI_OUTOFMEMORY;
    Copy_DoubleArray(Tmp, a, n);
    qsort(Tmp, n, sizeof(double), _cmp_Double);
    *Median = Tmp[n / 2];
    FREE_VSI(Tmp);
    return VSI_OK_FLAG;
}

ErrorFlagType MedianByteArray(const UInt8* a, int n, double* Median)
{
    UInt8* Tmp;
    /* Allocate temporary array where data are sorted */
    Tmp = MALLOC_VSI(sizeof(UInt8) * n);
    if (Tmp == NULL) return VSI_OUTOFMEMORY;
    memcpy(Tmp, a, n * sizeof(UInt8));
    qsort(Tmp, n, sizeof(UInt8), _cmp_Byte);
    *Median = (double) Tmp[n / 2];
    FREE_VSI(Tmp);
    return VSI_OK_FLAG;
}

static int pow2_atleast(int x)
{
    int h;
    for (h = 1; h < x; h = 2 * h)
        ;
    return h;
}

/***********************************************************************************
 FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT
 FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT
 FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT - FFT
/***********************************************************************************/

static char* WisdomStr = NULL;
#define PLAN_LENGTH 31
static fftw_plan        fft_PlanRTab[PLAN_LENGTH] = { NULL };
static fftw_plan        fft_PlanCTab[PLAN_LENGTH] = { NULL };
static fftw_complex*    fft_InTab[PLAN_LENGTH] = { NULL };
static fftw_complex*    fft_OutTab[PLAN_LENGTH] = { NULL };

static fftw_plan		ifft_PlanTab[PLAN_LENGTH] = { NULL };
static fftw_complex*    ifft_InTab[PLAN_LENGTH] = { NULL };
static fftw_complex*    ifft_OutTab[PLAN_LENGTH] = { NULL };

__inline int Log2N(UInt32 v)
/* floor of log2 of (int)(v)
  See http://graphics.stanford.edu/~seander/bithacks.html#IntegerLogDeBruijn
*/
{
    int r; // result of log_2(v) goes here
    union { UInt32 u[2]; double d; } t; // temp

    t.u[__FLOAT_WORD_ORDER == ENDIAN_LITTLE] = 0x43300000;
    t.u[__FLOAT_WORD_ORDER != ENDIAN_LITTLE] = v;
    t.d -= 4503599627370496.0;
    r = (t.u[__FLOAT_WORD_ORDER == ENDIAN_LITTLE] >> 20) - 0x3FF;
    return r;
}

__inline void ResetNumArrayType(NumArrayType* A)
{
    if (A != NULL)
        memset(A, 0, sizeof(NumArrayType));
}

void FreeNumArrayType(NumArrayType* A)
{
    void* P;
    P = (void*)(A->data.Pc);
    if (P != NULL) FREE_VSI(P);
    ResetNumArrayType(A);
}

/* if data argument is NULL, CreateNumArrayType allocate memory
   otherwise it just uses preallocated data pointer as data buffer for A */
ErrorFlagType CreateNumArrayType(ArrayType Type, int n, const void* data, NumArrayType* A)
{
    A->Type = Type;
    A->n = n;
    if (data != NULL)
        A->data.Pr = (double*)data;
    else
    {
        if (Type == VSI_COMPLEX)
            A->data.Pc = (ComplexDouble*)MALLOC_VSI(n * sizeof(ComplexDouble));
        else
            A->data.Pr = (double*)MALLOC_VSI(n * sizeof(double));
        if (A->data.Pr == NULL) return VSI_OUTOFMEMORY;
    }
    return VSI_OK_FLAG;
}

ErrorFlagType CleanFFT()
{
    int i;
    fftw_plan p;
    fftw_complex* ptr;
    for (i = 0; i < PLAN_LENGTH; i++)
    {
        p = fft_PlanRTab[i];
        if (p != NULL) fftw_destroy_plan(p);
        fft_PlanRTab[i] = NULL;
        p = fft_PlanCTab[i];
        if (p != NULL) fftw_destroy_plan(p);
        fft_PlanCTab[i] = NULL;
        ptr = fft_InTab[i];
        if (p != NULL) fftw_free(ptr);
        fft_InTab[i] = NULL;
        ptr = fft_OutTab[i];
        if (p != NULL) fftw_free(ptr);
        fft_OutTab[i] = NULL;

        p = ifft_PlanTab[i];
        if (p != NULL) fftw_destroy_plan(p);
        ifft_PlanTab[i] = NULL;
        ptr = ifft_InTab[i];
        if (p != NULL) fftw_free(ptr);
        ifft_InTab[i] = NULL;
        ptr = ifft_OutTab[i];
        if (p != NULL) fftw_free(ptr);
        ifft_OutTab[i] = NULL;
    }
    return VSI_OK_FLAG;
}

ErrorFlagType VSI_fft(const NumArrayType* A_IN, NumArrayType* B_OUT)
{
    /* Perform FFT of array A_IN[N] of size N
    If A is complex, then (underling A_IN is *ComplexDouble), set ComplexFlag to 1
    The result is return in B_OUT[] of size N */

    fftw_complex* in, * out;
    fftw_plan p;
    unsigned int nbytes;
    unsigned int i, r;
    int isAReal, StoreWisdom, ispower2, NoMemCopy;
    int N;
    double* dblin;
    double* outr1, * outi1, * outrN, * outiN;

    //fftw_init_threads();
    StoreWisdom = 0;

    N = A_IN->n;
    isAReal = A_IN->Type == VSI_REAL;

    nbytes = sizeof(fftw_complex) * N;

    in = (fftw_complex*)(A_IN->data.Pc);
    dblin = A_IN->data.Pr;
    out = (fftw_complex*)(B_OUT->data.Pc);

    if (StoreWisdom)
    {
        if (isAReal)
            p = fftw_plan_dft_r2c_1d(N, dblin, out, FFTW_PATIENT);
        else
            p = fftw_plan_dft_1d(N, in, out, FFTW_FORWARD, FFTW_PATIENT);
        WisdomStr = fftw_export_wisdom_to_string();
        ispower2 = 0;
        NoMemCopy = 0;
    }
    else
    {
        r = Log2N(N);
        ispower2 = (r < PLAN_LENGTH) && ((0x1 << r) == N);
        if (ispower2)
        {
            p = isAReal ? fft_PlanRTab[r] : fft_PlanCTab[r];
            if (p == NULL)
            {
                if (fft_InTab[r] == NULL)     fft_InTab[r] = (fftw_complex*)fftw_malloc(nbytes);
                if (fft_OutTab[r] == NULL)    fft_OutTab[r] = (fftw_complex*)fftw_malloc(nbytes);
                if (isAReal)
                {
                    p = fftw_plan_dft_r2c_1d(N, (double*)fft_InTab[r], fft_OutTab[r],
                        FFTW_ESTIMATE | FFTW_DESTROY_INPUT);
                    fft_PlanRTab[r] = p;
                }
                else
                {
                    p = fftw_plan_dft_1d(N, fft_InTab[r], fft_OutTab[r],
                        FFTW_FORWARD, FFTW_ESTIMATE | FFTW_DESTROY_INPUT);
                    fft_PlanCTab[r] = p;
                }
            }
            if (isAReal)
            {
                nbytes = nbytes / 2;
                NoMemCopy = (fftw_alignment_of((double*)fft_InTab[r]) == fftw_alignment_of((double*)dblin)) &&
                    (fftw_alignment_of((double*)fft_OutTab[r]) == fftw_alignment_of((double*)out));

                if (!NoMemCopy)
                {
                    memcpy(fft_InTab[r], dblin, nbytes);
                    nbytes = nbytes + sizeof(fftw_complex); // Adjust +1 complex to prepare OUT copying size
                }
            }
            else
            {
                NoMemCopy = (fftw_alignment_of((double*)fft_InTab[r]) == fftw_alignment_of((double*)in)) &&
                    (fftw_alignment_of((double*)fft_OutTab[r]) == fftw_alignment_of((double*)out));
                if (!NoMemCopy) memcpy(fft_InTab[r], in, nbytes);
            }

        }
        else
        {
            NoMemCopy = 0;
            if (isAReal)
                p = fftw_plan_dft_r2c_1d(N, dblin, out, FFTW_ESTIMATE);
            else
                p = fftw_plan_dft_1d(N, in, out, FFTW_FORWARD, FFTW_ESTIMATE);
        }
    }

#if (CHECK_ARG == 1)
    if (p != NULL)
        if (NoMemCopy)
            if (isAReal)
                fftw_execute_dft_r2c(p, dblin, out);
            else
                fftw_execute_dft(p, in, out);
        else
            fftw_execute(p);
    else
        return VSI_FFT_FAILS;
#else
    if (NoMemCopy)
        if (isAReal)
            fftw_execute_dft_r2c(p, dblin, out);
        else
            fftw_execute_dft(p, in, out);
    else
        fftw_execute(p);
#endif

    if (ispower2)
    {
        if (!NoMemCopy) memcpy(out, fft_OutTab[r], nbytes);
    }
    else
    {
        fftw_destroy_plan(p);
    }

    if (isAReal)
    {
        /* Real input, -> create conjugate output */
        outr1 = (double*)(out + 1);
        outi1 = outr1 + 1;
        outrN = (double*)(out + N - 1);
        outiN = outrN + 1;
        for (i = (N - 1) / 2; i--;)
        {
            *outrN = (*outr1); outrN -= 2; outr1 += 2;
            *outiN = -(*outi1); outiN -= 2; outi1 += 2;
        }
    }

    return VSI_OK_FLAG;

} /* VSI_fft */


/* Create 3D double array from an array of UINt3 Images */
ErrorFlagType VSI_GetStackImages(const VSI_StackImages_Type* Images,
	ImageStack_Type* ImTable)
{
	UInt32 i, nxy;
	Float64* DestData;
	UInt8* SrcData;
    Int32 k, NbImages;

    /* Default value in case of error */
    ImTable->Data = NULL;
    ImTable->nx = 0;
    ImTable->ny = 0;
    ImTable->nz = 0;

    NbImages = Images->NumberOfImages;

	/* Need at meast 5 images to work */
	if (NbImages < 5)
		return VSI_NOT_ENOUGHIMAGES;

	/* Get the image number of pixels */
    ImTable->nx = Images->Width;
    ImTable->ny = Images->Height;
    ImTable->nz = NbImages;

	nxy = ImTable->nx * ImTable->ny;

	/* VSI_StackImages is in charge to free it */
	DestData = MALLOC_VSI(sizeof(Float64) * nxy * NbImages);
	if (DestData == NULL)
		return VSI_OUTOFMEMORY;
	ImTable->Data = DestData;

	/* Fill the 3D data while casting the UInt8 data to double */
	for (k = 0; k < NbImages; k++)
	{
		SrcData = Images->ImageArray[k];
		for (i = 0; i < nxy; i++)
			*DestData++ = (Float64)(*SrcData++);
	}
	return VSI_OK_FLAG;
}

/* Free Data of ImageStack_Type abd reste the structure */
ErrorFlagType FreeImageStack(ImageStack_Type* ImTable)
{
	if (ImTable->Data != NULL) FREE_VSI(ImTable->Data);
	memset(ImTable, 0, sizeof(ImageStack_Type));

	return VSI_OK_FLAG;
}

/* Allocate and populate window array with Gaussian envelop function */
ErrorFlagType eGaussianwin(int N, double Ewidth, NumArrayType *w)
{
	ErrorFlagType Flag;
	int i;
	double t, x;

	Flag = CreateNumArrayType(VSI_REAL, N, NULL, w);
	if (Flag < 0) return Flag;

	for (i = 0; i < N; i++)
	{
		t = (double)(2*i) / (double)(N - 1) - 1.0;
        x = t / Ewidth;
		w->data.Pr[i] = exp(-(x*x));
	}
	return VSI_OK_FLAG;
}

/* Allocate and populate window array with Tukey envelop function */
ErrorFlagType eTukeywin(int N, double ratio, NumArrayType* w)
{
    ErrorFlagType Flag;
    int i, tl, th;
    double t, per;

    Flag = CreateNumArrayType(VSI_REAL, N, NULL, w);
    if (Flag < 0) return Flag;

    per = ratio / 2.0;
    tl = (int)(FLOOR(per * (N - 1)));
    th = N - tl;
    for (i = 0; i < N; i++)
    {
        t = (double)(i) / (double)(N - 1);
        if (i <= tl)
            w->data.Pr[i] = (1 + cos(PI / per * (t - per))) / 2;
        else if (i >= th)
            w->data.Pr[i] = (1 + cos(PI / per * (t - 1 + per))) / 2;
        else
            w->data.Pr[i] = 1.0;
    }
    return VSI_OK_FLAG;
}

double SkewnessAdjust(double j, NumArrayType *P, double width)
{
    double jsk, hw, sumhw, b, sn, sd, Pwi;
    int i, i1, i9, e, n;

    hw = min(width / 2, j);
    hw = min(hw, P->n -1 - j);

    i1 = (int)ROUND(j - hw);
    i9 = (int)ROUND(j + hw);
    n = i9 - i1 + 1;
    if (n >= 2)
    {
        sumhw = (double)(i1 + i9) * (double)n / 2.0;
        b = (double)n * (double)j - sumhw;
        e = b > 0 ? i9 : i1;
        sn = sd = 0;
        for (i = i1; i <= i9; i++)
        {
            Pwi = P->data.Pr[i];
            if (i == e) Pwi *= (1.0 + b / (i - j));
            sn += Pwi * i;
            sd += Pwi;
        }
        jsk = sn / sd;
    }
    else
        jsk = j;

    return jsk;
}

/*
  Detect the oscillation sampling frequency of the fringe signal.
 
  INPUTS:
    fringe : vector of fringe signale acquired with constant sampling step
    Setting pointer to structure with the fields
        RuleStep : double scalar, step between two OPDs, [m]
        LambdaCenter : double scalar, wavelength of the light source, [m]
        FWHMLambda : double scalar, spectral bandwidth of the light source, [m]
            If RuleStep, LambdaCenter, FWHMLambda are provided
            the expected coherence length in number of samples are estimated
            and used to truncated the signal.Only applicable for FFT method
    options : structure contains fields
        FreqMethod : FrequencyMethod enumerate fft or x0
            Method used to detect spatial frequency nu, default fft
        WindowType : char array among{ 'gaussian', 'tukey' }
            Only applicable for 'fft' method.WindowType used for windowing
            the signal before FFT.Default 'tukey'
        Ewidth : double scalar, sample unit
            Only applicable for 'fft' method and WindowType 'gaussian'
            The width of the Gaussian window function.
            Ewidth : = sqrt(2) * sigma
        nwin : integer scalar, number of samples used to smooth interferogram
            signal for various tasks.Make sure nwin < 0.5 * fringe period
            Only applicable for x0 FreqMethod
        SpectrumSkewness : boolean scalar, Only applicable for FFT method.
            If TRUE adjust the light central wavelength according to the
            skewness of the spectrum, otherwise use the MODE of the spectrum
            as nominal spatial frequency.Default[TRUE].
        GraphicalMask : 0x40 plot the(apparent) light spectrum.
  OUTPUTS :
        nu : double scalar, spatial frequency, unit[1 / sample]
        period : spatial period, unit[sample]. It is also 1 / nu.
 
 NOTE : FFT is recommended for short fringe such as Profilometer
  X0 for long fringe signal such as Lenscan
 
  See also : FringeDemodulation, PeriodEstimate, FrequencyMethod
    */
ErrorFlagType DetectFrequency(const NumArrayType* fringe,
    const VSI_StackImages_Setting_Type* Setting,
    const VSI_StackImages_opt_Type* options,
    double* period)
{
    NumArrayType E, Iwin, Ihat;
    double LambdaCenter, FWHMLambda, RuleStep, expectedratio;
    double baseline, L, L_instep, * I, dummy;
    double nu, dnu, IhatMax, jmax, skewwsize, nubin;
    ErrorFlagType Flag;
    int n, locmax, HW, i1, i9, N, NPad, nnu;
    VSI_BOOL HardTruncated;
    double Ewidth, cosinfraction;

    Flag = MedianDblArray(fringe->data.Pr, fringe->n, &baseline);
    if (Flag < 0) return Flag;

    I = fringe->data.Pr;
    n = fringe->n;

#if DEBUG_ON_FILE == 1
    DEBUG_STORE_NUM(fringe);
    Flag = DEBUG_SAVE2FILE("fringe_s.txt");
    if (Flag < 0) return Flag;
#endif 
    
    DblArrayShift(I, I, n, -baseline);



    switch (options->FrequencyMethod)
    {
    case VSI_FREQDETECT_FFT:

        LambdaCenter = Setting->LambdaCenter;
        FWHMLambda = Setting->FWHMLambda;
        RuleStep = fabs(Setting->RuleStep);

        L = 0.5 * sqrt(log(2) / (PI)) * LambdaCenter * LambdaCenter / FWHMLambda;
        L_instep = L / RuleStep;

        HardTruncated = !isinf(L_instep);
        dummy = MaxDblArray(I, n, &locmax);

        if (HardTruncated)
        {
            HW = 3 * (int)ROUND(L_instep);
            i1 = max(0, locmax - HW + 1);
            i9 = min(n - 1, locmax + HW - 1);
            N = i9 - i1 + 1;
            I += i1;
        }
        else
        {
            N = n;
        }

        switch (options->WindowType)
        {
        case VSI_WINDOWTYPE_GAUSSIAN:
            Ewidth = options->Ewidth;
            Flag = eGaussianwin(N, Ewidth, &E);
            if (Flag < 0) return Flag;
#if DEBUG_ON_FILE == 1
            DEBUG_STORE_NUM(&E);
            Flag = DEBUG_SAVE2FILE("E.txt");
            if (Flag < 0) return Flag;
#endif
            break;

        case VSI_WINDOWTYPE_TUKEY:
            cosinfraction = HardTruncated ? 0.25 : 1.0;
            Flag = eTukeywin(N, cosinfraction, &E);
            if (Flag < 0) return Flag;
#if DEBUG_ON_FILE == 1
            DEBUG_STORE_NUM(&E);
            Flag = DEBUG_SAVE2FILE("E.txt");
            if (Flag < 0) return Flag;
#endif
            break;

        default:
            return VSI_NOTYET_IMPLEMENTED;
        }

        break;

    default:
        return VSI_NOTYET_IMPLEMENTED;
    }

    /* E is no longer used */
    Iwin = E;
    DblArrayMult(Iwin.data.Pr, I, Iwin.data.Pr, N);
#if DEBUG_ON_FILE == 1
    DEBUG_STORE_NUM(&Iwin);
    Flag = DEBUG_SAVE2FILE("Iwin.txt");
    if (Flag < 0)
    {
        FreeNumArrayType(&Iwin);
        return Flag;
    }
#endif

    /* increases fft length to have better resolution */
    NPad = 4 * pow2_atleast(N);
    NPad = max(NPad, 1024);
    dnu = 1.0 / NPad;

    /* Grow the array and pad with 0s */
    Iwin.data.Pr = REALLOC_VSI(Iwin.data.Pr, NPad * sizeof(double));
    if (Iwin.data.Pr == NULL) return VSI_OUTOFMEMORY;
    memset(Iwin.data.Pr + N, 0, (NPad - N) * sizeof(double));
    Iwin.n = NPad;

    Flag = CreateNumArrayType(VSI_COMPLEX, NPad, NULL, &Ihat);
    if (Flag < 0)
    {
        FreeNumArrayType(&Iwin);
        return Flag;
    }

    /* FFT */
    Flag = VSI_fft(&Iwin, &Ihat);

    /* No longer need it */
    FreeNumArrayType(&Iwin); // this does the job of FreeNumArrayType(&E); since Iwin is clone from E

    /* Check error flag returned by VSI_fft */
    if (Flag < 0) {
        FreeNumArrayType(&Ihat);
        return Flag;
    }

    /* Single-sided spectrum */
    nnu = Ihat.n = (int)CEIL((Ihat.n + 1) * 0.5);
    SqrDblModulus(&Ihat, Ihat.data.Pr);
    Ihat.Type = VSI_REAL;
    Ihat.data.Pr[0] = Ihat.data.Pr[0] / 2.0;
    Ihat.data.Pr[nnu-1] = Ihat.data.Pr[nnu-1] / 2.0;
    SqrtDblArray(Ihat.data.Pr, nnu, Ihat.data.Pr);

#if DEBUG_ON_FILE == 1
    DEBUG_STORE_NUM(&Ihat);
    Flag = DEBUG_SAVE2FILE("IHat.txt");
    if (Flag < 0) return Flag;
#endif

    IhatMax = FractionalMaxDblArray(Ihat.data.Pr, nnu, &jmax);
    if (options->SpectrumSkewness)
    {
        skewwsize = 0.5 * (double)NPad / L_instep;
        nubin = SkewnessAdjust(jmax, &Ihat, skewwsize);
    }
    else nubin = jmax;

    nu = nubin * dnu;
    *period = 1.0 / nu;

    if (isfinite(LambdaCenter))
    {
        expectedratio = (nu * LambdaCenter) / (2 * RuleStep);
        if (expectedratio < 0.8 || expectedratio > 1.25)
        {
            return VSI_DF_SRCWAVELENGTH_ERROR;
        }
    }

    FreeNumArrayType(&Ihat);

    return VSI_OK_FLAG;

} /* DetectFrequency*/

ErrorFlagType DetectFrequency_RTI(const NumArrayType* fringe,
    const VSI_StackImages_Setting_Type* Setting,
    const VSI_StackImages_opt_Type* options,
    double* period)
{
    NumArrayType Iwin, Ihat;
    double LambdaCenter, FWHMLambda, RuleStep, expectedratio;
    double baseline, L, L_instep, * I ;
    double nu, dnu, IhatMax, jmax, skewwsize, nubin;
    ErrorFlagType Flag;
    int n, locmax, HW, i1, i9, N, NPad, nnu;
    VSI_BOOL HardTruncated;
    double Ewidth, cosinfraction;

    Flag = MedianDblArray(fringe->data.Pr, fringe->n, &baseline);
    if (Flag < 0) 
        return Flag;

    I = fringe->data.Pr;
    n = fringe->n;
  
#if DEBUG_ON_FILE == 1
    DEBUG_STORE_NUM(fringe);
    Flag = DEBUG_SAVE2FILE("fringe_s.txt");
    if (Flag < 0) return Flag;
#endif

    switch (options->FrequencyMethod)
    {
    case VSI_FREQDETECT_FFT:

        LambdaCenter = Setting->LambdaCenter;
        FWHMLambda = Setting->FWHMLambda;
        RuleStep = fabs(Setting->RuleStep);

        L = 0.5 * sqrt(log(2) / (PI)) * LambdaCenter * LambdaCenter / FWHMLambda;
        L_instep = L / RuleStep;

        HardTruncated = !isinf(L_instep);
        MaxDblArray(fringe->data.Pr, n, &locmax);

        if (HardTruncated)
        {
            HW = 3 * (int)ROUND(L_instep);
            i1 = max(0, locmax - HW + 1);
            i9 = min(n - 1, locmax + HW - 1);
            N = i9 - i1 + 1;
            I += i1;
        }
        else
        {
            N = n;
        }

        switch (options->WindowType)
        {
        case VSI_WINDOWTYPE_GAUSSIAN:
            Ewidth = options->Ewidth;
            Flag = eGaussianwin(N, Ewidth, &Iwin); // to Former E window matrix
            if (Flag < 0)
                return Flag;
            break;

        case VSI_WINDOWTYPE_TUKEY:
            cosinfraction = HardTruncated ? 0.25 : 1.0;
            Flag = eTukeywin(N, cosinfraction, &Iwin); // to Former E window matrix
            if (Flag < 0) 
                return Flag;
            break;

        default:
            return VSI_NOTYET_IMPLEMENTED;
        }

        break;

    default:
        return VSI_NOTYET_IMPLEMENTED;
    }

   
    // IWin *= (Fringe - Baseline) * IWin -- The folloxin loop replace DblArrayShift(I, I, n, -baseline) above AND DblArrayMult(Iwin.data.Pr, I, Iwin.data.Pr, N);
    double* pI = I;
    double* pIWin = Iwin.data.Pr;
#if defined(_OPENMP)
    int li;
#pragma omp parallel for schedule(static, 32) firstprivate(pIWin,pI)
    for (li = 0; li < N; ++li)
    {
        pIWin[li] *= pI[li] - baseline;
    }
#else
    for (int li = 0; li < N; ++li)
    {
        *(pIWin++) *= *(pI++) - baseline;
    }
#endif
    pI = 0; pIWin = 0; //End Of calculation, detach pointers

#if DEBUG_ON_FILE == 1
    DEBUG_STORE_NUM(&Iwin);
    Flag = DEBUG_SAVE2FILE("Iwin.txt");
    if (Flag < 0)
    {
        FreeNumArrayType(&Iwin);
        return Flag;
    }
#endif

    /* increases fft length to have better resolution */
    NPad = 4 * pow2_atleast(N);
    NPad = max(NPad, 1024);
    dnu = 1.0 / NPad;

    /* Grow the array and pad with 0s */
    Iwin.data.Pr = REALLOC_VSI(Iwin.data.Pr, NPad * sizeof(double));
    if (Iwin.data.Pr == NULL)
    {
        FreeNumArrayType(&Iwin);
        return VSI_OUTOFMEMORY;
    }
    memset(Iwin.data.Pr + N, 0, (NPad - N) * sizeof(double));
    Iwin.n = NPad;

    Flag = CreateNumArrayType(VSI_COMPLEX, NPad, NULL, &Ihat);
    if (Flag < 0)
    {
        FreeNumArrayType(&Iwin);
        return Flag;
    }

    /* FFT */
    Flag = VSI_fft(&Iwin, &Ihat);

    /* No longer need it */
    FreeNumArrayType(&Iwin);

    /* Check error flag returned by VSI_fft */
    if (Flag < 0)
    {
        FreeNumArrayType(&Ihat);
        return Flag;
    }

    /* Single-sided spectrum */
    nnu = Ihat.n = (int)CEIL((Ihat.n + 1) * 0.5);
    SqrDblModulus(&Ihat, Ihat.data.Pr);
    Ihat.Type = VSI_REAL;
    Ihat.data.Pr[0] = Ihat.data.Pr[0] / 2.0;
    Ihat.data.Pr[nnu - 1] = Ihat.data.Pr[nnu - 1] / 2.0;
    SqrtDblArray(Ihat.data.Pr, nnu, Ihat.data.Pr);

#if DEBUG_ON_FILE == 1
    DEBUG_STORE_NUM(&Ihat);
    Flag = DEBUG_SAVE2FILE("IHat.txt");
    if (Flag < 0) return Flag;
#endif

    IhatMax = FractionalMaxDblArray(Ihat.data.Pr, nnu, &jmax);
    if (options->SpectrumSkewness)
    {
        skewwsize = 0.5 * (double)NPad / L_instep;
        nubin = SkewnessAdjust(jmax, &Ihat, skewwsize);
    }
    else nubin = jmax;

    nu = nubin * dnu;
    *period = 1.0 / nu;

    if (isfinite(LambdaCenter))
    {
        expectedratio = (nu * LambdaCenter) / (2 * RuleStep);
        if (expectedratio < 0.8 || expectedratio > 1.25)
        {
            FreeNumArrayType(&Ihat);
            return VSI_DF_SRCWAVELENGTH_ERROR;
        }
    }

    FreeNumArrayType(&Ihat);

    return VSI_OK_FLAG;

} /* DetectFrequency_RTI*/


/*
 Resampling an fringe signal with original ocillation period = p so that
 5-step demodulation formula with usage of integer index is exact.

 INPUTS:
   ImTable : 3D array of size(nx, ny, n) fringe signal(OPD is along the third dimension)
	p : double scalar, ocillation period, sample unit can has fractional part
   ResamplingMethod : char array resampling method, similar to methods of INTERP1
       stock function, default 'linear'
 OUTPUTS :
   ImResampling : 3D array of size(nx, ny, ni)
		with ni >= n; ni is the smallest such integer with the constraint
       of 5 - step requirement
	   ImResampling->Data is allocated here, up to caller to free it
   samplingstep: double scalar <= 1, resampling step of ImTable, sample
       unit.
*/
ErrorFlagType ResamplingSignalFor5Step(const ImageStack_Type* ImTable, 
	double p, VSI_ResamplingMethod ResamplingMethod, ImageStack_Type* ImResampling, double *samplingstep)
{
	double q, ki, w1, w2;
	double *I, *I1, *I2;
	int k1, k2, i, j, n, ni, nxy, ntotalpixels;
#if DEBUG_ON_FILE == 1
    int start, nimages;
    ErrorFlagType Flag;
#endif

	if (ResamplingMethod == VSI_RESAMPLING_CUBIC)
		return VSI_NOTYET_IMPLEMENTED;

	q = 4.0 * CEIL(p / 4);
	*samplingstep = p / q;

    switch (ResamplingMethod)
    {
    case VSI_RESAMPLING_NONE:

        *samplingstep = 1.0;
        ImResampling->nx = ImTable->nx;
        ImResampling->ny = ImTable->ny;
        ImResampling->nz = ImTable->nz;
        ntotalpixels = ImResampling->nx * ImResampling->ny * ImResampling->nz;
		/* We want the original data copied to ImResampling as for other branch of the code */
        ImResampling->Data = MALLOC_VSI(sizeof(Float64) * ntotalpixels);
        if (ImResampling->Data == NULL)
            return VSI_OUTOFMEMORY;

        Copy_DoubleArray(ImResampling->Data, ImTable->Data, ntotalpixels);
        break;

    case VSI_RESAMPLING_LINEAR:

        n = ImTable->nz;
        ni = 1 + (int)FLOOR((double)(n - 1) / *samplingstep);
        ImResampling->nx = ImTable->nx;
        ImResampling->ny = ImTable->ny;
        ImResampling->nz = ni;
        nxy = ImResampling->nx * ImResampling->ny;
        ntotalpixels = nxy * ImResampling->nz;
        ImResampling->Data = MALLOC_VSI(sizeof(Float64) * ntotalpixels);
        if (ImResampling->Data == NULL)
            return VSI_OUTOFMEMORY;

        /* Linear interpolation */
        I = ImResampling->Data;
        for (i = 0; i < ni; i++)
        {
            ki = i * *samplingstep;
            k1 = (int)FLOOR(ki);
            k2 = k1 + 1;
            w1 = k2 - ki;
            w2 = ki - k1;
            I1 = ImTable->Data + nxy * k1;
            I2 = ImTable->Data + nxy * k2;
            for (j = 0; j < nxy; j++)
            {
                *I++ = w1 * (*I1++) + w2 * (*I2++);
            }
        }

#if DEBUG_ON_FILE == 1
        start = 30;//217;
        nimages = 3;
        nimages = min(nimages, ni - start);
        DEBUG_STORE_RAW(ImResampling->Data + nxy * start, nimages * nxy, VSI_REAL);
        Flag = DEBUG_SAVE2FILE("ImResampling.txt");
        if (Flag < 0) return Flag;
#endif
        break;

    case VSI_RESAMPLING_CUBIC:
		memset(ImResampling, 0, sizeof(ImageStack_Type));
        return VSI_NOTYET_IMPLEMENTED;
        break;

    default:
		memset(ImResampling, 0, sizeof(ImageStack_Type));
        return VSI_NOTYET_IMPLEMENTED;
    }

	return VSI_OK_FLAG;
} /* ResamplingSignalFor5Step */

ErrorFlagType ResamplingSignalFor5Step_RTI(const VSI_StackImages_Type* Images,
    double p, VSI_ResamplingMethod ResamplingMethod, ImageStack_Type* ImResampling, double* samplingstep)
{
    double q;
    int i, nxy, ntotalpixels;

    if (ResamplingMethod == VSI_RESAMPLING_CUBIC)
        return VSI_NOTYET_IMPLEMENTED;

    q = 4.0 * CEIL(p / 4);
    *samplingstep = p / q;

    P_ImageData* pImData = Images->ImageArray;

    switch (ResamplingMethod)
    {
    case VSI_RESAMPLING_NONE:

        *samplingstep = 1.0;
        ImResampling->nx = Images->Width;
        ImResampling->ny = Images->Height;
        ImResampling->nz = Images->NumberOfImages;
        nxy = ImResampling->nx * ImResampling->ny;
        ntotalpixels = nxy * ImResampling->nz;
        /* We want the original data copied to ImResampling as for other branch of the code */
        ImResampling->Data = MALLOC_VSI(sizeof(Float64) * ntotalpixels);
        if (ImResampling->Data == NULL)
            return VSI_OUTOFMEMORY;
       
        double* pDest = ImResampling->Data;
#if defined(_OPENMP)
#pragma omp parallel for schedule(static, 8) firstprivate(nxy, pDest, pImData)
        for (i = 0; i < ImResampling->nz; i++)
        {
            UInt8* pSrc = pImData[i];
            for (int j = 0; j < nxy; j++)
            {
                pDest[i * nxy + j] = (double)(*(pSrc++));
            }
        }
#else
        for (i = 0; i < ImResampling->nz; i++)
        {
            UInt8* pSrc = pImData[i];
            for (int j = 0; j < nxy; j++)
            {
                *(pDest++) = (double)(*(pSrc++));
            }
        }
#endif
        break;

    case VSI_RESAMPLING_LINEAR:

        ImResampling->nx = Images->Width;
        ImResampling->ny = Images->Height;
        ImResampling->nz = 1 + (int)FLOOR((double)(Images->NumberOfImages - 1) / *samplingstep);
        nxy = ImResampling->nx * ImResampling->ny;
        ntotalpixels = nxy * ImResampling->nz;
        ImResampling->Data = MALLOC_VSI(sizeof(Float64) * ntotalpixels);
        if (ImResampling->Data == NULL)
            return VSI_OUTOFMEMORY;

        double* pI = ImResampling->Data;
        /* Linear interpolation */
#if defined(_OPENMP)
#pragma omp parallel for schedule(static, 8) firstprivate(nxy, pI, pImData, samplingstep)
        for (i = 0; i < ImResampling->nz; i++)
        {
            double ki = i * *samplingstep;
            int k1 = (int)FLOOR(ki);
            int k2 = k1 + 1;
            double w1 = k2 - ki;
            double w2 = ki - k1;
            UInt8* pI1 = pImData[k1];
            UInt8* pI2 = pImData[k2];
            double* pImDest = (pI + i * nxy);
            for (int j = 0; j < nxy; j++)
            {
                *pImDest++ = w1 * (double)(*pI1++) + w2 * (double)(*pI2++);
            }
        }
#else
        for (i = 0; i < ImResampling->nz; i++)
        {
            double ki = i * *samplingstep;
            int k1 = (int)FLOOR(ki);
            int k2 = k1 + 1;
            double w1 = k2 - ki;
            double w2 = ki - k1;
            UInt8* pI1 = Images->ImageArray[k1];
            UInt8* pI2 = Images->ImageArray[k2];
            for (int j = 0; j < nxy; j++)
            {
                *pI++ = w1 * (double)(*pI1++) + w2 * (double)(*pI2++);
            }
        }
#endif
        

#if DEBUG_ON_FILE == 1
        int start, nimages;
        ErrorFlagType Flag;
        start = 30; //217;
        nimages = 3;
        nimages = min(nimages, ImResampling->nz - start);
        DEBUG_STORE_RAW(ImResampling->Data + nxy * start, nimages * nxy, VSI_REAL);
        Flag = DEBUG_SAVE2FILE("ImResampling.txt");
        if (Flag < 0) return Flag;
#endif
        break;

    case VSI_RESAMPLING_CUBIC:
        memset(ImResampling, 0, sizeof(ImageStack_Type));
        return VSI_NOTYET_IMPLEMENTED;
        break;

    default:
        memset(ImResampling, 0, sizeof(ImageStack_Type));
        return VSI_NOTYET_IMPLEMENTED;
    }

    return VSI_OK_FLAG;
} /* ResamplingSignalFor5Step_RTI */


ErrorFlagType colduplicate(double* A, size_t m, const double *J, size_t nj, double **BAddr)
{
    size_t nB;
    double *B;
    size_t iB, jA, iBstart, jAstart, n;
    size_t Bsize, movesize;

    nB = nj;
    Bsize = (size_t)m * (size_t)nB * sizeof(double);
    *BAddr = B = MALLOC_VSI(Bsize);
    if (B == NULL)
        return VSI_OUTOFMEMORY;
    iB = iBstart = 0;
    jAstart = (int)J[0];
    while (iBstart < nB)
    {
        while (++iB < nB)
        {
            jA = (int)J[iB];
            if ((jA - jAstart) != (iB - iBstart)) 
                break;
        }
        n = iB - iBstart;
        movesize = (size_t)n * (size_t)m * sizeof(double);
        memcpy(B + iBstart * m, A + jAstart * m, movesize);
        iBstart = iB;
        jAstart = jA;
    }
    return VSI_OK_FLAG;
} /* colduplicate */


ErrorFlagType rowduplicate(double* A, size_t m, int n, const double* I, size_t ni, double** BAddr)
{
    size_t mB, nB, i, j;
    double *B, *Ai;
    size_t Bsize;

    mB = n;
    nB = ni;
    Bsize = (size_t)mB * (size_t)nB * sizeof(double);
    *BAddr = B = MALLOC_VSI(Bsize);
    if (B == NULL)
        return VSI_OUTOFMEMORY;
    for (i = 0; i < nB; i++)
    {
        Ai = A + (int)I[i];
        for (j = 0; j < mB; j++)
        {
            *B++ = *Ai;
            Ai += m;
        }
    }
    return VSI_OK_FLAG;
} /* rowduplicate */


/* Perform expension sum of two vectors, the resulting 2D array (first vector varies first)
   is allocated, up to caller to free it */
ErrorFlagType ExpansionSum(const double *A, int nA, const double *B, int nB, double **C)
{
    int i, j;
    const double *a, *b;
    double *c;
    *C = c = MALLOC_VSI(sizeof(double) * nA*nB);
    if (c == NULL) return VSI_OUTOFMEMORY;
    b = B;
    for (j = 0; j < nB; j++)
    {
        a = A;
        for (i = 0; i < nA; i++)
            *c++ = *a++ + *b;
        b++;
    }
    return VSI_OK_FLAG;
}

void unwrap(const double* p, int m, int n, double *q)
{
    int i = 0;
// Let open handle this depending on context and current CPUs usage
//#if defined(_OPENMP)
//    omp_set_num_threads(NUM_THREADS); /* NUM_THREADS 1 or 2 seems to be fastest */
//#endif

#if defined(_OPENMP)
#pragma omp parallel for schedule(static, 32) firstprivate(q,p)
#endif
    for (i = 0; i < m; i++)
    {
        double* qj = q + i;
        const double* pj = p + i;
        double qval = *pj;
        *qj = qval;
        for (int j = 1; j < n; j++)
        {
            pj += m;
            qj += m;
            *qj = qval = *pj - ROUND((*pj - qval) * IPI2) * PI2;
        }
    }
}

// phi = StandardizeAngle(phi)
// standardize the angle so that they are in[-pi, pi)
void StandardizeAngle(const double* p, int m, double *q)
{
    int i;
    double phi;

    for (i = 0; i < m; i++)
    {
        phi = IPI2 * *p++;
        phi -= ROUND(phi);
        *q++ = PI2 * phi;
    }
}


// standardize the angle so that they are in[-pi, pi)
inline double ToStandardAngle(double in)
{
    double out = IPI2 * in;
    out -= ROUND(out);
    return PI2 * out;
}

/*
 Estimate the windowing phase by 5 - points methods
 INPUTS:
ImTable : 3D array of size(nx, ny, n), for each pixel(i, j) ImTable(i, j, :)
    is a fringe sampling vector acquired with more or less regular OPD
options : structure contains fields
    PhaseMethod : DemodulationMethod enumerate Larkin, Hariharan, method
        to estimate the phase.Default Hariharan.
        AmplitudeMethod : DemodulationMethod enumerate Larkin, Hariharan, method
        to estimate the amplitude.Default Larkin.
    ResamplingMethod : char array among {'none', 'linear', cubic'},
        interpomation method used for resampling fringe so that 5 - step
        demodulation methods with usage of integer index is exact.
        If set to 'none' resampling is disabled.Default 'linear'.
    FreqMethod: FrequencyMethod enumerate fft or x0
        Method used by DetectFrequency, default fft
    WindowType : char array among {'gaussian', 'tukey'}
        Only applicable for 'fft' method.WindowType used for windowing
        before FFT.Default 'tukey'
    Ewidth : double scalar, sample unit
        Only applicable for 'fft' method and WindowType 'gaussian'
        The width of the Gaussian window function
    nwin : integer scalar, number of samples used to smooth interferogram
        signal for various tasks.Make sure nwin < 0.5 * fringe period
        Only applicable for x0 FreqMethod
    Setting: structure contains fields
        RuleStep : double scalar, step between two OPDs, [m]
        LambdaCenter : double scalar, wavelength of the light source, [m]
        FWHMLambda : double scalar, spectral bandwidth of the light source, [m]
            If RuleStep, LambdaCenter, FWHMLambda are provided
            the expected coherence length in number of samples are estimated
            and used to truncated the signal.Only applicable for FFT method
OUTPUTS
 Demodulation : structure contains fields
     samplingstep : double scalar, new resampling step(<= 1)
     xsampling : vector(1 x np), sampling vector, corresponds to the
        third(page) index of the input ImTable - 1 (started from 0).
     PeriodFrameLength : double scalar period of the(resampling) fringe,
        sample unit
     phi : 3D array(ny x nx x np) is the phase at the mid points
        np + PeriodFrameLength ~n
     A : 3D array is the amplitude at the mid points
        ImResampling : 3D array(ny x nx x np) is subsampled of ImTable at
        the mid points
    
    NOTE : Demodulation is approximate I(z) by :
     Iest(z) = offset(z) + A(z) * sin(4 * pi / lambda * z + phi(z));

 See also : DetectPeak
*/
ErrorFlagType FringeDemodulation(const ImageStack_Type* ImTable,
    const VSI_StackImages_Setting_Type* Setting,
    const VSI_StackImages_opt_Type* options,
    Demodulation_Type* Demodulation)
{
    Float64 fmax, period, samplingstep, newperiod;
    ImageStack_Type ImResampling;
    int npixels, idxmax, nxy, n, np, i, nt;
    NumArrayType fringe;
    ErrorFlagType Flag;
    double di[5], *i1, *Idx, *I, *xsampling;
    double *I1, *I2, *I3, *I4, *I5;
    double *phi, *cphi, *sphi, *A, *tmp, *tmp1, *tmp2;
#if DEBUG_ON_FILE == 1
    double exportpixel[1];
    double* IdxDbg;
#endif

	/* Default returned value when evrything is OK */
	Flag = VSI_OK_FLAG;

	/* Make sure Demodulation structure is clean */
    memset(Demodulation, 0, sizeof(Demodulation_Type));

    nxy = ImTable->nx * ImTable->ny;
    n = ImTable->nz;
    npixels = nxy * n;

    // Detect signal max and eventually saturation state
    fmax = MaxDblArray(ImTable->Data, npixels, &idxmax);
	if ((int)ROUND(fmax) == UCHAR_MAX) // 255, should be defined in <limits.h>
		Flag = SATURATED_INTEROGRAM;
	else if (fmax == 0)
		return VSI_DATACONTAIN0;

    // Extract the fringe for this pixel, fringe variable is allocated
    Flag = CreateNumArrayType(VSI_REAL, n, NULL, &fringe);
    if (Flag < 0) return Flag;
    idxmax = idxmax % nxy;
    for (i = 0; i < n; i++)
    {
        fringe.data.Pr[i] = ImTable->Data[idxmax + i * nxy];
    }
    Flag = DetectFrequency(&fringe, Setting, options, &period);

	/* No longer need it */
    FreeNumArrayType(&fringe);

	if (Flag < 0) return Flag;

    Flag = ResamplingSignalFor5Step(ImTable, period, options->ResamplingMethod,
        &ImResampling, &samplingstep);
    if (Flag < 0) return Flag;
    newperiod = period / samplingstep;

    np = ImResampling.nz;

    Demodulation->nx = ImResampling.nx;
    Demodulation->ny = ImResampling.ny;
    Demodulation->np = np; // ImResampling.nz;

    Demodulation->samplingstep = samplingstep;
    Demodulation->PeriodFrameLength = newperiod;
    Demodulation->ImResampling = ImResampling.Data;

    DblArrayLinspace(di, 5, 0, newperiod);
    DblArrayROUND(di, di, 5);
    Demodulation->nt = nt = np - (int)di[4];
    if (nt <= 0)
        return VSI_NOT_ENOUGHIMAGES;
    i1 = MALLOC_VSI(sizeof(double) * nt);
    if (i1 == NULL)
        return VSI_OUTOFMEMORY;
    DblArrayLinspace(i1, nt, 0, nt-1);
    
    Flag = ExpansionSum(i1, nt, di, 5, &Idx);
	if (Flag < 0)
	{
		FREE_VSI(i1);
		return Flag;
	}

    nxy = ImResampling.nx * ImResampling.ny;
    Flag = colduplicate(ImResampling.Data, nxy, Idx, nt*5, &I);

	/* No longer needed */
	FREE_VSI(Idx);
	Idx = NULL;

    /* check output Flag returned by colduplicate */
	if (Flag < 0)
	{
		FREE_VSI(i1);
		return Flag;
	}

    npixels = nxy * nt;
    I1 = I;
    I2 = I1 + npixels;
    I3 = I2 + npixels;
    I4 = I3 + npixels;
    I5 = I4 + npixels;

#if DEBUG_ON_FILE == 1
    int nimages = nt;
    DEBUG_STORE_RAW(I1, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I1.txt");
    if (Flag < 0) return Flag;

    DEBUG_STORE_RAW(I2, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I2.txt");
    if (Flag < 0) return Flag;

    DEBUG_STORE_RAW(I3, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I3.txt");
    if (Flag < 0) return Flag;

    DEBUG_STORE_RAW(I4, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I4.txt");
    if (Flag < 0) return Flag;

    DEBUG_STORE_RAW(I5, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I5.txt");
    if (Flag < 0) return Flag;
#endif

    phi = MALLOC_VSI(sizeof(double) * npixels);
	if (phi == NULL)
	{
		FREE_VSI(i1);
		FREE_VSI(I);
		return VSI_OUTOFMEMORY;
	}

	A = MALLOC_VSI(sizeof(double) * npixels);
    if (A == NULL)
	{
		FREE_VSI(i1);
		FREE_VSI(phi);
		FREE_VSI(I);
		return VSI_OUTOFMEMORY;
	}

    switch (options->PhaseMethod)
    {
    case VSI_PHASE_LARKIN:
        tmp = phi;

        sphi = MALLOC_VSI(sizeof(double) * npixels);
		if (sphi == NULL)
		{
			FREE_VSI(i1);
			FREE_VSI(A);
			FREE_VSI(phi);
			FREE_VSI(I);
			return VSI_OUTOFMEMORY;
		}
        cphi = MALLOC_VSI(sizeof(double) * npixels);
        if (cphi == NULL)
		{
			FREE_VSI(i1);
			FREE_VSI(sphi);
			FREE_VSI(A);
			FREE_VSI(phi);
			FREE_VSI(I);
			return VSI_OUTOFMEMORY;
		}

        //sphi = 2 * I3 - I5 - I1;
        DblArrayScale(I3, sphi, npixels, 2.0);
        DblArrayMinus(sphi, I5, sphi, npixels);
        DblArrayMinus(sphi, I1, sphi, npixels);

        //cphi = 4 * (I2 - I4). ^ 2;
        DblArrayMinus(I2, I4, cphi, npixels);
        SqrDblArray(cphi, npixels, cphi);
        DblArrayScale(cphi, cphi, npixels, 4.0);
        
        //cphi = cphi - (I1 - I5). ^ 2;
        DblArrayMinus(I1, I5, tmp, npixels);
        SqrDblArray(tmp, npixels, tmp);
        DblArrayMinus(cphi, tmp, cphi, npixels);

        //cphi = sign((I4 - I2)).*sqrt(abs(cphi));
        DblArrayMinus(I4, I2, tmp, npixels);
        DblArraySign(tmp, tmp, npixels);
        DblArrayAbs(cphi, cphi, npixels);
        SqrtDblArray(cphi, npixels, cphi);
        DblArrayMult(cphi, tmp, cphi, npixels);

        //phi = atan2(sphi, cphi);
        DblArrayAtan2(sphi, cphi, phi, npixels);

        FREE_VSI(sphi);
		sphi = NULL;
        FREE_VSI(cphi);
		cphi = NULL;
        break;

    case VSI_PHASE_HARIHARAN:
        
		sphi = MALLOC_VSI(sizeof(double) * npixels);
		if (sphi == NULL)
		{
			FREE_VSI(i1);
			FREE_VSI(A);
			FREE_VSI(phi);
			FREE_VSI(I);
			return VSI_OUTOFMEMORY;
		}
		cphi = MALLOC_VSI(sizeof(double) * npixels);
		if (cphi == NULL)
		{
			FREE_VSI(i1);
			FREE_VSI(sphi);
			FREE_VSI(A);
			FREE_VSI(phi);
			FREE_VSI(I);
			return VSI_OUTOFMEMORY;
		}

        //cphi = 2 * I3 - I5 - I1;
        DblArrayScale(I3, cphi, npixels, 2.0);
        DblArrayMinus(cphi, I5, cphi, npixels);
        DblArrayMinus(cphi, I1, cphi, npixels);

        //sphi = 2 * (I2 - I4).;
        DblArrayMinus(I2, I4, sphi, npixels);
        DblArrayScale(sphi, sphi, npixels, 2.0);

        //phi = atan2(2*(I2-I4),2*I3-I5-I1);
        DblArrayAtan2(sphi, cphi, phi, npixels);

		FREE_VSI(sphi);
		sphi = NULL;
		FREE_VSI(cphi);
		cphi = NULL;
        break;

    default:
		FREE_VSI(i1);
		FREE_VSI(A);
		FREE_VSI(phi);
		FREE_VSI(I);
        return VSI_NOTYET_IMPLEMENTED;
    }

    unwrap(phi, nxy, nt, phi);
#if DEBUG_ON_FILE == 1
    // Make a breal point in MATLAB FringeDemodulation.m
    // s = load('Phi.txt');
    // phi2 = reshape(phi,nx*ny,nt);
    // plot(squeeze(phi2(end/2+1,:))-s.')

    exportpixel[0] = (double)(nxy / 2);
    Flag = rowduplicate(phi, nxy, nt, exportpixel, 1, &IdxDbg);
    DEBUG_STORE_RAW(IdxDbg, nt * 1, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("Phiunwrap.txt");
    FREE_VSI(IdxDbg);
	if (Flag < 0)
	{
		FREE_VSI(i1);
		FREE_VSI(A);
		FREE_VSI(phi);
		FREE_VSI(I);
		return Flag;
	}
#endif

    switch (options->AmplitudeMethod)
    {
    case VSI_AMP_LARKIN:

		tmp1 = MALLOC_VSI(sizeof(double) * npixels);
		if (tmp1 == NULL)
		{
			FREE_VSI(i1);
			FREE_VSI(A);
			FREE_VSI(phi);
			FREE_VSI(I);
			return VSI_OUTOFMEMORY;
		}
		tmp2 = MALLOC_VSI(sizeof(double) * npixels);
		if (tmp2 == NULL)
		{
			FREE_VSI(tmp1);
			FREE_VSI(i1);
			FREE_VSI(A);
			FREE_VSI(phi);
			FREE_VSI(I);
			return VSI_OUTOFMEMORY;
		}

        //A = (I2 - I4). ^ 2 + (I3 - I1).*(I3 - I5);
        DblArrayMinus(I3, I1, tmp1, npixels);
        DblArrayMinus(I3, I5, tmp2, npixels);
        DblArrayMult(tmp1, tmp2, tmp1, npixels);
        DblArrayMinus(I2, I4, tmp2, npixels);
        SqrDblArray(tmp2, npixels, tmp2);

        DblArrayPlus(tmp1, tmp2, A, npixels);

        //A = abs(A);
        DblArrayAbs(A, A, npixels);

        FREE_VSI(tmp1);
		tmp1 = NULL;
        FREE_VSI(tmp2);
		tmp2 = NULL;
        break;

    case VSI_AMP_HARIHARAN:

		tmp1 = MALLOC_VSI(sizeof(double) * npixels);
		if (tmp1 == NULL)
		{
			FREE_VSI(i1);
			FREE_VSI(A);
			FREE_VSI(phi);
			FREE_VSI(I);
			return VSI_OUTOFMEMORY;
		}
		tmp2 = MALLOC_VSI(sizeof(double) * npixels);
		if (tmp2 == NULL)
		{
			FREE_VSI(tmp1);
			FREE_VSI(i1);
			FREE_VSI(A);
			FREE_VSI(phi);
			FREE_VSI(I);
			return VSI_OUTOFMEMORY;
		}

        //A = (I2 - I4). ^ 2 + (I3 - 0.5 * (I5 + I1)). ^ 2;
        DblArraySuperposition(I1, I5, tmp1, npixels, 0.5, 0.5);
        DblArrayMinus(I3, tmp1, tmp1, npixels);
        // bizzare on effectue pas le square ? cf Bruno pour + d'info

        DblArrayMinus(I2, I4, tmp2, npixels);
        SqrDblArray(tmp2, npixels, tmp2);

        DblArrayPlus(tmp2, tmp1, A, npixels);

		FREE_VSI(tmp1);
		tmp1 = NULL;
		FREE_VSI(tmp2);
		tmp2 = NULL;
        break;

    default:
		FREE_VSI(i1);
		FREE_VSI(A);
		FREE_VSI(phi);
		FREE_VSI(I);
    }

    // A = sqrt(A) / 2;
    SqrtDblArray(A, npixels, A);
    DblArrayScale(A, A, npixels, 0.5);

#if DEBUG_ON_FILE == 1
    // Make a breal point in MATLAB FringeDemodulation.m
    // s = load('A.txt');
    // A2 = reshape(A,nx*ny,nt);
    // plot(squeeze(A2(end/2+1,:))-s.')

    exportpixel[0] = (double)(nxy / 2);
    Flag = rowduplicate(A, nxy, nt, exportpixel, 1, &IdxDbg);
    DEBUG_STORE_RAW(IdxDbg, nt * 1, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("A.txt");
    FREE_VSI(IdxDbg);
	if (Flag < 0)
	{
		FREE_VSI(i1);
		FREE_VSI(A);
		FREE_VSI(phi);
		FREE_VSI(I);
		return Flag;
	}
#endif

	Flag = ExpansionSum(i1, nt, di+2, 1, &xsampling);
	if (Flag < 0)
	{
		FREE_VSI(i1);
		FREE_VSI(A);
		FREE_VSI(phi);
		FREE_VSI(I);
		return Flag;
	}

	DblArrayScale(xsampling, xsampling, nt, samplingstep);

	/* These allocated data fields are conserved in Demodulation */
	Demodulation->xsampling = xsampling;
	Demodulation->phi = phi;
	Demodulation->A = A;

	/* These will be populated with suitable memory allocation 
       by DetecPeak when needed */
    Demodulation->phi_transposed = NULL;
    Demodulation->A_transposed = NULL;

	FREE_VSI(i1);
    FREE_VSI(I);

	return Flag;
} /* of FringeDemodulation */

ErrorFlagType FringeDemodulation_RTI(const VSI_StackImages_Type* Images,
    const VSI_StackImages_Setting_Type* Setting,
    const VSI_StackImages_opt_Type* options,
    Demodulation_Type* Demodulation)
{
    Float64 period, samplingstep, newperiod;
    ImageStack_Type ImResampling;
    int npixels, idxmax, nxy, n, i;
    NumArrayType fringe;
    ErrorFlagType Flag;
    double di[5], * i1, * xsampling;
    double* I1, * I2, * I3, * I4, * I5;
    double* phi, * A;
#if DEBUG_ON_FILE == 1
    double exportpixel[1];
    double* IdxDbg;
#endif
    /* Default returned value when evrything is OK */
    Flag = VSI_OK_FLAG;

    /* Make sure Demodulation structure is clean */
    memset(Demodulation, 0, sizeof(Demodulation_Type));

    nxy = Images->Width * Images->Height;
    n = Images->NumberOfImages;
    npixels = nxy * n;

    // Detect signal max and eventually saturation state
  
    UInt8 ucMax = 0;
    int imMax = -1;
    for (int im = 0; im < n; ++im)
    {
        UInt8* ptr = Images->ImageArray[im];
        for (int li = 0; li < nxy; ++li)
        {
            if (*ptr > ucMax) {
                ucMax = *ptr;
                idxmax = li; // index of pixel containing maximum
                imMax = im; // index of image containing maximum (debug purpose)
            }
            ++ptr;
        }
    }
   
    if (ucMax == 0)
        return VSI_DATACONTAIN0;
    
    if(ucMax >= UCHAR_MAX)
        Flag |= SATURATED_INTEROGRAM;

    // Extract the fringe for this pixel, fringe variable is allocated
    Flag = CreateNumArrayType(VSI_REAL, n, NULL, &fringe);
    if (Flag < 0) 
        return Flag;
    // Allocate Fringe for index max pixel for all images
    double* pFr = fringe.data.Pr;
    P_ImageData* pImData= Images->ImageArray;
#if defined(_OPENMP)
#pragma omp parallel for schedule(static, 32) firstprivate(pFr,pImData)
#endif
    for (i = 0; i < n; i++)
    {
        pFr[i] = (double) ((pImData[i])[idxmax]);
    }
    // Calcution done with ptr
    pFr = NULL;   pImData = NULL;
    Flag = DetectFrequency_RTI(&fringe, Setting, options, &period);
 
    // free fringe arrays
    FreeNumArrayType(&fringe);

    if (Flag < 0) 
        return Flag;

    // Resample and Allocate ImResampling.Data from images
    Flag = ResamplingSignalFor5Step_RTI(Images, period, options->ResamplingMethod, &ImResampling, &samplingstep);
    if (Flag < 0) 
        return Flag;
    newperiod = period / samplingstep;

    Demodulation->nx = ImResampling.nx;
    Demodulation->ny = ImResampling.ny;
    Demodulation->np = ImResampling.nz;

    Demodulation->samplingstep = samplingstep;
    Demodulation->PeriodFrameLength = newperiod;
    Demodulation->ImResampling = ImResampling.Data;

    DblArrayLinspace(di, 5, 0, newperiod);
    DblArrayROUND(di, di, 5);
    Demodulation->nt = Demodulation->np - (int)di[4];
    if (Demodulation->nt <= 0)
    {
        FREE_VSI(ImResampling.Data);
        Demodulation->ImResampling = NULL;
        return VSI_NOT_ENOUGHIMAGES;
    }
    i1 = MALLOC_VSI(sizeof(double) * Demodulation->nt);
    if (i1 == NULL)
    {
        FREE_VSI(ImResampling.Data);
        Demodulation->ImResampling = NULL;
        return VSI_OUTOFMEMORY;
    }
    DblArrayLinspace(i1, Demodulation->nt, 0, Demodulation->nt - 1);

    // NEW WAY using pointer on resampling Cube
    nxy = ImResampling.nx * ImResampling.ny;
    npixels = nxy * Demodulation->nt;

    I1 = ImResampling.Data + (int)di[0];
    I2 = ImResampling.Data + (int)di[1] * nxy;
    I3 = ImResampling.Data + (int)di[2] * nxy;
    I4 = ImResampling.Data + (int)di[3] * nxy;
    I5 = ImResampling.Data + (int)di[4] * nxy;

#if DEBUG_ON_FILE == 1
    int nimages = Demodulation->nt;
    DEBUG_STORE_RAW(I1, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I1.txt");
    if (Flag < 0) return Flag;

    DEBUG_STORE_RAW(I2, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I2.txt");
    if (Flag < 0) return Flag;

    DEBUG_STORE_RAW(I3, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I3.txt");
    if (Flag < 0) return Flag;

    DEBUG_STORE_RAW(I4, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I4.txt");
    if (Flag < 0) return Flag;

    DEBUG_STORE_RAW(I5, nimages * nxy, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("I5.txt");
    if (Flag < 0) return Flag;
#endif

    phi = MALLOC_VSI(sizeof(double) * npixels);
    if (phi == NULL)
    {
        FREE_VSI(i1);
        return VSI_OUTOFMEMORY;
    }

    A = MALLOC_VSI(sizeof(double) * npixels);
    if (A == NULL)
    {
        FREE_VSI(i1);
        FREE_VSI(phi);
        return VSI_OUTOFMEMORY;
    }

    switch (options->PhaseMethod)
    {
    case VSI_PHASE_LARKIN:
        {
            double* pPhi = phi;
            double* pI1 = I1;
            double* pI2 = I2;
            double* pI3 = I3;
            double* pI4 = I4;
            double* pI5 = I5;

            //sphi = 2 * I3 - I5 - I1;
            //cphi = 4 * (I2 - I4). ^ 2;
            //cphi = cphi - (I1 - I5). ^ 2;
            //cphi = sign((I4 - I2)).*sqrt(abs(cphi));
            //phi = atan2(sphi, cphi);
#if defined(_OPENMP)
            int li;
#pragma omp parallel for schedule(static, 32) firstprivate(pI1, pI2, pI3, pI4, pI5, pPhi)
            for (li = 0; li < npixels; li++)
            {
                double difI1I5 = pI1[li] - pI5[li];
                double difI2I4 = pI2[li]  - pI4[li];
                pPhi[li] = atan2(2.0 * pI3[li] - pI5[li] - pI1[li], Dblsign(-difI2I4) * sqrt((double)fabs(4.0 * difI2I4 * difI2I4 - difI1I5 * difI1I5)));
            }
#else
            for (int li = 0; li < npixels; li++)
            {
                double difI1I5 = *pI1 - *pI5;
                double difI2I4 = (*(pI2++)) - (*(pI4++));
                double sphi = 2.0 * (*(pI3++)) - (*(pI5++)) - (*(pI1++));
                double cphi = Dblsign(-difI2I4) * sqrt((double)fabs(4.0 * difI2I4 * difI2I4 - difI1I5 * difI1I5));
                *(pPhi++) = atan2(sphi, cphi);
            }
#endif         
        }
        break;

    case VSI_PHASE_HARIHARAN: // default ?
        {
            double* pPhi = phi;
            double* pI1 = I1;
            double* pI2 = I2;
            double* pI3 = I3;
            double* pI4 = I4;
            double* pI5 = I5;
            //phi = atan2(2 * (I2 - I4), 2 * I3 - I5 - I1);
#if defined(_OPENMP)
            int li;
#pragma omp parallel for schedule(static, 32) firstprivate(pI1, pI2, pI3, pI4, pI5, pPhi)
            for (li = 0; li < npixels; li++)
            {
                pPhi[li] = atan2(2.0 * (pI2[li] - pI4[li]), 2.0 * pI3[li] - pI5[li] - pI1[li]);
            }
#else
            for (int li = 0; li < npixels; li++)
            {
                *(pPhi++) = atan2(2.0 * (*(pI2++) - *(pI4++)), 2.0 * (*(pI3++)) - (*(pI5++)) - (*(pI1++)));
            }
#endif  
        }
        break;

    default:
        FREE_VSI(i1);
        FREE_VSI(A);
        FREE_VSI(phi);
        return VSI_NOTYET_IMPLEMENTED;
    }

    unwrap(phi, nxy, Demodulation->nt, phi);

#if DEBUG_ON_FILE == 1
    // Make a breal point in MATLAB FringeDemodulation.m
    // s = load('Phi.txt');
    // phi2 = reshape(phi,nx*ny,nt);
    // plot(squeeze(phi2(end/2+1,:))-s.')
    int nt = Demodulation->nt;
    exportpixel[0] = (double)(nxy / 2);
    Flag = rowduplicate(phi, nxy, nt, exportpixel, 1, &IdxDbg);
    DEBUG_STORE_RAW(IdxDbg, nt * 1, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("Phiunwrap.txt");
    FREE_VSI(IdxDbg);
    if (Flag < 0)
    {
        FREE_VSI(i1);
        FREE_VSI(A);
        FREE_VSI(phi);
        return Flag;
    }
#endif
    switch (options->AmplitudeMethod)
    {
    case VSI_AMP_LARKIN: // (default)
        {
            double* pA = A;
            double* pI1 = I1;
            double* pI2 = I2;
            double* pI3 = I3;
            double* pI4 = I4;
            double* pI5 = I5;

            //A = (I2 - I4). ^ 2 + (I3 - I1).*(I3 - I5);
            //A = abs(A);
            // A = sqrt(A) / 2;
#if defined(_OPENMP)
            int li;
#pragma omp parallel for schedule(static, 32) firstprivate(pI1, pI2, pI3, pI4, pI5, pA)
            for (li = 0; li < npixels; li++)
            {
                pA[li] = 0.5 * sqrt(fabs(pow(pI2[li] - pI4[li], 2.0) + (pI3[li] - pI1[li]) * (pI3[li] - pI5[li])));
            }
#else
            for (int li = 0; li < npixels; li++)
            {
                double vi3 = *(pI3++);
                *(pA++) = 0.5 * sqrt(fabs(pow(*(pI2++) - *(pI4++), 2.0) + (vi3 - *(pI1++)) * (vi3 - *(pI5++))));
            }
#endif
        }
        break;

    case VSI_AMP_HARIHARAN:
        {
            double* pA = A;
            double* pI1 = I1;
            double* pI2 = I2;
            double* pI3 = I3;
            double* pI4 = I4;
            double* pI5 = I5;

            // A = (I2 - I4). ^ 2 + (I3 - 0.5 * (I5 + I1)). ^ 2;   // check with Bruno Square or Not (I3 - 0.5 * (I5 + I1)) term
            // A = sqrt(A) / 2;
#if defined(_OPENMP)
            int li;
#pragma omp parallel for schedule(static, 32) firstprivate(pI1, pI2, pI3, pI4, pI5, pA)
            for (li = 0; li < npixels; li++)
            {
                pA[li] = 0.5 * sqrt(pow(pI2[li] - pI4[li], 2.0) + pow(pI3[li] - 0.5 * (pI5[li] + pI1[li]), 2.0));
            }
#else
            for (int li = 0; li < npixels; li++)
            {
                *(pA++) = 0.5 * sqrt( pow(*(pI2++) - *(pI4++), 2.0) + pow(*(pI3++) - 0.5 * (*(pI5++) + *(pI1++)), 2.0));
            }
#endif
        }
        break;

    default:
        FREE_VSI(i1);
        FREE_VSI(A);
        FREE_VSI(phi);
    }


#if DEBUG_ON_FILE == 1
    // Make a breal point in MATLAB FringeDemodulation.m
    // s = load('A.txt');
    // A2 = reshape(A,nx*ny,nt);
    // plot(squeeze(A2(end/2+1,:))-s.')

    exportpixel[0] = (double)(nxy / 2);
    Flag = rowduplicate(A, nxy, nt, exportpixel, 1, &IdxDbg);
    DEBUG_STORE_RAW(IdxDbg, nt * 1, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("A.txt");
    FREE_VSI(IdxDbg);
    if (Flag < 0)
    {
        FREE_VSI(i1);
        FREE_VSI(A);
        FREE_VSI(phi);
        return Flag;
    }
#endif


    Flag = ExpansionSum(i1, Demodulation->nt, di + 2, 1, &xsampling);
    if (Flag < 0)
    {
        FREE_VSI(i1);
        FREE_VSI(A);
        FREE_VSI(phi);
        return Flag;
    }

    DblArrayScale(xsampling, xsampling, Demodulation->nt, samplingstep);

    /* These allocated data fields are conserved in Demodulation */
    Demodulation->xsampling = xsampling;
    Demodulation->phi = phi;
    Demodulation->A = A;

    /* These will be populated with suitable memory allocation
       by DetecPeak when needed */
    Demodulation->phi_transposed = NULL;
    Demodulation->A_transposed = NULL;

    FREE_VSI(i1);

    return Flag;
} /* of FringeDemodulation */


ErrorFlagType FreeDemodulation(
	Demodulation_Type* Demodulation)
{
	if (Demodulation)
	{
		FREE_VSI(Demodulation->xsampling);
		FREE_VSI(Demodulation->phi);
		FREE_VSI(Demodulation->A);
		FREE_VSI(Demodulation->ImResampling);
		FREE_VSI(Demodulation->phi_transposed);
		FREE_VSI(Demodulation->A_transposed);
		memset(Demodulation, 0, sizeof(Demodulation_Type));
	}
	return VSI_OK_FLAG;
}


#define NB_DOUBLE_RM_Image 4
ErrorFlagType AllocateVSI_Output(VSI_DblOutput_Type* VSIResultStruct, int nx, int ny)
{
    DOUBLE_RM_Image* Image;
    Float64* Data;
    int nxy, i, j;

    if (VSIResultStruct == NULL) return VSI_OK_FLAG;

    /* Reset to clean so that Free can work correctly */
    memset(VSIResultStruct, 0, sizeof(VSI_DblOutput_Type));

    nxy = nx * ny;
    for (i = 0; i < NB_DOUBLE_RM_Image; i++)
    {
        Image = MALLOC_VSI(sizeof(DOUBLE_RM_Image));
        if (Image == NULL) 
            return VSI_OUTOFMEMORY;
        Image->Data = Data = MALLOC_VSI(sizeof(double) * nxy);
        if (Data == NULL)
            return VSI_OUTOFMEMORY; // Memory leak (Image)
        for (j = 0; j < nxy; j++) *Data++ = NAN;
        Image->nx = nx;
        Image->ny = ny;
        switch (i)
        {
        case 0:
            VSIResultStruct->Topo_Image = Image;
            break;
        case 1:
            VSIResultStruct->TopoFlatten_Image = Image;
            break;
        case 2:
            VSIResultStruct->Amplitude_Image = Image;
            break;
        case 3:
            VSIResultStruct->Phase_Image = Image;
            break;
        //default:
        }       
    }

    return VSI_OK_FLAG;
}

ErrorFlagType FreeVSI_Output(VSI_DblOutput_Type* VSIResultStruct)
{
    DOUBLE_RM_Image* Image;
    int i;
    if (VSIResultStruct != NULL)
    {
        for (i = 0; i < NB_DOUBLE_RM_Image; i++)
        {
            switch (i)
            {
            case 0:
                Image = VSIResultStruct->Topo_Image;
                break;
            case 1:
                Image = VSIResultStruct->TopoFlatten_Image;
                break;
            case 2:
                Image = VSIResultStruct->Amplitude_Image;
                break;
            case 3:
                Image = VSIResultStruct->Phase_Image;
                break;
            default:
                // Some compiler requires this branch (which logically never run)
                Image = NULL;
            }
            if (Image != NULL)
            {
                if (Image->Data != NULL) FREE_VSI(Image->Data);
                Image->Data = NULL;
                FREE_VSI(Image);
            }
        }
        memset(VSIResultStruct, 0, sizeof(VSI_DblOutput_Type));
    }
    return VSI_OK_FLAG;
}

/*
 Return the contiguous index interval IDX where the (envelop)
 signal A is "strong"

 INPUTS:
   A: double vector, is the amplitude of the interferogram
   FilterKernel: vector of Kernel for filterring, must sum to 1
   PeakDetectionRelThreshold: double scalar in (0,1), threshold used
           to truncate fringe data for peak detection. The value is relative
           to the max amplitude.
           The smaller value the more data used for peak detection. Default 0.1.
   SelectAllWindowsOptions: structure contains fields
       NoiseLevel: double scalar, typical sensor noise standard deviation in LSB
       MinLength: double scalar, minimum sample length of the interval to to be
        recorgnize as valid peak, sample unit
 OUTPUT:
   IndexIntervalType: returns the two edges (included) of the contiguous intervals index
    of A that contains the peak

 See also: DetectPeak
*/
#define ABOVENOISE_FACT 3.0
ErrorFlagType SelectAllWindows(NumArrayType *A, SelectAllWindowsOptions_Type *SelectAllWindowsOptions, IndexIntervalType *IndexInterval)
{
    double NoiseLevel, minAmax, maxAmax;
    double Ai, Aselected, Amax_int, threshold, PeakDetectionRelThreshold, sumKernel;
    int n, nh, nA, nAmax, iMinLength;
    double *DblTemp, * Amax, *CumsumA;
    int i1, i2, imax, i, i1select, i2select, ishift;
    VSI_BOOL b, bold;

    NoiseLevel = SelectAllWindowsOptions->NoiseLevel;
    iMinLength = (int)SelectAllWindowsOptions->MinLength;
    PeakDetectionRelThreshold = SelectAllWindowsOptions->PeakDetectionRelThreshold;

    n = SelectAllWindowsOptions->FilterNWin;
    nh = (n - 1) / 2;
    nA = A->n;
    nAmax = nA - 2 * nh;
    
    if (SelectAllWindowsOptions->tmp != NULL)
        DblTemp = SelectAllWindowsOptions->tmp; // Use memory allocated by the caller
    else
    {
        /* Allocate all double arrays in one chunk then split them */
        DblTemp = MALLOC_VSI(sizeof(double) * 2 * nA);
        if (DblTemp == NULL) return VSI_OUTOFMEMORY;
    }
    Amax = DblTemp;
    CumsumA = Amax + nA;

    sumKernel = (double)n; // The kernel is vector of vector of 1 of length == n
    conv_rectangle(A->data.Pr, nA, n, -1, Amax, CumsumA);

    /* Start from valid part */
    Amax += nh;

    minAmax = MinDblArray(Amax, nAmax, NULL);
    maxAmax = MaxDblArray(Amax, nAmax, &imax);

    threshold = (PeakDetectionRelThreshold * maxAmax + 
                (1. - PeakDetectionRelThreshold) * minAmax);

    threshold = max(threshold, ABOVENOISE_FACT * NoiseLevel * sumKernel);
    
    /* Scan the array and look for interval such that
       length >= iMinLength, if draw select interval with largest max(Amplitude) */
    bold = FALSE;
    Amax_int = -INF;
    Aselected = -INF;
    i1select = i2select = NOFOUND_INDEX; // -1
    for (i = 0; i <= nAmax; i++)
    {
        if (i < nAmax)
        {
            Ai = Amax[i];
            b = Ai >= threshold;
            if (Ai > Amax_int) Amax_int = Ai;
        }
        else
        {
            b = FALSE;
        }
        if (b && !bold) // up-cross threshold 
        {
            i1 = i;
        }
        else if (bold && !b) // down-cross threshold 
        {
            if (i - i1 >= iMinLength)
            {
                i2 = i;
                if (Amax_int > Aselected)
                {
                    Aselected = Amax_int;
                    i1select = i1;
                    i2select = i2;
                }
                Amax_int = -INF;
            }
            // else ignore the interval
        }
        bold = b;
    }

    if (i1select != NOFOUND_INDEX)
    {
        // transfer index of Amax to index of A
        i2select--;                                                 // -1 to remove the bias due to comparison
        ishift = (int)(ROUND(imax - 0.5 * (i1select + i2select)));  // we also take into account the max position
        if (ishift > nh)                                            // avoid overflowed
            ishift = nh;
        else if (ishift < -nh)
            ishift = -nh;
        ishift += nh;                                               // add nh due to truncation of Amax wrt A
        IndexInterval->istart = i1select + ishift;
        IndexInterval->istop  = i2select + ishift;
        IndexInterval->data = A->data.Pr + IndexInterval->istart;   // adjust data pointer to the start position
    }
    else
    {
        IndexInterval->istart = IndexInterval->istop = NOFOUND_INDEX;
        IndexInterval->data = NULL;
    }
    // Compute the length of the interval of data
    IndexInterval->n = IndexInterval->istop - IndexInterval->istart + 1;

    /* If caller doesn't provide memory then DblTemp was allocated above, then we free it */
    if (SelectAllWindowsOptions->tmp == NULL) FREE_VSI(DblTemp);

    return VSI_OK_FLAG;
}

/*
 Return PL = polyfit(idx,y,1)
 with didx := 0:length(y)-1;
 tmp is double workspace of length 2*n, where n is length of y
*/
ErrorFlagType LineFit(const IndexIntervalType *y, PLType *PL, double *tmp)
{
    int n;
    double a, P0, P1;
    double *t, *ycentered;

    n = y->n;
    a = 0.5 * (n - 1);

    if (tmp == NULL)
    {
        /* we need to find our own memory */
        t = MALLOC_VSI(sizeof(double) * (2 * n));
        if (t == NULL) return VSI_OUTOFMEMORY;
    }
    else t = tmp;

    ycentered = t + n;
    DblArrayLinspace(t, n, -a, a);

    P0 = DblArraySum(y->data, n);
    P0 = P0 / (double)n;
    DblArrayShift(y->data, ycentered, n, -P0);
    P1 = DblArrayDot(t, ycentered, n);
    P1 = P1 * 12.0 / (n * (n * n - 1));

    PL->PL1 = P1;
    PL->PL2 = P0 - P1 * a;

    if (t !=  tmp) FREE_VSI(t);
    return VSI_OK_FLAG;
}

// Evaluate the LineFit polynomial PL at the point didx := iquery-idx(1)
double LineVal(const PLType* PL, IndexIntervalType * IndexInterval, double iquery)
{
    double y, istart;
    istart = IndexInterval->istart;
    y = PL->PL1 * (iquery - istart) + PL->PL2;
    return y;
}

/*
 Allocate fields of structure used bu Detect peak (by Gaussian correlation method)
*/
ErrorFlagType AllocateDetectPeakWS(int nmax, DetectPeakWSType* DetectPeakWS)
{
    if (DetectPeakWS == NULL) return VSI_OK_FLAG;

    DetectPeakWS->nmax = 0;
    // factor 10 since 8 variables + tmp that takes 2*nmax
    DetectPeakWS->AllocateMemory = MALLOC_VSI(sizeof(double) * (10 * nmax));
    if (DetectPeakWS->AllocateMemory == NULL) return VSI_OUTOFMEMORY;

    DetectPeakWS->dx        = DetectPeakWS->AllocateMemory;
    DetectPeakWS->A2        = DetectPeakWS->dx      + nmax;
    DetectPeakWS->n1        = DetectPeakWS->A2      + nmax;
    DetectPeakWS->model     = DetectPeakWS->n1      + nmax;
    DetectPeakWS->model2    = DetectPeakWS->model   + nmax;
    DetectPeakWS->c         = DetectPeakWS->model2  + nmax;
    DetectPeakWS->n2        = DetectPeakWS->c       + nmax;
    DetectPeakWS->corr      = DetectPeakWS->n2      + nmax;
    DetectPeakWS->tmp       = DetectPeakWS->corr    + nmax;

    DetectPeakWS->nmax = nmax;
    DetectPeakWS->n = 0;

    return VSI_OK_FLAG;
}

/* Perform correlation between a signal amplitude (envelop) with a Gaussian function 
   Return the results correspond to the maximum correlation */
GaussianCorrOutputType GaussianCorr(double FWHMLambda, DetectPeakWSType * DetectPeakWS)
{
    double L, LambdaCenter, x, maxcorr, summodel2, cmax;
    const IndexIntervalType* Ai;
    int i, n, i_imax;
    double* model, *dx, *A;
    GaussianCorrOutputType out;

    LambdaCenter = DetectPeakWS->LambdaCenter;

    // Coherence length, fringe amplitude reduces by half at
    //      x = x0 + / -L
    //      0.23485931967491283 is 0.5 * sqrt(log(2) / (pi))
    L = 0.23485931967491283 * LambdaCenter * LambdaCenter / FWHMLambda;

    Ai = DetectPeakWS->Ai;
    n = Ai->n;

    // Compute a Gaussian model
    model = DetectPeakWS->model;
    dx = DetectPeakWS->dx;
    for (i = 0; i < n; i++)
    {
        x = dx[i] / L;
        x = -0.6931471805599453 * x * x;   // the constant is -log(2) = -0.6931471805599453
        model[i] = exp(x);
    }

    SqrDblArray(model, n, DetectPeakWS->model2);

    A = DetectPeakWS->Ai->data;

    Dblconv1(A, n, model, n, DetectPeakWS->c, NULL, SAME_SHAPE);

    conv_rectangle(DetectPeakWS->model2, n, n, 1, DetectPeakWS->n2, DetectPeakWS->tmp);

    // n1 is already computed by findGaussPeak
    // conv_rectangle(DetectPeakWS->A2, n, n, -1, DetectPeakWS->n1, DetectPeakWS->tmp);
  
    // corr = c.^2 ./ (n1.*n2);
    DblArrayMult(DetectPeakWS->n1, DetectPeakWS->n2, DetectPeakWS->tmp, n);
    SqrDblArray(DetectPeakWS->c, n, DetectPeakWS->corr);
    DblArrayDiv(DetectPeakWS->corr, DetectPeakWS->tmp, DetectPeakWS->corr, n);

    maxcorr = MaxDblArray(DetectPeakWS->corr, n, &i_imax);

    summodel2 = DblArraySum(DetectPeakWS->model2, n);
    out.Amax = DetectPeakWS->c[i_imax] / summodel2;

    // Find the max by fitting a polynomial
    //[imax, cmax] = FractionalMax(corr, imax);

    cmax = FractionalMaxDblArray(DetectPeakWS->corr, n, &out.imax);
    out.f = -cmax; // maximize correlation   

    return out;
}

/*
 Find the minimum of 1D function fun
 in the bracket (a,b) by golden search method
 Tol is rellative tolerance
 maxiter is the maximum number of iterations
*/
ErrorFlagType goldensearch(double a, double b, double Tol, int maxiter, 
    DetectPeakWSType* DetectPeakWS,
    const VSI_StackImages_opt_Type* options)
{
    double c, x0, x1, x2, x3, xopt;
    double C, R, f1, f2;
    int n;
    GaussianCorrOutputType out1, out2;

    c = 0.5 * (a + b);

    x0 = a;
    x3 = c;

    if (x0 == x3)
    {
        // No bracketing
        xopt = x0;
        out1 = GaussianCorr(x0, DetectPeakWS);

        DetectPeakWS->FWHMLambda    = xopt;
        DetectPeakWS->Amax          = out1.Amax;
        DetectPeakWS->iopt          = out1.imax;
        
    }
    else
    {
        R = (sqrt(5.0) - 1.0) / 2.0;
        C = 1 - R;

        n = 0;
        if (fabs(c - b) > fabs(b - a))
        {
            x1 = b;
            x2 = b + C * (c - b);
        }
        else
        {
            x2 = b;
            x1 = b - C * (b - a);
        }

        out1 = GaussianCorr(x1, DetectPeakWS);
        out2 = GaussianCorr(x2, DetectPeakWS);

        while (fabs(x3 - x0) > Tol * (fabs(x1) + fabs(x2)))
        {
            f1 = out1.f;
            f2 = out2.f;
            if (f2 < f1)
            {
                out1 = out2;
                x0 = x1;
                x1 = x2;
                x2 = R * x1 + C * x3;
                f1 = f2;
                out2 = GaussianCorr(x2, DetectPeakWS);
            }
            else
            {
                out2 = out1;
                x3 = x2;
                x2 = x1;
                x1 = R * x2 + C * x0;
                f2 = f1;
                out1 = GaussianCorr(x1, DetectPeakWS);
            }
            if (n >= maxiter)
                break;
            n++;
        }

        f1 = out1.f;
        f2 = out2.f;
        if (f1 < f2)
        {
            DetectPeakWS->FWHMLambda = x1;
            DetectPeakWS->Amax = out1.Amax;
            DetectPeakWS->iopt = out1.imax;
        }
        else
        {
            DetectPeakWS->FWHMLambda = x2;
            DetectPeakWS->Amax = out2.Amax;
            DetectPeakWS->iopt = out2.imax;
        }
    }
    return VSI_OK_FLAG;
}

/*
 Allocate fields of structure used bu Detect peak (by Gaussian correlation method)
*/
ErrorFlagType FreeDetectPeakWS(DetectPeakWSType* DetectPeakWS)
{
    if (DetectPeakWS == NULL) return VSI_OK_FLAG;
    if (DetectPeakWS->AllocateMemory && DetectPeakWS->nmax > 0)
        FREE_VSI(DetectPeakWS->AllocateMemory);
    DetectPeakWS->nmax = 0;
    DetectPeakWS->AllocateMemory = NULL;
    return VSI_OK_FLAG;
}

/*
* Find a peak of a Gaussian by correlation
* The width of the Gaussian can be also be estimated to optimize the correlation
*/
#define GOLDENSEARCh_TOL 1e-6
ErrorFlagType findGaussPeak(DetectPeakWSType* DetectPeakWS,
    const VSI_StackImages_opt_Type* options)
{
    const IndexIntervalType* Ai;
    double LoRelwidth, HiRelwidth, FWHMLambda, a, b, Tol;
    double *xvec, w, dxpeak, iopt;
    int ileft, maxiter, n;
    ErrorFlagType Flag;

    Ai = DetectPeakWS->Ai;
    n = Ai->n;

    LoRelwidth = options->LoRelwidth;
    HiRelwidth = options->HiRelwidth;
    FWHMLambda = DetectPeakWS->FWHMLambda;
    a = FWHMLambda * LoRelwidth;
    b = FWHMLambda * HiRelwidth;
    Tol = GOLDENSEARCh_TOL;

    if (LoRelwidth == HiRelwidth)
        maxiter = 1;
    else
        maxiter = options->maxiter;
   
    SqrDblArray(Ai->data, n, DetectPeakWS->A2);

    conv_rectangle(DetectPeakWS->A2, n, n, -1, DetectPeakWS->n1, DetectPeakWS->tmp);

    Flag = goldensearch(a, b, Tol, maxiter, DetectPeakWS, options);
    if (Flag < 0) return Flag;

    xvec = DetectPeakWS->dx;
    if (n > 1)
    {
        iopt = DetectPeakWS->iopt;
        ileft = (int)FLOOR(iopt);
        ileft = min(ileft, n - 2);
        w = iopt - ileft;
        dxpeak = (1 - w) * xvec[ileft] + w * xvec[ileft + 1];
    }
    else if (n == 1)
    {
        dxpeak = xvec[0];
    }
    else
    {
        dxpeak = 0;
    }

    DetectPeakWS->dxpeak = dxpeak;

    return VSI_OK_FLAG;
}

/*
ErrorFlagType DetectPeak(const Demodulation_Type* Demodulation,
    const VSI_StackImages_Setting_Type* Setting,
    const VSI_StackImages_opt_Type* options,
    int pixels[], int npixel,
    VSI_DblOutput_Type* VSIResultStruct);

Detect the position of the fringe envelop peaks of individual pixel(s)

INPUTS:
    pixels : array of integers : index of the pixel to be treated.
        The pixel numbers are as if they are the linear indexes of an image
        of size(ny x nx); see Demodulation input description.
    OR
        NULL: Peaks are detected for all pixels
    Demodulation: structure as output by FringeDemodulation() function, contains fields
        samplingstep : double scalar, resampling step
        xsampling : vector(1 x np), sampling vector, corresponds to the
        third(page) index of the input ImTable, started from 0.
        PeriodFrameLength : double scalar period of the(resampling) fringe,
            sample unit
        phi : 3D array(ny x nx x np) is the phase at the mid points
        A : 3D array is the amplitude at the mid points
        ImResampling : 3D array(ny x nx x np) is subsampled of ImTable at the mid points
    Setting : structure contains fields
        NoiseLevel : double scalar, typical sensor noise standard deviation in LSB
        RuleStep : double scalar, step between two OPDs, [m]
        NOTE : The step value must be adapted to Demodulation.ImResampling array.
        LambdaCenter : double scalar, wavelength of the light source, [m]
        FWHMLambda : double scalar, spectral bandwidth of the light source, [m]
    options : structure contains fields
        PeakMethod : PeakMethods enumerate among correlation, barycentric, polynomialfit;
            method detect the peak location.Default correlation
        UseFixLambda: scalar boolean, if TRUE use wavelength LambdaCenter
            from Setting; if FALSE use wavelength estimated from the phase
            PHI in Demodulation structure.Default[TRUE]
        nwin: integer scalar, number of samples used to smooth interferogram
            signal for various tasks.Make sure nwin < 0.5 * fringe period
            maxiter : integer scalar, maximum number of iterations of goldensearch
            of coherence length(width of the signal envelop)
        LoRelwidth, HiRelwidth : double scalars :
            (LoRelwidth, MaskThreshold) are the search bracket of the bandwidth
            they are relative to the nominal coherence length computed from
            the light source wavelength and bandwidth.Default[0.8] and [1.2].
        PeakDetectionRelThreshold : double scalar in(0, 1), threshold used
            to truncate fringe data for peak detection.The value is relative
            to the max amplitude.
            The smaller value the more data used for peak detection.Default 0.1.
        GraphicalMask : integer mask for graphic output
            0x20 phase residual
        VerboseLevel : scalar integer, level of verbose
            0 : none
            1 : error
            2 : warning
            3 : info
            default[0]
 OUTPUT :
     VSIStruct : structure contains fields all are double arrays same size
        (ny x nx) than pixels
        Topo_Image : position of the peak, in[m]
        Amplitude_Image : Amplitude of the peak, [LSB]
        Phase_Image : phase at the peak, [rad]
        Lambda_Image : destimated the central wavelength of the light source, [m]
            NOTE : Lambda_Image is contracted to scalar if UseFixLambda is TRUE.
        FWHMLambda_Image : estimated bandwidth of the FWHMLambda_Image source,
        FWHM convention, [m].
            NOTE : FWHMLambda_Image is contracted to scalar if LoRelwidth == HiRelwidth
    
    See also : FringeDemodulation
    */
ErrorFlagType DetectPeak(Demodulation_Type* Demodulation,
    const VSI_StackImages_Setting_Type* Setting,
    const VSI_StackImages_opt_Type* options,
    int pixels[], int npixels,
    VSI_DblOutput_Type* VSIResultStruct)
{
    ErrorFlagType Flag;
    SelectAllWindowsOptions_Type SelectAllWindowsOptions;
    double *phiTab, *ATable;
    NumArrayType A, phi;
    IndexIntervalType Ai, phase, phasec;
    int nx, ny, nxy, n, ImSize[2];
    int ij, px, nbadpixels;
    Int32 FilterNWin;
    double NoiseLevel, RuleStep, LambdaCenter, FWHMLambda, LambdaEstimated, FWHMLambda_Estimated;
    double Polarization, LoRelwidth, HiRelwidth, PeakDetectionRelThreshold;
    UInt32 GraphicalMask;
    VSI_BOOL UseFixLambda, UseEstLambda, UseVariateBandwidth;
    double PeriodFrameLength, HalfCentralPhaseWindow, PhaseStep;
    double phasepeak, TolLambda, HalfRuleStep, AmpThreshold, L, NSampleByCycle;
    double MinLength, maxAi;
    PLType PL, PLc;
    DetectPeakWSType DetectPeakWS;
    int istart, istop;
    double xmid, xpeak, xhalflgt, Amax, ipeak, LambdaAtPeak;
	double *PhaseData;

	/* Makesure the output is clean */
	if (VSIResultStruct != NULL)
		memset(VSIResultStruct, 0, sizeof(VSI_DblOutput_Type));

    nx = Demodulation->nx;
    ny = Demodulation->ny;
    n = Demodulation->nt;

    nxy = nx * ny;

    /* Both arrays will be freeed by VSI_StackImages (the caller) */
    if (Demodulation->phi_transposed == NULL)
    {
        Flag = MatrixTranspose(Demodulation->phi, nxy, n, &Demodulation->phi_transposed);
        /* No longer need it, we use the transposed from now on */
        FREE_VSI(Demodulation->phi); Demodulation->phi = NULL;
        if (Flag < 0) return Flag;
    }
    phiTab = Demodulation->phi_transposed;

    if (Demodulation->A_transposed == NULL)
    {
        Flag = MatrixTranspose(Demodulation->A, nxy, n, &Demodulation->A_transposed);
        /* No longer need it, we use the transposed from now on */
        FREE_VSI(Demodulation->A); Demodulation->A = NULL;
        if (Flag < 0) return Flag;
    }
    ATable = Demodulation->A_transposed; 

    Flag = AllocateDetectPeakWS(n, &DetectPeakWS);
    if (Flag < 0) return Flag;

    FilterNWin = options->nwin;

    NoiseLevel      = Setting->NoiseLevel;
    RuleStep        = Setting->RuleStep;
    LambdaCenter    = Setting->LambdaCenter;
    FWHMLambda      = Setting->FWHMLambda;

    UseFixLambda    = options->UseFixLambda;
    //VerboseLevel  = options->VerboseLevel);

    LoRelwidth      = options->LoRelwidth;
    HiRelwidth      = options->HiRelwidth;

    PeakDetectionRelThreshold = options->PeakDetectionRelThreshold;

    GraphicalMask = options->GraphicalMask;

	Polarization = RuleStep >= 0 ? 1.0 : -1.0;
	RuleStep = fabs(RuleStep);

    UseVariateBandwidth = LoRelwidth < HiRelwidth;
    UseEstLambda = !UseFixLambda;

    if (pixels == NULL)
    {
        ImSize[0] = nx;
        ImSize[1] = ny;
    }
    else
    {
        ImSize[0] = npixels;
        ImSize[1] = 1;
    }

    Flag = AllocateVSI_Output(VSIResultStruct, ImSize[0], ImSize[1]);
    if (Flag < 0)
    {
        FreeDetectPeakWS(&DetectPeakWS);
        return Flag;
    }

    nbadpixels = 0;

    PeriodFrameLength = Demodulation->PeriodFrameLength;
    // Window size use to estimate phase at the peak
    HalfCentralPhaseWindow = 0.5 * PeriodFrameLength;

    PhaseStep = 4 * PI * RuleStep; // 12.566370614359172 is 4 * pi
    // No need to explicitly Prepare a fix kernel FilterKernel, only FilterNWin are used in C-implementation
    // FilterKernel = 1 / FilterNWin + zeros(FilterNWin, 1);
    TolLambda = 0.125 * LambdaCenter;
    phasepeak = NAN; // backup value for bitand (GraphicalMask, 0x20)
    
    // Compute some constants that does not change during the loop
    HalfRuleStep = RuleStep * 0.5;
    AmpThreshold = 6.0 * NoiseLevel;

    // Coherence length
    L = 0.5 * sqrt(log(2) / (PI)) * LambdaCenter * LambdaCenter / FWHMLambda;
    NSampleByCycle = LambdaCenter / (2.0 * RuleStep);
    MinLength = ROUND(2 * NSampleByCycle * L / LambdaCenter); // half FWHM in term of number of samples

    SelectAllWindowsOptions.FilterNWin                  = FilterNWin;
    SelectAllWindowsOptions.FilterKernel                = NULL;
    SelectAllWindowsOptions.NoiseLevel                  = NoiseLevel;
    SelectAllWindowsOptions.MinLength                   = MinLength;
    SelectAllWindowsOptions.PeakDetectionRelThreshold   = PeakDetectionRelThreshold;
    SelectAllWindowsOptions.tmp                         = DetectPeakWS.tmp; // SelectAllWindows uses memory allocated in DetectPeakWS

    if (pixels == NULL)
        /* Peaks of all pixels are required to be detected */
        npixels = nxy;

    A.n = n;
    A.Type = VSI_REAL;
    // A.data is set inside the loop
    phi.n = n;
    phi.Type = VSI_REAL;
    // phi.data is set inside the loop
    
    // Loop of pixels ... LONG
    // TO DO : optimized this loop handling memory
    for (ij = 0; ij < npixels; ij++)
    {
        if (pixels != NULL) px = pixels[ij];
        else px = ij;

        /* Pointer to start of the amplitude of the given pixel */
        A.data.Pr = ATable + px * n;

        // roughly locate where is the interference, only 1 peak for Unity use
        // case      
        Flag = SelectAllWindows(&A, &SelectAllWindowsOptions, &Ai);
        if (Flag < 0)
        {
            FreeDetectPeakWS(&DetectPeakWS);
            return Flag;
        }

        if (Ai.istart != NOFOUND_INDEX)
        {
            /* Pointer to start of the phase of the given pixel */
            phi.data.Pr = phiTab + px * n;

            maxAi = MaxDblArray(Ai.data, Ai.n, NULL);
            // Extract amplitude and phase
            LambdaEstimated = LambdaCenter;
            if (UseEstLambda && (maxAi > AmpThreshold))
            {
                phase = Ai;
                phase.data = phi.data.Pr + phase.istart;
                Flag = LineFit(&phase, &PL, DetectPeakWS.tmp);
                if (Flag < 0)
                {
                    FreeDetectPeakWS(&DetectPeakWS);
                    return Flag;
                }
                LambdaEstimated = PhaseStep / PL.PL1;
            }
            else
            {
                PL.PL1 = PL.PL2 = NAN;
            }

            // Set the size of the window
            DetectPeakWS.n = Ai.n;
            // Find the peak position by correlation
            istart = Ai.istart;
            istop = Ai.istop;
            xmid = HalfRuleStep * (double)(istop + istart);
            xhalflgt = HalfRuleStep * (double)(istop - istart);
            DblArrayLinspace(DetectPeakWS.dx, DetectPeakWS.n, -xhalflgt, xhalflgt);

            DetectPeakWS.LambdaCenter = LambdaEstimated;
            DetectPeakWS.FWHMLambda = FWHMLambda;
            DetectPeakWS.Ai = &Ai;
            Flag = findGaussPeak(&DetectPeakWS, options);
            if (Flag < 0)
            {
                FreeDetectPeakWS(&DetectPeakWS);
                return Flag;
            }
            xpeak = DetectPeakWS.dxpeak + xmid;
            Amax = DetectPeakWS.Amax;
            FWHMLambda_Estimated = DetectPeakWS.FWHMLambda;
            if (isfinite(Amax))
            {
                // Fit the phase on small interval(one period) around the peak
                // because the phase of the white light is not linear on the
                // whole coherence length range
                ipeak = xpeak / RuleStep;

                phasec.istart = (int)max(ROUND(ipeak - HalfCentralPhaseWindow), 0);
                phasec.istop = (int)min(ROUND(ipeak + HalfCentralPhaseWindow), n-1);
                phasec.n = phasec.istop - phasec.istart + 1;
                phasec.data = phi.data.Pr + phasec.istart;
                if (phasec.n >= 2)
                {
                    Flag = LineFit(&phasec, &PLc, DetectPeakWS.tmp);
                    if (Flag < 0)
                    {
                        FreeDetectPeakWS(&DetectPeakWS);
                        return Flag;
                    }
                    phasepeak = LineVal(&PLc, &phasec, ipeak);
                    if (UseEstLambda)
                    {
                        // Usually smaller than Lambda estimated on larger window
                        LambdaAtPeak = PhaseStep / PLc.PL1;
                        // Return wavelength result as the mean of
                        //  (i)central wavelength, and
                        //  (ii)wavelength on the larger window
                        LambdaEstimated = 0.5 * (LambdaAtPeak + LambdaEstimated);
                        if (!(fabs(LambdaEstimated - LambdaCenter) <= TolLambda))
                        {
                            // Detect data that are not coherent in term of spatial
                            // oscillation spatial frequency
                            nbadpixels++;
                            continue;
                        }
                    }
                }
                else
                {
                    // fallback value
                    if (isnan(PL.PL1))
                    {
                        phase = Ai;
                        phase.data = phi.data.Pr + phase.istart;
                        Flag = LineFit(&phase, &PL, DetectPeakWS.tmp);
                        if (Flag < 0)
                        {
                            FreeDetectPeakWS(&DetectPeakWS);
                            return Flag;
                        }
                    }
                    phasepeak = LineVal(&PL, &Ai, ipeak);
                }

                // Success when we come here, store results in arrays
                VSIResultStruct->Topo_Image->Data[ij]         = Polarization*xpeak;
                VSIResultStruct->Amplitude_Image->Data[ij]    = Amax;
                VSIResultStruct->Phase_Image->Data[ij]        = phasepeak;
            }
        }
    }

    if (UseEstLambda)
        VSIResultStruct->Lambda = LambdaEstimated;
    else
        VSIResultStruct->Lambda = LambdaCenter;

    if (UseVariateBandwidth)
        VSIResultStruct->FWHMLambda = FWHMLambda_Estimated;
    else
        VSIResultStruct->FWHMLambda = FWHMLambda;

    // Standard the phase to be in [-pi,pi)
	PhaseData = VSIResultStruct->Phase_Image->Data;
    StandardizeAngle(PhaseData, npixels, PhaseData);

    FreeDetectPeakWS(&DetectPeakWS);

    return VSI_OK_FLAG;
} /* DetectPeak */


/* Correct the topo by smoothing the phase */
ErrorFlagType PhaseSmoothing(VSI_DblOutput_Type* VSIResultStruct,
    const VSI_StackImages_opt_Type* options)
{
    int phasewinsize, nx, ny, maxnxny, n, i;
    double Lambda, *Phi, *Topo_Image;
    double *buffer, *s, *c, *sm, *cm, *PhiSmooth, *dPhi, *dZ, *tmp;
    VSI_BOOL* notvalid;
#if DEBUG_ON_FILE == 1
    ErrorFlagType Flag;
#endif

    phasewinsize = options->phasewinsize;
    if (phasewinsize <= 1) return VSI_OK_FLAG;

    Lambda = VSIResultStruct->Lambda;

    nx  = VSIResultStruct->Phase_Image->nx;
    ny  = VSIResultStruct->Phase_Image->ny;
    Phi = VSIResultStruct->Phase_Image->Data;

    n = nx * ny;
    maxnxny = max(nx, ny);

    buffer = MALLOC_VSI(sizeof(double) * (7*n + maxnxny));
    if (buffer == NULL) return VSI_OUTOFMEMORY;

    s = buffer;
    c = s + n;
    sm = c + n;
    cm = sm + n;
    PhiSmooth = cm + n;
    dPhi = PhiSmooth + n;
    dZ = dPhi + n;
    tmp = dZ + n; // length maxnxny

    notvalid = MALLOC_VSI(sizeof(VSI_BOOL) * n);
    if (notvalid == NULL)
    {
        FREE_VSI(buffer);
        return VSI_OUTOFMEMORY; // Memory Leak (buffer)
    }

    for (i = 0; i < n; i++) notvalid[i] = !isfinite(Phi[i]);

    DblArrayCos(Phi, c, n);
    DblArraySin(Phi, s, n);

    for (i = 0; i < n; i++)
        if (notvalid[i]) s[i] = c[i] = 0;

    conv2_rectangle(s, nx, ny, phasewinsize, -1, sm, tmp);
    conv2_rectangle(c, nx, ny, phasewinsize, -1, cm, tmp);

    DblArrayAtan2(sm, cm, PhiSmooth, n);

    DblArrayMinus(Phi, PhiSmooth, dPhi, n);
    StandardizeAngle(dPhi, n, dPhi);

    for (i = 0; i < n; i++)
        if (notvalid[i]) dPhi[i] = 0;

    DblArrayScale(dPhi, dZ, n, Lambda / (4.0 * PI));

#if DEBUG_ON_FILE == 1
    // fnames = { 'Phi','c','s','sm','cm','PhiSmooth','dPhi','dZ' };
    // clear s;
    // for i = 1:length(fnames)
    //  im = load([fnames{ i } '.txt']);
    //  im = reshape(im, 512, []);
    //  s.(fnames{ i }) = im;
    // end
    DEBUG_STORE_RAW(Phi, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("Phi.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(s, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("s.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(c, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("c.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(sm, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("sm.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(cm, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("cm.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(PhiSmooth, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("PhiSmooth.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(dPhi, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("dPhi.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(dZ, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("dZ.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }
#endif

    Topo_Image = VSIResultStruct->Topo_Image->Data;
    DblArrayMinus(Topo_Image, dZ, Topo_Image, n);

    FREE_VSI(notvalid);
    FREE_VSI(buffer);

    return VSI_OK_FLAG;
} // PhaseSmoothing

/* Correct the topo by smoothing the phase */
ErrorFlagType PhaseSmoothing_RTI(VSI_DblOutput_Type* VSIResultStruct,
    const VSI_StackImages_opt_Type* options)
{
    int phasewinsize, nx, ny, maxnxny, n, i;
    double Lambda, * Phi, * Topo_Image;
    double* buffer, * s, * c, * sm, * cm, * tmp;
    VSI_BOOL* notvalid;
#if DEBUG_ON_FILE == 1
    ErrorFlagType Flag;
#endif

    phasewinsize = options->phasewinsize;
    if (phasewinsize <= 1) return VSI_OK_FLAG;

    Lambda = VSIResultStruct->Lambda;

    nx = VSIResultStruct->Phase_Image->nx;
    ny = VSIResultStruct->Phase_Image->ny;
    Phi = VSIResultStruct->Phase_Image->Data;

    n = nx * ny;
    maxnxny = max(nx, ny);

    buffer = MALLOC_VSI(sizeof(double) * (4 * n + maxnxny));
    if (buffer == NULL) return VSI_OUTOFMEMORY;

    s = buffer;
    c = s + n;
    sm = c + n;
    cm = sm + n;
    tmp = cm + n; // length maxnxny

    notvalid = MALLOC_VSI(sizeof(VSI_BOOL) * n);
    if (notvalid == NULL)
    {
        FREE_VSI(buffer);
        return VSI_OUTOFMEMORY; // Memory Leak (buffer)
    }

#pragma omp parallel for schedule(static, 32) firstprivate(Phi, s, c, notvalid)
    for (i = 0; i < n; i++)
    {
        notvalid[i] = !isfinite(Phi[i]);
        if (notvalid[i])
            s[i] = c[i] = 0;
        else
        {
            s[i] = sin(Phi[i]);
            c[i] = cos(Phi[i]);
        }
    }

    conv2_rectangle(s, nx, ny, phasewinsize, -1, sm, tmp);
    conv2_rectangle(c, nx, ny, phasewinsize, -1, cm, tmp);


#if DEBUG_ON_FILE == 1
    // fnames = { 'Phi','c','s','sm','cm','PhiSmooth','dPhi','dZ' };
    // clear s;
    // for i = 1:length(fnames)
    //  im = load([fnames{ i } '.txt']);
    //  im = reshape(im, 512, []);
    //  s.(fnames{ i }) = im;
    // end
    DEBUG_STORE_RAW(Phi, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("Phi.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(s, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("s.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(c, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("c.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(sm, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("sm.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }

    DEBUG_STORE_RAW(cm, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("cm.txt");
    if (Flag < 0)
    {
        FREE_VSI(notvalid);
        FREE_VSI(buffer);
        return Flag;
    }
#endif

    Topo_Image = VSIResultStruct->Topo_Image->Data;
    double lambda4Pi = Lambda / (4.0 * PI);
#pragma omp parallel for schedule(static, 32) firstprivate(lambda4Pi, Topo_Image, Phi, sm, cm, notvalid)
    for (i = 0; i < n; i++)
    {
        if ( ! notvalid[i])
        {
            Topo_Image[i] -= lambda4Pi * ToStandardAngle(Phi[i] - atan2(sm[i], cm[i]));
        }
    }

    FREE_VSI(notvalid);
    FREE_VSI(buffer);

    return VSI_OK_FLAG;
} // PhaseSmoothing_RTI

/*
 Filter out low-order mode (or Correct the slope) of the topo image Z

 INPUTS:
   Z: double matrix of topo to be corrected
   options, with fields
       SlopeCorrectionMethod: char array, among {'none', 'poly', 'median'}
           method to compute the slope to be removed. Default ['median']
       PolyFilterFitOrder: double scalar, polynomial order of the low-order
           mode. Only applicable for SlopeCorrectionMethod == 'poly'. Default [1]

 See also: VSI_StackImages, RobustZPoly
*/
ErrorFlagType PolyHighPassImageFilter(VSI_DblOutput_Type* VSIResultStruct,
    const VSI_StackImages_opt_Type* options)
{
    double x, y, Gx, Gy, Z0;
    double *Z, *Zflatten;
    double *buffer, *dZ;
    double *Zp, * Zm;
    int i, j, nx, ny, n;
    ErrorFlagType Flag;

    Z = VSIResultStruct->Topo_Image->Data;
    Zflatten = VSIResultStruct->TopoFlatten_Image->Data;

    nx = VSIResultStruct->Phase_Image->nx;
    ny = VSIResultStruct->Phase_Image->ny;

    switch (options->SlopeCorrectionMethod)
    {
    case VSI_SLOPECORRECTIONMETHOD_NONE:

        /* Duplicate Topo_Image to TopoFlatten_Image */
        Copy_DoubleArray(Zflatten, Z, nx * ny);
        break;

    case VSI_SLOPECORRECTIONMETHOD_MEDIAN:

        n = nx * ny;
        buffer = MALLOC_VSI(sizeof(double) * n);
        if (buffer == NULL) return VSI_OUTOFMEMORY;

        /* Compute vertical finite difference and its median */
        dZ = buffer;
        for (j = 0; j < ny; j++)
        {
            Zm = Z + nx * j;
            Zp = Zm + 1;
            for (i = 0; i < nx - 1; i++) *dZ++ = *Zp++ - *Zm++;
        }
        Flag = MedianDblArray(buffer, (nx - 1) * ny, &Gx);
        if (Flag < 0)
        {
            FREE_VSI(buffer);
            return Flag; 
        }

        /* Compute horizontal finite difference and its median */
        dZ = buffer;
        for (j = 0; j < ny-1; j++)
        {
            Zm = Z + nx * j;
            Zp = Zm + nx;
            for (i = 0; i < nx; i++) *dZ++ = *Zp++ - *Zm++;
        }
        Flag = MedianDblArray(buffer, nx * (ny - 1), &Gy);
        if (Flag < 0)
        {
            FREE_VSI(buffer);
            return Flag;
        }
        Flag = MedianDblArray(Z, nx * ny, &Z0);
        if (Flag < 0)
        {
            FREE_VSI(buffer);
            return Flag;
        }

        /* Correct the slope */
        y = -0.5 * (ny - 1);
        for (j = 0; j < ny; j++)
        {
            x = -0.5 * (nx - 1);
            for (i = 0; i < nx; i++)
            {
                *Zflatten++ = *Z++ - (Z0 + Gx * x + Gy * y);
                x++;
            }
            y++;
        }

        FREE_VSI(buffer);

        break;

    case VSI_SLOPECORRECTIONMETHOD_POLY:
        
        return VSI_NOTYET_IMPLEMENTED;

    default:
        
        return VSI_NOTYET_IMPLEMENTED;
    }
    return VSI_OK_FLAG;
} // PolyHighPassImageFilter


// mask bad data of Topo_Image and TopoFlatten (weak amplitude)
// i.e., replace them with NaN value
ErrorFlagType MaskBadPixels(VSI_DblOutput_Type* VSIResultStruct,
    const VSI_StackImages_Setting_Type* Setting,
    const VSI_StackImages_opt_Type* options)
{
    ErrorFlagType Flag;
    int dimA[2];
    int i, n, AmplitudeCmpWindow;
    double MaskThreshold, NoiseLevel;
    DOUBLE_RM_Image* Amplitude_Image;
    double *Amplitude, *Aexpected;
    double *Topo, *TopoFlatten;

    /* By convention we don't mask if MaskThreshold is not in (0,1) */
    MaskThreshold = options->MaskThreshold;
    if (MaskThreshold <= 0.0 || MaskThreshold >= 1.0)
        return VSI_OK_FLAG;

    /* By convention we don't mask if AmplitudeCmpWindow is not in above 1 */
    AmplitudeCmpWindow = options->AmplitudeCmpWindow;
    if (AmplitudeCmpWindow <= 1)
            return VSI_OK_FLAG;

    /* Apply max filter to find the expected amplitude */
    Amplitude_Image = VSIResultStruct->Amplitude_Image;
    dimA[0] = Amplitude_Image->nx;
    dimA[1] = Amplitude_Image->ny;
    Amplitude = Amplitude_Image->Data;
    Flag = maxfilter(Amplitude, dimA, AmplitudeCmpWindow, SAME_SHAPE,
        &Aexpected, dimA);
    if (Flag < 0)
    {
        FREE_VSI(Aexpected);
        return Flag;
    }

    // Aexpected = MaskThreshold * (Aexpected + NoiseLevel)
    n = dimA[0] * dimA[1]; // nx*ny
    NoiseLevel = Setting->NoiseLevel;
    DblArrayShift(Aexpected, Aexpected, n, NoiseLevel);
    DblArrayScale(Aexpected, Aexpected, n, MaskThreshold);

    /* Detect and set the invalid pixels topo values with NaN */
    Topo = VSIResultStruct->Topo_Image->Data;
    TopoFlatten = VSIResultStruct->TopoFlatten_Image->Data;
    for (i = 0; i < n; i++)
        if (Amplitude[i] < Aexpected[i])
            Topo[i] = TopoFlatten[i] = NAN;

#if DEBUG_ON_FILE == 1
    DEBUG_STORE_RAW(Aexpected, n, VSI_REAL);
    Flag = DEBUG_SAVE2FILE("Aexpected.txt");
    if (Flag < 0) return Flag;
#endif

    FREE_VSI(Aexpected);

    return VSI_OK_FLAG;
}

ErrorFlagType MaskBadPixels_RTI(VSI_DblOutput_Type* VSIResultStruct,
    const VSI_StackImages_Setting_Type* Setting,
    const VSI_StackImages_opt_Type* options)
{
    ErrorFlagType Flag;
    int dimA[2];
    int i, n, AmplitudeCmpWindow;
    double MaskThreshold, NoiseLevel;
    DOUBLE_RM_Image* Amplitude_Image;
    double* Amplitude, * Aexpected;
    double* Topo, * TopoFlatten;

    /* By convention we don't mask if MaskThreshold is not in (0,1) */
    MaskThreshold = options->MaskThreshold;
    if (MaskThreshold <= 0.0 || MaskThreshold >= 1.0)
        return VSI_OK_FLAG;

    /* By convention we don't mask if AmplitudeCmpWindow is not in above 1 */
    AmplitudeCmpWindow = options->AmplitudeCmpWindow;
    if (AmplitudeCmpWindow <= 1)
        return VSI_OK_FLAG;

    /* Apply max filter to find the expected amplitude */
    Amplitude_Image = VSIResultStruct->Amplitude_Image;
    dimA[0] = Amplitude_Image->nx;
    dimA[1] = Amplitude_Image->ny;
    Amplitude = Amplitude_Image->Data;
    Flag = maxfilter(Amplitude, dimA, AmplitudeCmpWindow, SAME_SHAPE, &Aexpected, dimA);
    if (Flag < 0)
    {
        FREE_VSI(Aexpected);
        return Flag;
    }

    // Aexpected = MaskThreshold * (Aexpected + NoiseLevel)
    n = dimA[0] * dimA[1]; // nx*ny
    NoiseLevel = Setting->NoiseLevel;

    /* Detect and set the invalid pixels topo values with NaN */
    Topo = VSIResultStruct->Topo_Image->Data;
    TopoFlatten = VSIResultStruct->TopoFlatten_Image->Data;

#pragma omp parallel for schedule(static, 32) firstprivate(Topo, TopoFlatten, Aexpected, NoiseLevel, MaskThreshold, Amplitude)
    for (i = 0; i < n; i++)
    {
        Aexpected[i] = (Aexpected[i] + NoiseLevel) * MaskThreshold;
        if (Amplitude[i] < Aexpected[i])
            Topo[i] = TopoFlatten[i] = NAN;
    }

    FREE_VSI(Aexpected);

    return VSI_OK_FLAG;
}


ErrorFlagType CastVSIResultToSingle(const VSI_StackImages_opt_Type* options,
                                    VSI_DblOutput_Type* VSIResultStruct)
{
    VSI_Output_Type* VSIResult;
    DOUBLE_RM_Image **SrcImageAddr;
    DOUBLE_RM_Image* DummyImage = NULL;
    DOUBLE_RM_Image* DblImage;
    SINGLE_RM_Image* SglImage;
    Float64* DblData;
    Float32* SglData;
    int nxy, i, j;
    UInt32 OutputSelectionMask;

    if (VSIResultStruct == NULL) return VSI_OK_FLAG;

    /* NOTE: VSIResultStruct and VSIResult point to the same memory location */
    VSIResult = (VSI_Output_Type*)VSIResultStruct;
    OutputSelectionMask = options->OutputSelectionMask;

    double unitConvert = 1.0; // conserve SI unit
    for (i = 0; i < NB_DOUBLE_RM_Image; i++)
    {
        switch (i)
        {
        case 0:
            SrcImageAddr = &VSIResultStruct->Topo_Image;
            unitConvert = 10e5; // mm to µm
            break;
        case 1:
            SrcImageAddr = &VSIResultStruct->TopoFlatten_Image;
            unitConvert = 10e5; // mm to µm
            break;
        case 2:
            SrcImageAddr = &VSIResultStruct->Amplitude_Image;
            break;
        case 3:
            SrcImageAddr = &VSIResultStruct->Phase_Image;
            break;
        default:
            // Some compiler requires this branch (which logically never run)
            SrcImageAddr = &DummyImage;
        }
        DblImage = *SrcImageAddr;
        SglImage = (SINGLE_RM_Image*)DblImage;
		if (DblImage != NULL)
		{
			if (OutputSelectionMask & (0x01 << i)) // Check the mask if the field is wanted by caller
			{
                /* Cast double array to single array */
				DblData = DblImage->Data;
				SglData = SglImage->Data;
				nxy = DblImage->nx * DblImage->ny;

                // WARNING cannot use parallell loop since DblData & SglData point to the same memory area
                // unroll for a small optim 
                j = 0;
                int unrollstep = 2;
                int NroundDown = ((nxy) & ~((unrollstep)-1)); //will contain the value of Nxy rounded down to a multiple of stepsize.
                for (; j < NroundDown; j += unrollstep)
                {
                    // Make as many ops as unroll step
                    SglData[j] = (Float32)(unitConvert * DblData[j]);
                    SglData[j + 1] = (Float32)(unitConvert * DblData[j + 1]);
                }
                for (; j < nxy; j++)
                    SglData[j] = (Float32)(unitConvert * DblData[j]);

                /* Skrink the array by half */
                SglImage->Data = REALLOC_VSI(SglImage->Data, nxy * sizeof(Float32));
			}
            else
			{
                /* Free the field at set to NULL */
                DblData = DblImage->Data;
				if (DblData != NULL) FREE_VSI(DblData);
                DblImage->Data = NULL;
				FREE_VSI(DblImage);
                *SrcImageAddr = NULL;
			}
		}
    }
    return VSI_OK_FLAG;
} /* CastVSIResultToSingle*/


//*******************************************************************************
// Perform VSI algorithm on the stack of images
//*******************************************************************************

/*
 Perform VSI algorithm on the stack of images read from a folder
 INPUTS:
   DataPath: char array, folder where the 8-bit BMP images are stored
   Setting: structure contains fields
       RuleStep: double scalar, step between two OPDs, [m],
           Default [10.0e-9]
       LambdaCenter: double scalar, wavelength of the light source, [m],
           Default [630e-9]
       FWHMLambda: double scalar, spectral bandwidth of the light source,
           [m], Default [1300e-09]
       NoiseLevel: double scalar, typical sensor noise standard deviation, 
           [LSB], Default [2]
   options: structure contains fields
       PhaseMethod: DemodulationMethod enumerate Larkin, Hariharan, method
           to estimate the phase. Default Hariharan.
       AmplitudeMethod: DemodulationMethod enumerate Larkin, Hariharan, method
           to estimate the amplitude. Default Larkin.
       ResamplingMethod: char array among {'none', 'linear', cubic'},
           interpolation method used for resampling fringe so that 5-step
           demodulation methods with usage of integer index is exact.
           If set to 'none' resampling is disabled. Default 'linear'.
       FreqMethod: FrequencyMethod enumerate fft or x0
           Method used by DetectFrequency() to detect spatial frequency nu,
           default 'fft'
       WindowType: char array among {'gaussian', 'tukey'}
           Only applicable for 'fft' method. WindowType used for windowing
           the signal before FFT. Default 'tukey'
       Ewidth: double scalar, sample unit
           Only applicable for 'fft' method and WindowType 'gaussian'
           The width of the Gaussian window function. Default [1.0]
       SpectrumSkewness: boolean scalar, Only applicable for FFT method.
           If TRUE adjust the light central wavelength according to the
           skewness of the spectrum, otherwise use the MODE of the spectrum
           as nominal spatial frequency. Default [TRUE].
       nwin: integer scalar, number of samples used to smooth interferogram
           signal for various tasks. Make sure nwin < 0.5 * fringe period.
           Default [7].
       PeakMethod: PeakMethods enumerate among correlation, barycentric, polynomialfit;
           method detect the peak location. Default correlation
       maxiter: integer scalar, maximum number of iterations of goldensearch
           of coherence length (width of the signal envelop), Default [16].
       LoRelwidth, HiRelwidth: double scalars:
           are the search bracket of the bandwidth
           they are normalized value relative to the nominal coherence length
           computed from the light source wavelength and bandwidth.
           Default [0.8] and [1.2].
       PeakDetectionRelThreshold: double scalar in (0,1), threshold used
           to truncate fringe data for peak detection. The value is relative
           to the max amplitude.
           The smaller value the more data used for peak detection. Default 0.1.
       MaskThreshold: double scalar, relative threshold used to detect
           invalid pixels, due to strong diffraction from topo step or
           dusts. Default [0.6].
       AmplitudeCmpWindow: integer scalar, size of the window use to compare
           amplitude for masking, pixel unit. Set to 0 or 1 to disable
           masking. Default [13].
       phasewinsize: integer scalar, window size used by PhaseSmoothing,
           set to 0 or 1 to disable
           to 3 for mild smoothing
           to 5 for recommended value; this is the default
       SlopeCorrectionMethod: char array, among {'none', 'poly', 'median'}
           method to compute the slope to be removed. Default 'median'
       PolyFilterFitOrder: double scalar, polynomial order of the low-order
           mode. Only applicable for SlopeCorrectionMethod == 'poly'. Default [1]
       DebugC: scalar boolean, if TRUE call Test_VSI_StackImages function.
           Default [FALSE].
       VerboseLevel: scalar integer, level of verbose
           0: none
           1: error
           2: warning
           3: info     
           default [0]
 OUTPUT:
   VSIResultStruct: structure contains fields
     Topo_Image: double array (ny x nx), position of the peak, in  [m]
     TopoFlatten_Image: double array (ny x nx), topo with slope corrected, in  [m]
     Amplitude_Image: double array (ny x nx), Amplitude of the peak, [LSB]
     Phase_Image: double array (ny x nx), phase at the peak, [rad]
     NOTE: Mask: values of non reliable pixels with weak amplitude in Topo_Image and
        TopoFlatten_Image are replaced by NaN
     Lambda: double scalar, estimated the central wavelength of the
       light source, [m]
     FWHMLambda: double scalar, estimated bandwidth of the
       source, FWHM convention, [m].

*/
VSI_API(ErrorFlagType) VSI_StackImages(const VSI_StackImages_Type* Images,	// Image stack
	const VSI_StackImages_Setting_Type* Setting,                            // Input, acquisition setting
	const VSI_StackImages_opt_Type* options,                                // Input, processing options
	VSI_Output_Type* VSIResult)                                       // Output, results
{
    VSI_StackImages_Setting_Type l_Setting;
    VSI_StackImages_opt_Type l_options;
	ImageStack_Type ImTable;
	Demodulation_Type Demodulation;
	ErrorFlagType Flag;
    double fmax;
    int nxy, npixels, idxmax, px;
    VSI_DblOutput_Type *VSIResultStruct, Peak;
	VSI_BOOL IsSaturated;

    VSIResultStruct = (VSI_DblOutput_Type*)VSIResult;

	memset(VSIResultStruct, 0, sizeof(VSI_DblOutput_Type));

    /* ImTable.Data will be allocated here */
    VSIResultStruct->Flag = Flag = VSI_GetStackImages(Images, &ImTable);
    if (Flag < 0)
    {
        FreeImageStack(&ImTable);
        return Flag;
    }

    l_Setting = *Setting;
    l_options = *options;
    VSIResultStruct->Flag = Flag = FringeDemodulation(&ImTable, &l_Setting, &l_options, &Demodulation);

	/* No longer need it */
	FreeImageStack(&ImTable);

	/* Idem, allocated by ResamplingSignalFor5Step */
	FREE_VSI(Demodulation.ImResampling);
	Demodulation.ImResampling = NULL;

	/* Check FringeDemodulation output Flag */
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }

	/* Keep the state of saturation */
    IsSaturated = Flag == SATURATED_INTEROGRAM; // ce flag est ecrasé dans FringeDemodulation

    // Adjust the step with new resampling data
    l_Setting.RuleStep = l_Setting.RuleStep * Demodulation.samplingstep;

    // Select the "reference" interferogram as strongest signal amplitude
    nxy = Demodulation.nx * Demodulation.ny;
    npixels = Demodulation.nt * nxy;
    fmax = MaxDblArray(Demodulation.A, npixels, &idxmax);
    px = idxmax % nxy;

    // Re-estimate LambdaCenter and FWHMLambda from the reference fringe
    l_options.PeakMethod = VSI_PEAKMETHOD_CORRELATION; // only this method can give meaningful FWHMLambda
    l_options.UseFixLambda = FALSE;
    l_options.Verbose_Level = VSI_VERBOSE_NONE;
    VSIResultStruct->Flag = Flag = DetectPeak(&Demodulation, &l_Setting, &l_options, &px, 1, &Peak);
	if (Flag < 0)
	{
		FreeVSI_Output(&Peak);
		FreeDemodulation(&Demodulation);
		return Flag;
	}

    if (isfinite(Peak.Lambda))
    {
        l_Setting.LambdaCenter = Peak.Lambda;
        if (isfinite(Peak.FWHMLambda))
            l_Setting.FWHMLambda = Peak.FWHMLambda;
    }

    // Detect peaks for all pixels, long
    // Disable search for bandwidth, assuming the bandwidth estimated with reference
    // fringe is appropriate
    l_options = *options;
    l_options.LoRelwidth = 1;
    l_options.HiRelwidth = 1;
    l_options.UseFixLambda = TRUE;
    VSIResultStruct->Flag = Flag = DetectPeak(&Demodulation, &l_Setting, &l_options,
		NULL, 0, VSIResultStruct);
	if (Flag < 0)
	{
		FreeVSI_Output(&Peak);
		FreeDemodulation(&Demodulation);
		return Flag;
	}

    // Correct topo error from the phase map, it is somewhat a filtering
    // but less intrusive in case of topo has large step
    VSIResultStruct->Flag = Flag = PhaseSmoothing(VSIResultStruct, options);
    if (Flag < 0)
	{
		FreeVSI_Output(&Peak);
		FreeDemodulation(&Demodulation);
		return Flag;
	}
    // Correct the slope of the topo to make it flat
    VSIResultStruct->Flag = Flag = PolyHighPassImageFilter(VSIResultStruct, options);
    if (Flag < 0)
	{
		FreeVSI_Output(&Peak);
		FreeDemodulation(&Demodulation);
		return Flag;
	}

    // mask bad data (based on amplitude strength)
    VSIResultStruct->Flag = Flag = MaskBadPixels(VSIResultStruct, Setting, options);
    if (Flag < 0)
	{
		FreeVSI_Output(&Peak);
		FreeDemodulation(&Demodulation);
		return Flag;
	}

#if DEBUG_ON_FILE == 1
    DEBUG_STORE_IMAGE(VSIResultStruct->TopoFlatten_Image);
    Flag = DEBUG_SAVE2FILE("TopoFlatten_Image.txt");
    if (Flag < 0)
	{
		FreeVSI_Output(&Peak);
		FreeDemodulation(&Demodulation);
		return Flag;
	}

    DEBUG_STORE_IMAGE(VSIResultStruct->Amplitude_Image);
    Flag = DEBUG_SAVE2FILE("Amplitude_Image.txt");
    if (Flag < 0)
	{
		FreeVSI_Output(&Peak);
		FreeDemodulation(&Demodulation);
		return Flag;
	}

    DEBUG_STORE_IMAGE(VSIResultStruct->Phase_Image);
    Flag = DEBUG_SAVE2FILE("Phase_Image.txt");
    if (Flag < 0)
	{
		FreeVSI_Output(&Peak);
		FreeDemodulation(&Demodulation);
		return Flag;
	}
#endif


    /* NOTE: VSIResultStruct and VSIResult point to the same memory location */
    CastVSIResultToSingle(options, VSIResultStruct);

	/* Enventually restore the state of saturation for information of the caller */
	if (IsSaturated) VSIResultStruct->Flag = SATURATED_INTEROGRAM;

	/* Free the fields inside Demodulations */
	FreeDemodulation(&Demodulation);

	FreeVSI_Output(&Peak);

	return VSI_OK_FLAG;
}

VSI_API(ErrorFlagType) VSI_RTI_StackImages(const VSI_StackImages_Type* Images,	// Image stack
    const VSI_StackImages_Setting_Type* Setting,                            // Input, acquisition setting
    const VSI_StackImages_opt_Type* options,                                // Input, processing options
    VSI_Output_Type* VSIResult)                                       // Output, results
{
    VSI_StackImages_Setting_Type l_Setting;
    VSI_StackImages_opt_Type l_options;
    Demodulation_Type Demodulation;
    ErrorFlagType Flag;
    double fmax;
    int nxy, npixels, idxmax, px;
    VSI_DblOutput_Type* VSIResultStruct, Peak;
    VSI_BOOL IsSaturated;

    if (Images->NumberOfImages < 5)
        return VSI_NOT_ENOUGHIMAGES;

    VSIResultStruct = (VSI_DblOutput_Type*)VSIResult;

    memset(VSIResultStruct, 0, sizeof(VSI_DblOutput_Type));

    l_Setting = *Setting;
    l_options = *options;
    VSIResultStruct->Flag = Flag = FringeDemodulation_RTI(Images, &l_Setting, &l_options, &Demodulation);

    /* Idem, allocated by ResamplingSignalFor5Step */
    FREE_VSI(Demodulation.ImResampling);
    Demodulation.ImResampling = NULL;

    /* Check FringeDemodulation output Flag */
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }

    /* Keep the state of saturation */
    IsSaturated = (Flag == SATURATED_INTEROGRAM); // ce flag est ecrasé dans FringeDemodulation

    // Adjust the step with new resampling data
    l_Setting.RuleStep = l_Setting.RuleStep * Demodulation.samplingstep;

    // Select the "reference" interferogram as strongest signal amplitude
    nxy = Demodulation.nx * Demodulation.ny;
    npixels = Demodulation.nt * nxy;
    fmax = MaxDblArray(Demodulation.A, npixels, &idxmax);
    px = idxmax % nxy;

    // Re-estimate LambdaCenter and FWHMLambda from the reference fringe
    l_options.PeakMethod = VSI_PEAKMETHOD_CORRELATION; // only this method can give meaningful FWHMLambda
    l_options.UseFixLambda = FALSE;
    l_options.Verbose_Level = VSI_VERBOSE_NONE;
    VSIResultStruct->Flag = Flag = DetectPeak(&Demodulation, &l_Setting, &l_options, &px, 1, &Peak);
    if (Flag < 0)
    {
        FreeVSI_Output(&Peak);
        FreeDemodulation(&Demodulation);
        return Flag;
    }

    if (isfinite(Peak.Lambda))
    {
        l_Setting.LambdaCenter = Peak.Lambda;
        if (isfinite(Peak.FWHMLambda))
            l_Setting.FWHMLambda = Peak.FWHMLambda;
    }
    // Peak is no longer needed
    FreeVSI_Output(&Peak);

    // Detect peaks for all pixels, long
    // Disable search for bandwidth, assuming the bandwidth estimated with reference
    // fringe is appropriate
    l_options = *options;
    l_options.LoRelwidth = 1;
    l_options.HiRelwidth = 1;
    l_options.UseFixLambda = TRUE;
    VSIResultStruct->Flag = Flag = DetectPeak(&Demodulation, &l_Setting, &l_options, NULL, 0, VSIResultStruct);
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }

    // Correct topo error from the phase map, it is somewhat a filtering
    // but less intrusive in case of topo has large step
    VSIResultStruct->Flag = Flag = PhaseSmoothing_RTI(VSIResultStruct, options);
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }
    // Correct the slope of the topo to make it flat
    VSIResultStruct->Flag = Flag = PolyHighPassImageFilter(VSIResultStruct, options);
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }

    // mask bad data (based on amplitude strength)
    VSIResultStruct->Flag = Flag = MaskBadPixels_RTI(VSIResultStruct, Setting, options);
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }

#if DEBUG_ON_FILE == 1
    DEBUG_STORE_IMAGE(VSIResultStruct->TopoFlatten_Image);
    Flag = DEBUG_SAVE2FILE("TopoFlatten_Image.txt");
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }

    DEBUG_STORE_IMAGE(VSIResultStruct->Amplitude_Image);
    Flag = DEBUG_SAVE2FILE("Amplitude_Image.txt");
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }

    DEBUG_STORE_IMAGE(VSIResultStruct->Phase_Image);
    Flag = DEBUG_SAVE2FILE("Phase_Image.txt");
    if (Flag < 0)
    {
        FreeDemodulation(&Demodulation);
        return Flag;
    }
#endif

    /* NOTE: VSIResultStruct and VSIResult point to the same memory location */
    CastVSIResultToSingle(options, VSIResultStruct);

    /* Enventually restore the state of saturation for information of the caller */
    if (IsSaturated) VSIResultStruct->Flag = SATURATED_INTEROGRAM;

    /* Free the fields inside Demodulations */
    FreeDemodulation(&Demodulation);

    FreeVSI_Output(&Peak);

    return VSI_OK_FLAG;
}

//*******************************************************************************
// release memory occupied by VSIResultStruct
//*******************************************************************************
VSI_API(ErrorFlagType) VSI_FreeResultStruct(VSI_Output_Type* VSIResult)
{
    ErrorFlagType Flag;
    Flag = FreeVSI_Output((VSI_DblOutput_Type*)VSIResult);
	return Flag;
}
