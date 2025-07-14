#include "CDetectDies.hpp"
#include "CEdgeDetector.hpp"
#include <algorithm>
#include <cmath>
#include <exception>
#include <opencv2/core.hpp>
#include <opencv2/core/base.hpp>
#include <opencv2/core/hal/interface.h>
#include <opencv2/core/mat.hpp>
#include <opencv2/core/types.hpp>
#include <opencv2/imgcodecs.hpp>
#include <opencv2/imgproc.hpp>
#include <vector>
namespace psd
{
    namespace
    {
        /**
         * @brief   Given a roi and an image, returns a new roi that is a cut version of
         *          the given roi so that it fits in the said image.
         *          If the roi is outside the image, all the fields will be at 0.
         */
        cv::Rect ClipRoiInsideImage(cv::Rect roi, cv::Mat image)
        {
            cv::Rect clippedRoi;
            clippedRoi.x = std::max(0, roi.x);
            clippedRoi.y = std::max(0, roi.y);
            clippedRoi.width = std::min(roi.width, image.cols - roi.x) - (clippedRoi.x - roi.x);
            clippedRoi.height = std::min(roi.height, image.rows - roi.y) - (clippedRoi.y - roi.y);

            if (clippedRoi.empty())
            {
                clippedRoi.x = 0;
                clippedRoi.y = 0;
                clippedRoi.width = 0;
                clippedRoi.height = 0;
            }

            return clippedRoi;
        }

        /**
         * @brief   Enlarges the roi by pixelIncreaseX pixels on the left & right
         *          and by pixelIncreaseY pixels on the top & bottom
         */
        cv::Rect EnlargeRoi(cv::Rect roi, int pixelIncreaseX, int pixelIncreaseY)
        {
            cv::Rect enlargedRoi;
            enlargedRoi.x = roi.x - pixelIncreaseX;
            enlargedRoi.y = roi.y - pixelIncreaseY;
            enlargedRoi.width = roi.width + pixelIncreaseX * 2;
            enlargedRoi.height = roi.height + pixelIncreaseY * 2;

            return enlargedRoi;
        }
    }

    void DetectDies(
        cv::Mat &waferImg,
        cv::Mat &waferMask,
        std::vector<cv::Mat> refDies,
        std::vector<std::vector<cv::Point>> theoreticalPositionsPerDie,
        int nbOfDownsamplingSteps,
        double gamma,
        bool useHighResolutionPrecision,
        double pass1PositionConfidenceThreshold,
        double pass1AngleConfidenceThreshold,
        std::function<void(DieResult)> dieCallback,
        std::function<void(MissingDie)> missingDiesCallback
    ) {
         // Downscaling is not properly implemented
        nbOfDownsamplingSteps = 0;
        useHighResolutionPrecision = false;

        std::vector<DieResult> detectedDies;
        std::vector<DieResult> missingDies;

        // The following lines are to prepare the algorithm:
        // - run an edge detection. Note that the waferImg will NOT be normalized yet
        // - apply the down-sampling
        // - create the callback to return valid dies

        std::vector<cv::Mat> downsampledReferenceDies;
        cv::Mat downsampledWaferImg;
        cv::Mat downsampledWaferMask;

        // Edge detection and down-sampling on ref dies
        for (auto referenceDie : refDies)
        {
            std::vector<cv::Mat> referenceDiePyr;

            cv::buildPyramid(
                filter::edge_detector::ShenGradient(referenceDie, gamma, true, false),
                referenceDiePyr,
                nbOfDownsamplingSteps
            );
            downsampledReferenceDies.push_back(referenceDiePyr[nbOfDownsamplingSteps]);
        }

        // Edge detection and down-sampling on wafer image
        std::vector<cv::Mat> waferImgPyr;
        std::vector<cv::Mat> waferMaskPyr;
        cv::buildPyramid(
            filter::edge_detector::ShenGradient(waferImg, gamma, false, false),
            waferImgPyr,
            nbOfDownsamplingSteps
        );
        cv::buildPyramid(waferMask, waferMaskPyr, nbOfDownsamplingSteps);
        downsampledWaferImg = waferImgPyr[nbOfDownsamplingSteps];
        downsampledWaferMask = waferMaskPyr[nbOfDownsamplingSteps];

        // Callback definition
        std::function<void(DieResult)> cb;
        if(useHighResolutionPrecision) {
            cb = [
                &downsampledWaferImg, &downsampledReferenceDies, &waferMask,
                nbOfDownsamplingSteps, &detectedDies, &dieCallback
            ] (DieResult die) {
                die.DetectedROI = DetectDiesHighResPrecision(
                    downsampledWaferImg,
                    downsampledReferenceDies[die.dieKindIndex],
                    waferMask,
                    die.DetectedROI.tl(),
                    nbOfDownsamplingSteps
                );
                detectedDies.push_back(die);
                dieCallback(die);
            };
        } else {
            cb = [&detectedDies, &dieCallback] (DieResult die) {
                detectedDies.push_back(die);
                dieCallback(die);
            };
        }

        // Pass 1
        for (int dieKindIndex = 0; dieKindIndex < theoreticalPositionsPerDie.size(); ++dieKindIndex)
        {
            auto theoreticalPositions = theoreticalPositionsPerDie[dieKindIndex];
            
            auto pass1Res = DetectDiesPass1(
                downsampledWaferImg,
                downsampledReferenceDies[dieKindIndex],
                downsampledWaferMask,
                theoreticalPositions,
                dieKindIndex,
                pass1PositionConfidenceThreshold,
                pass1AngleConfidenceThreshold,
                cb
            );
            for (auto remainingDie : pass1Res.missingDies) {
                missingDies.push_back(remainingDie);
            }
        }

        // cv::Mat tmp;
        // cv::normalize(downsampledWaferImg, tmp, 0, 255, cv::NORM_MINMAX);
        // cv::imwrite(RESULTS_DIR + "edge_detected_wafer.tif", tmp);

        // Prepare pass 2 by removing the already-detected dies
        // for (auto& die : detectedDies) {
        //     auto& refdie = downsampledReferenceDies[die.dieKindIndex];
        //     cv::subtract(downsampledWaferImg(die.DetectedROI), refdie, downsampledWaferImg(die.DetectedROI));
        // }

        // Pass 2
        auto pass2Res = DetectDiesPass2(downsampledWaferImg, downsampledReferenceDies, waferMask, missingDies, detectedDies, 0, cb);
        std::cout << "missing count: " << pass2Res.missingDies.size() << '\n';
        // cv::normalize(downsampledWaferImg, tmp, 0, 255, cv::NORM_MINMAX);
        // cv::imwrite(RESULTS_DIR + "subtracted.tif", tmp);
    }

