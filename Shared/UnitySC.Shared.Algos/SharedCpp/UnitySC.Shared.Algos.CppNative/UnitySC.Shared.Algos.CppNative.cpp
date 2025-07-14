// UnitySC.Shared.Algos.CppLib.cpp : Defines the functions for the static library.
//

#include "pch.h"
#include "framework.h"
#include "UnitySC.Shared.Algos.CppNative.h"

#include <omp.h>

#pragma unmanaged

// fonction
// for unit testing - ONLY
void fnTestopenmp(const int nbpoint, bool bUseopenMP)
{
    double* par = new double[nbpoint * nbpoint];
    if (bUseopenMP)
    {
#pragma omp parallel for schedule(dynamic, 16), firstprivate(par)
        for (int y = 0; y < nbpoint; y++)
        {
            double* pLine = par + (y * nbpoint);
            for (int x = 0; x < nbpoint; x++)
            {
                (*pLine) = x;
                pLine++;
            }
        }
    }
    else
    {
        double* pIter = par;
        for (int y = 0; y < nbpoint; y++)
        {
            for (int x = 0; x < nbpoint; x++)
            {
                (*pIter) = x;
                pIter++;
            }
        }

    }
    delete[] par;
}
