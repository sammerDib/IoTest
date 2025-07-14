#include "pch.h"
#include "CppUnitTest.h"

#include "Profile.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(ProfileTests)
    {
    private:
        static constexpr double _tolerance = 1e-4;

    public:
        TEST_METHOD(Profile_Iterator)
        {
            std::vector<geometry::Point2d> points(5);
            Profile profile(points);

            std::iota(profile.begin_x(), profile.end_x(), -2);
            std::iota(profile.begin_y(), profile.end_y(), 50);

            Assert::AreEqual(points.size(), profile.Size());
            Assert::AreEqual(-2.0, profile.GetPoints()[0].X);
            Assert::AreEqual(-1.0, profile.GetPoints()[1].X);
            Assert::AreEqual(0.0, profile.GetPoints()[2].X);
            Assert::AreEqual(1.0, profile.GetPoints()[3].X);
            Assert::AreEqual(2.0, profile.GetPoints()[4].X);

            Assert::AreEqual(50.0, profile.GetPoints()[0].Y);
            Assert::AreEqual(51.0, profile.GetPoints()[1].Y);
            Assert::AreEqual(52.0, profile.GetPoints()[2].Y);
            Assert::AreEqual(53.0, profile.GetPoints()[3].Y);
            Assert::AreEqual(54.0, profile.GetPoints()[4].Y);

            auto it = profile.begin_x();
            Assert::AreEqual(1.0, *(it + 3));

            const auto size = std::distance(profile.begin_y(), profile.end_y());
            Assert::AreEqual(static_cast<int>(points.size()), static_cast<int>(size));
        }

        TEST_METHOD(Profile_ConstIterator)
        {
            const std::vector<geometry::Point2d> points{ {0.0, 1}, {0.1, 2}, {0.2, 3}, {0.3, 4}, {0.4, 5} };
            const Profile profile(points);

            std::vector<double> x, y;
            for (auto it = profile.begin_x(); it != profile.end_x(); ++it)
            {
                x.push_back(*it);
            }
            for (auto it = profile.begin_y(); it != profile.end_y(); ++it)
            {
                y.push_back(*it);
            }

            Assert::AreEqual(points.size(), x.size());
            Assert::AreEqual(0.0, x[0], _tolerance);
            Assert::AreEqual(0.1, x[1], _tolerance);
            Assert::AreEqual(0.2, x[2], _tolerance);
            Assert::AreEqual(0.3, x[3], _tolerance);
            Assert::AreEqual(0.4, x[4], _tolerance);

            Assert::AreEqual(points.size(), y.size());
            Assert::AreEqual(1, y[0], _tolerance);
            Assert::AreEqual(2, y[1], _tolerance);
            Assert::AreEqual(3, y[2], _tolerance);
            Assert::AreEqual(4, y[3], _tolerance);
            Assert::AreEqual(5, y[4], _tolerance);

            const auto it = profile.begin_x();
            Assert::AreEqual(0.3, *(it + 3));

            const auto size = std::distance(profile.begin_y(), profile.end_y());
            Assert::AreEqual(static_cast<int>(points.size()), static_cast<int>(size));
        }

        TEST_METHOD(Profile_Reverse)
        {
            Profile profile({ {0.0, 1}, {0.1, 2}, {0.2, 3}, {0.3, 4}, {0.4, 5} });

            profile.Reverse();

            auto xIt = profile.begin_x();
            auto yIt = profile.begin_y();
            Assert::AreEqual(0.0, *xIt);
            Assert::AreEqual(5.0, *yIt);

            Assert::AreEqual(0.1, *(++xIt));
            Assert::AreEqual(4.0, *(++yIt));

            Assert::AreEqual(0.2, *(++xIt));
            Assert::AreEqual(3.0, *(++yIt));

            Assert::AreEqual(0.3, *(++xIt));
            Assert::AreEqual(2.0, *(++yIt));

            Assert::AreEqual(0.4, *(++xIt));
            Assert::AreEqual(1.0, *(++yIt));
        }
    };
}