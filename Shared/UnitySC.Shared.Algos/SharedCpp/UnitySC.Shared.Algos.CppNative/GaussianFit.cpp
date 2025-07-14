// We can't use pch here because it messes up with lmfit
#include <vector>
#include <algorithm>
#include <string>
#include <iterator>
#include <numeric>

#include "GaussianFit.h"

#include "Profile.h"
#include "lmfit.hpp"
#include "MathUtils.h"

double Gauss(const double x, const double* params)
{
    // params : 0 , 1 , 2 
    //          A , mu , sigma
    return params[0] * std::exp(-(x - params[1]) * (x - params[1]) / (2 * params[2] * params[2]));
}

GaussianParams FittedGaussianParams(const std::vector<double>& xInput, const std::vector<double>& yInput)
{
    std::vector<double> positiveY;
    std::transform(yInput.begin(), yInput.end(), std::back_inserter(positiveY), [](const auto v) { return v > 0.0 ? v : -v; });

    const auto [yMinIt, yMaxIt] = std::minmax_element(yInput.begin(), yInput.end());
    const auto average = WeightedAverage(xInput, positiveY);
    const auto stddev = WeightedStandardDeviation(xInput, positiveY, average);
    std::vector<double> initialGuess{
        *yMaxIt - *yMinIt,
        average,
        stddev
    };

    const auto fittedParams = lmfit::fit_curve(initialGuess, xInput, yInput, Gauss, lm_control_double).par;
    return { fittedParams[0], fittedParams[1], fittedParams[2] };
}
