#include <assert.h>
#include <math.h>
#include <vector>

#include "GoldsteinUnwrap.hpp"
#include "ErrorLogging.hpp"

#pragma unmanaged

#define POS_RES 0x01                    // 1st bit
#define NEG_RES 0x02                    // 2nd bit
#define BRANCH_CUT 0x04                 // 3rd bit
#define BORDER 0x08                     // 4th bit
#define UNWRAPPED 0x10                  // 5th bit
#define RESIDUE (POS_RES | NEG_RES)

using namespace cv;
using namespace std;

namespace phase_unwrapping {

  namespace {

    enum PixelPosition { ABOVE, BELOW, RIGHT, LEFT };

    /**
     * Calculates the phase residues for a given wrapped phase image.
     * Info : Because of external disturbance, the phase data will be affected by noise and appear discontinuous points.
     *        Mark the discontinuous points in the wrapped phase diagram as positive or negative residual points in the flags array.
     *
     * @param wrappedPhaseMap      - the wrapped phase map
     * @param bitFlags             - the bit flags array
     */
    void Residues(cv::Mat& wrappedPhaseMap, cv::Mat& bitFlags);

    /**
     * Calculate difference between two pixels, wrapped in the interval (-PI,PI）
     * Info : When the difference exceeds (-PI,PI) , this method wrap it to ensure that it returns to a single cycle.
     *
     * @param p1     - First pixel
     * @param p2     - Second pixel
     *
     * @return Delta value
     */
    float WindingDifference(float p1, float p2);

    /**
     * Find and process the adjoining residues of opposite sign : the connection forms a branch cut in the flags array.
     *
     * @param bitFlags      - the bit flags array
     */
    void PlaceBranchCuts(cv::Mat& bitFlags);

    /**
     * Try to balance a residues with near residues of opposite sign.
     * Info: A search window is used to browse pixel information at the edge of the window,
     *       starting with adjacent pixels and then expanding the search if necessary.
     *
     * @param bitFlags                 - the bit flags array
     * @param centerPixel              - the pixel to balance
     * @param searchWindowBoundary     - the given search window boundary (begin with searchWindowBoundary = 3 before expend at 5 and 7).
     * @param adjacentResidues         - the adjacent pixels stored if the equilibrium state is not reached with the given search window boundary
     *
     * @return Remaining charge : 0 if the given pixel has been balanced, -1 or +1 if the pixel and its adjacent pixels have not been balanced.
     */
    int TryToBalanceCenterPixelWithPixelsAtTheEdgeOfTheSearchWindow(cv::Mat& bitFlags, phase_unwrapping::Pixel& centerPixel, int searchWindowBoundary, std::vector<Pixel>& adjacentResidues);

    /**
     * Connect single residue (not balanced in previous step) with any pixel that can be connected (no matter positive or negative).
     * It can avoid the omission of some residual points
     *
     * @param bitFlags     - the bit flags array
     */
    void ConnectSingleResidues(cv::Mat& bitFlags);

    /**
     * Place a branch cut in the bitflags array from a pixel to another.
     * Info : All pixels on the approximate straight line between two points are considered to be online.
     *
     * @param bitFlags       - the bit flags array
     * @param pixel1         - the first pixel
     * @param pixel2         - the second pixel
     */
    void PlaceCutAlongBranchTangent(cv::Mat& bitFlags, phase_unwrapping::Pixel& pixel1, phase_unwrapping::Pixel& pixel2);

    /**
     * Unwrap the phase data (by Itoh's method) without crossing any branch cuts.
     *
     * @param wrappedPhase         - the wrapped phase map
     * @param bitFlags             - the bit flags array
     *
     * @return Return the unwrapped phase map
     */
    cv::Mat FloodFillUnwrappingAroundCuts(cv::Mat& wrappedPhase, cv::Mat& bitFlags);

    /**
     * Select a known true phase reference pixel on the full image
     * (i.e. a pixel that is not on a cut branch and not on a border)
     *
     * @param wrappedPhase         - the wrapped phase map
     * @param bitFlags             - the bit flags array
     *
     * @return Return the reference pixel
     */
    phase_unwrapping::Pixel FindReferencePoint(cv::Mat& wrappedPhase, cv::Mat& bitFlags);

