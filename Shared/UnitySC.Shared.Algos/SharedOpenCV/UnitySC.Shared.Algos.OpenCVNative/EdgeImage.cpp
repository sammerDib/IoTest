#include <cmath>
#include <filesystem>
#include <iostream>
#include <opencv2/highgui.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <random>
#include <regex>

#include "CEventQueue.hpp"
#include "CEdgeDetector.hpp"
#include "EdgeImage.hpp"
#include "LoggerOCV.hpp"
#include "Point.hpp"

#include "ErrorLogging.hpp"

#include "CImageTypeConvertor.hpp"

#pragma unmanaged
namespace {
    void drawContours(cv::Mat& image, std::vector<std::vector<cv::Point>> contours) {
        CV_Assert(image.depth() == CV_8U);

        for (int i = 0; i < contours.size(); i++) {
            cv::Scalar color = cv::Scalar(cv::theRNG().uniform(0, 255), cv::theRNG().uniform(0, 255), cv::theRNG().uniform(0, 255));
            drawContours(image, contours, i, color, 2, 8, cv::noArray(), 0, cv::Point());
        }
    }

    std::string RandomString(int stringLength) {
        std::string str(std::string("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"));

        std::random_device rd;
        std::mt19937 generator(rd());

        std::shuffle(str.begin(), str.end(), generator);

        return str.substr(0, stringLength);
    }

    std::string EdgePositionToString(EdgeImage::EdgePosition edgePosition)
    {
        switch (edgePosition)
        {
            case EdgeImage::EdgePosition::BOTTOM:
                return "BOTTOM";
            case EdgeImage::EdgePosition::LEFT:
                return "LEFT";
            case EdgeImage::EdgePosition::TOP:
                return "TOP";
            case EdgeImage::EdgePosition::RIGHT:
                return "RIGHT";
            default:
                return "UNKNOWN";
        }
    }
} // namespace

const double EdgeImage::PROBABLE_OUTSIDE_REGION_HEIGHT_FRACTION = (10. / 100);
const double EdgeImage::PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION = (10. / 100);

const double EdgeImage::WAFER_EDGE_LINE_RMSE_THRESHOLD_300MM = 20.0;
const double EdgeImage::WAFER_EDGE_LINE_RMSE_THRESHOLD_200MM = 30.0;
const double EdgeImage::WAFER_EDGE_LINE_RMSE_THRESHOLD_150MM = 30.0;
const double EdgeImage::WAFER_EDGE_LINE_RMSE_THRESHOLD_100MM = 30.0;

/**
 * Given a contour here with origin relative to upper left corner of field of
 * view, at 0,0. Coordinates of each contour point must be translated so
 * that origin is no longer the upper left corner at 0,0, but the field of view
 * center. Coordinates of that center are provided, as well as total image
 * dimensions.
 * WARNING it iS INDEED in Stage referential not Chuck referential
 */
std::vector<cv::Point2d> EdgeImage::ShiftPointsFromImageToChuckReferential(std::vector<cv::Point2i>& contour) const {
    auto pixelSizeInMicrons = GetPixelSize().get();

    std::stringstream s;
    s << "[EdgeImage::ShiftPointsFromImageToChuckReferential] PixelSize: " << pixelSizeInMicrons;
    LoggerOCV::Debug(s.str());

    cv::Point2d imageOrigin = GetOrigin();
    std::vector<cv::Point2d> shiftedContour;
    shiftedContour.reserve(contour.size());

    // translate the point to the new image origin coordinates
    std::transform(contour.begin(), contour.end(), std::back_inserter(shiftedContour), [&imageOrigin, pixelSizeInMicrons](cv::Point2i& pt) {
        cv::Point2d shiftedPoint;
    shiftedPoint.x = imageOrigin.x + (pt.x * pixelSizeInMicrons.x);
    shiftedPoint.y = imageOrigin.y - (pt.y * pixelSizeInMicrons.y);

    return shiftedPoint;
        });

    return shiftedContour;
}

std::string EdgeImage::GetName() const {
    std::string result = "";
    if (_filename.empty()) {
        static const int FILENAME_LENGTH = 32;
        _filename = RandomString(FILENAME_LENGTH);
    }
    std::filesystem::path path(_filename);
    return path.filename().string();
}

const EdgeImage::PixelSize EdgeImage::GetPixelSize() const { return _pixelSize; }

void EdgeImage::dbgTrace(std::string const& message) { LoggerOCV::Debug(message); }

/*
 *
 * @return image centroid distance to wafer center, in micrometer
 *
 * `[o]` is the image, x and y are coordinates of its centroid relative to wafer center.
 *
 *
 */
const EdgeImage::ImageCentroid EdgeImage::GetCentroid() const { return _center; }

const cv::Point2d EdgeImage::GetDimensionsInMicrometers() const {
    cv::Point2d result;
    result.x = _image.cols * _pixelSize.get().x;
    result.y = _image.rows * _pixelSize.get().y;
    return result;
}

