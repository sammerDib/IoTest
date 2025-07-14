#include <vector>

#include "Utils.hpp"
#include "CImageTypeConvertor.hpp"
#include "ErrorLogging.hpp"

using namespace std;
using namespace cv;

#pragma unmanaged
vector<Mat> AverageInterferogramsPerStep(vector<Mat>& interferometricImgs, int stepsNb) {
    if (interferometricImgs.size() % stepsNb != 0)
    {
        ErrorLogging::LogErrorAndThrow("[Phase mapping] The interferograms to be averaged must be multiple of the number of steps: ", interferometricImgs.size(), " is not multiple of ", stepsNb);
    }
    if (interferometricImgs.size() == stepsNb)
    {
        return interferometricImgs;
    }

    Size size = interferometricImgs.at(0).size();
    int type = interferometricImgs.at(0).type();

    for (Mat img : interferometricImgs) {
        if ((img.size() != size))
            ErrorLogging::LogErrorAndThrow("[Phase mapping] Input interferograms must all be the same size: ", img.size(), " != ", size);
        if ((img.type() != type))
            ErrorLogging::LogErrorAndThrow("[Phase mapping] Input interferograms must all be the same type: ", img.type(), " != ", type);
    }

    vector<Mat> averagedInterferometricImgs;
    int imageNbPerStep = (int)interferometricImgs.size() / stepsNb;
    for (int step = 0; step < stepsNb; step++) {
        Mat temp = Mat::zeros(size, type);
        for (int img = 0; img < imageNbPerStep; img++) {
            int index = step * imageNbPerStep + img;
            temp += interferometricImgs.at(index);
        }
        temp /= imageNbPerStep;
        averagedInterferometricImgs.push_back(temp);
    }

    return averagedInterferometricImgs;
}