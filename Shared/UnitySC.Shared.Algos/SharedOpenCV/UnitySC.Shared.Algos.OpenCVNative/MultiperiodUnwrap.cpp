#include <vector>

#include "C2DSignalAnalysis.hpp"
#include "MultiperiodUnwrap.hpp"
#include "ErrorLogging.hpp"

#pragma unmanaged

using namespace cv;
using namespace std;

class ParallelUnwrapping : public ParallelLoopBody
{
private:
    Mat& wrappedPhase;
    Mat& unwrappedPhase;
    Mat& mask;
    float previousFactor;
    float currentFactor;

public:
    ParallelUnwrapping(Mat& wrappedPhase, Mat& unwrappedPhase, Mat& mask, float previousFactor, float currentFactor) : wrappedPhase(wrappedPhase), unwrappedPhase(unwrappedPhase), mask(mask), previousFactor(previousFactor), currentFactor(currentFactor)
    {
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        float prevFactor2Pi = static_cast<float>(((double)previousFactor * CV_2PI));
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / unwrappedPhase.cols;
            int j = (r % unwrappedPhase.cols);

            float order = 0.0f;

            if (mask.ptr<uchar>(i)[j] != 0)
            {
                order = wrappedPhase.ptr<float>(i)[j] * currentFactor;
                order -= unwrappedPhase.ptr<float>(i)[j];
                order /= prevFactor2Pi;
                order += 0.5f;
                order = floor(order);

                unwrappedPhase.ptr<float>(i)[j] += order * prevFactor2Pi;
            }
            else {
                unwrappedPhase.ptr<float>(i)[j] = 0.0f;
            }
        }
    }
};

class ParallelPlaneSubstraction : public ParallelLoopBody
{
private:
    Mat& unwrappedPhase;
    Mat& plane;
    Mat& mask;
    float maskValue;

public:
    ParallelPlaneSubstraction(Mat& unwrappedPhase, Mat& plane, Mat& mask, float maskValue) : plane(plane), unwrappedPhase(unwrappedPhase), mask(mask), maskValue(maskValue)
    {
    }

    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / unwrappedPhase.cols;
            int j = (r % unwrappedPhase.cols);
            if (mask.ptr<uchar>(i)[j] != 0)
            {
                unwrappedPhase.ptr<float>(i)[j] -= plane.ptr<float>(i)[j];
            }
            else {
                unwrappedPhase.ptr<float>(i)[j] = maskValue;
            }
        }
    }
};

namespace phase_unwrapping {
    Mat MultiperiodUnwrap(vector<Mat>& wrappedPhaseMaps, Mat mask, vector<int> periods, int nbPeriod) {
        Mat unwrappedPhase = wrappedPhaseMaps[0].clone();

        if (nbPeriod == 0)
        {
            ErrorLogging::LogError("[MultiperiodUnwrap] The number of periods expected is different from the number of periods provided.");
            return unwrappedPhase;
        }

        if (periods.size() != nbPeriod)
        {
            ErrorLogging::LogError("[MultiperiodUnwrap] The number of periods expected is different from the number of periods provided.");
            return unwrappedPhase;
        }

        for (Mat phase : wrappedPhaseMaps)
        {
            if (phase.type() != CV_32FC1)
            {
                ErrorLogging::LogError("[MultiperiodUnwrap] The provided phase map is not the right type.");
                return unwrappedPhase;
            }
        }

        if (mask.type() != CV_8UC1)
        {
            ErrorLogging::LogError("[MultiperiodUnwrap] The provided mask is not the right type.");
            return unwrappedPhase;
        }

        vector<float> ratios;
        ratios.push_back(1);
        for (int p = 1; p < nbPeriod; p++)
        {
            ratios.push_back((float)periods[p] / (float)periods[p - 1]);
        }

        float currentFactor = 1;
        float previousFactor = 0;

        for (int p = 1; p < nbPeriod; p++)
        {
            previousFactor = currentFactor;
            currentFactor = currentFactor * ratios[p];

            ParallelUnwrapping unwrap(wrappedPhaseMaps[p], unwrappedPhase, mask, previousFactor, currentFactor);
            parallel_for_(Range(0, unwrappedPhase.rows * unwrappedPhase.cols), unwrap);
        }

        return unwrappedPhase;
    }

    void SubstractPlaneFromUnwrapped(Mat& unwrappedPhase, Mat mask)
    {
        if (unwrappedPhase.type() != CV_32FC1)
        {
            ErrorLogging::LogErrorAndThrow("[SubstractPlaneFromUnwrapped] The provided phase map is not the right type.");
            return;
        }

        if (mask.type() != CV_8UC1)
        {
            ErrorLogging::LogErrorAndThrow("[SubstractPlaneFromUnwrapped] The provided mask is not the right type.");
            return;
        }

        Mat plane = signal_2D::SolvePlaneEquation(unwrappedPhase, mask);

            double minPlane, maxPlane, minPhase, maxPhase;
            cv::minMaxLoc(plane, &minPlane, &maxPlane);
            cv::minMaxLoc(unwrappedPhase, &minPhase, &maxPhase);
            float maskValue = static_cast<float>(floor(minPhase - maxPlane)); //conversion from 'double' to 'float', possible loss of data

        ParallelPlaneSubstraction planeSub(unwrappedPhase, plane, mask, maskValue);
        parallel_for_(Range(0, unwrappedPhase.rows * unwrappedPhase.cols), planeSub);
    }
} // namespace phase_unwrapping