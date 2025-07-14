#pragma once

#include "ProfileAnalyser.h"

enum class KindStep
{
    UP,
    DOWN,
};

/// <summary>
/// Parameters for the Step profile analyser.
/// kindStep: if the analyser should look for a up or down step.
/// targetHeight: the expected height of the step
/// toleranceHeight: tolerance between the target height and the actual one
/// stepZone: (optional) the exclusion zone to apply around the step.
/// </summary>
struct ProfileStepAnalyserParameters : public ProfileAnalyserParameters
{
    ProfileStepAnalyserParameters(const KindStep kindStep, const double targetHeight, const double tolerance) : 
        ProfileAnalyserParameters(), kindStep(kindStep), targetHeight(targetHeight), toleranceHeight(tolerance)
    {}
    ~ProfileStepAnalyserParameters() override = default;

    KindStep kindStep;

    double targetHeight;
    double toleranceHeight;

    std::optional<ExclusionZone> stepZone;
};

/// <summary>
/// Results of the Step profile analyser.
/// stepX is the X coordinate of where the step has been located
/// stepHeight is the measure height of the step
/// </summary>
struct ProfileStepAnalyserResult : public ProfileAnalyserResult
{
    ProfileStepAnalyserResult() = default;
    ProfileStepAnalyserResult(const Status status) : ProfileAnalyserResult(status)
    {}
    ~ProfileStepAnalyserResult() override = default;

    double stepX{ 0.0 };
    double stepHeight{ 0.0 };
};

/// <summary>
/// Functor to perform the Step profile analyse.
/// Here is a sum up of the algorithm:
/// - Filter points that have Y coordinate = NaN
/// - Filter points that have Y coordinate further than average +/- nbStdDevFiltering * std dev
/// - Transform the profile so that its average is zero.
/// - Apply a convolution with a step kernel (1, 1, ..., -1, -1).
/// - Find a gaussian curve fitting the convolution result.
/// - Look for the peak values (min and max) of the gaussian. Where the peak is found is where the step is.
/// - Apply exclusion zones.
/// - Compute the average height left and right to the step. The difference between the two averages is the height.
/// This is greatly inspired of what is done here: https://stackoverflow.com/questions/48000663/step-detection-in-one-dimensional-data
/// </summary>
class ProfileStepAnalyser : public ProfileAnalyser
{
public:
    ProfileStepAnalyser(const ProfileStepAnalyserParameters& parameters) :
        ProfileAnalyser(), _parameters(parameters)
    {
    }

    ~ProfileStepAnalyser() override = default;

    std::unique_ptr<ProfileAnalyserResult> operator()(const Profile& scan) const override;

protected:
    ProfileStepAnalyserParameters _parameters;
};