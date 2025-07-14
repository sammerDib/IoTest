#include "pch.h"
#include "CppUnitTest.h"

#include "CInterpolatorIDW.h"
#include <string>
using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(InterpolationIDWTests)
    {
    private:
        static constexpr double _tolerance = 1e-5;

    public:
        TEST_METHOD(TestInterpolationIDW)
        {
            CInterpolatorIDW idw;
            double settings[] = { 3.0 };

            //Logger::WriteMessage("---- InitSettings\n");
            Assert::IsTrue(idw.InitSettings(1, settings));

            double cooX[] = {
                   0.0, 5.0,  10.0, 8.0, 9.0,
                   7.0, 8.5,  5.5,  7.5, 4.2,
                   2.5, 2.0,  4.5,  6.5, 9.0,
                   8.0, 10.0, 6.0,  3.0, 1.5,
                   4.5, 1.0,  0.0,  1.0, 3.0
            };

            double cooY[] = {
                   0.0, 0.5 , 0.0 , 1.0, 2.0,
                   2.5, 4.5 , 3.0 , 5.0, 4.1,
                   2.5, 4.7 , 6.3 , 8.5, 6.0,
                   8.0, 10.0, 6.0 , 8.0, 6.5,
                   9.5, 3.0 , 10.0, 8.0, 9.7
            };

            double valZ[] = {
                2.0, -1.0, 1.0, 0.0, 0.5,
                3.0, 4.0, -0.5, 0.0, 8.0,
                5.0, 2.0, 2.5, 1.5, 1.5,
                0.0, -1.0, 4.0, 6.0, 4.0,
                3.5, 1.0, 0.0, 1.0, 0.5,
            };
            //Logger::WriteMessage("---- SetInputsPoints\n");
            Assert::IsTrue(idw.SetInputsPoints(25, cooX, cooY, valZ));
            //Logger::WriteMessage("---- ComputeData\n");
            Assert::IsTrue(idw.ComputeData());

            double maxerror = 0.0;
            double error = 0.0;
            for (int i = 0; i < 25; i++)
            {
                double interp = idw.Interpolate(cooX[i], cooY[i]);
                maxerror = std::max(maxerror, std::abs(valZ[i] - interp));
                error = std::max(error, std::abs(valZ[i]));
            }
            error = maxerror / error;
            //char buf[255];
           // sprintf_s(buf, "all Error = %lf \n", error);
            //Logger::WriteMessage(buf);
            Assert::IsTrue(error <= 1e-8);

            //sprintf_s(buf, "[5,5] = %lf \n", idw.Interpolate(5.0, 5.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(4.315357, idw.Interpolate(5.0, 5.0), _tolerance);

            //sprintf_s(buf, "[9,2] = %lf \n", idw.Interpolate(9.0, 2.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(0.5, idw.Interpolate(9.0, 2.0), _tolerance);

            //sprintf_s(buf, "Ext [3, 0.25] = %lf \n", idw.Interpolate(3.0, 0.25));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(1.717792506, idw.Interpolate(3.0, 0.25), _tolerance);

            //sprintf_s(buf, "Ext [-0.1, -1.0] = %lf \n", idw.Interpolate(-0.1, -1.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(2.016293182, idw.Interpolate(-0.1, -1.0), _tolerance);

            //sprintf_s(buf, "Done\n");
            //Logger::WriteMessage(buf);
            Assert::AreEqual(0, 0);

        }
    };
}

