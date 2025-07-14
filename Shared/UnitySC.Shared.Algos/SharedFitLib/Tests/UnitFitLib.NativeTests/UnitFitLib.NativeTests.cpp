#include "CppUnitTest.h"

#include "lmfit.hpp"
#include <iostream>
#include <vector>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;
using namespace lmfit;

#define MPI 3.1415926535897932384626433832795

//#define USELIB_CONTROL_STRUCT 1
#ifdef USELIB_CONTROL_STRUCT
const lm_control_struct _lm_control_UTEST = lm_control_double;
#else
#define LM_USERTOL    30*DBL_EPSILON 
const lm_control_struct _lm_control_UTEST = {LM_USERTOL, LM_USERTOL, LM_USERTOL, LM_USERTOL, 100., 200, 1, NULL, 0, -1, -1 };
#endif // USELIB_CONTROL_STRUCT


namespace UnitFitLibNativeTests
{

    double gaussProbabilityDensityFunction(const double t, const double* p)
    {
        // p : 0 , 1 , 2 
        //     A , mu , sigma

        return (p[0] / p[2] * sqrtl(2.0 * MPI)) * exp(-(t - p[1]) * (t - p[1]) / (2.0 * p[2] * p[2]));
    }

    double PolyDegree2Fct(const double t, const double* p)
    {
        // y = p0 + p1 * x + p2 * x^2
        return p[0] + p[1] * t + p[2] * t * t;
    }

    void evaluate_nonlin1(const double* p, int n, const void* data, double* f, int* info)
    {
        f[0] = p[0] * p[0] + p[1] * p[1] - 1; // unit circle    x^2+y^2=1
        f[1] = p[1] - p[0] * p[0];            // standard parabola  y=x^2
    }

    void LogUnitTestEvalGauss(lmfit::result_t& result, std::vector<double>& t, std::vector<double>& y, std::vector<std::string>& paramlabel, double (*gFct)(const double t, const double* par), double dstep = 0.1)
    {
        char buf[255];

        Logger::WriteMessage("Results:\n");
        _snprintf_s(buf, 255, "status after [%i] function evalution \n=> %s\n", result.status.nfev, lm_infmsg[result.status.outcome]);
        Logger::WriteMessage(buf);

        Logger::WriteMessage("\nResults Params:\n");
        for (int i = 0; i < (int)paramlabel.size(); i++)
        {
            _snprintf_s(buf, 255, "%s\t\t: p%i  = %lf\n", paramlabel[i].c_str(), i, result.par[i]);
            Logger::WriteMessage(buf);
        }

        _snprintf_s(buf, 255, "\nObtained norm : %lf\n", result.status.fnorm);
        Logger::WriteMessage(buf);

        double parResult[3]{ result.par[0], result.par[1], result.par[2] };
        Logger::WriteMessage("--------------\n");
        Logger::WriteMessage("xIn;yIn;yFct;evalX;evalFctY;");
        for (int i = 0; i < (int)paramlabel.size(); i++)
        {
            _snprintf_s(buf, 255, "%s;%lf;", paramlabel[i].c_str(), result.par[i]);
            Logger::WriteMessage(buf);
        }
        Logger::WriteMessage("\n");

        int nbt = (int)t.size() - 1;
        double dXEvalStart = t[0] - 1.0;
        double dXEvalEnd = t[nbt] + 1.0;

        int MaxIt = (int)((dXEvalEnd - dXEvalStart) / dstep);

        double xEval = dXEvalStart;
        for (int i = 0; i < MaxIt; ++i)
        {
            if (i <= nbt)
            {
                _snprintf_s(buf, 255, "%lf;%lf;%lf;%lf;%lf\n", t[i], y[i], gFct(t[i], parResult), xEval, gFct(xEval, parResult));
            }
            else
            {
                _snprintf_s(buf, 255, ";;;%lf;%lf\n", xEval, gFct(xEval, parResult));
            }

            Logger::WriteMessage(buf);

            xEval += dstep;
        }
    }