std::vector<cv::Point> EdgeImage::GetContourPoints(int waferDiameterInMm, int edgeDetectionVersion, int cannyThreshold, std::string const& reportPath, bwa::ReportOption reportOption) {
    std::vector<cv::Point> contour;
    switch (edgeDetectionVersion)
    {
        case 2:
            contour = FindWaferBorderV2(waferDiameterInMm, cannyThreshold, reportPath, reportOption);
            break;
        case 1:
        default:
            contour = FindWaferBorder(waferDiameterInMm, reportPath, reportOption);
            break;
        case 0:
            //Alternate version of the detection only used for the PSD
            contour = FindWaferBorderSimple(waferDiameterInMm, reportPath, reportOption);
            break;
    }
    return contour;
}

std::vector<cv::Point2d> EdgeImage::GetShiftedContourPoints(int waferDiameterInMm, int edgeDetectionVersion, int cannyThreshold, std::string const& reportPath, bwa::ReportOption reportOption) {
    auto contour = GetContourPoints(waferDiameterInMm, edgeDetectionVersion, cannyThreshold, reportPath, reportOption);
    auto shiftedContour = ShiftPointsFromImageToChuckReferential(contour);
    return shiftedContour;
}

const EdgeImage::PositionClass EdgeImage::GetPositionClass() const {
    bool isRight = (_center.get().x > 0);
    bool isTop = (_center.get().y > 0);
    if (isRight) {
        if (isTop) {
            return PositionClass::TOP_RIGHT;
        }
        else {
            return PositionClass::BOTTOM_RIGHT;
        }
    }
    else {
        if (isTop) {
            return PositionClass::TOP_LEFT;
        }
        else {
            return PositionClass::BOTTOM_LEFT;
        }
    }
    return _positionClass;
}

const cv::Rect EdgeImage::GetProbableOutsideRegion() const {
    static const int margin = 5;

    cv::Rect region;
    region.width = static_cast<int>(_image.cols * PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION);
    region.height = static_cast<int>(_image.rows * PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION);

    switch (GetPositionClass()) {
    case PositionClass::BOTTOM_LEFT:
        region.x = margin;
        region.y = _image.rows - (margin + region.height);
        break;

    case PositionClass::BOTTOM_RIGHT:
        region.x = _image.cols - (margin + region.width);
        region.y = _image.rows - (margin + region.height);
        break;

    case PositionClass::TOP_LEFT:
        region.x = margin;
        region.y = margin;
        break;

    case PositionClass::TOP_RIGHT:
        region.x = _image.cols - (margin + region.width);
        region.y = margin;
        break;
    }
    return region;
}

const cv::Rect EdgeImage::GetProbableInsideRegion() const {
    static const int margin = 5;

    cv::Rect region;
    region.width = static_cast<int>(_image.cols * PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION);
    region.height = static_cast<int>(_image.rows * PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION);

    switch (GetPositionClass()) {
    case PositionClass::BOTTOM_LEFT:
        region.x = _image.cols - (margin + region.width);
        region.y = margin;
        break;

    case PositionClass::BOTTOM_RIGHT:
        region.x = margin;
        region.y = margin;
        break;

    case PositionClass::TOP_LEFT:
        region.x = _image.cols - (margin + region.width);
        region.y = _image.rows - (margin + region.height);
        break;

    case PositionClass::TOP_RIGHT:
        region.x = margin;
        region.y = _image.rows - (margin + region.height);
        break;
    }
    return region;
}

std::vector<cv::Point> EdgeImage::FindContourContainingTheProbableChuckRegion(std::vector<std::vector<cv::Point>> const& contours, cv::Rect const& probableBackgroundRegion) const {
    std::vector<cv::Point> result;
    cv::Mat tmp = cv::Mat::zeros(_image.size(), CV_8U);

    for (std::vector<cv::Point> const contour : contours) {
        // draw full contoured region (with its content) in `tmp`
        cv::fillConvexPoly(tmp, contour, cv::Scalar(255));

        std::vector<cv::Point> interestRegionPixels;
        cv::findNonZero(tmp, interestRegionPixels);

        // if at least one pixel of the region defined by current contour
        // is inside the probable outside region, we're done
        for (auto& interestRegionPixel : interestRegionPixels) {
            if (probableBackgroundRegion.contains(interestRegionPixel)) {
                result = contour;
                break;
            }
        }
        // "clear" temp matrix
        tmp = 0;
    }
    return result;
}

