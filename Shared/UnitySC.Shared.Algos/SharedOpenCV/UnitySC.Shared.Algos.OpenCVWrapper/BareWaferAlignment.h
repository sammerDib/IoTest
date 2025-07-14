#pragma once

#include "BareWaferAlignmentImageData.h"
#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    using namespace System;
    using namespace System::Collections::Generic;
    using namespace System::Runtime::InteropServices;

    public enum class WaferType : int {
        UNKNOWN,
        NOTCH
    };

    public enum class ReportOption {
        NoReport,
        OverlayReport,
        AdvancedOverlayReport,
    };

    public ref struct WaferInfos {
        int RadiusInMicrons;
        int NotchWidthInMicrons;
        WaferType Type;
    };

    public ref struct ContourExtractionResult {
        ContourExtractionResult() {
            Status = gcnew ExecutionStatus();
            Contour = gcnew System::Collections::Generic::List<Point2d^>();
        }
        /*! use same status code as in the C++ library*/
        ExecutionStatus^ Status;
        List<Point2d^>^ Contour;
    };

    public ref struct BareWaferAlignmentResult {
        BareWaferAlignmentResult() {
            Status = gcnew ExecutionStatus();
            Shift = gcnew Point2d(0, 0);
        }
        /*! use same status code as in the C++ library*/
        ExecutionStatus^ Status;
        double Rotation;
        Point2d^ Shift;
    };

    public ref class BareWaferAlignment {

    public:
        static const int MINIMAL_IMAGES_FOR_BWA = 3;

        /**
        * Perform wafer alignement with given images and wafer data
        *
        * @param imageData             - images with data used to perfom computation
        * @param waferData             - wafer data on which the images were taken
        * @param edgeDetectionVersion  - version of the edge detection to use for shift computation
        * @param notchDetectionVersion - version of the notch detection to use for rotation computation
        * @param cannyThreshold        - canny threshold used for the wafer edge detection (only used for the 2nd version)
        * @param reportPath            - optional path to store report files
        * @param reportOption          - allows the reporting of additional images with overlay information (edges, notch location, notch similarity..)  
        * 
        * @return Data allowing wafer alignment
        */
        static BareWaferAlignmentResult^ PerformBareWaferAlignment(List<BareWaferAlignmentImageData^>^ images, WaferInfos^ waferData, int edgeDetectionVersion, int notchDetectionVersion, int cannyThreshold, [OptionalAttribute] String^ reportPath, [OptionalAttribute] ReportOption reportOption);

        /**
        * Perform contour extraction on images taken on the edges of the wafer
        *
        * @param imageData             - images with data used to perform computation
        * @param waferDiameterInMm     - known diameter of the wafer present in the image
        * @param edgeDetectionVersion  - version of the edge detection to use for shift computation
        * @param cannyThreshold        - canny threshold used for the wafer edge detection (only used for the 2nd version)
        * @param reportPath            - optional path to store report files
        * @param reportOption          - allows the reporting of additional images with overlay information (edges, notch location, notch similarity..)
        * 
        * @return Contour extracted
        */
        static ContourExtractionResult^ PerformEdgeImageContourExtraction(BareWaferAlignmentImageData^ imageData, int waferDiameterInMm, int edgeDetectionVersion, int cannyThreshold, [OptionalAttribute] String^ reportPath, [OptionalAttribute] ReportOption reportOption);
    };
}
