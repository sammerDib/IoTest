#include "CheckerBoardsDetector.hpp"
#include "CImageTypeConvertor.hpp"
#include <opencv2/calib3d.hpp>
#include "ReportingUtils.hpp"

#include <map>

using namespace std;
using namespace cv;

namespace psd {
    namespace {
        /**
        * Finds a cosine of angle between vectors. From pt0->pt1 and from pt0->pt2
        */
        double angle(Point pt1, Point pt2, Point pt0);

        /**
        * Returns sequence of squares detected on the image.
        */
        void findSquares(const Mat& image, vector<vector<Point> >& squares);

        /**
        * Find position of checker board between four available positions on calibration wafer
        */
        CheckerBoardPosition findCheckerBoardPosition(cv::Size waferImgSize, cv::Point topLeftCornerPosition, cv::Size checkerBoardSize);

        /**
        * Compute real corners position of given checker board on calibration wafer
        */
        vector<Point3f> checkerBoardCornersOnCalibrationWafer(CheckerBoardsSettings checkerBoardsSettings, CheckerBoardPosition checkerBoardPosition);

        /**
        * For debug purpose
        */
        void showCheckerBoard(CheckerBoard checkerBoard);

        /**
        * For debug purpose
        */
        void showSquaresDetected(Mat img, vector<vector<Point>> squares);
    }

