#pragma once

#include <functional>
#include <opencv2/core/types.hpp>
#include <opencv2/opencv.hpp>

#pragma unmanaged
namespace psd
{
    /**
     * @brief  The resulting die of a die detection algorithm.
     * See the documentation on the wrapper function for details.
     */
    struct DieResult
    {
        DieResult() = default;

        cv::Rect TheoreticalROI;
        cv::Rect DetectedROI;
        double Angle;
        double PosConfidence;
        double AngleConfidence;
        int dieKindIndex;
        int index;
        int passNb;
    };
    struct MissingDie
    {
        cv::Rect TheoreticalROI;
        int dieKindIndex;
        int index;
    };
    struct Pass1Result
    {
        std::vector<DieResult> missingDies;
    };
    struct Pass2Result
    {
        std::vector<DieResult> missingDies;
    };

    /**
     * @brief      Runs the die detection algorithm. See the documentation on the wrapper function for details.
     */
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
    );

    /**
     * @brief      Runs the first pass of the algorithm, detecting dies close to the therotical
     *             position with high confidence. Detected dies are sent through `dieCallback`,
     *             the skipped ones are returned through the return value.
     *
     * @param      waferImg                  The image of the wafer
     * @param[in]  referenceDie              The reference die
     * @param[in]  waferMask                 The mask of the wafer
     * @param[in]  theoreticalPositions      The list of the theoretical positions at which
     *                                       the reference die can be found
     * @param[in]  dieKindIndex              The die kind index
     * @param[in]  confidenceThreshold       The position confidence threshold. Dies with a lower
     *                                       position confidence will be skipped.
     * @param[in]  angleConfidenceThreshold  The angle confidence threshold. Dies with a lower
     *                                       angle confidence will be skipped.
     * @param[in]  dieCallback               Called for each die that is successfully detected
     *
     * @return     The list of skipped dies
     */
    Pass1Result DetectDiesPass1(
        const cv::Mat &waferImg,
        const cv::Mat &referenceDie,
        const cv::Mat &waferMask,
        std::vector<cv::Point> theoreticalPositions,
        int dieKindIndex,
        double confidenceThreshold,
        double angleConfidenceThreshold,
        std::function<void(DieResult)> dieCallback
    );

    /**
     * @brief      Runs the second pass of the algorithm, detecting as many dies as possible.
     *             Detected dies are sent through `dieCallback`, the skipped ones are returned
     *             through the return value.
     *
     * @param[in]  waferImg             Same as for function `DetectDies`
     * @param[in]  refDies              Same as for function `DetectDies`
     * @param[in]  waferMask            Same as for function `DetectDies`
     * @param[in]  remainingDies        Same as for function `DetectDies`
     * @param[in]  detectedDies         Same as for function `DetectDies`
     * @param[in]  confidenceThreshold  The position confidence threshold. Dies with a lower
     *                                  position confidence will be skipped.
     * @param[in]  dieCallback          Same as for function `DetectDies`
     *
     * @return     The list of skipped dies
     */
    Pass2Result DetectDiesPass2(
        const cv::Mat &waferImg,
        const std::vector<cv::Mat> &refDies,
        const cv::Mat &waferMask,
        std::vector<DieResult> remainingDies,
        const std::vector<DieResult> &detectedDies,
        double confidenceThreshold,
        std::function<void(DieResult)> dieCallback
    );

    /**
     */
    /**
     * @brief      Detects the angle by doing a log transform of the image followed by
     *             a template matching on this new image.
     *
     * @param[in]  waferImg        The wafer image
     * @param[in]  referenceDie    The reference die
     * @param[in]  waferMask       The wafer mask
     * @param      templateResult  The detected die. It will be used to know the position
     *                             of the die, and both the `angle` and `AngleConfidence`
     *                             will be set.
     */
    void DetectAngle(
        const cv::Mat &waferImg,
        const cv::Mat &referenceDie,
        const cv::Mat &waferMask,
        DieResult &templateResult
    );

    /**
     * @brief Checks the position of a die detected at a lower resolution. Not implemented
     */
    cv::Rect DetectDiesHighResPrecision(
        const cv::Mat &waferImg,
        const cv::Mat &referenceDie,
        const cv::Mat &waferMask,
        cv::Point detectedPosition,
        int DownsamplingFactor
    );

}
