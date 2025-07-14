#pragma once

#include <opencv2/opencv.hpp>
#include "MetroBase.hpp"

#pragma unmanaged
namespace metroCD
{

#pragma region Enums

    enum class SeekMode : int
    {
        BlackToWhite = 0,
        WhiteToBlack = 1,
        CorrelationModel = 2,
    };

    //                        BtoW                 WtoB      
    //                    * ---------  TOP   ------------ *
    //                   /                                 \
    //                  /                                   \
    //                 *        MIDDLE (pt inflexion)        *
    //                /                                       \
    //               /                                         \
    //    --------- *                 BOTTOM                    * -----------
    //
    // Middle is locatd at the mu - gauss peak location
    // Top is locatd at the mu + sigma peak location (in BtoW seekmod - invert in WtoB ==> bottom)
    // Bottom is locatd at the mu - sigma peak location (in BtoW seekmode - invert in WtoB ==> top) 

    enum class SeekLocInEdge : int
    {
        MinusSigma = -1, // Bottom (BtoW) - Top (WtoB) => mu - sigma 
        AtPeak = 0,      // Middle (gaussian localisation peak at mu)
        PlusSigma = 1,   // Top (BtoW) - Bottom (WtoB) => mu + sigma
    };

    enum class SeekScale : int
    {
        NoScale = 0,
        Scale2 = 1,
        Scale4 = 2,
        Scale8 = 3,
    };

#pragma endregion

#pragma region Inputs

    class SeekerInputs : public MInputs
    {
    public:
        SeekerInputs(cv::Point2d origin, cv::Point2d end, double width, SeekMode mode,  int kernelSize = 3, SeekLocInEdge edgeLoc = SeekLocInEdge::AtPeak,
                    SeekScale scale = SeekScale::NoScale, int searchEdgeNumber = 1)
            :MInputs(MType::Seeker)
        {
            Origin = origin;
            End = end;
            Width = width;
           
            SeekMode = mode;
            SearchEdgeNumber = searchEdgeNumber;
            SeekScale = (int) pow(2, (int)scale);
            
            KernelSize = kernelSize;
            EdgeLocalizePreference = edgeLoc;
        }

        void SetSignalAnalysisAdvancedParam(double threshold, int peakMaxWindowSize)
        {
            SigAnalysisThreshold = threshold;
            SigAnalysisPeakWindowSize = peakMaxWindowSize;
        }

    public:
        SeekMode SeekMode;
        cv::Point2d Origin; // in Image (pixels)
        cv::Point2d End; // in Image (pixels)
        double Width; // seeker width (pixels) - broadth of search area rect

        // advanced parameters
        int SeekScale;
        int SearchEdgeNumber; //[1 ..[
        int KernelSize; //[ 3, 5, 7, 9, 11 ..  // Kernel Size for first derivative signal
        SeekLocInEdge  EdgeLocalizePreference;  // preference edge detection AtPeak (middle ramp), MinusSigma (Bottom if BtoW, Top if WtoB), PlusSigma (Top if BtoW, Bottom if WtoB)
    
        // signal analysis Expert parameter
        double SigAnalysisThreshold = 10.0;  // threshold coef toward stddev moving window
        int SigAnalysisPeakWindowSize = 100;  // Maximal window size around detected Peak to perfom fit gauss
    };
       

#pragma endregion
    
#pragma region Outputs
    class SeekerOutputs : public MOutputs
    {
    public:
        SeekerOutputs()
            :MOutputs(MType::Seeker)
        {

        }

        virtual void ReportFile(std::string ReportPath, uint flags);

        std::vector<cv::Point2d> Results; // in image

        std::vector<cv::Point2d> SeekLine; // in seeker box row
        std::vector<cv::Point2d> SeekLineTreated; // in seeker box row
        std::vector<cv::Point2d> SeekLineSpikes;

        std::vector<std::vector<double>> SeekFits; // appox function fit coefficient

    };

#pragma endregion

#pragma region classes

    class Seeker : public MetroBase
    {
    public:
        Seeker();
        Seeker(std::shared_ptr<SeekerInputs> inputsParams);
        Seeker(std::shared_ptr<SeekerInputs> inputsParams, cv::Mat inputImage);

        virtual void DrawOverlay(cv::Mat& colorimg, uint flags);

        virtual std::shared_ptr<MOutputs> Compute();

    protected:
        cv::RotatedRect _rotatedRect;
        cv::Mat _affineTransformMat;

        void InitSeeker();
    
    private:
       
    };

#pragma endregion

}