    CornersPoints ComputeCornersPoints(std::vector<cv::Mat> imgs, CheckerBoardsSettings checkerBoardsSettings, bool useRealScreenCoordinate, std::filesystem::path directoryPathToStoreReport) {
        int interiorCornersX = checkerBoardsSettings.SquareXNb - 1;
        int interiorCornersY = checkerBoardsSettings.SquareYNb - 1;
        Size boardSize = Size(interiorCornersX, interiorCornersY);

        //vectors of 3D points for all checker board images (real wafer coordinate frame)
        vector<vector<Point3f>> cornersObjPoints;
        std::map<CheckerBoardPosition, vector<Point3f>> cornersObjPointsByPosition;

        //vectors of 2D points for all checker board images (camera coordinate frame)
        vector<vector<Point2f>> cornersImgPoints;
        std::map<CheckerBoardPosition, vector<Point2f>> cornersImgPointsByPosition;

        int sampleRatio = 10;
        int index = 0;
        for (Mat img : imgs)
        {
            vector<CheckerBoard> checkerBoardDetectedImgs = DetectCheckerBoards(img, sampleRatio, checkerBoardsSettings);

            for (CheckerBoard checkerBoardDetectedImg : checkerBoardDetectedImgs)
            {
                index++;

                Point topLeftPosition = checkerBoardDetectedImg.TopLeftCoordinate;
                Mat checkerBoardImg = checkerBoardDetectedImg.CheckerBoardImg;

                vector<Point2f> cornersImg;
                bool success = findChessboardCorners(checkerBoardImg, boardSize, cornersImg);
                if (success)
                {
                    //rescale corners coordinates to full resolution and subpixel-refine them for greater accuracy

                    std::for_each(cornersImg.begin(), cornersImg.end(), [sampleRatio, topLeftPosition](Point2f& e) {
                        e *= sampleRatio;
                        e.x += (topLeftPosition.x * sampleRatio);
                        e.y += (topLeftPosition.y * sampleRatio);
                        });
                    cornerSubPix(img, cornersImg, Size(11, 11), Size(-1, -1), cv::TermCriteria((cv::TermCriteria::EPS + cv::TermCriteria::MAX_ITER), 30, 0.001));

                    //rearrange the corners line by line, from left to right starting from the top left corner

                    bool firstCornerIsTop = cornersImg.front().y < cornersImg.back().y;
                    bool firstCornerIsBottom = cornersImg.front().y > cornersImg.back().y;
                    bool firstCornerIsLeft = cornersImg.front().x < cornersImg.back().x;
                    bool firstCornerIsRight = cornersImg.front().x > cornersImg.back().x;

                    if (firstCornerIsBottom && firstCornerIsRight)
                    {
                        std::reverse(cornersImg.begin(), cornersImg.end());
                    }
                    else if (firstCornerIsBottom && firstCornerIsLeft)
                    {
                        vector<Point2f> tmpCornersImg;
                        for (int j = 1; j <= boardSize.height; j++)
                        {
                            for (int i = 1; i <= boardSize.width; i++)
                            {
                                tmpCornersImg.push_back(Point2f(cornersImg[(boardSize.height * i) - j].x, cornersImg[(boardSize.height * i) - j].y));
                            }
                        }
                        cornersImg = tmpCornersImg;
                    }
                    else if (firstCornerIsTop && firstCornerIsRight)
                    {
                        vector<Point2f> tmpCornersImg;
                        for (int j = 0; j < boardSize.height; j++)
                        {
                            for (int i = boardSize.width - 1; i >= 0; i--)
                            {
                                tmpCornersImg.push_back(Point2f(cornersImg[(boardSize.height * i) + j].x, cornersImg[(boardSize.height * i) + j].y));
                            }
                        }
                        cornersImg = tmpCornersImg;
                    }

                    //associate the checker board position on wafer if necessary

                    if (useRealScreenCoordinate)
                    {
                        CheckerBoardPosition position = checkerBoardDetectedImg.Position;
                        cornersImgPointsByPosition[position] = cornersImg;

                        vector<Point3f> cornersObj = checkerBoardCornersOnCalibrationWafer(checkerBoardsSettings, position);
                        cornersObjPointsByPosition[position] = cornersObj;
                    }
                    else {
                        cornersImgPoints.push_back(cornersImg);
                        vector<Point3f> cornersObj = checkerBoardCornersOnCalibrationWafer(checkerBoardsSettings, CheckerBoardPosition::Unknown);
                        cornersObjPoints.push_back(cornersObj);
                    }
                }

                if (directoryPathToStoreReport.string() != "")
                {
                    Mat reportImg;
                    cv::cvtColor(img, reportImg, COLOR_GRAY2RGB);
                    drawChessboardCorners(reportImg, boardSize, cornersImg, success);
                    Reporting::writePngImage(reportImg, directoryPathToStoreReport.string() + "\\chessboardCornersImg" + std::to_string(index) + ".png");
                }
            }
        }

        //rearrange the checker board images by position on wafer if necessary

        if (useRealScreenCoordinate)
        {
            if (!cornersImgPointsByPosition[CheckerBoardPosition::Left].empty() && !cornersObjPointsByPosition[CheckerBoardPosition::Left].empty())
            {
                cornersImgPoints.push_back(cornersImgPointsByPosition.at(CheckerBoardPosition::Left));
                cornersObjPoints.push_back(cornersObjPointsByPosition.at(CheckerBoardPosition::Left));
            }
            if (!cornersImgPointsByPosition[CheckerBoardPosition::Top].empty() && !cornersObjPointsByPosition[CheckerBoardPosition::Top].empty())
            {
                cornersImgPoints.push_back(cornersImgPointsByPosition.at(CheckerBoardPosition::Top));
                cornersObjPoints.push_back(cornersObjPointsByPosition.at(CheckerBoardPosition::Top));
            }
            if (!cornersImgPointsByPosition[CheckerBoardPosition::Right].empty() && !cornersObjPointsByPosition[CheckerBoardPosition::Right].empty())
            {
                cornersImgPoints.push_back(cornersImgPointsByPosition.at(CheckerBoardPosition::Right));
                cornersObjPoints.push_back(cornersObjPointsByPosition.at(CheckerBoardPosition::Right));
            }
            if (!cornersImgPointsByPosition[CheckerBoardPosition::Bottom].empty() && !cornersObjPointsByPosition[CheckerBoardPosition::Bottom].empty())
            {
                cornersImgPoints.push_back(cornersImgPointsByPosition.at(CheckerBoardPosition::Bottom));
                cornersObjPoints.push_back(cornersObjPointsByPosition.at(CheckerBoardPosition::Bottom));
            }
        }

        return CornersPoints(cornersObjPoints, cornersImgPoints);
    }

