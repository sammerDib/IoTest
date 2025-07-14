#pragma once

#include "ProfileStepAnalyser.h"

/// <summary>
/// Parameters for the EdgeTrim profile analyser.
/// The same parameters as Step analyser plus:
/// targetWidth: the expected width of the edge trim
/// toleranceWidth: tolerance between the target width and the actual one
/// </summary>
struct ProfileEdgeTrimAnalyserParameters : public ProfileStepAnalyserParameters
{
    ProfileEdgeTrimAnalyserParameters(const KindStep kindStep, 
        const double targetHeight, const double toleranceHeight,
        const double targetWidth, const double toleranceWidth) :
        ProfileStepAnalyserParameters(kindStep, targetHeight, toleranceHeight), 
        targetWidth(targetWidth), toleranceWidth(toleranceWidth)
    {
    }
    ~ProfileEdgeTrimAnalyserParameters() override = default;

    double targetWidth;
    double toleranceWidth;
};

/// <summary>
/// Results of the EdgeTrim profile analyser.
/// The same results as Step analyser plus:
/// width is the measure width of the edge trim
/// </summary>
struct ProfileEdgeTrimAnalyserResult : public ProfileStepAnalyserResult
{
    ProfileEdgeTrimAnalyserResult() = default;
    ProfileEdgeTrimAnalyserResult(const ProfileStepAnalyserResult& stepResult) : 
        ProfileStepAnalyserResult(stepResult)
    {
    }
    ~ProfileEdgeTrimAnalyserResult() override = default;

    double width{ 0.0 };
};

/// <summary>
/// Functor to perform the EdgeTrim profile analyse.
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
class ProfileEdgeTrimAnalyser : public ProfileAnalyser
{
public:
    ProfileEdgeTrimAnalyser(const ProfileEdgeTrimAnalyserParameters& parameters) :
        ProfileAnalyser(), _parameters(parameters)
    {
    }

    ~ProfileEdgeTrimAnalyser() override = default;

    std::unique_ptr<ProfileAnalyserResult> operator()(const Profile& scan) const override;

protected:
    ProfileEdgeTrimAnalyserParameters _parameters;
};