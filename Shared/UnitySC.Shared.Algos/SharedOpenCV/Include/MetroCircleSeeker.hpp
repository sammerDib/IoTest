#pragma once

#include <opencv2/opencv.hpp>
#include "MetroBase.hpp"
#include "MetroSeeker.hpp"
#pragma unmanaged

namespace metroCD
{
#pragma region Enums
    enum class CircleSeekMode : int
    {
        BlackToWhite = SeekMode::BlackToWhite,
        WhiteToBlack = SeekMode::WhiteToBlack,
    };
#pragma endregion

#pragma region Inputs

    class CircleSeekerInputs : public MInputs
    {
    public:
        CircleSeekerInputs(cv::Point2d center, double radius_px, double radiusToleranceSearch_px, int seekerNumber, double seekerWidth, CircleSeekMode mode = CircleSeekMode::BlackToWhite, int kernelSize = 3, SeekLocInEdge edgeLoc = SeekLocInEdge::AtPeak)
            :MInputs(MType::Circle)
        {
            Center = center;
            Radius = radius_px;
            RadiusToleranceSearch = radiusToleranceSearch_px;

            SeekMode = mode;
            SeekerNumber = seekerNumber;
            SeekerWidth = seekerWidth;

            KernelSize = kernelSize;
            EdgeLocalizePreference = edgeLoc;
        }

        // for advanced expert mode -- and debug use -- might not be called in source code but to let it their to avoid producing a special nuget version for debug
        void SetSignalAnalysisAdvancedParam(double threshold, int peakMaxWindowSize, int scale) {
            SigAnalysisThreshold = threshold;
            SigAnalysisPeakWindowSize = peakMaxWindowSize;
            SeekScale = (metroCD::SeekScale)scale;
        };


    public:

        cv::Point2d Center; // in Image (pixels)
        double Radius;  // in pixels unit
        double RadiusToleranceSearch; // in pixel unit
        CircleSeekMode SeekMode;

        int SeekerNumber;  //0 == Auto; [4 ..[
        double SeekerWidth; // 0 == Auto; seeker width (pixels) - broadth of search area rect

        int KernelSize; //[ 3, 5, 7, 9, 11 ..  // Kernel Size for first derivative signal
        SeekLocInEdge EdgeLocalizePreference; // advanced edge Loc search, define if winner should localize in middle step, top stop or botrom step 
      
        // expert advanced parameters
        SeekScale SeekScale = SeekScale::NoScale;  // signal analysis Expert parameter
        double SigAnalysisThreshold = 10.0;  // threshold coef toward stddev moving window
        int SigAnalysisPeakWindowSize = 100;  // Maximal window size around detected Peak to perfom fit gauss
    };
#pragma endregion

#pragma region Outputs
    class CircleSeekerOutputs : public MOutputs
    {
    public:
        CircleSeekerOutputs()
            :MOutputs(MType::Circle)
        {

        }

        virtual void ReportFile(std::string ReportPath, uint flags);

        cv::Point2d FoundCenter = cv::Point2d(0.0,0.0); // in image
        double FoundRadius = 0.0; // in pixel

        std::vector<std::tuple<std::shared_ptr<SeekerOutputs>, double>> SeekersOutputs; // <seeker outputs , seeker Angle>
    };


#pragma endregion

#pragma region classes
    class CircleSeeker : public MetroBase
    {
    public:
        CircleSeeker();
        CircleSeeker(std::shared_ptr<CircleSeekerInputs> inputsParams);
        CircleSeeker(std::shared_ptr<CircleSeekerInputs> inputsParams, cv::Mat inputImage);

        virtual void DrawOverlay(cv::Mat& colorimg, uint flags);

        virtual std::shared_ptr<MOutputs> Compute();

        

    protected:
        void InitSeeker();

    private:
        struct SeekerOriented
        {
            double angle;  // in degree, trigo oriented
            Seeker seeker; 
        };
        std::vector<SeekerOriented> _seekersRing; // seekers ring around center
    };
#pragma endregion

}