    /**
     * Find the non unwrapped neighboring pixels of the given pixel to process unwrapping.
     *
     * @param bitFlags                    - the bit flags array
     * @param currentPixel                - the given pixel
     * @param currentPixelPosition        - the given pixel position
     *
     * @return All non unwrapped neighboring pixels.
     */
    std::vector<Pixel> FindNonUnwrappedNeighbours(cv::Mat& bitFlags, phase_unwrapping::Pixel& currentPixel, phase_unwrapping::PixelPosition currentPixelPosition);

    /**
     * Find the non unwrapped pixels on the whole image.
     *
     * @param bitFlags                    - the bit flags array
     *
     * @return All non unwrapped pixels.
     */
    std::vector<Pixel> FindNonUnwrappedPixels(cv::Mat& bitFlags);

    /**
     * Unwrap a pixel.
     *
     * @param wrappedPhase                   - the wrapped phase map
     * @param unwrappedPhase                 - the resulting unwrapped phase map
     * @param bitFlags                       - the bit flags array
     * @param adjoinPixelAlreadyUnwrapped    - an adjacent pixel already unpacked, used as a reference
     * @param pixelToUnwrap                  - the pixel to unwrap
     */
    void UnwrapPixel(cv::Mat& wrappedPhase, cv::Mat& unwrappedPhase, cv::Mat& bitFlags, phase_unwrapping::Pixel& adjoinPixelAlreadyUnwrapped, phase_unwrapping::Pixel& pixelToUnwrap);

  } // namespace

  GoldsteinUnwrappingResult GoldsteinUnwrap(Mat &wrappedPhaseMap) {

    if (wrappedPhaseMap.type() != CV_32FC1) {
      ErrorLogging::LogError("[Goldstein unwrap] Input phase map should be stored into a single channel 32-bits float");
      return GoldsteinUnwrappingResult(ResiduesData(), BranchCutsData(), Mat());
    }

    double minMax = CV_PI + 0.01;
    bool validRangeValues = checkRange(wrappedPhaseMap, true, 0, -minMax, minMax) && !checkRange(wrappedPhaseMap, true, 0, 0, 1);
    if (!validRangeValues) {
      ErrorLogging::LogError("[Goldstein unwrap] Input phase map should be wrapped between [-pi, pi]");
      return GoldsteinUnwrappingResult(ResiduesData(), BranchCutsData(), Mat());
    }

    // Compute initial bit flags with border flag
    int height = wrappedPhaseMap.rows;
    int width = wrappedPhaseMap.cols;
    Mat bitFlags(height, width, CV_8UC1);
    bitFlags = Scalar::all(BORDER);
    bitFlags(Rect(1, 1, width - 2, height - 2)) = 0;

    // Calculate phase residues
    Residues(wrappedPhaseMap, bitFlags);
    ResiduesData residues = ResiduesData(bitFlags);

    // Place branch cuts
    PlaceBranchCuts(bitFlags);
    BranchCutsData branchcuts = BranchCutsData(bitFlags);

    // Unwrap around cuts
    Mat unwrappedPhaseMap = FloodFillUnwrappingAroundCuts(wrappedPhaseMap, bitFlags);
    return GoldsteinUnwrappingResult(residues, branchcuts, unwrappedPhaseMap);;
  }

  Mat ResiduesData::ToRGBImage(){

      if (ResiduesData::DataMatrix.type() != CV_8UC1) {
          ErrorLogging::LogError("[ResiduesData::ToRGBImage] Residues map should be stored into a single channel 8-bits uchar");
          return Mat();
      }

      int height = ResiduesData::DataMatrix.rows;
      int width = ResiduesData::DataMatrix.cols;

      Mat phaseResiduesImg(height, width, CV_8UC3, Scalar(255, 255, 255));
      for (int row = 0; row < ResiduesData::DataMatrix.rows; row++) {
          for (int col = 0; col < ResiduesData::DataMatrix.cols; col++) {
              if (ResiduesData::DataMatrix.at<uchar>(row, col) & BORDER) {
                  Vec3b black(0, 0, 0);
                  phaseResiduesImg.at<Vec3b>(Point(col, row)) = black;
              }
              else if (ResiduesData::DataMatrix.at<uchar>(row, col) & POS_RES) {
                  Vec3b blue(255, 0, 0);
                  phaseResiduesImg.at<Vec3b>(Point(col, row)) = blue;
              }
              else if (ResiduesData::DataMatrix.at<uchar>(row, col) & NEG_RES) {
                  Vec3b red(0, 0, 255);
                  phaseResiduesImg.at<Vec3b>(Point(col, row)) = red;
              }
          }
      }

      return phaseResiduesImg;
  }

