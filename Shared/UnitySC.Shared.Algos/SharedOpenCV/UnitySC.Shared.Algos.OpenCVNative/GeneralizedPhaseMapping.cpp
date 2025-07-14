#include <vector>

#include "PhaseMappingWithHariharan.hpp"
#include "CImageTypeConvertor.hpp"
#include "ErrorLogging.hpp"
#include "WrappedPhaseMap.hpp"
#include "Utils.hpp"
#include "GeneralizedPhaseMapping.hpp"

using namespace std;
using namespace cv;

class ParallelWrappedPhaseComputationForUnsignedChar : public ParallelLoopBody
{
private:
    vector<Mat>& interferograms;
    Mat& phaseMap;
    Mat& phaseMap2;
    Mat& amplitudeMap;
    Mat& backgroundMap;
    int stepNb;
    float deltaPhase;
    vector<vector<float>> lookupTableSin;
    vector<vector<float>> lookupTableCos;
public:
    ParallelWrappedPhaseComputationForUnsignedChar(vector<Mat>& interferograms, Mat& phaseMap, Mat& phaseMap2, Mat& amplitudeMap, Mat& backgroundMap, int stepNb)
        : interferograms(interferograms), phaseMap(phaseMap), phaseMap2(phaseMap2), amplitudeMap(amplitudeMap), backgroundMap(backgroundMap), stepNb(stepNb)
    {
        deltaPhase = static_cast<float>(CV_2PI / (double)stepNb); //conversion from 'double' to 'float', possible loss of data

        // We have images of unsigned char, that means 256 possible inputs to your function. Calculate them once, and store result in lookup table.
        for (int step = 0; step < stepNb; step++)
        {
            vector<float> lookupTableSinStep;
            vector<float> lookupTableCosStep;

            for (int i = 0; i < 256; i++)
            {
                lookupTableSinStep.push_back(i * sin(deltaPhase * step));
                lookupTableCosStep.push_back(i * cos(deltaPhase * step));
            }
            lookupTableSin.push_back(lookupTableSinStep);
            lookupTableCos.push_back(lookupTableCosStep);
        }
    }
    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        for (int r = range.start; r < range.end; r++)
        {
            int i = r / interferograms[0].cols;
            int j = (r % interferograms[0].cols);

            float numerator = 0.0;
            float denominator = 0.0;
            float sum = 0.0;

            for (int step = 0; step < stepNb; step++)
            {
                uchar* sptr = interferograms[step].ptr<uchar>(i);
                uchar value = sptr[j];

                numerator += lookupTableSin[step][(int)value];
                denominator += lookupTableCos[step][(int)value];
                sum += (float)value;
            }

            float phase = atan2(numerator, denominator);
            float amplitude = static_cast<float>(sqrt(pow(numerator, 2) + pow(denominator, 2))); //conversion from 'double' to 'float', possible loss of data // use only once
            float background = sum / (float)stepNb; // only use once why a variable is needed
            amplitudeMap.ptr<float>(i)[j] = amplitude;
            phaseMap.ptr<float>(i)[j] = phase;
            phaseMap2.ptr<float>(i)[j] = phase < 0.0f ? static_cast<float>(phase + CV_2PI) : phase;
            backgroundMap.ptr<float>(i)[j] = background;
        }
    }
};

class ParallelWrappedPhaseComputationForFloat : public ParallelLoopBody
{
private:
    vector<Mat>& interferograms;
    Mat& phaseMap;
    Mat& phaseMap2;
    Mat& amplitudeMap;
    Mat& backgroundMap;
    int stepNb;
    float deltaPhase;
public:
    ParallelWrappedPhaseComputationForFloat(vector<Mat>& interferograms, Mat& phaseMap, Mat& phaseMap2, Mat& amplitudeMap, Mat& backgroundMap, int stepNb)
        : interferograms(interferograms), phaseMap(phaseMap), phaseMap2(phaseMap2), amplitudeMap(amplitudeMap), backgroundMap(backgroundMap), stepNb(stepNb)
    {
        deltaPhase = static_cast<float>(CV_2PI / (double)stepNb); //conversion from 'double' to 'float', possible loss of data
    }
    virtual void operator()(const Range& range) const CV_OVERRIDE
    {
        size_t elem_step = interferograms[0].step / sizeof(float);

        for (int r = range.start; r < range.end; r++)
        {
            int i = r / interferograms[0].cols;
            int j = (r % interferograms[0].cols);

            float numerator = 0.0f;
            float denominator = 0.0f;
            float sum = 0.0f;

            for (int step = 0; step < stepNb; step++)
            {
                //int type = interferograms[step].type(); // Not used
                float* ptr = interferograms[step].ptr<float>(i);
                float value = ptr[j];

                numerator += value * sin(deltaPhase * (float) step);
                denominator += value * cos(deltaPhase * (float) step);
                sum += value;
            }

            float phase = atan2(numerator, denominator);
            float amplitude = static_cast<float>(sqrt(pow(numerator, 2) + pow(denominator, 2))); //conversion from 'double' to 'float', possible loss of data // use only once
            float background = sum / (float)stepNb; // only use once why a variable is needed
            amplitudeMap.ptr<float>(i)[j] = amplitude;
            phaseMap.ptr<float>(i)[j] = phase;
            phaseMap2.ptr<float>(i)[j] = phase < 0.0f ? static_cast<float>(phase + CV_2PI) : phase;
            backgroundMap.ptr<float>(i)[j] = background;
        }
    }
};

