#include "CircularTheoricalGrid.hpp"
#include "ICircleFitter.hpp"
#include "HyperAccurateCircleFitter.hpp"
#include <cmath>
#include <opencv2/core/persistence.hpp>
#include <opencv2/core/types.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/opencv.hpp>
#include <string>
#include <tuple>
#include <vector>

namespace {
    // Gives the borders of a horizontal segment of y coordinate `rowY` and that's inside the
    // cirlce of center `waferCenter` and radius `waferRadius`
    std::tuple<double, double> CircleSegmentBorders(double rowY, double waferRadius, cv::Point2d waferCenter) {
        // To have them, we use the Pythagorean theorem. Indeed, we can trace a
        // rectangle triangle where the three points are:
        // 1. on the border of the wafer at the required y coordinate
        // 2. on the center of the wafer
        // 3. at the required y coordinate and the x coordinate of the center of the wafer
        // 
        // We thus have a right angle at the point n°3. And if we name A the length of the
        // side between points 1 and 3, B between 2 and 3, and C between 1 and 2, then we
        // have that A² + B² = C². Here, we know B and C, and A can allow us to find
        // the x coordinate of the borders

        double c = waferRadius;
        double b = rowY - waferCenter.y;
        double a = sqrt(c * c - b * b); // Let's cross fingers and hope c >= b

        double halfSegmentLength = a;
        double leftBorder = waferCenter.x - halfSegmentLength;
        double rightBorder = waferCenter.x + halfSegmentLength;
        return std::tuple(leftBorder, rightBorder);
    }

    double LeftmostDie(double segmentLeftX, double originX, double dieWidth) {
        // To find the x position of the leftmost die, the logic is the same as
        // for the y coordinate: we need to take origin.x and subtract (or add) the
        // width of the die until we are the nearest possible to the left border of
        // the wafer at the y position.
        // To do that, we can also simply use a modulo in the same way as for y.

        // The x position of the first die of the row
        double x0RelativeToSegmentLeft = std::fmod(originX - segmentLeftX, dieWidth);
        // Fix the case where the mod is negative
        if (x0RelativeToSegmentLeft < 0) {
            x0RelativeToSegmentLeft += dieWidth;
        }
        // Add back the x position of the left border to get the real x position
        return segmentLeftX + x0RelativeToSegmentLeft;
    }


    // List all the points along the horizontal line of coordinate rowY such
    // that the whole segment of length dieWidth that starts at this point is
    // inside the given wafer circle. These points are also aligned with
    // originX
    std::vector<double> DiesOnLine(
        double rowY,
        double waferRadius,
        cv::Point2d waferCenter,
        double originX,
        double dieWidth
    ) {
        std::vector<double> result{};

        // We're computing the x positions of the row of dies at the given y position.
        // To know said position, we first need to know where are the borders of
        // the wafer at the y position.
        
        auto tmp = CircleSegmentBorders(rowY, waferRadius, waferCenter);
        double leftBorder = std::get<0>(tmp);
        double rightBorder = std::get<1>(tmp);

        // And then know the position of the leftmost die
        double x0 = LeftmostDie(leftBorder, originX, dieWidth);

        for (double x = x0; x < rightBorder - dieWidth; x += dieWidth) {
            result.push_back(x);
        }

        return result;
    }

    std::vector<double> DiesOnBorder(
        double narrowestY,
        double widestY,
        double waferRadius,
        cv::Point2d waferCenter,
        double originX,
        double dieWidth
    ) {
        std::vector<double> result{};

        auto narrowBorders = CircleSegmentBorders(narrowestY, waferRadius, waferCenter);
        auto wideBorders = CircleSegmentBorders(widestY, waferRadius, waferCenter);
        
        double leftSideLeftBorder = std::get<0>(wideBorders);
        double leftSideRightBorder = std::get<0>(narrowBorders);
        
        double rightSideLeftBorder = std::get<1>(narrowBorders);
        double rightSideRightBorder = std::get<1>(wideBorders);

        double x0 = LeftmostDie(leftSideLeftBorder, originX, dieWidth);
        double xend = LeftmostDie(leftSideRightBorder, originX, dieWidth);
        x0 -= dieWidth;
        for (double x = x0; x < xend; x += dieWidth) {
            result.push_back(x);
        }

        x0 = LeftmostDie(rightSideLeftBorder, originX, dieWidth);
        x0 -= dieWidth;
        for (double x = x0; x < rightSideRightBorder; x += dieWidth) {
            result.push_back(x);
        }

        return result;
    }

}

CircularTheoricalGrid::CircularTheoricalGrid() {}
CircularTheoricalGrid::CircularTheoricalGrid(
    std::vector<cv::Point2d> const& waferCirlce,
    cv::Size2d dieSize,
    cv::Point2d diePosition
) {
    ICircleFitter::Result circleRes;
    Algorithms::Status status = HyperAccurateCircleFitter().Fit(waferCirlce, circleRes);
    // TODO: check the confidence
    this->waferCenter = circleRes.center;
    this->waferRadius = circleRes.radius;
    this->dieSize = dieSize;
    this->origin = diePosition;
}

