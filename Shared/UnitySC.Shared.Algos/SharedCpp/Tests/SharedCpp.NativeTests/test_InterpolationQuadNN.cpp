#include "pch.h"
#include "CppUnitTest.h"

#include "CInterpolatorQuadNN.h"
#include <string>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(InterpolationQuadNNTests)
    {
    private:
        static constexpr double _tolerance = 1e-5;

    public:
        TEST_METHOD(TestInterpolationQuadNN_value1d)
        {
            CInterpolatorQuadNN qnn;

            //Logger::WriteMessage("---- InitSettings\n"); 
            double settings[4] = { 0.0, 0.0, 0.0, 5.0 };
            Assert::IsTrue(qnn.InitSettings(4, settings));

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
            Assert::IsTrue(qnn.SetInputsPoints(25, cooX, cooY, valZ));
            //Logger::WriteMessage("---- ComputeData\n");
            Assert::IsTrue(qnn.ComputeData());

            double maxerror = 0.0;
            double error = 0.0;
            for (int i = 0; i < 25; i++)
            {
                double interp = qnn.Interpolate(cooX[i], cooY[i]);
                maxerror = std::max(maxerror, std::abs(valZ[i] - interp));
                error = std::max(error, std::abs(valZ[i]));
            }
            error = maxerror / error;
            //char buf[255];
            //sprintf_s(buf, "all Error = %lf \n", error);
            //Logger::WriteMessage(buf);
            Assert::IsTrue(error <= 1e-15);

            //sprintf_s(buf, "[5,5] = %lf \n", qnn.Interpolate(5.0, 5.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(4.849343059, qnn.Interpolate(5.0, 5.0), _tolerance);

            //sprintf_s(buf, "[4.00,3.25] = %lf \n", qnn.Interpolate(4.00, 3.25));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(8.0, qnn.Interpolate(4.0, 3.25), _tolerance);

            //sprintf_s(buf, "Extrapolation [3.00,0.25] = %lf \n", qnn.Interpolate(3.00, 0.25));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(-1.0, qnn.Interpolate(3.00, 0.25), _tolerance);

            //sprintf_s(buf, "Extrapolation [-0.05, -1.0] = %lf \n", qnn.Interpolate(-0.05, -1.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(2.0, qnn.Interpolate(-0.05, -1.0), _tolerance);

            //sprintf_s(buf, "Done\n");
            //Logger::WriteMessage(buf);
            Assert::AreEqual(0, 0);

        }

        TEST_METHOD(TestInterpolationQuadNN_value2d)
        {
            CInterpolatorQuadNN qnn;

            //Logger::WriteMessage("---- InitSettings\n");
            double settings[4] = { 0.0, 0.0, 0.0, 5.0 };
            Assert::IsTrue(qnn.InitSettings(4, settings));

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
            Assert::IsTrue(qnn.SetInputsPoints(25, cooX, cooY, valZ, valZ2));
            //Logger::WriteMessage("---- ComputeData\n");
            Assert::IsTrue(qnn.ComputeData());

            double maxerror = 0.0;
            double error = 0.0;
            for (int i = 0; i < 25; i++)
            {
                double interp = qnn.Interpolate(cooX[i], cooY[i]);
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
                qnn.Interpolate(cooX[i], cooY[i], v1, v2);
                double e1 = v1 - valZ[i];
                double e2 = v2 - valZ2[i];
                Assert::AreEqual(0.0, e1);
                Assert::AreEqual(0.0, e2);
                error += e2;
            }

            //sprintf_s(buf, "all cumulate Error v2 = %lf \n", error);
            //Logger::WriteMessage(buf);
            Assert::IsTrue(error <= 1e-15);

            qnn.Interpolate(5.0, 5.0, v1, v2);
            //sprintf_s(buf, "[5,5] =  %lf | %lf  \n", v1, v2);
            //Logger::WriteMessage(buf);
            Assert::AreEqual(4.849343059, v1, _tolerance);
            Assert::AreEqual(10.0*v1, v2, _tolerance);

            qnn.Interpolate(4.00, 3.25, v1, v2);
            //sprintf_s(buf, "[4.00,3.25] =  %lf | %lf  \n", v1, v2);
            //Logger::WriteMessage(buf);
            Assert::AreEqual(8.0, v1, _tolerance);
            Assert::AreEqual(10.0 * v1, v2, _tolerance);

            qnn.Interpolate(3.00, 0.25, v1, v2);
            //sprintf_s(buf, "Extrapolation [3.00,0.25] = %lf | %lf  \n", v1, v2);
            //Logger::WriteMessage(buf);
            Assert::AreEqual(-1.0, v1, _tolerance);
            Assert::AreEqual(10.0 * v1, v2, _tolerance);

            qnn.Interpolate(-0.05, -1.0, v1, v2);
            //sprintf_s(buf, "Extrapolation [-0.05, -1.0] = %lf | %lf  \n", v1, v2);
            //Logger::WriteMessage(buf);
            Assert::AreEqual(2.0, v1, _tolerance);
            Assert::AreEqual(10.0 * v1, v2, _tolerance);

            //sprintf_s(buf, "Done\n");
            //Logger::WriteMessage(buf);
            Assert::AreEqual(0, 0);
        }
    };
}