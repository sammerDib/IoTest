#include "CPhaseShiftingInterferometry.hpp"
#include "Utils.hpp"
#include "ReportingUtils.hpp"
#include "C2DSignalAnalysis.hpp"
#include "GoldsteinUnwrap.hpp"
#include "PhaseMapping.hpp"
#include "QualGuidedUnwrap.hpp"
#include "ReliabilityHistUnwrap.hpp"
#include "ErrorLogging.hpp"
#include "CImageTypeConvertor.hpp"

using namespace std;
using namespace cv;
namespace fs = std::filesystem;

#pragma unmanaged
namespace psi {
    namespace {
        void WriteReport(PSIResultData psiResult, phase_unwrapping::GoldsteinUnwrappingResult unwrapResult, fs::path directoryPathToStoreReport);
    }

    PSIResultData TopoReconstructionFromPhase(vector<Mat> imgs, double wavelengthNm, int stepNb, UnwrapMode mode, bool removeResidualFringes, fs::path directoryPathToStoreReport) {
        PSIResultData result = PSIResultData();

        // compute phase map
        WrappedPhaseMap phaseMap = phase_mapping::PhaseMapping(imgs, stepNb, removeResidualFringes);
        result.WrappedPhaseMap = phaseMap.Phase;
        Mat backgroundMap = phaseMap.Background;

        // unwrap phase map
        phase_unwrapping::GoldsteinUnwrappingResult unwrapResult;
        Mat phase;
        Mat mask;
        switch (mode) {
        case UnwrapMode::Goldstein:
            unwrapResult = phase_unwrapping::GoldsteinUnwrap(result.WrappedPhaseMap);
            phase = unwrapResult.UnwrappedPhase;
            break;
        case UnwrapMode::GradientQuality:
            phase = phase_unwrapping::QualityGuidedUnwrap(result.WrappedPhaseMap, phase_unwrapping::QualityMode::Gradient);
            break;
        case UnwrapMode::VarianceQuality:
            phase = phase_unwrapping::QualityGuidedUnwrap(result.WrappedPhaseMap, phase_unwrapping::QualityMode::Variance);
            break;
        case UnwrapMode::PseudoCorrelationQuality:
            phase = phase_unwrapping::QualityGuidedUnwrap(result.WrappedPhaseMap, phase_unwrapping::QualityMode::PseudoCorrelation);
            break;
        case UnwrapMode::HistogramReliabilityPath:
            phase = phase_unwrapping::ReliabilityHistUnwrap().UnwrapPhaseMap(result.WrappedPhaseMap, mask);
            break;
        default:
            ErrorLogging::LogErrorAndThrow("[Topo reconstruction from phase] Unsupported unwrap mode");
        }
        result.UnwrappedPhaseMap = phase;

        // compute topography
        Mat topo = phase * (wavelengthNm / 1000) / (4 * CV_PI);
        result.RawTopographyMap = topo;

        // remove tilt
        Mat plane = signal_2D::SolvePlaneEquation(topo, Mat::ones(topo.rows, topo.cols, CV_8UC1));
        result.Plane = plane;

        Mat topoWithoutPlane = topo - plane;
        result.TopographyMap = topoWithoutPlane;

        if (directoryPathToStoreReport.string() != "")
        {
            WriteReport(result, unwrapResult, directoryPathToStoreReport);
        }

        return result;
    }

    namespace {
        void WriteReport(PSIResultData psiResult, phase_unwrapping::GoldsteinUnwrappingResult unwrapResult, fs::path directoryPathToStoreReport)
        {
            fs::create_directory(directoryPathToStoreReport);

            if (!unwrapResult.Residues.DataMatrix.empty())
            {
                cv::imwrite(directoryPathToStoreReport.string() + "\\bitFlagsWithBorderAndResidues.png", unwrapResult.Residues.ToRGBImage());
            }
            if (!unwrapResult.BranchCuts.DataMatrix.empty())
            {
                cv::imwrite(directoryPathToStoreReport.string() + "\\bitFlagsWithBranchCuts.png", unwrapResult.BranchCuts.ToRGBImage());
            }

            Reporting::writePngImage(psiResult.WrappedPhaseMap, directoryPathToStoreReport.string() + "\\WrappedPhaseMap.png");
            Reporting::writePngImage(psiResult.UnwrappedPhaseMap, directoryPathToStoreReport.string() + "\\UnwrappedPhaseMap.png");
            Reporting::writePngImage(psiResult.RawTopographyMap, directoryPathToStoreReport.string() + "\\RawTopographyMap.png");
            Reporting::writePngImage(psiResult.TopographyMap, directoryPathToStoreReport.string() + "\\TopographyMap.png");
            Reporting::writePngImage(psiResult.Plane, directoryPathToStoreReport.string() + "\\Plane.png");

            Reporting::writeYamlImage(psiResult.WrappedPhaseMap, directoryPathToStoreReport.string() + "\\WrappedPhaseMap.yml", "WrappedPhaseMap");
            Reporting::writeYamlImage(psiResult.UnwrappedPhaseMap, directoryPathToStoreReport.string() + "\\UnwrappedPhaseMap.yml", "UnwrappedPhaseMap");
            Reporting::writeYamlImage(psiResult.RawTopographyMap, directoryPathToStoreReport.string() + "\\RawTopographyMap.yml", "RawTopographyMap");
            Reporting::writeYamlImage(psiResult.TopographyMap, directoryPathToStoreReport.string() + "\\TopographyMap.yml", "TopographyMap");

            Reporting::writeRawImage(psiResult.WrappedPhaseMap, directoryPathToStoreReport.string() + "\\WrappedPhaseMap.bin");
            Reporting::writeRawImage(psiResult.UnwrappedPhaseMap, directoryPathToStoreReport.string() + "\\UnwrappedPhaseMap.bin");
            Reporting::writeRawImage(psiResult.RawTopographyMap, directoryPathToStoreReport.string() + "\\RawTopographyMap.bin");
            Reporting::writeRawImage(psiResult.TopographyMap, directoryPathToStoreReport.string() + "\\TopographyMap.bin");
        }
    }
} // namespace psi