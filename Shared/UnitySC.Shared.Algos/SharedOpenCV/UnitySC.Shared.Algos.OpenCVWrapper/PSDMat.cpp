#include "PSDMat.h"
#include "Tools.h"
#include <opencv2/imgproc.hpp>
#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    
    array<System::Byte>^ PSDMat::CreateFullScreenCorrectedImage(array<double>^ doubleArray, int source_width, int source_height, int destination_width, int destination_height)
    {
        //pin_ptr is needed for the managed C# to C++ conversion, simply passing the doubleArray to the Mat constructor does not work properly in all cases
        pin_ptr<double> doublePinPtr = &doubleArray[0];
        double* doublePtr = doublePinPtr;
        cv::Mat correctedMatrix = cv::Mat(source_height, source_width, CV_64FC1, doublePtr);
        cv::Mat fullScreenMatrix = cv::Mat(destination_height, destination_width, CV_64FC1);
        cv::resize(correctedMatrix, fullScreenMatrix, fullScreenMatrix.size(), 0, 0, cv::INTER_CUBIC);
        cv::Mat correctedImage = cv::Mat(destination_height, destination_width, CV_8UC1);
        fullScreenMatrix.convertTo(correctedImage, CV_8UC1);
        return CreateByteArrayFromMat(correctedImage);
    }
}