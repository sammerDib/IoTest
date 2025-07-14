#pragma once

#pragma managed

namespace VSIWrapper {

    public ref class ImageData {

    public:
        ImageData() {};

        ImageData(array<System::Byte>^ byteArray, int width, int height, int depth) : ByteArray(byteArray), Width(width), Height(height), Depth(depth) {}

        ImageData(int width, int height) : Width(width), Height(height) {
            ByteArray = gcnew array<System::Byte>(width * height);
        }

        array<System::Byte>^ ByteArray;
        int Depth;
        int Height;
        int Width;
    };
}
