#pragma once
#include "BareWaferAlignmentImageData.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    public ref class Stitcher {
        public:
             /**
             * @brief Stitches images together
             * @details Uses image coordinates to stitch images on a black image big enough to fit all of the images combined
             *
             * @param images :          - list of images to stitch (must be either a PositionImageData list or a BareWaferAlignmentData list).
             * @return : the stitched image
             */
            static PositionImageData^ StitchImages(List<PositionImageData^>^ images);
            static BareWaferAlignmentImageData^ StitchImages(List<BareWaferAlignmentImageData^>^ images);
    };
    

}