std::vector<std::vector<cv::Point>> EdgeImage::FilterFoundContours(std::vector<std::vector<cv::Point>> const& contours, PositionClass edgePosition, int waferDiameterInMm) const {
    std::vector<std::vector<cv::Point>> filteredContours;
    //filtering contours that are too small or don't resemble a line
    double rmseThreshold = 0.0;

    switch (waferDiameterInMm)
    {
    case 300:
        rmseThreshold = WAFER_EDGE_LINE_RMSE_THRESHOLD_300MM;
        break;
    case 200:
        rmseThreshold = WAFER_EDGE_LINE_RMSE_THRESHOLD_200MM;
        break;
    case 150:
        rmseThreshold = WAFER_EDGE_LINE_RMSE_THRESHOLD_150MM;
        break;
    case 100:
        rmseThreshold = WAFER_EDGE_LINE_RMSE_THRESHOLD_100MM;
        break;
    default:
        ErrorLogging::LogError("The BWA is not compatible with the current wafer diameter");
        return filteredContours;
    }

    float acceptableRatio = 0.5f;

    for (int i = 0; i < contours.size(); i++)
    {
        cv::Rect contourROI = cv::boundingRect(contours[i]);

        if (contours[i].size() == 0) continue;

        bool acceptable = ((float)contourROI.height >= ((float)_image.rows * acceptableRatio) || (float)contourROI.width >= ((float)_image.cols * acceptableRatio));

        double rmse = lineRMSE(contours[i]);
        acceptable = acceptable && (rmse < rmseThreshold);

        if (acceptable) filteredContours.push_back(contours[i]);
    }
    return filteredContours;
}

double EdgeImage::lineRMSE(std::vector<cv::Point> contour) const {
    cv::Vec4f lineVec;
    double fitLineReps = 0.01;
    double fitLineAeps = 0.01;
    cv::fitLine(contour, lineVec, cv::DIST_WELSCH, 0, fitLineReps, fitLineAeps);

    double sum = 0.;

    if (abs(lineVec[1]) < abs(lineVec[0]))
    {
        for (int i = 0; i < contour.size(); i++) {

            int x = contour[i].x;
            int y = contour[i].y;

            double y2 = int(((x - lineVec[2]) * lineVec[1] / lineVec[0]) + lineVec[3]);

            double diff = y - y2;

            sum += std::pow(diff, 2);
        }
    }
    else
    {
        for (int i = 0; i < contour.size(); i++) {

            int x = contour[i].x;
            int y = contour[i].y;

            double x2 = int(((y - lineVec[3]) * lineVec[0] / lineVec[1]) + lineVec[2]);

            double diff = x - x2;

            sum += std::pow(diff, 2);
        }
    }

    double rmse = sqrt(sum / contour.size());
    return rmse;
}

std::vector<cv::Point> EdgeImage::FindBestContourCandidateForWaferBorder(std::vector<std::vector<cv::Point>> const& contours, int waferDiameterInMm, bool cutContourInHalf) const {
    std::vector<cv::Point> result;
    PositionClass edgePosition = GetPositionClass();

    std::vector<std::vector<cv::Point>> filteredContours = FilterFoundContours(contours, edgePosition, waferDiameterInMm);

    double bestDistanceFromCenter = -1.0;
    cv::Point2d waferCenter(0.0, 0.0);

    //Getting the contour that is the furthest to the wafer center
    for (int i = 0; i < filteredContours.size(); i++)
    {
        cv::Rect contourROI = cv::boundingRect(filteredContours[i]);

        cv::Point2d contourCenter = cv::Point2d((double)contourROI.x + ((double)contourROI.width / 2.0) + _center.get().x, -((double)contourROI.y + ((double)contourROI.height / 2.0)) + _center.get().y);
        double currentDistanceFromCenter = cv::norm(waferCenter - contourCenter);

        if (currentDistanceFromCenter > bestDistanceFromCenter || bestDistanceFromCenter == -1.0)
        {
            bestDistanceFromCenter = currentDistanceFromCenter;
            result = filteredContours[i];
        }
    }

    //The contour is a closed loop but we want a curved line, so we split it in half
    if (cutContourInHalf)
    {
        std::vector<cv::Point> resultHalved(result.begin(), result.begin() + result.size() / 2);
        return resultHalved;
    }
    else
    {
        return result;
    }
}

bool EdgeImage::IsWaferDark()
{
    cv::Rect insideRoi = GetProbableInsideRegion();
    cv::Rect outsideRoi = GetProbableOutsideRegion();

    cv::Scalar insideMean = cv::mean(_image(insideRoi));
    cv::Scalar outsideMean = cv::mean(_image(outsideRoi));

    return insideMean[0] < outsideMean[0];
}

