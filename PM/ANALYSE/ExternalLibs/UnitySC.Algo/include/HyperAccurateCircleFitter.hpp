#pragma once

#include <ICircleFitter.hpp>
#include <Status.hpp>
#include <opencv2/core.hpp>
#include <vector>

/*
 * Circle fit to a given set of data points (in 2D)
 *
 * This is an algebraic fit based on the journal article
 *
 * A. Al-Sharadqah and N. Chernov, "Error analysis for circle fitting
 * algorithms", Electronic Journal of Statistics, Vol. 3, pages 886-911,
 * (2009)
 *
 * It is an algebraic circle fit with "hyperaccuracy" (with zero essential
 * bias). The term "hyperaccuracy" first appeared in papers by Kenichi
 * Kanatani around 2006
 */
class HyperAccurateCircleFitter : public ICircleFitter {
public:
  Algorithms::Status Fit(std::vector<cv::Point> const &points, ICircleFitter::Result &fit, Wafer *wafer) const;

  Algorithms::Status BuildStatus(const ICircleFitter::Result &fit, Wafer *wafer) const;

private:
  static const int MAX_ITERATIONS = 99;
};