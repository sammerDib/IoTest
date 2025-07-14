#pragma once

#include "Tools.h"
#include "PSDCalibration.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref class NanoTopography {
    public:
        /**
        * (WIP!!!!, right now this method does not return anything
           and just reports/saves debug images from the normals computation
        *  which is just an intermediate step of the nanotopography)
        *
        * @param unwrappedX                 - phase unwrap of the X direction
        * @param unwrappedY                 - phase unwrap of the Y direction
        * @param mask                       - binary image to select the calculation area
        * @param calibrationParams          - intrinsic camera calibration result from a previous calibration
        * @param extrinsicCameraParams      - extrinsic camera calibration result from a previous calibration
        * @param extrinsicScreenParams      - extrinsic screen calibration result from a previous calibration
        * @param screenPixelSizeInMm        - pixel size of the screen in millimeters
        * @param fringePeriodInPixels       - size of the fringe period in pixels
        * @param reportPath                 - report path where to save debug images of the intermediate steps
        *
        * @return nothing for now (WIP)
        */
        static void ComputeNanoTopography(ImageData^ unwrappedX, ImageData^ unwrappedY, ImageData^ mask, CalibrationParameters^ calibrationParams, ExtrinsicCameraParameters^ extrinsicCameraParams, ExtrinsicScreenParameters^ extrinsicScreenParams, float screenPixelSizeInMm, int fringePeriodInPixels, [OptionalAttribute] String^ reportPath);
    };
}