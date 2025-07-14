#pragma once

#include <opencv2/opencv.hpp>
#include "CheckerBoardsSettings.hpp"

#pragma unmanaged

namespace psd {
    /**
     * Input parameters to compute system calibration
     */
    struct InputSystemParameters {
        InputSystemParameters() {
            EdgeExclusion = 0.0f;
            WaferRadius = 150.0f;
            NbPtsScreen = 500.0f;
            FrangePixels = 36.0f;
            ScreenPixelSizeMm = 0.2451f; //245.1 microns
            CheckerBoards = CheckerBoardsSettings();
        }

        InputSystemParameters(CheckerBoardsSettings checkerBoardsSettings, float edgeExclusion, float waferRadius, float nbPtsScreen, float frangePixels, float screenPixelSize) {
            EdgeExclusion = edgeExclusion;
            WaferRadius = waferRadius;
            NbPtsScreen = nbPtsScreen;
            FrangePixels = frangePixels;
            CheckerBoards = checkerBoardsSettings;
            ScreenPixelSizeMm = screenPixelSize;
        }

        float EdgeExclusion;
        float WaferRadius;
        float NbPtsScreen;
        float FrangePixels;
        float ScreenPixelSizeMm;

        CheckerBoardsSettings CheckerBoards;
    };
}