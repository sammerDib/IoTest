#include "Status.hpp"
#include "Wafer.hpp"

#include "BareWaferAlignment.h"

using namespace System;
using namespace System::Collections::Generic;

#pragma managed
namespace UnitySCSharedAlgosOpenCVWrapper {

  namespace {
    static bool IsEdgeImage(BareWaferAlignmentImageData^ imageData) {
        return (imageData->ExpectedShape.Equals(WaferEdgeShape::EDGE));
    }

    static bool IsNotchImage(BareWaferAlignmentImageData^ imageData) {
        return (imageData->ExpectedShape.Equals(WaferEdgeShape::NOTCH));
    }

    Wafer::WaferData ToCppType(WaferInfos^ value) {
        Wafer::WaferData result;
        result.type = static_cast<Wafer::WaferData::Type>(value->Type);
        result.radiusInMicrons = value->RadiusInMicrons;
        result.notchWidthInMicrons = value->NotchWidthInMicrons;
        return result;
    }

    ref class AddImageAsEdgeHelper {
      Wafer *_wafer;
      String ^ _reportPath;

    public:
      AddImageAsEdgeHelper(Wafer *wafer, String ^ reportPath) : _wafer(wafer), _reportPath(reportPath) {}

      void Perform(BareWaferAlignmentImageData^ imageData) {
        if (imageData->ExpectedShape != WaferEdgeShape::EDGE) {
          throw gcnew System::Exception("Type mismatch: Cannot use as EDGE image");
        }

        cv::Mat edgeDataMat = CreateMatFromImageData(imageData);
        EdgeImage::PixelSize pixelSize(cv::Point2d(imageData->Scale->X, imageData->Scale->Y));
        EdgeImage::ImageCentroid centroid(cv::Point2d(imageData->Centroid->X, imageData->Centroid->Y));
        EdgeImage::Pointer edgeImage = EdgeImage::New(edgeDataMat, pixelSize, centroid, (EdgeImage::EdgePosition) imageData->EdgePosition);
        _wafer->AddEdgeImage(edgeImage, StringToCharArray(_reportPath));
      }
   };

    ref class AddImageAsNotchHelper {
      Wafer *_wafer;
      String ^ _reportPath;

    public:
      AddImageAsNotchHelper(Wafer *wafer, String ^ reportPath) : _wafer(wafer), _reportPath(reportPath) {}

      void Perform(BareWaferAlignmentImageData^ imageData) {
        if (imageData->ExpectedShape != WaferEdgeShape::NOTCH) {
          throw gcnew System::Exception("Type mismatch: Cannot use as NOTCH image");
        }

        cv::Mat notchDataMat = CreateMatFromImageData(imageData);
        NotchImage::PixelSize pixelSize(cv::Point2d(imageData->Scale->X, imageData->Scale->Y));
        NotchImage::ImageCentroid centroid(cv::Point2d(imageData->Centroid->X, imageData->Centroid->Y));
        NotchImage::Pointer notchImage = NotchImage::New(notchDataMat, pixelSize, centroid);
        _wafer->AddNotchImage(notchImage, StringToCharArray(_reportPath));
      }
    };
  }


