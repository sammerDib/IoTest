#include "pch.h"

#include <omp.h>
#include "UnitySC.Shared.Algos.CppWrapper.h"
#include "UnitySC.Shared.Algos.CppNative.h"

#pragma managed
using namespace System::Diagnostics;
namespace UnitySCSharedAlgosCppWrapper {
    double Utils::testopenmp(const int nbpoint, bool bUseopenMP)
    {
        Stopwatch^ sw = gcnew Stopwatch();
        sw->Start();
        fnTestopenmp(nbpoint, bUseopenMP);
        sw->Stop();
        return (double)sw->ElapsedMilliseconds;
    }

    void Utils::DisableOpenMP()
    {
        omp_set_num_threads(1);
    }
}
