#include "pch.h"
#include "CppUnitTest.h"

#include "CInterpolatorFNN.h"
#include <string>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(InterpolationFNNTests)
    {
    private:
        static constexpr double _tolerance = 1e-6;

    public:
        TEST_METHOD(TestInterpolationFNN_value1d)
        {
            CInterpolatorFNN fnn;

            //Logger::WriteMessage("---- InitSettings\n");
            Assert::IsTrue(fnn.InitSettings(1, nullptr));

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
            Assert::IsTrue(fnn.SetInputsPoints(25, cooX, cooY, valZ));
            //Logger::WriteMessage("---- ComputeData\n");
            Assert::IsTrue(fnn.ComputeData());

            double maxerror = 0.0;
            double error = 0.0;
            for (int i = 0; i < 25; i++)
            {
                double interp = fnn.Interpolate(cooX[i], cooY[i]);
                maxerror = std::max(maxerror, std::abs(valZ[i] - interp));
                error = std::max(error, std::abs(valZ[i]));
            }
            error = maxerror / error;
            //char buf[255];
            //sprintf_s(buf, "all Error = %lf \n", error);
            //Logger::WriteMessage(buf);
            Assert::IsTrue(error <= 1e-15);

            //sprintf_s(buf, "[5,5] = %lf \n", fnn.Interpolate(5.0, 5.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(8.0, fnn.Interpolate(5.0, 5.0));

            //sprintf_s(buf, "[4.00,3.25] = %lf \n", fnn.Interpolate(4.00, 3.25));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(8.0, fnn.Interpolate(4.0, 3.25));

            //sprintf_s(buf, "[3.00,0.25] = %lf \n", fnn.Interpolate(3.00, 0.25));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(-1.0, fnn.Interpolate(3.00, 0.25));

            //sprintf_s(buf, "[-0.05, -1.0] = %lf \n", fnn.Interpolate(-0.05, -1.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(2.0, fnn.Interpolate(-0.05, -1.0));

            //sprintf_s(buf, "Done\n");
            //Logger::WriteMessage(buf);
            Assert::AreEqual(0, 0);
        }

        TEST_METHOD(TestInterpolationFNN_value2d)
        {
            CInterpolatorFNN fnn;

            //Logger::WriteMessage("---- InitSettings\n");
            Assert::IsTrue(fnn.InitSettings(1, nullptr));

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

            double valZ2[25];
            for (int i = 0; i < 25; i++)
            {
                valZ2[i] = 10.0 * valZ[i];
            }

            //Logger::WriteMessage("---- SetInputsPoints\n");
            Assert::IsTrue(fnn.SetInputsPoints(25, cooX, cooY, valZ, valZ2));
            //Logger::WriteMessage("---- ComputeData\n");
            Assert::IsTrue(fnn.ComputeData());

            double maxerror = 0.0;
            double error = 0.0;
            for (int i = 0; i < 25; i++)
            {
                double interp = fnn.Interpolate(cooX[i], cooY[i]);
                maxerror = std::max(maxerror, std::abs(valZ[i] - interp));
                error = std::max(error, std::abs(valZ[i]));
            }
            error = maxerror / error;
            //char buf[255];
            //sprintf_s(buf, "all Error = %lf \n", error);
            //Logger::WriteMessage(buf);
            Assert::IsTrue(error <= 1e-15);

            double v1, v2;
            for (int i = 0; i < 25; i++)
            {
                fnn.Interpolate(cooX[i], cooY[i], v1, v2);
                double e1 = v1 - valZ[i];
                double e2 = v2 - valZ2[i];
                Assert::AreEqual(0.0, e1);
                Assert::AreEqual(0.0, e2);
                error += e2;
            }

            //sprintf_s(buf, "all cumulate Error v2 = %lf \n", error);
            //Logger::WriteMessage(buf);
            Assert::IsTrue(error <= 1e-15);

            fnn.Interpolate(5.0, 5.0, v1, v2);
            //sprintf_s(buf, "[5,5] =  %lf | %lf  \n", v1, v2);
            //Logger::WriteMessage(buf);
            Assert::AreEqual(8.0, v1);
            Assert::AreEqual(80.0, v2);

            fnn.Interpolate(4.00, 3.25, v1, v2);
            //sprintf_s(buf, "[4.00,3.25] =  %lf | %lf  \n", v1, v2);
            //Logger::WriteMessage(buf);
            Assert::AreEqual(8.0, v1);
            Assert::AreEqual(80.0, v2);

            fnn.Interpolate(3.00, 0.25, v1, v2);
            //sprintf_s(buf, "[3.00,0.25] = %lf | %lf  \n", v1, v2);
            //Logger::WriteMessage(buf);
            Assert::AreEqual(-1.0, v1);
            Assert::AreEqual(-10.0, v2);

            fnn.Interpolate(-0.05, -1.0, v1, v2);
            //sprintf_s(buf, "[-0.05, -1.0] = %lf | %lf  \n", v1, v2);
            //Logger::WriteMessage(buf);
            Assert::AreEqual(2.0, v1);
            Assert::AreEqual(20.0, v2);

            //sprintf_s(buf, "Done\n");
            //Logger::WriteMessage(buf);
            Assert::AreEqual(0, 0);
        }
    };
}