#pragma once

#include <opencv2/opencv.hpp>
#include <filesystem>

namespace signal_2D {
    /**
     * Calculate the tilt of the plane from a sample 2D map
     * Map = ax + by + c
     *
     * @param map     - 2D map tilted
     * @param mask    - binary image to select the calculation area
     *
     * @return Plane representing the tilt of the map
     */
    cv::Mat SolvePlaneEquation(cv::Mat map, cv::Mat mask);

    /**
     * Calculate second order (k=2) polynomial surface (parabolic curve) from a sample 2D map
     * f(x,y)= a*x^2 + b*y^2 + c*x*y + d*x +  e*y + f
     *
     * @param img     - img
     * @param mask    - binary image to select the calculation area
     *
     * @return quadratic surface
     */
    cv::Mat SolveQuadraticEquation(cv::Mat img, cv::Mat mask);

    /**
    * Calculate third order (k=3) polynomial surface from a sample 2D map
    * f(x,y)= a*x^3 + b*y^3 + c*x*y^2 + d*y*x^2 + e*x^2 + f*y^2 + g*x*y + h*x +  i*y + j
    *
    * @param img     - img
    * @param mask    - binary image to select the calculation area
    *
    * @return cubic surface
    */
    cv::Mat SolveCubicEquation(cv::Mat img, cv::Mat mask);

    /**
    * Calculate fourth order (k=4) polynomial surface from a sample 2D map
    * f(x,y)= ax^4 + by^4 + cx^3y + dx^2y^2 + exy^3 + fx^3 + gy^3 + hxy^2 + iyx^2 + jx^2 + ky^2 + lxy + mx +  ny + o
    *
    * @param img     - img
    * @param mask    - binary image to select the calculation area
    *
    * @return quartic surface
    */
    cv::Mat SolveQuarticEquation(cv::Mat img, cv::Mat mask);
} // namespace signal_2D