    vector<CheckerBoard> DetectCheckerBoards(Mat img, int sampleRatio, CheckerBoardsSettings checkerBoardsSettings)
    {
        vector<Mat> chessboardsDetectedImgs = vector<Mat>();
        vector<vector<Point> > squares;
        vector<CheckerBoard> checkerBoardImgs;
#pragma warning(push)
#pragma warning (disable: 4244) // initializing': conversion from 'float' to 'int', possible loss of data
        // Note : why not use directly float or double 
        // Optimisation : simplify calculation some variables are only use once ! avoid variable creation
        int checkerBoardWidthMm = (checkerBoardsSettings.SquareXNb * checkerBoardsSettings.SquareSizeMm); // use only once
        int checkerBoardHeightMm = (checkerBoardsSettings.SquareYNb * checkerBoardsSettings.SquareSizeMm); // use only once
        int checkerBoardWidthPixel = checkerBoardWidthMm / checkerBoardsSettings.PixelSizeMm; // use only once
        int checkerBoardHeightPixel = checkerBoardHeightMm / checkerBoardsSettings.PixelSizeMm; // use only once
        int sampleCheckerBoardWidth = checkerBoardWidthPixel / sampleRatio; // use x4 - double ? float ?
        int sampleCheckerBoardHeight = checkerBoardHeightPixel / sampleRatio; // use x4 - double ? float ?
        double samplePerimeter = (sampleCheckerBoardWidth + sampleCheckerBoardHeight) * 2; // use x3 -- en double alors que l'on est sur du int ?
        int samplePerimeterAccuracy = (samplePerimeter * 0.4); // de nouveau de double à int -- oO'
        int sampleCenterAccuracy = (sampleCheckerBoardWidth + sampleCheckerBoardHeight) / 2;
#pragma warning(pop)

        // Gain some time by detecting at lower resolution
        Mat img8Bits = Convertor::ConvertTo8UC1(img);
        Size size = img8Bits.size();
        int sampleSizeWidth = std::max(size.width / sampleRatio, 10);
        int sampleSizeHeight = std::max(size.height / sampleRatio, 10);
        Size downsampleSize = cv::Size(sampleSizeWidth, sampleSizeHeight);
        Mat downsampledImg = Mat::zeros(downsampleSize, CV_8UC1);
        cv::resize(img8Bits, downsampledImg, downsampleSize, 0, 0, INTER_AREA);

        findSquares(downsampledImg, squares);
        //showSquaresDetected(downsampledImg, squares);

        vector<vector<Point>> selectedSquares;

        for (vector<Point> square : squares)
        {
            double currentPerimeter = arcLength(square, true);
            double currentDistPerimeter = abs(currentPerimeter - samplePerimeter);
            bool selected = currentDistPerimeter <= samplePerimeterAccuracy;
            if (selected)
            {
                Moments M = moments(square);
                double cx = int(M.m10 / M.m00);
                double cy = int(M.m01 / M.m00);

                bool haveRivalSquare = false;
                vector<Point> rivalSquare;

                for (vector<Point> selectedSquare : selectedSquares)
                {
                    Moments currentM = moments(square);
                    Point currentCenter = Point(int(currentM.m10 / currentM.m00), int(currentM.m01 / currentM.m00));
                    Moments previousM = moments(selectedSquare);
                    Point previousCenter = Point(int(previousM.m10 / previousM.m00), int(previousM.m01 / previousM.m00));
                    double centerDist = cv::norm(currentCenter - previousCenter);
                    haveRivalSquare = centerDist < sampleCenterAccuracy;
                    rivalSquare = selectedSquare;
                    if (haveRivalSquare)
                    {
                        break;
                    }
                }

                if (!haveRivalSquare)
                {
                    selectedSquares.push_back(square);
                }
                else
                {
                    double rivalPerimeter = arcLength(rivalSquare, true);
                    double rivalDistPerimeter = abs(rivalPerimeter - samplePerimeter);
                    bool betterSquare = currentDistPerimeter < rivalDistPerimeter;

                    if (betterSquare)
                    {
                        selectedSquares.erase(std::remove(selectedSquares.begin(), selectedSquares.end(), rivalSquare), selectedSquares.end());
                        selectedSquares.push_back(square);
                    }
                }
            }
        }

        //showSquaresDetected(downsampledImg, selectedSquares);

        vector<Rect> squareAreas;
        for (vector<Point> square : selectedSquares)
        {
            Moments M = moments(square);
            Point center = Point(int(M.m10 / M.m00), int(M.m01 / M.m00));

            double width = sampleCheckerBoardWidth + 1000 / sampleRatio;
            double height = sampleCheckerBoardHeight + 1000 / sampleRatio;
            double leftX = std::max(center.x - (width / 2), 0.0);
            double topY = std::max(center.y - (height / 2), 0.0);
            Rect squareArea = Rect((int)leftX, (int)topY, (int)width, (int)height); // Rect reclame du INT ! pourquoi etre passé en double ? oO'
            squareAreas.push_back(squareArea);

            Mat croppedImage = downsampledImg(squareArea);
            if (checkChessboard(croppedImage, Size(checkerBoardsSettings.SquareXNb, checkerBoardsSettings.SquareYNb)))
            {
                Point topLeftCoordinate = Point((int)leftX, (int)topY); // idem on store du double mais on utilise du int oO'
                CheckerBoard checkerBoard = CheckerBoard(findCheckerBoardPosition(downsampledImg.size(), topLeftCoordinate, Size(sampleCheckerBoardWidth, sampleCheckerBoardHeight)), topLeftCoordinate, croppedImage);
                checkerBoardImgs.push_back(checkerBoard);
                //showCheckerBoard(checkerBoard);
            }
        }

        return checkerBoardImgs;
    }