    Pass1Result DetectDiesPass1(
        const cv::Mat &waferImg,
        const cv::Mat &referenceDie,
        const cv::Mat &waferMask,
        std::vector<cv::Point> theoreticalPositions,
        int dieKindIndex,
        double confidenceThreshold,
        double angleConfidenceThreshold,
        std::function<void(DieResult)> dieCallback
    ) {
        Pass1Result res;

        cv::Mat waferImgNoBackground = waferImg.clone();
        //Setting all values outside the wafer to 0
        waferImgNoBackground.setTo(cv::Scalar(0), 255 - waferMask);

        auto dieSize = referenceDie.size();

        for (int i = 0; i < theoreticalPositions.size(); i++)
        {
            cv::Point &diePos = theoreticalPositions[i];
            cv::Rect dieRoi = cv::Rect(diePos, dieSize);

            // Enlarging the theoretical ROI to be able to match a template correctly
            // TODO: study what zone should be sensed and see if it's useful to put it
            // as parameter
            int added_padding = std::max(dieSize.width / 20, dieSize.height / 20) + 1;
            cv::Rect sensedRoi = EnlargeRoi(dieRoi, added_padding, added_padding);

            //Clipping roi x, y, width & height so it doesn't go outside the bounds of the image
            sensedRoi = ClipRoiInsideImage(sensedRoi, waferImg);

            cv::Mat sensedArea = waferImg(sensedRoi);

            cv::Mat matchResult;
            cv::matchTemplate(sensedArea, referenceDie, matchResult, cv::TM_CCOEFF_NORMED);

            double confidence;
            cv::Point shift;

            cv::minMaxLoc(matchResult, NULL, &confidence, NULL, &shift);

            DieResult detectedDie;
            detectedDie.TheoreticalROI = dieRoi;
            detectedDie.DetectedROI = cv::Rect(sensedRoi.tl() + shift, dieSize);
            detectedDie.PosConfidence = confidence;
            detectedDie.AngleConfidence = 0;
            detectedDie.Angle = 0;
            detectedDie.dieKindIndex = dieKindIndex;
            detectedDie.index = i;
            detectedDie.passNb = 1;

            if (confidence < confidenceThreshold) {
                res.missingDies.push_back(detectedDie);
            } else {
                DetectAngle(waferImg, referenceDie, waferMask, detectedDie);
                if (detectedDie.AngleConfidence < angleConfidenceThreshold) {
                    res.missingDies.push_back(detectedDie);
                } else {
                    dieCallback(detectedDie);
                }
            }
        }
        return res;
    }

