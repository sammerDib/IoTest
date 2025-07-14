#include "WaferDetector.hpp"
#include "ReportingUtils.hpp"
#include <CShapeFinder.hpp>
#include <CImageTypeConvertor.hpp>

using namespace std;
using namespace cv;

namespace psd {
    Mat CreateWaferMask(Mat img, float pixelSize, float waferRadius, float edgeExclusion, std::filesystem::path directoryPathToStoreReport) {
        int cannyThreshold = 100;
        int sampleRatio = 10;

        Mat img8Bits = Convertor::ConvertTo8UC1(img);
        Size size = img8Bits.size();
        int sampleSizeWidth = std::max(size.width / sampleRatio, 10);
        int sampleSizeHeight = std::max(size.height / sampleRatio, 10);
        Size downsampleSize = cv::Size(sampleSizeWidth, sampleSizeHeight);
        Mat downsampledImg = Mat::zeros(downsampleSize, CV_8UC1);
        cv::resize(img8Bits, downsampledImg, downsampleSize, 0, 0, INTER_AREA);

        float diameterInPixel = (waferRadius * 2) / pixelSize;
        diameterInPixel /= sampleRatio;
        std::pair<float, float> expectedAxes = std::pair<float, float>(diameterInPixel, diameterInPixel);
        float majorAxisTolerance = (waferRadius / 2) / pixelSize;
        float minorAxisTolerance = (waferRadius / 2) / pixelSize;
        majorAxisTolerance /= sampleRatio;
        minorAxisTolerance /= sampleRatio;

        float edgeOfExclusionInPixel = edgeExclusion / pixelSize;
        edgeOfExclusionInPixel /= sampleRatio;

        std::vector<shape_finder::Ellipse> ellipses = shape_finder::EllipseFinder(downsampledImg, shape_finder::EllipseFinderParams(expectedAxes, majorAxisTolerance, minorAxisTolerance, cannyThreshold));

        cv::Mat mask = cv::Mat::zeros(size, CV_8UC1);
        if (!ellipses.empty())
        {
            shape_finder::Ellipse wafer = ellipses[0];
            wafer.CenterPos *= sampleRatio;
            wafer.HeightAxis *= sampleRatio;
            wafer.WidthAxis *= sampleRatio;

            edgeOfExclusionInPixel *= sampleRatio;

            auto szWidth = (int) ((wafer.WidthAxis * 0.5f) - edgeOfExclusionInPixel);
            auto szHeight = (int)((wafer.HeightAxis * 0.5f) - edgeOfExclusionInPixel);

            cv::ellipse(mask, wafer.CenterPos, cv::Size(szWidth, szHeight), wafer.Angle, 0, 360, 255, -1);
            cv::ellipse(img8Bits, wafer.CenterPos, cv::Size(szWidth, szHeight), wafer.Angle, 0, 360, 255, 2);

            //making the wafer border stand out from the rest of the mask
            cv::ellipse(mask, wafer.CenterPos, cv::Size(szWidth, szHeight), wafer.Angle, 0, 360, 254, 1);

            if (directoryPathToStoreReport.string() != "")
            {
                Reporting::writePngImage(mask, directoryPathToStoreReport.string() + "\\Mask.png");
                Reporting::writePngImage(img8Bits, directoryPathToStoreReport.string() + "\\EdgeOfExclusion.png");
            }
        }

        return mask;
    }
}