cv::Mat EdgeImage::RotateEdgeToVertical(cv::Mat const img, int waferDiameterInMm, double& edgeDirection)
{
    cv::Mat filtered;
    cv::Mat binary;
    cv::Mat rotated;
    cv::Mat can;

    int baseFilter = 5;
    int cannyLowThreshold = 80;
    int cannyHighThreshold = 150;

    cv::bilateralFilter(img, filtered, baseFilter, baseFilter, baseFilter);
    cv::medianBlur(filtered, filtered, baseFilter * 3);

    cv::threshold(filtered, binary, 0, 255, cv::THRESH_OTSU);


    cv::Canny(binary, can, cannyLowThreshold, cannyHighThreshold);

    std::vector<std::vector<cv::Point>> tempContours;
    cv::findContours(can, tempContours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_NONE);

    std::vector<cv::Point> rectContour = FindBestContourCandidateForWaferBorder(tempContours, waferDiameterInMm);
    if (rectContour.size() == 0) return rotated;

    cv::RotatedRect rotRect = cv::minAreaRect(rectContour);

    cv::Point2f vertices[4];
    rotRect.points(vertices);

    //The direction of the wafer border is the angle of the rotated rectangle containing the best contour
    edgeDirection = rotRect.angle;

    bool isLong = (rotRect.size.height > rotRect.size.width); // Top-Left or Bottom-Right

    int quadrant = 0;
    

    //Image is in the Top-Left quadrant :     0
    //Image is in the Top-Right quadrant :    1
    //Image is in the Bottom-Left quadrant :  2
    //Image is in the Bottom-Right quadrant : 3

    cv::Point firstCorner  = cv::Point(static_cast<int>((double)vertices[0].x + GetOrigin().x), static_cast<int>(GetOrigin().y - (double)vertices[0].y));
    cv::Point secondCorner = cv::Point(static_cast<int>((double)vertices[1].x + GetOrigin().x), static_cast<int>(GetOrigin().y - (double)vertices[1].y));
    cv::Point thirdCorner  = cv::Point(static_cast<int>((double)vertices[2].x + GetOrigin().x), static_cast<int>(GetOrigin().y - (double)vertices[2].y));
    cv::Point origin = cv::Point(0, 0);

    if (isLong)
    {
        double secondDist = cv::norm(origin - secondCorner);
        double thirdDist = cv::norm(origin - thirdCorner);

        //Top-Left
        if (secondDist > thirdDist) quadrant = 0;
        //Bottom-Right
        else quadrant = 3;
    }
    else
    {
        double firstDist = cv::norm(origin - firstCorner);
        double secondDist = cv::norm(origin - secondCorner);

        //Bottom-Left
        if (firstDist > secondDist) quadrant = 2;
        //Top-Right
        else quadrant = 1;
    }

    //Changing the angle so that the wafer is always on the left side of the image (no need to change on quadrant = 3 / Bottom-Right)
    switch (quadrant)
    {
        case 1:
            edgeDirection += 270;
            break;
        case 2:
            edgeDirection += 90;
            break;
        case 0:
            edgeDirection += 180;
            break;
    }
    
    cv::Point tempCenter = cv::Point(img.cols / 2, img.rows / 2);
    cv::Mat rotationMat = cv::getRotationMatrix2D(tempCenter, edgeDirection, 1.0);

    cv::warpAffine(_image, rotated, rotationMat, cv::Size(img.cols, img.rows), cv::INTER_LINEAR, cv::BORDER_REPLICATE);

    return rotated;
}

cv::Rect EdgeImage::GetVerticalBorderROI(cv::Mat rotated, bool outerBorder)
{
    std::vector<double> means, stddevs;

    for (int i = 0; i < rotated.cols; i++)
    {
        cv::Mat col = rotated.col(i).clone();
        cv::Mat mean, stddev;
        meanStdDev(col, mean, stddev);
        means.push_back(mean.at<double>(0, 0));
        stddevs.push_back(stddev.at<double>(0, 0));
    }

    //Might want to change this value if we're not using 5x images
    int meanWindowRatio = 150;
    int meanWindowSize = rotated.cols / meanWindowRatio;
    double slopeThreshold = 0.9;
    double lastSlope = 0.0;

    std::vector<int> downSlopePoints;

    for (int x = meanWindowSize; x < means.size() - meanWindowSize; x++)
    {
        double slope = (means[x + meanWindowSize] - means[x - meanWindowSize]) / ((x + meanWindowSize) - (x - meanWindowSize));
        bool bigSlope = (slope >= slopeThreshold || slope <= -slopeThreshold);
        bool opposite = (slope < 0) != (lastSlope < 0);
        if (lastSlope == 0.0) opposite = true;
        if (bigSlope && opposite)
        {
            if (slope < 0)
            {
                downSlopePoints.push_back(x);
            }
        }
        if (bigSlope) lastSlope = slope;
    }

    int borderIndex = outerBorder ? ((int)downSlopePoints.size()) - 1 : 0;

    int borderRoiRatio = 15;
    cv::Rect borderRoi = cv::Rect(rotated.cols - (rotated.cols / borderRoiRatio), 0, (rotated.cols / borderRoiRatio) - 1, rotated.rows - 1);

    if (downSlopePoints.size() - 1 < borderIndex || downSlopePoints.size() == 0) return borderRoi;

    int borderRoiWindowSize = std::min({ rotated.cols / borderRoiRatio, rotated.cols - downSlopePoints[borderIndex], downSlopePoints[borderIndex] });

    borderRoi = cv::Rect(downSlopePoints[borderIndex] - (borderRoiWindowSize / 2), 0, borderRoiWindowSize, rotated.rows - 1);

    
    return borderRoi;

}