  Mat BranchCutsData::ToRGBImage() {

      if (BranchCutsData::DataMatrix.type() != CV_8UC1) {
          ErrorLogging::LogError("[ResiduesData::ToRGBImage] Residues map should be stored into a single channel 8-bits uchar");
          return Mat();
      }

      int height = BranchCutsData::DataMatrix.rows;
      int width = BranchCutsData::DataMatrix.cols;

      Mat branchCutsImg(height, width, CV_8UC3, Scalar(255, 255, 255));
      for (int row = 0; row < BranchCutsData::DataMatrix.rows; row++) {
          for (int col = 0; col < BranchCutsData::DataMatrix.cols; col++) {
              if (BranchCutsData::DataMatrix.at<uchar>(row, col) & BRANCH_CUT) {
                  Vec3b green(0, 255, 0);
                  branchCutsImg.at<Vec3b>(Point(col, row)) = green;
              }
              else if (BranchCutsData::DataMatrix.at<uchar>(row, col) & NEG_RES) {
                  Vec3b blue(255, 0, 0);
                  branchCutsImg.at<Vec3b>(Point(col, row)) = blue;
              }
              else if (BranchCutsData::DataMatrix.at<uchar>(row, col) & POS_RES) {
                  Vec3b red(0, 0, 255);
                  branchCutsImg.at<Vec3b>(Point(col, row)) = red;
              }
              else if (BranchCutsData::DataMatrix.at<uchar>(row, col) & BORDER) {
                  Vec3b black(0, 0, 0);
                  branchCutsImg.at<Vec3b>(Point(col, row)) = black;
              }
          }
      }
      return branchCutsImg;
  }

  namespace {
    void Residues(Mat &wrappedPhaseMap, Mat &bitFlags) {
      int height = wrappedPhaseMap.rows;
      int width = wrappedPhaseMap.cols;

      for (int row = 0; row < height - 1; row++) {
        for (int col = 0; col < width - 1; col++) {
          // Don't unwrap masked regions
          if (bitFlags.at<uchar>(row, col) & (BORDER) || bitFlags.at<uchar>(row, col + 1) & (BORDER) || bitFlags.at<uchar>(row + 1, col + 1) & (BORDER) || bitFlags.at<uchar>(row + 1, col) & (BORDER)) {
            continue;
          }

          // Note that by convention the positions of the phase residues are marked on the top left corner of the 2 by 2 regions.
          //    active---res1---right
          //      |              |
          //     res4           res2
          //      |              |
          //   below---res3---belowright
          float active = wrappedPhaseMap.at<float>(row, col);
          float right = wrappedPhaseMap.at<float>(row, col + 1);
          float below = wrappedPhaseMap.at<float>(row + 1, col);
          float belowRight = wrappedPhaseMap.at<float>(row + 1, col + 1);

          // Calculate the difference between adjacent phases in a clockwise direction
          double res1 = WindingDifference(right, active);
          double res2 = WindingDifference(belowRight, right);
          double res3 = WindingDifference(below, belowRight);
          double res4 = WindingDifference(active, below);
          double tempResidues = res1 + res2 + res3 + res4;

          // The sum of the residuals equal to positive integer multiple of 2*pi indicate a positive residue
          if (tempResidues > 0)
            bitFlags.at<uchar>(row, col) |= POS_RES;
          // The sum of the residuals equal to negative integer multiple of 2*pi indicate a negative residue
          else if (tempResidues < 0)
            bitFlags.at<uchar>(row, col) |= NEG_RES;
          // The sum of the residuals equal to zero indicates that the four points are continuous
        }
      }
    }

    float WindingDifference(float p1, float p2) {
      float delta = p1 - p2;

      // when the difference exceeds (-PI,PI）in the interval, wrap it (add or subtract from this value 2PI, ensure that it returns to a single cycle
      if (delta > CV_PI)
        delta -= (float)(2.0 * CV_PI);
      if (delta < -CV_PI)
        delta += (float)(2.0 * CV_PI);

      return delta;
    }