    namespace {
        double angle(Point pt1, Point pt2, Point pt0)
        {
            double dx1 = pt1.x - pt0.x;
            double dy1 = pt1.y - pt0.y;
            double dx2 = pt2.x - pt0.x;
            double dy2 = pt2.y - pt0.y;
            return (dx1 * dx2 + dy1 * dy2) / sqrt((dx1 * dx1 + dy1 * dy1) * (dx2 * dx2 + dy2 * dy2) + 1e-10);
        }

        void findSquares(const Mat& image, vector<vector<Point> >& squares)
        {
            int thresh = 50, N = 11;
            squares.clear();

            Mat pyr, timg, gray0(image.size(), CV_8U), gray;

            // Down-scale and upscale the image to filter out the noise
            pyrDown(image, pyr, Size(image.cols / 2, image.rows / 2));
            pyrUp(pyr, timg, image.size());
            vector<vector<Point> > contours;

            // Find squares in every color plane of the image
            int ch[] = { 0, 0 };
            mixChannels(&timg, 1, &gray0, 1, ch, 1);

            for (int l = 0; l < N; l++)
            {
                if (l == 0)
                {
                    // Use Canny instead of zero threshold level (helps to catch squares with gradient shading)
                    // Take the upper threshold from slider and set the lower to 0 (which forces edges merging)
                    Canny(gray0, gray, 0, thresh, 5);
                    dilate(gray, gray, Mat(), Point(-1, -1));
                }
                else
                {
                    gray = gray0 >= (l + 1) * 255 / N;
                }

                findContours(gray, contours, RETR_LIST, CHAIN_APPROX_SIMPLE);

                vector<Point> approx;
                for (size_t i = 0; i < contours.size(); i++)
                {
                    // Square contours should have 4 vertices after approximation, relatively large area (to filter out noisy contours) and be convex
                    approxPolyDP(contours[i], approx, arcLength(contours[i], true) * 0.02, true);
                    if (approx.size() == 4 && fabs(contourArea(approx)) > 1000 && isContourConvex(approx))
                    {
                        double maxCosine = 0;
                        for (int j = 2; j < 5; j++)
                        {
                            // Find the maximum cosine of the angle between joint edges
                            double cosine = fabs(angle(approx[j % 4], approx[j - 2], approx[j - 1]));
                            maxCosine = MAX(maxCosine, cosine);
                        }

                        // If cosines of all angles are small (all angles are ~90 degree) then write quandrange vertices to resultant sequence
                        if (maxCosine < 0.3)
                        {
                            squares.push_back(approx);
                        }
                    }
                }
            }
        }

