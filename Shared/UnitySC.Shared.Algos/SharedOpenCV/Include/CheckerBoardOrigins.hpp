#pragma once

#include <opencv2/opencv.hpp>

#pragma unmanaged

namespace psd {
    /**
     * Origins of the four checker boards on calibration wafer
     */
    struct CheckerBoardsOrigins {
        CheckerBoardsOrigins()
        {
            LeftCheckerBoardOrigin = cv::Point2f(-130.97f, 31.5f);
            TopCheckerBoardOrigin = cv::Point2f(-31.5f, 130.97f);
            RightCheckerBoardOrigin = cv::Point2f(67.97f, 31.5f);
            BottomCheckerBoardOrigin = cv::Point2f(-31.5f, -67.97f);
        }

        CheckerBoardsOrigins(cv::Point2f leftCheckerBoardOrigin, cv::Point2f bottomCheckerBoardOrigin, cv::Point2f rightCheckerBoardOrigin, cv::Point2f topCheckerBoardOrigin)
        {
            LeftCheckerBoardOrigin = leftCheckerBoardOrigin;
            TopCheckerBoardOrigin = topCheckerBoardOrigin;
            RightCheckerBoardOrigin = rightCheckerBoardOrigin;
            BottomCheckerBoardOrigin = bottomCheckerBoardOrigin;
        }

        cv::Point2f LeftCheckerBoardOrigin;
        cv::Point2f TopCheckerBoardOrigin;
        cv::Point2f RightCheckerBoardOrigin;
        cv::Point2f BottomCheckerBoardOrigin;
    };
}