    void PlaceBranchCuts(Mat &bitFlags) {

      // Set initial size of search window centered on the residual point r*r
      int initialSearchWindowSize = 3;
      int maxSearchWindowSize = 7;
      int searchWindowIncrement = 2;

      // Loop through the residues
      for (int row = 0; row < bitFlags.rows; row++) {
        for (int col = 0; col < bitFlags.cols; col++) {

          Pixel centerPixel = Pixel(row, col, bitFlags.at<uchar>(row, col));
          bool centerPixelIsResidueNotConnected = (centerPixel.Value & RESIDUE) && !(centerPixel.Value & BRANCH_CUT);
          if (!centerPixelIsResidueNotConnected) {
            continue;
          }

          vector<Pixel> adjacentResidues;
          int searchWindowBoundary = (int)floor(initialSearchWindowSize / 2);
          int chargeCounter = TryToBalanceCenterPixelWithPixelsAtTheEdgeOfTheSearchWindow(bitFlags, centerPixel, searchWindowBoundary, adjacentResidues);

          // Loop until balanced, or until the maximum size of search windows is reached
          while (chargeCounter != 0 && searchWindowBoundary < floor(maxSearchWindowSize / 2)) {

            // If there are adjacent residues
            if (!adjacentResidues.empty()) {
              centerPixel = adjacentResidues.back();
              adjacentResidues.pop_back();
            }
            // If the charge is still not balanced after moving through all adjacent residues, increase the box radius and centre the larger box about the original active residue.
            else {
              centerPixel = Pixel(row, col, bitFlags.at<uchar>(row, col));
              searchWindowBoundary++;
            }

            vector<Pixel> temp;
            chargeCounter = TryToBalanceCenterPixelWithPixelsAtTheEdgeOfTheSearchWindow(bitFlags, centerPixel, searchWindowBoundary, temp);
          }
        }
      }
      ConnectSingleResidues(bitFlags);
    }

    int TryToBalanceCenterPixelWithPixelsAtTheEdgeOfTheSearchWindow(Mat &bitFlags, Pixel &centerPixel, int searchWindowBoundary, vector<Pixel> &adjacentResidues) {
      // Try to balance center pixel with pixels at the edge of the search window

      int chargeCounter = centerPixel.Value & POS_RES ? 1 : -1;

      for (int edgePixelRow = centerPixel.Row - searchWindowBoundary; edgePixelRow <= centerPixel.Row + searchWindowBoundary; edgePixelRow++) {
        for (int edgePixelCol = centerPixel.Column - searchWindowBoundary; edgePixelCol <= centerPixel.Column + searchWindowBoundary; edgePixelCol++) {

          bool equilibriumStateAlreadyReached = chargeCounter == 0;
          if (equilibriumStateAlreadyReached) {
            return chargeCounter;
          }

          bool isFirstOrLastRow = (edgePixelRow == centerPixel.Row - searchWindowBoundary) || (edgePixelRow == centerPixel.Row + searchWindowBoundary);
          bool isFirstOrLastColumn = (edgePixelCol == centerPixel.Column - searchWindowBoundary) || (edgePixelCol == centerPixel.Column + searchWindowBoundary);
          bool isEdgePixel = isFirstOrLastRow || isFirstOrLastColumn;
          if (!isEdgePixel) {
            continue;
          }

          bool isOutOfBounds = edgePixelCol < 0 || edgePixelCol >= bitFlags.cols || edgePixelRow < 0 || edgePixelRow >= bitFlags.rows;
          if (isOutOfBounds) {
            continue;
          }

          Pixel edgePixel = Pixel(edgePixelRow, edgePixelCol, bitFlags.at<uchar>(edgePixelRow, edgePixelCol));

          // A border is found, the connection forms a branch cut
          bool isBorder = edgePixel.Value & BORDER; // edgePixelCol == 0 || edgePixelCol == bitFlags.cols - 1 || edgePixelRow == 0 || edgePixelRow == bitFlags.rows - 1
          if (isBorder) {
            PlaceCutAlongBranchTangent(bitFlags, centerPixel, edgePixel);
            chargeCounter = 0;
            return chargeCounter;
          }

          // Another residual point is found, the connection forms a branch tangent and judge its polarity.
          bool edgePixelIsResidue = edgePixel.Value & RESIDUE;
          if (edgePixelIsResidue) {
            // Opposite polarity : the equilibrium state is reached, branch tangent is a branch cut and end this round of search directly.
            bool edgePixelIsResidueOfOppositePolarity = (centerPixel.Value & NEG_RES && edgePixel.Value & POS_RES) || (centerPixel.Value & POS_RES && edgePixel.Value & NEG_RES);
            if (edgePixelIsResidueOfOppositePolarity) {
              PlaceCutAlongBranchTangent(bitFlags, centerPixel, edgePixel);
              chargeCounter = 0;
              return chargeCounter;
            }
            // Same polarity : the equilibrium state is not reached, keep looking for the next one edge pixel.
            int edgePixelCharge = edgePixel.Value & POS_RES ? 1 : -1;
            chargeCounter += edgePixelCharge;
            adjacentResidues.push_back(edgePixel);
          }
        }
      }
      return chargeCounter;
    }

