#pragma once

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

    public ref class PSDMat {

    public:
        static array<System::Byte>^ CreateFullScreenCorrectedImage(array<double>^ doubleArray, int source_width, int source_height, int destination_width, int destination_height);
    };
    
}
