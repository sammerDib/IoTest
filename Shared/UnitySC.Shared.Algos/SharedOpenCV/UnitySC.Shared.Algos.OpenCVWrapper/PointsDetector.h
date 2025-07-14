#pragma once

#include "Tools.h"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref struct TransformationParameters {
        TransformationParameters() {
            Translation = gcnew Point2d(0, 0);
        }

        double RotationRad;
        double Scale;
        Point2d^ Translation;
    };

    public ref class PointsDetector {
    public:
        /* Computes an optimal transformation parameters with 4 degrees of freedom between two 2D point sets:
         *  - the rotation angle
         *  - the scaling factor
         *  - the translations in x,y axes
         *
         * @param from - Origin point set
         * @param to   - Destination point set
         *
         * @return TransformationParameters to apply to transform the origin points to points closest to the destination ones.
         */
        static TransformationParameters^ OptimalTransformationParameters(array<Point2d^>^ from, array<Point2d^>^ to);
    };
}
