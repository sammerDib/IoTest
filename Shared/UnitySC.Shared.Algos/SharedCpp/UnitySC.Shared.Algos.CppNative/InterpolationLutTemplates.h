#pragma once

#pragma unmanaged

#pragma region
////////////////////////////////
// Interpolation templates
////////////////////////////////

// Binary search : find pointer "p"  such as *p <= k< *(p+1)  -- Algo en O(Log(n))
// WARNING ARRAY MUST SORTED in ASCENDING ORDER !!!! (IDEALLY MONOTONOUS)
// a = points to the beginning of the array ; b= points to one element past the end of the array (b must be gretaer than a); k value to search 
template<typename  T> T* BinarySearch(T* a, T* b, T k)
{
    if (k < *a)
        return a - 1; // This could point outside the array so need to be treated where
    for (T* c; k < *--b; k > *c ? a = c : b = c + 1)
        c = a + (b - a) / 2;
    return b;
}

// Unitary Linear Interpolation : such as y(x) = pyi + ((pyi+1) - pyi) * [(x - pxi) / (((pxi+1) - pxi)] where ( *X <= x < x[1 )] 
// pX = points to an array that is used to store values for pxi. Specifically, *X = pxi and X[1] = (pxi+1). Thus, both X and X+1 must point to valid addresses
// pY = points to an array that is used to store values for pyi. Specifically, *Y = pyi and Y[1] = (pyi+1). Thus, both Y and Y+1 must point to valid addresses
// x = value to interpolate at (TYPICALLY, *X <= x <= x[1])
template<typename  T> T LinearInterpolation(const T* pX, const T* pY, T x)
{
    return *pY + (pY[1] - *pY) * (x - *pX) / (pX[1] - *pX);
}

// Array Interpolation 
// ArraySize =  size of Array pX and pY
// pX = points to an array that is used to store values for pxi. 
// pY = points to an array that is used to store values for pyi. 
// x = value to interpolate at (TYPICALLY, *X <= x <= x[1])
template<typename  T> T InterpolArray(int ArraySize, T* pX, T* pY, T x)
{
    int nMaxL = ArraySize - 1;
    if (x < *pX)
        return *pY;
    if (x >= *(pX + nMaxL))
        return *(pY + nMaxL);
    __int64 i = BinarySearch(pX + 1, pX + nMaxL, x) - pX;
    return LinearInterpolation(pX + i, pY + i, x);
}

#pragma endregion  // Interpolation templates
