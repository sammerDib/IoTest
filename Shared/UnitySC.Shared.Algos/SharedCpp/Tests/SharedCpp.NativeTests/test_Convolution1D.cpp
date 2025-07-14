#include "pch.h"
#include "CppUnitTest.h"

#include "Convolution1D.h"
#include "Profile.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(ConvolutionTests)
    {
    public:
        TEST_METHOD(TestFullConvolution)
        {
            std::vector<double> f{ 1.0, 2.0, 3.0, 4.0, 5.0 };
            std::vector<double> g{ 0.0, 1.0, 0.5, 0.0 };

            const auto r(FullConvolution1D(f, g));

            Assert::AreEqual(f.size() + g.size() - 1, r.size());
            Assert::AreEqual(0.0, r[0]);
            Assert::AreEqual(1.0, r[1]);
            Assert::AreEqual(2.5, r[2]);
            Assert::AreEqual(4.0, r[3]);
            Assert::AreEqual(5.5, r[4]);
            Assert::AreEqual(7.0, r[5]);
            Assert::AreEqual(2.5, r[6]);
            Assert::AreEqual(0.0, r[7]);
        }

        TEST_METHOD(TestValidConvolution)
        {
            std::vector<double> f{ 1.0, 2.0, 3.0, 4.0, 5.0 };
            std::vector<double> g{ 0.0, 1.0, 0.5, 0.0 };

            const auto r(ValidConvolution1D(f, g));

            Assert::AreEqual(std::max(f.size(), g.size()) - std::min(f.size(), g.size()) + 1, r.size());
            Assert::AreEqual(4.0, r[0]);
            Assert::AreEqual(5.5, r[1]);
        }

        TEST_METHOD(TestValidConvolutionIterators)
        {
            std::vector<geometry::Point2d> points{ {0.0, 1.0}, {0.1, 2.0}, {0.2, 3.0}, {0.3, 4.0}, {0.4, 5.0} };
            Profile f(points);
            std::vector<double> kernel{ 0.0, 1.0, 0.5, 0.0 };

            const auto r(ValidConvolution1D(f.begin_y(), f.end_y(), kernel.begin(), kernel.end()));

            Assert::AreEqual(std::max(f.Size(), kernel.size()) - std::min(f.Size(), kernel.size()) + 1, r.size());
            Assert::AreEqual(4.0, r[0]);
            Assert::AreEqual(5.5, r[1]);
        }
    };
}