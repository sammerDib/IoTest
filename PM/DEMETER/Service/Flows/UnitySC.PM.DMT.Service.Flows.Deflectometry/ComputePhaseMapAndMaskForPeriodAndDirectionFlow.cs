using System;
using System.IO;

using Matrox.MatroxImagingLibrary;

using UnitySC.PM.DMT.Service.Flows.Shared;
using UnitySC.PM.DMT.Service.Interface.Flow;
using UnitySC.PM.Shared.Flow.Implementation;
using UnitySC.Shared.LibMIL;
using UnitySC.Shared.Tools.Units;

using UnitySCSharedAlgosOpenCVWrapper;

namespace UnitySC.PM.DMT.Service.Flows.Deflectometry
{
    public class ComputePhaseMapAndMaskForPeriodAndDirectionFlow : FlowComponent<
        ComputePhaseMapAndMaskForPeriodAndDirectionInput, ComputePhaseMapAndMaskForPeriodAndDirectionResult,
        ComputePhaseMapAndMaskForPeriodAndDirectionConfiguration>
    {

        public const int MaximumNumberOfSteps = 7;

        public ComputePhaseMapAndMaskForPeriodAndDirectionFlow(ComputePhaseMapAndMaskForPeriodAndDirectionInput input) :
            base(input, "ComputePhaseMapAndMaskForPeriodAndDirectionFlow")
        {
        }

        protected override void Process()
        {
            string directionString = Enum.GetName(typeof(FringesDisplacement), Input.FringesDisplacementDirection);
            Result.FringesDisplacementDirection = Input.FringesDisplacementDirection;
            Result.Period = Input.Period;
            Result.Fringe = Input.Fringe;
            SetProgressMessage(
                     $"Starting {Input.Side}side {Name} for period {Input.Period}px and direction {directionString}");
            SetProgressMessage(
                     $"Computing phase map for period {Input.Period}px and direction {directionString}");
            CheckCancellation();
            Result.PsdResult = DeflectometryCalculations.ComputePhaseMap(Input.PhaseImages,
                                                                             Input.Fringe.NbImagesPerDirection,
                                                                             Input.FringesDisplacementDirection);
            SetProgressMessage(
                     $"Successfully computed phase map for period {Input.Period}px and direction {directionString}");
            SetProgressMessage(
                     $"Computing mask for period {Input.Period}px and direction {directionString}");
            CheckCancellation();

            //The enhance mask option on the opencv side is now useless since it is also done on the c# side
            //(in the EnhanceMaskIfNeeded() method
            //Will need to make another opencv nuget version to get rid of the unused maskParams parameter
            bool useEnhanceMaskOpenCVSide = false;
            var maskParams = new MaskParams(useEnhanceMaskOpenCVSide, 
                                            Input.MaskFillExclusionInMicrons, 
                                            Input.PerspectiveCalibration.PixelSize, 
                                            Input.WaferDiameter.Micrometers,
                                            Input.CorrectorResult is null ? 0 : Input.CorrectorResult.WaferXShift.Micrometers,
                                            Input.CorrectorResult is null ? 0 : Input.CorrectorResult.WaferYShift.Micrometers);

            Result.PsdResult.Mask = PhaseShiftingDeflectometry.ComputeMask(Result.PsdResult, maskParams);

            EnhanceMaskIfNeeded();

            SetProgressMessage(
                     $"Successfully computed mask for period {Input.Period}px and direction {directionString}");
            SetProgressMessage(
                     $"{Name} for period {Input.Period}px and direction {directionString} successful");

            if (Configuration.IsAnyReportEnabled() && !(Result.PsdResult is null))
            {
                CheckCancellation();
                using (var reportPhaseMapImage =
                       Result.PsdResult.WrappedPhaseMap.ConvertToUSPImageMil(false))
                {
                    reportPhaseMapImage.AddRef();
                    reportPhaseMapImage.Save(Path.Combine(ReportFolder,
                                                          $"PhaseMap_{Input.Period}px_{directionString}.tif"));
                }

                CheckCancellation();
                using (var reportMaskImage =
                       Result.PsdResult.Mask.ConvertToUSPImageMil())
                {
                    reportMaskImage.AddRef();
                    reportMaskImage.Save(Path.Combine(ReportFolder, $"Mask_{Input.Period}px_{directionString}.tif"));
                }
            }
        }

        private Length GetEdgeFilterFromConfigIfItExists()
        {
            if (Configuration.EdgeFilterByDiameterAndSide.TryGetValue(Input.Side, out var diameterWithEdgeLength))
            {
                if (diameterWithEdgeLength.TryGetValue(Input.WaferDiameter, out var edgeLength))
                {
                    return edgeLength;
                }
            }
            return 0.Millimeters();
        }

        private void EnhanceMaskIfNeeded()
        {
            if (!(Input.PerspectiveCalibration is null) && Input.UseEnhancedMask)
            {
                using (var maskMil = Result.PsdResult.Mask.ConvertToUSPImageMil())
                {
                    using (var milGC = new MilGraphicsContext())
                    {
                        var maskMilImage = maskMil.GetMilImage();
                        milGC.Alloc(Mil.Instance.HostSystem);
                        milGC.Image = maskMilImage;
                        milGC.Color = 255;
                        MIL.MgraControl(milGC, MIL.M_GRAPHIC_SOURCE_CALIBRATION, Input.PerspectiveCalibration.MilCalib);
                        MIL.MgraControl(milGC, MIL.M_INPUT_UNITS, MIL.M_WORLD);
                        MIL.MgraControl(milGC, MIL.M_GRAPHIC_CONVERSION_MODE, MIL.M_RESHAPE_FOLLOWING_DISTORTION);
                        var edgeFilterLength = GetEdgeFilterFromConfigIfItExists();
                        double radius = (Input.WaferDiameter.Micrometers / 2) - edgeFilterLength?.Micrometers ?? 0;
                        double xCenter = Input.CorrectorResult is null ? 0 : Input.CorrectorResult.WaferXShift.Micrometers;
                        double yCenter = Input.CorrectorResult is null ? 0 : Input.CorrectorResult.WaferYShift.Micrometers;
                        milGC.ArcFill(xCenter, yCenter, radius, radius);
                    }
                    Result.PsdResult.Mask.ByteArray = maskMil.ToByteArray();
                }
            }
        }
    }
}
