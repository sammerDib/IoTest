#include "pch.h"
#include "ProfileEdgeTrimAnalyser.h"

std::unique_ptr<ProfileAnalyserResult> ProfileEdgeTrimAnalyser::operator()(const Profile& scan) const
{
    const auto analyseResult = ProfileStepAnalyser(_parameters)(scan);
    const auto stepResult = static_cast<ProfileStepAnalyserResult*>(analyseResult.get());
    auto result = std::make_unique<ProfileEdgeTrimAnalyserResult>(*stepResult);
    if (stepResult->status != ProfileStepAnalyserResult::Status::OK)
    {
        return result;
    }

    auto profile = scan;

    double xStartWidth;
    if (_parameters.stepZone.has_value())
    {
        auto zone = _parameters.stepZone.value();
        zone.SetX(result->stepX);
        profile.RemoveExclusionZone(zone);
        xStartWidth = *std::upper_bound(profile.begin_x(), profile.end_x(), result->stepX);
    }
    else
    {
        xStartWidth = result->stepX;
    }

    if (_parameters.endZone.has_value())
    {
        profile.RemoveExclusionZone(_parameters.stepZone.value());
    }
    double xEndWidth = profile.Back().X;

    result->width = xEndWidth - xStartWidth;

    return result;
}