  BareWaferAlignmentResult ^ BareWaferAlignment::PerformBareWaferAlignment(List<BareWaferAlignmentImageData^> ^ images, WaferInfos^ waferData, int edgeDetectionVersion, int notchDetectionVersion, int cannyThreshold, String ^ reportPath, ReportOption reportOption) {
    List<BareWaferAlignmentImageData^> ^ edgeImages = images->FindAll(gcnew Predicate<BareWaferAlignmentImageData^>(&IsEdgeImage));
    List<BareWaferAlignmentImageData^> ^ notchImages = images->FindAll(gcnew Predicate<BareWaferAlignmentImageData^>(&IsNotchImage));

    BareWaferAlignmentResult ^ result = gcnew BareWaferAlignmentResult();
    double shiftConfidence = 0.0;
    double angleConfidence = 0.0;
    const double shiftWeight = 0.7;
    const double angleWeight = 0.3;
    result->Status->Code = StatusCode::UNKNOWN_ERROR;
    result->Rotation = 0;
    result->Shift = gcnew Point2d(0, 0);
    if (edgeImages->Count < BareWaferAlignment::MINIMAL_IMAGES_FOR_BWA) {
      result->Status->Message = "Bare wafer alignment needs at least three wafer edges image";
      result->Status->Code = StatusCode::MISSING_EDGE_IMAGE;
    } else {
      if (WaferType::NOTCH == waferData->Type) {
        if (notchImages->Count == 1) {
          Wafer wafer(ToCppType(waferData));

          edgeImages->ForEach(gcnew Action<BareWaferAlignmentImageData^>(gcnew AddImageAsEdgeHelper(&wafer, reportPath), (&AddImageAsEdgeHelper::Perform)));
          notchImages->ForEach(gcnew Action<BareWaferAlignmentImageData^>(gcnew AddImageAsNotchHelper(&wafer, reportPath), (&AddImageAsNotchHelper::Perform)));

          Wafer::WaferGeometricalParameters geoMeasure;

          std::string cppStringReportPath = "";

          if (reportPath != nullptr)
          {
              cppStringReportPath = CSharpStringToCppString(reportPath);
          }

          Algorithms::Status status = wafer.GetGeometricalParameters(&geoMeasure, edgeDetectionVersion, cannyThreshold, cppStringReportPath, (bwa::ReportOption) reportOption);
          shiftConfidence = status.confidence;

          if (Algorithms::StatusCode::OK == status.code) {
            double angle;
            status = wafer.GetMisalignmentAngle(angle, geoMeasure.centerShift, notchDetectionVersion, cppStringReportPath, (bwa::ReportOption) reportOption);
            angleConfidence = status.confidence;
            if (angleConfidence == 0.0) shiftConfidence = 0.0;

            if (Algorithms::StatusCode::OK == status.code) {         
              result->Status->Code = StatusCode::OK;
              result->Rotation = angle;
              double shiftX = geoMeasure.centerShift.x;
              double shiftY = geoMeasure.centerShift.y;
              result->Shift->X = shiftX;
              result->Shift->Y = shiftY;
            } else {
              result->Status->Code = ExecutionStatus::CodeFrom(static_cast<int>(status.code));
              result->Status->Message = gcnew String(status.message.c_str());
            }
          } else {
            result->Status->Code = ExecutionStatus::CodeFrom(static_cast<int>(status.code));
            result->Status->Message = gcnew String(status.message.c_str());
          }
        } else {
          result->Status->Message = "Bare 'notch' wafer alignment needs a notch image";
          result->Status->Code = StatusCode::MISSING_NOTCH_IMAGE;
        }
      }
    }
    result->Status->Confidence = shiftConfidence * shiftWeight + angleConfidence * angleWeight;
    return result;
  }

  ContourExtractionResult ^ BareWaferAlignment::PerformEdgeImageContourExtraction(BareWaferAlignmentImageData^ imageData, int waferDiameterInMm, int edgeDetectionVersion, int cannyThreshold, String ^ reportPath, ReportOption reportOption) {
    ContourExtractionResult ^ result = gcnew ContourExtractionResult();
    result->Status->Code = StatusCode::UNKNOWN_ERROR;
    if (imageData->ExpectedShape == WaferEdgeShape::NOTCH) {
      result->Status->Code = StatusCode::CANNOT_DETECT_EDGE_ON_NOTCH_IMAGE;
      result->Status->Message = "$Cannot extract edge contour from Notch image";
    } else {
      cv::Mat edgeDataMat = CreateMatFromImageData(imageData);
      EdgeImage::PixelSize pixelSize(cv::Point2d(imageData->Scale->X, imageData->Scale->Y));
      EdgeImage::ImageCentroid centroid(cv::Point2d(imageData->Centroid->X, imageData->Centroid->Y));
      EdgeImage::Pointer edgeImage = EdgeImage::New(edgeDataMat, pixelSize, centroid);
      std::string _reportPath = StringToCharArray(reportPath);
      for (auto const &point : edgeImage->GetContourPoints(waferDiameterInMm, edgeDetectionVersion, cannyThreshold, _reportPath, (bwa::ReportOption)reportOption)) {
        result->Contour->Add(gcnew Point2d(point.x, point.y));
      }
      if(result->Contour->Count > 0) result->Status->Code = StatusCode::OK;
    }
    return result;
  }
}