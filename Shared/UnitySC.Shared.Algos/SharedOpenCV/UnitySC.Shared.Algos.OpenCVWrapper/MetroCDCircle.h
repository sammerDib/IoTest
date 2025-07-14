#pragma once


#pragma managed
#include "Tools.h"
#include "ImageData.h"    
#include "ShapeDetector.h"

namespace UnitySCSharedAlgosOpenCVWrapper {

    public enum class MetroSeekerMode : int { BlackToWhite, WhiteToBlack };

    public enum class MetroSeekerLocInEdge : int { MinusSigma=-1, AtPeak=0, PlusSigma=1 };

    // check coherence with native MetroDrawFlag
    public enum class MetroCDDrawFlag : unsigned int
    {
        NoDraw = 0, // do not draw / no report Overlay
        DrawSeekers = 1 << 0, //MetroDrawFlag::DrawSeekers,
        DrawDetection = 1 << 1, //MetroDrawFlag::DrawDetection,
        DrawSkipDetection = 1 << 2, //MetroDrawFlag::DrawSkipDetection,
        DrawFit = 1 << 3, //MetroDrawFlag::DrawFit,
        DrawCenterFit = 1 << 4, //MetroDrawFlag::DrawCenterFit,
    };

    public ref class MetroCDFlags abstract sealed  // abstract sealed equivalent to c# static class in CLI/C++
    {
    public:
        static bool HasDrawFlag(MetroCDDrawFlag flags, MetroCDDrawFlag cdflag)
        {
            return HasDrawFlag((unsigned int)flags, cdflag);
        };
        static bool HasDrawFlag(unsigned int uflags, MetroCDDrawFlag cdflag)
        {
            return ((uflags & (unsigned int)cdflag) == (unsigned int)cdflag);
        }
    };
  
    public ref struct MetroCDReport
    {
    public:
        System::String^ ReportPathBase;
        unsigned int Drawflag = (uint)MetroCDDrawFlag::DrawDetection | (uint)MetroCDDrawFlag::DrawFit; //DefaultMetroDrawFlags;
        bool IsReportOverlay = false;
        bool IsReportCsv = false;
    };

    public ref class MetroCDCircleInput 
    {
    public:
        MetroCDCircleInput(Point2d^ seekcenter, double radius_px, double radiusToleranceSearch_px)
        {
            center = seekcenter;
            radiusTarget = radius_px;
            radiusTolerance = radiusToleranceSearch_px;
        };

    public:
        Point2d^ center;        // circle expected center in pixels in image referential
        double radiusTarget;    // target in pixels
        double radiusTolerance; // tolerance in pixels to find target +/- Tolerance

        // advanced expert parameters
        int seekerNumber = 0; // 0 == Auto; else [4 ..[
        double seekerWidth = 0; // 0 == Auto; seeker width (pixels) - broadth of search area rect by seekers
        MetroSeekerMode mode = MetroSeekerMode::BlackToWhite;
        int KernelSize = 3; //[ 3, 5, 7, 9, 11 ..  // Kernel Size for first derivative signal
        MetroSeekerLocInEdge EdgeLocalizePreference = MetroSeekerLocInEdge::AtPeak; // advanced edge Loc search, define if winner should localize in middle step, top stop or botrom step 

        // expert advanced parameters
        int SeekScale = 0;  // signal analysis Expert parameter
        double SigAnalysisThreshold = 10.0;  // threshold coef toward stddev moving window
        int SigAnalysisPeakWindowSize = 100;  // Maximal window size around detected Peak to perfom fit gauss
    };

    public ref class MetroCDCircleOutput {
    public:
        MetroCDCircleOutput(){
        
        }
    public:
        bool IsSuccess = true;
        System::String^ Message = "";
        Point2d^ foundCenter;  // circle center found in pixels in image referential
        double foundRadius;    // radius in pixels
    };


    public ref class MetroCDCircle {

    public:
       static MetroCDCircleOutput^ DetectCircle(ImageData^ img, MetroCDCircleInput^ input, MetroCDReport^ report);
    };
}