#include "pch.h"
#include "CppUnitTest.h"

#include "CLutInterpolation.h"
#include <string>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(LutInterpolationTests)
    {
    private:
        static constexpr double _tolerance = 1e-6;

    public:
        TEST_METHOD(TestLutInterpolation)
        {
            CLutInterpolation LinInterp;
            int NbPoints = 16;
            //char buf[255];

            auto x = new double[NbPoints];
            auto y = new double[NbPoints];

            x[0] = -2.5;
            y[0] = x[0] * x[0] * x[0];
            for (int i = 1; i < NbPoints; i++)
            {
                x[i] = x[i - 1] + 0.5;
                y[i] = x[i] * x[i] * x[i];
            }

            LinInterp.CopyArraysFrom(NbPoints, x, y);

            double xExpect[6]{ -3.0, -1.25, 1.2, 3.65, 5.5 };
            double yExpect[6]{ -15.625, -2.1875, 1.95, 49.2125, 125 }; // value outside array are bounded to ymin and ymax
            double yVal = 0.0; double xVal = 0.0; double detlaTolerance = _tolerance;
            //Logger::WriteMessage("---- Search Y from x values\n");
            for (int i = 0; i < 6; i++)
            {
                yVal = LinInterp.Y(xExpect[i]);
                Assert::AreEqual(yExpect[i], yVal, detlaTolerance);

            }
            //Logger::WriteMessage("---- Search X from y values\n");
            xExpect[0] = -2.5; xExpect[4] = 5.0; // value outside array are bounded to xmin and xmax
            for (int i = 0; i < 6; i++)
            {
                xVal = LinInterp.X(yExpect[i]);
                Assert::AreEqual(xExpect[i], xVal, detlaTolerance);
            }

            //Logger::WriteMessage("---- Copy Constructor\n");
            CLutInterpolation LinInterp2 = LinInterp;
            xVal = 4.2;
            double expected = LinInterp.Y(xVal);
            Assert::AreEqual(expected, LinInterp2.Y(xVal), detlaTolerance);

            //Logger::WriteMessage("---- Delete arrays\n");

            delete[] x;
            delete[] y;

            //sprintf_s(buf, "Done\n");
            //Logger::WriteMessage(buf);
            Assert::AreEqual(0, 0); // needed to avoid test host to randomly crashed and abort
        }
    };
}