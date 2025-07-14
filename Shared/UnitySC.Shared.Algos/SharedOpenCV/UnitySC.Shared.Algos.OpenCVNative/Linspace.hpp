#pragma once

#pragma unmanaged
#include <vector>

/*
 * Create a linear collection of values, given a start value, an end value and a number of steps
 */
template <typename T> std::vector<T> linspace(T startValue, T endValue, int numberOfSteps) {

  std::vector<T> linspaced;

  double start = static_cast<double>(startValue);
  double end = static_cast<double>(endValue);
  double steps = static_cast<double>(numberOfSteps);

  if (steps == 0) {
    return linspaced;
  }
  if (steps == 1) {
    linspaced.push_back((T)start);
    return linspaced;
  }

  double delta = (end - start) / (steps - 1);

  for (int i = 0; i < steps - 1; ++i) {
    linspaced.push_back((T)(start + delta * i));
  }
  linspaced.push_back((T)end);
  return linspaced;
}