std::vector <cv::Point> EdgeImage::FindWaferBorderV2(int waferDiameterInMm, int cannyThreshold, std::string const& reportPath, bwa::ReportOption reportOption)
{
    cv::Mat gray;
    gray = Convertor::ConvertTo8UC1(_image);

    cv::Mat dx, dy, canny, closedCanny;

    double cannyCoefficient = 0.5;
    cv::Scharr(gray, dx, CV_16S, 1, 0);
    cv::Scharr(gray, dy, CV_16S, 0, 1);

    cv::Canny(dx, dy, canny, std::max(1, int(cannyThreshold * cannyCoefficient)), cannyThreshold);

    //Closing the canny contours and filling the insides to make the contour detection easier
    int numberOfCloseIterations = 5;
    int closeMorphoSize = 15;
    cv::morphologyEx(canny, closedCanny, cv::MORPH_CLOSE, cv::getStructuringElement(cv::MORPH_ELLIPSE, cv::Size(closeMorphoSize, closeMorphoSize)), cv::Point(-1, -1), numberOfCloseIterations);

    cv::floodFill(closedCanny, cv::Point(GetProbableInsideRegion().x, GetProbableInsideRegion().y), cv::Scalar(255, 255, 255));

    cv::Mat labels;
    cv::Mat stats;
    cv::Mat centroids;
    int nbOfConnectedComponents = cv::connectedComponentsWithStats(closedCanny, labels, stats, centroids, 8);

    std::vector<std::vector<cv::Point>> contours;
    std::vector<std::vector<cv::Point>> contoursNoImageBorder;

    //Find contours for each connected components :
    //Drawing lines on the images borders to get good contour detection results (OpenCV gives weird contours otherwise)
    int lineThickness = 5;

    for (int label = 0; label < nbOfConnectedComponents; label++)
    {
        bool isConnectedComponentBig = (stats.at<int>(label, cv::CC_STAT_WIDTH) > _image.cols / 2) || (stats.at<int>(label, cv::CC_STAT_HEIGHT) > _image.rows / 2);
        if (isConnectedComponentBig)
        {
            std::vector<std::vector<cv::Point>> connectedComponentContour;
            cv::Mat connectedComponentCanny;
            cv::inRange(labels, cv::Scalar(label), cv::Scalar(label), connectedComponentCanny);
            cv::line(connectedComponentCanny, cv::Point(0, 0), cv::Point(closedCanny.cols, 0), cv::Scalar(255), lineThickness);
            cv::line(connectedComponentCanny, cv::Point(closedCanny.cols, 0), cv::Point(closedCanny.cols, closedCanny.rows), cv::Scalar(255), lineThickness);
            cv::line(connectedComponentCanny, cv::Point(closedCanny.cols, closedCanny.rows), cv::Point(0, closedCanny.rows), cv::Scalar(255), lineThickness);
            cv::line(connectedComponentCanny, cv::Point(0, closedCanny.rows), cv::Point(0, 0), cv::Scalar(255), lineThickness);
            cv::findContours(connectedComponentCanny, connectedComponentContour, cv::RETR_CCOMP, cv::CHAIN_APPROX_NONE);
            contours.insert(contours.begin(), connectedComponentContour.begin(), connectedComponentContour.end());
        }
    }

    for (int i = 0; i < contours.size(); i++)
    {
        contoursNoImageBorder.push_back(RemovePointsTouchingBorders(contours[i]));
    }

    std::vector<cv::Point> bestContour = FindBestContourCandidateForWaferBorder(contoursNoImageBorder, waferDiameterInMm, false);

    //Reporting overlay images
    if (!reportPath.empty() && reportOption > 0) // OverlayReport
    {
        std::string edgePositionString = EdgePositionToString(_edgePosition);
        cv::Mat reportContourMat;
        cv::cvtColor(_image.clone(), reportContourMat, cv::COLOR_GRAY2BGR);
        auto contourColor = cv::Scalar(0, 0, 255);
        cv::polylines(reportContourMat, bestContour, false, contourColor);
        cv::imwrite(reportPath + "/" + edgePositionString + "_contour.png", reportContourMat);

        //Reporting extra images
        if (reportOption > 1) // AdvancedOverlayReport
        {
            cv::Mat absx, absy;
            cv::convertScaleAbs(dx, absx);
            cv::convertScaleAbs(dy, absy);
            cv::Mat edges;
            cv::addWeighted(absx, 0.5, absy, 0.5, 0, edges);
            cv::imwrite(reportPath + "/" + edgePositionString + "_scharr.png", edges);
            cv::imwrite(reportPath + "/" + edgePositionString + "_scharrx.png", dx);
            cv::imwrite(reportPath + "/" + edgePositionString + "_scharry.png", dy);
            cv::imwrite(reportPath + "/" + edgePositionString + "_canny.png", canny);
            cv::imwrite(reportPath + "/" + edgePositionString + "_closedcanny.png", closedCanny);

            std::vector<cv::Scalar> randomColors(contoursNoImageBorder.size());
            cv::randu(randomColors, cv::Scalar(0, 0, 0), cv::Scalar(255, 255, 255));
            cv::Mat contoursBeforeFiltering;
            cv::cvtColor(gray, contoursBeforeFiltering, cv::COLOR_GRAY2BGR);
            for (int i = 0; i < contoursNoImageBorder.size(); i++)
            {
                cv::polylines(contoursBeforeFiltering, contoursNoImageBorder[i], false, randomColors[i]);
            }
            cv::imwrite(reportPath + "/" + edgePositionString + "_contoursbeforefiltering.png", contoursBeforeFiltering);
        }
    }

    return bestContour;

}


