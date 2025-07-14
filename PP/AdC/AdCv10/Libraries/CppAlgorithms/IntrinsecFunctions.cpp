// for SSE2 instruction
#include <emmintrin.h>

static inline int RoundFlt(float value)
{
	__m128 t = _mm_set_ss(value);
	return _mm_cvtss_si32(t);
}

template<typename _Tp> static inline _Tp saturateCAST(int v) { return _Tp(v); }
template<typename _Tp> static inline _Tp saturateCAST(float v) { return _Tp(v); }

template<> inline unsigned char saturateCAST<unsigned char>(int v) { return (unsigned char)((unsigned)v <= 0xff ? v : v > 0 ? 0xff : 0); }
template<> inline unsigned char saturateCAST<unsigned char>(float v) { int iv = RoundFlt(v); return saturateCAST<unsigned char>(iv); }


int convert8uScale_Row(const unsigned char* pSrc, unsigned char* pDst, int p_Width, float p_alpha, float p_beta)
{
	int x = 0;
	__m128 v_scale(_mm_set1_ps((float)p_alpha)); // 4 x Float32bits
	__m128 v_shift(_mm_set1_ps((float)p_beta));  // 4 x Float32bits
	int cWidth = 8;//v_uint16x8::nlanes;
	for (; x <= p_Width - cWidth; x += cWidth)
	{
		// load buffer expand in 8 x uint16 
		__m128i tmpvarZ0(_mm_setzero_si128());
		__m128i loadexpand = _mm_unpacklo_epi8(_mm_loadl_epi64((const __m128i*)(pSrc + x)), tmpvarZ0);
		// expand in4 x uint32 then in 4 x Float32 to calculate alpha * x + beta (convert signed int 32 in float)
		__m128	v_src1 = _mm_add_ps(v_shift, _mm_mul_ps(v_scale, _mm_cvtepi32_ps(_mm_unpacklo_epi16(loadexpand, tmpvarZ0)))); //v_float32x4 + v_float32x4 * v_float32x4
		__m128	v_src2 = _mm_add_ps(v_shift, _mm_mul_ps(v_scale, _mm_cvtepi32_ps(_mm_unpackhi_epi16(loadexpand, tmpvarZ0)))); //v_float32x4 + v_float32x4 * v_float32x4
		// pack after rounding float to nearest int)																				  //v_int16x8 v_dst = v_pack(v_round(v_src1), v_round(v_src2));
		__m128i v_dst = _mm_packs_epi32(_mm_cvtps_epi32(v_src1), _mm_cvtps_epi32(v_src2));
		// store result in destination buffer
		_mm_storel_epi64(((__m128i*)(pDst + x)), _mm_packus_epi16(v_dst, v_dst));
	}
	return x;
};

void Scaling8U(const unsigned char* src, size_t sstep, unsigned char* dst, size_t dstep, int nWidth, int nHeight, float p_alpha, float p_beta)
{

	// the following not needed since sizeof(src[0]) should be 1
	// keep it in case
	//sstep /= sizeof(src[0]);
	//dstep /= sizeof(dst[0]);

	for (; nHeight--; src += sstep, dst += dstep)
	{
		int x = convert8uScale_Row(src, dst, nWidth, p_alpha, p_beta);
		//on fini ce qui reste en cpu
		// unroll
		for (; x <= nWidth - 4; x += 4)
		{
			unsigned char t0, t1;
			t0 = saturateCAST<unsigned char>((float)src[x] * p_alpha + p_beta);
			t1 = saturateCAST<unsigned char>((float)src[x + 1] * p_alpha + p_beta);
			dst[x] = t0; dst[x + 1] = t1;
			t0 = saturateCAST<unsigned char>((float)src[x + 2] * p_alpha + p_beta);
			t1 = saturateCAST<unsigned char>((float)src[x + 3] * p_alpha + p_beta);
			dst[x + 2] = t0; dst[x + 3] = t1;
		}

		for (; x < nWidth; x++)
			dst[x] = saturateCAST<unsigned char>((float)src[x] * p_alpha + p_beta);

	}
}