    void ConnectSingleResidues(Mat &bitFlags) {
      // Ensures that single satellite residues are not left unaccounted for.
      // The box is simply expanded regardless until the border or ANY other residue is found.

      // Set initial size of search window centered on the residual point r*r
      int initialSearchWindowSize = 3;
      int min = std::min(bitFlags.rows, bitFlags.cols);
      int maxSearchWindowSize = min / 2;
      int searchWindowIncrement = 2;

      // Loop through the residues
      for (int row = 0; row < bitFlags.rows; row++) {
        for (int col = 0; col < bitFlags.cols; col++) {

          Pixel centerPixel = Pixel(row, col, bitFlags.at<uchar>(row, col));
          bool centerPixelIsSingleResidue = (centerPixel.Value & RESIDUE) && !(centerPixel.Value & BRANCH_CUT);
          if (!centerPixelIsSingleResidue) {
            continue;
          }

          for (int boxSize = initialSearchWindowSize; boxSize <= maxSearchWindowSize; boxSize += 2) {
            Pixel centerPixel = Pixel(row, col, bitFlags.at<uchar>(row, col));
            bool centerPixelIsAlreadyConnected = centerPixel.Value & BRANCH_CUT;
            if (centerPixelIsAlreadyConnected) {
              continue;
            }

            int searchWindowBoundary = (int)floor(boxSize / 2);
            for (int edgePixelRow = centerPixel.Row - searchWindowBoundary; edgePixelRow <= centerPixel.Row + searchWindowBoundary; edgePixelRow++) {
              for (int edgePixelCol = centerPixel.Column - searchWindowBoundary; edgePixelCol <= centerPixel.Column + searchWindowBoundary; edgePixelCol++) {

                Pixel centerPixel = Pixel(row, col, bitFlags.at<uchar>(row, col));
                bool centerPixelIsAlreadyConnected = centerPixel.Value & BRANCH_CUT;
                if (centerPixelIsAlreadyConnected) {
                  continue;
                }

                bool isOutOfBounds = edgePixelCol < 0 || edgePixelCol >= bitFlags.cols || edgePixelRow < 0 || edgePixelRow >= bitFlags.rows;
                if (isOutOfBounds) {
                  continue;
                }

                Pixel edgePixel = Pixel(edgePixelRow, edgePixelCol, bitFlags.at<uchar>(edgePixelRow, edgePixelCol));

                // A border or another residual point is found, the connection forms a branch cut
                bool isBorder = edgePixel.Value & BORDER; // edgePixelCol == 0 || edgePixelCol == bitFlags.cols - 1 || edgePixelRow == 0 || edgePixelRow == bitFlags.rows - 1
                bool isResidue = edgePixel.Value & RESIDUE;
                if (isBorder || isResidue) {
                  PlaceCutAlongBranchTangent(bitFlags, centerPixel, edgePixel);
                  continue;
                }
              }
            }
          }
        }
      }
    }