    Pass2Result DetectDiesPass2(
        const cv::Mat &waferImg,
        const std::vector<cv::Mat> &refDies,
        const cv::Mat &waferMask,
        std::vector<DieResult> remainingDies,
        const std::vector<DieResult> &detectedDies,
        double confidenceThreshold,
        std::function<void(DieResult)> dieCallback
    ) {
        // TODO: apply mask on wafer
        Pass2Result res;
        std::vector<int> dieKindCount;

        // Compute an image to use as multiplier of the template matching results, so that
        // positions near the center (= theoretical position of the die) are favored
        // compared to dies far away.
        // The said image is a gaussian curve
        int gaussianRadius = 0;
        for (auto &refDie : refDies) {
            dieKindCount.push_back(0);
            gaussianRadius = std::max(gaussianRadius, std::max(refDie.size().width, refDie.size().height));
        }
        int gaussianSize = gaussianRadius * 2 + 1;
        cv::Point diePosInGaussianImage(gaussianRadius, gaussianRadius); // center of the image
        cv::Mat gaussianImage(gaussianSize, gaussianSize, CV_32F);
        for (int y = 0; y < gaussianSize; ++y) {
            for (int x = 0; x < gaussianSize; ++x) {

                // Value at position (x, y) of a Gaussian function
                // having `(gaussianRadius, gaussianRadius)` as its center
                // and a standard deviation of `gaussianRadius`
                auto val = exp(
                    - (
                        pow(((double) gaussianRadius) - (double) x, 2.0)
                        + pow(((double) gaussianRadius) - (double) y, 2.0)
                    ) / (2.0 * pow(gaussianRadius/2, 2.0))
                );
                gaussianImage.at<float>(y, x) = val;

            }
        }

        for (auto &die : remainingDies) {
            dieKindCount[die.dieKindIndex] += 1;

            //Enlarging the theoretical ROI to be able to match a template correctly
            int added_padding = std::max(die.TheoreticalROI.width, die.TheoreticalROI.height);
            cv::Rect defaultSensedRoi = EnlargeRoi(die.TheoreticalROI, added_padding, added_padding);

            //Clamping roi x, y, width & height so it doesn't go outside the bounds of the image
            cv::Rect clippedSensedRoi = ClipRoiInsideImage(defaultSensedRoi, waferImg);

            // cv::imwrite(RESULTS_DIR + "gaussian.tif", gaussianImage);

            auto diePosInClippedSensed = clippedSensedRoi.tl() - defaultSensedRoi.tl() + cv::Point(added_padding, added_padding);

            cv::Mat sensedDie = waferImg(clippedSensedRoi);

            cv::Mat matchRes;
            auto refdie = refDies[die.dieKindIndex];
            cv::matchTemplate(sensedDie, refdie, matchRes, cv::TM_CCOEFF_NORMED);

            auto sensedGaussianFactor = gaussianImage(
                cv::Rect(diePosInGaussianImage - diePosInClippedSensed, matchRes.size())
            );
            cv::multiply(matchRes, sensedGaussianFactor, matchRes);


            // cv::imwrite(RESULTS_DIR + "match_res" + std::to_string(die.index) + "_gaussed.tif", matchRes);

            // For each detected die, remove the part of the results matrix that implies that the
            // die we're trying to detect would overlap the detected die
            for(auto& detected : detectedDies) {
                auto allowedOverlapping = 0.2 * static_cast<cv::Point>(refdie.size());
                // The dies start overlapping when the current die's position + it size is equal to the detected die's position
                auto detectedStartPos = detected.DetectedROI.tl() - clippedSensedRoi.tl() - static_cast<cv::Point>(refdie.size()) + allowedOverlapping;
                // The dies stop overlapping when the detected die's position + it size is equal to the current die's position
                auto detectedEndPos = detected.DetectedROI.tl() - clippedSensedRoi.tl() + static_cast<cv::Point>(detected.DetectedROI.size()) - allowedOverlapping;

                if (detectedStartPos.x < 0) {
                    detectedStartPos.x = 0;
                }
                if (detectedStartPos.y < 0) {
                    detectedStartPos.y = 0;
                }
                if (detectedEndPos.x >= matchRes.size().width) {
                    detectedEndPos.x = matchRes.size().width;
                }
                if (detectedEndPos.y >= matchRes.size().height) {
                    detectedEndPos.y = matchRes.size().height;
                }
                if (detectedEndPos.x <= detectedStartPos.x || detectedEndPos.y <= detectedStartPos.y) {
                    continue;
                }

                auto overlap = cv::Rect(detectedStartPos, detectedEndPos);
                matchRes(overlap) = cv::Scalar(0);
            }

            double confidence = 0;
            cv::Point shift(0, 0);
            cv::minMaxLoc(matchRes, NULL, &confidence, NULL, &shift);
            confidence /= sensedGaussianFactor.at<float>(shift);

            shift += clippedSensedRoi.tl();
            die.DetectedROI = cv::Rect(shift, refdie.size());
            die.PosConfidence = confidence;
            die.AngleConfidence = 0;
            die.Angle = 0;
            die.passNb = 2;
            
            if (confidence < confidenceThreshold) {
                res.missingDies.push_back(die);
            } else {
                DetectAngle(waferImg, refdie, waferMask, die);
                dieCallback(die);
            }
        }

        return res;
    }