	TEST_CLASS(UnitFitLibNativeTests)
	{
	public:
        TEST_METHOD(_01_FitGaussPeakR)
        {
#ifdef DEBUG            
            Logger::WriteMessage("_01_FitGaussPeakR\n");
#endif // DEBUG
            std::vector<std::string> parLbl{ "A   ", "Mu   ", "sigma"};
            std::vector<double> par0{ 5.0, 194.0, 10.0 };

            std::vector<double> t;
            for (double d = 188.0; d <= 200.0; d += 1.0)
            {
                t.push_back(d);
            }
            std::vector<double> y{
                0.283333,
                0.966667,
                -1.38333,
                -1.31667,
                0.95,
                219.483,
                220.9,
                2.48333,
                -1.88333,
                -1.46667,
                1.3,
                0.4,
                0.266667 };

            Assert::AreEqual((int)t.size(), (int)y.size(), L"BAD inputs size");

            lm_control_struct control = _lm_control_UTEST;
            control.verbosity = 0;
#ifdef DEBUG  
            Logger::WriteMessage("Fitting...\n");
#endif // DEBUG
            auto result = lmfit::fit_curve(par0, t, y, &gaussProbabilityDensityFunction, control);
#ifdef DEBUG
            LogUnitTestEvalGauss(result, t, y, parLbl, &gaussProbabilityDensityFunction, 0.1);
#endif // DEBUG
            Assert::IsFalse(result.status.outcome > 5);
            Assert::IsTrue(result.status.fnorm <= 10.0);
            Assert::AreEqual(193.5, result.par[1], 0.05);
            Assert::AreEqual(0.5, result.par[2], 0.3);
        }
        TEST_METHOD(_02_FitGaussPeakL)
        {
#ifdef DEBUG  
            Logger::WriteMessage("_02_FitGaussPeakL\n");
#endif // DEBUG
            std::vector<std::string> parLbl{ "A   ", "Mu   ", "sigma" };
            std::vector<double> par0{ 5.0, 193.0, 10.0 };

            std::vector<double> t;
            for (double d = 188.0; d <= 200.0; d += 1.0)
            {
                t.push_back(d);
            }
            std::vector<double> y{
                2.1,
                -0.65,
                -1.06667,
                -0.35,
                -0.916667,
                220.65,
                220.6,
                -1.38333,
                -1.66667,
                1.25,
                1.01667,
                0.533333,
                -0.233333
            };

            lm_control_struct control = _lm_control_UTEST;//lm_control_double;
            control.verbosity = 0;
#ifdef DEBUG 
            Logger::WriteMessage("Fitting...\n");
#endif // DEBUG
            auto result = lmfit::fit_curve(par0, t, y, &gaussProbabilityDensityFunction, control);

#ifdef DEBUG
            LogUnitTestEvalGauss(result, t, y, parLbl, &gaussProbabilityDensityFunction, 0.1);
#endif // DEBUG
            Assert::IsFalse(result.status.outcome > 5);
            Assert::IsTrue(result.status.fnorm <= 10.0);
            Assert::AreEqual(193.5, result.par[1], 0.05);
            Assert::AreEqual(0.5, result.par[2], 0.30);
        }
        TEST_METHOD(_03_FitGaussPeakRampNoiseL)
        {
#ifdef DEBUG  
            Logger::WriteMessage("FitGaussPeakRampNoiseL\n");
#endif // DEBUG
            std::vector<std::string> parLbl{ "A   ", "Mu   ", "sigma" };
            std::vector<double> par0{ 5.0, 187.0, 10.0 };

            std::vector<double> t;
            for (double d = 175.0; d <= 210.0; d += 1.0)
            {
                t.push_back(d);
            }
            std::vector<double> y{
                -1.4,
                1.06667,
                -0.716667,
                -0.25,
                1.13333,
                1.51667,
                -0.733333,
                -1.85,
                14.6,
                21.9333,
                18.8167,
                22.15,
                24.8667,
                23.15,
                23.5667,
                23.35,
                23.3333,
                23.5833,
                23.0667,
                24.85,
                22.5,
                22.1833,
                21.8,
                23.4667,
                23.6833,
                22.7333,
                22.4167,
                12.4333,
                1.06667,
                -0.75,
                -1.11667,
                -0.2,
                1.6,
                -1.36667,
                -2.31667,
                1.11667,
            };

            lm_control_struct control = _lm_control_UTEST;//lm_control_double;
            control.verbosity = 0;
#ifdef DEBUG  
            Logger::WriteMessage("Fitting...\n");
#endif // DEBUG
            auto result = lmfit::fit_curve(par0, t, y, &gaussProbabilityDensityFunction, control);
#ifdef DEBUG
            LogUnitTestEvalGauss(result, t, y, parLbl, &gaussProbabilityDensityFunction, 0.1);
#endif // DEBUG
            Assert::IsFalse(result.status.outcome > 5);
            Assert::IsTrue(result.status.fnorm <= 30.0);
            Assert::AreEqual(192.5, result.par[1], 0.05);
            Assert::AreEqual(6.7, result.par[2], 0.30);
        }

        TEST_METHOD(_04_FitGaussPeakSigmoidNoiseL)
        {
#ifdef DEBUG  
            Logger::WriteMessage("FitGaussPeakSigmoidNoiseL\n");
#endif // DEBUG
            std::vector<std::string> parLbl{ "A   ", "Mu   ", "sigma" };
            std::vector<double> par0{ 5.0, 202.0, 10.0 };

            std::vector<double> t;
            for (double d = 100.0; d <= 250.0; d += 1.0)
            {
                t.push_back(d);
            }
            std::vector<double> y{
-0.133333      ,
-2.28333      ,
0.35           ,
1.03333        ,
-0.05         ,
1.38333        ,
-0.533333     ,
-0.266667     ,
1.8            ,
-0.0666667    ,
1.71667        ,
-0.733333     ,
-0.233333     ,
0.766667       ,
-0.516667     ,
1.18333        ,
0.7            ,
-0.116667     ,
0.183333       ,
0.733333       ,
1.83333        ,
0.5            ,
0.133333       ,
1.58333        ,
1.78333        ,
0.0333333      ,
0.366667       ,
0.65           ,
-0.0833333    ,
2.41667        ,
0.916667       ,
-1.33333      ,
0.25           ,
1.88333        ,
-0.05         ,
1.33333        ,
3.15           ,
0.733333       ,
0.266667       ,
0.183333       ,
1.46667        ,
0.766667       ,
1.63333        ,
4.21667        ,
3.51667        ,
0.4            ,
0.616667       ,
0.333333       ,
-0.733333     ,
3.16667        ,
3.93333        ,
3.35           ,
2.18333        ,
1.38333        ,
3.43333        ,
1.9            ,
0.883333       ,
4.83333        ,
3.51667        ,
1.75           ,
2.56667        ,
4.65           ,
3.75           ,
-0.283333     ,
3              ,
6.95           ,
3.41667        ,
1.65           ,
3.25           ,
4.66667        ,
3.28333        ,
2.61667        ,
6.33333        ,
4.6            ,
3.33333        ,
5.55           ,
4.68333        ,
4.43333        ,
6.16667        ,
3.33333        ,
4.78333        ,
5.5            ,
5.56667        ,
6.23333        ,
3.03333        ,
4.23333        ,
6.55           ,
6.5            ,
5.86667        ,
4.46667        ,
3.71667        ,
5.43333        ,
7              ,
5.2            ,
4.55           ,
4.75           ,
5.23333        ,
6.48333        ,
7.16667        ,
6.31667        ,
1.71667        ,
5.35           ,
8.66667        ,
4.55           ,
2.36667        ,
4.41667        ,
5.65           ,
3.33333        ,
4.08333        ,
6.41667        ,
6.9            ,
4.46667        ,
3.78333        ,
3.26667        ,
2.71667        ,
4.7            ,
5.58333        ,
4.33333        ,
1.25           ,
2.48333        ,
4.06667        ,
5.28333        ,
4.1            ,
2.33333        ,
1.71667        ,
2.58333        ,
5.21667        ,
2.83333        ,
0.35           ,
3.08333        ,
5.7            ,
1.75           ,
0.216667       ,
2.23333        ,
3.85           ,
2.46667        ,
2.03333        ,
3.56667        ,
-0.5          ,
-0.216667     ,
0.95           ,
1.66667        ,
4.61667        ,
1.61667        ,
1              ,
1.28333        ,
1.93333        ,
3.01667        ,
-0.783333     ,
-0.466667     ,
1.76667        ,

            };

            lm_control_struct control = _lm_control_UTEST;//lm_control_double;
            control.verbosity = 0;
#ifdef DEBUG 
            Logger::WriteMessage("Fitting...\n");
#endif // DEBUG
            auto result = lmfit::fit_curve(par0, t, y, &gaussProbabilityDensityFunction, control);
#ifdef DEBUG
            LogUnitTestEvalGauss(result, t, y, parLbl, &gaussProbabilityDensityFunction, 0.1);
#endif // DEBUG
            Assert::IsFalse(result.status.outcome > 5);
            Assert::IsTrue(result.status.fnorm <= 30.0);
            Assert::AreEqual(192.5, result.par[1], 0.1);
            Assert::AreEqual(31.2, result.par[2], 0.30);
        }

	};
}