    void PlaceCutAlongBranchTangent(Mat &bitFlags, Pixel &pixel1, Pixel &pixel2) {
      // Place a branch cut in the bitflags array from pixel1 to pixel2 (pixel2 > pixel1).
      // (all pixels on the approximate straight line between these two points are considered to be online)

      int distCol = pixel2.Column - pixel1.Column;
      int distRow = pixel2.Row - pixel1.Row;

      bitFlags.at<uchar>(pixel1.Row, pixel1.Column) |= BRANCH_CUT;
      pixel1.Value = bitFlags.at<uchar>(pixel1.Row, pixel1.Column);

      if (distCol == 0 && distRow == 0) {
        return;
      }

      if (distCol != 0 && distRow != 0) {
        if ((abs(distCol) == abs(distRow))) {
          for (int i = 1; i <= abs(distCol); i++) {
            int colSign = distCol >= 0 ? 1 : -1;
            int rowSign = distRow >= 0 ? 1 : -1;
            bitFlags.at<uchar>(pixel1.Row + (i * rowSign), pixel1.Column + (i * colSign)) |= BRANCH_CUT;
          }
        } else {
          for (int r = 1; r <= abs(distRow); r++) {
            for (int c = 1; c <= abs(distCol); c++) {
              int colSign = distCol >= 0 ? 1 : -1;
              int rowSign = distRow >= 0 ? 1 : -1;
              bitFlags.at<uchar>(pixel1.Row + (r * rowSign), pixel1.Column + (c * colSign)) |= BRANCH_CUT;
            }
          }
        }
        pixel2.Value = bitFlags.at<uchar>(pixel2.Row, pixel2.Column);
      } else if (distCol != 0) {
        for (int i = 1; i <= abs(distCol); i++) {
          int sign = distCol >= 0 ? 1 : -1;
          bitFlags.at<uchar>(pixel1.Row, pixel1.Column + (i * distCol)) |= BRANCH_CUT;
        }
        pixel2.Value = bitFlags.at<uchar>(pixel2.Row, pixel2.Column);
      } else if (distRow != 0) {
        for (int i = 1; i <= abs(distRow); i++) {
          int sign = distRow >= 0 ? 1 : -1;
          bitFlags.at<uchar>(pixel1.Row + (i * sign), pixel1.Column) |= BRANCH_CUT;
        }
        pixel2.Value = bitFlags.at<uchar>(pixel2.Row, pixel2.Column);
      }
    }

