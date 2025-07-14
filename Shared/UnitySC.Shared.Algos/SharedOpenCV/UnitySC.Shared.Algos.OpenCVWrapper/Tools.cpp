#include "Tools.h"

#pragma unmanaged
#include "opencv2/opencv.hpp"

#pragma managed
using namespace System;
using namespace System::Runtime::InteropServices;

typedef unsigned char byte;
namespace UnitySCSharedAlgosOpenCVWrapper {
    template <class T> array<T>^ CreateArrayFromVector(const std::vector<T>& dataVec) {
        const int size = (int) dataVec.size();
        array<T>^ dataArray = gcnew array<T>(size);
        for (int i = 0; i < size; i++) {
            dataArray[i] = dataVec[i];
        }
        return dataArray;
    }

    template array<double>^ CreateArrayFromVector<double>(const std::vector<double>& dataVec);

    cv::Point2i ToCVPoint2i(Point2i^ pt) { 
        return cv::Point2i(pt->X, pt->Y); 
    }

    cv::Point2d ToCVPoint2d(Point2d^ pt) {
        return cv::Point2d(pt->X, pt->Y);
    }

    cv::Point2f ToCVPoint2d(Point2f^ pt) { 
        return cv::Point2f(pt->X, pt->Y); 
    }

    cv::Rect ToCVRect(RegionOfInterest^ roi){ 
        return cv::Rect(roi->X, roi->Y, static_cast<int>(roi->Width), static_cast<int>(roi->Height));
    }

    cv::Rect2d ToCVRect2d(RegionOfInterest^ roi) {
        return cv::Rect2d(roi->X, roi->Y, roi->Width, roi->Height);
    }

    RegionOfInterest^ FromCVRect2d(const cv::Rect2d &rect) {
        auto res = gcnew RegionOfInterest();
        res->X = rect.x;
        res->Y = rect.y;
        res->Width = rect.width;
        res->Height = rect.height;

        return res;
    }

    array<System::Byte>^ CreateByteArrayFromMat(const cv::Mat& data) {
        size_t size = data.total() * data.elemSize();
        array<Byte>^ byteArray = gcnew array<Byte>((int)size); //conversion from 'size_t' to 'int', possible loss of data
        Marshal::Copy((IntPtr)data.data, byteArray, 0, (int)size); //conversion from 'size_t' to 'int', possible loss of data

        return byteArray;
    }

    ImageType CreateImageType(const cv::Mat& img) {
        switch (img.type()) {
        case CV_8UC1:
            return ImageType::GRAYSCALE_Unsigned8bits;
        case CV_16UC1:
            return ImageType::GRAYSCALE_Unsigned16bits;
        case CV_8UC3:
            return ImageType::RGB_Unsigned8bits;
        case CV_32FC1:
            return ImageType::GRAYSCALE_Float32bits;
        default:
            throw gcnew Exception("unknown image format");
        }
    }

    cv::Mat CreateMatFromImageData(ImageData^ img) {
        int imgType;
        switch (img->Type) {
        case ImageType::GRAYSCALE_Unsigned8bits:
            imgType = CV_8UC1;
            break;
        case ImageType::GRAYSCALE_Unsigned16bits:
            imgType = CV_16UC1;
            break;
        case ImageType::RGB_Unsigned8bits:
            imgType = CV_8UC3;
            break;
        case ImageType::GRAYSCALE_Float32bits:
            imgType = CV_32FC1;
            break;
        default:
            throw gcnew Exception("unknown image format");
        }
        pin_ptr<System::Byte> p = &img->ByteArray[0];
        unsigned char* pby = p;
        char* pch = reinterpret_cast<char*>(pby);
        cv::Mat img_mat = cv::Mat(img->Height, img->Width, imgType, (void*)pch);

        // OpenCV use BGR colour space instead of RGB
        if (CV_8UC3 == imgType) {
            cv::cvtColor(img_mat, img_mat, cv::COLOR_RGB2BGR);
        }

        return img_mat.clone();
    }

    const char* StringToCharArray(String^ string) {
        const char* result = "";
        if (nullptr != string) {
            const char* str = (const char*)(Marshal::StringToHGlobalAnsi(string)).ToPointer();
        }
        return result;
    }

    std::string CSharpStringToCppString(String^ string)
    {
        IntPtr ip = Marshal::StringToHGlobalAnsi(string);
        std::string cppString = static_cast<const char*>(ip.ToPointer());
        Marshal::FreeHGlobal(ip);
        return cppString;
    }

    Exception^ HandleAndRethrowCppException(const std::exception& exception)
    {
        String^ message = gcnew String(exception.what());
        throw gcnew Exception(message);
    }

    
}