CircularTheoricalGrid::CircularTheoricalGrid(
    cv::Point waferCenter,
    int waferRadius,
    cv::Size2d dieSize,
    cv::Point2d diePosition
)
{
    this->waferCenter = cv::Point2d((double)waferCenter.x, (double)waferCenter.y);
    this->waferRadius = waferRadius;
    this->dieSize = dieSize;
    this->origin = diePosition;
}

void CircularTheoricalGrid::write(cv::FileStorage& fs) const
{
    fs << "{"
        << "wafer_center" << this->waferCenter
        << "wafer_radius" << this->waferRadius
        << "dieSize" << this->dieSize
        << "origin" << this->origin
        << "}";
}
void CircularTheoricalGrid::read(const cv::FileNode& node)
{
    node["wafer_center"] >> this->waferCenter;
    node["wafer_radius"] >> this->waferRadius;
    node["dieSize"] >> this->dieSize;
    node["origin"] >> this->origin;
}
void CircularTheoricalGrid::DrawParts(cv::Mat& img, cv::Scalar color)
{
    cv::circle(img, waferCenter, waferRadius, color, 4);
    cv::rectangle(img, cv::Rect2d(origin, dieSize), color, 5);
}

double CircularTheoricalGrid::FirstRow() {
    const double waferTop = waferCenter.y - waferRadius;

    // The first row relatively to the top
    double y0 = std::fmod(origin.y - waferTop, dieSize.height);
    // The position shall be positive (i.e. under the top of the wafer) for it
    // to work properly
    if (y0 < 0) {
        y0 += dieSize.height;
    }
    // Add back the y position of the top to get the real y position
    return y0 + waferTop;
}

std::vector<cv::Rect2i> CircularTheoricalGrid::AsRects() {
    std::vector<cv::Rect2i> result{};

    // We're computing the y position of the first row of dies.
    // To know said position, we need to take origin.y (which is a position of
    // one of the dies of the wafer) and subtract the height of the die until
    // we are at the top of the wafer. To do that, we can simply use a modulo:
    // by subtracting the y position of the top of the wafer to origin.y and
    // then do a mod dieHeight, we obtain the y position of the first row of
    // dies relatively to the top of the wafer


    double y = FirstRow();
    // Top half of the wafer
    for (; y < waferCenter.y + dieSize.height / 2; y += dieSize.height) {
        auto positions = DiesOnLine(y, waferRadius, waferCenter, origin.x, dieSize.width);

        for (auto pos = positions.begin(); pos != positions.end(); pos++) {
            // If the segment that starts at pos and extends of dieSize.width to the right
            // is inside the wafer (as guaranteed by DiesOnLine), then the whole die just
            // UNDER this segment is inside the wafer (because we're in the top half, so there's
            // more horizontal space under the segment than above)
            result.push_back(cv::Rect(cv::Point(*pos, y), dieSize));
        }
    }

    // Skip a line because we'll start drawing the dies from the bottom rather than from the top
    y += dieSize.height;

    // Bottom half of the wafer
    double end = waferCenter.y + waferRadius;
    for (; y < end; y += dieSize.height) {
        auto positions = DiesOnLine(y, waferRadius, waferCenter, origin.x, dieSize.width);
        for (auto pos = positions.begin(); pos != positions.end(); pos++) {
            // If the segment that starts at pos and extends of dieSize.width to the right
            // is inside the wafer (as guaranteed by DiesOnLine), then the whole die just
            // ABOVE this segment is inside the wafer (because we're in the bottom half,
            // so there's more horizontal space above the segment than under)
            result.push_back(cv::Rect(cv::Point(*pos, y - dieSize.height), dieSize));
        }
    }
    return result;
}

