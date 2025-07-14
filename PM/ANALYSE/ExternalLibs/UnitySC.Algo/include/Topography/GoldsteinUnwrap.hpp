#pragma once

#include <opencv2/opencv.hpp>

namespace phase_unwrapping {

    /*
     * Describes a pixel index
     */
    struct Pixel {
        Pixel(int row, int col) : Row(row), Column(col) { Value = -1; }
        Pixel(int row, int col, uchar value) : Row(row), Column(col), Value(value) {}

        int Row;
        int Column;
        uchar Value;

        bool operator==(const Pixel& pixel)
        {
            return Row == pixel.Row && Column == pixel.Column;
        }
    };

    struct ResiduesData {
        ResiduesData() { DataMatrix = cv::Mat(); }
        ResiduesData(cv::Mat& dataMatrix) { dataMatrix.copyTo(DataMatrix); }

        cv::Mat DataMatrix;

        /**
         * Create image with residues data: black pixels for border, blue pixels for negative residues and red pixel for positive residues.
         */
        cv::Mat ToRGBImage();
    };

    struct BranchCutsData {
        BranchCutsData() { DataMatrix = cv::Mat(); }
        BranchCutsData(cv::Mat& dataMatrix) { dataMatrix.copyTo(DataMatrix); }

        cv::Mat DataMatrix;

        /**
         * Create image with branch cuts data : green pixels for branch cuts.
         */
        cv::Mat ToRGBImage();
    };

    /*
     * Contains data recorded during unwrapping
     */
    struct GoldsteinUnwrappingResult {
        GoldsteinUnwrappingResult() {
            Residues = ResiduesData();
            BranchCuts = BranchCutsData();
            UnwrappedPhase = cv::Mat();
        }
        GoldsteinUnwrappingResult(ResiduesData residues, BranchCutsData branchCuts, cv::Mat unwrappedPhase) : Residues(residues), BranchCuts(branchCuts), UnwrappedPhase(unwrappedPhase){}

        ResiduesData Residues;
        BranchCutsData BranchCuts;
        cv::Mat UnwrappedPhase;
    };

    /**
     * Two-dimensional phase unwrapping based on 2D Goldstein branch cut phase unwrapping algorithm.
     * Reference: [R. M. Goldstein, H. A. Zebken, and C. L. Werner] Satellite radar interferometry : Two dimensional phase unwrapping
     *
     * @param wrappedPhaseMap     - The phase map of type CV_32FC1 wrapped between [-pi, pi]
     *
     * @return the unwrapped result with unwrapped phase map, stored in CV_32FC1 Mat, the residues map and branch cuts map stored in CV_8UC1
     */
    GoldsteinUnwrappingResult GoldsteinUnwrap(cv::Mat& wrappedPhaseMap);

} // namespace phase_unwrapping