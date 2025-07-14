#pragma once

#include "CRegistration.hpp"
#include "ImageData.h"
#include "Tools.h"
#include "Filter.h"

namespace UnitySCSharedAlgosOpenCVWrapper {

    public ref struct RegistrationInfos {
        double PixelShiftX;
        double PixelShiftY;
        double AngleInDegrees;
        double Confidence;
    };

    public ref struct RegistrationQualityInfos {
        double InitialSimilarityScore;
        double FinalSimilarityScore;
        ImageData^ ImgRegistered;
    };

    public ref class Registration {

    public:
        /**
         * Compute an alignement of a sensed image according to a reference image
         *
         * @param refImg              - reference image
         * @param sensedImg           - image to align
         * @param regionOfInterest    - rectangle to select the region of interest (ROI) on images
         * @param dilationSize        - dilation size to use for the mask of the pattern rec (Set to 0 to not apply any dilation mask)
         * @param reportPath           - path to report the images
         *
         * @return A data structure with the realigned image, the values used to realign the image and the similarity scores before and after realignment
         */
        static RegistrationInfos^ ImgRegistration(ImageData^ refImg, ImageData^ sensedImg, RegionOfInterest^ regionOfInterest, int dilationSize, System::String^ reportPath);


        /*
         * Overload angles / scale limits and reporting
         * @param refImg               - reference image
         * @param sensedImg            - image to align
         * @param regionOfInterest     - rectangle to select the region of interest (ROI) on images
         * @param angleSigmaTolerance  - Tolerance sigma in degrees in which the rotation angle is considered acceptable ( angles values too divergent from the sigma will be forced to 0.0)
         * @param scaleSigmaTolerance  - Tolerance sigma in which the scale is considered acceptable(scale values too divergent from the sigma will be forced to 1.0)
         * @param dilationSize        - dilation size to use for the mask of the pattern rec (Set to 0 to not apply any dilation mask)
         * @param reportPath           - path to report the images
         * 
         * @return A data structure with the realigned image, the values used to realign the image and the similarity scores before and after realignment
         */
        static RegistrationInfos^ ImgRegistration(ImageData^ refImg, ImageData^ sensedImg, RegionOfInterest^ regionOfInterest, double angleSigmaTolerance, double scaleSigmaTolerance, int dilationSize, System::String^ reportPath);

        /**
         * Compute image registration quality
         *
         * @param refImg              - reference image
         * @param sensedImg           - image to align
         * @param registrationData    - alignment data, use to realign sensed image according to reference image
         * @param regionOfInterest    - rectangle to select the region of interest (ROI) on images
         *
         * @return Quality of the image registration
         */
        static RegistrationQualityInfos^ ComputeQualityOfRegistration(ImageData^ refImg, ImageData^ sensedImg, RegistrationInfos^ registrationData, RegionOfInterest^ regionOfInterest, BlurFilterMethod filterMethod, double gamma, bool removeNoise);


        /**
         * Realign images according to shift
         *
         * @param refImg              - reference image
         * @param sensedImg           - image to align
         * @param registrationData    - alignment data, use to realign sensed image according to reference image
         *
         * @return realigned image
         */
        static ImageData^ RealignImages(ImageData^ refImg, ImageData^ sensedImg, RegistrationInfos^ registrationData);
    };
}