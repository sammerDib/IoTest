#include "Stitcher.h"
#include <opencv2/highgui.hpp>
#include <numeric>

namespace UnitySCSharedAlgosOpenCVWrapper {
    namespace {
        bool compare_x_positions(const cv::Point& p1, const cv::Point& p2)
        {
            return p1.x < p2.x;
        }

        bool compare_y_positions(const cv::Point& p1, const cv::Point& p2)
        {
            return p1.y < p2.y;
        }
    }

    PositionImageData^ Stitcher::StitchImages(List<PositionImageData^>^ images)
    {
        std::vector<cv::Mat> mats;
        std::vector<cv::Point2d> positions;

        for (int i = 0; i < images->Count; i++)
        {
            mats.push_back(CreateMatFromImageData(images[i]));
            positions.push_back(cv::Point2d(images[i]->Centroid->X, images[i]->Centroid->Y));
        }

        double pxSize = images[0]->Scale->X;

        int width = mats[0].cols;
        int height = mats[0].rows;

        double lowestX = (*std::min_element(positions.begin(), positions.end(), compare_x_positions)).x;
        double lowestY = (*std::min_element(positions.begin(), positions.end(), compare_y_positions)).y;
        double highestX = (*std::max_element(positions.begin(), positions.end(), compare_x_positions)).x;
        double highestY = (*std::max_element(positions.begin(), positions.end(), compare_y_positions)).y;

        int stitchWidth = (int)round(((highestX - lowestX) / pxSize)) + width;
        int stitchHeight = (int)round(((highestY - lowestY) / pxSize)) + height;

        cv::Mat stitchedMat = cv::Mat::zeros(cv::Size(stitchWidth, stitchHeight), mats[0].type());

        for (int i = 0; i < mats.size(); i++)
        {
            int xOnStitched = (int)round((positions[i].x - lowestX) / pxSize);
            int yOnStitched = (int)round((highestY - positions[i].y) / pxSize);
            //Making sure the x and y positions don't go outside the image
            xOnStitched = std::min(xOnStitched, stitchWidth - mats[i].cols);
            yOnStitched = std::min(yOnStitched, stitchHeight - mats[i].rows);
            mats[i].copyTo(stitchedMat(cv::Rect(xOnStitched, yOnStitched, mats[i].cols, mats[i].rows)));
        }

        cv::Point2d zeroPoint(0.0, 0.0);
        cv::Point2d positionSum = std::accumulate(positions.begin(), positions.end(), zeroPoint);
        cv::Point2d meanPosition = cv::Point2d(positionSum.x / positions.size(), positionSum.y / positions.size());

        BareWaferAlignmentImageData^ stitched = gcnew BareWaferAlignmentImageData(CreateByteArrayFromMat(stitchedMat), stitchedMat.cols, stitchedMat.rows, images[0]->Type);
        stitched->Centroid = gcnew Point2d(meanPosition.x, meanPosition.y);
        stitched->Scale = images[0]->Scale;

        return stitched;
    }

    BareWaferAlignmentImageData^ Stitcher::StitchImages(List<BareWaferAlignmentImageData^>^ images)
    {
        List<PositionImageData^>^ imagesInPositionImageData = gcnew List<PositionImageData^>();

        for each (BareWaferAlignmentImageData ^ bwaImageData in images)
        {
            imagesInPositionImageData->Add(bwaImageData);
        }

        PositionImageData^ posStitch = StitchImages(imagesInPositionImageData);

        BareWaferAlignmentImageData^ stitched = gcnew BareWaferAlignmentImageData(posStitch->ByteArray, posStitch->Width, posStitch->Height, posStitch->Type);
        stitched->ExpectedShape = images[0]->ExpectedShape;
        stitched->Centroid = posStitch->Centroid;
        stitched->EdgePosition = images[0]->EdgePosition;
        stitched->Scale = posStitch->Scale;

        return stitched;
    }
}