    Mat FloodFillUnwrappingAroundCuts(Mat &wrappedPhase, Mat &bitFlags) {

      Mat unwrappedPhase = Mat::zeros(wrappedPhase.rows, wrappedPhase.cols, CV_32FC1);

      // Step 1 : Unwrap "standard" pixels
      // Unpack from a non residual point in the interferogram to the surrounding direction,
      // if a point on the branch tangent is encountered, the unwrapping stops,
      // until all normal phase points in the whole picture are unwrapped.

      // Select a known true phase reference point
      Pixel referencePixel = FindReferencePoint(wrappedPhase, bitFlags);
      unwrappedPhase.at<float>(referencePixel.Row, referencePixel.Column) = wrappedPhase.at<float>(referencePixel.Row, referencePixel.Column);
      bitFlags.at<uchar>(referencePixel.Row, referencePixel.Column) |= UNWRAPPED;

      // Select the first four adjoining pixels
      vector<Pixel> nextAdjoinPixels = vector<Pixel>();
      nextAdjoinPixels.push_back(Pixel(referencePixel.Row - 1, referencePixel.Column));
      nextAdjoinPixels.push_back(Pixel(referencePixel.Row + 1, referencePixel.Column));
      nextAdjoinPixels.push_back(Pixel(referencePixel.Row, referencePixel.Column - 1));
      nextAdjoinPixels.push_back(Pixel(referencePixel.Row, referencePixel.Column + 1));

      // Loop until there are no adjoining pixels
      while (!nextAdjoinPixels.empty()) {

        vector<Pixel> currentAdjoinPixel = nextAdjoinPixels;
        nextAdjoinPixels = vector<Pixel>();

        for (int i = 0; i < currentAdjoinPixel.size(); i++) {

          Pixel pixelToUnwrap = currentAdjoinPixel[i];

          bool pixelWasAlreadyUnwrapped = (bitFlags.at<uchar>(pixelToUnwrap.Row, pixelToUnwrap.Column) & (UNWRAPPED));
          bool pixelIsOnBorder = (bitFlags.at<uchar>(pixelToUnwrap.Row, pixelToUnwrap.Column) & (BORDER));
          if (pixelIsOnBorder | pixelWasAlreadyUnwrapped) {
            continue;
          }

          // First search below for an adjoining unwrapped phase pixel
          Pixel belowPixel = Pixel(pixelToUnwrap.Row + 1, pixelToUnwrap.Column);
          bool belowPixelIsUnwrapped = (bitFlags.at<uchar>(belowPixel.Row, belowPixel.Column) & (UNWRAPPED));
          if (belowPixelIsUnwrapped) {
            UnwrapPixel(wrappedPhase, unwrappedPhase, bitFlags, belowPixel, pixelToUnwrap);
            vector<Pixel> adjoinPixels = FindNonUnwrappedNeighbours(bitFlags, pixelToUnwrap, PixelPosition::BELOW);
            nextAdjoinPixels.insert(nextAdjoinPixels.end(), adjoinPixels.begin(), adjoinPixels.end());
          }

          // Then search above
          Pixel abovePixel = Pixel(pixelToUnwrap.Row - 1, pixelToUnwrap.Column);
          bool abovePixelIsUnwrapped = (bitFlags.at<uchar>(abovePixel.Row, abovePixel.Column) & (UNWRAPPED));
          if (abovePixelIsUnwrapped) {
            UnwrapPixel(wrappedPhase, unwrappedPhase, bitFlags, abovePixel, pixelToUnwrap);
            vector<Pixel> adjoinPixels = FindNonUnwrappedNeighbours(bitFlags, pixelToUnwrap, PixelPosition::ABOVE);
            nextAdjoinPixels.insert(nextAdjoinPixels.end(), adjoinPixels.begin(), adjoinPixels.end());
          }

          // Then search on the right
          Pixel rightPixel = Pixel(pixelToUnwrap.Row, pixelToUnwrap.Column + 1);
          bool rightPixelIsUnwrapped = (bitFlags.at<uchar>(rightPixel.Row, rightPixel.Column) & (UNWRAPPED));
          if (rightPixelIsUnwrapped) {
            UnwrapPixel(wrappedPhase, unwrappedPhase, bitFlags, rightPixel, pixelToUnwrap);
            vector<Pixel> adjoinPixels = FindNonUnwrappedNeighbours(bitFlags, pixelToUnwrap, PixelPosition::RIGHT);
            nextAdjoinPixels.insert(nextAdjoinPixels.end(), adjoinPixels.begin(), adjoinPixels.end());
          }

          // Then search on the left
          Pixel leftPixel = Pixel(pixelToUnwrap.Row, pixelToUnwrap.Column - 1);
          bool leftPixelIsUnwrapped = (bitFlags.at<uchar>(leftPixel.Row, leftPixel.Column) & (UNWRAPPED));
          if (leftPixelIsUnwrapped) {
            UnwrapPixel(wrappedPhase, unwrappedPhase, bitFlags, leftPixel, pixelToUnwrap);
            vector<Pixel> adjoinPixels = FindNonUnwrappedNeighbours(bitFlags, pixelToUnwrap, PixelPosition::LEFT);
            nextAdjoinPixels.insert(nextAdjoinPixels.end(), adjoinPixels.begin(), adjoinPixels.end());
          }
        }
      }

      // Step 2 : Unwrap branch cut pixels
      // Analyze the normal point phase information around the branch tangent line,
      // combined with the normal information, the phase on the branch tangent is finally unwrapped.

      vector<Pixel> remainingUnwrappedPixels = FindNonUnwrappedPixels(bitFlags);

      while (!remainingUnwrappedPixels.empty())
      {
          for (Pixel pixelToUnwrap : remainingUnwrappedPixels) {

              Pixel belowPixel = Pixel(pixelToUnwrap.Row + 1, pixelToUnwrap.Column);
              bool belowPixelIsValidAndUnwrapped = belowPixel.Row < bitFlags.rows && (bitFlags.at<uchar>(belowPixel.Row, belowPixel.Column) & (UNWRAPPED));

              Pixel abovePixel = Pixel(pixelToUnwrap.Row - 1, pixelToUnwrap.Column);
              bool abovePixelIsValidAndUnwrapped = abovePixel.Row >= 0 && (bitFlags.at<uchar>(abovePixel.Row, abovePixel.Column) & (UNWRAPPED));

              Pixel rightPixel = Pixel(pixelToUnwrap.Row, pixelToUnwrap.Column + 1);
              bool rightPixelIsValidAndUnwrapped = rightPixel.Column < bitFlags.cols && (bitFlags.at<uchar>(rightPixel.Row, rightPixel.Column) & (UNWRAPPED));

              Pixel leftPixel = Pixel(pixelToUnwrap.Row, pixelToUnwrap.Column - 1);
              bool leftPixelIsValidAndUnwrapped = leftPixel.Column >= 0 && (bitFlags.at<uchar>(leftPixel.Row, leftPixel.Column) & (UNWRAPPED));

              if (belowPixelIsValidAndUnwrapped) {
                  UnwrapPixel(wrappedPhase, unwrappedPhase, bitFlags, belowPixel, pixelToUnwrap);
              }
              else if (abovePixelIsValidAndUnwrapped) {
                  UnwrapPixel(wrappedPhase, unwrappedPhase, bitFlags, abovePixel, pixelToUnwrap);
              }
              else if (rightPixelIsValidAndUnwrapped) {
                  UnwrapPixel(wrappedPhase, unwrappedPhase, bitFlags, rightPixel, pixelToUnwrap);
              }
              else if (leftPixelIsValidAndUnwrapped) {
                  UnwrapPixel(wrappedPhase, unwrappedPhase, bitFlags, leftPixel, pixelToUnwrap);
              }
          }
          remainingUnwrappedPixels = FindNonUnwrappedPixels(bitFlags);
      }

      return unwrappedPhase;
    }

