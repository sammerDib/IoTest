#include "pch.h"
#include "CppUnitTest.h"

#include "CInterpolatorMBA.h"
#include <string>
#include <omp.h>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace SharedCpp_NativeTests
{
    TEST_CLASS(InterpolationMBATests)
    {
    private:
        static constexpr double _tolerance = 1e-6;

    public: 
        TEST_CLASS_INITIALIZE(DeactivateOpenMP)
        {
            omp_set_num_threads(1);
        }

        TEST_METHOD(TestInterpolationMBA)
        {
            CInterpolatorMBA mba;
            double settings[] = { -0.1,-0.1,10.1,10.1,2.0,2.0 };

            //Logger::WriteMessage("---- InitSettings\n");
            Assert::IsTrue(mba.InitSettings(6, settings));

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
            Assert::IsTrue(mba.SetInputsPoints(25, cooX, cooY, valZ));
            //Logger::WriteMessage("---- ComputeData\n");
            Assert::IsTrue(mba.ComputeData());

            double maxerror = 0.0;
            double error = 0.0;
            for (int i = 0; i < 25; i++)
            {
                double interp = mba.Interpolate(cooX[i], cooY[i]);
                maxerror = std::max(maxerror, std::abs(valZ[i] - interp));
                error = std::max(error, std::abs(valZ[i]));
            }
            error = maxerror / error;
            //char buf[255];
            //sprintf_s(buf, "all Error = %lf \n", error);
            //Logger::WriteMessage(buf);
            Assert::IsTrue(error <= 1e-8);

            //sprintf_s(buf, "[5,5] = %lf \n", mba.Interpolate(5.0, 5.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(5.033138, mba.Interpolate(5.0, 5.0), 0.0005);

            //sprintf_s(buf, "Done\n");
            //Logger::WriteMessage(buf);
        }

        TEST_METHOD(TestInterpolationMBAWithInitialisation)
        {
            CInterpolatorMBA mba;
            double settings[] = { -0.1,-0.1, 10.1, 10.1, 2.0, 2.0, 3.95 };

            //Logger::WriteMessage("---- InitSettings\n");
            Assert::IsTrue(mba.InitSettings(7, settings));

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
            Assert::IsTrue(mba.SetInputsPoints(25, cooX, cooY, valZ));
            //Logger::WriteMessage("---- ComputeData\n");
            Assert::IsTrue(mba.ComputeData());

            double maxerror = 0.0;
            double error = 0.0;
            for (int i = 0; i < 25; i++)
            {
                double interp = mba.Interpolate(cooX[i], cooY[i]);
                maxerror = std::max(maxerror, std::abs(valZ[i] - interp));
                error = std::max(error, std::abs(valZ[i]));
            }
            error = maxerror / error;
            //char buf[255];
            //sprintf_s(buf, "all Error = %lf \n", error);
            //Logger::WriteMessage(buf);
            Assert::IsTrue(error <= 1e-8);

            //sprintf_s(buf, "[5,5] = %lf \n", mba.Interpolate(5.0, 5.0));
            //Logger::WriteMessage(buf);
            Assert::AreEqual(5.025328, mba.Interpolate(5.0, 5.0), 0.0005);

            //sprintf_s(buf, "Done\n");
            //Logger::WriteMessage(buf);
        }
    };
}