std::vector<cv::Point> EdgeImage::FindWaferBorder(int waferDiameterInMm, std::string const& reportPath, bwa::ReportOption reportOption, bool outerBorder) {
#if !defined(NDEBUG) && defined(SHOW_IMAGES)
    cv::imshow(GetName() + " input", _image);
#endif

    cv::Mat src;
    cv::Mat gray;
    cv::Mat binary;
    std::vector<cv::Point> edgeContour;

    gray = Convertor::ConvertTo8UC1(_image);

    cv::Rect probableChuckRegion = GetProbableOutsideRegion();
    auto probableChuckROI = gray(probableChuckRegion);
    cv::Scalar probableChuckROIMean;
    cv::Scalar probableChuckROIStdDev;
    cv::meanStdDev(probableChuckROI, probableChuckROIMean, probableChuckROIStdDev);
    if (probableChuckROIMean[0] < 127) {
        LoggerOCV::Debug("[EdgeImage] The chuck region is dark");
        // non-wafer region is dark, we
        // have to invert the image because "findcontours"
        // will "contour" bright regions
        gray = 255 - gray;
    }
    else {
        LoggerOCV::Debug("[EdgeImage] The chuck region is bright");
    }

    if (probableChuckROIStdDev[0] > 10) {
        LoggerOCV::Warning("[EdgeImage] The probable chuck ROI is not uniform");
    }

#if defined(USE_MORPH_OPERATOR)
    // illumination correction top-hat
    auto kernel = cv::getStructuringElement(cv::MORPH_ELLIPSE, cv::Size(30, 30));
    cv::morphologyEx(gray, gray, cv::MORPH_TOPHAT, kernel);

    cv::Mat dilated;
    int dilation_size = 3;
    cv::Mat element = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(2 * dilation_size + 1, 2 * dilation_size + 1), cv::Point(dilation_size, dilation_size));
    cv::dilate(gray, dilated, element);

    cv::Mat eroded;
    int erosion_size = 3;
    element = cv::getStructuringElement(cv::MORPH_RECT, cv::Size(2 * erosion_size + 1, 2 * erosion_size + 1), cv::Point(erosion_size, erosion_size));
    cv::erode(dilated, eroded, element);
    gray = eroded;

#if !defined(NDEBUG) && defined(SHOW_IMAGES)
    cv::imshow(GetName() + " morphed", gray);
#endif
#endif

    double edgeDirection = 0.0;
    //Rotating the image so the edge is vertical for easier border detect
    cv::Mat rotated = RotateEdgeToVertical(gray, waferDiameterInMm, edgeDirection);
    if (rotated.empty()) return edgeContour;

    //Finding the slice of the rotated image where the potential border is
    cv::Rect borderRoi = GetVerticalBorderROI(rotated, outerBorder);
    cv::Mat borderSection = rotated(borderRoi);
    
    //Filtering for easier contour detection
    int cannyLowThreshold = 80;
    int cannyHighThreshold = 150;
    int baseFilter = 5;
    cv::Mat filteredBorderSection;
    cv::bilateralFilter(borderSection, filteredBorderSection, baseFilter, baseFilter, baseFilter);
    cv::medianBlur(filteredBorderSection, filteredBorderSection, baseFilter * 3);
    cv::Mat binaryBorderSection;
    cv::threshold(filteredBorderSection, binaryBorderSection, 0, 255, cv::THRESH_OTSU);
    cv::Mat canBorderSection;
    cv::Canny(binaryBorderSection, canBorderSection, cannyLowThreshold, cannyHighThreshold);

    //Getting the best contour on the rotated image slice
    std::vector<std::vector<cv::Point>> rotatedContours;
    cv::findContours(canBorderSection, rotatedContours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_NONE);
    std::vector<cv::Point> rotatedBestContour = FindBestContourCandidateForWaferBorder(rotatedContours, waferDiameterInMm);

    cv::Point tempCenter = cv::Point(gray.cols / 2, gray.rows / 2);

    cv::Mat rotationMat = cv::getRotationMatrix2D(tempCenter, -edgeDirection, 1.0);

    //Shifting the contour coordinates to the rotated image referential
    for (int i = 0; i < rotatedBestContour.size(); i++)
    {
        rotatedBestContour[i] = cv::Point(rotatedBestContour[i].x + borderRoi.x, rotatedBestContour[i].y + borderRoi.y);
    }

    //Shifting the contour coordinates to the original image referential
    if (rotatedBestContour.size() > 0) cv::transform(rotatedBestContour, edgeContour, rotationMat);

    #if !defined(NDEBUG) && defined(SHOW_IMAGES)
        cv::imshow(GetName() + " rotated", rotated);
        cv::imshow(GetName() + " borderSection", borderSection);
        cv::imshow(GetName() + " binaryBorderSection", binaryBorderSection);
    #endif

    if (!reportPath.empty() && rotatedBestContour.size() > 0) {
        std::vector<std::vector<cv::Point>> colourContours = { rotatedBestContour };
        cv::Mat colour(gray.size(), CV_8UC3);
        drawContours(colour, colourContours);
        #if !defined(NDEBUG) && defined(SHOW_IMAGES)
            cv::imshow(GetName() + " contours", colour);
        #endif
    }

    //Report overlay images
    if (!reportPath.empty() && reportOption > 0)
    {
        std::string edgePositionString = EdgePositionToString(_edgePosition);
        cv::Mat reportContourMat;
        cv::cvtColor(_image.clone(), reportContourMat, cv::COLOR_GRAY2BGR);
        auto contourColor = cv::Scalar(0, 0, 255);
        cv::polylines(reportContourMat, edgeContour, false, contourColor);
        cv::imwrite(reportPath + "/" + edgePositionString + "_contour.png", reportContourMat);
    }
    
    return edgeContour;
}

