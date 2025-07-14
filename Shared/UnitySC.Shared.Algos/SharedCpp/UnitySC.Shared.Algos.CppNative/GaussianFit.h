#pragma once

class Profile;

/// <summary>
/// Parameters of a gaussian curve.
/// A: amplitude
/// mu: average
/// sigma: standard deviation
/// </summary>
struct GaussianParams
{
    double A;
    double mu;
    double sigma;
};

/// <summary>
/// Find the best parameters to have a gaussian curve fitting the curve (xInput, yInput).
/// It relies on the FitLib project.
/// </summary>
/// <param name="xInput">The X coordinates of the points of the curves to find its fit</param>
/// <param name="yInput">The Y coordinates of the points of the curves to find its fit</param>
/// <returns>Params for a fitting gaussian curve</returns>
GaussianParams FittedGaussianParams(const std::vector<double>& xInput, const std::vector<double>& yInput);
