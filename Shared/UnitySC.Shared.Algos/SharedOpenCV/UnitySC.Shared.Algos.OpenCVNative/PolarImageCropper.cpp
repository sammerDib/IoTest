#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>

#include "PolarImageCropper.hpp"
#include "LoggerOCV.hpp"

#include "ErrorLogging.hpp"

#pragma unmanaged

const double PolarImageCropper::GOOD_NON_EMPTY_PIXEL_PERCENTAGE_300MM = 0.96;
const double PolarImageCropper::GOOD_NON_EMPTY_PIXEL_PERCENTAGE_200MM = 0.85;
const double PolarImageCropper::GOOD_NON_EMPTY_PIXEL_PERCENTAGE_150MM = 0.80;
const double PolarImageCropper::GOOD_NON_EMPTY_PIXEL_PERCENTAGE_100MM = 0.60;

cv::Mat PolarImageCropper::Crop(cv::Mat &image, int waferDiameterInMm, cv::Rect *boundingRect) {

  int nonZerosCount = cv::countNonZero(image);

  //// total crop case
  bool allBlackImage = (nonZerosCount == 0);

  //// no crop case
  bool allValuedImage = !allBlackImage && (nonZerosCount == (image.rows * image.cols));

  if (allBlackImage) {
    if (nullptr != boundingRect) {
      boundingRect->x = 0;
      boundingRect->y = 0;
      boundingRect->width = image.cols;
      boundingRect->height = image.rows;
    }
    return cv::Mat(cv::Size(0, 0), CV_8U);

  } else if (allValuedImage) {
    if (nullptr != boundingRect) {
      boundingRect->x = 0;
      boundingRect->y = 0;
      boundingRect->width = 0;
      boundingRect->height = 0;
    }
    return image.clone();
  } else {

    cv::Mat mask = image > 0;
    mask -= 254;
    // now mask contains 1 or 0

    const uchar REDUCE_TO_ONE_ROW = 0;
    const uchar REDUCE_TO_ONE_COLUMN = 1;

    cv::Mat rowSums;
    cv::reduce(mask, rowSums, REDUCE_TO_ONE_COLUMN, cv::ReduceTypes::REDUCE_SUM, CV_32F);
    int rowIndex;
    bool foundFirstCompleteLine = false;
    int firstCompleteLineIndex = 0;
    bool foundLastCompleteLine = false;
    int lastCompleteLineIndex = 0;

    double goodNonEmptyPixelPercentage = 0.0;

    switch (waferDiameterInMm)
    {
        case 300 :
            goodNonEmptyPixelPercentage = GOOD_NON_EMPTY_PIXEL_PERCENTAGE_300MM;
            break;
        case 200:
            goodNonEmptyPixelPercentage = GOOD_NON_EMPTY_PIXEL_PERCENTAGE_200MM;
            break;
        case 150:
            goodNonEmptyPixelPercentage = GOOD_NON_EMPTY_PIXEL_PERCENTAGE_150MM;
            break;
        case 100:
            goodNonEmptyPixelPercentage = GOOD_NON_EMPTY_PIXEL_PERCENTAGE_100MM;
            break;
        default:
            ErrorLogging::LogError("The BWA is not compatible with the current wafer diameter");
            return image.clone();
    }

    /* % of image pixels should be non-empty to consider line as good, not to crop */
    double minimalSumForConsideredGoodLine = image.cols * goodNonEmptyPixelPercentage;

    for (rowIndex = 0; rowIndex < rowSums.rows; ++rowIndex) {
      float currentValue = rowSums.at<float>(rowIndex, 0);
      if (!foundFirstCompleteLine && currentValue >= minimalSumForConsideredGoodLine) {
        foundFirstCompleteLine = true;
        firstCompleteLineIndex = rowIndex;
      }
      if (foundFirstCompleteLine && !foundLastCompleteLine && currentValue <= minimalSumForConsideredGoodLine) {
        foundLastCompleteLine = true;
        // the last good one was the previous one
        lastCompleteLineIndex = rowIndex - 1;
      }
    }
    int heightStart = (foundFirstCompleteLine) ? firstCompleteLineIndex : 0;
    // we add 1 because we need a count, not an index
    int heightStop = (foundLastCompleteLine) ? lastCompleteLineIndex + 1 : image.rows;

    cv::Mat colSums;
    cv::reduce(mask, colSums, REDUCE_TO_ONE_ROW, cv::ReduceTypes::REDUCE_SUM, CV_32F);
    bool foundFirstCompleteColumn = false;
    int firstCompleteColumnIndex = 0;
    bool foundLastCompleteColumn = false;
    int lastCompleteColumnIndex = 0;
    double minimalSumForConsideredGoodColumn = image.rows * goodNonEmptyPixelPercentage;

    int colIndex;
    for (colIndex = 0; colIndex < colSums.cols; ++colIndex) {
      float currentValue = colSums.at<float>(0, colIndex);
      if (!foundFirstCompleteColumn && currentValue >= minimalSumForConsideredGoodColumn) {
        foundFirstCompleteColumn = true;
        firstCompleteColumnIndex = colIndex;
      }
      if (foundFirstCompleteColumn && !foundLastCompleteColumn && currentValue <= minimalSumForConsideredGoodColumn) {
        foundLastCompleteColumn = true;
        // the last good one was the previous one
        lastCompleteColumnIndex = colIndex - 1;
      }
    }
    int widthStart = (foundFirstCompleteColumn) ? firstCompleteColumnIndex : 0;
    // we add 1 because we need a count, not an index
    int widthStop = (foundLastCompleteColumn) ? lastCompleteColumnIndex + 1 : image.cols;

    cv::Mat result;
    std::stringstream s;
    if (nullptr != boundingRect) {
      boundingRect->x = widthStart;
      boundingRect->y = heightStart;
      boundingRect->width = widthStop - widthStart;
      boundingRect->height = heightStop - heightStart;
      cv::copyTo(image(*boundingRect), result, cv::noArray());
      s << "[PolarImageCropper::Crop] cropped " << *boundingRect;

    } else {

      LoggerOCV::Warning("[PolarImageCropper::Crop] Please pass 'boundingRect' parameter to Crop()");

      cv::Rect _boundingRect(widthStart, heightStart, widthStop - widthStart, heightStop - heightStart);
      cv::Mat result;
      cv::copyTo(image(_boundingRect), result, cv::noArray());
      s << "[PolarImageCropper::Crop] cropped " << _boundingRect;
    }
    LoggerOCV::Debug(s.str());

    return result;
  }
}