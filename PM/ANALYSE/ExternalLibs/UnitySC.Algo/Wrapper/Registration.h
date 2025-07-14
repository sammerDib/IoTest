#pragma once

#include <BaseAlgos/Registration.hpp>
#include <ImageData.h>
#include <Tools.h>

namespace AlgosLibrary {

    public ref struct RegistrationInfos {
        double PixelShiftX;
        double PixelShiftY;
        double AngleInDegrees;
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
         *
         * @return A data structure with the realigned image, the values used to realign the image and the similarity scores before and after realignment
         */
        static RegistrationInfos^ ImgRegistration(ImageData^ refImg, ImageData^ sensedImg, RegionOfInterest^ regionOfInterest);
    };
}