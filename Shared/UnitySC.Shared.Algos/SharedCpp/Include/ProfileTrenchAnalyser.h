#pragma once

#include "ProfileEdgeTrimAnalyser.h"

/// <summary>
/// Parameters for the Trench profile analyser.
/// targetDepth: the expected depth of the trench
/// toleranceDepth: tolerance between the target depth and the actual one
/// targetWidth: the expected width of the trench
/// toleranceWidth: tolerance between the target width and the actual one
/// trenchDownZone: (optional) the exclusion zone to apply around the down step.
/// trenchUpZone: (optional) the exclusion zone to apply around the up step.
/// </summary>
struct ProfileTrenchAnalyserParameters : public ProfileAnalyserParameters
{
    ProfileTrenchAnalyserParameters(
        const double targetDepth, const double toleranceDepth,
        const double targetWidth, const double toleranceWidth) :
        targetDepth(targetDepth), toleranceDepth(toleranceDepth),
        targetWidth(targetWidth), toleranceWidth(toleranceWidth)
    {
    }

    double targetDepth;
    double toleranceDepth;
    double targetWidth;
    double toleranceWidth;

    std::optional<ExclusionZone> trenchDownZone;
    std::optional<ExclusionZone> trenchUpZone;
};

/// <summary>
/// Results of the Trench profile analyser.
/// depth is the depth of the trench.
/// width is the width of the trench.
/// </summary>
struct ProfileTrenchAnalyserResult : public ProfileAnalyserResult
{
    double depth;
    double width;
};

/// <summary>
/// Functor to perform the Trench profile analyse.
/// </summary>
class ProfileTrenchAnalyser : public ProfileAnalyser
{
public:
    ProfileTrenchAnalyser(const ProfileTrenchAnalyserParameters& parameters) :
        ProfileAnalyser(), _parameters(parameters)
    {
    }

    ~ProfileTrenchAnalyser() override = default;

    std::unique_ptr<ProfileAnalyserResult> operator()(const Profile& scan) const override;

protected:
    ProfileTrenchAnalyserParameters _parameters;
};
