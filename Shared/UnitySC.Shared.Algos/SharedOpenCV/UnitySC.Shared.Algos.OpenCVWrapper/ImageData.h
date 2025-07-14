#pragma once

#include "ImageType.h"

#pragma managed

namespace UnitySCSharedAlgosOpenCVWrapper {
    public ref class ImageData {
    public:
        ImageData() {};

        ImageData(array<System::Byte>^ byteArray, int width, int height, ImageType type) : ByteArray(byteArray), Width(width), Height(height), Type(type) {}

        ImageData(int width, int height) : Width(width), Height(height) {
            ByteArray = gcnew array<System::Byte>(width * height);
        }

        void fill(const System::Byte value)
        {
            for (int i = 0; i < Width * Height; i++)
            {
                ByteArray[i] = value;
            }
        }

        array<System::Byte>^ ByteArray;
        ImageType Type;
        int Height;
        int Width;
    };
}
