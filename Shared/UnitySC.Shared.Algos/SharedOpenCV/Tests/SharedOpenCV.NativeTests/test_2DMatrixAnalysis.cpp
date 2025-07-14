#include "CppUnitTest.h"

#define _USE_MATH_DEFINES

#include <opencv2/opencv.hpp>
#include "2DMatrixAnalysis.hpp"

using namespace matrix_2D;

#pragma unmanaged
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedOpenCVNativeTests
{
    TEST_CLASS(D2MatrixAnalysisTest)
    {
    private:
        static constexpr double s_precision = 1e-4;

    public:
        TEST_METHOD(OptimalTransformationParameters_OnlyOnePointThrows)
        {
            // Given Only one point from/to
            std::vector<cv::Point2d> from;
            from.push_back(cv::Point2d(1.0, 0.0));
            
            std::vector<cv::Point2d> to;
            to.push_back(cv::Point2d(1.2, 0.4));

            // When calling function 
            // Then it throws
            Assert::ExpectException<std::exception>([from, to]() {OptimalTransformationParameters(from, to);});
        };

        TEST_METHOD(OptimalTransformationParameters_DifferentNumberOfPointsThrows)
        {
            // Given different number of points from/to
            std::vector<cv::Point2d> from;
            from.push_back(cv::Point2d(1.0, 0.0));
            from.push_back(cv::Point2d(2.0, 0.0));
            from.push_back(cv::Point2d(3.0, 0.0));
            from.push_back(cv::Point2d(4.0, 0.0));

            std::vector<cv::Point2d> to;
            to.push_back(cv::Point2d(1.0, 0.0));
            to.push_back(cv::Point2d(2.0, 0.0));
            to.push_back(cv::Point2d(3.0, 0.0));

            // When calling function 
            // Then it throws
            Assert::ExpectException<std::exception>([from, to]() {OptimalTransformationParameters(from, to);});
        }

        TEST_METHOD(OptimalTransformationParameters_NonFeasibleInputThrows)
        {
            // Given non feasible input: twice the same point with different destinations
            std::vector<cv::Point2d> from;
            from.push_back(cv::Point2d(1.0, 0.0));
            from.push_back(cv::Point2d(1.0, 0.0));
            from.push_back(cv::Point2d(1.0, 0.0));

            std::vector<cv::Point2d> to;
            to.push_back(cv::Point2d(1.0, 0.0));
            to.push_back(cv::Point2d(-1.0, 0.0));
            to.push_back(cv::Point2d(-1.0, 0.0));

            // When calling function 
            // Then it throws
            Assert::ExpectException<std::exception>([from, to]() {OptimalTransformationParameters(from, to);});
        }

        TEST_METHOD(OptimalTransformationParameters_SimpleRotationFound)
        {
            // Given point on x-axis rotated to be on the y-axis with no other translation
            std::vector<cv::Point2d> from;
            from.push_back(cv::Point2d(0.0, 0.0));
            from.push_back(cv::Point2d(1.0, 0.0));
            from.push_back(cv::Point2d(2.0, 0.0));

            std::vector<cv::Point2d> to;
            to.push_back(cv::Point2d(0.0, 0.0));
            to.push_back(cv::Point2d(0.0, 1.0));
            to.push_back(cv::Point2d(0.0, 2.0));

            // When calling function 
            auto res = OptimalTransformationParameters(from, to);

            // Then transformation parameters are the proper ones
            Assert::AreEqual(0.0, res.Translation.x, s_precision, L"Unexpected x-translation");
            Assert::AreEqual(0.0, res.Translation.y, s_precision, L"Unexpected y-translation");
            Assert::AreEqual(1.0, res.Scale, s_precision, L"Unexpected scale");
            Assert::AreEqual(M_PI_2, res.RotationRad, s_precision, L"Unexpected rotation");
        }

        TEST_METHOD(OptimalTransformationParameters_SimpleTranslationFound)
        {
            // Given point on x-axis rotated only translated
            std::vector<cv::Point2d> from;
            from.push_back(cv::Point2d(0.0, 0.0));
            from.push_back(cv::Point2d(1.0, 0.0));
            from.push_back(cv::Point2d(2.0, 0.0));

            std::vector<cv::Point2d> to;
            to.push_back(cv::Point2d(0.5, 0.2));
            to.push_back(cv::Point2d(1.5, 0.2));
            to.push_back(cv::Point2d(2.5, 0.2));

            // When calling function 
            auto res = OptimalTransformationParameters(from, to);

            // Then transformation parameters are the proper ones
            Assert::AreEqual(0.5, res.Translation.x, s_precision, L"Unexpected x-translation");
            Assert::AreEqual(0.2, res.Translation.y, s_precision, L"Unexpected y-translation");
            Assert::AreEqual(1.0, res.Scale, s_precision, L"Unexpected scale");
            Assert::AreEqual(0.0, res.RotationRad, s_precision, L"Unexpected rotation");
        }

        TEST_METHOD(OptimalTransformationParameters_SimpleScaleFound)
        {
            // Given points scaled by factor 2.0
            std::vector<cv::Point2d> from;
            from.push_back(cv::Point2d(0.0, 0.0));
            from.push_back(cv::Point2d(1.0, 0.5));
            from.push_back(cv::Point2d(2.0, -0.5));

            std::vector<cv::Point2d> to;
            to.push_back(cv::Point2d(0.0, 0.0));
            to.push_back(cv::Point2d(2.0, 1.0));
            to.push_back(cv::Point2d(4.0, -1.0));

            // When calling function 
            auto res = OptimalTransformationParameters(from, to);

            // Then transformation parameters are the proper ones
            Assert::AreEqual(0.0, res.Translation.x, s_precision, L"Unexpected x-translation");
            Assert::AreEqual(0.0, res.Translation.y, s_precision, L"Unexpected y-translation");
            Assert::AreEqual(2.0, res.Scale, s_precision, L"Unexpected scale");
            Assert::AreEqual(0.0, res.RotationRad, s_precision, L"Unexpected rotation");
        }

        TEST_METHOD(OptimalTransformationParameters_CompoundRotationTranslationFound)
        {
            // Given point on x-axis rotated to be on the y-axis with a translation
            std::vector<cv::Point2d> from;
            from.push_back(cv::Point2d(0.0, 0.0));
            from.push_back(cv::Point2d(1.0, 0.0));
            from.push_back(cv::Point2d(2.0, 0.0));

            std::vector<cv::Point2d> to;
            to.push_back(cv::Point2d(0.5, 0.2));
            to.push_back(cv::Point2d(0.5, 1.2));
            to.push_back(cv::Point2d(0.5, 2.2));

            // When calling function 
            auto res = OptimalTransformationParameters(from, to);

            // Then transformation parameters are the proper ones
            Assert::AreEqual(0.5, res.Translation.x, s_precision, L"Unexpected x-translation");
            Assert::AreEqual(0.2, res.Translation.y, s_precision, L"Unexpected y-translation");
            Assert::AreEqual(1.0, res.Scale, s_precision, L"Unexpected scale");
            Assert::AreEqual(M_PI_2, res.RotationRad, s_precision, L"Unexpected rotation");
        }

        TEST_METHOD(OptimalTransformationParameters_CompoundRotationScaleTranslationFound)
        {
            // Given point on x-axis rotated to be on the y-axis a translation and scale
            std::vector<cv::Point2d> from;
            from.push_back(cv::Point2d(0.0, 0.0));
            from.push_back(cv::Point2d(1.0, 0.0));
            from.push_back(cv::Point2d(2.0, 0.0));

            std::vector<cv::Point2d> to;
            to.push_back(cv::Point2d(0.5, 0.2));
            to.push_back(cv::Point2d(0.5, 2.2));
            to.push_back(cv::Point2d(0.5, 4.2));

            // When calling function 
            auto res = OptimalTransformationParameters(from, to);

            // Then transformation parameters are the proper ones
            Assert::AreEqual(0.5, res.Translation.x, s_precision, L"Unexpected x-translation");
            Assert::AreEqual(0.2, res.Translation.y, s_precision, L"Unexpected y-translation");
            Assert::AreEqual(2.0, res.Scale, s_precision, L"Unexpected scale");
            Assert::AreEqual(M_PI_2, res.RotationRad, s_precision, L"Unexpected rotation");
        }
    };
}