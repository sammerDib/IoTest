#include "pch.h"
#include "ProfileStepAnalyser.h"

#include "MathUtils.h"
#include "Convolution1D.h"
#include "GaussianFit.h"

static double StepHeight(const Profile& profile, const double stepX)
{
    const auto [beforeStepPointIt, afterStepPointIt] = std::equal_range(profile.begin_point(), profile.end_point(),
        stepX, Profile::CompareX{});
    const auto beforeStepIt = profile.begin_y() + std::distance(profile.begin_point(), beforeStepPointIt == profile.begin_point() ? beforeStepPointIt : beforeStepPointIt - 1);
    const auto afterStepIt = profile.begin_y() + std::distance(profile.begin_point(), afterStepPointIt);
    const auto averageLow = Average(profile.begin_y(), beforeStepIt + 1);
    const auto averageHigh = Average(afterStepIt, profile.end_y());
    return std::abs(averageHigh - averageLow);
}

std::unique_ptr<ProfileAnalyserResult> ProfileStepAnalyser::operator()(const Profile& scan) const
{
    if (scan.GetPoints().empty())
    {
        return std::make_unique<ProfileStepAnalyserResult>(ProfileStepAnalyserResult::Status::EMPTY_PROFILE);
    }

    auto profile = scan;

    if (_parameters.startZone)
    {
        profile.RemoveExclusionZone(_parameters.startZone.value());
    }
    if (_parameters.endZone)
    {
        profile.RemoveExclusionZone(_parameters.endZone.value());
    }

    profile.FilterNanY();
    if (profile.GetPoints().empty())
    {
        return std::make_unique<ProfileStepAnalyserResult>(ProfileStepAnalyserResult::Status::EMPTY_PROFILE_NAN);
    }
    auto average = Average(profile.begin_y(), profile.end_y());

    const auto stdDev = StandardDeviation(profile.begin_y(), profile.end_y(), average);
    profile.RemoveStdDevY(average, stdDev, _parameters.nbStdDevFiltering);
    if (profile.Size() < 2)
    {
        return std::make_unique<ProfileStepAnalyserResult>(ProfileStepAnalyserResult::Status::PROFILE_TOO_SMALL_AFTER_STD_DEV_FILTERING);
    }

    average = Average(profile.begin_y(), profile.end_y());
    std::for_each(profile.begin_y(), profile.end_y(), [&average](auto& v) { v -= average; });

    const auto kernelHalfSize = std::min(
        _parameters.maxConvolutionKernelHalfSize, 
        static_cast<std::size_t>(std::floor(profile.Size() / 2))
    );
    std::vector<double> convolutionKernel(kernelHalfSize * 2);
    if (_parameters.kindStep == KindStep::UP)
    {
        const auto it = std::fill_n(convolutionKernel.begin(), kernelHalfSize, 1);
        std::fill_n(it, kernelHalfSize, -1);
    }
    else
    {
        const auto it = std::fill_n(convolutionKernel.begin(), kernelHalfSize, -1);
        std::fill_n(it, kernelHalfSize, 1);
    }
    const auto convolutionResult = ValidConvolution1D(profile.begin_y(), profile.end_y(), convolutionKernel.begin(), convolutionKernel.end());

    const std::vector<double> xConvolution(
        profile.begin_x() + kernelHalfSize - 1,
        profile.begin_x() + profile.Size() - kernelHalfSize);

    const auto fittedGaussParams = FittedGaussianParams(xConvolution, convolutionResult);

    auto result = std::make_unique<ProfileStepAnalyserResult>();
    result->stepX = fittedGaussParams.mu;

    if (_parameters.stepZone)
    {
        auto zone = _parameters.stepZone.value();
        zone.SetX(result->stepX);
        profile.RemoveExclusionZone(zone);
    }

    result->stepHeight = StepHeight(profile, result->stepX);
    result->status = ProfileStepAnalyserResult::Status::OK;

    return result;
}