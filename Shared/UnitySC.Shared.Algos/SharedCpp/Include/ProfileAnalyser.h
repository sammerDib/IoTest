#pragma once

#include "Profile.h"
#include "ExclusionZone.h"

/// <summary>
/// Parameters common to all profile analysis
/// profile: the scan to perform the measure on
/// startZone: (optional) the exclusion zone to apply at the beginning of the scan.
/// endZone: (optional) the exclusion zone to apply at the end of the scan.
/// nbStdDevFiltering: use to filter points that are at a distance of the mean > nbStdDevFiltering * std dev
/// maxConvolutionKernelHalfSize: the convolution kernel (used during the analysis) cannot be bigger than twice this size.
/// </summary>
struct ProfileAnalyserParameters
{
    ProfileAnalyserParameters() = default;
    virtual ~ProfileAnalyserParameters() = default;

    std::optional<ExclusionZone> startZone;
    std::optional<ExclusionZone> endZone;

    double nbStdDevFiltering{ 3.0 };
    std::size_t maxConvolutionKernelHalfSize{ 5 };
};

/// <summary>
/// Results common to all profile analysis
/// If status is different than OK, something wrong happened and other members shouldn't be looked at.
/// </summary>
struct ProfileAnalyserResult
{
    /// <summary>
    /// OK: everything went fine
    /// EMPTY_PROFILE: ProfileStepAnalyserParameters::profile was empty, nothing could be done
    /// EMPTY_PROFILE_NAN: given profile was emptied by NaN filtering
    /// PROFILE_TOO_SMALL_AFTER_STD_DEV_FILTERING: Standard deviation filtering made the profile too small, try to reduce ProfileStepAnalyserParameters::nbStdDevFiltering to solve.
    /// </summary>
    enum class Status
    {
        OK,
        EMPTY_PROFILE,
        EMPTY_PROFILE_NAN,
        PROFILE_TOO_SMALL_AFTER_STD_DEV_FILTERING,
    };

    ProfileAnalyserResult() = default;
    ProfileAnalyserResult(const Status status) : status(status)
    {
    }
    virtual ~ProfileAnalyserResult() = default;

    Status status{ Status::OK };
};

/// <summary>
/// Base class for all profile analysers.
/// </summary>
class ProfileAnalyser
{
public:
    virtual ~ProfileAnalyser() = default;

    virtual std::unique_ptr<ProfileAnalyserResult> operator()(const Profile& scan) const = 0;
};