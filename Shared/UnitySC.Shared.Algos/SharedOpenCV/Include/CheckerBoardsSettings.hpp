#pragma once

#include <opencv2/opencv.hpp>
#include "CheckerBoardOrigins.hpp"

#pragma unmanaged

namespace psd {
    /**
     * Settings of checker boards on calibration wafer
     */
    struct CheckerBoardsSettings {
        CheckerBoardsSettings()
        {
            UseAllCheckerBoards = true;
            SquareXNb = 11;
            SquareYNb = 11;
            SquareSizeMm = 7.0f;
            PixelSizeMm = 0.030f; //(30 microns)

            CheckerBoardsTopLeftOrigins = CheckerBoardsOrigins();
        }

        CheckerBoardsSettings(CheckerBoardsOrigins checkerBoardOrigins, int squareXNumber, int squareYNumber, float squareSizeMm, float pixelSize, bool useAllCheckerBoards)
        {
            UseAllCheckerBoards = useAllCheckerBoards;

            SquareXNb = squareXNumber;
            SquareYNb = squareYNumber;
            SquareSizeMm = squareSizeMm;
            PixelSizeMm = pixelSize;

            CheckerBoardsTopLeftOrigins = checkerBoardOrigins;
        }

        bool UseAllCheckerBoards;
        int SquareXNb;
        int SquareYNb;
        float SquareSizeMm;
        float PixelSizeMm;
        CheckerBoardsOrigins CheckerBoardsTopLeftOrigins;
    };
}