#pragma unmanaged
namespace generalized_phase_mapping {
    namespace {
        WrappedPhaseMap quickMethodHightPrecision(vector<Mat> interferograms, int stepNb);
        WrappedPhaseMap quickMethodLowPrecision(vector<Mat> interferograms, int stepNb);
    } // namespace

    WrappedPhaseMap GeneralizedPhaseMapping(vector<Mat> interferometricImgs, int stepNb, Precision precision) {
        if (stepNb < 3 || interferometricImgs.size() < 3) {
            ErrorLogging::LogErrorAndThrow("[Generalized phase mapping] Expected at least 3 steps with one interferograms per step to compute an unambiguous phase map of the unknown wavefront");
        }

        if (interferometricImgs.size() % stepNb != 0) {
            ErrorLogging::LogErrorAndThrow("[Generalized phase mapping] The interferograms count must be multiple of the number of steps.");
        }

        Size size = interferometricImgs.at(0).size();
        for (Mat img : interferometricImgs) {
            if ((img.size() != size)) {
                ErrorLogging::LogErrorAndThrow("[Phase mapping] Input interferograms must all be the same size: ", img.size(), " != ", size);
            }
        }

        WrappedPhaseMap wrappedPhaseMap;
        if (precision == High) {
            wrappedPhaseMap = quickMethodHightPrecision(interferometricImgs, stepNb);
        }
        else {
            wrappedPhaseMap = quickMethodLowPrecision(interferometricImgs, stepNb);
        }

        if (wrappedPhaseMap.Phase.empty() || wrappedPhaseMap.Background.empty()) {
            stringstream strStrm;
            strStrm << "[Generalized phase mapping] Algorithm failed to compute phase map.";
            string message = strStrm.str();
            LoggerOCV::Error(message);
            throw exception(message.c_str());
        }

        return wrappedPhaseMap;
    }

    namespace {
        WrappedPhaseMap quickMethodLowPrecision(vector<Mat> interferometricImgs, int stepNb)
        {
            // average all images from the same step
            vector<Mat> interferogramsCV8U = Convertor::ConvertAllToCV8UC1(interferometricImgs);
            vector<Mat> interferograms = AverageInterferogramsPerStep(interferogramsCV8U, stepNb);

            WrappedPhaseMap wrappedPhaseMap;

            Size size = interferograms.at(0).size();
            Mat amplitudeMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap2 = Mat::zeros(size, CV_32FC1);
            Mat backgroundMap = Mat::zeros(size, CV_32FC1);

            ParallelWrappedPhaseComputationForUnsignedChar obj(interferograms, phaseMap, phaseMap2, amplitudeMap, backgroundMap, stepNb);
            parallel_for_(Range(0, interferograms[0].rows * interferograms[0].cols), obj);

            cv::Mat dark = backgroundMap - amplitudeMap;

            wrappedPhaseMap = WrappedPhaseMap(phaseMap, phaseMap2, amplitudeMap, backgroundMap, dark);
            return wrappedPhaseMap;
        }

        WrappedPhaseMap quickMethodHightPrecision(vector<Mat> interferometricImgs, int stepNb)
        {
            // average all images from the same step
            vector<Mat> interferogramsCV32F = Convertor::ConvertAllToCV32FC1(interferometricImgs);
            vector<Mat> interferograms = AverageInterferogramsPerStep(interferogramsCV32F, stepNb);

            WrappedPhaseMap wrappedPhaseMap;

            Size size = interferograms.at(0).size();
            Mat amplitudeMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap = Mat::zeros(size, CV_32FC1);
            Mat phaseMap2 = Mat::zeros(size, CV_32FC1);
            Mat backgroundMap = Mat::zeros(size, CV_32FC1);

            ParallelWrappedPhaseComputationForFloat obj(interferograms, phaseMap, phaseMap2, amplitudeMap, backgroundMap, stepNb);
            parallel_for_(Range(0, size.height * size.width), obj);

            cv::Mat dark = backgroundMap - amplitudeMap;

            wrappedPhaseMap = WrappedPhaseMap(phaseMap, phaseMap2, amplitudeMap, backgroundMap, dark);
            return wrappedPhaseMap;
        }
    } // namespace
} // namespace generalized_phase_mapping