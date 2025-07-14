#pragma once
#include <iostream>
#include <memory>
#include <opencv2/core.hpp>
#include <opencv2/highgui.hpp>

#include "NamedType.hpp"
#include "BWAReportOption.hpp"

/*
 * Contains information about wafer edge: This kind of image is used for "blank
 * wafer alignment" operation. N images (N >= 3) of wafer edge are used to fit a
 * circle and obtain its radius and real center position.
 * The radius should be very close of expected wafer type (ie 300mm, 200mm).
 * The real center should be close of chuck center (at 0,0), with a small translation.
 * This translation will be used further to correct measurements coordinates.
 *
 * NOTE: EdgeImage is provided at early stage and uses chuck coordinates, in micrometers.
 * It means provided image center is relative to chuck center, in micrometers.
 * To allow wafer circle fit, image dimension has to be converted to micrometers using
 * pixel size information provided by the used objective (ie 5x, 50x and so on).
 *
 * \NOTE: the image stored internally must use the CV_8U format, by convention.
 */
#pragma unmanaged
class EdgeImage {

protected:
    struct PixelSizeTag {};
    struct ImageCenterTag {};

public:
    // PixelSize must be expressed as micrometers
    using PixelSize = NamedType<cv::Point2d, EdgeImage::PixelSizeTag>;
    using ImageCentroid = NamedType<cv::Point2d, EdgeImage::ImageCenterTag>;
    using Pointer = std::shared_ptr<EdgeImage>;

    /*
     * When a sample region is extracted for "outside" detection,
     * its size is calculated using a percentage of image dimension.
     * That percentage is provided for image width and height
     */
    static const double PROBABLE_OUTSIDE_REGION_HEIGHT_FRACTION;

    /*
     * When a sample region is extracted for "outside" detection,
     * its size is calculated using a percentage of image dimension.
     * That percentage is provided for image width and height
     */
    static const double PROBABLE_OUTSIDE_REGION_WIDTH_FRACTION;

    /*
     * The shape of the wafer border is different depending on the wafer diameter
     * (For example : the 300mm border almost looks like a straight line in the image
     *  whereas the 100mm looks more like a curve of a circle)
     * Therefore, a different threshold for line ressemblance is needed depending on the wafer diameter.
     */
    static const double WAFER_EDGE_LINE_RMSE_THRESHOLD_300MM;
    static const double WAFER_EDGE_LINE_RMSE_THRESHOLD_200MM;
    static const double WAFER_EDGE_LINE_RMSE_THRESHOLD_150MM;
    static const double WAFER_EDGE_LINE_RMSE_THRESHOLD_100MM;


    /*!
     * An edge image must be in one of these four quadrants
     */
    enum class PositionClass { UNKNOWN, BOTTOM_RIGHT, BOTTOM_LEFT, TOP_LEFT, TOP_RIGHT };

    enum class EdgePosition : int {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
    };

    static Pointer New(std::string imageFileName, PixelSize& pixelSize, ImageCentroid const& center) { 
        return Pointer(new EdgeImage(imageFileName, pixelSize, center)); }

    static Pointer New(cv::Mat data, PixelSize& pixelSize, ImageCentroid const& center) { 
        return Pointer(new EdgeImage("", data, pixelSize, center)); }

    static Pointer New(cv::Mat data, PixelSize& pixelSize, ImageCentroid const& center, EdgePosition const& edgePosition) {
        return Pointer(new EdgeImage("", data, pixelSize, center, edgePosition));
    }

    static Pointer New(std::string imageName, cv::Mat data, PixelSize& pixelSize, ImageCentroid const& center) { 
        return Pointer(new EdgeImage(imageName, data, pixelSize, center)); }


    const PixelSize GetPixelSize() const;
    const cv::Point2d GetDimensionsInMicrometers() const;
    const PositionClass GetPositionClass() const;
    std::string GetName() const;
    bool Empty();

    /*!
     * @return image centroid coordinates, before any correction
     */
    const ImageCentroid GetCentroid() const;

    /*
     * @return image origin (upper left corner) in the chuck referential
     */
    const cv::Point2d GetOrigin() const {
        auto imageCentroid = GetCentroid().get();
        auto pixelSize = GetPixelSize().get();
        // when moving up and left, X diminishes, and Y increases
        double xShift = (std::ceil(_image.cols / 2) - 0.5) * pixelSize.x;
        double yShift = (std::ceil(_image.rows / 2) - 0.5) * pixelSize.y;

        cv::Point2d origin(imageCentroid.x - xShift, imageCentroid.y + yShift);
        return origin;
    }

    cv::Mat const& Mat() const { return _image; }

    /*!
     * Given image classification (top-left, bottom-left..) we can deduce that
     * some region in the corner of the image farthest from the center of the
     * wafer will contain chuck (and no wafer) pixels
     */
    const cv::Rect GetProbableOutsideRegion() const;

    /*!
     * Given image classification (top-left, bottom-left..) we can deduce that
     * some region in the corner of the image closest from the center of the
     * wafer will contain wafer (and no chuck) pixels
     */
    const cv::Rect GetProbableInsideRegion() const;

    /*!
     * Given image classification (top-left, bottom-left..) we can deduce that
     * if the wafer is darker than the chuck
     */
    bool IsWaferDark();

    /*!
     *
     * Return extracted wafer contour points in the wafer referential
     *
     * NOTE: only image size and center position are used to shift points position.
     */
    std::vector<cv::Point2d> GetShiftedContourPoints(int waferDiameterInMm, int edgeDetectionVersion, int cannyThreshold, std::string const& reportPath, bwa::ReportOption reportOption = bwa::ReportOption::OverlayReport);

