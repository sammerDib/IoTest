#include "DieDetector.h"

#include <msclr/gcroot.h>

#include "CDetectDies.hpp"

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {
    void DieDetector::DetectDies(ImageData^ waferImg,
                                                   ImageData^ waferMask,
                                                   array<DieKindDetails^>^ theoreticalDies,
                                                   DieDetectionSettings^ settings)
    {
        std::vector<cv::Mat> refDies {};
        std::vector<std::vector<cv::Point>> theoPos {};
        for each (auto die_kind_details in theoreticalDies)
        {
            refDies.push_back(CreateMatFromImageData(die_kind_details->ReferenceDie));
            std::vector<cv::Point> diePos {};
            for each (auto pos in die_kind_details->theoreticalPositions)
            {
                diePos.push_back(ToCVPoint2i(pos));
            }
            theoPos.push_back(diePos);
        }

        msclr::gcroot dieCb = settings->DieCallback;
        msclr::gcroot missingCb = settings->MissingDiesCallback;

        auto waferImgMat = CreateMatFromImageData(waferImg);
        auto waferMaskMat = CreateMatFromImageData(waferMask);
        
        psd::DetectDies(
            waferImgMat,
            waferMaskMat,
            refDies,
            theoPos,
            settings->NbOfDownsamplingSteps,
            settings->Gamma,
            settings->UseHighResolutionPrecision,
            settings->Pass1PositionConfidenceThreshold,
            settings->Pass1AngleConfidenceThreshold,
            [&dieCb] (const psd::DieResult &dieRes) {
                    auto newDieRes = gcnew DieResult();
                    newDieRes->TheoreticalROI = FromCVRect2d(dieRes.TheoreticalROI);
                    newDieRes->DetectedROI = FromCVRect2d(dieRes.DetectedROI);
                    newDieRes->Angle = dieRes.Angle;
                    newDieRes->PosConfidence = dieRes.PosConfidence;
                    newDieRes->AngleConfidence = dieRes.AngleConfidence;
                    newDieRes->dieKindIndex = dieRes.dieKindIndex;
                    newDieRes->theoreticalPositionIndex = dieRes.index;
                    newDieRes->passNb = dieRes.passNb;
                    dieCb->Invoke(newDieRes);
                },[&missingCb] (const psd::MissingDie &dieRes) {
                    auto missingDie = gcnew MissingDie();
                    missingDie->TheoreticalROI = FromCVRect2d(dieRes.TheoreticalROI);
                    missingDie->dieKindIndex = dieRes.dieKindIndex;
                    missingDie->theoreticalPositionIndex = dieRes.index;
                    missingCb->Invoke(missingDie);
                }
            );
    }
}