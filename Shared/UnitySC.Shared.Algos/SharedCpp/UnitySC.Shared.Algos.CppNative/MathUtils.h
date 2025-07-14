#pragma once

/// <summary>
/// Computes the mathematical average of the element in the range [begin, end)
/// </summary>
/// <typeparam name="T">Must be an iterator type</typeparam>
/// <param name="begin">Iterator to the first element</param>
/// <param name="end">Past-the-end iterator</param>
/// <returns>The average of the values in the given range</returns>
template <typename T>
constexpr double Average(const T begin, const T end)
{
    return std::accumulate(begin, end, 0.0) / std::distance(begin, end);
}
double Average(const std::vector<double>& values);
// values and weights must have the same length.
double WeightedAverage(const std::vector<double>& values, const std::vector<double>& weights);

/// <summary>
/// Computes the variance of the element in the range [begin, end)
/// </summary>
/// <typeparam name="T">Must be an iterator type</typeparam>
/// <param name="begin">Iterator to the first element</param>
/// <param name="end">Past-the-end iterator</param>
/// <param name="average">The average of the values in the given range</param>
/// <returns>The variance of the values in the given range</returns>
template <typename T>
constexpr double Variance(const T begin, const T end, const double average)
{
    const auto size = std::distance(begin, end);
    const auto varianceFunc = [&average, &size](double accumulator, const double val) {
        return accumulator + ((val - average) * (val - average) / (size - 1));
        };

    return std::accumulate(begin, end, 0.0, varianceFunc);
}
double Variance(const std::vector<double>& values, const double average);
// Implemented according to this definition of weighted std dev : https://www.itl.nist.gov/div898/software/dataplot/refman2/ch2/weightsd.pdf
// But without Bessel Correction (i.e. N/(N-1) factor)
// values and weights must have the same length.
double WeightedVariance(const std::vector<double>& values, const std::vector<double>& weights, const double weightedAverage);

/// <summary>
/// Computes the standard deviation of the element in the range [begin, end)
/// </summary>
/// <typeparam name="T">Must be an iterator type</typeparam>
/// <param name="begin">Iterator to the first element</param>
/// <param name="end">Past-the-end iterator</param>
/// <param name="average">The average of the values in the given range</param>
/// <returns>The standard deviation of the values in the given range</returns>
template <typename T>
constexpr double StandardDeviation(const T begin, const T end, const double average)
{
    return std::sqrt(Variance(begin, end, average));
}
double StandardDeviation(const std::vector<double>& values, const double average);
// values and weights must have the same length.
double WeightedStandardDeviation(const std::vector<double>& values, const std::vector<double>& weights, const double weightedAverage);
