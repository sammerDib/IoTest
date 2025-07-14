#include "pch.h"
#include "ProfileTrenchAnalyser.h"

std::unique_ptr<ProfileAnalyserResult> ProfileTrenchAnalyser::operator()(const Profile& scan) const
{
    const auto midValueX = scan.Front().X + ((scan.Back().X - scan.Front().X) / 2.0);
    const auto midIt = std::lower_bound(scan.begin_point(), scan.end_point(), midValueX, Profile::CompareX{});

    ProfileEdgeTrimAnalyserParameters paramsLeft(KindStep::DOWN,
        _parameters.targetDepth, _parameters.toleranceDepth,
        _parameters.targetWidth / 2.0, _parameters.toleranceWidth);
    paramsLeft.startZone = _parameters.startZone;
    paramsLeft.stepZone = _parameters.trenchDownZone;
    paramsLeft.nbStdDevFiltering = _parameters.nbStdDevFiltering;
    paramsLeft.maxConvolutionKernelHalfSize = _parameters.maxConvolutionKernelHalfSize;
    Profile profileLeft({ scan.begin_point(), std::next(midIt) });
    auto resultLeft = ProfileEdgeTrimAnalyser(paramsLeft)(profileLeft);
    if (resultLeft->status != ProfileAnalyserResult::Status::OK)
    {
        return resultLeft;
    }
    const auto resultEdgeTrimLeft = static_cast<ProfileEdgeTrimAnalyserResult*>(resultLeft.get());

    ProfileEdgeTrimAnalyserParameters paramsRight(KindStep::DOWN,
        _parameters.targetDepth, _parameters.toleranceDepth,
        _parameters.targetWidth / 2.0, _parameters.toleranceWidth);
    paramsRight.endZone = _parameters.endZone;
    paramsRight.stepZone = _parameters.trenchUpZone;
    paramsRight.nbStdDevFiltering = _parameters.nbStdDevFiltering;
    paramsRight.maxConvolutionKernelHalfSize = _parameters.maxConvolutionKernelHalfSize;
    Profile profileRight({ midIt, scan.end_point() });
    profileRight.Reverse();
    auto resultRight = ProfileEdgeTrimAnalyser(paramsRight)(profileRight);
    if (resultRight->status != ProfileAnalyserResult::Status::OK)
    {
        return resultRight;
    }
    const auto resultEdgeTrimRight = static_cast<ProfileEdgeTrimAnalyserResult*>(resultRight.get());

    auto result = std::make_unique<ProfileTrenchAnalyserResult>();
    result->status = ProfileAnalyserResult::Status::OK;
    result->depth = (resultEdgeTrimLeft->stepHeight + resultEdgeTrimRight->stepHeight) / 2.0;
    result->width = resultEdgeTrimLeft->width + resultEdgeTrimRight->width;

    return result;
}