std::vector<cv::Point> EdgeImage::FindWaferBorderSimple(int waferDiameterInMm, std::string const& reportPath, bwa::ReportOption reportOption) {

    cv::Mat src;
    cv::Mat gray;
    cv::Mat binary;
    std::vector<cv::Point> edgeContour;

    gray = Convertor::ConvertTo8UC1(_image);

    cv::Rect probableChuckRegion = GetProbableOutsideRegion();
    auto probableChuckROI = gray(probableChuckRegion);
    cv::Scalar probableChuckROIMean;
    cv::Scalar probableChuckROIStdDev;
    cv::meanStdDev(probableChuckROI, probableChuckROIMean, probableChuckROIStdDev);
    if (probableChuckROIMean[0] < 127) {
        LoggerOCV::Debug("[EdgeImage] The chuck region is dark");
        // non-wafer region is dark, we
        // have to invert the image because "findcontours"
        // will "contour" bright regions
        gray = 255 - gray;
    }
    else {
        LoggerOCV::Debug("[EdgeImage] The chuck region is bright");
    }

    if (probableChuckROIStdDev[0] > 10) {
        LoggerOCV::Warning("[EdgeImage] The probable chuck ROI is not uniform");
    }

    //Filtering for easier contour detection
    int cannyLowThreshold = 80;
    int cannyHighThreshold = 150;
    int baseFilter = 5;
    cv::Mat filteredBorderSection;
    cv::bilateralFilter(gray, filteredBorderSection, baseFilter, baseFilter, baseFilter);
    cv::medianBlur(filteredBorderSection, filteredBorderSection, baseFilter * 3);
    cv::Mat binaryBorderSection;
    cv::threshold(filteredBorderSection, binaryBorderSection, 0, 255, cv::THRESH_OTSU);
    cv::Mat canBorderSection;
    cv::Canny(binaryBorderSection, canBorderSection, cannyLowThreshold, cannyHighThreshold);

    //Getting the best contour on the image
    std::vector<std::vector<cv::Point>> canContours;
    cv::findContours(canBorderSection, canContours, cv::RETR_EXTERNAL, cv::CHAIN_APPROX_NONE);
    edgeContour = FindBestContourCandidateForWaferBorder(canContours, waferDiameterInMm);

    //Report overlay images
    if (!reportPath.empty() && reportOption > 0)
    {
        std::string edgePositionString = EdgePositionToString(_edgePosition);
        cv::Mat reportContourMat;
        cv::cvtColor(_image.clone(), reportContourMat, cv::COLOR_GRAY2BGR);
        auto contourColor = cv::Scalar(0, 0, 255);
        cv::polylines(reportContourMat, edgeContour, false, contourColor);
        cv::imwrite(reportPath + "/" + edgePositionString + "_contour.png", reportContourMat);
    }

    return edgeContour;
}

std::vector<cv::Point> EdgeImage::RemovePointsTouchingBorders(std::vector<cv::Point> const& initial) const {
    // Some buggy points appear when we are close to borders of the image.
    // The workaround is for now to remove points closer to 20px of each border.
    const int OPENCV_FINDCONTOUR_FIX = 20;
    std::vector<cv::Point> filtered;
    auto isPixelFarEnoughFromImageBorder = [this, OPENCV_FINDCONTOUR_FIX](cv::Point const& point) {
        bool tooCloseOfLeft = (point.x < OPENCV_FINDCONTOUR_FIX);
        bool tooCloseOfBottom = (point.y > (_image.rows - OPENCV_FINDCONTOUR_FIX));
        bool tooCloseOfTop = (point.y < OPENCV_FINDCONTOUR_FIX);
        bool tooCloseOfRight = (point.x > _image.cols - OPENCV_FINDCONTOUR_FIX);
        return (!tooCloseOfLeft && !tooCloseOfBottom && !tooCloseOfTop && !tooCloseOfRight);
    };
    std::copy_if(initial.begin(), initial.end(), std::back_inserter(filtered), isPixelFarEnoughFromImageBorder);
    return filtered;
}

