#pragma once
#include <opencv2/opencv.hpp>
#include "MetroBaseEnums.hpp"

//#pragma unmanaged
//using namespace std;

namespace metroCD
{
//#define csvOUTPUT_COMPUTE //uncomment to produce csv in unit tests
#ifdef csvOUTPUT_COMPUTE
    #define csv_OUTPUT_PATH  std::string("C:\\Work\\Data\\")
#endif 

#pragma region Enums
   
#pragma endregion

#pragma region Inputs
    class MInputs
    {
        protected:
            MType _type;
        public:
            MInputs(MType mtype) { _type = mtype; };

            MType Type() { return _type; }
    };
#pragma endregion

#pragma region Outputs
    class MOutputs
    {
        protected:
            MType _type;
            bool _success = true;
            std::string _message = "";

        public:
            MOutputs(MType mtype) { _type = mtype; };

            MType Type() { return _type; }


            virtual void ReportFile(std::string ReportPath, uint flags) {};

            virtual void SetErrorMessage(const std::string& message) {
                _success = false; _message = message.c_str();
            }

            bool IsSuccess() { return _success; };

            std::string GetMessage() { return _message.c_str(); };



    };
#pragma endregion

#pragma region classes

    class MetroBase
    {
    protected :
        MType _type;
        MStatus _status;

        cv::Mat  _inputImage;
        //input parameters
        std::shared_ptr<MInputs> _inputsPrm;
        //output
        std::shared_ptr<MOutputs> _outputs;

    public:
        MetroBase();

        MetroBase(MType mtype);

        MetroBase(MType mtype, std::shared_ptr<MInputs> inputsParams, cv::Mat inputImage);

        void SetInputImage(cv::Mat img);

        virtual void DrawOverlay(cv::Mat& colorimg, uint flags) = 0;

        virtual std::shared_ptr<MOutputs> Compute() = 0;

        virtual void Abort();

        MType GetType() { return _type; }
        MStatus GetStatus() { return _status; }

    };

#pragma endregion

}

