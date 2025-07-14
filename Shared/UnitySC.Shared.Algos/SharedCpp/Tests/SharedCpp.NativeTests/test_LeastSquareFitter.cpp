#include "pch.h"
#include "CppUnitTest.h"

#include "LeastSquareFitter.h"
#include <vector>
#include <string>
#include <limits>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace least_square_fitter;
using namespace geometry;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(LeastSquareFitterTests)
    {
    private:
        static constexpr double _tolerance = 1e-6;

        void AssertPointsEqual(const Point3d& expected, const Point3d& actual) {
            Assert::AreEqual(expected.X, actual.X, _tolerance, L"X coordinates are different");
            Assert::AreEqual(expected.Y, actual.Y, _tolerance, L"Y coordinates are different");
            Assert::AreEqual(expected.Z, actual.Z, _tolerance, L"Z coordinates are different");
        }

    public:

        BEGIN_TEST_METHOD_ATTRIBUTE(TooFewPointsThrowsError)
        //    TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(TooFewPointsThrowsError)
        {
            // Given only 2 points
            std::vector<Point3d> points;
            points.push_back(Point3d(10.83, 11.12, 12.398));
            points.push_back(Point3d(9, 8, 13));

            auto func = [&points] { FitLeastSquarePlane(points); };
            Assert::ExpectException<std::exception>(func, L"FitLeastSquarePlane has not thrown any exception - expected Impossible to compute least square plane: to few points (<3)");          
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(InfinitePointThrowsError)
        //    TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(InfinitePointThrowsError)
        {
            // Given an infinite coordinate among points
            std::vector<Point3d> points;
            points.push_back(Point3d(10.83, 11.12, 12.398));
            points.push_back(Point3d(9, 8, 13));
            points.push_back(Point3d(9, 8, std::numeric_limits<double>::infinity()));

            auto func = [&points] { FitLeastSquarePlane(points); };
            Assert::ExpectException<std::exception>(func, L"FitLeastSquarePlane has not thrown any exception - Impossible to compute least square plane: infinite/NaN mean of points");
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(NaNPointThrowsError)
        //    TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(NaNPointThrowsError)
        {
            // Given an infinite coordinate among points
            std::vector<Point3d> points;
            points.push_back(Point3d(10.83, 11.12, 12.398));
            points.push_back(Point3d(9, 8, 13));
            points.push_back(Point3d(9, 8, std::numeric_limits<double>::quiet_NaN()));

            auto func = [&points] { FitLeastSquarePlane(points); };
            Assert::ExpectException<std::exception>(func, L"FitLeastSquarePlane has not thrown any exception - Impossible to compute least square plane: infinite/NaN mean of points");
        }

        BEGIN_TEST_METHOD_ATTRIBUTE(ColinearPointsThrowsError)
        //    TEST_IGNORE()
        END_TEST_METHOD_ATTRIBUTE()
        TEST_METHOD(ColinearPointsThrowsError)
        {
            // Given 3 colinear points
            std::vector<Point3d> points;
            points.push_back(Point3d(1, 2, 3));
            points.push_back(Point3d(2, 3, 4));
            points.push_back(Point3d(4, 5, 6));

            auto func = [&points] { FitLeastSquarePlane(points); };
            Assert::ExpectException<std::exception>(func, L"FitLeastSquarePlane has not thrown any exception - Impossible to compute least square plane: 0 determinant.");
        }

        TEST_METHOD(BasicHorizontalPlaneIsFound)
        {
            // Given 3 points representing the horizontal plane
            std::vector<Point3d> points;
            points.push_back(Point3d(0, 0, 0));
            points.push_back(Point3d(0, 1, 0));
            points.push_back(Point3d(1, 0, 0));

            // When fitting the plane
            Plane plane = FitLeastSquarePlane(points);

            // Then plane is properly found
            AssertPointsEqual(Point3d(1.0 / 3, 1.0 / 3, 0.0), plane.Center);
            AssertPointsEqual(Point3d(0.0, 0.0, -1.0), plane.Normal);
        }

        TEST_METHOD(BasicTiltedPlaneIsFound)
        {
            // Given 3 points representing a tilted plane
            std::vector<Point3d> points;
            points.push_back(Point3d(0, 0, 1));
            points.push_back(Point3d(0, 1, 0));
            points.push_back(Point3d(1, 0, 0));

            // When fitting the plane
            Plane plane = FitLeastSquarePlane(points);

            // Then plane is properly found
            AssertPointsEqual(Point3d(1.0 / 3, 1.0 / 3, 1.0 / 3), plane.Center);
            AssertPointsEqual(Point3d(-1.0, -1.0, -1.0), plane.Normal);
        }

        TEST_METHOD(ComplexHorizontalPlaneIsFound)
        {
            // Given many points representing whose fitting plane is a tilted plane
            std::vector<Point3d> points;
            points.push_back(Point3d(1, 1, 1));
            points.push_back(Point3d(1, 1, -1));
            points.push_back(Point3d(-1, 1, 2));
            points.push_back(Point3d(-1, 1, -2));
            points.push_back(Point3d(-1, -1, 3));
            points.push_back(Point3d(-1, -1, -3));
            points.push_back(Point3d(1, -1, 4));
            points.push_back(Point3d(1, -1, -4));

            // When fitting the plane
            Plane plane = FitLeastSquarePlane(points);

            // Then plane is properly found
            AssertPointsEqual(Point3d(0.0, 0.0, 0.0), plane.Center);
            AssertPointsEqual(Point3d(0.0, 0.0, -1.0), plane.Normal);
        }
    };
}