bool EdgeImage::Empty() { return _image.empty(); }

cv::Mat EdgeImage::WithWaferEdge(int waferDiameterInMm) {
    cv::Mat imageWithContours;
    cv::cvtColor(_image.clone(), imageWithContours, cv::COLOR_GRAY2BGR);
    auto green = cv::Scalar(0, 255, 0);
    auto blue = cv::Scalar(255, 0, 0);
    auto contour = FindWaferBorder(waferDiameterInMm, "");
    cv::polylines(imageWithContours, contour, false, green);
    cv::rectangle(imageWithContours, GetProbableOutsideRegion(), blue);
    return imageWithContours;
}

EdgeImage::MinAndMax EdgeImage::FindOptimalMinAndMaxRadius(cv::Point2d actualWaferCenterPosition) {

    MinAndMax result{ 0, 0 };
    const cv::Point2d centroidAtZero(0, 0);
    cv::Point2d imageCentroid = GetCentroid().get();
    cv::Point2d origin = GetOrigin();

    if (imageCentroid == centroidAtZero) {
        result.min = 0;
        result.max = cv::norm(GetOrigin());
    }
    else {
        result.min = std::numeric_limits<long>::max();
        result.max = 0;
        double scaledHalfHeightOfImage = ((Mat().rows * GetPixelSize().get().y) / 2) - 0.5 * GetPixelSize().get().x;
        double lastLineYPosition = imageCentroid.y - scaledHalfHeightOfImage;
        for (size_t colIndex = 0; colIndex < Mat().cols; colIndex++) {
            cv::Point2d currentPoint = origin;
            currentPoint.x += colIndex * GetPixelSize().get().x;
            if (colIndex > 0) {
                currentPoint.x -= 0.5 * GetPixelSize().get().x;
            }
            double distanceToCenter = cv::norm(currentPoint - actualWaferCenterPosition);
            if (distanceToCenter < result.min) {
                result.min = distanceToCenter;
            }
            currentPoint.y = lastLineYPosition;
            distanceToCenter = cv::norm(currentPoint - actualWaferCenterPosition);
            if (distanceToCenter > result.max) {
                result.max = distanceToCenter;
            }
        }
    }
    return result;
}

EdgeImage::MinAndMax EdgeImage::FindOptimalMinAndMaxAngles(cv::Point2d actualWaferCenterPosition) {

    MinAndMax result{ 0, 0 };
    std::pair<double, double> minAndMaxAngles;
    cv::Point imageCentroid = GetCentroid().get();
    bool centerIsInsideTheImage = (imageCentroid.x > 0 && imageCentroid.x < Mat().cols&& imageCentroid.y > 0 && imageCentroid.y < Mat().rows);
    if (centerIsInsideTheImage) {
        result.min = 0;
        result.max = CV_PI * 2;
    }
    else {

        /*
         *
         *            + wafer center
         *               / \
         *              /    \
         *            /        \
         *           /           \
         *          /              \
         * -----(A)---------(B)----- <-- named points used for angle computation
         *         |                |
         *         |                |
         * ....../-|-\..........    | <-- wafer edge and notch
         *         |                |
         * -----(C) ---------(D)
         *
         * The function will search min and max angle between wafer center and these four points.
         */

        cv::Mat src = Mat();
        cv::Point2d imageOrigin = GetOrigin();
        cv::Point2d imageCentroid = GetCentroid().get();

        double halfPixelX = GetPixelSize().get().x / 2;
        double halfPixelY = GetPixelSize().get().y / 2;

        double scaledHalfWidth = (src.cols / 2 * GetPixelSize().get().x) - halfPixelX;
        double scaledHalfHeight = (src.rows / 2 * GetPixelSize().get().y) + halfPixelY;

        cv::Point2d cartesianPointA = imageOrigin;
        cv::Point2d cartesianPointB = cv::Point2d(imageCentroid.x + scaledHalfWidth, imageOrigin.y);
        cv::Point2d cartesianPointC = cv::Point2d(imageOrigin.x, imageCentroid.y - scaledHalfHeight);
        cv::Point2d cartesianPointD = cv::Point2d(imageCentroid.x + scaledHalfWidth, imageOrigin.y - scaledHalfHeight);

        cv::Point2d polarPointA(Point::CartesianToPolar(cartesianPointA - actualWaferCenterPosition));
        cv::Point2d polarPointB(Point::CartesianToPolar(cartesianPointB - actualWaferCenterPosition));
        cv::Point2d polarPointC(Point::CartesianToPolar(cartesianPointC - actualWaferCenterPosition));
        cv::Point2d polarPointD(Point::CartesianToPolar(cartesianPointD - actualWaferCenterPosition));

        double thetaA = polarPointA.y;
        double thetaB = polarPointB.y;
        double thetaC = polarPointC.y;
        double thetaD = polarPointD.y;


        result.min = std::min(std::min(thetaA, thetaB), std::min(thetaC, thetaD));
        result.max = std::max(std::max(thetaA, thetaB), std::max(thetaC, thetaD));
    }
    return result;
}