std::vector<cv::Rect2i> CircularTheoricalGrid::BorderDies() {
    std::vector<cv::Rect2i> result{};

    // Here, we do the same as in AsLayout, but only take the dies that are not
    // fully on the wafer

    double y = FirstRow();

    // Do the row above the first row
    auto borders = CircleSegmentBorders(y, waferRadius, waferCenter);
    auto start = LeftmostDie(std::get<0>(borders), origin.x, dieSize.width);
    for (auto x = start - dieSize.width; x < std::get<1>(borders) + dieSize.width; x += dieSize.width) {
        result.push_back(cv::Rect(cv::Point(x, y - dieSize.width), dieSize));
    }


    // Top half of the wafer
    for (; y < waferCenter.y - dieSize.height; y += dieSize.height) {
        auto positions = DiesOnBorder(y, y + dieSize.height, waferRadius, waferCenter, origin.x, dieSize.width);

        for (auto pos = positions.begin(); pos != positions.end(); pos++) {
            result.push_back(cv::Rect(cv::Point(*pos, y), dieSize));
        }
    }

    // center
    auto narrowestY = waferCenter.y - y < (y + dieSize.height) - waferCenter.y ? y : y + dieSize.height;
    auto positions = DiesOnBorder(narrowestY, waferCenter.y, waferRadius, waferCenter, origin.x, dieSize.width);
    for (auto pos = positions.begin(); pos != positions.end(); pos++) {
        result.push_back(cv::Rect(cv::Point(*pos, y), dieSize));
    }
    // Center finished (Note: we didn't skip a line)
    y += dieSize.height;

    for (; y < waferCenter.y + waferRadius; y += dieSize.height) {
        auto positions = DiesOnBorder(y + dieSize.height, y, waferRadius, waferCenter, origin.x, dieSize.width);

        for (auto pos = positions.begin(); pos != positions.end(); pos++) {
            result.push_back(cv::Rect(cv::Point(*pos, y), dieSize));
        }
    }


    // Do the row above the first row
    y -= dieSize.height;
    borders = CircleSegmentBorders(y, waferRadius, waferCenter);
    start = LeftmostDie(std::get<0>(borders), origin.x, dieSize.width);
    for (auto x = start - dieSize.width; x < std::get<1>(borders) + dieSize.width; x += dieSize.width) {
        result.push_back(cv::Rect(cv::Point(x, y), dieSize));
    }
    return result;
}

std::vector<cv::Rect2d> CircularTheoricalGrid::ApplySublayout(const std::vector<cv::Rect2d>& sublayout) {
    std::vector<cv::Rect2d> res;
    // TODO: add border dies
    for (auto reticle : AsRects()) {
        for (auto die : sublayout) {
            res.push_back(die + cv::Point2d(reticle.tl()));
        }
    }
    return res;
}


bool CircularTheoricalGrid::operator==(const CircularTheoricalGrid& other)
{
    // TODO: check if this is the right thing to do
    return waferCenter == other.waferCenter
        && waferRadius == other.waferRadius
        && dieSize == other.dieSize
        && origin == other.origin;
}

bool CircularTheoricalGrid::operator!=(const CircularTheoricalGrid& other)
{
    // TODO: check if this is the right thing to do
    return !(*this == other);
}

std::vector<std::vector<cv::Rect>> generateWaferWithDiesAndTheoreticalGrid(
    cv::Mat& generatedWaferImg,
    cv::Size imgSize,
    int reticleStep,
    const std::vector<cv::Rect>& reticleDies,
    const std::vector<cv::Scalar>& dieColor)
{
    generatedWaferImg = cv::Mat(imgSize.height, imgSize.width, CV_8UC1, cv::Scalar(0));
    int waferRadius = std::min(imgSize.width, imgSize.height) / 2;
    cv::Point waferCenter = cv::Point(generatedWaferImg.cols / 2, generatedWaferImg.rows / 2);
    cv::circle(generatedWaferImg, waferCenter, waferRadius, cv::Scalar(255), -1);

    std::vector<std::vector<cv::Rect>> generatedTheoreticalGrid;
    for (int i = 0; i < reticleDies.size(); ++i)
    {
        generatedTheoreticalGrid.push_back(std::vector<cv::Rect>());
    }

    auto grid = CircularTheoricalGrid(waferCenter,
        waferRadius,
        cv::Size2d((double)reticleStep, (double)reticleStep),
        cv::Point(0, 0));

    for (cv::Rect reticle : grid.AsRects()) {
        for (int i = 0; i < reticleDies.size(); i++) {
            auto die = reticleDies[i];
            cv::Rect roi(reticle.x + die.x, reticle.y + die.y, die.width, die.height);
            cv::rectangle(generatedWaferImg, roi, dieColor[i]);
            generatedTheoreticalGrid[i].push_back(roi);
        }
    }

    for (cv::Rect reticle : grid.BorderDies()) {
        for (int i = 0; i < reticleDies.size(); i++) {
            auto die = reticleDies[i];
            cv::Rect roi(reticle.x + die.x, reticle.y + die.y, die.width, die.height);
            cv::rectangle(generatedWaferImg, roi, dieColor[i]);
        }
    }

    return generatedTheoreticalGrid;
}


// Allows writing into cv::FileStorage with operator<<
void write(cv::FileStorage& fs, const std::string&, const CircularTheoricalGrid& grid)
{
    grid.write(fs);
}
// Allows reading from cv::FileStorage with operator>>
void read(const cv::FileNode& node, CircularTheoricalGrid& grid, const CircularTheoricalGrid& defaultValue) {
    if (node.empty()) {
        grid = defaultValue;
    }
    else {
        grid.read(node);
    }
}
