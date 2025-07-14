#pragma once

#include "ImageData.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public enum class FringesDisplacement {
        X,
        Y
    };

    public enum class FitSurface {
        None,
        PolynomeOrder2,
        PolynomeOrder3,
        PolynomeOrder4
    };

    public ref struct MaskParams
    {
        MaskParams(bool useWaferFill, double fillEdgeExclusionInMicrons, double pixelSizeInMicrons, double waferDiameterInMicrons, double waferShiftXInMicrons, double waferShiftYInMicrons)
        {
            UseWaferFill = useWaferFill;
            FillEdgeExclusionInMicrons = fillEdgeExclusionInMicrons;
            PixelSizeInMicrons = pixelSizeInMicrons;
            WaferDiameterInMicrons = waferDiameterInMicrons;
            WaferShiftXInMicrons = waferShiftXInMicrons;
            WaferShiftYInMicrons = waferShiftYInMicrons;
        };

        bool UseWaferFill; //Whether or not to fill the mask with a circle
        double FillEdgeExclusionInMicrons; //Edge size to exclude from the mask circle fill
        double PixelSizeInMicrons; //Pixel size of the image in microns
        double WaferDiameterInMicrons; //Diameter of the wafer in microns used for the fill
        double WaferShiftXInMicrons; //Horizontal shift of the wafer in relation to the origin (0,0) in microns
        double WaferShiftYInMicrons; //Vertical shift of the wafer in relation to the origin (0,0) in microns

    };

    public ref struct PSDParams {
        PSDParams(int stepNb, FringesDisplacement fringesDisplacement)
        {
            StepNb = stepNb;
            FringesDisplacement = fringesDisplacement;
        };
        int StepNb;                                //number of phase shifts used to capture interferometry images
        FringesDisplacement FringesDisplacement;   //direction of displacement of fringes
    };

    public ref struct PSDResult {
        ImageData^ WrappedPhaseMap; // phase map of the wavefront, wrapped between [-pi, pi]
        ImageData^ WrappedPhaseMap2;  // phase map of the wavefront, wrapped between [0, 2pi]
        ImageData^ Amplitude;  // amplitude of the sine curve
        ImageData^ Background;
        ImageData^ Dark;
        ImageData^ Mask;
    };

    public ref class PhaseShiftingDeflectometry {
    public:
        /**
         * Calculate phase map from interferometry images
         *
         * @param imgs              - interferometry images
         * @param params            - parameter
         *
         * @return The wrapped phase map, curvature map, amplitude, background, dark and mask
         */
        static PSDResult^ ComputePhaseMap(array<ImageData^>^ input, PSDParams^ params);

        /**
        * Calculate mask of wafer
        *
        * @param wrappedPhaseResult          - phase map calculated from interferometry images
        * @param maskParams                  - parameters to help fill the wafer inside the mask in order to have an uniform circle
        *
        * @return The mask
        */
        static ImageData^ ComputeMask(PSDResult^ wrappedPhaseResult, MaskParams^ maskParams);

        /**
        * Calculate curvature map from wrapped phase map
        *
        * @param wrappedPhaseResult                - phase map calculated from interferometry images
        * @param mask                              - mask to apply
        * @param params                            - parameter
        *
        * @return The curvature map
        */
        static ImageData^ ComputeCurvature(PSDResult^ wrappedPhaseResult, ImageData^ mask, PSDParams^ params);

        /**
        * Calculate the final dark as the average of both directions
        *
        * @param darkX                          - dark images in X direction
        * @param darkY                          - dark images in Y direction
        * @param mask                           - mask to apply
        * @param removeBackgroundSurfaceMethod  - methode use to remove background wafer surface after dark calculation (default value is PolynomeOrder2)
        * @param directoryPathToStoreReport     - directory path to store report if not empty
        *
        * @return The final mask
        */
        static ImageData^ ComputeDark(ImageData^ darkXImg, ImageData^ darkYImg, ImageData^ mask, FitSurface removeBackgroundSurfaceMethod);

        /**
        * The curvature dynammic calibration needs the acquisition of an image of the calibration wafer and computing the raw curvature maps, including mask.
        * Then, this function compute their background level.
        *
        * @param curvatureX                     - curvature image in X direction
        * @param curvatureY                     - curvature image in Y direction
        * @param mask                           - mask to apply
        *
        * @return the background standard deviation averaged on X and Y maps
        */
        static float CalibrateCurvatureDynamics(ImageData^ curvatureX, ImageData^ curvatureY, ImageData^ mask);

        /**
        * Using dynamics calibration to adjust background noise
        *
        * @param img                            - initial image
        * @param mask                           - mask to apply
        * @param calibrationDynamicCoef         - noise level of the calibration wafer, obtained by calibration of current machine (for tool matching) in radians / pixel (default value is 0.005 and coming from Matlab calculation on PSD prototype).
        * @param noisyGrayLevel                 - target background gray level (default value is 20).
        * @param userDynamicCoef                - additional term from user, for special defects. Must be > 0 (default value is 1).
        */
        static ImageData^ ApplyDynamicCalibration(ImageData^ img, ImageData^ mask, float calibrationDynamicCoef, float noisyGrayLevel, float userDynamicCoef);

        /**
        * Using dynamics coefficient to adjust dark image
        *
        * @param dark                           - initial image
        * @param mask                           - mask to apply
        * @param percentageOfLowSaturation      - percentage of low saturation (default value is 0.03).
        * @param dynamicCoef                    - dynamic coeficient (default value is 20).
        */
        static ImageData^ ApplyDynamicCoefficient(ImageData^ img, ImageData^ mask, float dynamicCoef, float percentageOfLowSaturation);

        /**
        * Two-dimensional phase unwrapping based on multiperiod phase unwrapping algorithm.
        *
        * @param wrappedPhaseMaps     - The phase map of type CV_32FC1 wrapped between [-pi, pi], at each period
        * @param mask                 - Binary mask of same dimension than phases images to select wafer area in witch calculate the unwrapping
        * @param periods              - Period associated at each phase map
        * @param nbPeriod             - Number of different periods
        *
        * @return the nwrapped phase map, stored in CV_32FC1 Mat
        */
        static ImageData^ MultiperiodUnwrap(array<ImageData^>^ wrappedPhaseMaps, ImageData^ mask, array<int>^ periods, int nbPeriod);

        /**
        * Substract the global wafer plane from an unwrapped phase map
        *
        * @param unwrappedPhase       - The phase map to substract the plane from
        * @param mask                 - Binary mask of same dimension than the phase map to select wafer area in which to substract the plane from
        *
        * @return the wrapped phase map, with the plane substracted
        */
        static ImageData^ SubstractPlaneFromUnwrapped(ImageData^ unwrappedPhase, ImageData^ mask);
    };
} // namespace UnitySCSharedAlgosOpenCVWrapper