    Pixel FindReferencePoint(Mat& wrappedPhase, Mat& bitFlags) {
        // Select a known true phase reference point
        for (int row = 0; row < wrappedPhase.rows; row++) {
            for (int col = 0; col < wrappedPhase.cols; col++) {

                bool isTruePhase = !(bitFlags.at<uchar>(row, col) & (BRANCH_CUT | BORDER));
                if (isTruePhase) {
                    return Pixel(row, col);
                }
            }
        }

        throw exception("Unable to select a known true phase reference point. All points in the image belong to cut branches or borders.");
    }

    vector<Pixel> FindNonUnwrappedNeighbours(Mat& bitFlags, Pixel& currentPixel, PixelPosition currentPixelPosition) {

        vector<Pixel> adjoinPixels;

        Pixel abovePixel = Pixel(currentPixel.Row - 1, currentPixel.Column);
        bool abovePixelMustBeUnwrapped = !(bitFlags.at<uchar>(abovePixel.Row, abovePixel.Column) & (UNWRAPPED | BRANCH_CUT | BORDER));
        if (abovePixelMustBeUnwrapped && currentPixelPosition != PixelPosition::ABOVE) {
            adjoinPixels.push_back(abovePixel);
        }

        Pixel belowPixel = Pixel(currentPixel.Row + 1, currentPixel.Column);
        bool belowPixelMustBeUnwrapped = !(bitFlags.at<uchar>(belowPixel.Row, belowPixel.Column) & (UNWRAPPED | BRANCH_CUT | BORDER));
        if (belowPixelMustBeUnwrapped && currentPixelPosition != PixelPosition::BELOW) {
            adjoinPixels.push_back(belowPixel);
        }

        Pixel leftPixel = Pixel(currentPixel.Row, currentPixel.Column - 1);
        bool leftPixelMustBeUnwrapped = !(bitFlags.at<uchar>(leftPixel.Row, leftPixel.Column) & (UNWRAPPED | BRANCH_CUT | BORDER));
        if (leftPixelMustBeUnwrapped && currentPixelPosition != PixelPosition::LEFT) {
            adjoinPixels.push_back(leftPixel);
        }

        Pixel rightPixel = Pixel(currentPixel.Row, currentPixel.Column + 1);
        bool rightPixelMustBeUnwrapped = !(bitFlags.at<uchar>(rightPixel.Row, rightPixel.Column) & (UNWRAPPED | BRANCH_CUT | BORDER));
        if (rightPixelMustBeUnwrapped && currentPixelPosition != PixelPosition::RIGHT) {
            adjoinPixels.push_back(rightPixel);
        }

        return adjoinPixels;
    }

    vector<Pixel> FindNonUnwrappedPixels(Mat& bitFlags)
    {
        vector<Pixel> remainingUnwrappedPixels;
        for (int row = 0; row < bitFlags.rows; row++) {
            for (int col = 0; col < bitFlags.cols; col++) {
                if (!(bitFlags.at<uchar>(row, col) & (UNWRAPPED))) {
                    remainingUnwrappedPixels.push_back(Pixel(row, col));
                }
            }
        }
        return remainingUnwrappedPixels;
    }

    void UnwrapPixel(Mat& wrappedPhase, Mat& unwrappedPhase, Mat& bitFlags, Pixel& adjoinPixelAlreadyUnwrapped, Pixel& pixelToUnwrap) {

        float phaseRef = unwrappedPhase.at<float>(adjoinPixelAlreadyUnwrapped.Row, adjoinPixelAlreadyUnwrapped.Column);
        float diff = -WindingDifference(wrappedPhase.at<float>(adjoinPixelAlreadyUnwrapped.Row, adjoinPixelAlreadyUnwrapped.Column), wrappedPhase.at<float>(pixelToUnwrap.Row, pixelToUnwrap.Column));
        unwrappedPhase.at<float>(pixelToUnwrap.Row, pixelToUnwrap.Column) = phaseRef + diff;

        bitFlags.at<uchar>(pixelToUnwrap.Row, pixelToUnwrap.Column) |= UNWRAPPED;
    }
  } // namespace
} // namespace phase_unwrapping
