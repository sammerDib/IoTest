#include "Tools.h"

#include "opencv2/opencv.hpp"

using namespace AlgosLibrary;
using namespace System;
using namespace System::Runtime::InteropServices;

typedef unsigned char byte;

namespace AlgosLibrary {

    template <class T> array<T>^ CreateArrayFromVector(const std::vector<T>& dataVec) {
        const int size = dataVec.size();
        array<T>^ dataArray = gcnew array<T>(size);
        for (int i = 0; i < size; i++) {
            dataArray[i] = dataVec[i];
        }
        return dataArray;
    }

    template array<double>^ CreateArrayFromVector<double>(const std::vector<double>& dataVec);

    array<System::Byte>^ CreateByteArrayFromMat(const cv::Mat& data) {
        int size = data.total() * data.elemSize();
        byte* bytes = new byte[size];
        std::memcpy(bytes, data.data, size * sizeof(byte));
        array<Byte>^ byteArray = gcnew array<Byte>(size);
        Marshal::Copy((IntPtr)bytes, byteArray, 0, size);
        return byteArray;
    }

    cv::Mat CreateMatFromImageData(ImageData^ img) {
        int imgType;
        switch (img->Depth) {
        case 8: // ImageType.Greyscale
            imgType = CV_8UC1;
            break;
        case 24: // ImageType.RGB
            imgType = CV_8UC3;
            break;
        case 32: // ImageType._3DA
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

        return img_mat;
    }

    const char* StringToCharArray(String^ string) {
        const char* result = "";
        if (nullptr != string) {
            const char* str = (const char*)(Marshal::StringToHGlobalAnsi(string)).ToPointer();
        }
        return result;
    }
}