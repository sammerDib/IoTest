#pragma unmanaged
#include <opencv2/core/core.hpp>

#include "MetroCircleSeeker.hpp"

#pragma managed
//#include "ImageData.h"    
#include "MetroCDCircle.h"

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace metroCD;

namespace UnitySCSharedAlgosOpenCVWrapper {

#pragma managed(push, off)
    std::shared_ptr<CircleSeekerOutputs> ComputeMetroCircle(cv::Mat& inputImage, std::shared_ptr<CircleSeekerInputs> seekin, std::string& ReportBase, bool isOverlayRequired, uint drawFlags, bool isReportCsv)
    {
        cv::Mat  overlayImage;
        // if overlay is required create color input image
        if (isOverlayRequired)
        {
            cv::cvtColor(inputImage, overlayImage, cv::COLOR_GRAY2RGB);
        }

        CircleSeeker sk(seekin, inputImage);
        auto seekout = std::static_pointer_cast<CircleSeekerOutputs>(sk.Compute());

        if (isOverlayRequired)
        {
            sk.DrawOverlay(overlayImage, drawFlags);
            cv::imwrite(ReportBase + "CircleSeekerOverlay.png", overlayImage);
        }

        if (isReportCsv)
        {
            seekout->ReportFile(ReportBase + "CircleSeeker.csv", 0);
        }

        return seekout;
    }
#pragma managed(pop)

    MetroCDCircleOutput ^ UnitySCSharedAlgosOpenCVWrapper::MetroCDCircle::DetectCircle(ImageData^ img, MetroCDCircleInput^ input, MetroCDReport^ report)
    {
        MetroCDCircleOutput^ output = gcnew MetroCDCircleOutput();

        // image data to open cv:: Mat

        // Process input parameters
        cv::Mat inputImage = CreateMatFromImageData(img);
 
        //CircleSeekerInputs
        bool isOverlayRequired =  false;
        bool isReportCsv =  false;
        std::string reportBasePath = "";
        uint drawflags = 0;

        if (report != nullptr)
        {
            reportBasePath = CSharpStringToCppString(report->ReportPathBase);
            isOverlayRequired = report->IsReportOverlay;
            isReportCsv = report->IsReportCsv;
            drawflags = report->Drawflag;
        }

        auto center = ToCVPoint2d(input->center);
        auto radiusTarget = input->radiusTarget;
        auto radiusTolerance = input->radiusTolerance;
        auto seekerNumber = input->seekerNumber;
        auto seekerWidth = input->seekerWidth;
        auto mode = (CircleSeekMode)((int)input->mode);
        auto kernelSize = input->KernelSize;
        auto edgeLocalizePreference = (SeekLocInEdge)((int)input->EdgeLocalizePreference);
        auto SeekScale = input->SeekScale;
        auto SigAnalysisThreshold = input->SigAnalysisThreshold;
        auto SigAnalysisPeakWindowSize = input->SigAnalysisPeakWindowSize;

        auto seekin = std::make_shared<CircleSeekerInputs>(center, radiusTarget, radiusTolerance, seekerNumber, seekerWidth, mode, kernelSize, edgeLocalizePreference);

        (seekin.get())->SetSignalAnalysisAdvancedParam(SigAnalysisThreshold, SigAnalysisPeakWindowSize, SeekScale);
   
      
        auto outptr = ComputeMetroCircle(inputImage, seekin, reportBasePath, isOverlayRequired, drawflags, isReportCsv);

        output->IsSuccess = outptr->IsSuccess();
        output->Message = gcnew String(outptr->GetMessage().c_str()); 
        if (output->IsSuccess)
        {
            auto cvpt = (outptr.get())->FoundCenter;
            output->foundCenter = gcnew Point2d(cvpt.x, cvpt.y);
            output->foundRadius = (outptr.get())->FoundRadius;
        }
 
        return output;

    }

}
