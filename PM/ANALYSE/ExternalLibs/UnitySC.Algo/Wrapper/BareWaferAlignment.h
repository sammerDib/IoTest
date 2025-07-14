#pragma once

#include <BareWaferAlignmentImageData.h>
#include <Tools.h>

namespace AlgosLibrary {
    using namespace System;
    using namespace System::Collections::Generic;
    using namespace System::Runtime::InteropServices;

    public enum class WaferType : int {
        UNKNOWN,
        NOTCH
    };

    public ref struct WaferInfos {
        int RadiusInMicrons;
        WaferType Type;
    };

    public ref struct ContourExtractionResult {
        ContourExtractionResult() {
            Status = gcnew AlgosLibrary::ExecutionStatus();
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
        * @param imageData      - images with data used to perfom computation
        * @param waferData      - wafer data on which the images were taken
        * @param reportPath     - optional path to store report files
        * 
        * @return Data allowing wafer alignment
        */
        static BareWaferAlignmentResult^ PerformBareWaferAlignment(List<BareWaferAlignmentImageData^>^ images, WaferInfos^ waferData, [OptionalAttribute] String^ reportPath);

        /**
        * Perform contour extraction on images taken on the edges of the wafer
        *
        * @param imageData      - images with data used to perform computation
        * @param reportPath     - optional path to store report files
        * 
        * @return Contour extracted
        */
        static ContourExtractionResult^ PerformEdgeImageContourExtraction(BareWaferAlignmentImageData^ imageData, [OptionalAttribute] String^ reportPath);
    };
}
