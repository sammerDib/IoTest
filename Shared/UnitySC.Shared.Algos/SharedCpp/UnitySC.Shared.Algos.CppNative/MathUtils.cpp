#include "pch.h"
#include "MathUtils.h"

double Average(const std::vector<double>& values)
{
    return Average(values.begin(), values.end());
}

double WeightedAverage(const std::vector<double>& values, const std::vector<double>& weights)
{
    return std::inner_product(values.begin(), values.end(), weights.begin(), 0.0) / 
           std::accumulate(weights.begin(), weights.end(), 0.0);
}

double Variance(const std::vector<double>& values, const double average)
{
    return Variance(values.begin(), values.end(), average);
}

double WeightedVariance(const std::vector<double>& values, const std::vector<double>& weights, const double weightedAverage)
{
    const auto size = values.size();
    if (size != weights.size())
    {
        return 0.0;
    }

    const auto a = std::inner_product(values.begin(), values.end(), weights.begin(), 0.0, 
        std::plus<>(),
        [&weightedAverage](const double a, const double b) { return b * (a - weightedAverage) * (a - weightedAverage); });
    const auto b = std::accumulate(weights.begin(), weights.end(), 0.0);
    return a / b;
}

double StandardDeviation(const std::vector<double>& values, const double average)
{
    return StandardDeviation(values.begin(), values.end(), average);
}

double WeightedStandardDeviation(const std::vector<double>& values, const std::vector<double>& weights, const double weightedAverage)
{
    return std::sqrt(WeightedVariance(values, weights, weightedAverage));
}
