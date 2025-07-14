#pragma once

#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    /**
     * The result of the die detection algorithm
     */
    public ref struct DieResult
    {
        /// The estimated ROI given to the algorithm
        RegionOfInterest^ TheoreticalROI;

        /// The ROI detected by the algorithm
        RegionOfInterest^ DetectedROI;

        /// The confidence in the position (i.e. how much the
        /// reference die and the detected die match)
        double PosConfidence;
        
        /// The angle detected by the algorithm
        double Angle;

        /// The confidence in the angle
        double AngleConfidence;

        /// The index of the die kind associated to the detected die
        int dieKindIndex;

        /// The index of the given theoretical position in the list
        /// of positions for this die kind
        int theoreticalPositionIndex;

        /// The number of the pass in which the die was detected:
        ///
        /// * 1 if it was detected on the first pass, which means that the
        ///   detected ROI is close to the theoretical one and both the position
        ///   and angle confidence are high enough
        ///
        /// * 2 if it was not detected on the first pass but was the most likely
        ///   result in an area around the theoretical ROI
        int passNb;
    };

    /**
     * Returned by the die detection algorithm when a die can't be found
     */
    public ref struct MissingDie
    {
        /// The estimated ROI given to the algorithm
        RegionOfInterest^ TheoreticalROI;

        /// The index of the die kind associated to the detected die
        int dieKindIndex;

        /// The index of the given theoretical position in the list
        /// of positions for this die kind
        int theoreticalPositionIndex;
    };

    /**
     * Details of a kind of die to detect
     */
    public ref struct DieKindDetails
    {
        /// An image of the reference die. Its pixel size will be used to
        /// define the sizes of the ROIs
        ImageData^ ReferenceDie;
        /// Estimated top left position of each die to detect. These are
        /// pixel positions on the wafer image.
        array<Point2i^>^ theoreticalPositions;
    };

    /**
     * Parameters a die detection. Note that this contains two callbacks used to report any detected die
     * as soon as it is detected
     */
    public ref struct DieDetectionSettings
    {
        /**
         * Currently, downsampling is NOT yet implemented, so this parmeter is skipped (as if the value was 0).
         * 
         * This parameter will allow to reduce the resolution of the wafer image to run the algo, making it faster.
         * For each downsampling step, the size of the image will be divided by 2.
         */
        int NbOfDownsamplingSteps;
        
        /**
         * To improve the results, this algorithm does all the template matching on a Shen edge filtered image.
         * This is the gamma used to parameter the Shen edge detection.
         */
        double Gamma;

        /**
         * Currently, downsampling is NOT yet implemented, so this parmeter is skipped (as if the value was false).
         */
        bool UseHighResolutionPrecision;

        /**
         * The confidence threshold at which a die will be allowed to be detected in the pass 1 rather than 2.
         */
        double Pass1PositionConfidenceThreshold;
        
        /**
         * The same as `pass1PositionConfidenceThreshold`, but for the angle confidence (rather than the
         * position confidence)
         */
        double Pass1AngleConfidenceThreshold;

        /// This callback is called when a die is successfully detected. This architecture will
        /// allow on the long run to send the detected dies to the ADC while the flow is running.
        Action<DieResult^>^ DieCallback;
        
        /// This callback is called when a die is considered fully missing.
        Action<MissingDie^>^ MissingDiesCallback;
    };
    
    public ref class DieDetector
    {
    public:
        /**
         * Runs the die detection algorithm. The results are reported to the callbacks in the `settings` argument.
         * 
         * @param waferImg         Wafer image containing the dies to detect
         * @param waferMask        Mask of the wafer
         * @param theoreticalDies  Details of the dies to detect on said wafer
         * @param settings         Remaining of the arguments of the algorithm
         */
        static void DetectDies(ImageData^ waferImg,
                               ImageData^ waferMask,
                               array<DieKindDetails^>^ theoreticalDies,
                               DieDetectionSettings^ settings);
    };
}
