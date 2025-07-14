#include <Logger.hpp>

#include <ReportingUtils.hpp>
#include <BaseAlgos/2DSignalAnalysis.hpp>
#include <Topography/GoldsteinUnwrap.hpp>
#include <Topography/PhaseMapping.hpp>
#include <Topography/PhaseShiftingInterferometry.hpp>
#include <Topography/QualGuidedUnwrap.hpp>
#include <Topography/ReliabilityHistUnwrap.hpp>

using namespace std;
using namespace cv;
namespace fs = std::filesystem;

namespace psi {

    namespace{
        void WriteReport(PSIResultData psiResult, phase_unwrapping::GoldsteinUnwrappingResult unwrapResult, fs::path directoryPathToStoreReport);
    }

    PSIResultData TopoReconstructionFromPhase(vector<Mat> imgs, double wavelengthNm, int stepNb, UnwrapMode mode, fs::path directoryPathToStoreReport) {
        PSIResultData result = PSIResultData();

        // compute phase map
        vector<Mat> averagedImgs = phase_mapping::AverageImgs(imgs, stepNb);
        Mat phaseMap;
        Mat intensityMap;
        Mat backgroundMap;
        Mat wrappedPhaseMap = phase_mapping::PhaseMapping(averagedImgs, phaseMap, intensityMap, backgroundMap);
        result.WrappedPhaseMap = wrappedPhaseMap;

        // unwrap phase map
        phase_unwrapping::GoldsteinUnwrappingResult unwrapResult;
        Mat phase;
        Mat mask;
        switch (mode) {
        case UnwrapMode::Goldstein:
            unwrapResult = phase_unwrapping::GoldsteinUnwrap(wrappedPhaseMap);
            phase = unwrapResult.UnwrappedPhase;
            break;
        case UnwrapMode::GradientQuality:
            phase = phase_unwrapping::QualityGuidedUnwrap(wrappedPhaseMap, phase_unwrapping::QualityMode::Gradient);
            break;
        case UnwrapMode::VarianceQuality:
            phase = phase_unwrapping::QualityGuidedUnwrap(wrappedPhaseMap, phase_unwrapping::QualityMode::Variance);
            break;
        case UnwrapMode::PseudoCorrelationQuality:
            phase = phase_unwrapping::QualityGuidedUnwrap(wrappedPhaseMap, phase_unwrapping::QualityMode::PseudoCorrelation);
            break;
        case UnwrapMode::HistogramReliabilityPath:
            phase = phase_unwrapping::ReliabilityHistUnwrap().UnwrapPhaseMap(wrappedPhaseMap, mask);
            break;
        default:
            stringstream strStrm;
            strStrm << "[Topo reconstruction from phase] Insupported unwrap mode.";
            string message = strStrm.str();
            Logger::Error(message);
            throw exception(message.c_str());
        }
        result.UnwrappedPhaseMap = phase;

        // compute topography
        Mat topo = phase * (wavelengthNm / 1000) / (4 * CV_PI);
        result.RawTopographyMap = topo;

        // remove tilt
        Mat plane = signal_2D::SolvePlaneEquation(topo);
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