    /*!
     *
     * \return extracted wafer contour points in the image referential
     */
    std::vector<cv::Point> GetContourPoints(int waferDiameterInMm, int edgeDetectionVersion, int cannyThreshold, std::string const& reportPath, bwa::ReportOption reportOption = bwa::ReportOption::OverlayReport);

    /**
     * Return source image with fitted wafer border and chuck region
     */
    cv::Mat WithWaferEdge(int waferDiameterInMm);

    /*!
     * When the wafer contour is detected, its points coordinates are relative to image origin, at upper left corner.
     * This method shifts these coordinates to make them relative to theorical wafer center at (0,0).
     */
    std::vector<cv::Point2d> ShiftPointsFromImageToChuckReferential(std::vector<cv::Point2i>& points) const;

    struct MinAndMax {
        double min;
        double max;
    };

    MinAndMax FindOptimalMinAndMaxRadius(cv::Point2d actualWaferCenterPosition);

    // Compute an angle range so that all image pixel will be projected in the polar image
    MinAndMax FindOptimalMinAndMaxAngles(cv::Point2d actualWaferCenterPosition);

protected:
    cv::Mat _image;
    EdgePosition _edgePosition;
    // can be changed from the inside
    mutable std::string _filename;

    PixelSize _pixelSize;
    ImageCentroid _center;
    PositionClass _positionClass;

    explicit EdgeImage(std::string const& imageFileName, PixelSize const& PixelSize, ImageCentroid const& imageCenter)
        : _filename(imageFileName), _pixelSize(PixelSize), _center(imageCenter), _positionClass(PositionClass::UNKNOWN) {

        _image = cv::Mat();
        bool retval = cv::haveImageReader(_filename);
        if (retval)
        {
        _image = cv::imread(_filename, cv::ImreadModes::IMREAD_GRAYSCALE);
        }
         dbgTrace("[EdgeImage] Loaded new '" + imageFileName + "' instance");
    }

    explicit EdgeImage(std::string const& imageName, cv::Mat& data, PixelSize const& PixelSize, ImageCentroid const& imageCenter) 
        : _filename(imageName), _image(data), _pixelSize(PixelSize), _center(imageCenter), _positionClass(PositionClass::UNKNOWN)
    { 
        dbgTrace("[EdgeImage] Create new '" + imageName + "' instance"); 
    }

    explicit EdgeImage(std::string const& imageName, cv::Mat& data, PixelSize const& PixelSize, ImageCentroid const& imageCenter, EdgePosition const& edgePosition)
        : _filename(imageName), _image(data), _pixelSize(PixelSize), _center(imageCenter), _edgePosition(edgePosition), _positionClass(PositionClass::UNKNOWN)
    {
        dbgTrace("[EdgeImage] Create new '" + imageName + "' instance");
    }

private:
    /*!
     * Detect wafer border in the image 
     * By default the function will try to detect the outer border of the wafer, change outerBorder to false if the border is a little bit on the inside of the wafer
     */
    std::vector<cv::Point> FindWaferBorder(int waferDiameterInMm, std::string const& reportPath, bwa::ReportOption reportOption = bwa::ReportOption::OverlayReport, bool outerBorder = true);
    std::vector<cv::Point> FindWaferBorderV2(int waferDiameterInMm, int cannyThreshold, std::string const& reportPath, bwa::ReportOption reportOption = bwa::ReportOption::OverlayReport);
     /*!
     * Simpler and faster version of the FindWaferBorder method
     * It is faster, but the search for the border is less extensive so some results might be slightly innacurate on complicated wafers
     */
    std::vector<cv::Point> FindWaferBorderSimple(int waferDiameterInMm, std::string const& reportPath, bwa::ReportOption reportOption = bwa::ReportOption::OverlayReport);

    /*!
     * Filtering method which removes points touching border from a point list
     */
    std::vector<cv::Point> RemovePointsTouchingBorders(std::vector<cv::Point> const& initial) const;

    /*!
     * When searching wafer edge, the algorithm collects contour of objects in the image.
     * The only one we want to retain is the one containing the "probable chuck region",
     * a region containing only chuck and no wafer pixels.
     */
    std::vector<cv::Point> FindContourContainingTheProbableChuckRegion(std::vector<std::vector<cv::Point>> const& contours, cv::Rect const& outsideRegion) const;
    /*!
     * Filters contours that are too small or don't resemble a line
     */
    std::vector<std::vector<cv::Point>> FilterFoundContours(std::vector<std::vector<cv::Point>> const& contours, PositionClass edgePosition, int waferDiameterInMm) const;
    /*!
     * Finds the contour that has the highest chance of being the wafer border
     */
    std::vector<cv::Point> FindBestContourCandidateForWaferBorder(std::vector<std::vector<cv::Point>> const& contours, int waferDiameterInMm, bool cutContourInHalf = true) const;
    /*!
     * Computes the rmse between a contour and a line fitted onto it
     */
    double lineRMSE(std::vector<cv::Point> contour) const;
    /*!
     * Rotates an image so the wafer border is vertical
     */
    cv::Mat RotateEdgeToVertical(cv::Mat const img, int waferDiameterInMm, double& edgeDirection);
    /*!
     * Computes a slice of the rotated image where the potential wafer border might be located
     */
    cv::Rect GetVerticalBorderROI(cv::Mat rotated, bool outerBorder);

    void dbgTrace(std::string const& message);
};
