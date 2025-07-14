#include <vector>

#include "PhaseMapping.hpp"
#include "CImageTypeConvertor.hpp"
#include "ErrorLogging.hpp"
#include "PhaseMappingWithHariharan.hpp"
#include "PhaseMappingWithResidualFringeRemoving.hpp"

using namespace std;
using namespace cv;

#pragma unmanaged
namespace phase_mapping {

    WrappedPhaseMap PhaseMapping(vector<Mat> interferograms, int stepNb, bool removeResidualFringes) {

        if (removeResidualFringes)
        {
            return residual_fringe_removing::WrappedPhaseMapWithoutResidualFringe(interferograms, stepNb);
        }

        return hariharan::HariharanPhaseMapping(interferograms, stepNb);
    }
} // namespace phase_mapping