    void DetectAngle(
        const cv::Mat &waferImg,
        const cv::Mat &refDie,
        const cv::Mat &waferMask,
        DieResult &templateResult
    ) {
        cv::Mat sensedDie = waferImg(templateResult.DetectedROI);

        // If there will be any pre-processing to do, then refProcessed and sensedProcessed
        // should contain the result (without changing refDie and sensedDie)
        cv::Mat refProcessed = refDie;
        cv::Mat sensedProcessed = sensedDie;

        double die_diag_size = 1 + sqrt(refDie.size().height * refDie.size().height + refDie.size().width * refDie.size().width) / 2.0;

        // Initialize the image to set the color on the pixels that will not be set by warpPolar.
        // Thus change this initialization if the wafer background color cannot be assumed to be
        // black.
        cv::Mat sensedPolar = cv::Mat(sensedDie.size(), CV_8U, cv::Scalar(0));
        cv::Mat refPolar = cv::Mat(refDie.size(), CV_8U, cv::Scalar(0));
        cv::warpPolar(
            refProcessed,
            refPolar,
            refDie.size(),
            ((cv::Point2f) refDie.size()) / 2.0,
            die_diag_size,
            cv::INTER_LINEAR
        );
        cv::warpPolar(
            sensedProcessed,
            sensedPolar,
            refDie.size(),
            ((cv::Point2f) refDie.size()) / 2.0,
            die_diag_size,
            cv::INTER_LINEAR
        );


        // Extend the sensed polar extension to be able to do a template matching:
        // - wrap vertically
        // - extend the color on the border horizontally
        cv::Mat sensedPolarExt;
        cv::copyMakeBorder(sensedPolar, sensedPolarExt, 0, refPolar.size().height, 0, 0, cv::BORDER_WRAP);
        cv::copyMakeBorder(sensedPolarExt, sensedPolarExt, 0, 0, refPolar.size().width / 20, refPolar.size().width / 20, cv::BORDER_REPLICATE);

        cv::Mat matchRes;
        cv::matchTemplate(sensedPolarExt, refPolar, matchRes, cv::TM_CCOEFF_NORMED);
        
        double max;
        cv::Point maxLoc;
        cv::minMaxLoc(matchRes, NULL, &max, NULL, &maxLoc);

        templateResult.Angle = maxLoc.y * 360.0 / refPolar.size().height;
        templateResult.AngleConfidence = max;

        // auto res_dir = RESULTS_DIR + "die_" + std::to_string(diekind) + "_" + std::to_string(templateResult.index) + "\\";
        // std::filesystem::create_directory(res_dir);
        // cv::imwrite(res_dir + "base_sensed.png", sensedDie);
        // cv::imwrite(res_dir + "base_ref.png", refDie);
        // cv::imwrite(res_dir + "proc_sensed.png", sensedProcessed);
        // cv::imwrite(res_dir + "proc_ref.png", refProcessed);
        // cv::imwrite(res_dir + "polar_sensed.png", sensedPolar);
        // cv::imwrite(res_dir + "polar_ref.png", refPolar);
        // cv::imwrite(res_dir + "polar_sensed_extended.png", sensedPolarExt);
        // cv::imwrite(res_dir + "matchres.tif", matchRes);
    }


    cv::Rect DetectDiesHighResPrecision(
        const cv::Mat &waferImg,
        const cv::Mat &referenceDie,
        const cv::Mat &waferMask,
        cv::Point detectedPosition,
        int DownsamplingFactor
    ) {
        throw std::exception("not yet implemented");
    }
}