        vector<Point3f> checkerBoardCornersOnCalibrationWafer(CheckerBoardsSettings checkerBoardsSettings, CheckerBoardPosition checkerBoardPosition)
        {
            int interiorCornersX = checkerBoardsSettings.SquareXNb - 1;
            int interiorCornersY = checkerBoardsSettings.SquareYNb - 1;
            Size boardSize = Size(interiorCornersX, interiorCornersY);

            vector<Point3f> corners;
            for (int j = 0; j < interiorCornersY; j++)
            {
                for (int i = 0; i < interiorCornersX; i++)
                {
                    corners.push_back(Point3f(i * checkerBoardsSettings.SquareSizeMm, -j * checkerBoardsSettings.SquareSizeMm, 0));
                }
            }

            switch (checkerBoardPosition)
            {
            case CheckerBoardPosition::Top:
            {
                std::for_each(corners.begin(), corners.end(), [checkerBoardsSettings](Point3f& corner) {
                    corner.x += checkerBoardsSettings.CheckerBoardsTopLeftOrigins.TopCheckerBoardOrigin.x;
                    corner.y += checkerBoardsSettings.CheckerBoardsTopLeftOrigins.TopCheckerBoardOrigin.y; });
                break;
            }
            case CheckerBoardPosition::Bottom:
            {
                std::for_each(corners.begin(), corners.end(), [checkerBoardsSettings](Point3f& corner) {
                    corner.x += checkerBoardsSettings.CheckerBoardsTopLeftOrigins.BottomCheckerBoardOrigin.x;
                    corner.y += checkerBoardsSettings.CheckerBoardsTopLeftOrigins.BottomCheckerBoardOrigin.y; });
                break;
            }
            case CheckerBoardPosition::Left:
            {
                std::for_each(corners.begin(), corners.end(), [checkerBoardsSettings](Point3f& corner) {
                    corner.x += checkerBoardsSettings.CheckerBoardsTopLeftOrigins.LeftCheckerBoardOrigin.x;
                    corner.y += checkerBoardsSettings.CheckerBoardsTopLeftOrigins.LeftCheckerBoardOrigin.y; });
                break;
            }
            case CheckerBoardPosition::Right:
            {
                std::for_each(corners.begin(), corners.end(), [checkerBoardsSettings](Point3f& corner) {
                    corner.x += checkerBoardsSettings.CheckerBoardsTopLeftOrigins.RightCheckerBoardOrigin.x;
                    corner.y += checkerBoardsSettings.CheckerBoardsTopLeftOrigins.RightCheckerBoardOrigin.y; });
                break;
            }
            }

            return corners;
        }

        CheckerBoardPosition findCheckerBoardPosition(cv::Size waferImgSize, cv::Point topLeftCornerPosition, cv::Size checkerBoardSize)
        {
            int width = waferImgSize.width;
            int height = waferImgSize.height;
            int xThreshold = (width / 2); // TODO Compute from wafer center
            int yThreshold = (height / 2) - (height / 10); // TODO Compute from wafer center

            bool isPotentialLeft = topLeftCornerPosition.x <= xThreshold && topLeftCornerPosition.x + checkerBoardSize.width <= xThreshold;
            bool isPotentialTop = topLeftCornerPosition.y <= yThreshold && topLeftCornerPosition.y + checkerBoardSize.height <= yThreshold;
            bool isPotentialRight = topLeftCornerPosition.x >= xThreshold;
            bool isPotentialBottom = topLeftCornerPosition.y >= yThreshold;
            if (isPotentialLeft && !isPotentialRight && !isPotentialTop && !isPotentialBottom)
            {
                return CheckerBoardPosition::Left;
            }
            else if (isPotentialTop && !isPotentialBottom && !isPotentialLeft && !isPotentialRight)
            {
                return CheckerBoardPosition::Top;
            }
            else if (isPotentialRight && !isPotentialLeft && !isPotentialTop && !isPotentialBottom)
            {
                return CheckerBoardPosition::Right;
            }
            else if (isPotentialBottom && !isPotentialTop && !isPotentialRight && !isPotentialLeft)
            {
                return CheckerBoardPosition::Bottom;
            }
            else {
                return CheckerBoardPosition::Unknown;
            }
        }

        void showCheckerBoard(CheckerBoard checkerBoard)
        {
            string pos = "Invalid";
            switch (checkerBoard.Position)
            {
            case CheckerBoardPosition::Top:
                pos = "Top";
                break;
            case CheckerBoardPosition::Bottom:
                pos = "Bottom";
                break;
            case CheckerBoardPosition::Left:
                pos = "Left";
                break;
            case CheckerBoardPosition::Right:
                pos = "Right";
                break;
            }
            imshow(pos, checkerBoard.CheckerBoardImg);
            waitKey();
        }

        void showSquaresDetected(Mat img, vector<vector<Point>> squares)
        {
            Mat cloneOfImg;
            img.copyTo(cloneOfImg);
            cv::polylines(cloneOfImg, squares, true, Scalar(0, 255, 0), 3, LINE_AA);
            cv::imshow("Squares detected", cloneOfImg);
            cv::waitKey();
        }
    }
}